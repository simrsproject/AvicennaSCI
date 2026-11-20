using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaTransportTests
    {
        [TestMethod]
        public void PostForm_BuildsUrlEncodedBody()
        {
            var body = HttpTilakaTransport.BuildFormBody(new Dictionary<string, string>
            {
                { "client_id", "chan 1" },
                { "grant_type", "client_credentials" },
                { "client_secret", "a&b" }
            });

            Assert.AreEqual("client_id=chan%201&grant_type=client_credentials&client_secret=a%26b", body);
        }

        [TestMethod]
        public void TransportException_CarriesStatusAndBody()
        {
            var ex = new TilakaTransportException("failed", 401, "{\"success\":false}");
            Assert.AreEqual(401, ex.StatusCode);
            Assert.AreEqual("{\"success\":false}", ex.ResponseBody);
        }
    }
}
