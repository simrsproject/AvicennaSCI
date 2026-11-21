using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignRequestResponse : TilakaResponse
    {
        [JsonProperty("auth_response")]
        public QuickSignAuthResponse[] AuthResponse { get; set; }

        [JsonProperty("failed_doc_name")]
        public string FailedDocName { get; set; }
    }

    public class QuickSignAuthResponse
    {
        [JsonProperty("url")] public string Url { get; set; }
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("status")] public bool Status { get; set; }
        [JsonProperty("error_message")] public string ErrorMessage { get; set; }
        [JsonProperty("token_key")] public string TokenKey { get; set; }
    }
}
