using System;
using System.Web;
using System.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;
using AdmedikaService = Temiang.Avicenna.Common.Admedika.Service;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEligibilityDetail : BasePageDetail
    {
        private long ClaimTransactionID
        {
            get
            {
                long id;
                long.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEligibility;
            UrlPageList = "AdmedikaEligibilityList.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDetail();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuDelete.Visible = false;
            ToolBarMenuSave.Visible = false;
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
            ToolBarMenuApproval.Visible = false;
            ToolBarMenuUnApproval.Visible = false;
            ToolBarMenuVoid.Visible = false;
            ToolBarMenuUnVoid.Visible = false;
        }

        private void LoadDetail()
        {
            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowInformationHeader("Data eligibility tidak ditemukan.");
                btnRePrintLOA.Enabled = false;
                btnGetClaimStatus.Enabled = false;
                return;
            }

            var patientLoaded = false;
            var patient = new Patient();
            if (!string.IsNullOrEmpty(claim.PatientID))
                patientLoaded = patient.LoadByPrimaryKey(claim.PatientID);

            var registrationLoaded = false;
            var registration = new Registration();
            if (!string.IsNullOrEmpty(claim.RegistrationNo))
                registrationLoaded = registration.LoadByPrimaryKey(claim.RegistrationNo);

            var guarantorLoaded = false;
            var guarantor = new Guarantor();
            if (registrationLoaded && !string.IsNullOrEmpty(registration.GuarantorID))
                guarantorLoaded = guarantor.LoadByPrimaryKey(registration.GuarantorID);

            lblClID.Text = Value(claim.ClID);
            lblClStatus.Text = Value(claim.ClStatus);
            lblClDesc.Text = Value(claim.ClDesc);
            lblCreatedDateTime.Text = FormatDateTime(claim.CreatedDateTime);
            lblIsClosed.Text = claim.IsClosed == true ? "Yes" : "No";
            lblDischargeDateTime.Text = FormatDateTime(claim.DischargeDateTime);

            var coverage = AdmedikaCoverageType.Get(claim.CovID);
            var covCode = claim.CovCode ?? string.Empty;
            if (coverage != null && !string.IsNullOrWhiteSpace(coverage.Description))
                covCode = (string.IsNullOrWhiteSpace(covCode) ? string.Empty : covCode + " - ") + coverage.Description;

            lblCovID.Text = Value(claim.CovID);
            lblCovCode.Text = Value(covCode);
            lblPlanCode.Text = Value(claim.PlanCode);
            lblPolicyNo.Text = Value(claim.PolicyNo);
            lblPayorMemberID.Text = Value(claim.PayorMemberID);
            lblPolicyDate.Text = Value(claim.PolicyDate);

            lblCardNo.Text = Value(claim.CardNo);
            lblPatientID.Text = Value(claim.PatientID);
            lblPatientNameAvicenna.Text = patientLoaded ? Value(patient.FullName) : "-";
            lblPatientNameAdmedika.Text = Value(claim.PatientName);
            lblMedicalNo.Text = Value(claim.MedicalNo);
            lblDateOfBirth.Text = FormatDate(claim.DateOfBirth);

            lblRegistrationNo.Text = Value(claim.RegistrationNo);
            lblGuarantor.Text = registrationLoaded
                ? Value(registration.GuarantorID) + (guarantorLoaded ? " / " + Value(guarantor.GuarantorName) : string.Empty)
                : "-";
            lblIsVoid.Text = registrationLoaded ? (registration.IsVoid == true ? "Yes" : "No") : "-";

            var printOut = string.IsNullOrWhiteSpace(claim.PrintOutLOA) ? claim.PrintOut : claim.PrintOutLOA;
            litPrintOutLOA.Text = BuildPrintOutHtml(printOut);
            btnRePrintLOA.Enabled = !string.IsNullOrWhiteSpace(claim.ClID);
            btnGetClaimStatus.Enabled =
                !string.IsNullOrWhiteSpace(claim.CardNo) &&
                !string.IsNullOrWhiteSpace(claim.ClID);
        }

        protected void btnRePrintLOA_Click(object sender, EventArgs e)
        {
            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowInformationHeader("Data eligibility tidak ditemukan.");
                return;
            }

            if (string.IsNullOrWhiteSpace(claim.ClID))
            {
                ShowInformationHeader("ClID tidak tersedia.");
                return;
            }

            var service = new AdmedikaService();

            try
            {
                var result = service.RePrintOut(claim.ClID, AdmedikaConstant.PrintTypeID.LOA);

                string message;
                if (!service.IsOk(result, out message))
                {
                    ShowInformationHeader(message);
                    return;
                }

                if (string.IsNullOrWhiteSpace(result.PrintOut))
                {
                    ShowInformationHeader("Response RePrintOut tidak berisi LOA.");
                    return;
                }

                claim.PrintOutLOA = result.PrintOut;
                claim.PrintOut = result.PrintOut;
                claim.Save();

                LoadDetail();
                ShowInformationHeader("Re-request LOA berhasil.");
            }
            catch (Exception ex)
            {
                ShowInformationHeader(ex.Message);
            }
        }

        protected void btnGetClaimStatus_Click(object sender, EventArgs e)
        {
            lblClaimStatusMessage.Text = string.Empty;

            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                SetClaimStatusMessage("Data eligibility tidak ditemukan.", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(claim.CardNo) ||
                string.IsNullOrWhiteSpace(claim.ClID))
            {
                SetClaimStatusMessage("Card No atau ClID tidak tersedia.", true);
                return;
            }

            var service = new AdmedikaService();

            try
            {
                var result = service.CheckMemberClaimTxnStatus(claim.CardNo, claim.ClID);

                string message;
                if (!service.IsOk(result, out message))
                {
                    SetClaimStatusMessage(message, true);
                    return;
                }

                claim.ClStatus = result.ClStatus;
                claim.ClDesc = result.ClDesc;
                claim.Save();

                LoadDetail();
                SetClaimStatusMessage("Check Claim Status berhasil.", false);
            }
            catch (Exception ex)
            {
                SetClaimStatusMessage(ex.Message, true);
            }
        }

        private void SetClaimStatusMessage(string message, bool isError)
        {
            lblClaimStatusMessage.Text = message;
            lblClaimStatusMessage.ForeColor = isError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.Green;
        }

        protected void btnPrintLOA_Click(object sender, EventArgs e)
        {
            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID) ||
                string.IsNullOrWhiteSpace(claim.ClID))
            {
                ShowInformationHeader("ClaimID tidak tersedia.");
                return;
            }

            var jobParameters = new PrintJobParameterCollection();
            var claimParameter = jobParameters.AddNew();
            claimParameter.Name = "ClaimID";
            claimParameter.ValueString = claim.ClID;

            AppSession.PrintJobReportID = AppConstant.Report.AdmedikaPrintLOA;
            AppSession.PrintJobParameters = jobParameters;

            string script = @"var oWnd = $find('" + winPrint.ClientID + "');" +
                            "oWnd.SetUrl('" + Page.ResolveUrl("~/Module/Reports/ReportViewer.aspx") + "');" +
                            "oWnd.Show();" +
                            "oWnd.Maximize();";

            Helper.RegisterStartupScript(this, "printLOA", script);
        }

        private static string Value(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static string FormatDateTime(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MM/dd/yyyy HH:mm") : "-";
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MM/dd/yyyy") : "-";
        }

        private static string BuildPrintOutHtml(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "<div style=\"padding:8px;border:1px solid #ccc;\">Belum ada Print Out LOA.</div>";

            return "<pre style=\"white-space:pre-wrap;border:1px solid #ccc;padding:8px;max-height:500px;overflow:auto;\">" +
                   HttpUtility.HtmlEncode(value) +
                   "</pre>";
        }
    }
}
