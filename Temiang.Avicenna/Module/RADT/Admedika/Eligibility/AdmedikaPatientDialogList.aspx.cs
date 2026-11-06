using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaPatientDialogList : BasePageDialog
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEligibility;

            if (IsPostBack) return;

            txtGuarantorCardNo.Text = Request.QueryString["cardNo"];
            txtPatientName.Text = Request.QueryString["patientName"];
        }

        protected void btnSearch_Click(object sender, ImageClickEventArgs e)
        {
            grdPatient.Rebind();
        }

        protected void grdPatient_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdPatient.DataSource = Patients;
        }

        private DataTable Patients
        {
            get
            {
                var query = new PatientQuery("a");
                query.es.WithNoLock = true;
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                query.Select(
                    query.PatientID,
                    "<RTRIM(LTRIM(ISNULL(a.FirstName, '') + ' ' + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, ''))) AS PatientName>",
                    query.MedicalNo,
                    query.OldMedicalNo,
                    query.Ssn.As("SSN"),
                    query.Sex.As("Gender"),
                    query.DateOfBirth,
                    query.StreetName.As("Address"),
                    query.PhoneNo,
                    query.MobilePhoneNo
                );

                if (!string.IsNullOrWhiteSpace(txtGuarantorCardNo.Text))
                    query.Where(query.GuarantorCardNo == txtGuarantorCardNo.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                    query.Where(string.Format("<RTRIM(LTRIM(ISNULL(a.FirstName, '') + ' ' + ISNULL(a.MiddleName, '') + ' ' + ISNULL(a.LastName, ''))) LIKE '%{0}%'>",
                        Helper.EscapeQuery(txtPatientName.Text)));

                query.OrderBy(query.FirstName.Ascending, query.MiddleName.Ascending, query.LastName.Ascending);

                return query.LoadDataTable();
            }
        }

        public override bool OnButtonOkClicked()
        {
            if (grdPatient.SelectedValue == null || string.IsNullOrEmpty(grdPatient.SelectedValue.ToString()))
            {
                ShowInformationHeader("Select patient first.");
                return false;
            }

            return true;
        }

        public override string OnGetAdditionalJavaScriptCloseAndApply()
        {
            long transactionID;
            long.TryParse(Request.QueryString["id"], out transactionID);

            var selectedValue = grdPatient.SelectedValue;
            var patientID = HttpUtility.JavaScriptStringEncode(
                selectedValue == null ? string.Empty : selectedValue.ToString());

            return "oWnd.argument.mode = 'selectpatient!" + transactionID + "!" + patientID + "'";
        }
    }
}
