using System.Data;
using Temiang.Dal.Interfaces;
using Temiang.Dal.DynamicQuery;

namespace Temiang.Avicenna.BusinessObject
{
    public partial class WebServiceAPILogCollection
    {
        public int DeletePrevMonth()
        {
            var retentionInMonths = 1;
            var parameterValue = AppParameter.GetParameterValue(AppParameter.ParameterItem.WebServiceAPILogRetentionInMonths);

            if (!int.TryParse(parameterValue, out retentionInMonths) || retentionInMonths < 1)
                retentionInMonths = 1;

            string cmd = string.Format(@"DELETE TOP (100) FROM WebServiceAPILog WHERE DateRequest < DATEADD(MONTH,-{0}, GETDATE())", retentionInMonths);
            return ExecuteNonQuery(esQueryType.Text, cmd);
        }
    }
}
