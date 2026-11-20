using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaCallbackOutcome
    {
        public bool Applied { get; set; }
        public string Reason { get; set; }
    }

    public class TilakaCallbackProcessor
    {
        private readonly ITilakaRegistrationRepository _repository;

        public TilakaCallbackProcessor(ITilakaRegistrationRepository repository)
        {
            _repository = repository;
        }

        public TilakaCallbackOutcome ProcessRegistration(CallbackRegistrationRequest payload)
        {
            if (payload == null || string.IsNullOrWhiteSpace(payload.RegisterId))
                return NotApplied("register-id-missing");

            var record = _repository.FindByRequestId(payload.RegisterId);
            if (record == null) return NotApplied("register-id-unknown");

            var data = payload.Data;
            if (data != null)
            {
                if (!string.IsNullOrWhiteSpace(data.Status)) record.KycStatus = data.Status;
                if (!string.IsNullOrWhiteSpace(data.ReasonCode)) record.KycReasonCode = ParseInt(data.ReasonCode);
                if (!string.IsNullOrWhiteSpace(data.ManualRegistrationStatus))
                    record.ManualRegistrationStatus = data.ManualRegistrationStatus;
                if (!string.IsNullOrWhiteSpace(data.TilakaName)) record.TilakaName = data.TilakaName;
            }
            else if (!payload.Success && !string.IsNullOrWhiteSpace(payload.Message))
            {
                record.LastErrorMessage = payload.Message;
            }

            if (record.RegistrationDateTime.HasValue == false && !string.IsNullOrWhiteSpace(record.KycStatus))
                record.RegistrationDateTime = DateTime.Now;

            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Callback;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            _repository.Save(record);

            return new TilakaCallbackOutcome { Applied = true };
        }

        public TilakaCallbackOutcome ProcessCertificateStatus(CallbackCertificateStatusRequest payload, DateTime now)
        {
            if (payload == null || string.IsNullOrWhiteSpace(payload.RequestId))
                return NotApplied("request-id-missing");

            var record = _repository.FindByRequestId(payload.RequestId);
            if (record == null) return NotApplied("request-id-unknown");

            if (!payload.Success) return NotApplied("success-false");

            if (payload.Status < 0 || payload.Status > 4) return NotApplied("status-invalid");

            if (!TilakaCertificateStatusRules.CanApply(record.CertificateStatus, payload.Status))
                return NotApplied("status-regression");

            record.CertificateStatus = payload.Status;
            if (!string.IsNullOrWhiteSpace(payload.UserIdentifier) && string.IsNullOrWhiteSpace(record.TilakaName))
                record.TilakaName = payload.UserIdentifier;

            record.LastCheckedDateTime = now;
            record.LastCheckSource = TilakaConstant.LogSource.Callback;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, now);
            _repository.Save(record);

            return new TilakaCallbackOutcome { Applied = true };
        }

        private static TilakaCallbackOutcome NotApplied(string reason)
        {
            return new TilakaCallbackOutcome { Applied = false, Reason = reason };
        }

        private static int? ParseInt(string value)
        {
            int parsed;
            return int.TryParse(value, out parsed) ? parsed : (int?)null;
        }
    }
}
