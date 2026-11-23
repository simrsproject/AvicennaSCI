using System;
using System.Data;
using System.Globalization;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaDailyMonitoringList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaDailyMonitoring;
            UrlPageDetail = "AdmedikaDailyMonitoringDetail.aspx";
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            // Entri ditambah dari halaman detail, bukan dari toolbar list.
            ToolBarMenuAdd.Visible = false;
            ToolBarMenuSearch.Visible = false;
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToDetail(dataItems[0]);
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToDetail(dataItems[0]);
        }

        private void RedirectToDetail(GridDataItem dataItem)
        {
            var id = dataItem.GetDataKeyValue(
                AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID);

            Page.Response.Redirect("AdmedikaDailyMonitoringDetail.aspx?id=" + id, true);
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            grdList.Rebind();
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = MonitoringTable;
        }

        protected void grdList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item == null) return;

            var row = item.DataItem as DataRowView;
            if (row == null) return;

            if (row["LastMonitoringDate"] == DBNull.Value)
                item["LastMonitoringDate"].BackColor = System.Drawing.Color.FromArgb(255, 249, 196);
        }

        protected string BuildRegistrationDateHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var dateValue = row["RegistrationDate"];
            if (dateValue == DBNull.Value) return string.Empty;

            var result = Convert.ToDateTime(dateValue)
                .ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

            var time = Convert.ToString(row["RegistrationTime"]);
            if (!string.IsNullOrWhiteSpace(time))
                result += " " + time;

            return result;
        }

        protected string BuildLosHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var start = row["RegistrationDate"];
            if (start == DBNull.Value) return "-";

            var end = row["DischargeDate"] == DBNull.Value
                ? DateTime.Today
                : Convert.ToDateTime(row["DischargeDate"]).Date;

            var days = (end.Date - Convert.ToDateTime(start).Date).Days;
            if (days < 0) days = 0;

            return days + " hari";
        }

        protected string BuildPatientStatusHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            return row["DischargeDate"] == DBNull.Value ? "Proses Rawat" : "Pulang";
        }

        protected string BuildLastMonitoringHtml(object dataItem)
        {
            var row = dataItem as DataRowView;
            if (row == null) return string.Empty;

            var value = row["LastMonitoringDate"];
            if (value == DBNull.Value) return "-";

            return Convert.ToDateTime(value).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private DataTable MonitoringTable
        {
            get
            {
                var query = new AdmedikaClaimTransactionQuery("a");
                var registration = new RegistrationQuery("b");
                var room = new ServiceRoomQuery("c");

                query.es.WithNoLock = true;
                registration.es.WithNoLock = true;
                room.es.WithNoLock = true;

                query.InnerJoin(registration).On(registration.RegistrationNo == query.RegistrationNo);
                query.LeftJoin(room).On(room.RoomID == registration.RoomID);

                query.Select(
                    query.AdmedikaClaimTransactionID,
                    query.RegistrationNo,
                    registration.RegistrationDate,
                    registration.RegistrationTime,
                    query.CardNo,
                    query.ClID,
                    query.PatientName,
                    room.RoomName,
                    registration.DischargeDate,
                    query.ClStatus,
                    query.ClDesc,
                    query.IsClosed,
                    "<(SELECT MAX(m.MonitoringDate) FROM AdmedikaDailyMonitoring m " +
                    "WHERE m.AdmedikaClaimTransactionID = a.AdmedikaClaimTransactionID) AS LastMonitoringDate>",
                    "<(SELECT TOP 1 ISNULL(NULLIF(x.DiagnosisText, ''), y.DiagnoseName) " +
                    "FROM RegistrationInfoMedicDiagnose x " +
                    "LEFT JOIN Diagnose y ON y.DiagnoseID = x.DiagnoseID " +
                    "WHERE x.RegistrationNo = b.RegistrationNo AND ISNULL(x.IsVoid, 0) = 0 " +
                    "ORDER BY x.DiagnoseDateTime DESC) AS LastDiagnosis>"
                );

                // Daily monitoring hanya untuk klaim rawat inap (covID 01/HNS).
                query.Where(
                    query.CovID == "01",
                    query.ClID.IsNotNull(),
                    query.ClID != string.Empty);

                if (!chkIncludeClosed.Checked)
                    query.Where(query.IsClosed == false);

                if (chkNotMonitored.Checked)
                    query.Where("<NOT EXISTS (SELECT 1 FROM AdmedikaDailyMonitoring m " +
                                "WHERE m.AdmedikaClaimTransactionID = a.AdmedikaClaimTransactionID)>");

                if (!txtRegistrationDate.IsEmpty)
                    query.Where(registration.RegistrationDate >= txtRegistrationDate.SelectedDate.Value.Date);

                if (!txtRegistrationDateTo.IsEmpty)
                    query.Where(registration.RegistrationDate < txtRegistrationDateTo.SelectedDate.Value.Date.AddDays(1));

                if (!string.IsNullOrWhiteSpace(txtRegistrationNo.Text))
                    query.Where(query.RegistrationNo == txtRegistrationNo.Text.Trim());

                if (!string.IsNullOrWhiteSpace(txtCardNo.Text))
                    query.Where(query.CardNo.Like("%" + Helper.EscapeQuery(txtCardNo.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtClID.Text))
                    query.Where(query.ClID.Like("%" + Helper.EscapeQuery(txtClID.Text) + "%"));

                if (!string.IsNullOrWhiteSpace(txtPatientName.Text))
                    query.Where(query.PatientName.Like("%" + Helper.EscapeQuery(txtPatientName.Text) + "%"));

                query.OrderBy(registration.RegistrationDate.Descending, query.RegistrationNo.Descending);
                query.es.Top = AppSession.Parameter.MaxResultRecord;

                return query.LoadDataTable();
            }
        }
    }
}
