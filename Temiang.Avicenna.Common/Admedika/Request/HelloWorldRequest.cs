using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class HelloWorldTxnData
    {
        [JsonProperty("helloRequest")]
        public HelloWorldRequest Request { get; set; }
    }

    public class HelloWorldRequest
    {
        [JsonProperty("customerID")]
        public string CustomerID { get; set; }
    }
}
