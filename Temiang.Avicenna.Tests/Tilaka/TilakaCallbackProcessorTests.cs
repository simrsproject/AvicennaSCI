using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaCallbackProcessorTests
    {
        private class FakeRepository : ITilakaRegistrationRepository
        {
            public readonly TilakaRegistrationRecord Record = new TilakaRegistrationRecord
            {
                TilakaRegistrationID = 1,
                RelatedType = TilakaConstant.RelatedType.Paramedic,
                RelatedID = "DR001",
                RegistrationRequestID = "uuid-1",
                DateExpire = DateTime.Now.AddDays(3)
            };

            public int SaveCalls;

            public TilakaRegistrationRecord FindByRequestId(string requestId)
            {
                return Record.RegistrationRequestID == requestId ? Record : null;
            }

            public TilakaRegistrationRecord FindLatest(string relatedType, string relatedId)
            {
                return Record;
            }

            public IList<TilakaRegistrationRecord> FindPending(int max)
            {
                return new List<TilakaRegistrationRecord> { Record };
            }

            public void Save(TilakaRegistrationRecord record)
            {
                SaveCalls++;
                Record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            }
        }

        [TestMethod]
        public void Registration_UnknownId_IsNotApplied()
        {
            var repo = new FakeRepository();
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessRegistration(new CallbackRegistrationRequest { RegisterId = "unknown" });

            Assert.IsFalse(outcome.Applied);
            Assert.AreEqual(0, repo.SaveCalls);
        }

        [TestMethod]
        public void Registration_KycSuccess_UpdatesFields()
        {
            var repo = new FakeRepository();
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessRegistration(new CallbackRegistrationRequest
            {
                RegisterId = "uuid-1",
                Success = true,
                Data = new TilakaKycData
                {
                    Status = TilakaConstant.KycStatus.Success,
                    TilakaName = "anita001",
                    ReasonCode = "0"
                }
            });

            Assert.IsTrue(outcome.Applied);
            Assert.AreEqual("S", repo.Record.KycStatus);
            Assert.AreEqual("anita001", repo.Record.TilakaName);
            Assert.AreEqual(TilakaConstant.LogSource.Callback, repo.Record.LastCheckSource);
        }

        [TestMethod]
        public void CertificateStatus_ArrivesBeforeRegistration_KeepsKycFields()
        {
            var repo = new FakeRepository();
            repo.Record.KycStatus = "S";
            repo.Record.TilakaName = "anita001";
            var processor = new TilakaCallbackProcessor(repo);

            processor.ProcessCertificateStatus(new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                UserIdentifier = "anita001",
                Success = true,
                Status = 2
            }, DateTime.Now);

            Assert.AreEqual("S", repo.Record.KycStatus);
            Assert.AreEqual("anita001", repo.Record.TilakaName);
            Assert.AreEqual(2, repo.Record.CertificateStatus);
        }

        [TestMethod]
        public void DuplicateCallback_IsIdempotent()
        {
            var repo = new FakeRepository();
            var processor = new TilakaCallbackProcessor(repo);
            var payload = new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                UserIdentifier = "anita001",
                Success = true,
                Status = 3
            };

            processor.ProcessCertificateStatus(payload, DateTime.Now);
            var second = processor.ProcessCertificateStatus(payload, DateTime.Now);

            Assert.AreEqual(3, repo.Record.CertificateStatus);
            Assert.IsFalse(second.Applied);
            Assert.AreEqual("status-regression", second.Reason);
        }

        [TestMethod]
        public void CertificateStatus_SuccessFalse_DoesNotChangeStatus()
        {
            var repo = new FakeRepository();
            repo.Record.CertificateStatus = TilakaConstant.CertificateStatus.Registered;
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessCertificateStatus(new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                UserIdentifier = "anita001",
                Success = false,
                Status = TilakaConstant.CertificateStatus.Active
            }, DateTime.Now);

            Assert.IsFalse(outcome.Applied);
            Assert.AreEqual("success-false", outcome.Reason);
            Assert.AreEqual(TilakaConstant.CertificateStatus.Registered, repo.Record.CertificateStatus);
        }

        [TestMethod]
        public void CertificateStatus_StatusZero_DoesNotChangeStatus()
        {
            var repo = new FakeRepository();
            repo.Record.CertificateStatus = TilakaConstant.CertificateStatus.Registered;
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessCertificateStatus(new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                Success = true,
                Status = TilakaConstant.CertificateStatus.None
            }, DateTime.Now);

            Assert.IsFalse(outcome.Applied);
            Assert.AreEqual(TilakaConstant.CertificateStatus.Registered, repo.Record.CertificateStatus);
        }

        [TestMethod]
        public void CertificateStatus_ActiveThenLowerStatus_IsIgnored()
        {
            var repo = new FakeRepository();
            repo.Record.CertificateStatus = TilakaConstant.CertificateStatus.Active;
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessCertificateStatus(new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                Success = true,
                Status = TilakaConstant.CertificateStatus.Registered
            }, DateTime.Now);

            Assert.IsFalse(outcome.Applied);
            Assert.AreEqual("status-regression", outcome.Reason);
            Assert.AreEqual(TilakaConstant.CertificateStatus.Active, repo.Record.CertificateStatus);
        }

        [TestMethod]
        public void CertificateStatus_HigherStatus_IsApplied()
        {
            var repo = new FakeRepository();
            repo.Record.CertificateStatus = TilakaConstant.CertificateStatus.InVerification;
            var processor = new TilakaCallbackProcessor(repo);

            var outcome = processor.ProcessCertificateStatus(new CallbackCertificateStatusRequest
            {
                RequestId = "uuid-1",
                Success = true,
                Status = TilakaConstant.CertificateStatus.Registered
            }, DateTime.Now);

            Assert.IsTrue(outcome.Applied);
            Assert.AreEqual(TilakaConstant.CertificateStatus.Registered, repo.Record.CertificateStatus);
        }
    }
}
