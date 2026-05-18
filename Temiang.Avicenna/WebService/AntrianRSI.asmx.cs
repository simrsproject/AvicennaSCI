using DevExpress.XtraRichEdit.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Generated;
using Temiang.Dal;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Summary description for Antrol
    /// </summary>
    public class ApiResponeForAntrian
    {
        public static void Success(System.Web.HttpContext context, object data, string message = "OK")
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                success = true,
                code = 200,
                errorCode = (string)null,
                message = message,
                data = data
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }

        public static void Error(System.Web.HttpContext context, string message, int code = 500)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                success = false,
                code = code,
                errorCode = "ERR",
                message = message,
                data = (object)null
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.Write(json);
        }
    }

    public class CounterItem
    {
        public string CounterID { get; set; }
        public string CounterName { get; set; }
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class AntrianRSI : System.Web.Services.WebService
    {
        private List<CounterItem> GetCounterList()
        {
            var data = new List<CounterItem>();

            for (int i = 1; i <= 10; i++)
            {
                data.Add(new CounterItem
                {
                    CounterID = i.ToString(),
                    CounterName = "Counter_" + i
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

        //1. Pasien Ambil Antrian
        [WebMethod( Description = @"
           Ambil Data List PayerType untuk pasien memilih type TUNAI, MITRA DAN BPJS

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
                var collection = new AntrianAutoNumberSemanticCollection();

                collection.Query.Where(
                    collection.Query.Channel == "LOKET_PD",
                    collection.Query.IsActive == true
                );

                collection.Query.Load();

                var data = collection
                    .Select(x => x.PayerType)
                    .Distinct()
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

                if (data.Count == 0)
                {
                    ApiResponeForAntrian.Error(
                        Context,
                        "Data PayerType tidak ditemukan",
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

            CONTOH:
            GetDisplayAntrianPasien?
            QueueDate=2026-05-13&
            QueueLocation=LOKET_PD
            
            KETERANGAN:
               - QueueDate : Tanggal antrian (default hari ini jika kosong atau invalid)
               - QueueLocation : Lokasi/channel antrian

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

                // =========================================
                // GET DATA FROM BO
                // =========================================
                var data =
                    VisitQueue.GetDisplayAntrianPasien(
                        queueDate,
                        QueueLocation
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

        [WebMethod (Description = @"
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
                        ParamedicID
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
                        ParamedicID
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

        //7. CALL, RECALL, PENDING All Service Unit
        [WebMethod(Description = @"
            Digunakan untuk memanggil antrian pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            CallAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0015&
            UserID=Admin

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
                            UserID
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
            Digunakan untuk recall antrian pada seluruh Service Unit.

            PARAMETER:
            - VisitQueueNo (required)
            - UserID (required)

            EXAMPLE:
            RecallAntrianAllServiceUnit?
            VisitQueueNo=VQUE-260516-0015&
            UserID=Admin

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


    }
}