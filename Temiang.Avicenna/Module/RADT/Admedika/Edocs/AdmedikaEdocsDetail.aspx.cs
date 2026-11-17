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

        private bool IsListEditMode
        {
            get
            {
                return string.Equals(Request.QueryString["md"], "edit", StringComparison.OrdinalIgnoreCase);
            }
        }

        private bool IsEditMode
        {
            get { return DataModeCurrent != AppEnum.DataMode.Read; }
        }

        private AdmedikaClaimTransaction _currentClaim;

        private AdmedikaClaimTransaction CurrentClaim
        {
            get
            {
                if (_currentClaim == null)
                {
                    var claim = new AdmedikaClaimTransaction();
                    if (claim.LoadByPrimaryKey(ClaimTransactionID))
                        _currentClaim = claim;
                }

                return _currentClaim;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEdocs;
            UrlPageList = "AdmedikaEdocsList.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Tombol Edit di list membuka halaman dalam mode edit.
            if (IsListEditMode && DataModeCurrent == AppEnum.DataMode.Read)
                DataModeCurrent = AppEnum.DataMode.Edit;

            LoadDetail();
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            base.OnDataModeChanged(oldVal, newVal);

            // Tombol Send/Re-send pada grid mengikuti mode, jadi grid perlu di-rebind.
            if (grdDocuments != null)
                grdDocuments.Rebind();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Enabled = false;
            ToolBarMenuDelete.Enabled = false;
            ToolBarMenuSave.Visible = false;
            ToolBarMenuSearch.Enabled = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
            ToolBarMenuApproval.Visible = false;
            ToolBarMenuUnApproval.Visible = false;
            ToolBarMenuVoid.Visible = false;
            ToolBarMenuUnVoid.Visible = false;

            var hasClaim = CurrentClaim != null;
            btnUpload.Visible = IsEditMode && hasClaim;
            btnUpload.Enabled = btnUpload.Visible;
        }

        private void LoadDetail()
        {
            var claim = CurrentClaim;
            if (claim == null)
            {
                ShowInformationHeader("Data klaim tidak ditemukan.");
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
            if (!IsEditMode) return;

            long documentID;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out documentID)) return;

            if (e.CommandName == "DeleteDocument")
            {
                DeleteDocument(documentID);
                return;
            }

            if (e.CommandName != "SendDocument") return;

            var claim = CurrentClaim;
            if (claim == null)
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

        private void DeleteDocument(long documentID)
        {
            var document = new AdmedikaDocument();
            if (!document.LoadByPrimaryKey(documentID))
            {
                grdDocuments.Rebind();
                ShowInformationHeader("Dokumen tidak ditemukan.");
                return;
            }

            if (document.IsSentSuccess != null)
            {
                grdDocuments.Rebind();
                ShowInformationHeader("Dokumen sudah pernah dikirim dan tidak dapat dihapus.");
                return;
            }

            var claim = CurrentClaim;

            try
            {
                var filePath = AdmedikaEdocsHelper.GetDocumentFilePath(document, claim);
                if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }
            catch (Exception ex)
            {
                grdDocuments.Rebind();
                ShowInformationHeader("Gagal menghapus file: " + ex.Message);
                return;
            }

            try
            {
                document.MarkAsDeleted();
                document.Save();
                ShowInformationHeader("Dokumen berhasil dihapus.");
            }
            catch (Exception ex)
            {
                ShowInformationHeader("Gagal menghapus data dokumen: " + ex.Message);
            }

            grdDocuments.Rebind();
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

        protected string BuildFileUrl(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var claim = CurrentClaim;
            if (claim == null) return string.Empty;

            var fileName = Convert.ToString(row[AdmedikaDocumentMetadata.ColumnNames.FileName]);
            return AdmedikaEdocsHelper.BuildFileUrl(claim, fileName);
        }

        protected bool IsOpenVisible(object dataItem)
        {
            return !string.IsNullOrEmpty(BuildFileUrl(dataItem));
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

        protected bool IsDeleteVisible(object dataItem)
        {
            if (!IsEditMode) return false;

            var row = dataItem as DataRowView;
            if (row == null) return false;

            // Hanya dokumen yang belum pernah dikirim yang boleh dihapus.
            return row[AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess] == DBNull.Value;
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
            return value.HasValue ? value.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : "-";
        }
    }
}
