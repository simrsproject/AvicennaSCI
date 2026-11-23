using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaPreRegisterEbillEnquiryResponse")]
    public class PreRegisterEbillEnquiryResponse : AdmedikaResponse
    {
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("coverage")] public string Coverage { get; set; }
        [JsonProperty("dateFrom")] public string DateFrom { get; set; }
        [JsonProperty("dateTo")] public string DateTo { get; set; }
        [JsonProperty("tot_claims")] public string TotClaims { get; set; }

        private List<PreRegisterEbillClaim> _claimsList = new List<PreRegisterEbillClaim>();

        [JsonProperty("claimsList")]
        public List<PreRegisterEbillClaim> ClaimsList
        {
            get { return _claimsList; }
            set { _claimsList = value ?? new List<PreRegisterEbillClaim>(); }
        }
    }

    public class PreRegisterEbillClaim
    {
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("invoiceDate")] public string InvoiceDate { get; set; }
        [JsonProperty("registerDate")] public string RegisterDate { get; set; }
        [JsonProperty("registerBy")] public string RegisterBy { get; set; }
        [JsonProperty("dateAdmission")] public string DateAdmission { get; set; }
        [JsonProperty("dateDischarge")] public string DateDischarge { get; set; }
        [JsonProperty("icdCode")] public string IcdCode { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtApproved")] public string TotAmtApproved { get; set; }
        [JsonProperty("totAmtNotApproved")] public string TotAmtNotApproved { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("provCode")] public string ProvCode { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }

    [XmlRoot("AdmedikaPreRegisterEbillResponse")]
    public class PreRegisterEbillResponse : AdmedikaResponse
    {
        [JsonProperty("surat_jalan_link")] public string SuratJalanLink { get; set; }

        private List<PreRegisterEbillClaimResult> _selectedClaims =
            new List<PreRegisterEbillClaimResult>();

        [JsonProperty("selectedClaims")]
        public List<PreRegisterEbillClaimResult> SelectedClaims
        {
            get { return _selectedClaims; }
            set { _selectedClaims = value ?? new List<PreRegisterEbillClaimResult>(); }
        }
    }

    public class PreRegisterEbillClaimResult
    {
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}
