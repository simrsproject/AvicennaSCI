using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class InvoiceTrackingDetailsTxnData
    {
        [JsonProperty("invoiceTrackingDetailsRequest")]
        public InvoiceTrackingDetailsRequest Request { get; set; }
    }

    public class InvoiceTrackingDetailsRequest
    {
        [JsonProperty("invoiceTrackingDetails")]
        public InvoiceTrackingDetailsPayload InvoiceTrackingDetails { get; set; }
    }

    public class InvoiceTrackingDetailsPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
    }
}
