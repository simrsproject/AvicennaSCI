using System;
using System.Web.UI;
using Temiang.Avicenna.BusinessObject.Common;

namespace Temiang.Avicenna.Module.Kiosk
{
    public partial class BedMonitoringDisplayMjkn : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.Theme = "";
        }

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetBrandName()
        {
            return "RS. IMMANUEL";
        }
    }
}
