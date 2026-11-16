using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaDocTypeTests
    {
        [TestMethod]
        public void DocType_ContainsDocumentedList()
        {
            var items = AdmedikaDocType.All;

            Assert.AreEqual(28, items.Count);
            Assert.AreEqual(8, items.Count(i => i.Group == AdmedikaDocType.GroupEForms));
            Assert.AreEqual(20, items.Count(i => i.Group == AdmedikaDocType.GroupMandatoryEdocs));
            Assert.AreEqual(28, items.Select(i => i.Code).Distinct().Count());

            Assert.AreEqual("Laporan Medis Awal", AdmedikaDocType.Get("LMA").Description);
            Assert.AreEqual("Surat Jaminan Biaya (Guarantee Letter)",
                AdmedikaDocType.Get("GL").Description);
            Assert.AreEqual("Other related documents",
                AdmedikaDocType.Get("OTHERS").Description);
        }

        [TestMethod]
        public void DocType_ValidatesKnownAndUnknownCodes()
        {
            foreach (var item in AdmedikaDocType.All)
            {
                Assert.IsTrue(AdmedikaDocType.IsValid(item.Code), item.Code);
            }

            Assert.IsTrue(AdmedikaDocType.IsValid(" lma "));
            Assert.IsTrue(AdmedikaDocType.IsValid("gl"));
            Assert.AreEqual("GL", AdmedikaDocType.Get(" gl ").Code);

            Assert.IsFalse(AdmedikaDocType.IsValid("XXX"));
            Assert.IsFalse(AdmedikaDocType.IsValid(""));
            Assert.IsFalse(AdmedikaDocType.IsValid(null));
        }

        [TestMethod]
        public void DocType_AllowedTextContainsEveryCode()
        {
            var text = AdmedikaDocType.AllowedText();

            foreach (var item in AdmedikaDocType.All)
            {
                StringAssert.Contains(text, item.Code);
            }
        }
    }
}
