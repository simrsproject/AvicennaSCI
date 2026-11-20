using Newtonsoft.Json;

namespace Temiang.Avicenna.Common.Tilaka
{
    public partial class Service
    {
        public CheckCertStatusResponse CheckCertificateStatus(string tilakaName)
        {
            var body = JsonConvert.SerializeObject(new { user_identifier = tilakaName });
            return Execute<CheckCertStatusResponse>(
                "CheckCertStatus", TilakaUrls.CheckCertStatus(_config), body);
        }
    }
}
