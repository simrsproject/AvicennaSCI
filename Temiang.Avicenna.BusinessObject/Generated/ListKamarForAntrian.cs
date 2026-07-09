/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-07-09 02:15:15 PM
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
	abstract public class esListKamarForAntrianCollection : esEntityCollectionWAuditLog
	{
		public esListKamarForAntrianCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "ListKamarForAntrianCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esListKamarForAntrianQuery query)
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
			this.InitQuery(query as esListKamarForAntrianQuery);
		}
		#endregion
			
		virtual public ListKamarForAntrian DetachEntity(ListKamarForAntrian entity)
		{
			return base.DetachEntity(entity) as ListKamarForAntrian;
		}
		
		virtual public ListKamarForAntrian AttachEntity(ListKamarForAntrian entity)
		{
			return base.AttachEntity(entity) as ListKamarForAntrian;
		}
		
		virtual public void Combine(ListKamarForAntrianCollection collection)
		{
			base.Combine(collection);
		}
		
		new public ListKamarForAntrian this[int index]
		{
			get
			{
				return base[index] as ListKamarForAntrian;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(ListKamarForAntrian);
		}
	}

	[Serializable]
	abstract public class esListKamarForAntrian : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esListKamarForAntrianQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esListKamarForAntrian()
		{
		}
	
		public esListKamarForAntrian(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int32 kamarID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(kamarID);
			else
				return LoadByPrimaryKeyStoredProcedure(kamarID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 kamarID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(kamarID);
			else
				return LoadByPrimaryKeyStoredProcedure(kamarID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int32 kamarID)
		{
			esListKamarForAntrianQuery query = this.GetDynamicQuery();
			query.Where(query.KamarID == kamarID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int32 kamarID)
		{
			esParameters parms = new esParameters();
			parms.Add("KamarID", kamarID);
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
						case "KamarID": this.str.KamarID = (string)value; break;
						case "KamarCode": this.str.KamarCode = (string)value; break;
						case "KamarName": this.str.KamarName = (string)value; break;
						case "IsActive": this.str.IsActive = (string)value; break;
						case "CreatedDate": this.str.CreatedDate = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "KamarID":
						
							if (value == null || value is System.Int32)
								this.KamarID = (System.Int32?)value;
							break;
						case "IsActive":
						
							if (value == null || value is System.Boolean)
								this.IsActive = (System.Boolean?)value;
							break;
						case "CreatedDate":
						
							if (value == null || value is System.DateTime)
								this.CreatedDate = (System.DateTime?)value;
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
		/// Maps to ListKamarForAntrian.KamarID
		/// </summary>
		virtual public System.Int32? KamarID
		{
			get
			{
				return base.GetSystemInt32(ListKamarForAntrianMetadata.ColumnNames.KamarID);
			}
			
			set
			{
				base.SetSystemInt32(ListKamarForAntrianMetadata.ColumnNames.KamarID, value);
			}
		}
		/// <summary>
		/// Maps to ListKamarForAntrian.KamarCode
		/// </summary>
		virtual public System.String KamarCode
		{
			get
			{
				return base.GetSystemString(ListKamarForAntrianMetadata.ColumnNames.KamarCode);
			}
			
			set
			{
				base.SetSystemString(ListKamarForAntrianMetadata.ColumnNames.KamarCode, value);
			}
		}
		/// <summary>
		/// Maps to ListKamarForAntrian.KamarName
		/// </summary>
		virtual public System.String KamarName
		{
			get
			{
				return base.GetSystemString(ListKamarForAntrianMetadata.ColumnNames.KamarName);
			}
			
			set
			{
				base.SetSystemString(ListKamarForAntrianMetadata.ColumnNames.KamarName, value);
			}
		}
		/// <summary>
		/// Maps to ListKamarForAntrian.IsActive
		/// </summary>
		virtual public System.Boolean? IsActive
		{
			get
			{
				return base.GetSystemBoolean(ListKamarForAntrianMetadata.ColumnNames.IsActive);
			}
			
			set
			{
				base.SetSystemBoolean(ListKamarForAntrianMetadata.ColumnNames.IsActive, value);
			}
		}
		/// <summary>
		/// Maps to ListKamarForAntrian.CreatedDate
		/// </summary>
		virtual public System.DateTime? CreatedDate
		{
			get
			{
				return base.GetSystemDateTime(ListKamarForAntrianMetadata.ColumnNames.CreatedDate);
			}
			
			set
			{
				base.SetSystemDateTime(ListKamarForAntrianMetadata.ColumnNames.CreatedDate, value);
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
			public esStrings(esListKamarForAntrian entity)
			{
				this.entity = entity;
			}
			public System.String KamarID
			{
				get
				{
					System.Int32? data = entity.KamarID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KamarID = null;
					else entity.KamarID = Convert.ToInt32(value);
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
			public System.String KamarName
			{
				get
				{
					System.String data = entity.KamarName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KamarName = null;
					else entity.KamarName = Convert.ToString(value);
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
			
			private esListKamarForAntrian entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esListKamarForAntrianQuery query)
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
				throw new Exception("esListKamarForAntrian can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class ListKamarForAntrian : esListKamarForAntrian
	{	
	}

	[Serializable]
	abstract public class esListKamarForAntrianQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return ListKamarForAntrianMetadata.Meta();
			}
		}	
			
		public esQueryItem KamarID
		{
			get
			{
				return new esQueryItem(this, ListKamarForAntrianMetadata.ColumnNames.KamarID, esSystemType.Int32);
			}
		} 
		public esQueryItem KamarCode
		{
			get
			{
				return new esQueryItem(this, ListKamarForAntrianMetadata.ColumnNames.KamarCode, esSystemType.String);
			}
		} 
		public esQueryItem KamarName
		{
			get
			{
				return new esQueryItem(this, ListKamarForAntrianMetadata.ColumnNames.KamarName, esSystemType.String);
			}
		} 
		public esQueryItem IsActive
		{
			get
			{
				return new esQueryItem(this, ListKamarForAntrianMetadata.ColumnNames.IsActive, esSystemType.Boolean);
			}
		} 
		public esQueryItem CreatedDate
		{
			get
			{
				return new esQueryItem(this, ListKamarForAntrianMetadata.ColumnNames.CreatedDate, esSystemType.DateTime);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("ListKamarForAntrianCollection")]
	public partial class ListKamarForAntrianCollection : esListKamarForAntrianCollection, IEnumerable< ListKamarForAntrian>
	{
		public ListKamarForAntrianCollection()
		{

		}	
		
		public static implicit operator List< ListKamarForAntrian>(ListKamarForAntrianCollection coll)
		{
			List< ListKamarForAntrian> list = new List< ListKamarForAntrian>();
			
			foreach (ListKamarForAntrian emp in coll)
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
				return  ListKamarForAntrianMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ListKamarForAntrianQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new ListKamarForAntrian(row);
		}

		override protected esEntity CreateEntity()
		{
			return new ListKamarForAntrian();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public ListKamarForAntrianQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ListKamarForAntrianQuery();
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
		public bool Load(ListKamarForAntrianQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public ListKamarForAntrian AddNew()
		{
			ListKamarForAntrian entity = base.AddNewEntity() as ListKamarForAntrian;
			
			return entity;		
		}
		public ListKamarForAntrian FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as ListKamarForAntrian;
		}

		#region IEnumerable< ListKamarForAntrian> Members

		IEnumerator< ListKamarForAntrian> IEnumerable< ListKamarForAntrian>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as ListKamarForAntrian;
			}
		}

		#endregion
		
		private ListKamarForAntrianQuery query;
	}


	/// <summary>
	/// Encapsulates the 'ListKamarForAntrian' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("ListKamarForAntrian ({StandardReferenceID})")]
	[Serializable]
	public partial class ListKamarForAntrian : esListKamarForAntrian
	{
		public ListKamarForAntrian()
		{
		}	
	
		public ListKamarForAntrian(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return ListKamarForAntrianMetadata.Meta();
			}
		}	
	
		override protected esListKamarForAntrianQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ListKamarForAntrianQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public ListKamarForAntrianQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ListKamarForAntrianQuery();
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
		public bool Load(ListKamarForAntrianQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private ListKamarForAntrianQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class ListKamarForAntrianQuery : esListKamarForAntrianQuery
	{
		public ListKamarForAntrianQuery()
		{

		}		
		
		public ListKamarForAntrianQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "ListKamarForAntrianQuery";
        }
	}

	[Serializable]
	public partial class ListKamarForAntrianMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected ListKamarForAntrianMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(ListKamarForAntrianMetadata.ColumnNames.KamarID, 0, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = ListKamarForAntrianMetadata.PropertyNames.KamarID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(ListKamarForAntrianMetadata.ColumnNames.KamarCode, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = ListKamarForAntrianMetadata.PropertyNames.KamarCode;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(ListKamarForAntrianMetadata.ColumnNames.KamarName, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = ListKamarForAntrianMetadata.PropertyNames.KamarName;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(ListKamarForAntrianMetadata.ColumnNames.IsActive, 3, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = ListKamarForAntrianMetadata.PropertyNames.IsActive;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(ListKamarForAntrianMetadata.ColumnNames.CreatedDate, 4, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = ListKamarForAntrianMetadata.PropertyNames.CreatedDate;
			c.HasDefault = true;
			c.Default = @"(getdate())";
			_columns.Add(c); 
		}
		#endregion
	
		static public ListKamarForAntrianMetadata Meta()
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
			public const string KamarID = "KamarID";
			public const string KamarCode = "KamarCode";
			public const string KamarName = "KamarName";
			public const string IsActive = "IsActive";
			public const string CreatedDate = "CreatedDate";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string KamarID = "KamarID";
			public const string KamarCode = "KamarCode";
			public const string KamarName = "KamarName";
			public const string IsActive = "IsActive";
			public const string CreatedDate = "CreatedDate";
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
			lock (typeof(ListKamarForAntrianMetadata))
			{
				if(ListKamarForAntrianMetadata.mapDelegates == null)
				{
					ListKamarForAntrianMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (ListKamarForAntrianMetadata.meta == null)
				{
					ListKamarForAntrianMetadata.meta = new ListKamarForAntrianMetadata();
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
				
				meta.AddTypeMap("KamarID", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("KamarCode", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KamarName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("CreatedDate", new esTypeMap("datetime", "System.DateTime"));
		

				meta.Source = "ListKamarForAntrian";
				meta.Destination = "ListKamarForAntrian";
				meta.spInsert = "proc_ListKamarForAntrianInsert";				
				meta.spUpdate = "proc_ListKamarForAntrianUpdate";		
				meta.spDelete = "proc_ListKamarForAntrianDelete";
				meta.spLoadAll = "proc_ListKamarForAntrianLoadAll";
				meta.spLoadByPrimaryKey = "proc_ListKamarForAntrianLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private ListKamarForAntrianMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		