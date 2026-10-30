using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class EdocsUploadResponse : AdmedikaResponse
    {
        [JsonProperty("msg")] public string Msg { get; set; }
    }
}
