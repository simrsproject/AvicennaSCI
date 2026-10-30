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
    }
}
