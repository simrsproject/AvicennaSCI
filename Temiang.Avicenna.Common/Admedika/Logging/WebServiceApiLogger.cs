using System;
using System.Text.RegularExpressions;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Common.Admedika
{
    public class WebServiceApiLogger : IAdmedikaLogger
    {
        private static readonly Regex CardNoPattern = new Regex(
            "(\"cardNo\"\\s*:\\s*)(?:\"([^\"]*)\"|([^,}\\]\\s]+))",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly AdmedikaConfig _config;

        public WebServiceApiLogger(AdmedikaConfig config)
        {
            _config = config ?? new AdmedikaConfig();
        }

        public void Log(string serviceId, string requestBody, string responseBody, long elapsedMs)
        {
            if (!_config.IsLogEnabled) return;

            try
            {
                CreateLogEntry(serviceId, requestBody, responseBody, elapsedMs).Save();
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }

        public static WebServiceAPILog CreateLogEntry(string serviceId, string requestBody,
            string responseBody, long elapsedMs)
        {
            return new WebServiceAPILog
            {
                DateRequest = DateTime.Now,
                IPAddress = "ADMEDIKA",
                UrlAddress = serviceId,
                Params = MaskSensitive(requestBody),
                Response = MaskSensitive(responseBody),
                Totalms = (int)elapsedMs
            };
        }

        public static string MaskSensitive(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            return CardNoPattern.Replace(
                json,
                match =>
                {
                    var value = match.Groups[2].Success
                        ? match.Groups[2].Value
                        : match.Groups[3].Value;

                    return match.Groups[1].Value + "\"" + AdmedikaMask.CardNo(value) + "\"";
                });
        }
    }
}
