using System;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.ReportDataSource.RSMM.Billing
{
    /// <summary>
    /// Summary description for Assessment
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class BillingPatientStatement : BaseDataService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetDetailByRegistrationNo(string RegistrationNo, string UserID, string UserName)
        {
            //For RSBK
            //execute SP spxml_BillingPatientStatementByRegistration

            var pars = new esParameters();
            pars.Add("RegistrationNo", RegistrationNo);
            pars.Add("UserID", UserID);
            pars.Add("UserName", UserName);
            var tbl = BusinessObject.Common.Utils.LoadDataTableFromStoreProcedure("spxml_BillingPatientStatementByRegistration", pars, 0);

            ResponseWrite(ConvertDataTabletoObject(tbl));
        }
    }
}
