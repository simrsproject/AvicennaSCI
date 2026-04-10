using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Module.RADT.Master;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.RADT.Master
{
    public partial class ImmunizationDetail : BasePageDetail
    {
        #region Page Event & Initialize

        protected void Page_Init(object sender, EventArgs e)
        {
            // Url Search & List
            UrlPageSearch = "ImmunizationSearch.aspx";
            UrlPageList = "ImmunizationList.aspx";

            ProgramID = AppConstant.Program.Immunization;

            ToolBarMenuSearch.Visible = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInitializeAjaxManagerSettingsCollection(AjaxSettingsCollection ajax)
        {

        }

        #endregion

        #region Toolbar Menu Event
        protected override void OnMenuEditClick()
        {
        }
        protected override void OnMenuNewClick()
        {
            OnPopulateEntryControl(new Immunization());
        }

        protected override void OnMenuDeleteClick(ValidateArgs args)
        {
            Immunization entity = new Immunization();
            if (entity.LoadByPrimaryKey(txtImmunizationID.Text))
            {
                entity.MarkAsDeleted();
                SaveEntity(entity);
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
            }
        }

        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            var entity = new Immunization();
            SetEntityValue(entity);
            SaveEntity(entity);
        }

        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            var entity = new Immunization();
            if (entity.LoadByPrimaryKey(txtImmunizationID.Text))
            {
                SetEntityValue(entity);
                SaveEntity(entity);
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
            }
        }

        protected override void OnMenuMoveNextClick(ValidateArgs args)
        {
            MoveRecord(true);
        }

        protected override void OnMenuMovePrevClick(ValidateArgs args)
        {
            MoveRecord(false);
        }

        protected override void OnMenuAuditLogClick(AuditLogFilter auditLogFilter)
        {
            auditLogFilter.PrimaryKeyData = string.Format("ImmunizationID='{0}'", txtImmunizationID.Text.Trim());
            auditLogFilter.TableName = "Immunization";
        }

        #endregion

        #region ToolBar Menu Support

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            txtImmunizationID.Enabled = (newVal == AppEnum.DataMode.New);
            RefreshCommandItemImmunizationBridging(newVal);
        }

        protected override void OnPopulateEntryControl(params string[] parameters)
        {
            var entity = new Immunization();
            if (parameters.Length > 0)
            {
                String ImmunizationID = (String)parameters[0];

                if (!parameters[0].Equals(string.Empty))
                    entity.LoadByPrimaryKey(ImmunizationID);
            }
            else
            {
                entity.LoadByPrimaryKey(txtImmunizationID.Text);
            }
            OnPopulateEntryControl(entity);
        }

        protected override void OnPopulateEntryControl(esEntity entity)
        {
            var imm = (Immunization)entity;
            txtImmunizationID.Text = imm.ImmunizationID;
            txtImmunizationName.Text = imm.ImmunizationName;
            txtMaxCount.Value = imm.MaxCount;
            txtIndexNo.Value = imm.IndexNo;

            PopulateItemBridgingGrid();
        }

        #endregion

        #region Private Method Standard

        private void SetEntityValue(Immunization entity)
        {
            entity.ImmunizationID = txtImmunizationID.Text;
            entity.ImmunizationName = txtImmunizationName.Text;
            entity.MaxCount = Convert.ToInt32( txtMaxCount.Value);
            entity.IndexNo = Convert.ToInt32(txtIndexNo.Value);
        }

        private void SaveEntity(Immunization entity)
        {
            using (esTransactionScope trans = new esTransactionScope())
            {
                entity.Save();
                ImmunizationBridgings.Save();

                //Commit if success, Rollback if failed
                trans.Complete();
            }
        }

        private void MoveRecord(bool isNextRecord)
        {
            ImmunizationQuery que = new ImmunizationQuery();
            que.es.Top = 1; // SELECT TOP 1 ..
            if (isNextRecord)
            {
                que.Where(que.ImmunizationID > txtImmunizationID.Text);
                que.OrderBy(que.ImmunizationID.Ascending);
            }
            else
            {
                que.Where(que.ImmunizationID < txtImmunizationID.Text);
                que.OrderBy(que.ImmunizationID.Descending);
            }

            Immunization entity = new Immunization();
            if (entity.Load(que))
                OnPopulateEntryControl(entity);
        }

        #endregion


        #region Record Detail Method Function Bridging

        private ImmunizationBridgingCollection ImmunizationBridgings
        {
            get
            {
                if (IsPostBack)
                {
                    object obj = Session["collImmunizationBridging"];
                    if (obj != null) return ((ImmunizationBridgingCollection)(obj));
                }

                ImmunizationBridgingCollection coll = new ImmunizationBridgingCollection();

                ImmunizationBridgingQuery query = new ImmunizationBridgingQuery("a");
                AppStandardReferenceItemQuery asri = new AppStandardReferenceItemQuery("b");

                query.Select(query, asri.ItemName.As("refToAppStandardReferenceItem_ItemName"));
                query.InnerJoin(asri).On(query.SRBridgingType == asri.ItemID && asri.StandardReferenceID == AppEnum.StandardReference.BridgingType.ToString());
                query.Where(query.ImmunizationID == txtImmunizationID.Text);
                coll.Load(query);

                Session["collImmunizationBridging"] = coll;
                return coll;
            }
            set
            {
                Session["collImmunizationBridging"] = value;
            }
        }

        private void RefreshCommandItemImmunizationBridging(AppEnum.DataMode newVal)
        {
            bool isVisible = (newVal != AppEnum.DataMode.Read);
            grdAliasName.Columns[0].Visible = isVisible;
            grdAliasName.Columns[grdAliasName.Columns.Count - 1].Visible = isVisible;

            grdAliasName.MasterTableView.CommandItemDisplay = isVisible ? GridCommandItemDisplay.Top : GridCommandItemDisplay.None;

            if (newVal == AppEnum.DataMode.Read)
            {
                grdAliasName.MasterTableView.IsItemInserted = false;
                grdAliasName.MasterTableView.ClearEditItems();
            }

            grdAliasName.Rebind();


        }

        private void PopulateItemBridgingGrid()
        {
            // Reset
            ImmunizationBridgings = null;
            grdAliasName.Rebind();
        }

        protected void grdAliasName_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdAliasName.DataSource = ImmunizationBridgings;
        }

        protected void grdAliasName_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;
            if (editedItem == null) return;

            String type = Convert.ToString(editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][ImmunizationBridgingMetadata.ColumnNames.SRBridgingType]);
            String id = Convert.ToString(editedItem.OwnerTableView.DataKeyValues[editedItem.ItemIndex][ImmunizationBridgingMetadata.ColumnNames.BridgingID]);

            var entity = FindImmunizationBridging(type, id);
            if (entity != null) SetEntityValue(entity, e);
        }

        protected void grdAliasName_DeleteCommand(object source, GridCommandEventArgs e)
        {
            GridDataItem item = e.Item as GridDataItem;
            if (item == null) return;

            String type = Convert.ToString(item.OwnerTableView.DataKeyValues[item.ItemIndex][ImmunizationBridgingMetadata.ColumnNames.SRBridgingType]);
            String id = Convert.ToString(item.OwnerTableView.DataKeyValues[item.ItemIndex][ImmunizationBridgingMetadata.ColumnNames.BridgingID]);

            var entity = FindImmunizationBridging(type, id);
            if (entity != null) entity.MarkAsDeleted();
        }

        protected void grdAliasName_InsertCommand(object source, GridCommandEventArgs e)
        {
            var entity = ImmunizationBridgings.AddNew();
            SetEntityValue(entity, e);

            e.Canceled = true;
            grdAliasName.Rebind();
        }

        private ImmunizationBridging FindImmunizationBridging(String type, string id)
        {
            var coll = ImmunizationBridgings;
            return coll.FirstOrDefault(rec => rec.SRBridgingType.Equals(type) && rec.BridgingID.Equals(id));
        }

        private void SetEntityValue(ImmunizationBridging entity, GridCommandEventArgs e)
        {
            ItemAliasDetail userControl = (ItemAliasDetail)e.Item.FindControl(GridEditFormItem.EditFormUserControlID);
            if (userControl != null)
            {
                entity.ImmunizationID = txtImmunizationID.Text;
                entity.SRBridgingType = userControl.BridgingType;
                entity.BridgingID = userControl.BridgingID;
                entity.BridgingName = string.IsNullOrEmpty(userControl.BridgingName) ? txtImmunizationName.Text : userControl.BridgingName;
                entity.IsActive = userControl.IsActive;
            }
        }

        #endregion

    }
}
