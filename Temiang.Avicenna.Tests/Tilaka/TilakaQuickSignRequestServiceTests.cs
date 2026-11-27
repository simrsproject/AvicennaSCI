using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignRequestServiceTests
    {
        private class FakeTransport : ITilakaTransport
        {
            public string UploadResponse = "{\"success\":true,\"message\":\"ok\",\"filename\":\"uploaded.pdf\"}";
            public string RequestResponse = "{\"success\":true,\"message\":\"ok\",\"auth_response\":[{\"url\":\"http://auth\",\"user_identifier\":\"dr001\",\"status\":false,\"token_key\":\"tk\"}],\"failed_doc_name\":null}";
            public bool FailRequestSign;
            public int UploadCalls;

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
                if (url.Contains("requestquicksign"))
                {
                    return new TilakaTransportResult
                    {
                        StatusCode = FailRequestSign ? 400 : 200,
                        Body = FailRequestSign
                            ? "{\"success\":false,\"message\":\"ditolak\"}"
                            : RequestResponse
                    };
                }

                return new TilakaTransportResult { StatusCode = 200, Body = "{\"success\":true}" };
            }

            public TilakaTransportResult Get(string url, string bearerToken, int timeoutSeconds)
            {
                return new TilakaTransportResult { StatusCode = 200, Body = "{}" };
            }

            public TilakaTransportResult PostMultipart(string url, string formName, string fileName,
                byte[] content, string contentType, string bearerToken, int timeoutSeconds)
            {
                UploadCalls++;
                return new TilakaTransportResult { StatusCode = 200, Body = UploadResponse };
            }
        }

        private class FakeQuickSignRepository : ITilakaQuickSignRepository
        {
            public TilakaQuickSignRecord Record;
            public List<TilakaQuickSignSignerRecord> Signers = new List<TilakaQuickSignSignerRecord>();

            public TilakaQuickSignRecord FindByRequestId(string requestId)
            {
                return Record != null && Record.RequestID == requestId ? Record : null;
            }

            public IList<TilakaQuickSignSignerRecord> LoadSigners(long quickSignRequestId)
            {
                return Signers;
            }

            public IList<TilakaQuickSignRecord> FindPending(int max)
            {
                return Record == null
                    ? new List<TilakaQuickSignRecord>()
                    : new List<TilakaQuickSignRecord> { Record };
            }

            public void Save(TilakaQuickSignRecord record, IList<TilakaQuickSignSignerRecord> signers)
            {
                Record = record;
                if (record.QuickSignRequestID <= 0) record.QuickSignRequestID = 1;
                Signers = new List<TilakaQuickSignSignerRecord>(signers ?? new List<TilakaQuickSignSignerRecord>());
            }
        }

        private class FakeCasemixRepository : ITilakaCasemixFileRepository
        {
            public TilakaCasemixFileRecord Record;
            public long LinkedRequest;

            public TilakaCasemixFileRecord FindByID(long casemixFileID)
            {
                return Record != null && Record.CasemixFileID == casemixFileID ? Record : null;
            }

            public IList<TilakaCasemixFileRecord> FindAll()
            {
                return new List<TilakaCasemixFileRecord> { Record };
            }

            public void UpdateLink(long casemixFileID, long quickSignRequestID, DateTime processedDateTime)
            {
                LinkedRequest = quickSignRequestID;
            }

            public void UpdateDownloadPath(long casemixFileID, long quickSignRequestID, string downloadPath) { }
        }

        private class FakeTemplateRepository : ITilakaTemplateSignFileRepository
        {
            public TilakaTemplateSignFileRecord Record;

            public TilakaTemplateSignFileRecord FindByID(long templateSignFileID) { return Record; }

            public TilakaTemplateSignFileRecord FindDefault(string paramedicID) { return Record; }

            public TilakaTemplateSignFileRecord FindByParamedicAndTemplateNo(string paramedicID, string templateNo)
            {
                return Record;
            }

            public IList<TilakaTemplateSignFileRecord> FindByParamedic(string paramedicID)
            {
                return new List<TilakaTemplateSignFileRecord> { Record };
            }

            public void Save(TilakaTemplateSignFileRecord record) { }

            public void Delete(long templateSignFileID) { }
        }

        private class FakeRegistrationRepository : ITilakaRegistrationRepository
        {
            public TilakaRegistrationRecord Record;

            public TilakaRegistrationRecord FindByRequestId(string requestId) { return Record; }

            public TilakaRegistrationRecord FindLatest(string relatedType, string relatedId) { return Record; }

            public IList<TilakaRegistrationRecord> FindPending(int max)
            {
                return new List<TilakaRegistrationRecord>();
            }

            public void Save(TilakaRegistrationRecord record) { }
        }

        private class FakeProfile : ITilakaUserProfileProvider
        {
            public string Signature = "AAAA";
            public string Email = "dr001@rs.example";
            public bool UsingESignature = true;

            public string GetEmail(string paramedicID) { return Email; }
            public string GetSignatureImageBase64(string paramedicID) { return Signature; }
            public bool IsUsingESignature(string paramedicID) { return UsingESignature; }
        }

        private class FakeReader : ITilakaCasemixFileReader
        {
            public byte[] Load(string filePath)
            {
                return System.Text.Encoding.UTF8.GetBytes("%PDF-1.4 test");
            }
        }

        private static TilakaConfig Config()
        {
            return new TilakaConfig
            {
                LiteBaseUrl = "https://lite",
                ChannelId = "chan",
                ChannelSecret = "secret",
                LiteTimeoutSeconds = 5
            };
        }

        private static TilakaQuickSignRequestService CreateService(FakeTransport transport,
            FakeQuickSignRepository repository, FakeCasemixRepository casemix,
            FakeTemplateRepository template, FakeRegistrationRepository registration,
            FakeProfile profile)
        {
            var serviceConfig = new TilakaConfig
            {
                BaseUrl = "https://lite",
                AuthPath = "/auth",
                ChannelId = "chan",
                ChannelSecret = "secret",
                TimeoutSeconds = 5
            };
            var service = new Temiang.Avicenna.Common.Tilaka.Service(
                serviceConfig, transport, new NullTilakaLogger());

            return new TilakaQuickSignRequestService(service, Config(), repository, casemix,
                template, registration, profile, new FakeReader());
        }

        private static FakeQuickSignRepository Repository(out FakeCasemixRepository casemix,
            out FakeTemplateRepository template, out FakeRegistrationRepository registration,
            out FakeProfile profile)
        {
            casemix = new FakeCasemixRepository
            {
                Record = new TilakaCasemixFileRecord
                {
                    CasemixFileID = 1,
                    ParamedicID = "DR001",
                    RegistrationNo = "REG-1",
                    FileName = "casemix.pdf",
                    FilePath = "casemix.pdf"
                }
            };
            template = new FakeTemplateRepository
            {
                Record = new TilakaTemplateSignFileRecord
                {
                    TemplateSignFileID = 10,
                    ParamedicID = "DR001",
                    TemplateNo = "TTD-DR001-01",
                    IsDefault = true,
                    IsRegistered = true,
                    IsActive = true
                }
            };
            registration = new FakeRegistrationRepository
            {
                Record = new TilakaRegistrationRecord
                {
                    RelatedType = TilakaConstant.RelatedType.Paramedic,
                    RelatedID = "DR001",
                    TilakaName = "dr001",
                    CertificateStatus = TilakaConstant.CertificateStatus.Active,
                    IsQuickSignUser = true
                }
            };
            profile = new FakeProfile();
            return new FakeQuickSignRepository();
        }

        [TestInitialize]
        public void Init()
        {
            Temiang.Avicenna.Common.Tilaka.Service.ResetTokenCache();
        }

        [TestMethod]
        public void Submit_Success_StoresRequestAndAuthUrl()
        {
            FakeCasemixRepository casemix;
            FakeTemplateRepository template;
            FakeRegistrationRepository registration;
            FakeProfile profile;
            var repository = Repository(out casemix, out template, out registration, out profile);
            var service = CreateService(new FakeTransport(), repository, casemix, template, registration, profile);

            var result = service.Submit(1, null, null, null, null, "tester");

            Assert.IsTrue(result.IsSuccess, result.Message);
            Assert.AreEqual(1, result.Signers.Count);
            Assert.AreEqual("http://auth", result.Signers[0].AuthUrl);
            Assert.AreEqual(TilakaQuickSignConstant.SignerStatus.Unauthorized, result.Signers[0].Status);
            Assert.AreEqual(10, repository.Record.TemplateSignFileID);
            Assert.AreEqual("TTD-DR001-01", repository.Record.TemplateNo);
            Assert.AreEqual(repository.Record.QuickSignRequestID, casemix.LinkedRequest);
        }

        [TestMethod]
        public void Submit_WithoutSignatureImage_FailsBeforeUpload()
        {
            FakeCasemixRepository casemix;
            FakeTemplateRepository template;
            FakeRegistrationRepository registration;
            FakeProfile profile;
            var repository = Repository(out casemix, out template, out registration, out profile);
            profile.Signature = null;
            var service = CreateService(new FakeTransport(), repository, casemix, template, registration, profile);

            var result = service.Submit(1, null, null, null, null, "tester");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "tanda tangan");
        }

        [TestMethod]
        public void Submit_DoctorNotUsingESignature_FailsBeforeUpload()
        {
            FakeCasemixRepository casemix;
            FakeTemplateRepository template;
            FakeRegistrationRepository registration;
            FakeProfile profile;
            var repository = Repository(out casemix, out template, out registration, out profile);
            profile.UsingESignature = false;
            var transport = new FakeTransport();
            var service = CreateService(transport, repository, casemix, template, registration, profile);

            var result = service.Submit(1, null, null, null, null, "tester");

            Assert.IsFalse(result.IsSuccess);
            StringAssert.Contains(result.Message, "tanda tangan elektronik");
            Assert.AreEqual(0, transport.UploadCalls);
        }

        [TestMethod]
        public void Submit_ApiFailure_MarksRequestFailed()
        {
            FakeCasemixRepository casemix;
            FakeTemplateRepository template;
            FakeRegistrationRepository registration;
            FakeProfile profile;
            var repository = Repository(out casemix, out template, out registration, out profile);
            var transport = new FakeTransport { FailRequestSign = true };
            var service = CreateService(transport, repository, casemix, template, registration, profile);

            var result = service.Submit(1, null, null, null, null, "tester");

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TilakaQuickSignConstant.RequestStatus.Failed, repository.Record.Status);
            Assert.IsFalse(repository.Record.IsActive);
        }
    }
}
