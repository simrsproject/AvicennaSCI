using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CheckIcdExclusionTxnData
    {
        [JsonProperty("checkIcdExclusionRequest")]
        public CheckIcdExclusionRequest Request { get; set; }
    }

    public class CheckIcdExclusionRequest
    {
        [JsonProperty("checkIcdExclusion")]
        public CheckIcdExclusionPayload CheckIcdExclusion { get; set; }
    }

    public class CheckIcdExclusionPayload
    {
        [JsonProperty("cardNo")]
        public string CardNo { get; set; }

        [JsonProperty("covID")]
        public string CovID { get; set; }

        [JsonProperty("diagnosisCodeList")]
        public string DiagnosisCodeList { get; set; }
    }
}
