using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class CallbackCertificateStatusRequest
    {
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("success")] public bool Success { get; set; }
        [JsonProperty("request_id")] public string RequestId { get; set; }

        [JsonProperty("status")]
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int Status { get; set; }
    }
}
