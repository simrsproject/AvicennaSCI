namespace Temiang.Avicenna.Common.Admedika
{
    public static class AdmedikaMask
    {
        public static string CardNo(string value)
        {
            if (value == null) return null;
            if (value.Length == 0) return string.Empty;
            if (value.Length <= 8) return new string('*', value.Length);

            return value.Substring(0, 4)
                   + new string('*', value.Length - 8)
                   + value.Substring(value.Length - 4);
        }
    }
}
