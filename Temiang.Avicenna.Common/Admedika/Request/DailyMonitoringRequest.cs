using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class DailyMonitoringTxnData
    {
        [JsonProperty("dailyMonitoringRequest")]
        public DailyMonitoringRequest Request { get; set; }
    }

    public class DailyMonitoringRequest
    {
        [JsonProperty("dailyMonitoring")]
        public DailyMonitoringPayload DailyMonitoring { get; set; }
    }

    public class DailyMonitoringPayload
    {
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("dmUser")] public string DmUser { get; set; }
        [JsonProperty("dmForms")] public DailyMonitoringForm[] DmForms { get; set; }
    }

    public class DailyMonitoringForm
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("input")] public string Input { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
    }
}
