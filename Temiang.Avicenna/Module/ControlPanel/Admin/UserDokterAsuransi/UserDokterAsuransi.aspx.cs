using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.ControlPanel.Admin.UserDokterAsuransi
{
    public partial class UserDokterAsuransi : BasePage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ProgramID = AppConstant.Program.UserDokterAsuransi;

            if (!IsPostBack)
            {
                LoadParamedic();
                LoadUser();
            }
        }

        private void LoadParamedic()
        {
            var meds = new ParamedicCollection();
            meds.Query.Where(meds.Query.IsActive == true,
                             meds.Query.IsAvailable == true);
            meds.LoadAll();

            cboParamedicID.Items.Clear();
            cboParamedicID.Items.Add(new RadComboBoxItem("", ""));

            foreach (var med in meds)
            {
                cboParamedicID.Items.Add(
                    new RadComboBoxItem(med.ParamedicName, med.ParamedicID));
            }
        }

        private void LoadUser()
        {
            var appUser = new AppUser();

            if (!appUser.LoadByPrimaryKey(AppSession.UserLogin.UserID))
                return;

            txtUserID.Text = appUser.UserID;
            txtUserName.Text = appUser.UserName;

            if (!string.IsNullOrEmpty(appUser.ParamedicID))
            {
                cboParamedicID.SelectedValue = appUser.ParamedicID;
            }
        }

        private void SetEntityValue(AppUser entity)
        {
            entity.ParamedicID = cboParamedicID.SelectedValue;

            entity.LastUpdateByUserID = AppSession.UserLogin.UserID;
            entity.LastUpdateDateTime = DateTime.Now;
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var entity = new AppUser();

            if (!entity.LoadByPrimaryKey(AppSession.UserLogin.UserID))
            {
                ShowInformation("User tidak ditemukan.");
                return;
            }

            SetEntityValue(entity);

            using (var trans = new esTransactionScope())
            {
                entity.Save();
                trans.Complete();
            }

            ShowInformation("Data Paramedic berhasil disimpan.");
        }

        private void ShowInformation(string message)
        {
            lblInformation.Text = message;
            pnlInformation.Visible = true;
        }
    }
}