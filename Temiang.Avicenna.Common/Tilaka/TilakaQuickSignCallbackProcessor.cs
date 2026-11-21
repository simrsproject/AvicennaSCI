using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaQuickSignCallbackProcessor
    {
        private readonly ITilakaQuickSignRepository _repository;
        private readonly ITilakaSignedFileDownloader _downloader;

        public TilakaQuickSignCallbackProcessor(ITilakaQuickSignRepository repository,
            ITilakaSignedFileDownloader downloader)
        {
            _repository = repository;
            _downloader = downloader;
        }

        public TilakaCallbackOutcome Process(CallbackSigningRequest payload)
        {
            if (payload == null || string.IsNullOrWhiteSpace(payload.RequestId))
                return NotApplied("request-id-missing");

            var record = _repository.FindByRequestId(payload.RequestId);
            if (record == null) return NotApplied("request-id-unknown");

            var signers = _repository.LoadSigners(record.QuickSignRequestID);
            if (signers == null || signers.Count == 0) return NotApplied("signer-missing");

            if (record.Status == TilakaQuickSignConstant.RequestStatus.Completed)
                return new TilakaCallbackOutcome { Applied = true, Reason = "already-completed" };

            var info = payload.UserInfo;
            if (info == null) return NotApplied("user-info-missing");

            TilakaQuickSignSignerRecord target = null;
            foreach (var signer in signers)
            {
                if (signer.Sequence == info.Sequence
                    || string.Equals(signer.UserIdentifier, info.UserIdentifier, StringComparison.OrdinalIgnoreCase))
                {
                    target = signer;
                    break;
                }
            }

            if (target == null) return NotApplied("signer-unknown");

            if (TilakaQuickSignState.CanApplySignerStatus(target.Status, info.Status))
            {
                target.Status = info.Status;
                target.NumSignaturesDone = info.NumSignaturesDone ?? target.NumSignaturesDone;
                target.NumSignatures = info.NumSignatures ?? target.NumSignatures;
                target.LastCheckedDateTime = DateTime.Now;
                target.LastUpdateByUserID = "TILAKA";
            }

            ApplySignInfo(payload, target);

            var state = TilakaQuickSignState.Evaluate(record, signers);
            record.LastUpdateByUserID = "TILAKA";
            record.ErrorMessage = null;

            if (state == TilakaQuickSignConstant.RequestStatus.Completed)
            {
                TryDownload(record, signers);
                if (!string.IsNullOrWhiteSpace(record.SignedFilePath))
                {
                    record.Status = TilakaQuickSignConstant.RequestStatus.Completed;
                    record.IsActive = false;
                    record.CompletedDateTime = DateTime.Now;
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
            }
            else
            {
                record.Status = TilakaQuickSignConstant.RequestStatus.InProgress;
                record.IsActive = true;
            }

            _repository.Save(record, signers);
            return new TilakaCallbackOutcome { Applied = true };
        }

        private static void ApplySignInfo(CallbackSigningRequest payload,
            TilakaQuickSignSignerRecord target)
        {
            if (payload.SignInfo == null) return;

            foreach (var info in payload.SignInfo)
            {
                if (!string.IsNullOrWhiteSpace(info.PresignedUrl))
                    target.SignedUrl = info.PresignedUrl;
            }
        }

        private void TryDownload(TilakaQuickSignRecord record,
            System.Collections.Generic.IList<TilakaQuickSignSignerRecord> signers)
        {
            if (_downloader == null || !string.IsNullOrWhiteSpace(record.SignedFilePath)) return;

            string presignedUrl = null;
            foreach (var signer in signers)
            {
                if (!string.IsNullOrWhiteSpace(signer.SignedUrl))
                {
                    presignedUrl = signer.SignedUrl;
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(presignedUrl)) return;

            try
            {
                record.SignedFilePath = _downloader.Download(record.RequestID, record.FileName, presignedUrl);
            }
            catch (Exception ex)
            {
                record.ErrorMessage = "Gagal mengunduh dokumen bertanda tangan: " + ex.Message;
            }
        }

        private static TilakaCallbackOutcome NotApplied(string reason)
        {
            return new TilakaCallbackOutcome { Applied = false, Reason = reason };
        }
    }
}
