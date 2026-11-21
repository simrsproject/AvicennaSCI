using System;
using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        private static readonly JsonSerializerSettings QuickSignSerializer = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.None
        };

        public QuickSignUploadResponse UploadPdf(byte[] content, string fileName)
        {
            return ExecuteMultipart<QuickSignUploadResponse>(
                "QuickSignUpload",
                TilakaQuickSignUrls.Upload(_config),
                "file",
                fileName,
                content,
                "application/pdf");
        }

        public QuickSignRequestResponse RequestSign(QuickSignRequest request)
        {
            var body = JsonConvert.SerializeObject(request, QuickSignSerializer);
            return Execute<QuickSignRequestResponse>(
                "QuickSignRequestSign", TilakaQuickSignUrls.RequestSign(_config), body);
        }

        public QuickSignStatusResponse CheckSignStatus(string requestId)
        {
            var body = JsonConvert.SerializeObject(new { request_id = requestId }, QuickSignSerializer);
            return Execute<QuickSignStatusResponse>(
                "QuickSignCheckStatus", TilakaQuickSignUrls.CheckSignStatus(_config), body);
        }

        public string GetPresignedUrl(string fileName)
        {
            var response = ExecuteGet<QuickSignPresignResponse>(
                "QuickSignPresign", TilakaQuickSignUrls.GetPresignedUrl(_config, fileName));

            string message;
            if (!IsOk(response, out message))
                throw new InvalidOperationException(message);

            return response.PresignedUrl;
        }
    }
}
