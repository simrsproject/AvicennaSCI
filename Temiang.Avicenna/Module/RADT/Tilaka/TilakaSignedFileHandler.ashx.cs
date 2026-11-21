using System;
using System.IO;
using System.Web;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public class TilakaSignedFileHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

            var access = new UserAccess(AppSession.UserLogin.UserID, AppConstant.Program.TilakaSigning);
            if (!access.IsExist)
            {
                context.Response.StatusCode = 403;
                return;
            }

            var requestId = context.Request.QueryString["id"];
            if (string.IsNullOrWhiteSpace(requestId))
            {
                context.Response.StatusCode = 400;
                return;
            }

            var record = new TilakaQuickSignRepository().FindByRequestId(requestId);
            if (record == null || string.IsNullOrWhiteSpace(record.SignedFilePath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var config = new TilakaConfig();
            var root = TilakaConfig.ResolveFilePath(config.QuickSignSignedFolder, context.Server.MapPath);
            if (string.IsNullOrWhiteSpace(root))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var fullPath = Path.Combine(root, record.SignedFilePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.ContentType = "application/pdf";
            context.Response.AddHeader("Content-Disposition",
                "inline; filename=\"" + Sanitize(record.FileName) + "\"");
            context.Response.AddHeader("X-Content-Type-Options", "nosniff");
            context.Response.BinaryWrite(File.ReadAllBytes(fullPath));
        }

        private static string Sanitize(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "signed.pdf";
            var builder = new System.Text.StringBuilder(value.Length);
            foreach (var ch in value)
                builder.Append(ch < 32 || ch == 127 || ch == '"' ? '_' : ch);
            return builder.ToString();
        }
    }
}
