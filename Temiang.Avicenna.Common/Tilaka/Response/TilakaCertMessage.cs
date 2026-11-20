using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaCertMessage
    {
        [JsonProperty("info")] public string Info { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("company")] public string Company { get; set; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("serialnumber")] public string SerialNumber { get; set; }
    }
}
