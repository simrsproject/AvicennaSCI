using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class AdmedikaResponse
    {
        [JsonProperty("referenceID")]
        public string ReferenceID { get; set; }

        [JsonProperty("statusCode")]
        public string StatusCode { get; set; }

        [JsonProperty("statusMsg")]
        public string StatusMsg { get; set; }

        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("txnResponseDateTime")]
        public string TxnResponseDateTime { get; set; }

        public string ErrorCode { get; set; }

        public string ErrorDesc { get; set; }

        public string TechnicalError { get; set; }

        public string RawResponse { get; set; }

        public bool IsSuccess
        {
            get
            {
                var code = (StatusCode ?? string.Empty).Trim();
                if (code.Length == 0) return false;

                foreach (var character in code)
                {
                    if (character != '0') return false;
                }

                return true;
            }
        }
    }
}
