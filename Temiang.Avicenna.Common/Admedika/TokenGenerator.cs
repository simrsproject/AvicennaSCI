using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Temiang.Avicenna.Common.Admedika
{
    public static class TokenGenerator
    {
        public static string CreateToken(string customerId, string securityWord, DateTime utcDateTime,
            string requestId, string serviceId)
        {
            if (customerId == null) throw new ArgumentNullException("customerId");
            if (securityWord == null) throw new ArgumentNullException("securityWord");
            if (requestId == null) throw new ArgumentNullException("requestId");
            if (serviceId == null) throw new ArgumentNullException("serviceId");

            var line = string.Concat(
                customerId, ":",
                Sha256Hex(securityWord), ":",
                utcDateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture), ":",
                requestId, ":",
                serviceId);

            return Sha256Hex(line);
        }

        public static string Sha256Hex(string value)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value ?? string.Empty));
                var builder = new StringBuilder(bytes.Length * 2);
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                return builder.ToString();
            }
        }
    }
}
