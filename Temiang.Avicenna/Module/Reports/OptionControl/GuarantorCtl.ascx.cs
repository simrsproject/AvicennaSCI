using System;
using System.Collections.Generic;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.Reports.OptionControl
{
    public partial class GuarantorCtl : BaseOptionCtl
    {

        #region ComboBox 
        public void cboGuarantor_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            ComboBox.GuarantorItemsRequestedClean((RadComboBox)sender,e.Text);
        }
        //public void cboGuarantor_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        //{
        //    var dataItem = e.Item.DataItem as DataRowView;
        //    if (dataItem != null)
        //    {
        //        e.Item.Value = dataItem["GuarantorID"].ToString();
        //        e.Item.Text = dataItem["GuarantorName"].ToString();
        //    }
        //}

        protected void cboGuarantor_SelectedIndexChanged(object sender,RadComboBoxSelectedIndexChangedEventArgs e)
        {
            hdnGuarantorID.Value = e.Value;
        }
        #endregion

        public override PrintJobParameterCollection PrintJobParameters()
        {
            PrintJobParameterCollection parameters = new PrintJobParameterCollection();
            parameters.AddNew("p_GuarantorID", hdnGuarantorID.Value);

            //Retun List
            return parameters;
        }

        public override string ParameterCaption
        {
            get { return lblCaption.Text; }
            set { lblCaption.Text = value; }
        }
        public override string ReportSubTitle
        {
            get
            {
                return string.Format("Guarantor : {0}", cboGuarantorID.SelectedValue);
            }
        }

    }
}