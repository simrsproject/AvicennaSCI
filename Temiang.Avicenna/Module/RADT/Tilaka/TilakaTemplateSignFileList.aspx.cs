using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaTemplateSignFileList : BasePageList
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaTemplateSignFile;
            UrlPageSearch = "TilakaTemplateSignFileList.aspx";
            UrlPageDetail = "TilakaTemplateSignFileList.aspx";
        }

        protected void btnSearch_OnClick(object sender, EventArgs e)
        {
            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = GetList();
        }

        protected void btnUpload_OnClick(object sender, EventArgs e)
        {
            if (uplTemplate.UploadedFiles.Count == 0)
            {
                lblMessage.Text = "Pilih file PDF template terlebih dahulu.";
                return;
            }

            var paramedicID = (txtParamedicID.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(paramedicID))
            {
                lblMessage.Text = "Isi ID dokter terlebih dahulu.";
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(paramedicID))
            {
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            var config = new TilakaConfig();
            try
            {
                var store = new TilakaTemplateSignFileStore(config.TemplateSignFolder);
                var repository = new TilakaTemplateSignFileRepository();
                var existing = repository.FindByParamedic(paramedicID);
                var next = NextSequence(existing);
                var templateNo = "TTD-" + paramedicID.ToUpperInvariant() + "-" + next.ToString("00");

                var file = uplTemplate.UploadedFiles[0];
                byte[] content;
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    content = stream.ToArray();
                }

                var relative = store.Save(paramedicID, templateNo, content, file.ContentType, DateTime.Now);
                var hasDefault = existing.Any(x => x.IsDefault);

                repository.Save(new TilakaTemplateSignFileRecord
                {
                    ParamedicID = paramedicID,
                    TemplateNo = templateNo,
                    FileName = file.GetName(),
                    FilePath = relative,
                    ContentType = file.ContentType,
                    FileSize = content.Length,
                    IsDefault = !hasDefault,
                    IsActive = true,
                    IsRegistered = false,
                    CreatedDateTime = DateTime.Now,
                    CreatedByUserID = AppSession.UserLogin.UserID
                });

                lblMessage.Text = "Template " + templateNo + " tersimpan. Lanjutkan dengan Daftarkan.";
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }

            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        protected void grdList_ItemCommand(object source, GridCommandEventArgs e)
        {
            long id;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            var repository = new TilakaTemplateSignFileRepository();
            var record = repository.FindByID(id);
            if (record == null) return;

            if (e.CommandName == "default")
            {
                foreach (var item in repository.FindByParamedic(record.ParamedicID))
                {
                    if (item.TemplateSignFileID == id) continue;
                    item.IsDefault = false;
                    item.LastUpdateByUserID = AppSession.UserLogin.UserID;
                    repository.Save(item);
                }

                record.IsDefault = true;
                record.LastUpdateByUserID = AppSession.UserLogin.UserID;
                repository.Save(record);
            }
            else if (e.CommandName == "deactivate")
            {
                record.IsActive = false;
                record.IsDefault = false;
                record.LastUpdateByUserID = AppSession.UserLogin.UserID;
                repository.Save(record);
            }
            else if (e.CommandName == "register")
            {
                RegisterTemplate(record, repository);
            }

            Session.Remove(SessionNameForList);
            grdList.Rebind();
        }

        private void RegisterTemplate(TilakaTemplateSignFileRecord record,
            ITilakaTemplateSignFileRepository repository)
        {
            var config = new TilakaConfig();
            if (!config.IsQuickSignConfigured)
            {
                lblMessage.Text = "Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).";
                return;
            }

            var registration = new TilakaRegistrationRepository().FindLatest(
                TilakaConstant.RelatedType.Paramedic, record.ParamedicID);
            if (registration == null || registration.CertificateStatus != TilakaConstant.CertificateStatus.Active
                || !registration.IsQuickSignUser || string.IsNullOrWhiteSpace(registration.TilakaName))
            {
                lblMessage.Text = "Dokter harus memiliki sertifikat aktif dan terdaftar sebagai Quick Sign user.";
                return;
            }

            try
            {
                var store = new TilakaTemplateSignFileStore(config.TemplateSignFolder);
                var content = store.Load(record.FilePath);
                if (content == null)
                {
                    lblMessage.Text = "File template tidak ditemukan.";
                    return;
                }

                var service = TilakaQuickSignServiceFactory.Create(config);
                var paramedic = new Paramedic();
                paramedic.LoadByPrimaryKey(record.ParamedicID);

                var response = service.AddTemplates(new QuickSignAddTemplatesRequest
                {
                    Templates = new[]
                    {
                        new QuickSignTemplateInput
                        {
                            FileName = record.FileName,
                            TemplateNumber = record.TemplateNo,
                            TemplateFile = Convert.ToBase64String(content),
                            AssignedUsers = new[]
                            {
                                new QuickSignAssignedUser
                                {
                                    UserIdentifier = registration.TilakaName,
                                    Email = paramedic.Email
                                }
                            }
                        }
                    }
                });

                var success = false;
                string error = response.Message;
                if (response.Successful != null)
                {
                    foreach (var item in response.Successful)
                    {
                        if (string.Equals(item.TemplateNo, record.TemplateNo, StringComparison.OrdinalIgnoreCase))
                            success = true;
                    }
                }
                if (!success && response.Failed != null && response.Failed.Length > 0)
                    error = response.Failed[0].Error;

                record.IsRegistered = success;
                record.RegisteredDateTime = success ? DateTime.Now : (DateTime?)null;
                record.LastErrorMessage = success ? null : error;
                record.LastUpdateByUserID = AppSession.UserLogin.UserID;
                repository.Save(record);

                lblMessage.Text = success
                    ? "Template berhasil didaftarkan ke Tilaka."
                    : error;
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
            }
        }

        private System.Data.DataTable GetList()
        {
            object cached = Session[SessionNameForList];
            if (cached != null) return (System.Data.DataTable)cached;

            var query = new TilakaTemplateSignFileQuery();
            if (!string.IsNullOrWhiteSpace(txtParamedicID.Text))
                query.Where(query.ParamedicID == txtParamedicID.Text.Trim());

            query.es.Top = AppSession.Parameter.MaxResultRecord;
            query.OrderBy(query.ParamedicID.Ascending, query.TemplateNo.Ascending);

            var table = query.LoadDataTable();
            Session[SessionNameForList] = table;
            return table;
        }

        private static int NextSequence(IList<TilakaTemplateSignFileRecord> existing)
        {
            var max = 0;
            foreach (var item in existing)
            {
                if (string.IsNullOrWhiteSpace(item.TemplateNo)) continue;
                var parts = item.TemplateNo.Split('-');
                int value;
                if (int.TryParse(parts[parts.Length - 1], out value) && value > max) max = value;
            }

            return max + 1;
        }
    }
}
