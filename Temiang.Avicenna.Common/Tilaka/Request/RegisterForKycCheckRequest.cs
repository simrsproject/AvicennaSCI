using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class RegisterForKycCheckRequest
    {
        [JsonProperty("registration_id")] public string RegistrationId { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("nik")] public string Nik { get; set; }
        [JsonProperty("photo_ktp")] public string PhotoKtp { get; set; }
        [JsonProperty("date_expire")] public string DateExpire { get; set; }
        [JsonProperty("is_approved")] public bool IsApproved { get; set; }
        [JsonProperty("consent_text")] public string ConsentText { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("hash_consent")] public string HashConsent { get; set; }
        [JsonProperty("consent_timestamp")] public string ConsentTimestamp { get; set; }

        [JsonProperty("nationality_type", NullValueHandling = NullValueHandling.Ignore)] public string NationalityType { get; set; }
        [JsonProperty("identity_type", NullValueHandling = NullValueHandling.Ignore)] public string IdentityType { get; set; }
        [JsonProperty("country_code", NullValueHandling = NullValueHandling.Ignore)] public string CountryCode { get; set; }
        [JsonProperty("passport_number", NullValueHandling = NullValueHandling.Ignore)] public string PassportNumber { get; set; }
        [JsonProperty("passport_file", NullValueHandling = NullValueHandling.Ignore)] public string PassportFile { get; set; }
        [JsonProperty("passport_date_expire", NullValueHandling = NullValueHandling.Ignore)] public string PassportDateExpire { get; set; }
        [JsonProperty("identity_date_expire", NullValueHandling = NullValueHandling.Ignore)] public string IdentityDateExpire { get; set; }
        [JsonProperty("company_supporting_document", NullValueHandling = NullValueHandling.Ignore)] public string CompanySupportingDocument { get; set; }
    }
}
