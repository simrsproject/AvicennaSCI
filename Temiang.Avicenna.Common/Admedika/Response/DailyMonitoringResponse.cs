using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class DailyMonitoringResponse : AdmedikaResponse
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("er")] public string Er { get; set; }
        [JsonProperty("msg")] public string Msg { get; set; }
        [JsonProperty("ok")] public string Ok { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("providerTransID")] public string ProviderTransID { get; set; }
        [JsonProperty("terminalID")] public string TerminalID { get; set; }
        [JsonProperty("totAmtApproved")] public string TotAmtApproved { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtNotApproved")] public string TotAmtNotApproved { get; set; }

        private List<DailyMonitoringFormResult> _dmForms = new List<DailyMonitoringFormResult>();

        [JsonProperty("dmForms")]
        public List<DailyMonitoringFormResult> DmForms
        {
            get { return _dmForms; }
            set { _dmForms = value ?? new List<DailyMonitoringFormResult>(); }
        }
    }

    public class DailyMonitoringFormResult
    {
        [JsonProperty("code")] public string Code { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("input")] public string Input { get; set; }
        [JsonProperty("date")] public string Date { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
    }
}
