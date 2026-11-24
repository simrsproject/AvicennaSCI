using System;
using System.Collections.Generic;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignSignerInput
    {
        public string ParamedicID { get; set; }
        public string UserIdentifier { get; set; }
        public string Email { get; set; }
        public string SignatureImageBase64 { get; set; }
    }

    public class TilakaQuickSignSubmitResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public TilakaQuickSignRecord Record { get; set; }
        public IList<TilakaQuickSignSignerRecord> Signers { get; set; }
    }

    public class TilakaQuickSignRequestService
    {
        private readonly Service _service;
        private readonly TilakaConfig _config;
        private readonly ITilakaQuickSignRepository _repository;
        private readonly ITilakaCasemixFileRepository _casemixRepository;
        private readonly ITilakaTemplateSignFileRepository _templateRepository;
        private readonly ITilakaRegistrationRepository _registrationRepository;
        private readonly ITilakaUserProfileProvider _profileProvider;
        private readonly ITilakaCasemixFileReader _fileReader;

        public TilakaQuickSignRequestService(Service service, TilakaConfig config,
            ITilakaQuickSignRepository repository,
            ITilakaCasemixFileRepository casemixRepository,
            ITilakaTemplateSignFileRepository templateRepository,
            ITilakaRegistrationRepository registrationRepository,
            ITilakaUserProfileProvider profileProvider,
            ITilakaCasemixFileReader fileReader)
        {
            _service = service;
            _config = config ?? new TilakaConfig();
            _repository = repository;
            _casemixRepository = casemixRepository;
            _templateRepository = templateRepository;
            _registrationRepository = registrationRepository;
            _profileProvider = profileProvider;
            _fileReader = fileReader;
        }

        public TilakaQuickSignSubmitResult Submit(long casemixFileId,
            IList<QuickSignSignerInput> signers, string reason, string location, string qrOption,
            string userID)
        {
            if (!_config.IsQuickSignConfigured)
                return Fail("Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).");

            var casemix = _casemixRepository.FindByID(casemixFileId);
            if (casemix == null) return Fail("File Casemix tidak ditemukan.");
            if (casemix.IsVoid) return Fail("File Casemix sudah dibatalkan.");

            if (signers == null || signers.Count == 0)
                signers = new List<QuickSignSignerInput> { new QuickSignSignerInput { ParamedicID = casemix.ParamedicID } };

            var resolved = new List<QuickSignSignerInput>();
            var primaryTemplate = default(TilakaTemplateSignFileRecord);

            for (var i = 0; i < signers.Count; i++)
            {
                var input = signers[i] ?? new QuickSignSignerInput { ParamedicID = casemix.ParamedicID };
                if (string.IsNullOrWhiteSpace(input.ParamedicID))
                    input.ParamedicID = casemix.ParamedicID;

                if (!_profileProvider.IsUsingESignature(input.ParamedicID))
                    return Fail("Dokter " + input.ParamedicID + " tidak menggunakan tanda tangan elektronik.");

                var registration = _registrationRepository.FindLatest(
                    TilakaConstant.RelatedType.Paramedic, input.ParamedicID);
                if (registration == null)
                    return Fail("Dokter " + input.ParamedicID + " belum terdaftar di Tilaka.");
                if (registration.CertificateStatus != TilakaConstant.CertificateStatus.Active)
                    return Fail("Sertifikat dokter " + input.ParamedicID + " belum aktif.");
                if (!registration.IsQuickSignUser)
                    return Fail("Dokter " + input.ParamedicID + " belum terdaftar sebagai Quick Sign user.");
                if (string.IsNullOrWhiteSpace(registration.TilakaName))
                    return Fail("Tilaka Name dokter " + input.ParamedicID + " belum tersedia.");

                var signer = new QuickSignSignerInput
                {
                    ParamedicID = input.ParamedicID,
                    UserIdentifier = string.IsNullOrWhiteSpace(input.UserIdentifier)
                        ? registration.TilakaName
                        : input.UserIdentifier.Trim(),
                    Email = string.IsNullOrWhiteSpace(input.Email)
                        ? _profileProvider.GetEmail(input.ParamedicID)
                        : input.Email.Trim(),
                    SignatureImageBase64 = input.SignatureImageBase64
                };

                if (string.IsNullOrWhiteSpace(signer.Email))
                    return Fail("Email dokter " + signer.ParamedicID + " belum diisi.");
                if (!IsBase64Image(signer.SignatureImageBase64))
                    signer.SignatureImageBase64 = _profileProvider.GetSignatureImageBase64(signer.ParamedicID);
                if (string.IsNullOrWhiteSpace(signer.SignatureImageBase64))
                    return Fail("Gambar tanda tangan dokter " + signer.ParamedicID + " belum tersedia.");

                if (primaryTemplate == null)
                {
                    primaryTemplate = _templateRepository.FindDefault(signer.ParamedicID);
                    if (primaryTemplate == null && string.IsNullOrWhiteSpace(_config.QuickSignDefaultTemplateNo))
                        return Fail("Template tanda tangan dokter " + signer.ParamedicID + " belum terdaftar.");
                }

                resolved.Add(signer);
            }

            byte[] pdf;
            try
            {
                pdf = _fileReader.Load(casemix.FilePath);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }

            var upload = _service.UploadPdf(pdf, casemix.FileName ?? "document.pdf");
            string uploadMessage;
            if (!_service.IsOk(upload, out uploadMessage) || string.IsNullOrWhiteSpace(upload.FileName))
                return Fail(uploadMessage ?? "Upload dokumen ke Tilaka gagal.");

            var requestId = Guid.NewGuid().ToString("N");
            var templateNo = primaryTemplate != null
                ? primaryTemplate.TemplateNo
                : _config.QuickSignDefaultTemplateNo;

            var record = new TilakaQuickSignRecord
            {
                RequestID = requestId,
                CasemixFileID = casemix.CasemixFileID,
                TemplateSignFileID = primaryTemplate == null ? (long?)null : primaryTemplate.TemplateSignFileID,
                FileName = upload.FileName,
                TemplateNo = templateNo,
                Status = TilakaQuickSignConstant.RequestStatus.Requested,
                IsActive = true,
                RequestDateTime = DateTime.Now,
                CreatedDateTime = DateTime.Now,
                CreatedByUserID = userID
            };

            var signerRecords = new List<TilakaQuickSignSignerRecord>();
            for (var i = 0; i < resolved.Count; i++)
            {
                signerRecords.Add(new TilakaQuickSignSignerRecord
                {
                    Sequence = i + 1,
                    UserIdentifier = resolved[i].UserIdentifier,
                    Email = resolved[i].Email,
                    Status = TilakaQuickSignConstant.SignerStatus.Unauthorized,
                    NumSignatures = 1,
                    NumSignaturesDone = 0,
                    CreatedDateTime = DateTime.Now,
                    CreatedByUserID = userID
                });
            }

            _repository.Save(record, signerRecords);

            var request = new QuickSignRequest
            {
                RequestId = requestId,
                Signatures = new QuickSignSignature[resolved.Count],
                ListPdf = new[]
                {
                    new QuickSignDocument
                    {
                        FileName = upload.FileName,
                        TemplateNo = templateNo,
                        Signatures = new QuickSignDocumentSignature[resolved.Count]
                    }
                }
            };

            for (var i = 0; i < resolved.Count; i++)
            {
                request.Signatures[i] = new QuickSignSignature
                {
                    UserIdentifier = resolved[i].UserIdentifier,
                    Email = resolved[i].Email,
                    SignatureImage = resolved[i].SignatureImageBase64,
                    Sequence = i + 1
                };

                request.ListPdf[0].Signatures[i] = new QuickSignDocumentSignature
                {
                    UserIdentifier = resolved[i].UserIdentifier,
                    Width = TilakaQuickSignConstant.SignaturePlacement.Width,
                    Height = TilakaQuickSignConstant.SignaturePlacement.Height,
                    CoordinateX = TilakaQuickSignConstant.SignaturePlacement.CoordinateX,
                    CoordinateY = TilakaQuickSignConstant.SignaturePlacement.CoordinateY,
                    PageNumber = TilakaQuickSignConstant.SignaturePlacement.PageNumber,
                    Reason = reason,
                    Location = location,
                    QrOption = qrOption
                };
            }

            var response = _service.RequestSign(request);
            string message;
            if (!_service.IsOk(response, out message))
            {
                record.Status = TilakaQuickSignConstant.RequestStatus.Failed;
                record.IsActive = false;
                record.ErrorMessage = message;
                record.CompletedDateTime = DateTime.Now;
                record.LastUpdateByUserID = userID;
                _repository.Save(record, signerRecords);
                return Fail(message, record, signerRecords);
            }

            ApplyAuthResponse(response, signerRecords, userID);

            var state = TilakaQuickSignState.Evaluate(record, signerRecords);
            record.Status = state == TilakaQuickSignConstant.RequestStatus.Completed
                ? TilakaQuickSignConstant.RequestStatus.InProgress
                : state;
            record.LastUpdateByUserID = userID;
            _repository.Save(record, signerRecords);

            _casemixRepository.UpdateLink(casemix.CasemixFileID, record.QuickSignRequestID, DateTime.Now);

            return new TilakaQuickSignSubmitResult
            {
                IsSuccess = true,
                Message = "Permintaan tanda tangan berhasil dibuat.",
                Record = record,
                Signers = signerRecords
            };
        }

        private static void ApplyAuthResponse(QuickSignRequestResponse response,
            IList<TilakaQuickSignSignerRecord> signers, string userID)
        {
            if (response.AuthResponse == null) return;

            foreach (var auth in response.AuthResponse)
            {
                foreach (var signer in signers)
                {
                    if (!string.Equals(signer.UserIdentifier, auth.UserIdentifier, StringComparison.OrdinalIgnoreCase))
                        continue;

                    signer.AuthUrl = auth.Url;
                    signer.TokenKey = auth.TokenKey;
                    signer.Status = auth.Status
                        ? TilakaQuickSignConstant.SignerStatus.Process
                        : TilakaQuickSignConstant.SignerStatus.Unauthorized;
                    signer.LastUpdateByUserID = userID;
                    signer.LastCheckedDateTime = DateTime.Now;
                }
            }
        }

        private static bool IsBase64Image(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;
            try
            {
                Convert.FromBase64String(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static TilakaQuickSignSubmitResult Fail(string message)
        {
            return new TilakaQuickSignSubmitResult { IsSuccess = false, Message = message };
        }

        private static TilakaQuickSignSubmitResult Fail(string message, TilakaQuickSignRecord record,
            IList<TilakaQuickSignSignerRecord> signers)
        {
            return new TilakaQuickSignSubmitResult
            {
                IsSuccess = false,
                Message = message,
                Record = record,
                Signers = signers
            };
        }
    }
}
