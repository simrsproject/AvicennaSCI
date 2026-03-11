using System;
using System.Data;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Telerik.Web.UI;

namespace Temiang.Avicenna.Module.ControlPanel.Admin.UserDokterAsuransi
{
    public partial class UserDokterAsuransiDetail : BasePageDetail
    {
        private void SetEntityValue(esAppUser entity)
        {
            entity.UserID = txtUserID.Text;
            entity.UserName = txtUserName.Text;

            // Last Update Status
            if (entity.es.IsAdded || entity.es.IsModified)
            {
                entity.LastUpdateByUserID = AppSession.UserLogin.UserID;
                entity.LastUpdateDateTime = DateTime.Now;
            }

            entity.ParamedicID = cboParamedicID.SelectedValue;
        }

        //private void MoveRecord(bool isNextRecord)
        //{
           // AppUserQuery que = new AppUserQuery();
           // que.es.Top = 1; // SELECT TOP 1 ..
           // if (isNextRecord)
           // {
           //     que.Where(que.UserID > txtUserID.Text);
               // que.OrderBy(que.UserID.Ascending);
           // }
           // else
           // {
             //   que.Where(que.UserID < txtUserID.Text);
             //   que.OrderBy(que.UserID.Descending);
           // }
           // AppUser entity = new AppUser();
           // entity.Load(que);
           // OnPopulateEntryControl(entity);
        //}

        #region Override Method & Function

        protected override void OnPopulateEntryControl(params string[] parameters)
        {
            AppUser entity = new AppUser();
            if (parameters.Length > 0)
            {
                String userID = parameters[0];
                if (!parameters[0].Equals(string.Empty))
                    entity.LoadByPrimaryKey(userID);
            }
            else
                entity.LoadByPrimaryKey(txtUserID.Text);

            OnPopulateEntryControl(entity);
        }

        protected override void OnPopulateEntryControl(esEntity entity)
        {
            var appUser = (AppUser)entity;
            txtUserID.Text = appUser.UserID;
            txtUserName.Text = appUser.UserName;

            if (!string.IsNullOrEmpty(appUser.ParamedicID))
                cboParamedicID.SelectedValue = appUser.ParamedicID;
            else
            {
                cboParamedicID.SelectedValue = string.Empty;
                cboParamedicID.Text = string.Empty;
            }
        }

        protected override void OnInitializeAjaxManagerSettingsCollection(AjaxSettingsCollection ajax)
        {
            // Tidak ada upload signature lagi, hapus
        }

        protected override void OnMenuMoveNextClick(ValidateArgs args)
        {
            //MoveRecord(true);
        }

        protected override void OnMenuMovePrevClick(ValidateArgs args)
        {
            //MoveRecord(false);
        }

        protected override void OnMenuAuditLogClick(AuditLogFilter auditLogFilter)
        {
            auditLogFilter.PrimaryKeyData = "UserID='" + txtUserID.Text.Trim() + "'";
            auditLogFilter.TableName = "AppUser";
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            txtUserID.Enabled = (newVal == AppEnum.DataMode.New);

            if (newVal == AppEnum.DataMode.New)
                OnPopulateEntryControl(new AppUser());

            // Refresh combobox
            cboParamedicID.Enabled = (newVal != AppEnum.DataMode.Read);

            // Sembunyikan tombol toolbar terakhir kali
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuDelete.Visible = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Sembunyikan tombol toolbar terakhir kali
            ToolBarMenuSearch.Visible = false;
            ToolBarMenuDelete.Visible = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UrlPageList = "UserDokterAsuransiList.aspx";
            ProgramID = AppConstant.Program.UserDokterAsuransi;

            if (!IsPostBack)
            {
                var meds = new ParamedicCollection();
                meds.Query.Where(meds.Query.IsActive == true, meds.Query.IsAvailable == true);
                meds.LoadAll();

                cboParamedicID.Items.Add(new Telerik.Web.UI.RadComboBoxItem(string.Empty, string.Empty));
                foreach (var med in meds)
                {
                    cboParamedicID.Items.Add(new Telerik.Web.UI.RadComboBoxItem(med.ParamedicName, med.ParamedicID));
                }
            }
        }

        protected override void OnMenuDeleteClick(ValidateArgs args)
        {
            if (txtUserID.Text == "sci" && AppSession.UserLogin.UserID != "sci")
            {
                args.MessageText = "You don't have authorization to delete this data";
                args.IsCancel = true;
                return;
            }

            AppUser entity = new AppUser();
            entity.LoadByPrimaryKey(txtUserID.Text);
            entity.MarkAsDeleted();
            entity.Save();
        }

        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            AppUser entity = new AppUser();
            if (entity.LoadByPrimaryKey(txtUserID.Text))
            {
                args.MessageText = AppConstant.Message.DuplicateKey;
                args.IsCancel = true;
                return;
            }

            entity.AddNew();
            SetEntityValue(entity);
            entity.Save();
        }

        protected override void OnBeforeMenuEditClick(ValidateArgs args)
        {
            if (txtUserID.Text == "sci" && AppSession.UserLogin.UserID != "sci")
            {
                args.MessageText = "You don't have authorization to edit this data";
                args.IsCancel = true;
                return;
            }
        }

        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            AppUser entity = new AppUser();
            if (entity.LoadByPrimaryKey(txtUserID.Text))
            {
                SetEntityValue(entity);
                entity.Save();
            }
        }

        #endregion
    }
}