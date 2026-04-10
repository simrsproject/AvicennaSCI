using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Master
{
    public partial class ItemConsumptionDetail : BaseUserControl
    {
        private object _dataItem;

        public object DataItem
        {
            get { return _dataItem; }
            set { _dataItem = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            PopUpSearch.InitializeOnButtonClick(AppEnum.PopUpSearch.ItemProductInventory, txtDetailItemID);

            if (DataItem is GridInsertionObject)
            {
                ViewState["IsNewRecord"] = true;
                return;
            }
            ViewState["IsNewRecord"] = false;
            txtDetailItemID.Enabled = false;
            txtDetailItemID.ShowButton = false;
            txtDetailItemID.Text = (String)DataBinder.Eval(DataItem, ItemConsumptionMetadata.ColumnNames.DetailItemID);
            txtQty.Value = Convert.ToDouble(DataBinder.Eval(DataItem, ItemConsumptionMetadata.ColumnNames.Qty));
            txtSRItemUnit.Text = (String)DataBinder.Eval(DataItem, ItemConsumptionMetadata.ColumnNames.SRItemUnit);

            PopulateDetailItemName(false);
            try
            {
                txtQtyDosage.Value = Convert.ToDouble(DataBinder.Eval(DataItem, ItemConsumptionMetadata.ColumnNames.QtyDosage));
            }
            catch
            {
                txtQtyDosage.Value = 0;
            }
            try
            {
                var dosageUnit = (String)DataBinder.Eval(DataItem, ItemConsumptionMetadata.ColumnNames.SRDosageUnit);
                if (!string.IsNullOrEmpty(dosageUnit))
                {
                    var dosageq = new AppStandardReferenceItemQuery();
                    dosageq.Where(dosageq.StandardReferenceID == AppEnum.StandardReference.DosageUnit.ToString(), dosageq.ItemID == dosageUnit);
                    dosageq.Select(dosageq.ItemID, dosageq.ItemName);
                    cboSRDosageUnit.DataSource = dosageq.LoadDataTable();
                    cboSRDosageUnit.DataBind();
                    cboSRDosageUnit.SelectedValue = dosageUnit;
                }
                else
                {
                    cboSRDosageUnit.Items.Clear();
                    cboSRDosageUnit.SelectedValue = string.Empty;
                    cboSRDosageUnit.Text = string.Empty;
                }
            }
            catch
            {
                cboSRDosageUnit.Items.Clear();
                cboSRDosageUnit.SelectedValue = string.Empty;
                cboSRDosageUnit.Text = string.Empty;
            }
        }

        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Check duplicate key
            if (ViewState["IsNewRecord"].Equals(true))
            {
                ItemConsumptionCollection coll =
                    (ItemConsumptionCollection)Session["collItemConsumption"];

                string detailItemID = txtDetailItemID.Text;
                bool isExist = false;
                foreach (ItemConsumption item in coll)
                {
                    if (item.DetailItemID.Equals(detailItemID))
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist)
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = string.Format("Detail Item ID: {0} has exist", detailItemID);
                }
            }
        }

        #region Properties for return entry value
        public String DetailItemID
        {
            get { return txtDetailItemID.Text; }
        }
        public String DetailItemName
        {
            get { return lblDetailItemName.Text; }
        }
        public Decimal? Qty
        {
            get { return Convert.ToDecimal(txtQty.Value); }
        }
        public String SRItemUnit
        {
            get { return txtSRItemUnit.Text; }
        }
        public Decimal? QtyDosage
        {
            get { return Convert.ToDecimal(txtQtyDosage.Value); }
        }
        public String SRDosageUnit
        {
            get { return cboSRDosageUnit.SelectedValue; }
        }
        #endregion

        #region Method & Event TextChanged
        protected void txtDetailItemID_TextChanged(object sender, EventArgs e)
        {
            PopulateDetailItemName(true);
        }

        private void PopulateDetailItemName(bool isResetIdIfNotExist)
        {
            if (txtDetailItemID.Text == string.Empty)
            {
                lblDetailItemName.Text = string.Empty;
                return;
            }
            Item entity = new Item();
            if (entity.LoadByPrimaryKey(txtDetailItemID.Text))
            {
                txtDetailItemID.Text = entity.ItemID;
                lblDetailItemName.Text = entity.ItemName;

                PopulateItemUnit(entity.ItemID, entity.SRItemType, isResetIdIfNotExist);
            }
            else
            {
                lblDetailItemName.Text = string.Empty;
                txtSRItemUnit.Text = string.Empty;
                if (isResetIdIfNotExist)
                    txtDetailItemID.Text = string.Empty;
            }
        }

        private void PopulateItemUnit(string itemID, string itemType, bool isResetIdIfNotExist)
        {
            if (BusinessObject.Reference.ItemType.Medical.Equals(itemType))
            {
                ItemProductMedic item = new ItemProductMedic();
                if (item.LoadByPrimaryKey(itemID))
                {
                    txtSRItemUnit.Text = item.SRItemUnit;

                    if (isResetIdIfNotExist)
                    {
                        if (!string.IsNullOrEmpty(item.SRDosageUnit))
                        {
                            txtQtyDosage.Value = Convert.ToDouble(item.Dosage);
                            var dosageq = new AppStandardReferenceItemQuery();
                            dosageq.Where(dosageq.StandardReferenceID == AppEnum.StandardReference.DosageUnit.ToString(), dosageq.ItemID == item.SRDosageUnit);
                            dosageq.Select(dosageq.ItemID, dosageq.ItemName);
                            cboSRDosageUnit.DataSource = dosageq.LoadDataTable();
                            cboSRDosageUnit.DataBind();
                            cboSRDosageUnit.SelectedValue = item.SRDosageUnit;
                        }
                        else
                        {
                            txtQtyDosage.Value = 0;
                            cboSRDosageUnit.Items.Clear();
                            cboSRDosageUnit.SelectedValue = string.Empty;
                            cboSRDosageUnit.Text = string.Empty;
                        }
                    }
                }
                else
                {
                    txtSRItemUnit.Text = string.Empty;
                    txtQtyDosage.Value = 0;
                    cboSRDosageUnit.Items.Clear();
                    cboSRDosageUnit.SelectedValue = string.Empty;
                    cboSRDosageUnit.Text = string.Empty;
                }
            }
            else
            {
                ItemProductNonMedic item = new ItemProductNonMedic();
                if (item.LoadByPrimaryKey(itemID))
                    txtSRItemUnit.Text = item.SRItemUnit;

                txtQtyDosage.Value = 0;
                cboSRDosageUnit.Items.Clear();
                cboSRDosageUnit.SelectedValue = string.Empty;
                cboSRDosageUnit.Text = string.Empty;
            }
        }

        protected void cboSRDosageUnit_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            string searchTextContain = string.Format("%{0}%", e.Text);
            var query = new AppStandardReferenceItemQuery();
            query.es.Top = 10;
            query.Select
                (
                    query.ItemID,
                    query.ItemName
                );
            query.Where
                (
                    query.Or
                        (
                            query.ItemID.Like(searchTextContain),
                            query.ItemName.Like(searchTextContain)
                        ),
                        query.StandardReferenceID == AppEnum.StandardReference.DosageUnit.ToString(),
                        query.IsActive == true
                );

            cboSRDosageUnit.DataSource = query.LoadDataTable();
            cboSRDosageUnit.DataBind();
        }

        protected void cboStandardReferenceItem_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((DataRowView)e.Item.DataItem)["ItemName"].ToString();
            e.Item.Value = ((DataRowView)e.Item.DataItem)["ItemID"].ToString();
        }
        #endregion
    }
}
