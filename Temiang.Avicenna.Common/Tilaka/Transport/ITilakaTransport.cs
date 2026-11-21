using System.Collections.Generic;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaTransport
    {
        TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds);
        TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds);
        TilakaTransportResult Get(string url, string bearerToken, int timeoutSeconds);
        TilakaTransportResult PostMultipart(string url, string formName, string fileName,
            byte[] content, string contentType, string bearerToken, int timeoutSeconds);
    }
}
