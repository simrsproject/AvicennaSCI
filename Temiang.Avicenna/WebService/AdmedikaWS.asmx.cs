using System.Web.Services;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Fasade integrasi Gateway@AdMedika (customer host).
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
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

        [WebMethod]
        public HelloWorldResponse HelloWorld()
        {
            return _service.HelloWorld();
        }

        [WebMethod]
        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            return _service.CheckIcdExclusion(cardNo, covID, diagnosisCodeList);
        }

        [WebMethod]
        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            return _service.Eligibility(cardNo, covID, diagnosisCodeList, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks);
        }

        [WebMethod]
        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            return _service.GetEntitlement(cardNo);
        }

        [WebMethod]
        public DischargeOpResponse DischargeOp(string cardNo, string diagnosisCodeList, string mcDays,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks,
            DischargeEntitlementInput[] entitlement)
        {
            return _service.DischargeOp(cardNo, diagnosisCodeList, mcDays, providerTransID, nationalID,
                familyCardID, physicianName, accidentFlag, surgicalFlag, roomType, roomPrice, remarks, entitlement);
        }

        [WebMethod]
        public CheckMemberClaimTxnStatusResponse CheckMemberClaimTxnStatus(string cardNo, string clID)
        {
            return _service.CheckMemberClaimTxnStatus(cardNo, clID);
        }

        [WebMethod]
        public GetMemberEnrolledPlanTcResponse GetMemberEnrolledPlanTc(string cardNo, string covID,
            string searchForTermCondition)
        {
            return _service.GetMemberEnrolledPlanTc(cardNo, covID, searchForTermCondition);
        }

        [WebMethod]
        public CancelOpenClaimsTxnResponse CancelOpenClaimsTxn(string cardNo, string remarks)
        {
            return _service.CancelOpenClaimsTxn(cardNo, remarks);
        }

        [WebMethod]
        public DailyMonitoringResponse DailyMonitoring(string cardNo, string providerTransID, string clID,
            string dmUser, DailyMonitoringForm[] dmForms)
        {
            return _service.DailyMonitoring(cardNo, providerTransID, clID, dmUser, dmForms);
        }

        [WebMethod]
        public EdocsUploadResponse EdocsUpload(string cardNo, string clID, string docType, string remarks,
            string patientName, string userBy, AdmedikaFile[] files)
        {
            return _service.EdocsUpload(cardNo, clID, docType, remarks, patientName, userBy, files);
        }

        [WebMethod]
        public InvoiceTrackingResponse InvoiceTracking(string invoiceNo, string dateName, string dateFrom,
            string dateTo)
        {
            return _service.InvoiceTracking(invoiceNo, dateName, dateFrom, dateTo);
        }

        [WebMethod]
        public RePrintOutResponse RePrintOut(string clID, string rePrintType)
        {
            return _service.RePrintOut(clID, rePrintType);
        }
    }
}
