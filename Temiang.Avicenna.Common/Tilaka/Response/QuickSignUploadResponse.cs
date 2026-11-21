using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignUploadResponse : TilakaResponse
    {
        [JsonProperty("filename")]
        public string FileName { get; set; }
    }
}
