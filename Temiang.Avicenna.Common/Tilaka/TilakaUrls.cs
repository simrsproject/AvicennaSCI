using System.Collections.Generic;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaUrls
    {
        public static string Auth(TilakaConfig config)
        {
            var baseUrl = string.IsNullOrWhiteSpace(config.AuthBaseUrl)
                ? config.BaseUrl
                : config.AuthBaseUrl;
            return baseUrl + config.AuthPath;
        }

        public static string GenerateUuid(TilakaConfig config, string name, string email)
        {
            var url = config.BaseUrl + "/generateUUID";
            if (!config.IsCompanyChannel) return url;

            var query = new Dictionary<string, string>();
            AddIfAny(query, "name", name);
            AddIfAny(query, "email", email);
            return url + BuildQuery(query);
        }

        public static string GenerateUuidReEnroll(TilakaConfig config, string tilakaName)
        {
            var query = new Dictionary<string, string>
            {
                { "request_type", "re_enroll" },
                { "user_identifier", tilakaName }
            };
            return config.BaseUrl + "/generateUUID" + BuildQuery(query);
        }

        public static string RegisterForKycCheck(TilakaConfig config)
        {
            return config.BaseUrl + "/registerForKycCheck";
        }

        public static string UserRegStatus(TilakaConfig config)
        {
            return config.BaseUrl + "/userregstatus";
        }

        public static string CheckCertStatus(TilakaConfig config)
        {
            return config.BaseUrl + "/checkcertstatus";
        }

        public static string UserReEnrollStatus(TilakaConfig config)
        {
            return config.BaseUrl + "/userreenrollstatus";
        }

        public static string RegistrationWebview(TilakaConfig config, string requestId)
        {
            var query = new Dictionary<string, string>();
            query["request_id"] = requestId;
            AddIfAny(query, "redirect_url", config.WebviewRedirectUrl);
            return config.BaseUrl + "/personal-webview/guide" + BuildQuery(query);
        }

        public static string ActivationWebview(TilakaConfig config, string requestId)
        {
            var query = new Dictionary<string, string>();
            query["request_id"] = requestId;
            query["setting"] = "1";
            AddIfAny(query, "channel_id", config.ChannelId);
            AddIfAny(query, "redirect_url", config.WebviewRedirectUrl);
            return config.BaseUrl + "/personal-webview/link-account" + BuildQuery(query);
        }

        public static string ActivationWebviewByIssueId(TilakaConfig config, string issueId)
        {
            var query = new Dictionary<string, string>
            {
                { "issue_id", issueId },
                { "setting", "1" }
            };
            AddIfAny(query, "channel_id", config.ChannelId);
            AddIfAny(query, "redirect_url", config.WebviewRedirectUrl);
            return config.BaseUrl + "/personal-webview/link-account" + BuildQuery(query);
        }

        public static string ReEnrollWebview(TilakaConfig config, string issueId)
        {
            var query = new Dictionary<string, string>();
            query["issue_id"] = issueId;
            AddIfAny(query, "redirect_url", config.WebviewRedirectUrl);
            return config.BaseUrl + "/personal-webview/kyc/re-enroll" + BuildQuery(query);
        }

        private static void AddIfAny(IDictionary<string, string> query, string key, string value)
        {
            if (!string.IsNullOrWhiteSpace(value)) query[key] = value;
        }

        private static string BuildQuery(IDictionary<string, string> query)
        {
            if (query == null || query.Count == 0) return string.Empty;

            var builder = new StringBuilder();
            foreach (var item in query)
            {
                builder.Append(builder.Length == 0 ? '?' : '&');
                builder.Append(item.Key);
                builder.Append('=');
                builder.Append(System.Uri.EscapeDataString(item.Value ?? string.Empty));
            }

            return builder.ToString();
        }
    }
}
