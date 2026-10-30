using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class GetMemberEnrolledPlanTcTxnData
    {
        [JsonProperty("getMemberEnrolledPlanTCRequest")]
        public GetMemberEnrolledPlanTcRequest Request { get; set; }
    }

    public class GetMemberEnrolledPlanTcRequest
    {
        [JsonProperty("getMemberEnrolledPlanTC")]
        public GetMemberEnrolledPlanTcPayload GetMemberEnrolledPlanTC { get; set; }
    }

    public class GetMemberEnrolledPlanTcPayload
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("covID")] public string CovID { get; set; }
        [JsonProperty("searchForTermCondition")] public string SearchForTermCondition { get; set; }
    }
}
