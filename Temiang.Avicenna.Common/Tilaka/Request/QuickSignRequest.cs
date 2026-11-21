using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class QuickSignRequest
    {
        [JsonProperty("request_id")] public string RequestId { get; set; }
        [JsonProperty("signatures")] public QuickSignSignature[] Signatures { get; set; }
        [JsonProperty("list_pdf")] public QuickSignDocument[] ListPdf { get; set; }
    }

    public class QuickSignSignature
    {
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }
        [JsonProperty("email")] public string Email { get; set; }
        [JsonProperty("signature_image")] public string SignatureImage { get; set; }
        [JsonProperty("sequence")] public int Sequence { get; set; }
    }

    public class QuickSignDocument
    {
        [JsonProperty("filename")] public string FileName { get; set; }
        [JsonProperty("template_no")] public string TemplateNo { get; set; }
        [JsonProperty("signatures")] public QuickSignDocumentSignature[] Signatures { get; set; }
    }

    public class QuickSignDocumentSignature
    {
        [JsonProperty("user_identifier")] public string UserIdentifier { get; set; }

        [JsonProperty("signature_image", NullValueHandling = NullValueHandling.Ignore)]
        public string SignatureImage { get; set; }

        [JsonProperty("reason", NullValueHandling = NullValueHandling.Ignore)]
        public string Reason { get; set; }

        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        [JsonProperty("width")] public int Width { get; set; }
        [JsonProperty("height")] public int Height { get; set; }
        [JsonProperty("coordinate_x")] public int CoordinateX { get; set; }
        [JsonProperty("coordinate_y")] public int CoordinateY { get; set; }
        [JsonProperty("page_number")] public int PageNumber { get; set; }

        [JsonProperty("qr_option", NullValueHandling = NullValueHandling.Ignore)]
        public string QrOption { get; set; }
    }
}
