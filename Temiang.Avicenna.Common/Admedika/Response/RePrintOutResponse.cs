using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class RePrintOutResponse : AdmedikaResponse
    {
        [JsonProperty("printOut")] public string PrintOut { get; set; }
    }
}
