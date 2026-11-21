using System.Globalization;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaQuickSignUrls
    {
        public static string Upload(TilakaConfig config)
        {
            return Base(config) + "/api/v1/upload";
        }

        public static string RequestSign(TilakaConfig config)
        {
            return Base(config) + "/api/v1/requestquicksign";
        }

        public static string CheckSignStatus(TilakaConfig config)
        {
            return Base(config) + "/api/v1/checksignstatus";
        }

        public static string GetPresignedUrl(TilakaConfig config, string fileName)
        {
            return Base(config) + "/getpresignurl?filename=" + System.Uri.EscapeDataString(fileName ?? string.Empty);
        }

        public static string AddUsers(TilakaConfig config)
        {
            return Base(config) + "/addquicksignusers";
        }

        public static string AddTemplates(TilakaConfig config)
        {
            return Base(config) + "/quicksign-addtemplates";
        }

        public static string AuthSessions(TilakaConfig config, string email, string authStatus,
            int pageNumber, int limit)
        {
            var builder = new StringBuilder(Base(config) + "/quicksign-authsessions?page_number=" +
                                           pageNumber.ToString(CultureInfo.InvariantCulture) +
                                           "&limit=" + limit.ToString(CultureInfo.InvariantCulture));
            if (!string.IsNullOrWhiteSpace(email))
                builder.Append("&email=" + System.Uri.EscapeDataString(email));
            if (!string.IsNullOrWhiteSpace(authStatus))
                builder.Append("&auth_status=" + System.Uri.EscapeDataString(authStatus));
            return builder.ToString();
        }

        public static string Base(TilakaConfig config)
        {
            return (config.LiteBaseUrl ?? string.Empty).TrimEnd('/');
        }
    }
}
