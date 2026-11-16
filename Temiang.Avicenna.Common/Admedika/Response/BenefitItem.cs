using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class BenefitItem
    {
        [JsonProperty("benID")] public string BenID { get; set; }
        [JsonProperty("shortName")] public string ShortName { get; set; }
        [JsonProperty("longName")] public string LongName { get; set; }
        [JsonProperty("availLimit")] public string AvailLimit { get; set; }
        [JsonProperty("freqDesc")] public string FreqDesc { get; set; }
        [JsonProperty("limitDesc")] public string LimitDesc { get; set; }
    }
}
