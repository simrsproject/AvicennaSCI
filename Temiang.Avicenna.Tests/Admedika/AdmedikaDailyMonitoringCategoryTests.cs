using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;
using Temiang.Avicenna.Module.RADT.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaDailyMonitoringCategoryTests
    {
        [TestMethod]
        public void All_ContainsExpectedCategories()
        {
            var codes = AdmedikaDailyMonitoringCategory.All.Select(x => x.Code).ToArray();

            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 4, 5, 6, 7, 8, 9, 10, 11, 12, 15 },
                codes);
        }

        [TestMethod]
        public void Get_ReturnsDefaultFormCode()
        {
            Assert.AreEqual(AdmedikaConstant.DailyMonitoringFormID.DCM003C,
                AdmedikaDailyMonitoringCategory.Get(1).DefaultFormCode);
            Assert.AreEqual(AdmedikaConstant.DailyMonitoringFormID.DCM010A,
                AdmedikaDailyMonitoringCategory.Get(8).DefaultFormCode);
            Assert.AreEqual(AdmedikaConstant.DailyMonitoringFormID.DCM011A,
                AdmedikaDailyMonitoringCategory.Get(9).DefaultFormCode);
            Assert.AreEqual(AdmedikaConstant.DailyMonitoringFormID.DCM002A,
                AdmedikaDailyMonitoringCategory.Get(15).DefaultFormCode);
        }

        [TestMethod]
        public void NameOf_ReturnsNameOrCode()
        {
            Assert.AreEqual("Plan / Kamar", AdmedikaDailyMonitoringCategory.NameOf(1));
            Assert.AreEqual("99", AdmedikaDailyMonitoringCategory.NameOf(99));
        }
    }
}
