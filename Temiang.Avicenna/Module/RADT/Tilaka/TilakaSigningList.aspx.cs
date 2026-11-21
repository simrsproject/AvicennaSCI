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
            Response.Redirect("TilakaSigningDetail.aspx?id=" +
                              Server.UrlEncode(Convert.ToString(e.CommandArgument)));
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
