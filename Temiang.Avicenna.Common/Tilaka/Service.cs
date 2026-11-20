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
                message = string.IsNullOrEmpty(response.Message)
                    ? "Permintaan Tilaka gagal."
                    : response.Message;
                return false;
            }

            return true;
        }

        public TResponse Execute<TResponse>(string operation, string url, string jsonBody)
            where TResponse : TilakaResponse, new()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var token = GetAccessToken();
                var result = _transport.PostJson(url, jsonBody, token, _config.TimeoutSeconds);

                if (result.StatusCode == 401)
                {
                    InvalidateToken();
                    token = GetAccessToken(true);
                    result = _transport.PostJson(url, jsonBody, token, _config.TimeoutSeconds);
                }

                stopwatch.Stop();
                var response = TilakaResponseParser.Parse<TResponse>(result.Body, result.StatusCode);
                WriteLog(operation, jsonBody, result.Body, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (TilakaTransportException ex)
            {
                stopwatch.Stop();
                var failed = string.IsNullOrWhiteSpace(ex.ResponseBody)
                    ? new TResponse()
                    : TilakaResponseParser.Parse<TResponse>(ex.ResponseBody, ex.StatusCode);

                if (string.IsNullOrEmpty(failed.TechnicalError)) failed.TechnicalError = ex.Message;
                WriteLog(operation, jsonBody, ex.ResponseBody, stopwatch.ElapsedMilliseconds);
                return failed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                WriteLog(operation, jsonBody, null, stopwatch.ElapsedMilliseconds);
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
