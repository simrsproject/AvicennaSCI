namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaConstant
    {
        public static class KycStatus
        {
            public const string Success = "S";
            public const string Fail = "F";
            public const string Error = "E";
            public const string InVerification = "B";
            public const string FillingForm = "D";
        }

        public static class ManualRegistrationStatus
        {
            public const string NotStarted = "P";
            public const string FillingForm = "I";
            public const string InVerification = "V";
            public const string Success = "S";
            public const string Fail = "F";
            public const string Expired = "E";
        }

        public static class CertificateDataStatus
        {
            public const string Active = "Aktif";
            public const string Registered = "Registered";
            public const string Revoke = "Revoke";
        }

        public static class CertificateStatus
        {
            public const int None = 0;
            public const int InVerification = 1;
            public const int Registered = 2;
            public const int Active = 3;
            public const int Rejected = 4;
        }

        public static class RevokeStatus
        {
            public const string Requested = "Requested";
            public const string Success = "Success";
            public const string Failed = "Failed";
        }

        public static class RevokeReason
        {
            public const string Resign = "Resign";
            public const string Phk = "PHK";
            public const string HabisKontrak = "Habis Kontrak";
            public const string Mutasi = "Mutasi";
            public const string PemindahanDepartemen = "Pemindahan Departemen";
            public const string PindahDivisi = "Pindah Divisi";
            public const string InternalFraud = "Internal Fraud";
            public const string PenutupanHakAkses = "Penutupan Hak Akses";
            public const string PelanggaranHukumDariUser = "Pelanggaran Hukum dari User";
            public const string PerangkatHilangAtauDicuri = "Perangkat Hilang, Perangkat Dicuri";

            public static readonly string[] All =
            {
                Resign,
                Phk,
                HabisKontrak,
                Mutasi,
                PemindahanDepartemen,
                PindahDivisi,
                InternalFraud,
                PenutupanHakAkses,
                PelanggaranHukumDariUser,
                PerangkatHilangAtauDicuri
            };
        }

        public static class LogSource
        {
            public const string Callback = "callback";
            public const string Manual = "manual";
            public const string Reconcile = "reconcile";
            public const string Revoke = "revoke";
        }

        public static class RelatedType
        {
            public const string Paramedic = "Paramedic";
        }

        public static class RequestType
        {
            public const string Register = "register";
            public const string ReEnroll = "re_enroll";
        }
    }
}
