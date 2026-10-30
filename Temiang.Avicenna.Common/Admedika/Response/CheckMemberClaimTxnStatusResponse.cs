using System.Collections.Generic;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class CheckMemberClaimTxnStatusResponse : AdmedikaResponse
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("dob")] public string Dob { get; set; }
        [JsonProperty("covCode")] public string CovCode { get; set; }
        [JsonProperty("icdCode")] public string IcdCode { get; set; }
        [JsonProperty("icdDesc")] public string IcdDesc { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("clStatus")] public string ClStatus { get; set; }
        [JsonProperty("clDesc")] public string ClDesc { get; set; }
        [JsonProperty("payorName")] public string PayorName { get; set; }
        [JsonProperty("totAmtIncurred")] public string TotAmtIncurred { get; set; }
        [JsonProperty("totAmtApproved")] public string TotAmtApproved { get; set; }
        [JsonProperty("totAmtNotApproved")] public string TotAmtNotApproved { get; set; }
        [JsonProperty("entitlement")] public List<BenefitItem> Entitlement { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }

        public CheckMemberClaimTxnStatusResponse()
        {
            Entitlement = new List<BenefitItem>();
        }
    }
}
