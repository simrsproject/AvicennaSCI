namespace Temiang.Avicenna.Common.Admedika
{
    public class WebServiceApiLogger : IAdmedikaLogger
    {
        public WebServiceApiLogger(AdmedikaConfig config)
        {
        }

        public void Log(string serviceId, string requestBody, string responseBody, long elapsedMs)
        {
        }
    }
}
