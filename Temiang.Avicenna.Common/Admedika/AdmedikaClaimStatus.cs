using System.Collections.Generic;
using System.Linq;

namespace Temiang.Avicenna.Common.Admedika
{
    public sealed class AdmedikaClaimStatus
    {
        public string Code { get; private set; }
        public string Description { get; private set; }

        private AdmedikaClaimStatus(string code, string description)
        {
            Code = code;
            Description = description;
        }

        private static readonly AdmedikaClaimStatus[] Items =
        {
            new AdmedikaClaimStatus("08", "Eligibility Approved"),
            new AdmedikaClaimStatus("39", "Claims on hold"),
            new AdmedikaClaimStatus("40", "Valid claims"),
            new AdmedikaClaimStatus("41", "Incomplete documents"),
            new AdmedikaClaimStatus("42", "Reimbursement/Claims transaction initial status"),
            new AdmedikaClaimStatus("43", "Reimbursement/Claims transaction pending"),
            new AdmedikaClaimStatus("44", "Reimbursement/Claims transaction denied"),
            new AdmedikaClaimStatus("22", "Claims rejected")
        };

        public static IReadOnlyList<AdmedikaClaimStatus> All
        {
            get { return Items; }
        }

        public static string GetDescription(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return string.Empty;

            var item = Items.FirstOrDefault(i => i.Code == code);
            return item == null ? string.Empty : item.Description;
        }
    }
}
