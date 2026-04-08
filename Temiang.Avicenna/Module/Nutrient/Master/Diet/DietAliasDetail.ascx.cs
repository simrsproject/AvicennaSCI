using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.BusinessObject;
using Telerik.Web.UI;
using System.Data;

namespace Temiang.Avicenna.Module.Nutrient.Master
{
    public partial class DietAliasDetail : BaseUserControl
    {
        private object _dataItem;

        public object DataItem
        {
            get { return _dataItem; }
            set { _dataItem = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            cboSsBridgingID.Visible = false;
            StandardReference.InitializeIncludeSpace(cboBridgingType, AppEnum.StandardReference.BridgingType);


            if (cboBridgingType.SelectedValue.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
            {
                cboSsBridgingID.Visible = true;
                cboServiceUnitAliasID.Visible = false;
                rfvServiceUnitAliasID.ControlToValidate = "cboSsBridgingID";
            }

            if (DataItem is GridInsertionObject)
            {
                ViewState["IsNewRecord"] = true;

                chkIsActive.Checked = true;
                return;
            }
            ViewState["IsNewRecord"] = false;


            cboBridgingType.SelectedValue = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.SRBridgingType);
            cboBridgingType_SelectedIndexChanged(null, new RadComboBoxSelectedIndexChangedEventArgs(string.Empty, string.Empty, cboBridgingType.SelectedValue, string.Empty));
            if (cboServiceUnitAliasID.Items.Any())
            {
               cboServiceUnitAliasID.SelectedValue = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.BridgingID);
            }
            else if (cboBridgingType.SelectedValue.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
                cboSsBridgingID.Text = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.BridgingID);
            else
                cboServiceUnitAliasID.Text = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.BridgingID);

            txtServiceUnitAliasName.Text = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.BridgingName);
            //txtItemIdExternal.Text = (String)DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.ItemIdExternal);
            chkIsActive.Checked = Convert.ToBoolean(DataBinder.Eval(DataItem, DietBridgingMetadata.ColumnNames.IsActive));
        }

        protected void cboBridgingType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            cboSsBridgingID.Visible = false;
            //cboServiceUnitAliasID.Items.Clear();

            //if (e.Value == AppEnum.BridgingType.Inhealth.ToString() && Common.Helper.IsInhealthIntegration)
            //    StandardReference.InitializeIncludeSpace(cboServiceUnitAliasID, AppEnum.StandardReference.InhealthItemService);

            if (e.Value == AppEnum.BridgingType.LINK_LIS.ToString())
            {
                var lpar = new BusinessObject.Interop.LINKLIS.ListParameterQuery("a");
                lpar.es2.Connection.Name = AppConstant.HIS_INTEROP.LINK_LIS_INTEROP_CONNECTION_NAME;
                var lpem = new BusinessObject.Interop.LINKLIS.ListPemeriksaanQuery("b");
                lpem.es2.Connection.Name = AppConstant.HIS_INTEROP.LINK_LIS_INTEROP_CONNECTION_NAME;

                lpar.Select(lpar.SelectAll(), lpem.NamaPemeriksaan);
                lpar.InnerJoin(lpem).On(lpar.KodePemeriksaan == lpem.KodePemeriksaan);
                lpar.Where(lpar.KodePemeriksaan != string.Empty);

                var table = lpar.LoadDataTable();
                cboServiceUnitAliasID.Items.Clear();
                cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                foreach (DataRow group in table.Rows)
                {
                    cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(string.Format("{0}|{1}", group["NamaPemeriksaan"].ToString(), group["NamaParameter"].ToString()), string.Format("{0}|{1}", group["KodePemeriksaan"].ToString(), group["KodeParameter"].ToString())));
                }
            }
            else if (cboBridgingType.SelectedValue.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
            {
                cboSsBridgingID.Visible = true;
                cboServiceUnitAliasID.Visible = false;
                rfvServiceUnitAliasID.ControlToValidate = "cboSsBridgingID";
            }
            else
            {
                var lis = new AppStandardReferenceItem();
                if (lis.LoadByPrimaryKey(AppEnum.StandardReference.BridgingType.ToString(), e.Value) && !string.IsNullOrEmpty(lis.Note) && lis.Note == "LIS")
                    rfvServiceUnitAliasID.Visible = false;
                cboServiceUnitAliasID.Items.Clear();
            }

        }

        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Check duplicate key
            if (ViewState["IsNewRecord"].Equals(true))
            {
                var coll = (DietBridgingCollection)Session["collDietBridging"];

                string itemID = cboServiceUnitAliasID.SelectedValue;
                bool isExist = false;
                foreach (var item in coll)
                {
                    if (item.BridgingID.Equals(itemID) && item.SRBridgingType.Equals(cboBridgingType.SelectedValue))
                    {
                        isExist = true;
                        break;
                    }
                }
                if (isExist)
                {
                    args.IsValid = false;
                    ((CustomValidator)source).ErrorMessage = string.Format("Bridging ID : {0} already exist", itemID);
                }
            }
        }

        public String BridgingType
        {
            get { return cboBridgingType.SelectedValue; }
        }

        public String BridgingTypeName
        {
            get { return cboBridgingType.Text; }
        }

        public String BridgingGroupID
        {
            get { return cboBridgingType.SelectedValue == AppEnum.BridgingType.LINK_LIS.ToString() ? cboServiceUnitAliasID.SelectedValue.Split('|')[0] : string.Empty; }
        }

        public String BridgingGroupName
        {
            get { return cboBridgingType.SelectedValue == AppEnum.BridgingType.LINK_LIS.ToString() ? cboServiceUnitAliasID.Text.Split('|')[0] : string.Empty; }
        }

        public String BridgingID
        {
            get
            {
                if (cboBridgingType.SelectedValue.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
                    return cboSsBridgingID.Text;
                return string.IsNullOrEmpty(cboServiceUnitAliasID.SelectedValue) ? cboServiceUnitAliasID.Text : cboBridgingType.SelectedValue == AppEnum.BridgingType.LINK_LIS.ToString() ? cboServiceUnitAliasID.SelectedValue.Split('|')[1] : cboServiceUnitAliasID.SelectedValue;
            }
        }

        public String BridgingName
        {
            get { return cboBridgingType.SelectedValue == AppEnum.BridgingType.LINK_LIS.ToString() ? cboServiceUnitAliasID.Text.Split('|')[1] : txtServiceUnitAliasName.Text; }
        }

        //public String ItemIdExternal
        //{
        //    get { return txtItemIdExternal.Text; }
        //}

        public Boolean IsActive
        {
            get { return chkIsActive.Checked; }
        }

        protected void cboBridgingGroupID_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Value))
            {
                cboServiceUnitAliasID.Items.Clear();
                return;
            }

            var svc = new Common.LinkLis.Service();
            var groups = svc.GetListParameter(e.Value);
            cboServiceUnitAliasID.Items.Clear();
            cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
            foreach (var group in groups.ListParameter)
            {
                cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(group.NamaPemeriksaan, group.Kode));
            }
        }
    }
}