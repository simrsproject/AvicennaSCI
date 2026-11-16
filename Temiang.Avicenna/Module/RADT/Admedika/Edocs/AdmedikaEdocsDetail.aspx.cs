using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEdocsDetail : BasePageDetail
    {
        protected long ClaimTransactionID
        {
            get
            {
                long id;
                long.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        private bool IsEditMode
        {
            get
            {
                return string.Equals(Request.QueryString["md"], "edit", StringComparison.OrdinalIgnoreCase);
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEdocs;
            UrlPageList = "AdmedikaEdocsList.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadDetail();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Enabled = false;
            ToolBarMenuEdit.Enabled = false;
            ToolBarMenuDelete.Enabled = false;
            ToolBarMenuSave.Enabled = false;
            ToolBarMenuSearch.Enabled = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
            ToolBarMenuApproval.Visible = false;
            ToolBarMenuUnApproval.Visible = false;
            ToolBarMenuVoid.Visible = false;
            ToolBarMenuUnVoid.Visible = false;

            btnUpload.Visible = IsEditMode;
        }

        private void LoadDetail()
        {
            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowInformationHeader("Data klaim tidak ditemukan.");
                btnUpload.Enabled = false;
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
            lblIsClosed.Text = claim.IsClosed == true ? "Yes" : "No";

            lblCardNo.Text = Value(claim.CardNo);
            lblPatientID.Text = Value(claim.PatientID);
            lblPatientName.Text = Value(claim.PatientName);
            lblMedicalNo.Text = Value(claim.MedicalNo);

            lblRegistrationNo.Text = Value(claim.RegistrationNo);
            lblRegistrationDate.Text = registrationLoaded ? FormatDate(registration.RegistrationDate) : "-";
            lblGuarantor.Text = registrationLoaded
                ? Value(registration.GuarantorID) + (guarantorLoaded ? " / " + Value(guarantor.GuarantorName) : string.Empty)
                : "-";
            lblIsVoid.Text = registrationLoaded ? (registration.IsVoid == true ? "Yes" : "No") : "-";

            btnUpload.Enabled = IsEditMode;
        }

        protected void grdDocuments_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdDocuments.DataSource = DocumentsTable;
        }

        protected void grdDocuments_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item == null) return;

            var row = item.DataItem as DataRowView;
            if (row == null) return;

            var isSent = row[AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess];
            if (isSent == DBNull.Value)
                item["Status"].BackColor = Color.FromArgb(255, 249, 196);
            else if (Convert.ToBoolean(isSent))
                item["Status"].BackColor = Color.FromArgb(212, 237, 218);
            else
                item["Status"].BackColor = Color.FromArgb(248, 215, 218);
        }

        protected void grdDocuments_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "SendDocument") return;
            if (!IsEditMode) return;

            long documentID;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out documentID)) return;

            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowInformationHeader("Data klaim tidak ditemukan.");
                return;
            }

            var document = new AdmedikaDocument();
            if (!document.LoadByPrimaryKey(documentID))
            {
                ShowInformationHeader("Dokumen tidak ditemukan.");
                return;
            }

            bool success;
            var message = AdmedikaEdocsHelper.Send(document, claim, out success);

            grdDocuments.Rebind();
            ShowInformationHeader(message);
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            if (sourceControl == grdDocuments && !string.IsNullOrEmpty(eventArgument) &&
                (eventArgument == "rebind" || eventArgument == "uploaded" || eventArgument == "sent"))
            {
                grdDocuments.Rebind();

                if (eventArgument == "uploaded")
                    ShowInformationHeader("Dokumen berhasil di-upload, belum dikirim.");
                else if (eventArgument == "sent")
                    ShowInformationHeader("Dokumen berhasil dikirim.");

                return;
            }

            base.RaisePostBackEvent(sourceControl, eventArgument);
        }

        protected string BuildDocTypeHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var code = Convert.ToString(row[AdmedikaDocumentMetadata.ColumnNames.DocType]);
            var docType = AdmedikaDocType.Get(code);

            var html = "<b>" + Server.HtmlEncode(code) + "</b>";

            if (docType != null && !string.IsNullOrWhiteSpace(docType.Description))
            {
                if (html.Length > 0) html += "<br />";
                html += Server.HtmlEncode(docType.Description);
            }

            return html;
        }

        protected string GetStatusText(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var isSent = row[AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess];
            if (isSent == DBNull.Value) return "Not Sent";

            return Convert.ToBoolean(isSent) ? "Sent" : "Failed";
        }

        protected string GetSendText(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var isSent = row[AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess];
            return isSent == DBNull.Value ? "Send" : "Re-send";
        }

        protected bool IsSendVisible(object dataItem)
        {
            if (!IsEditMode) return false;

            var row = dataItem as DataRowView;
            if (row == null) return false;

            var isSent = row[AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess];
            return isSent == DBNull.Value || !Convert.ToBoolean(isSent);
        }

        private DataTable DocumentsTable
        {
            get
            {
                var query = new AdmedikaDocumentQuery("a");
                query.es.WithNoLock = true;

                query.Select(query);
                query.Where(query.AdmedikaClaimTransactionID == ClaimTransactionID);
                query.OrderBy(query.CreatedDateTime.Descending, query.AdmedikaDocumentID.Descending);

                return query.LoadDataTable();
            }
        }

        private static string Value(object value)
        {
            return value == null ? string.Empty : Convert.ToString(value);
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue ? value.Value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : "-";
        }
    }
}
