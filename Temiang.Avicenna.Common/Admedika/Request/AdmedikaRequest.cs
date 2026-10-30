using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class AdmedikaRequest<T>
    {
        [JsonProperty("tokenAuth")]
        public string TokenAuth { get; set; }

        [JsonProperty("serviceID")]
        public string ServiceID { get; set; }

        [JsonProperty("customerID")]
        public string CustomerID { get; set; }

        [JsonProperty("requestID")]
        public string RequestID { get; set; }

        [JsonProperty("txnData")]
        public T TxnData { get; set; }

        [JsonProperty("txnRequestDateTime")]
        public string TxnRequestDateTime { get; set; }
    }
}
