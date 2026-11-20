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
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                HideActions();
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            lblParamedicName.Text = paramedic.ParamedicName;
            lblNamaKTP.Text = paramedic.NamaKTP;
            lblSsn.Text = paramedic.Ssn;
            lblEmail.Text = paramedic.Email;

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

            chkConsent.Visible = state == TilakaRegistrationState.BelumTerdaftar;
            btnDaftar.Visible = state == TilakaRegistrationState.BelumTerdaftar;
            btnKirimData.Visible = state == TilakaRegistrationState.UuidTerbit;
            btnWebview.Visible = !string.IsNullOrWhiteSpace(webviewUrl)
                                 && IsWebviewState(state);
            btnCekStatus.Visible = record != null && state != TilakaRegistrationState.BelumTerdaftar;

            ConfigureConsent(config);
        }

        private static bool IsWebviewState(string state)
        {
            return state == TilakaRegistrationState.MenungguKyc
                   || state == TilakaRegistrationState.KycGagal
                   || state == TilakaRegistrationState.Kedaluwarsa
                   || state == TilakaRegistrationState.SertifikatTerbit;
        }

        private void ConfigureConsent(TilakaConfig config)
        {
            var consentText = config.ReadConsentText();
            if (string.IsNullOrWhiteSpace(consentText))
            {
                lblMessage.Text = "File TnC Tilaka tidak ditemukan atau kosong: " + config.ConsentTextFile;
                if (btnDaftar.Visible)
                {
                    chkConsent.Enabled = false;
                    btnDaftar.Enabled = false;
                }
            }

            var isWebPath = !string.IsNullOrWhiteSpace(config.ConsentTextFile)
                            && config.ConsentTextFile.StartsWith("~/", StringComparison.Ordinal);
            lnkConsent.Visible = isWebPath;
            if (isWebPath) lnkConsent.NavigateUrl = ResolveUrl(config.ConsentTextFile);
        }

        private void HideActions()
        {
            chkConsent.Visible = false;
            uplKtp.Visible = false;
            btnDaftar.Visible = false;
            btnKirimData.Visible = false;
            btnWebview.Visible = false;
            btnCekStatus.Visible = false;
        }

        private static string BuildWebviewUrl(TilakaConfig config, TilakaRegistrationRecord record, string state)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.RegistrationRequestID)) return string.Empty;
            if (state == TilakaRegistrationState.SertifikatTerbit)
                return TilakaUrls.ActivationWebview(config, record.RegistrationRequestID);
            return TilakaUrls.RegistrationWebview(config, record.RegistrationRequestID);
        }

        protected void btnDaftar_OnClick(object sender, EventArgs e)
        {
            try
            {
                RegisterRegistration();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception)
            {
                lblMessage.Text = "Gagal memproses pendaftaran Tilaka. Silakan coba lagi atau hubungi administrator.";
            }
        }

        private void RegisterRegistration()
        {
            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            var config = new TilakaConfig();
            if (string.IsNullOrWhiteSpace(config.ReadConsentText()))
            {
                lblMessage.Text = "File TnC Tilaka tidak ditemukan atau kosong: " + config.ConsentTextFile;
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
                lblMessage.Text = guard.Message;
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
                        ktpPath = store.Save(ParamedicID, stream.ToArray(), file.ContentType, DateTime.Now);
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
                lblMessage.Text = string.IsNullOrWhiteSpace(message)
                    ? "Gagal mendapatkan UUID Tilaka."
                    : message;
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
            lblMessage.Text = "UUID Tilaka berhasil dibuat. Lanjutkan dengan Kirim Data KYC.";
        }

        protected void btnKirimData_OnClick(object sender, EventArgs e)
        {
            try
            {
                SendKycData();
            }
            catch (InvalidOperationException ex)
            {
                lblMessage.Text = ex.Message;
            }
            catch (Exception)
            {
                lblMessage.Text = "Gagal mengirim data KYC Tilaka. Silakan coba lagi atau hubungi administrator.";
            }
        }

        private void SendKycData()
        {
            var paramedic = new Paramedic();
            if (!paramedic.LoadByPrimaryKey(ParamedicID))
            {
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            var config = new TilakaConfig();
            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            if (record == null)
            {
                lblMessage.Text = "Data pendaftaran Tilaka tidak ditemukan.";
                return;
            }

            var consentText = config.ReadConsentText();
            if (string.IsNullOrWhiteSpace(consentText))
            {
                lblMessage.Text = "File TnC Tilaka tidak ditemukan atau kosong: " + config.ConsentTextFile;
                return;
            }

            var store = new TilakaKtpFileStore(config.UploadKtpPath, config.MaxKtpSizeKb);
            var ktpBytes = store.Load(record.KtpFilePath);
            if (ktpBytes == null)
            {
                lblMessage.Text = "Foto KTP belum tersedia.";
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
                lblMessage.Text = string.IsNullOrWhiteSpace(message)
                    ? "Permintaan KYC Tilaka gagal."
                    : message;
                return;
            }

            record.RegisteredName = request.Name;
            record.RegisteredNIK = request.Nik;
            record.RegisteredEmail = request.Email;
            record.RegistrationDateTime = DateTime.Now;
            record.RegistrationState = TilakaRegistrationState.MenungguKyc;
            repository.Save(record);

            LoadData();
            lblMessage.Text = "Data KYC terkirim. Silakan buka webview registrasi.";
        }

        protected void btnCekStatus_OnClick(object sender, EventArgs e)
        {
            try
            {
                CheckStatus();
            }
            catch (Exception)
            {
                lblMessage.Text = "Gagal memeriksa status Tilaka. Silakan coba lagi atau hubungi administrator.";
            }
        }

        private void CheckStatus()
        {
            if (string.IsNullOrWhiteSpace(ParamedicID))
            {
                lblMessage.Text = "Dokter tidak ditemukan.";
                return;
            }

            var repository = new TilakaRegistrationRepository();
            var record = repository.FindLatest(TilakaConstant.RelatedType.Paramedic, ParamedicID);
            if (record == null)
            {
                lblMessage.Text = "Data pendaftaran Tilaka tidak ditemukan.";
                return;
            }

            var service = new Service();
            string message;

            if (string.IsNullOrWhiteSpace(record.TilakaName))
            {
                var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                if (!service.IsOk(registration, out message))
                {
                    lblMessage.Text = message;
                    return;
                }

                if (registration.Data != null)
                {
                    record.TilakaName = registration.Data.TilakaName;
                    record.KycStatus = registration.Data.Status;
                    record.ManualRegistrationStatus = registration.Data.ManualRegistrationStatus;
                }
            }

            if (!string.IsNullOrWhiteSpace(record.TilakaName))
            {
                var certificate = service.CheckCertificateStatus(record.TilakaName);
                if (!service.IsOk(certificate, out message))
                {
                    lblMessage.Text = message;
                    return;
                }

                record.CertificateStatus = certificate.Status;
                record.CertificateStatusInfo = certificate.Message == null ? null : certificate.Message.Info;
                if (certificate.Data != null && certificate.Data.Length > 0)
                {
                    record.CertificateSerialNumber = certificate.Data[0].SerialNumber;
                    record.CertificateStartDate = ParseDate(certificate.Data[0].StartActiveDate);
                    record.CertificateExpiryDate = ParseDate(certificate.Data[0].ExpiryDate);
                }
            }

            record.LastErrorMessage = null;
            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Manual;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            repository.Save(record);

            var state = record.RegistrationState;
            LoadData();
            lblMessage.Text = "Status diperbarui: " + state;
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
