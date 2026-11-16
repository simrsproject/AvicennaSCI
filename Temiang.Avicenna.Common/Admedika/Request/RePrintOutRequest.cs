using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class RePrintOutTxnData
    {
        [JsonProperty("rePrintOutRequest")]
        public RePrintOutRequest Request { get; set; }
    }

    public class RePrintOutRequest
    {
        [JsonProperty("rePrintOut")]
        public RePrintOutPayload RePrintOut { get; set; }
    }

    public class RePrintOutPayload
    {
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("rePrintType")] public string RePrintType { get; set; }
    }
}
