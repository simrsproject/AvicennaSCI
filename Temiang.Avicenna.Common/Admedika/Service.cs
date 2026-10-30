using System;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class Service
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        private readonly AdmedikaConfig _config;
        private readonly IAdmedikaTransport _transport;
        private readonly IAdmedikaLogger _logger;

        public Service()
        {
            _config = new AdmedikaConfig();
            _transport = new HttpAdmedikaTransport();
            _logger = new WebServiceApiLogger(_config);
        }

        public Service(AdmedikaConfig config, IAdmedikaTransport transport, IAdmedikaLogger logger)
        {
            _config = config ?? new AdmedikaConfig();
            _transport = transport ?? new HttpAdmedikaTransport();
            _logger = logger ?? new NullAdmedikaLogger();
        }

        public AdmedikaConfig Config
        {
            get { return _config; }
        }

        public HelloWorldResponse HelloWorld()
        {
            return Send<HelloWorldResponse>(
                AdmedikaConstant.ServiceID.HelloWorld,
                new HelloWorldTxnData
                {
                    Request = new HelloWorldRequest { CustomerID = _config.CustomerID }
                });
        }

        public CheckIcdExclusionResponse CheckIcdExclusion(string cardNo, string covID, string diagnosisCodeList)
        {
            return Send<CheckIcdExclusionResponse>(
                AdmedikaConstant.ServiceID.CheckIcdExclusion,
                new CheckIcdExclusionTxnData
                {
                    Request = new CheckIcdExclusionRequest
                    {
                        CheckIcdExclusion = new CheckIcdExclusionPayload
                        {
                            CardNo = cardNo,
                            CovID = covID,
                            DiagnosisCodeList = diagnosisCodeList
                        }
                    }
                });
        }

        private bool RequireTerminalId(AdmedikaResponse response)
        {
            if (!string.IsNullOrWhiteSpace(_config.TerminalID)) return true;

            response.TechnicalError = "AdMedika terminalID belum dikonfigurasi (AdmedikaTerminalID).";
            response.StatusMsg = response.TechnicalError;
            return false;
        }

        public EligibilityResponse Eligibility(string cardNo, string covID, string diagnosisCodeList,
            string providerTransID, string nationalID, string familyCardID, string physicianName,
            string accidentFlag, string surgicalFlag, string roomType, string roomPrice, string remarks)
        {
            var response = new EligibilityResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<EligibilityResponse>(
                AdmedikaConstant.ServiceID.Eligibility,
                new EligibilityTxnData
                {
                    Request = new EligibilityRequest
                    {
                        Eligibility = new EligibilityPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo,
                            CovID = covID,
                            DiagnosisCodeList = diagnosisCodeList,
                            ProviderTransID = providerTransID,
                            NationalID = nationalID,
                            FamilyCardID = familyCardID,
                            PhysicianName = physicianName,
                            AccidentFlag = accidentFlag,
                            SurgicalFlag = surgicalFlag,
                            RoomType = roomType,
                            RoomPrice = roomPrice,
                            Remarks = remarks
                        }
                    }
                });
        }

        public GetEntitlementResponse GetEntitlement(string cardNo)
        {
            var response = new GetEntitlementResponse();
            if (!RequireTerminalId(response)) return response;

            return Send<GetEntitlementResponse>(
                AdmedikaConstant.ServiceID.GetEntitlement,
                new GetEntitlementTxnData
                {
                    Request = new GetEntitlementRequest
                    {
                        GetEntitlement = new GetEntitlementPayload
                        {
                            TerminalID = _config.TerminalID,
                            CardNo = cardNo
                        }
                    }
                });
        }

        private TResponse Send<TResponse>(string serviceId, object txnData)
            where TResponse : AdmedikaResponse, new()
        {
            var utcNow = DateTime.UtcNow;
            var requestId = RequestIdGenerator.Next();

            var request = new AdmedikaRequest<object>
            {
                TokenAuth = TokenGenerator.CreateToken(
                    _config.CustomerID, _config.SecurityWord, utcNow, requestId, serviceId),
                ServiceID = serviceId,
                CustomerID = _config.CustomerID,
                RequestID = requestId,
                TxnData = txnData,
                TxnRequestDateTime = utcNow.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            };

            var body = JsonConvert.SerializeObject(new { input = request }, SerializerSettings);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var raw = _transport.Post(_config.ServiceUrl, body, _config.TimeoutSeconds);
                stopwatch.Stop();

                var response = ResponseParser.Parse<TResponse>(raw);
                WriteLog(serviceId, body, raw, stopwatch.ElapsedMilliseconds);
                return response;
            }
            catch (AdmedikaTransportException ex)
            {
                stopwatch.Stop();

                var failed = string.IsNullOrWhiteSpace(ex.ResponseBody)
                    ? new TResponse()
                    : ResponseParser.Parse<TResponse>(ex.ResponseBody);

                if (string.IsNullOrEmpty(failed.TechnicalError)) failed.TechnicalError = ex.Message;
                if (string.IsNullOrEmpty(failed.StatusMsg)) failed.StatusMsg = ex.Message;

                WriteLog(serviceId, body, ex.ResponseBody, stopwatch.ElapsedMilliseconds);
                return failed;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                var failed = new TResponse
                {
                    TechnicalError = ex.Message,
                    StatusMsg = ex.Message
                };

                WriteLog(serviceId, body, null, stopwatch.ElapsedMilliseconds);
                return failed;
            }
        }

        private void WriteLog(string serviceId, string requestBody, string responseBody, long elapsedMs)
        {
            if (_logger == null) return;

            try
            {
                _logger.Log(serviceId, requestBody, responseBody, elapsedMs);
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }
    }
}
