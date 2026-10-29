using System.Linq;

namespace Temiang.Avicenna.BusinessObject
{
    public partial class VwGuarantorAdmedika
    {
        /// <summary>
        /// Gets all GuarantorID from Vw_GuarantorAdmedika
        /// (active guarantors whose name contains "admedika").
        /// </summary>
        public static string[] GetGuarantorIDs()
        {
            var coll = new VwGuarantorAdmedikaCollection();
            if (!coll.Query.Load())
                return new string[] { };

            return coll.Select(g => g.GuarantorID)
                       .Where(id => !string.IsNullOrEmpty(id))
                       .Distinct()
                       .ToArray();
        }

        /// <summary>
        /// Gets all GuarantorID from Vw_GuarantorAdmedika as a comma separated string,
        /// e.g. "G001,G002,G003". Returns an empty string when there is no data.
        /// </summary>
        public static string GetGuarantorIDsString()
        {
            return string.Join(",", GetGuarantorIDs());
        }
    }
}
