using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class HelloWorldResponse : AdmedikaResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("apiName")]
        public string ApiName { get; set; }

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; }
    }
}
