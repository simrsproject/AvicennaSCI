using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    [XmlRoot("AdmedikaPreAssessmentStatusResponse")]
    public class PreAssessmentStatusResponse : AdmedikaResponse
    {
        [JsonProperty("statusText")] public string StatusText { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("form_f_link")] public string FormFLink { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtApproved")] public string TotAmtApproved { get; set; }
        [JsonProperty("totAmtNotApproved")] public string TotAmtNotApproved { get; set; }

        private List<DischargeEntitlementResult> _entitlement =
            new List<DischargeEntitlementResult>();

        [JsonProperty("entitlement")]
        public List<DischargeEntitlementResult> Entitlement
        {
            get { return _entitlement; }
            set { _entitlement = value ?? new List<DischargeEntitlementResult>(); }
        }
    }
}
