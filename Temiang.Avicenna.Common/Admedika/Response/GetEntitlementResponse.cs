using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class GetEntitlementResponse : AdmedikaResponse
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("entitlement")] public List<BenefitItem> Entitlement { get; set; }
    }
}
