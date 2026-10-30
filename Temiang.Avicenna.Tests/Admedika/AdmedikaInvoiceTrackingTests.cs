using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaInvoiceTrackingTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"8001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"invoiceTrackingResponse\":{\"invoiceTracking\":["
            + "{\"payorName\":\"ADMEDIKA\",\"payorCode\":\"PT ADMINISTRASI MEDIKA\",\"invoiceNo\":\"TES KLAIM 2\","
            + "\"totClaims\":\"1\",\"totAmtIncurred\":\"1800000\",\"totAmtPaid\":\"0\","
            + "\"receivedDate\":\"Mar-04-2026\",\"registerDate\":\"\",\"submissionDate\":\"\","
            + "\"admedNo\":\"\",\"paymentDate\":\"\"},"
            + "{\"payorName\":\"ADMEDIKA\",\"payorCode\":\"PT ADMINISTRASI MEDIKA\",\"invoiceNo\":\"TES KLAIM 3\","
            + "\"totClaims\":\"2\",\"totAmtIncurred\":\"2500000\",\"totAmtPaid\":\"100000\","
            + "\"receivedDate\":\"Mar-05-2026\",\"registerDate\":\"Mar-06-2026\",\"submissionDate\":\"\","
            + "\"admedNo\":\"ADM-1\",\"paymentDate\":\"\"}]}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

        private const string EmptyJson =
            "{\"output\":{\"referenceID\":\"8002\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"invoiceTrackingResponse\":{\"invoiceTracking\":[]}},"
            + "\"txnResponseDateTime\":\"20261030200001\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"8003\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"invoiceTrackingResponse\":{\"invoiceTracking\":{"
            + "\"errorCode\":1840,\"errorDesc\":\"Not found! [INVOICE]. Please check your request parameters.\"}}},"
            + "\"txnResponseDateTime\":\"20261030200002\"}}";

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
        public void InvoiceTracking_MapsArrayAndSendsFilters()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTracking("TES", "RECEIVED_DATE", "01032026", "04032026");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.InvoiceTracking.Count);
            Assert.AreEqual("TES KLAIM 2", response.InvoiceTracking[0].InvoiceNo);
            Assert.AreEqual("1800000", response.InvoiceTracking[0].TotAmtIncurred);
            Assert.AreEqual("ADM-1", response.InvoiceTracking[1].AdmedNo);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("INVOICE_TRACKING", input["serviceID"].ToString());

            var payload = input["txnData"]["invoiceTrackingRequest"]["invoiceTracking"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("TES", payload["invoiceNo"].ToString());
            Assert.AreEqual("RECEIVED_DATE", payload["dateName"].ToString());
            Assert.AreEqual("01032026", payload["dateFrom"].ToString());
            Assert.AreEqual("04032026", payload["dateTo"].ToString());
        }

        [TestMethod]
        public void Parse_MixedArrayAndObjectInWrapper_PrefersObjectPayload()
        {
            const string json =
                "{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"mixedResponse\":{"
                + "\"items\":[\"A\",\"B\"],\"summary\":{\"patientName\":\"X\"}}}}}";

            var response = ResponseParser.Parse<CheckIcdExclusionResponse>(json);

            Assert.AreEqual("X", response.PatientName);
        }

        [TestMethod]
        public void InvoiceTracking_EmptyArray_ReturnsEmptyList()
        {
            var transport = new FakeTransport { NextResponse = EmptyJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTracking("TES", null, null, null);

            Assert.IsTrue(response.IsSuccess);
            Assert.IsNotNull(response.InvoiceTracking);
            Assert.AreEqual(0, response.InvoiceTracking.Count);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["invoiceTrackingRequest"]["invoiceTracking"];
            Assert.IsNull(payload["dateName"]);
            Assert.IsNull(payload["dateFrom"]);
            Assert.IsNull(payload["dateTo"]);
        }

        [TestMethod]
        public void InvoiceTracking_FailedObject_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.InvoiceTracking("TES", null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1840", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "INVOICE");
        }

        [TestMethod]
        public void InvoiceTracking_WithoutTerminalId_SkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var config = CreateConfig();
            config.TerminalID = string.Empty;
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.InvoiceTracking("TES", null, null, null);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.TechnicalError, "terminalID");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
