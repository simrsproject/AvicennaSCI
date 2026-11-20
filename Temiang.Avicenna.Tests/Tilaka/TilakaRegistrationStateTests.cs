using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaRegistrationStateTests
    {
        private static TilakaRegistrationRecord Record()
        {
            return new TilakaRegistrationRecord
            {
                RegistrationRequestID = "uuid-1",
                DateExpire = DateTime.Now.AddDays(3)
            };
        }

        [TestMethod]
        public void Null_IsBelumTerdaftar()
        {
            Assert.AreEqual(TilakaRegistrationState.BelumTerdaftar, TilakaRegistrationState.Evaluate(null, DateTime.Now));
        }

        [TestMethod]
        public void ActiveCertificate_WinsOverEverything()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            record.KycStatus = TilakaConstant.KycStatus.Fail;
            record.DateExpire = DateTime.Now.AddDays(-1);

            Assert.AreEqual(TilakaRegistrationState.Aktif, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void KycSuccess_WaitsForCertificate()
        {
            var record = Record();
            record.KycStatus = TilakaConstant.KycStatus.Success;

            Assert.AreEqual(TilakaRegistrationState.MenungguVerifikasi, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void KycFail_IsKycGagal()
        {
            var record = Record();
            record.KycStatus = TilakaConstant.KycStatus.Fail;
            record.ManualRegistrationStatus = TilakaConstant.ManualRegistrationStatus.NotStarted;

            Assert.AreEqual(TilakaRegistrationState.KycGagal, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void ManualRegistrationInVerification_Waits()
        {
            var record = Record();
            record.KycStatus = TilakaConstant.KycStatus.Fail;
            record.ManualRegistrationStatus = TilakaConstant.ManualRegistrationStatus.InVerification;

            Assert.AreEqual(TilakaRegistrationState.MenungguVerifikasi, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void Expired_WhenNoCertificateAndDatePassed()
        {
            var record = Record();
            record.DateExpire = DateTime.Now.AddMinutes(-1);

            Assert.AreEqual(TilakaRegistrationState.Kedaluwarsa, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void UuidTerbit_WhenRegistrationNotSubmitted()
        {
            var record = new TilakaRegistrationRecord { DateExpire = DateTime.Now.AddDays(3) };
            Assert.AreEqual(TilakaRegistrationState.UuidTerbit, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }
    }
}
