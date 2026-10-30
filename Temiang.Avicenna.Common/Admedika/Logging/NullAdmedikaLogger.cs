namespace Temiang.Avicenna.Common.Admedika
{
    public class NullAdmedikaLogger : IAdmedikaLogger
    {
        public void Log(string serviceId, string requestBody, string responseBody, long elapsedMs)
        {
        }
    }
}
