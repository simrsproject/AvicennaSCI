using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaCasemixFileList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaCasemixFile;
            UrlPageSearch = "TilakaCasemixFileList.aspx";
            UrlPageDetail = "TilakaSigningDetail.aspx";
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
            if (e.CommandName == "status")
            {
                var requestId = Convert.ToString(e.CommandArgument);
                if (!string.IsNullOrWhiteSpace(requestId))
                    Response.Redirect("TilakaSigningDetail.aspx?id=" + Server.UrlEncode(requestId));
                return;
            }

            if (e.CommandName != "process") return;

            long id;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            var result = ProcessSignature(id);
            if (result.IsSuccess && result.Record != null)
            {
                Response.Redirect("TilakaSigningDetail.aspx?id=" +
                                  Server.UrlEncode(result.Record.RequestID));
                return;
            }

            lblMessage.Text = result.Message;
            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        private TilakaQuickSignSubmitResult ProcessSignature(long casemixFileId)
        {
            try
            {
                var config = new TilakaConfig();
                if (!config.IsQuickSignConfigured)
                    return new TilakaQuickSignSubmitResult
                    {
                        IsSuccess = false,
                        Message = "Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong)."
                    };

                var service = TilakaQuickSignServiceFactory.Create(config);
                var requestService = new TilakaQuickSignRequestService(
                    service,
                    config,
                    new TilakaQuickSignRepository(),
                    new TilakaCasemixFileRepository(),
                    new TilakaTemplateSignFileRepository(),
                    new TilakaRegistrationRepository(),
                    new TilakaUserProfileProvider(),
                    new TilakaCasemixFileReader(AppSession.Parameter.ApplicationDocumentFolder));

                return requestService.Submit(casemixFileId, null, null, null, null,
                    AppSession.UserLogin.UserID);
            }
            catch (Exception ex)
            {
                return new TilakaQuickSignSubmitResult { IsSuccess = false, Message = ex.Message };
            }
        }

        private DataTable GetList()
        {
            object cached = Session[SessionNameForList];
            if (cached != null) return (DataTable)cached;

            var query = new TilakaCasemixFileQuery("a");
            var paramedic = new ParamedicQuery("b");
            query.LeftJoin(paramedic).On(query.ParamedicID == paramedic.ParamedicID);
            query.Select(query, paramedic.IsUsingESignature);
            query.Where(query.IsVoid == false);

            if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                query.Where(query.RegistrationNo.Like("%" + txtRegistrationNo.Text.Trim() + "%"));
            if (!string.IsNullOrWhiteSpace(txtParamedicID.Text))
                query.Where(query.ParamedicID == txtParamedicID.Text.Trim());

            query.es.Top = AppSession.Parameter.MaxResultRecord;
            query.OrderBy(query.CasemixFileID.Descending);

            var table = query.LoadDataTable();
            table.Columns.Add("RequestID", typeof(string));
            table.Columns.Add("RequestStatus", typeof(string));
            table.Columns.Add("CanProcess", typeof(bool));

            foreach (DataRow row in table.Rows)
            {
                var isUsingESignature = row["IsUsingESignature"] != DBNull.Value
                                        && Convert.ToBoolean(row["IsUsingESignature"]);

                var requestQuery = new TilakaQuickSignRequestQuery();
                requestQuery.Where(requestQuery.CasemixFileID == Convert.ToInt64(row["CasemixFileID"]));
                requestQuery.es.Top = 1;
                requestQuery.OrderBy(requestQuery.QuickSignRequestID.Descending);

                var request = new TilakaQuickSignRequest();
                if (request.Load(requestQuery))
                {
                    var status = request.Status;
                    row["RequestID"] = request.RequestID;
                    row["RequestStatus"] = status;
                    row["CanProcess"] = isUsingESignature
                                        && status == TilakaQuickSignConstant.RequestStatus.Failed;
                }
                else
                {
                    row["RequestID"] = DBNull.Value;
                    row["RequestStatus"] = "Belum diproses";
                    row["CanProcess"] = isUsingESignature;
                }
            }

            Session[SessionNameForList] = table;
            return table;
        }
    }
}
