namespace Temiang.Avicenna.Common.Admedika
{
    public interface IAdmedikaLogger
    {
        void Log(string serviceId, string requestBody, string responseBody, long elapsedMs);
    }
}
