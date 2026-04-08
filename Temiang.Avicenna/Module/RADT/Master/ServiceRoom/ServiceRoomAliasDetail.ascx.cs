using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Master
{
    public partial class ServiceRoomAliasDetail : BaseUserControl
    {
        private object _dataItem;

        public object DataItem
        {
            get { return _dataItem; }
            set { _dataItem = value; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            StandardReference.InitializeIncludeSpace(cboBridgingType, AppEnum.StandardReference.BridgingType);

            if (DataItem is GridInsertionObject)
            {
                ViewState["IsNewRecord"] = true;

                chkIsActive.Checked = true;
                return;
            }
            ViewState["IsNewRecord"] = false;

            cboBridgingType.SelectedValue = (String)DataBinder.Eval(DataItem, ClassBridgingMetadata.ColumnNames.SRBridgingType);
            cboBridgingType_SelectedIndexChanged(null, new RadComboBoxSelectedIndexChangedEventArgs(string.Empty, string.Empty, cboBridgingType.SelectedValue, string.Empty));
            cboServiceUnitAliasID.SelectedValue = (String)DataBinder.Eval(DataItem, ClassBridgingMetadata.ColumnNames.BridgingID);
            txtServiceUnitAliasName.Text = (String)DataBinder.Eval(DataItem, ClassBridgingMetadata.ColumnNames.BridgingName);
            chkIsActive.Checked = Convert.ToBoolean(DataBinder.Eval(DataItem, ClassBridgingMetadata.ColumnNames.IsActive));
        }

        protected void cboBridgingType_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            cboServiceUnitAliasID.Items.Clear();
            cboServiceUnitAliasID.SelectedValue = string.Empty;
            cboServiceUnitAliasID.Text = string.Empty;

            if (e.Value == AppEnum.BridgingType.BPJS.ToString() && Common.Helper.IsBpjsIntegration)
            {
            }
            else if (e.Value == AppEnum.BridgingType.RS_ONLINE.ToString() && Common.Helper.IsRsOnlineIntegration)
            {
                var svc = new Common.RsOnline.Service();
                var response = svc.ReferensiTempatTidur();
                if (response?.TempatTidur != null && response.TempatTidur.Any())
                {
                    cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                    foreach (var item in response.TempatTidur)
                    {
                        cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(item.NamaTt, item.KodeTt));
                    }
                }
            }
            else if (e.Value == AppEnum.BridgingType.Inhealth.ToString() && Common.Helper.IsInhealthIntegration)
            {
                var collTitle = new AppStandardReferenceItemCollection();
                collTitle.Query.Where(
                    collTitle.Query.StandardReferenceID == AppEnum.StandardReference.InhealthClassType,
                    collTitle.Query.IsActive == true
                    );
                collTitle.Query.OrderBy(collTitle.Query.ItemID.Ascending);
                collTitle.LoadAll();
                cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                foreach (var item in collTitle)
                {
                    cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(item.ItemID + " - " + item.ItemName, item.ItemID));
                }
            } else if (e.Value.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
            {
                if (e.Text.Length < 3)
                {
                    cboServiceUnitAliasID.DataSource = null;
                    cboServiceUnitAliasID.DataBind();
                    cboServiceUnitAliasID.Items.Clear();
                    cboServiceUnitAliasID.SelectedValue = string.Empty;
                    return;
                }

                var util = new Bridging.SatuSehat.Utils();
                var token = string.Empty;
                cboServiceUnitAliasID.Items.Clear();
                var response = util.RestClientGet(String.Concat("Location?name==", e.Text), string.Empty, ref token);
                if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var searchResponse = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.LocationSearchResponse>(response.Content);
                    if (searchResponse.Total > 0)
                    {
                        foreach (var item in searchResponse.Entry)
                        {
                            cboServiceUnitAliasID.Items.Add(new RadComboBoxItem(item.Resource.Name, item.Resource.Id));
                        }
                    }
                    cboServiceUnitAliasID.Items.Add(new RadComboBoxItem("Not found, create a new Bridging ID when saving", "CREATE"));
                }
            }
        }

        protected void cboServiceUnitAliasID_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboBridgingType.SelectedValue == AppEnum.BridgingType.BPJS.ToString() && Common.Helper.IsBpjsIntegration)
            {
            }
            else if (cboBridgingType.SelectedValue == AppEnum.BridgingType.Inhealth.ToString() && Common.Helper.IsInhealthIntegration)
            {
                var collTitle = new AppStandardReferenceItem();
                collTitle.Query.es.Top = 1;
                collTitle.Query.Where(
                    collTitle.Query.StandardReferenceID == AppEnum.StandardReference.InhealthClassType,
                    collTitle.Query.ItemID == cboServiceUnitAliasID.SelectedValue
                    );
                if (collTitle.Query.Load()) txtServiceUnitAliasName.Text = collTitle.ItemName;
            }


            if (cboBridgingType.SelectedValue.ToLower() == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower())
                cboServiceUnitAliasID.Filter = RadComboBoxFilter.None;
            else
                cboServiceUnitAliasID.Filter = RadComboBoxFilter.Contains;
        }

        protected void customValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //Check duplicate key
            if (ViewState["IsNewRecord"].Equals(true))
            {
                var coll = (ServiceRoomBridgingCollection)Session["collServiceRoomBridging"];

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

        public String BridgingID
        {
            get { return string.IsNullOrEmpty(cboServiceUnitAliasID.SelectedValue) ? cboServiceUnitAliasID.Text : cboServiceUnitAliasID.SelectedValue; }
        }

        public String BridgingName
        {
            get
            {
                return string.IsNullOrWhiteSpace(txtServiceUnitAliasName.Text) ? (Helper.FindControlRecursive(this.Page, "txtRoomName") as RadTextBox).Text : txtServiceUnitAliasName.Text;
            }
        }

        public Boolean IsActive
        {
            get { return chkIsActive.Checked; }
        }
    }
}