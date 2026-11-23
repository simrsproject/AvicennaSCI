/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-23 05:34:40 PM
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
	abstract public class esAdmedikaDailyMonitoringCollection : esEntityCollectionWAuditLog
	{
		public esAdmedikaDailyMonitoringCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "AdmedikaDailyMonitoringCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esAdmedikaDailyMonitoringQuery query)
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
			this.InitQuery(query as esAdmedikaDailyMonitoringQuery);
		}
		#endregion
			
		virtual public AdmedikaDailyMonitoring DetachEntity(AdmedikaDailyMonitoring entity)
		{
			return base.DetachEntity(entity) as AdmedikaDailyMonitoring;
		}
		
		virtual public AdmedikaDailyMonitoring AttachEntity(AdmedikaDailyMonitoring entity)
		{
			return base.AttachEntity(entity) as AdmedikaDailyMonitoring;
		}
		
		virtual public void Combine(AdmedikaDailyMonitoringCollection collection)
		{
			base.Combine(collection);
		}
		
		new public AdmedikaDailyMonitoring this[int index]
		{
			get
			{
				return base[index] as AdmedikaDailyMonitoring;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(AdmedikaDailyMonitoring);
		}
	}

	[Serializable]
	abstract public class esAdmedikaDailyMonitoring : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esAdmedikaDailyMonitoringQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esAdmedikaDailyMonitoring()
		{
		}
	
		public esAdmedikaDailyMonitoring(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 admedikaDailyMonitoringID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(admedikaDailyMonitoringID);
			else
				return LoadByPrimaryKeyStoredProcedure(admedikaDailyMonitoringID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 admedikaDailyMonitoringID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(admedikaDailyMonitoringID);
			else
				return LoadByPrimaryKeyStoredProcedure(admedikaDailyMonitoringID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 admedikaDailyMonitoringID)
		{
			esAdmedikaDailyMonitoringQuery query = this.GetDynamicQuery();
			query.Where(query.AdmedikaDailyMonitoringID == admedikaDailyMonitoringID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 admedikaDailyMonitoringID)
		{
			esParameters parms = new esParameters();
			parms.Add("AdmedikaDailyMonitoringID", admedikaDailyMonitoringID);
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
						case "AdmedikaDailyMonitoringID": this.str.AdmedikaDailyMonitoringID = (string)value; break;
						case "AdmedikaClaimTransactionID": this.str.AdmedikaClaimTransactionID = (string)value; break;
						case "MonitoringDate": this.str.MonitoringDate = (string)value; break;
						case "IsSent": this.str.IsSent = (string)value; break;
						case "SentDateTime": this.str.SentDateTime = (string)value; break;
						case "SentByUserID": this.str.SentByUserID = (string)value; break;
						case "IsSuccess": this.str.IsSuccess = (string)value; break;
						case "OkCount": this.str.OkCount = (string)value; break;
						case "ErCount": this.str.ErCount = (string)value; break;
						case "ClStatus": this.str.ClStatus = (string)value; break;
						case "ClDesc": this.str.ClDesc = (string)value; break;
						case "Message": this.str.Message = (string)value; break;
						case "ErrorCode": this.str.ErrorCode = (string)value; break;
						case "ErrorDesc": this.str.ErrorDesc = (string)value; break;
						case "RequestJson": this.str.RequestJson = (string)value; break;
						case "RawResponse": this.str.RawResponse = (string)value; break;
						case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
						case "CreatedByUserID": this.str.CreatedByUserID = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "AdmedikaDailyMonitoringID":
						
							if (value == null || value is System.Int64)
								this.AdmedikaDailyMonitoringID = (System.Int64?)value;
							break;
						case "AdmedikaClaimTransactionID":
						
							if (value == null || value is System.Int64)
								this.AdmedikaClaimTransactionID = (System.Int64?)value;
							break;
						case "MonitoringDate":
						
							if (value == null || value is System.DateTime)
								this.MonitoringDate = (System.DateTime?)value;
							break;
						case "IsSent":
						
							if (value == null || value is System.Boolean)
								this.IsSent = (System.Boolean?)value;
							break;
						case "SentDateTime":
						
							if (value == null || value is System.DateTime)
								this.SentDateTime = (System.DateTime?)value;
							break;
						case "IsSuccess":
						
							if (value == null || value is System.Boolean)
								this.IsSuccess = (System.Boolean?)value;
							break;
						case "OkCount":
						
							if (value == null || value is System.Int32)
								this.OkCount = (System.Int32?)value;
							break;
						case "ErCount":
						
							if (value == null || value is System.Int32)
								this.ErCount = (System.Int32?)value;
							break;
						case "CreatedDateTime":
						
							if (value == null || value is System.DateTime)
								this.CreatedDateTime = (System.DateTime?)value;
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
		/// Maps to AdmedikaDailyMonitoring.AdmedikaDailyMonitoringID
		/// </summary>
		virtual public System.Int64? AdmedikaDailyMonitoringID
		{
			get
			{
				return base.GetSystemInt64(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaDailyMonitoringID);
			}
			
			set
			{
				base.SetSystemInt64(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaDailyMonitoringID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.AdmedikaClaimTransactionID
		/// </summary>
		virtual public System.Int64? AdmedikaClaimTransactionID
		{
			get
			{
				return base.GetSystemInt64(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaClaimTransactionID);
			}
			
			set
			{
				base.SetSystemInt64(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaClaimTransactionID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.MonitoringDate
		/// </summary>
		virtual public System.DateTime? MonitoringDate
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.MonitoringDate);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.MonitoringDate, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.IsSent
		/// </summary>
		virtual public System.Boolean? IsSent
		{
			get
			{
				return base.GetSystemBoolean(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSent);
			}
			
			set
			{
				base.SetSystemBoolean(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSent, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.SentDateTime
		/// </summary>
		virtual public System.DateTime? SentDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.SentDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.SentDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.SentByUserID
		/// </summary>
		virtual public System.String SentByUserID
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.SentByUserID);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.SentByUserID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.IsSuccess
		/// </summary>
		virtual public System.Boolean? IsSuccess
		{
			get
			{
				return base.GetSystemBoolean(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSuccess);
			}
			
			set
			{
				base.SetSystemBoolean(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSuccess, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.OkCount
		/// </summary>
		virtual public System.Int32? OkCount
		{
			get
			{
				return base.GetSystemInt32(AdmedikaDailyMonitoringMetadata.ColumnNames.OkCount);
			}
			
			set
			{
				base.SetSystemInt32(AdmedikaDailyMonitoringMetadata.ColumnNames.OkCount, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.ErCount
		/// </summary>
		virtual public System.Int32? ErCount
		{
			get
			{
				return base.GetSystemInt32(AdmedikaDailyMonitoringMetadata.ColumnNames.ErCount);
			}
			
			set
			{
				base.SetSystemInt32(AdmedikaDailyMonitoringMetadata.ColumnNames.ErCount, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.ClStatus
		/// </summary>
		virtual public System.String ClStatus
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ClStatus);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ClStatus, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.ClDesc
		/// </summary>
		virtual public System.String ClDesc
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ClDesc);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ClDesc, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.Message
		/// </summary>
		virtual public System.String Message
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.Message);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.Message, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.ErrorCode
		/// </summary>
		virtual public System.String ErrorCode
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorCode);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorCode, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.ErrorDesc
		/// </summary>
		virtual public System.String ErrorDesc
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorDesc);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorDesc, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.RequestJson
		/// </summary>
		virtual public System.String RequestJson
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.RequestJson);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.RequestJson, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.RawResponse
		/// </summary>
		virtual public System.String RawResponse
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.RawResponse);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.RawResponse, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoring.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esAdmedikaDailyMonitoring entity)
			{
				this.entity = entity;
			}
			public System.String AdmedikaDailyMonitoringID
			{
				get
				{
					System.Int64? data = entity.AdmedikaDailyMonitoringID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AdmedikaDailyMonitoringID = null;
					else entity.AdmedikaDailyMonitoringID = Convert.ToInt64(value);
				}
			}
			public System.String AdmedikaClaimTransactionID
			{
				get
				{
					System.Int64? data = entity.AdmedikaClaimTransactionID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AdmedikaClaimTransactionID = null;
					else entity.AdmedikaClaimTransactionID = Convert.ToInt64(value);
				}
			}
			public System.String MonitoringDate
			{
				get
				{
					System.DateTime? data = entity.MonitoringDate;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.MonitoringDate = null;
					else entity.MonitoringDate = Convert.ToDateTime(value);
				}
			}
			public System.String IsSent
			{
				get
				{
					System.Boolean? data = entity.IsSent;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsSent = null;
					else entity.IsSent = Convert.ToBoolean(value);
				}
			}
			public System.String SentDateTime
			{
				get
				{
					System.DateTime? data = entity.SentDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SentDateTime = null;
					else entity.SentDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String SentByUserID
			{
				get
				{
					System.String data = entity.SentByUserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SentByUserID = null;
					else entity.SentByUserID = Convert.ToString(value);
				}
			}
			public System.String IsSuccess
			{
				get
				{
					System.Boolean? data = entity.IsSuccess;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsSuccess = null;
					else entity.IsSuccess = Convert.ToBoolean(value);
				}
			}
			public System.String OkCount
			{
				get
				{
					System.Int32? data = entity.OkCount;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.OkCount = null;
					else entity.OkCount = Convert.ToInt32(value);
				}
			}
			public System.String ErCount
			{
				get
				{
					System.Int32? data = entity.ErCount;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ErCount = null;
					else entity.ErCount = Convert.ToInt32(value);
				}
			}
			public System.String ClStatus
			{
				get
				{
					System.String data = entity.ClStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ClStatus = null;
					else entity.ClStatus = Convert.ToString(value);
				}
			}
			public System.String ClDesc
			{
				get
				{
					System.String data = entity.ClDesc;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ClDesc = null;
					else entity.ClDesc = Convert.ToString(value);
				}
			}
			public System.String Message
			{
				get
				{
					System.String data = entity.Message;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Message = null;
					else entity.Message = Convert.ToString(value);
				}
			}
			public System.String ErrorCode
			{
				get
				{
					System.String data = entity.ErrorCode;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ErrorCode = null;
					else entity.ErrorCode = Convert.ToString(value);
				}
			}
			public System.String ErrorDesc
			{
				get
				{
					System.String data = entity.ErrorDesc;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ErrorDesc = null;
					else entity.ErrorDesc = Convert.ToString(value);
				}
			}
			public System.String RequestJson
			{
				get
				{
					System.String data = entity.RequestJson;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RequestJson = null;
					else entity.RequestJson = Convert.ToString(value);
				}
			}
			public System.String RawResponse
			{
				get
				{
					System.String data = entity.RawResponse;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RawResponse = null;
					else entity.RawResponse = Convert.ToString(value);
				}
			}
			public System.String CreatedDateTime
			{
				get
				{
					System.DateTime? data = entity.CreatedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CreatedDateTime = null;
					else entity.CreatedDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String CreatedByUserID
			{
				get
				{
					System.String data = entity.CreatedByUserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CreatedByUserID = null;
					else entity.CreatedByUserID = Convert.ToString(value);
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
			
			private esAdmedikaDailyMonitoring entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esAdmedikaDailyMonitoringQuery query)
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
				throw new Exception("esAdmedikaDailyMonitoring can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class AdmedikaDailyMonitoring : esAdmedikaDailyMonitoring
	{	
	}

	[Serializable]
	abstract public class esAdmedikaDailyMonitoringQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return AdmedikaDailyMonitoringMetadata.Meta();
			}
		}	
			
		public esQueryItem AdmedikaDailyMonitoringID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaDailyMonitoringID, esSystemType.Int64);
			}
		} 
		public esQueryItem AdmedikaClaimTransactionID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaClaimTransactionID, esSystemType.Int64);
			}
		} 
		public esQueryItem MonitoringDate
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.MonitoringDate, esSystemType.DateTime);
			}
		} 
		public esQueryItem IsSent
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.IsSent, esSystemType.Boolean);
			}
		} 
		public esQueryItem SentDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.SentDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem SentByUserID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.SentByUserID, esSystemType.String);
			}
		} 
		public esQueryItem IsSuccess
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.IsSuccess, esSystemType.Boolean);
			}
		} 
		public esQueryItem OkCount
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.OkCount, esSystemType.Int32);
			}
		} 
		public esQueryItem ErCount
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.ErCount, esSystemType.Int32);
			}
		} 
		public esQueryItem ClStatus
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.ClStatus, esSystemType.String);
			}
		} 
		public esQueryItem ClDesc
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.ClDesc, esSystemType.String);
			}
		} 
		public esQueryItem Message
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.Message, esSystemType.String);
			}
		} 
		public esQueryItem ErrorCode
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorCode, esSystemType.String);
			}
		} 
		public esQueryItem ErrorDesc
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorDesc, esSystemType.String);
			}
		} 
		public esQueryItem RequestJson
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.RequestJson, esSystemType.String);
			}
		} 
		public esQueryItem RawResponse
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.RawResponse, esSystemType.String);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("AdmedikaDailyMonitoringCollection")]
	public partial class AdmedikaDailyMonitoringCollection : esAdmedikaDailyMonitoringCollection, IEnumerable< AdmedikaDailyMonitoring>
	{
		public AdmedikaDailyMonitoringCollection()
		{

		}	
		
		public static implicit operator List< AdmedikaDailyMonitoring>(AdmedikaDailyMonitoringCollection coll)
		{
			List< AdmedikaDailyMonitoring> list = new List< AdmedikaDailyMonitoring>();
			
			foreach (AdmedikaDailyMonitoring emp in coll)
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
				return  AdmedikaDailyMonitoringMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new AdmedikaDailyMonitoringQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new AdmedikaDailyMonitoring(row);
		}

		override protected esEntity CreateEntity()
		{
			return new AdmedikaDailyMonitoring();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public AdmedikaDailyMonitoringQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new AdmedikaDailyMonitoringQuery();
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
		public bool Load(AdmedikaDailyMonitoringQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public AdmedikaDailyMonitoring AddNew()
		{
			AdmedikaDailyMonitoring entity = base.AddNewEntity() as AdmedikaDailyMonitoring;
			
			return entity;		
		}
		public AdmedikaDailyMonitoring FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as AdmedikaDailyMonitoring;
		}

		#region IEnumerable< AdmedikaDailyMonitoring> Members

		IEnumerator< AdmedikaDailyMonitoring> IEnumerable< AdmedikaDailyMonitoring>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as AdmedikaDailyMonitoring;
			}
		}

		#endregion
		
		private AdmedikaDailyMonitoringQuery query;
	}


	/// <summary>
	/// Encapsulates the 'AdmedikaDailyMonitoring' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("AdmedikaDailyMonitoring ({StandardReferenceID})")]
	[Serializable]
	public partial class AdmedikaDailyMonitoring : esAdmedikaDailyMonitoring
	{
		public AdmedikaDailyMonitoring()
		{
		}	
	
		public AdmedikaDailyMonitoring(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return AdmedikaDailyMonitoringMetadata.Meta();
			}
		}	
	
		override protected esAdmedikaDailyMonitoringQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new AdmedikaDailyMonitoringQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public AdmedikaDailyMonitoringQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new AdmedikaDailyMonitoringQuery();
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
		public bool Load(AdmedikaDailyMonitoringQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private AdmedikaDailyMonitoringQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class AdmedikaDailyMonitoringQuery : esAdmedikaDailyMonitoringQuery
	{
		public AdmedikaDailyMonitoringQuery()
		{

		}		
		
		public AdmedikaDailyMonitoringQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "AdmedikaDailyMonitoringQuery";
        }
	}

	[Serializable]
	public partial class AdmedikaDailyMonitoringMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected AdmedikaDailyMonitoringMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaDailyMonitoringID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.AdmedikaDailyMonitoringID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.AdmedikaClaimTransactionID, 1, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.AdmedikaClaimTransactionID;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.MonitoringDate, 2, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.MonitoringDate;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSent, 3, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.IsSent;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.SentDateTime, 4, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.SentDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.SentByUserID, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.SentByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.IsSuccess, 6, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.IsSuccess;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.OkCount, 7, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.OkCount;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.ErCount, 8, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.ErCount;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.ClStatus, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.ClStatus;
			c.CharacterMaxLength = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.ClDesc, 10, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.ClDesc;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.Message, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.Message;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorCode, 12, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.ErrorCode;
			c.CharacterMaxLength = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.ErrorDesc, 13, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.ErrorDesc;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.RequestJson, 14, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.RequestJson;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.RawResponse, 15, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.RawResponse;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedDateTime, 16, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.CreatedDateTime;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.CreatedByUserID, 17, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateDateTime, 18, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringMetadata.ColumnNames.LastUpdateByUserID, 19, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public AdmedikaDailyMonitoringMetadata Meta()
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
			public const string AdmedikaDailyMonitoringID = "AdmedikaDailyMonitoringID";
			public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
			public const string MonitoringDate = "MonitoringDate";
			public const string IsSent = "IsSent";
			public const string SentDateTime = "SentDateTime";
			public const string SentByUserID = "SentByUserID";
			public const string IsSuccess = "IsSuccess";
			public const string OkCount = "OkCount";
			public const string ErCount = "ErCount";
			public const string ClStatus = "ClStatus";
			public const string ClDesc = "ClDesc";
			public const string Message = "Message";
			public const string ErrorCode = "ErrorCode";
			public const string ErrorDesc = "ErrorDesc";
			public const string RequestJson = "RequestJson";
			public const string RawResponse = "RawResponse";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string AdmedikaDailyMonitoringID = "AdmedikaDailyMonitoringID";
			public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
			public const string MonitoringDate = "MonitoringDate";
			public const string IsSent = "IsSent";
			public const string SentDateTime = "SentDateTime";
			public const string SentByUserID = "SentByUserID";
			public const string IsSuccess = "IsSuccess";
			public const string OkCount = "OkCount";
			public const string ErCount = "ErCount";
			public const string ClStatus = "ClStatus";
			public const string ClDesc = "ClDesc";
			public const string Message = "Message";
			public const string ErrorCode = "ErrorCode";
			public const string ErrorDesc = "ErrorDesc";
			public const string RequestJson = "RequestJson";
			public const string RawResponse = "RawResponse";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
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
			lock (typeof(AdmedikaDailyMonitoringMetadata))
			{
				if(AdmedikaDailyMonitoringMetadata.mapDelegates == null)
				{
					AdmedikaDailyMonitoringMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (AdmedikaDailyMonitoringMetadata.meta == null)
				{
					AdmedikaDailyMonitoringMetadata.meta = new AdmedikaDailyMonitoringMetadata();
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
				
				meta.AddTypeMap("AdmedikaDailyMonitoringID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("AdmedikaClaimTransactionID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("MonitoringDate", new esTypeMap("date", "System.DateTime"));
				meta.AddTypeMap("IsSent", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("SentDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("SentByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsSuccess", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("OkCount", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("ErCount", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("ClStatus", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ClDesc", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Message", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("ErrorCode", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ErrorDesc", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RequestJson", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RawResponse", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "AdmedikaDailyMonitoring";
				meta.Destination = "AdmedikaDailyMonitoring";
				meta.spInsert = "proc_AdmedikaDailyMonitoringInsert";				
				meta.spUpdate = "proc_AdmedikaDailyMonitoringUpdate";		
				meta.spDelete = "proc_AdmedikaDailyMonitoringDelete";
				meta.spLoadAll = "proc_AdmedikaDailyMonitoringLoadAll";
				meta.spLoadByPrimaryKey = "proc_AdmedikaDailyMonitoringLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private AdmedikaDailyMonitoringMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		