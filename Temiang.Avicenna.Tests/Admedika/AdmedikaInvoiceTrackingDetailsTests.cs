using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaInvoiceTrackingDetailsTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"8501\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"invoiceTrackingDetailsResponse\":{\"invoiceTrackingDetails\":{"
            + "\"payorCode\":\"ADMEDKA\",\"invoiceNo\":\"Arie245\",\"details\":["
            + "{\"claimsID\":\"XXXX1\",\"patientName\":\"Test Name1\",\"provCode\":\"XXXX\","
            + "\"provName\":\"ADMEDIKA MEDICAL CENTRE\",\"covCode\":\"GP\",\"admissionDate\":\"Jan-14-2026\","
            + "\"dischargeDate\":\"Jan-14-2026\",\"amtIncurred\":\"2000\",\"amtApproved\":\"2000\","
            + "\"amtToPay\":\"0\",\"amtPaidToProvider\":\"0\",\"icdCode\":\"R23.2\",\"clStatus\":\"40\",\"remarks\":\"\"},"
            + "{\"claimsID\":\"XXXX2\",\"patientName\":\"Test Name 2\",\"provCode\":\"XXX\","
            + "\"provName\":\"ADMEDIKA MEDICAL CENTRE\",\"covCode\":\"GP\",\"admissionDate\":\"Jan-14-2026\","
            + "\"dischargeDate\":\"Jan-14-2026\",\"amtIncurred\":\"6000\",\"amtApproved\":\"6000\","
            + "\"amtToPay\":\"0\",\"amtPaidToProvider\":\"0\",\"icdCode\":\"J02\",\"clStatus\":\"40\","
            + "\"remarks\":\"PENDING HASIL PENUNJANG\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"8502\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"invoiceTrackingDetailsResponse\":{\"invoiceTrackingDetails\":{"
            + "\"errorCode\":1840,\"errorDesc\":\"Not found! [invoiceNo: XXXXXXX] for this [payorCode: XXXXXXXX].\"}}},"
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
        public void InvoiceTrackingDetails_MapsDetailsAndSendsRequest()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTrackingDetails("ADMEDKA", "Arie245");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("Arie245", response.InvoiceNo);
            Assert.AreEqual(2, response.Details.Count);
            Assert.AreEqual("XXXX1", response.Details[0].ClaimsID);
            Assert.AreEqual("2000", response.Details[0].AmtApproved);
            Assert.AreEqual("PENDING HASIL PENUNJANG", response.Details[1].Remarks);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("INVOICE_TRACKING_DETAILS", input["serviceID"].ToString());

            var payload = input["txnData"]["invoiceTrackingDetailsRequest"]["invoiceTrackingDetails"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("ADMEDKA", payload["payorCode"].ToString());
            Assert.AreEqual("Arie245", payload["invoiceNo"].ToString());
        }

        [TestMethod]
        public void InvoiceTrackingDetails_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTrackingDetails("ADMEDKA", "XXXXXXX");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1840", response.ErrorCode);
        }

        [TestMethod]
        public void InvoiceTrackingDetails_MissingPayorCode_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTrackingDetails("", "Arie245");

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.ErrorDesc, "payorCode");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
