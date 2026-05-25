/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-05-25 10:02:22 AM
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
    abstract public class esVisitQueueBarcodeCollection : esEntityCollectionWAuditLog
    {
        public esVisitQueueBarcodeCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "VisitQueueBarcodeCollection";
        }

        #region Query Logic
        protected void InitQuery(esVisitQueueBarcodeQuery query)
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
            this.InitQuery(query as esVisitQueueBarcodeQuery);
        }
        #endregion

        virtual public VisitQueueBarcode DetachEntity(VisitQueueBarcode entity)
        {
            return base.DetachEntity(entity) as VisitQueueBarcode;
        }

        virtual public VisitQueueBarcode AttachEntity(VisitQueueBarcode entity)
        {
            return base.AttachEntity(entity) as VisitQueueBarcode;
        }

        virtual public void Combine(VisitQueueBarcodeCollection collection)
        {
            base.Combine(collection);
        }

        new public VisitQueueBarcode this[int index]
        {
            get
            {
                return base[index] as VisitQueueBarcode;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(VisitQueueBarcode);
        }
    }

    [Serializable]
    abstract public class esVisitQueueBarcode : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esVisitQueueBarcodeQuery GetDynamicQuery()
        {
            return null;
        }

        public esVisitQueueBarcode()
        {
        }

        public esVisitQueueBarcode(DataRow row)
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
                        case "VisitQueueNo": this.str.VisitQueueNo = (string)value; break;
                        case "BarcodeImage": this.str.BarcodeImage = (string)value; break;
                        case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "BarcodeImage":

                            if (value == null || value is System.Byte[])
                                this.BarcodeImage = (System.Byte[])value;
                            break;
                        case "CreatedDateTime":

                            if (value == null || value is System.DateTime)
                                this.CreatedDateTime = (System.DateTime?)value;
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
        /// Maps to VisitQueueBarcode.VisitQueueNo
        /// </summary>
        virtual public System.String VisitQueueNo
        {
            get
            {
                return base.GetSystemString(VisitQueueBarcodeMetadata.ColumnNames.VisitQueueNo);
            }

            set
            {
                base.SetSystemString(VisitQueueBarcodeMetadata.ColumnNames.VisitQueueNo, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueueBarcode.BarcodeImage
        /// </summary>
        virtual public System.Byte[] BarcodeImage
        {
            get
            {
                return base.GetSystemByteArray(VisitQueueBarcodeMetadata.ColumnNames.BarcodeImage);
            }

            set
            {
                base.SetSystemByteArray(VisitQueueBarcodeMetadata.ColumnNames.BarcodeImage, value);
            }
        }
        /// <summary>
        /// Maps to VisitQueueBarcode.CreatedDateTime
        /// </summary>
        virtual public System.DateTime? CreatedDateTime
        {
            get
            {
                return base.GetSystemDateTime(VisitQueueBarcodeMetadata.ColumnNames.CreatedDateTime);
            }

            set
            {
                base.SetSystemDateTime(VisitQueueBarcodeMetadata.ColumnNames.CreatedDateTime, value);
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
            public esStrings(esVisitQueueBarcode entity)
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
            public System.String BarcodeImage
            {
                get
                {
                    System.Byte[] data = entity.BarcodeImage;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.BarcodeImage = null;
                    else entity.BarcodeImage = Convert.FromBase64String(value);
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

            private esVisitQueueBarcode entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esVisitQueueBarcodeQuery query)
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
                throw new Exception("esVisitQueueBarcode can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class VisitQueueBarcode : esVisitQueueBarcode
    {
    }

    [Serializable]
    abstract public class esVisitQueueBarcodeQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return VisitQueueBarcodeMetadata.Meta();
            }
        }

        public esQueryItem VisitQueueNo
        {
            get
            {
                return new esQueryItem(this, VisitQueueBarcodeMetadata.ColumnNames.VisitQueueNo, esSystemType.String);
            }
        }
        public esQueryItem BarcodeImage
        {
            get
            {
                return new esQueryItem(this, VisitQueueBarcodeMetadata.ColumnNames.BarcodeImage, esSystemType.ByteArray);
            }
        }
        public esQueryItem CreatedDateTime
        {
            get
            {
                return new esQueryItem(this, VisitQueueBarcodeMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("VisitQueueBarcodeCollection")]
    public partial class VisitQueueBarcodeCollection : esVisitQueueBarcodeCollection, IEnumerable<VisitQueueBarcode>
    {
        public VisitQueueBarcodeCollection()
        {

        }

        public static implicit operator List<VisitQueueBarcode>(VisitQueueBarcodeCollection coll)
        {
            List<VisitQueueBarcode> list = new List<VisitQueueBarcode>();

            foreach (VisitQueueBarcode emp in coll)
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
                return VisitQueueBarcodeMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new VisitQueueBarcodeQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new VisitQueueBarcode(row);
        }

        override protected esEntity CreateEntity()
        {
            return new VisitQueueBarcode();
        }

        #endregion

        [BrowsableAttribute(false)]
        public VisitQueueBarcodeQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new VisitQueueBarcodeQuery();
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
        public bool Load(VisitQueueBarcodeQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public VisitQueueBarcode AddNew()
        {
            VisitQueueBarcode entity = base.AddNewEntity() as VisitQueueBarcode;

            return entity;
        }
        public VisitQueueBarcode FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as VisitQueueBarcode;
        }

        #region IEnumerable< VisitQueueBarcode> Members

        IEnumerator<VisitQueueBarcode> IEnumerable<VisitQueueBarcode>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as VisitQueueBarcode;
            }
        }

        #endregion

        private VisitQueueBarcodeQuery query;
    }


    /// <summary>
    /// Encapsulates the 'VisitQueueBarcode' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("VisitQueueBarcode ({StandardReferenceID})")]
    [Serializable]
    public partial class VisitQueueBarcode : esVisitQueueBarcode
    {
        public VisitQueueBarcode()
        {
        }

        public VisitQueueBarcode(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return VisitQueueBarcodeMetadata.Meta();
            }
        }

        override protected esVisitQueueBarcodeQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new VisitQueueBarcodeQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public VisitQueueBarcodeQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new VisitQueueBarcodeQuery();
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
        public bool Load(VisitQueueBarcodeQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private VisitQueueBarcodeQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class VisitQueueBarcodeQuery : esVisitQueueBarcodeQuery
    {
        public VisitQueueBarcodeQuery()
        {

        }

        public VisitQueueBarcodeQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "VisitQueueBarcodeQuery";
        }
    }

    [Serializable]
    public partial class VisitQueueBarcodeMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected VisitQueueBarcodeMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(VisitQueueBarcodeMetadata.ColumnNames.VisitQueueNo, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = VisitQueueBarcodeMetadata.PropertyNames.VisitQueueNo;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueBarcodeMetadata.ColumnNames.BarcodeImage, 1, typeof(System.Byte[]), esSystemType.ByteArray);
            c.PropertyName = VisitQueueBarcodeMetadata.PropertyNames.BarcodeImage;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(VisitQueueBarcodeMetadata.ColumnNames.CreatedDateTime, 2, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = VisitQueueBarcodeMetadata.PropertyNames.CreatedDateTime;
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public VisitQueueBarcodeMetadata Meta()
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
            public const string BarcodeImage = "BarcodeImage";
            public const string CreatedDateTime = "CreatedDateTime";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string VisitQueueNo = "VisitQueueNo";
            public const string BarcodeImage = "BarcodeImage";
            public const string CreatedDateTime = "CreatedDateTime";
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
            lock (typeof(VisitQueueBarcodeMetadata))
            {
                if (VisitQueueBarcodeMetadata.mapDelegates == null)
                {
                    VisitQueueBarcodeMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (VisitQueueBarcodeMetadata.meta == null)
                {
                    VisitQueueBarcodeMetadata.meta = new VisitQueueBarcodeMetadata();
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
                meta.AddTypeMap("BarcodeImage", new esTypeMap("varbinary", "System.Byte[]"));
                meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));


                meta.Source = "VisitQueueBarcode";
                meta.Destination = "VisitQueueBarcode";
                meta.spInsert = "proc_VisitQueueBarcodeInsert";
                meta.spUpdate = "proc_VisitQueueBarcodeUpdate";
                meta.spDelete = "proc_VisitQueueBarcodeDelete";
                meta.spLoadAll = "proc_VisitQueueBarcodeLoadAll";
                meta.spLoadByPrimaryKey = "proc_VisitQueueBarcodeLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private VisitQueueBarcodeMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}