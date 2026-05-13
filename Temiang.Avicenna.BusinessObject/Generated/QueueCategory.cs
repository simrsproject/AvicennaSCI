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
    abstract public class esQueueCategoryCollection : esEntityCollectionWAuditLog
    {
        public esQueueCategoryCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "QueueCategoryCollection";
        }

        #region Query Logic
        protected void InitQuery(esQueueCategoryQuery query)
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
            this.InitQuery(query as esQueueCategoryQuery);
        }
        #endregion

        virtual public QueueCategory DetachEntity(QueueCategory entity)
        {
            return base.DetachEntity(entity) as QueueCategory;
        }

        virtual public QueueCategory AttachEntity(QueueCategory entity)
        {
            return base.AttachEntity(entity) as QueueCategory;
        }

        virtual public void Combine(QueueCategoryCollection collection)
        {
            base.Combine(collection);
        }

        new public QueueCategory this[int index]
        {
            get
            {
                return base[index] as QueueCategory;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(QueueCategory);
        }
    }

    [Serializable]
    abstract public class esQueueCategory : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esQueueCategoryQuery GetDynamicQuery()
        {
            return null;
        }

        public esQueueCategory()
        {
        }

        public esQueueCategory(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(String categoryID)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(categoryID);
            else
                return LoadByPrimaryKeyStoredProcedure(categoryID);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String categoryID)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(categoryID);
            else
                return LoadByPrimaryKeyStoredProcedure(categoryID);
        }

        private bool LoadByPrimaryKeyDynamic(String categoryID)
        {
            esQueueCategoryQuery query = this.GetDynamicQuery();
            query.Where(query.CategoryID == categoryID);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(String categoryID)
        {
            esParameters parms = new esParameters();
            parms.Add("CategoryID", categoryID);
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
                        case "CategoryID": this.str.CategoryID = (string)value; break;
                        case "CategoryName": this.str.CategoryName = (string)value; break;
                        case "StageID": this.str.StageID = (string)value; break;
                        case "ServiceUnitID": this.str.ServiceUnitID = (string)value; break;
                        case "IsActive": this.str.IsActive = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
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
        /// Maps to QueueCategory.CategoryID
        /// </summary>
        virtual public System.String CategoryID
        {
            get
            {
                return base.GetSystemString(QueueCategoryMetadata.ColumnNames.CategoryID);
            }

            set
            {
                base.SetSystemString(QueueCategoryMetadata.ColumnNames.CategoryID, value);
            }
        }
        /// <summary>
        /// Maps to QueueCategory.CategoryName
        /// </summary>
        virtual public System.String CategoryName
        {
            get
            {
                return base.GetSystemString(QueueCategoryMetadata.ColumnNames.CategoryName);
            }

            set
            {
                base.SetSystemString(QueueCategoryMetadata.ColumnNames.CategoryName, value);
            }
        }
        /// <summary>
        /// Maps to QueueCategory.StageID
        /// </summary>
        virtual public System.String StageID
        {
            get
            {
                return base.GetSystemString(QueueCategoryMetadata.ColumnNames.StageID);
            }

            set
            {
                base.SetSystemString(QueueCategoryMetadata.ColumnNames.StageID, value);
            }
        }
        /// <summary>
        /// Maps to QueueCategory.ServiceUnitID
        /// </summary>
        virtual public System.String ServiceUnitID
        {
            get
            {
                return base.GetSystemString(QueueCategoryMetadata.ColumnNames.ServiceUnitID);
            }

            set
            {
                base.SetSystemString(QueueCategoryMetadata.ColumnNames.ServiceUnitID, value);
            }
        }
        /// <summary>
        /// Maps to QueueCategory.IsActive
        /// </summary>
        virtual public System.Boolean? IsActive
        {
            get
            {
                return base.GetSystemBoolean(QueueCategoryMetadata.ColumnNames.IsActive);
            }

            set
            {
                base.SetSystemBoolean(QueueCategoryMetadata.ColumnNames.IsActive, value);
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
            public esStrings(esQueueCategory entity)
            {
                this.entity = entity;
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
            public System.String CategoryName
            {
                get
                {
                    System.String data = entity.CategoryName;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CategoryName = null;
                    else entity.CategoryName = Convert.ToString(value);
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

            private esQueueCategory entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esQueueCategoryQuery query)
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
                throw new Exception("esQueueCategory can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class QueueCategory : esQueueCategory
    {
    }

    [Serializable]
    abstract public class esQueueCategoryQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return QueueCategoryMetadata.Meta();
            }
        }

        public esQueryItem CategoryID
        {
            get
            {
                return new esQueryItem(this, QueueCategoryMetadata.ColumnNames.CategoryID, esSystemType.String);
            }
        }
        public esQueryItem CategoryName
        {
            get
            {
                return new esQueryItem(this, QueueCategoryMetadata.ColumnNames.CategoryName, esSystemType.String);
            }
        }
        public esQueryItem StageID
        {
            get
            {
                return new esQueryItem(this, QueueCategoryMetadata.ColumnNames.StageID, esSystemType.String);
            }
        }
        public esQueryItem ServiceUnitID
        {
            get
            {
                return new esQueryItem(this, QueueCategoryMetadata.ColumnNames.ServiceUnitID, esSystemType.String);
            }
        }
        public esQueryItem IsActive
        {
            get
            {
                return new esQueryItem(this, QueueCategoryMetadata.ColumnNames.IsActive, esSystemType.Boolean);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("QueueCategoryCollection")]
    public partial class QueueCategoryCollection : esQueueCategoryCollection, IEnumerable<QueueCategory>
    {
        public QueueCategoryCollection()
        {

        }

        public static implicit operator List<QueueCategory>(QueueCategoryCollection coll)
        {
            List<QueueCategory> list = new List<QueueCategory>();

            foreach (QueueCategory emp in coll)
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
                return QueueCategoryMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueCategoryQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new QueueCategory(row);
        }

        override protected esEntity CreateEntity()
        {
            return new QueueCategory();
        }

        #endregion

        [BrowsableAttribute(false)]
        public QueueCategoryQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueCategoryQuery();
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
        public bool Load(QueueCategoryQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public QueueCategory AddNew()
        {
            QueueCategory entity = base.AddNewEntity() as QueueCategory;

            return entity;
        }
        public QueueCategory FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as QueueCategory;
        }

        #region IEnumerable< QueueCategory> Members

        IEnumerator<QueueCategory> IEnumerable<QueueCategory>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as QueueCategory;
            }
        }

        #endregion

        private QueueCategoryQuery query;
    }


    /// <summary>
    /// Encapsulates the 'QueueCategory' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("QueueCategory ({StandardReferenceID})")]
    [Serializable]
    public partial class QueueCategory : esQueueCategory
    {
        public QueueCategory()
        {
        }

        public QueueCategory(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return QueueCategoryMetadata.Meta();
            }
        }

        override protected esQueueCategoryQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueCategoryQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public QueueCategoryQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueCategoryQuery();
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
        public bool Load(QueueCategoryQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private QueueCategoryQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class QueueCategoryQuery : esQueueCategoryQuery
    {
        public QueueCategoryQuery()
        {

        }

        public QueueCategoryQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "QueueCategoryQuery";
        }
    }

    [Serializable]
    public partial class QueueCategoryMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected QueueCategoryMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(QueueCategoryMetadata.ColumnNames.CategoryID, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueCategoryMetadata.PropertyNames.CategoryID;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(QueueCategoryMetadata.ColumnNames.CategoryName, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueCategoryMetadata.PropertyNames.CategoryName;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueCategoryMetadata.ColumnNames.StageID, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueCategoryMetadata.PropertyNames.StageID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueCategoryMetadata.ColumnNames.ServiceUnitID, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueCategoryMetadata.PropertyNames.ServiceUnitID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueCategoryMetadata.ColumnNames.IsActive, 4, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = QueueCategoryMetadata.PropertyNames.IsActive;
            c.HasDefault = true;
            c.Default = @"((1))";
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public QueueCategoryMetadata Meta()
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
            public const string CategoryID = "CategoryID";
            public const string CategoryName = "CategoryName";
            public const string StageID = "StageID";
            public const string ServiceUnitID = "ServiceUnitID";
            public const string IsActive = "IsActive";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string CategoryID = "CategoryID";
            public const string CategoryName = "CategoryName";
            public const string StageID = "StageID";
            public const string ServiceUnitID = "ServiceUnitID";
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
            lock (typeof(QueueCategoryMetadata))
            {
                if (QueueCategoryMetadata.mapDelegates == null)
                {
                    QueueCategoryMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (QueueCategoryMetadata.meta == null)
                {
                    QueueCategoryMetadata.meta = new QueueCategoryMetadata();
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

                meta.AddTypeMap("CategoryID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CategoryName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("StageID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceUnitID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));


                meta.Source = "QueueCategory";
                meta.Destination = "QueueCategory";
                meta.spInsert = "proc_QueueCategoryInsert";
                meta.spUpdate = "proc_QueueCategoryUpdate";
                meta.spDelete = "proc_QueueCategoryDelete";
                meta.spLoadAll = "proc_QueueCategoryLoadAll";
                meta.spLoadByPrimaryKey = "proc_QueueCategoryLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private QueueCategoryMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}