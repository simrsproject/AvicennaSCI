using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class InvoiceTrackingTxnData
    {
        [JsonProperty("invoiceTrackingRequest")]
        public InvoiceTrackingRequest Request { get; set; }
    }

    public class InvoiceTrackingRequest
    {
        [JsonProperty("invoiceTracking")]
        public InvoiceTrackingPayload InvoiceTracking { get; set; }
    }

    public class InvoiceTrackingPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("dateName")] public string DateName { get; set; }
        [JsonProperty("dateFrom")] public string DateFrom { get; set; }
        [JsonProperty("dateTo")] public string DateTo { get; set; }
    }
}
