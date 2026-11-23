using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaFinalBillResponse")]
    public class FinalBillResponse : AdmedikaResponse
    {
        [JsonProperty("statusText")] public string StatusText { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
    }
}
