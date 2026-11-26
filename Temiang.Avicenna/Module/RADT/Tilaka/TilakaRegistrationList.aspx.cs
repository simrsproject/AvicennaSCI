using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaRegistrationList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaRegistration;
            UrlPageSearch = "TilakaRegistrationList.aspx";
            UrlPageDetail = "TilakaRegistrationDetail.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuQuickSearch.Visible = false;
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuAdd.Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = GetList();
        }

        protected void grdList_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "detail") return;
            RedirectToDetail(Convert.ToString(e.CommandArgument));
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            if (dataItems == null || dataItems.Length == 0) return;

            RedirectToDetail(Convert.ToString(dataItems[0].GetDataKeyValue("ParamedicID")));
        }

        private void RedirectToDetail(string paramedicId)
        {
            if (string.IsNullOrWhiteSpace(paramedicId)) return;

            Response.Redirect("TilakaRegistrationDetail.aspx?md=view&id=" +
                              Server.UrlEncode(paramedicId));
        }

        protected void btnReconcile_OnClick(object sender, EventArgs e)
        {
            var service = new Service();
            var repository = new TilakaRegistrationRepository();
            var checkedCount = 0;
            var failed = 0;

            foreach (var record in repository.FindPending(5))
            {
                checkedCount++;
                try
                {
                    var certificateRejected = false;

                    if (!string.IsNullOrWhiteSpace(record.RegistrationRequestID))
                    {
                        var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                        string message;
                        if (!service.IsOk(registration, out message))
                        {
                            // Tetap stempel waktu agar record gagal tidak terus berada di urutan teratas.
                            record.LastErrorMessage = message;
                            SaveProgress(repository, record);
                            failed++;
                            continue;
                        }
                        record.ApplyUserRegistrationStatus(registration.Data);
                    }

                    if (!string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var certificate = service.CheckCertificateStatus(record.TilakaName);
                        string message;
                        if (!service.IsOk(certificate, out message))
                        {
                            // Simpan hasil registrasi yang sudah didapat sebelum keluar.
                            record.LastErrorMessage = message;
                            SaveProgress(repository, record);
                            failed++;
                            continue;
                        }

                        if (TilakaCertificateStatusRules.CanApply(record.CertificateStatus, certificate.Status))
                            record.ApplyCertificateStatus(certificate);
                        else
                            certificateRejected = true;
                    }

                    record.LastErrorMessage = certificateRejected
                        ? "Status sertifikat dari Tilaka diabaikan karena tidak lebih maju."
                        : null;
                    SaveProgress(repository, record);
                    if (certificateRejected) failed++;
                }
                catch (Exception ex)
                {
                    record.LastErrorMessage = ex.Message;
                    try
                    {
                        SaveProgress(repository, record);
                    }
                    catch
                    {
                        // Kegagalan menyimpan tidak boleh menghentikan proses batch.
                    }
                    failed++;
                }
            }

            var summary = string.Format(
                "{0} diperiksa (maksimal 5 per klik), {1} gagal. Grid sudah diperbarui.",
                checkedCount, failed);
            lblMessage.Text = failed > 0
                ? TilakaAlertHtml.Warning(summary)
                : TilakaAlertHtml.Info(summary);
            grdList.Rebind();
        }

        private static void SaveProgress(TilakaRegistrationRepository repository, TilakaRegistrationRecord record)
        {
            record.LastErrorMessage = Truncate(record.LastErrorMessage, 1000);
            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Manual;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            repository.Save(record);
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return value.Substring(0, maxLength);
        }

        private DataTable GetList()
        {
            var query = new ParamedicQuery("a");
            var tilaka = new TilakaRegistrationQuery("b");

            query.LeftJoin(tilaka)
                .On(query.ParamedicID == tilaka.RelatedID & tilaka.RelatedType == TilakaConstant.RelatedType.Paramedic & tilaka.IsLatest == true);

            query.Select(
                query.ParamedicID,
                query.ParamedicName,
                query.NamaKTP,
                query.Ssn,
                query.Email,
                tilaka.TilakaRegistrationID,
                tilaka.RegistrationRequestID,
                tilaka.RegistrationState,
                tilaka.RegistrationDateTime,
                tilaka.KycStatus,
                tilaka.ManualRegistrationStatus,
                tilaka.CertificateStatus,
                tilaka.CertificateExpiryDate,
                tilaka.CertificateDataStatus,
                tilaka.TilakaName,
                tilaka.DateExpire,
                tilaka.LastCheckedDateTime);

            query.Where(query.IsActive == true);
            query.Where(query.Or(
                query.IsUsingESignature == true,
                tilaka.TilakaRegistrationID.IsNotNull()));
            var doctorKeyword = txtParamedicName.Text.Trim();
            if (!string.IsNullOrWhiteSpace(doctorKeyword))
            {
                var keyword = "%" + doctorKeyword + "%";
                query.Where(query.Or(
                    query.ParamedicID.Like(keyword),
                    query.ParamedicName.Like(keyword)));
            }

            query.OrderBy(
                query.ParamedicName.Ascending,
                query.ParamedicID.Ascending);

            var table = query.LoadDataTable();
            table.Columns.Add("StatusText", typeof(string));
            foreach (DataRow row in table.Rows)
            {
                if (row["TilakaRegistrationID"] == DBNull.Value)
                {
                    row["StatusText"] = TilakaRegistrationState.BelumTerdaftar;
                    continue;
                }

                var record = new TilakaRegistrationRecord
                {
                    RegistrationState = Convert.ToString(row["RegistrationState"]),
                    RegistrationRequestID = Convert.ToString(row["RegistrationRequestID"]),
                    RegistrationDateTime = row["RegistrationDateTime"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(row["RegistrationDateTime"]),
                    KycStatus = Convert.ToString(row["KycStatus"]),
                    ManualRegistrationStatus = Convert.ToString(row["ManualRegistrationStatus"]),
                    CertificateStatus = row["CertificateStatus"] == DBNull.Value
                        ? (int?)null
                        : Convert.ToInt32(row["CertificateStatus"]),
                    CertificateExpiryDate = row["CertificateExpiryDate"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(row["CertificateExpiryDate"]),
                    CertificateDataStatus = Convert.ToString(row["CertificateDataStatus"]),
                    DateExpire = row["DateExpire"] == DBNull.Value
                        ? (DateTime?)null
                        : Convert.ToDateTime(row["DateExpire"])
                };

                row["StatusText"] = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            }

            if (!string.IsNullOrWhiteSpace(cboState.SelectedValue))
            {
                var filtered = table.Clone();
                foreach (DataRow row in table.Rows)
                    if (Convert.ToString(row["StatusText"]) == cboState.SelectedValue)
                        filtered.ImportRow(row);
                table = filtered;
            }

            return table;
        }
    }
}
