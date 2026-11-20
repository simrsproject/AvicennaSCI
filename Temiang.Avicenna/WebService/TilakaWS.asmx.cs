using System;
using System.Collections.Generic;
using System.Web.Script.Services;
using System.Web.Services;
using Newtonsoft.Json;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common.Tilaka;

namespace Temiang.Avicenna.WebService
{
    [WebService(Namespace = "http://tempuri.org/",
        Description = "Rekonsiliasi status pendaftaran Tilaka.")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class TilakaWS : System.Web.Services.WebService
    {
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReconcilePending(string accessKey, int max)
        {
            ValidateAccessKey(accessKey);

            if (max <= 0 || max > 50) max = 10;

            var config = new TilakaConfig();
            var service = new Service();
            var repository = new TilakaRegistrationRepository();
            var checkedCount = 0;
            var changedCount = 0;
            var failed = 0;
            var messages = new List<string>();

            foreach (var record in repository.FindPending(max))
            {
                checkedCount++;

                try
                {
                    if (string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                        string message;
                        if (service.IsOk(registration, out message) && registration.Data != null)
                        {
                            if (!string.IsNullOrWhiteSpace(registration.Data.TilakaName))
                                record.TilakaName = registration.Data.TilakaName;
                            if (!string.IsNullOrWhiteSpace(registration.Data.Status))
                                record.KycStatus = registration.Data.Status;
                            if (!string.IsNullOrWhiteSpace(registration.Data.ManualRegistrationStatus))
                                record.ManualRegistrationStatus = registration.Data.ManualRegistrationStatus;
                        }
                        else
                        {
                            failed++;
                            messages.Add(record.RelatedID + ": " + message);
                            continue;
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var certificate = service.CheckCertificateStatus(record.TilakaName);
                        string message;
                        if (service.IsOk(certificate, out message))
                        {
                            record.CertificateStatus = certificate.Status;
                            record.CertificateStatusInfo = certificate.Message == null ? null : certificate.Message.Info;
                            if (certificate.Data != null && certificate.Data.Length > 0)
                                record.CertificateSerialNumber = certificate.Data[0].SerialNumber;
                        }
                        else
                        {
                            failed++;
                            messages.Add(record.RelatedID + ": " + message);
                        }
                    }

                    record.LastCheckedDateTime = DateTime.Now;
                    record.LastCheckSource = TilakaConstant.LogSource.Reconcile;
                    record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                    repository.Save(record);
                    changedCount++;
                }
                catch (Exception ex)
                {
                    failed++;
                    messages.Add(record.RelatedID + ": " + ex.Message);
                }
            }

            return JsonConvert.SerializeObject(new
            {
                @checked = checkedCount,
                changed = changedCount,
                failed,
                messages
            });
        }

        private static void ValidateAccessKey(string accessKey)
        {
            if (string.IsNullOrWhiteSpace(accessKey))
                throw new Exception("AccessKey required.");

            var collection = new WebServiceAccessKeyCollection();
            var query = new WebServiceAccessKeyQuery();
            query.Where(query.AccessKey == accessKey);
            if (!collection.Load(query)) throw new Exception("AccessKey tidak valid.");

            var key = collection[0];
            var now = DateTime.Now;
            if ((key.StartDate.HasValue && key.StartDate.Value > now) ||
                (key.EndDate.HasValue && key.EndDate.Value < now))
                throw new Exception("AccessKey tidak valid.");
        }
    }
}
