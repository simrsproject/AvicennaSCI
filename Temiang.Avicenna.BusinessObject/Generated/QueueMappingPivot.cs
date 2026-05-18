/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-05-15 09:16:39 AM
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
    abstract public class esQueueMappingPivotCollection : esEntityCollectionWAuditLog
    {
        public esQueueMappingPivotCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "QueueMappingPivotCollection";
        }

        #region Query Logic
        protected void InitQuery(esQueueMappingPivotQuery query)
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
            this.InitQuery(query as esQueueMappingPivotQuery);
        }
        #endregion

        virtual public QueueMappingPivot DetachEntity(QueueMappingPivot entity)
        {
            return base.DetachEntity(entity) as QueueMappingPivot;
        }

        virtual public QueueMappingPivot AttachEntity(QueueMappingPivot entity)
        {
            return base.AttachEntity(entity) as QueueMappingPivot;
        }

        virtual public void Combine(QueueMappingPivotCollection collection)
        {
            base.Combine(collection);
        }

        new public QueueMappingPivot this[int index]
        {
            get
            {
                return base[index] as QueueMappingPivot;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(QueueMappingPivot);
        }
    }

    [Serializable]
    abstract public class esQueueMappingPivot : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esQueueMappingPivotQuery GetDynamicQuery()
        {
            return null;
        }

        public esQueueMappingPivot()
        {
        }

        public esQueueMappingPivot(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
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
                        case "ServiceUnitID": this.str.ServiceUnitID = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {

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
        /// Maps to QueueMappingPivot.StageID
        /// </summary>
        virtual public System.String StageID
        {
            get
            {
                return base.GetSystemString(QueueMappingPivotMetadata.ColumnNames.StageID);
            }

            set
            {
                base.SetSystemString(QueueMappingPivotMetadata.ColumnNames.StageID, value);
            }
        }
        /// <summary>
        /// Maps to QueueMappingPivot.ServiceUnitID
        /// </summary>
        virtual public System.String ServiceUnitID
        {
            get
            {
                return base.GetSystemString(QueueMappingPivotMetadata.ColumnNames.ServiceUnitID);
            }

            set
            {
                base.SetSystemString(QueueMappingPivotMetadata.ColumnNames.ServiceUnitID, value);
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
            public esStrings(esQueueMappingPivot entity)
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

            private esQueueMappingPivot entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esQueueMappingPivotQuery query)
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
                throw new Exception("esQueueMappingPivot can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class QueueMappingPivot : esQueueMappingPivot
    {
    }

    [Serializable]
    abstract public class esQueueMappingPivotQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return QueueMappingPivotMetadata.Meta();
            }
        }

        public esQueryItem StageID
        {
            get
            {
                return new esQueryItem(this, QueueMappingPivotMetadata.ColumnNames.StageID, esSystemType.String);
            }
        }
        public esQueryItem ServiceUnitID
        {
            get
            {
                return new esQueryItem(this, QueueMappingPivotMetadata.ColumnNames.ServiceUnitID, esSystemType.String);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("QueueMappingPivotCollection")]
    public partial class QueueMappingPivotCollection : esQueueMappingPivotCollection, IEnumerable<QueueMappingPivot>
    {
        public QueueMappingPivotCollection()
        {

        }

        public static implicit operator List<QueueMappingPivot>(QueueMappingPivotCollection coll)
        {
            List<QueueMappingPivot> list = new List<QueueMappingPivot>();

            foreach (QueueMappingPivot emp in coll)
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
                return QueueMappingPivotMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueMappingPivotQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new QueueMappingPivot(row);
        }

        override protected esEntity CreateEntity()
        {
            return new QueueMappingPivot();
        }

        #endregion

        [BrowsableAttribute(false)]
        public QueueMappingPivotQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueMappingPivotQuery();
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
        public bool Load(QueueMappingPivotQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public QueueMappingPivot AddNew()
        {
            QueueMappingPivot entity = base.AddNewEntity() as QueueMappingPivot;

            return entity;
        }
        public QueueMappingPivot FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as QueueMappingPivot;
        }

        #region IEnumerable< QueueMappingPivot> Members

        IEnumerator<QueueMappingPivot> IEnumerable<QueueMappingPivot>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as QueueMappingPivot;
            }
        }

        #endregion

        private QueueMappingPivotQuery query;
    }


    /// <summary>
    /// Encapsulates the 'QueueMappingPivot' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("QueueMappingPivot ({StandardReferenceID})")]
    [Serializable]
    public partial class QueueMappingPivot : esQueueMappingPivot
    {
        public QueueMappingPivot()
        {
        }

        public QueueMappingPivot(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return QueueMappingPivotMetadata.Meta();
            }
        }

        override protected esQueueMappingPivotQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new QueueMappingPivotQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public QueueMappingPivotQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new QueueMappingPivotQuery();
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
        public bool Load(QueueMappingPivotQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private QueueMappingPivotQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class QueueMappingPivotQuery : esQueueMappingPivotQuery
    {
        public QueueMappingPivotQuery()
        {

        }

        public QueueMappingPivotQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "QueueMappingPivotQuery";
        }
    }

    [Serializable]
    public partial class QueueMappingPivotMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected QueueMappingPivotMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(QueueMappingPivotMetadata.ColumnNames.StageID, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueMappingPivotMetadata.PropertyNames.StageID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(QueueMappingPivotMetadata.ColumnNames.ServiceUnitID, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = QueueMappingPivotMetadata.PropertyNames.ServiceUnitID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public QueueMappingPivotMetadata Meta()
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
            public const string ServiceUnitID = "ServiceUnitID";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string StageID = "StageID";
            public const string ServiceUnitID = "ServiceUnitID";
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
            lock (typeof(QueueMappingPivotMetadata))
            {
                if (QueueMappingPivotMetadata.mapDelegates == null)
                {
                    QueueMappingPivotMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (QueueMappingPivotMetadata.meta == null)
                {
                    QueueMappingPivotMetadata.meta = new QueueMappingPivotMetadata();
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
                meta.AddTypeMap("ServiceUnitID", new esTypeMap("varchar", "System.String"));


                meta.Source = "QueueMappingPivot";
                meta.Destination = "QueueMappingPivot";
                meta.spInsert = "proc_QueueMappingPivotInsert";
                meta.spUpdate = "proc_QueueMappingPivotUpdate";
                meta.spDelete = "proc_QueueMappingPivotDelete";
                meta.spLoadAll = "proc_QueueMappingPivotLoadAll";
                meta.spLoadByPrimaryKey = "proc_QueueMappingPivotLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private QueueMappingPivotMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}