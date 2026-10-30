using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class InvoiceTrackingResponse : AdmedikaResponse
    {
        private List<InvoiceTrackingItem> _invoiceTracking = new List<InvoiceTrackingItem>();

        [JsonProperty("invoiceTracking")]
        public List<InvoiceTrackingItem> InvoiceTracking
        {
            get { return _invoiceTracking; }
            set { _invoiceTracking = value ?? new List<InvoiceTrackingItem>(); }
        }
    }

    public class InvoiceTrackingItem
    {
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("totClaims")] public string TotClaims { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtPaid")] public string TotAmtPaid { get; set; }
        [JsonProperty("receivedDate")] public string ReceivedDate { get; set; }
        [JsonProperty("registerDate")] public string RegisterDate { get; set; }
        [JsonProperty("submissionDate")] public string SubmissionDate { get; set; }
        [JsonProperty("admedNo")] public string AdmedNo { get; set; }
        [JsonProperty("paymentDate")] public string PaymentDate { get; set; }
    }
}
