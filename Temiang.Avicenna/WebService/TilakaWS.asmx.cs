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
                    var changed = false;
                    var failedThisRecord = false;
                    var certificateRejected = false;

                    if (!string.IsNullOrWhiteSpace(record.RegistrationRequestID))
                    {
                        var registration = service.GetUserRegistrationStatus(record.RegistrationRequestID);
                        string message;
                        if (!service.IsOk(registration, out message))
                        {
                            record.LastErrorMessage = message;
                            try
                            {
                                StampAndSave(repository, record);
                            }
                            catch
                            {
                                // Kegagalan menyimpan tidak boleh menghentikan proses batch.
                            }
                            failed++;
                            messages.Add(Truncate(record.RelatedID + ": " + message, 1000));
                            continue;
                        }

                        if (ApplyUserRegistrationStatus(record, registration.Data)) changed = true;
                    }

                    if (!string.IsNullOrWhiteSpace(record.TilakaName))
                    {
                        var certificate = service.CheckCertificateStatus(record.TilakaName);
                        string message;
                        if (!service.IsOk(certificate, out message))
                        {
                            record.LastErrorMessage = message;
                            failedThisRecord = true;
                            failed++;
                            messages.Add(Truncate(record.RelatedID + ": " + message, 1000));
                        }
                        else if (TilakaCertificateStatusRules.CanApply(record.CertificateStatus, certificate.Status))
                        {
                            if (record.CertificateStatus != certificate.Status) changed = true;
                            record.ApplyCertificateStatus(certificate);
                        }
                        else
                        {
                            // Status mundur/tidak valid tidak menimpa status yang sudah lebih maju.
                            certificateRejected = true;
                            failedThisRecord = true;
                            failed++;
                            messages.Add(Truncate(record.RelatedID +
                                ": status sertifikat diabaikan karena tidak lebih maju.", 1000));
                        }
                    }

                    if (certificateRejected)
                        record.LastErrorMessage = "Status sertifikat dari Tilaka diabaikan karena tidak lebih maju.";
                    else if (!failedThisRecord)
                        record.LastErrorMessage = null;

                    var previousState = record.RegistrationState;
                    record.LastCheckedDateTime = DateTime.Now;
                    record.LastCheckSource = TilakaConstant.LogSource.Reconcile;
                    record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
                    if (!string.Equals(previousState, record.RegistrationState, StringComparison.Ordinal))
                        changed = true;

                    repository.Save(record);
                    if (changed) changedCount++;
                }
                catch (Exception ex)
                {
                    record.LastErrorMessage = Truncate(ex.Message, 1000);
                    try
                    {
                        StampAndSave(repository, record);
                    }
                    catch
                    {
                        // Kegagalan menyimpan tidak boleh menghentikan proses batch.
                    }
                    failed++;
                    messages.Add(Truncate(record.RelatedID + ": " + ex.Message, 1000));
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

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ReconcileSigning(string accessKey, int max)
        {
            ValidateAccessKey(accessKey);

            if (max <= 0 || max > 50) max = 10;

            var config = new TilakaConfig();
            var messages = new List<string>();
            var checkedCount = 0;
            var changedCount = 0;
            var failed = 0;

            if (!config.IsQuickSignConfigured)
            {
                messages.Add("Quick Sign belum dikonfigurasi (TilakaLiteBaseUrl kosong).");
                return JsonConvert.SerializeObject(new { @checked = 0, changed = 0, failed = 1, messages });
            }

            var service = TilakaQuickSignServiceFactory.Create(config);
            var repository = new TilakaQuickSignRepository();

            ITilakaSignedFileDownloader downloader = null;
            if (!string.IsNullOrWhiteSpace(config.QuickSignSignedFolder))
                downloader = new TilakaSignedFileDownloader(config.QuickSignSignedFolder, service);

            foreach (var record in repository.FindPending(max))
            {
                checkedCount++;

                try
                {
                    var status = service.CheckSignStatus(record.RequestID);
                    string message;
                    if (!service.IsOk(status, out message))
                    {
                        failed++;
                        messages.Add(Truncate(record.RequestID + ": " + message, 1000));
                        continue;
                    }

                    var signers = repository.LoadSigners(record.QuickSignRequestID);
                    var changed = ApplySigningStatus(record, signers, status);

                    var state = TilakaQuickSignState.Evaluate(record, signers);
                    if (state == TilakaQuickSignConstant.RequestStatus.Completed)
                    {
                        if (downloader != null && string.IsNullOrWhiteSpace(record.SignedFilePath))
                        {
                            var url = FirstSignedUrl(signers);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                try
                                {
                                    record.SignedFilePath = downloader.Download(record.RequestID,
                                        record.FileName, url);
                                }
                                catch (Exception ex)
                                {
                                    record.ErrorMessage = Truncate(
                                        "Gagal mengunduh dokumen bertanda tangan: " + ex.Message, 1000);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
                        {
                            record.Status = TilakaQuickSignConstant.RequestStatus.Completed;
                            record.IsActive = false;
                            record.CompletedDateTime = DateTime.Now;
                            changed = true;
                        }
                        else
                        {
                            record.Status = TilakaQuickSignConstant.RequestStatus.InProgress;
                            record.IsActive = true;
                        }
                    }
                    else if (state == TilakaQuickSignConstant.RequestStatus.Failed)
                    {
                        record.Status = TilakaQuickSignConstant.RequestStatus.Failed;
                        record.IsActive = false;
                        record.CompletedDateTime = DateTime.Now;
                        changed = true;
                    }
                    else
                    {
                        record.Status = TilakaQuickSignConstant.RequestStatus.InProgress;
                        record.IsActive = true;
                    }

                    record.LastUpdateByUserID = "TILAKA-RECONCILE";
                    repository.Save(record, signers);
                    if (changed) changedCount++;
                }
                catch (Exception ex)
                {
                    failed++;
                    messages.Add(Truncate(record.RequestID + ": " + ex.Message, 1000));
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

        private static bool ApplySigningStatus(TilakaQuickSignRecord record,
            IList<TilakaQuickSignSignerRecord> signers, QuickSignStatusResponse status)
        {
            var changed = false;

            if (status.Status != null)
            {
                foreach (var item in status.Status)
                {
                    foreach (var signer in signers)
                    {
                        if (signer.Sequence != item.Sequence
                            && !string.Equals(signer.UserIdentifier, item.UserIdentifier,
                                StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (!TilakaQuickSignState.CanApplySignerStatus(signer.Status, item.Status)) continue;

                        if (!string.Equals(signer.Status, item.Status, StringComparison.Ordinal)) changed = true;
                        signer.Status = item.Status;
                        signer.NumSignatures = item.NumSignatures ?? signer.NumSignatures;
                        signer.NumSignaturesDone = item.NumSignaturesDone ?? signer.NumSignaturesDone;
                        signer.LastCheckedDateTime = DateTime.Now;
                        signer.LastUpdateByUserID = "TILAKA-RECONCILE";
                    }
                }
            }

            if (status.ListPdf != null)
            {
                var url = string.Empty;
                foreach (var pdf in status.ListPdf)
                {
                    if (string.IsNullOrWhiteSpace(pdf.PresignedUrl)) continue;
                    url = pdf.PresignedUrl;
                }

                if (!string.IsNullOrWhiteSpace(url))
                {
                    foreach (var signer in signers) signer.SignedUrl = url;
                }
            }

            if (!string.IsNullOrWhiteSpace(record.ErrorMessage))
            {
                record.ErrorMessage = null;
                changed = true;
            }

            return changed;
        }

        private static string FirstSignedUrl(IList<TilakaQuickSignSignerRecord> signers)
        {
            foreach (var signer in signers)
                if (!string.IsNullOrWhiteSpace(signer.SignedUrl)) return signer.SignedUrl;
            return null;
        }

        private static bool ApplyUserRegistrationStatus(TilakaRegistrationRecord record, TilakaKycData data)
        {
            if (data == null) return false;

            var previousName = record.TilakaName;
            var previousKyc = record.KycStatus;
            var previousManual = record.ManualRegistrationStatus;
            var previousReason = record.KycReasonCode;

            record.ApplyUserRegistrationStatus(data);

            return !string.Equals(previousName, record.TilakaName, StringComparison.Ordinal)
                   || !string.Equals(previousKyc, record.KycStatus, StringComparison.Ordinal)
                   || !string.Equals(previousManual, record.ManualRegistrationStatus, StringComparison.Ordinal)
                   || previousReason != record.KycReasonCode;
        }

        private static void StampAndSave(TilakaRegistrationRepository repository, TilakaRegistrationRecord record)
        {
            record.LastErrorMessage = Truncate(record.LastErrorMessage, 1000);
            record.LastCheckedDateTime = DateTime.Now;
            record.LastCheckSource = TilakaConstant.LogSource.Reconcile;
            record.RegistrationState = TilakaRegistrationState.Evaluate(record, DateTime.Now);
            repository.Save(record);
        }

        private static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength) return value;
            return value.Substring(0, maxLength);
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
