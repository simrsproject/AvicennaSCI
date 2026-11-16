using System;
using System.Web;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaPrintOut : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaDischargeOp;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            long id;
            if (!long.TryParse(Request.QueryString["id"], out id))
            {
                ShowEmpty("Invalid transaction ID.");
                return;
            }

            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(id))
            {
                ShowEmpty("AdMedika transaction was not found.");
                return;
            }

            var printType = (Request.QueryString["type"] ?? AdmedikaConstant.PrintTypeID.LOC)
                .Trim().ToUpperInvariant();
            if (printType == AdmedikaConstant.PrintTypeID.LOC)
            {
                if (string.IsNullOrWhiteSpace(claim.ClID) || string.IsNullOrWhiteSpace(claim.PrintOutLOC))
                {
                    ShowEmpty("LOC belum tersedia untuk claim ini.");
                    return;
                }

                var parameters = new PrintJobParameterCollection();
                var parameter = parameters.AddNew();
                parameter.Name = "ClaimID";
                parameter.ValueString = claim.ClID;
                AppSession.PrintJobReportID = AppConstant.Report.AdmedikaPrintLOC;
                AppSession.PrintJobParameters = parameters;
                Response.Redirect(ResolveUrl("~/Module/Reports/ReportViewer.aspx"), false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
            string printOut;

            switch (printType)
            {
                case AdmedikaConstant.PrintTypeID.LOA:
                    lblTitle.Text = "LETTER OF AUTHORIZATION (LOA)";
                    printOut = claim.PrintOutLOA;
                    break;
                case AdmedikaConstant.PrintTypeID.LOR:
                    lblTitle.Text = "LETTER OF REJECTION (LOR)";
                    printOut = claim.PrintOutLOR;
                    break;
                default:
                    lblTitle.Text = "LETTER OF CONFIRMATION (LOC)";
                    printOut = claim.PrintOutLOC;
                    break;
            }

            lblClaimID.Text = HttpUtility.HtmlEncode(claim.ClID ?? string.Empty);
            lblRegistrationNo.Text = HttpUtility.HtmlEncode(claim.RegistrationNo ?? string.Empty);
            lblPatientName.Text = HttpUtility.HtmlEncode(claim.PatientName ?? string.Empty);
            lblCardNo.Text = HttpUtility.HtmlEncode(AdmedikaMask.CardNo(claim.CardNo) ?? string.Empty);

            if (string.IsNullOrWhiteSpace(printOut))
                ShowEmpty("Print out is not available.");
            else
                litPrintOut.Text = "<pre>" + HttpUtility.HtmlEncode(printOut) + "</pre>";
        }

        private void ShowEmpty(string message)
        {
            litPrintOut.Text = "<div style=\"padding:12px;border:1px solid #d9534f;color:#a94442;\">" +
                               HttpUtility.HtmlEncode(message) + "</div>";
        }
    }
}
