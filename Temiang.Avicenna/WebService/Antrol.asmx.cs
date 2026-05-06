using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.WebService
{
    /// <summary>
    /// Summary description for Antrol
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Antrol : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        public string CancelOutstandingAppointment()
        {
            var appt = new AppointmentCollection();
            appt.Query.Where(
                appt.Query.AppointmentDate.Date() == DateTime.Now.Date.AddDays(-1).Date,
                appt.Query.SRAppointmentStatus.In(AppSession.Parameter.AppointmentStatusOpen, AppSession.Parameter.AppointmentStatusConfirmed)
                );
            appt.Query.Load();

            foreach (var data in appt)
            {
                var svc = new Common.BPJS.Antrian.Service();
                var param = new Common.BPJS.Antrian.Update.BatalAntrian.Request.Root()
                {
                    Kodebooking = data.AppointmentNo,
                    Keterangan = "tidak hadir"
                };
                var respose = svc.BatalAntrian(param);
                var log = new WebServiceAPILog
                {
                    DateRequest = DateTime.Now,
                    IPAddress = "10.200.200.188",
                    UrlAddress = "BatalAntrean",
                    Params = JsonConvert.SerializeObject(param),
                    Response = JsonConvert.SerializeObject(respose),
                    Totalms = 0
                };
                log.Save();
                if (!respose.Metadata.IsAntrolValid) continue;
                var antrian = new Appointment();
                antrian.LoadByPrimaryKey(data.AppointmentNo);
                antrian.SRAppointmentStatus = AppSession.Parameter.AppointmentStatusCancel;
                antrian.LastUpdateByUserID = "WEBSERVICE";
                antrian.LastUpdateDateTime = DateTime.Now;
                antrian.Save();
            }

            return "ok";
        }

        [WebMethod]
        public string Decrypt(string param)
        {
            //param = @"eyJub21vckthcnR1IjoiMDAwMTUyMDkwMTIzNSIsIm5hbWEiOiJCSURVTkkgRUxJWkEgU1lFQkFUIE1VUllBVEkgUFVUUkkiLCJhbGFtYXQiOiJKTC4gTUVMVVIgSUkgTk8uIDgxIDEyLzE1IFJBTkNBRUtFSyBLRU5DQU5BLCBSQU5DQUVLRUssIEtBQlVQQVRFTiBCQU5EVU5HIiwidGdsTGhyIjoiMDYtMTEtMTk5MiIsIm5payI6IjMyMDQyODQ2MTE5MjAwMDIiLCJma3RwIjoiS2xpbmlrIFByYXRhbWEgWXVzdWYgSSIsImZrdHBHaWdpIjoiLSIsInN0YXR1c1Blc2VydGEiOiIwIn0=";

            param = @"eyJub2thcHN0IjoiMDAwMTUxOTIzMjIxOCIsImtvZGVCb29raW5nIjoiQVBULTI2MDQxNy0wNTI2Iiwibm9SdWp1a2FuIjoiMTAwMVIwMDcwNDI2SzAwNjcyNCIsIm5vcm0iOiIwMDA5ODA3MSIsImtldEt1bmp1bmdhbiI6IktvbnRyb2wiLCJuYW1hRmFza2VzQXNhbFJ1anVrIjpudWxsLCJuYW1hUG9saSI6IkdJTkpBTC1ISVBFUlRFTlNJICIsIm5hbWFEb2t0ZXIiOiJEUi4gQUdVTkcgTlVHUk9ITywgU1AuUEQiLCJub21vckFudHJlYW4iOiJUQ1FCIC0gNiJ9";

            var str = Encoding.UTF8.GetString(Convert.FromBase64String(param));
            return str;
        }
    }
}
