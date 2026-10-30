using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaResponseParserTests
    {
        private const string CheckIcdSuccessJson =
            "{\"output\":{\"referenceID\":\"222074\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"checkIcdExclusionResponse\":{\"checkIcdExclusion\":{"
            + "\"PayorName\":\"Jaminan Kesehatan\",\"cardNo\":\"8000991200054479\",\"covID\":\"02\","
            + "\"diagnosisCodeList\":\"J06, J05, B00.1\",\"dob\":\"Apr-21-1968\",\"patientName\":\"Juan Tamad\","
            + "\"payorMemberID\":\"xoxo-01DED\",\"pd\":\"Principal\",\"planCode\":\"OP-850\","
            + "\"policyDate\":\"Jan-01-2022 to Dec-31-2023\",\"policyNo\":\"xixixi-01DED\",\"principalName\":\"\"}}},"
            + "\"txnResponseDateTime\":\"20261030143012\"}}";

        private const string CheckIcdFailedJson =
            "{\"output\":{\"referenceID\":\"222075\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\","
            + "\"format\":\"JSON\",\"txnData\":{\"edocsUploadResponse\":{\"edocsUpload\":{"
            + "\"errorCode\":1833,"
            + "\"errorDesc\":\"Rejected -> Diagnosis [B01.9] was exclusion on Plan Level [GP]\"}}},"
            + "\"txnResponseDateTime\":\"20261030143013\"}}";

        [TestMethod]
        public void Parse_SuccessResponse_MapsEnvelopeAndPayload()
        {
            var response = ResponseParser.Parse<CheckIcdExclusionResponse>(CheckIcdSuccessJson);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("00", response.StatusCode);
            Assert.AreEqual("222074", response.ReferenceID);
            Assert.AreEqual("Juan Tamad", response.PatientName);
            Assert.AreEqual("OP-850", response.PlanCode);
            Assert.AreEqual("Jaminan Kesehatan", response.PayorName);
            Assert.AreEqual("8000991200054479", response.CardNo);
            Assert.IsNull(response.ErrorCode);
            Assert.IsNull(response.TechnicalError);
        }

        [TestMethod]
        public void Parse_FailedResponse_ReadsNumericErrorCodeFromWrongWrapperName()
        {
            var response = ResponseParser.Parse<CheckIcdExclusionResponse>(CheckIcdFailedJson);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("910", response.StatusCode);
            Assert.AreEqual("1833", response.ErrorCode);
            StringAssert.Contains(response.ErrorDesc, "exclusion");
        }

        [TestMethod]
        public void Parse_DoubleNestedOutput_IsTolerated()
        {
            var json = "{\"output\":{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\","
                       + "\"format\":\"JSON\",\"txnResponseDateTime\":\"20261030143012\"}}}";

            var response = ResponseParser.Parse<HelloWorldResponse>(json);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("1", response.ReferenceID);
        }

        [TestMethod]
        public void Parse_EmptyOrMalformedJson_ReturnsTechnicalError()
        {
            var empty = ResponseParser.Parse<HelloWorldResponse>("");
            var malformed = ResponseParser.Parse<HelloWorldResponse>("not a json");

            Assert.IsFalse(string.IsNullOrEmpty(empty.TechnicalError));
            Assert.IsFalse(string.IsNullOrEmpty(malformed.TechnicalError));
            Assert.IsFalse(empty.IsSuccess);
        }

        [TestMethod]
        public void Parse_PayloadWithUnexpectedJsonType_ReturnsTechnicalErrorWithoutThrowing()
        {
            const string json =
                "{\"output\":{\"referenceID\":\"1\",\"statusCode\":\"910\",\"statusMsg\":\"error\","
                + "\"format\":\"JSON\",\"txnData\":{\"errorResponse\":{\"diag\":{"
                + "\"diagnosisCodeList\":[\"J06\",\"J05\"],\"errorCode\":{\"code\":1833}}}},"
                + "\"txnResponseDateTime\":\"20261030143012\"}}";

            var response = ResponseParser.Parse<CheckIcdExclusionResponse>(json);

            Assert.IsFalse(string.IsNullOrEmpty(response.TechnicalError));
            Assert.AreEqual("910", response.StatusCode);
            Assert.AreEqual("1", response.ReferenceID);
        }

        [TestMethod]
        public void Parse_HelloWorldPayload()
        {
            const string json =
                "{\"output\":{\"referenceID\":\"586\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
                + "\"txnData\":{\"helloResponse\":{\"hello\":{\"message\":\"Hello there\","
                + "\"apiName\":\"Gateway@AdMedika\",\"apiVersion\":\"Rust 1.0.1\"}}},"
                + "\"txnResponseDateTime\":\"20171121145633\"}}";

            var response = ResponseParser.Parse<HelloWorldResponse>(json);

            Assert.AreEqual("Hello there", response.Message);
            Assert.AreEqual("Gateway@AdMedika", response.ApiName);
            Assert.AreEqual("Rust 1.0.1", response.ApiVersion);
        }
    }
}
