using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaStatusPresentationTests
    {
        [TestMethod]
        public void RegistrationStates_MapToLifecycleCodesAndColors()
        {
            AssertState(TilakaRegistrationState.BelumTerdaftar, 0, "Belum Terdaftar", string.Empty);
            AssertState(TilakaRegistrationState.MenungguKyc, 1, "Menunggu KYC", "#E67E22");
            AssertState(TilakaRegistrationState.MenungguVerifikasi, 2, "Menunggu Verifikasi", "#E67E22");
            AssertState(TilakaRegistrationState.SertifikatTerbit, 3, "Sertifikat Terbit", "#5BC0DE");
            AssertState(TilakaRegistrationState.Aktif, 4, "Aktif", "#28A745");
            AssertState(TilakaRegistrationState.KycGagal, 5, "KYC Gagal", "#D9534F");
            AssertState(TilakaRegistrationState.Ditolak, 6, "Ditolak", "#D9534F");
            AssertState(TilakaRegistrationState.Dicabut, 7, "Dicabut", "#D9534F");
            AssertState(TilakaRegistrationState.Kedaluwarsa, 8, "Kedaluwarsa", "#D9534F");
            AssertState(TilakaRegistrationState.UuidTerbit, 9, "UUID Terbit", "#E67E22");
        }

        [TestMethod]
        public void UnknownRegistrationState_IsBelumTerdaftar()
        {
            AssertState("TidakDikenal", 0, "Belum Terdaftar", string.Empty);
        }

        [TestMethod]
        public void QuickSignRegisteredStatus_IsDarkGreen()
        {
            Assert.AreEqual("#1E7E34", TilakaStatusPresentation.QuickSignStatusColor(true));
            Assert.AreEqual(string.Empty, TilakaStatusPresentation.QuickSignStatusColor(false));
        }

        private static void AssertState(string state, int code, string name, string color)
        {
            var status = TilakaStatusPresentation.RegistrationState(state);

            Assert.AreEqual(code, status.Code);
            Assert.AreEqual(name, status.Name);
            Assert.AreEqual(color, status.Color);
            Assert.AreEqual(code + " - " + name, status.Text);
        }
    }
}
