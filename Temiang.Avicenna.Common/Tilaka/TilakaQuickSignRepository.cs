using System;
using System.Collections.Generic;
using Temiang.Avicenna.BusinessObject;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Common.Tilaka
{
    public interface ITilakaQuickSignRepository
    {
        TilakaQuickSignRecord FindByRequestId(string requestId);
        IList<TilakaQuickSignSignerRecord> LoadSigners(long quickSignRequestId);
        IList<TilakaQuickSignRecord> FindPending(int max);
        void Save(TilakaQuickSignRecord record, IList<TilakaQuickSignSignerRecord> signers);
    }

    public class TilakaQuickSignRepository : ITilakaQuickSignRepository
    {
        public TilakaQuickSignRecord FindByRequestId(string requestId)
        {
            if (string.IsNullOrWhiteSpace(requestId)) return null;

            var query = new TilakaQuickSignRequestQuery();
            query.Where(query.RequestID == requestId);
            query.es.Top = 1;

            var entity = new TilakaQuickSignRequest();
            return entity.Load(query) ? Map(entity) : null;
        }

        public IList<TilakaQuickSignSignerRecord> LoadSigners(long quickSignRequestId)
        {
            var query = new TilakaQuickSignSignerQuery();
            query.Where(query.QuickSignRequestID == quickSignRequestId);
            query.OrderBy(query.Sequence.Ascending);

            var collection = new TilakaQuickSignSignerCollection();
            collection.Load(query);

            var result = new List<TilakaQuickSignSignerRecord>();
            foreach (var entity in collection) result.Add(Map(entity));
            return result;
        }

        public IList<TilakaQuickSignRecord> FindPending(int max)
        {
            var query = new TilakaQuickSignRequestQuery();
            query.Where(query.IsActive == true);
            query.es.Top = max <= 0 ? 10 : max;
            query.OrderBy(query.RequestDateTime.Ascending);

            var collection = new TilakaQuickSignRequestCollection();
            collection.Load(query);

            var result = new List<TilakaQuickSignRecord>();
            foreach (var entity in collection) result.Add(Map(entity));
            return result;
        }

        public void Save(TilakaQuickSignRecord record, IList<TilakaQuickSignSignerRecord> signers)
        {
            if (record == null) return;

            var entity = new TilakaQuickSignRequest();
            if (record.QuickSignRequestID > 0 && entity.LoadByPrimaryKey(record.QuickSignRequestID))
            {
                Map(record, entity);
            }
            else
            {
                entity.AddNew();
                Map(record, entity);
                entity.CreatedDateTime = record.CreatedDateTime ?? DateTime.Now;
                entity.CreatedByUserID = record.CreatedByUserID;
            }

            entity.LastUpdateDateTime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(record.LastUpdateByUserID))
                entity.LastUpdateByUserID = record.LastUpdateByUserID;

            using (var scope = new esTransactionScope())
            {
                entity.Save();

                if (record.QuickSignRequestID <= 0)
                    record.QuickSignRequestID = entity.QuickSignRequestID ?? 0;

                if (signers != null)
                {
                    foreach (var signer in signers)
                    {
                        signer.QuickSignRequestID = record.QuickSignRequestID;
                        SaveSigner(signer);
                    }
                }

                scope.Complete();
            }
        }

        private static void SaveSigner(TilakaQuickSignSignerRecord record)
        {
            var entity = new TilakaQuickSignSigner();
            if (record.QuickSignSignerID > 0 && entity.LoadByPrimaryKey(record.QuickSignSignerID))
            {
                Map(record, entity);
            }
            else
            {
                entity.AddNew();
                Map(record, entity);
                entity.CreatedDateTime = record.CreatedDateTime ?? DateTime.Now;
                entity.CreatedByUserID = record.CreatedByUserID;
            }

            entity.LastUpdateDateTime = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(record.LastUpdateByUserID))
                entity.LastUpdateByUserID = record.LastUpdateByUserID;

            entity.Save();
            if (record.QuickSignSignerID <= 0)
                record.QuickSignSignerID = entity.QuickSignSignerID ?? 0;
        }

        private static TilakaQuickSignRecord Map(TilakaQuickSignRequest entity)
        {
            return new TilakaQuickSignRecord
            {
                QuickSignRequestID = entity.QuickSignRequestID ?? 0,
                RequestID = entity.RequestID,
                CasemixFileID = entity.CasemixFileID,
                TemplateSignFileID = entity.TemplateSignFileID,
                FileName = entity.FileName,
                TemplateNo = entity.TemplateNo,
                Status = entity.Status,
                IsActive = entity.IsActive ?? false,
                RequestDateTime = entity.RequestDateTime,
                CompletedDateTime = entity.CompletedDateTime,
                SignedFilePath = entity.SignedFilePath,
                ErrorMessage = entity.ErrorMessage,
                CreatedDateTime = entity.CreatedDateTime,
                CreatedByUserID = entity.CreatedByUserID,
                LastUpdateDateTime = entity.LastUpdateDateTime,
                LastUpdateByUserID = entity.LastUpdateByUserID
            };
        }

        private static void Map(TilakaQuickSignRecord record, TilakaQuickSignRequest entity)
        {
            entity.RequestID = record.RequestID;
            entity.CasemixFileID = record.CasemixFileID;
            entity.TemplateSignFileID = record.TemplateSignFileID;
            entity.FileName = record.FileName;
            entity.TemplateNo = record.TemplateNo;
            entity.Status = record.Status;
            entity.IsActive = record.IsActive;
            entity.RequestDateTime = record.RequestDateTime;
            entity.CompletedDateTime = record.CompletedDateTime;
            entity.SignedFilePath = record.SignedFilePath;
            entity.ErrorMessage = record.ErrorMessage;
        }

        private static TilakaQuickSignSignerRecord Map(TilakaQuickSignSigner entity)
        {
            return new TilakaQuickSignSignerRecord
            {
                QuickSignSignerID = entity.QuickSignSignerID ?? 0,
                QuickSignRequestID = entity.QuickSignRequestID ?? 0,
                Sequence = entity.Sequence ?? 0,
                UserIdentifier = entity.UserIdentifier,
                Email = entity.Email,
                Status = entity.Status,
                NumSignatures = entity.NumSignatures,
                NumSignaturesDone = entity.NumSignaturesDone,
                AuthUrl = entity.AuthUrl,
                TokenKey = entity.TokenKey,
                SignedUrl = entity.SignedUrl,
                ErrorMessage = entity.ErrorMessage,
                LastCheckedDateTime = entity.LastCheckedDateTime,
                CreatedDateTime = entity.CreatedDateTime,
                CreatedByUserID = entity.CreatedByUserID,
                LastUpdateDateTime = entity.LastUpdateDateTime,
                LastUpdateByUserID = entity.LastUpdateByUserID
            };
        }

        private static void Map(TilakaQuickSignSignerRecord record, TilakaQuickSignSigner entity)
        {
            entity.QuickSignRequestID = record.QuickSignRequestID;
            entity.Sequence = record.Sequence;
            entity.UserIdentifier = record.UserIdentifier;
            entity.Email = record.Email;
            entity.Status = record.Status;
            entity.NumSignatures = record.NumSignatures;
            entity.NumSignaturesDone = record.NumSignaturesDone;
            entity.AuthUrl = record.AuthUrl;
            entity.TokenKey = record.TokenKey;
            entity.SignedUrl = record.SignedUrl;
            entity.ErrorMessage = record.ErrorMessage;
            entity.LastCheckedDateTime = record.LastCheckedDateTime;
        }
    }
}
