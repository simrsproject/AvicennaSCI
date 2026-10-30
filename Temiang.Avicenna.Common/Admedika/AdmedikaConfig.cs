using System.Configuration;

namespace Temiang.Avicenna.Common.Admedika
{
    public class AdmedikaConfig
    {
        public const string DefaultServiceUrl =
            "https://adcpslite.admedika.co.id:553/admedgateway/services/customerhost_dev";

        public const string DefaultApiServiceUrl =
            "https://adcpslite.admedika.co.id:553/admedgateway/services/api/customerhost_dev";

        public string ServiceUrl { get; set; }
        public string ApiServiceUrl { get; set; }
        public string CustomerID { get; set; }
        public string SecurityWord { get; set; }
        public string TerminalID { get; set; }
        public int TimeoutSeconds { get; set; }
        public bool IsLogEnabled { get; set; }

        public AdmedikaConfig()
        {
            ServiceUrl = GetString("AdmedikaServiceUrlLocation", DefaultServiceUrl);
            ApiServiceUrl = GetString("AdmedikaApiServiceUrlLocation", DefaultApiServiceUrl);
            CustomerID = GetString("AdmedikaCustomerID", string.Empty);
            SecurityWord = GetString("AdmedikaSecurityWord", string.Empty);
            TerminalID = GetString("AdmedikaTerminalID", string.Empty);
            TimeoutSeconds = GetInt("AdmedikaTimeoutSeconds", 60);
            IsLogEnabled = GetBool("AdmedikaIsLogEnabled", true);
        }

        private static string GetString(string key, string fallback)
        {
            var value = ConfigurationManager.AppSettings[key];
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static int GetInt(string key, int fallback)
        {
            int value;
            return int.TryParse(ConfigurationManager.AppSettings[key], out value) && value > 0
                ? value
                : fallback;
        }

        private static bool GetBool(string key, bool fallback)
        {
            bool value;
            return bool.TryParse(ConfigurationManager.AppSettings[key], out value) ? value : fallback;
        }
    }
}
