using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Reference;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Inacbg.v510.Detail;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Summary description for Inacbg
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Inacbg : System.Web.Services.WebService
    {
        private string _imgReady = "../../../../Images/Toolbar/refresh16.png";

        [WebMethod]
        public string HelloWorld(string name)
        {
            return "Hello World, " + name;
        }

        [WebMethod]
        public void CreateGroupper(string sepNo)
        {
            var sep = new BpjsSEP();
            if (!sep.LoadByPrimaryKey(sepNo)) return;

            var reg = new Registration();
            reg.Query.Where(reg.Query.BpjsSepNo == sep.NoSEP);
            if (!reg.Query.Load()) return;

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);

            var medic = new Paramedic();
            medic.LoadByPrimaryKey(reg.ParamedicID);

            var diag = new Diagnose();
            diag.LoadByPrimaryKey(sep.DiagnosaAwal);

            //var inacbg = new Temiang.Avicenna.Common.BPJS.v20.Service();
            //inacbg.CreateInacbgGroupper(false, patient.MedicalNo, "5", sep.NoSEP, sep.JenisPelayanan, sep.KelasRawat,
            //    reg.RegistrationDate.Value.Date.ToString("yyyy-MM-dd"), reg.RegistrationDate.Value.Date.ToString("yyyy-MM-dd"), "1", medic.ParamedicName, "0", "10000", "0", "0", "0",
            //    sep.NomorKartu, sep.NamaPasien, sep.JenisKelamin, sep.TanggalLahir.Value.Date.ToString("yyyy-MM-dd"), "", "", "", "",
            //    sep.DiagnosaAwal, diag.DiagnoseName, "", "", "", "", "", "");
        }

        [WebMethod]
        public void SetCalculationTable()
        {
            //var inacbg = new Module.Finance.Integration.InacbgProcessList();

            //var service = new Common.Inacbg.v41.Service();
            //service.GetCalculationTable(GetDataTable);


        }

        public DataTable GetDataTable
        {
            get
            {
                var sep = new BpjsSEPQuery("a");
                var reg = new RegistrationQuery("b");
                var diag = new DiagnoseQuery("c");
                var std = new AppStandardReferenceItemQuery("d");
                var medic = new ParamedicQuery("e");
                var invoiceItem = new InvoicesItemQuery("f");
                var invoice = new InvoicesQuery("g");
                var payment = new TransPaymentQuery("h");

                sep.es.Distinct = true;
                sep.Select(
                    sep,
                    reg.DischargeDate,
                    std.ItemName,
                    diag.DiagnoseID,
                    diag.DiagnoseName,
                    reg.RegistrationNo,
                    "<CASE WHEN a.KodeCBG != '' THEN CASE WHEN h.PaymentNo IS NULL THEN '' ELSE 'SUKSES' END ELSE '' END AS [Status]>",
                    "<'' AS Keterangan>",
                    string.Format("<'{0}' AS ImgUrl>", _imgReady),
                    payment.PaymentNo.Coalesce("''"),
                    payment.PaymentDate.Coalesce("''")
                    );
                sep.InnerJoin(reg).On(sep.NoSEP == reg.BpjsSepNo && reg.SRRegistrationType == AppConstant.RegistrationType.InPatient && reg.IsClosed == false && reg.IsVoid == false);
                sep.InnerJoin(diag).On(sep.DiagnosaAwal == diag.DiagnoseID);
                sep.InnerJoin(std).On(sep.JenisPelayanan == std.ItemID && std.StandardReferenceID == AppEnum.StandardReference.BpjsTypeOfService);
                sep.InnerJoin(medic).On(reg.ParamedicID == medic.ParamedicID);
                sep.LeftJoin(invoiceItem).On(reg.RegistrationNo == invoiceItem.RegistrationNo);
                sep.LeftJoin(invoice).On(invoice.InvoiceNo == invoiceItem.InvoiceNo && invoice.InvoiceReferenceNo.IsNull() && invoice.IsApproved == true);
                sep.LeftJoin(payment).On(reg.RegistrationNo == payment.RegistrationNo && payment.TransactionCode == TransactionCode.Payment && payment.IsApproved == true);

                sep.Where(invoiceItem.RegistrationNo.IsNull());

                return sep.LoadDataTable();
            }
        }

        [WebMethod]
        public string GetBase64PdfPrint(string nomor_sep)
        {
            var service = new Common.Inacbg.v51.Service();
            var print = service.Print(new Common.Inacbg.v51.Claim.Create.Data() { nomor_sep = nomor_sep });
            //return print;

            //if (print.Metadata.IsValid && string.IsNullOrEmpty(print.Data))
            //{
            //    string byteArrayEncoded = System.Convert.ToBase64String(print.Data, 0, print.Data.Length);
            //    string byteArrayUrlEncoded = System.Web.HttpUtility.UrlEncode(byteArrayEncoded);
            //}

            return (print.Metadata.IsValid) ? print.Data : string.Empty;
        }

        [WebMethod]
        public string ReloadGrouper()
        {
            var ncc = new NccInacbgCollection();
            ncc.LoadAll();

            foreach (var entity in ncc)
            {
                var reg = new Registration();
                if (!reg.LoadByPrimaryKey(entity.RegistrationNo)) continue;
                if (string.IsNullOrEmpty(reg.BpjsSepNo)) continue;
                if (entity.CoverageAmount != null && entity.CoverageAmount > 0) continue;

                var service = new Common.Inacbg.v51.Service();
                var response = service.GetDetail(new Common.Inacbg.v51.Claim.Get.GetDetail.Data() { nomor_sep = reg.BpjsSepNo });
                if (response.Metadata.IsValid)
                {
                    entity.AddPaymentAmt = string.IsNullOrEmpty(response.DataResponse.Data.AddPaymentAmt) ? 0 : Convert.ToDecimal(response.DataResponse.Data.AddPaymentAmt);

                    var grouper = response.DataResponse.Data.Grouper;
                    if (grouper.Response != null)
                    {
                        entity.CoverageAmount = Convert.ToDecimal(grouper.Response.Cbg.Tariff);
                        entity.CbgID = grouper.Response.Cbg.Code;
                        entity.CbgName = grouper.Response.Cbg.Description;
                    }
                    else entity.CoverageAmount = 0;

                    entity.CbgStatus = response.DataResponse.Data.KlaimStatusCd;
                    entity.CbgSentStatus = response.DataResponse.Data.KemenkesDcStatusCd;
                }
            }

            ncc.Save();

            return "ok";
        }

        [WebMethod]
        public void LoadGroupper(string registrationNo)
        {
            var reg = new Registration();
            reg.LoadByPrimaryKey(registrationNo);

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);

            var pmedic = new Paramedic();
            pmedic.LoadByPrimaryKey(reg.ParamedicID);

            if (string.IsNullOrWhiteSpace(reg.BpjsSepNo)) return;

            var svc = new Common.Inacbg.v51.Service();
            var response = svc.Insert(new Common.Inacbg.v51.Claim.Create.Data()
            {
                nomor_kartu = "0000000000001", //
                nomor_sep = reg.RegistrationNo,
                nomor_rm = "0000000000001",
                nama_pasien = "PASIEN SAMPLE",
                tgl_lahir = "2000-01-01 00:00:00",
                gender = (patient.Sex == "M" ? "1" : "2")
            });
            {
                var ncc = new NccInacbg();
                if (!ncc.LoadByPrimaryKey(reg.RegistrationNo)) ncc = new NccInacbg();
                ncc.RegistrationNo = reg.RegistrationNo;
                if (response.Metadata.IsValid)
                {
                    ncc.PatientId = response.Response.PatientId;
                    ncc.AdmissionId = response.Response.AdmissionId;
                    ncc.HospitalAdmissionId = response.Response.HospitalAdmissionId;
                }
                ncc.LastUpdateDateTime = DateTime.Now;
                ncc.LastUpdateByUserID = "webservice";
                ncc.AddPaymentAmt = 0;
                ncc.Save();

                // Untuk Inpatient disimpan di table RegistrationnfoMedicDiagnose - (Handono 230323)
                //var diag = string.Empty;
                //var diags = new EpisodeDiagnoseCollection();
                //diags.Query.Where(diags.Query.RegistrationNo == reg.RegistrationNo, diags.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-004"), diags.Query.IsVoid == false); // main & secondary diagnose
                //diags.Query.OrderBy(diags.Query.SRDiagnoseType.Ascending);
                //if (!diags.Query.Load())
                //{
                //    var sep = new BpjsSEP();
                //    if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                //    else diag = "#";
                //}
                //else
                //{
                //    foreach (var d in diags)
                //    {
                //        diag += d.DiagnoseID + "#";
                //    }
                //}

                // Start modif - untuk Inpatient disimpan di table RegistrationnfoMedicDiagnose - (Handono 230323)
                var diag = string.Empty;
                if (reg.SRRegistrationType == AppConstant.RegistrationType.InPatient)
                {
                    var diags = new RegistrationInfoMedicDiagnoseCollection();
                    diags.Query.Where(diags.Query.RegistrationNo == reg.RegistrationNo,
                        diags.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-004"),
                        diags.Query.IsVoid == false,
                        diags.Query.Or(diags.Query.DiagnoseID != string.Empty, diags.Query.DiagnoseID.IsNotNull())); // main & secondary diagnose
                    diags.Query.OrderBy(diags.Query.SRDiagnoseType.Ascending);

                    if (diags.Query.Load())
                    {
                        foreach (var d in diags.Where(d => !string.IsNullOrWhiteSpace(d.DiagnoseID)))
                        {
                            var diagnose = new Diagnose();
                            if (!diagnose.LoadByPrimaryKey(d.DiagnoseID)) continue;
                            diag += d.DiagnoseID + "#";
                        }
                    }
                    else
                    {
                        var diags2 = new EpisodeDiagnoseCollection();
                        diags2.Query.Where(diags2.Query.RegistrationNo == reg.RegistrationNo,
                            diags2.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-004"),
                            diags2.Query.IsVoid == false); // main & secondary diagnose
                        diags2.Query.OrderBy(diags2.Query.SRDiagnoseType.Ascending);

                        if (diags2.Query.Load())
                        {
                            foreach (var d in diags2)
                            {
                                diag += d.DiagnoseID + "#";
                            }
                        }
                        else
                        {
                            var sep = new BpjsSEP();
                            if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                            else diag = "#";
                        }
                    }

                    if (diag.Split('#').Where(d => !string.IsNullOrWhiteSpace(d)).Count() == 0)
                    {
                        diag = string.Empty;

                        var diags2 = new EpisodeDiagnoseCollection();
                        diags2.Query.Where(diags2.Query.RegistrationNo == reg.RegistrationNo,
                            diags2.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-004"),
                            diags2.Query.IsVoid == false); // main & secondary diagnose
                        diags2.Query.OrderBy(diags2.Query.SRDiagnoseType.Ascending);

                        if (diags2.Query.Load())
                        {
                            foreach (var d in diags2)
                            {
                                diag += d.DiagnoseID + "#";
                            }
                        }
                        else
                        {
                            var sep = new BpjsSEP();
                            if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                            else diag = "#";
                        }
                    }

                    if (diag.Split('#').Where(d => !string.IsNullOrWhiteSpace(d)).Count() == 0)
                    {
                        var sep = new BpjsSEP();
                        if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                        else diag = "#";
                    }
                }
                else
                {
                    var diags = new EpisodeDiagnoseCollection();
                    diags.Query.Where(diags.Query.RegistrationNo == reg.RegistrationNo,
                        diags.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-004"),
                        diags.Query.IsVoid == false); // main & secondary diagnose
                    diags.Query.OrderBy(diags.Query.SRDiagnoseType.Ascending);

                    if (diags.Query.Load())
                    {
                        foreach (var d in diags)
                        {
                            diag += d.DiagnoseID + "#";
                        }
                    }
                    else
                    {
                        var sep = new BpjsSEP();
                        if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                        else diag = "#";
                    }

                    if (diag.Split('#').Where(d => !string.IsNullOrWhiteSpace(d)).Count() == 0)
                    {
                        var sep = new BpjsSEP();
                        if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                        else diag = "#";
                    }
                }

                //if (string.IsNullOrWhiteSpace(diag))
                //{
                //    var sep = new BpjsSEP();
                //    if (sep.LoadByPrimaryKey(reg.BpjsSepNo)) diag = $"{sep.DiagnosaAwal}#";
                //    else diag = "#";
                //}
                // End modif (Handono 230323)

                var proc = string.Empty;
                var procedures = new EpisodeProcedureCollection();
                procedures.Query.Where(procedures.Query.RegistrationNo == reg.RegistrationNo, procedures.Query.IsVoid == false);
                procedures.Query.OrderBy(procedures.Query.ProcedureID.Ascending);
                if (!procedures.Query.Load()) proc = "#";
                else
                {
                    foreach (var d in procedures)
                    {
                        proc += d.ProcedureID + "#";
                    }
                }

                var std = new AppStandardReferenceItem();
                std.Query.Where(std.Query.StandardReferenceID == AppEnum.StandardReference.BpjsTariffType.ToString());
                std.Query.Load();

                var svc54 = new Common.Inacbg.v58.Service();
                var detail = svc54.Insert(new Common.Inacbg.v58.Detail.Data()
                {
                    nomor_sep = reg.RegistrationNo,
                    nomor_kartu = reg.GuarantorCardNo,
                    tgl_masuk = reg.RegistrationDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    tgl_pulang = new[] { AppConstant.RegistrationType.OutPatient, AppConstant.RegistrationType.EmergencyPatient }.Contains(reg.SRRegistrationType) ? reg.RegistrationDate.Value.ToString("yyyy-MM-dd HH:mm:ss") :
                        reg.DischargeDate == null ? DateTime.Now.Date.ToString("yyyy-MM-dd HH:mm:ss") : reg.DischargeDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    cara_masuk = "other",
                    jenis_rawat = reg.SRRegistrationType == AppConstant.RegistrationType.OutPatient ? "Jalan" : (reg.SRRegistrationType == AppConstant.RegistrationType.InPatient ? "Inap" : "IGD"),
                    kelas_rawat = reg.SRRegistrationType == AppConstant.RegistrationType.OutPatient ? "2" : (reg.SRRegistrationType == AppConstant.RegistrationType.InPatient ? "1" : "3"),
                    adl_sub_acute = string.Empty,
                    adl_chronic = string.Empty,
                    icu_indikator = "0",
                    icu_los = "0",
                    ventilator_hour = "0",
                    use_ind = "0",
                    start_dttm = string.Empty,
                    stop_dttm = string.Empty,
                    upgrade_class_ind = "0",
                    upgrade_class_class = string.Empty,
                    upgrade_class_los = string.Empty,
                    upgrade_class_payor = string.Empty,
                    add_payment_pct = "0",
                    birth_weight = "0",
                    sistole = "110",
                    diastole = "80",
                    discharge_status = "5",
                    diagnosa = diag,
                    procedure = proc,
                    tarif_rs = new Common.Inacbg.v54.Detail.TarifRs() // fix
                    {
                        prosedur_non_bedah = "1000000",
                        prosedur_bedah = "1000000",
                        konsultasi = "1000000",
                        tenaga_ahli = "1000000",
                        keperawatan = "1000000",
                        penunjang = "1000000",
                        radiologi = "1000000",
                        laboratorium = "1000000",
                        pelayanan_darah = "1000000",
                        rehabilitasi = "1000000",
                        kamar = "1000000",
                        rawat_intensif = "1000000",
                        obat = "1000000",
                        obat_kronis = "1000000",
                        obat_kemoterapi = "1000000",
                        alkes = "1000000",
                        bmhp = "1000000",
                        sewa_alat = "1000000"
                    },
                    pemulasaraan_jenazah = "0",
                    kantong_jenazah = "0",
                    peti_jenazah = "0",
                    plastik_erat = "0",
                    desinfektan_jenazah = "0",
                    mobil_jenazah = "0",
                    desinfektan_mobil_jenazah = "0",
                    covid19_status_cd = string.Empty,
                    nomor_kartu_t = "kartu_jkn",
                    episodes = string.Empty,
                    covid19_cc_ind = "0",
                    covid19_rs_darurat_ind = "0", //
                    covid19_co_insidense_ind = "0", //
                    covid19_penunjang_pengurang = new Common.Inacbg.v54.Detail.Covid19PenunjangPengurang()
                    {
                        lab_asam_laktat = "0", //
                        lab_procalcitonin = "0", //
                        lab_crp = "0", //
                        lab_kultur = "0", //
                        lab_d_dimer = "0", //
                        lab_pt = "0", //
                        lab_aptt = "0", //
                        lab_waktu_pendarahan = "0", //
                        lab_anti_hiv = "0", //
                        lab_analisa_gas = "0", //
                        lab_albumin = "0", //
                        rad_thorax_ap_pa = "0"  //
                    },
                    terapi_konvalesen = "0", //
                    akses_naat = "A", //
                    isoman_ind = "1", //
                    bayi_lahir_status_cd = string.Empty, //
                    dializer_single_use = "1",
                    kantong_darah = string.Empty,
                    menit_1_appearance = string.Empty,
                    menit_1_pulse = string.Empty,
                    menit_1_grimace = string.Empty,
                    menit_1_activity = string.Empty,
                    menit_1_respiration = string.Empty,
                    menit_5_appearance = string.Empty,
                    menit_5_pulse = string.Empty,
                    menit_5_grimace = string.Empty,
                    menit_5_activity = string.Empty,
                    menit_5_respiration = string.Empty,
                    usia_kehamilan = string.Empty,
                    gravida = string.Empty,
                    partus = string.Empty,
                    abortus = string.Empty,
                    onset_kontraksi = string.Empty,
                    delivery = string.Empty,
                    tarif_poli_eks = "0",
                    nama_dokter = pmedic.ParamedicName,
                    kode_tarif = std.ItemID,
                    payor_id = "3",
                    payor_cd = "JKN",
                    cob_cd = string.Empty,
                    coder_nik = ConfigurationManager.AppSettings["InacbgUserID"]
                });
                if (detail.Metadata.IsValid)
                {
                    svc = new Common.Inacbg.v51.Service();
                    var grouper1 = svc.Grouper1(new Common.Inacbg.v51.Grouper.Grouper1.Data() { nomor_sep = reg.RegistrationNo });
                    if (grouper1.Metadata.IsValid)
                    {
                        var data = grouper1.Response;
                        var cbg = data.Cbg;
                        ncc = new NccInacbg();
                        if (ncc.LoadByPrimaryKey(reg.RegistrationNo))
                        {
                            ncc.AddPaymentAmt = 0;
                            ncc.CoverageAmount = Convert.ToDecimal(cbg.Tariff);
                            ncc.CbgID = cbg.Code;
                            ncc.CbgName = cbg.Description;
                            ncc.Save();
                        }

                        reg.ApproximatePlafondAmount = Convert.ToDecimal(Convert.ToDecimal(cbg.Tariff));
                        if (reg.PlavonAmount == 0)
                        {
                            reg.PlavonAmount = Convert.ToDecimal(cbg.Tariff);
                            reg.Save();
                        }

                        var cov = new RegistrationApproximateCoverageDetail();
                        if (!cov.LoadByPrimaryKey(reg.RegistrationNo, reg.CoverageClassID)) cov = new RegistrationApproximateCoverageDetail();
                        cov.RegistrationNo = reg.RegistrationNo;
                        cov.ClassID = reg.CoverageClassID;
                        cov.CoverageAmount = Convert.ToDecimal(cbg.Tariff);
                        cov.CalculatedAmount = 0;
                        cov.LastUpdateDateTime = DateTime.Now;
                        cov.LastUpdateByUserID = "WEBSERVICE";
                        cov.Save();

                    }
                }
            }
        }

        [WebMethod(EnableSession = true)]
        public string LoadGroupperAll()
        {
            var regs = new RegistrationCollection();
            regs.Query.Where(regs.Query.SRRegistrationType == AppConstant.RegistrationType.InPatient, regs.Query.DischargeDate.IsNull(), regs.Query.IsVoid == false, regs.Query.IsClosed == false, regs.Query.GuarantorID.In(AppSession.Parameter.GuarantorAskesID));
            regs.Query.Load();
            foreach (var reg in regs) LoadGroupper(reg.RegistrationNo);
            return "success";
        }

        [WebMethod(EnableSession = true)]
        public string GroupperOutpatientByDate(string date)
        {
            var regs = new RegistrationCollection();
            regs.Query.Where(regs.Query.SRRegistrationType.In(AppConstant.RegistrationType.OutPatient, AppConstant.RegistrationType.EmergencyPatient), //regs.Query.RegistrationDate.Date() == DateTime.Now.Date,
                regs.Query.GuarantorID.In(AppSession.Parameter.GuarantorAskesID),
                regs.Query.IsVoid == false,
                regs.Query.IsFromDispensary == false,
                regs.Query.GuarantorCardNo.IsNotNull(),
                regs.Query.GuarantorCardNo != string.Empty,
                regs.Query.BpjsSepNo.IsNotNull(),
                regs.Query.BpjsSepNo != string.Empty,
                regs.Query.BpjsSepNo != "0",
                regs.Query.RegistrationDate == DateTime.ParseExact(date, "yyyy-MM-dd", null, DateTimeStyles.None).Date);
            regs.Query.Load();
            var count = 0;
            foreach (var reg in regs)
            {
                try
                {
                    GroupperOutpatient(reg.RegistrationNo);
                    count++;
                }
                catch (Exception e)
                {
                    var log = new WebServiceAPILog();
                    log.Query.Where(log.Query.IPAddress == "EklaimGroupper", log.Query.UrlAddress == reg.RegistrationNo, log.Query.Params == reg.BpjsSepNo);
                    log.Query.es.Top = 1;
                    if (log.Query.Load())
                    {
                        log = new WebServiceAPILog
                        {
                            DateRequest = DateTime.Now,
                            IPAddress = "EklaimGroupper",
                            UrlAddress = reg.RegistrationNo,
                            Params = reg.BpjsSepNo,
                            Totalms = 0
                        };
                    }
                    log.Response = e.Message;
                    log.Save();
                }
            }

            return $"success, {count} data";
        }

        [WebMethod(EnableSession = true)]
        public string GroupperOutpatient(string registrationNo)
        {
            var tariff = new AppStandardReferenceItem();
            tariff.Query.es.Top = 1;
            tariff.Query.Where(tariff.Query.StandardReferenceID == AppEnum.StandardReference.BpjsTariffType.ToString(), tariff.Query.IsActive == true);
            tariff.Query.Load();

            var grr = new GuarantorQuery("a");
            var brid = new GuarantorBridgingQuery("b");

            grr.es.Distinct = true;
            grr.Select(grr.GuarantorID, grr.GuarantorName, brid.BridgingID, brid.BridgingCode);
            grr.InnerJoin(brid).On(grr.GuarantorID == brid.GuarantorID && brid.SRBridgingType == AppEnum.BridgingType.INACBG.ToString());
            var guarantors = grr.LoadDataTable();
            if (guarantors.Rows.Count == 0) return "not ok";

            var jaminans = (from DataRow row in guarantors.Rows select row["GuarantorID"].ToString() + "|" + row["BridgingID"].ToString() + "|" + row["BridgingCode"].ToString()).ToList();

            var reg = new Registration();
            reg.Query.Where(reg.Query.SRRegistrationType.In(AppConstant.RegistrationType.OutPatient, AppConstant.RegistrationType.EmergencyPatient), //regs.Query.RegistrationDate.Date() == DateTime.Now.Date,
                reg.Query.GuarantorID.In(AppSession.Parameter.GuarantorAskesID),
                reg.Query.IsVoid == false,
                reg.Query.IsFromDispensary == false,
                reg.Query.GuarantorCardNo.IsNotNull(),
                reg.Query.GuarantorCardNo != string.Empty,
                reg.Query.BpjsSepNo.IsNotNull(),
                reg.Query.BpjsSepNo != string.Empty,
                reg.Query.BpjsSepNo != "0",
                reg.Query.RegistrationNo == registrationNo);
            if (!reg.Query.Load())
            {
                reg = new Registration();
                reg.Query.Where(
                    reg.Query.SRRegistrationType.In(AppConstant.RegistrationType.OutPatient, AppConstant.RegistrationType.EmergencyPatient), //regs.Query.RegistrationDate.Date() == DateTime.Now.Date,
                    reg.Query.GuarantorID.In(AppSession.Parameter.GuarantorAskesID),
                    reg.Query.IsVoid == false,
                    reg.Query.IsFromDispensary == false,
                    reg.Query.GuarantorCardNo.IsNotNull(),
                    reg.Query.GuarantorCardNo != string.Empty,
                    reg.Query.BpjsSepNo.IsNotNull(),
                    reg.Query.BpjsSepNo != string.Empty,
                    reg.Query.BpjsSepNo != "0",
                    reg.Query.BpjsSepNo == registrationNo);
                if (reg.Query.Load()) registrationNo = reg.RegistrationNo;
            }

            var log = new WebServiceAPILog
            {
                DateRequest = DateTime.Now,
                IPAddress = "EklaimGroupper",
                UrlAddress = reg.RegistrationNo,
                Params = reg.BpjsSepNo,
                Totalms = 0
            };

            if (string.IsNullOrWhiteSpace(reg.RegistrationNo) || string.IsNullOrWhiteSpace(reg.BpjsSepNo) || reg.BpjsSepNo == "0")
            {
                log.Response = string.Format("Data registrasi / SEP tidak valid. Input: {0}", registrationNo);
                log.Save();
                return string.Format("not ok - {0}", log.Response);
            }

            var pmedic = new Paramedic();
            pmedic.LoadByPrimaryKey(reg.ParamedicID);

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);

            var medicalNo = !string.IsNullOrWhiteSpace(AppSession.Parameter.EklaimRemoveDashSeparatorOnMedicalNo) && AppSession.Parameter.EklaimRemoveDashSeparatorOnMedicalNo.ToLower() == "yes" ? patient.MedicalNo.Replace("-", string.Empty) : patient.MedicalNo;
            switch (AppSession.Parameter.HealthcareInitialAppsVersion)
            {
                case "YBRSGKP":
                    var pat = new Patient();
                    pat.LoadByMedicalNo(patient.MedicalNo);
                    if (!string.IsNullOrEmpty(pat.OldMedicalNo)) medicalNo = medicalNo.ToInt().ToString();
                    medicalNo = medicalNo.ToInt().ToString();
                    break;
            }

            var sep = new BpjsSEP();
            sep.Query.es.Top = 1;
            sep.Query.Where(sep.Query.NoSEP == reg.BpjsSepNo);
            if (!sep.Query.Load())
            {
                log.Response = "Data SEP tidak ditemukan";
                log.Save();
                return "not ok - sep tidak ditemukan";
            }

            var mds = new MedicalDischargeSummary();
            if (!mds.LoadByPrimaryKey(reg.RegistrationNo))
            {
                log.Response = "Data MDS tidak ditemukan";
                log.Save();
                return "not ok -  data mds tidak ditemukan";
            }

            var discharge = string.Empty;
            if (reg.SRRegistrationType == AppConstant.RegistrationType.EmergencyPatient) discharge = "E03";
            else if (reg.SRRegistrationType == AppConstant.RegistrationType.OutPatient) discharge = "O07";
            else discharge = "I01";

            var asri = new AppStandardReferenceItem();
            asri.Query.Where(asri.Query.StandardReferenceID == AppEnum.StandardReference.DischargeMethod, asri.Query.ItemID == (string.IsNullOrWhiteSpace(mds.SRDischargeMethod) ? discharge : mds.SRDischargeMethod));
            if (!asri.Query.Load())
            {
                log.Response = "Data MDS DischargeMethod tidak ditemukan : " + mds.SRDischargeMethod;
                log.Save();
                return "not ok - data mds dischargemethod tidak ditemukan";
            }

            var registrationNos = Helper.MergeBilling.GetFullMergeRegistration(reg.RegistrationNo);

            var bill = new IntermBillCollection();
            bill.Query.Where(bill.Query.RegistrationNo.In(registrationNos), bill.Query.IsVoid == false);
            if (!bill.Query.Load())
            {
                log.Response = "Data Interm Bill tidak ditemukan";
                log.Save();
                return "not ok - data intermbill tidak ditemukan";
            }

            var admin = bill.Select(b => b.AdministrationAmount + b.GuarantorAdministrationAmount).Sum();

            var cc = new CostCalculationQuery("a");
            var i = new ItemQuery("b");

            cc.Select(i.SREklaimTariffGroup, i.SREklaimFactorGroup, (cc.PatientAmount.Sum() + cc.GuarantorAmount.Sum()).As("Amount"));
            cc.InnerJoin(i).On(cc.ItemID == i.ItemID);
            cc.Where(cc.IntermBillNo.In(bill.Select(b => b.IntermBillNo)));
            cc.GroupBy(i.SREklaimTariffGroup, i.SREklaimFactorGroup);

            var tbl = cc.LoadDataTable();
            if (tbl.Rows.Count == 0)
            {
                log.Response = "Data Billing tidak ditemukan";
                log.Save();
                return "not ok - data billing tidak ditemukan";
            }

            var total = tbl.AsEnumerable().Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var prosedur_non_bedah = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "01").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var tenaga_ahli = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "04").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var radiologi = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "07").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var rehabilitasi = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "10").Sum(t => t.Field<decimal?>("Amount"));
            var obat = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "13").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var sewa_alat = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "16").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var prosedur_bedah = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "02").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var keperawatan = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "05").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var laboratorium = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "08").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var kamar_akomodasi = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "11").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var alkes = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "14").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var konsultasi = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "03").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var penunjang = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "06").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var pelayanan_darah = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "09").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var rawat_intensif = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "12").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var bmhp = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "15").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var obat_kronis = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "17").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var obat_kemoterapi = tbl.AsEnumerable().Where(t => t.Field<string>("SREklaimTariffGroup") == "18").Sum(t => t.Field<decimal?>("Amount")) ?? 0;
            var lab_asam_laktat = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_asam_laktat");
            var lab_procalcitonin = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_procalcitonin");
            var lab_crp = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_crp");
            var lab_kultur = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_kultur");
            var lab_d_dimer = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_d_dimer");
            var lab_pt = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_pt");
            var lab_aptt = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_aptt");
            var lab_waktu_pendarahan = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_waktu_pendarahan");
            var lab_anti_hiv = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_anti_hiv");
            var lab_analisa_gas = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_analisa_gas");
            var lab_albumin = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "lab_albumin");
            var rad_thorax_ap_pa = !tbl.AsEnumerable().Any(t => t.Field<string>("SREklaimFactorGroup") == "rad_thorax_ap_pa");

            var svc54 = new Common.Inacbg.v510.Service();
            var param51 = new Common.Inacbg.v510.Claim.Create.Data()
            {
                nomor_kartu = reg.GuarantorCardNo,
                nomor_sep = reg.BpjsSepNo,
                nomor_rm = medicalNo,
                nama_pasien = patient.PatientName,
                tgl_lahir = patient.DateOfBirth?.ToString("yyyy-MM-dd HH:mm:ss"),
                gender = (patient.Sex == "M" ? "1" : "2")
            };
            var response51 = svc54.Insert(param51);
            if (!response51.Metadata.IsValid && response51.Metadata.ErrorNo == "E2007") // Duplikasi nomor SEP
                response51.Metadata.Code = "200";

            if (!response51.Metadata.IsDuplicate && !response51.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(param51);
                log.Response = JsonConvert.SerializeObject(response51);
                log.Save();
                return "not ok - duplikat nomor SEP";
            }

            var ncc = new NccInacbg();
            if (!ncc.LoadByPrimaryKey(reg.RegistrationNo))
            {
                ncc = new NccInacbg();
                ncc.RegistrationNo = reg.RegistrationNo;
                ncc.PatientId = response51.Response.PatientId;
                ncc.AdmissionId = response51.Response.AdmissionId;
                ncc.HospitalAdmissionId = response51.Response.HospitalAdmissionId;
                ncc.LastUpdateDateTime = DateTime.Now;
                ncc.LastUpdateByUserID = AppSession.UserLogin.UserID;
                ncc.AddPaymentAmt = 0;
                ncc.Save();

                SaveNccIdrg(registrationNo, "ClaimData", param51, response51);
            }

            var diagnose = EpisodeDiagnoses(reg.RegistrationNo).OrderBy(e => e.SRDiagnoseType).Aggregate(string.Empty, (current, d) => current + (d.DiagnoseID + "#"));
            if (string.IsNullOrEmpty(diagnose)) diagnose = "#";

            var procedure = BuildProcedureStringWithQty(EpisodeProcedures(reg.RegistrationNo));
            if (string.IsNullOrEmpty(procedure)) procedure = "#";

            var sistolic = 0;
            var diastolic = 0;
            var merge = Registration.RelatedRegistrations(reg.RegistrationNo);
            var table = VitalSign.VitalSignLastValue(reg.RegistrationNo, merge, true, DateTime.Now.Date);
            if (table.Rows.Count > 0)
            {
                var row_sistolic = table.AsEnumerable().SingleOrDefault(t => t.Field<string>("VitalSignID") == "BP1");
                if (row_sistolic != null) sistolic = row_sistolic["QuestionAnswerText"].ToString().ToInt();
                var row_diastolic = table.AsEnumerable().SingleOrDefault(t => t.Field<string>("VitalSignID") == "BP2");
                if (row_diastolic != null) diastolic = row_diastolic["QuestionAnswerText"].ToString().ToInt();
            }
            else
            {
                var nthd = new NursingTransHD();
                nthd.Query.es.Top = 1;
                nthd.Query.Where(nthd.Query.RegistrationNo == reg.RegistrationNo);
                nthd.Query.OrderBy(nthd.Query.TransactionNo.Ascending);
                if (nthd.Query.Load())
                {
                    var ndtdt = new NursingDiagnosaTransDTCollection();
                    ndtdt.Query.Where(ndtdt.Query.TransactionNo == nthd.TransactionNo, ndtdt.Query.ReferenceToPhrNo.IsNotNull());
                    ndtdt.Query.OrderBy(ndtdt.Query.ID.Ascending);
                    if (ndtdt.Query.Load())
                    {
                        foreach (var item in ndtdt)
                        {
                            var phrl = new PatientHealthRecordLineCollection();
                            if (AppSession.Parameter.HealthcareInitialAppsVersion == "YBRSGKP")
                            {
                                phrl.Query.Where(phrl.Query.TransactionNo == item.ReferenceToPhrNo, phrl.Query.RegistrationNo == reg.RegistrationNo, phrl.Query.QuestionID.In("KTHD1.038", "KTHD1.039"));
                                if (!phrl.Query.Load()) continue;
                                sistolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "KTHD1.038").QuestionAnswerNum ?? 0);
                                diastolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "KTHD1.039").QuestionAnswerNum ?? 0);
                                break;
                            }
                            else if (AppSession.Parameter.HealthcareInitialAppsVersion == "RSEE")
                            {
                                phrl.Query.Where(phrl.Query.TransactionNo == item.ReferenceToPhrNo, phrl.Query.RegistrationNo == reg.RegistrationNo, phrl.Query.QuestionID.In("A.KDV.1TDS", "A.KDV.2TDD"));
                                if (!phrl.Query.Load()) continue;
                                sistolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "A.KDV.1TDS").QuestionAnswerNum ?? 0);
                                diastolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "A.KDV.2TDD").QuestionAnswerNum ?? 0);
                                break;
                            }
                        }
                    }
                }
            }

            if (sistolic == 0 || diastolic == 0)
            {
                var nthd = new NursingTransHD();
                nthd.Query.es.Top = 1;
                nthd.Query.Where(nthd.Query.RegistrationNo == reg.RegistrationNo);
                nthd.Query.OrderBy(nthd.Query.TransactionNo.Ascending);
                if (nthd.Query.Load())
                {
                    var ndtdt = new NursingDiagnosaTransDTCollection();
                    ndtdt.Query.Where(ndtdt.Query.TransactionNo == nthd.TransactionNo, ndtdt.Query.ReferenceToPhrNo.IsNotNull());
                    ndtdt.Query.OrderBy(ndtdt.Query.ID.Ascending);
                    if (ndtdt.Query.Load())
                    {
                        foreach (var item in ndtdt)
                        {
                            var phrl = new PatientHealthRecordLineCollection();
                            if (AppSession.Parameter.HealthcareInitialAppsVersion != "YBRSGKP") continue;
                            phrl.Query.Where(phrl.Query.TransactionNo == item.ReferenceToPhrNo, phrl.Query.RegistrationNo == reg.RegistrationNo, phrl.Query.QuestionID.In("KTHD1.038", "KTHD1.039"));
                            if (!phrl.Query.Load()) continue;
                            sistolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "KTHD1.038").QuestionAnswerNum ?? 0);
                            diastolic = Convert.ToInt32(phrl.Single(p => p.QuestionID == "KTHD1.039").QuestionAnswerNum ?? 0);
                            break;
                        }
                    }
                }
            }

            var penjamin = string.Empty;
            foreach (var jaminan in jaminans)
            {
                if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "3") //JKN
                {
                    penjamin = jaminan;
                    break;
                }
                else if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "71") //JAMINAN COVID-19
                {
                    penjamin = jaminan;
                    break;
                }
                else if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "72") //JAMINAN KIPI
                {
                    penjamin = jaminan;
                    break;
                }
                else if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "73") //JAMINAN BAYI BARU LAHIR
                {
                    penjamin = jaminan;
                    break;
                }
                else if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "74") //JAMINAN PERPANJANGAN MASA RAWAT
                {
                    penjamin = jaminan;
                    break;
                }
                else if (jaminan.Split('|')[0] == reg.GuarantorID && jaminan.Split('|')[1] == "75") //JAMINAN CO-INSIDENSE
                {
                    penjamin = jaminan;
                    break;
                }
            }

            svc54 = new Common.Inacbg.v510.Service();
            var params54 = new Common.Inacbg.v510.Detail.Datass()
            {
                nomor_sep = reg.BpjsSepNo,
                nomor_kartu = reg.GuarantorCardNo,
                tgl_masuk = reg.RegistrationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                tgl_pulang = reg.RegistrationDate?.ToString("yyyy-MM-dd HH:mm:ss"),
                cara_masuk = reg.SRRegistrationType == AppConstant.RegistrationType.OutPatient ? "gp" : "other", //
                jenis_rawat = "2",
                kelas_rawat = "3",
                adl_sub_acute = string.Empty,
                adl_chronic = string.Empty,
                icu_indikator = "0",
                icu_los = "0",
                ventilator_hour = "0",

                use_ind = "0",
                start_dttm = string.Empty,
                stop_dttm = string.Empty,

                upgrade_class_ind = "0",
                upgrade_class_class = string.Empty,
                upgrade_class_los = string.Empty,
                upgrade_class_payor = string.Empty,
                add_payment_pct = "0",
                birth_weight = "0",
                sistole = sistolic.ToString(),
                diastole = diastolic.ToString(),
                discharge_status = asri.NumericValue.ToString(),
                //diagnosa = diagnose,
                //procedure = procedure,
                //diagnosa_inagrouper = diagnose,
                //procedure_inagrouper = procedure,
                tarif_rs = new TarifRss()
                {
                    prosedur_non_bedah = prosedur_non_bedah.ToInt().ToString(),
                    prosedur_bedah = prosedur_bedah.ToInt().ToString(),
                    konsultasi = konsultasi.ToInt().ToString(),
                    tenaga_ahli = tenaga_ahli.ToInt().ToString(),
                    keperawatan = keperawatan.ToInt().ToString(),
                    penunjang = penunjang.ToInt().ToString(),
                    radiologi = radiologi.ToInt().ToString(),
                    laboratorium = laboratorium.ToInt().ToString(),
                    pelayanan_darah = pelayanan_darah.ToInt().ToString(),
                    rehabilitasi = rehabilitasi.ToInt().ToString(),
                    kamar = kamar_akomodasi.ToInt().ToString(),
                    rawat_intensif = rawat_intensif.ToInt().ToString(),
                    obat = obat.ToInt().ToString(),
                    obat_kronis = obat_kronis.ToInt().ToString(),
                    obat_kemoterapi = obat_kemoterapi.ToInt().ToString(),
                    alkes = alkes.ToInt().ToString(),
                    bmhp = bmhp.ToInt().ToString(),
                    sewa_alat = sewa_alat.ToInt().ToString()
                },
                pemulasaraan_jenazah = "0",
                kantong_jenazah = "0",
                peti_jenazah = "0",
                plastik_erat = "0",
                desinfektan_jenazah = "0",
                mobil_jenazah = "0",
                desinfektan_mobil_jenazah = "0",
                covid19_status_cd = string.Empty,
                nomor_kartu_t = "kartu_jkn",
                episodes = string.Empty,
                covid19_cc_ind = string.Empty,
                covid19_rs_darurat_ind = string.Empty, //
                covid19_co_insidense_ind = string.Empty, //
                covid19_penunjang_pengurang = new Covid19PenunjangPengurang()
                {
                    lab_asam_laktat = lab_asam_laktat ? "1" : "0", //
                    lab_procalcitonin = lab_procalcitonin ? "1" : "0", //
                    lab_crp = lab_crp ? "1" : "0", //
                    lab_kultur = lab_kultur ? "1" : "0", //
                    lab_d_dimer = lab_d_dimer ? "1" : "0", //
                    lab_pt = lab_pt ? "1" : "0", //
                    lab_aptt = lab_aptt ? "1" : "0", //
                    lab_waktu_pendarahan = lab_waktu_pendarahan ? "1" : "0", //
                    lab_anti_hiv = lab_anti_hiv ? "1" : "0", //
                    lab_analisa_gas = lab_analisa_gas ? "1" : "0", //
                    lab_albumin = lab_albumin ? "1" : "0", //
                    rad_thorax_ap_pa = rad_thorax_ap_pa ? "1" : "0" //
                },
                terapi_konvalesen = "0", //
                akses_naat = "A", //
                isoman_ind = "1", //
                bayi_lahir_status_cd = string.Empty, //

                dializer_single_use = "1",
                kantong_darah = pelayanan_darah.ToInt() == 0 ? "0" :
                    (pelayanan_darah.ToInt() > 0 && pelayanan_darah.ToInt() <= 1000000 ? "1" :
                        ((pelayanan_darah.ToInt() > 1000000 && pelayanan_darah.ToInt() <= 1800000 ? "2" :
                        "3"))),
                menit_1_appearance = string.Empty,
                menit_1_pulse = string.Empty,
                menit_1_grimace = string.Empty,
                menit_1_activity = string.Empty,
                menit_1_respiration = string.Empty,
                menit_5_appearance = string.Empty,
                menit_5_pulse = string.Empty,
                menit_5_grimace = string.Empty,
                menit_5_activity = string.Empty,
                menit_5_respiration = string.Empty,
                usia_kehamilan = string.Empty,
                gravida = string.Empty,
                partus = string.Empty,
                abortus = string.Empty,
                onset_kontraksi = string.Empty,
                delivery = string.Empty,

                tarif_poli_eks = "0",
                nama_dokter = pmedic.ParamedicName,
                kode_tarif = tariff.ItemID,
                payor_id = penjamin.Split('|')[1],
                payor_cd = penjamin.Split('|')[2],
                cob_cd = string.Empty,
                coder_nik = ConfigurationManager.AppSettings["InacbgUserID"]
            };
            var response54 = svc54.Insert(params54);
            //if (response54.Metadata.ErrorNo == "E2102") response54.Metadata.Code = "200"; // idrg final
            if (!response54.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(params54);
                log.Response = JsonConvert.SerializeObject(response54);
                log.Save();
                return string.Format("not ok - insert params54 {0}", log.Response);
            }

            svc54 = new Common.Inacbg.v510.Service();
            var paramDiag54 = new Common.Inacbg.v510.Diagnose.Data
            {
                nomor_sep = reg.BpjsSepNo,
                diagnosa = diagnose
            };
            var responseDiag54 = svc54.IDRGSetDiagnose(paramDiag54);
            //if (responseDiag54.Metadata.ErrorNo == "E2102") responseDiag54.Metadata.Code = "200"; // idrg final
            if (!responseDiag54.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramDiag54);
                log.Response = JsonConvert.SerializeObject(responseDiag54);
                log.Save();
                return string.Format("not ok - 1009 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgDiagnosaSet", paramDiag54, responseDiag54);

            svc54 = new Common.Inacbg.v510.Service();
            var paramGetDiag54 = new Common.Inacbg.v510.Diagnose.Get.Data { nomor_sep = reg.BpjsSepNo };
            var responseGetDiag54 = svc54.IDRGGetDiagnose(paramGetDiag54);
            //if (responseGetDiag54.Metadata.ErrorNo == "E2102") responseGetDiag54.Metadata.Code = "200"; // idrg final
            if (!responseGetDiag54.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramGetDiag54);
                log.Response = JsonConvert.SerializeObject(responseGetDiag54);
                log.Save();
                return string.Format("not ok - 1023 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgDiagnosaGet", paramGetDiag54, responseGetDiag54);

            svc54 = new Common.Inacbg.v510.Service();
            var paramProc54 = new Common.Inacbg.v510.Procedure.Data
            {
                nomor_sep = reg.BpjsSepNo,
                procedure = procedure
            };
            var responseProc54 = svc54.IDRGSetProcedure(paramProc54);
            //if (responseProc54.Metadata.ErrorNo == "E2102") responseProc54.Metadata.Code = "200"; // idrg final
            if (!responseProc54.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramProc54);
                log.Response = JsonConvert.SerializeObject(responseProc54);
                log.Save();
                return string.Format("not ok - 1041 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgProcedureSet", paramProc54, responseProc54);

            svc54 = new Common.Inacbg.v510.Service();
            var paramGetProc54 = new Common.Inacbg.v510.Procedure.Get.Data { nomor_sep = reg.BpjsSepNo };
            var responseGetProc54 = svc54.IDRGGetProcedure(paramGetProc54);
            //if (responseGetProc54.Metadata.ErrorNo == "E2102") responseGetProc54.Metadata.Code = "200"; // idrg final
            if (!responseGetProc54.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramGetProc54);
                log.Response = JsonConvert.SerializeObject(responseGetProc54);
                log.Save();
                return string.Format("not ok - 1055 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgProcedureGet", paramGetProc54, responseGetProc54);

            svc54 = new Common.Inacbg.v510.Service();
            var paramIdrgGroup = new Common.Inacbg.v510.Gruoper.IdrgGrouper.Data { nomor_sep = reg.BpjsSepNo };
            var responseIdrgGroup = svc54.IdrgGrouper(paramIdrgGroup);
            //if (responseIdrgGroup.Meta.ErrorNo == "E2102") responseIdrgGroup.Meta.Code = "200"; // idrg final
            if (!responseIdrgGroup.Meta.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramIdrgGroup);
                log.Response = JsonConvert.SerializeObject(responseIdrgGroup);
                log.Save();
                return string.Format("not ok - 1069 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgGroup", paramIdrgGroup, responseIdrgGroup);

            svc54 = new Common.Inacbg.v510.Service();
            var paramIdrgFinal = new Common.Inacbg.v510.Gruoper.IdrgGrouper.Data { nomor_sep = reg.BpjsSepNo };
            var responseIdrgFinal = svc54.FinalIdrgGrouper(paramIdrgFinal);
            //if (responseIdrgFinal.Meta.ErrorNo == "E2102") responseIdrgFinal.Meta.Code = "200"; // idrg final
            if (!responseIdrgFinal.Meta.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramIdrgFinal);
                log.Response = JsonConvert.SerializeObject(responseIdrgFinal);
                log.Save();
                return string.Format("not ok - 1083 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "IdrgFinal", paramIdrgFinal, responseIdrgFinal);

            svc54 = new Common.Inacbg.v510.Service();
            var paramImport54 = new Common.Inacbg.v510.Gruoper.importcoding.Data
            {
                nomor_sep = reg.BpjsSepNo
            };
            var responseImport54 = svc54.ImportIdrgToInacbg(paramImport54);
            if (!responseImport54.Meta.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramImport54);
                log.Response = JsonConvert.SerializeObject(responseImport54);
                log.Save();
                return string.Format("not ok - 1100 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "ImportIdrgToInacbg", paramImport54, responseImport54);

            svc54 = new Common.Inacbg.v510.Service();
            var paramGetDiag = new Common.Inacbg.v510.Diagnose.Get.Data { nomor_sep = reg.BpjsSepNo };
            var responseGetDiag = svc54.InacbgGetDiagnose(paramGetDiag);
            if (!responseGetDiag.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramGetDiag);
                log.Response = JsonConvert.SerializeObject(responseGetDiag);
                log.Save();
                return string.Format("not ok - 1113 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "InacbgDiagnosaGet", paramGetDiag, responseGetDiag);

            using (var trans = new esTransactionScope())
            {
                var oldDiagInas = new EpisodeDiagnoseInaGroupperCollection();
                oldDiagInas.Query.Where(oldDiagInas.Query.RegistrationNo == registrationNo);
                if (oldDiagInas.Query.Load())
                {
                    oldDiagInas.MarkAllAsDeleted();
                    oldDiagInas.Save();
                }

                var diagInas = new EpisodeDiagnoseInaGroupperCollection();
                foreach (var diag in responseGetDiag.Data.Expanded.Where(d => d.ValidCode == "1"))
                {
                    var diagIna = diagInas.AddNew();
                    diagIna.RegistrationNo = registrationNo;
                    diagIna.SequenceNo = diag.No;
                    diagIna.DiagnoseID = diag.Code;
                    diagIna.SRDiagnoseType = diag.No == "1" ? "DiagnoseType-001" : "DiagnoseType-003";
                    diagIna.DiagnosisText = diag.Display;
                    diagIna.MorphologyID = string.Empty;
                    diagIna.ParamedicID = string.Empty;
                    diagIna.IsAcuteDisease = false;
                    diagIna.IsChronicDisease = false;
                    diagIna.IsOldCase = false;
                    diagIna.IsConfirmed = true;
                    diagIna.IsVoid = false;
                    diagIna.Notes = string.Empty;
                    diagIna.LastUpdateDateTime = DateTime.Now;
                    diagIna.LastUpdateByUserName = "idrg";
                    diagIna.ExternalCauseID = string.Empty;
                    diagIna.CreateDateTime = DateTime.Now;
                    diagIna.CreateByUserID = "idrg";
                    diagIna.DiagnoseSynonym = string.Empty;
                }
                if (diagInas.Any()) diagInas.Save();

                trans.Complete();
            }

            svc54 = new Common.Inacbg.v510.Service();
            var paramGetProc = new Common.Inacbg.v510.Procedure.Get.Data { nomor_sep = reg.BpjsSepNo };
            var responseGetProc = svc54.InacbgGetProcedure(paramGetProc);
            if (!responseGetProc.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramGetProc);
                log.Response = JsonConvert.SerializeObject(responseGetProc);
                log.Save();
                return string.Format("not ok - 1152 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "InacbgProcedureGet", paramGetProc, responseGetProc);

            using (var trans = new esTransactionScope())
            {
                var oldDiagProcs = new EpisodeProcedureInaGroupperCollection();
                oldDiagProcs.Query.Where(oldDiagProcs.Query.RegistrationNo == registrationNo);
                if (oldDiagProcs.Query.Load())
                {
                    oldDiagProcs.MarkAllAsDeleted();
                    oldDiagProcs.Save();
                }

                var diagProcs = new EpisodeProcedureInaGroupperCollection();
                foreach (var proc in responseGetProc.Data.Expanded.Where(d => d.ValidCode == "1"))
                {
                    var diagProc = diagProcs.AddNew();
                    diagProc.RegistrationNo = registrationNo;
                    diagProc.SequenceNo = proc.No;
                    diagProc.ProcedureDate = DateTime.Now.Date;
                    diagProc.ProcedureTime = string.Empty;
                    diagProc.ProcedureDate2 = DateTime.Now.Date;
                    diagProc.ProcedureTime2 = string.Empty;
                    diagProc.ParamedicID = string.Empty;
                    diagProc.ParamedicID2 = string.Empty;
                    diagProc.ProcedureID = proc.Code;
                    diagProc.SRProcedureCategory = string.Empty;
                    diagProc.SRAnestesi = string.Empty;
                    diagProc.RoomID = string.Empty;
                    diagProc.IsCito = false;
                    diagProc.IsVoid = false;
                    diagProc.LastUpdateDateTime = DateTime.Now;
                    diagProc.LastUpdateByUserName = "idrg";
                    diagProc.AssistantID1 = string.Empty;
                    diagProc.AssistantID2 = string.Empty;
                    diagProc.Notes = string.Empty;
                    diagProc.BookingNo = string.Empty;
                    diagProc.ParamedicID2a = string.Empty;
                    diagProc.ParamedicID3a = string.Empty;
                    diagProc.ParamedicID4a = string.Empty;
                    diagProc.ParamedicIDAnestesi = string.Empty;
                    diagProc.AssistantIDAnestesi = string.Empty;
                    diagProc.InstrumentatorID1 = string.Empty;
                    diagProc.InstrumentatorID2 = string.Empty;
                    diagProc.IsFromOperatingRoom = true;
                    diagProc.CreateDateTime = DateTime.Now;
                    diagProc.CreateByUserID = "idrg";
                    diagProc.AnestesyNotes = string.Empty;
                    diagProc.ProcedureName = proc.Display;
                    diagProc.OpNotesSeqNo = string.Empty;
                    diagProc.OperatingNotes = string.Empty;
                    diagProc.ProcedureSynonym = string.Empty;
                }
                if (diagProcs.Any()) diagProcs.Save();

                trans.Complete();
            }

            svc54 = new Common.Inacbg.v510.Service();
            var paramsGrouper = new Common.Inacbg.v510.Gruoper.Grouper1.Data() { nomor_sep = reg.BpjsSepNo };
            var responseGrouper = svc54.InacbgGrouper1(paramsGrouper);
            if (!responseGrouper.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramsGrouper);
                log.Response = JsonConvert.SerializeObject(responseGrouper);
                log.Save();
                return string.Format("not ok - 1207 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "InacbgStage1", paramsGrouper, responseGrouper);

            ncc = new NccInacbg();
            if (!ncc.LoadByPrimaryKey(reg.RegistrationNo))
            {
                log.Response = "Data NCC tidak ditemukan";
                log.Save();
                return string.Format("not ok - 1218 {0}", log.Response);
            }
            if (responseGrouper.ResponseInacbg == null)
            {
                log.Response = "Response Inacbg null";
                log.Save();
                return string.Format("not ok - 1225 {0}", log.Response);
            }

            ncc.AddPaymentAmt = string.IsNullOrEmpty(responseGrouper.ResponseInacbg.AddPaymentAmt) ? 0 : Convert.ToDecimal(responseGrouper.ResponseInacbg.AddPaymentAmt);
            ncc.CoverageAmount = Convert.ToDecimal(responseGrouper.ResponseInacbg.Cbg.Tariff);
            ncc.CbgID = responseGrouper.ResponseInacbg.Cbg.Code;
            ncc.CbgName = responseGrouper.ResponseInacbg.Cbg.Description;
            ncc.Save();

            if (AppSession.Parameter.HealthcareInitial == "RSI")
            {
                if (reg.PlavonAmount == 0)
                {
                    reg.PlavonAmount = Convert.ToDecimal(responseGrouper.ResponseInacbg.Cbg.Tariff) == 0 ? Convert.ToDecimal(responseGrouper.ResponseInacbg.Tariff) : Convert.ToDecimal(responseGrouper.ResponseInacbg.Cbg.Tariff);
                    reg.Save();
                }
            }

            svc54 = new Common.Inacbg.v510.Service();
            var paramsFinal = new Common.Inacbg.v510.Gruoper.IdrgGrouper.Data { nomor_sep = reg.BpjsSepNo };
            var responseFinal = svc54.FinalInacbgGrouper(paramsFinal);
            if (!responseFinal.Meta.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramsFinal);
                log.Response = JsonConvert.SerializeObject(responseFinal);
                log.Save();
                return string.Format("not ok - 1249 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "InacbgFinal", paramsFinal, responseFinal);

            svc54 = new Common.Inacbg.v510.Service();
            var paramsFinalClaim = new Common.Inacbg.v510.Claim.Final.Data()
            {
                nomor_sep = reg.BpjsSepNo,
                coder_nik = ConfigurationManager.AppSettings["InacbgUserID"]
            };
            var responseFinalClaim = svc54.Final(paramsFinalClaim);
            if (!responseFinalClaim.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramsFinalClaim);
                log.Response = JsonConvert.SerializeObject(responseFinalClaim);
                log.Save();
                return string.Format("not ok - 1266 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "ClaimFinal", paramsFinalClaim, responseFinalClaim);

            ncc.CbgStatus = "final";
            ncc.Save();

            svc54 = new Common.Inacbg.v510.Service();
            var paramsKirim = new Common.Inacbg.v510.Claim.Create.Data() { nomor_sep = reg.BpjsSepNo };
            var responseKirim = svc54.Send(paramsKirim);
            if (!responseKirim.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramsKirim);
                log.Response = JsonConvert.SerializeObject(responseKirim);
                log.Save();
                return string.Format("not ok - 1282 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "ClaimSend", paramsKirim, responseKirim);

            ncc.CbgSentStatus = responseKirim.Response.Data.First().KemkesDcStatus;
            ncc.Save();

            svc54 = new Common.Inacbg.v510.Service();
            var paramDetail = new Common.Inacbg.v510.Claim.Get.GetDetail.Data() { nomor_sep = reg.BpjsSepNo };
            var responseDetail = svc54.GetDetail(paramDetail);
            if (!responseDetail.Metadata.IsValid)
            {
                log.Params = JsonConvert.SerializeObject(paramDetail);
                log.Response = JsonConvert.SerializeObject(responseDetail);
                log.Save();
                return string.Format("not ok - 1298 {0}", log.Response);
            }

            SaveNccIdrg(registrationNo, "GetClaimData", paramDetail, responseDetail);

            //svc = new Common.Inacbg.v51.Service();
            //var print = svc.Print(new Common.Inacbg.v51.Claim.Create.Data()
            //{
            //    nomor_sep = reg.BpjsSepNo
            //});
            //if (!kirim.Metadata.IsValid) continue;

            log.Response = "ok";
            log.Save();

            return "ok";
        }

        private static string BuildProcedureStringWithQty(IEnumerable<EpisodeProcedure> rows)
        {
            var tokens = rows
                .Where(x => x != null && x.IsVoid == false)
                .OrderBy(x => x.SequenceNo.ToInt())
                .Select(x =>
                {
                    var q = (x.QtyICD <= 0 ? 1 : x.QtyICD);
                    return q > 1 ? $"{x.ProcedureID}+{q}" : x.ProcedureID;
                })
                .ToList();

            return tokens.Count == 0 ? "#" : string.Join("#", tokens);
        }

        private void SaveNccIdrg(string registrationNo, string action, object reqObj, object resObj)
        {
            var row = new NccIDRG();
            if (!row.LoadByPrimaryKey(registrationNo))
                row = new NccIDRG { RegistrationNo = registrationNo };

            var reg = new Registration();
            reg.LoadByPrimaryKey(registrationNo);

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);

            row.MedicalNo = patient.MedicalNo;
            row.SEP = reg.BpjsSepNo;

            string reqJson = reqObj == null ? null : JsonConvert.SerializeObject(reqObj);
            string resJson = resObj == null ? null : JsonConvert.SerializeObject(resObj);

            switch (action)
            {
                case "ClaimData": row.ClaimDataRequest = reqJson; row.ClaimDataResponse = resJson; break;
                case "IdrgDiagnosaSet": row.IdrgDiagnosaSetReq = reqJson; row.IdrgDiagnosaSetRes = resJson; break;
                case "IdrgDiagnosaGet": row.IdrgDiagnosaGetReq = reqJson; row.IdrgDiagnosaGetRes = resJson; break;
                case "IdrgProcedureSet": row.IdrgProcedureSetReq = reqJson; row.IdrgProcedureSetRes = resJson; break;
                case "IdrgProcedureGet": row.IdrgProcedureGetReq = reqJson; row.IdrgProcedureGetRes = resJson; break;
                case "IdrgGroup": row.GroupingIdrgReq = reqJson; row.GroupingIdrgRes = resJson; break;
                case "IdrgFinal": row.FinalIdrgReq = reqJson; row.FinalIdrgRes = resJson; break;
                case "IdrgReEdit": row.ReEditIdrgReq = reqJson; row.ReEditIdrgRes = resJson; break;
                case "ImportIdrgToInacbg": row.IdrgToInacbgImportReq = reqJson; row.IdrgToInacbgImportRes = resJson; break;

                case "InacbgDiagnosaGet": row.InacbgDiagnosaGetReq = reqJson; row.InacbgDiagnosaGetRes = resJson; break;
                case "InacbgDiagnosaSet": row.InacbgDiagnosaSetReq = reqJson; row.InacbgDiagnosaSetRes = resJson; break;
                case "InacbgProcedureSet": row.InacbgProcedureSetReq = reqJson; row.InacbgProcedureSetRes = resJson; break;
                case "InacbgProcedureGet": row.InacbgProcedureGetReq = reqJson; row.InacbgProcedureGetRes = resJson; break;
                case "InacbgStage1": row.GroupingInacbgStage1Req = reqJson; row.GroupingInacbgStage1Res = resJson; break;
                case "InacbgStage2": row.GroupingInacbgStage2Req = reqJson; row.GroupingInacbgStage2Res = resJson; break;
                case "InacbgFinal": row.FinalInacbgReq = reqJson; row.FinalInacbgRes = resJson; break;
                case "InacbgReEdit": row.ReEditInacbgReq = reqJson; row.ReEditInacbgRes = resJson; break;
                case "ClaimFinal": row.ClaimFinalReq = reqJson; row.ClaimFinalRes = resJson; break;
                case "ClaimReEdit": row.ClaimReEditReq = reqJson; row.ClaimReEditRes = resJson; break;
                case "ClaimSend": row.ClaimSendReq = reqJson; row.ClaimSendRes = resJson; break;
                case "GetClaimData": row.GetClaimDataReq = reqJson; row.GetClaimDataRes = resJson; break;
                default:
                    break;
            }

            row.LastUpdateDateTime = DateTime.Now;
            row.LastUpdateByUserID = AppSession.UserLogin.UserID;
            row.Save();
        }

        private EpisodeDiagnoseCollection EpisodeDiagnoses(string registrationNo)
        {
            var coll = new EpisodeDiagnoseCollection();

            var query = new EpisodeDiagnoseQuery("a");
            var diag = new DiagnoseQuery("b");
            var item = new AppStandardReferenceItemQuery("e");
            var morph = new MorphologyQuery("c");
            var param = new ParamedicQuery("d");

            query.LeftJoin(diag).On(query.DiagnoseID == diag.DiagnoseID);
            query.InnerJoin(item).On(query.SRDiagnoseType == item.ItemID && item.StandardReferenceID == AppEnum.StandardReference.DiagnoseType);
            query.LeftJoin(morph).On(query.MorphologyID == morph.MorphologyID);
            query.LeftJoin(param).On(query.ParamedicID == param.ParamedicID);

            query.Select
                (
                    query,
                    //diag.DiagnoseName.As("refToDiagnose_DiagnoseName"),
                    "<ISNULL(b.DiagnoseName, a.DiagnosisText) AS refToDiagnose_DiagnoseName>",
                    item.ItemName.As("refToAppStandardReferenceItem_SRDiagnoseType"),
                    morph.MorphologyName.As("refToMorphology_MorphologyName"),
                    param.ParamedicName.As("refToParamedic_ParamedicName")
                );

            query.Where(query.RegistrationNo == registrationNo, query.IsVoid == false, query.DiagnoseID != string.Empty);
            query.OrderBy(query.SRDiagnoseType.Ascending);

            coll.Load(query);

            return coll;
        }

        private EpisodeProcedureCollection EpisodeProcedures(string registrationNo)
        {
            var coll = new EpisodeProcedureCollection();

            var query = new EpisodeProcedureQuery("a");
            var proc = new ProcedureQuery("b");
            var param = new ParamedicQuery("c");

            query.LeftJoin(proc).On(query.ProcedureID == proc.ProcedureID);
            query.LeftJoin(param).On(query.ParamedicID == param.ParamedicID);

            query.Select
            (
                query,
                param.ParamedicName.As("refToParamedic_ParamedicName"),
                proc.ProcedureName.As("refToProcedure_ProcedureName")
            );

            query.Where(query.RegistrationNo == registrationNo, query.IsVoid == false, query.ProcedureID != string.Empty);
            query.OrderBy(query.SequenceNo.Ascending);

            coll.Load(query);

            return coll;
        }


        [WebMethod]
        public IntermBillAutoResponse CreateIntermBillAuto(string regDate)
        {
            int total = 0;
            int successCount = 0;
            int failedCount = 0;

            try
            {
                DateTime trxDate = Convert.ToDateTime(regDate);

                var regis = new RegistrationCollection();
                regis.Query
                     .Where(regis.Query.IsVoid == false)
                     .Where(regis.Query.SRRegistrationType == AppConstant.RegistrationType.OutPatient)
                     .Where(regis.Query.BpjsSepNo.IsNotNull())
                     .Where(regis.Query.BpjsSepNo != "")
                     .Where(regis.Query.IsHoldTransactionEntry == false);

                regis.Query.Where(
                    regis.Query.RegistrationDate >= trxDate,
                    regis.Query.RegistrationDate < trxDate.AddDays(1)
                );

                regis.LoadAll();

                foreach (Registration reg in regis)
                {
                    try
                    {
                        if (!AppSession.Parameter.GuarantorAskesID.Contains(reg.GuarantorID))
                            continue;

                        total++;

                        string registrationNo = reg.RegistrationNo;

                        var autoNumber = Helper.GetNewAutoNumber(
                            (new DateTime()).NowAtSqlServer().Date,
                            AppEnum.AutoNumber.IntermBill);

                        var ibNo = autoNumber.LastCompleteNumber;
                        autoNumber.Save();

                        using (var trans = new esTransactionScope())
                        {
                            DateTime startDate = (new DateTime()).NowAtSqlServer();
                            DateTime endDate = (new DateTime()).NowAtSqlServer().Date.AddDays(-100);

                            decimal patAmount = 0;
                            decimal guarAmount = 0;

                            var costCalculations = new CostCalculationCollection();
                            costCalculations.Query.Where(costCalculations.Query.RegistrationNo == registrationNo);
                            costCalculations.LoadAll();

                            foreach (CostCalculation item in costCalculations)
                            {
                                item.IntermBillNo = ibNo;

                                if (item.TransactionDate < startDate)
                                    startDate = item.TransactionDate ?? DateTime.Now;

                                if (item.TransactionDate > endDate)
                                    endDate = item.TransactionDate ?? DateTime.Now;

                                patAmount += item.PatientAmount ?? 0;
                                guarAmount += item.GuarantorAmount ?? 0;
                            }

                            var entity = new IntermBill();
                            entity.AddNew();
                            entity.IntermBillNo = ibNo;
                            entity.IntermBillDate = DateTime.Now;
                            entity.RegistrationNo = registrationNo;
                            entity.StartDate = startDate;
                            entity.EndDate = endDate;
                            entity.PatientAmount = patAmount;
                            entity.GuarantorAmount = guarAmount;
                            entity.AdministrationAmount = 0;
                            entity.GuarantorAdministrationAmount = 0;
                            entity.DiscAdmPatient = 0;
                            entity.DiscAdmGuarantor = 0;
                            entity.IsVoid = false;
                            entity.IsApproved = false;

                            costCalculations.Save();
                            entity.Save();

                            trans.Complete();
                        }

                        //---------------------------------------------
                        // UPDATE TOTAL
                        //---------------------------------------------

                        var ccq = new CostCalculationQuery("a");
                        ccq.Where(ccq.IntermBillNo == ibNo);

                        ccq.Select(
                            ccq.PatientAmount.Sum().As("PatientAmount"),
                            ccq.GuarantorAmount.Sum().As("GuarantorAmount"),
                            ccq.DiscountAmount.Sum().As("DiscountAmount"),
                            @"<SUM(ISNULL(a.DiscountAmount2,0)) AS DiscountAmount2>"
                        );

                        DataTable dtb = ccq.LoadDataTable();

                        if (dtb.Rows.Count == 0)
                            continue;

                        decimal tpatient = dtb.AsEnumerable().Sum(t => t.Field<decimal>("PatientAmount"));
                        decimal tguarantor = dtb.AsEnumerable().Sum(t => t.Field<decimal>("GuarantorAmount"));
                        decimal tdiscount = dtb.AsEnumerable().Sum(t => t.Field<decimal>("DiscountAmount"));

                        var intermBill = new IntermBill();
                        if (intermBill.LoadByPrimaryKey(ibNo))
                        {
                            intermBill.PatientAmount = tpatient;
                            intermBill.GuarantorAmount = tguarantor;
                            intermBill.Save();
                        }

                        //---------------------------------------------
                        // HITUNG ADMIN
                        //---------------------------------------------

                        var grr = new Guarantor();
                        grr.LoadByPrimaryKey(reg.GuarantorID);

                        decimal admin = 0;
                        decimal admingr = 0;

                        if (grr.IsIncludeAdminValue ?? false)
                        {
                            admin = Helper.CostCalculation.GetAdminValue(
                                reg.GuarantorID,
                                tpatient + tdiscount,
                                reg.SRRegistrationType);

                            admingr = Helper.CostCalculation.GetAdminValue(
                                reg.GuarantorID,
                                tguarantor,
                                reg.SRRegistrationType);
                        }
                        else
                        {
                            admin = Helper.CostCalculation.GetAdminValue(
                                reg.GuarantorID,
                                tpatient + tguarantor,
                                reg.SRRegistrationType);
                        }

                        admin = Convert.ToDecimal(string.Format("{0:n2}", admin));
                        admingr = Convert.ToDecimal(string.Format("{0:n2}", admingr));

                        if (admin + admingr != 0)
                        {
                            intermBill = new IntermBill();
                            if (intermBill.LoadByPrimaryKey(ibNo))
                            {
                                intermBill.AdministrationAmount = admin;
                                intermBill.GuarantorAdministrationAmount = admingr;
                                intermBill.Save();
                            }
                        }

                        reg.AdministrationAmount += (admin + admingr);
                        reg.PatientAdm += admin;
                        reg.GuarantorAdm += admingr;

                        reg.IsHoldTransactionEntry = true;

                        //reg.Save();
                        regis.Save();

                        successCount++;
                    }
                    catch
                    {
                        failedCount++;
                    }
                }

                return new IntermBillAutoResponse
                {
                    success = true,
                    total = total,
                    successCount = successCount,
                    failedCount = failedCount
                };
            }
            catch (Exception ex)
            {
                return new IntermBillAutoResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        [WebMethod]
        public IntermBillAutoResponse CreateIntermBillByRegistration(string registrationNo)
        {
            try
            {
                var reg = new Registration();
                if (!reg.LoadByPrimaryKey(registrationNo))
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "Registration not found"
                    };
                }

                if (reg.IsVoid ?? false)
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "Registration is void"
                    };
                }

                if (reg.SRRegistrationType != AppConstant.RegistrationType.OutPatient)
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "Not OutPatient"
                    };
                }

                if (!AppSession.Parameter.GuarantorAskesID.Contains(reg.GuarantorID))
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "Not BPJS guarantor"
                    };
                }

                if (string.IsNullOrWhiteSpace(reg.BpjsSepNo))
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "SEP not found"
                    };
                }

                if (reg.IsHoldTransactionEntry ?? false)
                {
                    return new IntermBillAutoResponse
                    {
                        success = false,
                        message = "IntermBill already created"
                    };
                }

                //---------------------------------
                // GENERATE INTERMBILL
                //---------------------------------

                var autoNumber = Helper.GetNewAutoNumber(
                    (new DateTime()).NowAtSqlServer().Date,
                    AppEnum.AutoNumber.IntermBill);

                var ibNo = autoNumber.LastCompleteNumber;
                autoNumber.Save();

                using (var trans = new esTransactionScope())
                {
                    DateTime startDate = (new DateTime()).NowAtSqlServer();
                    DateTime endDate = (new DateTime()).NowAtSqlServer().Date.AddDays(-100);

                    decimal patAmount = 0;
                    decimal guarAmount = 0;

                    var costCalculations = new CostCalculationCollection();
                    costCalculations.Query.Where(costCalculations.Query.RegistrationNo == registrationNo);
                    costCalculations.LoadAll();

                    foreach (CostCalculation item in costCalculations)
                    {
                        item.IntermBillNo = ibNo;

                        if (item.TransactionDate < startDate)
                            startDate = item.TransactionDate ?? DateTime.Now;

                        if (item.TransactionDate > endDate)
                            endDate = item.TransactionDate ?? DateTime.Now;

                        patAmount += item.PatientAmount ?? 0;
                        guarAmount += item.GuarantorAmount ?? 0;
                    }

                    var entity = new IntermBill();
                    entity.AddNew();
                    entity.IntermBillNo = ibNo;
                    entity.IntermBillDate = DateTime.Now;
                    entity.RegistrationNo = registrationNo;
                    entity.StartDate = startDate;
                    entity.EndDate = endDate;
                    entity.PatientAmount = patAmount;
                    entity.GuarantorAmount = guarAmount;
                    entity.IsVoid = false;
                    entity.IsApproved = false;
                    entity.LastUpdateByUserID = "WEBSERVICE";
                    entity.LastUpdateDateTime = (new DateTime()).NowAtSqlServer();
                    entity.AskesCoveredSeqNo = string.Empty;
                    entity.AdministrationAmount = 0;
                    entity.GuarantorAdministrationAmount = 0;
                    entity.DiscAdmPatient = 0;
                    entity.DiscAdmGuarantor = 0;
                    entity.CreatedDateTime = (new DateTime()).NowAtSqlServer();
                    entity.CreatedByUserID = "WEBSERVICE";

                    costCalculations.Save();
                    entity.Save();

                    trans.Complete();
                }

                //---------------------------------
                // HITUNG ADMIN
                //---------------------------------

                decimal admin = 0;

                var grr = new Guarantor();
                grr.LoadByPrimaryKey(reg.GuarantorID);

                admin = Helper.CostCalculation.GetAdminValue(
                    reg.GuarantorID,
                    reg.AdministrationAmount ?? 0,
                    reg.SRRegistrationType);

                reg.AdministrationAmount += admin;
                reg.PatientAdm += admin;
                reg.IsHoldTransactionEntry = true;

                reg.Save();

                return new IntermBillAutoResponse
                {
                    success = true,
                    intermBillNo = ibNo,
                    adminValue = admin,
                    message = "IntermBill created"
                };
            }
            catch (Exception ex)
            {
                return new IntermBillAutoResponse
                {
                    success = false,
                    message = ex.Message
                };
            }
        }

        public class IntermBillAutoResponse
        {
            public bool success { get; set; }
            public int total { get; set; }
            public int successCount { get; set; }
            public int failedCount { get; set; }
            public string message { get; set; }
            public string intermBillNo { get; set; }
            public decimal adminValue { get; set; }
        }

        //SP
        /********************
         * AUTO INTERMBILL CALL WS
         ********************/

        //CREATE PROCEDURE dbo.sp_exec_auto_intermbill_auto
        //    @response VARCHAR(MAX) OUTPUT
        //AS
        //    BEGIN

        //    SET NOCOUNT ON

        //    DECLARE @date VARCHAR(10)
        //    DECLARE @url VARCHAR(MAX)
        //    DECLARE @postData VARCHAR(MAX)

        //    DECLARE @obj INT
        //    DECLARE @hr INT
        //    DECLARE @status INT
        //    DECLARE @msg VARCHAR(255)

        //    -- tanggal kemarin
        //    SET @date = CONVERT(VARCHAR(10), DATEADD(DAY, -1, GETDATE()), 23)

        //    SET @url = 'http://10.10.10.5/vklaim/webservice/inacbg.asmx/CreateIntermBillAuto'

        //    -- JSON BODY
        //    SET @postData = '{"regDate":"' + @date + '"}'

        //    EXEC @hr = sp_OACreate 'MSXML2.ServerXMLHttp', @obj OUT
        //    IF @hr<> 0
        //    BEGIN
        //    RAISERROR('sp_OACreate MSXML2.ServerXMLHttp failed',16,1)
        //    RETURN
        //    END

        //    EXEC @hr = sp_OAMethod @obj, 'open', NULL, 'POST', @url, FALSE
        //    IF @hr<> 0
        //    BEGIN
        //    SET @msg = 'sp_OAMethod Open failed'
        //    GOTO eh
        //    END

        //    EXEC @hr = sp_OAMethod @obj, 'setRequestHeader', NULL, 'Content-Type', 'application/json'
        //    IF @hr<> 0
        //    BEGIN
        //    SET @msg = 'setRequestHeader failed'
        //    GOTO eh
        //    END

        //    EXEC @hr = sp_OAMethod @obj, 'send', NULL, @postData
        //    IF @hr<> 0
        //    BEGIN
        //    SET @msg = 'Send failed'
        //    GOTO eh
        //    END

        //    EXEC @hr = sp_OAGetProperty @obj, 'status', @status OUT
        //    IF @hr <> 0
        //    BEGIN
        //    SET @msg = 'read status failed'
        //    GOTO eh
        //    END

        //    IF @status<> 200
        //    BEGIN
        //    SET @msg = 'HTTP status ' + CAST(@status AS VARCHAR)
        //    GOTO eh
        //    END

        //    EXEC @hr = sp_OAGetProperty @obj, 'responseText', @response OUT
        //    IF @hr <> 0
        //    BEGIN
        //    SET @msg = 'read response failed'
        //    GOTO eh
        //    END

        //    SELECT @response AS Response

        //    EXEC sp_OADestroy @obj
        //    RETURN

        //    eh:

        //    EXEC sp_OADestroy @obj
        //    SET @response = @msg

        //    SELECT @response AS ErrorMessage

        //    RETURN

        //    END

    }
}
