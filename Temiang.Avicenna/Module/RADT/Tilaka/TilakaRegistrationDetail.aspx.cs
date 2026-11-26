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

            var canReEnroll = isUsingESignature && CanReEnroll(state, record)
                              && record != null
                              && !string.IsNullOrWhiteSpace(record.TilakaName);
            var canStartRegistration = isUsingESignature
                                       && CanStartRegistration(state)
                                       && !canReEnroll;
            var showConsent = canStartRegistration || canReEnroll;

            pnlConsent.Visible = showConsent;
            chkConsent.Visible = showConsent;
            btnDaftar.Visible = canStartRegistration;
            btnReEnroll.Visible = canReEnroll;
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

        private static bool CanReEnroll(string state, TilakaRegistrationRecord record)
        {
            if (state == TilakaRegistrationState.Kedaluwarsa
                || state == TilakaRegistrationState.Dicabut)
                return true;

            return record != null
                   && string.Equals(record.RequestType,
                       TilakaConstant.RequestType.ReEnroll, StringComparison.OrdinalIgnoreCase)
                   && state == TilakaRegistrationState.KycGagal;
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

        private void ShowInfo(string message)
        {
            HideInformationHeader();
            lblMessage.Text = TilakaAlertHtml.Info(message);
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
            btnReEnroll.Visible = false;
            btnKirimData.Visible = false;
            btnWebview.Visible = false;
            btnCekStatus.Visible = false;
        }

        private static string BuildWebviewUrl(TilakaConfig config, TilakaRegistrationRecord record, string state)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.RegistrationRequestID)) return string.Empty;
            if (state == TilakaRegistrationState.Kedaluwarsa
                || state == TilakaRegistrationState.Dicabut
                || state == TilakaRegistrationState.Ditolak) return string.Empty;

            var isReEnroll = string.Equals(record.RequestType,
                TilakaConstant.RequestType.ReEnroll, StringComparison.OrdinalIgnoreCase);

            if (state == TilakaRegistrationState.SertifikatTerbit)
            {
                return isReEnroll
                    ? TilakaUrls.ActivationWebviewByIssueId(config, record.RegistrationRequestID)
                    : TilakaUrls.ActivationWebview(config, record.RegistrationRequestID);
            }

            return isReEnroll
                ? TilakaUrls.ReEnrollWebview(config, record.RegistrationRequestID)
                : TilakaUrls.RegistrationWebview(config, record.RegistrationRequestID);
        }

        protected void btnReEnroll_OnClick(object sender, EventArgs e)
        {
            ClearMessages();
            try
            {
                StartReEnroll();
            }
            catch (InvalidOperationException ex)
            {
                ShowWarning(ex.Message);
            }
            catch (Exception)
            {
                ShowWarning("Gagal memproses re-enroll Tilaka. Silakan coba lagi atau hubungi administrator.");
            }
        }

        private void StartReEnroll()
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

            if (!chkConsent.Checked)
            {
                ShowWarning("Persetujuan TnC Tilaka belum dicentang.");
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
            if (latest == null || string.IsNullOrWhiteSpace(latest.TilakaName))
            {
                ShowWarning("Tilaka Name dokter belum tersedia.");
                return;
            }

            var state = TilakaRegistrationState.Evaluate(latest, DateTime.Now);
            if (!CanReEnroll(state, latest))
            {
                ShowWarning("Re-enroll hanya tersedia untuk sertifikat kedaluwarsa atau dicabut.");
                return;
            }

            var service = new Service();
            var uuid = service.GenerateUuidReEnroll(latest.TilakaName);
            string message;
            if (!service.IsOk(uuid, out message) || uuid.Data == null || uuid.Data.Length == 0)
            {
                ShowWarning(string.IsNullOrWhiteSpace(message)
                    ? "Gagal mendapatkan UUID re-enroll Tilaka."
                    : message);
                return;
            }

            repository.Save(new TilakaRegistrationRecord
            {
                RelatedType = TilakaConstant.RelatedType.Paramedic,
                RelatedID = ParamedicID,
                IsLatest = true,
                RegistrationRequestID = uuid.Data[0],
                RequestType = TilakaConstant.RequestType.ReEnroll,
                DateExpire = DateTime.Now.AddDays(3),
                RegistrationState = TilakaRegistrationState.UuidTerbit,
                RegisteredName = paramedic.NamaKTP,
                RegisteredNIK = paramedic.Ssn,
                RegisteredEmail = paramedic.Email,
                TilakaName = latest.TilakaName,
                KtpFilePath = latest.KtpFilePath,
                KtpFileName = latest.KtpFileName,
                KtpContentType = latest.KtpContentType,
                KtpUploadedDateTime = latest.KtpUploadedDateTime,
                CertificateStatus = null,
                CertificateDataStatus = null,
                ConsentVersion = config.ConsentVersion,
                ConsentApprovedByUserID = AppSession.UserLogin.UserID,
                LastUpdateByUserID = AppSession.UserLogin.UserID
            });

            LoadData();
            ShowSuccess("UUID re-enroll berhasil dibuat. Lanjutkan dengan Kirim Data KYC.");
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

            if (string.Equals(record.RequestType,
                TilakaConstant.RequestType.ReEnroll, StringComparison.OrdinalIgnoreCase))
            {
                SendReEnrollKyc(record, config, consentText, repository);
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

        private void SendReEnrollKyc(TilakaRegistrationRecord record, TilakaConfig config,
            string consentText, TilakaRegistrationRepository repository)
        {
            var consentTimestamp = DateTime.Now;
            var request = new RegisterForKycCheckRequest
            {
                RegistrationId = record.RegistrationRequestID,
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
                    ? "Permintaan KYC re-enroll Tilaka gagal."
                    : message);
                return;
            }

            record.RegistrationDateTime = DateTime.Now;
            record.RegistrationState = TilakaRegistrationState.MenungguKyc;
            record.KycStatus = null;
            record.KycReasonCode = null;
            record.ConsentVersion = config.ConsentVersion;
            record.ConsentTimestamp = consentTimestamp;
            record.ConsentApprovedByUserID = AppSession.UserLogin.UserID;
            record.LastUpdateByUserID = AppSession.UserLogin.UserID;
            repository.Save(record);

            LoadData();
            ShowSuccess("Data KYC re-enroll terkirim. Silakan buka webview re-enroll.");
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

            var isReEnroll = string.Equals(record.RequestType,
                TilakaConstant.RequestType.ReEnroll, StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(record.RegistrationRequestID))
            {
                if (isReEnroll)
                {
                    var reEnroll = service.GetUserReEnrollStatus(record.RegistrationRequestID);
                    if (!service.IsOk(reEnroll, out message))
                    {
                        ShowWarning(message);
                        return;
                    }

                    record.ApplyUserReEnrollStatus(reEnroll.Data);

                    record.LastCheckedDateTime = DateTime.Now;
                    record.LastCheckSource = TilakaConstant.LogSource.Manual;
                    record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                    repository.Save(record);

                    if (!string.Equals(record.KycStatus,
                        TilakaConstant.KycStatus.Success, StringComparison.OrdinalIgnoreCase))
                    {
                        LoadData();

                        if (string.Equals(record.KycStatus,
                            TilakaConstant.KycStatus.Fail, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(record.KycStatus,
                                TilakaConstant.KycStatus.Error, StringComparison.OrdinalIgnoreCase))
                        {
                            ShowWarning("KYC re-enroll gagal" +
                                        (record.KycReasonCode.HasValue
                                            ? " (reason code " + record.KycReasonCode.Value + ")."
                                            : "."));
                        }
                        else
                        {
                            ShowInfo("Re-enroll masih dalam proses KYC/liveness.");
                        }
                        return;
                    }
                }
                else
                {
                    var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                    if (!service.IsOk(registration, out message))
                    {
                        ShowWarning(message);
                        return;
                    }

                    record.ApplyUserRegistrationStatus(registration.Data);
                }
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
                    record.ApplyCertificateStatus(certificate);
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
