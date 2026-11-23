using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaInvoiceTrackingDetailsResponse")]
    public class InvoiceTrackingDetailsResponse : AdmedikaResponse
    {
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }

        private List<InvoiceTrackingDetailItem> _details = new List<InvoiceTrackingDetailItem>();

        [JsonProperty("details")]
        public List<InvoiceTrackingDetailItem> Details
        {
            get { return _details; }
            set { _details = value ?? new List<InvoiceTrackingDetailItem>(); }
        }
    }

    public class InvoiceTrackingDetailItem
    {
        [JsonProperty("claimsID")] public string ClaimsID { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("provCode")] public string ProvCode { get; set; }
        [JsonProperty("provName")] public string ProvName { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("admissionDate")] public string AdmissionDate { get; set; }
        [JsonProperty("dischargeDate")] public string DischargeDate { get; set; }
        [JsonProperty("amtIncurred")] public string AmtIncurred { get; set; }
        [JsonProperty("amtApproved")] public string AmtApproved { get; set; }
        [JsonProperty("amtToPay")] public string AmtToPay { get; set; }
        [JsonProperty("amtPaidToProvider")] public string AmtPaidToProvider { get; set; }
        [JsonProperty("icdCode")] public string IcdCode { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
