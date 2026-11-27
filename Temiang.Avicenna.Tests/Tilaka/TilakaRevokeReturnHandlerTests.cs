using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaRevokeReturnHandlerTests
    {
        private class FakeRepository : ITilakaRegistrationRepository
        {
            public TilakaRegistrationRecord Record;
            public int SaveCalls;

            public TilakaRegistrationRecord FindByRequestId(string requestId)
            {
                return Record != null && Record.RegistrationRequestID == requestId
                    ? Record
                    : null;
            }

            public TilakaRegistrationRecord FindLatest(string relatedType, string relatedId)
            {
                return Record;
            }

            public IList<TilakaRegistrationRecord> FindPending(int max)
            {
                return new List<TilakaRegistrationRecord>();
            }

            public void Save(TilakaRegistrationRecord record)
            {
                SaveCalls++;
                Record = record;
            }
        }

        private class FakeTransport : ITilakaTransport
        {
            public string CertificateJson =
                "{\"success\":true,\"status\":3,\"message\":{\"info\":\"Aktif\"},\"data\":[{\"status\":\"Aktif\"}]}";

            public TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds)
            {
                return new TilakaTransportResult
                {
                    StatusCode = 200,
                    Body = "{\"access_token\":\"tok\",\"expires_in\":300}"
                };
            }

            public TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds)
            {
                return new TilakaTransportResult { StatusCode = 200, Body = CertificateJson };
            }

            public TilakaTransportResult Get(string url, string bearerToken, int timeoutSeconds)
            {
                return new TilakaTransportResult { StatusCode = 200, Body = "{}" };
            }

            public TilakaTransportResult PostMultipart(string url, string formName, string fileName,
                byte[] content, string contentType, string bearerToken, int timeoutSeconds)
            {
                return new TilakaTransportResult { StatusCode = 200, Body = "{}" };
            }
        }

        private static TilakaRegistrationRecord Record()
        {
            return new TilakaRegistrationRecord
            {
                TilakaRegistrationID = 1,
                RelatedType = TilakaConstant.RelatedType.Paramedic,
                RelatedID = "DR001",
                RegistrationRequestID = "uuid-1",
                TilakaName = "anita001",
                CertificateStatus = TilakaConstant.CertificateStatus.Active,
                CertificateExpiryDate = DateTime.Now.AddDays(30),
                RevokeStatus = TilakaConstant.RevokeStatus.Requested,
                RevokeRequestID = "rev-1"
            };
        }

        private static Temiang.Avicenna.Common.Tilaka.Service CreateService(FakeTransport transport)
        {
            return new Temiang.Avicenna.Common.Tilaka.Service(
                new TilakaConfig
                {
                    BaseUrl = "https://sb-api.tilaka.id",
                    AuthPath = "/auth",
                    ChannelId = "chan",
                    ChannelSecret = "secret",
                    TimeoutSeconds = 5
                },
                transport,
                new NullTilakaLogger());
        }

        [TestInitialize]
        public void Init()
        {
            Temiang.Avicenna.Common.Tilaka.Service.ResetTokenCache();
        }

        [TestMethod]
        public void MissingRequestId_ReturnsFailure()
        {
            var repository = new FakeRepository();
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process(null, "rev-1", "Sukses");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "request-id");
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void MissingStatus_ReturnsFailure()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", null);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "Status");
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void FailedStatus_MarksFailedWithoutRevoking()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", "Gagal");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TilakaConstant.RevokeStatus.Failed, repository.Record.RevokeStatus);
            Assert.IsNull(repository.Record.CertificateDataStatus);
            Assert.AreEqual(TilakaRegistrationState.Aktif,
                TilakaRegistrationState.Evaluate(repository.Record, DateTime.Now));
            Assert.AreEqual(1, repository.SaveCalls);
        }

        [TestMethod]
        public void SuccessStatus_MarksRevokedAndPersists()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", "Sukses");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TilakaConstant.RevokeStatus.Success, repository.Record.RevokeStatus);
            Assert.AreEqual("rev-1", repository.Record.RevokeRequestID);
            Assert.AreEqual(TilakaConstant.CertificateDataStatus.Revoke,
                repository.Record.CertificateDataStatus);
            Assert.AreEqual(TilakaRegistrationState.Dicabut,
                repository.Record.RegistrationState);
            Assert.AreEqual(1, repository.SaveCalls);
        }

        [TestMethod]
        public void SuccessStatus_StaleActiveCertificateDoesNotUndoRevoke()
        {
            var repository = new FakeRepository { Record = Record() };
            var transport = new FakeTransport
            {
                CertificateJson = "{\"success\":true,\"status\":3,\"message\":{\"info\":\"Aktif\"},\"data\":[{\"status\":\"Aktif\"}]}"
            };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(transport));

            handler.Process("uuid-1", "rev-1", "Sukses");

            Assert.AreEqual(TilakaConstant.CertificateDataStatus.Revoke,
                repository.Record.CertificateDataStatus);
            Assert.AreEqual(TilakaRegistrationState.Dicabut,
                TilakaRegistrationState.Evaluate(repository.Record, DateTime.Now));
        }

        [TestMethod]
        public void StatusWithWhitespace_IsSuccess()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", "Sukses ");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TilakaConstant.RevokeStatus.Success,
                repository.Record.RevokeStatus);
        }

        [TestMethod]
        public void NeverRequested_DoesNotRevoke()
        {
            var repository = new FakeRepository { Record = Record() };
            repository.Record.RevokeStatus = null;
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", "Sukses");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(repository.Record.CertificateDataStatus);
            Assert.AreEqual(TilakaRegistrationState.Aktif,
                TilakaRegistrationState.Evaluate(repository.Record, DateTime.Now));
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void MismatchedRevokeId_DoesNotRevoke()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-other", "Sukses");

            Assert.IsFalse(result.IsSuccess);
            Assert.IsNull(repository.Record.CertificateDataStatus);
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void FailedAfterSuccess_DoesNotDowngrade()
        {
            var repository = new FakeRepository { Record = Record() };
            repository.Record.RevokeStatus = TilakaConstant.RevokeStatus.Success;
            repository.Record.CertificateDataStatus = TilakaConstant.CertificateDataStatus.Revoke;
            repository.Record.RegistrationState = TilakaRegistrationState.Dicabut;
            var handler = new TilakaRevokeReturnHandler(
                repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1", "rev-1", "Gagal");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TilakaConstant.RevokeStatus.Success,
                repository.Record.RevokeStatus);
            Assert.AreEqual(TilakaConstant.CertificateDataStatus.Revoke,
                repository.Record.CertificateDataStatus);
            Assert.AreEqual(0, repository.SaveCalls);
        }
    }
}
