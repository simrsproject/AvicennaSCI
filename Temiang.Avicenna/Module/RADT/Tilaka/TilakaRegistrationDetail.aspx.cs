using System;
using System.Globalization;
using System.IO;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public partial class TilakaRegistrationDetail : BasePageDetail
    {
        private string ParamedicID
        {
            get { return hfParamedicID.Value; }
            set { hfParamedicID.Value = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ProgramID = AppConstant.Program.TilakaRegistration;
            UrlPageList = "TilakaRegistrationList.aspx";
            UrlPageSearch = "TilakaRegistrationList.aspx";
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            // BasePageDetail menonaktifkan kontrol entry pada mode baca. Halaman ini
            // bersifat operasional, sehingga upload KTP dan checkbox TnC harus tetap aktif.
            chkConsent.Enabled = true;
            uplKtp.Enabled = true;
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
            if (IsPostBack) return;

            ParamedicID = Request.QueryString["id"];
            LoadData();
        }

        private void LoadData()
        {
            if (string.IsNullOrWhiteSpace(ParamedicID))
            {
                HideActions();
                ShowWarning("Dokter tidak ditemukan.");
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                HideActions();
                ShowWarning("Dokter tidak ditemukan.");
                return;
            }

            lblParamedicName.Text = paramedic.ParamedicName;
            lblNamaKTP.Text = paramedic.NamaKTP;
            lblSsn.Text = paramedic.Ssn;
            lblEmail.Text = paramedic.Email;

            var isUsingESignature = paramedic.IsUsingESignature == true;
            var config = new TilakaConfig();
            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            var state = TilakaRegistrationState.Evaluate(record, DateTime.Now);

            lblStatus.Text = state;
            lblTilakaName.Text = record == null ? string.Empty : record.TilakaName;

            var webviewUrl = BuildWebviewUrl(config, record, state);
            hfWebviewUrl.Value = webviewUrl;

            if (record != null && !string.IsNullOrWhiteSpace(record.KtpFilePath))
            {
                imgKtp.ImageUrl = ResolveUrl("TilakaKtpImageHandler.ashx?id="
                    + record.TilakaRegistrationID.ToString(CultureInfo.InvariantCulture));
                imgKtp.Visible = true;
            }
            else
            {
                imgKtp.Visible = false;
            }

            var canStartRegistration = isUsingESignature && CanStartRegistration(state);
            pnlConsent.Visible = canStartRegistration;
            chkConsent.Visible = canStartRegistration;
            btnDaftar.Visible = canStartRegistration;
            btnKirimData.Visible = isUsingESignature && state == TilakaRegistrationState.UuidTerbit;
            btnWebview.Visible = isUsingESignature
                                 && !string.IsNullOrWhiteSpace(webviewUrl)
                                 && IsWebviewState(state);
            btnCekStatus.Visible = record != null && state != TilakaRegistrationState.BelumTerdaftar;

            ConfigureConsent(config);

            if (!isUsingESignature)
                ShowWarning("Dokter ini tidak menggunakan tanda tangan elektronik.");
        }

        private static bool CanStartRegistration(string state)
        {
            return state == TilakaRegistrationState.BelumTerdaftar
                   || state == TilakaRegistrationState.Kedaluwarsa
                   || state == TilakaRegistrationState.KycGagal
                   || state == TilakaRegistrationState.Ditolak;
        }

        private static bool IsWebviewState(string state)
        {
            return state == TilakaRegistrationState.MenungguKyc
                   || state == TilakaRegistrationState.KycGagal
                   || state == TilakaRegistrationState.SertifikatTerbit;
        }

        private void ConfigureConsent(TilakaConfig config)
        {
            var consentText = config.ReadConsentText();
            btnReadConsent.Visible = chkConsent.Visible
                                     && !string.IsNullOrWhiteSpace(consentText);

            if (string.IsNullOrWhiteSpace(consentText))
            {
                ShowWarning("File TnC Tilaka tidak ditemukan, kosong, atau tidak dapat diakses: " + config.ConsentTextFile);
                pnlConsent.Visible = false;
                if (btnDaftar.Visible)
                {
                    chkConsent.Enabled = false;
                    btnDaftar.Enabled = false;
                }
            }
        }

        private void ShowWarning(string message)
        {
            lblMessage.Text = string.Empty;
            ShowInformationHeader(message);
        }

        private void ShowSuccess(string message)
        {
            HideInformationHeader();
            lblMessage.Text = TilakaAlertHtml.Success(message);
        }

        private void ClearMessages()
        {
            lblMessage.Text = string.Empty;
            HideInformationHeader();
        }

        private void HideActions()
        {
            pnlConsent.Visible = false;
            chkConsent.Visible = false;
            btnReadConsent.Visible = false;
            uplKtp.Visible = false;
            btnDaftar.Visible = false;
            btnKirimData.Visible = false;
            btnWebview.Visible = false;
            btnCekStatus.Visible = false;
        }

        private static string BuildWebviewUrl(TilakaConfig config, TilakaRegistrationRecord record, string state)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.RegistrationRequestID)) return string.Empty;
            if (state == TilakaRegistrationState.Kedaluwarsa
                || state == TilakaRegistrationState.Ditolak) return string.Empty;
            if (state == TilakaRegistrationState.SertifikatTerbit)
                return TilakaUrls.ActivationWebview(config, record.RegistrationRequestID);
            return TilakaUrls.RegistrationWebview(config, record.RegistrationRequestID);
        }

        protected void btnDaftar_OnClick(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                RegisterRegistration();
            }
            catch (InvalidOperationException ex)
            {
                ShowWarning(ex.Message);
            }
            catch (Exception)
            {
                ShowWarning("Gagal memproses pendaftaran Tilaka. Silakan coba lagi atau hubungi administrator.");
            }
        }

        private void RegisterRegistration()
        {
            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                ShowWarning("Dokter tidak ditemukan.");
                return;
            }

            if (paramedic.IsUsingESignature != true)
            {
                ShowWarning("Dokter ini tidak menggunakan tanda tangan elektronik.");
                return;
            }

            var config = new TilakaConfig();
            if (string.IsNullOrWhiteSpace(config.ReadConsentText()))
            {
                ShowWarning("File TnC Tilaka tidak ditemukan, kosong, atau tidak dapat diakses: " + config.ConsentTextFile);
                return;
            }

            var repository = new TilakaRegistrationRepository();
            var latest = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            var ktpPath = latest == null ? null : latest.KtpFilePath;
            var ktpFileName = latest == null ? null : latest.KtpFileName;
            var ktpContentType = latest == null ? null : latest.KtpContentType;
            var ktpUploadedDateTime = latest == null ? null : latest.KtpUploadedDateTime;

            var hasNewUpload = uplKtp.UploadedFiles.Count > 0;
            var input = new TilakaDoctorInput
            {
                ParamedicID = ParamedicID,
                NamaKTP = paramedic.NamaKTP,
                Ssn = paramedic.Ssn,
                Email = paramedic.Email,
                HasKtpFile = !string.IsNullOrWhiteSpace(ktpPath) || hasNewUpload,
                ConsentApproved = chkConsent.Checked,
                NikUsedByOtherParamedic = IsNikUsedByOther(paramedic.Ssn)
            };

            var guard = TilakaRegistrationGuard.Check(input, latest, DateTime.Now);
            if (!guard.CanRegister)
            {
                ShowWarning(guard.Message);
                return;
            }

            if (hasNewUpload)
            {
                var store = new TilakaKtpFileStore(config.UploadKtpPath, config.MaxKtpSizeKb);
                foreach (UploadedFile file in uplKtp.UploadedFiles)
                {
                    using (var stream = new MemoryStream())
                    {
                        file.InputStream.CopyTo(stream);
                        ktpPath = store.Save(ParamedicID, paramedic.Ssn, stream.ToArray(), file.ContentType, DateTime.Now);
                    }

                    ktpFileName = file.GetName();
                    ktpContentType = file.ContentType;
                    ktpUploadedDateTime = DateTime.Now;
                }
            }

            var service = new Service();
            var uuid = service.GenerateUuid(paramedic.NamaKTP, paramedic.Email);
            string message;
            if (!service.IsOk(uuid, out message) || uuid.Data == null || uuid.Data.Length == 0)
            {
                ShowWarning(string.IsNullOrWhiteSpace(message)
                    ? "Gagal mendapatkan UUID Tilaka."
                    : message);
                return;
            }

            var record = new TilakaRegistrationRecord
            {
                RelatedType = TilakaConstant.RelatedType.Paramedic,
                RelatedID = ParamedicID,
                IsLatest = true,
                RegistrationRequestID = uuid.Data[0],
                RequestType = TilakaConstant.RequestType.Register,
                DateExpire = DateTime.Now.AddDays(config.RegistrationExpireDays),
                RegistrationState = TilakaRegistrationState.UuidTerbit,
                KtpFilePath = ktpPath,
                KtpFileName = ktpFileName,
                KtpContentType = ktpContentType,
                KtpUploadedDateTime = ktpUploadedDateTime,
                ConsentVersion = config.ConsentVersion,
                ConsentTimestamp = DateTime.Now,
                ConsentApprovedByUserID = AppSession.UserLogin.UserID
            };

            repository.Save(record);

            LoadData();
            ShowSuccess("UUID Tilaka berhasil dibuat. Lanjutkan dengan Kirim Data KYC.");
        }

        protected void btnKirimData_OnClick(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                SendKycData();
            }
            catch (InvalidOperationException ex)
            {
                ShowWarning(ex.Message);
            }
            catch (Exception)
            {
                ShowWarning("Gagal mengirim data KYC Tilaka. Silakan coba lagi atau hubungi administrator.");
            }
        }

        private void SendKycData()
        {
            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                ShowWarning("Dokter tidak ditemukan.");
                return;
            }

            if (paramedic.IsUsingESignature != true)
            {
                ShowWarning("Dokter ini tidak menggunakan tanda tangan elektronik.");
                return;
            }

            var config = new TilakaConfig();
            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            if (record == null)
            {
                ShowWarning("Data pendaftaran Tilaka tidak ditemukan.");
                return;
            }

            var consentText = config.ReadConsentText();
            if (string.IsNullOrWhiteSpace(consentText))
            {
                ShowWarning("File TnC Tilaka tidak ditemukan, kosong, atau tidak dapat diakses: " + config.ConsentTextFile);
                return;
            }

            var store = new TilakaKtpFileStore(config.UploadKtpPath, config.MaxKtpSizeKb);
            var ktpBytes = store.Load(record.KtpFilePath);
            if (ktpBytes == null)
            {
                ShowWarning("Foto KTP belum tersedia.");
                return;
            }

            if (!record.DateExpire.HasValue)
                record.DateExpire = DateTime.Now.AddDays(config.RegistrationExpireDays);

            var consentTimestamp = record.ConsentTimestamp ?? DateTime.Now;
            var request = new RegisterForKycCheckRequest
            {
                RegistrationId = record.RegistrationRequestID,
                Email = paramedic.Email,
                Name = paramedic.NamaKTP,
                Nik = paramedic.Ssn,
                PhotoKtp = "data:image/jpeg;base64," + Convert.ToBase64String(ktpBytes),
                DateExpire = record.DateExpire.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                IsApproved = true,
                ConsentText = consentText,
                Version = config.ConsentVersion,
                ConsentTimestamp = consentTimestamp.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
            };
            request.HashConsent = TilakaSignature.CreateConsentHash(config.ChannelSecret, config.ChannelId,
                request.ConsentText, request.Version, request.ConsentTimestamp);

            var service = new Service();
            var response = service.RegisterForKycCheck(request);
            string message;
            if (!service.IsOk(response, out message))
            {
                ShowWarning(string.IsNullOrWhiteSpace(message)
                    ? "Permintaan KYC Tilaka gagal."
                    : message);
                return;
            }

            record.RegisteredName = request.Name;
            record.RegisteredNIK = request.Nik;
            record.RegisteredEmail = request.Email;
            record.RegistrationDateTime = DateTime.Now;
            record.RegistrationState = TilakaRegistrationState.MenungguKyc;
            record.ConsentVersion = config.ConsentVersion;
            record.ConsentTimestamp = consentTimestamp;
            record.ConsentApprovedByUserID = AppSession.UserLogin.UserID;
            repository.Save(record);

            LoadData();
            ShowSuccess("Data KYC terkirim. Silakan buka webview registrasi.");
        }

        protected void btnCekStatus_OnClick(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                CheckStatus();
            }
            catch (Exception)
            {
                ShowWarning("Gagal memeriksa status Tilaka. Silakan coba lagi atau hubungi administrator.");
            }
        }

        private void CheckStatus()
        {
            if (string.IsNullOrWhiteSpace(ParamedicID))
            {
                ShowWarning("Dokter tidak ditemukan.");
                return;
            }

            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            if (record == null)
            {
                ShowWarning("Data pendaftaran Tilaka tidak ditemukan.");
                return;
            }

            var service = new Service();
            string message;
            var certificateRejected = false;
            var statusUnchanged = false;

            if (!string.IsNullOrWhiteSpace(record.RegistrationRequestID))
            {
                var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                if (!service.IsOk(registration, out message))
                {
                    ShowWarning(message);
                    return;
                }

                record.ApplyUserRegistrationStatus(registration.Data);
            }

            if (!string.IsNullOrWhiteSpace(record.TilakaName))
            {
                var certificate = service.CheckCertificateStatus(record.TilakaName);
                if (!service.IsOk(certificate, out message))
                {
                    ShowWarning(message);
                    return;
                }

                statusUnchanged = record.CertificateStatus == certificate.Status;
                if (TilakaCertificateStatusRules.CanApply(record.CertificateStatus, certificate.Status))
                {
                    record.CertificateStatus = certificate.Status;
                    record.CertificateStatusInfo = certificate.Message == null ? null : certificate.Message.Info;
                    if (certificate.Data != null && certificate.Data.Length > 0)
                    {
                        record.CertificateSerialNumber = certificate.Data[0].SerialNumber;
                        record.CertificateStartDate = ParseDate(certificate.Data[0].StartActiveDate);
                        record.CertificateExpiryDate = ParseDate(certificate.Data[0].ExpiryDate);
                    }
                }
                else
                {
                    // Status mundur/tidak valid dari Tilaka tidak boleh menimpa status yang sudah lebih maju.
                    certificateRejected = true;
                }
            }

            record.LastErrorMessage = null;
            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Manual;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            repository.Save(record);

            var state = record.RegistrationState;
            LoadData();

            if (statusUnchanged)
                ShowSuccess("Status sudah " + state +
                            ". Tidak ada perubahan status sertifikat.");
            else if (certificateRejected)
                ShowWarning("Status diperbarui: " + state +
                            ". Status sertifikat dari Tilaka diabaikan karena tidak lebih maju.");
            else
                ShowSuccess("Status diperbarui: " + state);
        }

        private bool IsNikUsedByOther(string ssn)
        {
            if (string.IsNullOrWhiteSpace(ssn)) return false;

            var query = new ParamedicQuery();
            query.Where(query.Ssn == ssn, query.ParamedicID != ParamedicID);
            query.es.Top = 1;
            return new Paramedic().Load(query);
        }

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            DateTime parsed;
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                return parsed;
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out parsed))
                return parsed;
            return null;
        }
    }
}
