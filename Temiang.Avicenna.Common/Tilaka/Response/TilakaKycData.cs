using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaKycData
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("nik")] public bool? Nik { get; set; }
        [JsonProperty("nama")] public bool? Nama { get; set; }
        [JsonProperty("photo_selfie")] public string PhotoSelfie { get; set; }
        [JsonProperty("fr_score")] public string FrScore { get; set; }
        [JsonProperty("fr_score_percentage")] public string FrScorePercentage { get; set; }
        [JsonProperty("liveness_result")] public bool? LivenessResult { get; set; }
        [JsonProperty("liveness_fail_message")] public string LivenessFailMessage { get; set; }
        [JsonProperty("summary_verification_result")] public bool? SummaryVerificationResult { get; set; }
        [JsonProperty("tilaka_name")] public string TilakaName { get; set; }
        [JsonProperty("reason_code")] public string ReasonCode { get; set; }
        [JsonProperty("manual_registration_status")] public string ManualRegistrationStatus { get; set; }
    }
}
