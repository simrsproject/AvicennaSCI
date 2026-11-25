using System;
using Temiang.Dal.Interfaces;
using System.Data.SqlClient;
using System.Linq;
using BpjsAntrian = global::Temiang.Avicenna.Common.BPJS.Antrian;

namespace Temiang.Avicenna.Bridging
{
internal static class AntrolTaskQueueProcessor
{
    internal static bool EmrQueueEnabled
    {
        get
        {
            return string.Equals(System.Configuration.ConfigurationManager.AppSettings["BpjsAntrolEmrQueueEnabled"], "true", StringComparison.OrdinalIgnoreCase)
                && string.Equals(System.Configuration.ConfigurationManager.AppSettings["BpjsAntrolTaskQueueEnabled"], "true", StringComparison.OrdinalIgnoreCase)
                && string.Equals(global::Temiang.Avicenna.Common.AppSession.Parameter.HealthcareID, "RSI", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static bool PharmacyQueueEnabled
    {
        get
        {
            return EmrQueueEnabled
                && string.Equals(System.Configuration.ConfigurationManager.AppSettings["BpjsAntrolPharmacyQueueEnabled"], "true", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal static void EnqueuePharmacy(string booking, int task, long eventMs)
    {
        if (string.IsNullOrWhiteSpace(booking) || (task != 6 && task != 7) || eventMs <= 0)
            throw new ArgumentException("Invalid pharmacy task.");

        var parameters = new esParameters();
        parameters.Add("Booking", booking, esParameterDirection.Input, System.Data.DbType.String, 50);
        parameters.Add("TaskId", task, esParameterDirection.Input, System.Data.DbType.Int32, 0);
        parameters.Add("EventMs", eventMs, esParameterDirection.Input, System.Data.DbType.Int64, 0);
        new Temiang.Dal.Core.esUtility().ExecuteNonQuery("dbo", "BpjsAntrolEnqueuePharmacy", parameters);
    }
    internal static void EnqueueAttendance(string booking, int task, long eventMs)
    {
        if (eventMs <= 0)
            throw new InvalidOperationException("Waktu attendance belum tercatat.");

        var info = esConfigSettings.ConnectionInfo;
        using (var db = new SqlConnection(info.Connections[info.Default].ConnectionString))
        using (var cmd = Command(db, @"INSERT dbo.BpjsAntrolTaskQueue(Booking,TaskId,EventMs,StartedMs)
            SELECT @b,@t,@time,@time WHERE NOT EXISTS
            (SELECT 1 FROM dbo.BpjsAntrolTaskQueue WITH (UPDLOCK,HOLDLOCK) WHERE Booking=@b AND TaskId=@t)", booking, task))
        {
            cmd.Parameters.Add("@time", System.Data.SqlDbType.BigInt).Value = eventMs;
            db.Open();
            cmd.ExecuteNonQuery();
        }
    }

    private static SqlCommand Command(SqlConnection db, string sql, string booking, int task)
    {
        var cmd = new SqlCommand(sql, db);
        cmd.Parameters.AddWithValue("@b", booking);
        cmd.Parameters.AddWithValue("@t", task);
        return cmd;
    }

    private static void Finish(SqlConnection db, string booking, int task, string status, string detail)
    {
        using (var cmd = Command(db, @"UPDATE dbo.BpjsAntrolTaskQueue
            SET Status=@status, Detail=LEFT(@detail,500), DueUtc=DATEADD(minute,5,SYSUTCDATETIME())
            WHERE Booking=@b AND TaskId=@t", booking, task))
        {
            cmd.Parameters.AddWithValue("@status", status);
            cmd.Parameters.AddWithValue("@detail", detail ?? "");
            cmd.ExecuteNonQuery();
        }
    }

    private static bool ProcessOne(SqlConnection db, string bookingFilter)
    {
        string booking;
        int task, attempts;
        long time;
        using (var cmd = new SqlCommand(@";WITH work AS (
            SELECT TOP(1) q.* FROM dbo.BpjsAntrolTaskQueue q
            WHERE Status='Pending' AND DueUtc<=SYSUTCDATETIME() AND (@booking IS NULL OR Booking=@booking)
            AND (TaskId=2 OR NOT EXISTS (SELECT 1 FROM dbo.BpjsAntrolTaskQueue p
                WHERE p.Booking=q.Booking AND p.TaskId<q.TaskId AND p.Status NOT IN ('Sent','Skipped')))
            ORDER BY DueUtc, Booking, TaskId)
            UPDATE work SET Attempts=Attempts+1, DueUtc=DATEADD(minute,5,SYSUTCDATETIME())
            OUTPUT inserted.Booking,inserted.TaskId,inserted.EventMs,inserted.Attempts;", db))
        {
        cmd.Parameters.Add("@booking", System.Data.SqlDbType.NVarChar, 50).Value = (object)bookingFilter ?? DBNull.Value;
        using (var r = cmd.ExecuteReader())
        {
            if (!r.Read()) return false;
            booking=r.GetString(0); task=r.GetInt32(1); time=r.GetInt64(2); attempts=r.GetInt32(3);
        }

        }
        try
        {
            var history = new BpjsAntrian.Service().GetListWaktuTaskId(
                new BpjsAntrian.List.TaskId.Request.Root { Kodebooking=booking }, 10000);
            bool confirmedEmpty = false;
            if (task == 3 && Convert.ToString(history?.Metadata?.Code) == "204"
                && (history.Response == null || !history.Response.Any()))
            {
                var registration = new BpjsAntrian.Service().GetAntreanPerKodeBooking(booking);
                confirmedEmpty = registration?.Metadata?.IsAntrolValid == true
                    && registration.Response?.List?.Any(row => row != null && row.Kodebooking == booking
                        && string.Equals(row.Status, "Belum dilayani", StringComparison.OrdinalIgnoreCase)) == true;
            }
            if (!confirmedEmpty && (history == null || history.Metadata == null || !history.Metadata.IsAntrolValid || history.Response == null))
                throw new InvalidOperationException("Riwayat BPJS belum dapat dipastikan. Code="
                    + (history == null || history.Metadata == null ? "null" : history.Metadata.Code) + ", Message="
                    + (history == null || history.Metadata == null ? "Respons kosong" : history.Metadata.Message));
            var ids = confirmedEmpty ? new System.Collections.Generic.List<int>()
                : history.Response.Where(x => x != null).Select(x => x.Taskid).ToList();
            if (ids.Contains(task))
                Finish(db, booking, task, "Sent", "Terkonfirmasi dari riwayat");
            else if (ids.Contains(99) || (task == 2 && ids.Contains(3)))
                Finish(db, booking, task, "Skipped", "Booking batal atau task 3 sudah ada");
            else if (task >= 4 && !ids.Contains(task - 1))
                Finish(db, booking, task, attempts >= 5 ? "Review" : "Pending", "Menunggu task " + (task - 1));
            else if (task == 3 && ids.Contains(1) && !ids.Contains(2))
            {
                using (var cmd = Command(db, @"INSERT dbo.BpjsAntrolTaskQueue(Booking,TaskId,EventMs,StartedMs)
                    SELECT Booking,2,StartedMs,StartedMs FROM dbo.BpjsAntrolTaskQueue
                    WHERE Booking=@b AND TaskId=@t AND NOT EXISTS
                    (SELECT 1 FROM dbo.BpjsAntrolTaskQueue WHERE Booking=@b AND TaskId=2); UPDATE dbo.BpjsAntrolTaskQueue SET DueUtc=SYSUTCDATETIME() WHERE Booking=@b AND TaskId=3 AND Status='Pending';", booking, task))
                    if (cmd.ExecuteNonQuery() == 0)
                        Finish(db, booking, task, attempts >= 5 ? "Review" : "Pending", "Task 2 belum terlihat di riwayat");
            }
            else
            {
                var result = new BpjsAntrian.Service().UpdateWaktuAntrian(
                    new BpjsAntrian.Update.WaktuAntrian.Request.Root { Kodebooking=booking, Taskid=task, Waktu=time });
                if (result == null || result.Metadata == null)
                    throw new InvalidOperationException("Respons pengiriman kosong");
                Finish(db, booking, task, result.Metadata.IsAntrolValid ? "Sent" : "Review", result.Metadata.Message);
            }
        }
        catch (SqlException) { throw; }
        catch (Exception ex)
        {
            Finish(db, booking, task, attempts >= 5 ? "Review" : "Pending", ex.GetType().Name + ": " + ex.Message);
        }
        return true;
    }

    public static int Run(string booking = null)
    {
        var connectionInfo = esConfigSettings.ConnectionInfo;
        using (var db = new SqlConnection(connectionInfo.Connections[connectionInfo.Default].ConnectionString))
        {
            db.Open();
            using (var cmd = new SqlCommand(@"DECLARE @r int; EXEC @r=sys.sp_getapplock
                @Resource='BpjsAntrolTaskQueue.Worker',@LockMode='Exclusive',@LockOwner='Session',@LockTimeout=0; SELECT @r", db))
            {
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                if (result == -1) return 0;
                if (result < 0) throw new InvalidOperationException("Queue lock failed");
            }
            try
            {
                int count = 0;
                while (count < (booking == null ? 10 : 3) && ProcessOne(db, booking)) count++;
                return count;
            }
            finally
            {
                using (var release = new SqlCommand(@"EXEC sys.sp_releaseapplock
                    @Resource='BpjsAntrolTaskQueue.Worker', @LockOwner='Session';", db))
                    release.ExecuteNonQuery();
            }
        }
    }
}
}
