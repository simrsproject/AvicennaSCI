using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Common.RsOnline
{
    public class Service
    {
        private string _url = ConfigurationManager.AppSettings["RsOnlineServiceUrlLocation"];
        private string _cid = ConfigurationManager.AppSettings["RsOnlineConsumerID"];
        private string _key = ConfigurationManager.AppSettings["RsOnlineSaltConsumerID"];

        private HttpWebRequest PopulateWebRequest(string url, BPJS.Helper.WebRequestMethod method, BPJS.Helper.WebRequestContentType contentType, string parameter)
        {
            BPJS.Helper.IgnoreBadCertificates();

            var webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = method.ToString();

            if (method != BPJS.Helper.WebRequestMethod.GET) webrequest.ContentType = contentType.ToString();

            webrequest.Headers.Add("X-rs-id", _cid);
            string stamp = BPJS.Helper.GetUnixTimeStamp();
            webrequest.Headers.Add("X-Timestamp", stamp);
            webrequest.Headers.Add("X-pass", _key);

            if (method != BPJS.Helper.WebRequestMethod.GET)
            {
                byte[] formData = Encoding.UTF8.GetBytes(parameter.ToString());
                webrequest.ContentLength = formData.Length;

                using (var post = webrequest.GetRequestStream())
                {
                    post.Write(formData, 0, formData.Length);
                }
            }

            return webrequest;
        }

        public Json.Response.ReferensiTempatTidur.Root ReferensiTempatTidur()
        {
            _url += "fo/index.php/Referensi/tempat_tidur";

            using (var response = PopulateWebRequest(_url, BPJS.Helper.WebRequestMethod.GET, BPJS.Helper.WebRequestContentType.JSON, string.Empty).GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                return JsonConvert.DeserializeObject<Json.Response.ReferensiTempatTidur.Root>(sr.ReadToEnd());
            }
        }

        public Json.Response.DataTempatTidur.Root Get()
        {
            _url += "fo/index.php/Fasyankes";

            using (var response = PopulateWebRequest(_url, BPJS.Helper.WebRequestMethod.GET, BPJS.Helper.WebRequestContentType.JSON, string.Empty).GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                return JsonConvert.DeserializeObject<Json.Response.DataTempatTidur.Root>(sr.ReadToEnd());
            }
        }

        public string Insert(Json.Request.Insert insert)
        {
            _url += "fo/index.php/Fasyankes";

            using (var response = PopulateWebRequest(_url, BPJS.Helper.WebRequestMethod.POST, BPJS.Helper.WebRequestContentType.JSON, JsonConvert.SerializeObject(insert)).GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                return (sr.ReadToEnd());
            }
        }

        public string Update(Json.Request.Update update)
        {
            _url += "fo/index.php/Fasyankes";

            using (var response = PopulateWebRequest(_url, BPJS.Helper.WebRequestMethod.PUT, BPJS.Helper.WebRequestContentType.JSON, JsonConvert.SerializeObject(update)).GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                return (sr.ReadToEnd());
            }
        }

        public string Delete(Json.Request.Delete delete)
        {
            _url += "fo/index.php/Fasyankes";

            using (var response = PopulateWebRequest(_url, BPJS.Helper.WebRequestMethod.DELETE, BPJS.Helper.WebRequestContentType.JSON, JsonConvert.SerializeObject(delete)).GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK) throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));

                var sr = new StreamReader(response.GetResponseStream());
                return (sr.ReadToEnd());
            }
        }
    }
}
