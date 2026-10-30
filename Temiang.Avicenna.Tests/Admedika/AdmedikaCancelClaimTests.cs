using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaCancelClaimTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"5001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"cancelOpenClaimTxnResponse\":{\"cancelOpenClaimTxn\":{"
            + "\"msg\":\"Success. Open Claims was Canceled @[claimsid: 12200001] ready to do ELIGIBILITY\"}}},"
            + "\"txnResponseDateTime\":\"20261030170000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"5002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"cancelOpenClaimTxnResponse\":{\"cancelOpenClaimTxn\":{"
            + "\"errorCode\":1998,"
            + "\"errorDesc\":\"Rejected -> This member has a pending Claims with another provider\"}}},"
            + "\"txnResponseDateTime\":\"20261030170001\"}}";

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
        public void CancelOpenClaimsTxn_SendsPayloadAndMapsMessage()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CancelOpenClaimsTxn("8000991200054479",
                "salah pendaftaran harusnya ke dental");

            Assert.IsTrue(response.IsSuccess);
            StringAssert.Contains(response.Msg, "ready to do ELIGIBILITY");

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("CANCEL_OPEN_CLAIMS_TXN", input["serviceID"].ToString());

            var payload = input["txnData"]["cancelOpenClaimTxnRequest"]["cancelOpenClaimTxn"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
            Assert.AreEqual("salah pendaftaran harusnya ke dental", payload["remarks"].ToString());
        }

        [TestMethod]
        public void CancelOpenClaimsTxn_FailedResponse_ReadsNumericErrorCode()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CancelOpenClaimsTxn("8000991200054479", "salah input");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1998", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "pending Claims");
        }

        [TestMethod]
        public void CancelOpenClaimsTxn_WithoutTerminalId_SkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var config = CreateConfig();
            config.TerminalID = string.Empty;
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.CancelOpenClaimsTxn("8000991200054479", "salah input");

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.TechnicalError, "terminalID");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
