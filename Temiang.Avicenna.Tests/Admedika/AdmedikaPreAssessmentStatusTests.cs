using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaPreAssessmentStatusTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"8401\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"preAssessmentStatusResponse\":{\"preAssessmentStatus\":{"
            + "\"statusText\":\"This claim is still being assessed [7 minutes ago] Please wait for a while.\","
            + "\"patientName\":\"PATIENT_NAME\",\"clID\":\"XXXXXXXXXX\",\"covCode\":\"HNS\","
            + "\"clStatus\":\"38\",\"clDesc\":\"ASSESSED CLAIMS\","
            + "\"form_f_link\":\"https://webclaim.admedika.com/letter/l_formf_temporary.php?RefID=XXXXXXXX\","
            + "\"totAmtIncurred\":\"57721261\",\"totAmtApproved\":\"40000000\",\"totAmtNotApproved\":\"17721261\","
            + "\"entitlement\":["
            + "{\"benID\":\"02\",\"shortName\":\"BIAYA KAMAR\",\"longName\":\"Biaya Kamar\","
            + "\"amtIncurred\":\"1980000\",\"amtApproved\":\"1980000\",\"amtNotApproved\":\"0\"},"
            + "{\"benID\":\"05\",\"shortName\":\"ANEKA PRWTN\",\"longName\":\"Aneka Perawatan\","
            + "\"amtIncurred\":\"25741261\",\"amtApproved\":\"8020000\",\"amtNotApproved\":\"17721261\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"8402\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"preAssessmentStatusResponse\":{\"preAssessmentStatus\":{"
            + "\"errorCode\":1995,\"errorDesc\":\"Please use serviceID=DISCHARGE_IP. This claim is already valid.\"}}},"
            + "\"txnResponseDateTime\":\"20261030200001\"}}";

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
        public void PreAssessmentStatus_MapsResponseAndEntitlement()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreAssessmentStatus("8000991200054479", "XXXXXXXXXX");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("38", response.ClStatus);
            Assert.AreEqual("57721261", response.TotAmtIncurred);
            Assert.AreEqual(2, response.Entitlement.Count);
            Assert.AreEqual("17721261", response.Entitlement[1].AmtNotApproved);
            StringAssert.Contains(response.FormFLink, "l_formf_temporary.php");

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("PRE_ASSESSMENT_STATUS", input["serviceID"].ToString());

            var payload = input["txnData"]["preAssessmentStatusRequest"]["preAssessmentStatus"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
            Assert.AreEqual("XXXXXXXXXX", payload["clID"].ToString());
        }

        [TestMethod]
        public void PreAssessmentStatus_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreAssessmentStatus("8000991200054479", "XXXXXXXXXX");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1995", response.ErrorCode);
        }
    }
}
