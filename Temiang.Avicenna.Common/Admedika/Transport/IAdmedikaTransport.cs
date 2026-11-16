namespace Temiang.Avicenna.Common.Admedika
{
    public interface IAdmedikaTransport
    {
        string Post(string url, string jsonBody, int timeoutSeconds);
        string PostMultipart(string url, string jsonBody, AdmedikaFile[] files, int timeoutSeconds);
    }
}
