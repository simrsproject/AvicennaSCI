using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class GenerateUuidResponse : TilakaResponse
    {
        [JsonProperty("data")]
        public string[] Data { get; set; }
    }
}
