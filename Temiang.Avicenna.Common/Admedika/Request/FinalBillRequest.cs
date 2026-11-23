using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class FinalBillTxnData
    {
        [JsonProperty("finalBillRequest")]
        public FinalBillRequest Request { get; set; }
    }

    public class FinalBillRequest
    {
        [JsonProperty("finalBill")]
        public FinalBillPayload FinalBill { get; set; }
    }

    public class FinalBillPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("diagnosisCodeList")] public string DiagnosisCodeList { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("finalFlag")] public string FinalFlag { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("invoiceDate")] public string InvoiceDate { get; set; }
        [JsonProperty("dateAdmission")] public string DateAdmission { get; set; }
        [JsonProperty("dateDischarge")] public string DateDischarge { get; set; }
        [JsonProperty("preparedBy")] public string PreparedBy { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
        [JsonProperty("totAmountDue")] public string TotAmountDue { get; set; }
        [JsonProperty("totItems")] public string TotItems { get; set; }
        [JsonProperty("itemizedBill")] public FinalBillGroup[] ItemizedBill { get; set; }

        [JsonProperty("nationalID")] public string NationalID { get; set; }
        [JsonProperty("familyCardID")] public string FamilyCardID { get; set; }
        [JsonProperty("accidentFlag")] public string AccidentFlag { get; set; }
        [JsonProperty("surgicalFlag")] public string SurgicalFlag { get; set; }
        [JsonProperty("roomName")] public string RoomName { get; set; }
        [JsonProperty("roomType")] public string RoomType { get; set; }
        [JsonProperty("roomStatusInfo")] public string RoomStatusInfo { get; set; }
        [JsonProperty("roomPrice")] public string RoomPrice { get; set; }
        [JsonProperty("physicianName")] public string PhysicianName { get; set; }
        [JsonProperty("coManagingPhysician")] public string CoManagingPhysician { get; set; }
        [JsonProperty("consultingPhysician")] public string ConsultingPhysician { get; set; }
        [JsonProperty("anamnesisSignsSymptoms")] public string AnamnesisSignsSymptoms { get; set; }
        [JsonProperty("bloodPressure")] public string BloodPressure { get; set; }
        [JsonProperty("pulseRate")] public string PulseRate { get; set; }
        [JsonProperty("respiratoryRate")] public string RespiratoryRate { get; set; }
        [JsonProperty("temperature")] public string Temperature { get; set; }
        [JsonProperty("bodyWeight")] public string BodyWeight { get; set; }
        [JsonProperty("treatmentTherapyMedications")] public string TreatmentTherapyMedications { get; set; }
        [JsonProperty("diagnosticTestResults")] public string DiagnosticTestResults { get; set; }
        [JsonProperty("indicationForHospitalization")] public string IndicationForHospitalization { get; set; }
        [JsonProperty("icd9cmPlannedIntervention")] public string Icd9cmPlannedIntervention { get; set; }
        [JsonProperty("icd9cmSurgicalProcedure")] public string Icd9cmSurgicalProcedure { get; set; }
        [JsonProperty("bpjsNo")] public string BpjsNo { get; set; }
        [JsonProperty("sepSjpNo")] public string SepSjpNo { get; set; }
        [JsonProperty("sourceOfInpatientReferral")] public string SourceOfInpatientReferral { get; set; }
        [JsonProperty("referringPhysician")] public string ReferringPhysician { get; set; }
        [JsonProperty("reasonForReferral")] public string ReasonForReferral { get; set; }
        [JsonProperty("telNoOfRs")] public string TelNoOfRs { get; set; }
        [JsonProperty("telNoOfPatient")] public string TelNoOfPatient { get; set; }
        [JsonProperty("listOfAssistiveDevices")] public string ListOfAssistiveDevices { get; set; }
    }

    public class FinalBillGroup
    {
        [JsonProperty("groupCode")] public string GroupCode { get; set; }
        [JsonProperty("groupName")] public string GroupName { get; set; }
        [JsonProperty("itemList")] public FinalBillItem[] ItemList { get; set; }
    }

    public class FinalBillItem
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("dateIssue")] public string DateIssue { get; set; }
        [JsonProperty("qty")] public string Qty { get; set; }
        [JsonProperty("totPrice")] public string TotPrice { get; set; }
        [JsonProperty("discount")] public string Discount { get; set; }
        [JsonProperty("nonMedicalFlag")] public string NonMedicalFlag { get; set; }
        [JsonProperty("benID")] public string BenID { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
