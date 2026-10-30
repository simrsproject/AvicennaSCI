using System.Web.Services;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Fasade integrasi Gateway@AdMedika (customer host).
    /// </summary>
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

        [WebMethod(Description = "Uji konektivitas Gateway@AdMedika (smoke test autentikasi dan envelope).")]
        public HelloWorldResponse HelloWorld()
        {
            return _service.HelloWorld();
        }

        [WebMethod(Description = "Memeriksa apakah daftar ICD-10 termasuk pengecualian (exclusion) pada plan member.")]
        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            return _service.CheckIcdExclusion(cardNo, covID, diagnosisCodeList);
        }

        [WebMethod(Description = "Mendaftarkan eligibility rawat jalan member dan mengambil informasi plan/entitlement.")]
        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            return _service.Eligibility(cardNo, covID, diagnosisCodeList, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks);
        }

        [WebMethod(Description = "Mengambil rincian benefit (entitlement) member yang memiliki klaim terbuka.")]
        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            return _service.GetEntitlement(cardNo);
        }

        [WebMethod(Description = "Mengirim pengesahan/discharge rawat jalan beserta rincian biaya dan menerima hasil approval.")]
        public DischargeOpResponse DischargeOp(string cardNo, string diagnosisCodeList, string mcDays,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            return _service.DischargeOp(cardNo, diagnosisCodeList, mcDays, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks, entitlement);
        }

        [WebMethod(Description = "Memeriksa status klaim terkini member atau klaim tertentu berdasarkan clID.")]
        public CheckMemberClaimTxnStatusResponse CheckMemberClaimTxnStatus(string cardNo, string clID)
        {
            return _service.CheckMemberClaimTxnStatus(cardNo, clID);
        }

        [WebMethod(Description = "Mengambil plan yang diikuti member beserta term & condition (pengecualian).")]
        public GetMemberEnrolledPlanTcResponse GetMemberEnrolledPlanTc(string cardNo, string covID,
            string searchForTermCondition)
        {
            return _service.GetMemberEnrolledPlanTc(cardNo, covID, searchForTermCondition);
        }

        [WebMethod(Description = "Membatalkan klaim terbuka member agar dapat melakukan eligibility ulang.")]
        public CancelOpenClaimsTxnResponse CancelOpenClaimsTxn(string cardNo, string remarks)
        {
            return _service.CancelOpenClaimsTxn(cardNo, remarks);
        }

        [WebMethod(Description = "Mengirim form daily monitoring rawat inap (dmForms) untuk klaim.")]
        public DailyMonitoringResponse DailyMonitoring(string cardNo, string providerTransID, string clID,
            string dmUser, DailyMonitoringForm[] dmForms)
        {
            return _service.DailyMonitoring(cardNo, providerTransID, clID, dmUser, dmForms);
        }

        [WebMethod(Description = "Mengunggah dokumen pendukung klaim (multipart: data + files).")]
        public EdocsUploadResponse EdocsUpload(string cardNo, string clID, string docType, string remarks,
            string patientName, string userBy, AdmedikaFile[] files)
        {
            return _service.EdocsUpload(cardNo, clID, docType, remarks, patientName, userBy, files);
        }

        [WebMethod(Description = "Melacak status invoice/klaim berdasarkan nomor invoice atau rentang tanggal.")]
        public InvoiceTrackingResponse InvoiceTracking(string invoiceNo, string dateName, string dateFrom,
            string dateTo)
        {
            return _service.InvoiceTracking(invoiceNo, dateName, dateFrom, dateTo);
        }

        [WebMethod(Description = "Mencetak ulang LOA/LOC/LOR berdasarkan clID dan tipe cetak.")]
        public RePrintOutResponse RePrintOut(string clID, string rePrintType)
        {
            return _service.RePrintOut(clID, rePrintType);
        }
    }
}
