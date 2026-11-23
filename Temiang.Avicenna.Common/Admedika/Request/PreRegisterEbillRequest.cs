using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class PreRegisterEbillEnquiryTxnData
    {
        [JsonProperty("preRegisterEbillEnquiryRequest")]
        public PreRegisterEbillEnquiryRequest Request { get; set; }
    }

    public class PreRegisterEbillEnquiryRequest
    {
        [JsonProperty("preRegisterEbillEnquiry")]
        public PreRegisterEbillEnquiryPayload PreRegisterEbillEnquiry { get; set; }
    }

    public class PreRegisterEbillEnquiryPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("coverage")] public string Coverage { get; set; }
        [JsonProperty("dateFrom")] public string DateFrom { get; set; }
        [JsonProperty("dateTo")] public string DateTo { get; set; }
    }

    public class PreRegisterEbillTxnData
    {
        [JsonProperty("preRegisterEbillRequest")]
        public PreRegisterEbillRequest Request { get; set; }
    }

    public class PreRegisterEbillRequest
    {
        [JsonProperty("preRegisterEbill")]
        public PreRegisterEbillPayload PreRegisterEbill { get; set; }
    }

    public class PreRegisterEbillPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("payorCode")] public string PayorCode { get; set; }
        [JsonProperty("coverage")] public string Coverage { get; set; }
        [JsonProperty("dateFrom")] public string DateFrom { get; set; }
        [JsonProperty("dateTo")] public string DateTo { get; set; }
        [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; }
        [JsonProperty("invoiceDate")] public string InvoiceDate { get; set; }
        [JsonProperty("preparedBy")] public string PreparedBy { get; set; }
        [JsonProperty("totAmountDue")] public string TotAmountDue { get; set; }
        [JsonProperty("totSelectedClaims")] public string TotSelectedClaims { get; set; }
        [JsonProperty("selectedClaims")] public PreRegisterEbillSelectedClaim[] SelectedClaims { get; set; }
    }

    public class PreRegisterEbillSelectedClaim
    {
        [JsonProperty("clID")] public string ClID { get; set; }
    }
}
