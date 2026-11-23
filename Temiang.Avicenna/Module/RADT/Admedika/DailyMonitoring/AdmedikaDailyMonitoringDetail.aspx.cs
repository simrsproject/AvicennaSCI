using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaDailyMonitoringDetail : BasePageDetail
    {
        private AdmedikaClaimTransaction _claim;
        private Registration _registration;

        protected long ClaimTransactionID
        {
            get
            {
                long id;
                long.TryParse(Request.QueryString["id"], out id);
                return id;
            }
        }

        private AdmedikaClaimTransaction CurrentClaim
        {
            get
            {
                if (_claim == null)
                {
                    var claim = new AdmedikaClaimTransaction();
                    if (claim.LoadByPrimaryKey(ClaimTransactionID))
                        _claim = claim;
                }

                return _claim;
            }
        }

        private Registration CurrentRegistration
        {
            get
            {
                if (_registration == null)
                {
                    var claim = CurrentClaim;
                    if (claim != null && !string.IsNullOrWhiteSpace(claim.RegistrationNo))
                    {
                        var registration = new Registration();
                        if (registration.LoadByPrimaryKey(claim.RegistrationNo))
                            _registration = registration;
                    }
                }

                return _registration;
            }
        }

        private DateTime SelectedDate
        {
            get
            {
                return txtMonitoringDate.SelectedDate.HasValue
                    ? txtMonitoringDate.SelectedDate.Value.Date
                    : DateTime.Today;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaDailyMonitoring;
            UrlPageList = "AdmedikaDailyMonitoringList.aspx";

            if (!IsPostBack)
            {
                cboCategory.Items.Clear();
                foreach (var category in AdmedikaDailyMonitoringCategory.All)
                {
                    cboCategory.Items.Add(new RadComboBoxItem(
                        "[" + category.Code + "] " + category.Name,
                        category.Code.ToString(CultureInfo.InvariantCulture)));
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            txtMonitoringDate.SelectedDate = DateTime.Today;
            LoadDetail();
            BindEntries();
            BindHistory();
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            ToolBarMenuAdd.Enabled = false;
            ToolBarMenuEdit.Enabled = false;
            ToolBarMenuDelete.Enabled = false;
            ToolBarMenuSave.Visible = false;
            ToolBarMenuSearch.Enabled = false;
            ToolBarMenuMovePrev.Visible = false;
            ToolBarMenuMoveNext.Visible = false;
            ToolBarMenuApproval.Visible = false;
            ToolBarMenuUnApproval.Visible = false;
            ToolBarMenuVoid.Visible = false;
            ToolBarMenuUnVoid.Visible = false;
        }

        private void LoadDetail()
        {
            var claim = CurrentClaim;
            if (claim == null)
            {
                ShowInformationHeader("Data klaim tidak ditemukan.");
                return;
            }

            lblClID.Text = Value(claim.ClID);
            lblClStatus.Text = Value(claim.ClStatus);
            lblClDesc.Text = Value(claim.ClDesc);

            lblCardNo.Text = Value(claim.CardNo);
            lblPatientID.Text = Value(claim.PatientID);
            lblPatientName.Text = Value(claim.PatientName);
            lblMedicalNo.Text = Value(claim.MedicalNo);

            var registration = CurrentRegistration;
            if (registration != null)
            {
                lblRegistrationNo.Text = Value(registration.RegistrationNo);
                lblRegistrationDate.Text = FormatDate(registration.RegistrationDate);

                var roomName = string.Empty;
                if (!string.IsNullOrWhiteSpace(registration.RoomID))
                {
                    var room = new ServiceRoom();
                    if (room.LoadByPrimaryKey(registration.RoomID))
                        roomName = room.RoomName;
                }

                lblRoom.Text = string.Join(" / ", new[]
                {
                    roomName,
                    registration.ClassID,
                    registration.BedID
                }.Where(x => !string.IsNullOrWhiteSpace(x)));

                lblDischargeDate.Text = registration.DischargeDate.HasValue
                    ? FormatDate(registration.DischargeDate)
                    : "-";

                lblPatientStatus.Text = registration.DischargeDate.HasValue ? "Pulang" : "Proses Rawat";
            }
            else
            {
                lblRegistrationNo.Text = "-";
                lblRegistrationDate.Text = "-";
                lblRoom.Text = "-";
                lblDischargeDate.Text = "-";
                lblPatientStatus.Text = "-";
            }

            lblLastDiagnosis.Text = LoadLastDiagnosis(registration);

            var days = 0;
            if (registration != null && registration.RegistrationDate.HasValue)
            {
                days = (DateTime.Today - registration.RegistrationDate.Value.Date).Days + 1;
                if (days < 1) days = 1;
            }
            lblDayInfo.Text = days > 0 ? days + " hari" : "-";
        }

        private static string LoadLastDiagnosis(Registration registration)
        {
            if (registration == null || string.IsNullOrWhiteSpace(registration.RegistrationNo))
                return "-";

            var query = new RegistrationInfoMedicDiagnoseQuery("a");
            var diagnose = new DiagnoseQuery("b");

            query.LeftJoin(diagnose).On(query.DiagnoseID == diagnose.DiagnoseID);
            query.Select(query.DiagnosisText, diagnose.DiagnoseName);            query.Where(
                query.RegistrationNo == registration.RegistrationNo,
                query.IsVoid == false);
            query.OrderBy(query.DiagnoseDateTime.Descending);
            query.es.Top = 1;

            var table = query.LoadDataTable();
            if (table.Rows.Count == 0) return "-";

            var name = Convert.ToString(table.Rows[0]["DiagnosisText"]);
            if (string.IsNullOrWhiteSpace(name))
                name = Convert.ToString(table.Rows[0]["DiagnoseName"]);

            return string.IsNullOrWhiteSpace(name) ? "-" : name;
        }

        private AdmedikaDailyMonitoring EnsureHeader()
        {
            return AdmedikaDailyMonitoringHelper.EnsureHeader(ClaimTransactionID, SelectedDate);
        }

        private void BindEntries()
        {
            var table = new DataTable();
            table.Columns.Add("AdmedikaDailyMonitoringItemID", typeof(long));
            table.Columns.Add("CategoryName", typeof(string));
            table.Columns.Add("Remarks", typeof(string));
            table.Columns.Add("EntryTime", typeof(string));
            table.Columns.Add("SourceType", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("IsSent", typeof(bool));

            var header = AdmedikaDailyMonitoringHelper.GetHeader(ClaimTransactionID, SelectedDate);
            if (header != null)
            {
                foreach (var item in AdmedikaDailyMonitoringHelper.GetItems(header.AdmedikaDailyMonitoringID ?? 0))
                {
                    var row = table.NewRow();
                    row["AdmedikaDailyMonitoringItemID"] = item.AdmedikaDailyMonitoringItemID ?? 0;
                    row["CategoryName"] = "[" + item.CategoryCode + "] " +
                                          AdmedikaDailyMonitoringCategory.NameOf(item.CategoryCode ?? 0);
                    row["Remarks"] = item.Remarks ?? string.Empty;
                    row["EntryTime"] = item.EntryDateTime.HasValue
                        ? item.EntryDateTime.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                        : string.Empty;
                    row["SourceType"] = item.SourceType ?? string.Empty;
                    row["Status"] = item.Status ?? string.Empty;
                    row["IsSent"] = item.IsSent == true;
                    table.Rows.Add(row);
                }

                lblSendStatus.Text = header.IsSent == true
                    ? (header.IsSuccess == true ? "Terkirim " : "Gagal dikirim ")
                      + (header.SentDateTime.HasValue
                          ? header.SentDateTime.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture)
                          : string.Empty)
                      + (string.IsNullOrWhiteSpace(header.Message) ? string.Empty : " - " + header.Message)
                    : "Belum dikirim";
            }
            else
            {
                lblSendStatus.Text = "Belum ada data monitoring";
            }

            rptEntries.DataSource = table;
            rptEntries.DataBind();

            var days = 0;
            var registration = CurrentRegistration;
            if (registration != null && registration.RegistrationDate.HasValue)
            {
                days = (SelectedDate - registration.RegistrationDate.Value.Date).Days + 1;
                if (days < 1) days = 1;
            }
            lblDayInfo.Text = days > 0 ? days + " hari" : "-";
        }

        private void BindHistory()
        {
            var table = new DataTable();
            table.Columns.Add("AdmedikaDailyMonitoringID", typeof(long));
            table.Columns.Add("MonitoringDate", typeof(DateTime));
            table.Columns.Add("SentDateTime", typeof(DateTime));
            table.Columns.Add("OkCount", typeof(string));
            table.Columns.Add("ErCount", typeof(string));
            table.Columns.Add("Message", typeof(string));

            var query = new AdmedikaDailyMonitoringQuery("a");
            query.Where(query.AdmedikaClaimTransactionID == ClaimTransactionID);
            query.OrderBy(query.MonitoringDate.Descending);
            query.es.Top = 60;

            var collection = new AdmedikaDailyMonitoringCollection();
            collection.Load(query);

            foreach (var header in collection)
            {
                var row = table.NewRow();
                row["AdmedikaDailyMonitoringID"] = header.AdmedikaDailyMonitoringID ?? 0;
                row["MonitoringDate"] = header.MonitoringDate ?? DateTime.Today;
                if (header.SentDateTime.HasValue)
                    row["SentDateTime"] = header.SentDateTime.Value;
                row["OkCount"] = header.OkCount.HasValue ? header.OkCount.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
                row["ErCount"] = header.ErCount.HasValue ? header.ErCount.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
                row["Message"] = header.Message ?? string.Empty;
                table.Rows.Add(row);
            }

            grdHistory.DataSource = table;
            grdHistory.DataBind();
        }

        protected void rptEntries_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                return;

            var textBox = e.Item.FindControl("txtRemarks") as System.Web.UI.WebControls.TextBox;
            if (textBox == null) return;

            var isSent = DataBinder.Eval(e.Item.DataItem, "IsSent");
            textBox.ReadOnly = isSent != null && Convert.ToBoolean(isSent);
        }

        protected void rptEntries_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "DeleteItem") return;

            long itemID;
            if (!long.TryParse(Convert.ToString(e.CommandArgument), out itemID)) return;

            AdmedikaDailyMonitoringHelper.DeleteItem(itemID);
            BindEntries();
        }

        protected void btnShow_Click(object sender, EventArgs e)
        {
            SaveEntries();
            BindEntries();
            BindHistory();
        }

        protected void btnPull_Click(object sender, EventArgs e)
        {
            SaveEntries();

            var header = EnsureHeader();
            AdmedikaDailyMonitoringHelper.PullAutoEntries(header, CurrentClaim, CurrentRegistration,
                AppSession.UserLogin.UserID);

            BindEntries();
            BindHistory();
            ShowInformationHeader("Data otomatis berhasil ditarik. Periksa dan sesuaikan keterangan sebelum dikirim.");
        }

        protected void btnAddEntry_Click(object sender, EventArgs e)
        {
            SaveEntries();

            int categoryCode;
            if (!int.TryParse(cboCategory.SelectedValue, out categoryCode)) return;

            var header = EnsureHeader();
            AdmedikaDailyMonitoringHelper.AddItem(header, categoryCode, null, string.Empty,
                AdmedikaDailyMonitoringHelper.SourceManual);

            BindEntries();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveEntries();
            BindEntries();
            BindHistory();
            ShowInformationHeader("Entri monitoring berhasil disimpan.");
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            SaveEntries();

            var header = AdmedikaDailyMonitoringHelper.GetHeader(ClaimTransactionID, SelectedDate);
            if (header == null)
            {
                ShowInformationHeader("Belum ada entri monitoring untuk tanggal ini.");
                return;
            }

            var items = AdmedikaDailyMonitoringHelper.GetItems(header.AdmedikaDailyMonitoringID ?? 0);

            bool success;
            var message = AdmedikaDailyMonitoringHelper.Send(header, items, out success);

            BindEntries();
            BindHistory();
            ShowInformationHeader(message);
        }

        private void SaveEntries()
        {
            foreach (System.Web.UI.WebControls.RepeaterItem item in rptEntries.Items)
            {
                if (item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
                    item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
                    continue;

                var hidden = item.FindControl("hdnItemID") as System.Web.UI.WebControls.HiddenField;
                var textBox = item.FindControl("txtRemarks") as System.Web.UI.WebControls.TextBox;
                if (hidden == null || textBox == null) continue;

                long itemID;
                if (!long.TryParse(hidden.Value, out itemID)) continue;

                var entity = new AdmedikaDailyMonitoringItem();
                if (!entity.LoadByPrimaryKey(itemID)) continue;
                if (entity.IsSent == true) continue;

                var remarks = textBox.Text ?? string.Empty;
                if (entity.Remarks == remarks) continue;

                entity.Remarks = remarks;
                entity.LastUpdateDateTime = DateTime.Now;
                entity.LastUpdateByUserID = AppSession.UserLogin.UserID;
                entity.Save();
            }
        }

        protected void grdHistory_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName != "SelectDate") return;

            DateTime date;
            if (!DateTime.TryParseExact(Convert.ToString(e.CommandArgument), "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return;

            txtMonitoringDate.SelectedDate = date;
            BindEntries();
        }

        private static string Value(object value)
        {
            return value == null ? string.Empty : Convert.ToString(value);
        }

        private static string FormatDate(DateTime? value)
        {
            return value.HasValue
                ? value.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                : "-";
        }
    }
}
