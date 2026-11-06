using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;
using AdmedikaService = Temiang.Avicenna.Common.Admedika.Service;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEligibilityDialog : BasePageDialog
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEligibility;

            ButtonOk.Visible = false;
            ButtonCancel.Text = "Close";

            if (IsPostBack) return;

            BindCoverageTypes();
        }

        private void BindCoverageTypes()
        {
            rblCoverageType.Items.Clear();

            foreach (var coverage in AdmedikaCoverageType.All)
            {
                rblCoverageType.Items.Add(new ListItem(
                    string.Format("{0} ({1})", coverage.CoverageCode, coverage.Description),
                    coverage.CoverageID));
            }

            rblCoverageType.SelectedValue = AdmedikaConstant.CoverageID.GP;
        }

        protected void btnCheckBenefit_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            HideInformationHeader();
            var service = new AdmedikaService();

            try
            {
                var coverage = AdmedikaCoverageType.Get(rblCoverageType.SelectedValue);
                var searchForTermCondition = coverage == null ? string.Empty : coverage.Description;

                var result = service.GetMemberEnrolledPlanTc(
                    txtCardNo.Text.Trim(),
                    rblCoverageType.SelectedValue,
                    searchForTermCondition);

                string message;
                if (!service.IsOk(result, out message))
                {
                    litBenefit.Text = BuildErrorHtml(result, message);
                    ShowInformationHeader(message);
                    return;
                }

                litBenefit.Text = BuildBenefitHtml(result);
            }
            catch (Exception ex)
            {
                litBenefit.Text = BuildErrorHtml(null, ex.Message);
                ShowInformationHeader(ex.Message);
            }
        }

        protected void btnProcessEligibility_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            HideInformationHeader();
            var service = new AdmedikaService();
            var covID = rblCoverageType.SelectedValue;
            var diagnosisCodeList = txtDiagnosisCodeList.Text.Trim();

            try
            {
                var result = service.Eligibility(
                    txtCardNo.Text.Trim(),
                    covID,
                    diagnosisCodeList,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    string.Empty,
                    txtRemarks.Text.Trim());

                string message;
                if (!service.IsOk(result, out message))
                {
                    litEligibilityResponse.Text = BuildErrorHtml(result, message);
                    ShowInformationHeader(message);
                    return;
                }

                litEligibilityResponse.Text = BuildEligibilityHtml(result);

                try
                {
                    SaveEligibility(result, covID, diagnosisCodeList);
                    litEligibilityResponse.Text = BuildSuccessHtml("Eligibility berhasil disimpan.") + litEligibilityResponse.Text;
                    btnProcessEligibility.Enabled = false;
                    ShowInformationHeader("Eligibility berhasil disimpan.");
                }
                catch (Exception ex)
                {
                    litEligibilityResponse.Text += BuildErrorHtml(null, "Gagal menyimpan: " + ex.Message);
                    ShowInformationHeader("Eligibility berhasil diproses, tetapi gagal disimpan.");
                }
            }
            catch (Exception ex)
            {
                litEligibilityResponse.Text = BuildErrorHtml(null, ex.Message);
                ShowInformationHeader(ex.Message);
            }
        }

        private bool ValidateInput()
        {
            if (!Page.IsValid) return false;

            if (string.IsNullOrWhiteSpace(txtCardNo.Text))
            {
                ShowInformationHeader("Card No required.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(rblCoverageType.SelectedValue))
            {
                ShowInformationHeader("Coverage required.");
                return false;
            }

            return true;
        }

        private void SaveEligibility(EligibilityResponse response, string covID, string diagnosisCodeList)
        {
            var entity = new AdmedikaClaimTransaction();

            if (!string.IsNullOrEmpty(response.ClID))
            {
                entity.Query.es.Top = 1;
                entity.Query.Where(entity.Query.ClID == response.ClID);

                if (!entity.Query.Load())
                    entity.AddNew();
            }
            else
            {
                entity.AddNew();
            }

            var coverage = AdmedikaCoverageType.Get(covID);

            entity.CardNo = string.IsNullOrEmpty(response.CardNo) ? txtCardNo.Text.Trim() : response.CardNo;
            entity.ClID = response.ClID;
            entity.CovID = covID;
            entity.CovCode = coverage == null ? null : coverage.CoverageCode;
            entity.PlanCode = response.PlanCode;
            entity.DiagnosisCodeList = string.IsNullOrEmpty(diagnosisCodeList) ? null : diagnosisCodeList;
            entity.ClStatus = response.ClStatus;
            entity.ClDesc = response.ClDesc;
            entity.ServiceID = AdmedikaConstant.ServiceID.Eligibility;
            entity.PrintOut = response.PrintOut;
            entity.PrintOutLOA = response.PrintOut;
            entity.PatientName = response.PatientName;
            entity.PolicyNo = response.PolicyNo;
            entity.PayorMemberID = response.PayorMemberID;
            entity.PolicyDate = response.PolicyDate;

            DateTime dateOfBirth;
            entity.DateOfBirth = DateTime.TryParse(response.Dob, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dateOfBirth)
                ? dateOfBirth
                : (DateTime?)null;

            if (entity.es.IsAdded)
                entity.IsClosed = false;

            entity.Save();
        }

        private static string BuildBenefitHtml(GetMemberEnrolledPlanTcResponse response)
        {
            var html = new StringBuilder();

            html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\">");
            AppendSummaryRow(html, "Card No", response.CardNo);
            AppendSummaryRow(html, "Patient Name", response.PatientName);
            AppendSummaryRow(html, "Date of Birth", response.Dob);
            AppendSummaryRow(html, "Payor", response.PayorName);
            AppendSummaryRow(html, "Coverage", JoinValue(response.CovID, response.CovCode, " / "));
            html.Append("</table>");

            if (response.EnrolledPlan == null || response.EnrolledPlan.Count == 0)
            {
                html.Append("<div style=\"margin-top:8px;\">Tidak ada plan terdaftar.</div>");
                return html.ToString();
            }

            foreach (var plan in response.EnrolledPlan)
            {
                html.Append("<div style=\"margin-top:10px;font-weight:bold;\">");
                html.Append(Encode(plan.CovCode));
                html.Append(" - ");
                html.Append(Encode(plan.PlanID));
                html.Append("</div>");
                html.Append("<div style=\"margin-bottom:4px;\">").Append(Encode(plan.Desc)).Append("</div>");

                if (plan.TermCondition == null || plan.TermCondition.Count == 0)
                {
                    html.Append("<div>Tidak ada term &amp; condition.</div>");
                    continue;
                }

                html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\" border=\"1\" cellpadding=\"4\" cellspacing=\"0\">");
                html.Append("<thead><tr style=\"background:#f0f0f0;\">");
                html.Append("<th align=\"left\">Term &amp; Condition</th>");
                html.Append("<th align=\"center\" style=\"width:80px;\">Guarantee</th>");
                html.Append("<th align=\"left\">Remarks</th>");
                html.Append("</tr></thead><tbody>");

                foreach (var term in plan.TermCondition)
                {
                    html.Append("<tr>");
                    html.Append("<td>").Append(Encode(term.TcDesc)).Append("</td>");
                    html.Append("<td align=\"center\">").Append(Encode(term.GuaranteeFlag)).Append("</td>");
                    html.Append("<td>").Append(Encode(term.Remarks)).Append("</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody></table>");
            }

            return html.ToString();
        }

        private static string BuildEligibilityHtml(EligibilityResponse response)
        {
            var html = new StringBuilder();

            html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\">");
            AppendSummaryRow(html, "ClID", response.ClID);
            AppendSummaryRow(html, "Status", response.ClStatus);
            AppendSummaryRow(html, "Description", response.ClDesc);
            AppendSummaryRow(html, "Patient Name", response.PatientName);
            AppendSummaryRow(html, "Date of Birth", response.Dob);
            AppendSummaryRow(html, "Payor", JoinValue(response.PayorName, response.PayorCode, " / "));
            AppendSummaryRow(html, "Plan Code", response.PlanCode);
            AppendSummaryRow(html, "Card No", response.CardNo);
            AppendSummaryRow(html, "Policy No", response.PolicyNo);
            AppendSummaryRow(html, "Policy Date", response.PolicyDate);
            AppendSummaryRow(html, "Payor Member ID", response.PayorMemberID);
            AppendSummaryRow(html, "Principal Name", response.PrincipalName);
            AppendSummaryRow(html, "Previous ICD Code", response.PreviousIcdCode);
            AppendSummaryRow(html, "Referral Info Flag", response.ReferralInfoFlag);
            html.Append("</table>");

            if (response.Entitlement != null && response.Entitlement.Count > 0)
            {
                html.Append("<div style=\"margin-top:10px;font-weight:bold;\">Entitlement</div>");
                html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\" border=\"1\" cellpadding=\"4\" cellspacing=\"0\">");
                html.Append("<thead><tr style=\"background:#f0f0f0;\">");
                html.Append("<th align=\"left\">Benefit</th>");
                html.Append("<th align=\"left\">Short Name</th>");
                html.Append("<th align=\"left\">Long Name</th>");
                html.Append("<th align=\"right\" style=\"width:100px;\">Available Limit</th>");
                html.Append("<th align=\"left\" style=\"width:120px;\">Frequency</th>");
                html.Append("<th align=\"left\">Limit Description</th>");
                html.Append("</tr></thead><tbody>");

                foreach (var benefit in response.Entitlement)
                {
                    html.Append("<tr>");
                    html.Append("<td>").Append(Encode(benefit.BenID)).Append("</td>");
                    html.Append("<td>").Append(Encode(benefit.ShortName)).Append("</td>");
                    html.Append("<td>").Append(Encode(benefit.LongName)).Append("</td>");
                    html.Append("<td align=\"right\">").Append(Encode(benefit.AvailLimit)).Append("</td>");
                    html.Append("<td>").Append(Encode(benefit.FreqDesc)).Append("</td>");
                    html.Append("<td>").Append(Encode(benefit.LimitDesc)).Append("</td>");
                    html.Append("</tr>");
                }

                html.Append("</tbody></table>");
            }

            //if (!string.IsNullOrWhiteSpace(response.PrintOut))
            //{
            //    html.Append("<div style=\"margin-top:10px;font-weight:bold;\">Print Out (LOA)</div>");
            //    html.Append("<pre style=\"white-space:pre-wrap;border:1px solid #ccc;padding:6px;font-size:12px;\">");
            //    html.Append(Encode(response.PrintOut));
            //    html.Append("</pre>");
            //}

            return html.ToString();
        }

        private static string BuildErrorHtml(AdmedikaResponse response, string message)
        {
            var html = new StringBuilder();

            html.Append("<div style=\"border:1px solid #d9534f;background:#f2dede;color:#a94442;padding:8px;margin-bottom:8px;\">");
            html.Append(Encode(string.IsNullOrEmpty(message) ? "Permintaan AdMedika gagal." : message));
            html.Append("</div>");

            if (response != null)
            {
                html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\">");
                AppendSummaryRow(html, "Reference ID", response.ReferenceID);
                AppendSummaryRow(html, "Status Code", response.StatusCode);
                AppendSummaryRow(html, "Status Message", response.StatusMsg);
                AppendSummaryRow(html, "Error Code", response.ErrorCode);
                AppendSummaryRow(html, "Error Description", response.ErrorDesc);
                AppendSummaryRow(html, "Technical Error", response.TechnicalError);
                html.Append("</table>");
            }

            return html.ToString();
        }

        private static string BuildSuccessHtml(string message)
        {
            return string.Concat(
                "<div style=\"border:1px solid #3c763d;background:#dff0d8;color:#3c763d;padding:8px;margin-bottom:8px;\">",
                Encode(message),
                "</div>");
        }

        private static void AppendSummaryRow(StringBuilder html, string label, string value)
        {
            html.Append("<tr>");
            html.Append("<td style=\"width:35%;padding:4px;border-bottom:1px solid #eee;font-weight:bold;vertical-align:top;\">");
            html.Append(Encode(label));
            html.Append("</td>");
            html.Append("<td style=\"padding:4px;border-bottom:1px solid #eee;vertical-align:top;\">");
            html.Append(Encode(value));
            html.Append("</td>");
            html.Append("</tr>");
        }

        private static string JoinValue(string left, string right, string separator)
        {
            if (string.IsNullOrWhiteSpace(left)) return right ?? string.Empty;
            if (string.IsNullOrWhiteSpace(right)) return left;
            return left + separator + right;
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
