using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class DischargeOpResponse : AdmedikaResponse
    {
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("dob")] public string Dob { get; set; }
        [JsonProperty("policyNo")] public string PolicyNo { get; set; }
        [JsonProperty("payorMemberID")] public string PayorMemberID { get; set; }
        [JsonProperty("policyDate")] public string PolicyDate { get; set; }
        [JsonProperty("planCode")] public string PlanCode { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("pd")] public string Pd { get; set; }
        [JsonProperty("principalName")] public string PrincipalName { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtApproved")] public string TotAmtApproved { get; set; }
        [JsonProperty("totAmtNotApproved")] public string TotAmtNotApproved { get; set; }
        [JsonProperty("printOut")] public string PrintOut { get; set; }
        [JsonProperty("entitlement")] public List<DischargeEntitlementResult> Entitlement { get; set; }

        public DischargeOpResponse()
        {
            Entitlement = new List<DischargeEntitlementResult>();
        }
    }

    public class DischargeEntitlementResult
    {
        [JsonProperty("benID")] public string BenID { get; set; }
        [JsonProperty("shortName")] public string ShortName { get; set; }
        [JsonProperty("longName")] public string LongName { get; set; }
        [JsonProperty("amtIncurred")] public string AmtIncurred { get; set; }
        [JsonProperty("amtApproved")] public string AmtApproved { get; set; }
        [JsonProperty("amtNotApproved")] public string AmtNotApproved { get; set; }
    }
}
