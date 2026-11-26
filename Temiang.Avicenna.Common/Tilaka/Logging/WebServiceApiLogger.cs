using System;
using System.Text.RegularExpressions;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class WebServiceApiLogger : ITilakaLogger
    {
        private static readonly Regex NikPattern = new Regex(
            "(\"nik\"\\s*:\\s*)(\"([^\"]*)\"|([^,}\\]\\s]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex EmailPattern = new Regex(
            "(\"email\"\\s*:\\s*)(\"([^\"]*)\"|([^,}\\]\\s]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex Base64Pattern = new Regex(
            "(\"(photo_ktp|passport_file|company_supporting_document|photo_selfie|certificate)\"\\s*:\\s*)\"[^\"]*\"",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex TokenPattern = new Regex(
            "(\"(access_token|client_secret|refresh_token)\"\\s*:\\s*)\"[^\"]*\"",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex FormSecretPattern = new Regex(
            "(client_secret|password)=([^&\\s]+)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly TilakaConfig _config;

        public WebServiceApiLogger(TilakaConfig config)
        {
            _config = config ?? new TilakaConfig();
        }

        public void Log(string operation, string requestBody, string responseBody, long elapsedMs)
        {
            if (_config.IsLogEnabled == false) return;

            try
            {
                new WebServiceAPILog
                {
                    DateRequest = DateTime.Now,
                    IPAddress = "TILAKA",
                    UrlAddress = operation,
                    Params = Mask(requestBody),
                    Response = Mask(responseBody),
                    Totalms = (int)elapsedMs
                }.Save();
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }

        public static string Mask(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            json = TokenPattern.Replace(json, m => m.Groups[1].Value + "\"[redacted]\"");
            json = FormSecretPattern.Replace(json, m => m.Groups[1].Value + "=[redacted]");
            json = Base64Pattern.Replace(json, m => m.Groups[1].Value + "\"[base64]\"");
            json = NikPattern.Replace(json, m =>
            {
                var value = m.Groups[3].Success ? m.Groups[3].Value : m.Groups[4].Value;
                return m.Groups[1].Value + "\"" + MaskNik(value) + "\"";
            });
            json = EmailPattern.Replace(json, m =>
            {
                var value = m.Groups[3].Success ? m.Groups[3].Value : m.Groups[4].Value;
                return m.Groups[1].Value + "\"" + MaskEmail(value) + "\"";
            });

            return json;
        }

        private static string MaskNik(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length <= 4) return "****";
            return new string('*', value.Length - 4) + value.Substring(value.Length - 4);
        }

        private static string MaskEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            var at = value.IndexOf('@');
            if (at <= 0) return "***";
            return value.Substring(0, 1) + "***" + value.Substring(at);
        }
    }
}
