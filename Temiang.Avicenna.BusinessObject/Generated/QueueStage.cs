/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-05-05 08:43:08 AM
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
    abstract public class esQueueStageCollection : esEntityCollectionWAuditLog
    {
        public esQueueStageCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "QueueStageCollection";
        }

        #region Query Logic
        protected void InitQuery(esQueueStageQuery query)
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
            this.InitQuery(query as esQueueStageQuery);
        }
        #endregion

        virtual public QueueStage DetachEntity(QueueStage entity)
        {
            return base.DetachEntity(entity) as QueueStage;
        }

        virtual public QueueStage AttachEntity(QueueStage entity)
        {
            return base.AttachEntity(entity) as QueueStage;
        }

        virtual public void Combine(QueueStageCollection collection)
        {
            base.Combine(collection);
        }

        new public QueueStage this[int index]
        {
            get
            {
                return base[index] as QueueStage;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(QueueStage);
        }
    }

    [Serializable]
    abstract public class esQueueStage : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esQueueStageQuery GetDynamicQuery()
        {
            return null;
        }

        public esQueueStage()
        {
        }

        public esQueueStage(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(String stageID)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(stageID);
            else
                return LoadByPrimaryKeyStoredProcedure(stageID);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String stageID)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(stageID);
            else
                return LoadByPrimaryKeyStoredProcedure(stageID);
        }

        private bool LoadByPrimaryKeyDynamic(String stageID)
        {
            esQueueStageQuery query = this.GetDynamicQuery();
            query.Where(query.StageID == stageID);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(String stageID)
        {
            esParameters parms = new esParameters();
            parms.Add("StageID", stageID);
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
                        case "StageID": this.str.StageID = (string)value; break;
                        case "StageName": this.str.StageName = (string)value; break;
                        case "ServiceGroup": this.str.ServiceGroup = (string)value; break;
                        case "StepOrder": this.str.StepOrder = (string)value; break;
                        case "IsQueue": this.str.IsQueue = (string)value; break;
                        case "IsActive": this.str.IsActive = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "StepOrder":

                            if (value == null || value is System.Int32)
                                this.StepOrder = (System.Int32?)value;
                            break;
                        case "IsQueue":

                            if (value == null || value is System.Boolean)
                                this.IsQueue = (System.Boolean?)value;
                            break;
                        case "IsActive":

                            if (value == null || value is System.Boolean)
                                this.IsActive = (System.Boolean?)value;
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
        /// Maps to QueueStage.StageID
        /// </summary>
        virtual public System.String StageID
        {
            get
            {
                return base.GetSystemString(QueueStageMetadata.ColumnNames.StageID);
            }

            set
            {
                base.SetSystemString(QueueStageMetadata.ColumnNames.StageID, value);
            }
        }
        /// <summary>
        /// Maps to QueueStage.StageName
        /// </summary>
        virtual public System.String StageName
        {
            get
            {
                return base.GetSystemString(QueueStageMetadata.ColumnNames.StageName);
            }

            set
            {
                base.SetSystemString(QueueStageMetadata.ColumnNames.StageName, value);
            }
        }
        /// <summary>
        /// Maps to QueueStage.ServiceGroup
        /// </summary>
        virtual public System.String ServiceGroup
        {
            get
            {
                return base.GetSystemString(QueueStageMetadata.ColumnNames.ServiceGroup);
            }

            set
            {
                base.SetSystemString(QueueStageMetadata.ColumnNames.ServiceGroup, value);
            }
        }
        /// <summary>
        /// Maps to QueueStage.StepOrder
        /// </summary>
        virtual public System.Int32? StepOrder
        {
            get
            {
                return base.GetSystemInt32(QueueStageMetadata.ColumnNames.StepOrder);
            }

            set
            {
                base.SetSystemInt32(QueueStageMetadata.ColumnNames.StepOrder, value);
            }
        }
        /// <summary>
        /// Maps to QueueStage.IsQueue
        /// </summary>
        virtual public System.Boolean? IsQueue
        {
            get
            {
                return base.GetSystemBoolean(QueueStageMetadata.ColumnNames.IsQueue);
            }

            set
            {
                base.SetSystemBoolean(QueueStageMetadata.ColumnNames.IsQueue, value);
            }
        }
        /// <summary>
        /// Maps to QueueStage.IsActive
        /// </summary>
        virtual public System.Boolean? IsActive
        {
            get
            {
                return base.GetSystemBoolean(QueueStageMetadata.ColumnNames.IsActive);
            }

            set
            {
                base.SetSystemBoolean(QueueStageMetadata.ColumnNames.IsActive, value);
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
            public esStrings(esQueueStage entity)
            {
                this.entity = entity;
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
            public System.String StageName
            {
                get
                {
                    System.String data = entity.StageName;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.StageName = null;
                    else entity.StageName = Convert.ToString(value);
                }
            }
            public System.String ServiceGroup
            {
                get
                {
                    System.String data = entity.ServiceGroup;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ServiceGroup = null;
                    else entity.ServiceGroup = Convert.ToString(value);
                }
            }
            public System.String StepOrder
            {
                get
                {
                    System.Int32? data = entity.StepOrder;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.StepOrder = null;
                    else entity.StepOrder = Convert.ToInt32(value);
                }
            }
            public System.String IsQueue
            {
                get
                {
                    System.Boolean? data = entity.IsQueue;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsQueue = null;
                    else entity.IsQueue = Convert.ToBoolean(value);
                }
            }
            public System.String IsActive
            {
                get
                {
                    System.Boolean? data = entity.IsActive;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsActive = null;
                    else entity.IsActive = Convert.ToBoolean(value);
                }
            }

            private esQueueStage entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esQueueStageQuery query)
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
                throw new Exception("esQueueStage can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class QueueStage : esQueueStage
    {
        public static List<object> GetQueueStage(
     string stageID,
     string serviceGroup,
     string isActive
 )
        {
            var result = new List<object>();

            var entity = new QueueStage();

            var parameters = new esParameters();

            string sql = @"
                SELECT
                    StageID,
                    StageName,
                    ServiceGroup,
                    StepOrder,
                    IsQueue,
                    IsActive
                FROM QueueStage
                WHERE 1 = 1
            ";

                    if (!string.IsNullOrEmpty(stageID))
                    {
                        sql += " AND StageID = @StageID";

                        parameters.Add(
                            "StageID",
                            stageID,
                            esParameterDirection.Input,
                            DbType.String,
                            50
                        );
                    }

                    if (!string.IsNullOrEmpty(serviceGroup))
                    {
                        sql += " AND ServiceGroup = @ServiceGroup";

                        parameters.Add(
                            "ServiceGroup",
                            serviceGroup,
                            esParameterDirection.Input,
                            DbType.String,
                            50
                        );
                    }

                    if (!string.IsNullOrEmpty(isActive))
                    {
                        sql += " AND CAST(IsActive AS VARCHAR(1)) = @IsActive";

                        parameters.Add(
                            "IsActive",
                            isActive,
                            esParameterDirection.Input,
                            DbType.String,
                            5
                        );
                    }

                    sql += @"
                ORDER BY
                    ServiceGroup,
                    StepOrder,
                    StageName
            ";

            using (
                var reader =
                    entity.ExecuteReader(
                        esQueryType.Text,
                        sql,
                        parameters
                    )
            )
            {
                while (reader.Read())
                {
                    result.Add(new
                    {
                        StageID = reader["StageID"].ToString(),
                        StageName = reader["StageName"].ToString(),
                        ServiceGroup = reader["ServiceGroup"].ToString(),
                        StepOrder = Convert.ToInt32(reader["StepOrder"]),
                        IsQueue = Convert.ToBoolean(reader["IsQueue"]),
                        IsActive = Convert.ToBoolean(reader["IsActive"])
                    });
                }
            }

            return result;
        }
    }

    [Serializable]
    abstract public class esQueueStageQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return QueueStageMetadata.Meta();
            }
        }

        public esQueryItem StageID
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.StageID, esSystemType.String);
            }
        }
        public esQueryItem StageName
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.StageName, esSystemType.String);
            }
        }
        public esQueryItem ServiceGroup
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.ServiceGroup, esSystemType.String);
            }
        }
        public esQueryItem StepOrder
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.StepOrder, esSystemType.Int32);
            }
        }
        public esQueryItem IsQueue
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.IsQueue, esSystemType.Boolean);
            }
        }
        public esQueryItem IsActive
        {
            get
            {
                return new esQueryItem(this, QueueStageMetadata.ColumnNames.IsActive, esSystemType.Boolean);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("QueueStageCollection")]
    public partial class QueueStageCollection : esQueueStageCollection, IEnumerable<QueueStage>
    {
        public QueueStageCollection()
        {

        }

        public static implicit operator List<QueueStage>(QueueStageCollection coll)
        {
            List<QueueStage> list = new List<QueueStage>();

            foreach (QueueStage emp in coll)
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
                return QueueStageMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueStageQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new QueueStage(row);
        }

        override protected esEntity CreateEntity()
        {
            return new QueueStage();
        }

        #endregion

        [BrowsableAttribute(false)]
        public QueueStageQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueStageQuery();
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
        public bool Load(QueueStageQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public QueueStage AddNew()
        {
            QueueStage entity = base.AddNewEntity() as QueueStage;

            return entity;
        }
        public QueueStage FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as QueueStage;
        }

        #region IEnumerable< QueueStage> Members

        IEnumerator<QueueStage> IEnumerable<QueueStage>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as QueueStage;
            }
        }

        #endregion

        private QueueStageQuery query;
    }


    /// <summary>
    /// Encapsulates the 'QueueStage' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("QueueStage ({StandardReferenceID})")]
    [Serializable]
    public partial class QueueStage : esQueueStage
    {
        public QueueStage()
        {
        }

        public QueueStage(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return QueueStageMetadata.Meta();
            }
        }

        override protected esQueueStageQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueStageQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public QueueStageQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueStageQuery();
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
        public bool Load(QueueStageQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private QueueStageQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class QueueStageQuery : esQueueStageQuery
    {
        public QueueStageQuery()
        {

        }

        public QueueStageQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "QueueStageQuery";
        }
    }

    [Serializable]
    public partial class QueueStageMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected QueueStageMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.StageID, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueStageMetadata.PropertyNames.StageID;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.StageName, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueStageMetadata.PropertyNames.StageName;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.ServiceGroup, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueStageMetadata.PropertyNames.ServiceGroup;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.StepOrder, 3, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = QueueStageMetadata.PropertyNames.StepOrder;
            c.NumericPrecision = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.IsQueue, 4, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = QueueStageMetadata.PropertyNames.IsQueue;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueStageMetadata.ColumnNames.IsActive, 5, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = QueueStageMetadata.PropertyNames.IsActive;
            c.HasDefault = true;
            c.Default = @"((1))";
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public QueueStageMetadata Meta()
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
            public const string StageID = "StageID";
            public const string StageName = "StageName";
            public const string ServiceGroup = "ServiceGroup";
            public const string StepOrder = "StepOrder";
            public const string IsQueue = "IsQueue";
            public const string IsActive = "IsActive";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string StageID = "StageID";
            public const string StageName = "StageName";
            public const string ServiceGroup = "ServiceGroup";
            public const string StepOrder = "StepOrder";
            public const string IsQueue = "IsQueue";
            public const string IsActive = "IsActive";
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
            lock (typeof(QueueStageMetadata))
            {
                if (QueueStageMetadata.mapDelegates == null)
                {
                    QueueStageMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (QueueStageMetadata.meta == null)
                {
                    QueueStageMetadata.meta = new QueueStageMetadata();
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

                meta.AddTypeMap("StageID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("StageName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceGroup", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("StepOrder", new esTypeMap("int", "System.Int32"));
                meta.AddTypeMap("IsQueue", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));


                meta.Source = "QueueStage";
                meta.Destination = "QueueStage";
                meta.spInsert = "proc_QueueStageInsert";
                meta.spUpdate = "proc_QueueStageUpdate";
                meta.spDelete = "proc_QueueStageDelete";
                meta.spLoadAll = "proc_QueueStageLoadAll";
                meta.spLoadByPrimaryKey = "proc_QueueStageLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private QueueStageMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}