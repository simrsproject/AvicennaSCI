/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 9/8/2025 1:58:47 PM
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
	abstract public class esDietBridgingCollection : esEntityCollectionWAuditLog
	{
		public esDietBridgingCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "DietBridgingCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esDietBridgingQuery query)
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
			this.InitQuery(query as esDietBridgingQuery);
		}
		#endregion
			
		virtual public DietBridging DetachEntity(DietBridging entity)
		{
			return base.DetachEntity(entity) as DietBridging;
		}
		
		virtual public DietBridging AttachEntity(DietBridging entity)
		{
			return base.AttachEntity(entity) as DietBridging;
		}
		
		virtual public void Combine(DietBridgingCollection collection)
		{
			base.Combine(collection);
		}
		
		new public DietBridging this[int index]
		{
			get
			{
				return base[index] as DietBridging;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(DietBridging);
		}
	}

	[Serializable]
	abstract public class esDietBridging : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esDietBridgingQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esDietBridging()
		{
		}
	
		public esDietBridging(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(String dietId, String sRBridgingType, String bridgingID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(dietId, sRBridgingType, bridgingID);
			else
				return LoadByPrimaryKeyStoredProcedure(dietId, sRBridgingType, bridgingID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String dietId, String sRBridgingType, String bridgingID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(dietId, sRBridgingType, bridgingID);
			else
				return LoadByPrimaryKeyStoredProcedure(dietId, sRBridgingType, bridgingID);
		}
	
		private bool LoadByPrimaryKeyDynamic(String dietId, String sRBridgingType, String bridgingID)
		{
			esDietBridgingQuery query = this.GetDynamicQuery();
			query.Where(query.DietId == dietId, query.SRBridgingType == sRBridgingType, query.BridgingID == bridgingID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(String dietId, String sRBridgingType, String bridgingID)
		{
			esParameters parms = new esParameters();
			parms.Add("DietId",dietId);
			parms.Add("SRBridgingType",sRBridgingType);
			parms.Add("BridgingID",bridgingID);
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
						case "DietId": this.str.DietId = (string)value; break;
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
		/// Maps to DietBridging.DietId
		/// </summary>
		virtual public System.String DietId
		{
			get
			{
				return base.GetSystemString(DietBridgingMetadata.ColumnNames.DietId);
			}
			
			set
			{
				base.SetSystemString(DietBridgingMetadata.ColumnNames.DietId, value);
			}
		}
		/// <summary>
		/// Maps to DietBridging.SRBridgingType
		/// </summary>
		virtual public System.String SRBridgingType
		{
			get
			{
				return base.GetSystemString(DietBridgingMetadata.ColumnNames.SRBridgingType);
			}
			
			set
			{
				base.SetSystemString(DietBridgingMetadata.ColumnNames.SRBridgingType, value);
			}
		}
		/// <summary>
		/// Maps to DietBridging.BridgingID
		/// </summary>
		virtual public System.String BridgingID
		{
			get
			{
				return base.GetSystemString(DietBridgingMetadata.ColumnNames.BridgingID);
			}
			
			set
			{
				base.SetSystemString(DietBridgingMetadata.ColumnNames.BridgingID, value);
			}
		}
		/// <summary>
		/// Maps to DietBridging.BridgingName
		/// </summary>
		virtual public System.String BridgingName
		{
			get
			{
				return base.GetSystemString(DietBridgingMetadata.ColumnNames.BridgingName);
			}
			
			set
			{
				base.SetSystemString(DietBridgingMetadata.ColumnNames.BridgingName, value);
			}
		}
        /// <summary>
        /// Maps to DietBridging.IsActive
        /// </summary>
        virtual public System.Boolean? IsActive
        {
            get
            {
                return base.GetSystemBoolean(DietBridgingMetadata.ColumnNames.IsActive);
            }

            set
            {
                base.SetSystemBoolean(DietBridgingMetadata.ColumnNames.IsActive, value);
            }
        }
        /// <summary>
        /// Maps to DietBridging.LastUpdateDateTime
        /// </summary>
        virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(DietBridgingMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(DietBridgingMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to DietBridging.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(DietBridgingMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(DietBridgingMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esDietBridging entity)
			{
				this.entity = entity;
			}
			public System.String DietId
			{
				get
				{
					System.String data = entity.DietId;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DietId = null;
					else entity.DietId = Convert.ToString(value);
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
			private esDietBridging entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esDietBridgingQuery query)
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
				throw new Exception("esDietBridging can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class DietBridging : esDietBridging
	{	
	}

	[Serializable]
	abstract public class esDietBridgingQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return DietBridgingMetadata.Meta();
			}
		}	
			
		public esQueryItem DietId
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.DietId, esSystemType.String);
			}
		} 
			
		public esQueryItem SRBridgingType
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.SRBridgingType, esSystemType.String);
			}
		} 
			
		public esQueryItem BridgingID
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.BridgingID, esSystemType.String);
			}
		} 
			
		public esQueryItem BridgingName
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.BridgingName, esSystemType.String);
			}
		}
        public esQueryItem IsActive
        {
            get
            {
                return new esQueryItem(this, DietBridgingMetadata.ColumnNames.IsActive, esSystemType.Boolean);
            }
        }

        public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
			
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, DietBridgingMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	
	}

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("DietBridgingCollection")]
	public partial class DietBridgingCollection : esDietBridgingCollection, IEnumerable< DietBridging>
	{
		public DietBridgingCollection()
		{

		}	
		
		public static implicit operator List< DietBridging>(DietBridgingCollection coll)
		{
			List< DietBridging> list = new List< DietBridging>();
			
			foreach (DietBridging emp in coll)
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
				return  DietBridgingMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DietBridgingQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new DietBridging(row);
		}

		override protected esEntity CreateEntity()
		{
			return new DietBridging();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public DietBridgingQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DietBridgingQuery();
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
		public bool Load(DietBridgingQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public DietBridging AddNew()
		{
			DietBridging entity = base.AddNewEntity() as DietBridging;
			
			return entity;		
		}
		public DietBridging FindByPrimaryKey(String dietId, String sRBridgingType, String bridgingID)
		{
			return base.FindByPrimaryKey(dietId, sRBridgingType, bridgingID) as DietBridging;
		}

		#region IEnumerable< DietBridging> Members

		IEnumerator< DietBridging> IEnumerable< DietBridging>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as DietBridging;
			}
		}

		#endregion
		
		private DietBridgingQuery query;
	}


	/// <summary>
	/// Encapsulates the 'DietBridging' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("DietBridging ({DietId, SRBridgingType, BridgingID})")]
	[Serializable]
	public partial class DietBridging : esDietBridging
	{
		public DietBridging()
		{
		}	
	
		public DietBridging(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return DietBridgingMetadata.Meta();
			}
		}	
	
		override protected esDietBridgingQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DietBridgingQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public DietBridgingQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DietBridgingQuery();
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
		public bool Load(DietBridgingQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private DietBridgingQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class DietBridgingQuery : esDietBridgingQuery
	{
		public DietBridgingQuery()
		{

		}		
		
		public DietBridgingQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "DietBridgingQuery";
        }
	}

	[Serializable]
	public partial class DietBridgingMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected DietBridgingMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.DietId, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = DietBridgingMetadata.PropertyNames.DietId;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.SRBridgingType, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = DietBridgingMetadata.PropertyNames.SRBridgingType;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.BridgingID, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = DietBridgingMetadata.PropertyNames.BridgingID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 15;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.BridgingName, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = DietBridgingMetadata.PropertyNames.BridgingName;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c);

            c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.IsActive, 4, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = DietBridgingMetadata.PropertyNames.IsActive;
            _columns.Add(c);

            c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.LastUpdateDateTime, 5, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = DietBridgingMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DietBridgingMetadata.ColumnNames.LastUpdateByUserID, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = DietBridgingMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				

		}
		#endregion
	
		static public DietBridgingMetadata Meta()
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
			public const string DietId = "DietId";
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
			public const string DietId = "DietId";
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
			lock (typeof(DietBridgingMetadata))
			{
				if(DietBridgingMetadata.mapDelegates == null)
				{
					DietBridgingMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (DietBridgingMetadata.meta == null)
				{
					DietBridgingMetadata.meta = new DietBridgingMetadata();
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
				
				meta.AddTypeMap("DietId", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("SRBridgingType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("BridgingID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("BridgingName", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "DietBridging";
				meta.Destination = "DietBridging";
				meta.spInsert = "proc_DietBridgingInsert";				
				meta.spUpdate = "proc_DietBridgingUpdate";		
				meta.spDelete = "proc_DietBridgingDelete";
				meta.spLoadAll = "proc_DietBridgingLoadAll";
				meta.spLoadByPrimaryKey = "proc_DietBridgingLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private DietBridgingMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
