using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Web;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Newtonsoft.Json;
using Temiang.Avicenna.Common.RsOnline;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Summary description for SirsOnline
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SirsOnline : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public string DataKunjunganIgd(DateTime tanggal)
        {
            var reg = new RegistrationQuery("a");
            var unit = new ServiceUnitQuery("b");

            reg.Select(unit.ServiceUnitName, reg.GuarantorID);
            reg.InnerJoin(unit).On(reg.ServiceUnitID == unit.ServiceUnitID);
            reg.Where(reg.SRRegistrationType == AppConstant.RegistrationType.EmergencyPatient, reg.RegistrationDate.Date() == tanggal.Date, reg.IsConsul == false, reg.IsVoid == false);

            var table = reg.LoadDataTable();

            var grrID = AppSession.Parameter.GuarantorAskesID;

            var json = new List<Common.SirsOnline.DataKunjungan.Igd>();

            foreach (string klinik in table.AsEnumerable().Select(t => t.Field<string>("ServiceUnitName")).Distinct())
            {
                json.Add(new Common.SirsOnline.DataKunjungan.Igd
                {
                    JKN = table.AsEnumerable().Where(t => t.Field<string>("ServiceUnitName") == klinik && grrID.Contains(t.Field<string>("GuarantorID"))).Count(),
                    NONJKN = table.AsEnumerable().Where(t => t.Field<string>("ServiceUnitName") == klinik && !grrID.Contains(t.Field<string>("GuarantorID"))).Count()
                });
            }

            var svc = new Common.SirsOnline.Service();
            var response = svc.DataKunjunganIgd(json, tanggal.ToString("d-M-yyyy"));

            return response;
        }

        [WebMethod]
        public string DataKunjunganIrj(DateTime tanggal)
        {
            var reg = new RegistrationQuery("a");
            var unit = new ServiceUnitQuery("b");

            reg.Select(unit.ServiceUnitName, reg.GuarantorID);
            reg.InnerJoin(unit).On(reg.ServiceUnitID == unit.ServiceUnitID);
            reg.Where(reg.SRRegistrationType == AppConstant.RegistrationType.OutPatient, reg.RegistrationDate.Date() == tanggal.Date, reg.IsConsul == false, reg.IsVoid == false);

            var table = reg.LoadDataTable();

            var grrID = AppSession.Parameter.GuarantorAskesID;

            var json = new List<Common.SirsOnline.DataKunjungan.Irj>();

            foreach (string klinik in table.AsEnumerable().Select(t => t.Field<string>("ServiceUnitName")).Distinct())
            {
                json.Add(new Common.SirsOnline.DataKunjungan.Irj
                {
                    KLINIK = klinik,
                    JKN = table.AsEnumerable().Where(t => t.Field<string>("ServiceUnitName") == klinik && grrID.Contains(t.Field<string>("GuarantorID"))).Count(),
                    NONJKN = table.AsEnumerable().Where(t => t.Field<string>("ServiceUnitName") == klinik && !grrID.Contains(t.Field<string>("GuarantorID"))).Count()
                });
            }

            var svc = new Common.SirsOnline.Service();
            var response = svc.DataKunjunganIrj(json, tanggal.ToString("d-M-yyyy"));

            return response;
        }

        [WebMethod]
        public string DataKunjunganIri(DateTime tanggal)
        {
            var reg = new RegistrationQuery("a");
            var kelas = new ClassQuery("b");

            reg.Select(kelas.ClassName, reg.RegistrationNo);
            reg.InnerJoin(kelas).On(reg.ChargeClassID == kelas.ClassID);
            reg.Where(reg.SRRegistrationType == AppConstant.RegistrationType.InPatient, reg.RegistrationDate.Date() == tanggal.Date, reg.IsConsul == false, reg.IsVoid == false);

            var table = reg.LoadDataTable();

            var json = new List<Common.SirsOnline.DataKunjungan.Iri>();

            foreach (string klinik in table.AsEnumerable().Select(t => t.Field<string>("ClassName")).Distinct())
            {
                json.Add(new Common.SirsOnline.DataKunjungan.Iri
                {
                    CONTENT = klinik,
                    JLH = table.AsEnumerable().Where(t => t.Field<string>("ClassName") == klinik).Count().ToString()
                });
            }

            var svc = new Common.SirsOnline.Service();
            var response = svc.DataKunjunganIri(json, tanggal.ToString("d-M-yyyy"));

            return response;
        }

        [WebMethod]
        public string DiagnosaTerbesar(bool isIri, DateTime tanggal)
        {
            var reg = new RegistrationQuery("a");
            var epi = new EpisodeDiagnoseQuery("b");
            var diag = new DiagnoseQuery("c");

            reg.es.Top = 10;
            reg.Select(reg.RegistrationDate, epi.DiagnoseID, epi.DiagnoseID.Count().As("Total"));
            reg.InnerJoin(epi).On(reg.RegistrationNo == epi.RegistrationNo && epi.SRDiagnoseType == AppSession.Parameter.DiagnoseTypeMain && epi.IsVoid == false);
            reg.InnerJoin(diag).On(epi.DiagnoseID == diag.DiagnoseID);
            if (isIri) reg.Where(reg.SRRegistrationType == AppConstant.RegistrationType.InPatient);
            else reg.Where(reg.SRRegistrationType.In(AppConstant.RegistrationType.EmergencyPatient, AppConstant.RegistrationType.InPatient));
            reg.Where(reg.RegistrationDate.DatePart("month") == tanggal.Date.Month, reg.RegistrationDate.DatePart("year") == tanggal.Date.Year, reg.IsConsul == false, reg.IsVoid == false, epi.DiagnoseID != string.Empty);
            reg.GroupBy(reg.RegistrationDate, epi.DiagnoseID);

            var table = reg.LoadDataTable();

            var json = new List<Common.SirsOnline.DiagnosaTerbesar>();

            foreach (DateTime date in table.AsEnumerable().Select(t => t.Field<DateTime>("RegistrationDate")).OrderBy(t => t).Distinct())
            {
                foreach (DataRow row in table.AsEnumerable().Where(t => t.Field<DateTime>("RegistrationDate") == date).OrderByDescending(t => t.Field<int>("Count")).Take(10))
                {
                    json.Add(new Common.SirsOnline.DiagnosaTerbesar
                    {
                        IDDIAG = row["DiagnoseID"].ToString(),
                        JUMLAHKASUS = Convert.ToInt32(row["Count"]),
                        TANGGAL = date.ToString("dd-MM-yyyy")
                    });
                }
            }

            var svc = new Common.SirsOnline.Service();
            var response = svc.DiagnosaTerbesar(json, isIri, tanggal.ToString("M-yyyy"));

            return response;
        }

        [WebMethod]
        public string IndikatorPelayanan(int bulan, int tahun)
        {
            var rlhd = new RlTxReportQuery("a");
            var rldt = new RlTxReport12Query("b");

            rlhd.es.Top = 1;
            rlhd.Select(rldt.Bor);
            rlhd.InnerJoin(rldt).On(rlhd.RlTxReportNo == rldt.RlTxReportNo);
            rlhd.Where(rlhd.PeriodMonthStart == bulan, rlhd.PeriodMonthEnd == bulan, rlhd.PeriodYear == tahun);
            rlhd.OrderBy(rlhd.LastUpdateDateTime.Descending);

            var table = rlhd.LoadDataTable();

            if (table != null && table.Rows.Count > 0)
            {
                var svc = new Common.SirsOnline.Service();
                var response = svc.IndikatorPelayanan(new Common.SirsOnline.IndikatorPelayanan { BOR = Convert.ToDouble(table.Rows[0]["Bor"]) }, $"{bulan}-{tahun}");

                return response;
            }
            else return "no data";
        }

        [WebMethod]
        public string ReferensiTempatTidur()
        {
            var svc = new Common.RsOnline.Service();
            var response = svc.ReferensiTempatTidur();

            return JsonConvert.SerializeObject(response);
        }

        [WebMethod]
        public string GetTempatTidur()
        {
            var svc = new Common.RsOnline.Service();
            var get = svc.Get();

            return JsonConvert.SerializeObject(get);
        }

        //[WebMethod]
        //public string InsertTempatTidur()
        //{
        //    var delete = DeleteTempatTidur();

        //    var bed = new BedQuery("a");
        //    var cls = new ClassQuery("b");
        //    var clb = new ClassBridgingQuery("c");
        //    var room = new ServiceRoomQuery("d");
        //    var unit = new ServiceUnitQuery("e");
        //    bed.Select(
        //            clb.BridgingID,
        //            unit.ServiceUnitID,
        //            unit.ServiceUnitName,
        //            room.RoomID,
        //            room.RoomName,
        //            bed.BedID,
        //            bed.SRBedStatus
        //        );
        //    bed.InnerJoin(cls).On(bed.ClassID == cls.ClassID && cls.IsActive == true);
        //    bed.InnerJoin(clb).On(bed.ClassID == clb.ClassID && clb.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString());
        //    bed.InnerJoin(room).On(bed.RoomID == room.RoomID && room.IsActive == true);
        //    bed.InnerJoin(unit).On(room.ServiceUnitID == unit.ServiceUnitID && unit.IsActive == true);
        //    bed.Where(bed.SRBedStatus.NotIn("BedStatus-07"), bed.IsActive == true);
        //    //bed.Where(unit.ServiceUnitID == "D2.3.14");
        //    var src = bed.LoadDataTable();

        //    var regis = new RegistrationQuery("a");
        //    clb = new ClassBridgingQuery("b");
        //    regis.Select(regis.RegistrationNo, regis.ServiceUnitID, clb.BridgingID);
        //    regis.InnerJoin(clb).On(regis.ClassID == clb.ClassID && clb.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString());
        //    regis.Where(regis.SRRegistrationType == AppConstant.RegistrationType.InPatient,
        //        regis.DischargeDate.IsNull(),
        //        regis.IsClosed == false,
        //        regis.IsVoid == false);
        //    //regis.Where(regis.ServiceUnitID == "D2.3.14");
        //    var srd = regis.LoadDataTable();

        //    var list = src.AsEnumerable()
        //        .GroupBy(s => new
        //        {
        //            IdTt = s.Field<string>("BridgingID"),
        //            ServiceUnitID = s.Field<string>("ServiceUnitID"),
        //            Ruang = s.Field<string>("RoomName"),
        //            RoomID = s.Field<string>("RoomID")
        //        })
        //        .Select(s => new Common.RsOnline.Json.Request.Insert()
        //        {
        //            IdTt = s.Key.IdTt,
        //            ServiceUnitID = s.Key.ServiceUnitID,
        //            Ruang = s.Key.Ruang,
        //            JumlahRuang = src.AsEnumerable().Where(ss => ss.Field<string>("ServiceUnitID") == s.Key.ServiceUnitID && ss.Field<string>("RoomID") == s.Key.RoomID)
        //                .GroupBy(ss => new { ServiceUnitID = ss.Field<string>("ServiceUnitID"), RoomID = ss.Field<string>("RoomID") }).Count().ToString(),
        //            Jumlah = src.AsEnumerable().Where(ss => ss.Field<string>("ServiceUnitID") == s.Key.ServiceUnitID && ss.Field<string>("RoomID") == s.Key.RoomID)
        //                .GroupBy(ss => new { ServiceUnitID = ss.Field<string>("ServiceUnitID"), RoomID = ss.Field<string>("RoomID"), BedID = ss.Field<string>("BedID") }).Count().ToString(),
        //            Antrian = src.AsEnumerable().Where(ss => ss.Field<string>("ServiceUnitID") == s.Key.ServiceUnitID && ss.Field<string>("RoomID") == s.Key.RoomID && ss.Field<string>("SRBedStatus") == "BedStatus-03")
        //                .GroupBy(ss => new { ServiceUnitID = ss.Field<string>("ServiceUnitID"), RoomID = ss.Field<string>("RoomID"), BedID = ss.Field<string>("BedID") }).Count().ToString()
        //        });

        //    var sum = list.GroupBy(l => new
        //    {
        //        IdTt = l.IdTt,
        //        ServiceUnitID = l.ServiceUnitID,
        //        Ruang = l.Ruang
        //    }).Select(l => new Common.RsOnline.Json.Request.Insert()
        //    {
        //        IdTt = l.Key.IdTt,
        //        Ruang = l.Key.Ruang,
        //        JumlahRuang = l.Sum(x => x.JumlahRuang.ToInt()).ToString(),
        //        Jumlah = l.Sum(x => x.Jumlah.ToInt()).ToString(),
        //        Terpakai = srd.AsEnumerable().Count(ss => ss.Field<string>("ServiceUnitID") == l.Key.ServiceUnitID && ss.Field<string>("BridgingID") == l.Key.IdTt).ToString(),
        //        TerpakaiSuspek = "0",
        //        TerpakaiKonfirmasi = "0",
        //        Antrian = l.Sum(x => x.Antrian.ToInt()).ToString(),
        //        Prepare = "0",
        //        PreparePlan = "0",
        //        Covid = "0"
        //    });

        //    foreach (var data in sum)
        //    {
        //        var svc = new Service();
        //        var insert = svc.Insert(data);
        //    }

        //    return JsonConvert.SerializeObject(sum);
        //}

        [WebMethod]
        public string DeleteTempatTidur()
        {
            var svc = new Service();
            var get = svc.Get();
            if (get == null || !get.Fasyankes.Any()) return string.Empty;

            foreach (var data in get.Fasyankes)
            {
                svc = new Service();
                var delete = svc.Delete(new Json.Request.Delete() { IdTtt = data.IdTTt });
            }

            return string.Empty;
        }

        [WebMethod]
        public string InsertTempatTidur()
        {
            var delete = DeleteTempatTidur();

            var sum = JsonConvert.DeserializeObject<List<Json.Request.Insert>>(InsertTempatTidurByRoomDataOnly(string.Empty));

            foreach (var data in sum)
            {
                var svc = new Service();
                var insert = svc.Insert(data);
            }

            return JsonConvert.SerializeObject(sum);
        }

        [WebMethod]
        public string InsertTempatTidurByBed()
        {
            var delete = DeleteTempatTidur();

            var sum = JsonConvert.DeserializeObject<List<Json.Request.Insert>>(InsertTempatTidurByBedDataOnly(string.Empty));

            foreach (var data in sum)
            {
                var svc = new Service();
                var insert = svc.Insert(data);
            }

            return JsonConvert.SerializeObject(sum);
        }

        [WebMethod]
        public string InsertTempatTidurByRoomDataOnly(string serviceUnitID)
        {
            var bed = new BedQuery("a");
            var room = new ServiceRoomQuery("d");
            var clr = new ServiceRoomBridgingQuery("e");
            bed.Select(
                    clr.BridgingID,
                    room.RoomID,
                    room.RoomName,
                    bed.BedID,
                    bed.SRBedStatus
                );
            bed.InnerJoin(room).On(bed.RoomID == room.RoomID && room.IsActive == true);
            bed.InnerJoin(clr).On(room.RoomID == clr.RoomID && clr.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString());
            bed.Where(bed.SRBedStatus.NotIn("BedStatus-07"),
                bed.IsVisibleTo3rdParty == true,
                bed.IsActive == true);
            if (!string.IsNullOrWhiteSpace(serviceUnitID)) bed.Where(room.ServiceUnitID == serviceUnitID);
            var src = bed.LoadDataTable();

            var regis = new RegistrationQuery("a");
            clr = new ServiceRoomBridgingQuery("e");
            regis.Select(
                    regis.RegistrationNo,
                    regis.RoomID,
                    clr.BridgingID,
                    regis.BedID
                );
            regis.InnerJoin(clr).On(regis.RoomID == clr.RoomID && clr.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString());
            regis.Where(regis.SRRegistrationType == AppConstant.RegistrationType.InPatient,
                regis.DischargeDate.IsNull(),
                regis.IsClosed == false,
                regis.IsVoid == false);
            if (!string.IsNullOrWhiteSpace(serviceUnitID)) regis.Where(regis.ServiceUnitID == serviceUnitID);
            var srd = regis.LoadDataTable();

            var list = src.AsEnumerable()
                .GroupBy(s => new
                {
                    IdTt = s.Field<string>("BridgingID"),
                    RoomID = s.Field<string>("RoomID"),
                    Ruang = s.Field<string>("RoomName")
                }).Select(s => new Common.RsOnline.Json.Request.Insert()
                {
                    IdTt = s.Key.IdTt,
                    RoomID = s.Key.RoomID,
                    Ruang = s.Key.Ruang,
                    JumlahRuang = src.AsEnumerable().Where(ss => ss.Field<string>("RoomID") == s.Key.RoomID)
                        .GroupBy(ss => new { RoomID = ss.Field<string>("RoomID") }).Count().ToString(),
                    Jumlah = src.AsEnumerable().Where(ss => ss.Field<string>("RoomID") == s.Key.RoomID)
                        .GroupBy(ss => new { RoomID = ss.Field<string>("RoomID"), BedID = ss.Field<string>("BedID") }).Count().ToString(),
                    Antrian = src.AsEnumerable().Where(ss => ss.Field<string>("RoomID") == s.Key.RoomID && ss.Field<string>("SRBedStatus") == "BedStatus-03")
                        .GroupBy(ss => new { RoomID = ss.Field<string>("RoomID"), BedID = ss.Field<string>("BedID") }).Count().ToString()
                });

            var sum = list.GroupBy(l => new
            {
                l.IdTt,
                l.RoomID,
                l.Ruang
            }).Select(l => new Common.RsOnline.Json.Request.Insert()
            {
                IdTt = l.Key.IdTt,
                Ruang = l.Key.Ruang,
                JumlahRuang = l.Sum(x => x.JumlahRuang.ToInt()).ToString(),
                Jumlah = l.Sum(x => x.Jumlah.ToInt()).ToString(),
                Terpakai = srd.AsEnumerable().Count(ss => ss.Field<string>("RoomID") == l.Key.RoomID && ss.Field<string>("BridgingID") == l.Key.IdTt).ToString(),
                TerpakaiSuspek = "0",
                TerpakaiKonfirmasi = "0",
                Antrian = l.Sum(x => x.Antrian.ToInt()).ToString(),
                Prepare = "0",
                PreparePlan = "0",
                Covid = "0"
            }).OrderBy(l => l.Ruang);

            return JsonConvert.SerializeObject(sum);
        }

        [WebMethod]
        public string InsertTempatTidurByBedDataOnly(string serviceUnitID)
        {
            var bed = new BedQuery("a");
            var room = new ServiceRoomQuery("d");
            bed.Select(
                    bed.BridgingID,
                    room.RoomID,
                    room.RoomName,
                    bed.BedID,
                    bed.SRBedStatus
                );
            bed.InnerJoin(room).On(bed.RoomID == room.RoomID && room.IsActive == true);
            bed.Where(bed.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString(),
                bed.SRBedStatus.NotIn("BedStatus-07"),
                bed.IsVisibleTo3rdParty == true,
                bed.IsActive == true);
            if (!string.IsNullOrWhiteSpace(serviceUnitID)) bed.Where(room.ServiceUnitID == serviceUnitID);
            var src = bed.LoadDataTable();

            var regis = new RegistrationQuery("a");
            bed = new BedQuery("b");
            regis.Select(
                    regis.RegistrationNo,
                    regis.RoomID,
                    bed.BridgingID,
                    regis.BedID
                );
            regis.InnerJoin(bed).On(regis.BedID == bed.BedID && 
                bed.SRBridgingType == AppEnum.BridgingType.RS_ONLINE.ToString() && 
                bed.IsVisibleTo3rdParty == true && 
                bed.IsActive == true);
            regis.Where(regis.SRRegistrationType == AppConstant.RegistrationType.InPatient,
                regis.DischargeDate.IsNull(),
                regis.IsClosed == false,
                regis.IsVoid == false);
            if (!string.IsNullOrWhiteSpace(serviceUnitID)) regis.Where(regis.ServiceUnitID == serviceUnitID);
            var srd = regis.LoadDataTable();

            var list = src.AsEnumerable()
                .GroupBy(s => new
                {
                    IdTt = s.Field<string>("BridgingID"),
                    RoomID = s.Field<string>("RoomID"),
                    Ruang = s.Field<string>("RoomName")
                }).Select(s => new Common.RsOnline.Json.Request.Insert()
                {
                    IdTt = s.Key.IdTt,
                    RoomID = s.Key.RoomID,
                    Ruang = s.Key.Ruang,
                    JumlahRuang = src.AsEnumerable().Where(ss => ss.Field<string>("BridgingID") == s.Key.IdTt && ss.Field<string>("RoomID") == s.Key.RoomID)
                        .GroupBy(ss => new { IdTt = ss.Field<string>("BridgingID"), RoomID = ss.Field<string>("RoomID"), Ruang = ss.Field<string>("RoomName") }).Count().ToString(),
                    Jumlah = src.AsEnumerable().Where(ss => ss.Field<string>("BridgingID") == s.Key.IdTt && ss.Field<string>("RoomID") == s.Key.RoomID)
                        .GroupBy(ss => new { IdTt = ss.Field<string>("BridgingID"), RoomID = ss.Field<string>("RoomID"), Ruang = ss.Field<string>("RoomName"), BedID = ss.Field<string>("BedID") }).Count().ToString(),
                    Antrian = src.AsEnumerable().Where(ss => ss.Field<string>("BridgingID") == s.Key.IdTt && ss.Field<string>("RoomID") == s.Key.RoomID && ss.Field<string>("SRBedStatus") == "BedStatus-03")
                        .GroupBy(ss => new { IdTt = ss.Field<string>("BridgingID"), RoomID = ss.Field<string>("RoomID"), Ruang = ss.Field<string>("RoomName"), BedID = ss.Field<string>("BedID") }).Count().ToString(),
                });

            var sum = list.GroupBy(l => new
            {
                l.IdTt,
                l.RoomID,
                l.Ruang
            }).Select(l => new Common.RsOnline.Json.Request.Insert()
            {
                IdTt = l.Key.IdTt,
                Ruang = l.Key.Ruang,
                JumlahRuang = l.Sum(x => x.JumlahRuang.ToInt()).ToString(),
                Jumlah = l.Sum(x => x.Jumlah.ToInt()).ToString(),
                Terpakai = srd.AsEnumerable().Count(ss => ss.Field<string>("RoomID") == l.Key.RoomID && ss.Field<string>("BridgingID") == l.Key.IdTt).ToString(),
                TerpakaiSuspek = "0",
                TerpakaiKonfirmasi = "0",
                Antrian = l.Sum(x => x.Antrian.ToInt()).ToString(),
                Prepare = "0",
                PreparePlan = "0",
                Covid = "0"
            }).OrderBy(l => l.Ruang);

            return JsonConvert.SerializeObject(sum);
        }
    }
}
