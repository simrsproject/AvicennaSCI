using System;
using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.ReportDataSource.RSMM.Emr
{
    /// <summary>
    /// Summary description for Assessment
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class SEP : BaseDataService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetByPrescriptionNo(string p_PrescriptionNo)
        {
            try
            {
                var presQ = new TransPrescriptionQuery("a");
                var sepQ = new BpjsSEPQuery("b");
                var regQ = new RegistrationQuery("c");

                presQ.InnerJoin(regQ).On(presQ.RegistrationNo == regQ.RegistrationNo)
                    .InnerJoin(sepQ).On(regQ.BpjsSepNo == sepQ.NoSEP)
                    .Where(presQ.PrescriptionNo == p_PrescriptionNo)
                    .Select(sepQ.NoSEP, sepQ.TanggalRujukan, sepQ.TanggalSEP);
                presQ.es.Top = 1;
                var resultTable = presQ.LoadDataTable();

                if(resultTable.Rows.Count == 0)
                {
                    ResponseWrite(
                        new { 
                            NoSEP = string.Empty, 
                            TanggalRujukan = string.Empty, 
                            TannggalSEP = string.Empty,
                            TanggalBerlakuRujukan = string.Empty
                        }
                    );
                    return;
                }

                string tanggalRujukan = Convert.ToString(resultTable.Rows[0]["TanggalRujukan"]);
                DateTime tanggal = Convert.ToDateTime(tanggalRujukan);
                string tanggalPlus90Hari = tanggal.AddDays(90).ToString("yyyy-MM-dd");

                var result = new {
                    NoSEP = resultTable.Rows[0]["NoSEP"],
                    TanggalRujukan = resultTable.Rows[0]["TanggalRujukan"],
                    TanggalSEP = resultTable.Rows[0]["TanggalSEP"],
                    TanggalBerlakuRujukan = tanggalPlus90Hari
                };

                ResponseWrite(result);
            }
            catch (Exception ex)
            {
                ResponseWrite(
                    new { 
                        NoSEP = string.Empty, 
                        TanggalRujukan = string.Empty,
                        TanggalSEP = string.Empty,
                        TanggalBerlakuRujukan = string.Empty
                    }
                );
            }
        }
    }
}
