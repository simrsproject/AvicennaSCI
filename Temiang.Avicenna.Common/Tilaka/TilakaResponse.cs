using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonIgnore]
        public string RawResponse { get; set; }

        [JsonIgnore]
        public int HttpStatus { get; set; }

        [JsonIgnore]
        public string TechnicalError { get; set; }

        [JsonIgnore]
        public bool IsOk
        {
            get { return string.IsNullOrEmpty(TechnicalError) && Success; }
        }
    }
}
