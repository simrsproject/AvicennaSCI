using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Admedika
{
    public class EdocsUploadTxnData
    {
        [JsonProperty("edocsUploadRequest")]
        public EdocsUploadRequest Request { get; set; }
    }

    public class EdocsUploadRequest
    {
        [JsonProperty("edocsUpload")]
        public EdocsUploadPayload EdocsUpload { get; set; }
    }

    public class EdocsUploadPayload
    {
        [JsonProperty("cardNo")] public string CardNo { get; set; }
        [JsonProperty("clID")] public string ClID { get; set; }
        [JsonProperty("docType")] public string DocType { get; set; }
        [JsonProperty("remarks")] public string Remarks { get; set; }
        [JsonProperty("patientName")] public string PatientName { get; set; }
        [JsonProperty("userBy")] public string UserBy { get; set; }
    }
}
