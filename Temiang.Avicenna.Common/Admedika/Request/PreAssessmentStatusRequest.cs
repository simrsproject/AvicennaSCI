using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class PreAssessmentStatusTxnData
    {
        [JsonProperty("preAssessmentStatusRequest")]
        public PreAssessmentStatusRequest Request { get; set; }
    }

    public class PreAssessmentStatusRequest
    {
        [JsonProperty("preAssessmentStatus")]
        public PreAssessmentStatusPayload PreAssessmentStatus { get; set; }
    }

    public class PreAssessmentStatusPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
    }
}
