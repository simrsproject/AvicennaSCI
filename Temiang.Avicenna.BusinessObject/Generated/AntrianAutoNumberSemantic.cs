/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-05-05 08:43:07 AM
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
using Temiang.Dal.Core;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.BusinessObject.Generated
{
    [Serializable]
    abstract public class esAntrianAutoNumberSemanticCollection : esEntityCollectionWAuditLog
    {
        public esAntrianAutoNumberSemanticCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "AntrianAutoNumberSemanticCollection";
        }

        #region Query Logic
        protected void InitQuery(esAntrianAutoNumberSemanticQuery query)
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
            this.InitQuery(query as esAntrianAutoNumberSemanticQuery);
        }
        #endregion

        virtual public AntrianAutoNumberSemantic DetachEntity(AntrianAutoNumberSemantic entity)
        {
            return base.DetachEntity(entity) as AntrianAutoNumberSemantic;
        }

        virtual public AntrianAutoNumberSemantic AttachEntity(AntrianAutoNumberSemantic entity)
        {
            return base.AttachEntity(entity) as AntrianAutoNumberSemantic;
        }

        virtual public void Combine(AntrianAutoNumberSemanticCollection collection)
        {
            base.Combine(collection);
        }

        new public AntrianAutoNumberSemantic this[int index]
        {
            get
            {
                return base[index] as AntrianAutoNumberSemantic;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(AntrianAutoNumberSemantic);
        }
    }

    [Serializable]
    abstract public class esAntrianAutoNumberSemantic : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esAntrianAutoNumberSemanticQuery GetDynamicQuery()
        {
            return null;
        }

        public esAntrianAutoNumberSemantic()
        {
        }

        public esAntrianAutoNumberSemantic(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(Int32 antrianAutoNumberSemanticNo)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(antrianAutoNumberSemanticNo);
            else
                return LoadByPrimaryKeyStoredProcedure(antrianAutoNumberSemanticNo);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 antrianAutoNumberSemanticNo)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(antrianAutoNumberSemanticNo);
            else
                return LoadByPrimaryKeyStoredProcedure(antrianAutoNumberSemanticNo);
        }

        private bool LoadByPrimaryKeyDynamic(Int32 antrianAutoNumberSemanticNo)
        {
            esAntrianAutoNumberSemanticQuery query = this.GetDynamicQuery();
            query.Where(query.AntrianAutoNumberSemanticNo == antrianAutoNumberSemanticNo);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(Int32 antrianAutoNumberSemanticNo)
        {
            esParameters parms = new esParameters();
            parms.Add("AntrianAutoNumberSemanticNo", antrianAutoNumberSemanticNo);
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
                        case "AntrianAutoNumberSemanticNo": this.str.AntrianAutoNumberSemanticNo = (string)value; break;
                        case "SRAutoNumber": this.str.SRAutoNumber = (string)value; break;
                        case "PayerType": this.str.PayerType = (string)value; break;
                        case "ServiceGroup": this.str.ServiceGroup = (string)value; break;
                        case "Channel": this.str.Channel = (string)value; break;
                        case "DisplayOrder": this.str.DisplayOrder = (string)value; break;
                        case "DisplayName": this.str.DisplayName = (string)value; break;
                        case "IsActive": this.str.IsActive = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "AntrianAutoNumberSemanticNo":

                            if (value == null || value is System.Int32)
                                this.AntrianAutoNumberSemanticNo = (System.Int32?)value;
                            break;
                        case "DisplayOrder":

                            if (value == null || value is System.Int32)
                                this.DisplayOrder = (System.Int32?)value;
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
        /// Maps to AntrianAutoNumberSemantic.AntrianAutoNumberSemanticNo
        /// </summary>
        virtual public System.Int32? AntrianAutoNumberSemanticNo
        {
            get
            {
                return base.GetSystemInt32(AntrianAutoNumberSemanticMetadata.ColumnNames.AntrianAutoNumberSemanticNo);
            }

            set
            {
                base.SetSystemInt32(AntrianAutoNumberSemanticMetadata.ColumnNames.AntrianAutoNumberSemanticNo, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.SRAutoNumber
        /// </summary>
        virtual public System.String SRAutoNumber
        {
            get
            {
                return base.GetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.SRAutoNumber);
            }

            set
            {
                base.SetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.SRAutoNumber, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.PayerType
        /// </summary>
        virtual public System.String PayerType
        {
            get
            {
                return base.GetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.PayerType);
            }

            set
            {
                base.SetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.PayerType, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.ServiceGroup
        /// </summary>
        virtual public System.String ServiceGroup
        {
            get
            {
                return base.GetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.ServiceGroup);
            }

            set
            {
                base.SetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.ServiceGroup, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.Channel
        /// </summary>
        virtual public System.String Channel
        {
            get
            {
                return base.GetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.Channel);
            }

            set
            {
                base.SetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.Channel, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.DisplayOrder
        /// </summary>
        virtual public System.Int32? DisplayOrder
        {
            get
            {
                return base.GetSystemInt32(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayOrder);
            }

            set
            {
                base.SetSystemInt32(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayOrder, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.DisplayName
        /// </summary>
        virtual public System.String DisplayName
        {
            get
            {
                return base.GetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayName);
            }

            set
            {
                base.SetSystemString(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayName, value);
            }
        }
        /// <summary>
        /// Maps to AntrianAutoNumberSemantic.IsActive
        /// </summary>
        virtual public System.Boolean? IsActive
        {
            get
            {
                return base.GetSystemBoolean(AntrianAutoNumberSemanticMetadata.ColumnNames.IsActive);
            }

            set
            {
                base.SetSystemBoolean(AntrianAutoNumberSemanticMetadata.ColumnNames.IsActive, value);
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
            public esStrings(esAntrianAutoNumberSemantic entity)
            {
                this.entity = entity;
            }
            public System.String AntrianAutoNumberSemanticNo
            {
                get
                {
                    System.Int32? data = entity.AntrianAutoNumberSemanticNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.AntrianAutoNumberSemanticNo = null;
                    else entity.AntrianAutoNumberSemanticNo = Convert.ToInt32(value);
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
            public System.String PayerType
            {
                get
                {
                    System.String data = entity.PayerType;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PayerType = null;
                    else entity.PayerType = Convert.ToString(value);
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
            public System.String Channel
            {
                get
                {
                    System.String data = entity.Channel;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.Channel = null;
                    else entity.Channel = Convert.ToString(value);
                }
            }
            public System.String DisplayOrder
            {
                get
                {
                    System.Int32? data = entity.DisplayOrder;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DisplayOrder = null;
                    else entity.DisplayOrder = Convert.ToInt32(value);
                }
            }
            public System.String DisplayName
            {
                get
                {
                    System.String data = entity.DisplayName;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DisplayName = null;
                    else entity.DisplayName = Convert.ToString(value);
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

            private esAntrianAutoNumberSemantic entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esAntrianAutoNumberSemanticQuery query)
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
                throw new Exception("esAntrianAutoNumberSemantic can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class AntrianAutoNumberSemantic : esAntrianAutoNumberSemantic
    {
        public static object TakeQueueVisitNumber(
           string srAutoNumber,
           string queueLocation
       )
        {
            esParameters prms = new esParameters();

            prms.Add(
                "SRAutoNumber",
                srAutoNumber,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "UserID",
                "AntrianRSI",
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "TransDate",
                DateTime.Now.Date,
                esParameterDirection.Input,
                DbType.Date,
                0
            );

            prms.Add(
                "ServiceUnitID",
                DBNull.Value,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "QueueLocation",
                queueLocation,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "CategoryID",
                DBNull.Value,
                esParameterDirection.Input,
                DbType.String,
                50
            );

            prms.Add(
                "VisitNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            prms.Add(
                "VisitQueueNo",
                esParameterDirection.Output,
                DbType.String,
                50
            );

            var entity =
                new AntrianAutoNumberSemantic();

            entity.ExecuteNonQuery(
                esQueryType.StoredProcedure,
                "TakeQueueVisitNumber",
                prms
            );

            return new
            {
                VisitNo =
                    prms["VisitNo"].Value == null
                        ? ""
                        : prms["VisitNo"].Value.ToString(),

                VisitQueueNo =
                    prms["VisitQueueNo"].Value == null
                        ? ""
                        : prms["VisitQueueNo"].Value.ToString()
            };
        }

        public static object GetSRAutoNumberList(
            string payerType,
            string serviceGroup,
            string channel
        )
        {
            var entity =
                new AntrianAutoNumberSemantic();

            var q = entity.Query;

            q.Where(q.IsActive == 1);

            if (!string.IsNullOrWhiteSpace(payerType))
                q.And(q.PayerType == payerType);

            if (!string.IsNullOrWhiteSpace(serviceGroup))
                q.And(q.ServiceGroup == serviceGroup);

            if (!string.IsNullOrWhiteSpace(channel))
                q.And(q.Channel == channel);

            q.OrderBy(q.DisplayOrder.Ascending);

            // 🔥 INI KUNCINYA
            DataTable dt = q.LoadDataTable();

            if (dt == null || dt.Rows.Count == 0)
                return new List<object>();

            return dt.AsEnumerable()
                .Select(r => new
                {
                    AntrianAutoNumberSemanticNo = r["AntrianAutoNumberSemanticNo"],
                    SRAutoNumber = r["SRAutoNumber"],
                    PayerType = r["PayerType"],
                    ServiceGroup = r["ServiceGroup"],
                    Channel = r["Channel"],
                    DisplayOrder = r["DisplayOrder"],
                    DisplayName = r["DisplayName"],
                    IsActive = r["IsActive"]
                })
                .ToList();
        }

    }

    [Serializable]
    abstract public class esAntrianAutoNumberSemanticQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return AntrianAutoNumberSemanticMetadata.Meta();
            }
        }

        public esQueryItem AntrianAutoNumberSemanticNo
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.AntrianAutoNumberSemanticNo, esSystemType.Int32);
            }
        }
        public esQueryItem SRAutoNumber
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.SRAutoNumber, esSystemType.String);
            }
        }
        public esQueryItem PayerType
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.PayerType, esSystemType.String);
            }
        }
        public esQueryItem ServiceGroup
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.ServiceGroup, esSystemType.String);
            }
        }
        public esQueryItem Channel
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.Channel, esSystemType.String);
            }
        }
        public esQueryItem DisplayOrder
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayOrder, esSystemType.Int32);
            }
        }
        public esQueryItem DisplayName
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayName, esSystemType.String);
            }
        }
        public esQueryItem IsActive
        {
            get
            {
                return new esQueryItem(this, AntrianAutoNumberSemanticMetadata.ColumnNames.IsActive, esSystemType.Boolean);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("AntrianAutoNumberSemanticCollection")]
    public partial class AntrianAutoNumberSemanticCollection : esAntrianAutoNumberSemanticCollection, IEnumerable<AntrianAutoNumberSemantic>
    {
        public AntrianAutoNumberSemanticCollection()
        {

        }

        public static implicit operator List<AntrianAutoNumberSemantic>(AntrianAutoNumberSemanticCollection coll)
        {
            List<AntrianAutoNumberSemantic> list = new List<AntrianAutoNumberSemantic>();

            foreach (AntrianAutoNumberSemantic emp in coll)
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
                return AntrianAutoNumberSemanticMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AntrianAutoNumberSemanticQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new AntrianAutoNumberSemantic(row);
        }

        override protected esEntity CreateEntity()
        {
            return new AntrianAutoNumberSemantic();
        }

        #endregion

        [BrowsableAttribute(false)]
        public AntrianAutoNumberSemanticQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AntrianAutoNumberSemanticQuery();
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
        public bool Load(AntrianAutoNumberSemanticQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public AntrianAutoNumberSemantic AddNew()
        {
            AntrianAutoNumberSemantic entity = base.AddNewEntity() as AntrianAutoNumberSemantic;

            return entity;
        }
        public AntrianAutoNumberSemantic FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as AntrianAutoNumberSemantic;
        }

        #region IEnumerable< AntrianAutoNumberSemantic> Members

        IEnumerator<AntrianAutoNumberSemantic> IEnumerable<AntrianAutoNumberSemantic>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as AntrianAutoNumberSemantic;
            }
        }

        #endregion

        private AntrianAutoNumberSemanticQuery query;
    }


    /// <summary>
    /// Encapsulates the 'AntrianAutoNumberSemantic' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("AntrianAutoNumberSemantic ({StandardReferenceID})")]
    [Serializable]
    public partial class AntrianAutoNumberSemantic : esAntrianAutoNumberSemantic
    {
        public AntrianAutoNumberSemantic()
        {
        }

        public AntrianAutoNumberSemantic(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return AntrianAutoNumberSemanticMetadata.Meta();
            }
        }

        override protected esAntrianAutoNumberSemanticQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AntrianAutoNumberSemanticQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public AntrianAutoNumberSemanticQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AntrianAutoNumberSemanticQuery();
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
        public bool Load(AntrianAutoNumberSemanticQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private AntrianAutoNumberSemanticQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class AntrianAutoNumberSemanticQuery : esAntrianAutoNumberSemanticQuery
    {
        public AntrianAutoNumberSemanticQuery()
        {

        }

        public AntrianAutoNumberSemanticQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "AntrianAutoNumberSemanticQuery";
        }
    }

    [Serializable]
    public partial class AntrianAutoNumberSemanticMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected AntrianAutoNumberSemanticMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.AntrianAutoNumberSemanticNo, 0, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.AntrianAutoNumberSemanticNo;
            c.IsInPrimaryKey = true;
            c.NumericPrecision = 10;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.SRAutoNumber, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.SRAutoNumber;
            c.CharacterMaxLength = 100;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.PayerType, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.PayerType;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.ServiceGroup, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.ServiceGroup;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.Channel, 4, typeof(System.String), esSystemType.String);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.Channel;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayOrder, 5, typeof(System.Int32), esSystemType.Int32);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.DisplayOrder;
            c.NumericPrecision = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.DisplayName, 6, typeof(System.String), esSystemType.String);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.DisplayName;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AntrianAutoNumberSemanticMetadata.ColumnNames.IsActive, 7, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = AntrianAutoNumberSemanticMetadata.PropertyNames.IsActive;
            c.HasDefault = true;
            c.Default = @"((1))";
            _columns.Add(c);
        }
        #endregion

        static public AntrianAutoNumberSemanticMetadata Meta()
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
            public const string AntrianAutoNumberSemanticNo = "AntrianAutoNumberSemanticNo";
            public const string SRAutoNumber = "SRAutoNumber";
            public const string PayerType = "PayerType";
            public const string ServiceGroup = "ServiceGroup";
            public const string Channel = "Channel";
            public const string DisplayOrder = "DisplayOrder";
            public const string DisplayName = "DisplayName";
            public const string IsActive = "IsActive";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string AntrianAutoNumberSemanticNo = "AntrianAutoNumberSemanticNo";
            public const string SRAutoNumber = "SRAutoNumber";
            public const string PayerType = "PayerType";
            public const string ServiceGroup = "ServiceGroup";
            public const string Channel = "Channel";
            public const string DisplayOrder = "DisplayOrder";
            public const string DisplayName = "DisplayName";
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
            lock (typeof(AntrianAutoNumberSemanticMetadata))
            {
                if (AntrianAutoNumberSemanticMetadata.mapDelegates == null)
                {
                    AntrianAutoNumberSemanticMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (AntrianAutoNumberSemanticMetadata.meta == null)
                {
                    AntrianAutoNumberSemanticMetadata.meta = new AntrianAutoNumberSemanticMetadata();
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

                meta.AddTypeMap("AntrianAutoNumberSemanticNo", new esTypeMap("int", "System.Int32"));
                meta.AddTypeMap("SRAutoNumber", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PayerType", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceGroup", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("Channel", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("DisplayOrder", new esTypeMap("int", "System.Int32"));
                meta.AddTypeMap("DisplayName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));


                meta.Source = "AntrianAutoNumberSemantic";
                meta.Destination = "AntrianAutoNumberSemantic";
                meta.spInsert = "proc_AntrianAutoNumberSemanticInsert";
                meta.spUpdate = "proc_AntrianAutoNumberSemanticUpdate";
                meta.spDelete = "proc_AntrianAutoNumberSemanticDelete";
                meta.spLoadAll = "proc_AntrianAutoNumberSemanticLoadAll";
                meta.spLoadByPrimaryKey = "proc_AntrianAutoNumberSemanticLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private AntrianAutoNumberSemanticMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }
}