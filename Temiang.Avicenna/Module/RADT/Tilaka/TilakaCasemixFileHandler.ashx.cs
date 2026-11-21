using System;
using System.IO;
using System.Web;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public class TilakaCasemixFileHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(AppSession.UserLogin.UserID))
            {
                context.Response.StatusCode = 401;
                return;
            }

            var access = new UserAccess(AppSession.UserLogin.UserID, AppConstant.Program.TilakaCasemixFile);
            if (!access.IsExist)
            {
                context.Response.StatusCode = 403;
                return;
            }

            long id;
            if (!long.TryParse(context.Request.QueryString["id"], out id))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var record = new TilakaCasemixFileRepository().FindByID(id);
            if (record == null || record.IsVoid)
            {
                context.Response.StatusCode = 404;
                return;
            }

            try
            {
                var root = AppSession.Parameter.ApplicationDocumentFolder;
                var reader = new TilakaCasemixFileReader(root);
                var bytes = reader.Load(record.FilePath);

                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("Content-Disposition",
                    "inline; filename=\"" + Sanitize(record.FileName) + "\"");
                context.Response.AddHeader("X-Content-Type-Options", "nosniff");
                context.Response.BinaryWrite(bytes);
            }
            catch (InvalidOperationException)
            {
                context.Response.StatusCode = 404;
            }
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "casemix.pdf";
            var builder = new System.Text.StringBuilder(value.Length);
            foreach (var ch in value)
                builder.Append(ch < 32 || ch == 127 || ch == '"' ? '_' : ch);
            return builder.ToString();
        }
    }
}
