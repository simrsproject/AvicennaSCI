using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaRegistrationActionRules
    {
        public static bool CanChangeMfa(TilakaRegistrationRecord record, string state)
        {
            return record != null
                   && state == TilakaRegistrationState.Aktif
                   && !string.IsNullOrWhiteSpace(record.TilakaName);
        }

        public static bool CanRevoke(TilakaRegistrationRecord record, string state)
        {
            return CanChangeMfa(record, state)
                   && !string.Equals(record.RevokeStatus,
                       TilakaConstant.RevokeStatus.Success,
                       StringComparison.OrdinalIgnoreCase);
        }
    }
}
