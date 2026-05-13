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
        [WebMethod]
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

        [WebMethod]
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
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void TakeQueueVisitNumber(
            string PayerType,
            string ServiceGroup,
            string QueueLocation
        )
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                PayerType = (PayerType ?? "").Trim().ToUpper();
                QueueLocation = (QueueLocation ?? "").Trim().ToUpper();
                ServiceGroup = (ServiceGroup ?? "").Trim().ToUpper();

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
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianPasien(
            string QueueDate,
            string QueueLocation
        )
        {
            try
            {
                // =========================================
                // DEFAULT DATE
                // =========================================
                DateTime queueDate;

                if (!DateTime.TryParse(QueueDate, out queueDate))
                {
                    queueDate = DateTime.Now.Date;
                }

                // =========================================
                // NORMALIZE
                // =========================================
                QueueLocation =
                    (QueueLocation ?? "")
                    .Trim()
                    .ToUpper();

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
                    "Berhasil mengambil Display Antrian Pasien"
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDisplayAntrianPendaftaran(
            string QueueDate,
            string Status,
            string SRAutoNumber,
            string CurrentStage,
            string QueueLocation
        )
        {
            try
            {
                // =========================================
                // DEFAULT DATE
                // =========================================
                DateTime queueDate;

                if (!DateTime.TryParse(QueueDate, out queueDate))
                {
                    queueDate = DateTime.Now.Date;
                }

                // =========================================
                // NORMALIZE
                // =========================================
                Status =
                    (Status ?? "")
                    .Trim()
                    .ToUpper();

                SRAutoNumber =
                    (SRAutoNumber ?? "")
                    .Trim()
                    .ToUpper();

                CurrentStage =
                    string.IsNullOrEmpty(CurrentStage)
                        ? "LOKET"
                        : CurrentStage.Trim().ToUpper();

                QueueLocation =
                    (QueueLocation ?? "")
                    .Trim()
                    .ToUpper();

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

        //4.Panggil Antrian di Pendaftaran
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void CallAntrianSekarangPendaftaran(
            string VisitQueueNo,
            string UserID,
            string CounterID
        )
        {
            try
            {
                // =========================================
                // NORMALIZE
                // =========================================
                VisitQueueNo =
                    (VisitQueueNo ?? "")
                    .Trim();

                UserID =
                    (UserID ?? "")
                    .Trim();

                CounterID =
                    (CounterID ?? "")
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void RecallAntrianPendaftaran(
            string VisitQueueNo,
            string UserID
        )
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                VisitQueueNo =
                    (VisitQueueNo ?? "")
                    .Trim();

                UserID =
                    (UserID ?? "")
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void PendingAntrianPendaftaran(
            string VisitQueueNo,
            string UserID
        )
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                VisitQueueNo =
                    (VisitQueueNo ?? "")
                    .Trim();

                UserID =
                    (UserID ?? "")
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void WaitingFromPendingStatusPendaftaran(
            string VisitQueueNo,
            string UserID
        )
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                VisitQueueNo =
                    (VisitQueueNo ?? "")
                    .Trim();

                UserID =
                    (UserID ?? "")
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void NextAntrianPendaftaran(
            string QueueLocation,
            string UserID,
            string CounterID,
            string QueueDate
        )
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                QueueLocation =
                    (QueueLocation ?? "")
                    .Trim()
                    .ToUpper();

                UserID =
                    (UserID ?? "")
                    .Trim();

                CounterID =
                    (CounterID ?? "")
                    .Trim()
                    .ToUpper();

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

        [WebMethod]
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetQueueSoundPendaftaran(string VisitQueueNo)
        {
            try
            {
                // =========================
                // NORMALIZE
                // =========================
                VisitQueueNo =
                    (VisitQueueNo ?? "")
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

    }
}