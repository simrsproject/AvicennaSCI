using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEligibilityList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            UrlPageDetail = "AdmedikaEligibilityDetail.aspx";
            ProgramID = AppConstant.Program.AdmedikaEligibility;

            if (IsPostBack) return;

            txtCreatedDate.SelectedDate = DateTime.Now.Date;
            txtCreatedDateTo.SelectedDate = DateTime.Now.Date;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            // Halaman ini hanya membaca data. Eligibility baru dibuat lewat dialog.
            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;

            // Filter pencarian sudah tersedia inline, tidak memakai halaman search terpisah.
            ToolBarMenuSearch.Visible = false;

            btnNewEligibility.Visible = IsUserAddAble;
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string id = dataItem.GetDataKeyValue(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID).ToString();
            Page.Response.Redirect("AdmedikaEligibilityDetail.aspx?md=" + mode + "&id=" + id, true);
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = EligibilityTable;
        }

        protected void grdList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item == null) return;

            var row = item.DataItem as DataRowView;
            if (row == null) return;

            if (string.IsNullOrEmpty(Convert.ToString(row["PatientID"])))
                item["PatientID"].BackColor = System.Drawing.Color.Yellow;

            if (string.IsNullOrEmpty(Convert.ToString(row["RegistrationNo"])))
                item["RegistrationNo"].BackColor = System.Drawing.Color.Yellow;
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);

            if (sourceControl == grdList && !string.IsNullOrEmpty(eventArgument) &&
                eventArgument.StartsWith("selectpatient!", StringComparison.Ordinal))
            {
                var parts = eventArgument.Split('!');
                if (parts.Length >= 3 && !string.IsNullOrEmpty(parts[2]))
                {
                    long transactionID;
                    if (long.TryParse(parts[1], out transactionID))
                    {
                        var entity = new AdmedikaClaimTransaction();
                        if (entity.LoadByPrimaryKey(transactionID))
                        {
                            entity.PatientID = parts[2];
                            entity.Save();
                        }
                    }
                }

                grdList.Rebind();
            }
        }

        protected string BuildPatientIdHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var patientId = Convert.ToString(row["PatientID"]);
            var registrationNo = Convert.ToString(row["RegistrationNo"]);
            var refPatientName = Convert.ToString(row["RefPatientName"]);

            string html;

            if (!string.IsNullOrEmpty(registrationNo))
            {
                html = "<b>" + Server.HtmlEncode(patientId) + "</b>";
            }
            else
            {
                var claimId = Convert.ToString(row["AdmedikaClaimTransactionID"]);
                var cardNo = Convert.ToString(row["CardNo"]);
                var patientName = Convert.ToString(row["PatientName"]);

                var args = string.Format("'{0}','{1}','{2}'",
                    HttpUtility.JavaScriptStringEncode(claimId),
                    HttpUtility.JavaScriptStringEncode(cardNo),
                    HttpUtility.JavaScriptStringEncode(patientName));

                html = string.IsNullOrEmpty(patientId)
                    ? string.Format("<b><a href=\"#\" onclick=\"openSelectPatient({0}); return false;\">Pilih Patient</a></b>", args)
                    : string.Format("<b><a href=\"#\" onclick=\"openSelectPatient({0}); return false;\" title=\"Change Patient\">{1}</a></b>",
                        args, Server.HtmlEncode(patientId));
            }

            if (!string.IsNullOrEmpty(refPatientName))
            {
                if (html.Length > 0) html += "<br />";
                html += Server.HtmlEncode(refPatientName);
            }

            return html;
        }

        protected string BuildCovCodeHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var covCode = Convert.ToString(row["CovCode"]);
            var covID = Convert.ToString(row["CovID"]);
            var coverage = AdmedikaCoverageType.Get(covID);

            var html = Server.HtmlEncode(covCode);

            if (coverage != null && !string.IsNullOrWhiteSpace(coverage.Description))
            {
                if (html.Length > 0) html += "<br />";
                html += Server.HtmlEncode(coverage.Description);
            }

            return html;
        }

        protected string BuildActionHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var registrationNo = Convert.ToString(row["RegistrationNo"]);
            var patientId = Convert.ToString(row["PatientID"]);
            var isClosed = row["IsClosed"] != DBNull.Value && Convert.ToBoolean(row["IsClosed"]);

            var html = string.Empty;

            if (!string.IsNullOrEmpty(patientId) && string.IsNullOrEmpty(registrationNo) && !isClosed)
            {
                html += "<a href=\"#\" onclick=\"return false;\" title=\"Process To Registration\" style=\"text-decoration:none;white-space:nowrap;\">";
                html += "<img src=\"../../../../Images/Toolbar/process16.png\" border=\"0\" alt=\"Process To Registration\" title=\"Process To Registration\" style=\"vertical-align:middle;\" />";
                html += " Process To Registration</a>&nbsp;&nbsp;";
            }

            return html;    
        }

        protected void grdList_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "PrintLOA")
            {
                var jobParameters = new PrintJobParameterCollection();
                var claimParameter = jobParameters.AddNew();

                var claimID = e.CommandArgument.ToString();
                claimParameter.Name = "ClaimID";
                claimParameter.ValueString = claimID;

                AppSession.PrintJobReportID = AppConstant.Report.AdmedikaPrintLOA;
                AppSession.PrintJobParameters = jobParameters;

                string script = @"var oWnd = $find('" + winPrint.ClientID + "');" +
                                "oWnd.SetUrl('" + Page.ResolveUrl("~/Module/Reports/ReportViewer.aspx") + "');" +
                                "oWnd.Show();" +
                                "oWnd.Maximize();";
                RadAjaxPanel1.ResponseScripts.Add(script);

            }
        }

        private DataTable EligibilityTable
        {
            get
            {
                var query = new AdmedikaClaimTransactionQuery("a");
                var patient = new PatientQuery("b");

                query.es.WithNoLock = true;
                patient.es.WithNoLock = true;

                query.LeftJoin(patient).On(patient.PatientID == query.PatientID);

                query.Select(
                    query.AdmedikaClaimTransactionID,
                    query.CreatedDateTime,
                    query.RegistrationNo,
                    query.PatientID,
                    query.PatientName,
                    query.CardNo,
                    query.ClID,
                    query.CovID,
                    query.CovCode,
                    query.ClStatus,
                    query.ClDesc,
                    query.IsClosed,
                    "<RTRIM(LTRIM(ISNULL(b.FirstName, '') + ' ' + ISNULL(b.MiddleName, '') + ' ' + ISNULL(b.LastName, ''))) AS RefPatientName>"
                );

                if (!txtCreatedDate.IsEmpty)
                    query.Where(query.CreatedDateTime >= txtCreatedDate.SelectedDate.Value.Date);

                if (!txtCreatedDateTo.IsEmpty)
                    query.Where(query.CreatedDateTime < txtCreatedDateTo.SelectedDate.Value.Date.AddDays(1));

                if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                    query.Where(query.RegistrationNo == txtRegistrationNo.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                    query.Where(query.PatientName.Like("%" + Helper.EscapeQuery(txtPatientName.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtCardNo.Text))
                    query.Where(query.CardNo.Like("%" + Helper.EscapeQuery(txtCardNo.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtClID.Text))
                    query.Where(query.ClID.Like("%" + Helper.EscapeQuery(txtClID.Text) + "%"));

                query.OrderBy(query.CreatedDateTime.Descending);
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                return query.LoadDataTable();
            }
        }
    }
}
