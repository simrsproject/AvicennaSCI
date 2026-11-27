using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class RequestRevokeCertificateResponse : TilakaResponse
    {
        [JsonProperty("data")]
        public string[] Data { get; set; }
    }
}
