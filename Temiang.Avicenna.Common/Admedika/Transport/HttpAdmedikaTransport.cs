using System;
using System.IO;
using System.Net;
using System.Text;

namespace Temiang.Avicenna.Common.Admedika
{
    public class HttpAdmedikaTransport : IAdmedikaTransport
    {
        static HttpAdmedikaTransport()
        {
            ServicePointManager.SecurityProtocol =
                ServicePointManager.SecurityProtocol | SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
        }

        public string Post(string url, string jsonBody, int timeoutSeconds)
        {
            return PostRaw(url, Encoding.UTF8.GetBytes(jsonBody ?? string.Empty),
                "application/json", timeoutSeconds);
        }

        public string PostMultipart(string url, string jsonBody, AdmedikaFile[] files, int timeoutSeconds)
        {
            var boundary = CreateBoundary();
            var body = BuildMultipartBody(boundary, jsonBody, files);
            return PostRaw(url, body, "multipart/form-data; boundary=" + boundary, timeoutSeconds);
        }

        public static string CreateBoundary()
        {
            return "----AdmedikaBoundary" + Guid.NewGuid().ToString("N");
        }

        public static byte[] BuildMultipartBody(string boundary, string jsonBody, AdmedikaFile[] files)
        {
            using (var stream = new MemoryStream())
            {
                Write(stream, "--" + boundary + "\r\n");
                Write(stream, "Content-Disposition: form-data; name=\"data\"\r\n");
                Write(stream, "Content-Type: application/json; charset=utf-8\r\n\r\n");
                Write(stream, jsonBody ?? string.Empty);
                Write(stream, "\r\n");

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file == null) continue;

                        var fileName = (file.FileName ?? "file").Replace("\"", "_");
                        var contentType = string.IsNullOrWhiteSpace(file.ContentType)
                            ? "application/octet-stream"
                            : file.ContentType;

                        Write(stream, "--" + boundary + "\r\n");
                        Write(stream, "Content-Disposition: form-data; name=\"files\"; filename=\""
                                      + fileName + "\"\r\n");
                        Write(stream, "Content-Type: " + contentType + "\r\n\r\n");

                        var content = file.Content ?? new byte[0];
                        stream.Write(content, 0, content.Length);
                        Write(stream, "\r\n");
                    }
                }

                Write(stream, "--" + boundary + "--\r\n");
                return stream.ToArray();
            }
        }

        private static void Write(Stream stream, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        private static string PostRaw(string url, byte[] body, string contentType, int timeoutSeconds)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = contentType;
                request.Accept = "application/json";
                request.KeepAlive = false;
                request.Timeout = timeoutSeconds * 1000;
                request.ReadWriteTimeout = timeoutSeconds * 1000;
                request.ContentLength = body.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                throw new AdmedikaTransportException(ex.Message, ReadBody(ex.Response), ex);
            }
            catch (AdmedikaTransportException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AdmedikaTransportException(ex.Message, null, ex);
            }
        }

        private static string ReadBody(WebResponse response)
        {
            if (response == null) return null;

            using (var stream = response.GetResponseStream())
            {
                if (stream == null) return null;

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
