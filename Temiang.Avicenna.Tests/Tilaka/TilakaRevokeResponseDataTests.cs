using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaRevokeResponseDataTests
    {
        [TestMethod]
        public void TryCreate_ValidData_ReturnsIdAndUrl()
        {
            var response = new RequestRevokeCertificateResponse
            {
                Success = true,
                Data = new[] { "rev-1", "https://example/revoke" }
            };

            TilakaRevokeResponseData data;
            var ok = TilakaRevokeResponseData.TryCreate(response, out data);

            Assert.IsTrue(ok);
            Assert.AreEqual("rev-1", data.RevokeId);
            Assert.AreEqual("https://example/revoke", data.RevokeUrl);
        }

        [TestMethod]
        public void TryCreate_NullOrShortData_ReturnsFalse()
        {
            TilakaRevokeResponseData data;

            Assert.IsFalse(TilakaRevokeResponseData.TryCreate(null, out data));
            Assert.IsFalse(TilakaRevokeResponseData.TryCreate(
                new RequestRevokeCertificateResponse { Data = null }, out data));
            Assert.IsFalse(TilakaRevokeResponseData.TryCreate(
                new RequestRevokeCertificateResponse { Data = new[] { "rev-1" } }, out data));
            Assert.IsFalse(TilakaRevokeResponseData.TryCreate(
                new RequestRevokeCertificateResponse { Data = new[] { " ", "https://example/revoke" } }, out data));
        }
    }
}
