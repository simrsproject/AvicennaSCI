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
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Accept = "application/json";
                request.KeepAlive = false;
                request.Timeout = timeoutSeconds * 1000;
                request.ReadWriteTimeout = timeoutSeconds * 1000;

                var bytes = Encoding.UTF8.GetBytes(jsonBody ?? string.Empty);
                request.ContentLength = bytes.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(bytes, 0, bytes.Length);
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
