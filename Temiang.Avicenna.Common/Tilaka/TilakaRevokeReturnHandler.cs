using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaRevokeReturnHandler
    {
        private readonly ITilakaRegistrationRepository _repository;
        private readonly Service _service;

        public TilakaRevokeReturnHandler(
            ITilakaRegistrationRepository repository, Service service)
        {
            _repository = repository;
            _service = service;
        }

        public TilakaWebviewReturnResult Process(
            string requestId, string revokeId, string status)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestId))
                    return Fail("Parameter request-id tidak ditemukan.");

                var record = _repository.FindByRequestId(requestId);
                if (record == null)
                    return Fail("Data pendaftaran tidak ditemukan.");

                if (string.IsNullOrWhiteSpace(status))
                    return Fail("Status revoke tidak ditemukan.");

                if (!string.Equals(record.RevokeStatus,
                        TilakaConstant.RevokeStatus.Requested,
                        StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(record.RevokeStatus,
                            TilakaConstant.RevokeStatus.Success,
                            StringComparison.OrdinalIgnoreCase))
                        return new TilakaWebviewReturnResult
                        {
                            IsSuccess = true,
                            Message = "Revoke sertifikat sudah berhasil."
                        };

                    return Fail("Tidak ada pengajuan revoke yang sedang diproses.");
                }

                if (string.IsNullOrWhiteSpace(revokeId)
                    || string.IsNullOrWhiteSpace(record.RevokeRequestID)
                    || !string.Equals(revokeId, record.RevokeRequestID,
                        StringComparison.OrdinalIgnoreCase))
                {
                    return Fail("ID revoke tidak cocok dengan pengajuan.");
                }

                var now = DateTime.Now;
                record.RevokeCompletedDateTime = now;
                record.LastCheckedDateTime = now;
                record.LastCheckSource = TilakaConstant.LogSource.Revoke;

                if (!IsSuccess(status))
                {
                    record.RevokeStatus = TilakaConstant.RevokeStatus.Failed;
                    record.RevokeMessage = "Liveness revoke gagal. Sertifikat belum dicabut.";
                    record.RegistrationState = TilakaRegistrationState.Evaluate(record, now);
                    _repository.Save(record);

                    return new TilakaWebviewReturnResult
                    {
                        IsSuccess = false,
                        Message = record.RevokeMessage
                    };
                }

                record.RevokeStatus = TilakaConstant.RevokeStatus.Success;
                record.CertificateDataStatus = TilakaConstant.CertificateDataStatus.Revoke;
                record.RevokeMessage = "Revoke sertifikat berhasil.";
                record.RegistrationState = TilakaRegistrationState.Dicabut;

                if (!string.IsNullOrWhiteSpace(record.TilakaName))
                {
                    var certificate = _service.CheckCertificateStatus(record.TilakaName);
                    string message;
                    if (_service.IsOk(certificate, out message))
                    {
                        record.ApplyCertificateStatus(certificate);
                        record.RegistrationState = TilakaRegistrationState.Dicabut;
                    }
                }

                record.CertificateStatus = TilakaConstant.CertificateStatus.None;
                record.CertificateStatusInfo = "Belum memiliki sertifikat";
                _repository.Save(record);

                return new TilakaWebviewReturnResult
                {
                    IsSuccess = true,
                    Message = record.RevokeMessage
                };
            }
            catch (Exception)
            {
                return Fail("Gagal memproses hasil revoke. Silakan gunakan tombol Cek Status.");
            }
        }

        private static bool IsSuccess(string status)
        {
            var normalized = (status ?? string.Empty).Trim();
            return string.Equals(normalized, "Sukses", StringComparison.OrdinalIgnoreCase)
                   || string.Equals(normalized, "Success", StringComparison.OrdinalIgnoreCase);
        }

        private static TilakaWebviewReturnResult Fail(string message)
        {
            return new TilakaWebviewReturnResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
