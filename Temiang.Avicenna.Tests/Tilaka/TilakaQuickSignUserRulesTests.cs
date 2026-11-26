using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaQuickSignUserRulesTests
    {
        [TestMethod]
        public void NullRegistration_IsNotRegistered()
        {
            Assert.IsFalse(TilakaQuickSignUserRules.IsRegistered(null));
        }

        [TestMethod]
        public void QuickSignUserFalse_IsNotRegistered()
        {
            var record = new TilakaRegistrationRecord { IsQuickSignUser = false };

            Assert.IsFalse(TilakaQuickSignUserRules.IsRegistered(record));
        }

        [TestMethod]
        public void QuickSignUserTrue_IsRegistered()
        {
            var record = new TilakaRegistrationRecord { IsQuickSignUser = true };

            Assert.IsTrue(TilakaQuickSignUserRules.IsRegistered(record));
        }
    }
}
