using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaTemplateSignFileDetail : BasePageDetail
    {
        private long TemplateSignFileID
        {
            get
            {
                long id;
                long.TryParse(hfTemplateSignFileID.Value, out id);
                return id;
            }
            set { hfTemplateSignFileID.Value = value.ToString(CultureInfo.InvariantCulture); }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.TilakaTemplateSignFile;
            UrlPageList = "TilakaTemplateSignFileList.aspx";
            UrlPageSearch = "TilakaTemplateSignFileList.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuDelete.Enabled = false;
            ToolBarMenuMovePrev.Enabled = false;
            ToolBarMenuMoveNext.Enabled = false;
            ToolBarMenuSearch.Enabled = false;
        }

        protected override void OnMenuMoveNextClick(ValidateArgs args)
        {
            Response.Redirect("TilakaTemplateSignFileList.aspx");
        }

        protected override void OnMenuMovePrevClick(ValidateArgs args)
        {
            Response.Redirect("TilakaTemplateSignFileList.aspx");
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            base.OnDataModeChanged(oldVal, newVal);

            var isNew = newVal == AppEnum.DataMode.New;
            var isRead = newVal == AppEnum.DataMode.Read;

            cboParamedicID.Enabled = isNew;
            uplTemplate.Visible = isNew;
            chkIsDefault.Enabled = !isRead;
            chkIsActive.Enabled = !isRead;
        }

        protected override void OnMenuNewClick()
        {
            TemplateSignFileID = 0;
            cboParamedicID.Items.Clear();
            cboParamedicID.Text = string.Empty;
            cboParamedicID.SelectedValue = string.Empty;

            lblTemplateNo.Text = string.Empty;
            lblFileName.Text = string.Empty;
            lblFileSize.Text = string.Empty;
            lblRegistered.Text = "Belum terdaftar";
            lblRegisteredDateTime.Text = string.Empty;
            lblError.Text = string.Empty;
            lblMessage.Text = string.Empty;
            lnkDownload.Visible = false;

            chkIsDefault.Checked = true;
            chkIsActive.Checked = true;
        }

        protected override void OnPopulateEntryControl(params string[] parameters)
        {
            long id;
            if (parameters != null && parameters.Length > 0)
                long.TryParse(parameters[0], out id);
            else
                id = TemplateSignFileID;

            LoadData(id);
        }

        private void LoadData(long id)
        {
            var record = new TilakaTemplateSignFileRepository().FindByID(id);
            if (record == null)
            {
                ShowInformationHeader("Data template tidak ditemukan.");
                return;
            }

            TemplateSignFileID = record.TemplateSignFileID;
            lblTemplateNo.Text = record.TemplateNo;
            lblFileName.Text = record.FileName;
            lblFileSize.Text = FormatFileSize(record.FileSize);
            lblRegistered.Text = record.IsRegistered ? "Terdaftar" : "Belum terdaftar";
            lblRegisteredDateTime.Text = record.RegisteredDateTime.HasValue
                ? record.RegisteredDateTime.Value.ToString("dd-MM-yyyy HH:mm", CultureInfo.InvariantCulture)
                : string.Empty;
            lblError.Text = record.LastErrorMessage;

            chkIsDefault.Checked = record.IsDefault;
            chkIsActive.Checked = record.IsActive;

            lnkDownload.Visible = !string.IsNullOrWhiteSpace(record.FilePath);
            if (lnkDownload.Visible)
                lnkDownload.NavigateUrl = "TilakaTemplateSignFileHandler.ashx?id=" +
                                          record.TemplateSignFileID.ToString(CultureInfo.InvariantCulture);

            var paramedic = new Paramedic();
            var paramedicName = record.ParamedicID;
            if (paramedic.LoadByPrimaryKey(record.ParamedicID))
                paramedicName = paramedic.ParamedicName;

            cboParamedicID.Items.Clear();
            cboParamedicID.Items.Add(new RadComboBoxItem(
                paramedicName + " (" + record.ParamedicID + ")", record.ParamedicID));
            cboParamedicID.SelectedValue = record.ParamedicID;
        }

        protected void cboParamedicID_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var query = new ParamedicQuery();
            query.Where(query.IsActive == true, query.IsUsingESignature == true);

            var text = (e.Text ?? string.Empty).Trim();
            if (!string.IsNullOrWhiteSpace(text))
            {
                var keyword = "%" + text + "%";
                query.Where(query.Or(
                    query.ParamedicID.Like(keyword),
                    query.ParamedicName.Like(keyword)));
            }

            query.Select(query.ParamedicID, query.ParamedicName);
            query.es.Top = 20;
            query.OrderBy(query.ParamedicName.Ascending);

            cboParamedicID.DataSource = query.LoadDataTable();
            cboParamedicID.DataTextField = "ParamedicName";
            cboParamedicID.DataValueField = "ParamedicID";
            cboParamedicID.DataBind();
        }

        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            try
            {
                var paramedicID = (cboParamedicID.SelectedValue ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(paramedicID))
                {
                    Fail(args, "Dokter belum dipilih.");
                    return;
                }

                var paramedic = new Paramedic();
                if (!paramedic.LoadByPrimaryKey(paramedicID)
                    || paramedic.IsActive != true
                    || paramedic.IsUsingESignature != true)
                {
                    Fail(args, "Dokter harus aktif dan menggunakan tanda tangan elektronik.");
                    return;
                }

                if (uplTemplate.UploadedFiles.Count == 0)
                {
                    Fail(args, "File PDF template belum dipilih.");
                    return;
                }

                var config = new TilakaConfig();
                var store = new TilakaTemplateSignFileStore(config.TemplateSignFolder);
                var repository = new TilakaTemplateSignFileRepository();
                var existing = repository.FindByParamedic(paramedicID);

                var existingNumbers = new List<string>();
                foreach (var item in existing) existingNumbers.Add(item.TemplateNo);

                var templateNo = TilakaTemplateSignFileRules.NextTemplateNo(paramedicID, existingNumbers);
                var file = uplTemplate.UploadedFiles[0];
                byte[] content;
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    content = stream.ToArray();
                }

                var relativePath = store.Save(paramedicID, templateNo, content,
                    file.ContentType, DateTime.Now);

                var isActive = chkIsActive.Checked;
                var isDefault = TilakaTemplateSignFileRules.ResolveIsDefault(isActive, chkIsDefault.Checked);

                if (isDefault)
                    ClearOtherDefaults(repository, paramedicID, 0);

                repository.Save(new TilakaTemplateSignFileRecord
                {
                    ParamedicID = paramedicID,
                    TemplateNo = templateNo,
                    FileName = file.GetName(),
                    FilePath = relativePath,
                    ContentType = file.ContentType,
                    FileSize = content.Length,
                    IsDefault = isDefault,
                    IsActive = isActive,
                    IsRegistered = false,
                    CreatedDateTime = DateTime.Now,
                    CreatedByUserID = AppSession.UserLogin.UserID
                });

                Response.Redirect("TilakaTemplateSignFileList.aspx");
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Fail(args, ex.Message);
            }
        }

        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            try
            {
                var repository = new TilakaTemplateSignFileRepository();
                var record = repository.FindByID(TemplateSignFileID);
                if (record == null)
                {
                    Fail(args, "Data template tidak ditemukan.");
                    return;
                }

                var isActive = chkIsActive.Checked;
                var isDefault = TilakaTemplateSignFileRules.ResolveIsDefault(isActive, chkIsDefault.Checked);

                if (isDefault)
                    ClearOtherDefaults(repository, record.ParamedicID, record.TemplateSignFileID);

                record.IsActive = isActive;
                record.IsDefault = isDefault;
                record.LastUpdateByUserID = AppSession.UserLogin.UserID;
                repository.Save(record);

                Response.Redirect("TilakaTemplateSignFileList.aspx");
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Fail(args, ex.Message);
            }
        }

        private static void ClearOtherDefaults(ITilakaTemplateSignFileRepository repository,
            string paramedicID, long keepID)
        {
            foreach (var item in repository.FindByParamedic(paramedicID))
            {
                if (item.TemplateSignFileID == keepID || !item.IsDefault) continue;

                item.IsDefault = false;
                item.LastUpdateByUserID = AppSession.UserLogin.UserID;
                repository.Save(item);
            }
        }

        private void Fail(ValidateArgs args, string message)
        {
            args.IsCancel = true;
            args.MessageText = message;
            lblMessage.Text = message;
            ShowInformationHeader(message);
        }

        private static string FormatFileSize(long? size)
        {
            if (!size.HasValue || size.Value <= 0) return string.Empty;
            if (size.Value >= 1024 * 1024)
                return (size.Value / (1024d * 1024d)).ToString("0.##", CultureInfo.InvariantCulture) + " MB";

            return (size.Value / 1024d).ToString("0.##", CultureInfo.InvariantCulture) + " KB";
        }
    }
}
