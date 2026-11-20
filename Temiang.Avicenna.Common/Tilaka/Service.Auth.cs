using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        private static readonly object TokenSync = new object();
        private static readonly Dictionary<string, CachedToken> TokenCache = new Dictionary<string, CachedToken>();

        private class CachedToken
        {
            public string Token;
            public DateTime ExpiresUtc;
        }

        public static void ResetTokenCache()
        {
            lock (TokenSync) TokenCache.Clear();
        }

        public string GetAccessToken(bool forceRefresh = false)
        {
            var key = _config.ChannelId ?? string.Empty;

            lock (TokenSync)
            {
                CachedToken cached;
                if (!forceRefresh && TokenCache.TryGetValue(key, out cached) && cached.ExpiresUtc > DateTime.UtcNow)
                    return cached.Token;
            }

            var form = new Dictionary<string, string>
            {
                { "client_id", _config.ChannelId },
                { "grant_type", "client_credentials" },
                { "client_secret", _config.ChannelSecret }
            };

            var result = _transport.PostForm(TilakaUrls.Auth(_config), form, _config.TimeoutSeconds);
            if (result.StatusCode != 200 || string.IsNullOrWhiteSpace(result.Body))
                throw new InvalidOperationException("Gagal mendapatkan access token Tilaka. HTTP " + result.StatusCode);

            var json = JObject.Parse(result.Body);
            var token = (string)json["access_token"];
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("Respons auth Tilaka tidak berisi access_token.");

            var expiresIn = 300;
            var expiresToken = json["expires_in"];
            if (expiresToken != null) int.TryParse(expiresToken.ToString(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out expiresIn);

            lock (TokenSync)
            {
                TokenCache[key] = new CachedToken
                {
                    Token = token,
                    ExpiresUtc = DateTime.UtcNow.AddSeconds(Math.Max(expiresIn - 60, 30))
                };
            }

            return token;
        }

        private void InvalidateToken()
        {
            lock (TokenSync) TokenCache.Remove(_config.ChannelId ?? string.Empty);
        }
    }
}
