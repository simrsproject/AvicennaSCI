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

            var cert = record.CertificateStatus ?? -1;
            if (cert == TilakaConstant.CertificateStatus.Active) return Aktif;
            if (cert == TilakaConstant.CertificateStatus.Rejected) return Ditolak;
            if (cert == TilakaConstant.CertificateStatus.Registered) return SertifikatTerbit;
            if (cert == TilakaConstant.CertificateStatus.InVerification) return MenungguVerifikasi;

            if (record.ManualRegistrationStatus == TilakaConstant.ManualRegistrationStatus.InVerification)
                return MenungguVerifikasi;

            if (record.KycStatus == TilakaConstant.KycStatus.Success) return MenungguVerifikasi;

            if (record.KycStatus == TilakaConstant.KycStatus.Fail
                || record.KycStatus == TilakaConstant.KycStatus.Error)
                return KycGagal;

            if (!record.DateExpire.HasValue || record.DateExpire.Value < now) return Kedaluwarsa;

            return string.IsNullOrWhiteSpace(record.RegistrationRequestID) ? UuidTerbit : MenungguKyc;
        }
    }
}
