namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaQuickSignConstant
    {
        public static class SignerStatus
        {
            public const string Unauthorized = "UNAUTHORIZED";
            public const string Process = "PROCESS";
            public const string Done = "DONE";
            public const string Failed = "FAILED";
            public const string Denied = "DENIED";
            public const string Idle = "IDLE";
        }

        public static class RequestStatus
        {
            public const string Requested = "Requested";
            public const string InProgress = "InProgress";
            public const string Completed = "Completed";
            public const string Failed = "Failed";
        }

        public static class AuthStatus
        {
            public const string Active = "active";
            public const string Expired = "expired";
            public const string NotActivated = "not_activated";
        }

        public static class SignaturePlacement
        {
            public const int PageNumber = 1;
            public const int CoordinateX = 1;
            public const int CoordinateY = 1;
            public const int Width = 1;
            public const int Height = 1;
        }

        public const int MaxFileSizeMb = 20;
    }
}
