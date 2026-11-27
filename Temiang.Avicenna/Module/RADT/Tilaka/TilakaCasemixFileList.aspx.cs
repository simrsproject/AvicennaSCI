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

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuView.Visible = false;
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
            if (e.CommandName == "status")
            {
                var requestId = Convert.ToString(e.CommandArgument);
                if (!string.IsNullOrWhiteSpace(requestId))
                    Response.Redirect("TilakaSigningDetail.aspx?id=" + Server.UrlEncode(requestId));
                return;
            }

            if (e.CommandName == "delete")
            {
                DeleteCasemixFile(Convert.ToString(e.CommandArgument));
                grdList.Rebind();
                return;
            }

            if (e.CommandName != "process") return;

            long id;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            lblMessage.Text = string.Empty;

            var result = ProcessSignature(id);
            if (result.IsSuccess && result.Record != null)
            {
                Response.Redirect("TilakaSigningDetail.aspx?id=" +
                                  Server.UrlEncode(result.Record.RequestID));
                return;
            }

            if (result.IsSuccess)
            {
                lblMessage.Text = TilakaAlertHtml.Success("Request signature berhasil dibuat.");
            }
            else
            {
                lblMessage.Text = TilakaAlertHtml.Error(string.IsNullOrWhiteSpace(result.Message)
                    ? "Gagal memproses tanda tangan."
                    : result.Message);
            }

            grdList.Rebind();
        }

        private void DeleteCasemixFile(string commandArgument)
        {
            long id;
            if (!long.TryParse(commandArgument, out id)) return;

            var repository = new TilakaCasemixFileRepository();
            var record = repository.FindByID(id);
            if (record == null) return;

            if (!TilakaCasemixFileRules.CanDelete(HasQuickSignRequest(id)))
            {
                lblMessage.Text = TilakaAlertHtml.Error("File Casemix yang sudah diproses tidak dapat dihapus.");
                return;
            }

            try
            {
                repository.SoftDelete(id, AppSession.UserLogin.UserID);
                lblMessage.Text = TilakaAlertHtml.Success("File Casemix berhasil dihapus.");
            }
            catch (Exception ex)
            {
                lblMessage.Text = TilakaAlertHtml.Error(ex.Message);
            }
        }

        private static bool HasQuickSignRequest(long casemixFileId)
        {
            var query = new TilakaQuickSignRequestQuery();
            query.Where(query.CasemixFileID == casemixFileId);
            query.es.Top = 1;

            return new TilakaQuickSignRequest().Load(query);
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
            var query = new TilakaCasemixFileQuery("a");
            var paramedic = new ParamedicQuery("b");
            query.LeftJoin(paramedic).On(query.ParamedicID == paramedic.ParamedicID);
            query.Select(query,
                paramedic.ParamedicName,
                paramedic.IsUsingESignature);
            query.Where(query.IsVoid == false, query.IsDeleted == false);

            if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                query.Where(query.RegistrationNo.Like("%" + txtRegistrationNo.Text.Trim() + "%"));

            var doctorKeyword = txtParamedicID.Text.Trim();
            if (!string.IsNullOrWhiteSpace(doctorKeyword))
            {
                var keyword = "%" + doctorKeyword + "%";
                query.Where(query.Or(
                    query.ParamedicID.Like(keyword),
                    paramedic.ParamedicName.Like(keyword)));
            }

            query.es.Top = AppSession.Parameter.MaxResultRecord;
            query.OrderBy(query.CasemixFileID.Descending);

            var table = query.LoadDataTable();
            table.Columns.Add("RequestID", typeof(string));
            table.Columns.Add("RequestStatus", typeof(string));
            table.Columns.Add("CanProcess", typeof(bool));
            table.Columns.Add("CanDelete", typeof(bool));

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
                    row["CanDelete"] = TilakaCasemixFileRules.CanDelete(true);
                }
                else
                {
                    row["RequestID"] = DBNull.Value;
                    row["RequestStatus"] = "Belum diproses";
                    row["CanProcess"] = isUsingESignature;
                    row["CanDelete"] = TilakaCasemixFileRules.CanDelete(false);
                }
            }

            return table;
        }
    }
}
