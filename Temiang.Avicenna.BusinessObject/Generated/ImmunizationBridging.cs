/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 17/10/2025 00:24:27
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
    abstract public class esImmunizationBridgingCollection : esEntityCollectionWAuditLog
    {
        public esImmunizationBridgingCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "ImmunizationBridgingCollection";
        }

        #region Query Logic
        protected void InitQuery(esImmunizationBridgingQuery query)
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
            this.InitQuery(query as esImmunizationBridgingQuery);
        }
        #endregion

        virtual public ImmunizationBridging DetachEntity(ImmunizationBridging entity)
        {
            return base.DetachEntity(entity) as ImmunizationBridging;
        }

        virtual public ImmunizationBridging AttachEntity(ImmunizationBridging entity)
        {
            return base.AttachEntity(entity) as ImmunizationBridging;
        }

        virtual public void Combine(ImmunizationBridgingCollection collection)
        {
            base.Combine(collection);
        }

        new public ImmunizationBridging this[int index]
        {
            get
            {
                return base[index] as ImmunizationBridging;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(ImmunizationBridging);
        }
    }

    [Serializable]
    abstract public class esImmunizationBridging : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esImmunizationBridgingQuery GetDynamicQuery()
        {
            return null;
        }

        public esImmunizationBridging()
        {
        }

        public esImmunizationBridging(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(String immunizationID, String sRBridgingType, String bridgingID)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(immunizationID, sRBridgingType, bridgingID);
            else
                return LoadByPrimaryKeyStoredProcedure(immunizationID, sRBridgingType, bridgingID);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String immunizationID, String sRBridgingType, String bridgingID)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(immunizationID, sRBridgingType, bridgingID);
            else
                return LoadByPrimaryKeyStoredProcedure(immunizationID, sRBridgingType, bridgingID);
        }

        private bool LoadByPrimaryKeyDynamic(String immunizationID, String sRBridgingType, String bridgingID)
        {
            esImmunizationBridgingQuery query = this.GetDynamicQuery();
            query.Where(query.ImmunizationID == immunizationID, query.SRBridgingType == sRBridgingType, query.BridgingID == bridgingID);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(String immunizationID, String sRBridgingType, String bridgingID)
        {
            esParameters parms = new esParameters();
            parms.Add("ImmunizationID", immunizationID);
            parms.Add("SRBridgingType", sRBridgingType);
            parms.Add("BridgingID", bridgingID);
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
                        case "ImmunizationID": this.str.ImmunizationID = (string)value; break;
                        case "SRBridgingType": this.str.SRBridgingType = (string)value; break;
                        case "BridgingID": this.str.BridgingID = (string)value; break;
                        case "BridgingName": this.str.BridgingName = (string)value; break;
                        case "IsActive": this.str.IsActive = (string)value; break;
                        case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
                        case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
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
                        case "LastUpdateDateTime":

                            if (value == null || value is System.DateTime)
                                this.LastUpdateDateTime = (System.DateTime?)value;
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
        /// Maps to ImmunizationBridging.ImmunizationID
        /// </summary>
        virtual public System.String ImmunizationID
        {
            get
            {
                return base.GetSystemString(ImmunizationBridgingMetadata.ColumnNames.ImmunizationID);
            }

            set
            {
                base.SetSystemString(ImmunizationBridgingMetadata.ColumnNames.ImmunizationID, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.SRBridgingType
        /// </summary>
        virtual public System.String SRBridgingType
        {
            get
            {
                return base.GetSystemString(ImmunizationBridgingMetadata.ColumnNames.SRBridgingType);
            }

            set
            {
                base.SetSystemString(ImmunizationBridgingMetadata.ColumnNames.SRBridgingType, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.BridgingID
        /// </summary>
        virtual public System.String BridgingID
        {
            get
            {
                return base.GetSystemString(ImmunizationBridgingMetadata.ColumnNames.BridgingID);
            }

            set
            {
                base.SetSystemString(ImmunizationBridgingMetadata.ColumnNames.BridgingID, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.BridgingName
        /// </summary>
        virtual public System.String BridgingName
        {
            get
            {
                return base.GetSystemString(ImmunizationBridgingMetadata.ColumnNames.BridgingName);
            }

            set
            {
                base.SetSystemString(ImmunizationBridgingMetadata.ColumnNames.BridgingName, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.IsActive
        /// </summary>
        virtual public System.Boolean? IsActive
        {
            get
            {
                return base.GetSystemBoolean(ImmunizationBridgingMetadata.ColumnNames.IsActive);
            }

            set
            {
                base.SetSystemBoolean(ImmunizationBridgingMetadata.ColumnNames.IsActive, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.LastUpdateDateTime
        /// </summary>
        virtual public System.DateTime? LastUpdateDateTime
        {
            get
            {
                return base.GetSystemDateTime(ImmunizationBridgingMetadata.ColumnNames.LastUpdateDateTime);
            }

            set
            {
                base.SetSystemDateTime(ImmunizationBridgingMetadata.ColumnNames.LastUpdateDateTime, value);
            }
        }
        /// <summary>
        /// Maps to ImmunizationBridging.LastUpdateByUserID
        /// </summary>
        virtual public System.String LastUpdateByUserID
        {
            get
            {
                return base.GetSystemString(ImmunizationBridgingMetadata.ColumnNames.LastUpdateByUserID);
            }

            set
            {
                base.SetSystemString(ImmunizationBridgingMetadata.ColumnNames.LastUpdateByUserID, value);
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
            public esStrings(esImmunizationBridging entity)
            {
                this.entity = entity;
            }
            public System.String ImmunizationID
            {
                get
                {
                    System.String data = entity.ImmunizationID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ImmunizationID = null;
                    else entity.ImmunizationID = Convert.ToString(value);
                }
            }
            public System.String SRBridgingType
            {
                get
                {
                    System.String data = entity.SRBridgingType;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.SRBridgingType = null;
                    else entity.SRBridgingType = Convert.ToString(value);
                }
            }
            public System.String BridgingID
            {
                get
                {
                    System.String data = entity.BridgingID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.BridgingID = null;
                    else entity.BridgingID = Convert.ToString(value);
                }
            }
            public System.String BridgingName
            {
                get
                {
                    System.String data = entity.BridgingName;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.BridgingName = null;
                    else entity.BridgingName = Convert.ToString(value);
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
            private esImmunizationBridging entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esImmunizationBridgingQuery query)
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
                throw new Exception("esImmunizationBridging can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class ImmunizationBridging : esImmunizationBridging
    {
    }

    [Serializable]
    abstract public class esImmunizationBridgingQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return ImmunizationBridgingMetadata.Meta();
            }
        }

        public esQueryItem ImmunizationID
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.ImmunizationID, esSystemType.String);
            }
        }

        public esQueryItem SRBridgingType
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.SRBridgingType, esSystemType.String);
            }
        }

        public esQueryItem BridgingID
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.BridgingID, esSystemType.String);
            }
        }

        public esQueryItem BridgingName
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.BridgingName, esSystemType.String);
            }
        }

        public esQueryItem IsActive
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.IsActive, esSystemType.Boolean);
            }
        }

        public esQueryItem LastUpdateDateTime
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
            }
        }

        public esQueryItem LastUpdateByUserID
        {
            get
            {
                return new esQueryItem(this, ImmunizationBridgingMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
            }
        }

    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("ImmunizationBridgingCollection")]
    public partial class ImmunizationBridgingCollection : esImmunizationBridgingCollection, IEnumerable<ImmunizationBridging>
    {
        public ImmunizationBridgingCollection()
        {

        }

        public static implicit operator List<ImmunizationBridging>(ImmunizationBridgingCollection coll)
        {
            List<ImmunizationBridging> list = new List<ImmunizationBridging>();

            foreach (ImmunizationBridging emp in coll)
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
                return ImmunizationBridgingMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new ImmunizationBridgingQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new ImmunizationBridging(row);
        }

        override protected esEntity CreateEntity()
        {
            return new ImmunizationBridging();
        }

        #endregion

        [BrowsableAttribute(false)]
        public ImmunizationBridgingQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new ImmunizationBridgingQuery();
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
        public bool Load(ImmunizationBridgingQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public ImmunizationBridging AddNew()
        {
            ImmunizationBridging entity = base.AddNewEntity() as ImmunizationBridging;

            return entity;
        }
        public ImmunizationBridging FindByPrimaryKey(String immunizationID, String sRBridgingType, String bridgingID)
        {
            return base.FindByPrimaryKey(immunizationID, sRBridgingType, bridgingID) as ImmunizationBridging;
        }

        #region IEnumerable< ImmunizationBridging> Members

        IEnumerator<ImmunizationBridging> IEnumerable<ImmunizationBridging>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as ImmunizationBridging;
            }
        }

        #endregion

        private ImmunizationBridgingQuery query;
    }


    /// <summary>
    /// Encapsulates the 'ImmunizationBridging' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ImmunizationBridging ({ImmunizationID, SRBridgingType, BridgingID})")]
    [Serializable]
    public partial class ImmunizationBridging : esImmunizationBridging
    {
        public ImmunizationBridging()
        {
        }

        public ImmunizationBridging(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return ImmunizationBridgingMetadata.Meta();
            }
        }

        override protected esImmunizationBridgingQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new ImmunizationBridgingQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public ImmunizationBridgingQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new ImmunizationBridgingQuery();
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
        public bool Load(ImmunizationBridgingQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private ImmunizationBridgingQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class ImmunizationBridgingQuery : esImmunizationBridgingQuery
    {
        public ImmunizationBridgingQuery()
        {

        }

        public ImmunizationBridgingQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "ImmunizationBridgingQuery";
        }
    }

    [Serializable]
    public partial class ImmunizationBridgingMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected ImmunizationBridgingMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.ImmunizationID, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.ImmunizationID;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 3;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.SRBridgingType, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.SRBridgingType;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 20;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.BridgingID, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.BridgingID;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 20;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.BridgingName, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.BridgingName;
            c.CharacterMaxLength = 255;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.IsActive, 4, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.IsActive;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.LastUpdateDateTime, 5, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.LastUpdateDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(ImmunizationBridgingMetadata.ColumnNames.LastUpdateByUserID, 6, typeof(System.String), esSystemType.String);
            c.PropertyName = ImmunizationBridgingMetadata.PropertyNames.LastUpdateByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);


        }
        #endregion

        static public ImmunizationBridgingMetadata Meta()
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
            public const string ImmunizationID = "ImmunizationID";
            public const string SRBridgingType = "SRBridgingType";
            public const string BridgingID = "BridgingID";
            public const string BridgingName = "BridgingName";
            public const string IsActive = "IsActive";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string ImmunizationID = "ImmunizationID";
            public const string SRBridgingType = "SRBridgingType";
            public const string BridgingID = "BridgingID";
            public const string BridgingName = "BridgingName";
            public const string IsActive = "IsActive";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
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
            lock (typeof(ImmunizationBridgingMetadata))
            {
                if (ImmunizationBridgingMetadata.mapDelegates == null)
                {
                    ImmunizationBridgingMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (ImmunizationBridgingMetadata.meta == null)
                {
                    ImmunizationBridgingMetadata.meta = new ImmunizationBridgingMetadata();
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

                meta.AddTypeMap("ImmunizationID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("SRBridgingType", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("BridgingID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("BridgingName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));


                meta.Source = "ImmunizationBridging";
                meta.Destination = "ImmunizationBridging";
                meta.spInsert = "proc_ImmunizationBridgingInsert";
                meta.spUpdate = "proc_ImmunizationBridgingUpdate";
                meta.spDelete = "proc_ImmunizationBridgingDelete";
                meta.spLoadAll = "proc_ImmunizationBridgingLoadAll";
                meta.spLoadByPrimaryKey = "proc_ImmunizationBridgingLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private ImmunizationBridgingMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}
