using System;
using System.Collections.Generic;
using Temiang.Avicenna.BusinessObject;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaCasemixFileRecord
    {
        public long CasemixFileID { get; set; }
        public string RegistrationNo { get; set; }
        public string ParamedicID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public bool IsVoid { get; set; }
        public long? LastQuickSignRequestID { get; set; }
        public DateTime? LastProcessedDateTime { get; set; }
        public string DownloadPath { get; set; }
    }

    public interface ITilakaCasemixFileRepository
    {
        TilakaCasemixFileRecord FindByID(long casemixFileID);
        IList<TilakaCasemixFileRecord> FindAll();
        void UpdateLink(long casemixFileID, long quickSignRequestID, DateTime processedDateTime);
        void UpdateDownloadPath(long casemixFileID, long quickSignRequestID, string downloadPath);
        void Delete(long casemixFileID);
    }

    public class TilakaCasemixFileRepository : ITilakaCasemixFileRepository
    {
        public TilakaCasemixFileRecord FindByID(long casemixFileID)
        {
            var entity = new TilakaCasemixFile();
            return entity.LoadByPrimaryKey(casemixFileID) ? Map(entity) : null;
        }

        public IList<TilakaCasemixFileRecord> FindAll()
        {
            var collection = new TilakaCasemixFileCollection();
            collection.Query.Where(collection.Query.IsVoid == false);
            collection.Query.OrderBy(collection.Query.CasemixFileID.Descending);
            collection.Load(collection.Query);

            var result = new List<TilakaCasemixFileRecord>();
            foreach (var entity in collection) result.Add(Map(entity));
            return result;
        }

        public void UpdateLink(long casemixFileID, long quickSignRequestID, DateTime processedDateTime)
        {
            var entity = new TilakaCasemixFile();
            if (!entity.LoadByPrimaryKey(casemixFileID)) return;

            entity.LastQuickSignRequestID = quickSignRequestID;
            entity.LastProcessedDateTime = processedDateTime;
            entity.LastUpdateDateTime = DateTime.Now;

            using (var scope = new esTransactionScope())
            {
                entity.Save();
                scope.Complete();
            }
        }

        public void UpdateDownloadPath(long casemixFileID, long quickSignRequestID, string downloadPath)
        {
            if (string.IsNullOrWhiteSpace(downloadPath)) return;

            var entity = new TilakaCasemixFile();
            if (!entity.LoadByPrimaryKey(casemixFileID)) return;

            // Jangan biarkan callback request lama menimpa path request terbaru.
            if (!TilakaSignedFileRules.ShouldUpdateDownloadPath(
                    entity.DownloadPath, entity.LastQuickSignRequestID, quickSignRequestID))
                return;

            entity.DownloadPath = downloadPath;
            entity.LastUpdateDateTime = DateTime.Now;

            using (var scope = new esTransactionScope())
            {
                entity.Save();
                scope.Complete();
            }
        }

        public void Delete(long casemixFileID)
        {
            var entity = new TilakaCasemixFile();
            if (!entity.LoadByPrimaryKey(casemixFileID)) return;

            entity.MarkAsDeleted();
            using (var scope = new esTransactionScope())
            {
                entity.Save();
                scope.Complete();
            }
        }

        private static TilakaCasemixFileRecord Map(TilakaCasemixFile entity)
        {
            return new TilakaCasemixFileRecord
            {
                CasemixFileID = entity.CasemixFileID ?? 0,
                RegistrationNo = entity.RegistrationNo,
                ParamedicID = entity.ParamedicID,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                IsVoid = entity.IsVoid ?? false,
                LastQuickSignRequestID = entity.LastQuickSignRequestID,
                LastProcessedDateTime = entity.LastProcessedDateTime,
                DownloadPath = entity.DownloadPath
            };
        }
    }
}
