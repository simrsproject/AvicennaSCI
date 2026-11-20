namespace Temiang.Avicenna.Common.Tilaka
{
    public class NullTilakaLogger : ITilakaLogger
    {
        public void Log(string operation, string requestBody, string responseBody, long elapsedMs)
        {
        }
    }
}
