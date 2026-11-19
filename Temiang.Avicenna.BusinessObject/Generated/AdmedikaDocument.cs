/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-18 09:18:07 AM
===============================================================================
				Author: Wiliam Decosta (wiliamdecosta@gmail.com) - YBRS
===============================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;
using Temiang.Dal.DynamicQuery;

namespace Temiang.Avicenna.BusinessObject
{
    [Serializable]
    abstract public class esAdmedikaDocumentCollection : esEntityCollectionWAuditLog
    {
        public esAdmedikaDocumentCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "AdmedikaDocumentCollection";
        }

        #region Query Logic
        protected void InitQuery(esAdmedikaDocumentQuery query)
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
            this.InitQuery(query as esAdmedikaDocumentQuery);
        }
        #endregion

        virtual public AdmedikaDocument DetachEntity(AdmedikaDocument entity)
        {
            return base.DetachEntity(entity) as AdmedikaDocument;
        }

        virtual public AdmedikaDocument AttachEntity(AdmedikaDocument entity)
        {
            return base.AttachEntity(entity) as AdmedikaDocument;
        }

        virtual public void Combine(AdmedikaDocumentCollection collection)
        {
            base.Combine(collection);
        }

        new public AdmedikaDocument this[int index]
        {
            get
            {
                return base[index] as AdmedikaDocument;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(AdmedikaDocument);
        }
    }

    [Serializable]
    abstract public class esAdmedikaDocument : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esAdmedikaDocumentQuery GetDynamicQuery()
        {
            return null;
        }

        public esAdmedikaDocument()
        {
        }

        public esAdmedikaDocument(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(Int64 admedikaDocumentID)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(admedikaDocumentID);
            else
                return LoadByPrimaryKeyStoredProcedure(admedikaDocumentID);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 admedikaDocumentID)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(admedikaDocumentID);
            else
                return LoadByPrimaryKeyStoredProcedure(admedikaDocumentID);
        }

        private bool LoadByPrimaryKeyDynamic(Int64 admedikaDocumentID)
        {
            esAdmedikaDocumentQuery query = this.GetDynamicQuery();
            query.Where(query.AdmedikaDocumentID == admedikaDocumentID);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(Int64 admedikaDocumentID)
        {
            esParameters parms = new esParameters();
            parms.Add("AdmedikaDocumentID", admedikaDocumentID);
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
                        case "AdmedikaDocumentID": this.str.AdmedikaDocumentID = (string)value; break;
                        case "AdmedikaClaimTransactionID": this.str.AdmedikaClaimTransactionID = (string)value; break;
                        case "DocType": this.str.DocType = (string)value; break;
                        case "FileName": this.str.FileName = (string)value; break;
                        case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
                        case "CreatedByUserID": this.str.CreatedByUserID = (string)value; break;
                        case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
                        case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
                        case "IsSentSuccess": this.str.IsSentSuccess = (string)value; break;
                        case "SentDateTime": this.str.SentDateTime = (string)value; break;
                        case "ResponseMessage": this.str.ResponseMessage = (string)value; break;
                        case "SentByUserID": this.str.SentByUserID = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "AdmedikaDocumentID":

                            if (value == null || value is System.Int64)
                                this.AdmedikaDocumentID = (System.Int64?)value;
                            break;
                        case "AdmedikaClaimTransactionID":

                            if (value == null || value is System.Int64)
                                this.AdmedikaClaimTransactionID = (System.Int64?)value;
                            break;
                        case "CreatedDateTime":

                            if (value == null || value is System.DateTime)
                                this.CreatedDateTime = (System.DateTime?)value;
                            break;
                        case "LastUpdateDateTime":

                            if (value == null || value is System.DateTime)
                                this.LastUpdateDateTime = (System.DateTime?)value;
                            break;
                        case "IsSentSuccess":

                            if (value == null || value is System.Boolean)
                                this.IsSentSuccess = (System.Boolean?)value;
                            break;
                        case "SentDateTime":

                            if (value == null || value is System.DateTime)
                                this.SentDateTime = (System.DateTime?)value;
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
        /// Maps to AdmedikaDocument.AdmedikaDocumentID
        /// </summary>
        virtual public System.Int64? AdmedikaDocumentID
        {
            get
            {
                return base.GetSystemInt64(AdmedikaDocumentMetadata.ColumnNames.AdmedikaDocumentID);
            }

            set
            {
                base.SetSystemInt64(AdmedikaDocumentMetadata.ColumnNames.AdmedikaDocumentID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.AdmedikaClaimTransactionID
        /// </summary>
        virtual public System.Int64? AdmedikaClaimTransactionID
        {
            get
            {
                return base.GetSystemInt64(AdmedikaDocumentMetadata.ColumnNames.AdmedikaClaimTransactionID);
            }

            set
            {
                base.SetSystemInt64(AdmedikaDocumentMetadata.ColumnNames.AdmedikaClaimTransactionID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.DocType
        /// </summary>
        virtual public System.String DocType
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.DocType);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.DocType, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.FileName
        /// </summary>
        virtual public System.String FileName
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.FileName);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.FileName, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.CreatedDateTime
        /// </summary>
        virtual public System.DateTime? CreatedDateTime
        {
            get
            {
                return base.GetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.CreatedDateTime);
            }

            set
            {
                base.SetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.CreatedDateTime, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.CreatedByUserID
        /// </summary>
        virtual public System.String CreatedByUserID
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.CreatedByUserID);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.CreatedByUserID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.LastUpdateDateTime
        /// </summary>
        virtual public System.DateTime? LastUpdateDateTime
        {
            get
            {
                return base.GetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.LastUpdateDateTime);
            }

            set
            {
                base.SetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.LastUpdateDateTime, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.LastUpdateByUserID
        /// </summary>
        virtual public System.String LastUpdateByUserID
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.LastUpdateByUserID);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.LastUpdateByUserID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.IsSentSuccess
        /// </summary>
        virtual public System.Boolean? IsSentSuccess
        {
            get
            {
                return base.GetSystemBoolean(AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess);
            }

            set
            {
                base.SetSystemBoolean(AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.SentDateTime
        /// </summary>
        virtual public System.DateTime? SentDateTime
        {
            get
            {
                return base.GetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.SentDateTime);
            }

            set
            {
                base.SetSystemDateTime(AdmedikaDocumentMetadata.ColumnNames.SentDateTime, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.ResponseMessage
        /// </summary>
        virtual public System.String ResponseMessage
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.ResponseMessage);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.ResponseMessage, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaDocument.SentByUserID
        /// </summary>
        virtual public System.String SentByUserID
        {
            get
            {
                return base.GetSystemString(AdmedikaDocumentMetadata.ColumnNames.SentByUserID);
            }

            set
            {
                base.SetSystemString(AdmedikaDocumentMetadata.ColumnNames.SentByUserID, value);
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
            public esStrings(esAdmedikaDocument entity)
            {
                this.entity = entity;
            }
            public System.String AdmedikaDocumentID
            {
                get
                {
                    System.Int64? data = entity.AdmedikaDocumentID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.AdmedikaDocumentID = null;
                    else entity.AdmedikaDocumentID = Convert.ToInt64(value);
                }
            }
            public System.String AdmedikaClaimTransactionID
            {
                get
                {
                    System.Int64? data = entity.AdmedikaClaimTransactionID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.AdmedikaClaimTransactionID = null;
                    else entity.AdmedikaClaimTransactionID = Convert.ToInt64(value);
                }
            }
            public System.String DocType
            {
                get
                {
                    System.String data = entity.DocType;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DocType = null;
                    else entity.DocType = Convert.ToString(value);
                }
            }
            public System.String FileName
            {
                get
                {
                    System.String data = entity.FileName;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.FileName = null;
                    else entity.FileName = Convert.ToString(value);
                }
            }
            public System.String CreatedDateTime
            {
                get
                {
                    System.DateTime? data = entity.CreatedDateTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CreatedDateTime = null;
                    else entity.CreatedDateTime = Convert.ToDateTime(value);
                }
            }
            public System.String CreatedByUserID
            {
                get
                {
                    System.String data = entity.CreatedByUserID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CreatedByUserID = null;
                    else entity.CreatedByUserID = Convert.ToString(value);
                }
            }
            public System.String LastUpdateDateTime
            {
                get
                {
                    System.DateTime? data = entity.LastUpdateDateTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.LastUpdateDateTime = null;
                    else entity.LastUpdateDateTime = Convert.ToDateTime(value);
                }
            }
            public System.String LastUpdateByUserID
            {
                get
                {
                    System.String data = entity.LastUpdateByUserID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.LastUpdateByUserID = null;
                    else entity.LastUpdateByUserID = Convert.ToString(value);
                }
            }
            public System.String IsSentSuccess
            {
                get
                {
                    System.Boolean? data = entity.IsSentSuccess;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsSentSuccess = null;
                    else entity.IsSentSuccess = Convert.ToBoolean(value);
                }
            }
            public System.String SentDateTime
            {
                get
                {
                    System.DateTime? data = entity.SentDateTime;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.SentDateTime = null;
                    else entity.SentDateTime = Convert.ToDateTime(value);
                }
            }
            public System.String ResponseMessage
            {
                get
                {
                    System.String data = entity.ResponseMessage;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ResponseMessage = null;
                    else entity.ResponseMessage = Convert.ToString(value);
                }
            }
            public System.String SentByUserID
            {
                get
                {
                    System.String data = entity.SentByUserID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.SentByUserID = null;
                    else entity.SentByUserID = Convert.ToString(value);
                }
            }

            private esAdmedikaDocument entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esAdmedikaDocumentQuery query)
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
                throw new Exception("esAdmedikaDocument can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class AdmedikaDocument : esAdmedikaDocument
    {
    }

    [Serializable]
    abstract public class esAdmedikaDocumentQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return AdmedikaDocumentMetadata.Meta();
            }
        }

        public esQueryItem AdmedikaDocumentID
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.AdmedikaDocumentID, esSystemType.Int64);
            }
        }
        public esQueryItem AdmedikaClaimTransactionID
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.AdmedikaClaimTransactionID, esSystemType.Int64);
            }
        }
        public esQueryItem DocType
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.DocType, esSystemType.String);
            }
        }
        public esQueryItem FileName
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.FileName, esSystemType.String);
            }
        }
        public esQueryItem CreatedDateTime
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
            }
        }
        public esQueryItem CreatedByUserID
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
            }
        }
        public esQueryItem LastUpdateDateTime
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
            }
        }
        public esQueryItem LastUpdateByUserID
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
            }
        }
        public esQueryItem IsSentSuccess
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess, esSystemType.Boolean);
            }
        }
        public esQueryItem SentDateTime
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.SentDateTime, esSystemType.DateTime);
            }
        }
        public esQueryItem ResponseMessage
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.ResponseMessage, esSystemType.String);
            }
        }
        public esQueryItem SentByUserID
        {
            get
            {
                return new esQueryItem(this, AdmedikaDocumentMetadata.ColumnNames.SentByUserID, esSystemType.String);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("AdmedikaDocumentCollection")]
    public partial class AdmedikaDocumentCollection : esAdmedikaDocumentCollection, IEnumerable<AdmedikaDocument>
    {
        public AdmedikaDocumentCollection()
        {

        }

        public static implicit operator List<AdmedikaDocument>(AdmedikaDocumentCollection coll)
        {
            List<AdmedikaDocument> list = new List<AdmedikaDocument>();

            foreach (AdmedikaDocument emp in coll)
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
                return AdmedikaDocumentMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AdmedikaDocumentQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new AdmedikaDocument(row);
        }

        override protected esEntity CreateEntity()
        {
            return new AdmedikaDocument();
        }

        #endregion

        [BrowsableAttribute(false)]
        public AdmedikaDocumentQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AdmedikaDocumentQuery();
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
        public bool Load(AdmedikaDocumentQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public AdmedikaDocument AddNew()
        {
            AdmedikaDocument entity = base.AddNewEntity() as AdmedikaDocument;

            return entity;
        }
        public AdmedikaDocument FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as AdmedikaDocument;
        }

        #region IEnumerable< AdmedikaDocument> Members

        IEnumerator<AdmedikaDocument> IEnumerable<AdmedikaDocument>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as AdmedikaDocument;
            }
        }

        #endregion

        private AdmedikaDocumentQuery query;
    }


    /// <summary>
    /// Encapsulates the 'AdmedikaDocument' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("AdmedikaDocument ({StandardReferenceID})")]
    [Serializable]
    public partial class AdmedikaDocument : esAdmedikaDocument
    {
        public AdmedikaDocument()
        {
        }

        public AdmedikaDocument(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return AdmedikaDocumentMetadata.Meta();
            }
        }

        override protected esAdmedikaDocumentQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AdmedikaDocumentQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public AdmedikaDocumentQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AdmedikaDocumentQuery();
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
        public bool Load(AdmedikaDocumentQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private AdmedikaDocumentQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class AdmedikaDocumentQuery : esAdmedikaDocumentQuery
    {
        public AdmedikaDocumentQuery()
        {

        }

        public AdmedikaDocumentQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "AdmedikaDocumentQuery";
        }
    }

    [Serializable]
    public partial class AdmedikaDocumentMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected AdmedikaDocumentMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.AdmedikaDocumentID, 0, typeof(System.Int64), esSystemType.Int64);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.AdmedikaDocumentID;
            c.IsInPrimaryKey = true;
            c.IsAutoIncrement = true;
            c.NumericPrecision = 19;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.AdmedikaClaimTransactionID, 1, typeof(System.Int64), esSystemType.Int64);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.AdmedikaClaimTransactionID;
            c.NumericPrecision = 19;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.DocType, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.DocType;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.FileName, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.FileName;
            c.CharacterMaxLength = 500;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.CreatedDateTime, 4, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.CreatedDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.CreatedByUserID, 5, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.CreatedByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.LastUpdateDateTime, 6, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.LastUpdateDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.LastUpdateByUserID, 7, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.LastUpdateByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.IsSentSuccess, 8, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.IsSentSuccess;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.SentDateTime, 9, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.SentDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.ResponseMessage, 10, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.ResponseMessage;
            c.CharacterMaxLength = 500;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaDocumentMetadata.ColumnNames.SentByUserID, 11, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaDocumentMetadata.PropertyNames.SentByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public AdmedikaDocumentMetadata Meta()
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
            public const string AdmedikaDocumentID = "AdmedikaDocumentID";
            public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
            public const string DocType = "DocType";
            public const string FileName = "FileName";
            public const string CreatedDateTime = "CreatedDateTime";
            public const string CreatedByUserID = "CreatedByUserID";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
            public const string IsSentSuccess = "IsSentSuccess";
            public const string SentDateTime = "SentDateTime";
            public const string ResponseMessage = "ResponseMessage";
            public const string SentByUserID = "SentByUserID";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string AdmedikaDocumentID = "AdmedikaDocumentID";
            public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
            public const string DocType = "DocType";
            public const string FileName = "FileName";
            public const string CreatedDateTime = "CreatedDateTime";
            public const string CreatedByUserID = "CreatedByUserID";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
            public const string IsSentSuccess = "IsSentSuccess";
            public const string SentDateTime = "SentDateTime";
            public const string ResponseMessage = "ResponseMessage";
            public const string SentByUserID = "SentByUserID";
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
            lock (typeof(AdmedikaDocumentMetadata))
            {
                if (AdmedikaDocumentMetadata.mapDelegates == null)
                {
                    AdmedikaDocumentMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (AdmedikaDocumentMetadata.meta == null)
                {
                    AdmedikaDocumentMetadata.meta = new AdmedikaDocumentMetadata();
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

                meta.AddTypeMap("AdmedikaDocumentID", new esTypeMap("bigint", "System.Int64"));
                meta.AddTypeMap("AdmedikaClaimTransactionID", new esTypeMap("bigint", "System.Int64"));
                meta.AddTypeMap("DocType", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("FileName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsSentSuccess", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("SentDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("ResponseMessage", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("SentByUserID", new esTypeMap("varchar", "System.String"));


                meta.Source = "AdmedikaDocument";
                meta.Destination = "AdmedikaDocument";
                meta.spInsert = "proc_AdmedikaDocumentInsert";
                meta.spUpdate = "proc_AdmedikaDocumentUpdate";
                meta.spDelete = "proc_AdmedikaDocumentDelete";
                meta.spLoadAll = "proc_AdmedikaDocumentLoadAll";
                meta.spLoadByPrimaryKey = "proc_AdmedikaDocumentLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private AdmedikaDocumentMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}