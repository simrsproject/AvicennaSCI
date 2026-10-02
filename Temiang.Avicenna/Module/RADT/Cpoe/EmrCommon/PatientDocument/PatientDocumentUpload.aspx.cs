using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Dal.Core;

namespace Temiang.Avicenna.Module.RADT.Emr
{
    public partial class PatientDocumentUpload : BasePageDialogEntry
    {
        private bool? _canEditPatientDocumentByMedicalRecord;

        protected long PatientDocumentID
        {
            get
            {
                if (!IsPostBack)
                    hdnPdId.Value = Request.QueryString["pdid"];
                if (string.IsNullOrEmpty(hdnPdId.Value))
                    hdnPdId.Value = "0";
                return Convert.ToInt32(hdnPdId.Value);
            }
            set { hdnPdId.Value = value.ToString(); }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Request.QueryString["progid"]))
            {
                ProgramID = Request.QueryString["progid"];
            }
            else
                ProgramID = AppConstant.Program.ElectronicMedicalRecord;

            // Menu hardcode
            IsSingleRecordMode = true;

            ToolBar.ApprovalUnApprovalVisible = false;
            ToolBar.VoidUnVoidVisible = false;
            ToolBar.PrintVisible = false;

            if (!IsPostBack)
            {
                var pat = new Patient();
                if (pat.LoadByPrimaryKey(PatientID))
                {
                    this.Title = "File Attachment of : " + pat.PatientName + " (MRN: " + pat.MedicalNo + ")";
                }
            }
        }


        protected override void OnPopulateEntryControl(ValidateArgs args)
        {
            var pd = new PatientDocument();
            pd.LoadByPrimaryKey(PatientDocumentID);
            PopulateEntryControl(pd);
        }
        protected void PopulateEntryControl(esEntity entity)
        {
            var pd = (PatientDocument)entity;
            txtDocumentName.Text = pd.DocumentName;
            txtFileAttachName.Text = pd.FileAttachName;
            txtDocumentDate.SelectedDate = pd.DocumentDate;
            txtNotes.Text = pd.Notes;
            txtReferenceNo.Text = pd.ReferenceNo;
            PatientDocumentID = pd.PatientDocumentID ?? 0;
        }
        protected override void OnMenuNewClick()
        {
            PatientDocumentID = 0;

            var ent = new PatientDocument();
            ent.RegistrationNo = RegistrationNo;
            ent.PatientID = PatientID;

            PopulateEntryControl(ent);

            if (!string.IsNullOrWhiteSpace(Request.QueryString["surgno"]))
                txtReferenceNo.Text = Request.QueryString["surgno"].ToString();

            txtDocumentDate.SelectedDate = (new DateTime()).NowAtSqlServer();
        }
        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            var entity = new PatientDocument();
            SetEntityValue(entity);
            var newID = SaveEntity(entity);
            PatientDocumentID = newID;

            // Upload file
            var fullPathFileName = UploadFile(entity, false);

            if (!string.IsNullOrWhiteSpace(fullPathFileName))
            {
                SaveThumbnail(entity, fullPathFileName);
                entity.Save();
            }
            else
            {
                // Fail Upload
                // Delete 
                entity.MarkAsDeleted();
                entity.Save();
            }

        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            rowUploadFile.Visible = newVal == AppEnum.DataMode.New || newVal == AppEnum.DataMode.Edit;
        }
        protected override void OnMenuEditClick()
        {
        }
        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            if (!CanEditPatientDocumentByMedicalRecord())
            {
                args.MessageText = "Edit attachment hanya dapat dilakukan oleh Rekam Medis level Manajer/Pengatur.";
                return;
            }

            var entity = new PatientDocument();
            if (entity.LoadByPrimaryKey(PatientDocumentID))
            {
                var oldFileAttachName = entity.FileAttachName;
                var oldPatientID = entity.PatientID;
                var oldDocumentFolderYearly = entity.DocumentFolderYearly;
                var oldIsUpload = entity.IsUpload ?? false;

                SetEntityValue(entity);
                var fullPathFileName = UploadFile(entity, true);
                if (!string.IsNullOrWhiteSpace(fullPathFileName))
                {
                    SaveThumbnail(entity, fullPathFileName);
                }

                SaveEntity(entity);

                if (!string.IsNullOrWhiteSpace(fullPathFileName))
                    DeletePreviousUploadedFile(oldIsUpload, oldPatientID, oldDocumentFolderYearly, oldFileAttachName, entity.FileAttachName);
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
            }
        }

        public override bool OnGetStatusMenuEdit()
        {
            return CanEditPatientDocumentByMedicalRecord();
        }

        private void SetEntityValue(PatientDocument entity)
        {
            entity.PatientID = PatientID;
            entity.RegistrationNo = RegistrationNo;
            entity.DocumentName = txtDocumentName.Text;
            entity.DocumentDate = txtDocumentDate.SelectedDate;
            entity.Notes = txtNotes.Text;
            entity.ReferenceNo = txtReferenceNo.Text;

            if (entity.es.IsAdded)
            {
                entity.IsUpload = true;
                entity.FileAttachName = string.Empty;
            }
        }

        private long SaveEntity(PatientDocument entity)
        {
            entity.Save();
            return entity.PatientDocumentID ?? 0;
        }

        private string UploadFile(PatientDocument entity, bool isReplaceFile)
        {
            if (uplFileTemplate.UploadedFiles.Count > 0)
            {
                foreach (UploadedFile validFile in uplFileTemplate.UploadedFiles)
                {
                    var fileName = isReplaceFile
                        ? string.Format("{0:000000000000000}_{1:yyyyMMddHHmmssfff}_{2}", entity.PatientDocumentID, DateTime.Now, validFile.GetName())
                        : string.Format("{0:000000000000000}_{1}", entity.PatientDocumentID, validFile.GetName());
                    entity.FileAttachName = fileName;
                    entity.OriFileName = validFile.GetName();

                    var targetFolderOld = Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocument", entity.PatientID.Trim());
                    var targetFolderYearly = "";
                    if (!string.IsNullOrEmpty(entity.DocumentFolderYearly))
                        targetFolderYearly = Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocumentYearly", entity.DocumentFolderYearly, entity.PatientID.Trim());

                    var targetFolder = targetFolderOld;
                    if (!System.IO.Directory.Exists(targetFolder)) {
                        // jika old blm ada brarti pakai yearly
                        targetFolder = string.IsNullOrEmpty(targetFolderYearly) ? targetFolderOld : targetFolderYearly;
                    }
                    
                    if (!System.IO.Directory.Exists(targetFolder))
                        System.IO.Directory.CreateDirectory(targetFolder);

                    var pathFile = Path.Combine(targetFolder, fileName);
                    validFile.SaveAs(pathFile, true);
                    return pathFile;
                }
            }

            return string.Empty;
        }

        private void SaveThumbnail(PatientDocument entity, string fullPathFileName)
        {
            entity.SmallImage = null;

            var oriFileName = (entity.OriFileName ?? string.Empty).ToLower();
            if (oriFileName.Contains(".jpg") || oriFileName.Contains(".jpeg") || oriFileName.Contains(".png"))
            {
                var imgHelper = new ImageHelper();
                var imgArr = imgHelper.LoadImageToArray(fullPathFileName);
                var smallImg = imgHelper.ResizeImage(imgArr, new Size(100, 100), true, InterpolationMode.Low);

                if (oriFileName.Contains(".jpg") || oriFileName.Contains(".jpeg"))
                    entity.SmallImage = imgHelper.ConvertImageToByteArray(smallImg, ImageFormat.Jpeg);
                else if (oriFileName.Contains(".png"))
                    entity.SmallImage = imgHelper.ConvertImageToByteArray(smallImg, ImageFormat.Png);
            }
        }

        private void DeletePreviousUploadedFile(bool oldIsUpload, string oldPatientID, string oldDocumentFolderYearly, string oldFileAttachName, string newFileAttachName)
        {
            if (!oldIsUpload || string.IsNullOrWhiteSpace(oldPatientID) || string.IsNullOrWhiteSpace(oldFileAttachName) || oldFileAttachName == newFileAttachName)
                return;

            var fileFolderOld = System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocument", oldPatientID.Trim());
            var fileFolderYearly = string.Empty;
            if (!string.IsNullOrEmpty(oldDocumentFolderYearly))
                fileFolderYearly = System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocumentYearly", oldDocumentFolderYearly, oldPatientID.Trim());

            var fileFolder = fileFolderOld;
            if (!System.IO.Directory.Exists(fileFolder))
                fileFolder = string.IsNullOrEmpty(fileFolderYearly) ? fileFolderOld : fileFolderYearly;

            var oldFilePath = System.IO.Path.Combine(fileFolder, oldFileAttachName);
            if (System.IO.File.Exists(oldFilePath))
                System.IO.File.Delete(oldFilePath);
        }

        private bool CanEditPatientDocumentByMedicalRecord()
        {
            if (!_canEditPatientDocumentByMedicalRecord.HasValue)
            {
                var query = new AppUserUserGroupQuery("a");
                query.Where(query.UserID == AppSession.UserLogin.UserID, query.UserGroupID.In("RM.01", "RM.02"));
                _canEditPatientDocumentByMedicalRecord = query.LoadDataTable().Rows.Count > 0;
            }

            return _canEditPatientDocumentByMedicalRecord.Value;
        }

        protected override void OnMenuDeleteClick(ValidateArgs args)
        {
            var ent = new PatientDocument();
            if (ent.LoadByPrimaryKey(PatientDocumentID))
            {
                // Rename File
                //var filePath = (ent.IsUpload ?? false) ?
                //System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocument", ent.PatientID.Trim(), ent.FileAttachName)
                //: ent.OriPath;

                var filePath = "";
                if (ent.IsUpload ?? false)
                {
                    //filePath = System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocument", ent.PatientID.Trim(), ent.FileAttachName);

                    var fileFolderOld = System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocument", ent.PatientID.Trim());
                    var fileFolderYearly = "";
                    if (!string.IsNullOrEmpty(ent.DocumentFolderYearly))
                        fileFolderYearly = System.IO.Path.Combine(AppSession.Parameter.ApplicationDocumentFolder, "PatientDocumentYearly", ent.DocumentFolderYearly, ent.PatientID.Trim());

                    var fileFolder = fileFolderOld;
                    if (!System.IO.Directory.Exists(fileFolder))
                    {
                        // jika old blm ada brarti pakai yearly
                        fileFolder = string.IsNullOrEmpty(fileFolderYearly) ? fileFolderOld : fileFolderYearly;
                    }

                    filePath = System.IO.Path.Combine(fileFolder, ent.FileAttachName);
                }
                else
                {
                    filePath = ent.OriPath;
                }

                if (System.IO.File.Exists(filePath))
                {
                    var newFilePath = Path.Combine(System.IO.Path.GetDirectoryName(filePath), "DEL_" + System.IO.Path.GetFileName(filePath));

                    File.Move(filePath, newFilePath);
                }

                ent.IsDeleted = true;
                ent.Save();
                PatientDocumentID = 0;
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
            }
        }

    }
}
