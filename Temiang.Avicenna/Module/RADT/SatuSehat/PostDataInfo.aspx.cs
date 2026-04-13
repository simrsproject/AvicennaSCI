using System;
using System.Text;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using System.Text.RegularExpressions;

namespace Temiang.Avicenna.Module.RADT
{
    /// <summary>
    /// Layar untuk keperluan perawat melihat status resep yg sudah complete tetapi belum diambil
    /// Dipanggil dari layar EMR List
    /// </summary>
    public partial class PostDataInfo : BasePageDialog
    {
        private string EncounterID => Request.QueryString["eid"];
        private string ResourceType => Request.QueryString["rtype"];
        private string IndexNo => Request.QueryString["idxno"];

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ButtonOk.Visible = false;
            ButtonCancel.Text = "Close";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (string.IsNullOrEmpty(IndexNo))
                {
                    var ssKunj = new SatuSehatKunjungan();
                    if (ssKunj.LoadByPrimaryKey(RegistrationNo))
                    {
                        Title = String.Format("Encounter Post Data: {0}", RegistrationNo);
                        txtContent.Text = ssKunj.KunjunganPostData;
                    }
                }
                else
                {
                    var ssResult = new SatuSehatResult();
                    if (ssResult.LoadByPrimaryKey(new Guid(EncounterID), ResourceType, Convert.ToInt32(IndexNo)))
                    {
                        Title = String.Format("{0} Post Data", ResourceType);
                    }
                }
            }
        }

    }
}