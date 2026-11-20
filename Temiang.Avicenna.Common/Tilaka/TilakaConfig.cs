using System;
using System.Configuration;
using System.IO;
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
        public string CallbackBaseUrl { get; set; }
        public int CallbackToleranceMinutes { get; set; }
        public int RegistrationExpireDays { get; set; }
        public string ConsentTextFile { get; set; }
        public string ConsentVersion { get; set; }
        public string UploadKtpPath { get; set; }
        public int MaxKtpSizeKb { get; set; }
        public string WebviewRedirectUrl { get; set; }
        public string ClientCertificatePath { get; set; }
        public string ClientCertificatePassword { get; set; }

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
            CallbackBaseUrl = GetString("TilakaCallbackBaseUrl", string.Empty).TrimEnd('/');
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
        }

        public string ReadConsentText()
        {
            var path = ResolveFilePath(ConsentTextFile, HostingEnvironment.MapPath);
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
            return File.ReadAllText(path, Encoding.UTF8).TrimEnd('\r', '\n');
        }

        public static string ResolveFilePath(string configured, Func<string, string> mapPath)
        {
            if (string.IsNullOrWhiteSpace(configured)) return null;
            if (configured.StartsWith("~/", StringComparison.Ordinal))
                return mapPath == null ? null : mapPath(configured);
            return Path.IsPathRooted(configured) ? configured : Path.GetFullPath(configured);
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
