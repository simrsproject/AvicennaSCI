using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaCasemixFileRulesTests
    {
        [TestMethod]
        public void CanDelete_NoRequest_ReturnsTrue()
        {
            Assert.IsTrue(TilakaCasemixFileRules.CanDelete(false));
        }

        [TestMethod]
        public void CanDelete_HasRequest_ReturnsFalse()
        {
            Assert.IsFalse(TilakaCasemixFileRules.CanDelete(true));
        }
    }
}
