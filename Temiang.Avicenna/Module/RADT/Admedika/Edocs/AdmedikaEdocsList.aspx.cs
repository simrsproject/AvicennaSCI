using System;
using System.Data;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaEdocsList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaEdocs;
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            // Data Edocs dibuat dari halaman detail (upload), bukan dari list.
            ToolBarMenuAdd.Visible = false;

            // Filter pencarian tersedia inline, tidak memakai halaman search terpisah.
            ToolBarMenuSearch.Visible = false;
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string id = dataItem.GetDataKeyValue(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID).ToString();
            Page.Response.Redirect("AdmedikaEdocsDetail.aspx?md=" + mode + "&id=" + id, true);
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = EdocsTable;
        }

        protected void grdList_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            var parentItem = e.DetailTableView.ParentItem;
            if (parentItem == null)
            {
                e.DetailTableView.DataSource = null;
                return;
            }

            long transactionID;
            if (!long.TryParse(Convert.ToString(parentItem.GetDataKeyValue(
                    AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID)), out transactionID))
            {
                e.DetailTableView.DataSource = null;
                return;
            }

            var query = new AdmedikaDocumentQuery("a");
            query.es.WithNoLock = true;

            query.Select(query);
            query.Where(query.AdmedikaClaimTransactionID == transactionID);
            query.OrderBy(query.CreatedDateTime.Descending, query.AdmedikaDocumentID.Descending);

            e.DetailTableView.DataSource = query.LoadDataTable();
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        protected string BuildDocTypeHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var code = Convert.ToString(row[AdmedikaDocumentMetadata.ColumnNames.DocType]);
            var docType = AdmedikaDocType.Get(code);

            var html = "<b>" + Server.HtmlEncode(code) + "</b>";

            if (docType != null && !string.IsNullOrWhiteSpace(docType.Description))
            {
                if (html.Length > 0) html += "<br />";
                html += Server.HtmlEncode(docType.Description);
            }

            return html;
        }

        private DataTable EdocsTable
        {
            get
            {
                var query = new AdmedikaClaimTransactionQuery("a");
                var registration = new RegistrationQuery("b");

                query.es.WithNoLock = true;
                registration.es.WithNoLock = true;

                // Inner join otomatis membatasi data ke klaim yang sudah memiliki registrasi.
                query.InnerJoin(registration).On(registration.RegistrationNo == query.RegistrationNo);

                query.Select(
                    query.AdmedikaClaimTransactionID,
                    query.RegistrationNo,
                    registration.RegistrationDate,
                    query.CardNo,
                    query.ClID,
                    query.PatientName,
                    "<ISNULL((SELECT COUNT(*) FROM AdmedikaDocument d " +
                    "WHERE d.AdmedikaClaimTransactionID = a.AdmedikaClaimTransactionID), 0) AS JumlahFile>"
                );

                query.Where(query.RegistrationNo.IsNotNull(), query.RegistrationNo != string.Empty);

                if (!txtRegistrationDate.IsEmpty)
                    query.Where(registration.RegistrationDate >= txtRegistrationDate.SelectedDate.Value.Date);

                if (!txtRegistrationDateTo.IsEmpty)
                    query.Where(registration.RegistrationDate < txtRegistrationDateTo.SelectedDate.Value.Date.AddDays(1));

                if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                    query.Where(query.RegistrationNo == txtRegistrationNo.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtClID.Text))
                    query.Where(query.ClID.Like("%" + Helper.EscapeQuery(txtClID.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtCardNo.Text))
                    query.Where(query.CardNo.Like("%" + Helper.EscapeQuery(txtCardNo.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                    query.Where(query.PatientName.Like("%" + Helper.EscapeQuery(txtPatientName.Text) + "%"));

                query.OrderBy(registration.RegistrationDate.Descending, query.RegistrationNo.Descending);
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                return query.LoadDataTable();
            }
        }
    }
}
