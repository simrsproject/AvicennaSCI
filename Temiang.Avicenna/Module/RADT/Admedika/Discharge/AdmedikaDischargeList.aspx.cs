using System;
using System.Data;
using System.Web;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaDischargeList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaDischargeOp;

            if (IsPostBack) return;

            txtDateFrom.SelectedDate = DateTime.Now.Date;
            txtDateTo.SelectedDate = DateTime.Now.Date;

            cboClaimStatus.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
            foreach (var status in AdmedikaClaimStatus.All)
                cboClaimStatus.Items.Add(new RadComboBoxItem(
                    status.Code + " - " + status.Description, status.Code));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsInpatientRoute) return;

            Title = "Discharge AdMedika InPatient - Not Available";
            pnlUnavailable.Visible = true;
            CollapsePanel1.Visible = false;
            grdList.Visible = false;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;
            ToolBarMenuView.Visible = false;
            ToolBarMenuSearch.Visible = false;
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            grdList.CurrentPageIndex = 0;
            grdList.Rebind();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = IsInpatientRoute ? new DataTable() : DischargeTable;
        }

        private bool IsInpatientRoute
        {
            get
            {
                return string.Equals(Request.QueryString["rt"], "IPR",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        protected override void RaisePostBackEvent(System.Web.UI.IPostBackEventHandler sourceControl,
            string eventArgument)
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);

            if (sourceControl == grdList && eventArgument == "rebind")
                grdList.Rebind();
        }

        protected string BuildStatusHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var status = Convert.ToString(row["ClStatus"]);
            var description = Convert.ToString(row["ClDesc"]);
            var color = "#555";
            var background = "#f5f5f5";

            if (status == AdmedikaConstant.ClaimStatusID.ValidClaims)
            {
                color = "#3c763d";
                background = "#dff0d8";
            }
            else if (status == AdmedikaConstant.ClaimStatusID.ClaimsOnHold ||
                     status == AdmedikaConstant.ClaimStatusID.IncompleteDocuments)
            {
                color = "#8a6d3b";
                background = "#fcf8e3";
            }
            else if (status == AdmedikaConstant.ClaimStatusID.ClaimsRejected ||
                     status == AdmedikaConstant.ClaimStatusID.ReimbursementDenied)
            {
                color = "#a94442";
                background = "#f2dede";
            }

            return string.Format(
                "<span style=\"display:inline-block;padding:3px 6px;color:{0};background:{1};border-radius:3px;\"><b>{2}</b> {3}</span>",
                color, background, Server.HtmlEncode(status), Server.HtmlEncode(description));
        }

        protected string BuildActionHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var id = Convert.ToString(row["AdmedikaClaimTransactionID"]);
            var status = Convert.ToString(row["ClStatus"]);
            var hasLoc = row["HasLOC"] != DBNull.Value && Convert.ToBoolean(row["HasLOC"]);
            var actionText = status == AdmedikaConstant.ClaimStatusID.ValidClaims
                ? "View"
                : "Process";

            var html = string.Format(
                "<a href=\"#\" onclick=\"openDischarge('{0}');return false;\" title=\"{1}\" style=\"display:inline-block;white-space:nowrap;text-decoration:none;background:url('{2}') no-repeat left center;padding-left:20px;line-height:20px;\">{1}</a>",
                HttpUtility.JavaScriptStringEncode(id), actionText,
                ResolveUrl("~/Images/Toolbar/" + (actionText == "View" ? "views16.png" : "process16.png")));

            if (hasLoc)
            {
                html += string.Format(
                    "&nbsp;&nbsp;<a href=\"#\" onclick=\"printLOC('{0}');return false;\" title=\"Print LOC\" style=\"display:inline-block;white-space:nowrap;text-decoration:none;background:url('{1}') no-repeat left center;padding-left:20px;line-height:20px;\">Print LOC</a>",
                    HttpUtility.JavaScriptStringEncode(id), ResolveUrl("~/Images/Toolbar/print16.png"));
            }

            return html;
        }

        private DataTable DischargeTable
        {
            get
            {
                var claim = new AdmedikaClaimTransactionQuery("a");
                var registration = new RegistrationQuery("b");
                var patient = new PatientQuery("c");
                var guarantor = new GuarantorQuery("d");

                claim.es.WithNoLock = true;
                registration.es.WithNoLock = true;
                patient.es.WithNoLock = true;
                guarantor.es.WithNoLock = true;

                claim.InnerJoin(registration).On(claim.RegistrationNo == registration.RegistrationNo);
                claim.LeftJoin(patient).On(registration.PatientID == patient.PatientID);
                claim.LeftJoin(guarantor).On(registration.GuarantorID == guarantor.GuarantorID);

                claim.Select(
                    claim.AdmedikaClaimTransactionID,
                    registration.RegistrationDate,
                    claim.RegistrationNo,
                    patient.MedicalNo,
                    patient.FullName.As("PatientName"),
                    guarantor.GuarantorName,
                    claim.CardNo,
                    claim.ClID,
                    claim.ClStatus,
                    claim.ClDesc,
                    claim.TotAmtApproved,
                    claim.TotAmtNotApproved,
                    claim.IsClosed,
                    claim.DischargeDateTime,
                    "<CASE WHEN ISNULL(a.PrintOutLOC, '') = '' THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS HasLOC>"
                );

                claim.Where(
                    claim.CovID == AdmedikaConstant.CoverageID.GP,
                    registration.SRRegistrationType == AppConstant.RegistrationType.OutPatient,
                    registration.IsVoid == false);

                if (!txtDateFrom.IsEmpty)
                    claim.Where(registration.RegistrationDate >= txtDateFrom.SelectedDate.Value.Date);

                if (!txtDateTo.IsEmpty)
                    claim.Where(registration.RegistrationDate < txtDateTo.SelectedDate.Value.Date.AddDays(1));

                if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                    claim.Where(claim.RegistrationNo.Like("%" +
                        Helper.EscapeQuery(txtRegistrationNo.Text.Trim()) + "%"));

                if (!string.IsNullOrWhiteSpace(txtPatient.Text))
                {
                    var value = "%" + Helper.EscapeQuery(txtPatient.Text.Trim()) + "%";
                    claim.Where(claim.Or(patient.FullName.Like(value), patient.MedicalNo.Like(value)));
                }

                if (!string.IsNullOrWhiteSpace(txtClaim.Text))
                {
                    var value = "%" + Helper.EscapeQuery(txtClaim.Text.Trim()) + "%";
                    claim.Where(claim.Or(claim.CardNo.Like(value), claim.ClID.Like(value)));
                }

                if (!string.IsNullOrWhiteSpace(cboClaimStatus.SelectedValue))
                    claim.Where(claim.ClStatus == cboClaimStatus.SelectedValue);

                claim.OrderBy(registration.RegistrationDate.Descending);
                claim.es.Top = AppSession.Parameter.MaxResultRecord;

                var table = claim.LoadDataTable();

                return table;
            }
        }
    }
}
