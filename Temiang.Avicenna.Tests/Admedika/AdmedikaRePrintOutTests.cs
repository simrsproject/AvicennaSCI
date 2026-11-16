using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaRePrintOutTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"9001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"rePrintOutResponse\":{\"rePrintOut\":{"
            + "\"printOut\":\"PENGESAHAN & TAGIHAN\\r\\nB. ADMIN 15,000.00\\r\\nJUM.BIAYA : 2,892,900.00\"}}},"
            + "\"txnResponseDateTime\":\"20261030210000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"9002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"rePrintOutResponse\":{\"rePrintOut\":{"
            + "\"errorCode\":1997,"
            + "\"errorDesc\":\"Invalid rePrintType [S]. Default value [E = ELIGIBILITY (LOA)] [D = DISCHARGE (LOC)] [X = REJECT (LOR)]\"}}},"
            + "\"txnResponseDateTime\":\"20261030210001\"}}";

        private class FakeTransport : IAdmedikaTransport
        {
            public int CallCount;
            public string LastBody;
            public string NextResponse;

            public string Post(string url, string jsonBody, int timeoutSeconds)
            {
                CallCount++;
                LastBody = jsonBody;
                return NextResponse;
            }

            public string PostMultipart(string url, string jsonBody, AdmedikaFile[] files, int timeoutSeconds)
            {
                throw new System.NotSupportedException();
            }
        }

        private static AdmedikaConfig CreateConfig()
        {
            return new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TerminalID = "12380291",
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
        }

        [TestMethod]
        public void RePrintOut_MapsPrintOutAndSendsRequest()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.RePrintOut("1500001", "D");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(
                "PENGESAHAN & TAGIHAN\r\nB. ADMIN 15,000.00\r\nJUM.BIAYA : 2,892,900.00",
                response.PrintOut);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("RE_PRINT_OUT", input["serviceID"].ToString());

            var payload = input["txnData"]["rePrintOutRequest"]["rePrintOut"];
            Assert.AreEqual("1500001", payload["clID"].ToString());
            Assert.AreEqual("D", payload["rePrintType"].ToString());
            Assert.IsNull(payload["terminalID"]);
        }

        [TestMethod]
        public void RePrintOut_FailedResponse_ReturnsInvalidTypeError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.RePrintOut("1500001", "S");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1997", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "rePrintType");
        }
    }
}
