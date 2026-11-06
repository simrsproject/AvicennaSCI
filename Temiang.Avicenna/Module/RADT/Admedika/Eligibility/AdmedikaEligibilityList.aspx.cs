using System;
using System.Data;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEligibilityList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            UrlPageDetail = "AdmedikaEligibilityDetail.aspx";
            ProgramID = AppConstant.Program.AdmedikaEligibility;

            if (IsPostBack) return;

            txtCreatedDate.SelectedDate = DateTime.Now.Date;
            txtCreatedDateTo.SelectedDate = DateTime.Now.Date;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            // Halaman ini hanya membaca data. Eligibility baru dibuat lewat dialog.
            ToolBarMenuAdd.Visible = false;
            ToolBarMenuEdit.Visible = false;

            // Filter pencarian sudah tersedia inline, tidak memakai halaman search terpisah.
            ToolBarMenuSearch.Visible = false;

            btnNewEligibility.Visible = IsUserAddAble;
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string id = dataItem.GetDataKeyValue(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID).ToString();
            Page.Response.Redirect("AdmedikaEligibilityDetail.aspx?md=" + mode + "&id=" + id, true);
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = EligibilityTable;
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        private DataTable EligibilityTable
        {
            get
            {
                var query = new AdmedikaClaimTransactionQuery("a");

                query.es.WithNoLock = true;

                query.Select(
                    query.AdmedikaClaimTransactionID,
                    query.CreatedDateTime,
                    query.RegistrationNo,
                    query.PatientName,
                    query.CardNo,
                    query.ClID,
                    query.ClStatus,
                    query.ClDesc,
                    query.IsClosed
                );

                if (!txtCreatedDate.IsEmpty)
                    query.Where(query.CreatedDateTime >= txtCreatedDate.SelectedDate.Value.Date);

                if (!txtCreatedDateTo.IsEmpty)
                    query.Where(query.CreatedDateTime < txtCreatedDateTo.SelectedDate.Value.Date.AddDays(1));

                if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                    query.Where(query.RegistrationNo == txtRegistrationNo.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                    query.Where(query.PatientName.Like("%" + Helper.EscapeQuery(txtPatientName.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtCardNo.Text))
                    query.Where(query.CardNo.Like("%" + Helper.EscapeQuery(txtCardNo.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtClID.Text))
                    query.Where(query.ClID.Like("%" + Helper.EscapeQuery(txtClID.Text) + "%"));

                query.OrderBy(query.CreatedDateTime.Descending);
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                return query.LoadDataTable();
            }
        }
    }
}
