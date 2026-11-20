using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaCertificate
    {
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("serialnumber")] public string SerialNumber { get; set; }
        [JsonProperty("subject_dn")] public string SubjectDn { get; set; }
        [JsonProperty("start_active_date")] public string StartActiveDate { get; set; }
        [JsonProperty("expiry_date")] public string ExpiryDate { get; set; }
        [JsonProperty("certificate")] public string Certificate { get; set; }
    }
}
