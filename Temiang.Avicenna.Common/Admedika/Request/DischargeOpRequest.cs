using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class DischargeOpTxnData
    {
        [JsonProperty("dischargeRequest")]
        public DischargeOpRequest Request { get; set; }
    }

    public class DischargeOpRequest
    {
        [JsonProperty("discharge")]
        public DischargeOpPayload Discharge { get; set; }
    }

    public class DischargeOpPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("diagnosisCodeList")] public string DiagnosisCodeList { get; set; }
        [JsonProperty("mcDays")] public string McDays { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("nationalID")] public string NationalID { get; set; }
        [JsonProperty("familyCardID")] public string FamilyCardID { get; set; }
        [JsonProperty("physicianName")] public string PhysicianName { get; set; }
        [JsonProperty("accidentFlag")] public string AccidentFlag { get; set; }
        [JsonProperty("surgicalFlag")] public string SurgicalFlag { get; set; }
        [JsonProperty("roomType")] public string RoomType { get; set; }
        [JsonProperty("roomPrice")] public string RoomPrice { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
        [JsonProperty("entitlement")] public DischargeEntitlementInput[] Entitlement { get; set; }
    }

    public class DischargeEntitlementInput
    {
        [JsonProperty("benID")] public string BenID { get; set; }
        [JsonProperty("benAmount")] public string BenAmount { get; set; }
        [JsonProperty("benItemList")] public DischargeBenItemInput[] BenItemList { get; set; }
    }

    public class DischargeBenItemInput
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("dateIssue")] public string DateIssue { get; set; }
        [JsonProperty("qty")] public string Qty { get; set; }
        [JsonProperty("totPrice")] public string TotPrice { get; set; }
    }
}
