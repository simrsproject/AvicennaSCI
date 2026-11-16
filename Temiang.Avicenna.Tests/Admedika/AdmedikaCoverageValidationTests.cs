using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaCoverageValidationTests
    {
        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
            + "\"format\":\"JSON\",\"txnData\":{},\"txnResponseDateTime\":\"20261030220000\"}}";

        private class FakeTransport : IAdmedikaTransport
        {
            public int CallCount;
            public string LastBody;

            public string Post(string url, string jsonBody, int timeoutSeconds)
            {
                CallCount++;
                LastBody = jsonBody;
                return SuccessJson;
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
        public void CoverageType_ContainsDocumentedList()
        {
            var items = AdmedikaCoverageType.All;

            Assert.AreEqual(8, items.Count);
            CollectionAssert.AreEqual(
                new[] { "01", "02", "03", "04", "05", "06", "07", "08" },
                items.Select(i => i.CoverageID).ToArray());
            CollectionAssert.AreEqual(
                new[] { "HNS", "GP", "SP", "OP", "DENTAL", "MAT", "LAB", "PHAR" },
                items.Select(i => i.CoverageCode).ToArray());
            CollectionAssert.AreEqual(
                new[] { "RAWAT INAP", "RAWAT JALAN", "PRE & POST", "KACAMATA", "RAWAT GIGI",
                        "PERSALINAN", "Laboratory", "Pharmacy/Apotek" },
                items.Select(i => i.Description).ToArray());
            Assert.AreEqual("01, 02, 03, 04, 05, 06, 07, 08", AdmedikaCoverageType.AllowedText());
        }

        [TestMethod]
        public void CoverageType_ValidatesKnownAndUnknownIds()
        {
            Assert.IsTrue(AdmedikaCoverageType.IsValid("01"));
            Assert.IsTrue(AdmedikaCoverageType.IsValid("08"));
            Assert.IsTrue(AdmedikaCoverageType.IsValid(" 02 "));
            Assert.IsFalse(AdmedikaCoverageType.IsValid("09"));
            Assert.IsFalse(AdmedikaCoverageType.IsValid("2"));
            Assert.IsFalse(AdmedikaCoverageType.IsValid("GP"));
            Assert.IsFalse(AdmedikaCoverageType.IsValid(""));
            Assert.IsFalse(AdmedikaCoverageType.IsValid(null));
            Assert.AreEqual("GP", AdmedikaCoverageType.Get("02").CoverageCode);
        }

        [TestMethod]
        public void CheckIcdExclusion_InvalidCovID_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport();
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.CheckIcdExclusion("8000991200054479", "09", "J06");

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1820", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "Coverage code is invalid");
            Assert.AreEqual(0, transport.CallCount);
        }

        [TestMethod]
        public void Eligibility_EmptyCovID_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport();
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.Eligibility("8000991200054479", "", "J06",
                null, null, null, null, null, null, null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1999", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "cannot be empty");
            Assert.AreEqual(0, transport.CallCount);
        }

        [TestMethod]
        public void Eligibility_TrimsValidCovIDBeforeSending()
        {
            var transport = new FakeTransport();
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.Eligibility("8000991200054479", " 02 ", "J06",
                null, null, null, null, null, null, null, null, null);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(1, transport.CallCount);

            var payload = JObject.Parse(transport.LastBody)["input"]["txnData"]["eligibilityRequest"]["eligibility"];
            Assert.AreEqual("02", payload["covID"].ToString());
        }

        [TestMethod]
        public void GetMemberEnrolledPlanTc_InvalidCovID_IsRejectedBeforeTransport()
        {
            var transport = new FakeTransport();
            var service = new Service(CreateConfig(), transport, new NullAdmedikaLogger());

            var response = service.GetMemberEnrolledPlanTc("8000991200054479", "99", null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1820", response.ErrorCode);
            Assert.AreEqual(0, transport.CallCount);
        }
    }
}
