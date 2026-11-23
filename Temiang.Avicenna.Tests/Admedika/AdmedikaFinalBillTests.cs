using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaFinalBillTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"8201\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"finalBillResponse\":{\"finalBill\":{"
            + "\"statusText\":\"Thank you for submitting your Final Bill Statement.\","
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"Juan Tamad\",\"providerTransID\":\"10101010\","
            + "\"clID\":\"28XXXXXX23\",\"payorName\":\"JAMINAN KESEHATAN\",\"covCode\":\"HNS\","
            + "\"clStatus\":\"34\",\"clDesc\":\"PRE-DISCHARGE | FINAL BILL RECEIVED\"}}},"
            + "\"txnResponseDateTime\":\"20261030200000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"8202\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"finalBillResponse\":{\"finalBill\":{"
            + "\"errorCode\":1810,\"errorDesc\":\"Not match! [totItems: 11] is not equal to itemizedBill\"}}},"
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

        private static FinalBillPayload CreatePayload()
        {
            return new FinalBillPayload
            {
                CardNo = "8000991200054479",
                FinalFlag = "N",
                DateAdmission = "10082026 103455",
                PreparedBy = "tester",
                TotAmountDue = "100000",
                ItemizedBill = new[]
                {
                    new FinalBillGroup
                    {
                        GroupCode = "99999",
                        GroupName = "Administration",
                        ItemList = new[]
                        {
                            new FinalBillItem
                            {
                                Code = "99999999", Name = "Materai", Qty = "1",
                                TotPrice = "6000", NonMedicalFlag = "Y"
                            }
                        }
                    },
                    new FinalBillGroup
                    {
                        GroupCode = "98999",
                        GroupName = "Pharmaceuticals",
                        ItemList = new[]
                        {
                            new FinalBillItem
                            {
                                Code = "980000001", Name = "Ecosol Nacl", Qty = "1",
                                TotPrice = "15000", NonMedicalFlag = "N"
                            },
                            new FinalBillItem
                            {
                                Code = "980000002", Name = "Transamin", Qty = "15",
                                TotPrice = "40000"
                            }
                        }
                    }
                }
            };
        }

        [TestMethod]
        public void FinalBill_SendsItemizedBillAndComputesTotItems()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.FinalBill(CreatePayload());

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("34", response.ClStatus);
            Assert.AreEqual("28XXXXXX23", response.ClID);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("FINAL_BILL", input["serviceID"].ToString());

            var payload = input["txnData"]["finalBillRequest"]["finalBill"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("3", payload["totItems"].ToString());
            Assert.AreEqual(2, ((JArray)payload["itemizedBill"]).Count);
        }

        [TestMethod]
        public void FinalBill_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.FinalBill(CreatePayload());

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1810", response.ErrorCode);
        }

        [TestMethod]
        public void FinalBill_MissingDateDischargeForFinalFlagY_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var payload = CreatePayload();
            payload.FinalFlag = "Y";

            var response = service.FinalBill(payload);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1815", response.ErrorCode);
            Assert.AreEqual(0, transport.CallCount);
        }

        [TestMethod]
        public void FinalBill_EmptyItemizedBill_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var payload = CreatePayload();
            payload.ItemizedBill = new FinalBillGroup[0];

            var response = service.FinalBill(payload);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.ErrorDesc, "itemizedBill");
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
