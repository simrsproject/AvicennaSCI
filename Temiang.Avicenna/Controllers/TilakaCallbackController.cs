using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.Controllers
{
    /// <summary>
    /// Penerima callback Tilaka. Selalu membalas HTTP 200 agar Tilaka tidak retry tanpa henti.
    /// </summary>
    public class TilakaCallbackController : Controller
    {
        [HttpPost]
        public ActionResult Registration()
        {
            var config = new TilakaConfig();
            var logger = new WebServiceApiLogger(config);
            string raw = null;

            try
            {
                raw = ReadRawBody();

                if (!IsConfigured(config))
                {
                    Log(logger, "Callback-Registration-Rejected", raw, "not-configured");
                    return Content("OK");
                }

                string error;
                if (!TilakaSignature.VerifyCallback(config.ChannelId, config.ChannelSecret,
                        Request.Headers["x-request-timestamp"], raw, Request.Headers["x-validation-token"],
                        config.CallbackToleranceMinutes, out error))
                {
                    Log(logger, "Callback-Registration-Rejected", raw, error);
                    return Content("OK");
                }

                var payload = JsonConvert.DeserializeObject<CallbackRegistrationRequest>(raw);
                var processor = new TilakaCallbackProcessor(new TilakaRegistrationRepository());
                var outcome = processor.ProcessRegistration(payload);
                Log(logger, "Callback-Registration", raw, outcome.Applied ? "applied" : outcome.Reason);
            }
            catch (Exception ex)
            {
                Log(logger, "Callback-Registration-Error", raw, ex.Message);
            }

            return Content("OK");
        }

        [HttpPost]
        public ActionResult CertificateStatus()
        {
            var config = new TilakaConfig();
            var logger = new WebServiceApiLogger(config);
            string raw = null;

            try
            {
                raw = ReadRawBody();

                if (!IsConfigured(config))
                {
                    Log(logger, "Callback-CertificateStatus-Rejected", raw, "not-configured");
                    return Content("OK");
                }

                string error;
                if (!TilakaSignature.VerifyCallback(config.ChannelId, config.ChannelSecret,
                        Request.Headers["x-request-timestamp"], raw, Request.Headers["x-validation-token"],
                        config.CallbackToleranceMinutes, out error))
                {
                    Log(logger, "Callback-CertificateStatus-Rejected", raw, error);
                    return Content("OK");
                }

                var payload = JsonConvert.DeserializeObject<CallbackCertificateStatusRequest>(raw);
                var processor = new TilakaCallbackProcessor(new TilakaRegistrationRepository());
                var outcome = processor.ProcessCertificateStatus(payload, DateTime.Now);
                Log(logger, "Callback-CertificateStatus", raw, outcome.Applied ? "applied" : outcome.Reason);
            }
            catch (Exception ex)
            {
                Log(logger, "Callback-CertificateStatus-Error", raw, ex.Message);
            }

            return Content("OK");
        }

        [HttpPost]
        public ActionResult Signing()
        {
            var config = new TilakaConfig();
            var logger = new WebServiceApiLogger(config);
            string raw = null;

            try
            {
                raw = ReadRawBody();

                if (!IsConfigured(config))
                {
                    Log(logger, "Callback-Signing-Rejected", raw, "not-configured");
                    return Content("OK");
                }

                var token = Request.Headers["x-validation-token"];
                if (!string.IsNullOrWhiteSpace(token))
                {
                    string error;
                    if (!TilakaSignature.VerifyCallback(config.ChannelId, config.ChannelSecret,
                            Request.Headers["x-request-timestamp"], raw, token,
                            config.CallbackToleranceMinutes, out error))
                    {
                        Log(logger, "Callback-Signing-Rejected", raw, error);
                        return Content("OK");
                    }
                }
                else if (!config.QuickSignAllowUnsignedCallback)
                {
                    Log(logger, "Callback-Signing-Rejected", raw, "callback-unsigned");
                    return Content("OK");
                }

                ITilakaSignedFileDownloader downloader = null;
                if (!string.IsNullOrWhiteSpace(config.QuickSignSignedFolder))
                {
                    downloader = new TilakaSignedFileDownloader(
                        config.QuickSignSignedFolder, TilakaQuickSignServiceFactory.Create(config));
                }

                var payload = JsonConvert.DeserializeObject<CallbackSigningRequest>(raw);
                var processor = new TilakaQuickSignCallbackProcessor(
                    new TilakaQuickSignRepository(), downloader, new TilakaCasemixFileRepository());
                var outcome = processor.Process(payload);
                Log(logger, "Callback-Signing", raw, outcome.Applied ? "applied" : outcome.Reason);
            }
            catch (Exception ex)
            {
                Log(logger, "Callback-Signing-Error", raw, ex.Message);
            }

            return Content("OK");
        }

        private static bool IsConfigured(TilakaConfig config)
        {
            return config != null
                   && !string.IsNullOrWhiteSpace(config.ChannelId)
                   && !string.IsNullOrWhiteSpace(config.ChannelSecret);
        }

        private string ReadRawBody()
        {
            Request.InputStream.Position = 0;
            using (var reader = new StreamReader(Request.InputStream, Request.ContentEncoding ?? Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        private static void Log(WebServiceApiLogger logger, string operation, string request, string response)
        {
            logger.Log(operation, request, response, 0);
        }
    }
}
