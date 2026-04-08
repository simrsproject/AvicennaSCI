using System;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using System.Data;
using Telerik.Web.UI;

namespace Temiang.Avicenna.Module.RADT.Master
{
    public partial class ImmunizationList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UrlPageDetail = "ImmunizationDetail.aspx";

            ProgramID = AppConstant.Program.Immunization;

            // Quick Search
            ToolBarMenuQuickSearch.Visible = true;

            ToolBarMenuSearch.Visible = false;
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string id = dataItem.GetDataKeyValue(ImmunizationMetadata.ColumnNames.ImmunizationID).ToString();
            Page.Response.Redirect("ImmunizationDetail.aspx?md=" + mode + "&id=" + id, true);
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = Immunizations;
        }

        private DataTable Immunizations
        {
            get
            {
                object obj = this.Session[SessionNameForList];
                if (obj != null)
                    return ((DataTable)(obj));

                ImmunizationQuery query;
                if (Session[SessionNameForQuery] != null)
                    query = (ImmunizationQuery)Session[SessionNameForQuery];
                else
                {
                    query = new ImmunizationQuery("a");
                    query.OrderBy(query.ImmunizationName.Ascending);

                    //Quick Search
                    ApplyQuickSearch(query);
                }
                
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                DataTable dtb = query.LoadDataTable();
                this.Session[SessionNameForList] = dtb;
                return dtb;
            }
        }
    }
}
