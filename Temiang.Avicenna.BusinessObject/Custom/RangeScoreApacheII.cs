/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-07-31 12:23:04 PM
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
	abstract public class esRangeScoreApacheIICollection : esEntityCollectionWAuditLog
	{
		public esRangeScoreApacheIICollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "RangeScoreApacheIICollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esRangeScoreApacheIIQuery query)
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
			this.InitQuery(query as esRangeScoreApacheIIQuery);
		}
		#endregion
			
		virtual public RangeScoreApacheII DetachEntity(RangeScoreApacheII entity)
		{
			return base.DetachEntity(entity) as RangeScoreApacheII;
		}
		
		virtual public RangeScoreApacheII AttachEntity(RangeScoreApacheII entity)
		{
			return base.AttachEntity(entity) as RangeScoreApacheII;
		}
		
		virtual public void Combine(RangeScoreApacheIICollection collection)
		{
			base.Combine(collection);
		}
		
		new public RangeScoreApacheII this[int index]
		{
			get
			{
				return base[index] as RangeScoreApacheII;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(RangeScoreApacheII);
		}
	}

	[Serializable]
	abstract public class esRangeScoreApacheII : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esRangeScoreApacheIIQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esRangeScoreApacheII()
		{
		}
	
		public esRangeScoreApacheII(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int32 iD)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(iD);
			else
				return LoadByPrimaryKeyStoredProcedure(iD);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 iD)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(iD);
			else
				return LoadByPrimaryKeyStoredProcedure(iD);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int32 iD)
		{
			esRangeScoreApacheIIQuery query = this.GetDynamicQuery();
			query.Where(query.ID == iD);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int32 iD)
		{
			esParameters parms = new esParameters();
			parms.Add("ID", iD);
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
						case "ID": this.str.ID = (string)value; break;
						case "QuestionID": this.str.QuestionID = (string)value; break;
						case "MinValue": this.str.MinValue = (string)value; break;
						case "MaxValue": this.str.MaxValue = (string)value; break;
						case "Point": this.str.Point = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "ID":
						
							if (value == null || value is System.Int32)
								this.ID = (System.Int32?)value;
							break;
						case "MinValue":
						
							if (value == null || value is System.Decimal)
								this.MinValue = (System.Decimal?)value;
							break;
						case "MaxValue":
						
							if (value == null || value is System.Decimal)
								this.MaxValue = (System.Decimal?)value;
							break;
						case "Point":
						
							if (value == null || value is System.Int32)
								this.Point = (System.Int32?)value;
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
		/// Maps to RangeScoreApacheII.ID
		/// </summary>
		virtual public System.Int32? ID
		{
			get
			{
				return base.GetSystemInt32(RangeScoreApacheIIMetadata.ColumnNames.ID);
			}
			
			set
			{
				base.SetSystemInt32(RangeScoreApacheIIMetadata.ColumnNames.ID, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.QuestionID
		/// </summary>
		virtual public System.String QuestionID
		{
			get
			{
				return base.GetSystemString(RangeScoreApacheIIMetadata.ColumnNames.QuestionID);
			}
			
			set
			{
				base.SetSystemString(RangeScoreApacheIIMetadata.ColumnNames.QuestionID, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.MinValue
		/// </summary>
		virtual public System.Decimal? MinValue
		{
			get
			{
				return base.GetSystemDecimal(RangeScoreApacheIIMetadata.ColumnNames.MinValue);
			}
			
			set
			{
				base.SetSystemDecimal(RangeScoreApacheIIMetadata.ColumnNames.MinValue, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.MaxValue
		/// </summary>
		virtual public System.Decimal? MaxValue
		{
			get
			{
				return base.GetSystemDecimal(RangeScoreApacheIIMetadata.ColumnNames.MaxValue);
			}
			
			set
			{
				base.SetSystemDecimal(RangeScoreApacheIIMetadata.ColumnNames.MaxValue, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.Point
		/// </summary>
		virtual public System.Int32? Point
		{
			get
			{
				return base.GetSystemInt32(RangeScoreApacheIIMetadata.ColumnNames.Point);
			}
			
			set
			{
				base.SetSystemInt32(RangeScoreApacheIIMetadata.ColumnNames.Point, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to RangeScoreApacheII.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esRangeScoreApacheII entity)
			{
				this.entity = entity;
			}
			public System.String ID
			{
				get
				{
					System.Int32? data = entity.ID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ID = null;
					else entity.ID = Convert.ToInt32(value);
				}
			}
			public System.String QuestionID
			{
				get
				{
					System.String data = entity.QuestionID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QuestionID = null;
					else entity.QuestionID = Convert.ToString(value);
				}
			}
			public System.String MinValue
			{
				get
				{
					System.Decimal? data = entity.MinValue;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.MinValue = null;
					else entity.MinValue = Convert.ToDecimal(value);
				}
			}
			public System.String MaxValue
			{
				get
				{
					System.Decimal? data = entity.MaxValue;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.MaxValue = null;
					else entity.MaxValue = Convert.ToDecimal(value);
				}
			}
			public System.String Point
			{
				get
				{
					System.Int32? data = entity.Point;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Point = null;
					else entity.Point = Convert.ToInt32(value);
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
			
			private esRangeScoreApacheII entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esRangeScoreApacheIIQuery query)
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
				throw new Exception("esRangeScoreApacheII can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


    public partial class RangeScoreApacheII : esRangeScoreApacheII
    {
        public static int GetPoint(string questionID, decimal value)
        {
            var collection = new RangeScoreApacheIICollection();
            var query = new RangeScoreApacheIIQuery();

            query.Where(query.QuestionID == questionID);
            query.Where(query.MinValue <= value);
            query.Where(query.MaxValue >= value);

            collection.Load(query);

            if (collection.Count > 0)
            {
                return collection[0].Point ?? 0;
            }

            return 0;
        }
    }

    [Serializable]
	abstract public class esRangeScoreApacheIIQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return RangeScoreApacheIIMetadata.Meta();
			}
		}	
			
		public esQueryItem ID
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.ID, esSystemType.Int32);
			}
		} 
		public esQueryItem QuestionID
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.QuestionID, esSystemType.String);
			}
		} 
		public esQueryItem MinValue
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.MinValue, esSystemType.Decimal);
			}
		} 
		public esQueryItem MaxValue
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.MaxValue, esSystemType.Decimal);
			}
		} 
		public esQueryItem Point
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.Point, esSystemType.Int32);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, RangeScoreApacheIIMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("RangeScoreApacheIICollection")]
	public partial class RangeScoreApacheIICollection : esRangeScoreApacheIICollection, IEnumerable< RangeScoreApacheII>
	{
		public RangeScoreApacheIICollection()
		{

		}	
		
		public static implicit operator List< RangeScoreApacheII>(RangeScoreApacheIICollection coll)
		{
			List< RangeScoreApacheII> list = new List< RangeScoreApacheII>();
			
			foreach (RangeScoreApacheII emp in coll)
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
				return  RangeScoreApacheIIMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new RangeScoreApacheIIQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new RangeScoreApacheII(row);
		}

		override protected esEntity CreateEntity()
		{
			return new RangeScoreApacheII();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public RangeScoreApacheIIQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new RangeScoreApacheIIQuery();
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
		public bool Load(RangeScoreApacheIIQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public RangeScoreApacheII AddNew()
		{
			RangeScoreApacheII entity = base.AddNewEntity() as RangeScoreApacheII;
			
			return entity;		
		}
		public RangeScoreApacheII FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as RangeScoreApacheII;
		}

		#region IEnumerable< RangeScoreApacheII> Members

		IEnumerator< RangeScoreApacheII> IEnumerable< RangeScoreApacheII>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as RangeScoreApacheII;
			}
		}

		#endregion
		
		private RangeScoreApacheIIQuery query;
	}


	/// <summary>
	/// Encapsulates the 'RangeScoreApacheII' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("RangeScoreApacheII ({StandardReferenceID})")]
	[Serializable]
	public partial class RangeScoreApacheII : esRangeScoreApacheII
	{
		public RangeScoreApacheII()
		{
		}	
	
		public RangeScoreApacheII(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return RangeScoreApacheIIMetadata.Meta();
			}
		}	
	
		override protected esRangeScoreApacheIIQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new RangeScoreApacheIIQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public RangeScoreApacheIIQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new RangeScoreApacheIIQuery();
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
		public bool Load(RangeScoreApacheIIQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private RangeScoreApacheIIQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class RangeScoreApacheIIQuery : esRangeScoreApacheIIQuery
	{
		public RangeScoreApacheIIQuery()
		{

		}		
		
		public RangeScoreApacheIIQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "RangeScoreApacheIIQuery";
        }
	}

	[Serializable]
	public partial class RangeScoreApacheIIMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected RangeScoreApacheIIMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.ID, 0, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.ID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.QuestionID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.QuestionID;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.MinValue, 2, typeof(System.Decimal), esSystemType.Decimal);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.MinValue;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.MaxValue, 3, typeof(System.Decimal), esSystemType.Decimal);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.MaxValue;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.Point, 4, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.Point;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateDateTime, 5, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.LastUpdateDateTime;
			c.HasDefault = true;
			c.Default = @"(getdate())";
			_columns.Add(c); 
				
			c = new esColumnMetadata(RangeScoreApacheIIMetadata.ColumnNames.LastUpdateByUserID, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = RangeScoreApacheIIMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public RangeScoreApacheIIMetadata Meta()
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
			public const string ID = "ID";
			public const string QuestionID = "QuestionID";
			public const string MinValue = "MinValue";
			public const string MaxValue = "MaxValue";
			public const string Point = "Point";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string ID = "ID";
			public const string QuestionID = "QuestionID";
			public const string MinValue = "MinValue";
			public const string MaxValue = "MaxValue";
			public const string Point = "Point";
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
			lock (typeof(RangeScoreApacheIIMetadata))
			{
				if(RangeScoreApacheIIMetadata.mapDelegates == null)
				{
					RangeScoreApacheIIMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (RangeScoreApacheIIMetadata.meta == null)
				{
					RangeScoreApacheIIMetadata.meta = new RangeScoreApacheIIMetadata();
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
				
				meta.AddTypeMap("ID", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("QuestionID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("MinValue", new esTypeMap("decimal", "System.Decimal"));
				meta.AddTypeMap("MaxValue", new esTypeMap("decimal", "System.Decimal"));
				meta.AddTypeMap("Point", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "RangeScoreApacheII";
				meta.Destination = "RangeScoreApacheII";
				meta.spInsert = "proc_RangeScoreApacheIIInsert";				
				meta.spUpdate = "proc_RangeScoreApacheIIUpdate";		
				meta.spDelete = "proc_RangeScoreApacheIIDelete";
				meta.spLoadAll = "proc_RangeScoreApacheIILoadAll";
				meta.spLoadByPrimaryKey = "proc_RangeScoreApacheIILoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private RangeScoreApacheIIMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		