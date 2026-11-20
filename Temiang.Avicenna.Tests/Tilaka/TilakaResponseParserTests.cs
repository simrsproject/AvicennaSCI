using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaResponseParserTests
    {
        [TestMethod]
        public void Parse_GenerateUuid_ReadsDataArray()
        {
            var json = "{\"success\":true,\"message\":\"Success\",\"data\":[\"094a6148-0da8-411c-87ba-5d4f2d43f3eb\"]}";
            var result = TilakaResponseParser.Parse<GenerateUuidResponse>(json, 200);

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual("094a6148-0da8-411c-87ba-5d4f2d43f3eb", result.Data[0]);
        }

        [TestMethod]
        public void Parse_BusinessFailure_IsNotOk()
        {
            var json = "{\"success\":false,\"message\":\"NIK sedang dalam proses pendaftaran/verifikasi\",\"data\":null}";
            var result = TilakaResponseParser.Parse<RegisterForKycCheckResponse>(json, 200);

            Assert.IsFalse(result.IsOk);
            Assert.AreEqual("NIK sedang dalam proses pendaftaran/verifikasi", result.Message);
        }

        [TestMethod]
        public void Parse_CheckCertStatus_AcceptsStringStatus()
        {
            var json = "{\"success\":true,\"status\":\"3\",\"message\":\"Aktif\",\"data\":[]}";
            var result = TilakaResponseParser.Parse<CheckCertStatusResponse>(json, 200);

            Assert.AreEqual(3, result.Status);
            Assert.AreEqual("Aktif", result.Message.Info);
        }

        [TestMethod]
        public void Parse_CheckCertStatus_AcceptsObjectMessage()
        {
            var json = "{\"success\":true,\"status\":2,\"message\":{\"info\":\"Ada sertifikat yang butuh approval\",\"name\":\"anita\",\"email\":\"anita@mail.com\",\"company\":\"PT RS\",\"serialnumber\":\"ABC\"},\"data\":[{\"status\":\"Registered\",\"serialnumber\":\"ABC\"}]}";
            var result = TilakaResponseParser.Parse<CheckCertStatusResponse>(json, 200);

            Assert.AreEqual(2, result.Status);
            Assert.AreEqual("Ada sertifikat yang butuh approval", result.Message.Info);
            Assert.AreEqual("PT RS", result.Message.Company);
            Assert.AreEqual("ABC", result.Data[0].SerialNumber);
        }

        [TestMethod]
        public void Parse_UserRegStatus_ReadsNestedData()
        {
            var json = "{\"success\":true,\"message\":\"ok\",\"data\":{\"status\":\"S\",\"nik\":true,\"nama\":true,\"tilaka_name\":\"anita001\",\"reason_code\":\"0\",\"manual_registration_status\":null}}";
            var result = TilakaResponseParser.Parse<UserRegStatusResponse>(json, 200);

            Assert.AreEqual("S", result.Data.Status);
            Assert.AreEqual("anita001", result.Data.TilakaName);
            Assert.IsNull(result.Data.ManualRegistrationStatus);
        }

        [TestMethod]
        public void Parse_CheckCertStatus_PreservesDateStrings()
        {
            var json = "{\"success\":true,\"status\":1,\"message\":\"Aktif\",\"data\":[{\"status\":\"Registered\",\"serialnumber\":\"ABC\",\"start_active_date\":\"2025-05-17 14:39:07\",\"expiry_date\":\"2026-05-17 14:39:07\"}]}";
            var result = TilakaResponseParser.Parse<CheckCertStatusResponse>(json, 200);

            Assert.AreEqual("2025-05-17 14:39:07", result.Data[0].StartActiveDate);
            Assert.AreEqual("2026-05-17 14:39:07", result.Data[0].ExpiryDate);
        }

        [TestMethod]
        public void Parse_NonJson_SetsTechnicalError()
        {
            var result = TilakaResponseParser.Parse<GenerateUuidResponse>("<html>error</html>", 500);

            Assert.IsFalse(result.IsOk);
            Assert.IsFalse(string.IsNullOrEmpty(result.TechnicalError));
        }
    }
}
