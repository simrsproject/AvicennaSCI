using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class UserRegStatusResponse : TilakaResponse
    {
        [JsonProperty("data")]
        public TilakaKycData Data { get; set; }
    }
}
