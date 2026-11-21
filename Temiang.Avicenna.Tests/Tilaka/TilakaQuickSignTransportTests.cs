using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignTransportTests
    {
        [TestMethod]
        public void MultipartBody_ContainsFileHeadersAndContent()
        {
            var content = Encoding.UTF8.GetBytes("%PDF-1.4 test");
            var body = HttpTilakaTransport.BuildMultipartBody(
                "BOUNDARY", "file", "casemix.pdf", content, "application/pdf");
            var text = Encoding.UTF8.GetString(body);

            StringAssert.Contains(text, "name=\"file\"");
            StringAssert.Contains(text, "filename=\"casemix.pdf\"");
            StringAssert.Contains(text, "Content-Type: application/pdf");
            StringAssert.Contains(text, "%PDF-1.4 test");
            StringAssert.Contains(text, "BOUNDARY--");
        }

        [TestMethod]
        public void MultipartBody_SanitizesQuotesInFileName()
        {
            var body = HttpTilakaTransport.BuildMultipartBody(
                "B", "file", "a\"b.pdf", new byte[] { 1 }, "application/pdf");
            var text = Encoding.UTF8.GetString(body);

            Assert.IsFalse(text.Contains("a\"b.pdf"));
            StringAssert.Contains(text, "a_b.pdf");
        }
    }
}
