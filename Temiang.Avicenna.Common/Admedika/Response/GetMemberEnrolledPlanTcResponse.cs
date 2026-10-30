using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class GetMemberEnrolledPlanTcResponse : AdmedikaResponse
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("dob")] public string Dob { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("covID")] public string CovID { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        private List<EnrolledPlan> _enrolledPlan = new List<EnrolledPlan>();

        [JsonProperty("enrolledPlan")]
        public List<EnrolledPlan> EnrolledPlan
        {
            get { return _enrolledPlan; }
            set { _enrolledPlan = value ?? new List<EnrolledPlan>(); }
        }
    }

    public class EnrolledPlan
    {
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("planID")] public string PlanID { get; set; }
        [JsonProperty("desc")] public string Desc { get; set; }

        private List<TermConditionItem> _termCondition = new List<TermConditionItem>();

        [JsonProperty("termCondition")]
        public List<TermConditionItem> TermCondition
        {
            get { return _termCondition; }
            set { _termCondition = value ?? new List<TermConditionItem>(); }
        }
    }

    public class TermConditionItem
    {
        [JsonProperty("tcDesc")] public string TcDesc { get; set; }
        [JsonProperty("guaranteeFlag")] public string GuaranteeFlag { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
