using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Tests.Tilaka
{
    [TestClass]
    public class TilakaConfigTests
    {
        [TestMethod]
        public void ResolveFilePath_RootedPath_IsUsedAsIs()
        {
            var result = TilakaConfig.ResolveFilePath(@"C:\Tilaka\TilakaConsent.txt", p => @"D:\app\" + p);
            Assert.AreEqual(@"C:\Tilaka\TilakaConsent.txt", result);
        }

        [TestMethod]
        public void ResolveFilePath_VirtualPath_IsMapped()
        {
            var result = TilakaConfig.ResolveFilePath("~/App_Data/TilakaConsent.txt", p => @"D:\app\App_Data\TilakaConsent.txt");
            Assert.AreEqual(@"D:\app\App_Data\TilakaConsent.txt", result);
        }

        [TestMethod]
        public void ResolveFilePath_Empty_ReturnsNull()
        {
            Assert.IsNull(TilakaConfig.ResolveFilePath("", p => @"D:\app"));
        }

        [TestMethod]
        public void ResolveFilePath_HttpUrl_ReturnsNull()
        {
            Assert.IsNull(TilakaConfig.ResolveFilePath("http://10.200.200.11/tilaka_doc/TilakaConsent.txt",
                p => @"D:\app"));
        }

        [TestMethod]
        public void ReadConsentText_LocalFile_TrimsTrailingNewLines()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "TnC lokal\r\n", Encoding.UTF8);
                var config = new TilakaConfig { ConsentTextFile = path };

                Assert.AreEqual("TnC lokal", config.ReadConsentText());
            }
            finally
            {
                File.Delete(path);
            }
        }

        [TestMethod]
        public void ReadConsentText_HttpUrl_ReturnsDownloadedText()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();

            try
            {
                var port = ((IPEndPoint)listener.LocalEndpoint).Port;
                var server = Task.Run(() => ServeOnce(listener, "TnC dari URL\r\n"));
                var config = new TilakaConfig
                {
                    ConsentTextFile = "http://127.0.0.1:" + port + "/TilakaConsent.txt"
                };

                var text = config.ReadConsentText();

                Assert.AreEqual("TnC dari URL", text);
                Assert.IsTrue(server.Wait(5000), "Server uji tidak selesai.");
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void ServeOnce(TcpListener listener, string body)
        {
            using (var client = listener.AcceptTcpClient())
            using (var stream = client.GetStream())
            {
                var reader = new StreamReader(stream, Encoding.ASCII);
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                }

                var content = Encoding.UTF8.GetBytes(body);
                var header = "HTTP/1.1 200 OK\r\n" +
                             "Content-Type: text/plain; charset=utf-8\r\n" +
                             "Content-Length: " + content.Length + "\r\n" +
                             "Connection: close\r\n\r\n";
                var headerBytes = Encoding.ASCII.GetBytes(header);
                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(content, 0, content.Length);
            }
        }

        [TestMethod]
        public void IsCompanyChannel_TrueWhenCompany()
        {
            var config = new TilakaConfig { ChannelType = "Company" };
            Assert.IsTrue(config.IsCompanyChannel);
        }
    }
}
