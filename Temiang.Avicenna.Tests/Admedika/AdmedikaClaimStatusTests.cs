using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaClaimStatusTests
    {
        private const string StatusSuccessJson =
            "{\"output\":{\"referenceID\":\"4001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"checkMemberClaimTxnStatusResponse\":{\"checkMemberClaimTxnStatus\":{"
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"Test member name\",\"dob\":\"Apr-12-1977\","
            + "\"covCode\":\"GP\",\"icdCode\":\"J06\",\"icdDesc\":\"Acute Upper Respiratory Infections\","
            + "\"clID\":\"2800001\",\"clStatus\":\"08\",\"clDesc\":\"ELIGIBILITY APPROVED\","
            + "\"PayorName\":\"Jaminan Kesehatan\",\"totAmtIncurred\":\"0.00\","
            + "\"totAmtApproved\":\"0.00\",\"totAmtNotApproved\":\"0.00\","
            + "\"entitlement\":[{\"benID\":\"02\",\"shortName\":\"BIAYA\",\"longName\":\"Biaya\"}],"
            + "\"remarks\":\"Test API for eligibility\"}}},"
            + "\"txnResponseDateTime\":\"20261030160000\"}}";

        private const string StatusFailedJson =
            "{\"output\":{\"referenceID\":\"4002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"checkMemberClaimTxnStatusResponse\":{\"checkMemberClaimTxnStatus\":{"
            + "\"errorCode\":\"1840\",\"errorDesc\":\"No open claims found. Please do the Eligibility first!\","
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"Test member name\",\"clID\":\"2800001\"}}},"
            + "\"txnResponseDateTime\":\"20261030160001\"}}";

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
        public void CheckMemberClaimTxnStatus_MapsPayloadAndSendsTerminalId()
        {
            var transport = new FakeTransport { NextResponse = StatusSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckMemberClaimTxnStatus("8000991200054479", "2800001");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("2800001", response.ClID);
            Assert.AreEqual("08", response.ClStatus);
            Assert.AreEqual("J06", response.IcdCode);
            Assert.AreEqual("Jaminan Kesehatan", response.PayorName);
            Assert.AreEqual(1, response.Entitlement.Count);
            Assert.AreEqual("02", response.Entitlement[0].BenID);
            Assert.AreEqual("Test API for eligibility", response.Remarks);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["checkMemberClaimTxnStatusRequest"]["checkMemberClaimTxnStatus"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("2800001", payload["clID"].ToString());
        }

        [TestMethod]
        public void CheckMemberClaimTxnStatus_OmitsNullClID()
        {
            var transport = new FakeTransport { NextResponse = StatusSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            service.CheckMemberClaimTxnStatus("8000991200054479", null);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["checkMemberClaimTxnStatusRequest"]["checkMemberClaimTxnStatus"];
            Assert.IsNull(payload["clID"]);
        }

        [TestMethod]
        public void CheckMemberClaimTxnStatus_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = StatusFailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckMemberClaimTxnStatus("8000991200054479", null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1840", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "No open claims");
        }

        [TestMethod]
        public void CheckMemberClaimTxnStatus_WithoutTerminalId_SkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = StatusSuccessJson };
            var config = CreateConfig();
            config.TerminalID = string.Empty;
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.CheckMemberClaimTxnStatus("8000991200054479", null);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.TechnicalError, "terminalID");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
