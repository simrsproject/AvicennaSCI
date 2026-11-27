namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaRegistrationState
    {
        public const string BelumTerdaftar = "BelumTerdaftar";
        public const string UuidTerbit = "UuidTerbit";
        public const string MenungguKyc = "MenungguKyc";
        public const string KycGagal = "KycGagal";
        public const string MenungguVerifikasi = "MenungguVerifikasi";
        public const string SertifikatTerbit = "SertifikatTerbit";
        public const string Aktif = "Aktif";
        public const string Ditolak = "Ditolak";
        public const string Kedaluwarsa = "Kedaluwarsa";
        public const string Dicabut = "Dicabut";

        public static string Evaluate(TilakaRegistrationRecord record, System.DateTime now)
        {
            if (record == null) return BelumTerdaftar;

            if (IsRevoked(record)) return Dicabut;

            var cert = record.CertificateStatus ?? -1;
            if (cert == TilakaConstant.CertificateStatus.Active)
            {
                if (record.CertificateExpiryDate.HasValue && record.CertificateExpiryDate.Value < now)
                    return Kedaluwarsa;
                return Aktif;
            }
            if (cert == TilakaConstant.CertificateStatus.Rejected) return Ditolak;
            if (cert == TilakaConstant.CertificateStatus.Registered) return SertifikatTerbit;
            if (cert == TilakaConstant.CertificateStatus.InVerification) return MenungguVerifikasi;

            if (record.CertificateExpiryDate.HasValue && record.CertificateExpiryDate.Value < now)
                return Kedaluwarsa;

            if (record.ManualRegistrationStatus == TilakaConstant.ManualRegistrationStatus.InVerification)
                return MenungguVerifikasi;

            if (record.KycStatus == TilakaConstant.KycStatus.Success) return MenungguVerifikasi;

            if (record.KycStatus == TilakaConstant.KycStatus.Fail
                || record.KycStatus == TilakaConstant.KycStatus.Error)
                return KycGagal;

            if (record.DateExpire.HasValue && record.DateExpire.Value < now) return Kedaluwarsa;

            if (string.IsNullOrWhiteSpace(record.RegistrationRequestID)) return UuidTerbit;

            // UUID sudah terbit tetapi data KYC belum dikirim: aksi berikutnya adalah Kirim Data KYC.
            if (!record.RegistrationDateTime.HasValue) return UuidTerbit;

            return MenungguKyc;
        }

        public static bool IsCertificateActive(TilakaRegistrationRecord record, System.DateTime now)
        {
            if (record == null) return false;
            if (record.CertificateStatus != TilakaConstant.CertificateStatus.Active) return false;
            if (IsRevoked(record)) return false;
            return !record.CertificateExpiryDate.HasValue || record.CertificateExpiryDate.Value >= now;
        }

        public static bool IsExpiringSoon(TilakaRegistrationRecord record, System.DateTime now, int days)
        {
            if (!IsCertificateActive(record, now) || !record.CertificateExpiryDate.HasValue)
                return false;

            return record.CertificateExpiryDate.Value <= now.AddDays(days);
        }

        private static bool IsRevoked(TilakaRegistrationRecord record)
        {
            return string.Equals(record.CertificateDataStatus,
                       TilakaConstant.CertificateDataStatus.Revoke,
                       System.StringComparison.OrdinalIgnoreCase)
                   || string.Equals(record.RevokeStatus,
                       TilakaConstant.RevokeStatus.Success,
                       System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
