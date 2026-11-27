namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaCasemixFileRules
    {
        public static bool CanDelete(bool hasRequest)
        {
            return !hasRequest;
        }
    }
}
