using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaServiceTests
    {
        private class FakeTransport : ITilakaTransport
        {
            public string LastJsonUrl;
            public string LastJsonBody;
            public string LastBearer;
            public string JsonResponse = "{\"success\":true,\"message\":\"Success\",\"data\":[\"uuid-1\"]}";
            public bool FailPostForm;

            public TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds)
            {
                if (FailPostForm)
                    throw new TilakaTransportException(
                        "The remote server returned an error: (401) Unauthorized.", 401, string.Empty);

                return new TilakaTransportResult
                {
                    StatusCode = 200,
                    Body = "{\"access_token\":\"tok\",\"expires_in\":300}"
                };
            }

            public TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds)
            {
                LastJsonUrl = url;
                LastJsonBody = jsonBody;
                LastBearer = bearerToken;
                return new TilakaTransportResult { StatusCode = 200, Body = JsonResponse };
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

        private class CapturingLogger : ITilakaLogger
        {
            public string Operation;
            public string Request;
            public string Response;
            public long ElapsedMs;

            public void Log(string operation, string requestBody, string responseBody, long elapsedMs)
            {
                Operation = operation;
                Request = requestBody;
                Response = responseBody;
                ElapsedMs = elapsedMs;
            }
        }

        private static TilakaConfig Config()
        {
            return new TilakaConfig
            {
                BaseUrl = "https://sb-api.tilaka.id",
                AuthPath = "/auth",
                ChannelId = "chan",
                ChannelSecret = "secret",
                ChannelType = "Company",
                TimeoutSeconds = 5
            };
        }

        [TestInitialize]
        public void Init()
        {
            Temiang.Avicenna.Common.Tilaka.Service.ResetTokenCache();
        }

        [TestMethod]
        public void GenerateUuid_CompanySendsNameAndEmail()
        {
            var transport = new FakeTransport();
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), transport, new NullTilakaLogger());

            service.GenerateUuid("Nama KTP", "a@b.com");

            StringAssert.Contains(transport.LastJsonUrl, "generateUUID?name=Nama%20KTP&email=a%40b.com");
            Assert.AreEqual("tok", transport.LastBearer);
        }

        [TestMethod]
        public void RegisterForKycCheck_PostsSerializedBody()
        {
            var transport = new FakeTransport();
            transport.JsonResponse = "{\"success\":true,\"message\":\"Data Diterima\",\"data\":[\"uuid-1\",\"a@b.com\"]}";
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), transport, new NullTilakaLogger());

            var result = service.RegisterForKycCheck(new RegisterForKycCheckRequest
            {
                RegistrationId = "uuid-1",
                Email = "a@b.com",
                Name = "Nama KTP",
                Nik = "3273010101900001",
                PhotoKtp = "data:image/jpeg;base64,AAAA",
                DateExpire = "2026-11-27 23:59",
                IsApproved = true,
                ConsentText = "TnC",
                Version = "v1",
                HashConsent = "hash",
                ConsentTimestamp = "2026-11-20 10:00:00"
            });

            Assert.IsTrue(result.IsOk);
            StringAssert.Contains(transport.LastJsonUrl, "/registerForKycCheck");
            StringAssert.Contains(transport.LastJsonBody, "\"registration_id\":\"uuid-1\"");
            Assert.IsFalse(transport.LastJsonBody.Contains("nationality_type"));
        }
        [TestMethod]
        public void QuickSignFactory_UsesCloudAuthAndLiteApiBase()
        {
            var source = new TilakaConfig
            {
                BaseUrl = "https://sb-api.tilaka.id",
                AuthPath = "/auth",
                LiteBaseUrl = "http://10.200.200.15:8088",
                ChannelId = "chan",
                ChannelSecret = "secret",
                LiteTimeoutSeconds = 5
            };

            var service = TilakaQuickSignServiceFactory.Create(source);

            Assert.AreEqual("http://10.200.200.15:8088", service.Config.BaseUrl);
            Assert.AreEqual("https://sb-api.tilaka.id", service.Config.AuthBaseUrl);
            Assert.AreEqual("https://sb-api.tilaka.id", service.Config.CloudBaseUrl);
            Assert.AreEqual("/auth", service.Config.AuthPath);
            Assert.AreEqual("http://10.200.200.15:8088/api/v1/upload",
                TilakaQuickSignUrls.Upload(service.Config));
            Assert.AreEqual("https://sb-api.tilaka.id/addquicksignusers",
                TilakaQuickSignUrls.AddUsers(service.Config));
        }

        [TestMethod]
        public void GetAccessToken_LogsAuthSuccessWithoutSecret()
        {
            var logger = new CapturingLogger();
            var service = new Temiang.Avicenna.Common.Tilaka.Service(
                Config(), new FakeTransport(), logger);

            service.GetAccessToken();

            StringAssert.StartsWith(logger.Operation, "Auth ");
            StringAssert.Contains(logger.Operation, "/auth");
            StringAssert.Contains(logger.Response, "[HTTP 200]");
            Assert.IsFalse(logger.Request.Contains("client_secret=secret"));
            StringAssert.Contains(logger.Request, "client_secret=[redacted]");
        }

        [TestMethod]
        public void GetAccessToken_LogsAuthFailureAs401()
        {
            var logger = new CapturingLogger();
            var transport = new FakeTransport { FailPostForm = true };
            var service = new Temiang.Avicenna.Common.Tilaka.Service(
                Config(), transport, logger);

            Assert.ThrowsException<TilakaTransportException>(() => service.GetAccessToken());

            StringAssert.StartsWith(logger.Operation, "Auth ");
            StringAssert.Contains(logger.Response, "[HTTP 401]");
            Assert.IsFalse(logger.Request.Contains("client_secret=secret"));
            StringAssert.Contains(logger.Request, "client_secret=[redacted]");
        }

        [TestMethod]
        public void IsOk_UsesCertificateMessageInfoWhenBaseMessageEmpty()
        {
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), new FakeTransport(), new NullTilakaLogger());
            string message;

            var ok = service.IsOk(new CheckCertStatusResponse
            {
                Success = false,
                Message = new TilakaCertMessage { Info = "Sertifikat tidak ditemukan" }
            }, out message);

            Assert.IsFalse(ok);
            Assert.AreEqual("Sertifikat tidak ditemukan", message);
        }
    }
}
