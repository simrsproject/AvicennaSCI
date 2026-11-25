using System.Web;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaAlertHtml
    {
        public static string Success(string message)
        {
            return Build("#dff0d8", "#3c763d", "#3c763d", message);
        }

        public static string Error(string message)
        {
            return Build("#f2dede", "#d9534f", "#a94442", message);
        }

        public static string Warning(string message)
        {
            return Build("#fcf8e3", "#faebcc", "#8a6d3b", message);
        }

        public static string Info(string message)
        {
            return Build("#d9edf7", "#bce8f1", "#31708f", message);
        }

        private static string Build(string background, string border, string color, string message)
        {
            return string.Concat(
                "<div style=\"display:inline-block;max-width:100%;box-sizing:border-box;",
                "vertical-align:top;overflow-wrap:break-word;",
                "border:1px solid ", border,
                ";background:", background,
                ";color:", color,
                ";padding:8px;margin-bottom:8px;\">",
                HttpUtility.HtmlEncode(message ?? string.Empty),
                "</div>");
        }
    }
}
