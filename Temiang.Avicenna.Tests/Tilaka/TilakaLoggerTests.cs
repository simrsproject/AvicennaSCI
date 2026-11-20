using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaLoggerTests
    {
        [TestMethod]
        public void Mask_HidesNikAndEmail()
        {
            var masked = WebServiceApiLogger.Mask("{\"nik\":\"3273010101900001\",\"email\":\"anita@mail.com\"}");
            Assert.IsFalse(masked.Contains("3273010101900001"));
            Assert.IsFalse(masked.Contains("anita@mail.com"));
            StringAssert.Contains(masked, "0001");
        }

        [TestMethod]
        public void Mask_ReplacesBase64Payload()
        {
            var masked = WebServiceApiLogger.Mask("{\"photo_ktp\":\"data:image/jpeg;base64,AAAA\"}");
            StringAssert.Contains(masked, "[base64");
        }

        [TestMethod]
        public void Mask_HidesAccessToken()
        {
            var masked = WebServiceApiLogger.Mask("{\"access_token\":\"secret-token\"}");
            Assert.IsFalse(masked.Contains("secret-token"));
        }
    }
}
