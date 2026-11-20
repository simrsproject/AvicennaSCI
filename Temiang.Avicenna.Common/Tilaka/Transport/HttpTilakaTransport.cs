using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class HttpTilakaTransport : ITilakaTransport
    {
        private readonly X509Certificate2 _clientCertificate;

        static HttpTilakaTransport()
        {
            ServicePointManager.SecurityProtocol =
                ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
        }

        public HttpTilakaTransport()
        {
        }

        public HttpTilakaTransport(string certificatePath, string certificatePassword)
        {
            if (string.IsNullOrWhiteSpace(certificatePath)) return;
            _clientCertificate = new X509Certificate2(certificatePath, certificatePassword);
        }

        public TilakaTransportResult PostJson(string url, string jsonBody, string bearerToken, int timeoutSeconds)
        {
            var request = CreateRequest(url, "POST", "application/json", timeoutSeconds);
            if (!string.IsNullOrWhiteSpace(bearerToken))
                request.Headers[HttpRequestHeader.Authorization] = "Bearer " + bearerToken;

            return Send(request, Encoding.UTF8.GetBytes(jsonBody ?? string.Empty));
        }

        public TilakaTransportResult PostForm(string url, IDictionary<string, string> form, int timeoutSeconds)
        {
            var request = CreateRequest(url, "POST", "application/x-www-form-urlencoded", timeoutSeconds);
            return Send(request, Encoding.UTF8.GetBytes(BuildFormBody(form)));
        }

        public static string BuildFormBody(IDictionary<string, string> form)
        {
            if (form == null) return string.Empty;

            var builder = new StringBuilder();
            foreach (var item in form)
            {
                if (builder.Length > 0) builder.Append('&');
                builder.Append(Uri.EscapeDataString(item.Key ?? string.Empty));
                builder.Append('=');
                builder.Append(Uri.EscapeDataString(item.Value ?? string.Empty));
            }

            return builder.ToString();
        }

        private HttpWebRequest CreateRequest(string url, string method, string contentType, int timeoutSeconds)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.ContentType = contentType;
            request.Accept = "application/json";
            request.AllowAutoRedirect = false;
            request.KeepAlive = false;
            request.Timeout = timeoutSeconds * 1000;
            request.ReadWriteTimeout = timeoutSeconds * 1000;
            if (_clientCertificate != null) request.ClientCertificates.Add(_clientCertificate);
            return request;
        }

        private static TilakaTransportResult Send(HttpWebRequest request, byte[] body)
        {
            try
            {
                request.ContentLength = body.Length;
                if (body.Length > 0)
                {
                    using (var stream = request.GetRequestStream()) stream.Write(body, 0, body.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var statusCode = (int)response.StatusCode;
                    string responseBody;
                    using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        responseBody = reader.ReadToEnd();
                    }

                    if (statusCode < 200 || statusCode > 299)
                    {
                        throw new TilakaTransportException(
                            "Tilaka request failed with HTTP status " + statusCode + ".",
                            statusCode, responseBody);
                    }

                    return new TilakaTransportResult
                    {
                        StatusCode = statusCode,
                        Body = responseBody
                    };
                }
            }
            catch (WebException ex)
            {
                var response = ex.Response as HttpWebResponse;
                var status = response == null ? 0 : (int)response.StatusCode;
                throw new TilakaTransportException(ex.Message, status, ReadBody(ex.Response), ex);
            }
        }

        private static string ReadBody(WebResponse response)
        {
            if (response == null) return null;
            using (response)
            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return null;
                using (var reader = new StreamReader(stream, Encoding.UTF8)) return reader.ReadToEnd();
            }
        }
    }
}
