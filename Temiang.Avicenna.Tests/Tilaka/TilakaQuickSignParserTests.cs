using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignParserTests
    {
        [TestMethod]
        public void Upload_ParsesFilename()
        {
            var result = TilakaResponseParser.Parse<QuickSignUploadResponse>(
                "{\"success\":true,\"message\":\"File uploaded successfully\",\"filename\":\"abc_Test.pdf\"}", 200);

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual("abc_Test.pdf", result.FileName);
        }

        [TestMethod]
        public void RequestSign_ParsesAuthResponse()
        {
            var json = "{\"success\":true,\"message\":\"request quicksign success\",\"auth_response\":[{\"url\":\"http://auth\",\"user_identifier\":\"dr001\",\"status\":false,\"error_message\":null,\"token_key\":\"tk\"}],\"failed_doc_name\":null}";
            var result = TilakaResponseParser.Parse<QuickSignRequestResponse>(json, 200);

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual(1, result.AuthResponse.Length);
            Assert.AreEqual("http://auth", result.AuthResponse[0].Url);
            Assert.IsFalse(result.AuthResponse[0].Status);
        }

        [TestMethod]
        public void CheckStatus_ParsesSignersAndPdf()
        {
            var json = "{\"success\":true,\"message\":\"DONE\",\"status\":[{\"sequence\":1,\"status\":\"DONE\",\"user_identifier\":\"dr001\",\"num_signatures\":1,\"num_signatures_done\":1}],\"request_id\":\"req-1\",\"list_pdf\":[{\"filename\":\"a.pdf\",\"error\":false,\"presigned_url\":\"http://file\"}]}";
            var result = TilakaResponseParser.Parse<QuickSignStatusResponse>(json, 200);

            Assert.AreEqual(1, result.Status.Length);
            Assert.AreEqual("DONE", result.Status[0].Status);
            Assert.AreEqual("http://file", result.ListPdf[0].PresignedUrl);
        }

        [TestMethod]
        public void Callback_ParsesNullSignInfo()
        {
            var json = "{\"success\":true,\"message\":\"DONE\",\"request_id\":\"req-1\",\"user_info\":{\"sequence\":1,\"status\":\"DONE\",\"user_identifier\":\"dr001\",\"num_signatures\":1,\"num_signatures_done\":1},\"sign_info\":null}";
            var result = TilakaResponseParser.Parse<CallbackSigningRequest>(json, 200);

            Assert.IsTrue(result.IsOk);
            Assert.AreEqual("req-1", result.RequestId);
            Assert.AreEqual("DONE", result.UserInfo.Status);
            Assert.IsNull(result.SignInfo);
        }

        [TestMethod]
        public void AuthSessions_ParsesDataAndPagination()
        {
            var json = "{\"success\":true,\"message\":\"ok\",\"data\":[{\"email\":\"a@b.com\",\"tilaka_name\":\"dr001\",\"auth_status\":\"active\",\"template_numbers\":[\"TTD-DR001-01\"]}],\"pagination\":{\"page_number\":1,\"limit\":5,\"total_elements\":1,\"total_pages\":1}}";
            var result = TilakaResponseParser.Parse<QuickSignAuthSessionResponse>(json, 200);

            Assert.AreEqual(1, result.Data.Length);
            Assert.AreEqual("active", result.Data[0].AuthStatus);
            Assert.AreEqual("TTD-DR001-01", result.Data[0].TemplateNumbers[0]);
            Assert.AreEqual(1, result.Pagination.TotalElements);
        }
    }
}
