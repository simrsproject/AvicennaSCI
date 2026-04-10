/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 06/03/2025 09:54:23
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
	abstract public class esSatuSehatILPTemplateCollection : esEntityCollectionWAuditLog
	{
		public esSatuSehatILPTemplateCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "SatuSehatILPTemplateCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateQuery query)
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
			this.InitQuery(query as esSatuSehatILPTemplateQuery);
		}
		#endregion
			
		virtual public SatuSehatILPTemplate DetachEntity(SatuSehatILPTemplate entity)
		{
			return base.DetachEntity(entity) as SatuSehatILPTemplate;
		}
		
		virtual public SatuSehatILPTemplate AttachEntity(SatuSehatILPTemplate entity)
		{
			return base.AttachEntity(entity) as SatuSehatILPTemplate;
		}
		
		virtual public void Combine(SatuSehatILPTemplateCollection collection)
		{
			base.Combine(collection);
		}
		
		new public SatuSehatILPTemplate this[int index]
		{
			get
			{
				return base[index] as SatuSehatILPTemplate;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(SatuSehatILPTemplate);
		}
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplate : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esSatuSehatILPTemplateQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esSatuSehatILPTemplate()
		{
		}
	
		public esSatuSehatILPTemplate(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int32 templateID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 templateID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int32 templateID)
		{
			esSatuSehatILPTemplateQuery query = this.GetDynamicQuery();
			query.Where(query.TemplateID==templateID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int32 templateID)
		{
			esParameters parms = new esParameters();
			parms.Add("TemplateID",templateID);
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
						case "TemplateID": this.str.TemplateID = (string)value; break;
						case "TemplateName": this.str.TemplateName = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "TemplateID":
						
							if (value == null || value is System.Int32)
								this.TemplateID = (System.Int32?)value;
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
		/// Maps to SatuSehatILPTemplate.TemplateID
		/// </summary>
		virtual public System.Int32? TemplateID
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateMetadata.ColumnNames.TemplateID);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateMetadata.ColumnNames.TemplateID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplate.TemplateName
		/// </summary>
		virtual public System.String TemplateName
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateMetadata.ColumnNames.TemplateName);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateMetadata.ColumnNames.TemplateName, value);
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
			public esStrings(esSatuSehatILPTemplate entity)
			{
				this.entity = entity;
			}
			public System.String TemplateID
			{
				get
				{
					System.Int32? data = entity.TemplateID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TemplateID = null;
					else entity.TemplateID = Convert.ToInt32(value);
				}
			}
			public System.String TemplateName
			{
				get
				{
					System.String data = entity.TemplateName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TemplateName = null;
					else entity.TemplateName = Convert.ToString(value);
				}
			}
			private esSatuSehatILPTemplate entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateQuery query)
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
				throw new Exception("esSatuSehatILPTemplate can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class SatuSehatILPTemplate : esSatuSehatILPTemplate
	{	
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplateQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateMetadata.Meta();
			}
		}	
			
		public esQueryItem TemplateID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateMetadata.ColumnNames.TemplateID, esSystemType.Int32);
			}
		} 
			
		public esQueryItem TemplateName
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateMetadata.ColumnNames.TemplateName, esSystemType.String);
			}
		} 
	
	}

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("SatuSehatILPTemplateCollection")]
	public partial class SatuSehatILPTemplateCollection : esSatuSehatILPTemplateCollection, IEnumerable< SatuSehatILPTemplate>
	{
		public SatuSehatILPTemplateCollection()
		{

		}	
		
		public static implicit operator List< SatuSehatILPTemplate>(SatuSehatILPTemplateCollection coll)
		{
			List< SatuSehatILPTemplate> list = new List< SatuSehatILPTemplate>();
			
			foreach (SatuSehatILPTemplate emp in coll)
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
				return  SatuSehatILPTemplateMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new SatuSehatILPTemplate(row);
		}

		override protected esEntity CreateEntity()
		{
			return new SatuSehatILPTemplate();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateQuery();
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
		public bool Load(SatuSehatILPTemplateQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public SatuSehatILPTemplate AddNew()
		{
			SatuSehatILPTemplate entity = base.AddNewEntity() as SatuSehatILPTemplate;
			
			return entity;		
		}
		public SatuSehatILPTemplate FindByPrimaryKey(Int32 templateID)
		{
			return base.FindByPrimaryKey(templateID) as SatuSehatILPTemplate;
		}

		#region IEnumerable< SatuSehatILPTemplate> Members

		IEnumerator< SatuSehatILPTemplate> IEnumerable< SatuSehatILPTemplate>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as SatuSehatILPTemplate;
			}
		}

		#endregion
		
		private SatuSehatILPTemplateQuery query;
	}


	/// <summary>
	/// Encapsulates the 'SatuSehatILPTemplate' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("SatuSehatILPTemplate ({TemplateID})")]
	[Serializable]
	public partial class SatuSehatILPTemplate : esSatuSehatILPTemplate
	{
		public SatuSehatILPTemplate()
		{
		}	
	
		public SatuSehatILPTemplate(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateMetadata.Meta();
			}
		}	
	
		override protected esSatuSehatILPTemplateQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateQuery();
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
		public bool Load(SatuSehatILPTemplateQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private SatuSehatILPTemplateQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class SatuSehatILPTemplateQuery : esSatuSehatILPTemplateQuery
	{
		public SatuSehatILPTemplateQuery()
		{

		}		
		
		public SatuSehatILPTemplateQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "SatuSehatILPTemplateQuery";
        }
	}

	[Serializable]
	public partial class SatuSehatILPTemplateMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected SatuSehatILPTemplateMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(SatuSehatILPTemplateMetadata.ColumnNames.TemplateID, 0, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateMetadata.PropertyNames.TemplateID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateMetadata.ColumnNames.TemplateName, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateMetadata.PropertyNames.TemplateName;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				

		}
		#endregion
	
		static public SatuSehatILPTemplateMetadata Meta()
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
			public const string TemplateID = "TemplateID";
			public const string TemplateName = "TemplateName";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string TemplateID = "TemplateID";
			public const string TemplateName = "TemplateName";
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
			lock (typeof(SatuSehatILPTemplateMetadata))
			{
				if(SatuSehatILPTemplateMetadata.mapDelegates == null)
				{
					SatuSehatILPTemplateMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (SatuSehatILPTemplateMetadata.meta == null)
				{
					SatuSehatILPTemplateMetadata.meta = new SatuSehatILPTemplateMetadata();
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
				
				meta.AddTypeMap("TemplateID", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("TemplateName", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "SatuSehatILPTemplate";
				meta.Destination = "SatuSehatILPTemplate";
				meta.spInsert = "proc_SatuSehatILPTemplateInsert";				
				meta.spUpdate = "proc_SatuSehatILPTemplateUpdate";		
				meta.spDelete = "proc_SatuSehatILPTemplateDelete";
				meta.spLoadAll = "proc_SatuSehatILPTemplateLoadAll";
				meta.spLoadByPrimaryKey = "proc_SatuSehatILPTemplateLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private SatuSehatILPTemplateMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
