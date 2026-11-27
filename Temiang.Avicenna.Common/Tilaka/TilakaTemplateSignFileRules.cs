using System.Collections.Generic;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaTemplateSignFileRules
    {
        public static bool CanRegister(bool isActive, bool isRegistered)
        {
            return isActive && !isRegistered;
        }

        public static bool CanDelete(bool isRegistered)
        {
            return !isRegistered;
        }

        public static bool ResolveIsDefault(bool isActive, bool requestedDefault)
        {
            return isActive && requestedDefault;
        }

        public static string NextTemplateNo(string paramedicID, IEnumerable<string> existingTemplateNos)
        {
            var max = 0;
            if (existingTemplateNos != null)
            {
                foreach (var templateNo in existingTemplateNos)
                {
                    if (string.IsNullOrWhiteSpace(templateNo)) continue;

                    var parts = templateNo.Split('-');
                    int value;
                    if (int.TryParse(parts[parts.Length - 1], out value) && value > max)
                        max = value;
                }
            }

            return "TTD-" + (paramedicID ?? string.Empty).Trim().ToUpperInvariant() +
                   "-" + (max + 1).ToString("00");
        }
    }
}
