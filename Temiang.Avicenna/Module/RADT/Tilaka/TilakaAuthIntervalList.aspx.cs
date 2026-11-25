using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaAuthIntervalList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaAuthInterval;
            UrlPageSearch = "TilakaAuthIntervalList.aspx";
            UrlPageDetail = "TilakaAuthIntervalList.aspx";
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
            LoadFromService();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var table = Session[SessionNameForList] as DataTable;
            if (table == null)
            {
                table = BuildTable(null);
                Session[SessionNameForList] = table;
            }

            grdList.DataSource = table;
        }

        private void LoadFromService()
        {
            var config = new TilakaConfig();
            if (!config.IsQuickSignConfigured)
            {
                lblMessage.Text = TilakaAlertHtml.Error("Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).");
                return;
            }

            try
            {
                var service = TilakaQuickSignServiceFactory.Create(config);
                var response = service.GetAuthSessions(txtEmail.Text.Trim(),
                    cboAuthStatus.SelectedValue, 1, 25);

                string message;
                if (!service.IsOk(response, out message))
                {
                    lblMessage.Text = TilakaAlertHtml.Error(message);
                    return;
                }

                var table = BuildTable(response);
                Session[SessionNameForList] = table;
                grdList.DataSource = table;
                grdList.DataBind();

                lblMessage.Text = table.Rows.Count == 0
                    ? string.Empty
                    : TilakaAlertHtml.Info("Total: " + (response.Pagination == null
                        ? table.Rows.Count
                        : response.Pagination.TotalElements));
            }
            catch (Exception ex)
            {
                lblMessage.Text = TilakaAlertHtml.Error(ex.Message);
            }
        }

        private static DataTable BuildTable(QuickSignAuthSessionResponse response)
        {
            var table = new DataTable();
            table.Columns.Add("Email", typeof(string));
            table.Columns.Add("TilakaName", typeof(string));
            table.Columns.Add("RegisteredDate", typeof(string));
            table.Columns.Add("AuthIntervalDuration", typeof(int));
            table.Columns.Add("AuthActivatedDate", typeof(string));
            table.Columns.Add("AuthExpiredDate", typeof(string));
            table.Columns.Add("AuthStatus", typeof(string));
            table.Columns.Add("TemplateNumbers", typeof(string));

            if (response == null || response.Data == null) return table;

            foreach (var item in response.Data)
            {
                var templates = item.TemplateNumbers == null
                    ? string.Empty
                    : string.Join(", ", item.TemplateNumbers);

                table.Rows.Add(
                    item.Email,
                    item.TilakaName,
                    item.RegisteredDate,
                    item.AuthIntervalDuration ?? 0,
                    item.AuthActivatedDate,
                    item.AuthExpiredDate,
                    item.AuthStatus,
                    templates);
            }

            return table;
        }
    }
}
