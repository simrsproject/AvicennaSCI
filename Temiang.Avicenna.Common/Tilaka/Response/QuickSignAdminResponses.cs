using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignPresignResponse : TilakaResponse
    {
        [JsonProperty("presigned_url")]
        public string PresignedUrl { get; set; }
    }

    public class QuickSignAddUsersRequest
    {
        [JsonProperty("users")] public QuickSignAddUser[] Users { get; set; }
    }

    public class QuickSignAddUser
    {
        [JsonProperty("email")] public string Email { get; set; }
    }

    public class QuickSignAddUsersResponse : TilakaResponse
    {
        [JsonProperty("data")] public QuickSignAddUserResult[] Data { get; set; }
    }

    public class QuickSignAddUserResult
    {
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("error")] public string Error { get; set; }
    }

    public class QuickSignAddTemplatesRequest
    {
        [JsonProperty("templates")] public QuickSignTemplateInput[] Templates { get; set; }
    }

    public class QuickSignTemplateInput
    {
        [JsonProperty("file_name")] public string FileName { get; set; }
        [JsonProperty("template_number")] public string TemplateNumber { get; set; }
        [JsonProperty("template_file")] public string TemplateFile { get; set; }
        [JsonProperty("assigned_users")] public QuickSignAssignedUser[] AssignedUsers { get; set; }
    }

    public class QuickSignAssignedUser
    {
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
    }

    public class QuickSignAddTemplatesResponse : TilakaResponse
    {
        [JsonProperty("successful")] public QuickSignTemplateResult[] Successful { get; set; }
        [JsonProperty("failed")] public QuickSignTemplateResult[] Failed { get; set; }
    }

    public class QuickSignTemplateResult
    {
        [JsonProperty("file_name")] public string FileName { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("template_no")] public string TemplateNo { get; set; }
        [JsonProperty("tilaka_name")] public string TilakaName { get; set; }
        [JsonProperty("variables")] public string Variables { get; set; }
        [JsonProperty("error")] public string Error { get; set; }
    }
}
