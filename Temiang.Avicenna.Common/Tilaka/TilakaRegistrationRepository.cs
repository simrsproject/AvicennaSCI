using System;
using System.Collections.Generic;
using Temiang.Avicenna.BusinessObject;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaRegistrationRepository : ITilakaRegistrationRepository
    {
        public TilakaRegistrationRecord FindByRequestId(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId)) return null;

            var query = new TilakaRegistrationQuery();
            query.Where(query.RegistrationRequestID == requestId);
            var entity = new TilakaRegistration();
            return entity.Load(query) ? Map(entity) : null;
        }

        public TilakaRegistrationRecord FindLatest(string relatedType, string relatedId)
        {
            var query = new TilakaRegistrationQuery();
            query.Where(query.RelatedType == relatedType, query.RelatedID == relatedId, query.IsLatest == true);
            query.es.Top = 1;
            query.OrderBy(query.TilakaRegistrationID.Descending);

            var entity = new TilakaRegistration();
            return entity.Load(query) ? Map(entity) : null;
        }

        public IList<TilakaRegistrationRecord> FindPending(int max)
        {
            var query = new TilakaRegistrationQuery();
            query.Where(query.IsLatest == true);
            query.Where(query.Or(
                query.CertificateStatus.IsNull(),
                query.CertificateStatus < TilakaConstant.CertificateStatus.Active,
                query.CertificateExpiryDate <= DateTime.Now.AddDays(30),
                query.CertificateDataStatus == TilakaConstant.CertificateDataStatus.Revoke));
            query.es.Top = max;
            query.OrderBy(query.LastCheckedDateTime.Ascending, query.TilakaRegistrationID.Ascending);

            var collection = new TilakaRegistrationCollection();
            collection.Load(query);

            var result = new List<TilakaRegistrationRecord>();
            foreach (var entity in collection) result.Add(Map(entity));
            return result;
        }

        public void Save(TilakaRegistrationRecord record)
        {
            if (record == null) return;

            var entity = new TilakaRegistration();
            if (record.TilakaRegistrationID > 0)
            {
                if (!entity.LoadByPrimaryKey(record.TilakaRegistrationID))
                    throw new InvalidOperationException(
                        "Data pendaftaran Tilaka dengan ID " + record.TilakaRegistrationID + " tidak ditemukan.");

                Map(record, entity);
            }
            else
            {
                entity.AddNew();
                Map(record, entity);
                entity.CreatedDateTime = DateTime.Now;
                entity.CreatedByUserID = record.ConsentApprovedByUserID;
            }

            entity.LastUpdateDateTime = DateTime.Now;
            var updater = !string.IsNullOrWhiteSpace(record.LastUpdateByUserID)
                ? record.LastUpdateByUserID
                : record.ConsentApprovedByUserID;
            if (!string.IsNullOrWhiteSpace(updater))
                entity.LastUpdateByUserID = updater;

            using (var scope = new esTransactionScope())
            {
                if (record.TilakaRegistrationID <= 0)
                    DemotePreviousLatest(record);

                entity.Save();
                scope.Complete();
            }

            record.TilakaRegistrationID = entity.TilakaRegistrationID ?? 0;
        }

        public void UpdateQuickSignUser(string relatedType, string relatedId, bool isRegistered,
            DateTime? registeredDateTime, string errorMessage, string userID)
        {
            var record = FindLatest(relatedType, relatedId);
            if (record == null) return;

            record.IsQuickSignUser = isRegistered;
            record.QuickSignRegisteredDateTime = registeredDateTime;
            record.QuickSignLastErrorMessage = errorMessage;
            record.LastUpdateByUserID = userID;
            Save(record);
        }

        private static void DemotePreviousLatest(TilakaRegistrationRecord record)
        {
            var query = new TilakaRegistrationQuery();
            query.Where(query.RelatedType == record.RelatedType,
                query.RelatedID == record.RelatedID,
                query.IsLatest == true);

            var previous = new TilakaRegistrationCollection();
            previous.Load(query);
            if (previous.Count == 0) return;

            foreach (var row in previous) row.IsLatest = false;
            previous.Save();
        }

        private static TilakaRegistrationRecord Map(TilakaRegistration entity)
        {
            return new TilakaRegistrationRecord
            {
                TilakaRegistrationID = entity.TilakaRegistrationID ?? 0,
                RelatedType = entity.RelatedType,
                RelatedID = entity.RelatedID,
                IsLatest = entity.IsLatest ?? false,
                RegistrationState = entity.RegistrationState,
                RegistrationRequestID = entity.RegistrationRequestID,
                RequestType = entity.RequestType,
                RegistrationDateTime = entity.RegistrationDateTime,
                DateExpire = entity.DateExpire,
                RegisteredName = entity.RegisteredName,
                RegisteredNIK = entity.RegisteredNIK,
                RegisteredEmail = entity.RegisteredEmail,
                KycStatus = entity.KycStatus,
                KycReasonCode = entity.KycReasonCode,
                ManualRegistrationStatus = entity.ManualRegistrationStatus,
                TilakaName = entity.TilakaName,
                CertificateStatus = entity.CertificateStatus,
                CertificateStatusInfo = entity.CertificateStatusInfo,
                CertificateDataStatus = entity.CertificateDataStatus,
                CertificateSerialNumber = entity.CertificateSerialNumber,
                CertificateStartDate = entity.CertificateStartDate,
                CertificateExpiryDate = entity.CertificateExpiryDate,
                RevokeRequestID = entity.RevokeRequestID,
                RevokeReason = entity.RevokeReason,
                RevokeNote = entity.RevokeNote,
                RevokeStatus = entity.RevokeStatus,
                RevokeMessage = entity.RevokeMessage,
                RevokeRequestedDateTime = entity.RevokeRequestedDateTime,
                RevokeRequestedByUserID = entity.RevokeRequestedByUserID,
                RevokeCompletedDateTime = entity.RevokeCompletedDateTime,
                ConsentVersion = entity.ConsentVersion,
                ConsentTimestamp = entity.ConsentTimestamp,
                ConsentApprovedByUserID = entity.ConsentApprovedByUserID,
                KtpFilePath = entity.KtpFilePath,
                KtpFileName = entity.KtpFileName,
                KtpContentType = entity.KtpContentType,
                KtpUploadedDateTime = entity.KtpUploadedDateTime,
                LastCheckedDateTime = entity.LastCheckedDateTime,
                LastCheckSource = entity.LastCheckSource,
                LastErrorMessage = entity.LastErrorMessage,
                IsQuickSignUser = entity.IsQuickSignUser ?? false,
                QuickSignRegisteredDateTime = entity.QuickSignRegisteredDateTime,
                QuickSignLastErrorMessage = entity.QuickSignLastErrorMessage
            };
        }

        private static void Map(TilakaRegistrationRecord record, TilakaRegistration entity)
        {
            entity.RelatedType = record.RelatedType;
            entity.RelatedID = record.RelatedID;
            entity.IsLatest = record.IsLatest;
            entity.RegistrationState = record.RegistrationState;
            entity.RegistrationRequestID = record.RegistrationRequestID;
            entity.RequestType = record.RequestType;
            entity.RegistrationDateTime = record.RegistrationDateTime;
            entity.DateExpire = record.DateExpire;
            entity.RegisteredName = record.RegisteredName;
            entity.RegisteredNIK = record.RegisteredNIK;
            entity.RegisteredEmail = record.RegisteredEmail;
            entity.KycStatus = record.KycStatus;
            entity.KycReasonCode = record.KycReasonCode;
            entity.ManualRegistrationStatus = record.ManualRegistrationStatus;
            entity.TilakaName = record.TilakaName;
            entity.CertificateStatus = record.CertificateStatus;
            entity.CertificateStatusInfo = record.CertificateStatusInfo;
            entity.CertificateDataStatus = record.CertificateDataStatus;
            entity.CertificateSerialNumber = record.CertificateSerialNumber;
            entity.CertificateStartDate = record.CertificateStartDate;
            entity.CertificateExpiryDate = record.CertificateExpiryDate;
            entity.RevokeRequestID = record.RevokeRequestID;
            entity.RevokeReason = record.RevokeReason;
            entity.RevokeNote = record.RevokeNote;
            entity.RevokeStatus = record.RevokeStatus;
            entity.RevokeMessage = record.RevokeMessage;
            entity.RevokeRequestedDateTime = record.RevokeRequestedDateTime;
            entity.RevokeRequestedByUserID = record.RevokeRequestedByUserID;
            entity.RevokeCompletedDateTime = record.RevokeCompletedDateTime;
            entity.ConsentVersion = record.ConsentVersion;
            entity.ConsentTimestamp = record.ConsentTimestamp;
            entity.ConsentApprovedByUserID = record.ConsentApprovedByUserID;
            entity.KtpFilePath = record.KtpFilePath;
            entity.KtpFileName = record.KtpFileName;
            entity.KtpContentType = record.KtpContentType;
            entity.KtpUploadedDateTime = record.KtpUploadedDateTime;
            entity.LastCheckedDateTime = record.LastCheckedDateTime;
            entity.LastCheckSource = record.LastCheckSource;
            entity.LastErrorMessage = record.LastErrorMessage;
            entity.IsQuickSignUser = record.IsQuickSignUser;
            entity.QuickSignRegisteredDateTime = record.QuickSignRegisteredDateTime;
            entity.QuickSignLastErrorMessage = record.QuickSignLastErrorMessage;
        }
    }
}
