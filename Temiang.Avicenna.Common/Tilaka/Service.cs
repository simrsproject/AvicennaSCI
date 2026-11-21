using System;
using System.Diagnostics;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        private readonly TilakaConfig _config;
        private readonly ITilakaTransport _transport;
        private readonly ITilakaLogger _logger;

        public TilakaConfig Config
        {
            get { return _config; }
        }

        public Service()
            : this(new TilakaConfig(), null, null)
        {
        }

        public Service(TilakaConfig config, ITilakaTransport transport, ITilakaLogger logger)
        {
            _config = config ?? new TilakaConfig();
            _transport = transport ?? new HttpTilakaTransport(
                _config.ClientCertificatePath, _config.ClientCertificatePassword);
            _logger = logger ?? new WebServiceApiLogger(_config);
        }

        public bool IsOk(TilakaResponse response, out string message)
        {
            message = null;

            if (response == null)
            {
                message = "Respons Tilaka kosong.";
                return false;
            }

            if (!string.IsNullOrEmpty(response.TechnicalError))
            {
                message = response.TechnicalError;
                return false;
            }

            if (!response.Success)
            {
                var failureMessage = response.Message;
                var certificate = response as CheckCertStatusResponse;
                if (string.IsNullOrEmpty(failureMessage) && certificate != null && certificate.Message != null)
                    failureMessage = certificate.Message.Info;

                message = string.IsNullOrEmpty(failureMessage)
                    ? "Permintaan Tilaka gagal."
                    : failureMessage;
                return false;
            }

            return true;
        }

        public TResponse Execute<TResponse>(string operation, string url, string jsonBody)
            where TResponse : TilakaResponse, new()
        {
            return ExecuteCore<TResponse>(operation, jsonBody,
                token => _transport.PostJson(url, jsonBody, token, _config.TimeoutSeconds));
        }

        public TResponse ExecuteGet<TResponse>(string operation, string url)
            where TResponse : TilakaResponse, new()
        {
            return ExecuteCore<TResponse>(operation, null,
                token => _transport.Get(url, token, _config.TimeoutSeconds));
        }

        public TResponse ExecuteMultipart<TResponse>(string operation, string url, string formName,
            string fileName, byte[] content, string contentType)
            where TResponse : TilakaResponse, new()
        {
            var logBody = string.Format("[multipart file={0}, bytes={1}]", fileName,
                content == null ? 0 : content.Length);
            return ExecuteCore<TResponse>(operation, logBody,
                token => _transport.PostMultipart(url, formName, fileName, content, contentType,
                    token, _config.TimeoutSeconds));
        }

        private TResponse ExecuteCore<TResponse>(string operation, string logBody,
            Func<string, TilakaTransportResult> send)
            where TResponse : TilakaResponse, new()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var token = GetAccessToken();
                TilakaTransportResult result;
                try
                {
                    result = send(token);
                }
                catch (TilakaTransportException ex)
                {
                    if (ex.StatusCode != 401) throw;

                    // Transport melempar untuk non-2xx: segarkan token lalu coba tepat satu kali lagi.
                    InvalidateToken();
                    token = GetAccessToken(true);
                    result = send(token);
                }

                if (result.StatusCode == 401)
                {
                    InvalidateToken();
                    token = GetAccessToken(true);
                    result = send(token);
                }

                stopwatch.Stop();
                var response = TilakaResponseParser.Parse<TResponse>(result.Body, result.StatusCode);
                WriteLog(operation, logBody, result.Body, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (TilakaTransportException ex)
            {
                stopwatch.Stop();
                var failed = string.IsNullOrWhiteSpace(ex.ResponseBody)
                    ? new TResponse()
                    : TilakaResponseParser.Parse<TResponse>(ex.ResponseBody, ex.StatusCode);

                if (string.IsNullOrEmpty(failed.TechnicalError)) failed.TechnicalError = ex.Message;
                WriteLog(operation, logBody, ex.ResponseBody, stopwatch.ElapsedMilliseconds);
                return failed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                WriteLog(operation, logBody, null, stopwatch.ElapsedMilliseconds);
                return new TResponse { TechnicalError = ex.Message };
            }
        }

        private void WriteLog(string operation, string requestBody, string responseBody, long elapsedMs)
        {
            try
            {
                _logger.Log(operation, requestBody, responseBody, elapsedMs);
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }
    }
}
