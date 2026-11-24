using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Hosting;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaConfig
    {
        public const string DefaultBaseUrl = "https://sb-api.tilaka.id";

        public string BaseUrl { get; set; }
        public string AuthPath { get; set; }
        public string ChannelId { get; set; }
        public string ChannelSecret { get; set; }
        public string ChannelType { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool IsLogEnabled { get; set; }
        public int CallbackToleranceMinutes { get; set; }
        public int RegistrationExpireDays { get; set; }
        public string ConsentTextFile { get; set; }
        public string ConsentVersion { get; set; }
        public string UploadKtpPath { get; set; }
        public int MaxKtpSizeKb { get; set; }
        public string WebviewRedirectUrl { get; set; }
        public string ClientCertificatePath { get; set; }
        public string ClientCertificatePassword { get; set; }

        public string LiteBaseUrl { get; set; }
        public string LiteAuthPath { get; set; }
        public int LiteTimeoutSeconds { get; set; }
        public bool LiteIsLogEnabled { get; set; }
        public bool QuickSignAllowUnsignedCallback { get; set; }
        public string QuickSignSignedFolder { get; set; }
        public string TemplateSignFolder { get; set; }
        public string QuickSignDefaultTemplateNo { get; set; }

        public bool IsQuickSignConfigured
        {
            get { return !string.IsNullOrWhiteSpace(LiteBaseUrl); }
        }

        public bool IsCompanyChannel
        {
            get { return string.Equals(ChannelType, "Company", StringComparison.OrdinalIgnoreCase); }
        }

        public TilakaConfig()
        {
            BaseUrl = GetString("TilakaBaseUrl", DefaultBaseUrl).TrimEnd('/');
            AuthPath = GetString("TilakaAuthPath", "/auth");
            ChannelId = GetString("TilakaChannelId", string.Empty);
            ChannelSecret = GetString("TilakaChannelSecret", string.Empty);
            ChannelType = GetString("TilakaChannelType", "Personal");
            TimeoutSeconds = GetInt("TilakaTimeoutSeconds", 60);
            IsLogEnabled = GetBool("TilakaIsLogEnabled", true);
            CallbackToleranceMinutes = GetInt("TilakaCallbackToleranceMinutes", 5);
            RegistrationExpireDays = GetInt("TilakaRegistrationExpireDays", 7);
            if (RegistrationExpireDays > 7) RegistrationExpireDays = 7;
            ConsentTextFile = GetString("TilakaConsentTextFile", string.Empty);
            ConsentVersion = GetString("TilakaConsentVersion", string.Empty);
            UploadKtpPath = GetString("TilakaUploadKTPImage", string.Empty);
            MaxKtpSizeKb = GetInt("TilakaMaxKtpSizeKb", 1024);
            WebviewRedirectUrl = GetString("TilakaWebviewRedirectUrl", string.Empty);
            ClientCertificatePath = GetString("TilakaClientCertificatePath", string.Empty);
            ClientCertificatePassword = GetString("TilakaClientCertificatePassword", string.Empty);

            LiteBaseUrl = GetString("TilakaLiteBaseUrl", string.Empty).TrimEnd('/');
            LiteAuthPath = GetString("TilakaLiteAuthPath", "/auth");
            LiteTimeoutSeconds = GetInt("TilakaLiteTimeoutSeconds", 120);
            LiteIsLogEnabled = GetBool("TilakaLiteIsLogEnabled", true);
            QuickSignAllowUnsignedCallback = GetBool("TilakaQuickSignAllowUnsignedCallback", false);
            QuickSignSignedFolder = GetString("TilakaQuickSignSignedFolder", string.Empty);
            TemplateSignFolder = GetString("TilakaUploadTemplateSignFilePath", string.Empty);
            QuickSignDefaultTemplateNo = GetString("TilakaQuickSignDefaultTemplateNo", string.Empty);
        }

        public string ReadConsentText()
        {
            if (string.IsNullOrWhiteSpace(ConsentTextFile)) return null;

            if (IsHttpUrl(ConsentTextFile))
                return TrimConsentText(ReadConsentTextFromUrl(ConsentTextFile));

            var path = ResolveFilePath(ConsentTextFile, HostingEnvironment.MapPath);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
            return TrimConsentText(File.ReadAllText(path, Encoding.UTF8));
        }

        public static string ResolveFilePath(string configured, Func<string, string> mapPath)
        {
            if (string.IsNullOrWhiteSpace(configured)) return null;
            if (configured.StartsWith("~/", StringComparison.Ordinal))
                return mapPath == null ? null : mapPath(configured);

            Uri uri;
            if (Uri.TryCreate(configured, UriKind.Absolute, out uri) && !uri.IsFile)
                return null;

            return Path.IsPathRooted(configured) ? configured : Path.GetFullPath(configured);
        }

        private static bool IsHttpUrl(string value)
        {
            Uri uri;
            return Uri.TryCreate(value, UriKind.Absolute, out uri)
                   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
        }

        private static string ReadConsentTextFromUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Timeout = 10000;
                request.ReadWriteTimeout = 10000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    var encoding = Encoding.UTF8;
                    if (!string.IsNullOrWhiteSpace(response.CharacterSet))
                    {
                        try
                        {
                            encoding = Encoding.GetEncoding(response.CharacterSet);
                        }
                        catch (ArgumentException)
                        {
                            // Gunakan UTF-8 bila charset dari server tidak dikenal.
                        }
                        catch (NotSupportedException)
                        {
                            // Gunakan UTF-8 bila charset dari server tidak didukung.
                        }
                    }

                    using (var reader = new StreamReader(stream, encoding, true))
                        return reader.ReadToEnd();
                }
            }
            catch
            {
                return null;
            }
        }

        private static string TrimConsentText(string value)
        {
            return value == null ? null : value.TrimEnd('\r', '\n');
        }

        private static string GetString(string key, string fallback)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
        }

        private static int GetInt(string key, int fallback)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) && value > 0 ? value : fallback;
        }

        private static bool GetBool(string key, bool fallback)
        {
            bool value;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out value) ? value : fallback;
        }
    }
}
