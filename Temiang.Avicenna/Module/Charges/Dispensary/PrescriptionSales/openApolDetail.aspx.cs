using System;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales
{
    public partial class openApolDetail : BasePageDialog
    {
        public string PrescriptionNo => Request.QueryString["pno"];
        public string RegistrationNo => Request.QueryString["rno"];

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ButtonOk.Visible = false;
                ButtonCancel.Text = "Close";
                LoadHeader();
            }
        }

        private void LoadHeader()
        {
            var apol = new BpjsApol();
            apol.Query.Where(
                apol.Query.PrescriptionNo == PrescriptionNo,
                apol.Query.RegistrationNo == RegistrationNo
            );

            if (apol.Query.Load())
            {
                lblNOSJP.Text = apol.NOAPOTIK;
                lblNORESEP.Text = apol.NORESEP;
            }
        }

        protected void grdApolDetail_NeedDataSource(
            object sender,
            Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var apolDet = new BpjsApolDetailCollection();

            apolDet.Query.Where(
                apolDet.Query.PrescriptionNo == PrescriptionNo
            );

            apolDet.Query.OrderBy(
                apolDet.Query.SequenceNo.Ascending
            );

            apolDet.Query.Load();

            grdApolDetail.DataSource = apolDet;
        }
    }
}