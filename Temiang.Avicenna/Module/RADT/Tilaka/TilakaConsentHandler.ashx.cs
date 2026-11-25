using System.Text;
using System.Web;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public class TilakaConsentHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";
            context.Response.ContentEncoding = Encoding.UTF8;
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            var config = new TilakaConfig();
            var consentText = config.ReadConsentText();

            context.Response.Write("<!DOCTYPE html>");
            context.Response.Write("<html><head><meta charset=\"utf-8\" />");
            context.Response.Write("<title>Persetujuan Tilaka</title>");
            context.Response.Write("<style>");
            context.Response.Write("body{font-family:'Segoe UI',Arial,sans-serif;color:#1f2d3d;");
            context.Response.Write("background:#f5f6f8;margin:0;padding:20px;padding-bottom:76px;");
            context.Response.Write("box-sizing:border-box;}");
            context.Response.Write("pre{white-space:pre-wrap;word-wrap:break-word;font-family:inherit;");
            context.Response.Write("font-size:14px;line-height:1.6;background:#fff;padding:20px;");
            context.Response.Write("border:1px solid #d8dee6;border-radius:8px;margin:0;}");
            context.Response.Write(".warn{background:#fdf1e7;border-left:4px solid #a0522d;");
            context.Response.Write("padding:12px 16px;border-radius:6px;}");
            context.Response.Write(".actions{position:fixed;left:0;right:0;bottom:0;");
            context.Response.Write("padding:12px 20px;text-align:right;background:#f5f6f8;");
            context.Response.Write("border-top:1px solid #d8dee6;}");
            context.Response.Write(".actions button{padding:7px 18px;border:1px solid #0b5cab;");
            context.Response.Write("border-radius:4px;background:#0b5cab;color:#fff;cursor:pointer;}");
            context.Response.Write("</style></head><body>");

            if (string.IsNullOrWhiteSpace(consentText))
            {
                context.Response.Write("<div class=\"warn\">Isi persetujuan Tilaka belum tersedia. ");
                context.Response.Write("Hubungi administrator untuk memeriksa konfigurasi TnC.</div>");
            }
            else
            {
                context.Response.Write("<pre>");
                context.Response.Write(HttpUtility.HtmlEncode(consentText));
                context.Response.Write("</pre>");
            }

            context.Response.Write("<div class=\"actions\">");
            context.Response.Write("<button type=\"button\" onclick=\"closeConsent()\">Close</button>");
            context.Response.Write("</div>");
            context.Response.Write("<script>");
            context.Response.Write("function closeConsent(){");
            context.Response.Write("var oWindow=window.radWindow||");
            context.Response.Write("(window.frameElement&&window.frameElement.radWindow);");
            context.Response.Write("if(oWindow){oWindow.close();return;}window.close();}");
            context.Response.Write("</script>");
            context.Response.Write("</body></html>");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
