using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CancelOpenClaimsTxnResponse : AdmedikaResponse
    {
        [JsonProperty("msg")] public string Msg { get; set; }
    }
}
