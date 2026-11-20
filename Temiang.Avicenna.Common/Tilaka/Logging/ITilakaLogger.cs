namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaLogger
    {
        void Log(string operation, string requestBody, string responseBody, long elapsedMs);
    }
}
