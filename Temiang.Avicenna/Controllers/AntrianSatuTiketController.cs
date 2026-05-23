using System.Web.Mvc;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Controllers
{
    public class AntrianSatuTiketController : Controller
    {

        public bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(AppSession.UserLogin.UserID);
        }

        public ActionResult Index()
        {
            if(!IsLoggedIn())
            {
                RedirectToAction("Login");
            }

            string redirectUrl = Request.QueryString["redirectUrl"];
            redirectUrl = redirectUrl.Replace("{uid}", AppSession.UserLogin.UserID);

            return Redirect(redirectUrl);
        }   
    }
}