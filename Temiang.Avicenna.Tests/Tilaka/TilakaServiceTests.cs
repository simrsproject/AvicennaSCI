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
                LastJsonUrl = url;
                LastJsonBody = jsonBody;
                LastBearer = bearerToken;
                return new TilakaTransportResult { StatusCode = 200, Body = JsonResponse };
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
