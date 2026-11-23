using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    internal static class AdmedikaDailyMonitoringHelper
    {
        public const string SourceAuto = "AUTO";
        public const string SourceManual = "MANUAL";

        public static AdmedikaDailyMonitoring GetHeader(long claimTransactionID, DateTime date)
        {
            var query = new AdmedikaDailyMonitoringQuery();
            query.Where(
                query.AdmedikaClaimTransactionID == claimTransactionID,
                query.MonitoringDate == date.Date);

            var header = new AdmedikaDailyMonitoring();
            return header.Load(query) ? header : null;
        }

        public static AdmedikaDailyMonitoring EnsureHeader(long claimTransactionID, DateTime date)
        {
            var header = GetHeader(claimTransactionID, date);
            if (header != null) return header;

            header = new AdmedikaDailyMonitoring
            {
                AdmedikaClaimTransactionID = claimTransactionID,
                MonitoringDate = date.Date,
                IsSent = false,
                CreatedDateTime = DateTime.Now,
                CreatedByUserID = AppSession.UserLogin.UserID
            };
            header.Save();

            return header;
        }

        public static List<AdmedikaDailyMonitoringItem> GetItems(long headerID)
        {
            var query = new AdmedikaDailyMonitoringItemQuery("a");
            query.Where(query.AdmedikaDailyMonitoringID == headerID);
            query.OrderBy(query.SeqNo.Ascending, query.AdmedikaDailyMonitoringItemID.Ascending);

            var collection = new AdmedikaDailyMonitoringItemCollection();
            collection.Load(query);

            return collection.ToList();
        }

        public static AdmedikaDailyMonitoringItem AddItem(AdmedikaDailyMonitoring header, int categoryCode,
            string formCode, string remarks, string sourceType)
        {
            var category = AdmedikaDailyMonitoringCategory.Get(categoryCode);
            var items = GetItems(header.AdmedikaDailyMonitoringID ?? 0);

            var item = new AdmedikaDailyMonitoringItem
            {
                AdmedikaDailyMonitoringID = header.AdmedikaDailyMonitoringID,
                SeqNo = items.Count + 1,
                CategoryCode = categoryCode,
                FormCode = string.IsNullOrWhiteSpace(formCode)
                    ? (category == null ? null : category.DefaultFormCode)
                    : formCode,
                Remarks = remarks,
                EntryDateTime = DateTime.Now,
                SourceType = sourceType,
                IsSent = false,
                CreatedDateTime = DateTime.Now,
                CreatedByUserID = AppSession.UserLogin.UserID
            };
            item.Save();

            return item;
        }

        public static void DeleteItem(long itemID)
        {
            var item = new AdmedikaDailyMonitoringItem();
            if (!item.LoadByPrimaryKey(itemID) || item.IsSent == true) return;

            item.MarkAsDeleted();
            item.Save();
        }

        /// <summary>
        /// Menarik entri otomatis dari modul yang sudah ada dan mengganti entri AUTO sebelumnya.
        /// Entri MANUAL tidak diubah.
        /// </summary>
        public static void PullAutoEntries(AdmedikaDailyMonitoring header, AdmedikaClaimTransaction claim,
            Registration registration, string userID)
        {
            if (header == null || claim == null) return;

            var existing = GetItems(header.AdmedikaDailyMonitoringID ?? 0);
            foreach (var item in existing.Where(x => string.Equals(x.SourceType, SourceAuto,
                         StringComparison.OrdinalIgnoreCase)))
            {
                item.MarkAsDeleted();
                item.Save();
            }

            if (registration != null)
            {
                PullRoom(header, registration, userID);
                PullPhysician(header, registration, userID);
                PullDiagnosis(header, registration, userID);
                PullAnamnesis(header, registration, userID);
                PullVitalSigns(header, registration, userID);
                PullPrescriptions(header, registration, userID);
            }
        }

        private static void AddAuto(AdmedikaDailyMonitoring header, int categoryCode, string formCode,
            string remarks, string userID)
        {
            if (string.IsNullOrWhiteSpace(remarks)) return;

            var category = AdmedikaDailyMonitoringCategory.Get(categoryCode);
            var items = GetItems(header.AdmedikaDailyMonitoringID ?? 0);

            var item = new AdmedikaDailyMonitoringItem
            {
                AdmedikaDailyMonitoringID = header.AdmedikaDailyMonitoringID,
                SeqNo = items.Count + 1,
                CategoryCode = categoryCode,
                FormCode = string.IsNullOrWhiteSpace(formCode)
                    ? (category == null ? null : category.DefaultFormCode)
                    : formCode,
                Remarks = remarks.Trim(),
                EntryDateTime = DateTime.Now,
                SourceType = SourceAuto,
                IsSent = false,
                CreatedDateTime = DateTime.Now,
                CreatedByUserID = userID
            };
            item.Save();
        }

        private static void PullRoom(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            var roomName = string.Empty;
            if (!string.IsNullOrWhiteSpace(registration.RoomID))
            {
                var room = new ServiceRoom();
                if (room.LoadByPrimaryKey(registration.RoomID))
                    roomName = room.RoomName;
            }

            var lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(roomName)) lines.Add("Ruang: " + roomName);
            if (!string.IsNullOrWhiteSpace(registration.ClassID)) lines.Add("Kelas: " + registration.ClassID);
            if (!string.IsNullOrWhiteSpace(registration.BedID)) lines.Add("Bed: " + registration.BedID);

            AddAuto(header, 1, AdmedikaConstant.DailyMonitoringFormID.DCM003C,
                string.Join(Environment.NewLine, lines), userID);
        }

        private static void PullPhysician(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            if (string.IsNullOrWhiteSpace(registration.ParamedicID)) return;

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(registration.ParamedicID)) return;

            AddAuto(header, 5, AdmedikaConstant.DailyMonitoringFormID.DCM007A,
                "DPJP: " + paramedic.ParamedicName, userID);
        }

        private static void PullDiagnosis(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            if (string.IsNullOrWhiteSpace(registration.RegistrationNo)) return;

            var query = new RegistrationInfoMedicDiagnoseQuery("a");
            var diagnose = new DiagnoseQuery("b");

            query.LeftJoin(diagnose).On(query.DiagnoseID == diagnose.DiagnoseID);
            query.Select(
                query.DiagnosisText,
                query.SRDiagnoseType,
                diagnose.DiagnoseName);
            query.Where(
                query.RegistrationNo == registration.RegistrationNo,
                query.IsVoid == false);
            query.OrderBy(query.DiagnoseDateTime.Descending);
            query.es.Top = 5;

            var table = query.LoadDataTable();
            var lines = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                var name = Convert.ToString(row["DiagnosisText"]);
                if (string.IsNullOrWhiteSpace(name))
                    name = Convert.ToString(row["DiagnoseName"]);
                if (string.IsNullOrWhiteSpace(name)) continue;

                var type = Convert.ToString(row["SRDiagnoseType"]);
                lines.Add(string.IsNullOrWhiteSpace(type) ? name : type + ": " + name);
            }

            AddAuto(header, 7, AdmedikaConstant.DailyMonitoringFormID.DCM012A,
                string.Join(Environment.NewLine, lines), userID);
        }

        private static void PullAnamnesis(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            var lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(registration.Anamnesis))
                lines.Add("Anamnesa: " + registration.Anamnesis.Trim());
            if (!string.IsNullOrWhiteSpace(registration.Complaint))
                lines.Add("Keluhan: " + registration.Complaint.Trim());

            AddAuto(header, 8, AdmedikaConstant.DailyMonitoringFormID.DCM010A,
                string.Join(Environment.NewLine, lines), userID);
        }

        private static void PullVitalSigns(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            if (string.IsNullOrWhiteSpace(registration.RegistrationNo)) return;

            var query = new RegistrationInfoMedicVitalSignQuery("a");
            var vitalSign = new VitalSignQuery("b");

            query.InnerJoin(vitalSign).On(query.VitalSignID == vitalSign.VitalSignID);
            query.Select(
                query.VitalSignValueText,
                query.VitalSignValueNum,
                query.VitalSignUnit,
                vitalSign.VitalSignName);
            query.Where(query.RegistrationNo == registration.RegistrationNo);
            query.OrderBy(query.LastUpdateDateTime.Descending);
            query.es.Top = 20;

            var table = query.LoadDataTable();
            var lines = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                var name = Convert.ToString(row["VitalSignName"]);
                if (string.IsNullOrWhiteSpace(name)) continue;

                var value = Convert.ToString(row["VitalSignValueText"]);
                if (string.IsNullOrWhiteSpace(value) && row["VitalSignValueNum"] != DBNull.Value)
                    value = Convert.ToDecimal(row["VitalSignValueNum"]).ToString("0.##", CultureInfo.InvariantCulture);

                var unit = Convert.ToString(row["VitalSignUnit"]);
                var line = name + ": " + value + (string.IsNullOrWhiteSpace(unit) ? string.Empty : " " + unit);

                if (!lines.Contains(line)) lines.Add(line);
            }

            AddAuto(header, 9, AdmedikaConstant.DailyMonitoringFormID.DCM011A,
                string.Join(Environment.NewLine, lines), userID);
        }

        private static void PullPrescriptions(AdmedikaDailyMonitoring header, Registration registration, string userID)
        {
            if (string.IsNullOrWhiteSpace(registration.RegistrationNo)) return;

            var query = new TransPrescriptionQuery("a");
            var item = new TransPrescriptionItemQuery("b");
            var master = new ItemQuery("c");

            query.InnerJoin(item).On(query.PrescriptionNo == item.PrescriptionNo);
            item.InnerJoin(master).On(item.ItemID == master.ItemID);
            query.Select(
                master.ItemName,
                item.ItemQtyInString,
                item.ConsumeMethod);
            query.Where(
                query.RegistrationNo == registration.RegistrationNo,
                query.IsVoid == false);
            query.OrderBy(query.PrescriptionDate.Descending);
            query.es.Top = 20;

            var table = query.LoadDataTable();
            var lines = new List<string>();

            foreach (DataRow row in table.Rows)
            {
                var name = Convert.ToString(row["ItemName"]);
                if (string.IsNullOrWhiteSpace(name)) continue;

                var qty = Convert.ToString(row["ItemQtyInString"]);
                var method = Convert.ToString(row["ConsumeMethod"]);
                var line = "- " + name + (string.IsNullOrWhiteSpace(qty) ? string.Empty : " (" + qty + ")")
                           + (string.IsNullOrWhiteSpace(method) ? string.Empty : " " + method);

                if (!lines.Contains(line)) lines.Add(line);
            }

            AddAuto(header, 12, AdmedikaConstant.DailyMonitoringFormID.DCM013A,
                string.Join(Environment.NewLine, lines), userID);
        }

        public static string Send(AdmedikaDailyMonitoring header, IList<AdmedikaDailyMonitoringItem> items,
            out bool success)
        {
            success = false;

            if (header == null) return "Data monitoring tidak ditemukan.";

            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(header.AdmedikaClaimTransactionID ?? 0))
                return "Data klaim tidak ditemukan.";

            if (string.IsNullOrWhiteSpace(claim.CardNo) || string.IsNullOrWhiteSpace(claim.ClID))
                return "Card No / ClID klaim tidak lengkap.";

            var registration = new Registration();
            if (!string.IsNullOrWhiteSpace(claim.RegistrationNo))
                registration.LoadByPrimaryKey(claim.RegistrationNo);

            var pending = items
                .Where(x => x.IsSent != true && !string.IsNullOrWhiteSpace(x.Remarks))
                .ToList();

            if (pending.Count == 0)
                return "Tidak ada entri yang siap dikirim.";

            var forms = pending.Select(x =>
            {
                var category = AdmedikaDailyMonitoringCategory.Get(x.CategoryCode ?? 0);
                return new DailyMonitoringForm
                {
                    Code = string.IsNullOrWhiteSpace(x.FormCode)
                        ? (category == null ? null : category.DefaultFormCode)
                        : x.FormCode,
                    Name = category == null ? null : category.Name,
                    Input = x.Remarks,
                    Date = (header.MonitoringDate ?? DateTime.Today)
                        .ToString("ddMMyyyy", CultureInfo.InvariantCulture)
                };
            }).ToArray();

            var providerTransID = claim.TransactionNo;
            if (string.IsNullOrWhiteSpace(providerTransID))
                providerTransID = registration.RegistrationNo;
            if (string.IsNullOrWhiteSpace(providerTransID))
                providerTransID = claim.RegistrationNo;

            var service = new Service();
            var result = service.DailyMonitoring(
                claim.CardNo,
                providerTransID,
                claim.ClID,
                AppSession.UserLogin.UserID,
                forms);

            string message;
            success = service.IsOk(result, out message);

            for (var i = 0; i < pending.Count; i++)
            {
                var item = pending[i];
                var status = result.DmForms.Count > i ? result.DmForms[i].Status : null;

                item.Status = status;
                item.IsSent = !string.IsNullOrWhiteSpace(status) &&
                              status.StartsWith("Success", StringComparison.OrdinalIgnoreCase);
                item.LastUpdateDateTime = DateTime.Now;
                item.LastUpdateByUserID = AppSession.UserLogin.UserID;
                item.Save();
            }

            header.IsSent = true;
            header.SentDateTime = DateTime.Now;
            header.SentByUserID = AppSession.UserLogin.UserID;
            header.IsSuccess = success;
            header.OkCount = ParseCount(result.Ok);
            header.ErCount = ParseCount(result.Er);
            header.ClStatus = result.ClStatus;
            header.ClDesc = result.ClDesc;
            header.Message = result.Msg;
            header.ErrorCode = success ? null : result.ErrorCode;
            header.ErrorDesc = success ? null : message;
            header.RawResponse = result.RawResponse;
            header.LastUpdateDateTime = DateTime.Now;
            header.LastUpdateByUserID = AppSession.UserLogin.UserID;
            header.Save();

            return success ? "Daily monitoring berhasil dikirim." : message;
        }

        private static int? ParseCount(string value)
        {
            int count;
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out count)
                ? (int?)count
                : null;
        }
    }
}
