using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    /// <summary>
    /// Menangani redirect balik dari webview Tilaka: memvalidasi request-id,
    /// menarik status sertifikat terbaru, lalu menyimpannya.
    /// </summary>
    public class TilakaWebviewReturnHandler
    {
        private readonly ITilakaRegistrationRepository _repository;
        private readonly Service _service;

        public TilakaWebviewReturnHandler(ITilakaRegistrationRepository repository, Service service)
        {
            _repository = repository;
            _service = service;
        }

        public TilakaWebviewReturnResult Process(string requestId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestId))
                    return Fail("Parameter request-id tidak ditemukan.");

                var record = _repository.FindByRequestId(requestId);
                if (record == null)
                    return Fail("Data pendaftaran tidak ditemukan.");

                if (string.IsNullOrWhiteSpace(record.TilakaName))
                    return Fail("Nama Tilaka belum tersedia. Silakan gunakan tombol Cek Status di Avicenna.");

                var certificate = _service.CheckCertificateStatus(record.TilakaName);
                string message;
                if (!_service.IsOk(certificate, out message))
                    return Fail(message);

                if (TilakaCertificateStatusRules.CanApply(record.CertificateStatus, certificate.Status))
                {
                    record.ApplyCertificateStatus(certificate);
                }

                record.LastErrorMessage = null;
                record.LastCheckedDateTime = DateTime.Now;
                record.LastCheckSource = TilakaConstant.LogSource.Manual;
                record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                _repository.Save(record);

                return new TilakaWebviewReturnResult
                {
                    IsSuccess = true,
                    Message = "Aktivasi selesai. Status: " + record.RegistrationState
                };
            }
            catch (Exception)
            {
                return Fail("Gagal memproses hasil aktivasi Tilaka. Silakan gunakan tombol Cek Status di Avicenna.");
            }
        }

        private static TilakaWebviewReturnResult Fail(string message)
        {
            return new TilakaWebviewReturnResult { IsSuccess = false, Message = message };
        }
    }
}
