using System.Collections.Generic;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaTransport
    {
        TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds);
        TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds);
    }
}
