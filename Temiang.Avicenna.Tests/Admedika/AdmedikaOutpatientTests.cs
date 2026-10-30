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

        [TestMethod]
        public void SuccessResponseWithoutEntitlement_ReturnsEmptyLists()
        {
            const string eligibility =
                "{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"eligibilityResponse\":{\"eligibility\":{\"clID\":\"1\"}}}}}";
            const string entitlement =
                "{\"output\":{\"referenceID\":\"2\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"getEntitlementResponse\":{\"getEntitlement\":{\"clID\":\"1\"}}}}}";
            const string discharge =
                "{\"output\":{\"referenceID\":\"3\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"dischargeResponse\":{\"discharge\":{\"clID\":\"1\"}}}}}";

            var eligibilityResponse = ResponseParser.Parse<EligibilityResponse>(eligibility);
            var entitlementResponse = ResponseParser.Parse<GetEntitlementResponse>(entitlement);
            var dischargeResponse = ResponseParser.Parse<DischargeOpResponse>(discharge);

            Assert.IsNotNull(eligibilityResponse.Entitlement);
            Assert.AreEqual(0, eligibilityResponse.Entitlement.Count);
            Assert.IsNotNull(entitlementResponse.Entitlement);
            Assert.AreEqual(0, entitlementResponse.Entitlement.Count);
            Assert.IsNotNull(dischargeResponse.Entitlement);
            Assert.AreEqual(0, dischargeResponse.Entitlement.Count);
        }

        private const string DischargeSuccessJson =
            "{\"output\":{\"referenceID\":\"3201\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"dischargeResponse\":{\"discharge\":{"
            + "\"clID\":\"1500001\",\"providerTransID\":\"RJ-001\",\"clStatus\":\"40\",\"clDesc\":\"VALID CLAIM\","
            + "\"PayorName\":\"Jaminan Kesehatan\",\"dob\":\"Nov-17-1990\",\"policyNo\":\"X\","
            + "\"payorMemberID\":\"201611_08\",\"policyDate\":\"Nov-14-2016 To Dec-29-2020\","
            + "\"planCode\":\"OP55A\",\"covCode\":\"GP\",\"cardNo\":\"8000XXXXXXXXXXXX\","
            + "\"patientName\":\"Juan Tamad\",\"totAmtIncurred\":\"1,810,000.51\","
            + "\"totAmtApproved\":\"1,303,000.41\",\"totAmtNotApproved\":\"507,000.10\","
            + "\"entitlement\":[{\"benID\":\"39\",\"shortName\":\"B.DR.UMUM\",\"longName\":\"Konsultasi\","
            + "\"amtIncurred\":\"250,000.00\",\"amtApproved\":\"55,000.00\",\"amtNotApproved\":\"195,000.00\"}],"
            + "\"printOut\":\"PENGESAHAN & TAGIHAN\\r\\nJUM.BIAYA :1810000\"}}},"
            + "\"txnResponseDateTime\":\"20261030150200\"}}";

        private const string DischargeFailedJson =
            "{\"output\":{\"referenceID\":\"3202\",\"statusCode\":\"999\",\"statusMsg\":\"Error\",\"format\":\"JSON\","
            + "\"txnData\":{\"dischargeResponse\":{\"discharge\":{"
            + "\"errorCode\":\"1998\",\"errorDesc\":\"Rejected -> benefit not covered\","
            + "\"cardNo\":\"8000XXXXXXXXXXXX\",\"patientName\":\"Juan Tamad\"}}},"
            + "\"txnResponseDateTime\":\"20261030150201\"}}";

        [TestMethod]
        public void DischargeOp_MapsAmountsAndPrintOut()
        {
            var transport = new FakeTransport { NextResponse = DischargeSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DischargeOp("8000991200054479", "J06", "3", "RJ-001",
                null, null, "dr. Test", "N", "N", null, null, null,
                new[]
                {
                    new DischargeEntitlementInput
                    {
                        BenID = "39",
                        BenAmount = "250,000.00",
                        BenItemList = new[]
                        {
                            new DischargeBenItemInput
                            {
                                Code = "S2080000xx", Name = "Biaya doctor Umum",
                                DateIssue = "30102026", Qty = "1", TotPrice = "250,000.00"
                            }
                        }
                    }
                });

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("1,810,000.51", response.TotAmtIncurred);
            Assert.AreEqual("1,303,000.41", response.TotAmtApproved);
            Assert.AreEqual(1, response.Entitlement.Count);
            Assert.AreEqual("55,000.00", response.Entitlement[0].AmtApproved);
            Assert.AreEqual("PENGESAHAN & TAGIHAN\r\nJUM.BIAYA :1810000", response.PrintOut);
        }

        [TestMethod]
        public void DischargeOp_SendsNestedEntitlementAndItems()
        {
            var transport = new FakeTransport { NextResponse = DischargeSuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            service.DischargeOp("8000991200054479", "J06", "3", "RJ-001",
                null, null, null, null, null, null, null, null,
                new[]
                {
                    new DischargeEntitlementInput
                    {
                        BenID = "41",
                        BenAmount = "210,000",
                        BenItemList = new[]
                        {
                            new DischargeBenItemInput { Code = "001-1", Name = "Obat A", Qty = "6", TotPrice = "125,000" },
                            new DischargeBenItemInput { Code = "001-2", Name = "Obat B", Qty = "12", TotPrice = "25,000" }
                        }
                    }
                });

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["dischargeRequest"]["discharge"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("J06", payload["diagnosisCodeList"].ToString());
            Assert.AreEqual("41", payload["entitlement"][0]["benID"].ToString());
            Assert.AreEqual("001-2", payload["entitlement"][0]["benItemList"][1]["code"].ToString());
            Assert.IsNull(payload["roomType"] == null ? null : payload["roomType"].ToString());
        }

        [TestMethod]
        public void DischargeOp_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = DischargeFailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DischargeOp("8000991200054479", "J06", null, null,
                null, null, null, null, null, null, null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1998", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "not covered");
        }
    }
}
