using System;
using System.IO;
using System.Web;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public class TilakaTemplateSignFileHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            long id;
            if (!long.TryParse(context.Request.QueryString["id"], out id))
            {
                NotFound(context);
                return;
            }

            var record = new TilakaTemplateSignFileRepository().FindByID(id);
            if (record == null || string.IsNullOrWhiteSpace(record.FilePath))
            {
                NotFound(context);
                return;
            }

            var config = new TilakaConfig();
            var content = new TilakaTemplateSignFileStore(config.TemplateSignFolder)
                .Load(record.FilePath);
            if (content == null)
            {
                NotFound(context);
                return;
            }

            var fileName = Path.GetFileName(record.FileName ?? "template.pdf");
            fileName = (fileName ?? "template.pdf").Replace("\"", string.Empty);

            context.Response.Clear();
            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition",
                "attachment; filename=\"" + fileName + "\"");
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.BinaryWrite(content);
        }

        private static void NotFound(HttpContext context)
        {
            context.Response.StatusCode = 404;
            context.Response.ContentType = "text/plain";
            context.Response.Write("File template tidak ditemukan.");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
