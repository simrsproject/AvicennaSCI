using DevExpress.XtraRichEdit.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Common;
using Temiang.Avicenna.BusinessObject.Generated;
using Temiang.Avicenna.Common;
using Temiang.Dal;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;
using static Temiang.Avicenna.BusinessObject.DashboardClinicConfig;
using static Temiang.Avicenna.BusinessObject.VisitQueue;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Summary description for Antrol
    /// </summary>
    public class ApiResponeForAntrian
    {
        public static void Success(HttpContext context, object data, string message = "OK")
        {
            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                code = 200,
                errorCode = (string)null,
                message = message,
                data = data
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write(json);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
        }

        public static void Error(HttpContext context, string message, int code = 500)
        {
            var json = JsonConvert.SerializeObject(new
            {
                success = false,
                code = code,
                errorCode = "ERR",
                message = message,
                data = (object)null
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            context.Response.Write(json);

            context.ApplicationInstance.CompleteRequest();
        }
    }

    public class CounterItem
    {
        public string CounterID { get; set; }
        public string CounterName { get; set; }
        public string Name {  get; set; }
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class AntrianRSI : System.Web.Services.WebService
    {
        private string GetChannelByServiceUnit(string serviceUnitID)
        {
            switch (serviceUnitID)
            {
                case "D2.2.60.4":
                    return "LOKET_PD";

                case "D2.2.60.5":
                    return "LOKET_PM";

                default:
                    return "";
            }
        }

        private List<CounterItem> GetCounterList()
        {
            var data = new List<CounterItem>();

            for (int i = 1; i <= 10; i++)
            {
                data.Add(new CounterItem
                {
                    CounterID = i.ToString(),
                    CounterName = "Counter_" + i,
                    Name = "Loket Pendaftaran " + i
                });
            }

            return data;
        }

        private List<string> GetValidCounterIDs()
        {
            return GetCounterList()
                .Select(x => x.CounterID)
                .ToList();
        }

        public class UpdateDisplayDoctorRequest
        {
            public string ServiceUnitID { get; set; }

            public List<DisplayDoctorItem> Doctors { get; set; }
        }

        public class GetDisplayDoctorListRequest
        {
            public List<string> ServiceUnitID { get; set; }

            public DateTime? QueueDate { get; set; }
        }

        public class DashboardClinicConfigRequest
        {
            public string ConfigID { get; set; }

            public string UserID { get; set; }

            public string ConfigName { get; set; }

            public DashboardClinicSetting Settings { get; set; }

            public List<DashboardClinicRoomItem> Rooms { get; set; }
        }

        public class DashboardClinicSetting
        {
            public bool AutoRefresh { get; set; }

            public int RefreshIntervalSec { get; set; }
        }

        //1. Pasien Ambil Antrian
        [WebMethod(EnableSession = false, Description = @"
           Ambil Data List PayerType untuk pasien memilih type TUNAI, MITRA DAN BPJS

           PARAMETER:
           - UserID (optional)

           RESPONSE:
            200 = Berhasil mendapatkan data payer type
            404 = Data payer type tidak ditemukan
            500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetPayerType()
        {
            try
            {
                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                System.Diagnostics.Debug.WriteLine("===== GET PAYER TYPE =====");
                System.Diagnostics.Debug.WriteLine("UserID : " + UserID);

                string serviceUnitID = "";
                string channel = "";

                // Jika UserID dikirim, coba cari channel
                if (!string.IsNullOrEmpty(UserID))
                {
                    AppUserServiceUnitCollection userSU =
                        new AppUserServiceUnitCollection();

                    userSU.Query.Where(
                        userSU.Query.UserID == UserID
                    );

                    userSU.Query.Load();

                    System.Diagnostics.Debug.WriteLine(
                        "Total Service Unit : " + userSU.Count
                    );

                    for (int i = 0; i < userSU.Count; i++)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"ServiceUnitID[{i}] = {userSU[i].ServiceUnitID}"
                        );
                    }

                    foreach (AppUserServiceUnit item in userSU)
                    {
                        string tempChannel =
                            GetChannelByServiceUnit(item.ServiceUnitID);

                        System.Diagnostics.Debug.WriteLine(
                            $"Check ServiceUnitID = {item.ServiceUnitID}, Channel = {tempChannel}"
                        );

                        if (!string.IsNullOrEmpty(tempChannel))
                        {
                            serviceUnitID = item.ServiceUnitID;
                            channel = tempChannel;
                            break;
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine(
                    $"Selected ServiceUnitID = {serviceUnitID}"
                );

                System.Diagnostics.Debug.WriteLine(
                    $"Channel = {channel}"
                );

                var collection =
                    new AntrianAutoNumberSemanticCollection();

                // Jika ada channel gunakan filter channel
                if (!string.IsNullOrEmpty(channel))
                {
                    collection.Query.Where(
                        collection.Query.Channel == channel,
                        collection.Query.IsActive == true
                    );
                }
                else
                {
                    // Fallback: ambil semua data aktif
                    collection.Query.Where(
                        collection.Query.IsActive == true
                    );
                }

                collection.Query.Load();

                System.Diagnostics.Debug.WriteLine(
                    $"Total Data = {collection.Count}"
                );

                var payerTypes = collection
                    .Select(x => x.PayerType)
                    .Distinct();

                // Filter khusus berdasarkan channel
                if (channel == "LOKET_PD")
                {
                    payerTypes = payerTypes
                        .Where(x =>
                            x == "TUNAI" ||
                            x == "MITRA");
                }
                else if (channel == "LOKET_PM")
                {
                    payerTypes = payerTypes
                        .Where(x =>
                            x == "TUNAI" ||
                            x == "BPJS" ||
                            x == "MITRA");
                }

                var data = payerTypes
                    .OrderBy(x =>
                        x == "TUNAI" ? 1 :
                        x == "BPJS" ? 2 :
                        x == "MITRA" ? 3 : 99
                    )
                    .Select(x => new
                    {
                        PayerType = x
                    })
                    .ToList();

                System.Diagnostics.Debug.WriteLine(
                    $"Total PayerType = {data.Count}"
                );

                if (data.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data PayerType tidak ditemukan",
                        404
                    );
                    return;
                }

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        UserID = UserID,
                        ServiceUnitID = serviceUnitID,
                        Channel = channel,
                        PayerTypes = data
                    }
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "ERROR : " + ex.ToString()
                );

                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Ambil Data List Service BPJS untuk kebutuhan pengambilan antrian pasien BPJS

            RESPONSE:
            200 = Berhasil mendapatkan list service BPJS
            404 = List data service BPJS tidak ditemukan
            500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetServiceBPJS()
        {
            try
            {
                var collection = new AntrianAutoNumberSemanticCollection();

                collection.Query.Where(
                    collection.Query.Channel == "LOKET_PD",
                    collection.Query.PayerType == "BPJS",
                    collection.Query.IsActive == true
                );

                collection.Query.OrderBy(
                    collection.Query.DisplayOrder.Ascending
                );

                collection.Query.Load();

                var data = collection
                    .Select(x => new
                    {
                        ServiceGroup = x.ServiceGroup,
                        SRAutoNumber = x.SRAutoNumber,
                        DisplayName = x.DisplayName
                    })
                    .ToList();

                if (data.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "List Data Service BPJS tidak ditemukan",
                        404
                    );
                    return;
                }

                ApiResponeForAntrian.Success(Context, data);
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(Context, ex.Message);
            }
        }
        //Akhir 1.Pasien Ambil Antrian


        //2. Generate Nomor Antrian dan Nomor Kunjungan
        [WebMethod(Description = @"
            Ambil Nomor Antrian Pasien

            PARAMETER:
            - PayerType (required)
            - ServiceGroup (optional untuk selain BPJS)
            - QueueLocation (required)

            CONTOH:
            TakeQueueVisitNumber?
            PayerType=BPJS&
            ServiceGroup=POLI&
            QueueLocation=LOKET_PD

            KETERANGAN:
            - PayerType : Jenis pembayaran pasien (BPJS, TUNAI, MITRA)
            - ServiceGroup : Wajib diisi jika PayerType = BPJS
            - QueueLocation : Lokasi/channel antrian

            RESPONSE:
               200 = Berhasil mengambil nomor antrian
               400 = Parameter request tidak valid (PayerType wajib diisi / QueueLocation wajib diisi)
               404 = Mapping antrian tidak ditemukan
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void TakeQueueVisitNumber()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string PayerType =
                    (Context.Request["PayerType"] ?? "")
                    .Trim()
                    .ToUpper();

                string ServiceGroup =
                    (Context.Request["ServiceGroup"] ?? "")
                    .Trim()
                    .ToUpper();

                string QueueLocation =
                    (Context.Request["QueueLocation"] ?? "")
                    .Trim()
                    .ToUpper();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(PayerType))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "PayerType wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(QueueLocation))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "QueueLocation wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // VALIDASI CHANNEL
                // =========================================
                var allowedLocation =
                    new AntrianAutoNumberSemanticCollection();

                allowedLocation.Query.Load();

                var listChannel = allowedLocation
                    .Where(x => !string.IsNullOrEmpty(x.Channel))
                    .Select(x => x.Channel.ToUpper())
                    .Distinct()
                    .ToList();

                if (!listChannel.Contains(QueueLocation))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "QueueLocation tidak valid",
                        400
                    );
                    return;
                }

                // =========================================
                // VALIDASI BPJS
                // =========================================
                if (
                    PayerType == "BPJS" &&
                    string.IsNullOrEmpty(ServiceGroup)
                )
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "ServiceGroup wajib untuk BPJS",
                        400
                    );
                    return;
                }

                // =========================================
                // AMBIL MAPPING
                // =========================================
                var collection =
                    new AntrianAutoNumberSemanticCollection();

                collection.Query.Where(
                    collection.Query.Channel == QueueLocation,
                    collection.Query.PayerType == PayerType,
                    collection.Query.IsActive == true
                );

                if (PayerType == "BPJS")
                {
                    collection.Query.Where(
                        collection.Query.ServiceGroup == ServiceGroup
                    );
                }

                collection.Query.Load();

                var mapping = collection.FirstOrDefault();

                if (mapping == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Mapping antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // EXECUTE PROCEDURE VIA BO
                // =========================================
                var result =
                    AntrianAutoNumberSemantic.TakeQueueVisitNumber(
                        mapping.SRAutoNumber,
                        QueueLocation
                    );

                // =========================================
                // AMBIL OUTPUT
                // =========================================
                string visitNo =
                    result.GetType()
                          .GetProperty("VisitNo")
                          .GetValue(result, null)
                          .ToString();

                string visitQueueNo =
                    result.GetType()
                          .GetProperty("VisitQueueNo")
                          .GetValue(result, null)
                          .ToString();

                // =========================================
                // VALIDASI HASIL
                // =========================================
                if (
                    string.IsNullOrEmpty(visitNo) ||
                    string.IsNullOrEmpty(visitQueueNo)
                )
                {
                    throw new Exception(
                        "Nomor antrian gagal dibuat"
                    );
                }

                // =========================================
                // RESPONSE
                // =========================================
                var now = DateTime.Now;

                var response = new
                {
                    VisitNo = visitNo,

                    VisitQueueNo = visitQueueNo,

                    QueueDate =
                        now.ToString("yyyy-MM-dd"),

                    QueueTime =
                        now.ToString("HH:mm:ss"),

                    ServiceGroup =
                        PayerType == "BPJS"
                            ? ServiceGroup
                            : null
                };

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    response,
                    "Berhasil mengambil nomor antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //3. Display Antrian Pasien
        [WebMethod(Description = @"
            Ambil Display Pasien Pendaftaran

            PARAMETER:
            - QueueDate (optional)
            - QueueLocation (required)
            - Status (Optional)

            CONTOH:
            GetDisplayAntrianPasien?
            QueueDate=2026-05-13&
            QueueLocation=LOKET_PD&Status=WAITING
            
            KETERANGAN:
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)
               - QueueLocation : Lokasi/channel antrian
               - Status : Status Antrian Pasien (WAITING, CALLED, PENDING, FINISHED)

            RESPONSE:
               200 = Berhasil mengambil display antrian pasien pendaftaran
               400 = Parameter request tidak valid (QueueLocation wajib diisi / QueueLocation tidak ditemukan)
               500 = Terjadi kesalahan pada server / Data display tidak ditemukan
            
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianPasienPendaftaran()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string QueueDate =
                    (Context.Request["QueueDate"] ?? "")
                    .Trim();

                string QueueLocation =
                    (Context.Request["QueueLocation"] ?? "")
                    .Trim()
                    .ToUpper();

                string Status =
                    (Context.Request["Status"] ?? "")
                    .Trim()
                    .ToUpper();

                // =========================================
                // DEFAULT DATE
                // =========================================
                DateTime queueDate;

                if (!DateTime.TryParse(QueueDate, out queueDate))
                {
                    queueDate = DateTime.Now.Date;
                }

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(QueueLocation))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "QueueLocation wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // VALIDASI CHANNEL
                // =========================================
                var allowedLocation =
                    new AntrianAutoNumberSemanticCollection();

                allowedLocation.Query.Load();

                var listChannel = allowedLocation
                    .Where(x => !string.IsNullOrEmpty(x.Channel))
                    .Select(x => x.Channel.ToUpper())
                    .Distinct()
                    .ToList();

                if (!listChannel.Contains(QueueLocation))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "QueueLocation tidak ditemukan",
                        400
                    );
                    return;
                }

                if (!string.IsNullOrEmpty(Status))
                {
                    var allowedStatus = new[]
                    {
                        "WAITING",
                        "CALLED",
                        "PENDING",
                        "FINISHED"
                    };

                    var statusList = Status
                        .Split(',')
                        .Select(x => x.Trim().ToUpper())
                        .Where(x => !string.IsNullOrEmpty(x))
                        .ToList();

                    if (statusList.Any(x => !allowedStatus.Contains(x)))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            "Status tidak valid",
                            400
                        );
                        return;
                    }
                }

                // =========================================
                // GET DATA FROM BO
                // =========================================
                var data =
                    VisitQueue.GetDisplayAntrianPasien(
                        queueDate,
                        QueueLocation,
                        Status
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil mengambil Display Antrian Pasien Pendaftaran"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Ambil Display Antrian khusus pegawai Pendaftaran

            PARAMETER:
            - QueueDate (optional)
            - Status (optional)
            - SRAutoNumber (optional)
            - CurrentStage (optional, default: LOKET)
            - QueueLocation (optional)

            EXAMPLE:
            GetDisplayAntrianPendaftaran?
            QueueDate=2026-05-13&
            Status=WAITING&
            SRAutoNumber=BPJS&
            CurrentStage=LOKET&
            QueueLocation=LOKET_PD

            KETERANGAN:
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)
               - Status : Filter status antrian
               - SRAutoNumber : Filter jenis nomor antrian
               - CurrentStage : Tahapan antrian saat ini
               - QueueLocation : Lokasi/channel antrian

            RESPONSE:
               200 = Berhasil mengambil display antrian pendaftaran
               400 = Parameter request tidak valid (QueueLocation tidak ditemukan)
               500 = Terjadi kesalahan pada server

         ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianPendaftaran()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string QueueDate =
                    (Context.Request["QueueDate"] ?? "")
                    .Trim();

                string Status =
                    (Context.Request["Status"] ?? "")
                    .Trim()
                    .ToUpper();

                string SRAutoNumber =
                    (Context.Request["SRAutoNumber"] ?? "")
                    .Trim()
                    .ToUpper();

                string CurrentStage =
                    (Context.Request["CurrentStage"] ?? "")
                    .Trim()
                    .ToUpper();

                string QueueLocation =
                    (Context.Request["QueueLocation"] ?? "")
                    .Trim()
                    .ToUpper();

                // =========================================
                // DEFAULT DATE
                // =========================================
                DateTime queueDate;

                if (!DateTime.TryParse(QueueDate, out queueDate))
                {
                    queueDate = DateTime.Now.Date;
                }

                // =========================================
                // VALIDASI QueueLocation
                // HANYA JIKA DIKIRIM
                // =========================================
                if (!string.IsNullOrEmpty(QueueLocation))
                {
                    var allowedLocation =
                        new AntrianAutoNumberSemanticCollection();

                    allowedLocation.Query.Load();

                    var listChannel = allowedLocation
                        .Where(x => !string.IsNullOrEmpty(x.Channel))
                        .Select(x => x.Channel.ToUpper())
                        .Distinct()
                        .ToList();

                    if (!listChannel.Contains(QueueLocation))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            "QueueLocation tidak ditemukan",
                            400
                        );
                        return;
                    }
                }

                // =========================================
                // GET DATA FROM BO
                // =========================================
                var data =
                    VisitQueue.GetDisplayAntrianPendaftaran(
                        queueDate,
                        Status,
                        SRAutoNumber,
                        CurrentStage,
                        QueueLocation
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil mengambil Display Antrian Pendaftaran"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //4. Panggil Antrian di Pendaftaran
        [WebMethod(Description = @"
            Digunakan untuk memanggil nomor antrian pasien pada loket pendaftaran

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - CounterID (required)

            EXAMPLE:
            CallAntrianSekarangPendaftaran?
            VisitQueueNo=VQUE-260513-0001&
            UserID=240076&
            CounterID=1

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien
               - UserID : User petugas yang memanggil antrian
               - CounterID : Nomor loket/counter pelayanan

            RESPONSE:
               200 = Antrian berhasil dipanggil
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi / CounterID tidak valid / tidak terdaftar)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CallAntrianSekarangPendaftaran()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string CounterID =
                    (Context.Request["CounterID"] ?? "")
                    .Trim()
                    .ToUpper();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(CounterID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "CounterID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // VALIDASI COUNTER
                // =========================================
                var validCounters = GetValidCounterIDs();

                if (!validCounters.Contains(CounterID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "CounterID tidak valid / tidak terdaftar",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var data =
                    VisitQueue
                        .CallAntrianSekarangPendaftaran(
                            VisitQueueNo,
                            UserID,
                            CounterID
                        );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Antrian berhasil dipanggil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memanggil ulang nomor antrian pasien pada loket pendaftaran.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            RecallAntrianPendaftaran?
            VisitQueueNo=VQUE-260513-0001&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien
               - UserID : User petugas yang melakukan recall antrian

            RESPONSE:
               200 = Antrian berhasil di-recall
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RecallAntrianPendaftaran()
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================
                // VALIDASI
                // =========================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================
                // EXEC BO
                // =========================
                var data =
                    VisitQueue.RecallAntrianPendaftaran(
                        VisitQueueNo,
                        UserID
                    );

                // =========================
                // RESPONSE
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Antrian berhasil di-recall"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengubah status antrian pasien menjadi pending pada loket pendaftaran.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            PendingAntrianPendaftaran?
            VisitQueueNo=VQUE-260513-0001&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien
               - UserID : User petugas yang melakukan pending antrian

            RESPONSE:
               200 = Antrian berhasil di-pending
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void PendingAntrianPendaftaran()
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================
                // VALIDASI
                // =========================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================
                // EXEC BO
                // =========================
                var data =
                    VisitQueue.PendingAntrianPendaftaran(
                        VisitQueueNo,
                        UserID
                    );

                // =========================
                // RESPONSE
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Antrian berhasil di-pending"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengubah status antrian pasien dari PENDING kembali menjadi WAITING pada loket pendaftaran.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            WaitingFromPendingStatusPendaftaran?
            VisitQueueNo=VQUE-260513-0001&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien
               - UserID : User petugas yang mengubah status antrian

            RESPONSE:
               200 = Antrian berhasil dikembalikan ke WAITING
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server

        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WaitingFromPendingStatusPendaftaran()
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================
                // VALIDASI
                // =========================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================
                // EXEC BO
                // =========================
                var data =
                    VisitQueue.WaitingFromPendingStatusPendaftaran(
                        VisitQueueNo,
                        UserID
                    );

                // =========================
                // SUCCESS
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Antrian berhasil dikembalikan ke WAITING"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memanggil nomor antrian berikutnya pada loket pendaftaran berdasarkan lokasi antrian dan counter petugas.

            PARAMETER:
            - QueueLocation (required)
            - UserID (required)
            - CounterID (required)
            - QueueDate (optional)

            EXAMPLE:
            NextAntrianPendaftaran?
            QueueLocation=LOKET_PD&
            UserID=240076&
            CounterID=1&
            QueueDate=2026-05-13

            KETERANGAN:
               - QueueLocation : Lokasi/channel antrian
               - UserID : User petugas yang memanggil antrian
               - CounterID : Nomor loket/counter pelayanan
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)

            RESPONSE:
               200 = Berhasil memanggil antrian berikutnya / Tidak ada antrian yang bisa dipanggil
               400 = Parameter request tidak valid (QueueLocation, UserID dan CounterID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void NextAntrianPendaftaran()
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                string QueueLocation =
                    (Context.Request["QueueLocation"] ?? "")
                    .Trim()
                    .ToUpper();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string CounterID =
                    (Context.Request["CounterID"] ?? "")
                    .Trim()
                    .ToUpper();

                string QueueDate =
                    (Context.Request["QueueDate"] ?? "")
                    .Trim();

                // =========================
                // DEFAULT DATE
                // =========================

                DateTime queueDateParsed;

                if (!DateTime.TryParse(QueueDate, out queueDateParsed))
                {
                    queueDateParsed = DateTime.Now.Date;
                }

                // =========================
                // VALIDASI
                // =========================
                if (string.IsNullOrEmpty(QueueLocation) ||
                    string.IsNullOrEmpty(UserID) ||
                    string.IsNullOrEmpty(CounterID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "QueueLocation, UserID dan CounterID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================
                // EXEC BO
                // =========================
                var data =
                    VisitQueue.NextAntrianPendaftaran(
                        QueueLocation,
                        UserID,
                        CounterID,
                        queueDateParsed
                    );

                // =========================
                // NO DATA
                // =========================
                if (data == null)
                {
                    ApiResponeForAntrian.Success(
                        Context,
                        null,
                        "Tidak ada antrian yang bisa dipanggil"
                    );
                    return;
                }

                // =========================
                // SUCCESS
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil memanggil antrian berikutnya"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk membatalkan antrian pendaftaran.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            CONTOH:
            CancelAntrian?
            VisitQueueNo=VQUE-260421-0010&
            UserID=admin

            RESPONSE:
               200 = Berhasil membatalkan antrian
               400 = Parameter tidak valid
               404 = Data tidak ditemukan
               500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CancelAntrianPendaftaran()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.SetCanceledPendaftaran(
                            VisitQueueNo,
                            UserID
                        );
                }
                catch (Exception ex)
                {
                    if (
                        ex.Message.ToUpper().Contains("TIDAK DITEMUKAN")
                    )
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // RESULT CHECK
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil membatalkan antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memunculkan suara berdasarkan VisitQueueNo.

            PARAMETER:
            - VisitQueueNo (required)

            EXAMPLE:
            GetQueueSoundPendaftaran?
            VisitQueueNo = 'VQUE-260518-0008'

            KETERANGAN:
               - VisitQueueNo = Nomor Antrian yang akan di convert menjadi suara
               - Contoh Test suara MP3 ada di link dev http://10.200.200.185/dev/Audio/nomor-urut.mp3

            RESPONSE:
               200 = Sound antrian berhasil diambil
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetQueueSoundPendaftaran()
        {
            try
            {
                // =========================
                // CORS
                // =========================
                Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                Context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                Context.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

                // Handle Preflight Request
                if (Context.Request.HttpMethod.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
                {
                    Context.Response.StatusCode = 200;
                    Context.Response.End();
                    return;
                }

                // =========================
                // NORMALIZE
                // =========================
                string VisitQueueNo =
                   (Context.Request["VisitQueueNo"] ?? "")
                   .Trim()
                   .ToUpper();

                // =========================
                // VALIDASI
                // =========================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                // =========================
                // EXEC BO
                // =========================
                var data =
                    QueueingSound.GetQueueSoundPendaftaran(
                        VisitQueueNo
                    );

                // =========================
                // RESPONSE
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Sound antrian berhasil diambil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengambil daftar counter/loket pendaftaran yang tersedia.

            KETERANGAN:
               - API akan mengembalikan daftar CounterID yang dapat digunakan pada proses pemanggilan antrian.

            RESPONSE:
                200 = List counter pendaftaran berhasil diambil
                500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetCounterIDList()
        {
            try
            {
                var data = GetCounterList();

                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "List counter pendaftaran berhasil diambil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //5.Get Display Semua Service Unit
        [WebMethod(Description = @"
            Digunakan untuk mengambil daftar antrian pasien berdasarkan stage/service unit tertentu.

            PARAMETER:
            - Status
            - ServiceUnitID
            - ParamedicID
            - QueueDate
            - StageID
            - CategoryID (untuk antrian FARMASI)
            
            DAFTAR STAGEID:
            - FARMASI_AMBIL
            - FARMASI_VERIF
            - LAB_SAMPLE
            - LAB_VERIF
            - LOKET
            - POLI
            - REHAB_TINDAKAN
            - USG_TINDAKAN
            - USG_VERIF
            - RADIOLOGI_VERIF
            - RADIOLOGI_AMBIL
            - CTSCAN_VERIF
            - CTSCAN_AMBIL
            - ENDOSCOPY_VERIF
            - ENDOSCOPY_AMBIL

            DAFTAR CATEGORYID :
            -FARMASI_A
            -FARMASI_B
            -FARMASI_C

            CONTOH REQUEST:
            GetDisplayAntrianForAllServiceUnitPasien?
            Status=WAITING&
            StageID=POLI&
            ServiceUnitID=D1.0.01&
            ParamedicID=DR001&
            QueueDate=2026-05-13

            KETERANGAN:
               - Status : Filter status antrian
               - StageID : Tahapan antrian/service
               - ServiceUnitID : Filter unit pelayanan
               - ParamedicID : Filter dokter/paramedis
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)

            RESPONSE:
               200 = Berhasil mengambil data antrian service unit pasien
               400 = Parameter request tidak valid
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianForAllServiceUnitPasien()
        {
            try
            {
                // =========================
                // NORMALIZE INPUT
                // =========================
                string Status =
                    (Context.Request["Status"] ?? "").Trim().ToUpper();

                string CategoryID =
                    (Context.Request["CategoryID"]);

                string StageID =
                    (Context.Request["StageID"] ?? "").Trim().ToUpper();

                string ServiceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "").Trim();

                string ParamedicID =
                    (Context.Request["ParamedicID"] ?? "").Trim();

                string QueueDateStr =
                    (Context.Request["QueueDate"] ?? "").Trim();

                // =========================
                // PARSE DATE
                // =========================
                DateTime queueDate;

                if (
                    !string.IsNullOrWhiteSpace(QueueDateStr)
                    && DateTime.TryParse(QueueDateStr, out queueDate)
                )
                {
                    // pakai tanggal input user
                }
                else
                {
                    // default hari ini
                    queueDate = DateTime.Now;
                }

                // =========================
                // CALL BUSINESS OBJECT
                // =========================
                var data =
                    VisitQueue.GetQueueForAllServieUnitPasien(
                        queueDate,
                        Status,
                        StageID,
                        ServiceUnitID,
                        ParamedicID,
                        CategoryID
                    );

                // =========================
                // RESPONSE
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil mengambil data antrian service unit pasien"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(Context, ex.Message, 500);
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengambil daftar antrian pasien berdasarkan stage/service unit tertentu untuk kebutuhan monitoring admin atau pegawai.

            PARAMETER:
            - Status
            - ServiceUnitID
            - ParamedicID
            - QueueDate
            - StageID

            DAFTAR STAGEID:
            - FARMASI_AMBIL
            - FARMASI_VERIF
            - LAB_SAMPLE
            - LAB_VERIF
            - LOKET
            - POLI
            - REHAB_TINDAKAN
            - USG_TINDAKAN
            - USG_VERIF

            CONTOH REQUEST:
            GetDisplayAntrianForAllServiceUnitAdmin?
            Status=WAITING&
            StageID=POLI&
            ServiceUnitID=D1.0.01&
            ParamedicID=DR001&
            QueueDate=2026-05-13

            KETERANGAN:
               - Status : Filter status antrian
               - StageID : Tahapan antrian/service
               - ServiceUnitID : Filter unit pelayanan
               - ParamedicID : Filter dokter/paramedis
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)

            RESPONSE:
               200 = Berhasil mengambil data antrian service unit admin atau pegawai
               400 = Parameter request tidak valid
               500 = Terjadi kesalahan pada server

        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianForAllServiceUnitAdmin()
        {
            try
            {
                // =========================
                // NORMALIZE INPUT
                // =========================
                string Status =
                    (Context.Request["Status"] ?? "").Trim().ToUpper();

                string StageID =
                    (Context.Request["StageID"] ?? "").Trim().ToUpper();

                string CategoryID =
                    (Context.Request["CategoryID"] ?? "").Trim().ToUpper();

                string ServiceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "").Trim();

                string ParamedicID =
                    (Context.Request["ParamedicID"] ?? "").Trim();

                string QueueDateStr =
                    (Context.Request["QueueDate"] ?? "").Trim();

                // =========================
                // PARSE DATE
                // =========================
                DateTime queueDate;

                if (
                    !string.IsNullOrWhiteSpace(QueueDateStr)
                    && DateTime.TryParse(QueueDateStr, out queueDate)
                )
                {
                    // pakai tanggal input user
                }
                else
                {
                    // default hari ini
                    queueDate = DateTime.Now;
                }

                // =========================
                // CALL BUSINESS OBJECT
                // =========================
                var data =
                    VisitQueue.GetQueueForAllServieUnitAdmin(
                        queueDate,
                        Status,
                        StageID,
                        ServiceUnitID,
                        ParamedicID,
                        CategoryID
                    );

                // =========================
                // RESPONSE
                // =========================
                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil mengambil data antrian service unit admin atau pegawai"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(Context, ex.Message, 500);
            }
        }

        //6.Edit atau Move Antrian
        [WebMethod(Description = @"
            Digunakan untuk memindahkan posisi antrian pasien ke urutan bawah dalam daftar antrian.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            MoveQueueDown?
            VisitQueueNo=VQUE-260516-0001&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien yang akan dipindahkan
               - UserID : User petugas yang melakukan perubahan antrian

            RESPONSE:
               200 = Berhasil memindahkan antrian ke bawah
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server

        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveQueueDown()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.MoveQueueDown(
                        VisitQueueNo,
                        UserID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan antrian ke bawah"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memindahkan posisi antrian pasien ke urutan atas dalam daftar antrian.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            MoveQueueUp?
            VisitQueueNo=VQUE-260516-0002&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien yang akan dipindahkan
               - UserID : User petugas yang melakukan perubahan antrian

            RESPONSE:
               200 = Berhasil memindahkan antrian ke atas
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server

        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveQueueUp()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.MoveQueueUp(
                        VisitQueueNo,
                        UserID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan antrian ke atas"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memindahkan posisi antrian pasien langsung ke urutan paling atas pada daftar antrian.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            MoveQueueToTop?
            VisitQueueNo=VQUE-260516-0011&
            UserID=240076
            
            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien yang akan dipindahkan
               - UserID : User petugas yang melakukan perubahan antrian

            RESPONSE:
               200 = Berhasil memindahkan antrian ke paling atas
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveQueueToTop()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.MoveQueueToTop(
                        VisitQueueNo,
                        UserID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan antrian ke paling atas"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memindahkan posisi antrian pasien langsung ke urutan paling bawah pada daftar antrian.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            MoveQueueToBottom?
            VisitQueueNo=VQUE-260516-0001&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian pasien yang akan dipindahkan
               - UserID : User petugas yang melakukan perubahan antrian

            RESPONSE:
               200 = Berhasil memindahkan antrian ke paling bawah
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / UserID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveQueueToBottom()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.MoveQueueToBottom(
                        VisitQueueNo,
                        UserID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan antrian ke paling bawah"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memindahkan posisi antrian pasien sebelum atau sesudah antrian target menggunakan metode drag & drop.

            PARAMETER:
            - VisitQueueNo (required)
            - TargetVisitQueueNo (required)
            - Position (required: BEFORE / AFTER)
            - UserID (required)

            EXAMPLE:
            MoveQueueDragDrop?
            VisitQueueNo=VQUE-260516-0001&
            TargetVisitQueueNo=VQUE-260516-0005&
            Position=BEFORE&
            UserID=240076

            KETERANGAN:
               - VisitQueueNo : Nomor antrian yang akan dipindahkan
               - TargetVisitQueueNo : Nomor antrian target tujuan
               - Position : Posisi penempatan antrian (BEFORE / AFTER)
               - UserID : User petugas yang melakukan perubahan antrian

            RESPONSE:
               200 = Berhasil memindahkan posisi antrian
               400 = Parameter request tidak valid (VisitQueueNo wajib diisi / TargetVisitQueueNo wajib diisi / Position wajib diisi / Position hanya boleh BEFORE atau AFTER / UserID wajib diisi)
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveQueueDragDrop()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string TargetVisitQueueNo =
                    (Context.Request["TargetVisitQueueNo"] ?? "")
                    .Trim();

                string Position =
                    (Context.Request["Position"] ?? "")
                    .Trim()
                    .ToUpper();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(TargetVisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "TargetVisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(Position))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Position wajib diisi",
                        400
                    );
                    return;
                }

                if (
                    Position != "BEFORE" &&
                    Position != "AFTER"
                )
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Position hanya boleh BEFORE atau AFTER",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.MoveQueueDragDrop(
                        VisitQueueNo,
                        TargetVisitQueueNo,
                        Position,
                        UserID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan posisi antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengatur ulang urutan antrian WAITING
            berdasarkan prioritas SRAutoNumber.

            PARAMETER:
            - SRAutoNumber (required)
            - UserID (required)
            - QueueDate (optional)
            
            EXAMPLE:
            ReorderBySRAutoNumber?
            SRAutoNumber=VisitTunaiNo&
            UserID=240076

            ReorderBySRAutoNumber?
            SRAutoNumber=VisitBpjsPoliNo&
            QueueDate=2026-06-17&
            UserID=240076

            RESPONSE:
               200 = Berhasil melakukan reorder antrian
               400 = Parameter request tidak valid
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void ReorderBySRAutoNumber()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string srAutoNumber =
                    (Context.Request["SRAutoNumber"] ?? "")
                    .Trim();

                string userID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                DateTime queueDate =
                    DateTime.Today;

                if (!string.IsNullOrWhiteSpace(
                    Context.Request["QueueDate"]))
                {
                    DateTime.TryParse(
                        Context.Request["QueueDate"],
                        out queueDate
                    );
                }

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(srAutoNumber))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "SRAutoNumber wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(userID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                var result =
                    VisitQueue.ReorderBySRAutoNumber(
                        srAutoNumber,
                        queueDate,
                        userID
                    );

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil melakukan reorder antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //7. CALL, RECALL, PENDING All Service Unit
        [WebMethod(Description = @"
            Digunakan untuk memanggil antrian pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - KamarCode (optional)

            EXAMPLE:
            CallAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0015&
            UserID=Admin&
            KamarCode=Kamar_5

            RESPONSE:
               200 = Berhasil memanggil antrian
               400 = Parameter request tidak valid
               404 = Data antrian tidak ditemukan
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CallAntrianAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string KamarCode =
                    (Context.Request["KamarCode"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.CallAntrianAllServiceUnit(
                            VisitQueueNo,
                            UserID,
                            KamarCode
                        );

                    if (result == null)
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            "Data antrian tidak ditemukan",
                            404
                        );
                        return;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToUpper().Contains("TIDAK DITEMUKAN"))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Antrian berhasil dipanggil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memunculkan suara antrian berdasarkan
            Service Unit (Poli, Farmasi, Radiologi, Lab, Rehab, USG, dll).

            PARAMETER:
            - VisitQueueNo (required)
            - KamarCode (optional)

            EXAMPLE:

            GetQueueSoundForAllServiceUnit?
            VisitQueueNo=VQUE-260603-0006&
            KamarCode=Kamar_1

            RESPONSE:
               200 = Sound antrian All Service Unit berhasil diambil
               400 = Parameter request tidak valid
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetQueueSoundForAllServiceUnit()
        {
            try
            {
                // =========================
                // CORS
                // =========================
                Context.Response.ClearHeaders();
                Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
                Context.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
                Context.Response.AddHeader("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, Authorization");

                // Handle Preflight Request
                if (Context.Request.HttpMethod == "OPTIONS")
                {
                    Context.Response.StatusCode = 200;
                    Context.Response.End();
                    return;
                }

                // =========================
                // NORMALIZE
                // =========================

                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim()
                    .ToUpper();

                string KamarCode =
                    (Context.Request["KamarCode"] ?? "")
                    .Trim()
                    .ToUpper();

                // =========================
                // VALIDASI
                // =========================

                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );

                    return;
                }

                // =========================
                // EXECUTE BO
                // =========================

                var data =
                    QueueingSound.GetQueueSoundForAllServiceUnit(
                        VisitQueueNo,
                        KamarCode
                    );

                // =========================
                // RESPONSE
                // =========================

                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Sound antrian All Service Unit berhasil diambil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk recall antrian pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - KamarCode (optional)

            EXAMPLE:
            RecallAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0015&
            UserID=Admin&
            KamarCode=Kamar_5

            RESPONSE:
               200 = Berhasil recall antrian
               400 = Parameter request tidak valid
               404 = Data antrian tidak ditemukan
               500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RecallAntrianAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string KamarCode =
                    (Context.Request["KamarCode"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.RecallAntrianAllServiceUnit(
                            VisitQueueNo,
                            UserID,
                            KamarCode
                        );
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToUpper().Contains("TIDAK DITEMUKAN"))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // NO DATA
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Antrian berhasil di-recall"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk menyelesaikan antrian yang sedang CALLED
            dan otomatis memanggil antrian berikutnya pada Service Unit yang sama.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - KamarCode (optional)

            EXAMPLE:
            CallNextQueueAllServiceUnit?
            VisitQueueNo=VQUE-260520-0020&
            UserID=240076&
            KamarCode=Kamar_5

            RESPONSE:
               200 = Berhasil memanggil antrian berikutnya
               400 = Parameter tidak valid
               404 = Data antrian tidak ditemukan
               500 = Error server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CallNextQueueAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string KamarCode =
                    (Context.Request["KamarCode"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXEC BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.CallNextQueueAllServiceUnit(
                            VisitQueueNo,
                            UserID,
                            KamarCode
                        );
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToUpper().Contains("TIDAK DITEMUKAN"))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // NOT FOUND
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memanggil antrian berikutnya"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengubah status antrian menjadi PENDING pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            PendingAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0013&
            UserID=admin

            RESPONSE:
               200 = Berhasil ubah status ke PENDING
               400 = Parameter tidak valid
               404 = Data antrian tidak ditemukan
               500 = Error server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void PendingAntrianAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(Context,
                        "VisitQueueNo wajib diisi",
                        400);
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(Context,
                        "UserID wajib diisi",
                        400);
                    return;
                }

                // =========================================
                // EXEC BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.SetPendingAllServiceUnit(
                            VisitQueueNo,
                            UserID
                        );
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToUpper().Contains("TIDAK DITEMUKAN"))
                    {
                        ApiResponeForAntrian.Error(Context,
                            ex.Message,
                            404);
                        return;
                    }

                    throw;
                }

                // =========================================
                // NOT FOUND
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(Context,
                        "Data antrian tidak ditemukan",
                        404);
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil ubah status ke PENDING"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(Context, ex.Message, 500);
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengembalikan antrian dari PENDING ke WAITING
            berdasarkan ServiceUnit, Stage, dan Paramedic (All Service Unit Context)

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            CONTOH:
            WaitingFromPendingStatusAllServiceUnit?
            VisitQueueNo=VQUE-260516-0012&
            UserID=admin

            RESPONSE:
               200 = Berhasil dikembalikan ke WAITING
               400 = Parameter tidak valid
               404 = Data tidak ditemukan
               500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WaitingFromPendingStatusAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.SetWaitingFromPendingAllServiceUnit(
                            VisitQueueNo,
                            UserID
                        );
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToUpper().Contains("TIDAK DITEMUKAN"))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // RESULT CHECK
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil mengembalikan antrian ke WAITING"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk membatalkan antrian pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            CONTOH:
            CancelAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0013&
            UserID=admin

            RESPONSE:
               200 = Berhasil membatalkan antrian
               400 = Parameter tidak valid
               404 = Data tidak ditemukan
               500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CancelAntrianAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXECUTE BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.SetCanceledAllServiceUnit(
                            VisitQueueNo,
                            UserID
                        );
                }
                catch (Exception ex)
                {
                    if (
                        ex.Message.ToUpper().Contains("TIDAK DITEMUKAN")
                    )
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // RESULT CHECK
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil membatalkan antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //8. SCAN BARCODE PENUNJANG
        [WebMethod(Description = @"
            Digunakan untuk generate antrian penunjang medis
            berdasarkan barcode pasien dan lokasi scanner ServiceUnit

            PARAMETER:
            - RegistrationNo (required)
            - ServiceUnitID (required)
            - TransDate (optional)

            CONTOH:
            TakeQueueVisitNumberForPenunjang?
            RegistrationNo=REG/OP/260516-0008&
            ServiceUnitID=D3.0.04&
            UserID=KIOSK&
            TransDate=2026-05-19

            RESPONSE:
               200 = Berhasil generate antrian
               400 = Parameter tidak valid
               404 = Data tidak ditemukan / Job Order Tidak ditemukan
               500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void TakeQueueVisitNumberForPenunjang()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================

                string RegistrationNo =
                    (Context.Request["RegistrationNo"] ?? "")
                    .Trim();

                string ServiceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "KIOSK")
                    .Trim();

                string TransDateText =
                    (Context.Request["TransDate"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================

                if (string.IsNullOrEmpty(RegistrationNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "RegistrationNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(ServiceUnitID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "ServiceUnitID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // PARSE DATE
                // =========================================

                DateTime? TransDate = null;

                if (!string.IsNullOrEmpty(TransDateText))
                {
                    DateTime tempDate;

                    if (!DateTime.TryParse(TransDateText, out tempDate))
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            "Format TransDate tidak valid",
                            400
                        );
                        return;
                    }

                    TransDate = tempDate;
                }

                // =========================================
                // EXECUTE BO
                // =========================================

                object result = null;

                try
                {
                    result =
                        VisitQueue.TakeQueueVisitNumberForPenunjang(
                            RegistrationNo,
                            ServiceUnitID,
                            UserID,
                            TransDate
                        );
                }
                catch (Exception ex)
                {
                    if (
                        ex.Message.ToUpper().Contains("TIDAK DITEMUKAN")
                    )
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // RESULT CHECK
                // =========================================

                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================

                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil generate antrian penunjang"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //9. SCAN BARCODE FARMASI
        [WebMethod(Description = @"
            Digunakan untuk mengambil nomor antrian pada farmasi

            PARAMETER:
            - VisitQueueNo (required)
            - ServiceUnitID (required)
            - UserID (optional)
            - TransDate (optional)

            RESPONSE:
            200 = Berhasil mengambil antrian farmasi
            400 = Parameter tidak valid (VisitQueueNo wajib diisi / ServiceUnitID wajib diisi)
            500 = Server error
        ")]
        public void TakeQueueVisitNumberForFarmasi()
        {
            try
            {
                string visitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string serviceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                string userID =
                    (Context.Request["UserID"] ?? "KIOSK_FARMASI")
                    .Trim();

                string transDateString =
                    (Context.Request["TransDate"] ?? "")
                    .Trim();

                DateTime? transDate = null;

                if (!string.IsNullOrEmpty(transDateString))
                {
                    transDate = Convert.ToDateTime(transDateString);
                }

                // =========================================
                // VALIDASI
                // =========================================

                if (string.IsNullOrEmpty(visitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );

                    return;
                }

                if (string.IsNullOrEmpty(serviceUnitID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "ServiceUnitID wajib diisi",
                        400
                    );

                    return;
                }

                // =========================================
                // EXECUTE
                // =========================================

                var result =
                    VisitQueue.TakeQueueVisitNumberForFarmasi(
                        visitQueueNo,
                        serviceUnitID,
                        userID,
                        transDate
                    );

                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Gagal mengambil antrian farmasi",
                        500
                    );

                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================

                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil mengambil antrian farmasi"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk melihat list CategoryID dan petugas farmasi dapat
            menentukan Verifikasi Antrian Farmasi

            RESPONSE:
                200 = Berhasil mengambil list CategoryID Farmasi
                404 = Data CategoryID tidak ditemukan
                500 = Server error
        ")]
        public void GetCategoryIDFarmasi()
        {
            try
            {
                // =========================================
                // EXECUTE BO
                // =========================================

                object result =
                    QueueCategory.GetQueueCategoryFarmasi();

                // =========================================
                // VALIDASI RESULT
                // =========================================

                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        HttpContext.Current,
                        "Data CategoryID tidak ditemukan",
                        404
                    );

                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================

                ApiResponeForAntrian.Success(
                    HttpContext.Current,
                    result,
                    "Berhasil mengambil list CategoryID farmasi"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    HttpContext.Current,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk update CategoryID Farmasi
            pada antrian pasien farmasi

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - CategoryID (optional)

            CONTOH:
            GetCategoryIDFarmasi?
            VisitQueueNo=VQUE-260520-0007&
            UserID=240076&
            CategoryID=FARMASI_A

            RESPONSE:
                200 = Berhasil update CategoryID Farmasi
                400 = Parameter tidak valid
                404 = Data tidak ditemukan
                500 = Server error
        ")]
        public void UpdateCategoryIDFarmasi()
        {
            try
            {
                // =========================================
                // PARAMETER
                // =========================================

                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string CategoryID =
                    (Context.Request["CategoryID"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================

                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );

                    return;
                }

                if (string.IsNullOrEmpty(CategoryID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "CategoryID wajib diisi",
                        400
                    );

                    return;
                }

                // =========================================
                // VALIDASI CATEGORY
                // =========================================

                var category =
                    new QueueCategory();

                var categoryQuery =
                    new QueueCategoryQuery("qc");

                categoryQuery.Where(
                    categoryQuery.CategoryID == CategoryID
                    && categoryQuery.IsActive == true
                );

                if (!category.Load(categoryQuery))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "CategoryID tidak ditemukan",
                        404
                    );

                    return;
                }

                // =========================================
                // LOAD VISIT QUEUE
                // =========================================

                var entity = new VisitQueue();

                if (!entity.LoadByPrimaryKey(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );

                    return;
                }

                // =========================================
                // UPDATE
                // =========================================

                entity.CategoryID = CategoryID;
                entity.UpdatedBy = UserID;
                entity.LastUpdated = DateTime.Now;

                // =========================================
                // LOAD PATIENT
                // =========================================

                string PatientName = "";

                var patient = new Patient();

                if (!string.IsNullOrEmpty(entity.PatientID))
                {
                    if (patient.LoadByPrimaryKey(entity.PatientID))
                    {
                        PatientName =
                            patient.FirstName;
                    }
                }

                // =========================================
                // LOAD CATEGORY NAME
                // =========================================

                string CategoryName = "";

                var ctgname = new QueueCategory();

                if (!string.IsNullOrEmpty(entity.CategoryID))
                {
                    if (ctgname.LoadByPrimaryKey(entity.CategoryID))
                    {
                        CategoryName =
                            ctgname.CategoryName;
                    }
                }

                entity.Save();

                // =========================================
                // SUCCESS
                // =========================================

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        VisitQueueNo = entity.VisitQueueNo,
                        VisitNo = entity.VisitNo,
                        PatienID = entity.PatientID,
                        PatientName = PatientName,
                        RegistrationNo = entity.RegistrationNo,
                        CategoryID = entity.CategoryID,
                        CategoryName = CategoryName,
                        Status = entity.Status,
                        CurrentStage = entity.CurrentStage,
                        StageID = entity.StageID
                    },
                    "Berhasil update CategoryID farmasi"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //10. PRINT STRUK ANTRIAN (APM)
        [WebMethod(EnableSession = true, Description = @"
            Digunakan untuk print struk antrian pasien

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)
            - Barcode (Base64)
            - IPAdress (optional)

            CONTOH:
            PrintVisitQueueReceipt?
            VisitQueueNo=VQUE-260520-0007&
            UserID=240076&
            Barcode=iVBORw0KGgoAAAANSUhEUgAAAeoAAAHqCAYAAADLbQ06AAA2UklEQVR4nO3dCZwcd33n/W9&
            IPAdress=192.168.8.52

            RESPONSE:
                200 = Berhasil print struk antrian
                400 = Parameter tidak valid
                404 = Data tidak ditemukan / Printer tidak ditemukan untuk host ini
                500 = Server error
        ")]
        public void PrintVisitQueueReceipt()
        {
            try
            {
                // =========================================
                // PARAMETER
                // =========================================

                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string Barcode =
                    (Context.Request["Barcode"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                string IPAdress =
                    (Context.Request["IPAdress"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================

                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );

                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );

                    return;
                }

                // =========================================
                // LOAD VISIT QUEUE
                // =========================================

                var entity = new VisitQueue();

                if (!entity.LoadByPrimaryKey(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );

                    return;
                }

                // =========================================
                // SESSION LOGIN DUMMY ATAU PALSU
                // =========================================

                if (HttpContext.Current != null &&
                    HttpContext.Current.Session != null &&
                    HttpContext.Current.Session["_UserLogin"] == null)
                {
                    HttpContext.Current.Session["_UserLogin"] =
                        new UserLogin()
                        {
                            UserID = "WEBSERVICE",
                            UserName = "WEBSERVICE"
                        };
                }

                // =========================================
                // SAVE BARCODE IMAGE
                // =========================================

                if (!string.IsNullOrEmpty(Barcode))
                {
                    byte[] barcodeBytes =
                        Convert.FromBase64String(Barcode);

                    var visitQueueBarcode =
                        new VisitQueueBarcode();

                    visitQueueBarcode.AddNew();

                    visitQueueBarcode.VisitQueueNo =
                        VisitQueueNo;

                    visitQueueBarcode.BarcodeImage =
                        barcodeBytes;

                    visitQueueBarcode.CreatedDateTime =
                        DateTime.Now;

                    visitQueueBarcode.Save();
                }

                // =========================================
                // PRINT
                // =========================================

                const string programID = "STK.01.0001";

                var parametersSlip =
                    new PrintJobParameterCollection();

                parametersSlip.AddNew(
                    "VisitQueueNo",
                    VisitQueueNo,
                    null,
                    null
                );

                parametersSlip.AddNew(
                    "UserID",
                    UserID,
                    null,
                    null
                );

                parametersSlip.AddNew(
                    "IPAdress",
                    IPAdress,
                    null,
                    null
                );

                string printerName = "";

                try
                {
                    printerName =
                        PrintManager.CreatePrintJob(
                            programID,
                            parametersSlip,
                            UserID,
                            IPAdress
                        );
                }
                catch (Exception ex)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "PRINT ERROR : " + ex.Message,
                        500
                    );

                    return;
                }

                if (string.IsNullOrEmpty(printerName))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Printer tidak ditemukan untuk host ini",
                        404
                    );

                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        VisitQueueNo = entity.VisitQueueNo,
                        VisitNo = entity.VisitNo,
                        IPAdress = IPAdress,
                        PrintedBy = UserID,
                        PrinterName = printerName,
                        ProgramID = programID
                    },
                    "Berhasil print struk antrian"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //11. PRINT STRUK ANTRIAN UNTUK PETUGAS 
        [WebMethod(Description = @"
            Digunakan untuk mengambil daftar antrian untuk kebutuhan reprint oleh petugas.

            PARAMETER:
            - StartDate (required)
            - EndDate (required)
            - ServiceUnitID (optional)
            - VisitNo (optional)
            - RegistrationNo (optional)
            - Status (optional)

            RESPONSE:
            200 = Data antrian untuk print berhasil diambil
            400 = Parameter tidak valid
            500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetQueueReprintForPetugas()
        {
            try
            {
                string startDateString =
                    (Context.Request["StartDate"] ?? "")
                    .Trim();

                string endDateString =
                    (Context.Request["EndDate"] ?? "")
                    .Trim();

                string serviceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                string visitNo =
                    (Context.Request["VisitNo"] ?? "")
                    .Trim();

                string registrationNo =
                    (Context.Request["RegistrationNo"] ?? "")
                    .Trim();

                string status =
                    (Context.Request["Status"] ?? "")
                    .Trim();

                if (string.IsNullOrEmpty(startDateString))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "StartDate wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(endDateString))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "EndDate wajib diisi",
                        400
                    );
                    return;
                }

                DateTime startDate;
                DateTime endDate;

                if (!DateTime.TryParse(startDateString, out startDate))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Format StartDate tidak valid",
                        400
                    );
                    return;
                }

                if (!DateTime.TryParse(endDateString, out endDate))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Format EndDate tidak valid",
                        400
                    );
                    return;
                }

                var result =
                    VisitQueue.GetQueueReprintForPetugas(
                        startDate,
                        endDate,
                        string.IsNullOrWhiteSpace(serviceUnitID) ? null : serviceUnitID,
                        string.IsNullOrWhiteSpace(visitNo) ? null : visitNo,
                        string.IsNullOrWhiteSpace(registrationNo) ? null : registrationNo,
                        string.IsNullOrWhiteSpace(status) ? null : status
                    );

                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Data antrian untuk print berhasil diambil"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        //12.Master Queue Stage
        [WebMethod(Description = @"
            Mengambil data master Queue Stage.

            PARAMETER (Optional):
            - StageID
            - ServiceGroup
            - IsActive
            - ServiceUnitID

            EXAMPLE:
            GetQueueStage

            GetQueueStage?ServiceGroup=LAB

            GetQueueStage?StageID=POLI

            GetQueueStage?IsActive=1

            GetQueueStage?ServiceUnitID=D2.2.41.1

            GetQueueStage?ServiceUnitID=D3.0.02

            GetQueueStage?ServiceGroup=USG&IsActive=1

            RESPONSE:
            200 = Berhasil mengambil data Queue Stage
            500 = Server error
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetQueueStage()
        {
            try
            {
                string StageID =
                    (Context.Request["StageID"] ?? "")
                    .Trim();

                string ServiceGroup =
                    (Context.Request["ServiceGroup"] ?? "")
                    .Trim();

                string IsActive =
                    (Context.Request["IsActive"] ?? "")
                    .Trim();

                string ServiceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                var result =
                    QueueStage.GetQueueStage(
                        string.IsNullOrEmpty(StageID)
                            ? null
                            : StageID,

                        string.IsNullOrEmpty(ServiceGroup)
                            ? null
                            : ServiceGroup,

                        string.IsNullOrEmpty(IsActive)
                            ? null
                            : IsActive,

                        string.IsNullOrEmpty(ServiceUnitID)
                            ? null
                            : ServiceUnitID
                    );

                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil mengambil data Queue Stage"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengambil daftar dokter yang aktif ditampilkan
            pada display antrian berdasarkan satu atau beberapa Service Unit.
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetListUpdateDisplayDokterForPoli()
        {
            try
            {
                List<string> serviceUnitID = new List<string>();
                DateTime? queueDate = null;

                // ===========================
                // PRIORITAS 1 : FORM / QUERY
                // ===========================
                string serviceUnitIDsText =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                if (!string.IsNullOrWhiteSpace(serviceUnitIDsText))
                {
                    serviceUnitID = serviceUnitIDsText
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    DateTime dt;
                    if (DateTime.TryParse(Context.Request["QueueDate"], out dt))
                    {
                        queueDate = dt;
                    }
                }
                else
                {
                    // ===========================
                    // PRIORITAS 2 : RAW JSON BODY
                    // ===========================
                    Context.Request.InputStream.Position = 0;

                    using (var reader = new StreamReader(Context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            var request =
                                JsonConvert.DeserializeObject<GetDisplayDoctorListRequest>(body);

                            if (request != null)
                            {
                                serviceUnitID = request.ServiceUnitID ?? new List<string>();
                                queueDate = request.QueueDate;
                            }
                        }
                    }
                }

                if (serviceUnitID.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "ServiceUnitIDs wajib diisi",
                        400
                    );
                    return;
                }

                var data =
                    VisitQueue.GetDisplayDoctorListForPoli(
                        serviceUnitID,
                        queueDate
                    );

                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil mengambil daftar dokter"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Mengambil daftar SRAutoNumber berdasarkan filter.

            PARAMETER (OPTIONAL):
            - PayerType
            - ServiceGroup
            - Channel

            EXAMPLE:
            GetSRAutoNumberList
            GetSRAutoNumberList?PayerType=BPJS
            GetSRAutoNumberList?ServiceGroup=POLI&Channel=LOKET_PD
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetSRAutoNumberList()
        {
            try
            {
                // =========================================
                // NORMALIZE INPUT
                // =========================================
                string payerType =
                    (Context.Request["PayerType"] ?? "").Trim();

                string serviceGroup =
                    (Context.Request["ServiceGroup"] ?? "").Trim();

                string channel =
                    (Context.Request["Channel"] ?? "").Trim();

                // =========================================
                // EXEC BO
                // =========================================
                var result =
                    AntrianAutoNumberSemantic.GetSRAutoNumberList(
                        payerType,
                        serviceGroup,
                        channel
                    );

                // =========================================
                // RESPONSE SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil mengambil daftar SRAutoNumber"
                );
            }
            catch (Exception ex)
            {
                // =========================================
                // RESPONSE ERROR
                // =========================================
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(EnableSession = false, Description = @"
            Mendapatkan daftar kamar untuk antrian.

            PARAMETER:
            - Tidak ada

            RESPONSE:
             200 = Berhasil mendapatkan data kamar
             404 = Data kamar tidak ditemukan
             500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetListKamarForPoli()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("===== GET ROOM LIST =====");

                ListKamarForAntrianCollection collection =
                    new ListKamarForAntrianCollection();

                collection.Query.Where(
                    collection.Query.IsActive == true
                );

                collection.Query.OrderBy(
                    collection.Query.KamarID.Ascending
                );

                collection.Query.Load();

                System.Diagnostics.Debug.WriteLine(
                    $"Total Room = {collection.Count}"
                );

                if (collection.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data kamar tidak ditemukan",
                        404
                    );
                    return;
                }

                var data = collection
                    .Select(x => new
                    {
                        KamarID = x.KamarID,
                        KamarCode = x.KamarCode,
                        KamarName = x.KamarName
                    })
                    .ToList();

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        Rooms = data
                    }
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "ERROR : " + ex.ToString()
                );

                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk memindahkan antrian ke Stage berikutnya.
            Data antrian lama akan diubah menjadi FINISHED
            dan otomatis membuat antrian baru pada Stage berikutnya.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            MoveNextStageAllServiceUnit?
            VisitQueueNo=VQUE-260713-0010&
            UserID=240092

            RESPONSE:
               200 = Berhasil memindahkan antrian ke stage berikutnya
               400 = Parameter tidak valid
               404 = Data antrian tidak ditemukan
               500 = Error server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void MoveNextStageAllServiceUnit()
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                string VisitQueueNo =
                    (Context.Request["VisitQueueNo"] ?? "")
                    .Trim();

                string UserID =
                    (Context.Request["UserID"] ?? "")
                    .Trim();

                // =========================================
                // VALIDASI
                // =========================================
                if (string.IsNullOrEmpty(VisitQueueNo))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "VisitQueueNo wajib diisi",
                        400
                    );
                    return;
                }

                if (string.IsNullOrEmpty(UserID))
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "UserID wajib diisi",
                        400
                    );
                    return;
                }

                // =========================================
                // EXEC BO
                // =========================================
                object result = null;

                try
                {
                    result =
                        VisitQueue.MoveNextStage(
                            VisitQueueNo,
                            UserID
                        );
                }
                catch (Exception ex)
                {
                    if (
                        ex.Message.ToUpper().Contains("TIDAK DITEMUKAN")
                    )
                    {
                        ApiResponeForAntrian.Error(
                            Context,
                            ex.Message,
                            404
                        );
                        return;
                    }

                    throw;
                }

                // =========================================
                // NOT FOUND
                // =========================================
                if (result == null)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data antrian tidak ditemukan",
                        404
                    );
                    return;
                }

                // =========================================
                // SUCCESS
                // =========================================
                ApiResponeForAntrian.Success(
                    Context,
                    result,
                    "Berhasil memindahkan antrian ke stage berikutnya"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(Description = @"
            Digunakan untuk mengatur dokter yang akan ditampilkan pada display antrian.
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void UpdateDisplayDoctorListForPoli()
        {
            try
            {
                string serviceUnitID = "";
                List<DisplayDoctorItem> doctors = new List<DisplayDoctorItem>();

                // ===========================
                // PRIORITAS 1 : FORM / QUERY
                // ===========================
                serviceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                string doctorsJson =
                    (Context.Request["Doctors"] ?? "")
                    .Trim();

                if (!string.IsNullOrWhiteSpace(doctorsJson))
                {
                    doctors =
                        JsonConvert.DeserializeObject<List<DisplayDoctorItem>>(doctorsJson);
                }
                else
                {
                    // ===========================
                    // PRIORITAS 2 : RAW JSON BODY
                    // ===========================
                    Context.Request.InputStream.Position = 0;

                    using (var reader = new StreamReader(Context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();

                        if (!string.IsNullOrWhiteSpace(body))
                        {
                            var request =
                                JsonConvert.DeserializeObject<UpdateDisplayDoctorRequest>(body);

                            if (request != null)
                            {
                                serviceUnitID = request.ServiceUnitID;
                                doctors = request.Doctors ?? new List<DisplayDoctorItem>();
                            }
                        }
                    }
                }

                VisitQueue.UpdateDisplayDoctorList(
                    serviceUnitID,
                    doctors
                );

                var data =
                    VisitQueue.GetDisplayDoctorListForPoli(
                        new List<string>
                        {
                        serviceUnitID
                        }
                    );

                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Berhasil update dokter display"
                );
                return;
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(EnableSession = false, Description = @"
            Mendapatkan daftar Service Unit Farmasi.

            PARAMETER:
            - ServiceUnitID (Optional)

            RESPONSE:
             200 = Berhasil mendapatkan data Service Unit Farmasi
             404 = Data Service Unit Farmasi tidak ditemukan
             500 = Terjadi kesalahan pada server
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetListServiceUnitFarmasi()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("===== GET SERVICE UNIT FARMASI =====");

                string serviceUnitID =
                    (Context.Request["ServiceUnitID"] ?? "")
                    .Trim();

                ServiceUnitCollection collection =
                    new ServiceUnitCollection();

                collection.Query.Where(
                    collection.Query.ShortName.Like("%FAR%")
                );

                if (!string.IsNullOrWhiteSpace(serviceUnitID))
                {
                    collection.Query.Where(
                        collection.Query.ServiceUnitID == serviceUnitID
                    );
                }

                collection.Query.OrderBy(
                    collection.Query.ServiceUnitName.Ascending
                );

                collection.Query.Load();

                System.Diagnostics.Debug.WriteLine(
                    $"Total Farmasi = {collection.Count}"
                );

                if (collection.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data Service Unit Farmasi tidak ditemukan",
                        404
                    );
                    return;
                }

                var data = collection
                    .Select(x => new
                    {
                        ServiceUnitCode = x.DepartmentID,
                        ServiceUnitID = x.ServiceUnitID,
                        ServiceUnitName = x.ServiceUnitName,
                        ShortName = x.ShortName
                    })
                    .ToList();

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        ServiceUnits = data
                    }
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    "ERROR : " + ex.ToString()
                );

                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

        [WebMethod(EnableSession = true, Description = @"
        Digunakan untuk menyimpan konfigurasi Dashboard Clinic.

        Method : POST

        Request Body (JSON)

        {
            ""ConfigID"": ""CFG-260723-0001"", // Optional. Kosongkan untuk membuat konfigurasi baru.
            ""UserID"": ""240092"",
            ""ConfigName"": ""Display Poli Anak Lantai 1"",
            ""Settings"": {
                ""AutoRefresh"": true,
                ""RefreshIntervalSec"": 5
            },
            ""Rooms"": [
                {
                    ""ServiceUnitID"": ""D2.2.03.2"",
                    ""StageID"": ""POLI"",
                    ""ParamedicID"": ""DR001"",
                    ""KamarID"": 1
                },
                {
                    ""ServiceUnitID"": ""D2.2.03.2"",
                    ""StageID"": ""POLI"",
                    ""ParamedicID"": ""DR002"",
                    ""KamarID"": 2
                }
            ]
        }

        Keterangan Parameter :

        ConfigID            : ID konfigurasi Dashboard Clinic.
                              - Kosong = Tambah konfigurasi baru.
                              - Diisi = Update konfigurasi yang sudah ada.

        UserID              : ID User pemilik konfigurasi.

        ConfigName          : Nama konfigurasi Dashboard Clinic.

        Settings.AutoRefresh
                            : Mengaktifkan atau menonaktifkan auto refresh dashboard.

        Settings.RefreshIntervalSec
                            : Interval refresh dashboard dalam satuan detik.

        Rooms               : Daftar konfigurasi room yang akan ditampilkan.

        Rooms.ServiceUnitID : ID Service Unit.

        Rooms.StageID       : ID Tahapan Antrian.

        Rooms.ParamedicID   : ID Dokter.

        Rooms.KamarID       : ID Kamar Display.

        Response Success :

        {
            ""success"": true,
            ""code"": 200,
            ""message"": ""Dashboard clinic configuration berhasil disimpan."",
            ""data"": {
                ""ConfigID"": ""CFG-260723-0001""
            }
        }
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void SaveDashboardClinicConfig()
        {
            try
            {
                if (AppSession.UserLogin == null)
                {
                    AppSession.UserLogin = new UserLogin
                    {
                        UserID = "WEBSERVICE",
                        UserName = "WEBSERVICE"
                    };
                }

                DashboardClinicConfigRequest request = null;

                // ==================================
                // PRIORITAS 1 : FORM / QUERY STRING
                // ==================================
                string userID = (Context.Request["UserID"] ?? "").Trim();

                if (!String.IsNullOrEmpty(userID))
                {
                    request = new DashboardClinicConfigRequest();

                    request.ConfigID = (Context.Request["ConfigID"] ?? "").Trim();
                    request.UserID = userID;
                    request.ConfigName = (Context.Request["ConfigName"] ?? "").Trim();

                    request.Settings = new DashboardClinicSetting
                    {
                        AutoRefresh = Convert.ToBoolean(Context.Request["AutoRefresh"] ?? "false"),
                        RefreshIntervalSec = Convert.ToInt32(Context.Request["RefreshIntervalSec"] ?? "0")
                    };

                    string roomsJson = (Context.Request["Rooms"] ?? "").Trim();

                    request.Rooms = String.IsNullOrWhiteSpace(roomsJson)
                        ? new List<DashboardClinicConfig.DashboardClinicRoomItem>()
                        : JsonConvert.DeserializeObject<List<DashboardClinicConfig.DashboardClinicRoomItem>>(roomsJson);
                }
                else
                {
                    // ==================================
                    // PRIORITAS 2 : RAW JSON BODY
                    // ==================================
                    Context.Request.InputStream.Position = 0;

                    using (var reader = new StreamReader(Context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();

                        if (!String.IsNullOrWhiteSpace(body))
                        {
                            request =
                                JsonConvert.DeserializeObject<DashboardClinicConfigRequest>(body);
                        }
                    }
                }

                if (request == null)
                    throw new Exception("Request tidak valid.");

                if (String.IsNullOrEmpty(request.UserID))
                    throw new Exception("UserID tidak boleh kosong.");

                if (request.Settings == null)
                    throw new Exception("Settings tidak boleh kosong.");

                if (request.Rooms == null || request.Rooms.Count == 0)
                    throw new Exception("Room minimal satu.");

                bool isNew = String.IsNullOrEmpty(request.ConfigID);

                string configID = request.ConfigID;

                if (isNew)
                {
                    var autoNumber = Helper.GetNewAutoNumber(
                        (new DateTime()).NowAtSqlServer().Date,
                        AppEnum.AutoNumber.DashboardClinicConfigNo
                    );

                    configID = autoNumber.LastCompleteNumber;

                    // jangan lupa simpan LastNumber
                    autoNumber.Save();
                }

                configID = DashboardClinicConfig.SaveConfig(
                    configID,
                    isNew,
                    request.UserID,
                    request.ConfigName,
                    request.Settings.AutoRefresh,
                    request.Settings.RefreshIntervalSec,
                    request.Rooms
                );

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        ConfigID = configID,
                        ConfigName = request.ConfigName,
                        UserID = request.UserID,

                        Settings = new
                        {
                            AutoRefresh = request.Settings.AutoRefresh,
                            RefreshIntervalSec = request.Settings.RefreshIntervalSec
                        },

                        RoomCount = request.Rooms.Count,

                        Rooms = request.Rooms,

                        LastUpdateDateTime = (new DateTime()).NowAtSqlServer()
                    },
                    "Dashboard poliklinik configuration berhasil disimpan."
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.ToString(),
                    500
                );
            }
        }

        [WebMethod(EnableSession = false, Description = @"
        Digunakan untuk mendapatkan daftar konfigurasi Dashboard Clinic.

        Request Body (JSON)

        {
            ""UserID"": ""240092"" // Optional
        }

        Keterangan Parameter :

        UserID : (Optional)
                 - Kosong = Menampilkan seluruh konfigurasi Dashboard Clinic.
                 - Diisi  = Menampilkan konfigurasi Dashboard Clinic milik User tersebut.

        Response Success :

        {
            ""success"": true,
            ""code"": 200,
            ""errorCode"": null,
            ""message"": ""Dashboard config ditemukan"",
            ""data"": {
                ""Configs"": [
                    {
                        ""ConfigID"": ""CFG-001"",
                        ""ConfigName"": ""Display Poli Anak Lantai 1"",
                        ""RoomCount"": 2,
                        ""UpdatedAt"": ""2026-07-23T10:30:00+07:00""
                    },
                    {
                        ""ConfigID"": ""CFG-002"",
                        ""ConfigName"": ""Dashboard Poli Penyakit Dalam"",
                        ""RoomCount"": 3,
                        ""UpdatedAt"": ""2026-07-23T11:00:00+07:00""
                    }
                ]
            }
        }
        ")]         
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDashboardClinicConfigList()
        {
            try
            {
                string userID = (Context.Request["UserID"] ?? "").Trim();

                if (String.IsNullOrWhiteSpace(userID))
                {
                    Context.Request.InputStream.Position = 0;

                    using (var reader = new StreamReader(Context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();

                        if (!String.IsNullOrWhiteSpace(body))
                        {
                            dynamic request = JsonConvert.DeserializeObject(body);

                            if (request != null && request.UserID != null)
                                userID = request.UserID.ToString();
                        }
                    }
                }

                var configs = DashboardClinicConfig.GetConfigList(userID);

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        Configs = configs
                    },
                    "Dashboard config ditemukan"
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.ToString(),
                    500
                );
            }
        }

        [WebMethod(EnableSession = true, Description = @"
        Digunakan untuk mengambil detail konfigurasi Dashboard Clinic berdasarkan UserID dan ConfigID.

        Parameter :

        ConfigID = ID konfigurasi Dashboard Clinic.
        UserID   = ID User pemilik konfigurasi.

        Contoh Request :

        GetDashboardClinicConfigDetail?
        ConfigID=CFG-260723-0007&
        UserID=240092

        Keterangan Parameter :

        ConfigID
            : Wajib.
              ID konfigurasi Dashboard Clinic yang akan diambil.

        UserID
            : Wajib.
              User pemilik konfigurasi. Digunakan untuk memastikan
              bahwa konfigurasi yang diminta memang milik User tersebut.

        Response Success :

        {
            ""success"": true,
            ""code"": 200,
            ""errorCode"": null,
            ""message"": ""Dashboard clinic configuration found"",
            ""data"": {
                ""ConfigID"": ""CFG-260723-0001"",
                ""ConfigName"": ""Display Executive Klinik"",
                ""UserID"": ""240092"",
                ""Rooms"": [
                    {
                        ""ServiceUnitID"": ""D2.2.03.2"",
                        ""ServiceUnitName"": ""Poliklinik Anak"",
                        ""StageID"": ""POLI"",
                        ""StageName"": ""Poliklinik"",
                        ""ParamedicID"": ""MD-00005"",
                        ""ParamedicName"": ""dr. Budi"",
                        ""KamarID"": ""1"",
                        ""KamarCode"": ""Kamar_1"",
                        ""KamarName"": ""Kamar 1""
                    }
                ],
                ""Settings"": {
                    ""AutoRefresh"": true,
                    ""RefreshIntervalSec"": 5
                },
                ""UpdatedAt"": ""2026-07-23T21:17:39+07:00""
            }
        }

        Response Error :

        {
            ""success"": false,
            ""code"": 500,
            ""errorCode"": ""ERR"",
            ""message"": ""Dashboard clinic configuration tidak ditemukan."",
            ""data"": null
        }
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDashboardClinicConfigDetail()
        {
            try
            {
                string configID = (Context.Request["ConfigID"] ?? "").Trim();
                string userID = (Context.Request["UserID"] ?? "").Trim();

                if (String.IsNullOrEmpty(configID))
                    throw new Exception("ConfigID tidak boleh kosong.");

                if (String.IsNullOrEmpty(userID))
                    throw new Exception("UserID tidak boleh kosong.");

                var data = DashboardClinicConfig.GetConfigDetail(
                    configID,
                    userID);

                ApiResponeForAntrian.Success(
                    Context,
                    data,
                    "Dashboard clinic detail ditemukan");
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500);
            }
        }

        [WebMethod(EnableSession = true, Description = @"
        Digunakan untuk menghapus konfigurasi Dashboard Clinic.

        Request Body (JSON)

        {
            ""ConfigID"": ""CFG-260723-0001""
        }

        Keterangan Parameter :

        ConfigID : ID konfigurasi Dashboard Clinic yang akan dihapus.

        Response Success :

        {
            ""success"": true,
            ""code"": 200,
            ""message"": ""Dashboard clinic configuration berhasil dihapus.""
        }
        ")]

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void DeleteDashboardClinicConfig()
        {
            try
            {
                if (AppSession.UserLogin == null)
                {
                    AppSession.UserLogin = new UserLogin
                    {
                        UserID = "WEBSERVICE",
                        UserName = "WEBSERVICE"
                    };
                }

                string configID = (Context.Request["ConfigID"] ?? "").Trim();

                // =============================
                // Support Raw JSON
                // =============================
                if (String.IsNullOrEmpty(configID))
                {
                    Context.Request.InputStream.Position = 0;

                    using (var reader = new StreamReader(Context.Request.InputStream))
                    {
                        string body = reader.ReadToEnd();

                        if (!String.IsNullOrWhiteSpace(body))
                        {
                            JObject obj = JsonConvert.DeserializeObject<JObject>(body);

                            configID = (obj["ConfigID"] ?? "").ToString();
                        }
                    }
                }

                if (String.IsNullOrWhiteSpace(configID))
                    throw new Exception("ConfigID tidak boleh kosong.");

                DashboardClinicConfig.DeleteConfig(configID);

                ApiResponeForAntrian.Success(
                    Context,
                    new
                    {
                        ConfigID = configID
                    },
                    "Dashboard clinic configuration berhasil dihapus."
                );
            }
            catch (Exception ex)
            {
                ApiResponeForAntrian.Error(
                    Context,
                    ex.Message,
                    500
                );
            }
        }

    }
}