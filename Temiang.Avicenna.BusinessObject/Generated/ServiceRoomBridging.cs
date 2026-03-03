/*
===============================================================================
                 Persistence Layer and Business Objects
===============================================================================
Version         : 2009.2.1214.0
Driver          : SQL
Date Generated  : 2/24/2026 11:52:55 AM
===============================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;


using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;
using Temiang.Dal.DynamicQuery;



namespace Temiang.Avicenna.BusinessObject
{

    [Serializable]
	abstract public class esServiceRoomBridgingCollection : esEntityCollectionWAuditLog
	{
		public esServiceRoomBridgingCollection()
		{

		}

		protected override string GetCollectionName()
		{
			return "ServiceRoomBridgingCollection";
		}

		#region Query Logic
		protected void InitQuery(esServiceRoomBridgingQuery query)
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
			this.InitQuery(query as esServiceRoomBridgingQuery);
		}
		#endregion
		
		virtual public ServiceRoomBridging DetachEntity(ServiceRoomBridging entity)
		{
			return base.DetachEntity(entity) as ServiceRoomBridging;
		}
		
		virtual public ServiceRoomBridging AttachEntity(ServiceRoomBridging entity)
		{
			return base.AttachEntity(entity) as ServiceRoomBridging;
		}
		
		virtual public void Combine(ServiceRoomBridgingCollection collection)
		{
			base.Combine(collection);
		}
		
		new public ServiceRoomBridging this[int index]
		{
			get
			{
				return base[index] as ServiceRoomBridging;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(ServiceRoomBridging);
		}
	}



	[Serializable]
	abstract public class esServiceRoomBridging : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esServiceRoomBridgingQuery GetDynamicQuery()
		{
			return null;
		}

		public esServiceRoomBridging()
		{

		}

		public esServiceRoomBridging(DataRow row)
			: base(row)
		{

		}
		
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(System.String bridgingID, System.String roomID, System.String sRBridgingType)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(bridgingID, roomID, sRBridgingType);
			else
				return LoadByPrimaryKeyStoredProcedure(bridgingID, roomID, sRBridgingType);
		}

		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, System.String bridgingID, System.String roomID, System.String sRBridgingType)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(bridgingID, roomID, sRBridgingType);
			else
				return LoadByPrimaryKeyStoredProcedure(bridgingID, roomID, sRBridgingType);
		}

		private bool LoadByPrimaryKeyDynamic(System.String bridgingID, System.String roomID, System.String sRBridgingType)
		{
			esServiceRoomBridgingQuery query = this.GetDynamicQuery();
			query.Where(query.BridgingID == bridgingID, query.RoomID == roomID, query.SRBridgingType == sRBridgingType);
			return query.Load();
		}

		private bool LoadByPrimaryKeyStoredProcedure(System.String bridgingID, System.String roomID, System.String sRBridgingType)
		{
			esParameters parms = new esParameters();
			parms.Add("BridgingID",bridgingID);			parms.Add("RoomID",roomID);			parms.Add("SRBridgingType",sRBridgingType);
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
						case "RoomID": this.str.RoomID = (string)value; break;							
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
		/// Maps to ServiceRoomBridging.RoomID
		/// </summary>
		virtual public System.String RoomID
		{
			get
			{
				return base.GetSystemString(ServiceRoomBridgingMetadata.ColumnNames.RoomID);
			}
			
			set
			{
				base.SetSystemString(ServiceRoomBridgingMetadata.ColumnNames.RoomID, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.SRBridgingType
		/// </summary>
		virtual public System.String SRBridgingType
		{
			get
			{
				return base.GetSystemString(ServiceRoomBridgingMetadata.ColumnNames.SRBridgingType);
			}
			
			set
			{
				base.SetSystemString(ServiceRoomBridgingMetadata.ColumnNames.SRBridgingType, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.BridgingID
		/// </summary>
		virtual public System.String BridgingID
		{
			get
			{
				return base.GetSystemString(ServiceRoomBridgingMetadata.ColumnNames.BridgingID);
			}
			
			set
			{
				base.SetSystemString(ServiceRoomBridgingMetadata.ColumnNames.BridgingID, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.BridgingName
		/// </summary>
		virtual public System.String BridgingName
		{
			get
			{
				return base.GetSystemString(ServiceRoomBridgingMetadata.ColumnNames.BridgingName);
			}
			
			set
			{
				base.SetSystemString(ServiceRoomBridgingMetadata.ColumnNames.BridgingName, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.IsActive
		/// </summary>
		virtual public System.Boolean? IsActive
		{
			get
			{
				return base.GetSystemBoolean(ServiceRoomBridgingMetadata.ColumnNames.IsActive);
			}
			
			set
			{
				base.SetSystemBoolean(ServiceRoomBridgingMetadata.ColumnNames.IsActive, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		
		/// <summary>
		/// Maps to ServiceRoomBridging.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		
		#endregion	

		#region String Properties


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
			public esStrings(esServiceRoomBridging entity)
			{
				this.entity = entity;
			}
			
	
			public System.String RoomID
			{
				get
				{
					System.String data = entity.RoomID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RoomID = null;
					else entity.RoomID = Convert.ToString(value);
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
			

			private esServiceRoomBridging entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esServiceRoomBridgingQuery query)
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
				throw new Exception("esServiceRoomBridging can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	
	public partial class ServiceRoomBridging : esServiceRoomBridging
	{

		
		/// <summary>
		/// Used internally by the entity's hierarchical properties.
		/// </summary>
		protected override List<esPropertyDescriptor> GetHierarchicalProperties()
		{
			List<esPropertyDescriptor> props = new List<esPropertyDescriptor>();
			
		
			return props;
		}	
		
		/// <summary>
		/// Used internally for retrieving AutoIncrementing keys
		/// during hierarchical PreSave.
		/// </summary>
		protected override void ApplyPreSaveKeys()
		{
		}
		
		/// <summary>
		/// Used internally for retrieving AutoIncrementing keys
		/// during hierarchical PostSave.
		/// </summary>
		protected override void ApplyPostSaveKeys()
		{
		}
		
		/// <summary>
		/// Used internally for retrieving AutoIncrementing keys
		/// during hierarchical PostOneToOneSave.
		/// </summary>
		protected override void ApplyPostOneSaveKeys()
		{
		}
		
	}



	[Serializable]
	abstract public class esServiceRoomBridgingQuery : esDynamicQuery
	{
		override protected IMetadata Meta
		{
			get
			{
				return ServiceRoomBridgingMetadata.Meta();
			}
		}	
		

		public esQueryItem RoomID
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.RoomID, esSystemType.String);
			}
		} 
		
		public esQueryItem SRBridgingType
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.SRBridgingType, esSystemType.String);
			}
		} 
		
		public esQueryItem BridgingID
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.BridgingID, esSystemType.String);
			}
		} 
		
		public esQueryItem BridgingName
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.BridgingName, esSystemType.String);
			}
		} 
		
		public esQueryItem IsActive
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.IsActive, esSystemType.Boolean);
			}
		} 
		
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, ServiceRoomBridgingMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
		
	}



    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("ServiceRoomBridgingCollection")]
	public partial class ServiceRoomBridgingCollection : esServiceRoomBridgingCollection, IEnumerable<ServiceRoomBridging>
	{
		public ServiceRoomBridgingCollection()
		{

		}
		
		public static implicit operator List<ServiceRoomBridging>(ServiceRoomBridgingCollection coll)
		{
			List<ServiceRoomBridging> list = new List<ServiceRoomBridging>();
			
			foreach (ServiceRoomBridging emp in coll)
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
				return  ServiceRoomBridgingMetadata.Meta();
			}
		}
		
		
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ServiceRoomBridgingQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new ServiceRoomBridging(row);
		}

		override protected esEntity CreateEntity()
		{
			return new ServiceRoomBridging();
		}
		
		
		#endregion


		[BrowsableAttribute( false )]
		public ServiceRoomBridgingQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ServiceRoomBridgingQuery();
					base.InitQuery(this.query);
				}

				return this.query;
			}
		}

		public void QueryReset()
		{
			this.query = null;
		}

		public bool Load(ServiceRoomBridgingQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}
		
		public ServiceRoomBridging AddNew()
		{
			ServiceRoomBridging entity = base.AddNewEntity() as ServiceRoomBridging;
			
			return entity;
		}

		public ServiceRoomBridging FindByPrimaryKey(System.String bridgingID, System.String roomID, System.String sRBridgingType)
		{
			return base.FindByPrimaryKey(bridgingID, roomID, sRBridgingType) as ServiceRoomBridging;
		}


		#region IEnumerable<ServiceRoomBridging> Members

		IEnumerator<ServiceRoomBridging> IEnumerable<ServiceRoomBridging>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as ServiceRoomBridging;
			}
		}

		#endregion
		
		private ServiceRoomBridgingQuery query;
	}


	/// <summary>
	/// Encapsulates the 'ServiceRoomBridging' table
	/// </summary>

    [System.Diagnostics.DebuggerDisplay("ServiceRoomBridging ({RoomID},{SRBridgingType},{BridgingID})")]
	[Serializable]
	public partial class ServiceRoomBridging : esServiceRoomBridging
	{
		public ServiceRoomBridging()
		{

		}
	
		public ServiceRoomBridging(DataRow row)
			: base(row)
		{

		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return ServiceRoomBridgingMetadata.Meta();
			}
		}
		
		
		
		override protected esServiceRoomBridgingQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new ServiceRoomBridgingQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		



		[BrowsableAttribute( false )]
		public ServiceRoomBridgingQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new ServiceRoomBridgingQuery();
					base.InitQuery(this.query);
				}

				return this.query;
			}
		}

		public void QueryReset()
		{
			this.query = null;
		}
		

		public bool Load(ServiceRoomBridgingQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}
		
		private ServiceRoomBridgingQuery query;
	}



    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
		
	public partial class ServiceRoomBridgingQuery : esServiceRoomBridgingQuery
	{
		public ServiceRoomBridgingQuery()
		{

		}		
		
		public ServiceRoomBridgingQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	

        override protected string GetQueryName()
        {
            return "ServiceRoomBridgingQuery";
        }
		
			
	}


	[Serializable]
	public partial class ServiceRoomBridgingMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected ServiceRoomBridgingMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;

			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.RoomID, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.RoomID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.SRBridgingType, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.SRBridgingType;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 20;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.BridgingID, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.BridgingID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 36;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.BridgingName, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.BridgingName;
			c.CharacterMaxLength = 100;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.IsActive, 4, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.IsActive;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateDateTime, 5, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c);
				
			c = new esColumnMetadata(ServiceRoomBridgingMetadata.ColumnNames.LastUpdateByUserID, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = ServiceRoomBridgingMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 15;
			c.IsNullable = true;
			_columns.Add(c);
				
		}
		#endregion	
	
		static public ServiceRoomBridgingMetadata Meta()
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
			 public const string RoomID = "RoomID";
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
			 public const string RoomID = "RoomID";
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
			lock (typeof(ServiceRoomBridgingMetadata))
			{
				if(ServiceRoomBridgingMetadata.mapDelegates == null)
				{
					ServiceRoomBridgingMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (ServiceRoomBridgingMetadata.meta == null)
				{
					ServiceRoomBridgingMetadata.meta = new ServiceRoomBridgingMetadata();
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
				

				meta.AddTypeMap("RoomID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("SRBridgingType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("BridgingID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("BridgingName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));			
				
				
				
				meta.Source = "ServiceRoomBridging";
				meta.Destination = "ServiceRoomBridging";
				
				meta.spInsert = "proc_ServiceRoomBridgingInsert";				
				meta.spUpdate = "proc_ServiceRoomBridgingUpdate";		
				meta.spDelete = "proc_ServiceRoomBridgingDelete";
				meta.spLoadAll = "proc_ServiceRoomBridgingLoadAll";
				meta.spLoadByPrimaryKey = "proc_ServiceRoomBridgingLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private ServiceRoomBridgingMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}
}
