using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignStatusResponse : TilakaResponse
    {
        [JsonProperty("status")] public QuickSignSignerStatus[] Status { get; set; }
        [JsonProperty("request_id")] public string RequestId { get; set; }
        [JsonProperty("list_pdf")] public QuickSignDocumentStatus[] ListPdf { get; set; }
    }

    public class QuickSignSignerStatus
    {
        [JsonProperty("sequence")] public int Sequence { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("num_signatures")] public int? NumSignatures { get; set; }
        [JsonProperty("num_signatures_done")] public int? NumSignaturesDone { get; set; }
    }

    public class QuickSignDocumentStatus
    {
        [JsonProperty("filename")] public string FileName { get; set; }
        [JsonProperty("error")] public bool? Error { get; set; }
        [JsonProperty("presigned_url")] public string PresignedUrl { get; set; }
    }
}
