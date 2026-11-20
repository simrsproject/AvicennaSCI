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
            ToolBarMenuQuickSearch.Visible = true;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
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
            if (e.CommandName != "detail") return;
            var id = Convert.ToString(e.CommandArgument);
            Response.Redirect(string.Format("TilakaRegistrationDetail.aspx?md=view&id={0}", id));
        }

        protected void btnReconcile_OnClick(object sender, EventArgs e)
        {
            var service = new Service();
            var repository = new TilakaRegistrationRepository();
            var checkedCount = 0;
            var failed = 0;

            foreach (var record in repository.FindPending(20))
            {
                checkedCount++;
                try
                {
                    if (string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                        string message;
                        if (!service.IsOk(registration, out message)) { failed++; continue; }
                        if (registration.Data != null)
                        {
                            record.TilakaName = registration.Data.TilakaName;
                            record.KycStatus = registration.Data.Status;
                            record.ManualRegistrationStatus = registration.Data.ManualRegistrationStatus;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var certificate = service.CheckCertificateStatus(record.TilakaName);
                        string message;
                        if (!service.IsOk(certificate, out message)) { failed++; continue; }
                        record.CertificateStatus = certificate.Status;
                    }

                    record.LastCheckedDateTime = DateTime.Now;
                    record.LastCheckSource = TilakaConstant.LogSource.Manual;
                    record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                    repository.Save(record);
                }
                catch
                {
                    failed++;
                }
            }

            lblMessage.Text = string.Format("{0} diperiksa, {1} gagal. Refresh grid untuk melihat hasil.", checkedCount, failed);
            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        private DataTable GetList()
        {
            object cached = Session[SessionNameForList];
            if (cached != null) return (DataTable)cached;

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
                tilaka.KycStatus,
                tilaka.ManualRegistrationStatus,
                tilaka.CertificateStatus,
                tilaka.TilakaName,
                tilaka.DateExpire,
                tilaka.LastCheckedDateTime);

            query.Where(query.IsActive == true);
            if (!string.IsNullOrWhiteSpace(txtParamedicName.Text))
                query.Where(query.ParamedicName.Like("%" + txtParamedicName.Text.Trim() + "%"));

            query.es.Top = AppSession.Parameter.MaxResultRecord;

            var table = query.LoadDataTable();
            table.Columns.Add("StatusText", typeof(string));
            foreach (DataRow row in table.Rows)
            {
                var state = Convert.ToString(row["RegistrationState"]);
                row["StatusText"] = string.IsNullOrWhiteSpace(state)
                    ? TilakaRegistrationState.BelumTerdaftar
                    : state;
            }

            if (!string.IsNullOrWhiteSpace(cboState.SelectedValue))
            {
                var filtered = table.Clone();
                foreach (DataRow row in table.Rows)
                    if (Convert.ToString(row["StatusText"]) == cboState.SelectedValue)
                        filtered.ImportRow(row);
                table = filtered;
            }

            Session[SessionNameForList] = table;
            return table;
        }
    }
}
