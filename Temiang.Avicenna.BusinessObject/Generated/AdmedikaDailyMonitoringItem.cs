/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-23 05:34:39 PM
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
	abstract public class esAdmedikaDailyMonitoringItemCollection : esEntityCollectionWAuditLog
	{
		public esAdmedikaDailyMonitoringItemCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "AdmedikaDailyMonitoringItemCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esAdmedikaDailyMonitoringItemQuery query)
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
			this.InitQuery(query as esAdmedikaDailyMonitoringItemQuery);
		}
		#endregion
			
		virtual public AdmedikaDailyMonitoringItem DetachEntity(AdmedikaDailyMonitoringItem entity)
		{
			return base.DetachEntity(entity) as AdmedikaDailyMonitoringItem;
		}
		
		virtual public AdmedikaDailyMonitoringItem AttachEntity(AdmedikaDailyMonitoringItem entity)
		{
			return base.AttachEntity(entity) as AdmedikaDailyMonitoringItem;
		}
		
		virtual public void Combine(AdmedikaDailyMonitoringItemCollection collection)
		{
			base.Combine(collection);
		}
		
		new public AdmedikaDailyMonitoringItem this[int index]
		{
			get
			{
				return base[index] as AdmedikaDailyMonitoringItem;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(AdmedikaDailyMonitoringItem);
		}
	}

	[Serializable]
	abstract public class esAdmedikaDailyMonitoringItem : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esAdmedikaDailyMonitoringItemQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esAdmedikaDailyMonitoringItem()
		{
		}
	
		public esAdmedikaDailyMonitoringItem(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 admedikaDailyMonitoringItemID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(admedikaDailyMonitoringItemID);
			else
				return LoadByPrimaryKeyStoredProcedure(admedikaDailyMonitoringItemID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 admedikaDailyMonitoringItemID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(admedikaDailyMonitoringItemID);
			else
				return LoadByPrimaryKeyStoredProcedure(admedikaDailyMonitoringItemID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 admedikaDailyMonitoringItemID)
		{
			esAdmedikaDailyMonitoringItemQuery query = this.GetDynamicQuery();
			query.Where(query.AdmedikaDailyMonitoringItemID == admedikaDailyMonitoringItemID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 admedikaDailyMonitoringItemID)
		{
			esParameters parms = new esParameters();
			parms.Add("AdmedikaDailyMonitoringItemID", admedikaDailyMonitoringItemID);
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
						case "AdmedikaDailyMonitoringItemID": this.str.AdmedikaDailyMonitoringItemID = (string)value; break;
						case "AdmedikaDailyMonitoringID": this.str.AdmedikaDailyMonitoringID = (string)value; break;
						case "SeqNo": this.str.SeqNo = (string)value; break;
						case "CategoryCode": this.str.CategoryCode = (string)value; break;
						case "FormCode": this.str.FormCode = (string)value; break;
						case "Remarks": this.str.Remarks = (string)value; break;
						case "EntryDateTime": this.str.EntryDateTime = (string)value; break;
						case "SourceType": this.str.SourceType = (string)value; break;
						case "Status": this.str.Status = (string)value; break;
						case "IsSent": this.str.IsSent = (string)value; break;
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
						case "AdmedikaDailyMonitoringItemID":
						
							if (value == null || value is System.Int64)
								this.AdmedikaDailyMonitoringItemID = (System.Int64?)value;
							break;
						case "AdmedikaDailyMonitoringID":
						
							if (value == null || value is System.Int64)
								this.AdmedikaDailyMonitoringID = (System.Int64?)value;
							break;
						case "SeqNo":
						
							if (value == null || value is System.Int32)
								this.SeqNo = (System.Int32?)value;
							break;
						case "CategoryCode":
						
							if (value == null || value is System.Int32)
								this.CategoryCode = (System.Int32?)value;
							break;
						case "EntryDateTime":
						
							if (value == null || value is System.DateTime)
								this.EntryDateTime = (System.DateTime?)value;
							break;
						case "IsSent":
						
							if (value == null || value is System.Boolean)
								this.IsSent = (System.Boolean?)value;
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
		/// Maps to AdmedikaDailyMonitoringItem.AdmedikaDailyMonitoringItemID
		/// </summary>
		virtual public System.Int64? AdmedikaDailyMonitoringItemID
		{
			get
			{
				return base.GetSystemInt64(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringItemID);
			}
			
			set
			{
				base.SetSystemInt64(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringItemID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.AdmedikaDailyMonitoringID
		/// </summary>
		virtual public System.Int64? AdmedikaDailyMonitoringID
		{
			get
			{
				return base.GetSystemInt64(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringID);
			}
			
			set
			{
				base.SetSystemInt64(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.SeqNo
		/// </summary>
		virtual public System.Int32? SeqNo
		{
			get
			{
				return base.GetSystemInt32(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SeqNo);
			}
			
			set
			{
				base.SetSystemInt32(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SeqNo, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.CategoryCode
		/// </summary>
		virtual public System.Int32? CategoryCode
		{
			get
			{
				return base.GetSystemInt32(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CategoryCode);
			}
			
			set
			{
				base.SetSystemInt32(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CategoryCode, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.FormCode
		/// </summary>
		virtual public System.String FormCode
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.FormCode);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.FormCode, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.Remarks
		/// </summary>
		virtual public System.String Remarks
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Remarks);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Remarks, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.EntryDateTime
		/// </summary>
		virtual public System.DateTime? EntryDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.EntryDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.EntryDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.SourceType
		/// </summary>
		virtual public System.String SourceType
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SourceType);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SourceType, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.Status
		/// </summary>
		virtual public System.String Status
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Status);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Status, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.IsSent
		/// </summary>
		virtual public System.Boolean? IsSent
		{
			get
			{
				return base.GetSystemBoolean(AdmedikaDailyMonitoringItemMetadata.ColumnNames.IsSent);
			}
			
			set
			{
				base.SetSystemBoolean(AdmedikaDailyMonitoringItemMetadata.ColumnNames.IsSent, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to AdmedikaDailyMonitoringItem.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esAdmedikaDailyMonitoringItem entity)
			{
				this.entity = entity;
			}
			public System.String AdmedikaDailyMonitoringItemID
			{
				get
				{
					System.Int64? data = entity.AdmedikaDailyMonitoringItemID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AdmedikaDailyMonitoringItemID = null;
					else entity.AdmedikaDailyMonitoringItemID = Convert.ToInt64(value);
				}
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
			public System.String SeqNo
			{
				get
				{
					System.Int32? data = entity.SeqNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SeqNo = null;
					else entity.SeqNo = Convert.ToInt32(value);
				}
			}
			public System.String CategoryCode
			{
				get
				{
					System.Int32? data = entity.CategoryCode;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CategoryCode = null;
					else entity.CategoryCode = Convert.ToInt32(value);
				}
			}
			public System.String FormCode
			{
				get
				{
					System.String data = entity.FormCode;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.FormCode = null;
					else entity.FormCode = Convert.ToString(value);
				}
			}
			public System.String Remarks
			{
				get
				{
					System.String data = entity.Remarks;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Remarks = null;
					else entity.Remarks = Convert.ToString(value);
				}
			}
			public System.String EntryDateTime
			{
				get
				{
					System.DateTime? data = entity.EntryDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.EntryDateTime = null;
					else entity.EntryDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String SourceType
			{
				get
				{
					System.String data = entity.SourceType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SourceType = null;
					else entity.SourceType = Convert.ToString(value);
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
			
			private esAdmedikaDailyMonitoringItem entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esAdmedikaDailyMonitoringItemQuery query)
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
				throw new Exception("esAdmedikaDailyMonitoringItem can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class AdmedikaDailyMonitoringItem : esAdmedikaDailyMonitoringItem
	{	
	}

	[Serializable]
	abstract public class esAdmedikaDailyMonitoringItemQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return AdmedikaDailyMonitoringItemMetadata.Meta();
			}
		}	
			
		public esQueryItem AdmedikaDailyMonitoringItemID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringItemID, esSystemType.Int64);
			}
		} 
		public esQueryItem AdmedikaDailyMonitoringID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringID, esSystemType.Int64);
			}
		} 
		public esQueryItem SeqNo
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.SeqNo, esSystemType.Int32);
			}
		} 
		public esQueryItem CategoryCode
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.CategoryCode, esSystemType.Int32);
			}
		} 
		public esQueryItem FormCode
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.FormCode, esSystemType.String);
			}
		} 
		public esQueryItem Remarks
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.Remarks, esSystemType.String);
			}
		} 
		public esQueryItem EntryDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.EntryDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem SourceType
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.SourceType, esSystemType.String);
			}
		} 
		public esQueryItem Status
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.Status, esSystemType.String);
			}
		} 
		public esQueryItem IsSent
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.IsSent, esSystemType.Boolean);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("AdmedikaDailyMonitoringItemCollection")]
	public partial class AdmedikaDailyMonitoringItemCollection : esAdmedikaDailyMonitoringItemCollection, IEnumerable< AdmedikaDailyMonitoringItem>
	{
		public AdmedikaDailyMonitoringItemCollection()
		{

		}	
		
		public static implicit operator List< AdmedikaDailyMonitoringItem>(AdmedikaDailyMonitoringItemCollection coll)
		{
			List< AdmedikaDailyMonitoringItem> list = new List< AdmedikaDailyMonitoringItem>();
			
			foreach (AdmedikaDailyMonitoringItem emp in coll)
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
				return  AdmedikaDailyMonitoringItemMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new AdmedikaDailyMonitoringItemQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new AdmedikaDailyMonitoringItem(row);
		}

		override protected esEntity CreateEntity()
		{
			return new AdmedikaDailyMonitoringItem();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public AdmedikaDailyMonitoringItemQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new AdmedikaDailyMonitoringItemQuery();
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
		public bool Load(AdmedikaDailyMonitoringItemQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public AdmedikaDailyMonitoringItem AddNew()
		{
			AdmedikaDailyMonitoringItem entity = base.AddNewEntity() as AdmedikaDailyMonitoringItem;
			
			return entity;		
		}
		public AdmedikaDailyMonitoringItem FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as AdmedikaDailyMonitoringItem;
		}

		#region IEnumerable< AdmedikaDailyMonitoringItem> Members

		IEnumerator< AdmedikaDailyMonitoringItem> IEnumerable< AdmedikaDailyMonitoringItem>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as AdmedikaDailyMonitoringItem;
			}
		}

		#endregion
		
		private AdmedikaDailyMonitoringItemQuery query;
	}


	/// <summary>
	/// Encapsulates the 'AdmedikaDailyMonitoringItem' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("AdmedikaDailyMonitoringItem ({StandardReferenceID})")]
	[Serializable]
	public partial class AdmedikaDailyMonitoringItem : esAdmedikaDailyMonitoringItem
	{
		public AdmedikaDailyMonitoringItem()
		{
		}	
	
		public AdmedikaDailyMonitoringItem(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return AdmedikaDailyMonitoringItemMetadata.Meta();
			}
		}	
	
		override protected esAdmedikaDailyMonitoringItemQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new AdmedikaDailyMonitoringItemQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public AdmedikaDailyMonitoringItemQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new AdmedikaDailyMonitoringItemQuery();
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
		public bool Load(AdmedikaDailyMonitoringItemQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private AdmedikaDailyMonitoringItemQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class AdmedikaDailyMonitoringItemQuery : esAdmedikaDailyMonitoringItemQuery
	{
		public AdmedikaDailyMonitoringItemQuery()
		{

		}		
		
		public AdmedikaDailyMonitoringItemQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "AdmedikaDailyMonitoringItemQuery";
        }
	}

	[Serializable]
	public partial class AdmedikaDailyMonitoringItemMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected AdmedikaDailyMonitoringItemMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringItemID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.AdmedikaDailyMonitoringItemID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.AdmedikaDailyMonitoringID, 1, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.AdmedikaDailyMonitoringID;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SeqNo, 2, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.SeqNo;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CategoryCode, 3, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.CategoryCode;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.FormCode, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.FormCode;
			c.CharacterMaxLength = 5;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Remarks, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.Remarks;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.EntryDateTime, 6, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.EntryDateTime;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.SourceType, 7, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.SourceType;
			c.CharacterMaxLength = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.Status, 8, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.Status;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.IsSent, 9, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.IsSent;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedDateTime, 10, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.CreatedDateTime;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.CreatedByUserID, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateDateTime, 12, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(AdmedikaDailyMonitoringItemMetadata.ColumnNames.LastUpdateByUserID, 13, typeof(System.String), esSystemType.String);
			c.PropertyName = AdmedikaDailyMonitoringItemMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public AdmedikaDailyMonitoringItemMetadata Meta()
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
			public const string AdmedikaDailyMonitoringItemID = "AdmedikaDailyMonitoringItemID";
			public const string AdmedikaDailyMonitoringID = "AdmedikaDailyMonitoringID";
			public const string SeqNo = "SeqNo";
			public const string CategoryCode = "CategoryCode";
			public const string FormCode = "FormCode";
			public const string Remarks = "Remarks";
			public const string EntryDateTime = "EntryDateTime";
			public const string SourceType = "SourceType";
			public const string Status = "Status";
			public const string IsSent = "IsSent";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string AdmedikaDailyMonitoringItemID = "AdmedikaDailyMonitoringItemID";
			public const string AdmedikaDailyMonitoringID = "AdmedikaDailyMonitoringID";
			public const string SeqNo = "SeqNo";
			public const string CategoryCode = "CategoryCode";
			public const string FormCode = "FormCode";
			public const string Remarks = "Remarks";
			public const string EntryDateTime = "EntryDateTime";
			public const string SourceType = "SourceType";
			public const string Status = "Status";
			public const string IsSent = "IsSent";
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
			lock (typeof(AdmedikaDailyMonitoringItemMetadata))
			{
				if(AdmedikaDailyMonitoringItemMetadata.mapDelegates == null)
				{
					AdmedikaDailyMonitoringItemMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (AdmedikaDailyMonitoringItemMetadata.meta == null)
				{
					AdmedikaDailyMonitoringItemMetadata.meta = new AdmedikaDailyMonitoringItemMetadata();
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
				
				meta.AddTypeMap("AdmedikaDailyMonitoringItemID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("AdmedikaDailyMonitoringID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("SeqNo", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("CategoryCode", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("FormCode", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Remarks", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("EntryDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("SourceType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Status", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsSent", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "AdmedikaDailyMonitoringItem";
				meta.Destination = "AdmedikaDailyMonitoringItem";
				meta.spInsert = "proc_AdmedikaDailyMonitoringItemInsert";				
				meta.spUpdate = "proc_AdmedikaDailyMonitoringItemUpdate";		
				meta.spDelete = "proc_AdmedikaDailyMonitoringItemDelete";
				meta.spLoadAll = "proc_AdmedikaDailyMonitoringItemLoadAll";
				meta.spLoadByPrimaryKey = "proc_AdmedikaDailyMonitoringItemLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private AdmedikaDailyMonitoringItemMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		