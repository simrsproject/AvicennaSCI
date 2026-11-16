using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaCancelOpenClaimsTxnResponse")]
    public class CancelOpenClaimsTxnResponse : AdmedikaResponse
    {
        [JsonProperty("msg")] public string Msg { get; set; }
    }
}
