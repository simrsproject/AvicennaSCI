using System;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaQuickSignRecord
    {
        public long QuickSignRequestID { get; set; }
        public string RequestID { get; set; }
        public long? CasemixFileID { get; set; }
        public long? TemplateSignFileID { get; set; }
        public string FileName { get; set; }
        public string TemplateNo { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public DateTime? RequestDateTime { get; set; }
        public DateTime? CompletedDateTime { get; set; }
        public string SignedFilePath { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedByUserID { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public string LastUpdateByUserID { get; set; }
    }

    public class TilakaQuickSignSignerRecord
    {
        public long QuickSignSignerID { get; set; }
        public long QuickSignRequestID { get; set; }
        public int Sequence { get; set; }
        public string UserIdentifier { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public int? NumSignatures { get; set; }
        public int? NumSignaturesDone { get; set; }
        public string AuthUrl { get; set; }
        public string TokenKey { get; set; }
        public string SignedUrl { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime? LastCheckedDateTime { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedByUserID { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public string LastUpdateByUserID { get; set; }
    }

    public static class TilakaQuickSignState
    {
        public static string Evaluate(TilakaQuickSignRecord request,
            System.Collections.Generic.IList<TilakaQuickSignSignerRecord> signers)
        {
            if (request == null) return TilakaQuickSignConstant.RequestStatus.Requested;

            if (request.Status == TilakaQuickSignConstant.RequestStatus.Completed
                || request.Status == TilakaQuickSignConstant.RequestStatus.Failed)
                return request.Status;

            if (signers == null || signers.Count == 0)
                return string.IsNullOrWhiteSpace(request.Status)
                    ? TilakaQuickSignConstant.RequestStatus.Requested
                    : request.Status;

            var allDone = true;
            var allFinalFailure = true;

            foreach (var signer in signers)
            {
                if (signer.Status != TilakaQuickSignConstant.SignerStatus.Done) allDone = false;
                if (signer.Status != TilakaQuickSignConstant.SignerStatus.Failed
                    && signer.Status != TilakaQuickSignConstant.SignerStatus.Denied)
                    allFinalFailure = false;
            }

            if (allDone) return TilakaQuickSignConstant.RequestStatus.Completed;
            if (allFinalFailure) return TilakaQuickSignConstant.RequestStatus.Failed;
            return TilakaQuickSignConstant.RequestStatus.InProgress;
        }

        public static bool CanApplySignerStatus(string current, string next)
        {
            if (string.IsNullOrWhiteSpace(next)) return false;

            if (current == TilakaQuickSignConstant.SignerStatus.Done) return false;

            if (next == TilakaQuickSignConstant.SignerStatus.Done) return true;
            if (next == TilakaQuickSignConstant.SignerStatus.Failed
                || next == TilakaQuickSignConstant.SignerStatus.Denied)
                return true;

            if (current == TilakaQuickSignConstant.SignerStatus.Failed
                || current == TilakaQuickSignConstant.SignerStatus.Denied)
                return false;

            return next == TilakaQuickSignConstant.SignerStatus.Process
                   || next == TilakaQuickSignConstant.SignerStatus.Unauthorized
                   || next == TilakaQuickSignConstant.SignerStatus.Idle;
        }
    }
}
