using System;
using System.Data;
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
            UrlPageDetail = "TilakaTemplateSignFileDetail.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuSearch.Visible = false;
            ToolBarMenuExport.Visible = false;
            ToolBarMenuImport.Visible = false;
            ToolBarMenuHome.Visible = false;
            ToolBarMenuQuickSearch.Visible = false;
            ToolBarMenuList.Visible = false;
            ToolBarMenuRejournal.Visible = false;
            ToolBarMenuRebalance.Visible = false;
            ToolBarMenuBPJSSearch.Visible = false;
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = GetList();
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            if (dataItems == null || dataItems.Length == 0) return;

            RedirectToDetail(Convert.ToString(dataItems[0].GetDataKeyValue("TemplateSignFileID")), "view");
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            if (dataItems == null || dataItems.Length == 0) return;

            RedirectToDetail(Convert.ToString(dataItems[0].GetDataKeyValue("TemplateSignFileID")), "edit");
        }

        private static void RedirectToDetail(string id, string mode)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            System.Web.HttpContext.Current.Response.Redirect(
                "TilakaTemplateSignFileDetail.aspx?md=" + mode + "&id=" + System.Web.HttpUtility.UrlEncode(id));
        }

        protected void grdList_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "register") return;

            long id;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out id)) return;

            var repository = new TilakaTemplateSignFileRepository();
            var record = repository.FindByID(id);
            if (record == null) return;

            RegisterTemplate(record, repository);

            grdList.Rebind();
        }

        private void RegisterTemplate(TilakaTemplateSignFileRecord record,
            ITilakaTemplateSignFileRepository repository)
        {
            var config = new TilakaConfig();
            if (!config.IsQuickSignConfigured)
            {
                lblMessage.Text = TilakaAlertHtml.Error("Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).");
                return;
            }

            var registration = new TilakaRegistrationRepository().FindLatest(
                TilakaConstant.RelatedType.Paramedic, record.ParamedicID);
            if (registration == null || registration.CertificateStatus != TilakaConstant.CertificateStatus.Active
                || !registration.IsQuickSignUser || string.IsNullOrWhiteSpace(registration.TilakaName))
            {
                lblMessage.Text = TilakaAlertHtml.Error("Dokter harus memiliki sertifikat aktif dan terdaftar sebagai Quick Sign user.");
                return;
            }

            try
            {
                var store = new TilakaTemplateSignFileStore(config.TemplateSignFolder);
                var content = store.Load(record.FilePath);
                if (content == null)
                {
                    lblMessage.Text = TilakaAlertHtml.Error("File template tidak ditemukan.");
                    return;
                }

                var service = TilakaQuickSignServiceFactory.Create(config);
                var paramedic = new Paramedic();
                paramedic.LoadByPrimaryKey(record.ParamedicID);

                if (paramedic.IsUsingESignature != true)
                {
                    lblMessage.Text = TilakaAlertHtml.Error("Dokter ini tidak menggunakan tanda tangan elektronik.");
                    return;
                }

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
                    ? TilakaAlertHtml.Success("Template berhasil didaftarkan ke Tilaka.")
                    : TilakaAlertHtml.Error(error);
            }
            catch (Exception ex)
            {
                lblMessage.Text = TilakaAlertHtml.Error(ex.Message);
            }
        }

        private DataTable GetList()
        {
            var query = new TilakaTemplateSignFileQuery("a");
            var paramedic = new ParamedicQuery("b");
            query.LeftJoin(paramedic).On(query.ParamedicID == paramedic.ParamedicID);
            query.Select(query, paramedic.ParamedicName);

            var keywordText = txtParamedic.Text.Trim();
            if (!string.IsNullOrWhiteSpace(keywordText))
            {
                var keyword = "%" + keywordText + "%";
                query.Where(query.Or(
                    query.ParamedicID.Like(keyword),
                    paramedic.ParamedicName.Like(keyword)));
            }

            query.es.Top = AppSession.Parameter.MaxResultRecord;
            query.OrderBy(query.ParamedicID.Ascending, query.TemplateNo.Ascending);

            var table = query.LoadDataTable();
            table.Columns.Add("CanRegister", typeof(bool));

            foreach (DataRow row in table.Rows)
            {
                var isActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]);
                var isRegistered = row["IsRegistered"] != DBNull.Value && Convert.ToBoolean(row["IsRegistered"]);
                row["CanRegister"] = TilakaTemplateSignFileRules.CanRegister(isActive, isRegistered);
            }

            return table;
        }
    }
}
