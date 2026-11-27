using System;
using System.Globalization;

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
        public string CertificateDataStatus { get; set; }
        public string CertificateSerialNumber { get; set; }
        public DateTime? CertificateStartDate { get; set; }
        public DateTime? CertificateExpiryDate { get; set; }

        public string RevokeRequestID { get; set; }
        public string RevokeReason { get; set; }
        public string RevokeNote { get; set; }
        public string RevokeStatus { get; set; }
        public string RevokeMessage { get; set; }
        public DateTime? RevokeRequestedDateTime { get; set; }
        public string RevokeRequestedByUserID { get; set; }
        public DateTime? RevokeCompletedDateTime { get; set; }

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

        public bool IsQuickSignUser { get; set; }
        public DateTime? QuickSignRegisteredDateTime { get; set; }
        public string QuickSignLastErrorMessage { get; set; }
        public string LastUpdateByUserID { get; set; }

        /// <summary>
        /// Menerapkan hasil /userregstatus secara field-by-field; nilai kosong tidak menimpa data lama.
        /// </summary>
        public void ApplyUserReEnrollStatus(TilakaUserReEnrollData data)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Status)) return;

            // C adalah status interim; diperlakukan seperti proses KYC berjalan.
            KycStatus = string.Equals(data.Status, "C", StringComparison.OrdinalIgnoreCase)
                ? TilakaConstant.KycStatus.InVerification
                : data.Status;

            int reasonCode;
            KycReasonCode = int.TryParse(data.ReasonCode, out reasonCode) ? reasonCode : (int?)null;

            if (!string.IsNullOrWhiteSpace(data.LivenessFailMessage))
                LastErrorMessage = data.LivenessFailMessage;
        }

        public void ApplyCertificateStatus(CheckCertStatusResponse certificate)
        {
            if (certificate == null) return;

            CertificateStatus = certificate.Status;
            CertificateStatusInfo = certificate.Message == null ? null : certificate.Message.Info;

            var isRevoked = string.Equals(CertificateDataStatus,
                                TilakaConstant.CertificateDataStatus.Revoke,
                                StringComparison.OrdinalIgnoreCase)
                            || string.Equals(RevokeStatus,
                                TilakaConstant.RevokeStatus.Success,
                                StringComparison.OrdinalIgnoreCase);

            if (isRevoked)
            {
                CertificateDataStatus = TilakaConstant.CertificateDataStatus.Revoke;
                return;
            }

            if (certificate.Data != null && certificate.Data.Length > 0)
            {
                var data = certificate.Data[0];
                CertificateDataStatus = data.Status;
                CertificateSerialNumber = data.SerialNumber;
                CertificateStartDate = ParseDate(data.StartActiveDate);
                CertificateExpiryDate = ParseDate(data.ExpiryDate);
            }
            else if (certificate.Status == TilakaConstant.CertificateStatus.None)
            {
                CertificateDataStatus = null;
            }
        }

        public void ApplyUserRegistrationStatus(TilakaKycData data)
        {
            if (data == null) return;

            if (!string.IsNullOrWhiteSpace(data.TilakaName)) TilakaName = data.TilakaName;
            if (!string.IsNullOrWhiteSpace(data.Status)) KycStatus = data.Status;
            if (!string.IsNullOrWhiteSpace(data.ManualRegistrationStatus))
                ManualRegistrationStatus = data.ManualRegistrationStatus;

            if (!string.IsNullOrWhiteSpace(data.ReasonCode))
            {
                int reasonCode;
                if (int.TryParse(data.ReasonCode, out reasonCode)) KycReasonCode = reasonCode;
            }
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
