using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaEdocsUploadTests
    {
        [TestMethod]
        public void BuildMultipartBody_WritesDataFieldAndOnePartPerFile()
        {
            var files = new[]
            {
                new AdmedikaFile
                {
                    FileName = "doc1.txt", ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("FILE-ONE")
                },
                new AdmedikaFile
                {
                    FileName = "doc2.txt", ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("FILE-TWO")
                }
            };

            var body = HttpAdmedikaTransport.BuildMultipartBody("BOUNDARY", "{\"input\":{}}", files);
            var text = Encoding.UTF8.GetString(body);

            StringAssert.Contains(text, "--BOUNDARY\r\n");
            StringAssert.Contains(text, "name=\"data\"");
            StringAssert.Contains(text, "{\"input\":{}}");
            StringAssert.Contains(text, "name=\"files\"; filename=\"doc1.txt\"");
            StringAssert.Contains(text, "FILE-ONE");
            StringAssert.Contains(text, "name=\"files\"; filename=\"doc2.txt\"");
            StringAssert.Contains(text, "FILE-TWO");
            StringAssert.EndsWith(text, "--BOUNDARY--\r\n");
        }

        [TestMethod]
        public void BuildMultipartBody_SanitizesQuotesInFileName()
        {
            var files = new[]
            {
                new AdmedikaFile
                {
                    FileName = "bad\"name.txt", ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("X")
                }
            };

            var text = Encoding.UTF8.GetString(
                HttpAdmedikaTransport.BuildMultipartBody("B", "{}", files));

            StringAssert.Contains(text, "filename=\"bad_name.txt\"");
            Assert.IsFalse(text.Contains("bad\"name.txt"));
        }

        [TestMethod]
        public void BuildMultipartBody_StripsControlCharactersAndDefaultsEmptyName()
        {
            var files = new[]
            {
                new AdmedikaFile
                {
                    FileName = "a\r\nX-Evil: 1.txt", ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("A")
                },
                new AdmedikaFile
                {
                    FileName = "   ", ContentType = "text/plain",
                    Content = Encoding.UTF8.GetBytes("B")
                }
            };

            var text = Encoding.UTF8.GetString(
                HttpAdmedikaTransport.BuildMultipartBody("B", "{}", files));

            Assert.IsFalse(text.Contains("\r\nX-Evil"));
            StringAssert.Contains(text, "filename=\"a__X-Evil: 1.txt\"");
            StringAssert.Contains(text, "filename=\"file\"");
        }

        [TestMethod]
        public void BuildMultipartBody_HasOneDataPartAndOnePartPerFile()
        {
            var files = new[]
            {
                new AdmedikaFile { FileName = "a.txt", Content = Encoding.UTF8.GetBytes("A") },
                new AdmedikaFile { FileName = "b.txt", Content = Encoding.UTF8.GetBytes("B") }
            };

            var body = HttpAdmedikaTransport.BuildMultipartBody("B", "{}", files);
            var text = Encoding.UTF8.GetString(body);

            Assert.AreEqual(1, CountOccurrences(text, "name=\"data\""));
            Assert.AreEqual(2, CountOccurrences(text, "name=\"files\""));
            Assert.AreEqual(3, CountOccurrences(text, "--B\r\n"));
        }

        [TestMethod]
        public void BuildMultipartBody_PreservesBinaryContent()
        {
            var content = new byte[] { 0x00, 0xFF, 0x0D, 0x0A, 0x7F };
            var files = new[]
            {
                new AdmedikaFile { FileName = "a.bin", Content = content }
            };

            var body = HttpAdmedikaTransport.BuildMultipartBody("B", "{}", files);

            Assert.IsTrue(ContainsSequence(body, content));
        }

        private static int CountOccurrences(string value, string token)
        {
            var count = 0;
            var index = 0;
            while ((index = value.IndexOf(token, index, System.StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }

            return count;
        }

        private static bool ContainsSequence(byte[] haystack, byte[] needle)
        {
            for (var i = 0; i <= haystack.Length - needle.Length; i++)
            {
                var match = true;
                for (var j = 0; j < needle.Length; j++)
                {
                    if (haystack[i + j] != needle[j]) { match = false; break; }
                }

                if (match) return true;
            }

            return false;
        }

        private const string SuccessJson =
            "{\"output\":{\"referenceID\":\"7001\",\"statusCode\":\"00\",\"statusMsg\":\"Successful\",\"format\":\"JSON\","
            + "\"txnData\":{\"edocsUploadResponse\":{\"edocsUpload\":{\"msg\":\"Successfully\"}}},"
            + "\"txnResponseDateTime\":\"20261030190000\"}}";

        private const string FailedJson =
            "{\"output\":{\"referenceID\":\"7002\",\"statusCode\":\"910\",\"statusMsg\":\"error on txnData\",\"format\":\"JSON\","
            + "\"txnData\":{\"edocsUploadResponse\":{\"edocsUpload\":{\"errorCode\":\"1838\",\"errorDesc\":\"claims ID not found\"}}},"
            + "\"txnResponseDateTime\":\"20261030190001\"}}";

        private class FakeTransport : IAdmedikaTransport
        {
            public string NextResponse;
            public int MultipartCallCount;
            public string LastMultipartUrl;
            public string LastMultipartData;
            public AdmedikaFile[] LastFiles;

            public string Post(string url, string jsonBody, int timeoutSeconds)
            {
                throw new AssertFailedException("Post biasa tidak boleh dipakai untuk EDOCS_UPLOAD.");
            }

            public string PostMultipart(string url, string jsonBody, AdmedikaFile[] files, int timeoutSeconds)
            {
                MultipartCallCount++;
                LastMultipartUrl = url;
                LastMultipartData = jsonBody;
                LastFiles = files;
                return NextResponse;
            }
        }

        [TestMethod]
        public void EdocsUpload_PostsDataAndFilesToApiEndpoint()
        {
            var transport = new FakeTransport { NextResponse = SuccessJson };
            var config = new AdmedikaConfig
            {
                ServiceUrl = "https://example.test/customerhost_dev",
                ApiServiceUrl = "https://example.test/api/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
            var service = new Service(config, transport, new NullAdmedikaLogger());
            var files = new[]
            {
                new AdmedikaFile
                {
                    FileName = "surat.pdf", ContentType = "application/pdf",
                    Content = Encoding.UTF8.GetBytes("%PDF-1.4")
                }
            };

            var response = service.EdocsUpload("8000991200054479", "12200001", "GL",
                "jaminan", "Juan Tamad", "user01", files);

            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual("Successfully", response.Msg);
            Assert.AreEqual("https://example.test/api/customerhost_dev", transport.LastMultipartUrl);
            Assert.AreEqual(1, transport.MultipartCallCount);
            Assert.AreEqual(1, transport.LastFiles.Length);
            Assert.AreEqual("surat.pdf", transport.LastFiles[0].FileName);

            var payload = Newtonsoft.Json.Linq.JObject.Parse(transport.LastMultipartData)["input"];
            Assert.AreEqual("EDOCS_UPLOAD", payload["serviceID"].ToString());
            var edocs = payload["txnData"]["edocsUploadRequest"]["edocsUpload"];
            Assert.AreEqual("8000991200054479", edocs["cardNo"].ToString());
            Assert.AreEqual("12200001", edocs["clID"].ToString());
            Assert.AreEqual("GL", edocs["docType"].ToString());
            Assert.AreEqual("user01", edocs["userBy"].ToString());
        }

        [TestMethod]
        public void EdocsUpload_FailedResponse_ReturnsBusinessError()
        {
            var transport = new FakeTransport { NextResponse = FailedJson };
            var config = new AdmedikaConfig
            {
                ApiServiceUrl = "https://example.test/api/customerhost_dev",
                CustomerID = "CUST001",
                SecurityWord = "secret",
                TimeoutSeconds = 17,
                IsLogEnabled = false
            };
            var service = new Service(config, transport, new NullAdmedikaLogger());

            var response = service.EdocsUpload("8000991200054479", "12200001", "GL",
                null, null, null, null);

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("1838", response.ErrorCode);
            Assert.AreEqual(1, transport.MultipartCallCount);
        }
    }
}
