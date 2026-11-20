using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaSignatureTests
    {
        // Vektor uji HMAC-SHA256 standar (RFC 4231 style, key "key").
        [TestMethod]
        public void HmacSha256Hex_MatchesKnownVector()
        {
            var result = TilakaSignature.HmacSha256Hex("key", "The quick brown fox jumps over the lazy dog");
            Assert.AreEqual("f7bc83f430538424b13298e6aa6fb143ef4d59a14946175997479dbc2d1a3cd8", result);
        }

        [TestMethod]
        public void CreateConsentHash_ConcatenatesWithoutSeparator()
        {
            var expected = TilakaSignature.HmacSha256Hex("secret",
                "chan" + "Terms" + "v1" + "2023-01-01 18:30:00");
            Assert.AreEqual(expected,
                TilakaSignature.CreateConsentHash("secret", "chan", "Terms", "v1", "2023-01-01 18:30:00"));
        }

        [TestMethod]
        public void VerifyCallback_AcceptsValidToken()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var body = "{\"user_identifier\":\"anita001\",\"status\":2}";
            var token = TilakaSignature.HmacSha256Hex("secret", "chan" + timestamp + body);

            string error;
            Assert.IsTrue(TilakaSignature.VerifyCallback("chan", "secret", timestamp, body, token, 60, out error), error);
        }

        [TestMethod]
        public void VerifyCallback_RejectsTamperedBody()
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var token = TilakaSignature.HmacSha256Hex("secret", "chan" + timestamp + "{\"status\":2}");

            string error;
            Assert.IsFalse(TilakaSignature.VerifyCallback("chan", "secret", timestamp, "{\"status\":3}", token, 60, out error));
            Assert.AreEqual("invalid-signature", error);
        }

        [TestMethod]
        public void VerifyCallback_RejectsStaleTimestamp()
        {
            var timestamp = DateTime.Now.AddHours(-2).ToString("yyyy-MM-dd HH:mm:ss");
            var body = "{}";
            var token = TilakaSignature.HmacSha256Hex("secret", "chan" + timestamp + body);

            string error;
            Assert.IsFalse(TilakaSignature.VerifyCallback("chan", "secret", timestamp, body, token, 5, out error));
            Assert.AreEqual("timestamp-out-of-range", error);
        }

        [TestMethod]
        public void FixedTimeEquals_HandlesDifferentLengths()
        {
            Assert.IsFalse(TilakaSignature.FixedTimeEquals("abc", "abcd"));
            Assert.IsTrue(TilakaSignature.FixedTimeEquals("abc", "abc"));
        }
    }
}
