using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaOutpatientTests
    {
        private const string EligibilitySuccessJson =
            "{\"output\":{\"referenceID\":\"3001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"eligibilityResponse\":{\"eligibility\":{"
            + "\"clID\":\"1500001\",\"clStatus\":\"08\",\"clDesc\":\"ELIGIBILITY APPROVED\","
            + "\"payorName\":\"Jaminan Kesehatan\",\"dob\":\"Apr-21-1968\",\"policyNo\":\"POL-1\","
            + "\"payorMemberID\":\"PM-1\",\"policyDate\":\"Jan-01-2022 To Dec-31-2023\",\"planCode\":\"OP-850\","
            + "\"cardNo\":\"8000991200054479\",\"referral_info_flag\":\"N\",\"patientName\":\"Juan Tamad\","
            + "\"pd\":\"Principal\",\"principalName\":\"\",\"printOut\":\"LOA\\nB.ADMIN\","
            + "\"entitlement\":[{\"benID\":\"39\",\"shortName\":\"B.DR.UMUM\",\"longName\":\"Konsultasi Dokter Umum\"},"
            + "{\"benID\":\"41\",\"shortName\":\"B.OBAT\",\"longName\":\"Resep Obat\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030150000\"}}";

        private const string EligibilityFailedJson =
            "{\"output\":{\"referenceID\":\"3002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"eligibilityResponse\":{\"eligibility\":{"
            + "\"errorCode\":1812,\"errorDesc\":\"Member not found\","
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"\"}}},"
            + "\"txnResponseDateTime\":\"20261030150001\"}}";

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
        public void Eligibility_MapsPayloadAndSendsTerminalId()
        {
            var transport = new FakeTransport { NextResponse = EligibilitySuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.Eligibility("8000991200054479", "02", "J06",
                "RJ-001", "3171", "KK-1", "dr. Test", "N", "N", "1", "500000", "catatan");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("1500001", response.ClID);
            Assert.AreEqual("08", response.ClStatus);
            Assert.AreEqual("OP-850", response.PlanCode);
            Assert.AreEqual("Juan Tamad", response.PatientName);
            Assert.AreEqual(2, response.Entitlement.Count);
            Assert.AreEqual("39", response.Entitlement[0].BenID);
            Assert.AreEqual("Resep Obat", response.Entitlement[1].LongName);
            Assert.AreEqual("LOA\nB.ADMIN", response.PrintOut);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["eligibilityRequest"]["eligibility"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("02", payload["covID"].ToString());
            Assert.AreEqual("RJ-001", payload["providerTransID"].ToString());
            Assert.AreEqual("1", payload["roomType"].ToString());
            Assert.AreEqual("500000", payload["roomPrice"].ToString());
            Assert.AreEqual("catatan", payload["remarks"].ToString());
        }

        [TestMethod]
        public void Eligibility_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = EligibilityFailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.Eligibility("8000991200054479", "02", "J06",
                null, null, null, null, null, null, null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1812", response.ErrorCode);
            Assert.AreEqual("Member not found", response.ErrorDesc);
        }

        [TestMethod]
        public void Eligibility_WithoutTerminalId_ReturnsErrorAndSkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = EligibilitySuccessJson };
            var config = CreateConfig();
            config.TerminalID = string.Empty;
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.Eligibility("8000991200054479", "02", "J06",
                null, null, null, null, null, null, null, null, null);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.TechnicalError, "terminalID");
            Assert.AreEqual(0, transport.CallCount);
        }

        private const string GetEntitlementSuccessJson =
            "{\"output\":{\"referenceID\":\"3101\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"getEntitlementResponse\":{\"getEntitlement\":{"
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"Juan Tamad\",\"clID\":\"1500001\","
            + "\"clStatus\":\"08\",\"clDesc\":\"ELIGIBILITY APPROVED\",\"PayorName\":\"Jaminan Kesehatan\","
            + "\"entitlement\":[{\"benID\":\"08\",\"shortName\":\"SPESIALISTIK\",\"longName\":\"Pelayanan Spesialistik\"},"
            + "{\"benID\":\"82\",\"shortName\":\"IMUNISASI\",\"longName\":\"Imunisasi Dasar\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030150100\"}}";

        private const string GetEntitlementFailedJson =
            "{\"output\":{\"referenceID\":\"3102\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"getEntitlementResponse\":{\"getEntitlement\":{"
            + "\"errorCode\":\"1840\",\"errorDesc\":\"No open claims found\",\"cardNo\":\"8000991200054479\"}}},"
            + "\"txnResponseDateTime\":\"20261030150101\"}}";

        [TestMethod]
        public void GetEntitlement_MapsBenefitsAndSendsTerminalId()
        {
            var transport = new FakeTransport { NextResponse = GetEntitlementSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.GetEntitlement("8000991200054479");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("1500001", response.ClID);
            Assert.AreEqual(2, response.Entitlement.Count);
            Assert.AreEqual("82", response.Entitlement[1].BenID);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["getEntitlementRequest"]["getEntitlement"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
        }

        [TestMethod]
        public void GetEntitlement_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = GetEntitlementFailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.GetEntitlement("8000991200054479");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1840", response.ErrorCode);
        }
    }
}
