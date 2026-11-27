using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        public RequestRevokeCertificateResponse RequestRevokeCertificate(
            string tilakaName, string reason)
        {
            var body = JsonConvert.SerializeObject(new
            {
                user_identifier = tilakaName,
                reason = reason
            });

            return Execute<RequestRevokeCertificateResponse>(
                "RequestRevokeCertificate",
                TilakaUrls.RequestRevokeCertificate(_config),
                body);
        }
    }
}
