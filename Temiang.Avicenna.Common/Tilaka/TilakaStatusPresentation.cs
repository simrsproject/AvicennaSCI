namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaStatusDisplay
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }

        public string Text
        {
            get { return Code + " - " + Name; }
        }
    }

    public static class TilakaStatusPresentation
    {
        private const string Orange = "#E67E22";
        private const string LightBlue = "#5BC0DE";
        private const string Green = "#28A745";
        private const string Red = "#D9534F";
        private const string DarkGreen = "#1E7E34";

        public static TilakaStatusDisplay RegistrationState(string state)
        {
            switch (state)
            {
                case TilakaRegistrationState.MenungguKyc:
                    return Create(1, "Menunggu KYC", Orange);
                case TilakaRegistrationState.MenungguVerifikasi:
                    return Create(2, "Menunggu Verifikasi", Orange);
                case TilakaRegistrationState.SertifikatTerbit:
                    return Create(3, "Sertifikat Terbit", LightBlue);
                case TilakaRegistrationState.Aktif:
                    return Create(4, "Aktif", Green);
                case TilakaRegistrationState.KycGagal:
                    return Create(5, "KYC Gagal", Red);
                case TilakaRegistrationState.Ditolak:
                    return Create(6, "Ditolak", Red);
                case TilakaRegistrationState.Dicabut:
                    return Create(7, "Dicabut", Red);
                case TilakaRegistrationState.Kedaluwarsa:
                    return Create(8, "Kedaluwarsa", Red);
                case TilakaRegistrationState.UuidTerbit:
                    return Create(9, "UUID Terbit", Orange);
                default:
                    return Create(0, "Belum Terdaftar", string.Empty);
            }
        }

        public static string QuickSignStatusColor(bool registered)
        {
            return registered ? DarkGreen : string.Empty;
        }

        private static TilakaStatusDisplay Create(int code, string name, string color)
        {
            return new TilakaStatusDisplay
            {
                Code = code,
                Name = name,
                Color = color
            };
        }
    }
}
