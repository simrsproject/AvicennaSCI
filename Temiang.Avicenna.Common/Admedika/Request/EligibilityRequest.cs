using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class EligibilityTxnData
    {
        [JsonProperty("eligibilityRequest")]
        public EligibilityRequest Request { get; set; }
    }

    public class EligibilityRequest
    {
        [JsonProperty("eligibility")]
        public EligibilityPayload Eligibility { get; set; }
    }

    public class EligibilityPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("covID")] public string CovID { get; set; }
        [JsonProperty("diagnosisCodeList")] public string DiagnosisCodeList { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("nationalID")] public string NationalID { get; set; }
        [JsonProperty("familyCardID")] public string FamilyCardID { get; set; }
        [JsonProperty("physicianName")] public string PhysicianName { get; set; }
        [JsonProperty("accidentFlag")] public string AccidentFlag { get; set; }
        [JsonProperty("surgicalFlag")] public string SurgicalFlag { get; set; }
        [JsonProperty("roomType")] public string RoomType { get; set; }
        [JsonProperty("roomPrice")] public string RoomPrice { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
