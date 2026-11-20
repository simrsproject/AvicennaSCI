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
            var handler = new TilakaWebviewReturnHandler(new TilakaRegistrationRepository(), new Service());
            return View(handler.Process(Request.QueryString["request-id"]));
        }
    }
}
