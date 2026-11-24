using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaTemplateSignFileRulesTests
    {
        [TestMethod]
        public void CanRegister_ActiveAndNotRegistered_ReturnsTrue()
        {
            Assert.IsTrue(TilakaTemplateSignFileRules.CanRegister(true, false));
        }

        [TestMethod]
        public void CanRegister_InactiveOrRegistered_ReturnsFalse()
        {
            Assert.IsFalse(TilakaTemplateSignFileRules.CanRegister(false, false));
            Assert.IsFalse(TilakaTemplateSignFileRules.CanRegister(true, true));
        }

        [TestMethod]
        public void ResolveIsDefault_Inactive_ForcesFalse()
        {
            Assert.IsFalse(TilakaTemplateSignFileRules.ResolveIsDefault(false, true));
        }

        [TestMethod]
        public void ResolveIsDefault_ActiveAndRequested_ReturnsTrue()
        {
            Assert.IsTrue(TilakaTemplateSignFileRules.ResolveIsDefault(true, true));
            Assert.IsFalse(TilakaTemplateSignFileRules.ResolveIsDefault(true, false));
        }

        [TestMethod]
        public void NextTemplateNo_FirstTemplate_Returns01()
        {
            var templateNo = TilakaTemplateSignFileRules.NextTemplateNo(
                "A001", new List<string>());

            Assert.AreEqual("TTD-A001-01", templateNo);
        }

        [TestMethod]
        public void NextTemplateNo_ExistingTemplates_ReturnsNextSequence()
        {
            var templateNo = TilakaTemplateSignFileRules.NextTemplateNo(
                "a001", new List<string> { "TTD-A001-01", "TTD-A001-02" });

            Assert.AreEqual("TTD-A001-03", templateNo);
        }
    }
}
