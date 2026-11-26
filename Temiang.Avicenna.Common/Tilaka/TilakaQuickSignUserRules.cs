namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaQuickSignUserRules
    {
        public static bool IsRegistered(TilakaRegistrationRecord record)
        {
            return record != null && record.IsQuickSignUser;
        }
    }
}
