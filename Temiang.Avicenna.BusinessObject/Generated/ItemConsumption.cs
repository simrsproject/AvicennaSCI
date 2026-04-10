/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 10/17/2025 11:03:19 AM
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
	abstract public class esItemConsumptionCollection : esEntityCollectionWAuditLog
	{
		public esItemConsumptionCollection()
		{

		}


		protected override string GetCollectionName()
		{
			return "ItemConsumptionCollection";
		}

		#region Query Logic
		protected void InitQuery(esItemConsumptionQuery query)
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
			this.InitQuery(query as esItemConsumptionQuery);
		}
		#endregion

		virtual public ItemConsumption DetachEntity(ItemConsumption entity)
		{
			return base.DetachEntity(entity) as ItemConsumption;
		}

		virtual public ItemConsumption AttachEntity(ItemConsumption entity)
		{
			return base.AttachEntity(entity) as ItemConsumption;
		}

		virtual public void Combine(ItemConsumptionCollection collection)
		{
			base.Combine(collection);
		}

		new public ItemConsumption this[int index]
		{
			get
			{
				return base[index] as ItemConsumption;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(ItemConsumption);
		}
	}

	[Serializable]
	abstract public class esItemConsumption : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esItemConsumptionQuery GetDynamicQuery()
		{
			return null;
		}

		public esItemConsumption()
		{
		}

		public esItemConsumption(DataRow row)
			: base(row)
		{
		}


		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(String itemID, String detailItemID)
		{
			if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(itemID, detailItemID);
			else
				return LoadByPrimaryKeyStoredProcedure(itemID, detailItemID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String itemID, String detailItemID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(itemID, detailItemID);
			else
				return LoadByPrimaryKeyStoredProcedure(itemID, detailItemID);
		}

		private bool LoadByPrimaryKeyDynamic(String itemID, String detailItemID)
		{
			esItemConsumptionQuery query = this.GetDynamicQuery();
			query.Where(query.ItemID == itemID, query.DetailItemID == detailItemID);
			return query.Load();
		}

		private bool LoadByPrimaryKeyStoredProcedure(String itemID, String detailItemID)
		{
			esParameters parms = new esParameters();
			parms.Add("ItemID", itemID);
			parms.Add("DetailItemID", detailItemID);
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
						case "ItemID": this.str.ItemID = (string)value; break;
						case "DetailItemID": this.str.DetailItemID = (string)value; break;
						case "Qty": this.str.Qty = (string)value; break;
						case "SRItemUnit": this.str.SRItemUnit = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
						case "QtyDosage": this.str.QtyDosage = (string)value; break;
						case "SRDosageUnit": this.str.SRDosageUnit = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{
						case "Qty":

							if (value == null || value is System.Decimal)
								this.Qty = (System.Decimal?)value;
							break;
						case "LastUpdateDateTime":

							if (value == null || value is System.DateTime)
								this.LastUpdateDateTime = (System.DateTime?)value;
							break;
						case "QtyDosage":

							if (value == null || value is System.Decimal)
								this.QtyDosage = (System.Decimal?)value;
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
		/// Maps to ItemConsumption.ItemID
		/// </summary>
		virtual public System.String ItemID
		{
			get
			{
				return base.GetSystemString(ItemConsumptionMetadata.ColumnNames.ItemID);
			}

			set
			{
				base.SetSystemString(ItemConsumptionMetadata.ColumnNames.ItemID, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.DetailItemID
		/// </summary>
		virtual public System.String DetailItemID
		{
			get
			{
				return base.GetSystemString(ItemConsumptionMetadata.ColumnNames.DetailItemID);
			}

			set
			{
				base.SetSystemString(ItemConsumptionMetadata.ColumnNames.DetailItemID, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.Qty
		/// </summary>
		virtual public System.Decimal? Qty
		{
			get
			{
				return base.GetSystemDecimal(ItemConsumptionMetadata.ColumnNames.Qty);
			}

			set
			{
				base.SetSystemDecimal(ItemConsumptionMetadata.ColumnNames.Qty, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.SRItemUnit
		/// </summary>
		virtual public System.String SRItemUnit
		{
			get
			{
				return base.GetSystemString(ItemConsumptionMetadata.ColumnNames.SRItemUnit);
			}

			set
			{
				base.SetSystemString(ItemConsumptionMetadata.ColumnNames.SRItemUnit, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(ItemConsumptionMetadata.ColumnNames.LastUpdateDateTime);
			}

			set
			{
				base.SetSystemDateTime(ItemConsumptionMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(ItemConsumptionMetadata.ColumnNames.LastUpdateByUserID);
			}

			set
			{
				base.SetSystemString(ItemConsumptionMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.QtyDosage
		/// </summary>
		virtual public System.Decimal? QtyDosage
		{
			get
			{
				return base.GetSystemDecimal(ItemConsumptionMetadata.ColumnNames.QtyDosage);
			}

			set
			{
				base.SetSystemDecimal(ItemConsumptionMetadata.ColumnNames.QtyDosage, value);
			}
		}
		/// <summary>
		/// Maps to ItemConsumption.SRDosageUnit
		/// </summary>
		virtual public System.String SRDosageUnit
		{
			get
			{
				return base.GetSystemString(ItemConsumptionMetadata.ColumnNames.SRDosageUnit);
			}

			set
			{
				base.SetSystemString(ItemConsumptionMetadata.ColumnNames.SRDosageUnit, value);
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
			public esStrings(esItemConsumption entity)
			{
				this.entity = entity;
			}
			public System.String ItemID
			{
				get
				{
					System.String data = entity.ItemID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ItemID = null;
					else entity.ItemID = Convert.ToString(value);
				}
			}
			public System.String DetailItemID
			{
				get
				{
					System.String data = entity.DetailItemID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DetailItemID = null;
					else entity.DetailItemID = Convert.ToString(value);
				}
			}
			public System.String Qty
			{
				get
				{
					System.Decimal? data = entity.Qty;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Qty = null;
					else entity.Qty = Convert.ToDecimal(value);
				}
			}
			public System.String SRItemUnit
			{
				get
				{
					System.String data = entity.SRItemUnit;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SRItemUnit = null;
					else entity.SRItemUnit = Convert.ToString(value);
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
			public System.String QtyDosage
			{
				get
				{
					System.Decimal? data = entity.QtyDosage;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QtyDosage = null;
					else entity.QtyDosage = Convert.ToDecimal(value);
				}
			}
			public System.String SRDosageUnit
			{
				get
				{
					System.String data = entity.SRDosageUnit;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SRDosageUnit = null;
					else entity.SRDosageUnit = Convert.ToString(value);
				}
			}
			private esItemConsumption entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esItemConsumptionQuery query)
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
				throw new Exception("esItemConsumption can only hold one record of data");
			}

			return dataFound;
		}
		#endregion

		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class ItemConsumption : esItemConsumption
	{
	}

	[Serializable]
	abstract public class esItemConsumptionQuery : esDynamicQuery
	{

		override protected IMetadata Meta
		{
			get
			{
				return ItemConsumptionMetadata.Meta();
			}
		}

		public esQueryItem ItemID
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.ItemID, esSystemType.String);
			}
		}

		public esQueryItem DetailItemID
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.DetailItemID, esSystemType.String);
			}
		}

		public esQueryItem Qty
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.Qty, esSystemType.Decimal);
			}
		}

		public esQueryItem SRItemUnit
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.SRItemUnit, esSystemType.String);
			}
		}

		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		}

		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		}

		public esQueryItem QtyDosage
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.QtyDosage, esSystemType.Decimal);
			}
		}

		public esQueryItem SRDosageUnit
		{
			get
			{
				return new esQueryItem(this, ItemConsumptionMetadata.ColumnNames.SRDosageUnit, esSystemType.String);
			}
		}

	}

	[System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("ItemConsumptionCollection")]
	public partial class ItemConsumptionCollection : esItemConsumptionCollection, IEnumerable<ItemConsumption>
	{
		public ItemConsumptionCollection()
		{

		}

		public static implicit operator List<ItemConsumption>(ItemConsumptionCollection coll)
		{
			List<ItemConsumption> list = new List<ItemConsumption>();

			foreach (ItemConsumption emp in coll)
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
				return ItemConsumptionMetadata.Meta();
			}
		}

		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ItemConsumptionQuery();
				this.InitQuery(query);
			}
			return this.query;
		}

		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new ItemConsumption(row);
		}

		override protected esEntity CreateEntity()
		{
			return new ItemConsumption();
		}

		#endregion

		[BrowsableAttribute(false)]
		public ItemConsumptionQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ItemConsumptionQuery();
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
		public bool Load(ItemConsumptionQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}

		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public ItemConsumption AddNew()
		{
			ItemConsumption entity = base.AddNewEntity() as ItemConsumption;

			return entity;
		}
		public ItemConsumption FindByPrimaryKey(String itemID, String detailItemID)
		{
			return base.FindByPrimaryKey(itemID, detailItemID) as ItemConsumption;
		}

		#region IEnumerable< ItemConsumption> Members

		IEnumerator<ItemConsumption> IEnumerable<ItemConsumption>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while (iterator.MoveNext())
			{
				yield return iterator.Current as ItemConsumption;
			}
		}

		#endregion

		private ItemConsumptionQuery query;
	}


	/// <summary>
	/// Encapsulates the 'ItemConsumption' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("ItemConsumption ({ItemID, DetailItemID})")]
	[Serializable]
	public partial class ItemConsumption : esItemConsumption
	{
		public ItemConsumption()
		{
		}

		public ItemConsumption(DataRow row)
			: base(row)
		{
		}

		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return ItemConsumptionMetadata.Meta();
			}
		}

		override protected esItemConsumptionQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ItemConsumptionQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion

		[BrowsableAttribute(false)]
		public ItemConsumptionQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ItemConsumptionQuery();
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
		public bool Load(ItemConsumptionQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}

		private ItemConsumptionQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class ItemConsumptionQuery : esItemConsumptionQuery
	{
		public ItemConsumptionQuery()
		{

		}

		public ItemConsumptionQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}

		override protected string GetQueryName()
		{
			return "ItemConsumptionQuery";
		}
	}

	[Serializable]
	public partial class ItemConsumptionMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected ItemConsumptionMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.ItemID, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.ItemID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			c.HasDefault = true;
			c.Default = @"('')";
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.DetailItemID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.DetailItemID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			c.HasDefault = true;
			c.Default = @"('')";
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.Qty, 2, typeof(System.Decimal), esSystemType.Decimal);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.Qty;
			c.NumericPrecision = 10;
			c.NumericScale = 2;
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.SRItemUnit, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.SRItemUnit;
			c.CharacterMaxLength = 20;
			c.HasDefault = true;
			c.Default = @"('')";
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.LastUpdateDateTime, 4, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.LastUpdateByUserID, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.QtyDosage, 6, typeof(System.Decimal), esSystemType.Decimal);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.QtyDosage;
			c.NumericPrecision = 10;
			c.NumericScale = 2;
			c.IsNullable = true;
			_columns.Add(c);

			c = new esColumnMetadata(ItemConsumptionMetadata.ColumnNames.SRDosageUnit, 7, typeof(System.String), esSystemType.String);
			c.PropertyName = ItemConsumptionMetadata.PropertyNames.SRDosageUnit;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c);


		}
		#endregion

		static public ItemConsumptionMetadata Meta()
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
			public const string ItemID = "ItemID";
			public const string DetailItemID = "DetailItemID";
			public const string Qty = "Qty";
			public const string SRItemUnit = "SRItemUnit";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string QtyDosage = "QtyDosage";
			public const string SRDosageUnit = "SRDosageUnit";
		}
		#endregion

		#region PropertyNames
		public class PropertyNames
		{
			public const string ItemID = "ItemID";
			public const string DetailItemID = "DetailItemID";
			public const string Qty = "Qty";
			public const string SRItemUnit = "SRItemUnit";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string QtyDosage = "QtyDosage";
			public const string SRDosageUnit = "SRDosageUnit";
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
			lock (typeof(ItemConsumptionMetadata))
			{
				if (ItemConsumptionMetadata.mapDelegates == null)
				{
					ItemConsumptionMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
				}

				if (ItemConsumptionMetadata.meta == null)
				{
					ItemConsumptionMetadata.meta = new ItemConsumptionMetadata();
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

				meta.AddTypeMap("ItemID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("DetailItemID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Qty", new esTypeMap("numeric", "System.Decimal"));
				meta.AddTypeMap("SRItemUnit", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("QtyDosage", new esTypeMap("numeric", "System.Decimal"));
				meta.AddTypeMap("SRDosageUnit", new esTypeMap("varchar", "System.String"));


				meta.Source = "ItemConsumption";
				meta.Destination = "ItemConsumption";
				meta.spInsert = "proc_ItemConsumptionInsert";
				meta.spUpdate = "proc_ItemConsumptionUpdate";
				meta.spDelete = "proc_ItemConsumptionDelete";
				meta.spLoadAll = "proc_ItemConsumptionLoadAll";
				meta.spLoadByPrimaryKey = "proc_ItemConsumptionLoadByPrimaryKey";

				this._providerMetadataMaps["esDefault"] = meta;
			}

			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private ItemConsumptionMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}
