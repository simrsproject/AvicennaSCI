/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-06-11 11:00:18 AM
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
	abstract public class esVwPOPreComputeQtyFinishedCollection : esEntityCollectionWAuditLog
	{
		public esVwPOPreComputeQtyFinishedCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "VwPOPreComputeQtyFinishedCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esVwPOPreComputeQtyFinishedQuery query)
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
			this.InitQuery(query as esVwPOPreComputeQtyFinishedQuery);
		}
		#endregion
			
		virtual public VwPOPreComputeQtyFinished DetachEntity(VwPOPreComputeQtyFinished entity)
		{
			return base.DetachEntity(entity) as VwPOPreComputeQtyFinished;
		}
		
		virtual public VwPOPreComputeQtyFinished AttachEntity(VwPOPreComputeQtyFinished entity)
		{
			return base.AttachEntity(entity) as VwPOPreComputeQtyFinished;
		}
		
		virtual public void Combine(VwPOPreComputeQtyFinishedCollection collection)
		{
			base.Combine(collection);
		}
		
		new public VwPOPreComputeQtyFinished this[int index]
		{
			get
			{
				return base[index] as VwPOPreComputeQtyFinished;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(VwPOPreComputeQtyFinished);
		}
	}

	[Serializable]
	abstract public class esVwPOPreComputeQtyFinished : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esVwPOPreComputeQtyFinishedQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esVwPOPreComputeQtyFinished()
		{
		}
	
		public esVwPOPreComputeQtyFinished(DataRow row)
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
			if(this.Row == null) this.AddNew();
			
			esColumnMetadata col = this.Meta.Columns.FindByPropertyName(name);
			if (col != null)
			{
				if(value == null || value is System.String)
				{				
					// Use the strongly typed property
					switch (name)
					{
						case "ReferenceNo": this.str.ReferenceNo = (string)value; break;
						case "ReferenceSequenceNo": this.str.ReferenceSequenceNo = (string)value; break;
						case "QtyFinished": this.str.QtyFinished = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "QtyFinished":
						
							if (value == null || value is System.Decimal)
								this.QtyFinished = (System.Decimal?)value;
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
		/// Maps to VwPOPreComputeQtyFinished.ReferenceNo
		/// </summary>
		virtual public System.String ReferenceNo
		{
			get
			{
				return base.GetSystemString(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceNo);
			}
			
			set
			{
				base.SetSystemString(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceNo, value);
			}
		}
		/// <summary>
		/// Maps to VwPOPreComputeQtyFinished.ReferenceSequenceNo
		/// </summary>
		virtual public System.String ReferenceSequenceNo
		{
			get
			{
				return base.GetSystemString(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceSequenceNo);
			}
			
			set
			{
				base.SetSystemString(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceSequenceNo, value);
			}
		}
		/// <summary>
		/// Maps to VwPOPreComputeQtyFinished.QtyFinished
		/// </summary>
		virtual public System.Decimal? QtyFinished
		{
			get
			{
				return base.GetSystemDecimal(VwPOPreComputeQtyFinishedMetadata.ColumnNames.QtyFinished);
			}
			
			set
			{
				base.SetSystemDecimal(VwPOPreComputeQtyFinishedMetadata.ColumnNames.QtyFinished, value);
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
			public esStrings(esVwPOPreComputeQtyFinished entity)
			{
				this.entity = entity;
			}
			public System.String ReferenceNo
			{
				get
				{
					System.String data = entity.ReferenceNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ReferenceNo = null;
					else entity.ReferenceNo = Convert.ToString(value);
				}
			}
			public System.String ReferenceSequenceNo
			{
				get
				{
					System.String data = entity.ReferenceSequenceNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ReferenceSequenceNo = null;
					else entity.ReferenceSequenceNo = Convert.ToString(value);
				}
			}
			public System.String QtyFinished
			{
				get
				{
					System.Decimal? data = entity.QtyFinished;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QtyFinished = null;
					else entity.QtyFinished = Convert.ToDecimal(value);
				}
			}
			
			private esVwPOPreComputeQtyFinished entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esVwPOPreComputeQtyFinishedQuery query)
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
				throw new Exception("esVwPOPreComputeQtyFinished can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class VwPOPreComputeQtyFinished : esVwPOPreComputeQtyFinished
	{	
	}

	[Serializable]
	abstract public class esVwPOPreComputeQtyFinishedQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return VwPOPreComputeQtyFinishedMetadata.Meta();
			}
		}	
			
		public esQueryItem ReferenceNo
		{
			get
			{
				return new esQueryItem(this, VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceNo, esSystemType.String);
			}
		} 
		public esQueryItem ReferenceSequenceNo
		{
			get
			{
				return new esQueryItem(this, VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceSequenceNo, esSystemType.String);
			}
		} 
		public esQueryItem QtyFinished
		{
			get
			{
				return new esQueryItem(this, VwPOPreComputeQtyFinishedMetadata.ColumnNames.QtyFinished, esSystemType.Decimal);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("VwPOPreComputeQtyFinishedCollection")]
	public partial class VwPOPreComputeQtyFinishedCollection : esVwPOPreComputeQtyFinishedCollection, IEnumerable< VwPOPreComputeQtyFinished>
	{
		public VwPOPreComputeQtyFinishedCollection()
		{

		}	
		
		public static implicit operator List< VwPOPreComputeQtyFinished>(VwPOPreComputeQtyFinishedCollection coll)
		{
			List< VwPOPreComputeQtyFinished> list = new List< VwPOPreComputeQtyFinished>();
			
			foreach (VwPOPreComputeQtyFinished emp in coll)
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
				return  VwPOPreComputeQtyFinishedMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new VwPOPreComputeQtyFinishedQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new VwPOPreComputeQtyFinished(row);
		}

		override protected esEntity CreateEntity()
		{
			return new VwPOPreComputeQtyFinished();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public VwPOPreComputeQtyFinishedQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new VwPOPreComputeQtyFinishedQuery();
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
		public bool Load(VwPOPreComputeQtyFinishedQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public VwPOPreComputeQtyFinished AddNew()
		{
			VwPOPreComputeQtyFinished entity = base.AddNewEntity() as VwPOPreComputeQtyFinished;
			
			return entity;		
		}
		public VwPOPreComputeQtyFinished FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as VwPOPreComputeQtyFinished;
		}

		#region IEnumerable< VwPOPreComputeQtyFinished> Members

		IEnumerator< VwPOPreComputeQtyFinished> IEnumerable< VwPOPreComputeQtyFinished>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as VwPOPreComputeQtyFinished;
			}
		}

		#endregion
		
		private VwPOPreComputeQtyFinishedQuery query;
	}


	/// <summary>
	/// Encapsulates the 'VwPOPreComputeQtyFinished' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("VwPOPreComputeQtyFinished ({StandardReferenceID})")]
	[Serializable]
	public partial class VwPOPreComputeQtyFinished : esVwPOPreComputeQtyFinished
	{
		public VwPOPreComputeQtyFinished()
		{
		}	
	
		public VwPOPreComputeQtyFinished(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return VwPOPreComputeQtyFinishedMetadata.Meta();
			}
		}	
	
		override protected esVwPOPreComputeQtyFinishedQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new VwPOPreComputeQtyFinishedQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public VwPOPreComputeQtyFinishedQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new VwPOPreComputeQtyFinishedQuery();
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
		public bool Load(VwPOPreComputeQtyFinishedQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private VwPOPreComputeQtyFinishedQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class VwPOPreComputeQtyFinishedQuery : esVwPOPreComputeQtyFinishedQuery
	{
		public VwPOPreComputeQtyFinishedQuery()
		{

		}		
		
		public VwPOPreComputeQtyFinishedQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "VwPOPreComputeQtyFinishedQuery";
        }
	}

	[Serializable]
	public partial class VwPOPreComputeQtyFinishedMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected VwPOPreComputeQtyFinishedMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceNo, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = VwPOPreComputeQtyFinishedMetadata.PropertyNames.ReferenceNo;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(VwPOPreComputeQtyFinishedMetadata.ColumnNames.ReferenceSequenceNo, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = VwPOPreComputeQtyFinishedMetadata.PropertyNames.ReferenceSequenceNo;
			c.CharacterMaxLength = 3;
			_columns.Add(c); 
				
			c = new esColumnMetadata(VwPOPreComputeQtyFinishedMetadata.ColumnNames.QtyFinished, 2, typeof(System.Decimal), esSystemType.Decimal);
			c.PropertyName = VwPOPreComputeQtyFinishedMetadata.PropertyNames.QtyFinished;
			c.NumericPrecision = 38;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public VwPOPreComputeQtyFinishedMetadata Meta()
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
			public const string ReferenceNo = "ReferenceNo";
			public const string ReferenceSequenceNo = "ReferenceSequenceNo";
			public const string QtyFinished = "QtyFinished";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string ReferenceNo = "ReferenceNo";
			public const string ReferenceSequenceNo = "ReferenceSequenceNo";
			public const string QtyFinished = "QtyFinished";
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
			lock (typeof(VwPOPreComputeQtyFinishedMetadata))
			{
				if(VwPOPreComputeQtyFinishedMetadata.mapDelegates == null)
				{
					VwPOPreComputeQtyFinishedMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (VwPOPreComputeQtyFinishedMetadata.meta == null)
				{
					VwPOPreComputeQtyFinishedMetadata.meta = new VwPOPreComputeQtyFinishedMetadata();
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
				
				meta.AddTypeMap("ReferenceNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ReferenceSequenceNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("QtyFinished", new esTypeMap("numeric", "System.Decimal"));
		

				meta.Source = "Vw_POPreComputeQtyFinished";
				meta.Destination = "Vw_POPreComputeQtyFinished";
				meta.spInsert = "proc_Vw_POPreComputeQtyFinishedInsert";				
				meta.spUpdate = "proc_Vw_POPreComputeQtyFinishedUpdate";		
				meta.spDelete = "proc_Vw_POPreComputeQtyFinishedDelete";
				meta.spLoadAll = "proc_Vw_POPreComputeQtyFinishedLoadAll";
				meta.spLoadByPrimaryKey = "proc_Vw_POPreComputeQtyFinishedLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private VwPOPreComputeQtyFinishedMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		