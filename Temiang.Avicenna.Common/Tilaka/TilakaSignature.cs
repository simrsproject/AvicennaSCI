using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaSignature
    {
        public static string Sha256Hex(string value)
        {
            using (var sha = SHA256.Create())
            {
                return ToHex(sha.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty)));
            }
        }

        public static string HmacSha256Hex(string key, string message)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key ?? string.Empty)))
            {
                return ToHex(hmac.ComputeHash(Encoding.UTF8.GetBytes(message ?? string.Empty)));
            }
        }

        public static string CreateConsentHash(string clientSecret, string channelId, string consentText,
            string version, string consentTimestamp)
        {
            var message = string.Concat(channelId, consentText, version, consentTimestamp);
            return HmacSha256Hex(clientSecret, message);
        }

        public static bool VerifyCallback(string clientId, string clientSecret, string timestamp,
            string rawBody, string providedToken, int toleranceMinutes, out string error)
        {
            error = null;

            if (string.IsNullOrWhiteSpace(providedToken))
            {
                error = "missing-token";
                return false;
            }

            DateTime parsedTimestamp;
            if (!DateTime.TryParseExact((timestamp ?? string.Empty).Trim(), "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTimestamp)
                && !DateTime.TryParse(timestamp, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedTimestamp))
            {
                error = "invalid-timestamp";
                return false;
            }

            if (Math.Abs((DateTime.Now - parsedTimestamp).TotalMinutes) > Math.Max(toleranceMinutes, 1))
            {
                error = "timestamp-out-of-range";
                return false;
            }

            var expected = HmacSha256Hex(clientSecret, string.Concat(clientId, timestamp, rawBody));
            if (!FixedTimeEquals(expected, providedToken.Trim()))
            {
                error = "invalid-signature";
                return false;
            }

            return true;
        }

        public static bool FixedTimeEquals(string left, string right)
        {
            left = left ?? string.Empty;
            right = right ?? string.Empty;

            var different = left.Length ^ right.Length;
            var length = Math.Min(left.Length, right.Length);
            for (var i = 0; i < length; i++) different |= left[i] ^ right[i];
            return different == 0;
        }

        private static string ToHex(byte[] bytes)
        {
            var builder = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return builder.ToString();
        }
    }
}
