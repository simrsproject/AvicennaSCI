using System;
using System.Text.RegularExpressions;
using System.Web;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Common.Admedika
{
    public class WebServiceApiLogger : IAdmedikaLogger
    {
        private static readonly Regex CardNoPattern = new Regex(
            "(\"cardNo\"\\s*:\\s*\")([^\"]*)(\")",
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
                var log = new WebServiceAPILog
                {
                    DateRequest = DateTime.Now,
                    IPAddress = HttpContext.Current == null
                        ? string.Empty
                        : HttpContext.Current.Request.UserHostAddress,
                    UrlAddress = serviceId,
                    Params = MaskSensitive(requestBody),
                    Response = MaskSensitive(responseBody),
                    Totalms = (int)elapsedMs
                };

                log.Save();
            }
            catch (Exception)
            {
                // Logging tidak boleh menggagalkan transaksi.
            }
        }

        public static string MaskSensitive(string json)
        {
            if (string.IsNullOrEmpty(json)) return json;

            return CardNoPattern.Replace(
                json,
                match => match.Groups[1].Value
                         + AdmedikaMask.CardNo(match.Groups[2].Value)
                         + match.Groups[3].Value);
        }
    }
}
