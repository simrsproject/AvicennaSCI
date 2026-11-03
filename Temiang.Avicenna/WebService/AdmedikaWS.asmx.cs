using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Fasade integrasi Gateway@AdMedika (customer host).
    /// </summary>
    /// <remarks>
    /// <para>Panduan pemanggilan dari code aplikasi. Contoh : </para>
    /// <code>
    /// var srv = new Temiang.Avicenna.Common.Admedika.Service();
    /// var result = srv.CheckIcdExclusion("8000991200054479", "02", "J06, B01.9");
    ///
    /// // 1. Periksa kegagalan teknis lebih dulu (timeout, TLS, body bukan JSON).
    /// if (!string.IsNullOrEmpty(result.TechnicalError))
    /// {
    ///     // StatusCode/ErrorCode bisa kosong pada kondisi ini.
    ///     ShowError(result.TechnicalError);
    /// }
    /// // 2. StatusCode "00" berarti sukses; selain itu gagal bisnis dari AdMedika.
    /// else if (!result.IsSuccess) // IsSuccess == (StatusCode == "00")
    /// {
    ///     // ErrorCode/ErrorDesc berasal dari txnData AdMedika,
    ///     // mis. "1833" = diagnosis exclusion, "1812" = member not found.
    ///     ShowError(result.ErrorCode + " - " + result.ErrorDesc);
    /// }
    /// // 3. Sukses: baca field hasil sesuai service.
    /// else
    /// {
    ///     ShowInfo(result.PatientName + " | " + result.PlanCode);
    /// }
    /// </code>
    /// <para>
    /// Catatan: kegagalan bisnis AdMedika tetap dikembalikan sebagai HTTP 200.
    /// Periksa field <c>TechnicalError</c>, <c>StatusCode</c>, <c>ErrorCode</c>,
    /// dan <c>ErrorDesc</c>; jangan bergantung pada HTTP status.
    /// </para>
    /// </remarks>
    [WebService(Namespace = "http://tempuri.org/",
        Description = "Integrasi customer host Avicenna dengan Gateway@AdMedika.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AdmedikaWS : System.Web.Services.WebService
    {
        private readonly Service _service;

        public AdmedikaWS()
        {
            _service = new Service();
        }

        internal AdmedikaWS(Service service)
        {
            _service = service;
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Uji konektivitas Gateway@AdMedika (smoke test autentikasi dan envelope). "
            + @"Request: tanpa parameter (customerID diisi otomatis dari konfigurasi). "
            + @"from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public HelloWorldResponse HelloWorld()
        {
            return _service.HelloWorld();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Memeriksa apakah daftar ICD-10 termasuk pengecualian (exclusion) pada plan member. "
            + @"Request: cardNo (R, nomor kartu); covID (R, coverage 01-08); "
            + @"diagnosisCodeList (R, ICD-10 dipisah koma). "
            + @"from source : CHECK_ICD_EXCLUSION.pdf")]
        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            return _service.CheckIcdExclusion(cardNo, covID, diagnosisCodeList);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mendaftarkan eligibility rawat jalan member dan mengambil informasi plan/entitlement. "
            + @"Request: cardNo (R); covID (R, 01-08); diagnosisCodeList (O, ICD-10 dipisah koma); "
            + @"providerTransID (O, ID transaksi provider); nationalID (O, KTP); familyCardID (O, kartu keluarga); "
            + @"physicianName (O, dokter); accidentFlag (O, Y=kecelakaan/N=sakit); surgicalFlag (O, Y=operasi/N=non-operasi); "
            + @"roomType (O, tipe kamar); roomPrice (O, harga kamar); remarks (O, maks 500 karakter). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            return _service.Eligibility(cardNo, covID, diagnosisCodeList, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mengambil rincian benefit (entitlement) member yang memiliki klaim terbuka. "
            + @"Request: cardNo (R, nomor kartu). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            return _service.GetEntitlement(cardNo);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mengirim pengesahan/discharge rawat jalan beserta rincian biaya dan menerima hasil approval. "
            + @"Request: cardNo (R); diagnosisCodeList (R, ICD-10 dipisah koma); mcDays (O, izin sakit hari); "
            + @"providerTransID (O); nationalID (O, KTP); familyCardID (O, KK); physicianName (O); "
            + @"accidentFlag (O, Y/N); surgicalFlag (O, Y/N); roomType (O); roomPrice (O); remarks (O, maks 500); "
            + @"entitlement (R): benID, benAmount, benItemList[code, name, dateIssue(ddmmyyyy), qty, totPrice]. "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public DischargeOpResponse DischargeOp(string cardNo, string diagnosisCodeList, string mcDays,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            return _service.DischargeOp(cardNo, diagnosisCodeList, mcDays, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks, entitlement);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Memeriksa status klaim terkini member atau klaim tertentu berdasarkan clID. "
            + @"Request: cardNo (R); clID (O, kosongkan untuk klaim terbaru). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : CHECK_MEMBER_CLAIM_TXN_STATUS-and-GET_MEMBER_ENROLLED_PLAN_TC.pdf")]
        public CheckMemberClaimTxnStatusResponse CheckMemberClaimTxnStatus(string cardNo, string clID)
        {
            return _service.CheckMemberClaimTxnStatus(cardNo, clID);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mengambil plan yang diikuti member beserta term & condition (pengecualian). "
            + @"Request: cardNo (R); covID (R, 01-08); searchForTermCondition (O, kata kunci term & condition). "
            + @"from source : CHECK_MEMBER_CLAIM_TXN_STATUS-and-GET_MEMBER_ENROLLED_PLAN_TC.pdf")]
        public GetMemberEnrolledPlanTcResponse GetMemberEnrolledPlanTc(string cardNo, string covID,
            string searchForTermCondition)
        {
            return _service.GetMemberEnrolledPlanTc(cardNo, covID, searchForTermCondition);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Membatalkan klaim terbuka member agar dapat melakukan eligibility ulang. "
            + @"Request: cardNo (R); remarks (R, alasan pembatalan, maks 500). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : CANCEL_OPEN_CLAIMS_TXN.pdf")]
        public CancelOpenClaimsTxnResponse CancelOpenClaimsTxn(string cardNo, string remarks)
        {
            return _service.CancelOpenClaimsTxn(cardNo, remarks);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mengirim form daily monitoring rawat inap (dmForms) untuk klaim. "
            + @"Request: cardNo (R); providerTransID (R/O, ID transaksi provider); clID (claims ID); "
            + @"dmUser (O, user provider); dmForms (R): code (R, lihat tabel form), name (O), "
            + @"input (R, teks bebas), date (O, ddmmyyyy). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : DAILY_MONITORING.pdf")]
        public DailyMonitoringResponse DailyMonitoring(string cardNo, string providerTransID, string clID,
            string dmUser, DailyMonitoringForm[] dmForms)
        {
            return _service.DailyMonitoring(cardNo, providerTransID, clID, dmUser, dmForms);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mengunggah dokumen pendukung klaim (multipart: data + files). "
            + @"Request: cardNo (R); clID (claims ID); docType (R, lihat AdmedikaDocType); remarks (O); "
            + @"patientName (O); userBy (O, maks 8 karakter); files (R, satu atau lebih file). "
            + @"from source : EDOCS_UPLOAD v.Rustlang.pdf")]
        public EdocsUploadResponse EdocsUpload(string cardNo, string clID, string docType, string remarks,
            string patientName, string userBy, AdmedikaFile[] files)
        {
            return _service.EdocsUpload(cardNo, clID, docType, remarks, patientName, userBy, files);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Melacak status invoice/klaim berdasarkan nomor invoice atau rentang tanggal. "
            + @"Request: invoiceNo (R); dateName (O, RECEIVED_DATE/REGISTER_DATE, default RECEIVED_DATE); "
            + @"dateFrom (O, ddmmyyyy); dateTo (O, ddmmyyyy). "
            + @"terminalID diisi otomatis dari konfigurasi. "
            + @"from source : INVOICE_TRACKING.pdf")]
        public InvoiceTrackingResponse InvoiceTracking(string invoiceNo, string dateName, string dateFrom,
            string dateTo)
        {
            return _service.InvoiceTracking(invoiceNo, dateName, dateFrom, dateTo);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = @"Mencetak ulang LOA/LOC/LOR berdasarkan clID dan tipe cetak. "
            + @"Request: clID (R, claims ID); rePrintType (R, E=LOA/D=LOC/X=LOR). "
            + @"from source : RE_PRINT_OUT.pdf")]
        public RePrintOutResponse RePrintOut(string clID, string rePrintType)
        {
            return _service.RePrintOut(clID, rePrintType);
        }
    }
}
