using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaPreRegisterEbillTests
    {
        private const string EnquirySuccessJson =
            "{\"output\":{\"referenceID\":\"8101\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"preRegisterEbillEnquiryResponse\":{\"preRegisterEbillEnquiry\":{"
            + "\"payorCode\":\"ADMEDKA\",\"payorName\":\"JAMINAN KESEHATAN\",\"coverage\":\"RJ\","
            + "\"dateFrom\":\"01102026\",\"dateTo\":\"08102026\",\"tot_claims\":2,\"claimsList\":["
            + "{\"clID\":\"xx650092\",\"patientName\":\"TESTDENTAL SEMBILAN\",\"invoiceNo\":\"\",\"invoiceDate\":\"\","
            + "\"registerDate\":\"\",\"registerBy\":\"\",\"dateAdmission\":\"Oct-02-2026\",\"dateDischarge\":\"Oct-02-2026\","
            + "\"icdCode\":\"K02.1\",\"totAmtIncurred\":\"90000.00\",\"totAmtApproved\":\"90000.00\","
            + "\"totAmtNotApproved\":\"0.00\",\"covCode\":\"DENTAL\",\"clStatus\":\"40\",\"clDesc\":\"VALID CLAIM\","
            + "\"provCode\":\"PROV1\",\"remarks\":\"\"},"
            + "{\"clID\":\"xx650093\",\"patientName\":\"TEST MEMBER\",\"invoiceNo\":\"\",\"invoiceDate\":\"\","
            + "\"registerDate\":\"\",\"registerBy\":\"\",\"dateAdmission\":\"Oct-02-2026\",\"dateDischarge\":\"Oct-02-2026\","
            + "\"icdCode\":\"K02.1\",\"totAmtIncurred\":\"20000.00\",\"totAmtApproved\":\"20000.00\","
            + "\"totAmtNotApproved\":\"0.00\",\"covCode\":\"DENTAL\",\"clStatus\":\"40\",\"clDesc\":\"VALID CLAIM\","
            + "\"provCode\":\"PROV1\",\"remarks\":\"\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

        private const string EnquiryFailedJson =
            "{\"output\":{\"referenceID\":\"8102\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"preRegisterEbillEnquiryResponse\":{\"preRegisterEbillEnquiry\":{"
            + "\"errorCode\":1840,\"errorDesc\":\"Not found! [payorCode] [ADMEDXXX]\"}}},"
            + "\"txnResponseDateTime\":\"20261030200001\"}}";

        private const string SubmitSuccessJson =
            "{\"output\":{\"referenceID\":\"8103\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"preRegisterEbillResponse\":{\"preRegisterEbill\":{"
            + "\"surat_jalan_link\":\"https://mobile.admedika.co.id/edc/letters/surat_jalan_pre_regist.php?xcode=1\","
            + "\"selectedClaims\":[{\"clID\":\"28XXXX\",\"status\":\"Success\"},"
            + "{\"clID\":\"2865XXX\",\"status\":\"Success\"}]}}},"
            + "\"txnResponseDateTime\":\"20261030200002\"}}";

        private class FakeTransport : IAdmedikaTransport
        {
            public int PostCallCount;
            public int MultipartCallCount;
            public string LastBody;
            public AdmedikaFile[] LastFiles;
            public string NextResponse;

            public string Post(string url, string jsonBody, int timeoutSeconds)
            {
                PostCallCount++;
                LastBody = jsonBody;
                return NextResponse;
            }

            public string PostMultipart(string url, string jsonBody, AdmedikaFile[] files, int timeoutSeconds)
            {
                MultipartCallCount++;
                LastBody = jsonBody;
                LastFiles = files;
                return NextResponse;
            }
        }

        private static AdmedikaConfig CreateConfig()
        {
            return new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost_dev",
                ApiServiceUrl = "https://example.test/api/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TerminalID = "12380291",
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
        }

        [TestMethod]
        public void PreRegisterEbillEnquiry_MapsClaimsAndSendsPeriod()
        {
            var transport = new FakeTransport { NextResponse = EnquirySuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreRegisterEbillEnquiry("ADMEDKA", "RJ", "01102026", "08102026");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("2", response.TotClaims);
            Assert.AreEqual(2, response.ClaimsList.Count);
            Assert.AreEqual("xx650092", response.ClaimsList[0].ClID);
            Assert.AreEqual("VALID CLAIM", response.ClaimsList[1].ClDesc);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("PRE_REGISTER_EBILL_ENQUIRY", input["serviceID"].ToString());

            var payload = input["txnData"]["preRegisterEbillEnquiryRequest"]["preRegisterEbillEnquiry"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("ADMEDKA", payload["payorCode"].ToString());
            Assert.AreEqual("RJ", payload["coverage"].ToString());
            Assert.AreEqual("01102026", payload["dateFrom"].ToString());
            Assert.AreEqual("08102026", payload["dateTo"].ToString());
        }

        [TestMethod]
        public void PreRegisterEbillEnquiry_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = EnquiryFailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreRegisterEbillEnquiry("ADMEDXXX", "RJ", "01102026", "08102026");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1840", response.ErrorCode);
        }

        [TestMethod]
        public void PreRegisterEbill_SendsMultipartAndComputesSelectedClaims()
        {
            var transport = new FakeTransport { NextResponse = SubmitSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreRegisterEbill(
                new PreRegisterEbillPayload
                {
                    PayorCode = "ADMEDKA",
                    Coverage = "RJ",
                    DateFrom = "01102026",
                    DateTo = "08102026",
                    InvoiceNo = "INV-001",
                    InvoiceDate = "02102026",
                    PreparedBy = "mulan",
                    TotAmountDue = "100000",
                    SelectedClaims = new[]
                    {
                        new PreRegisterEbillSelectedClaim { ClID = "xx650092" },
                        new PreRegisterEbillSelectedClaim { ClID = "xx650087" }
                    }
                },
                null);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.SelectedClaims.Count);
            Assert.AreEqual("Success", response.SelectedClaims[0].Status);
            Assert.IsFalse(string.IsNullOrEmpty(response.SuratJalanLink));

            Assert.AreEqual(1, transport.MultipartCallCount);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("PRE_REGISTER_EBILL", input["serviceID"].ToString());

            var payload = input["txnData"]["preRegisterEbillRequest"]["preRegisterEbill"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("2", payload["totSelectedClaims"].ToString());
            Assert.AreEqual(2, ((JArray)payload["selectedClaims"]).Count);
            Assert.AreEqual("INV-001", payload["invoiceNo"].ToString());
            Assert.IsTrue(transport.LastFiles == null || transport.LastFiles.Length == 0);
        }

        [TestMethod]
        public void PreRegisterEbill_MissingSelectedClaims_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SubmitSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreRegisterEbill(
                new PreRegisterEbillPayload
                {
                    PayorCode = "ADMEDKA",
                    Coverage = "RJ",
                    DateFrom = "01102026",
                    DateTo = "08102026",
                    InvoiceNo = "INV-001",
                    InvoiceDate = "02102026",
                    PreparedBy = "mulan",
                    TotAmountDue = "100000"
                },
                null);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.ErrorDesc, "selectedClaims");
            Assert.AreEqual(0, transport.PostCallCount);
            Assert.AreEqual(0, transport.MultipartCallCount);
        }

        [TestMethod]
        public void PreRegisterEbill_InvalidCoverage_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SubmitSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.PreRegisterEbillEnquiry("ADMEDKA", "XX", "01102026", "08102026");

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.ErrorDesc, "coverage");
            Assert.AreEqual(0, transport.PostCallCount);
        }
    }
}
