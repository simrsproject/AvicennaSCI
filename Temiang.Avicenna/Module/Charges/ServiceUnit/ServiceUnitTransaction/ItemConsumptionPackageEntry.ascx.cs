using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using System.Data;
using DevExpress.XtraEditors;

namespace Temiang.Avicenna.Module.Charges
{
    public partial class ItemConsumptionPackageEntry : BaseUserControl
    {
        private object _dataItem;

        public object DataItem
        {
            get { return _dataItem; }
            set { _dataItem = value; }
        }

        private string PageId
        {
            get { return Request.QueryString["pageId"].ToString(); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            if (DataItem is GridInsertionObject)
            {
                ViewState["IsNewRecord"] = true;
                return;
            }

            ViewState["IsNewRecord"] = false;
        }

        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Check duplicate key
            if (ViewState["IsNewRecord"].Equals(true))
            {

                var coll = ((TransChargesItemConsumptionCollection)Session["collTransChargesItemConsumption" + Request.UserHostName + PageId]).Where(i => i.TransactionNo == Request.QueryString["trans"] &&
                                                                                                                 i.SequenceNo.Substring(0, 3) == Request.QueryString["seq"]);

                string detailItemID = cboDetailItemID.SelectedValue;
                bool isExist = false;
                foreach (TransChargesItemConsumption item in coll)
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
                    return;
                }
            }

            // Check Vaccine Stat
            if (IsVaccine)
            {
                if (txtQtyDosage.Value == 0 || string.IsNullOrEmpty(cboSRDosageUnit.SelectedValue))
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = "The item is a vaccine drug that requires Dosage & Unit information.";
                    return;
                }

                if (string.IsNullOrEmpty(txtBatchNumber.Text))
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = "The item is a vaccine drug that requires Batch Number information.";
                    return;
                }
                if (string.IsNullOrEmpty(cboSRImmReason.SelectedValue))
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = "The item is a vaccine drug that requires Immunization Reason information.";
                    return;
                }
                if (string.IsNullOrEmpty(cboSRImmTiming.SelectedValue))
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = "The item is a vaccine drug that requires Immunization Routine Timing information.";
                    return;
                }
                if (txtExpirationDate.IsEmpty)
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = "The item is a vaccine drug that requires Expiration Date information.";
                    return;
                }
            }
        }

        #region Properties for return entry value
        public String DetailItemID
        {
            get { return cboDetailItemID.SelectedValue; }
        }
        public String DetailItemName
        {
            get { return cboDetailItemID.Text; }
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
        public String BatchNumber
        {
            get { return txtBatchNumber.Text; }
        }
        public String SRImmReason
        {
            get { return cboSRImmReason.SelectedValue; }
        }
        public String SRImmTiming
        {
            get { return cboSRImmTiming.SelectedValue; }
        }
        public DateTime ExpirationDate
        {
            get { return txtExpirationDate.SelectedDate.Value; }
        }
        public bool IsVaccine
        {
            get { return hdnIsVaccine.Value == "1"; ; }
        }

        #endregion

        #region cboItemID

        protected void cboDetailItemID_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            if (ViewState["LocationID" + Request.UserHostName + Request.QueryString["pageId"]] == null)
            {
                var unit = new ServiceUnit();
                unit.LoadByPrimaryKey(Request.QueryString["unit"]);
                ViewState["LocationID" + Request.UserHostName + Request.QueryString["pageId"]] = unit.GetMainLocationId(unit.ServiceUnitID);
            }

            string searchTextContain = string.Format("%{0}%", e.Text);
            var query = new ItemQuery("a");
            var prodmedQ = new VwItemProductMedicNonMedicQuery("b");
            var balance = new ItemBalanceQuery("d");

            query.es.Top = 20;
            query.InnerJoin(prodmedQ).On(query.ItemID == prodmedQ.ItemID);
            query.InnerJoin(balance).On
            (
                query.ItemID == balance.ItemID &
                balance.LocationID == ViewState["LocationID" + Request.UserHostName + Request.QueryString["pageId"]]
            );
            query.Where(query.Or(query.ItemID.Like(searchTextContain),
                                 query.ItemName.Like(searchTextContain)));
            query.Select
                (
                    query.ItemID,
                    query.ItemName,
                    (balance.Balance.Coalesce("0") - balance.Booking.Coalesce("0")).As("Balance")
                );
            cboDetailItemID.DataSource = query.LoadDataTable();
            cboDetailItemID.DataBind();
        }

        protected void cboDetailItemID_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((DataRowView)e.Item.DataItem)["ItemName"].ToString();
            e.Item.Value = ((DataRowView)e.Item.DataItem)["ItemID"].ToString();
        }

        protected void cboDetailItemID_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var item = new Item();
            if (!item.LoadByPrimaryKey(e.Value))
            {
                cboDetailItemID.Text = string.Empty;
                return;
            }

            PopulateItemUnit(item.ItemID, item.SRItemType);
        }

        private void PopulateItemUnit(string itemID, string itemType)
        {
            divVaccineInf.Visible = false;
            hdnIsVaccine.Value = "0";
            if (BusinessObject.Reference.ItemType.Medical.Equals(itemType))
            {
                var item = new ItemProductMedic();
                if (item.LoadByPrimaryKey(itemID))
                {
                    txtSRItemUnit.Text = item.SRItemUnit;
                    if (item.IsVaccine ?? false)
                    {
                        divVaccineInf.Visible = true;
                        hdnIsVaccine.Value = "1";

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

                        if (cboSRImmReason.Items.Count == 0)
                            StandardReference.InitializeIncludeSpace(cboSRImmReason, AppEnum.StandardReference.ImmReason);
                        if (cboSRImmTiming.Items.Count == 0)
                            StandardReference.InitializeIncludeSpace(cboSRImmTiming, AppEnum.StandardReference.ImmTiming);

                        txtBatchNumber.Text = string.Empty;
                        txtExpirationDate.Clear();
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
                var item = new ItemProductNonMedic();
                if (item.LoadByPrimaryKey(itemID))
                    txtSRItemUnit.Text = item.SRItemUnit;
                else
                    txtSRItemUnit.Text = string.Empty;

                txtSRItemUnit.Text = string.Empty;
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