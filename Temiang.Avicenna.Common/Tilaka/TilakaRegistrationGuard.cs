using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaDoctorInput
    {
        public string ParamedicID { get; set; }
        public string NamaKTP { get; set; }
        public string Ssn { get; set; }
        public string Email { get; set; }
        public bool HasKtpFile { get; set; }
        public bool ConsentApproved { get; set; }
        public bool NikUsedByOtherParamedic { get; set; }
    }

    public class TilakaGuardResult
    {
        public bool CanRegister { get; set; }
        public bool CanContinue { get; set; }
        public string Message { get; set; }
    }

    public static class TilakaRegistrationGuard
    {
        public static TilakaGuardResult Check(TilakaDoctorInput input, TilakaRegistrationRecord latest, DateTime now)
        {
            if (input == null) return Block("Data dokter tidak tersedia.");

            if (string.IsNullOrWhiteSpace(input.NamaKTP))
                return Block("Nama sesuai KTP belum diisi.");

            if (!IsSixteenDigits(input.Ssn))
                return Block("NIK (SSN) harus tepat 16 digit angka.");

            if (input.NikUsedByOtherParamedic)
                return Block("NIK sudah dipakai dokter lain.");

            if (string.IsNullOrWhiteSpace(input.Email))
                return Block("Email dokter belum diisi.");

            if (!input.HasKtpFile)
                return Block("Foto KTP belum diunggah.");

            if (!input.ConsentApproved)
                return Block("Persetujuan TnC Tilaka belum dicentang.");

            if (latest == null) return Allow();

            var state = TilakaRegistrationState.Evaluate(latest, now);
            if (state == TilakaRegistrationState.Aktif || state == TilakaRegistrationState.SertifikatTerbit)
                return Block("Dokter sudah memiliki sertifikat " + state + ".");

            if (state == TilakaRegistrationState.MenungguVerifikasi)
                return Block("Pendaftaran sedang dalam proses verifikasi.");

            if (state == TilakaRegistrationState.MenungguKyc || state == TilakaRegistrationState.UuidTerbit)
                return Continue("Pendaftaran belum selesai. Lanjutkan proses yang ada.");

            return Allow();
        }

        private static bool IsSixteenDigits(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 16) return false;
            foreach (var ch in value)
                if (ch < '0' || ch > '9') return false;
            return true;
        }

        private static TilakaGuardResult Allow()
        {
            return new TilakaGuardResult { CanRegister = true };
        }

        private static TilakaGuardResult Block(string message)
        {
            return new TilakaGuardResult { CanRegister = false, Message = message };
        }

        private static TilakaGuardResult Continue(string message)
        {
            return new TilakaGuardResult { CanRegister = false, CanContinue = true, Message = message };
        }
    }
}
