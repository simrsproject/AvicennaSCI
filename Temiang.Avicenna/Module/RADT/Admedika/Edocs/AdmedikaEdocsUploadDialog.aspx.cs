using System;
using System.IO;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEdocsUploadDialog : BasePageDialog
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
            ProgramID = AppConstant.Program.AdmedikaEdocs;

            ButtonOk.Visible = false;
            ButtonCancel.Text = "Close";
            ButtonCancel.OnClientClick = "closeUploadDialog(); return false;";

            if (IsPostBack) return;

            BindDocTypes();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadClaim();
        }

        private void BindDocTypes()
        {
            cboDocType.Items.Clear();
            AddDocTypeGroup("eFORMS", AdmedikaDocType.GroupEForms);
            AddDocTypeGroup("Mandatory Edocs", AdmedikaDocType.GroupMandatoryEdocs);
        }

        private void AddDocTypeGroup(string caption, string group)
        {
            var header = new RadComboBoxItem("- " + caption + " -", string.Empty) { Enabled = false };
            cboDocType.Items.Add(header);

            foreach (var docType in AdmedikaDocType.All)
            {
                if (docType.Group != group) continue;

                cboDocType.Items.Add(new RadComboBoxItem(
                    string.Format("{0} - {1}", docType.Code, docType.Description),
                    docType.Code));
            }
        }

        private AdmedikaClaimTransaction LoadClaim()
        {
            var claim = new AdmedikaClaimTransaction();
            if (!claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowMessage("Data klaim tidak ditemukan.");
                btnUpload.Enabled = false;
                btnUploadAndSend.Enabled = false;
                return null;
            }

            lblCardNo.Text = claim.CardNo ?? string.Empty;
            lblClID.Text = claim.ClID ?? string.Empty;
            lblPatientName.Text = claim.PatientName ?? string.Empty;

            var enabled = !string.IsNullOrWhiteSpace(claim.CardNo) && !string.IsNullOrWhiteSpace(claim.ClID);
            btnUpload.Enabled = enabled;
            btnUploadAndSend.Enabled = enabled;

            return claim;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            Process(false);
        }

        protected void btnUploadAndSend_Click(object sender, EventArgs e)
        {
            Process(true);
        }

        private void Process(bool send)
        {
            HideMessage();

            var claim = LoadClaim();
            if (claim == null) return;

            if (string.IsNullOrWhiteSpace(AdmedikaEdocsHelper.GetRootPath()))
            {
                ShowMessage("Folder upload (AdmedikaUploadEdocsPath) belum dikonfigurasi.");
                return;
            }

            long documentID;
            long.TryParse(hdnDocumentID.Value, out documentID);

            // Baris sudah dibuat pada klik sebelumnya (mis. Upload & Send gagal), jadi cukup retry kirim.
            if (documentID <= 0)
            {
                if (!SaveNewDocument(claim, out documentID)) return;
                hdnDocumentID.Value = documentID.ToString();
            }

            if (!send)
            {
                CloseDialog("uploaded");
                return;
            }

            var document = new AdmedikaDocument();
            if (!document.LoadByPrimaryKey(documentID))
            {
                ShowMessage("Dokumen tidak ditemukan.");
                return;
            }

            bool success;
            var message = AdmedikaEdocsHelper.Send(document, claim, out success);

            if (success)
                CloseDialog("sent");
            else
            {
                // File sudah tersimpan; tombol Upload tidak diperlukan lagi, cukup retry kirim.
                btnUpload.Enabled = false;
                btnUploadAndSend.Text = "Re-send";
                ShowMessage(message);
            }
        }

        private bool SaveNewDocument(AdmedikaClaimTransaction claim, out long documentID)
        {
            documentID = 0;

            var docType = cboDocType.SelectedValue;
            if (!AdmedikaDocType.IsValid(docType))
            {
                ShowMessage("File Type wajib dipilih.");
                return false;
            }

            if (uplFile.UploadedFiles.Count == 0)
            {
                ShowMessage("File wajib dipilih.");
                return false;
            }

            var uploadedFile = uplFile.UploadedFiles[0];
            var originalName = uploadedFile.GetName();

            if (!AdmedikaEdocsHelper.IsAllowedExtension(originalName))
            {
                ShowMessage("Jenis file tidak diizinkan. Gunakan PDF, JPG, atau PNG.");
                return false;
            }

            if (uploadedFile.ContentLength > AdmedikaEdocsHelper.MaxFileSizeBytes)
            {
                ShowMessage("Ukuran file melebihi 10 MB.");
                return false;
            }

            string fileName;
            string filePath;

            try
            {
                var folder = AdmedikaEdocsHelper.GetDocumentFolder(claim);
                Directory.CreateDirectory(folder);

                fileName = AdmedikaEdocsHelper.EnsureUniqueFileName(
                    folder,
                    AdmedikaEdocsHelper.SanitizeFileName(originalName));

                filePath = Path.Combine(folder, fileName);
                uploadedFile.SaveAs(filePath, true);
            }
            catch (Exception ex)
            {
                ShowMessage("Gagal menyimpan file: " + ex.Message);
                return false;
            }

            var document = new AdmedikaDocument
            {
                AdmedikaClaimTransactionID = ClaimTransactionID,
                DocType = docType,
                FileName = fileName,
                CreatedDateTime = DateTime.Now,
                CreatedByUserID = AppSession.UserLogin.UserID
            };

            try
            {
                document.Save();
                documentID = document.AdmedikaDocumentID ?? 0;
            }
            catch (Exception ex)
            {
                try
                {
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
                catch
                {
                    // abaikan kegagalan hapus file
                }

                ShowMessage("Gagal menyimpan data dokumen: " + ex.Message);
                return false;
            }

            return documentID > 0;
        }

        private void ShowMessage(string message)
        {
            lblMessage.Text = message;
            lblMessage.ForeColor = System.Drawing.Color.Red;
            lblMessage.Visible = !string.IsNullOrWhiteSpace(message);
        }

        private void HideMessage()
        {
            lblMessage.Text = string.Empty;
            lblMessage.Visible = false;
        }

        private void CloseDialog(string mode)
        {
            // Full postback pada dialog tidak mengeksekusi RadAjaxManager.ResponseScripts,
            // jadi gunakan helper yang mendaftarkan script lewat ScriptManager + Sys.Application.load.
            Helper.RegisterStartupScript(this, "admedikaEdocsClose",
                "var oWnd = GetRadWindow();" +
                "if (oWnd) {" +
                "oWnd.argument = oWnd.argument || {};" +
                "oWnd.argument.mode = '" + mode + "';" +
                "oWnd.close();" +
                "}");
        }
    }
}
