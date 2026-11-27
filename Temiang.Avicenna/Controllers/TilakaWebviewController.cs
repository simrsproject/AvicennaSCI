using System;
using System.Web.Mvc;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Controllers
{
    /// <summary>
    /// Halaman pendaratan setelah user menyelesaikan webview registrasi/aktivasi Tilaka.
    /// Tidak memerlukan login; identitas hanya diambil dari baris registrasi yang tersimpan.
    /// </summary>
    public class TilakaWebviewController : Controller
    {
        [HttpGet]
        public ActionResult Return()
        {
            var mode = Request.QueryString["mode"];
            if (string.Equals(mode, "mfa", StringComparison.OrdinalIgnoreCase))
            {
                return View("Return", new TilakaWebviewReturnResult
                {
                    IsSuccess = true,
                    Message = "Jika proses di Tilaka selesai, MFA sudah diperbarui."
                });
            }

            var requestId = TilakaWebviewRequest.ResolveRequestId(
                Request.QueryString["request_id"],
                Request.QueryString["request-id"],
                Request.QueryString["register_id"]);

            var service = new Service();
            var repository = new TilakaRegistrationRepository();

            if (string.Equals(Request.QueryString["mode"], "revoke",
                    StringComparison.OrdinalIgnoreCase)
                || !string.IsNullOrWhiteSpace(Request.QueryString["revoke_id"]))
            {
                var revokeResult = new TilakaRevokeReturnHandler(repository, service).Process(
                    requestId,
                    Request.QueryString["revoke_id"],
                    Request.QueryString["status"]);

                return View("Return", revokeResult);
            }

            if (string.IsNullOrWhiteSpace(requestId))
            {
                var issueId = Request.QueryString["issue_id"];
                if (!string.IsNullOrWhiteSpace(issueId))
                {
                    return View(new TilakaReEnrollReturnHandler(repository, service)
                        .Process(issueId));
                }
            }

            var handler = new TilakaWebviewReturnHandler(repository, service);
            return View(handler.Process(requestId));
        }
    }
}
