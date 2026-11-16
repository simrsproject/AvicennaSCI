using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaDailyMonitoringTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"6001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"dailyMonitoringResponse\":{\"dailyMonitoring\":{"
            + "\"cardNo\":\"8000991200054479\",\"clDesc\":\"ELIGIBILITY APPROVED\",\"clID\":\"12200001\","
            + "\"clStatus\":\"08\",\"covCode\":\"GP\","
            + "\"dmForms\":["
            + "{\"code\":\"007-A\",\"date\":\"25082022\",\"input\":\"Nama Dokter = dr. Test\\nSpecialist = SP.PD\","
            + "\"name\":\"DR. YANG MERAWAT\",\"status\":\"Success: insert\"},"
            + "{\"code\":\"007-B\",\"date\":\"\",\"input\":\"\","
            + "\"name\":\"DOKTER RAWAT BERSAMA\",\"status\":\"Skip: input was empty!\"}],"
            + "\"er\":1,\"msg\":\"Thank you for the daily monitoring! Successfully updated : 1\",\"ok\":1,"
            + "\"patientName\":\"LAWRENCE\",\"payorName\":\"PT ADMINISTRASI MEDIKA\","
            + "\"providerTransID\":\"provID001\",\"terminalID\":\"12380291\","
            + "\"totAmtApproved\":\"0\",\"totAmtIncurred\":\".00\",\"totAmtNotApproved\":\"0\"}}},"
            + "\"txnResponseDateTime\":\"20261030180000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"6002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"dailyMonitoringResponse\":{\"dailyMonitoring\":{"
            + "\"errorCode\":\"1844\",\"errorDesc\":\"No daily claims monitoring details found!\"}}},"
            + "\"txnResponseDateTime\":\"20261030180001\"}}";

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
        public void DailyMonitoring_SendsFormsAndMapsStatuses()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DailyMonitoring("8000991200054479", "provID001", "12200001", "user01",
                new[]
                {
                    new DailyMonitoringForm
                    {
                        Code = "007-A", Name = "DR. YANG MERAWAT",
                        Input = "Nama Dokter = dr. Test\nSpecialist = SP.PD", Date = "25082022"
                    },
                    new DailyMonitoringForm
                    {
                        Code = "007-B", Name = "DOKTER RAWAT BERSAMA", Input = "", Date = ""
                    }
                });

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("1", response.Ok);
            Assert.AreEqual("1", response.Er);
            Assert.AreEqual(2, response.DmForms.Count);
            Assert.AreEqual("Success: insert", response.DmForms[0].Status);
            Assert.AreEqual("Skip: input was empty!", response.DmForms[1].Status);
            Assert.AreEqual("Nama Dokter = dr. Test\nSpecialist = SP.PD", response.DmForms[0].Input);
            Assert.AreEqual(".00", response.TotAmtIncurred);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("DAILY_MONITORING", input["serviceID"].ToString());

            var payload = input["txnData"]["dailyMonitoringRequest"]["dailyMonitoring"];
            Assert.AreEqual("12380291", payload["terminalID"].ToString());
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
            Assert.AreEqual("provID001", payload["providerTransID"].ToString());
            Assert.AreEqual("12200001", payload["clID"].ToString());
            Assert.AreEqual("user01", payload["dmUser"].ToString());
            Assert.AreEqual("007-A", payload["dmForms"][0]["code"].ToString());
            Assert.AreEqual("Nama Dokter = dr. Test\nSpecialist = SP.PD",
                payload["dmForms"][0]["input"].ToString());
            Assert.AreEqual("007-B", payload["dmForms"][1]["code"].ToString());
            Assert.AreEqual("", payload["dmForms"][1]["input"].ToString());
        }

        [TestMethod]
        public void DailyMonitoring_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.DailyMonitoring("8000991200054479", "provID001", null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1844", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "monitoring");
        }

        [TestMethod]
        public void DailyMonitoring_WithoutTerminalId_SkipsTransport()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var config = CreateConfig();
            config.TerminalID = string.Empty;
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.DailyMonitoring("8000991200054479", "provID001", null, null, null);

            Assert.IsFalse(response.IsSuccess);
            StringAssert.Contains(response.TechnicalError, "terminalID");
            Assert.AreEqual(0, transport.CallCount);
        }

        [TestMethod]
        public void DailyMonitoring_MissingForms_ReturnsEmptyList()
        {
            const string nullForms =
                "{\"output\":{\"referenceID\":\"6003\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"dailyMonitoringResponse\":"
                + "{\"dailyMonitoring\":{\"dmForms\":null}}}}}";
            const string absentForms =
                "{\"output\":{\"referenceID\":\"6004\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                + "\"format\":\"JSON\",\"txnData\":{\"dailyMonitoringResponse\":"
                + "{\"dailyMonitoring\":{\"clID\":\"1\"}}}}}";

            var nullResponse = ResponseParser.Parse<DailyMonitoringResponse>(nullForms);
            var absentResponse = ResponseParser.Parse<DailyMonitoringResponse>(absentForms);

            Assert.IsNotNull(nullResponse.DmForms);
            Assert.AreEqual(0, nullResponse.DmForms.Count);
            Assert.IsNotNull(absentResponse.DmForms);
            Assert.AreEqual(0, absentResponse.DmForms.Count);
        }
    }
}
