using System;
using System.Web;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaRevokeDialog : BasePageDialog
    {
        private string ParamedicID
        {
            get { return Request.QueryString["paramedicId"]; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.TilakaRegistration;
            Title = "Revoke Sertifikat";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) LoadData();
            ButtonOk.OnClientClick = "return openRevokeWindow();";
        }

        private void LoadData()
        {
            if (string.IsNullOrWhiteSpace(ParamedicID))
            {
                DisableOk("Dokter tidak ditemukan.");
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                DisableOk("Dokter tidak ditemukan.");
                return;
            }

            if (paramedic.IsUsingESignature != true)
            {
                DisableOk("Dokter ini tidak menggunakan tanda tangan elektronik.");
                return;
            }

            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(
                TilakaConstant.RelatedType.Paramedic, ParamedicID);
            var state = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            if (!TilakaRegistrationActionRules.CanRevoke(
                    record, state, paramedic.IsUsingESignature == true))
            {
                DisableOk("Sertifikat tidak dalam status Aktif.");
                return;
            }

            lblParamedicName.Text = paramedic.ParamedicName;
            lblTilakaName.Text = record.TilakaName;
            lblCertificateSerialNumber.Text = record.CertificateSerialNumber;

            cboReason.DataSource = TilakaConstant.RevokeReason.All;
            cboReason.DataBind();
            cboReason.Items.Insert(0, new RadComboBoxItem(string.Empty, string.Empty));

            ButtonOk.Text = "Revoke";
        }

        private void DisableOk(string message)
        {
            ShowInformationHeader(message);
            ButtonOk.Enabled = false;
        }

        public override bool OnButtonOkClicked()
        {
            try
            {
                return Revoke();
            }
            catch (Exception)
            {
                return Fail(
                    "Gagal memproses revoke. Silakan coba lagi atau hubungi administrator.");
            }
        }

        private bool Revoke()
        {
            if (string.IsNullOrWhiteSpace(cboReason.SelectedValue))
                return Fail("Alasan revoke wajib dipilih.");

            if (!chkConfirm.Checked)
                return Fail("Konfirmasi revoke wajib dicentang.");

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID)
                || paramedic.IsUsingESignature != true)
                return Fail("Dokter ini tidak menggunakan tanda tangan elektronik.");

            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(
                TilakaConstant.RelatedType.Paramedic, ParamedicID);
            var state = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            if (!TilakaRegistrationActionRules.CanRevoke(
                    record, state, paramedic.IsUsingESignature == true))
                return Fail("Sertifikat tidak dalam status Aktif.");

            if (string.IsNullOrWhiteSpace(record.RegistrationRequestID))
                return Fail("Request ID Tilaka tidak tersedia.");

            var service = new Service();
            var response = service.RequestRevokeCertificate(
                record.TilakaName, cboReason.SelectedValue);

            string message;
            if (!service.IsOk(response, out message))
                return Fail(message);

            TilakaRevokeResponseData revokeData;
            if (!TilakaRevokeResponseData.TryCreate(response, out revokeData))
                return Fail("Respons revoke Tilaka tidak lengkap.");

            var now = DateTime.Now;
            record.RevokeRequestID = revokeData.RevokeId;
            record.RevokeReason = cboReason.SelectedValue;
            record.RevokeNote = txtNote.Text.Trim();
            record.RevokeStatus = TilakaConstant.RevokeStatus.Requested;
            record.RevokeMessage = response.Message;
            record.RevokeRequestedDateTime = now;
            record.RevokeRequestedByUserID = AppSession.UserLogin.UserID;
            record.LastUpdateByUserID = AppSession.UserLogin.UserID;
            repository.Save(record);

            var config = new TilakaConfig();
            var redirectUrl = TilakaUrls.WebviewRedirect(
                config, "revoke", record.RegistrationRequestID);
            var revokeUrl = TilakaUrls.AppendRedirectUrl(
                revokeData.RevokeUrl, redirectUrl);

            ClientScript.RegisterStartupScript(GetType(), "openRevoke",
                "window.open('" + HttpUtility.JavaScriptStringEncode(revokeUrl)
                + "','tilakaRevokeWindow');", true);
            return true;
        }

        private bool Fail(string message)
        {
            ClientScript.RegisterStartupScript(GetType(), "closeRevoke",
                "var w=window.open('','tilakaRevokeWindow'); if(w){w.close();}", true);
            ShowInformationHeader(message);
            return false;
        }
    }
}
