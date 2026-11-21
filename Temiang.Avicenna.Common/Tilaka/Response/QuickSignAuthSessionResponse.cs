using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignAuthSessionResponse : TilakaResponse
    {
        [JsonProperty("data")] public QuickSignAuthSession[] Data { get; set; }
        [JsonProperty("pagination")] public QuickSignPagination Pagination { get; set; }
    }

    public class QuickSignAuthSession
    {
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("tilaka_name")] public string TilakaName { get; set; }
        [JsonProperty("registered_date")] public string RegisteredDate { get; set; }
        [JsonProperty("auth_interval_duration")] public int? AuthIntervalDuration { get; set; }
        [JsonProperty("auth_activated_date")] public string AuthActivatedDate { get; set; }
        [JsonProperty("auth_expired_date")] public string AuthExpiredDate { get; set; }
        [JsonProperty("auth_status")] public string AuthStatus { get; set; }
        [JsonProperty("template_numbers")] public string[] TemplateNumbers { get; set; }
    }

    public class QuickSignPagination
    {
        [JsonProperty("page_number")] public int PageNumber { get; set; }
        [JsonProperty("number_of_elements")] public int NumberOfElements { get; set; }
        [JsonProperty("limit")] public int Limit { get; set; }
        [JsonProperty("total_elements")] public int TotalElements { get; set; }
        [JsonProperty("total_pages")] public int TotalPages { get; set; }
    }
}
