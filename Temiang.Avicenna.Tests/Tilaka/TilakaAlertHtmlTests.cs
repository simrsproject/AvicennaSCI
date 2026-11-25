using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaAlertHtmlTests
    {
        [TestMethod]
        public void Success_UsesGreenBootstrapPalette()
        {
            var html = TilakaAlertHtml.Success("Berhasil");

            StringAssert.Contains(html, "#dff0d8");
            StringAssert.Contains(html, "#3c763d");
            StringAssert.Contains(html, "Berhasil");
        }

        [TestMethod]
        public void Error_UsesRedBootstrapPalette()
        {
            var html = TilakaAlertHtml.Error("Gagal");

            StringAssert.Contains(html, "#f2dede");
            StringAssert.Contains(html, "#d9534f");
            StringAssert.Contains(html, "Gagal");
        }

        [TestMethod]
        public void Build_EncodesMessage()
        {
            var html = TilakaAlertHtml.Error("<script>alert(1)</script>");

            StringAssert.Contains(html, "&lt;script&gt;");
            Assert.IsFalse(html.Contains("<script>"));
        }
    }
}
