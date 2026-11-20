using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaTransportTests
    {
        [TestMethod]
        public void PostForm_BuildsUrlEncodedBody()
        {
            var body = HttpTilakaTransport.BuildFormBody(new Dictionary<string, string>
            {
                { "client_id", "chan 1" },
                { "grant_type", "client_credentials" },
                { "client_secret", "a&b" }
            });

            Assert.AreEqual("client_id=chan%201&grant_type=client_credentials&client_secret=a%26b", body);
        }

        [TestMethod]
        public void TransportException_CarriesStatusAndBody()
        {
            var ex = new TilakaTransportException("failed", 401, "{\"success\":false}");
            Assert.AreEqual(401, ex.StatusCode);
            Assert.AreEqual("{\"success\":false}", ex.ResponseBody);
        }

        [TestMethod]
        public void PostJson_RedirectSurfacesStatusAndBody()
        {
            using (var server = new SingleResponseServer(302, "Found", "/login", "{\"redirected\":true}"))
            {
                var transport = new HttpTilakaTransport();

                try
                {
                    transport.PostJson(server.Url, "{\"grant_type\":\"client_credentials\"}", null, 15);
                    Assert.Fail("Expected TilakaTransportException for a 302 response.");
                }
                catch (TilakaTransportException ex)
                {
                    Assert.AreEqual(302, ex.StatusCode);
                    Assert.AreEqual("{\"redirected\":true}", ex.ResponseBody);
                }
            }
        }

        private sealed class SingleResponseServer : IDisposable
        {
            private readonly TcpListener _listener;
            private readonly Thread _thread;
            private readonly int _status;
            private readonly string _reason;
            private readonly string _location;
            private readonly string _body;

            public string Url { get; private set; }

            public SingleResponseServer(int status, string reason, string location, string body)
            {
                _status = status;
                _reason = reason;
                _location = location;
                _body = body;

                _listener = new TcpListener(IPAddress.Loopback, 0);
                _listener.Start();

                var port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                Url = "http://127.0.0.1:" + port + "/auth";

                _thread = new Thread(Serve) { IsBackground = true };
                _thread.Start();
            }

            public void Dispose()
            {
                try { _listener.Stop(); } catch (SocketException) { }
                if (!_thread.Join(2000)) { /* background thread, do not block test teardown */ }
            }

            private void Serve()
            {
                try
                {
                    using (var client = _listener.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    {
                        ReadRequest(stream);

                        var bodyBytes = Encoding.UTF8.GetBytes(_body);
                        var head = "HTTP/1.1 " + _status + " " + _reason + "\r\n"
                                   + "Location: " + _location + "\r\n"
                                   + "Content-Type: application/json; charset=utf-8\r\n"
                                   + "Content-Length: " + bodyBytes.Length + "\r\n"
                                   + "Connection: close\r\n\r\n";

                        var headBytes = Encoding.ASCII.GetBytes(head);
                        stream.Write(headBytes, 0, headBytes.Length);
                        stream.Write(bodyBytes, 0, bodyBytes.Length);
                        stream.Flush();
                    }
                }
                catch (Exception)
                {
                    // The test asserts on the client side; a torn-down listener is expected.
                }
                finally
                {
                    try { _listener.Stop(); } catch (SocketException) { }
                }
            }

            private static void ReadRequest(Stream stream)
            {
                var buffer = new byte[4096];
                var received = new StringBuilder();

                while (received.ToString().IndexOf("\r\n\r\n", StringComparison.Ordinal) < 0)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0) return;
                    received.Append(Encoding.ASCII.GetString(buffer, 0, read));
                }

                var headers = received.ToString();
                var contentLength = 0;
                foreach (var line in headers.Split(new[] { "\r\n" }, StringSplitOptions.None))
                {
                    if (line.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
                    {
                        int.TryParse(line.Substring("Content-Length:".Length).Trim(), out contentLength);
                    }
                }

                var bodyStart = headers.IndexOf("\r\n\r\n", StringComparison.Ordinal) + 4;
                var alreadyRead = Encoding.ASCII.GetByteCount(headers.Substring(bodyStart));
                var remaining = contentLength - alreadyRead;
                while (remaining > 0)
                {
                    var read = stream.Read(buffer, 0, Math.Min(buffer.Length, remaining));
                    if (read <= 0) break;
                    remaining -= read;
                }
            }
        }
    }
}
