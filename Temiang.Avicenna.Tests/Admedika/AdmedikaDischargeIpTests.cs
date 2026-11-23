using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaDischargeIpTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"8301\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"dischargeResponse\":{\"discharge\":{"
            + "\"statusText\":\"Claim successfully processed.\",\"cardNo\":\"8000991200054479\","
            + "\"patientName\":\"TEST PATIENT\",\"clID\":\"286XXXX\",\"covCode\":\"HNS\","
            + "\"clStatus\":\"40\",\"clDesc\":\"VALID CLAIM\",\"dateAdmission\":\"Aug-24-2026\","
            + "\"dateDischarge\":\"Aug-30-2026\",\"payorCode\":\"ADMEDKA\",\"payorName\":\"Jaminan Kesehatan\","
            + "\"dob\":\"Apr-12-1977\",\"gender\":\"M\",\"policyNo\":\"LTAN-04121977\","
            + "\"payorMemberID\":\"ADMEDIKA-001\",\"pd\":\"Principal\",\"principalName\":\"\","
            + "\"form_f_link\":\"https://webclaim.admedika.com/letter/l_formf.php?RefID=28\","
            + "\"totAmtIncurred\":\"17300000\",\"totAmtApproved\":\"12400000\",\"totAmtNotApproved\":\"4900000\","
            + "\"entitlement\":["
            + "{\"benID\":\"02\",\"shortName\":\"BIAYA KAMAR\",\"longName\":\"Biaya Kamar\","
            + "\"amtIncurred\":\"5900000\",\"amtApproved\":\"1000000\",\"amtNotApproved\":\"4900000\"},"
            + "{\"benID\":\"03\",\"shortName\":\"ICU\",\"longName\":\"Perawatan ICU\","
            + "\"amtIncurred\":\"250000\",\"amtApproved\":\"250000\",\"amtNotApproved\":\"0\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

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

        private static AdmedikaConfig CreateConfig(bool withTerminal = true)
        {
            return new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TerminalID = withTerminal ? "12380291" : null,
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
        }

        [TestMethod]
        public void DischargeIp_MapsResponseAndEntitlement()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DischargeIp("8000991200054479", "286XXXX");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("40", response.ClStatus);
            Assert.AreEqual("Jaminan Kesehatan", response.PayorName);
            Assert.AreEqual(2, response.Entitlement.Count);
            Assert.AreEqual("4900000", response.Entitlement[0].AmtNotApproved);
            Assert.AreEqual("https://webclaim.admedika.com/letter/l_formf.php?RefID=28", response.FormFLink);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("DISCHARGE_IP", input["serviceID"].ToString());

            var payload = input["txnData"]["dischargeRequest"]["discharge"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
            Assert.AreEqual("286XXXX", payload["clID"].ToString());
        }

        [TestMethod]
        public void DischargeIp_WithoutTerminalId_SkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(false), transport, new NullAdmedikaLogger());

            var response = service.DischargeIp("8000991200054479", "286XXXX");

            Assert.IsFalse(response.IsSuccess);
            Assert.IsFalse(string.IsNullOrEmpty(response.TechnicalError));
            Assert.AreEqual(0, transport.CallCount);
        }

        [TestMethod]
        public void DischargeIp_MissingClId_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DischargeIp("8000991200054479", "");

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.ErrorDesc, "clID");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
