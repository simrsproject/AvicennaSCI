using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class CallbackSigningRequest : TilakaResponse
    {
        [JsonProperty("request_id")] public string RequestId { get; set; }
        [JsonProperty("user_info")] public QuickSignSignerInfo UserInfo { get; set; }
        [JsonProperty("sign_info")] public QuickSignSignInfo[] SignInfo { get; set; }
    }

    public class QuickSignSignerInfo
    {
        [JsonProperty("sequence")] public int Sequence { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("num_signatures")] public int? NumSignatures { get; set; }
        [JsonProperty("num_signatures_done")] public int? NumSignaturesDone { get; set; }
    }

    public class QuickSignSignInfo
    {
        [JsonProperty("filename")] public string FileName { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("presigned_url")] public string PresignedUrl { get; set; }
    }
}
