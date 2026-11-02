using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaRePrintOutResponse")]
    public class RePrintOutResponse : AdmedikaResponse
    {
        [JsonProperty("printOut")] public string PrintOut { get; set; }
    }
}
