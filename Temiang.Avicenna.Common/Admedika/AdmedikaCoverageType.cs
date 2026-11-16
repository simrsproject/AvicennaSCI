using System.Collections.Generic;
using System.Linq;

namespace Temiang.Avicenna.Common.Admedika
{
    public sealed class AdmedikaCoverageType
    {
        public string CoverageID { get; private set; }
        public string CoverageCode { get; private set; }
        public string Description { get; private set; }

        private AdmedikaCoverageType(string coverageId, string coverageCode, string description)
        {
            CoverageID = coverageId;
            CoverageCode = coverageCode;
            Description = description;
        }

        private static readonly AdmedikaCoverageType[] Items =
        {
            new AdmedikaCoverageType("01", "HNS", "RAWAT INAP"),
            new AdmedikaCoverageType("02", "GP", "RAWAT JALAN"),
            new AdmedikaCoverageType("03", "SP", "PRE & POST"),
            new AdmedikaCoverageType("04", "OP", "KACAMATA"),
            new AdmedikaCoverageType("05", "DENTAL", "RAWAT GIGI"),
            new AdmedikaCoverageType("06", "MAT", "PERSALINAN"),
            new AdmedikaCoverageType("07", "LAB", "Laboratory"),
            new AdmedikaCoverageType("08", "PHAR", "Pharmacy/Apotek")
        };

        public static IReadOnlyList<AdmedikaCoverageType> All
        {
            get { return Items; }
        }

        public static bool IsValid(string coverageId)
        {
            return Get(coverageId) != null;
        }

        public static AdmedikaCoverageType Get(string coverageId)
        {
            if (string.IsNullOrWhiteSpace(coverageId)) return null;

            var normalized = coverageId.Trim();
            return Items.FirstOrDefault(item => item.CoverageID == normalized);
        }

        public static string AllowedText()
        {
            return string.Join(", ", Items.Select(item => item.CoverageID));
        }
    }
}
