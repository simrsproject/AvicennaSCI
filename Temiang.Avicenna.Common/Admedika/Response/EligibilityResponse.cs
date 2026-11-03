using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaEligibilityResponse")]
    public class EligibilityResponse : AdmedikaResponse
    {
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("dob")] public string Dob { get; set; }
        [JsonProperty("policyNo")] public string PolicyNo { get; set; }
        [JsonProperty("payorMemberID")] public string PayorMemberID { get; set; }
        [JsonProperty("policyDate")] public string PolicyDate { get; set; }
        [JsonProperty("planCode")] public string PlanCode { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("pd")] public string Pd { get; set; }
        [JsonProperty("principalName")] public string PrincipalName { get; set; }
        [JsonProperty("referral_info_flag")] public string ReferralInfoFlag { get; set; }
        [JsonProperty("previous_icd_code")] public string PreviousIcdCode { get; set; }
        [JsonProperty("printOut")] public string PrintOut { get; set; }
        private List<BenefitItem> _entitlement = new List<BenefitItem>();

        [JsonProperty("entitlement")]
        public List<BenefitItem> Entitlement
        {
            get { return _entitlement; }
            set { _entitlement = value ?? new List<BenefitItem>(); }
        }
    }
}
