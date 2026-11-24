using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaSignedFileRulesTests
    {
        [TestMethod]
        public void BuildRelativePath_UsesYearMonthRequestAndFile()
        {
            var path = TilakaSignedFileRules.BuildRelativePath(
                "req-1", "doc.pdf", new DateTime(2026, 11, 24, 10, 30, 0));

            Assert.AreEqual("2026/11/req-1/doc.pdf", path);
        }

        [TestMethod]
        public void BuildRelativePath_SanitizesInvalidCharacters()
        {
            var path = TilakaSignedFileRules.BuildRelativePath(
                "req/1", "..\\doc.pdf", new DateTime(2026, 1, 2));

            Assert.AreEqual("2026/01/req_1/doc.pdf", path);
        }

        [TestMethod]
        public void ShouldUpdateDownloadPath_EmptyExistingPath_ReturnsTrue()
        {
            Assert.IsTrue(TilakaSignedFileRules.ShouldUpdateDownloadPath(null, 2, 1));
        }

        [TestMethod]
        public void ShouldUpdateDownloadPath_LatestRequest_ReturnsTrue()
        {
            Assert.IsTrue(TilakaSignedFileRules.ShouldUpdateDownloadPath("old", 2, 2));
        }

        [TestMethod]
        public void ShouldUpdateDownloadPath_OlderRequest_ReturnsFalse()
        {
            Assert.IsFalse(TilakaSignedFileRules.ShouldUpdateDownloadPath("old", 2, 1));
        }
    }
}
