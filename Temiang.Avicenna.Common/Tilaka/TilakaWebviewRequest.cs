namespace Temiang.Avicenna.Common.Tilaka
{
    public static class TilakaWebviewRequest
    {
        public static string ResolveRequestId(string requestId, string requestIdAlias, string registerId)
        {
            if (!string.IsNullOrWhiteSpace(requestId)) return requestId;
            if (!string.IsNullOrWhiteSpace(requestIdAlias)) return requestIdAlias;
            return registerId;
        }
    }
}
