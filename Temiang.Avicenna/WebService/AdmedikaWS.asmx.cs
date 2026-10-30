using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Fasade integrasi Gateway@AdMedika (customer host).
    /// </summary>
    /// <remarks>
    /// <para>Panduan pemanggilan dari code aplikasi:</para>
    /// <code>
    /// var ws = new Temiang.Avicenna.WebService.AdmedikaWS();
    /// var result = ws.CheckIcdExclusion("8000991200054479", "02", "J06, B01.9");
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
    /// Alternatif tanpa fasade:
    /// <c>new Temiang.Avicenna.Common.Admedika.Service()</c> memiliki method yang sama
    /// dan mengembalikan tipe respons yang sama.
    /// </para>
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
        [WebMethod(Description = "Uji konektivitas Gateway@AdMedika (smoke test autentikasi dan envelope). from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public HelloWorldResponse HelloWorld()
        {
            return _service.HelloWorld();
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Memeriksa apakah daftar ICD-10 termasuk pengecualian (exclusion) pada plan member. from source : CHECK_ICD_EXCLUSION.pdf")]
        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            return _service.CheckIcdExclusion(cardNo, covID, diagnosisCodeList);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mendaftarkan eligibility rawat jalan member dan mengambil informasi plan/entitlement. from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            return _service.Eligibility(cardNo, covID, diagnosisCodeList, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mengambil rincian benefit (entitlement) member yang memiliki klaim terbuka. from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            return _service.GetEntitlement(cardNo);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mengirim pengesahan/discharge rawat jalan beserta rincian biaya dan menerima hasil approval. from source : CustomerHost_ProviderBridging - OUTPATIENT.pdf")]
        public DischargeOpResponse DischargeOp(string cardNo, string diagnosisCodeList, string mcDays,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            return _service.DischargeOp(cardNo, diagnosisCodeList, mcDays, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks, entitlement);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Memeriksa status klaim terkini member atau klaim tertentu berdasarkan clID. from source : CHECK_MEMBER_CLAIM_TXN_STATUS-and-GET_MEMBER_ENROLLED_PLAN_TC.pdf")]
        public CheckMemberClaimTxnStatusResponse CheckMemberClaimTxnStatus(string cardNo, string clID)
        {
            return _service.CheckMemberClaimTxnStatus(cardNo, clID);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mengambil plan yang diikuti member beserta term & condition (pengecualian). from source : CHECK_MEMBER_CLAIM_TXN_STATUS-and-GET_MEMBER_ENROLLED_PLAN_TC.pdf")]
        public GetMemberEnrolledPlanTcResponse GetMemberEnrolledPlanTc(string cardNo, string covID,
            string searchForTermCondition)
        {
            return _service.GetMemberEnrolledPlanTc(cardNo, covID, searchForTermCondition);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Membatalkan klaim terbuka member agar dapat melakukan eligibility ulang. from source : CANCEL_OPEN_CLAIMS_TXN.pdf")]
        public CancelOpenClaimsTxnResponse CancelOpenClaimsTxn(string cardNo, string remarks)
        {
            return _service.CancelOpenClaimsTxn(cardNo, remarks);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mengirim form daily monitoring rawat inap (dmForms) untuk klaim. from source : DAILY_MONITORING.pdf")]
        public DailyMonitoringResponse DailyMonitoring(string cardNo, string providerTransID, string clID,
            string dmUser, DailyMonitoringForm[] dmForms)
        {
            return _service.DailyMonitoring(cardNo, providerTransID, clID, dmUser, dmForms);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mengunggah dokumen pendukung klaim (multipart: data + files). from source : EDOCS_UPLOAD v.Rustlang.pdf")]
        public EdocsUploadResponse EdocsUpload(string cardNo, string clID, string docType, string remarks,
            string patientName, string userBy, AdmedikaFile[] files)
        {
            return _service.EdocsUpload(cardNo, clID, docType, remarks, patientName, userBy, files);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Melacak status invoice/klaim berdasarkan nomor invoice atau rentang tanggal. from source : INVOICE_TRACKING.pdf")]
        public InvoiceTrackingResponse InvoiceTracking(string invoiceNo, string dateName, string dateFrom,
            string dateTo)
        {
            return _service.InvoiceTracking(invoiceNo, dateName, dateFrom, dateTo);
        }

        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        [WebMethod(Description = "Mencetak ulang LOA/LOC/LOR berdasarkan clID dan tipe cetak. from source : RE_PRINT_OUT.pdf")]
        public RePrintOutResponse RePrintOut(string clID, string rePrintType)
        {
            return _service.RePrintOut(clID, rePrintType);
        }
    }
}
