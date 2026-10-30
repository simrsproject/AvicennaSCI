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
        public void CardNo_MidLength_DisclosesAtMostLastFour()
        {
            Assert.AreEqual("*****3456", AdmedikaMask.CardNo("ABC123456"));
            Assert.AreEqual("********9012", AdmedikaMask.CardNo("123456789012"));
        }

        [TestMethod]
        public void MaskSensitive_MasksUnquotedNumericCardNo()
        {
            const string json = "{\"output\":{\"cardNo\":8000991200054479,\"covID\":\"02\"}}";

            var masked = WebServiceApiLogger.MaskSensitive(json);

            Assert.IsFalse(masked.Contains("8000991200054479"));
            StringAssert.Contains(masked, "\"cardNo\":\"8000********4479\"");
            StringAssert.Contains(masked, "\"covID\":\"02\"");
        }

        [TestMethod]
        public void CreateLogEntry_UsesAdmedikaIpAndMasksCard()
        {
            var entry = WebServiceApiLogger.CreateLogEntry("CHECK_ICD_EXCLUSION",
                "{\"cardNo\":\"8000991200054479\"}",
                "{\"cardNo\":\"8000991200054479\"}", 12);

            Assert.AreEqual("ADMEDIKA", entry.IPAddress);
            Assert.AreEqual("CHECK_ICD_EXCLUSION", entry.UrlAddress);
            Assert.AreEqual(12, entry.Totalms ?? 0);
            StringAssert.Contains(entry.Params, "8000********4479");
            StringAssert.Contains(entry.Response, "8000********4479");
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
