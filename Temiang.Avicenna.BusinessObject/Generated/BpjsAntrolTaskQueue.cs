/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-25 03:59:50 PM
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
	abstract public class esBpjsAntrolTaskQueueCollection : esEntityCollectionWAuditLog
	{
		public esBpjsAntrolTaskQueueCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "BpjsAntrolTaskQueueCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esBpjsAntrolTaskQueueQuery query)
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
			this.InitQuery(query as esBpjsAntrolTaskQueueQuery);
		}
		#endregion
			
		virtual public BpjsAntrolTaskQueue DetachEntity(BpjsAntrolTaskQueue entity)
		{
			return base.DetachEntity(entity) as BpjsAntrolTaskQueue;
		}
		
		virtual public BpjsAntrolTaskQueue AttachEntity(BpjsAntrolTaskQueue entity)
		{
			return base.AttachEntity(entity) as BpjsAntrolTaskQueue;
		}
		
		virtual public void Combine(BpjsAntrolTaskQueueCollection collection)
		{
			base.Combine(collection);
		}
		
		new public BpjsAntrolTaskQueue this[int index]
		{
			get
			{
				return base[index] as BpjsAntrolTaskQueue;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(BpjsAntrolTaskQueue);
		}
	}

	[Serializable]
	abstract public class esBpjsAntrolTaskQueue : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esBpjsAntrolTaskQueueQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esBpjsAntrolTaskQueue()
		{
		}
	
		public esBpjsAntrolTaskQueue(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(String booking, Int32 taskId)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(booking, taskId);
			else
				return LoadByPrimaryKeyStoredProcedure(booking, taskId);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String booking, Int32 taskId)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(booking, taskId);
			else
				return LoadByPrimaryKeyStoredProcedure(booking, taskId);
		}
	
		private bool LoadByPrimaryKeyDynamic(String booking, Int32 taskId)
		{
			esBpjsAntrolTaskQueueQuery query = this.GetDynamicQuery();
			query.Where(query.Booking == booking, query.TaskId == taskId);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(String booking, Int32 taskId)
		{
			esParameters parms = new esParameters();
			parms.Add("Booking", booking);
			parms.Add("TaskId", taskId);
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
						case "Booking": this.str.Booking = (string)value; break;
						case "TaskId": this.str.TaskId = (string)value; break;
						case "EventMs": this.str.EventMs = (string)value; break;
						case "StartedMs": this.str.StartedMs = (string)value; break;
						case "Status": this.str.Status = (string)value; break;
						case "Attempts": this.str.Attempts = (string)value; break;
						case "DueUtc": this.str.DueUtc = (string)value; break;
						case "Detail": this.str.Detail = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "TaskId":
						
							if (value == null || value is System.Int32)
								this.TaskId = (System.Int32?)value;
							break;
						case "EventMs":
						
							if (value == null || value is System.Int64)
								this.EventMs = (System.Int64?)value;
							break;
						case "StartedMs":
						
							if (value == null || value is System.Int64)
								this.StartedMs = (System.Int64?)value;
							break;
						case "Attempts":
						
							if (value == null || value is System.Int32)
								this.Attempts = (System.Int32?)value;
							break;
						case "DueUtc":
						
							if (value == null || value is System.DateTime)
								this.DueUtc = (System.DateTime?)value;
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
		/// Maps to BpjsAntrolTaskQueue.Booking
		/// </summary>
		virtual public System.String Booking
		{
			get
			{
				return base.GetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Booking);
			}
			
			set
			{
				base.SetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Booking, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.TaskId
		/// </summary>
		virtual public System.Int32? TaskId
		{
			get
			{
				return base.GetSystemInt32(BpjsAntrolTaskQueueMetadata.ColumnNames.TaskId);
			}
			
			set
			{
				base.SetSystemInt32(BpjsAntrolTaskQueueMetadata.ColumnNames.TaskId, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.EventMs
		/// </summary>
		virtual public System.Int64? EventMs
		{
			get
			{
				return base.GetSystemInt64(BpjsAntrolTaskQueueMetadata.ColumnNames.EventMs);
			}
			
			set
			{
				base.SetSystemInt64(BpjsAntrolTaskQueueMetadata.ColumnNames.EventMs, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.StartedMs
		/// </summary>
		virtual public System.Int64? StartedMs
		{
			get
			{
				return base.GetSystemInt64(BpjsAntrolTaskQueueMetadata.ColumnNames.StartedMs);
			}
			
			set
			{
				base.SetSystemInt64(BpjsAntrolTaskQueueMetadata.ColumnNames.StartedMs, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.Status
		/// </summary>
		virtual public System.String Status
		{
			get
			{
				return base.GetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Status);
			}
			
			set
			{
				base.SetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Status, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.Attempts
		/// </summary>
		virtual public System.Int32? Attempts
		{
			get
			{
				return base.GetSystemInt32(BpjsAntrolTaskQueueMetadata.ColumnNames.Attempts);
			}
			
			set
			{
				base.SetSystemInt32(BpjsAntrolTaskQueueMetadata.ColumnNames.Attempts, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.DueUtc
		/// </summary>
		virtual public System.DateTime? DueUtc
		{
			get
			{
				return base.GetSystemDateTime(BpjsAntrolTaskQueueMetadata.ColumnNames.DueUtc);
			}
			
			set
			{
				base.SetSystemDateTime(BpjsAntrolTaskQueueMetadata.ColumnNames.DueUtc, value);
			}
		}
		/// <summary>
		/// Maps to BpjsAntrolTaskQueue.Detail
		/// </summary>
		virtual public System.String Detail
		{
			get
			{
				return base.GetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Detail);
			}
			
			set
			{
				base.SetSystemString(BpjsAntrolTaskQueueMetadata.ColumnNames.Detail, value);
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
			public esStrings(esBpjsAntrolTaskQueue entity)
			{
				this.entity = entity;
			}
			public System.String Booking
			{
				get
				{
					System.String data = entity.Booking;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Booking = null;
					else entity.Booking = Convert.ToString(value);
				}
			}
			public System.String TaskId
			{
				get
				{
					System.Int32? data = entity.TaskId;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TaskId = null;
					else entity.TaskId = Convert.ToInt32(value);
				}
			}
			public System.String EventMs
			{
				get
				{
					System.Int64? data = entity.EventMs;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.EventMs = null;
					else entity.EventMs = Convert.ToInt64(value);
				}
			}
			public System.String StartedMs
			{
				get
				{
					System.Int64? data = entity.StartedMs;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.StartedMs = null;
					else entity.StartedMs = Convert.ToInt64(value);
				}
			}
			public System.String Status
			{
				get
				{
					System.String data = entity.Status;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Status = null;
					else entity.Status = Convert.ToString(value);
				}
			}
			public System.String Attempts
			{
				get
				{
					System.Int32? data = entity.Attempts;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Attempts = null;
					else entity.Attempts = Convert.ToInt32(value);
				}
			}
			public System.String DueUtc
			{
				get
				{
					System.DateTime? data = entity.DueUtc;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DueUtc = null;
					else entity.DueUtc = Convert.ToDateTime(value);
				}
			}
			public System.String Detail
			{
				get
				{
					System.String data = entity.Detail;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Detail = null;
					else entity.Detail = Convert.ToString(value);
				}
			}
			
			private esBpjsAntrolTaskQueue entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esBpjsAntrolTaskQueueQuery query)
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
				throw new Exception("esBpjsAntrolTaskQueue can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class BpjsAntrolTaskQueue : esBpjsAntrolTaskQueue
	{	
	}

	[Serializable]
	abstract public class esBpjsAntrolTaskQueueQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return BpjsAntrolTaskQueueMetadata.Meta();
			}
		}	
			
		public esQueryItem Booking
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.Booking, esSystemType.String);
			}
		} 
		public esQueryItem TaskId
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.TaskId, esSystemType.Int32);
			}
		} 
		public esQueryItem EventMs
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.EventMs, esSystemType.Int64);
			}
		} 
		public esQueryItem StartedMs
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.StartedMs, esSystemType.Int64);
			}
		} 
		public esQueryItem Status
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.Status, esSystemType.String);
			}
		} 
		public esQueryItem Attempts
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.Attempts, esSystemType.Int32);
			}
		} 
		public esQueryItem DueUtc
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.DueUtc, esSystemType.DateTime);
			}
		} 
		public esQueryItem Detail
		{
			get
			{
				return new esQueryItem(this, BpjsAntrolTaskQueueMetadata.ColumnNames.Detail, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("BpjsAntrolTaskQueueCollection")]
	public partial class BpjsAntrolTaskQueueCollection : esBpjsAntrolTaskQueueCollection, IEnumerable< BpjsAntrolTaskQueue>
	{
		public BpjsAntrolTaskQueueCollection()
		{

		}	
		
		public static implicit operator List< BpjsAntrolTaskQueue>(BpjsAntrolTaskQueueCollection coll)
		{
			List< BpjsAntrolTaskQueue> list = new List< BpjsAntrolTaskQueue>();
			
			foreach (BpjsAntrolTaskQueue emp in coll)
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
				return  BpjsAntrolTaskQueueMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new BpjsAntrolTaskQueueQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new BpjsAntrolTaskQueue(row);
		}

		override protected esEntity CreateEntity()
		{
			return new BpjsAntrolTaskQueue();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public BpjsAntrolTaskQueueQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new BpjsAntrolTaskQueueQuery();
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
		public bool Load(BpjsAntrolTaskQueueQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public BpjsAntrolTaskQueue AddNew()
		{
			BpjsAntrolTaskQueue entity = base.AddNewEntity() as BpjsAntrolTaskQueue;
			
			return entity;		
		}
		public BpjsAntrolTaskQueue FindByPrimaryKey(string booking, int taskId)
        {
            return base.FindByPrimaryKey(booking, taskId) as BpjsAntrolTaskQueue;
        }

		#region IEnumerable< BpjsAntrolTaskQueue> Members

		IEnumerator< BpjsAntrolTaskQueue> IEnumerable< BpjsAntrolTaskQueue>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as BpjsAntrolTaskQueue;
			}
		}

		#endregion
		
		private BpjsAntrolTaskQueueQuery query;
	}


	/// <summary>
	/// Encapsulates the 'BpjsAntrolTaskQueue' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("BpjsAntrolTaskQueue ({Booking}, {TaskId})")]
	[Serializable]
	public partial class BpjsAntrolTaskQueue : esBpjsAntrolTaskQueue
	{
		public BpjsAntrolTaskQueue()
		{
		}	
	
		public BpjsAntrolTaskQueue(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return BpjsAntrolTaskQueueMetadata.Meta();
			}
		}	
	
		override protected esBpjsAntrolTaskQueueQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new BpjsAntrolTaskQueueQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public BpjsAntrolTaskQueueQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new BpjsAntrolTaskQueueQuery();
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
		public bool Load(BpjsAntrolTaskQueueQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private BpjsAntrolTaskQueueQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class BpjsAntrolTaskQueueQuery : esBpjsAntrolTaskQueueQuery
	{
		public BpjsAntrolTaskQueueQuery()
		{

		}		
		
		public BpjsAntrolTaskQueueQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "BpjsAntrolTaskQueueQuery";
        }
	}

	[Serializable]
	public partial class BpjsAntrolTaskQueueMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected BpjsAntrolTaskQueueMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.Booking, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.Booking;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.TaskId, 1, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.TaskId;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.EventMs, 2, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.EventMs;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.StartedMs, 3, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.StartedMs;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.Status, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.Status;
			c.CharacterMaxLength = 12;
			c.HasDefault = true;
			c.Default = @"('Pending')";
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.Attempts, 5, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.Attempts;
			c.NumericPrecision = 10;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.DueUtc, 6, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.DueUtc;
			c.HasDefault = true;
			c.Default = @"(sysutcdatetime())";
			_columns.Add(c); 
				
			c = new esColumnMetadata(BpjsAntrolTaskQueueMetadata.ColumnNames.Detail, 7, typeof(System.String), esSystemType.String);
			c.PropertyName = BpjsAntrolTaskQueueMetadata.PropertyNames.Detail;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public BpjsAntrolTaskQueueMetadata Meta()
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
			public const string Booking = "Booking";
			public const string TaskId = "TaskId";
			public const string EventMs = "EventMs";
			public const string StartedMs = "StartedMs";
			public const string Status = "Status";
			public const string Attempts = "Attempts";
			public const string DueUtc = "DueUtc";
			public const string Detail = "Detail";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string Booking = "Booking";
			public const string TaskId = "TaskId";
			public const string EventMs = "EventMs";
			public const string StartedMs = "StartedMs";
			public const string Status = "Status";
			public const string Attempts = "Attempts";
			public const string DueUtc = "DueUtc";
			public const string Detail = "Detail";
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
			lock (typeof(BpjsAntrolTaskQueueMetadata))
			{
				if(BpjsAntrolTaskQueueMetadata.mapDelegates == null)
				{
					BpjsAntrolTaskQueueMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (BpjsAntrolTaskQueueMetadata.meta == null)
				{
					BpjsAntrolTaskQueueMetadata.meta = new BpjsAntrolTaskQueueMetadata();
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
				
				meta.AddTypeMap("Booking", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("TaskId", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("EventMs", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("StartedMs", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("Status", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Attempts", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("DueUtc", new esTypeMap("datetime2", "System.DateTime"));
				meta.AddTypeMap("Detail", new esTypeMap("nvarchar", "System.String"));
		

				meta.Source = "BpjsAntrolTaskQueue";
				meta.Destination = "BpjsAntrolTaskQueue";
				meta.spInsert = "proc_BpjsAntrolTaskQueueInsert";				
				meta.spUpdate = "proc_BpjsAntrolTaskQueueUpdate";		
				meta.spDelete = "proc_BpjsAntrolTaskQueueDelete";
				meta.spLoadAll = "proc_BpjsAntrolTaskQueueLoadAll";
				meta.spLoadByPrimaryKey = "proc_BpjsAntrolTaskQueueLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private BpjsAntrolTaskQueueMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
