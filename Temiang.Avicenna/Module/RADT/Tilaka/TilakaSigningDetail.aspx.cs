using System;
using System.Collections.Generic;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaSigningDetail : BasePageDetail
    {
        private string RequestID
        {
            get { return hfRequestID.Value; }
            set { hfRequestID.Value = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaSigning;
            UrlPageList = "TilakaSigningList.aspx";
            UrlPageSearch = "TilakaSigningList.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Enabled = false;
            ToolBarMenuEdit.Enabled = false;
            ToolBarMenuDelete.Enabled = false;
            ToolBarMenuMovePrev.Enabled = false;
            ToolBarMenuMoveNext.Enabled = false;
            ToolBarMenuSearch.Enabled = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RequestID = Request.QueryString["id"];
                LoadData();
            }
        }

        private void LoadData()
        {
            var record = new TilakaQuickSignRepository().FindByRequestId(RequestID);
            if (record == null)
            {
                lblMessage.Text = "Data request tidak ditemukan.";
                return;
            }

            lblRequestID.Text = record.RequestID;
            lblCasemixFileID.Text = Convert.ToString(record.CasemixFileID);
            lblFileName.Text = record.FileName;
            lblTemplateNo.Text = record.TemplateNo;
            lblStatus.Text = record.Status;
            lblError.Text = record.ErrorMessage;

            if (record.CasemixFileID.HasValue)
                lnkCasemix.NavigateUrl = "TilakaCasemixFileHandler.ashx?id=" + record.CasemixFileID.Value;

            lnkSignedFile.Visible = !string.IsNullOrWhiteSpace(record.SignedFilePath);
            if (lnkSignedFile.Visible)
                lnkSignedFile.NavigateUrl = "TilakaSignedFileHandler.ashx?id=" +
                                            Server.UrlEncode(record.RequestID);

            grdSigner.DataSource = new TilakaQuickSignRepository().LoadSigners(record.QuickSignRequestID);
            grdSigner.DataBind();
        }

        protected void grdSigner_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var record = new TilakaQuickSignRepository().FindByRequestId(RequestID);
            grdSigner.DataSource = record == null
                ? new List<TilakaQuickSignSignerRecord>()
                : new TilakaQuickSignRepository().LoadSigners(record.QuickSignRequestID);
        }

        protected void btnCheck_OnClick(object sender, EventArgs e)
        {
            var config = new TilakaConfig();
            if (!config.IsQuickSignConfigured)
            {
                lblMessage.Text = "Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).";
                return;
            }

            var repository = new TilakaQuickSignRepository();
            var record = repository.FindByRequestId(RequestID);
            if (record == null) return;

            var signers = repository.LoadSigners(record.QuickSignRequestID);
            var service = TilakaQuickSignServiceFactory.Create(config);
            var status = service.CheckSignStatus(record.RequestID);

            string message;
            if (!service.IsOk(status, out message))
            {
                lblMessage.Text = message;
                return;
            }

            ApplyStatus(record, signers, status);
            TryDownload(record, config, signers);

            var state = TilakaQuickSignState.Evaluate(record, signers);
            if (state == TilakaQuickSignConstant.RequestStatus.Completed
                && !string.IsNullOrWhiteSpace(record.SignedFilePath))
            {
                record.Status = TilakaQuickSignConstant.RequestStatus.Completed;
                record.IsActive = false;
                record.CompletedDateTime = DateTime.Now;
            }
            else if (state == TilakaQuickSignConstant.RequestStatus.Failed)
            {
                record.Status = TilakaQuickSignConstant.RequestStatus.Failed;
                record.IsActive = false;
                record.CompletedDateTime = DateTime.Now;
            }
            else
            {
                record.Status = TilakaQuickSignConstant.RequestStatus.InProgress;
                record.IsActive = true;
            }

            record.LastUpdateByUserID = AppSession.UserLogin.UserID;
            repository.Save(record, signers);

            lblMessage.Text = "Status diperbarui: " + record.Status;
            LoadData();
        }

        private static void ApplyStatus(TilakaQuickSignRecord record,
            IList<TilakaQuickSignSignerRecord> signers, QuickSignStatusResponse status)
        {
            if (status.Status != null)
            {
                foreach (var item in status.Status)
                {
                    foreach (var signer in signers)
                    {
                        if (signer.Sequence != item.Sequence
                            && !string.Equals(signer.UserIdentifier, item.UserIdentifier,
                                StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (TilakaQuickSignState.CanApplySignerStatus(signer.Status, item.Status))
                        {
                            signer.Status = item.Status;
                            signer.NumSignatures = item.NumSignatures ?? signer.NumSignatures;
                            signer.NumSignaturesDone = item.NumSignaturesDone ?? signer.NumSignaturesDone;
                            signer.LastCheckedDateTime = DateTime.Now;
                            signer.LastUpdateByUserID = AppSession.UserLogin.UserID;
                        }
                    }
                }
            }

            if (status.ListPdf != null)
            {
                foreach (var pdf in status.ListPdf)
                {
                    if (string.IsNullOrWhiteSpace(pdf.PresignedUrl)) continue;
                    foreach (var signer in signers) signer.SignedUrl = pdf.PresignedUrl;
                }
            }

            record.ErrorMessage = null;
        }

        private static void TryDownload(TilakaQuickSignRecord record, TilakaConfig config,
            IList<TilakaQuickSignSignerRecord> signers)
        {
            if (string.IsNullOrWhiteSpace(config.QuickSignSignedFolder)) return;
            if (!string.IsNullOrWhiteSpace(record.SignedFilePath)) return;

            string url = null;
            foreach (var signer in signers)
            {
                if (!string.IsNullOrWhiteSpace(signer.SignedUrl))
                {
                    url = signer.SignedUrl;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(url)) return;

            try
            {
                var downloader = new TilakaSignedFileDownloader(
                    config.QuickSignSignedFolder, TilakaQuickSignServiceFactory.Create(config));
                record.SignedFilePath = downloader.Download(record.RequestID, record.FileName, url);
            }
            catch (Exception ex)
            {
                record.ErrorMessage = "Gagal mengunduh dokumen bertanda tangan: " + ex.Message;
            }
        }
    }
}
