using System;
using System.Configuration;
using System.Web;
using System.Web.Services;
using Temiang.Avicenna.Common.BPJS.Antrian;

namespace Temiang.Avicenna.ReportDataSource.WebService
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class AntrolTaskQueue : System.Web.Services.WebService
    {
        [WebMethod]
        public int ProcessTaskQueue()
        {
            return ProcessTaskQueueCore(Context.Request.Headers["x-antrol-job-key"]);
        }

        [WebMethod(Description = "Memproses queue Antrol dengan job key untuk pengujian melalui Invoke.")]
        public int ProcessTaskQueueWithKey(string jobKey)
        {
            return ProcessTaskQueueCore(jobKey);
        }

        private int ProcessTaskQueueCore(string jobKey)
        {
            if (!string.Equals(Context.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
                throw new HttpException(405, "Gunakan POST.");

            var expectedKey = ConfigurationManager.AppSettings["BpjsAntrolTaskQueueJobKey"];
            if (string.IsNullOrWhiteSpace(expectedKey)
                || !string.Equals(jobKey, expectedKey, StringComparison.Ordinal))
                throw new HttpException(401, "Job key tidak valid.");

            if (!AntrolTaskQueueService.QueueEnabled)
                throw new HttpException(409, "Queue Antrol belum aktif untuk RSI.");

            return AntrolTaskQueueService.Run();
        }
    }
}
