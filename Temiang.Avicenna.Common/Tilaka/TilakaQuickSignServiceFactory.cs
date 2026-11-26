namespace Temiang.Avicenna.Common.Tilaka
{
    /// <summary>
    /// Membuat instance Service yang menunjuk ke tilaka-lite (Quick Sign),
    /// terpisah dari instance cloud Sign Plus.
    /// </summary>
    public static class TilakaQuickSignServiceFactory
    {
        public static Service Create()
        {
            return Create(new TilakaConfig());
        }

        public static Service Create(TilakaConfig source)
        {
            source = source ?? new TilakaConfig();

            var liteBaseUrl = (source.LiteBaseUrl ?? string.Empty).TrimEnd('/');
            var config = new TilakaConfig
            {
                BaseUrl = liteBaseUrl,
                LiteBaseUrl = liteBaseUrl,
                AuthBaseUrl = source.BaseUrl,
                CloudBaseUrl = source.BaseUrl,
                AuthPath = source.AuthPath,
                ChannelId = source.ChannelId,
                ChannelSecret = source.ChannelSecret,
                TimeoutSeconds = source.LiteTimeoutSeconds,
                IsLogEnabled = source.LiteIsLogEnabled,
                ClientCertificatePath = string.Empty,
                ClientCertificatePassword = string.Empty
            };

            return new Service(config, new HttpTilakaTransport(), new WebServiceApiLogger(config));
        }
    }
}
