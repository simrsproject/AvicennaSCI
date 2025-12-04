using System;
using System.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using System.Data;
using Telerik.Web.UI;

namespace Temiang.Avicenna.Module.Kiosk
{
    public partial class QueueEntry : Page //BasePageBootstrap
    {
        protected bool IsDirectButtonBetweenKioskVersion { get; set; }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.Theme = "";
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            IsDirectButtonBetweenKioskVersion = AppSession.Parameter.IsDirectButtonBetweenKioskVersion;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}