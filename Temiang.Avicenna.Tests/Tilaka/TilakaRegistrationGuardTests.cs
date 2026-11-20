using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaRegistrationGuardTests
    {
        private static TilakaDoctorInput Input()
        {
            return new TilakaDoctorInput
            {
                ParamedicID = "DR001",
                NamaKTP = "Nama Sesuai KTP",
                Ssn = "3273010101900001",
                Email = "dr@rs.example",
                HasKtpFile = true,
                ConsentApproved = true
            };
        }

        [TestMethod]
        public void ValidInput_CanRegister()
        {
            var result = TilakaRegistrationGuard.Check(Input(), null, DateTime.Now);
            Assert.IsTrue(result.CanRegister);
        }

        [TestMethod]
        public void NikNot16Digits_CannotRegister()
        {
            var input = Input();
            input.Ssn = "123";
            Assert.IsFalse(TilakaRegistrationGuard.Check(input, null, DateTime.Now).CanRegister);
        }

        [TestMethod]
        public void DuplicateNik_CannotRegister()
        {
            var input = Input();
            input.NikUsedByOtherParamedic = true;
            Assert.IsFalse(TilakaRegistrationGuard.Check(input, null, DateTime.Now).CanRegister);
        }

        [TestMethod]
        public void MissingConsent_CannotRegister()
        {
            var input = Input();
            input.ConsentApproved = false;
            Assert.IsFalse(TilakaRegistrationGuard.Check(input, null, DateTime.Now).CanRegister);
        }

        [TestMethod]
        public void LiveRegistration_OffersContinue()
        {
            var latest = new TilakaRegistrationRecord
            {
                RegistrationRequestID = "uuid-1",
                DateExpire = DateTime.Now.AddDays(2)
            };

            var result = TilakaRegistrationGuard.Check(Input(), latest, DateTime.Now);
            Assert.IsFalse(result.CanRegister);
            Assert.IsTrue(result.CanContinue);
        }

        [TestMethod]
        public void ActiveCertificate_BlocksRegistration()
        {
            var latest = new TilakaRegistrationRecord
            {
                CertificateStatus = TilakaConstant.CertificateStatus.Active,
                DateExpire = DateTime.Now.AddDays(2)
            };

            var result = TilakaRegistrationGuard.Check(Input(), latest, DateTime.Now);
            Assert.IsFalse(result.CanRegister);
            Assert.IsFalse(result.CanContinue);
            StringAssert.Contains(result.Message, "Aktif");
        }
    }
}
