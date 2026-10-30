using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaServiceTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"222074\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"checkIcdExclusionResponse\":{\"checkIcdExclusion\":{"
            + "\"cardNo\":\"8000991200054479\",\"patientName\":\"Juan Tamad\",\"planCode\":\"OP-850\"}}},"
            + "\"txnResponseDateTime\":\"20261030143012\"}}";

        private const string HelloJson =
            "{\"output\":{\"referenceID\":\"586\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"helloResponse\":{\"hello\":{\"message\":\"Hello there\","
            + "\"apiName\":\"Gateway@AdMedika\",\"apiVersion\":\"Rust 1.0.1\"}}},"
            + "\"txnResponseDateTime\":\"20171121145633\"}}";

        private class FakeTransport : IAdmedikaTransport
        {
            public string LastUrl;
            public string LastBody;
            public int LastTimeout;
            public string NextResponse;
            public Exception NextException;

            public string Post(string url, string jsonBody, int timeoutSeconds)
            {
                LastUrl = url;
                LastBody = jsonBody;
                LastTimeout = timeoutSeconds;

                if (NextException != null) throw NextException;
                return NextResponse;
            }
        }

        private class RecordingLogger : IAdmedikaLogger
        {
            public int CallCount;
            public string LastServiceId;
            public string LastRequestBody;
            public string LastResponseBody;
            public long LastElapsedMs;

            public void Log(string serviceId, string requestBody, string responseBody, long elapsedMs)
            {
                CallCount++;
                LastServiceId = serviceId;
                LastRequestBody = requestBody;
                LastResponseBody = responseBody;
                LastElapsedMs = elapsedMs;
            }
        }

        private static AdmedikaConfig CreateConfig()
        {
            return new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
        }

        [TestMethod]
        public void CheckIcdExclusion_SendsCorrectEnvelopeAndToken()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckIcdExclusion("8000991200054479", "02", "J06, J05, B00.1");

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("Juan Tamad", response.PatientName);
            Assert.AreEqual("https://example.test/customerhost_dev", transport.LastUrl);
            Assert.AreEqual(17, transport.LastTimeout);

            var root = JObject.Parse(transport.LastBody);
            var input = root["input"];
            Assert.AreEqual("CHECK_ICD_EXCLUSION", input["serviceID"].ToString());
            Assert.AreEqual("CUST001", input["customerID"].ToString());

            var requestId = input["requestID"].ToString();
            Assert.AreEqual(6, requestId.Length);

            var requestDateTime = input["txnRequestDateTime"].ToString();
            Assert.AreEqual(14, requestDateTime.Length);

            var expectedToken = TokenGenerator.CreateToken(
                "CUST001",
                "secret",
                DateTime.ParseExact(requestDateTime, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                requestId,
                "CHECK_ICD_EXCLUSION");
            Assert.AreEqual(expectedToken, input["tokenAuth"].ToString());

            var payload = input["txnData"]["checkIcdExclusionRequest"]["checkIcdExclusion"];
            Assert.AreEqual("8000991200054479", payload["cardNo"].ToString());
            Assert.AreEqual("02", payload["covID"].ToString());
            Assert.AreEqual("J06, J05, B00.1", payload["diagnosisCodeList"].ToString());
        }

        [TestMethod]
        public void HelloWorld_SendsCustomerIdInsideTxnData()
        {
            var transport = new FakeTransport { NextResponse = HelloJson };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.HelloWorld();

            Assert.AreEqual("Hello there", response.Message);
            Assert.AreEqual("Gateway@AdMedika", response.ApiName);

            var input = JObject.Parse(transport.LastBody)["input"];
            Assert.AreEqual("HELLO_WORLD", input["serviceID"].ToString());
            Assert.AreEqual("CUST001", input["txnData"]["helloRequest"]["customerID"].ToString());
        }

        [TestMethod]
        public void TransportFailure_WithoutBody_ReturnsTechnicalError()
        {
            var transport = new FakeTransport
            {
                NextException = new AdmedikaTransportException("timeout", null, null)
            };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckIcdExclusion("8000991200054479", "02", "J06");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("timeout", response.TechnicalError);
            Assert.IsFalse(string.IsNullOrEmpty(response.StatusMsg));
        }

        [TestMethod]
        public void TransportFailure_WithBusinessBody_ParsesErrorAndKeepsTechnicalError()
        {
            const string failedBody =
                "{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\","
                + "\"format\":\"JSON\",\"txnData\":{\"edocsUploadResponse\":{\"edocsUpload\":{"
                + "\"errorCode\":\"1833\",\"errorDesc\":\"Icd is exclusion\"}}},"
                + "\"txnResponseDateTime\":\"20261030143012\"}}";

            var transport = new FakeTransport
            {
                NextException = new AdmedikaTransportException("HTTP 500", failedBody, null)
            };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckIcdExclusion("8000991200054479", "02", "B01.9");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1833", response.ErrorCode);
            Assert.AreEqual("Icd is exclusion", response.ErrorDesc);
            Assert.AreEqual("HTTP 500", response.TechnicalError);
        }

        [TestMethod]
        public void InvalidJsonResponse_ReturnsTechnicalErrorWithoutThrowing()
        {
            var transport = new FakeTransport { NextResponse = "not-json" };
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckIcdExclusion("8000991200054479", "02", "J06");

            Assert.IsFalse(response.IsSuccess);
            Assert.IsFalse(string.IsNullOrEmpty(response.TechnicalError));
        }

        [TestMethod]
        public void Logger_ReceivesServiceIdAndBodies()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var logger = new RecordingLogger();
            var service = new Service(CreateConfig(), transport, logger);

            service.CheckIcdExclusion("8000991200054479", "02", "J06");

            Assert.AreEqual(1, logger.CallCount);
            Assert.AreEqual("CHECK_ICD_EXCLUSION", logger.LastServiceId);
            StringAssert.Contains(logger.LastRequestBody, "checkIcdExclusion");
            StringAssert.Contains(logger.LastResponseBody, "Juan Tamad");
            Assert.IsTrue(logger.LastElapsedMs >= 0);
        }
    }
}
