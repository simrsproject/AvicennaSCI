using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaRegistrationRecord
    {
        public long TilakaRegistrationID { get; set; }
        public string RelatedType { get; set; }
        public string RelatedID { get; set; }
        public bool IsLatest { get; set; }
        public string RegistrationState { get; set; }

        public string RegistrationRequestID { get; set; }
        public string RequestType { get; set; }
        public DateTime? RegistrationDateTime { get; set; }
        public DateTime? DateExpire { get; set; }

        public string RegisteredName { get; set; }
        public string RegisteredNIK { get; set; }
        public string RegisteredEmail { get; set; }

        public string KycStatus { get; set; }
        public int? KycReasonCode { get; set; }
        public string ManualRegistrationStatus { get; set; }
        public string TilakaName { get; set; }

        public int? CertificateStatus { get; set; }
        public string CertificateStatusInfo { get; set; }
        public string CertificateSerialNumber { get; set; }
        public DateTime? CertificateStartDate { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }

        public string ConsentVersion { get; set; }
        public DateTime? ConsentTimestamp { get; set; }
        public string ConsentApprovedByUserID { get; set; }

        public string KtpFilePath { get; set; }
        public string KtpFileName { get; set; }
        public string KtpContentType { get; set; }
        public DateTime? KtpUploadedDateTime { get; set; }

        public DateTime? LastCheckedDateTime { get; set; }
        public string LastCheckSource { get; set; }
        public string LastErrorMessage { get; set; }
    }
}
