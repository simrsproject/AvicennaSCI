using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Tests.Admedika
{
    [TestClass]
    public class AdmedikaMaskTests
    {
        [TestMethod]
        public void CardNo_SixteenDigits_MasksMiddleEight()
        {
            Assert.AreEqual("8000********4479", AdmedikaMask.CardNo("8000991200054479"));
        }

        [TestMethod]
        public void CardNo_ShortOrNonNumeric_DoesNotThrowAndDoesNotLeak()
        {
            Assert.AreEqual("******", AdmedikaMask.CardNo("ABC123"));
            Assert.AreEqual(string.Empty, AdmedikaMask.CardNo(string.Empty));
            Assert.IsNull(AdmedikaMask.CardNo(null));
        }

        [TestMethod]
        public void MaskSensitive_ReplacesCardNoOnlyInsideJson()
        {
            const string json =
                "{\"input\":{\"txnData\":{\"cardNo\":\"8000991200054479\",\"covID\":\"02\"}}}";

            var masked = WebServiceApiLogger.MaskSensitive(json);

            StringAssert.Contains(masked, "\"cardNo\":\"8000********4479\"");
            StringAssert.Contains(masked, "\"covID\":\"02\"");
            Assert.IsFalse(masked.Contains("8000991200054479"));
        }
    }
}
