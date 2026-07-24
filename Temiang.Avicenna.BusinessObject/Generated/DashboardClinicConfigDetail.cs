/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-07-23 01:41:47 PM
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
	abstract public class esDashboardClinicConfigDetailCollection : esEntityCollectionWAuditLog
	{
		public esDashboardClinicConfigDetailCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "DashboardClinicConfigDetailCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esDashboardClinicConfigDetailQuery query)
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
			this.InitQuery(query as esDashboardClinicConfigDetailQuery);
		}
		#endregion
			
		virtual public DashboardClinicConfigDetail DetachEntity(DashboardClinicConfigDetail entity)
		{
			return base.DetachEntity(entity) as DashboardClinicConfigDetail;
		}
		
		virtual public DashboardClinicConfigDetail AttachEntity(DashboardClinicConfigDetail entity)
		{
			return base.AttachEntity(entity) as DashboardClinicConfigDetail;
		}
		
		virtual public void Combine(DashboardClinicConfigDetailCollection collection)
		{
			base.Combine(collection);
		}
		
		new public DashboardClinicConfigDetail this[int index]
		{
			get
			{
				return base[index] as DashboardClinicConfigDetail;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(DashboardClinicConfigDetail);
		}
	}

	[Serializable]
	abstract public class esDashboardClinicConfigDetail : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esDashboardClinicConfigDetailQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esDashboardClinicConfigDetail()
		{
		}
	
		public esDashboardClinicConfigDetail(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 configDetailID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(configDetailID);
			else
				return LoadByPrimaryKeyStoredProcedure(configDetailID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 configDetailID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(configDetailID);
			else
				return LoadByPrimaryKeyStoredProcedure(configDetailID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 configDetailID)
		{
			esDashboardClinicConfigDetailQuery query = this.GetDynamicQuery();
			query.Where(query.ConfigDetailID == configDetailID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 configDetailID)
		{
			esParameters parms = new esParameters();
			parms.Add("ConfigDetailID", configDetailID);
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
			if(this.Row == null) this.AddNew();
			
			esColumnMetadata col = this.Meta.Columns.FindByPropertyName(name);
			if (col != null)
			{
				if(value == null || value is System.String)
				{				
					// Use the strongly typed property
					switch (name)
					{
						case "ConfigDetailID": this.str.ConfigDetailID = (string)value; break;
						case "ConfigID": this.str.ConfigID = (string)value; break;
						case "ServiceUnitID": this.str.ServiceUnitID = (string)value; break;
						case "StageID": this.str.StageID = (string)value; break;
						case "ParamedicID": this.str.ParamedicID = (string)value; break;
						case "KamarCode": this.str.KamarCode = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "ConfigDetailID":
						
							if (value == null || value is System.Int64)
								this.ConfigDetailID = (System.Int64?)value;
							break;

                        default:
							break;
					}
				}
			}
			else if(this.Row.Table.Columns.Contains(name))
			{
				this.Row[name] = value;
			}
			else
			{
				throw new Exception("SetProperty Error: '" + name + "' not found");
			}
		}

		/// <summary>
		/// Maps to DashboardClinicConfigDetail.ConfigDetailID
		/// </summary>
		virtual public System.Int64? ConfigDetailID
		{
			get
			{
				return base.GetSystemInt64(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigDetailID);
			}
			
			set
			{
				base.SetSystemInt64(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigDetailID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfigDetail.ConfigID
		/// </summary>
		virtual public System.String ConfigID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfigDetail.ServiceUnitID
		/// </summary>
		virtual public System.String ServiceUnitID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ServiceUnitID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ServiceUnitID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfigDetail.StageID
		/// </summary>
		virtual public System.String StageID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.StageID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.StageID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfigDetail.ParamedicID
		/// </summary>
		virtual public System.String ParamedicID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ParamedicID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.ParamedicID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfigDetail.KamarID
		/// </summary>
		virtual public System.String KamarCode
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.KamarCode);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigDetailMetadata.ColumnNames.KamarCode, value);
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
		[BrowsableAttribute( false )]		
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
			public esStrings(esDashboardClinicConfigDetail entity)
			{
				this.entity = entity;
			}
			public System.String ConfigDetailID
			{
				get
				{
					System.Int64? data = entity.ConfigDetailID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConfigDetailID = null;
					else entity.ConfigDetailID = Convert.ToInt64(value);
				}
			}
			public System.String ConfigID
			{
				get
				{
					System.String data = entity.ConfigID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConfigID = null;
					else entity.ConfigID = Convert.ToString(value);
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
			public System.String KamarCode
			{
				get
				{
					System.String data = entity.KamarCode;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KamarCode = null;
					else entity.KamarCode = Convert.ToString(value);
				}
			}
			
			private esDashboardClinicConfigDetail entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esDashboardClinicConfigDetailQuery query)
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
				throw new Exception("esDashboardClinicConfigDetail can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class DashboardClinicConfigDetail : esDashboardClinicConfigDetail
	{
        public static void DeleteByConfigID(string configID)
        {
            DashboardClinicConfigDetailCollection collection =
                new DashboardClinicConfigDetailCollection();

            DashboardClinicConfigDetailQuery q = new DashboardClinicConfigDetailQuery();

            q.Where(q.ConfigID == configID);

            if (collection.Load(q))
            {
                foreach (DashboardClinicConfigDetail item in collection)
                {
                    item.MarkAsDeleted();
                }

                collection.Save();
            }
        }
    }

	[Serializable]
	abstract public class esDashboardClinicConfigDetailQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return DashboardClinicConfigDetailMetadata.Meta();
			}
		}	
			
		public esQueryItem ConfigDetailID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.ConfigDetailID, esSystemType.Int64);
			}
		} 
		public esQueryItem ConfigID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.ConfigID, esSystemType.String);
			}
		} 
		public esQueryItem ServiceUnitID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.ServiceUnitID, esSystemType.String);
			}
		} 
		public esQueryItem StageID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.StageID, esSystemType.String);
			}
		} 
		public esQueryItem ParamedicID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.ParamedicID, esSystemType.String);
			}
		} 
		public esQueryItem KamarCode
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigDetailMetadata.ColumnNames.KamarCode, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("DashboardClinicConfigDetailCollection")]
	public partial class DashboardClinicConfigDetailCollection : esDashboardClinicConfigDetailCollection, IEnumerable< DashboardClinicConfigDetail>
	{
		public DashboardClinicConfigDetailCollection()
		{

		}	
		
		public static implicit operator List< DashboardClinicConfigDetail>(DashboardClinicConfigDetailCollection coll)
		{
			List< DashboardClinicConfigDetail> list = new List< DashboardClinicConfigDetail>();
			
			foreach (DashboardClinicConfigDetail emp in coll)
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
				return  DashboardClinicConfigDetailMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DashboardClinicConfigDetailQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new DashboardClinicConfigDetail(row);
		}

		override protected esEntity CreateEntity()
		{
			return new DashboardClinicConfigDetail();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public DashboardClinicConfigDetailQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DashboardClinicConfigDetailQuery();
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
		public bool Load(DashboardClinicConfigDetailQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public DashboardClinicConfigDetail AddNew()
		{
			DashboardClinicConfigDetail entity = base.AddNewEntity() as DashboardClinicConfigDetail;
			
			return entity;		
		}
		public DashboardClinicConfigDetail FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as DashboardClinicConfigDetail;
		}

		#region IEnumerable< DashboardClinicConfigDetail> Members

		IEnumerator< DashboardClinicConfigDetail> IEnumerable< DashboardClinicConfigDetail>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as DashboardClinicConfigDetail;
			}
		}

		#endregion
		
		private DashboardClinicConfigDetailQuery query;
	}


	/// <summary>
	/// Encapsulates the 'DashboardClinicConfigDetail' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("DashboardClinicConfigDetail ({StandardReferenceID})")]
	[Serializable]
	public partial class DashboardClinicConfigDetail : esDashboardClinicConfigDetail
	{
		public DashboardClinicConfigDetail()
		{
		}	
	
		public DashboardClinicConfigDetail(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return DashboardClinicConfigDetailMetadata.Meta();
			}
		}	
	
		override protected esDashboardClinicConfigDetailQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DashboardClinicConfigDetailQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public DashboardClinicConfigDetailQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DashboardClinicConfigDetailQuery();
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
		public bool Load(DashboardClinicConfigDetailQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private DashboardClinicConfigDetailQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class DashboardClinicConfigDetailQuery : esDashboardClinicConfigDetailQuery
	{
		public DashboardClinicConfigDetailQuery()
		{

		}		
		
		public DashboardClinicConfigDetailQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "DashboardClinicConfigDetailQuery";
        }
	}

	[Serializable]
	public partial class DashboardClinicConfigDetailMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected DashboardClinicConfigDetailMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigDetailID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.ConfigDetailID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.ConfigID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.ConfigID;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.ServiceUnitID, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.ServiceUnitID;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.StageID, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.StageID;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.ParamedicID, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.ParamedicID;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigDetailMetadata.ColumnNames.KamarCode, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigDetailMetadata.PropertyNames.KamarCode;
			c.NumericPrecision = 10;
			_columns.Add(c); 
		}
		#endregion
	
		static public DashboardClinicConfigDetailMetadata Meta()
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
			get	{ return base._columns; }
		}
		
		#region ColumnNames
		public class ColumnNames
		{ 
			public const string ConfigDetailID = "ConfigDetailID";
			public const string ConfigID = "ConfigID";
			public const string ServiceUnitID = "ServiceUnitID";
			public const string StageID = "StageID";
			public const string ParamedicID = "ParamedicID";
			public const string KamarCode = "KamarCode";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string ConfigDetailID = "ConfigDetailID";
			public const string ConfigID = "ConfigID";
			public const string ServiceUnitID = "ServiceUnitID";
			public const string StageID = "StageID";
			public const string ParamedicID = "ParamedicID";
			public const string KamarCode = "KamarCode";
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
			lock (typeof(DashboardClinicConfigDetailMetadata))
			{
				if(DashboardClinicConfigDetailMetadata.mapDelegates == null)
				{
					DashboardClinicConfigDetailMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (DashboardClinicConfigDetailMetadata.meta == null)
				{
					DashboardClinicConfigDetailMetadata.meta = new DashboardClinicConfigDetailMetadata();
				}
				
				MapToMeta mapMethod = new MapToMeta(meta.esDefault);
				mapDelegates.Add("esDefault", mapMethod);
				mapMethod("esDefault");
			}
			return 0;
		}			

		private esProviderSpecificMetadata esDefault(string mapName)
		{
			if(!_providerMetadataMaps.ContainsKey(mapName))
			{
				esProviderSpecificMetadata meta = new esProviderSpecificMetadata();
				
				meta.AddTypeMap("ConfigDetailID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("ConfigID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ServiceUnitID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("StageID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ParamedicID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KamarCode", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "DashboardClinicConfigDetail";
				meta.Destination = "DashboardClinicConfigDetail";
				meta.spInsert = "proc_DashboardClinicConfigDetailInsert";				
				meta.spUpdate = "proc_DashboardClinicConfigDetailUpdate";		
				meta.spDelete = "proc_DashboardClinicConfigDetailDelete";
				meta.spLoadAll = "proc_DashboardClinicConfigDetailLoadAll";
				meta.spLoadByPrimaryKey = "proc_DashboardClinicConfigDetailLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private DashboardClinicConfigDetailMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		