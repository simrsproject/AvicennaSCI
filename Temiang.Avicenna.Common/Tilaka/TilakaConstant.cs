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

        public static class LogSource
        {
            public const string Callback = "callback";
            public const string Manual = "manual";
            public const string Reconcile = "reconcile";
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
