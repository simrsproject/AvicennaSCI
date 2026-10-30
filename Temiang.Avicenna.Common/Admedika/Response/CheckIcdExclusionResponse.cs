using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CheckIcdExclusionResponse : AdmedikaResponse
    {
        [JsonProperty("PayorName")]
        public string PayorName { get; set; }

        [JsonProperty("cardNo")]
        public string CardNo { get; set; }

        [JsonProperty("covID")]
        public string CovID { get; set; }

        [JsonProperty("diagnosisCodeList")]
        public string DiagnosisCodeList { get; set; }

        [JsonProperty("dob")]
        public string Dob { get; set; }

        [JsonProperty("patientName")]
        public string PatientName { get; set; }

        [JsonProperty("payorMemberID")]
        public string PayorMemberID { get; set; }

        [JsonProperty("pd")]
        public string Pd { get; set; }

        [JsonProperty("planCode")]
        public string PlanCode { get; set; }

        [JsonProperty("policyDate")]
        public string PolicyDate { get; set; }

        [JsonProperty("policyNo")]
        public string PolicyNo { get; set; }

        [JsonProperty("principalName")]
        public string PrincipalName { get; set; }
    }
}
