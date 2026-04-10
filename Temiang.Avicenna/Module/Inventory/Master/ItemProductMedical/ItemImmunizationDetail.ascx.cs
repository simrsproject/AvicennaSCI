using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.Inventory.Master
{
    public partial class ItemImmunizationDetail : BaseUserControl
    {
        public object DataItem { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            if (DataItem is GridInsertionObject)
            {
                ViewState["IsNewRecord"] = true;

                return;
            }
            ViewState["IsNewRecord"] = false;

            var query = new ImmunizationQuery();
            query.Select
                (
                    query.ImmunizationID,
                    query.ImmunizationName
                );
            query.OrderBy(query.ImmunizationName.Ascending);

            cboImmunizationID.DataSource = query.LoadDataTable();
            cboImmunizationID.DataBind();

            cboImmunizationID.SelectedValue = DataBinder.Eval(DataItem, ItemImmunizationMetadata.ColumnNames.ImmunizationID).ToString();
        }

        protected void cboImmunizationID_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            string searchTextContain = string.Format("%{0}%", e.Text);
            var query = new ImmunizationQuery();
            query.es.Top = 10;
            query.Select
                (
                    query.ImmunizationID,
                    query.ImmunizationName
                );
            query.Where
                (
                    query.ImmunizationName.Like(searchTextContain)
                );

            cboImmunizationID.DataSource = query.LoadDataTable();
            cboImmunizationID.DataBind();
        }

        protected void cboImmunizationID_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((DataRowView)e.Item.DataItem)["ImmunizationName"].ToString();
            e.Item.Value = ((DataRowView)e.Item.DataItem)["ImmunizationID"].ToString();
        }

        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Check duplicate key
            if (ViewState["IsNewRecord"].Equals(true))
            {
                var coll = (BusinessObject.ItemImmunizationCollection)Session["collItemImmunization"];

                var id = cboImmunizationID.SelectedValue;
                var isExist = coll.Any(row => row.ItemID.Equals(id));
                if (isExist)
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = string.Format("Immunization: {0} has exist", cboImmunizationID.Text);
                }
            }
        }

        public String ImmunizationID
        {
            get { return cboImmunizationID.SelectedValue; }
        }

        public String Immunizationame
        {
            get { return cboImmunizationID.Text; }
        }

    }
}