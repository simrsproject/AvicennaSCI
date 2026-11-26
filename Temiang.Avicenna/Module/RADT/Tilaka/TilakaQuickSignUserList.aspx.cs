using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaQuickSignUserList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaQuickSignUser;
            UrlPageSearch = "TilakaQuickSignUserList.aspx";
            UrlPageDetail = "TilakaQuickSignUserList.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuView.Visible = false;
            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuExport.Visible = false;
            ToolBarMenuImport.Visible = false;
            ToolBarMenuHome.Visible = false;
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuQuickSearch.Visible = false;
            ToolBarMenuList.Visible = false;
            ToolBarMenuRejournal.Visible = false;
            ToolBarMenuRebalance.Visible = false;
            ToolBarMenuBPJSSearch.Visible = false;
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
            if (e.CommandName != "register") return;

            var paramedicID = Convert.ToString(e.CommandArgument);
            var config = new TilakaConfig();
            if (!config.IsQuickSignConfigured)
            {
                lblMessage.Text = TilakaAlertHtml.Error("Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).");
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(paramedicID) || string.IsNullOrWhiteSpace(paramedic.Email))
            {
                lblMessage.Text = TilakaAlertHtml.Error("Email dokter belum diisi.");
                return;
            }

            if (paramedic.IsUsingESignature != true)
            {
                lblMessage.Text = TilakaAlertHtml.Error("Dokter ini tidak menggunakan tanda tangan elektronik.");
                return;
            }

            var registration = new TilakaRegistrationRepository().FindLatest(
                TilakaConstant.RelatedType.Paramedic, paramedicID);
            if (!TilakaRegistrationState.IsCertificateActive(registration, DateTime.Now))
            {
                lblMessage.Text = TilakaAlertHtml.Error(
                    "Sertifikat dokter belum aktif, sudah kedaluwarsa, atau dicabut.");
                return;
            }

            try
            {
                var service = TilakaQuickSignServiceFactory.Create(config);
                var response = service.AddQuickSignUsers(new[] { paramedic.Email });

                var success = false;
                var error = response.TechnicalError;

                if (string.IsNullOrWhiteSpace(error))
                    error = response.Message;

                if (response.Data != null)
                {
                    foreach (var item in response.Data)
                    {
                        if (string.Equals(item.Status, "success", StringComparison.OrdinalIgnoreCase)
                            || (!string.IsNullOrEmpty(item.Error)
                                && item.Error.IndexOf("already registered", StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            success = true;
                        }
                        else if (!string.IsNullOrEmpty(item.Error))
                        {
                            error = item.Error;
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(error))
                    error = "Gagal mendaftarkan Quick Sign user.";

                var registrationRepository = new TilakaRegistrationRepository();
                registrationRepository.UpdateQuickSignUser(
                    TilakaConstant.RelatedType.Paramedic, paramedicID,
                    success, success ? DateTime.Now : (DateTime?)null,
                    success ? null : error, AppSession.UserLogin.UserID);

                lblMessage.Text = success
                    ? TilakaAlertHtml.Success("Dokter berhasil didaftarkan sebagai Quick Sign user.")
                    : TilakaAlertHtml.Error(error);
            }
            catch (Exception ex)
            {
                lblMessage.Text = TilakaAlertHtml.Error(ex.Message);
            }

            grdList.Rebind();
        }

        private DataTable GetList()
        {
            var par = new ParamedicQuery("a");
            var reg = new TilakaRegistrationQuery("b");

            par.LeftJoin(reg).On(par.ParamedicID == reg.RelatedID
                                 & reg.RelatedType == TilakaConstant.RelatedType.Paramedic
                                 & reg.IsLatest == true);

            par.Select(
                par.ParamedicID,
                par.ParamedicName,
                par.Email,
                reg.TilakaRegistrationID,
                reg.TilakaName,
                reg.RegistrationState,
                reg.RegistrationRequestID,
                reg.RegistrationDateTime,
                reg.KycStatus,
                reg.ManualRegistrationStatus,
                reg.DateExpire,
                reg.CertificateStatus,
                reg.CertificateExpiryDate,
                reg.CertificateDataStatus,
                reg.IsQuickSignUser,
                reg.QuickSignRegisteredDateTime,
                reg.QuickSignLastErrorMessage);

            par.Where(par.IsActive == true);
            par.Where(par.IsUsingESignature == true);
            if (!string.IsNullOrWhiteSpace(txtParamedicName.Text))
                par.Where(par.ParamedicName.Like("%" + txtParamedicName.Text.Trim() + "%"));

            if (!string.IsNullOrWhiteSpace(txtEmail.Text))
                par.Where(par.Email.Like("%" + txtEmail.Text.Trim() + "%"));

            par.OrderBy(par.ParamedicName.Ascending);

            var table = par.LoadDataTable();
            table.Columns.Add("CertificateStatusText", typeof(string));
            table.Columns.Add("CertificateStatusColor", typeof(string));
            table.Columns.Add("StatusCode", typeof(int));
            table.Columns.Add("StatusValue", typeof(string));
            table.Columns.Add("QuickSignStatus", typeof(string));
            table.Columns.Add("QuickSignStatusColor", typeof(string));
            table.Columns.Add("CanRegister", typeof(bool));

            var now = DateTime.Now;
            foreach (DataRow row in table.Rows)
            {
                var isRegistered = row["IsQuickSignUser"] != DBNull.Value && Convert.ToBoolean(row["IsQuickSignUser"]);
                var record = BuildRecord(row);
                var state = row["TilakaRegistrationID"] == DBNull.Value
                    ? TilakaRegistrationState.BelumTerdaftar
                    : TilakaRegistrationState.Evaluate(record, now);
                var status = TilakaStatusPresentation.RegistrationState(state);

                row["CertificateStatusText"] = status.Text;
                row["CertificateStatusColor"] = status.Color;
                row["StatusCode"] = status.Code;
                row["StatusValue"] = state;
                row["QuickSignStatus"] = isRegistered ? "Terdaftar" : "Belum terdaftar";
                row["QuickSignStatusColor"] = TilakaStatusPresentation.QuickSignStatusColor(isRegistered);
                row["CanRegister"] = !isRegistered && TilakaRegistrationState.IsCertificateActive(record, now);
            }

            int certificateStatusFilter;
            var filterCertificateStatus = int.TryParse(cboCertificateStatus.SelectedValue, out certificateStatusFilter);
            var filterQuickSign = !string.IsNullOrWhiteSpace(cboStatus.SelectedValue);

            if (filterCertificateStatus || filterQuickSign)
            {
                foreach (DataRow row in table.Rows)
                {
                    var isRegistered = row["IsQuickSignUser"] != DBNull.Value
                                       && Convert.ToBoolean(row["IsQuickSignUser"]);
                    var remove = filterCertificateStatus
                                 && Convert.ToInt32(row["StatusCode"]) != certificateStatusFilter;
                    remove = remove
                             || (filterQuickSign && cboStatus.SelectedValue == "1" && !isRegistered)
                             || (filterQuickSign && cboStatus.SelectedValue == "0" && isRegistered);

                    if (remove) row.Delete();
                }
            }

            table.AcceptChanges();
            return table;
        }

        private static TilakaRegistrationRecord BuildRecord(DataRow row)
        {
            return new TilakaRegistrationRecord
            {
                RegistrationState = Convert.ToString(row["RegistrationState"]),
                RegistrationRequestID = Convert.ToString(row["RegistrationRequestID"]),
                RegistrationDateTime = row["RegistrationDateTime"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(row["RegistrationDateTime"]),
                KycStatus = Convert.ToString(row["KycStatus"]),
                ManualRegistrationStatus = Convert.ToString(row["ManualRegistrationStatus"]),
                DateExpire = row["DateExpire"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(row["DateExpire"]),
                CertificateStatus = row["CertificateStatus"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(row["CertificateStatus"]),
                CertificateExpiryDate = row["CertificateExpiryDate"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(row["CertificateExpiryDate"]),
                CertificateDataStatus = Convert.ToString(row["CertificateDataStatus"])
            };
        }
    }
}
