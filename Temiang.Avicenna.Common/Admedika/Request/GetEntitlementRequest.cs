using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class GetEntitlementTxnData
    {
        [JsonProperty("getEntitlementRequest")]
        public GetEntitlementRequest Request { get; set; }
    }

    public class GetEntitlementRequest
    {
        [JsonProperty("getEntitlement")]
        public GetEntitlementPayload GetEntitlement { get; set; }
    }

    public class GetEntitlementPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
    }
}
