using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class DischargeIpTxnData
    {
        [JsonProperty("dischargeRequest")]
        public DischargeIpRequest Request { get; set; }
    }

    public class DischargeIpRequest
    {
        [JsonProperty("discharge")]
        public DischargeIpPayload Discharge { get; set; }
    }

    public class DischargeIpPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
    }
}
