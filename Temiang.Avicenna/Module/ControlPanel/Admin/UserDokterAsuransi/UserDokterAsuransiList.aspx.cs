using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.ControlPanel.Admin.UserDokterAsuransi
{
    public partial class UserDokterAsuransiList : BasePageList
    {
        public override void OnMenuEditClick(Telerik.Web.UI.GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        public override void OnMenuViewClick(Telerik.Web.UI.GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string id = dataItem.GetDataKeyValue(AppUserMetadata.ColumnNames.UserID).ToString();
            Page.Response.Redirect("UserDokterAsuransiDetail.aspx?md=" + mode + "&id=" + id, true);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //UrlPageSearch = "UserSearch.aspx";
            UrlPageDetail = "UserDokterAsuransiDetail.aspx";

            ProgramID = AppConstant.Program.UserDokterAsuransi;

            // Quick Search
            ToolBarMenuQuickSearch.Visible = false;

            ToolBarMenuSearch.Visible = false;
            
            //Window Search
            //WindowSearch.Height = 120;
        }

        protected void grdList_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = AppUsersDokterAsuransi;
        }

        private DataTable AppUsersDokterAsuransi
        {
            get
            {
                object obj = Session[SessionNameForList];
                if (obj != null)
                    return ((DataTable)(obj));

                AppUserQuery query;
                if (Session[SessionNameForQuery] != null)
                    query = (AppUserQuery)Session[SessionNameForQuery];
                else
                {
                    query = new AppUserQuery("u");
                    var par = new ParamedicQuery("p");

                    query.LeftJoin(par).On(query.ParamedicID == par.ParamedicID);

                    // 🔹 filter hanya user yang login
                    query.Where(query.UserID == AppSession.UserLogin.UserID);

                    query.Select(query, par.ParamedicName);

                    //Quick Search
                    //ApplyQuickSearch(query);
                }

                query.es.Top = AppSession.Parameter.MaxResultRecord;
                DataTable dtb = query.LoadDataTable();
                Session[SessionNameForList] = dtb;
                return dtb;
            }
        }
        //protected void grdList_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        //
            //GridDataItem dataItem = e.DetailTableView.ParentItem;
            //string userID = dataItem.GetDataKeyValue("UserID").ToString();

            //if (e.DetailTableView.Name.Equals("grdUserGroup"))
            //{
               //AppUserUserGroupQuery query = new AppUserUserGroupQuery("a");
                //AppUserGroupQuery qb = new AppUserGroupQuery("b");
               // query.InnerJoin(qb).On(query.UserGroupID == qb.UserGroupID);
               // query.Where(query.UserID == userID);
               // query.OrderBy(qb.UserGroupName.Ascending);

               // query.Select
                   // (
                 //       query.UserGroupID, qb.UserGroupName
                  //  );

               // e.DetailTableView.DataSource = query.LoadDataTable();
           // }
           // else
           // {
           //     AppUserServiceUnitQuery query = new AppUserServiceUnitQuery("a");
          //      ServiceUnitQuery qb = new ServiceUnitQuery("b");
            //    query.InnerJoin(qb).On(query.ServiceUnitID == qb.ServiceUnitID);
            //    query.Where(query.UserID == userID, qb.IsActive == true);
            //    query.OrderBy(qb.ServiceUnitName.Ascending);

             //   query.Select
             //       (
             //           query.ServiceUnitID, qb.ServiceUnitName
             //       );

              //  e.DetailTableView.DataSource = query.LoadDataTable();
           // }
       // }
    }
}