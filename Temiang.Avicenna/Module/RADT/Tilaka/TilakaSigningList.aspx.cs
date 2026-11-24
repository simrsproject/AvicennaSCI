using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaSigningList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaSigning;
            UrlPageSearch = "TilakaSigningList.aspx";
            UrlPageDetail = "TilakaSigningDetail.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuSearch.Visible = false;
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
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
            RedirectToDetail(Convert.ToString(e.CommandArgument));
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            if (dataItems == null || dataItems.Length == 0) return;

            RedirectToDetail(Convert.ToString(dataItems[0].GetDataKeyValue("RequestID")));
        }

        private void RedirectToDetail(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId)) return;

            Response.Redirect("TilakaSigningDetail.aspx?id=" + Server.UrlEncode(requestId));
        }

        private DataTable GetList()
        {
            object cached = Session[SessionNameForList];
            if (cached != null) return (DataTable)cached;

            var query = new TilakaQuickSignRequestQuery();
            if (!string.IsNullOrWhiteSpace(txtRequestID.Text))
                query.Where(query.RequestID.Like("%" + txtRequestID.Text.Trim() + "%"));
            if (!string.IsNullOrWhiteSpace(cboStatus.SelectedValue))
                query.Where(query.Status == cboStatus.SelectedValue);

            query.es.Top = AppSession.Parameter.MaxResultRecord;
            query.OrderBy(query.QuickSignRequestID.Descending);

            var table = query.LoadDataTable();
            Session[SessionNameForList] = table;
            return table;
        }
    }
}
