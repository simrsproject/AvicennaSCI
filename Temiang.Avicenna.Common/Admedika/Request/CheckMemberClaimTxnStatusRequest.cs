using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CheckMemberClaimTxnStatusTxnData
    {
        [JsonProperty("checkMemberClaimTxnStatusRequest")]
        public CheckMemberClaimTxnStatusRequest Request { get; set; }
    }

    public class CheckMemberClaimTxnStatusRequest
    {
        [JsonProperty("checkMemberClaimTxnStatus")]
        public CheckMemberClaimTxnStatusPayload CheckMemberClaimTxnStatus { get; set; }
    }

    public class CheckMemberClaimTxnStatusPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
    }
}
