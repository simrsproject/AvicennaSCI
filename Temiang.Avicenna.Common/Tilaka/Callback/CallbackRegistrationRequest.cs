using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class CallbackRegistrationRequest
    {
        [JsonProperty("RegisterID")] public string RegisterId { get; set; }
        [JsonProperty("success")] public bool Success { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("data")] public TilakaKycData Data { get; set; }
    }
}
