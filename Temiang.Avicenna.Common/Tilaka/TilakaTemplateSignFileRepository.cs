using System;
using System.Collections.Generic;
using Temiang.Avicenna.BusinessObject;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Common.Tilaka
{
    public class TilakaTemplateSignFileRecord
    {
        public long TemplateSignFileID { get; set; }
        public string ParamedicID { get; set; }
        public string TemplateNo { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long? FileSize { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public bool IsRegistered { get; set; }
        public DateTime? RegisteredDateTime { get; set; }
        public string LastErrorMessage { get; set; }
        public DateTime? CreatedDateTime { get; set; }
        public string CreatedByUserID { get; set; }
        public DateTime? LastUpdateDateTime { get; set; }
        public string LastUpdateByUserID { get; set; }
    }

    public interface ITilakaTemplateSignFileRepository
    {
        TilakaTemplateSignFileRecord FindByID(long templateSignFileID);
        TilakaTemplateSignFileRecord FindDefault(string paramedicID);
        TilakaTemplateSignFileRecord FindByParamedicAndTemplateNo(string paramedicID, string templateNo);
        IList<TilakaTemplateSignFileRecord> FindByParamedic(string paramedicID);
        void Save(TilakaTemplateSignFileRecord record);
    }

    public class TilakaTemplateSignFileRepository : ITilakaTemplateSignFileRepository
    {
        public TilakaTemplateSignFileRecord FindByID(long templateSignFileID)
        {
            var entity = new TilakaTemplateSignFile();
            return entity.LoadByPrimaryKey(templateSignFileID) ? Map(entity) : null;
        }

        public TilakaTemplateSignFileRecord FindDefault(string paramedicID)
        {
            if (string.IsNullOrWhiteSpace(paramedicID)) return null;

            var query = new TilakaTemplateSignFileQuery();
            query.Where(query.ParamedicID == paramedicID,
                query.IsDefault == true,
                query.IsRegistered == true,
                query.IsActive == true);
            query.es.Top = 1;

            var entity = new TilakaTemplateSignFile();
            return entity.Load(query) ? Map(entity) : null;
        }

        public TilakaTemplateSignFileRecord FindByParamedicAndTemplateNo(string paramedicID, string templateNo)
        {
            if (string.IsNullOrWhiteSpace(paramedicID) || string.IsNullOrWhiteSpace(templateNo)) return null;

            var query = new TilakaTemplateSignFileQuery();
            query.Where(query.ParamedicID == paramedicID, query.TemplateNo == templateNo);
            query.es.Top = 1;

            var entity = new TilakaTemplateSignFile();
            return entity.Load(query) ? Map(entity) : null;
        }

        public IList<TilakaTemplateSignFileRecord> FindByParamedic(string paramedicID)
        {
            var query = new TilakaTemplateSignFileQuery();
            query.Where(query.ParamedicID == paramedicID);
            query.OrderBy(query.IsDefault.Descending, query.TemplateNo.Ascending);

            var collection = new TilakaTemplateSignFileCollection();
            collection.Load(query);

            var result = new List<TilakaTemplateSignFileRecord>();
            foreach (var entity in collection) result.Add(Map(entity));
            return result;
        }

        public void Save(TilakaTemplateSignFileRecord record)
        {
            if (record == null) return;

            var entity = new TilakaTemplateSignFile();
            if (record.TemplateSignFileID > 0 && entity.LoadByPrimaryKey(record.TemplateSignFileID))
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
                scope.Complete();
            }

            record.TemplateSignFileID = entity.TemplateSignFileID ?? 0;
        }

        private static TilakaTemplateSignFileRecord Map(TilakaTemplateSignFile entity)
        {
            return new TilakaTemplateSignFileRecord
            {
                TemplateSignFileID = entity.TemplateSignFileID ?? 0,
                ParamedicID = entity.ParamedicID,
                TemplateNo = entity.TemplateNo,
                FileName = entity.FileName,
                FilePath = entity.FilePath,
                ContentType = entity.ContentType,
                FileSize = entity.FileSize,
                IsDefault = entity.IsDefault ?? false,
                IsActive = entity.IsActive ?? false,
                IsRegistered = entity.IsRegistered ?? false,
                RegisteredDateTime = entity.RegisteredDateTime,
                LastErrorMessage = entity.LastErrorMessage,
                CreatedDateTime = entity.CreatedDateTime,
                CreatedByUserID = entity.CreatedByUserID,
                LastUpdateDateTime = entity.LastUpdateDateTime,
                LastUpdateByUserID = entity.LastUpdateByUserID
            };
        }

        private static void Map(TilakaTemplateSignFileRecord record, TilakaTemplateSignFile entity)
        {
            entity.ParamedicID = record.ParamedicID;
            entity.TemplateNo = record.TemplateNo;
            entity.FileName = record.FileName;
            entity.FilePath = record.FilePath;
            entity.ContentType = record.ContentType;
            entity.FileSize = record.FileSize;
            entity.IsDefault = record.IsDefault;
            entity.IsActive = record.IsActive;
            entity.IsRegistered = record.IsRegistered;
            entity.RegisteredDateTime = record.RegisteredDateTime;
            entity.LastErrorMessage = record.LastErrorMessage;
        }
    }
}
