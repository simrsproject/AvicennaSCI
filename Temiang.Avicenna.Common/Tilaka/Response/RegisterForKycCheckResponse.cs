using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class RegisterForKycCheckResponse : TilakaResponse
    {
        [JsonProperty("data")]
        public string[] Data { get; set; }
    }
}
