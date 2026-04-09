using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Bridging.SatuSehat.BusinessObject
{
    public class BaseUtil
    {
        //private readonly string _clientID = ConfigurationManager.AppSettings["SatuSehatClientID"];
        //private readonly string _secretKey = ConfigurationManager.AppSettings["SatuSehatClientSecretKey"];
        //private readonly string _baseUrl = ConfigurationManager.AppSettings["SatuSehatBaseUrl"];
        //private readonly string _consentUrl = ConfigurationManager.AppSettings["SatuSehatConsentUrl"];
        //private readonly string _authUrl = ConfigurationManager.AppSettings["SatuSehatAuthUrl"];
        //private readonly string _organizationID = ConfigurationManager.AppSettings["SatuSehatOrganizationID"];

        // Pindah ke AppParameter
        protected readonly string ClientID = SatuSehatKey("SatuSehatClientID");
        private readonly string _secretKey = SatuSehatKey("SatuSehatClientSecretKey");
        protected readonly string BaseUrl = SatuSehatKey("SatuSehatBaseUrl");
        protected readonly string ConsentUrl = SatuSehatKey("SatuSehatConsentUrl");
        private readonly string _authUrl = SatuSehatKey("SatuSehatAuthUrl");
        protected readonly string OrganizationID = SatuSehatKey("SatuSehatOrganizationID");

        protected string SatuSehatBridgingType = AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID); //"BridgingType-008";

        private string _encounterID;
        protected const string DateFormatLong = "yyyy-MM-ddTHH:mm:ss";
        protected const string DateFormatSort = "yyyy-MM-dd";
        protected string[] DayNames = { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };
        protected int GmtDif = 0 - AppParameter.GetParameterValue(AppParameter.ParameterItem.GMT).ToInt();

        // diganti public karena akan di-invoke dari DynamicInvoker
        public static string SatuSehatKey(string key)
        {
            var configKey = string.Empty;

            var entity = new AppParameter();
            if (entity.LoadByPrimaryKey(key))
            {
                configKey = entity.ParameterValue;
            }
            else
            {
                configKey = ConfigurationManager.AppSettings[key];

                if (!HttpContext.Current.IsDebuggingEnabled) // anggap sudah mode di client
                {
                    entity = new AppParameter
                    {
                        ParameterID = key,
                        ParameterName = key,
                        ParameterValue = configKey,
                        ParameterType = string.Empty,
                        IsUsedBySystem = true

                    };
                    entity.Save();
                }
            }
            return configKey;
        }

        #region Common Method
        private static Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.TokenResponse _tokenRespose = null;
        private static DateTime _tokenExpireDate = DateTime.MinValue;
        private Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.TokenResponse GetToken()
        {
            if (_tokenRespose != null && _tokenExpireDate > DateTime.Now.AddMinutes(10)) // Token anggap expired jika 10 menit lagi
                return _tokenRespose;

            _tokenRespose = null;
            var url = string.Format("{0}/accesstoken?grant_type=client_credentials", _authUrl);
            var client = new RestClient(url);
            //client.Timeout = -1;
            var request = new RestSharp.RestRequest();
            request.Method = Method.Post;

            var timeOutInSecond = AppParameter.GetParameterValue(AppParameter.ParameterItem.PCareTimeOutInSecond);
            //var timeOut = Convert.ToInt16(timeOutPar) * 1000;
            request.Timeout = TimeSpan.FromSeconds(Convert.ToInt16(timeOutInSecond));
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", ClientID);
            request.AddParameter("client_secret", _secretKey);
            var response = client.Execute(request);
            try
            {
                if (response.Content.IsValidJson())
                {
                    var tokenResponse =
                        JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.TokenResponse>(
                            response.Content);

                    _tokenRespose = tokenResponse;
                    _tokenExpireDate = DateTime.Now.AddSeconds(tokenResponse.ExpiresIn.ToInt());
                }
                else
                    throw new Exception("Token not valid json format");
            }
            catch (Exception ex)
            {
                throw new Exception(string.Concat(ex.Message, Environment.NewLine, response.Content), ex);
            }

            return _tokenRespose;
        }
        public RestResponse RestClientPost(string requestBody, string resourceType, ref string accessToken)
        {
            var url = string.IsNullOrEmpty(resourceType) ? BaseUrl : string.Concat(BaseUrl, "/", resourceType);
            return RestClientExecute(requestBody, url, ref accessToken, Method.Post);
        }
        public RestResponse RestClientPatch(string requestBody, string resourceType, string id, ref string accessToken)
        {
            var url = string.IsNullOrEmpty(resourceType) ? BaseUrl : string.Concat(BaseUrl, "/", resourceType, "/", id);
            return RestClientExecute(requestBody, url, ref accessToken, Method.Patch);
        }
        public BaseResponse RestClientPatchAndSaveLog(string requestBody, string resourceType, string episodeOfCareId, SatuSehatResult ssResult, ref string accessToken)
        {
            BaseResponse conditionResponse = null;

            var response = RestClientPatch(requestBody, resourceType, episodeOfCareId, ref accessToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                conditionResponse = JsonConvert.DeserializeObject<BaseResponse>(response.Content);
                if (!string.IsNullOrEmpty(conditionResponse.Id))
                {
                    ssResult.ResultID = new Guid(conditionResponse.Id);
                    ssResult.ErrorResponse = string.Empty;
                }
            }
            else
            {
                ssResult.ErrorResponse = response.Content;
            }

            ssResult.ResourceType = resourceType + "/Patch";
            ssResult.PostData = requestBody;

            SetResultIndexNo(ssResult);

            ssResult.Save();

            return conditionResponse;
        }
        public RestResponse RestClientExecute(string requestBody, string url, ref string accessToken, RestSharp.Method method)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            request.Method = method;

            //if (string.IsNullOrWhiteSpace(accessToken) && HttpContext.Current.Cache["ssAccessToken"] != null)
            //    accessToken = HttpContext.Current.Cache["ssAccessToken"].ToString();

            //if (string.IsNullOrWhiteSpace(accessToken))
            //{
            //    var tokenResponse = GetToken();
            //    if (tokenResponse != null)
            //    {
            //        accessToken = tokenResponse.AccessToken;
            //        HttpContext.Current.Cache.Insert("ssAccessToken", accessToken, null,
            //            DateTime.Now.AddSeconds(tokenResponse.ExpiresIn.ToInt()), TimeSpan.Zero);
            //    }
            //}

            if (string.IsNullOrWhiteSpace(accessToken))
                accessToken = GetToken().AccessToken;

            request.AddHeader("Authorization", String.Format("Bearer {0}", accessToken));

            var timeOutInSecond = AppParameter.GetParameterValue(AppParameter.ParameterItem.PCareTimeOutInSecond);
            //var timeOut = Convert.ToInt16(timeOutPar) * 1000;
            request.Timeout = TimeSpan.FromSeconds(Convert.ToInt16(timeOutInSecond));

            if (!string.IsNullOrWhiteSpace(requestBody))
            {
                if (method == Method.Patch)
                {
                    request.AddHeader("Content-Type", "application/json-patch+json");
                    request.AddParameter("application/json-patch+json", requestBody, ParameterType.RequestBody);
                }
                else
                {
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
                }
            }

            return client.Execute(request);
        }

        public RestResponse RestClientPut(string requestBody, string resourceType, ref string accessToken)
        {
            var baseUrl = string.IsNullOrEmpty(resourceType) ? BaseUrl : string.Concat(BaseUrl, "/", resourceType);
            var client = new RestClient(baseUrl);
            var request = new RestRequest();
            request.Method = Method.Put;

            if (string.IsNullOrWhiteSpace(accessToken) && HttpContext.Current.Cache["ssAccessToken"] != null)
                accessToken = HttpContext.Current.Cache["ssAccessToken"].ToString();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var tokenResponse = GetToken();
                if (tokenResponse != null)
                {
                    accessToken = tokenResponse.AccessToken;
                    HttpContext.Current.Cache.Insert("ssAccessToken", accessToken, null,
                        DateTime.Now.AddSeconds(tokenResponse.ExpiresIn.ToInt()), TimeSpan.Zero);
                }
            }

            request.AddHeader("Authorization", String.Format("Bearer {0}", accessToken));
            request.AddHeader("Content-Type", "application/json");

            var timeOutInSecond = AppParameter.GetParameterValue(AppParameter.ParameterItem.PCareTimeOutInSecond);
            //var timeOut = Convert.ToInt16(timeOutPar) * 1000;
            request.Timeout = TimeSpan.FromSeconds(Convert.ToInt16(timeOutInSecond));

            request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            return client.Execute(request);
        }
        public RestResponse RestClientGet(string url, ref string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken) && HttpContext.Current.Cache["ssAccessToken"] != null)
                accessToken = HttpContext.Current.Cache["ssAccessToken"].ToString();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var tokenResponse = GetToken();
                if (tokenResponse != null)
                {
                    accessToken = tokenResponse.AccessToken;
                    HttpContext.Current.Cache.Insert("ssAccessToken", accessToken, null,
                        DateTime.Now.AddSeconds(tokenResponse.ExpiresIn.ToInt()), TimeSpan.Zero);
                }
            }
            var client = new RestClient(url);

            var request = new RestRequest();
            request.Method = Method.Get;
            request.AddHeader("Authorization", String.Format("Bearer {0}", accessToken));

            //var body = @"";
            //request.AddParameter("text/plain", body, ParameterType.RequestBody);
            var response = client.Execute(request);
            return response;

            return RestClientExecute(string.Empty, url, ref accessToken, RestSharp.Method.Get);
        }

        public RestResponse RestClientGet(string resourceType, string id, ref string accessToken)
        {
            var url = string.Empty;
            if (string.IsNullOrEmpty(id))
                url = string.Concat(BaseUrl, "/", resourceType);
            else
                url = string.Concat(BaseUrl, "/", resourceType, "/", id);

            return RestClientGet(url, ref accessToken);
        }

        protected BaseResponse RestClientPostAndSaveLog(string resourceType, string requestBody, SatuSehatResult ssResult, ref string accessToken)
        {
            BaseResponse conditionResponse = null;

            var response = RestClientPost(requestBody, resourceType, ref accessToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                conditionResponse = JsonConvert.DeserializeObject<BaseResponse>(response.Content);
                if (!string.IsNullOrEmpty(conditionResponse.Id))
                {
                    ssResult.ResultID = new Guid(conditionResponse.Id);
                    ssResult.ErrorResponse = string.Empty;
                }
            }
            else
            {
                ssResult.ErrorResponse = response.Content;
            }

            ssResult.ResourceType = resourceType;
            ssResult.PostData = requestBody;

            SetResultIndexNo(ssResult);

            ssResult.Save();

            return conditionResponse;
        }
        protected void SetResultIndexNo(SatuSehatResult ssResult)
        {
            if (ssResult.IndexNo == null || ssResult.IndexNo == 0)
            {
                var srQr = new SatuSehatResultQuery("sr");
                srQr.Where(srQr.EncounterID == ssResult.EncounterID);
                srQr.es.Top = 1;
                srQr.Select(srQr.IndexNo);
                srQr.OrderBy(srQr.IndexNo.Descending);
                var dtb = srQr.LoadDataTable();
                if (dtb.Rows.Count > 0)
                    ssResult.IndexNo = dtb.Rows[0][0].ToInt() + 1;
                else
                    ssResult.IndexNo = 1;
            }
        }

        protected SatuSehatResult LoadSatuSehatResult(string encounterId, string resourceType, string category, string code)
        {
            var ssResult = new SatuSehatResult();
            ssResult.Query.Where(ssResult.Query.EncounterID == new Guid(encounterId), ssResult.Query.ResourceType == resourceType, ssResult.Query.Category == category, ssResult.Query.Code == code);
            if (ssResult.Query.Load()) return ssResult;
            return null;
        }

        protected ParamedicBridging LoadPerformerByUserID(string userID)
        {
            return LoadPerformer(userID, "");
        }
        protected ParamedicBridging LoadPerformer(string userID = "", string defParamedicID = "")
        {
            string paramedicID = string.Empty;
            if (!string.IsNullOrEmpty(userID))
            {
                var user = new AppUser();
                if (user.LoadByPrimaryKey(userID) && !string.IsNullOrWhiteSpace(user.ParamedicID))
                {
                    var par = new Paramedic();
                    if (par.LoadByPrimaryKey(user.ParamedicID))
                        paramedicID = par.ParamedicID;
                }
            }

            // Override
            if (string.IsNullOrEmpty(paramedicID) && !string.IsNullOrEmpty(defParamedicID))
                paramedicID = defParamedicID;

            var parMedSs = new ParamedicBridging();
            parMedSs.Query.Where(parMedSs.Query.ParamedicID == paramedicID, parMedSs.Query.SRBridgingType == SatuSehatBridgingType);
            parMedSs.Query.es.Top = 1;
            if (parMedSs.Query.Load())
                return parMedSs;

            return null;
        }
        protected ParamedicBridging LoadPerformerByParamedicID(string paramedicID)
        {
            return LoadPerformer("", paramedicID);
        }

        protected ItemBridging LoadItem(string itemID)
        {
            var ssItem = new ItemBridging();
            ssItem.Query.Where(ssItem.Query.ItemID == itemID, ssItem.Query.SRBridgingType == SatuSehatBridgingType);
            ssItem.Query.es.Top = 1;
            if (ssItem.Query.Load())
                return ssItem;
            return null;
        }

        protected PatientAssessment FirstPatientAssessment(string regNo)
        {
            var patAssess = new PatientAssessment();
            patAssess.Query.Where(patAssess.Query.RegistrationNo == regNo, patAssess.Query.Or(patAssess.Query.IsDeleted.IsNull(), patAssess.Query.IsDeleted == false));
            patAssess.Query.OrderBy(patAssess.Query.AssessmentDateTime.Ascending);
            patAssess.Query.es.Top = 1;

            if (patAssess.Query.Load())
                return patAssess;

            return null;
        }

        protected AppStandardReferenceItemBridging LoadAppStandardReferenceItemBridging(AppStandardReferenceItemBridging.SatusehatRef satusehatRef, string itemID)
        {
            return AppStandardReferenceItemBridging.Load(satusehatRef, itemID, SatuSehatBridgingType);
        }

        protected ServiceUnitBridging LoadLocation(string serviceUnitID)
        {
            var locSsQr = new ServiceUnitBridgingQuery("pb");
            locSsQr.Where(locSsQr.ServiceUnitID == serviceUnitID, locSsQr.SRBridgingType == SatuSehatBridgingType);
            locSsQr.es.Top = 1;
            var locSs = new ServiceUnitBridging();
            if (locSs.Load((locSsQr)))
            {
                return locSs;
            }
            return null;
        }
        protected PatientBridging LoadPatientBridging(string patientID)
        {
            var patSs = new PatientBridging();
            if (patSs.LoadByPrimaryKey(patientID, SatuSehatBridgingType))
            {
                return patSs;
            }

            return new PatientBridging();
        }

        protected string FormatDateLong(DateTime date)
        {
            return string.Format("{0}+00:00", date.AddHours(GmtDif).ToString(DateFormatLong));
        }
        protected string FormatDateSort(DateTime date)
        {
            return string.Format("{0}", date.AddHours(GmtDif).ToString(DateFormatSort));
        }
        #endregion Common Method
    }
}
