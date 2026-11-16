using System;
using System.IO;
using System.Web;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public class AdmedikaEdocsFile : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            long documentID;
            if (!long.TryParse(context.Request.QueryString["id"], out documentID) || documentID <= 0)
            {
                context.Response.StatusCode = 400;
                return;
            }

            var document = new AdmedikaDocument();
            if (!document.LoadByPrimaryKey(documentID))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var claim = new AdmedikaClaimTransaction();
            if (document.AdmedikaClaimTransactionID == null ||
                !claim.LoadByPrimaryKey(document.AdmedikaClaimTransactionID.Value))
            {
                context.Response.StatusCode = 404;
                return;
            }

            var filePath = AdmedikaEdocsHelper.GetDocumentFilePath(document, claim);
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                context.Response.StatusCode = 404;
                return;
            }

            context.Response.Clear();
            context.Response.ContentType = AdmedikaEdocsHelper.GetContentType(document.FileName);
            context.Response.AddHeader(
                "Content-Disposition",
                "inline; filename=\"" + Path.GetFileName(document.FileName) + "\"");
            context.Response.AddHeader("Cache-Control", "private, max-age=0");
            context.Response.BinaryWrite(File.ReadAllBytes(filePath));
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
