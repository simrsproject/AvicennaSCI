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
        [JsonProperty("enrolledPlan")] public List<EnrolledPlan> EnrolledPlan { get; set; }

        public GetMemberEnrolledPlanTcResponse()
        {
            EnrolledPlan = new List<EnrolledPlan>();
        }
    }

    public class EnrolledPlan
    {
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("planID")] public string PlanID { get; set; }
        [JsonProperty("desc")] public string Desc { get; set; }
        [JsonProperty("termCondition")] public List<TermConditionItem> TermCondition { get; set; }

        public EnrolledPlan()
        {
            TermCondition = new List<TermConditionItem>();
        }
    }

    public class TermConditionItem
    {
        [JsonProperty("tcDesc")] public string TcDesc { get; set; }
        [JsonProperty("guaranteeFlag")] public string GuaranteeFlag { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
    }
}
