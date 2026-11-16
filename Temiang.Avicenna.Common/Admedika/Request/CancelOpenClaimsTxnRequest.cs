using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CancelOpenClaimsTxnTxnData
    {
        [JsonProperty("cancelOpenClaimTxnRequest")]
        public CancelOpenClaimsTxnRequest Request { get; set; }
    }

    public class CancelOpenClaimsTxnRequest
    {
        [JsonProperty("cancelOpenClaimTxn")]
        public CancelOpenClaimsTxnPayload CancelOpenClaimTxn { get; set; }
    }

    public class CancelOpenClaimsTxnPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
