using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class CheckCertStatusResponse : TilakaResponse
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(FlexibleIntConverter))]
        public int Status { get; set; }

        [JsonProperty("message")]
        [JsonConverter(typeof(TilakaCertMessageConverter))]
        public new TilakaCertMessage Message { get; set; }

        [JsonProperty("data")]
        public TilakaCertificate[] Data { get; set; }
    }
}
