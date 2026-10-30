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
    }
}
