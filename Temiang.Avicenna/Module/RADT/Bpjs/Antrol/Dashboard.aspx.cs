using Temiang.Avicenna.Bridging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Bpjs.Antrol
{
    public partial class Dashboard : BasePage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AntrianOnlineDashboard;
            winQueueHistory.VisibleOnPageLoad = false;
            ScriptManager.GetCurrent(Page)?.RegisterPostBackControl(btnExportAntreanPerTanggal);

            if (!IsPostBack)
            {
                txtQueueDate.SelectedDate = DateTime.Today;
                cboTaskId.SelectedValue = string.Empty;
                txtTanggal.SelectedDate = DateTime.Now.Date;
                txtJam.Clear();

                txtStart.SelectedDate = DateTime.Now.Date;
                foreach (var month in Enumerable.Range(1, 12).Select(i => new { Index = i.ToString().Length == 1 ? $"0{i}" : i.ToString(), Name = DateTimeFormatInfo.CurrentInfo.GetMonthName(i) }))
                {
                    cboMonthly.Items.Add(new Telerik.Web.UI.RadComboBoxItem(month.Name, month.Index.ToString()));
                }
                cboMonthly.SelectedValue = DateTime.Now.Month.ToString().Length == 1 ? $"0{DateTime.Now.Month}" : DateTime.Now.Month.ToString();

                txtTglOutstanding.SelectedDate = DateTime.Now.Date;
                txtAntreanPerTanggal.SelectedDate = DateTime.Now.Date;

                var sub = new BusinessObject.ServiceUnitBridgingCollection();
                sub.Query.es.Distinct = true;
                sub.Query.Where(sub.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                if (sub.Query.Load())
                {
                    cboPoliAntreanPerTanggal.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                    foreach (var item in sub)
                    {
                        cboPoliAntreanPerTanggal.Items.Add(new RadComboBoxItem($"{item.BridgingID.Split(';')[1]} - {item.BridgingName}", item.BridgingID.Split(';')[1]));
                    }
                }

                var pb = new BusinessObject.ParamedicBridgingCollection();
                pb.Query.Where(sub.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                if (pb.Query.Load())
                {
                    cboDokterAntreanPerTanggal.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                    foreach (var item in pb)
                    {
                        cboDokterAntreanPerTanggal.Items.Add(new RadComboBoxItem($"{item.BridgingID} - {item.BridgingName}", item.BridgingID));
                    }
                }
            }
        }

        protected void btnQueueFilter_Click(object sender, EventArgs e)
        {
            winQueueHistory.VisibleOnPageLoad = false;
            grdTaskQueue.CurrentPageIndex = 0;
            lblQueueHistory.Text = string.Empty;
            grdTaskQueue.Rebind();
        }

        protected void grdTaskQueue_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            var table = new DataTable();
            table.Columns.Add("Booking", typeof(string));
            table.Columns.Add("TaskId", typeof(int));
            table.Columns.Add("TaskName", typeof(string));
            table.Columns.Add("EventTime", typeof(DateTime));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("Attempts", typeof(int));
            table.Columns.Add("RetryTime", typeof(DateTime));
            table.Columns.Add("Detail", typeof(string));
            grdTaskQueue.DataSource = table;
            lblQueueSummary.Text = string.Empty;

            if (!txtQueueDate.SelectedDate.HasValue && string.IsNullOrWhiteSpace(txtQueueBooking.Text))
            {
                lblQueueSummary.Text = "Pilih tanggal pelayanan atau isi nomor booking.";
                return;
            }

            try
            {
                var tasks = new BpjsAntrolTaskQueueCollection();
                var query = tasks.Query;
                if (!string.IsNullOrWhiteSpace(txtQueueBooking.Text))
                    query.Where(query.Booking == txtQueueBooking.Text.Trim());
                if (txtQueueDate.SelectedDate.HasValue)
                {
                    var date = txtQueueDate.SelectedDate.Value.Date;
                    var start = new DateTimeOffset(DateTime.SpecifyKind(date, DateTimeKind.Unspecified), TimeSpan.FromHours(7));
                    query.Where(query.EventMs >= start.ToUnixTimeMilliseconds(),
                        query.EventMs < start.AddDays(1).ToUnixTimeMilliseconds());
                }
                if (!string.IsNullOrEmpty(cboQueueStatus.SelectedValue))
                    query.Where(query.Status == cboQueueStatus.SelectedValue);
                query.OrderBy(query.Booking.Ascending, query.TaskId.Ascending);
                query.Load();

                foreach (var task in tasks)
                {
                    var row = table.NewRow();
                    row["Booking"] = task.Booking;
                    row["TaskId"] = task.TaskId.GetValueOrDefault();
                    row["TaskName"] = QueueTaskName(task.TaskId.GetValueOrDefault());
                    row["EventTime"] = DateTimeOffset.FromUnixTimeMilliseconds(task.EventMs.Value).ToOffset(TimeSpan.FromHours(7)).DateTime;
                    row["Status"] = task.Status;
                    row["Attempts"] = task.Attempts.GetValueOrDefault();
                    if (task.Status == "Pending")
                        row["RetryTime"] = DateTime.SpecifyKind(task.DueUtc.Value, DateTimeKind.Utc).AddHours(7);
                    row["Detail"] = task.Detail ?? string.Empty;
                    table.Rows.Add(row);
                }

                lblQueueSummary.Text = string.Format(
                    "{0} task tercatat dari {1} booking. Pending: {2}, Sent: {3}, Review: {4}, Skipped: {5}.",
                    tasks.Count, tasks.Select(t => t.Booking).Distinct().Count(),
                    tasks.Count(t => t.Status == "Pending"), tasks.Count(t => t.Status == "Sent"),
                    tasks.Count(t => t.Status == "Review"), tasks.Count(t => t.Status == "Skipped"));
            }
            catch (System.Data.SqlClient.SqlException)
            {
                lblQueueSummary.Text = "Data monitoring belum dapat dibaca. Periksa instalasi tabel queue dan koneksi database.";
            }
        }

        private static string QueueTaskName(int taskId)
        {
            switch (taskId)
            {
                case 1: return "Mulai menunggu admisi";
                case 2: return "Mulai pelayanan admisi";
                case 3: return "Selesai admisi / menunggu poli";
                case 4: return "Mulai pelayanan poli";
                case 5: return "Selesai pelayanan poli";
                case 6: return "Mulai pelayanan farmasi";
                case 7: return "Selesai pelayanan farmasi";
                case 99: return "Pembatalan";
                default: return taskId.ToString();
            }
        }

        protected void grdTaskQueue_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName != "CheckQueueHistory") return;
            var item = e.Item as GridDataItem;
            if (item == null) return;
            var booking = Convert.ToString(item.GetDataKeyValue("Booking"));
            winQueueHistory.VisibleOnPageLoad = true;
            grdQueueHistory.DataSource = new Common.BPJS.Antrian.List.TaskId.Response.List[0];
            grdQueueHistory.DataBind();
            try
            {
                var response = new Common.BPJS.Antrian.Service().GetListWaktuTaskId(
                    new Common.BPJS.Antrian.List.TaskId.Request.Root { Kodebooking = booking }, 10000);
                lblQueueHistory.Text = Server.HtmlEncode(booking + " - "
                    + (response?.Metadata == null ? "Respons BPJS kosong." : response.Metadata.Code + " - " + response.Metadata.Message));
                if (response?.Response != null)
                {
                    grdQueueHistory.DataSource = response.Response.Where(task => task != null).OrderBy(task => task.Taskid).ToList();
                    grdQueueHistory.DataBind();
                }
            }
            catch (Exception ex)
            {
                lblQueueHistory.Text = Server.HtmlEncode("Riwayat BPJS belum dapat dibaca: " + ex.Message);
            }
        }
        protected void btnFilter_Click(object sender, EventArgs e)
        {
            if (txtStart.IsEmpty) return;

            var svc = new Common.BPJS.Antrian.Service();
            var data = new List<Common.BPJS.Antrian.List.Dashboard.PerTanggal.List>();
            var response = svc.DashboardPerTanggal(txtStart.SelectedDate?.ToString("yyyy-MM-dd"), "server");
            if (response != null && response.Metadata.IsValid)
            {
                if (response.Response.List.Any())
                {
                    foreach (var item in response.Response.List)
                    {
                        var t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask1));
                        string time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        var t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask1));
                        string time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask1 = time1;
                        item.AvgWaktuTask1 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask2));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask2));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask2 = time1;
                        item.AvgWaktuTask2 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask3));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask3));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask3 = time1;
                        item.AvgWaktuTask3 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask4));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask4));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask4 = time1;
                        item.AvgWaktuTask4 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask5));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask5));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask5 = time1;
                        item.AvgWaktuTask5 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask6));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask6));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask6 = time1;
                        item.AvgWaktuTask6 = time2;

                        var t = TimeSpan.FromMilliseconds(Convert.ToDouble(item.Insertdate));
                        item.Insertdate = (new DateTime(1970, 1, 1) + t).ToString("yyyy-MM-dd HH:mm:ss");

                        var poli = new ServiceUnitBridging();
                        poli.Query.es.Top = 1;
                        poli.Query.Where(poli.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                        poli.Query.Where($"< SUBSTRING(BridgingID, CHARINDEX(';', BridgingID) + 1, 3) = '{item.Kodepoli}'>");
                        if (!poli.Query.Load())
                        {
                            poli = new ServiceUnitBridging();
                            poli.Query.es.Top = 1;
                            poli.Query.Where(poli.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                            poli.Query.Where($"< SUBSTRING(BridgingID, 0, CHARINDEX(';', BridgingID)) = '{item.Kodepoli}'>");
                            poli.Query.Load();
                        }

                        var rq = new RegistrationQuery("a");
                        var sepq = new BpjsSEPQuery("b");

                        rq.InnerJoin(sepq).On(rq.BpjsSepNo == sepq.NoSEP);
                        rq.Where(rq.RegistrationDate.Date() == txtStart.SelectedDate?.Date, rq.ServiceUnitID == poli.ServiceUnitID, rq.IsVoid == false);

                        var regs = new RegistrationCollection();
                        if (regs.Load(rq))
                        {
                            item.JumlahPasien = regs.Count;
                            item.Persentase = Math.Round(Convert.ToDouble((Convert.ToDouble(item.JumlahAntrean) / Convert.ToDouble(regs.Count)) * 100), 2);
                        }

                        data.Add(item);
                    }
                }
                else data.Add(new Common.BPJS.Antrian.List.Dashboard.PerTanggal.List() { Tanggal = txtStart.SelectedDate?.ToString("yyyy-MM-dd"), Namapoli = response.Metadata.Message });
            }
            else data.Add(new Common.BPJS.Antrian.List.Dashboard.PerTanggal.List() { Tanggal = txtStart.SelectedDate?.ToString("yyyy-MM-dd"), Namapoli = response.Metadata.Message });

            //var seps = new BpjsSEPQuery("a");
            //var sub = new ServiceUnitBridgingQuery("b");
            //var su = new ServiceUnitQuery("c");
            //seps.Select(seps.PoliTujuan.Count(), su.ServiceUnitName);
            //seps.InnerJoin(sub).On(seps.PoliTujuan == sub.BridgingID && sub.SRBridgingType == AppEnum.BridgingType.BPJS.ToString());
            //seps.InnerJoin(su).On(sub.ServiceUnitID == su.ServiceUnitID);
            //seps.Where(seps.TanggalSEP.Date() == txtStart.SelectedDate?.Date, seps.PoliTujuan.NotIn(data.Select(d => d.Kodepoli), "IGD"));
            //seps.GroupBy(seps.PoliTujuan, su.ServiceUnitName);
            //var sepTable = seps.LoadDataTable();
            //if (sepTable.Rows.Count > 0)
            //{
            //    data.AddRange(sepTable.AsEnumerable().Select(s => new Common.BPJS.Antrian.List.Dashboard.PerTanggal.List()
            //    {
            //        Tanggal = txtStart.SelectedDate?.ToString("yyyy-MM-dd"),
            //        Namapoli = s.Field<string>("ServiceUnitName"),
            //        JumlahAntrean = 0,
            //        JumlahPasien = s.Field<int>("PoliTujuan"),
            //        Persentase = 0
            //    }));
            //}

            var tbl = data.ToDataTable();
            grdList.DataSource = tbl;
            grdList.DataBind();
        }

        protected void btnFilter2_Click(object sender, EventArgs e)
        {
            var svc = new Common.BPJS.Antrian.Service();
            var data = new List<Common.BPJS.Antrian.List.Dashboard.PerBulan.List>();
            var response = svc.DashboardPerBulan(cboMonthly.SelectedValue, DateTime.Now.Year.ToString(), "server");
            if (response != null && response.Metadata.IsValid)
            {
                if (response.Response.List.Any())
                {
                    foreach (var item in response.Response.List)
                    {
                        var t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask1));
                        string time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        var t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask1));
                        string time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask1 = time1;
                        item.AvgWaktuTask1 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask2));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask2));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask2 = time1;
                        item.AvgWaktuTask2 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask3));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask3));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask3 = time1;
                        item.AvgWaktuTask3 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask4));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask4));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask4 = time1;
                        item.AvgWaktuTask4 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask5));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask5));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask5 = time1;
                        item.AvgWaktuTask5 = time2;

                        t1 = TimeSpan.FromSeconds(Convert.ToDouble(item.WaktuTask6));
                        time1 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t1.Hours,
                                        t1.Minutes,
                                        t1.Seconds);
                        t2 = TimeSpan.FromSeconds(Convert.ToDouble(item.AvgWaktuTask6));
                        time2 = string.Format("{0:D2}h:{1:D2}m:{2:D2}s",
                                        t2.Hours,
                                        t2.Minutes,
                                        t2.Seconds);
                        item.WaktuTask6 = time1;
                        item.AvgWaktuTask6 = time2;

                        var t = TimeSpan.FromMilliseconds(Convert.ToDouble(item.Insertdate));
                        item.Insertdate = (new DateTime(1970, 1, 1) + t).ToString("yyyy-MM-dd HH:mm:ss");

                        var poli = new ServiceUnitBridging();
                        poli.Query.es.Top = 1;
                        poli.Query.Where(poli.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                        poli.Query.Where($"< SUBSTRING(BridgingID, CHARINDEX(';', BridgingID) + 1, 3) = '{item.Kodepoli}'>");
                        if (!poli.Query.Load())
                        {
                            poli = new ServiceUnitBridging();
                            poli.Query.es.Top = 1;
                            poli.Query.Where(poli.Query.SRBridgingType == AppEnum.BridgingType.ANTROL.ToString());
                            poli.Query.Where($"< SUBSTRING(BridgingID, 0, CHARINDEX(';', BridgingID)) = '{item.Kodepoli}'>");
                            poli.Query.Load();
                        }

                        string format = "yyyy-MM-dd";
                        DateTime.TryParseExact(item.Tanggal, format, null, System.Globalization.DateTimeStyles.None, out var parsed);

                        var rq = new RegistrationQuery("a");
                        var sepq = new BpjsSEPQuery("b");

                        rq.InnerJoin(sepq).On(rq.BpjsSepNo == sepq.NoSEP);
                        rq.Where(rq.RegistrationDate.Date() == parsed.Date, rq.ServiceUnitID == poli.ServiceUnitID, rq.IsVoid == false);

                        var regs = new RegistrationCollection();
                        if (regs.Load(rq))
                        {
                            item.JumlahPasien = regs.Count;
                            item.Persentase = Math.Round(Convert.ToDouble((Convert.ToDouble(item.JumlahAntrean) / Convert.ToDouble(regs.Count)) * 100), 2);
                        }

                        data.Add(item);
                    }
                }
                else data.Add(new Common.BPJS.Antrian.List.Dashboard.PerBulan.List() { Tanggal = cboMonthly.Text, Namapoli = response.Metadata.Message });
            }
            else data.Add(new Common.BPJS.Antrian.List.Dashboard.PerBulan.List() { Tanggal = cboMonthly.Text, Namapoli = response.Metadata.Message });

            var tbl = data.ToDataTable();
            grdList2.DataSource = tbl;
            grdList2.Rebind();
        }

        protected void btnFilter3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomorBooking.Text)) return;

            var tanggal = DateTime.Now.Date;

            var svc = new Common.BPJS.Antrian.Service();
            var data = new List<Common.BPJS.Antrian.List.TaskId.Response.List>();
            var response = svc.GetListWaktuTaskId(new Common.BPJS.Antrian.List.TaskId.Request.Root() { Kodebooking = txtNomorBooking.Text });
            if (response != null && response.Metadata.IsValid)
            {
                if (response.Response.Any())
                {
                    data.AddRange(response.Response);

                    var last = response.Response.OrderBy(r => r.Taskid).LastOrDefault();
                    if (last != null) DateTime.TryParseExact(last.Wakturs.Split(' ')[0], "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out tanggal);
                }
                else data.Add(new Common.BPJS.Antrian.List.TaskId.Response.List() { Taskname = response.Metadata.Message });
            }
            else data.Add(new Common.BPJS.Antrian.List.TaskId.Response.List() { Taskname = response.Metadata.Message });

            var tbl = data.ToDataTable();
            grdList3.DataSource = tbl;
            grdList3.DataBind();

            cboTaskId.SelectedValue = string.Empty;
            txtTanggal.SelectedDate = tanggal;
            txtJam.Clear();
        }

        protected void btnUpdateTaskId_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtNomorBooking.Text)) return;
            if (string.IsNullOrEmpty(cboTaskId.SelectedValue)) return;
            if (txtTanggal.IsEmpty) return;
            if (txtJam.IsEmpty) return;

            var waktu = txtTanggal.SelectedDate.Value.Add(txtJam.SelectedTime.Value);

            var taskId = cboTaskId.SelectedValue.ToInt();
            if (AntrolTaskQueueProcessor.QueueEnabled && taskId >= 2 && taskId <= 7)
            {
                string message;
                try
                {
                    // The form accepts seconds; keep the original milliseconds for an existing event.
                    var queue = new BpjsAntrolTaskQueue();
                    queue.Query.Where(queue.Query.Booking == txtNomorBooking.Text.Trim(), queue.Query.TaskId == taskId);
                    var eventMs = new DateTimeOffset(waktu).ToUnixTimeMilliseconds();
                    if (queue.Query.Load() && queue.EventMs.HasValue && queue.EventMs.Value / 1000 == eventMs / 1000)
                        eventMs = queue.EventMs.Value;

                    message = AntrolTaskQueueProcessor.ScheduleManualTask(
                        txtNomorBooking.Text.Trim(), taskId, eventMs);
                    var audit = new WebServiceAPILog
                    {
                        DateRequest = DateTime.Now,
                        IPAddress = Request.UserHostAddress ?? string.Empty,
                        UrlAddress = "Dashboard - ScheduleAntrolTask",
                        Params = JsonConvert.SerializeObject(new { Booking = txtNomorBooking.Text.Trim(), TaskId = taskId, EventMs = eventMs, User = AppSession.UserLogin.UserID }),
                        Response = JsonConvert.SerializeObject(new { Message = message })
                    };
                    audit.Save();
                }
                catch (Exception ex)
                {
                    message = "Penjadwalan belum dapat dikonfirmasi. Periksa Monitoring Task. " + ex.Message;
                }
                ScriptManager.RegisterStartupScript(this, GetType(), "scheduleTask",
                    "alert(" + JsonConvert.SerializeObject(message) + ");", true);
                return;
            }
            var log = new WebServiceAPILog
            {
                DateRequest = DateTime.Now,
                IPAddress = "10.200.200.188",
                UrlAddress = "Dashboard",
                Params = JsonConvert.SerializeObject(new Common.BPJS.Antrian.Update.WaktuAntrian.Request.Root()
                {
                    Kodebooking = txtNomorBooking.Text,
                    Taskid = cboTaskId.SelectedValue.ToInt(),
                    Waktu = Convert.ToInt64(waktu.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)
                }),
                Response = string.Empty
            };

            var svcAntrol = new Common.BPJS.Antrian.Service();
            var responseAntrol = svcAntrol.UpdateWaktuAntrian(new Common.BPJS.Antrian.Update.WaktuAntrian.Request.Root()
            {
                Kodebooking = txtNomorBooking.Text,
                Taskid = cboTaskId.SelectedValue.ToInt(),
                Waktu = Convert.ToInt64(waktu.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds)
            });

            log.Response = JsonConvert.SerializeObject(responseAntrol);
            log.Save();

            if (responseAntrol.Metadata.IsValid) btnFilter3_Click(null, null);
            else ScriptManager.RegisterStartupScript(this, GetType(), "update", $"alert('code : {responseAntrol.Metadata.Code}', message : {responseAntrol.Metadata.Message});", true);
        }

        protected void btnFilterOutstanding_Click(object sender, EventArgs e)
        {
            var svc = new Common.BPJS.Antrian.Service();
            var list = svc.GetAntreanBelumDilayani(string.Empty);
            if (list != null && list.Metadata.IsAntrolValid && list.Response.List.Any())
            {
                var data = list.Response.List.Where(l => DateTime.ParseExact(l.Tanggal, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None).Date == txtTglOutstanding.SelectedDate?.Date && l.Status.ToLower() == "belum dilayani");
                if (data.Any())
                {
                    svc = new Common.BPJS.Antrian.Service();
                    var poli = svc.GetReferensiPoli();

                    svc = new Common.BPJS.Antrian.Service();
                    var dokter = svc.GetReferensiDokter();

                    foreach (var item in data.Select(d => d.Kodepoli).Distinct())
                    {
                        var x = poli.Response.List.First(p => p.Kdsubspesialis == item);
                        if (x == null) poli.Response.List.First(p => p.Kdpoli == item);
                        if (!cboPoliOustanding.Items.Any(c => c.Value == item)) cboPoliOustanding.Items.Add(new Telerik.Web.UI.RadComboBoxItem(x.Nmsubspesialis, item));
                    }

                    foreach (var item in data.Select(d => d.Kodedokter).Distinct())
                    {
                        if (!cboDokterOutstanding.Items.Any(c => c.Value == item.ToString())) cboDokterOutstanding.Items.Add(new Telerik.Web.UI.RadComboBoxItem(dokter.Response.List.First(p => p.Kodedokter == item).Namadokter, item.ToString()));
                    }

                    if (!string.IsNullOrWhiteSpace(cboPoliOustanding.SelectedValue)) data = data.Where(d => d.Kodepoli == cboPoliOustanding.SelectedValue).ToList();
                    if (!string.IsNullOrWhiteSpace(cboDokterOutstanding.SelectedValue)) data = data.Where(d => d.Kodedokter == cboDokterOutstanding.SelectedValue.ToInt()).ToList();

                    foreach (var item in data)
                    {
                        var x = poli.Response.List.First(p => p.Kdsubspesialis == item.Kodepoli);
                        if (x == null) poli.Response.List.First(p => p.Kdpoli == item.Kodepoli);
                        item.Namapoli = x.Nmsubspesialis;
                        item.Namadokter = dokter.Response.List.First(p => p.Kodedokter == item.Kodedokter).Namadokter;

                        var appt = new BusinessObject.Appointment();
                        if (appt.LoadByPrimaryKey(item.Kodebooking.Replace("/", "-"))) item.Namapasien = appt.PatientName;
                        else
                        {
                            var reg = new Registration();
                            if (reg.LoadByPrimaryKey(item.Kodebooking))
                            {
                                var patient = new Patient();
                                if (patient.LoadByPrimaryKey(reg.PatientID))
                                    item.Namapasien = patient.PatientName;
                            }
                        }
                    }

                    grdOustanding.DataSource = data.OrderBy(d => d.Kodepoli).ThenBy(d => d.Kodedokter).ThenBy(d => d.Jampraktek).ThenBy(d => d.Kodebooking);
                }
            }
            else
            {
                cboPoliOustanding.Items.Clear();
                cboDokterOutstanding.Items.Clear();
                grdOustanding.DataSource = new List<Common.BPJS.Antrian.List.Antrean.BelumDilayani.List>();
            }
            grdOustanding.DataBind();
        }

        protected void grdOustanding_DeleteCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item == null) return;

            var id = Convert.ToString(item.OwnerTableView.DataKeyValues[item.ItemIndex]["Kodebooking"]);

            var svc = new Common.BPJS.Antrian.Service();
            var batal = svc.BatalAntrian(new Common.BPJS.Antrian.Update.BatalAntrian.Request.Root()
            {
                Kodebooking = id,
                Keterangan = "Batal"
            });
            if (batal.Metadata.IsAntrolValid) btnFilterOutstanding_Click(null, null);
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "delete", string.Format("alert('Code : {0}, Message : {1}');", batal.Metadata.Code, batal.Metadata.Message.Replace("'", string.Empty)), true);
            }
        }

        protected void grdOustanding_DetailTableDataBind(object sender, GridDetailTableDataBindEventArgs e)
        {
            var svc = new Common.BPJS.Antrian.Service();
            var response = svc.GetListWaktuTaskId(new Common.BPJS.Antrian.List.TaskId.Request.Root()
            {
                Kodebooking = e.DetailTableView.ParentItem.GetDataKeyValue("Kodebooking").ToString()
            });
            var data = new List<Common.BPJS.Antrian.List.TaskId.Response.List>();

            if (response != null && response.Metadata.IsValid)
            {
                if (response.Response.Any())
                {
                    data.AddRange(response.Response);

                    //var last = response.Response.OrderBy(r => r.Taskid).LastOrDefault();
                    //if (last != null) DateTime.TryParseExact(last.Wakturs.Split(' ')[0], "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out tanggal);
                }
                else data.Add(new Common.BPJS.Antrian.List.TaskId.Response.List() { Taskname = response.Metadata.Message });
            }
            else data.Add(new Common.BPJS.Antrian.List.TaskId.Response.List() { Taskname = response.Metadata.Message });

            e.DetailTableView.DataSource = data.ToDataTable();
        }

        protected void btnAntreanPerTanggal_Click(object sender, EventArgs e)
        {
            grdAntreanPerTanggal.DataSource = GetAntreanPerTanggal();
            grdAntreanPerTanggal.DataBind();
        }

        protected void btnExportAntreanPerTanggal_Click(object sender, ImageClickEventArgs e)
        {
            CreateExcelFile.CreateExcelDocument(GetAntreanPerTanggal(), $"AntreanPerTanggal-{txtAntreanPerTanggal.SelectedDate?.ToString("yyyyMMdd")}.xlsx", Response);
        }

        private List<Common.BPJS.Antrian.List.Antrean.PerTanggal.List> GetAntreanPerTanggal()
        {
            var svc = new Common.BPJS.Antrian.Service();
            var response = svc.GetAntreanPerTanggal(txtAntreanPerTanggal.SelectedDate?.ToString("yyyy-MM-dd"));
            if (response == null || response.Metadata == null || !response.Metadata.IsAntrolValid || response.Response == null || response.Response.List == null) return new List<Common.BPJS.Antrian.List.Antrean.PerTanggal.List>();

            var list = response.Response.List;
            if (!string.IsNullOrWhiteSpace(cboPoliAntreanPerTanggal.SelectedValue)) list = list.Where(l => l.Kodepoli == cboPoliAntreanPerTanggal.SelectedValue).ToList();
            if (!string.IsNullOrWhiteSpace(cboDokterAntreanPerTanggal.SelectedValue)) list = list.Where(l => l.Kodedokter == cboDokterAntreanPerTanggal.SelectedValue.ToInt()).ToList();

            return list;
        }
    }
}
