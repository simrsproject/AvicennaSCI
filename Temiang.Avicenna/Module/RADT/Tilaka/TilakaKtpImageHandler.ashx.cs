using System;
using System.IO;
using System.Text;
using System.Web;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Module.RADT.Tilaka
{
    public class TilakaKtpImageHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
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

            var access = new UserAccess(AppSession.UserLogin.UserID, AppConstant.Program.TilakaRegistration);
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

            var query = new TilakaRegistrationQuery();
            query.Where(query.TilakaRegistrationID == id);
            var entity = new TilakaRegistration();
            if (!entity.Load(query) || string.IsNullOrWhiteSpace(entity.KtpFilePath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            byte[] bytes;
            try
            {
                var config = new TilakaConfig();
                var store = new TilakaKtpFileStore(config.UploadKtpPath, config.MaxKtpSizeKb);
                bytes = store.Load(entity.KtpFilePath);
            }
            catch (InvalidOperationException)
            {
                // Path di luar folder upload atau konfigurasi folder tidak valid.
                context.Response.StatusCode = 404;
                return;
            }
            catch (IOException)
            {
                context.Response.StatusCode = 404;
                return;
            }
            catch (UnauthorizedAccessException)
            {
                context.Response.StatusCode = 404;
                return;
            }

            if (bytes == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();
            context.Response.ContentType = "image/jpeg";
            context.Response.AddHeader("Content-Disposition",
                "inline; filename=\"" + SanitizeFileName(entity.KtpFileName) + "\"");
            context.Response.BinaryWrite(bytes);
        }

        private static string SanitizeFileName(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "ktp.jpg";

            var builder = new StringBuilder(value.Length);
            foreach (var ch in value)
            {
                if (ch == '"' || char.IsControl(ch)) continue;
                builder.Append(ch);
            }

            var result = builder.ToString().Trim();
            return string.IsNullOrEmpty(result) ? "ktp.jpg" : result;
        }
    }
}
