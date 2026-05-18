/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-05-07 10:59:43 AM
===============================================================================
				Author: Wiliam Decosta (wiliamdecosta@gmail.com) - YBRS
===============================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Xml.Serialization;
using Temiang.Avicenna.BusinessObject.Generated;
using Temiang.Dal.Core;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.BusinessObject
{
    [Serializable]
    abstract public class esVisitQueueCollection : esEntityCollectionWAuditLog
    {
        public esVisitQueueCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "VisitQueueCollection";
        }

        #region Query Logic
        protected void InitQuery(esVisitQueueQuery query)
        {
            query.OnLoadDelegate = this.OnQueryLoaded;
            query.es2.Connection = ((IEntityCollection)this).Connection;
        }

        protected bool OnQueryLoaded(DataTable table)
        {
            this.PopulateCollection(table);
            return (this.RowCount > 0) ? true : false;
        }

        protected override void HookupQuery(esDynamicQuery query)
        {
            this.InitQuery(query as esVisitQueueQuery);
        }
        #endregion

        virtual public VisitQueue DetachEntity(VisitQueue entity)
        {
            return base.DetachEntity(entity) as VisitQueue;
        }

        virtual public VisitQueue AttachEntity(VisitQueue entity)
        {
            return base.AttachEntity(entity) as VisitQueue;
        }

        virtual public void Combine(VisitQueueCollection collection)
        {
            base.Combine(collection);
        }

        new public VisitQueue this[int index]
        {
            get
            {
                return base[index] as VisitQueue;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(VisitQueue);
        }
    }

    [Serializable]
    abstract public class esVisitQueue : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esVisitQueueQuery GetDynamicQuery()
        {
            return null;
        }

        public esVisitQueue()
        {
        }

        public esVisitQueue(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(String visitQueueNo)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(visitQueueNo);
            else
                return LoadByPrimaryKeyStoredProcedure(visitQueueNo);
        }

        /// <summary>
        /// Loads an entity by primary key
        /// </summary>
        /// <remarks>
        /// Requires primary keys be defined on all tables.
        /// If a table does not have a primary key set,
        /// this method will not compile.
        /// </remarks>
        /// <param name="sqlAccessType">Either esSqlAccessType StoredProcedure or DynamicSQL</param>
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String visitQueueNo)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(visitQueueNo);
            else
                return LoadByPrimaryKeyStoredProcedure(visitQueueNo);
        }

        private bool LoadByPrimaryKeyDynamic(String visitQueueNo)
        {
            esVisitQueueQuery query = this.GetDynamicQuery();
            query.Where(query.VisitQueueNo == visitQueueNo);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(String visitQueueNo)
        {
            esParameters parms = new esParameters();
            parms.Add("VisitQueueNo", visitQueueNo);
            return this.Load(esQueryType.StoredProcedure, this.es.spLoadByPrimaryKey, parms);
        }
        #endregion

        #region Properties

        public override void SetProperties(IDictionary values)
        {
            foreach (string propertyName in values.Keys)
            {
                this.SetProperty(propertyName, values[propertyName]);
            }
        }

        public override void SetProperty(string name, object value)
        {
            if (this.Row == null) this.AddNew();

            esColumnMetadata col = this.Meta.Columns.FindByPropertyName(name);
            if (col != null)
            {
                if (value == null || value is System.String)
                {
                    // Use the strongly typed property
                    switch (name)
                    {
                        case "VisitQueueNo": this.str.VisitQueueNo = (string)value; break;
                        case "VisitNo": this.str.VisitNo = (string)value; break;
                        case "SRAutoNumber": this.str.SRAutoNumber = (string)value; break;
                        case "RegistrationNo": this.str.RegistrationNo = (string)value; break;
                        case "QueueDate": this.str.QueueDate = (string)value; break;
                        case "Status": this.str.Status = (string)value; break;
                        case "CurrentStage": this.str.CurrentStage = (string)value; break;
                        case "CalledByCounterID": this.str.CalledByCounterID = (string)value; break;
                        case "CalledTime": this.str.CalledTime = (string)value; break;
                        case "ServedTime": this.str.ServedTime = (string)value; break;
                        case "FinishedTime": this.str.FinishedTime = (string)value; break;
                        case "PatientID": this.str.PatientID = (string)value; break;
                        case "CreatedDate": this.str.CreatedDate = (string)value; break;
                        case "CreatedBy": this.str.CreatedBy = (string)value; break;
                        case "QueueSequence": this.str.QueueSequence = (string)value; break;
                        case "Priority": this.str.Priority = (string)value; break;
                        case "IsManualOverride": this.str.IsManualOverride = (string)value; break;
                        case "LastUpdated": this.str.LastUpdated = (string)value; break;
                        case "UpdatedBy": this.str.UpdatedBy = (string)value; break;
                        case "ServiceUnitID": this.str.ServiceUnitID = (string)value; break;
                        case "ParamedicID": this.str.ParamedicID = (string)value; break;
                        case "StageID": this.str.StageID = (string)value; break;
                        case "CategoryID": this.str.CategoryID = (string)value; break;
                        case "QueueKey": this.str.QueueKey = (string)value; break;
                        case "QueueLocation": this.str.QueueLocation = (string)value; break;
                        case "IsRecall": this.str.IsRecall = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "QueueDate":

                            if (value == null || value is System.DateTime)
                                this.QueueDate = (System.DateTime?)value;
                            break;
                        case "CalledTime":

                            if (value == null || value is System.DateTime)
                                this.CalledTime = (System.DateTime?)value;
                            break;
                        case "ServedTime":

                            if (value == null || value is System.DateTime)
                                this.ServedTime = (System.DateTime?)value;
                            break;
                        case "FinishedTime":

                            if (value == null || value is System.DateTime)
                                this.FinishedTime = (System.DateTime?)value;
                            break;
                        case "CreatedDate":

                            if (value == null || value is System.DateTime)
                                this.CreatedDate = (System.DateTime?)value;
                            break;
                        case "QueueSequence":

                            if (value == null || value is System.Int32)
                                this.QueueSequence = (System.Int32?)value;
                            break;
                        case "Priority":

                            if (value == null || value is System.Int32)
                                this.Priority = (System.Int32?)value;
                            break;
                        case "IsManualOverride":

                            if (value == null || value is System.Boolean)
                                this.IsManualOverride = (System.Boolean?)value;
                            break;
                        case "LastUpdated":

                            if (value == null || value is System.DateTime)
                                this.LastUpdated = (System.DateTime?)value;
                            break;
                        case "IsRecall":

                            if (value == null || value is System.Int32)
                                this.IsRecall = (System.Int32?)value;
                            break;

                        default:
                            break;
                    }
                }
            }
            else if (this.Row.Table.Columns.Contains(name))
            {
                this.Row[name] = value;
            }
            else
            {
                throw new Exception("SetProperty Error: '" + name + "' not found");
            }
        }

        /// <summary>
        /// Maps to VisitQueue.VisitQueueNo
        /// </summary>
        virtual public System.String VisitQueueNo
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.VisitQueueNo);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.VisitQueueNo, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.VisitNo
        /// </summary>
        virtual public System.String VisitNo
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.VisitNo);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.VisitNo, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.SRAutoNumber
        /// </summary>
        virtual public System.String SRAutoNumber
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.SRAutoNumber);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.SRAutoNumber, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.RegistrationNo
        /// </summary>
        virtual public System.String RegistrationNo
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.RegistrationNo);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.RegistrationNo, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.QueueDate
        /// </summary>
        virtual public System.DateTime? QueueDate
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.QueueDate);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.QueueDate, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.Status
        /// </summary>
        virtual public System.String Status
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.Status);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.Status, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CurrentStage
        /// </summary>
        virtual public System.String CurrentStage
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.CurrentStage);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.CurrentStage, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CalledByCounterID
        /// </summary>
        virtual public System.String CalledByCounterID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.CalledByCounterID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.CalledByCounterID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CalledTime
        /// </summary>
        virtual public System.DateTime? CalledTime
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.CalledTime);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.CalledTime, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.ServedTime
        /// </summary>
        virtual public System.DateTime? ServedTime
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.ServedTime);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.ServedTime, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.FinishedTime
        /// </summary>
        virtual public System.DateTime? FinishedTime
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.FinishedTime);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.FinishedTime, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.PatientID
        /// </summary>
        virtual public System.String PatientID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.PatientID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.PatientID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CreatedDate
        /// </summary>
        virtual public System.DateTime? CreatedDate
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.CreatedDate);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.CreatedDate, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CreatedBy
        /// </summary>
        virtual public System.String CreatedBy
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.CreatedBy);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.CreatedBy, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.QueueSequence
        /// </summary>
        virtual public System.Int32? QueueSequence
        {
            get
            {
                return base.GetSystemInt32(VisitQueueMetadata.ColumnNames.QueueSequence);
            }

            set
            {
                base.SetSystemInt32(VisitQueueMetadata.ColumnNames.QueueSequence, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.Priority
        /// </summary>
        virtual public System.Int32? Priority
        {
            get
            {
                return base.GetSystemInt32(VisitQueueMetadata.ColumnNames.Priority);
            }

            set
            {
                base.SetSystemInt32(VisitQueueMetadata.ColumnNames.Priority, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.IsManualOverride
        /// </summary>
        virtual public System.Boolean? IsManualOverride
        {
            get
            {
                return base.GetSystemBoolean(VisitQueueMetadata.ColumnNames.IsManualOverride);
            }

            set
            {
                base.SetSystemBoolean(VisitQueueMetadata.ColumnNames.IsManualOverride, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.LastUpdated
        /// </summary>
        virtual public System.DateTime? LastUpdated
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueMetadata.ColumnNames.LastUpdated);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueMetadata.ColumnNames.LastUpdated, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.UpdatedBy
        /// </summary>
        virtual public System.String UpdatedBy
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.UpdatedBy);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.UpdatedBy, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.ServiceUnitID
        /// </summary>
        virtual public System.String ServiceUnitID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.ServiceUnitID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.ServiceUnitID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.ParamedicID
        /// </summary>
        virtual public System.String ParamedicID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.ParamedicID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.ParamedicID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.StageID
        /// </summary>
        virtual public System.String StageID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.StageID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.StageID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.CategoryID
        /// </summary>
        virtual public System.String CategoryID
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.CategoryID);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.CategoryID, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.QueueKey
        /// </summary>
        virtual public System.String QueueKey
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.QueueKey);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.QueueKey, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.QueueLocation
        /// </summary>
        virtual public System.String QueueLocation
        {
            get
            {
                return base.GetSystemString(VisitQueueMetadata.ColumnNames.QueueLocation);
            }

            set
            {
                base.SetSystemString(VisitQueueMetadata.ColumnNames.QueueLocation, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueue.IsRecall
        /// </summary>
        virtual public System.Int32? IsRecall
        {
            get
            {
                return base.GetSystemInt32(VisitQueueMetadata.ColumnNames.IsRecall);
            }

            set
            {
                base.SetSystemInt32(VisitQueueMetadata.ColumnNames.IsRecall, value);
            }
        }


        #endregion

        #region String Properties

        /// <summary>
        /// Converts an entity's properties to
        /// and from strings.
        /// </summary>
        /// <remarks>
        /// The str properties Get and Set provide easy conversion
        /// between a string and a property's data type. Not all
        /// data types will get a str property.
        /// </remarks>
        /// <example>
        /// Set a datetime from a string.
        /// <code>
        /// Employees entity = new Employees();
        /// entity.LoadByPrimaryKey(10);
        /// entity.str.HireDate = "2007-01-01 00:00:00";
        /// entity.Save();
        /// </code>
        /// Get a datetime as a string.
        /// <code>
        /// Employees entity = new Employees();
        /// entity.LoadByPrimaryKey(10);
        /// string theDate = entity.str.HireDate;
        /// </code>
        /// </example>
        [BrowsableAttribute(false)]
        public esStrings str
        {
            get
            {
                if (esstrings == null)
                {
                    esstrings = new esStrings(this);
                }
                return esstrings;
            }
        }

        [Serializable]
        sealed public class esStrings
        {
            public esStrings(esVisitQueue entity)
            {
                this.entity = entity;
            }
            public System.String VisitQueueNo
            {
                get
                {
                    System.String data = entity.VisitQueueNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.VisitQueueNo = null;
                    else entity.VisitQueueNo = Convert.ToString(value);
                }
            }
            public System.String VisitNo
            {
                get
                {
                    System.String data = entity.VisitNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.VisitNo = null;
                    else entity.VisitNo = Convert.ToString(value);
                }
            }
            public System.String SRAutoNumber
            {
                get
                {
                    System.String data = entity.SRAutoNumber;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.SRAutoNumber = null;
                    else entity.SRAutoNumber = Convert.ToString(value);
                }
            }
            public System.String RegistrationNo
            {
                get
                {
                    System.String data = entity.RegistrationNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.RegistrationNo = null;
                    else entity.RegistrationNo = Convert.ToString(value);
                }
            }
            public System.String QueueDate
            {
                get
                {
                    System.DateTime? data = entity.QueueDate;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.QueueDate = null;
                    else entity.QueueDate = Convert.ToDateTime(value);
                }
            }
            public System.String Status
            {
                get
                {
                    System.String data = entity.Status;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.Status = null;
                    else entity.Status = Convert.ToString(value);
                }
            }
            public System.String CurrentStage
            {
                get
                {
                    System.String data = entity.CurrentStage;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CurrentStage = null;
                    else entity.CurrentStage = Convert.ToString(value);
                }
            }
            public System.String CalledByCounterID
            {
                get
                {
                    System.String data = entity.CalledByCounterID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CalledByCounterID = null;
                    else entity.CalledByCounterID = Convert.ToString(value);
                }
            }
            public System.String CalledTime
            {
                get
                {
                    System.DateTime? data = entity.CalledTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CalledTime = null;
                    else entity.CalledTime = Convert.ToDateTime(value);
                }
            }
            public System.String ServedTime
            {
                get
                {
                    System.DateTime? data = entity.ServedTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ServedTime = null;
                    else entity.ServedTime = Convert.ToDateTime(value);
                }
            }
            public System.String FinishedTime
            {
                get
                {
                    System.DateTime? data = entity.FinishedTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.FinishedTime = null;
                    else entity.FinishedTime = Convert.ToDateTime(value);
                }
            }
            public System.String PatientID
            {
                get
                {
                    System.String data = entity.PatientID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PatientID = null;
                    else entity.PatientID = Convert.ToString(value);
                }
            }
            public System.String CreatedDate
            {
                get
                {
                    System.DateTime? data = entity.CreatedDate;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CreatedDate = null;
                    else entity.CreatedDate = Convert.ToDateTime(value);
                }
            }
            public System.String CreatedBy
            {
                get
                {
                    System.String data = entity.CreatedBy;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CreatedBy = null;
                    else entity.CreatedBy = Convert.ToString(value);
                }
            }
            public System.String QueueSequence
            {
                get
                {
                    System.Int32? data = entity.QueueSequence;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.QueueSequence = null;
                    else entity.QueueSequence = Convert.ToInt32(value);
                }
            }
            public System.String Priority
            {
                get
                {
                    System.Int32? data = entity.Priority;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.Priority = null;
                    else entity.Priority = Convert.ToInt32(value);
                }
            }
            public System.String IsManualOverride
            {
                get
                {
                    System.Boolean? data = entity.IsManualOverride;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsManualOverride = null;
                    else entity.IsManualOverride = Convert.ToBoolean(value);
                }
            }
            public System.String LastUpdated
            {
                get
                {
                    System.DateTime? data = entity.LastUpdated;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.LastUpdated = null;
                    else entity.LastUpdated = Convert.ToDateTime(value);
                }
            }
            public System.String UpdatedBy
            {
                get
                {
                    System.String data = entity.UpdatedBy;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.UpdatedBy = null;
                    else entity.UpdatedBy = Convert.ToString(value);
                }
            }
            public System.String ServiceUnitID
            {
                get
                {
                    System.String data = entity.ServiceUnitID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ServiceUnitID = null;
                    else entity.ServiceUnitID = Convert.ToString(value);
                }
            }
            public System.String ParamedicID
            {
                get
                {
                    System.String data = entity.ParamedicID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ParamedicID = null;
                    else entity.ParamedicID = Convert.ToString(value);
                }
            }
            public System.String StageID
            {
                get
                {
                    System.String data = entity.StageID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.StageID = null;
                    else entity.StageID = Convert.ToString(value);
                }
            }
            public System.String CategoryID
            {
                get
                {
                    System.String data = entity.CategoryID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CategoryID = null;
                    else entity.CategoryID = Convert.ToString(value);
                }
            }
            public System.String QueueKey
            {
                get
                {
                    System.String data = entity.QueueKey;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.QueueKey = null;
                    else entity.QueueKey = Convert.ToString(value);
                }
            }
            public System.String QueueLocation
            {
                get
                {
                    System.String data = entity.QueueLocation;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.QueueLocation = null;
                    else entity.QueueLocation = Convert.ToString(value);
                }
            }
            public System.String IsRecall
            {
                get
                {
                    System.Int32? data = entity.IsRecall;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsRecall = null;
                    else entity.IsRecall = Convert.ToInt32(value);
                }
            }

            private esVisitQueue entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esVisitQueueQuery query)
        {
            query.OnLoadDelegate = this.OnQueryLoaded;
            query.es2.Connection = ((IEntity)this).Connection;
        }

        [System.Diagnostics.DebuggerNonUserCode]
        protected bool OnQueryLoaded(DataTable table)
        {
            bool dataFound = this.PopulateEntity(table);

            if (this.RowCount > 1)
            {
                throw new Exception("esVisitQueue can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class VisitQueue : esVisitQueue
    {
        public static object GetDisplayAntrianPasien(
            DateTime queueDate,
            string queueLocation
        )
        {
            var collection =
                new VisitQueueCollection();

            var query =
                new VisitQueueQuery("v");

            var semantic =
                new AntrianAutoNumberSemanticQuery("s");

            query.es.WithNoLock = true;

            query.Select(
                query.VisitNo,
                query.Status,
                query.QueueLocation,
                semantic.DisplayName,
                query.CalledByCounterID
            );

            query.LeftJoin(semantic)
                .On(
                    query.SRAutoNumber == semantic.SRAutoNumber
                    &&
                    query.QueueLocation == semantic.Channel
                );

            query.Where(
                query.QueueLocation == queueLocation
            );

            query.Where(
                query.QueueDate >= queueDate.Date,
                query.QueueDate < queueDate.Date.AddDays(1)
            );

            query.OrderBy(
                query.QueueSequence.Ascending,
                semantic.DisplayOrder.Ascending,
                query.VisitNo.Ascending
            );

            collection.Load(query);

            return collection
                .Select(x => new
                {
                    VisitNo = x.VisitNo,

                    Status = x.Status,

                    QueueLocation = x.QueueLocation,

                    DisplayName =
                        x.GetColumn("DisplayName") == null
                            ? ""
                            : x.GetColumn("DisplayName").ToString(),

                    CalledByCounterID = x.CalledByCounterID
                })
                .ToList();
        }

        public static object GetDisplayAntrianPendaftaran(
            DateTime queueDate,
            string status,
            string srAutoNumber,
            string currentStage,
            string queueLocation
        )
        {
            var collection =
                new VisitQueueCollection();

            var query =
                new VisitQueueQuery("v");

            query.es.WithNoLock = true;

            query.Select(
                query.VisitQueueNo,
                query.VisitNo,
                query.QueueDate,
                query.Status,
                query.SRAutoNumber,
                query.CurrentStage,
                query.QueueLocation,
                query.QueueSequence,
                query.CalledByCounterID
            );

            // =========================================
            // FILTER DATE
            // =========================================
            query.Where(
                query.QueueDate >= queueDate.Date,
                query.QueueDate < queueDate.Date.AddDays(1)
            );

            // =========================================
            // OPTIONAL FILTER
            // =========================================
            if (!string.IsNullOrEmpty(status))
            {
                query.Where(
                    query.Status == status
                );
            }

            if (!string.IsNullOrEmpty(srAutoNumber))
            {
                query.Where(
                    query.SRAutoNumber == srAutoNumber
                );
            }

            if (!string.IsNullOrEmpty(currentStage))
            {
                query.Where(
                    query.CurrentStage == currentStage
                );
            }

            if (!string.IsNullOrEmpty(queueLocation))
            {
                query.Where(
                    query.QueueLocation == queueLocation
                );
            }

            // =========================================
            // ORDER
            // =========================================
            query.OrderBy(
                query.QueueSequence.Ascending
            );

            collection.Load(query);

            return collection
                .Select(x => new
                {
                    VisitQueueNo = x.VisitQueueNo,
                    VisitNo = x.VisitNo,
                    QueueDate = x.QueueDate,
                    Status = x.Status,
                    SRAutoNumber = x.SRAutoNumber,
                    CurrentStage = x.CurrentStage,
                    QueueLocation = x.QueueLocation,
                    QueueSequence = x.QueueSequence,
                    CalledByCounterID = x.CalledByCounterID
                })
                .ToList();
        }

        public static object CallAntrianSekarangPendaftaran(
            string visitQueueNo,
            string userID,
            string counterID
        )
        {
            esParameters prms =
                new esParameters();

            // =========================================
            // INPUT
            // =========================================
            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "CounterID",
                counterID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // OUTPUT
            // =========================================
            prms.Add(
                "VisitNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE SP
            // =========================================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianCallNow",
                prms
            );

            // =========================================
            // AMBIL DATA TERBARU
            // =========================================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo = visitQueueNo,

                VisitNo =
                    prms["VisitNo"].Value == null
                        ? ""
                        : prms["VisitNo"].Value.ToString(),

                CounterID = counterID,

                Status =
                    queue.Status ?? "CALLED",

                CalledTime =
                    queue.CalledTime
            };
        }

        public static object RecallAntrianPendaftaran(
            string visitQueueNo,
            string userID
        )
        {
            esParameters prms =
                new esParameters();

            // =========================
            // INPUT
            // =========================
            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================
            // OUTPUT
            // =========================
            prms.Add(
                "VisitNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            // =========================
            // EXEC SP
            // =========================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianRecall",
                prms
            );

            // =========================
            // GET UPDATED DATA
            // =========================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo = visitQueueNo,

                VisitNo =
                    prms["VisitNo"].Value == null
                        ? ""
                        : prms["VisitNo"].Value.ToString(),

                Status =
                    queue.Status ?? "CALLED",

                CalledTime =
                    queue.CalledTime ?? DateTime.Now,

                CounterID =
                    queue.CalledByCounterID
            };
        }

        public static object PendingAntrianPendaftaran(
            string visitQueueNo,
            string userID
        )
        {
            esParameters prms =
                new esParameters();

            // =========================
            // INPUT
            // =========================
            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================
            // EXEC SP
            // =========================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianSetPending",
                prms
            );

            // =========================
            // AMBIL DATA TERBARU
            // =========================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo =
                    queue.VisitQueueNo ?? visitQueueNo,

                VisitNo =
                    queue.VisitNo,

                Status =
                    queue.Status ?? "PENDING",

                CalledTime =
                    queue.CalledTime ?? DateTime.Now,

                CounterID =
                    queue.CalledByCounterID
            };
        }

        public static object WaitingFromPendingStatusPendaftaran(
            string visitQueueNo,
            string userID
        )
        {
            esParameters prms =
                new esParameters();

            // =========================
            // INPUT
            // =========================
            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================
            // EXEC SP
            // =========================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianSetWaitingFromPending",
                prms
            );

            // =========================
            // AMBIL DATA TERBARU
            // =========================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo = queue.VisitQueueNo ?? visitQueueNo,

                VisitNo = queue.VisitNo,

                Status = queue.Status ?? "WAITING",

                CalledTime = queue.CalledTime ?? DateTime.Now,

                CounterID = queue.CalledByCounterID,

                QueueSequence = queue.QueueSequence
            };
        }

        public static object NextAntrianPendaftaran(
            string queueLocation,
            string userID,
            string counterID,
            DateTime queueDate
        )
        {
            esParameters prms =
                new esParameters();

            // =========================
            // INPUT
            // =========================
            prms.Add(
                "QueueLocation",
                queueLocation,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "CounterID",
                counterID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "QueueDate",
                queueDate.Date,
                esParameterDirection.Input,
                DbType.Date,
                0
            );

            // =========================
            // OUTPUT
            // =========================
            prms.Add(
                "VisitQueueNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            prms.Add(
                "VisitNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            // =========================
            // EXEC SP
            // =========================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianCallNextQueue",
                prms
            );

            // =========================
            // VALIDASI OUTPUT
            // =========================
            var visitQueueNo =
                prms["VisitQueueNo"].Value == null
                    ? ""
                    : prms["VisitQueueNo"].Value.ToString();

            var visitNo =
                prms["VisitNo"].Value == null
                    ? ""
                    : prms["VisitNo"].Value.ToString();

            if (string.IsNullOrEmpty(visitQueueNo))
            {
                return null;
            }

            // =========================
            // AMBIL DETAIL TERBARU
            // =========================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo = visitQueueNo,

                VisitNo = visitNo,

                Status = queue.Status ?? "CALLED",

                CalledTime = queue.CalledTime ?? DateTime.Now,

                CounterID = queue.CalledByCounterID ?? counterID
            };
        }

        public static object InsertVisitQueueStage
        (
            string visitNo,
            string srAutoNumber,
            string userID,
            DateTime transDate,
            string serviceUnitID,
            string paramedicID,
            string registrationNo,
            string patientID
        )
        {
            esParameters prms =
                new esParameters();

            // =========================================
            // INPUT
            // =========================================

            prms.Add(
                "VisitNo",
                visitNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "SRAutoNumber",
                srAutoNumber,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "TransDate",
                transDate.Date,
                esParameterDirection.Input,
                DbType.Date,
                0
            );

            prms.Add(
                "ServiceUnitID",
                serviceUnitID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "ParamedicID",
                paramedicID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "RegistrationNo",
                registrationNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "PatientID",
                patientID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // OUTPUT
            // =========================================

            prms.Add(
                "VisitQueueNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE SP
            // =========================================

            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "TakeQueueVisitNumber_Stage",
                prms
            );

            // =========================================
            // RESULT
            // =========================================

            string visitQueueNo =
                prms["VisitQueueNo"].Value == null
                    ? ""
                    : prms["VisitQueueNo"].Value.ToString();

            if (string.IsNullOrEmpty(visitQueueNo))
            {
                return null;
            }

            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            return new
            {
                VisitQueueNo = queue.VisitQueueNo,
                VisitNo = queue.VisitNo,
                CurrentStage = queue.CurrentStage,
                StageID = queue.StageID,
                Status = queue.Status,
                QueueSequence = queue.QueueSequence
            };
        }

        public static object GetQueueForAllServieUnitPasien(
            DateTime queueDate,
            string status,
            string stageID,
            string serviceUnitID,
            string paramedicID
        )
        {
            var collection = new VisitQueueCollection();
            var query = new VisitQueueQuery("v");

            query.es.WithNoLock = true;

            query.Select(
                query.VisitQueueNo,
                query.VisitNo,
                query.QueueDate,
                query.Status,
                query.ServiceUnitID,
                query.ParamedicID,
                query.QueueSequence
            );

            // =========================================
            // FILTER DATE
            // =========================================
            var startDate = queueDate.Date;
            var endDate = startDate.AddDays(1);

            query.Where(
                query.QueueDate >= startDate,
                query.QueueDate < endDate
            );

            // =========================================
            // OPTIONAL FILTERS
            // =========================================
            if (!string.IsNullOrWhiteSpace(status))
            {
                query.Where(query.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(stageID))
            {
                query.Where(query.StageID == stageID);
            }

            if (!string.IsNullOrWhiteSpace(serviceUnitID))
            {
                query.Where(query.ServiceUnitID == serviceUnitID);
            }

            if (!string.IsNullOrWhiteSpace(paramedicID))
            {
                query.Where(query.ParamedicID == paramedicID);
            }

            // =========================================
            // ORDER + TOP
            // =========================================
            query.OrderBy(query.QueueSequence.Ascending);
            query.es.Top = 50;

            collection.Load(query);

            // =========================================
            // LOAD MASTER
            // =========================================
            var serviceUnits = new ServiceUnitCollection();
            serviceUnits.LoadAll();

            var paramedics = new ParamedicCollection();
            paramedics.LoadAll();

            // =========================================
            // RESULT
            // =========================================
            return collection
                .Select(x => new
                {
                    x.VisitQueueNo,
                    x.VisitNo,
                    x.QueueDate,
                    x.Status,

                    x.ServiceUnitID,

                    ServiceUnitName =
                        serviceUnits
                            .FirstOrDefault(s =>
                                s.ServiceUnitID == x.ServiceUnitID
                            )
                            ?.ServiceUnitName,

                    x.ParamedicID,

                    ParamedicName =
                        paramedics
                            .FirstOrDefault(p =>
                                p.ParamedicID == x.ParamedicID
                            )
                            ?.ParamedicName,
                })
                .ToList();
        }

        public static object GetQueueForAllServieUnitAdmin(
            DateTime queueDate,
            string status,
            string stageID,
            string serviceUnitID,
            string paramedicID
        )
        {
            var collection = new VisitQueueCollection();
            var query = new VisitQueueQuery("v");

            query.es.WithNoLock = true;

            query.Select(
                query.VisitQueueNo,
                query.VisitNo,
                query.RegistrationNo,
                query.PatientID,
                query.QueueDate,
                query.Status,
                query.ServiceUnitID,
                query.ParamedicID,
                query.QueueSequence,
                query.StageID
            );

            // =========================================
            // FILTER DATE
            // =========================================
            var startDate = queueDate.Date;
            var endDate = startDate.AddDays(1);

            query.Where(
                query.QueueDate >= startDate,
                query.QueueDate < endDate
            );

            // =========================================
            // OPTIONAL FILTERS
            // =========================================
            if (!string.IsNullOrWhiteSpace(status))
            {
                query.Where(query.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(stageID))
            {
                query.Where(query.StageID == stageID);
            }

            if (!string.IsNullOrWhiteSpace(serviceUnitID))
            {
                query.Where(query.ServiceUnitID == serviceUnitID);
            }

            if (!string.IsNullOrWhiteSpace(paramedicID))
            {
                query.Where(query.ParamedicID == paramedicID);
            }

            // =========================================
            // ORDER + TOP
            // =========================================
            query.OrderBy(query.QueueSequence.Ascending);
            query.es.Top = 50;

            collection.Load(query);

            // =========================================
            // LOAD SERVICE UNIT
            // =========================================
            var serviceUnits = new ServiceUnitCollection();
            serviceUnits.LoadAll();

            var serviceUnitDict =
                serviceUnits.ToDictionary(
                    x => x.ServiceUnitID,
                    x => x.ServiceUnitName
                );

            // =========================================
            // LOAD PARAMEDIC
            // =========================================
            var paramedics = new ParamedicCollection();
            paramedics.LoadAll();

            var paramedicDict =
                paramedics.ToDictionary(
                    x => x.ParamedicID,
                    x => x.ParamedicName
                );

            // =========================================
            // LOAD PATIENT YANG DIPAKAI SAJA
            // =========================================
            var patientIDs = collection
                .Where(x => !string.IsNullOrEmpty(x.PatientID))
                .Select(x => x.PatientID)
                .Distinct()
                .ToList();

            var patients = new PatientCollection();

            if (patientIDs.Count > 0)
            {
                patients.Query.Where(
                    patients.Query.PatientID.In(patientIDs)
                );

                patients.Query.Load();
            }

            var patientDict =
                patients.ToDictionary(
                    x => x.PatientID,
                    x => x.FirstName
                );

            // =========================================
            // RESULT
            // =========================================
            return collection
                .Select(x => new
                {
                    x.VisitQueueNo,
                    x.VisitNo,
                    x.RegistrationNo,
                    x.PatientID,
                    FirstName =
                        !string.IsNullOrEmpty(x.PatientID)
                        && patientDict.ContainsKey(x.PatientID)
                            ? patientDict[x.PatientID]
                            : null,
                    x.QueueDate,
                    x.Status,
                    x.QueueSequence,
                    x.StageID,

                    x.ServiceUnitID,
                    ServiceUnitName =
                        !string.IsNullOrEmpty(x.ServiceUnitID)
                        && serviceUnitDict.ContainsKey(x.ServiceUnitID)
                            ? serviceUnitDict[x.ServiceUnitID]
                            : null,

                    x.ParamedicID,

                    ParamedicName =
                        !string.IsNullOrEmpty(x.ParamedicID)
                        && paramedicDict.ContainsKey(x.ParamedicID)
                            ? paramedicDict[x.ParamedicID]
                            : null,
                })
                .ToList();
        }

        public static object MoveQueueDown(
            string visitQueueNo,
            string userID
        )
        {
            // =========================================
            // VALIDASI
            // =========================================
            if (string.IsNullOrWhiteSpace(visitQueueNo))
            {
                throw new Exception(
                    "VisitQueueNo wajib diisi"
                );
            }

            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new Exception(
                    "UserID wajib diisi"
                );
            }

            // =========================================
            // PARAMETERS
            // =========================================
            esParameters prms =
                new esParameters();

            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE PROCEDURE
            // =========================================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianMoveQueueDown",
                prms
            );

            // =========================================
            // RELOAD DATA
            // =========================================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            // =========================================
            // RESULT
            // =========================================
            return new
            {
                VisitQueueNo = queue.VisitQueueNo,
                VisitNo = queue.VisitNo,
                QueueSequence = queue.QueueSequence,
                QueueKey = queue.QueueKey,
                Status = queue.Status,
                StageID = queue.StageID,
                LastUpdated = queue.LastUpdated
            };
        }

        public static object MoveQueueUp(
            string visitQueueNo,
            string userID
        )
        {
            // =========================================
            // VALIDASI
            // =========================================
            if (string.IsNullOrWhiteSpace(visitQueueNo))
            {
                throw new Exception(
                    "VisitQueueNo wajib diisi"
                );
            }

            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new Exception(
                    "UserID wajib diisi"
                );
            }

            // =========================================
            // PARAMETERS
            // =========================================
            esParameters prms =
                new esParameters();

            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE PROCEDURE
            // =========================================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianMoveQueueUp",
                prms
            );

            // =========================================
            // RELOAD DATA
            // =========================================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            // =========================================
            // RESULT
            // =========================================
            return new
            {
                VisitQueueNo = queue.VisitQueueNo,
                VisitNo = queue.VisitNo,
                QueueSequence = queue.QueueSequence,
                QueueKey = queue.QueueKey,
                Status = queue.Status,
                StageID = queue.StageID,
                LastUpdated = queue.LastUpdated
            };
        }

        public static object MoveQueueToTop(
            string visitQueueNo,
            string userID
        )
        {
            // =========================================
            // VALIDASI
            // =========================================
            if (string.IsNullOrWhiteSpace(visitQueueNo))
            {
                throw new Exception(
                    "VisitQueueNo wajib diisi"
                );
            }

            if (string.IsNullOrWhiteSpace(userID))
            {
                throw new Exception(
                    "UserID wajib diisi"
                );
            }

            // =========================================
            // PARAMETERS
            // =========================================
            esParameters prms =
                new esParameters();

            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE PROCEDURE
            // =========================================
            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianMoveQueueToTop",
                prms
            );

            // =========================================
            // RELOAD DATA
            // =========================================
            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            // =========================================
            // RESULT
            // =========================================
            return new
            {
                VisitQueueNo = queue.VisitQueueNo,
                VisitNo = queue.VisitNo,
                QueueSequence = queue.QueueSequence,
                QueueKey = queue.QueueKey,
                Status = queue.Status,
                StageID = queue.StageID,
                LastUpdated = queue.LastUpdated
            };
        }

        public static object MoveQueueToBottom(
            string visitQueueNo,
            string userID
        )
        {
            var prms =
                new esParameters();

            // =========================================
            // INPUT
            // =========================================

            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE PROCEDURE
            // =========================================

            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianMoveQueueToBottom",
                prms
            );

            // =========================================
            // RELOAD DATA
            // =========================================

            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            // =========================================
            // RESULT
            // =========================================

            return new
            {
                queue.VisitQueueNo,
                queue.VisitNo,
                queue.QueueSequence,
                queue.QueueKey,
                queue.Status,
                queue.LastUpdated
            };
        }

        public static object MoveQueueDragDrop(
            string visitQueueNo,
            string targetVisitQueueNo,
            string position,
            string userID
        )
        {
            var prms =
                new esParameters();

            // =========================================
            // INPUT
            // =========================================

            prms.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "TargetVisitQueueNo",
                targetVisitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "Position",
                position,
                esParameterDirection.Input,
                DbType.String,
                10
            );

            prms.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE PROCEDURE
            // =========================================

            var entity =
                new VisitQueue();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianMoveQueueDragDrop",
                prms
            );

            // =========================================
            // RELOAD DATA
            // =========================================

            var queue =
                new VisitQueue();

            queue.LoadByPrimaryKey(
                visitQueueNo
            );

            // =========================================
            // RESULT
            // =========================================

            return new
            {
                queue.VisitQueueNo,
                queue.VisitNo,
                queue.QueueSequence,
                queue.QueueKey,
                queue.StageID,
                queue.Status,
                queue.LastUpdated
            };
        }

        public static object CallAntrianAllServiceUnit(
            string visitQueueNo,
            string userID
        )
        {
            object result = null;

            var entity = new VisitQueue();

            var parameters = new esParameters();

            parameters.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            parameters.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            using (
                var reader =
                    entity.ExecuteReader(
                        esQueryType.StoredProcedure,
                        "AntrianCallNowAllServiceUnit",
                        parameters
                    )
            )
            {
                if (reader.Read())
                {
                    result = new
                    {
                        VisitQueueNo =
                            reader["VisitQueueNo"].ToString(),

                        VisitNo =
                            reader["VisitNo"].ToString(),

                        Status =
                            reader["Status"].ToString(),

                        StageID =
                            reader["StageID"].ToString(),

                        ServiceUnitID =
                            reader["ServiceUnitID"].ToString(),

                        ParamedicID =
                            reader["ParamedicID"] == DBNull.Value
                                ? ""
                                : reader["ParamedicID"].ToString(),

                        CalledTime =
                            reader["CalledTime"] == DBNull.Value
                                ? null
                                : reader["CalledTime"],

                        LastUpdated =
                            reader["LastUpdated"] == DBNull.Value
                                ? null
                                : reader["LastUpdated"],

                        UpdatedBy =
                            reader["UpdatedBy"].ToString()
                    };
                }
            }

            return result;
        }

        public static object RecallAntrianAllServiceUnit(
            string visitQueueNo,
            string userID
        )
        {
            var entity = new VisitQueue();

            var parameters = new esParameters();

            parameters.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            parameters.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            // =========================================
            // EXECUTE SP
            // =========================================
            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "AntrianRecallAllServiceUnit",
                parameters
            );

            // =========================================
            // RELOAD DATA TERBARU
            // =========================================
            var result = new VisitQueue();

            if (!result.LoadByPrimaryKey(visitQueueNo))
            {
                return null;
            }

            return new
            {
                VisitQueueNo = result.VisitQueueNo,
                VisitNo = result.VisitNo,
                Status = result.Status,
                StageID = result.CurrentStage,
                ServiceUnitID = result.ServiceUnitID,
                ParamedicID = result.ParamedicID,
                CalledTime = result.CalledTime,
                LastUpdated = result.LastUpdated,
                UpdatedBy = result.UpdatedBy,
                IsRecall = result.GetColumn("IsRecall")
            };
        }

        public static object SetPendingAllServiceUnit(
            string visitQueueNo,
            string userID
        )
        {
            var entity = new VisitQueue();

            var parameters = new esParameters();

            parameters.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            parameters.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            var reader = entity.ExecuteReader(
                esQueryType.StoredProcedure,
                "AntrianSetPendingAllServiceUnit",
                parameters
            );

            if (reader == null || !reader.Read())
                return null;

            return new
            {
                VisitQueueNo = reader["VisitQueueNo"]?.ToString(),
                VisitNo = reader["VisitNo"]?.ToString(),
                Status = reader["Status"]?.ToString(),
                StageID = reader["StageID"]?.ToString(),
                ServiceUnitID = reader["ServiceUnitID"]?.ToString(),
                ParamedicID = reader["ParamedicID"]?.ToString(),
                CalledTime = reader["CalledTime"] == DBNull.Value ? null : reader["CalledTime"],
                LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : reader["LastUpdated"],
                UpdatedBy = reader["UpdatedBy"]?.ToString()
            };
        }

        public static object SetWaitingFromPendingAllServiceUnit(
            string visitQueueNo,
            string userID
        )
        {
            var entity = new VisitQueue();

            var parameters = new esParameters();

            parameters.Add(
                "VisitQueueNo",
                visitQueueNo,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            parameters.Add(
                "UserID",
                userID,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            using (var reader = entity.ExecuteReader(
                esQueryType.StoredProcedure,
                "AntrianSetWaitingFromPendingAllServiceUnit",
                parameters
            ))
            {
                if (reader == null || !reader.Read())
                    return null;

                return new
                {
                    VisitQueueNo = reader["VisitQueueNo"]?.ToString(),
                    VisitNo = reader["VisitNo"]?.ToString(),
                    Status = reader["Status"]?.ToString(),
                    StageID = reader["StageID"]?.ToString(),
                    ServiceUnitID = reader["ServiceUnitID"]?.ToString(),
                    ParamedicID = reader["ParamedicID"]?.ToString(),
                    QueueSequence = reader["QueueSequence"] == DBNull.Value ? null : reader["QueueSequence"],
                    CalledTime = reader["CalledTime"] == DBNull.Value ? null : reader["CalledTime"],
                    LastUpdated = reader["LastUpdated"] == DBNull.Value ? null : reader["LastUpdated"],
                    UpdatedBy = reader["UpdatedBy"]?.ToString()
                };
            }
        }


    }

    [Serializable]
    abstract public class esVisitQueueQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return VisitQueueMetadata.Meta();
            }
        }

        public esQueryItem VisitQueueNo
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.VisitQueueNo, esSystemType.String);
            }
        }
        public esQueryItem VisitNo
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.VisitNo, esSystemType.String);
            }
        }
        public esQueryItem SRAutoNumber
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.SRAutoNumber, esSystemType.String);
            }
        }
        public esQueryItem RegistrationNo
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.RegistrationNo, esSystemType.String);
            }
        }
        public esQueryItem QueueDate
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.QueueDate, esSystemType.DateTime);
            }
        }
        public esQueryItem Status
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.Status, esSystemType.String);
            }
        }
        public esQueryItem CurrentStage
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CurrentStage, esSystemType.String);
            }
        }
        public esQueryItem CalledByCounterID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CalledByCounterID, esSystemType.String);
            }
        }
        public esQueryItem CalledTime
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CalledTime, esSystemType.DateTime);
            }
        }
        public esQueryItem ServedTime
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.ServedTime, esSystemType.DateTime);
            }
        }
        public esQueryItem FinishedTime
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.FinishedTime, esSystemType.DateTime);
            }
        }
        public esQueryItem PatientID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.PatientID, esSystemType.String);
            }
        }
        public esQueryItem CreatedDate
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CreatedDate, esSystemType.DateTime);
            }
        }
        public esQueryItem CreatedBy
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CreatedBy, esSystemType.String);
            }
        }
        public esQueryItem QueueSequence
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.QueueSequence, esSystemType.Int32);
            }
        }
        public esQueryItem Priority
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.Priority, esSystemType.Int32);
            }
        }
        public esQueryItem IsManualOverride
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.IsManualOverride, esSystemType.Boolean);
            }
        }
        public esQueryItem LastUpdated
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.LastUpdated, esSystemType.DateTime);
            }
        }
        public esQueryItem UpdatedBy
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.UpdatedBy, esSystemType.String);
            }
        }
        public esQueryItem ServiceUnitID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.ServiceUnitID, esSystemType.String);
            }
        }
        public esQueryItem ParamedicID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.ParamedicID, esSystemType.String);
            }
        }
        public esQueryItem StageID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.StageID, esSystemType.String);
            }
        }
        public esQueryItem CategoryID
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.CategoryID, esSystemType.String);
            }
        }
        public esQueryItem QueueKey
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.QueueKey, esSystemType.String);
            }
        }
        public esQueryItem QueueLocation
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.QueueLocation, esSystemType.String);
            }
        }
        public esQueryItem IsRecall
        {
            get
            {
                return new esQueryItem(this, VisitQueueMetadata.ColumnNames.IsRecall, esSystemType.Int32);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("VisitQueueCollection")]
    public partial class VisitQueueCollection : esVisitQueueCollection, IEnumerable<VisitQueue>
    {
        public VisitQueueCollection()
        {

        }

        public static implicit operator List<VisitQueue>(VisitQueueCollection coll)
        {
            List<VisitQueue> list = new List<VisitQueue>();

            foreach (VisitQueue emp in coll)
            {
                list.Add(emp);
            }

            return list;
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return VisitQueueMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new VisitQueueQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new VisitQueue(row);
        }

        override protected esEntity CreateEntity()
        {
            return new VisitQueue();
        }

        #endregion

        [BrowsableAttribute(false)]
        public VisitQueueQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new VisitQueueQuery();
                    base.InitQuery(this.query);
                }

                return this.query;
            }
        }

        /// <summary>
        /// Useful for building up conditional queries.
        /// In most cases, before loading an entity or collection,
        /// you should instantiate a new one. This method was added
        /// to handle specialized circumstances, and should not be
        /// used as a substitute for that.
        /// </summary>
        /// <remarks>
        /// This just sets obj.Query to null/Nothing.
        /// In most cases, you will 'new' your object before
        /// loading it, rather than calling this method.
        /// It only affects obj.Query.Load(), so is not useful
        /// when Joins are involved, or for many other situations.
        /// Because it clears out any obj.Query.Where clauses,
        /// it can be useful for building conditional queries on the fly.
        /// <code>
        /// public bool ReQuery(string lastName, string firstName)
        /// {
        ///     this.QueryReset();
        ///     
        ///     if(!String.IsNullOrEmpty(lastName))
        ///     {
        ///         this.Query.Where(
        ///             this.Query.LastName == lastName);
        ///     }
        ///     if(!String.IsNullOrEmpty(firstName))
        ///     {
        ///         this.Query.Where(
        ///             this.Query.FirstName == firstName);
        ///     }
        ///     
        ///     return this.Query.Load();
        /// }
        /// </code>
        /// <code lang="vbnet">
        /// Public Function ReQuery(ByVal lastName As String, _
        ///     ByVal firstName As String) As Boolean
        /// 
        ///     Me.QueryReset()
        /// 
        ///     If Not [String].IsNullOrEmpty(lastName) Then
        ///         Me.Query.Where(Me.Query.LastName = lastName)
        ///     End If
        ///     If Not [String].IsNullOrEmpty(firstName) Then
        ///         Me.Query.Where(Me.Query.FirstName = firstName)
        ///     End If
        /// 
        ///     Return Me.Query.Load()
        /// End Function
        /// </code>
        /// </remarks>
        public void QueryReset()
        {
            this.query = null;
        }

        /// <summary>
        /// Used to custom load a Join query.
        /// Returns true if at least one record was loaded.
        /// </summary>
        /// <remarks>
        /// Provides support for InnerJoin, LeftJoin,
        /// RightJoin, and FullJoin. You must provide an alias
        /// for each query when instantiating them.
        /// <code>
        /// EmployeeCollection collection = new EmployeeCollection();
        /// 
        /// EmployeeQuery emp = new EmployeeQuery("eq");
        /// CustomerQuery cust = new CustomerQuery("cq");
        /// 
        /// emp.Select(emp.EmployeeID, emp.LastName, cust.CustomerName);
        /// emp.LeftJoin(cust).On(emp.EmployeeID == cust.StaffAssigned);
        /// 
        /// collection.Load(emp);
        /// </code>
        /// <code lang="vbnet">
        /// Dim collection As New EmployeeCollection()
        /// 
        /// Dim emp As New EmployeeQuery("eq")
        /// Dim cust As New CustomerQuery("cq")
        /// 
        /// emp.Select(emp.EmployeeID, emp.LastName, cust.CustomerName)
        /// emp.LeftJoin(cust).On(emp.EmployeeID = cust.StaffAssigned)
        /// 
        /// collection.Load(emp)
        /// </code>
        /// </remarks>
        /// <param name="query">The query object instance name.</param>
        /// <returns>True if at least one record was loaded.</returns>
        public bool Load(VisitQueueQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public VisitQueue AddNew()
        {
            VisitQueue entity = base.AddNewEntity() as VisitQueue;

            return entity;
        }
        public VisitQueue FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as VisitQueue;
        }

        #region IEnumerable< VisitQueue> Members

        IEnumerator<VisitQueue> IEnumerable<VisitQueue>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as VisitQueue;
            }
        }

        #endregion

        private VisitQueueQuery query;
    }


    /// <summary>
    /// Encapsulates the 'VisitQueue' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("VisitQueue ({StandardReferenceID})")]
    [Serializable]
    public partial class VisitQueue : esVisitQueue
    {
        public VisitQueue()
        {
        }

        public VisitQueue(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return VisitQueueMetadata.Meta();
            }
        }

        override protected esVisitQueueQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new VisitQueueQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public VisitQueueQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new VisitQueueQuery();
                    base.InitQuery(this.query);
                }

                return this.query;
            }
        }

        /// <summary>
        /// Useful for building up conditional queries.
        /// In most cases, before loading an entity or collection,
        /// you should instantiate a new one. This method was added
        /// to handle specialized circumstances, and should not be
        /// used as a substitute for that.
        /// </summary>
        /// <remarks>
        /// This just sets obj.Query to null/Nothing.
        /// In most cases, you will 'new' your object before
        /// loading it, rather than calling this method.
        /// It only affects obj.Query.Load(), so is not useful
        /// when Joins are involved, or for many other situations.
        /// Because it clears out any obj.Query.Where clauses,
        /// it can be useful for building conditional queries on the fly.
        /// <code>
        /// public bool ReQuery(string lastName, string firstName)
        /// {
        ///     this.QueryReset();
        ///     
        ///     if(!String.IsNullOrEmpty(lastName))
        ///     {
        ///         this.Query.Where(
        ///             this.Query.LastName == lastName);
        ///     }
        ///     if(!String.IsNullOrEmpty(firstName))
        ///     {
        ///         this.Query.Where(
        ///             this.Query.FirstName == firstName);
        ///     }
        ///     
        ///     return this.Query.Load();
        /// }
        /// </code>
        /// <code lang="vbnet">
        /// Public Function ReQuery(ByVal lastName As String, _
        ///     ByVal firstName As String) As Boolean
        /// 
        ///     Me.QueryReset()
        /// 
        ///     If Not [String].IsNullOrEmpty(lastName) Then
        ///         Me.Query.Where(Me.Query.LastName = lastName)
        ///     End If
        ///     If Not [String].IsNullOrEmpty(firstName) Then
        ///         Me.Query.Where(Me.Query.FirstName = firstName)
        ///     End If
        /// 
        ///     Return Me.Query.Load()
        /// End Function
        /// </code>
        /// </remarks>
        public void QueryReset()
        {
            this.query = null;
        }

        /// <summary>
        /// Used to custom load a Join query.
        /// Returns true if at least one row is loaded.
        /// For an entity, an exception will be thrown
        /// if more than one row is loaded.
        /// </summary>
        /// <remarks>
        /// Provides support for InnerJoin, LeftJoin,
        /// RightJoin, and FullJoin. You must provide an alias
        /// for each query when instantiating them.
        /// <code>
        /// EmployeeCollection collection = new EmployeeCollection();
        /// 
        /// EmployeeQuery emp = new EmployeeQuery("eq");
        /// CustomerQuery cust = new CustomerQuery("cq");
        /// 
        /// emp.Select(emp.EmployeeID, emp.LastName, cust.CustomerName);
        /// emp.LeftJoin(cust).On(emp.EmployeeID == cust.StaffAssigned);
        /// 
        /// collection.Load(emp);
        /// </code>
        /// <code lang="vbnet">
        /// Dim collection As New EmployeeCollection()
        /// 
        /// Dim emp As New EmployeeQuery("eq")
        /// Dim cust As New CustomerQuery("cq")
        /// 
        /// emp.Select(emp.EmployeeID, emp.LastName, cust.CustomerName)
        /// emp.LeftJoin(cust).On(emp.EmployeeID = cust.StaffAssigned)
        /// 
        /// collection.Load(emp)
        /// </code>
        /// </remarks>
        /// <param name="query">The query object instance name.</param>
        /// <returns>True if at least one record was loaded.</returns>
        public bool Load(VisitQueueQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private VisitQueueQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class VisitQueueQuery : esVisitQueueQuery
    {
        public VisitQueueQuery()
        {

        }

        public VisitQueueQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "VisitQueueQuery";
        }
    }

    [Serializable]
    public partial class VisitQueueMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected VisitQueueMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.VisitQueueNo, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.VisitQueueNo;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.VisitNo, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.VisitNo;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.SRAutoNumber, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.SRAutoNumber;
            c.CharacterMaxLength = 100;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.RegistrationNo, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.RegistrationNo;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.QueueDate, 4, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.QueueDate;
            c.HasDefault = true;
            c.Default = @"(CONVERT([date],getdate()))";
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.Status, 5, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.Status;
            c.CharacterMaxLength = 20;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CurrentStage, 6, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CurrentStage;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CalledByCounterID, 7, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CalledByCounterID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CalledTime, 8, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CalledTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.ServedTime, 9, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.ServedTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.FinishedTime, 10, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.FinishedTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.PatientID, 11, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.PatientID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CreatedDate, 12, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CreatedDate;
            c.HasDefault = true;
            c.Default = @"(getdate())";
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CreatedBy, 13, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CreatedBy;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.QueueSequence, 14, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = VisitQueueMetadata.PropertyNames.QueueSequence;
            c.NumericPrecision = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.Priority, 15, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = VisitQueueMetadata.PropertyNames.Priority;
            c.NumericPrecision = 10;
            c.HasDefault = true;
            c.Default = @"((100))";
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.IsManualOverride, 16, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = VisitQueueMetadata.PropertyNames.IsManualOverride;
            c.HasDefault = true;
            c.Default = @"((0))";
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.LastUpdated, 17, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueMetadata.PropertyNames.LastUpdated;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.UpdatedBy, 18, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.UpdatedBy;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.ServiceUnitID, 19, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.ServiceUnitID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.ParamedicID, 20, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.ParamedicID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.StageID, 21, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.StageID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.CategoryID, 22, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.CategoryID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.QueueKey, 23, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.QueueKey;
            c.CharacterMaxLength = 200;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.QueueLocation, 24, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueMetadata.PropertyNames.QueueLocation;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueMetadata.ColumnNames.IsRecall, 25, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = VisitQueueMetadata.PropertyNames.IsRecall;
            c.NumericPrecision = 10;
            c.HasDefault = true;
            c.Default = @"((0))";
            _columns.Add(c);
        }
        #endregion

        static public VisitQueueMetadata Meta()
        {
            return meta;
        }

        public Guid DataID
        {
            get { return base._dataID; }
        }

        public bool MultiProviderMode
        {
            get { return false; }
        }

        public esColumnMetadataCollection Columns
        {
            get { return base._columns; }
        }

        #region ColumnNames
        public class ColumnNames
        {
            public const string VisitQueueNo = "VisitQueueNo";
            public const string VisitNo = "VisitNo";
            public const string SRAutoNumber = "SRAutoNumber";
            public const string RegistrationNo = "RegistrationNo";
            public const string QueueDate = "QueueDate";
            public const string Status = "Status";
            public const string CurrentStage = "CurrentStage";
            public const string CalledByCounterID = "CalledByCounterID";
            public const string CalledTime = "CalledTime";
            public const string ServedTime = "ServedTime";
            public const string FinishedTime = "FinishedTime";
            public const string PatientID = "PatientID";
            public const string CreatedDate = "CreatedDate";
            public const string CreatedBy = "CreatedBy";
            public const string QueueSequence = "QueueSequence";
            public const string Priority = "Priority";
            public const string IsManualOverride = "IsManualOverride";
            public const string LastUpdated = "LastUpdated";
            public const string UpdatedBy = "UpdatedBy";
            public const string ServiceUnitID = "ServiceUnitID";
            public const string ParamedicID = "ParamedicID";
            public const string StageID = "StageID";
            public const string CategoryID = "CategoryID";
            public const string QueueKey = "QueueKey";
            public const string QueueLocation = "QueueLocation";
            public const string IsRecall = "IsRecall";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string VisitQueueNo = "VisitQueueNo";
            public const string VisitNo = "VisitNo";
            public const string SRAutoNumber = "SRAutoNumber";
            public const string RegistrationNo = "RegistrationNo";
            public const string QueueDate = "QueueDate";
            public const string Status = "Status";
            public const string CurrentStage = "CurrentStage";
            public const string CalledByCounterID = "CalledByCounterID";
            public const string CalledTime = "CalledTime";
            public const string ServedTime = "ServedTime";
            public const string FinishedTime = "FinishedTime";
            public const string PatientID = "PatientID";
            public const string CreatedDate = "CreatedDate";
            public const string CreatedBy = "CreatedBy";
            public const string QueueSequence = "QueueSequence";
            public const string Priority = "Priority";
            public const string IsManualOverride = "IsManualOverride";
            public const string LastUpdated = "LastUpdated";
            public const string UpdatedBy = "UpdatedBy";
            public const string ServiceUnitID = "ServiceUnitID";
            public const string ParamedicID = "ParamedicID";
            public const string StageID = "StageID";
            public const string CategoryID = "CategoryID";
            public const string QueueKey = "QueueKey";
            public const string QueueLocation = "QueueLocation";
            public const string IsRecall = "IsRecall";
        }
        #endregion

        public esProviderSpecificMetadata GetProviderMetadata(string mapName)
        {
            MapToMeta mapMethod = mapDelegates[mapName];

            if (mapMethod != null)
                return mapMethod(mapName);
            else
                return null;
        }

        #region MAP esDefault

        static private int RegisterDelegateesDefault()
        {
            // This is only executed once per the life of the application
            lock (typeof(VisitQueueMetadata))
            {
                if (VisitQueueMetadata.mapDelegates == null)
                {
                    VisitQueueMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (VisitQueueMetadata.meta == null)
                {
                    VisitQueueMetadata.meta = new VisitQueueMetadata();
                }

                MapToMeta mapMethod = new MapToMeta(meta.esDefault);
                mapDelegates.Add("esDefault", mapMethod);
                mapMethod("esDefault");
            }
            return 0;
        }

        private esProviderSpecificMetadata esDefault(string mapName)
        {
            if (!_providerMetadataMaps.ContainsKey(mapName))
            {
                esProviderSpecificMetadata meta = new esProviderSpecificMetadata();

                meta.AddTypeMap("VisitQueueNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("VisitNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("SRAutoNumber", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("RegistrationNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("QueueDate", new esTypeMap("date", "System.DateTime"));
                meta.AddTypeMap("Status", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CurrentStage", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CalledByCounterID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CalledTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("ServedTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("FinishedTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("PatientID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CreatedDate", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("CreatedBy", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("QueueSequence", new esTypeMap("int", "System.Int32"));
                meta.AddTypeMap("Priority", new esTypeMap("int", "System.Int32"));
                meta.AddTypeMap("IsManualOverride", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("LastUpdated", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("UpdatedBy", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceUnitID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ParamedicID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("StageID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CategoryID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("QueueKey", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("QueueLocation", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsRecall", new esTypeMap("int", "System.Int32"));


                meta.Source = "VisitQueue";
                meta.Destination = "VisitQueue";
                meta.spInsert = "proc_VisitQueueInsert";
                meta.spUpdate = "proc_VisitQueueUpdate";
                meta.spDelete = "proc_VisitQueueDelete";
                meta.spLoadAll = "proc_VisitQueueLoadAll";
                meta.spLoadByPrimaryKey = "proc_VisitQueueLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private VisitQueueMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}