using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaAuthTests
    {
        private class FakeTransport : ITilakaTransport
        {
            public int FormCalls;
            public int JsonCalls;
            public readonly List<int> JsonStatuses = new List<int>();
            public string TokenResponse = "{\"access_token\":\"tok-1\",\"expires_in\":300,\"token_type\":\"Bearer\"}";
            public string JsonResponse = "{\"success\":true,\"message\":\"ok\"}";

            public TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds)
            {
                FormCalls++;
                return new TilakaTransportResult { StatusCode = 200, Body = TokenResponse };
            }

            public TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds)
            {
                JsonCalls++;
                var status = JsonStatuses.Count > 0 ? JsonStatuses[0] : 200;
                if (JsonStatuses.Count > 0) JsonStatuses.RemoveAt(0);
                return new TilakaTransportResult { StatusCode = status, Body = JsonResponse };
            }
        }

        [TestInitialize]
        public void Init()
        {
            Temiang.Avicenna.Common.Tilaka.Service.ResetTokenCache();
        }

        private static TilakaConfig Config()
        {
            return new TilakaConfig
            {
                BaseUrl = "https://sb-api.tilaka.id",
                AuthPath = "/auth",
                ChannelId = "chan",
                ChannelSecret = "secret",
                TimeoutSeconds = 5
            };
        }

        [TestMethod]
        public void GetAccessToken_CachesToken()
        {
            var transport = new FakeTransport();
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), transport, new NullTilakaLogger());

            Assert.AreEqual("tok-1", service.GetAccessToken());
            Assert.AreEqual("tok-1", service.GetAccessToken());
            Assert.AreEqual(1, transport.FormCalls);
        }

        [TestMethod]
        public void Execute_On401_RefreshesTokenAndRetriesOnce()
        {
            var transport = new FakeTransport();
            transport.JsonStatuses.Add(401);
            transport.JsonStatuses.Add(200);
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), transport, new NullTilakaLogger());

            var result = service.Execute<GenerateUuidResponse>("GenerateUUID", "https://x/generateUUID", "{}");

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(2, transport.FormCalls);
            Assert.AreEqual(2, transport.JsonCalls);
        }

        [TestMethod]
        public void IsOk_ReturnsTechnicalErrorFirst()
        {
            string message;
            var service = new Temiang.Avicenna.Common.Tilaka.Service(Config(), new FakeTransport(), new NullTilakaLogger());
            var ok = service.IsOk(new TilakaResponse { Success = true, TechnicalError = "timeout" }, out message);

            Assert.IsFalse(ok);
            Assert.AreEqual("timeout", message);
        }
    }
}
