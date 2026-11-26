using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaReEnrollReturnHandler
    {
        private readonly ITilakaRegistrationRepository _repository;
        private readonly Service _service;

        public TilakaReEnrollReturnHandler(ITilakaRegistrationRepository repository, Service service)
        {
            _repository = repository;
            _service = service;
        }

        public TilakaWebviewReturnResult Process(string issueId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(issueId))
                    return Fail("Parameter issue_id tidak ditemukan.");

                var record = _repository.FindByRequestId(issueId);
                if (record == null)
                    return Fail("Data re-enroll tidak ditemukan.");

                var response = _service.GetUserReEnrollStatus(issueId);
                string message;
                if (!_service.IsOk(response, out message))
                    return Fail(message);

                record.ApplyUserReEnrollStatus(response.Data);
                record.LastCheckedDateTime = DateTime.Now;
                record.LastCheckSource = TilakaConstant.LogSource.Manual;
                record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                _repository.Save(record);

                return new TilakaWebviewReturnResult
                {
                    IsSuccess = true,
                    Message = "Re-enroll diperbarui. Status: " + record.RegistrationState
                };
            }
            catch (Exception)
            {
                return Fail("Gagal memproses hasil re-enroll Tilaka. Silakan gunakan tombol Cek Status.");
            }
        }

        private static TilakaWebviewReturnResult Fail(string message)
        {
            return new TilakaWebviewReturnResult { IsSuccess = false, Message = message };
        }
    }
}
