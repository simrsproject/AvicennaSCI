namespace Temiang.Avicenna.Common.Admedika
{
    public interface IAdmedikaTransport
    {
        string Post(string url, string jsonBody, int timeoutSeconds);
    }
}
