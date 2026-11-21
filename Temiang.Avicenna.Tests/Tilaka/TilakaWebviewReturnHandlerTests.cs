using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaWebviewReturnHandlerTests
    {
        private class FakeRepository : ITilakaRegistrationRepository
        {
            public TilakaRegistrationRecord Record;
            public int SaveCalls;

            public TilakaRegistrationRecord FindByRequestId(string requestId)
            {
                return Record != null && Record.RegistrationRequestID == requestId ? Record : null;
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
                "{\"success\":true,\"status\":3,\"message\":{\"info\":\"Aktif\"},\"data\":[]}";

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
                DateExpire = DateTime.Now.AddDays(3)
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
            var handler = new TilakaWebviewReturnHandler(repository, CreateService(new FakeTransport()));

            var result = handler.Process(null);

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "request-id");
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void UnknownRequestId_ReturnsFailureWithoutSaving()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaWebviewReturnHandler(repository, CreateService(new FakeTransport()));

            var result = handler.Process("unknown");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "tidak ditemukan");
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void EmptyTilakaName_AsksForCekStatusWithoutCallingApi()
        {
            var repository = new FakeRepository { Record = Record() };
            repository.Record.TilakaName = null;
            var handler = new TilakaWebviewReturnHandler(repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "Cek Status");
            Assert.AreEqual(0, repository.SaveCalls);
        }

        [TestMethod]
        public void ActiveStatus_IsAppliedAndSaved()
        {
            var repository = new FakeRepository { Record = Record() };
            var handler = new TilakaWebviewReturnHandler(repository, CreateService(new FakeTransport()));

            var result = handler.Process("uuid-1");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TilakaConstant.CertificateStatus.Active, repository.Record.CertificateStatus);
            Assert.AreEqual(TilakaRegistrationState.Aktif, repository.Record.RegistrationState);
            Assert.AreEqual(TilakaConstant.LogSource.Manual, repository.Record.LastCheckSource);
            Assert.AreEqual(1, repository.SaveCalls);
            StringAssert.Contains(result.Message, "Aktif");
        }

        [TestMethod]
        public void LowerStatus_DoesNotRegressActiveCertificate()
        {
            var repository = new FakeRepository { Record = Record() };
            repository.Record.CertificateStatus = TilakaConstant.CertificateStatus.Active;

            var transport = new FakeTransport
            {
                CertificateJson = "{\"success\":true,\"status\":2,\"message\":{\"info\":\"Registered\"},\"data\":[]}"
            };
            var handler = new TilakaWebviewReturnHandler(repository, CreateService(transport));

            handler.Process("uuid-1");

            Assert.AreEqual(TilakaConstant.CertificateStatus.Active, repository.Record.CertificateStatus);
            Assert.AreEqual(1, repository.SaveCalls);
        }
    }
}
