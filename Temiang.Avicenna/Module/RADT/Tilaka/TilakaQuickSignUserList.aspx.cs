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

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            Session.Remove(SessionNameForList);
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
                lblMessage.Text = "Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).";
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(paramedicID) || string.IsNullOrWhiteSpace(paramedic.Email))
            {
                lblMessage.Text = "Email dokter belum diisi.";
                return;
            }

            if (paramedic.IsUsingESignature != true)
            {
                lblMessage.Text = "Dokter ini tidak menggunakan tanda tangan elektronik.";
                return;
            }

            try
            {
                var service = TilakaQuickSignServiceFactory.Create(config);
                var response = service.AddQuickSignUsers(new[] { paramedic.Email });

                var success = false;
                var error = response.Message;
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

                var registrationRepository = new TilakaRegistrationRepository();
                registrationRepository.UpdateQuickSignUser(
                    TilakaConstant.RelatedType.Paramedic, paramedicID,
                    success, success ? DateTime.Now : (DateTime?)null,
                    success ? null : error, AppSession.UserLogin.UserID);

                lblMessage.Text = success
                    ? "Dokter berhasil didaftarkan sebagai Quick Sign user."
                    : error;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }

            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        private DataTable GetList()
        {
            object cached = Session[SessionNameForList];
            if (cached != null) return (DataTable)cached;

            var par = new ParamedicQuery("a");
            var reg = new TilakaRegistrationQuery("b");

            par.LeftJoin(reg).On(par.ParamedicID == reg.RelatedID
                                 & reg.RelatedType == TilakaConstant.RelatedType.Paramedic
                                 & reg.IsLatest == true);

            par.Select(
                par.ParamedicID,
                par.ParamedicName,
                par.Email,
                reg.TilakaName,
                reg.CertificateStatus,
                reg.IsQuickSignUser,
                reg.QuickSignRegisteredDateTime,
                reg.QuickSignLastErrorMessage);

            par.Where(par.IsActive == true);
            par.Where(par.IsUsingESignature == true);
            if (!string.IsNullOrWhiteSpace(txtParamedicName.Text))
                par.Where(par.ParamedicName.Like("%" + txtParamedicName.Text.Trim() + "%"));

            par.es.Top = AppSession.Parameter.MaxResultRecord;
            par.OrderBy(par.ParamedicName.Ascending);

            var table = par.LoadDataTable();
            table.Columns.Add("QuickSignStatus", typeof(string));
            table.Columns.Add("CanRegister", typeof(bool));

            foreach (DataRow row in table.Rows)
            {
                var isRegistered = row["IsQuickSignUser"] != DBNull.Value && Convert.ToBoolean(row["IsQuickSignUser"]);
                var certificate = row["CertificateStatus"] == DBNull.Value
                    ? 0
                    : Convert.ToInt32(row["CertificateStatus"]);

                row["QuickSignStatus"] = isRegistered ? "Terdaftar" : "Belum terdaftar";
                row["CanRegister"] = !isRegistered && certificate == TilakaConstant.CertificateStatus.Active;

                if (!string.IsNullOrWhiteSpace(cboStatus.SelectedValue) && cboStatus.SelectedValue == "1" && !isRegistered)
                    row.Delete();
                else if (!string.IsNullOrWhiteSpace(cboStatus.SelectedValue) && cboStatus.SelectedValue == "0" && isRegistered)
                    row.Delete();
            }

            table.AcceptChanges();
            Session[SessionNameForList] = table;
            return table;
        }
    }
}
