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
                RegistrationDateTime = DateTime.Now.AddMinutes(-1),
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

        [TestMethod]
        public void NoExpiry_WithRequestId_IsMenungguKyc()
        {
            var record = new TilakaRegistrationRecord
            {
                RegistrationRequestID = "uuid-1",
                RegistrationDateTime = DateTime.Now.AddMinutes(-1),
                DateExpire = null
            };

            Assert.AreEqual(TilakaRegistrationState.MenungguKyc, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void RequestIdWithoutRegistrationDate_IsUuidTerbit()
        {
            var record = new TilakaRegistrationRecord
            {
                RegistrationRequestID = "uuid-1",
                RegistrationDateTime = null,
                DateExpire = DateTime.Now.AddDays(3)
            };

            Assert.AreEqual(TilakaRegistrationState.UuidTerbit, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void RequestIdWithRegistrationDate_IsMenungguKyc()
        {
            var record = new TilakaRegistrationRecord
            {
                RegistrationRequestID = "uuid-1",
                RegistrationDateTime = DateTime.Now,
                DateExpire = DateTime.Now.AddDays(3)
            };

            Assert.AreEqual(TilakaRegistrationState.MenungguKyc, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void NoExpiry_WithoutRequestId_IsUuidTerbit()
        {
            var record = new TilakaRegistrationRecord { DateExpire = null };

            Assert.AreEqual(TilakaRegistrationState.UuidTerbit, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void CertificateRejected_IsDitolak()
        {
            var record = Record();
            record.CertificateStatus = 4;

            Assert.AreEqual(TilakaRegistrationState.Ditolak, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void CertificateRegistered_IsSertifikatTerbit()
        {
            var record = Record();
            record.CertificateStatus = 2;

            Assert.AreEqual(TilakaRegistrationState.SertifikatTerbit, TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }

        [TestMethod]
        public void ActiveCertificateWithExpiredDate_IsKedaluwarsa()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            record.CertificateExpiryDate = DateTime.Now.AddDays(-1);

            Assert.AreEqual(TilakaRegistrationState.Kedaluwarsa, TilakaRegistrationState.Evaluate(record, DateTime.Now));
            Assert.IsFalse(TilakaRegistrationState.IsCertificateActive(record, DateTime.Now));
        }

        [TestMethod]
        public void RevokedCertificateData_IsDicabut()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            record.CertificateExpiryDate = DateTime.Now.AddDays(30);
            record.CertificateDataStatus = TilakaConstant.CertificateDataStatus.Revoke;

            Assert.AreEqual(TilakaRegistrationState.Dicabut, TilakaRegistrationState.Evaluate(record, DateTime.Now));
            Assert.IsFalse(TilakaRegistrationState.IsCertificateActive(record, DateTime.Now));
        }

        [TestMethod]
        public void ActiveCertificateWithoutExpiry_IsActive()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;

            Assert.AreEqual(TilakaRegistrationState.Aktif, TilakaRegistrationState.Evaluate(record, DateTime.Now));
            Assert.IsTrue(TilakaRegistrationState.IsCertificateActive(record, DateTime.Now));
        }

        [TestMethod]
        public void ReEnrollStatus_MapsInterimAndSuccess()
        {
            var record = Record();

            record.ApplyUserReEnrollStatus(new TilakaUserReEnrollData { Status = "C", ReasonCode = "0" });
            Assert.AreEqual(TilakaConstant.KycStatus.InVerification, record.KycStatus);

            record.ApplyUserReEnrollStatus(new TilakaUserReEnrollData { Status = "S", ReasonCode = "0" });
            Assert.AreEqual(TilakaConstant.KycStatus.Success, record.KycStatus);
            Assert.AreEqual(0, record.KycReasonCode);
        }

        [TestMethod]
        public void RevokeStatusSuccess_IsDicabutEvenWithoutCertificateDataStatus()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            record.CertificateExpiryDate = DateTime.Now.AddDays(30);
            record.RevokeStatus = TilakaConstant.RevokeStatus.Success;

            Assert.AreEqual(TilakaRegistrationState.Dicabut,
                TilakaRegistrationState.Evaluate(record, DateTime.Now));
            Assert.IsFalse(TilakaRegistrationState.IsCertificateActive(record, DateTime.Now));
        }

        [TestMethod]
        public void ApplyCertificateStatus_DoesNotOverwriteRevoke()
        {
            var record = Record();
            record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            record.CertificateExpiryDate = DateTime.Now.AddDays(30);
            record.RevokeStatus = TilakaConstant.RevokeStatus.Success;
            record.CertificateDataStatus = TilakaConstant.CertificateDataStatus.Revoke;

            record.ApplyCertificateStatus(new CheckCertStatusResponse
            {
                Success = true,
                Status = TilakaConstant.CertificateStatus.Active,
                Data = new[]
                {
                    new TilakaCertificate
                    {
                        Status = TilakaConstant.CertificateDataStatus.Active,
                        SerialNumber = "SER-STALE"
                    }
                }
            });

            Assert.AreEqual(TilakaConstant.CertificateDataStatus.Revoke, record.CertificateDataStatus);
            Assert.AreEqual(TilakaRegistrationState.Dicabut,
                TilakaRegistrationState.Evaluate(record, DateTime.Now));
        }
    }
}
