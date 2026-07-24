/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-07-23 01:41:46 PM
===============================================================================
				Author: Wiliam Decosta (wiliamdecosta@gmail.com) - YBRS
===============================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.SqlClient;
using System.Data.SqlClient;
using System.Xml.Serialization;
using Temiang.Avicenna.BusinessObject.Common.Inacbg;
using Temiang.Dal.Core;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.BusinessObject
{
	[Serializable]
	abstract public class esDashboardClinicConfigCollection : esEntityCollectionWAuditLog
	{
		public esDashboardClinicConfigCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "DashboardClinicConfigCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esDashboardClinicConfigQuery query)
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
			this.InitQuery(query as esDashboardClinicConfigQuery);
		}
		#endregion
			
		virtual public DashboardClinicConfig DetachEntity(DashboardClinicConfig entity)
		{
			return base.DetachEntity(entity) as DashboardClinicConfig;
		}
		
		virtual public DashboardClinicConfig AttachEntity(DashboardClinicConfig entity)
		{
			return base.AttachEntity(entity) as DashboardClinicConfig;
		}
		
		virtual public void Combine(DashboardClinicConfigCollection collection)
		{
			base.Combine(collection);
		}
		
		new public DashboardClinicConfig this[int index]
		{
			get
			{
				return base[index] as DashboardClinicConfig;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(DashboardClinicConfig);
		}
	}

	[Serializable]
	abstract public class esDashboardClinicConfig : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esDashboardClinicConfigQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esDashboardClinicConfig()
		{
		}
	
		public esDashboardClinicConfig(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(String configID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(configID);
			else
				return LoadByPrimaryKeyStoredProcedure(configID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String configID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(configID);
			else
				return LoadByPrimaryKeyStoredProcedure(configID);
		}
	
		private bool LoadByPrimaryKeyDynamic(String configID)
		{
			esDashboardClinicConfigQuery query = this.GetDynamicQuery();
			query.Where(query.ConfigID == configID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(String configID)
		{
			esParameters parms = new esParameters();
			parms.Add("ConfigID", configID);
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
						case "ConfigID": this.str.ConfigID = (string)value; break;
						case "UserID": this.str.UserID = (string)value; break;
						case "ConfigName": this.str.ConfigName = (string)value; break;
						case "AutoRefresh": this.str.AutoRefresh = (string)value; break;
						case "RefreshIntervalSec": this.str.RefreshIntervalSec = (string)value; break;
						case "IsActive": this.str.IsActive = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "AutoRefresh":
						
							if (value == null || value is System.Boolean)
								this.AutoRefresh = (System.Boolean?)value;
							break;
						case "RefreshIntervalSec":
						
							if (value == null || value is System.Int32)
								this.RefreshIntervalSec = (System.Int32?)value;
							break;
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
		/// Maps to DashboardClinicConfig.ConfigID
		/// </summary>
		virtual public System.String ConfigID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigMetadata.ColumnNames.ConfigID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigMetadata.ColumnNames.ConfigID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.UserID
		/// </summary>
		virtual public System.String UserID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigMetadata.ColumnNames.UserID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigMetadata.ColumnNames.UserID, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.ConfigName
		/// </summary>
		virtual public System.String ConfigName
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigMetadata.ColumnNames.ConfigName);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigMetadata.ColumnNames.ConfigName, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.AutoRefresh
		/// </summary>
		virtual public System.Boolean? AutoRefresh
		{
			get
			{
				return base.GetSystemBoolean(DashboardClinicConfigMetadata.ColumnNames.AutoRefresh);
			}
			
			set
			{
				base.SetSystemBoolean(DashboardClinicConfigMetadata.ColumnNames.AutoRefresh, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.RefreshIntervalSec
		/// </summary>
		virtual public System.Int32? RefreshIntervalSec
		{
			get
			{
				return base.GetSystemInt32(DashboardClinicConfigMetadata.ColumnNames.RefreshIntervalSec);
			}
			
			set
			{
				base.SetSystemInt32(DashboardClinicConfigMetadata.ColumnNames.RefreshIntervalSec, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.IsActive
		/// </summary>
		virtual public System.Boolean? IsActive
		{
			get
			{
				return base.GetSystemBoolean(DashboardClinicConfigMetadata.ColumnNames.IsActive);
			}
			
			set
			{
				base.SetSystemBoolean(DashboardClinicConfigMetadata.ColumnNames.IsActive, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(DashboardClinicConfigMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(DashboardClinicConfigMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to DashboardClinicConfig.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(DashboardClinicConfigMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(DashboardClinicConfigMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esDashboardClinicConfig entity)
			{
				this.entity = entity;
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
			public System.String UserID
			{
				get
				{
					System.String data = entity.UserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.UserID = null;
					else entity.UserID = Convert.ToString(value);
				}
			}
			public System.String ConfigName
			{
				get
				{
					System.String data = entity.ConfigName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConfigName = null;
					else entity.ConfigName = Convert.ToString(value);
				}
			}
			public System.String AutoRefresh
			{
				get
				{
					System.Boolean? data = entity.AutoRefresh;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AutoRefresh = null;
					else entity.AutoRefresh = Convert.ToBoolean(value);
				}
			}
			public System.String RefreshIntervalSec
			{
				get
				{
					System.Int32? data = entity.RefreshIntervalSec;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RefreshIntervalSec = null;
					else entity.RefreshIntervalSec = Convert.ToInt32(value);
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
			
			private esDashboardClinicConfig entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esDashboardClinicConfigQuery query)
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
				throw new Exception("esDashboardClinicConfig can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class DashboardClinicConfig : esDashboardClinicConfig
	{
        public class DashboardClinicRoomItem
        {
            public string ServiceUnitID { get; set; }

            public string StageID { get; set; }

            public string ParamedicID { get; set; }

            public string KamarCode { get; set; }
        }

        public class DashboardClinicConfigDetailResponse
        {
            public string ConfigID { get; set; }

            public string ConfigName { get; set; }

            public string UserID { get; set; }

            public bool AutoRefresh { get; set; }

            public int RefreshIntervalSec { get; set; }

            public DateTime UpdatedAt { get; set; }

            public List<DashboardClinicRoomDetailResponse> Rooms { get; set; }
        }

        public class DashboardClinicRoomDetailResponse
        {
            public string ServiceUnitID { get; set; }

            public string ServiceUnitName { get; set; }

            public string StageID { get; set; }

            public string StageName { get; set; }

            public string ParamedicID { get; set; }

            public string ParamedicName { get; set; }

            public string KamarID { get; set; }

            public string KamarCode { get; set; }

            public string KamarName { get; set; }
        }

        public static string SaveConfig(
			string configID,
            bool isNew,
            string userID,
			string configName,
			bool autoRefresh,
			int refreshIntervalSec,
			List<DashboardClinicRoomItem> rooms)
        {
            DashboardClinicConfig config = new DashboardClinicConfig();

            if (!isNew)
            {
                if (!config.LoadByPrimaryKey(configID))
                    throw new Exception("Config tidak ditemukan.");
            }
            else
            {
                config.AddNew();
                config.ConfigID = configID;
            }

            config.UserID = userID;
            config.ConfigName = configName;
            config.AutoRefresh = autoRefresh;
            config.RefreshIntervalSec = refreshIntervalSec;
            config.IsActive = true;
            config.LastUpdateDateTime = (new DateTime()).NowAtSqlServer();
            config.LastUpdateByUserID = userID;

            config.Save();

            DashboardClinicConfigDetail.DeleteByConfigID(config.ConfigID);

            foreach (DashboardClinicRoomItem room in rooms)
            {
                DashboardClinicConfigDetail detail = new DashboardClinicConfigDetail();

                detail.AddNew();

                detail.ConfigID = config.ConfigID;
                detail.ServiceUnitID = room.ServiceUnitID;
                detail.StageID = room.StageID;
                detail.ParamedicID = room.ParamedicID;
                detail.KamarCode = room.KamarCode;

                detail.Save();
            }

            return config.ConfigID;
        }

        public static List<object> GetConfigList(string userID)
        {
            DashboardClinicConfigCollection collection =
                new DashboardClinicConfigCollection();

            DashboardClinicConfigQuery q =
                new DashboardClinicConfigQuery();

            // ==========================
            // Optional Filter UserID
            // ==========================
            if (!String.IsNullOrWhiteSpace(userID))
            {
                q.Where(q.UserID == userID);
            }

            q.OrderBy(
                q.LastUpdateDateTime,
                esOrderByDirection.Descending
            );

            collection.Load(q);

            List<object> result = new List<object>();

            foreach (DashboardClinicConfig item in collection)
            {
                DashboardClinicConfigDetailCollection details =
					new DashboardClinicConfigDetailCollection();

                DashboardClinicConfigDetailQuery detailQ =
                    new DashboardClinicConfigDetailQuery();

                detailQ.Where(detailQ.ConfigID == item.ConfigID);

                details.Load(detailQ);

                int roomCount = details.Count;

                result.Add(new
                {
                    ConfigID = item.ConfigID,
                    ConfigName = item.ConfigName,
                    UserID = item.UserID,
                    RoomCount = roomCount,
                    UpdatedAt = item.LastUpdateDateTime
                });
            }

            return result;
        }

        public static DashboardClinicConfigDetailResponse GetConfigDetail(
			string configID,
			string userID)
        {
            DashboardClinicConfig config = new DashboardClinicConfig();

            DashboardClinicConfigQuery q =
                new DashboardClinicConfigQuery();

            q.Where(
                q.ConfigID == configID,
                q.UserID == userID
            );

            if (!config.Load(q))
                throw new Exception("Dashboard clinic configuration tidak ditemukan.");

            DashboardClinicConfigDetailCollection details =
                new DashboardClinicConfigDetailCollection();

            DashboardClinicConfigDetailQuery dq =
                new DashboardClinicConfigDetailQuery();

            dq.Where(dq.ConfigID == configID);

            details.Load(dq);

            DashboardClinicConfigDetailResponse result =
                new DashboardClinicConfigDetailResponse();

            result.ConfigID = config.ConfigID;
            result.ConfigName = config.ConfigName;
            result.UserID = config.UserID;
            result.AutoRefresh = config.AutoRefresh ?? false;
            result.RefreshIntervalSec = config.RefreshIntervalSec ?? 0;
            result.UpdatedAt = config.LastUpdateDateTime ?? DateTime.MinValue;

            result.Rooms = new List<DashboardClinicRoomDetailResponse>();

            foreach (DashboardClinicConfigDetail item in details)
            {
                ServiceUnit su = new ServiceUnit();
                su.LoadByPrimaryKey(item.ServiceUnitID);

                QueueStage stage = new QueueStage();
                stage.LoadByPrimaryKey(item.StageID);

                Paramedic doctor = new Paramedic();
                doctor.LoadByPrimaryKey(item.ParamedicID);

                ListKamarForAntrian kamar =
                    new ListKamarForAntrian();

                ListKamarForAntrianQuery kq =
                    new ListKamarForAntrianQuery();

                kq.Where(kq.KamarCode == item.KamarCode);

                kamar.Load(kq);

                result.Rooms.Add(
                    new DashboardClinicRoomDetailResponse
                    {
                        ServiceUnitID = item.ServiceUnitID,
                        ServiceUnitName = su.ServiceUnitName,

                        StageID = item.StageID,
                        StageName = stage.StageName,

                        ParamedicID = item.ParamedicID,
                        ParamedicName = doctor.ParamedicName,

                        KamarID = kamar.KamarID.ToString(),
                        KamarCode = kamar.KamarCode,
                        KamarName = kamar.KamarName
                    });
            }

            return result;
        }
    }

	[Serializable]
	abstract public class esDashboardClinicConfigQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return DashboardClinicConfigMetadata.Meta();
			}
		}	
			
		public esQueryItem ConfigID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.ConfigID, esSystemType.String);
			}
		} 
		public esQueryItem UserID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.UserID, esSystemType.String);
			}
		} 
		public esQueryItem ConfigName
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.ConfigName, esSystemType.String);
			}
		} 
		public esQueryItem AutoRefresh
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.AutoRefresh, esSystemType.Boolean);
			}
		} 
		public esQueryItem RefreshIntervalSec
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.RefreshIntervalSec, esSystemType.Int32);
			}
		} 
		public esQueryItem IsActive
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.IsActive, esSystemType.Boolean);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, DashboardClinicConfigMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("DashboardClinicConfigCollection")]
	public partial class DashboardClinicConfigCollection : esDashboardClinicConfigCollection, IEnumerable< DashboardClinicConfig>
	{
		public DashboardClinicConfigCollection()
		{

		}	
		
		public static implicit operator List< DashboardClinicConfig>(DashboardClinicConfigCollection coll)
		{
			List< DashboardClinicConfig> list = new List< DashboardClinicConfig>();
			
			foreach (DashboardClinicConfig emp in coll)
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
				return  DashboardClinicConfigMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DashboardClinicConfigQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new DashboardClinicConfig(row);
		}

		override protected esEntity CreateEntity()
		{
			return new DashboardClinicConfig();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public DashboardClinicConfigQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DashboardClinicConfigQuery();
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
		public bool Load(DashboardClinicConfigQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public DashboardClinicConfig AddNew()
		{
			DashboardClinicConfig entity = base.AddNewEntity() as DashboardClinicConfig;
			
			return entity;		
		}
		public DashboardClinicConfig FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as DashboardClinicConfig;
		}

		#region IEnumerable< DashboardClinicConfig> Members

		IEnumerator< DashboardClinicConfig> IEnumerable< DashboardClinicConfig>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as DashboardClinicConfig;
			}
		}

		#endregion
		
		private DashboardClinicConfigQuery query;
	}


	/// <summary>
	/// Encapsulates the 'DashboardClinicConfig' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("DashboardClinicConfig ({StandardReferenceID})")]
	[Serializable]
	public partial class DashboardClinicConfig : esDashboardClinicConfig
	{
		public DashboardClinicConfig()
		{
		}	
	
		public DashboardClinicConfig(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return DashboardClinicConfigMetadata.Meta();
			}
		}	
	
		override protected esDashboardClinicConfigQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new DashboardClinicConfigQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public DashboardClinicConfigQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new DashboardClinicConfigQuery();
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
		public bool Load(DashboardClinicConfigQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private DashboardClinicConfigQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class DashboardClinicConfigQuery : esDashboardClinicConfigQuery
	{
		public DashboardClinicConfigQuery()
		{

		}		
		
		public DashboardClinicConfigQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "DashboardClinicConfigQuery";
        }
	}

	[Serializable]
	public partial class DashboardClinicConfigMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected DashboardClinicConfigMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.ConfigID, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.ConfigID;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.UserID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.UserID;
			c.CharacterMaxLength = 15;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.ConfigName, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.ConfigName;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.AutoRefresh, 3, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.AutoRefresh;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.RefreshIntervalSec, 4, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.RefreshIntervalSec;
			c.NumericPrecision = 10;
			c.HasDefault = true;
			c.Default = @"((5))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.IsActive, 5, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.IsActive;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.LastUpdateDateTime, 6, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.LastUpdateDateTime;
			c.HasDefault = true;
			c.Default = @"(getdate())";
			_columns.Add(c); 
				
			c = new esColumnMetadata(DashboardClinicConfigMetadata.ColumnNames.LastUpdateByUserID, 7, typeof(System.String), esSystemType.String);
			c.PropertyName = DashboardClinicConfigMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public DashboardClinicConfigMetadata Meta()
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
			public const string ConfigID = "ConfigID";
			public const string UserID = "UserID";
			public const string ConfigName = "ConfigName";
			public const string AutoRefresh = "AutoRefresh";
			public const string RefreshIntervalSec = "RefreshIntervalSec";
			public const string IsActive = "IsActive";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string ConfigID = "ConfigID";
			public const string UserID = "UserID";
			public const string ConfigName = "ConfigName";
			public const string AutoRefresh = "AutoRefresh";
			public const string RefreshIntervalSec = "RefreshIntervalSec";
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
			lock (typeof(DashboardClinicConfigMetadata))
			{
				if(DashboardClinicConfigMetadata.mapDelegates == null)
				{
					DashboardClinicConfigMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (DashboardClinicConfigMetadata.meta == null)
				{
					DashboardClinicConfigMetadata.meta = new DashboardClinicConfigMetadata();
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
				
				meta.AddTypeMap("ConfigID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("UserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ConfigName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AutoRefresh", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("RefreshIntervalSec", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "DashboardClinicConfig";
				meta.Destination = "DashboardClinicConfig";
				meta.spInsert = "proc_DashboardClinicConfigInsert";				
				meta.spUpdate = "proc_DashboardClinicConfigUpdate";		
				meta.spDelete = "proc_DashboardClinicConfigDelete";
				meta.spLoadAll = "proc_DashboardClinicConfigLoadAll";
				meta.spLoadByPrimaryKey = "proc_DashboardClinicConfigLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private DashboardClinicConfigMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		