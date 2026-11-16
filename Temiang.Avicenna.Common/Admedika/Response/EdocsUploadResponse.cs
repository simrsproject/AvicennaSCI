using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaEdocsUploadResponse")]
    public class EdocsUploadResponse : AdmedikaResponse
    {
        [JsonProperty("msg")] public string Msg { get; set; }
    }
}
