/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-20 09:40:03 PM
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
	abstract public class esTilakaQuickSignRequestCollection : esEntityCollectionWAuditLog
	{
		public esTilakaQuickSignRequestCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "TilakaQuickSignRequestCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esTilakaQuickSignRequestQuery query)
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
			this.InitQuery(query as esTilakaQuickSignRequestQuery);
		}
		#endregion
			
		virtual public TilakaQuickSignRequest DetachEntity(TilakaQuickSignRequest entity)
		{
			return base.DetachEntity(entity) as TilakaQuickSignRequest;
		}
		
		virtual public TilakaQuickSignRequest AttachEntity(TilakaQuickSignRequest entity)
		{
			return base.AttachEntity(entity) as TilakaQuickSignRequest;
		}
		
		virtual public void Combine(TilakaQuickSignRequestCollection collection)
		{
			base.Combine(collection);
		}
		
		new public TilakaQuickSignRequest this[int index]
		{
			get
			{
				return base[index] as TilakaQuickSignRequest;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(TilakaQuickSignRequest);
		}
	}

	[Serializable]
	abstract public class esTilakaQuickSignRequest : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esTilakaQuickSignRequestQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esTilakaQuickSignRequest()
		{
		}
	
		public esTilakaQuickSignRequest(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 quickSignRequestID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(quickSignRequestID);
			else
				return LoadByPrimaryKeyStoredProcedure(quickSignRequestID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 quickSignRequestID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(quickSignRequestID);
			else
				return LoadByPrimaryKeyStoredProcedure(quickSignRequestID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 quickSignRequestID)
		{
			esTilakaQuickSignRequestQuery query = this.GetDynamicQuery();
			query.Where(query.QuickSignRequestID == quickSignRequestID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 quickSignRequestID)
		{
			esParameters parms = new esParameters();
			parms.Add("QuickSignRequestID", quickSignRequestID);
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
						case "QuickSignRequestID": this.str.QuickSignRequestID = (string)value; break;
						case "RequestID": this.str.RequestID = (string)value; break;
						case "CasemixFileID": this.str.CasemixFileID = (string)value; break;
						case "FileName": this.str.FileName = (string)value; break;
						case "TemplateNo": this.str.TemplateNo = (string)value; break;
						case "Status": this.str.Status = (string)value; break;
						case "IsActive": this.str.IsActive = (string)value; break;
						case "RequestDateTime": this.str.RequestDateTime = (string)value; break;
						case "CompletedDateTime": this.str.CompletedDateTime = (string)value; break;
						case "SignedFilePath": this.str.SignedFilePath = (string)value; break;
						case "ErrorMessage": this.str.ErrorMessage = (string)value; break;
						case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
						case "CreatedByUserID": this.str.CreatedByUserID = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
						case "TemplateSignFileID": this.str.TemplateSignFileID = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "QuickSignRequestID":
						
							if (value == null || value is System.Int64)
								this.QuickSignRequestID = (System.Int64?)value;
							break;
						case "CasemixFileID":
						
							if (value == null || value is System.Int64)
								this.CasemixFileID = (System.Int64?)value;
							break;
						case "IsActive":
						
							if (value == null || value is System.Boolean)
								this.IsActive = (System.Boolean?)value;
							break;
						case "RequestDateTime":
						
							if (value == null || value is System.DateTime)
								this.RequestDateTime = (System.DateTime?)value;
							break;
						case "CompletedDateTime":
						
							if (value == null || value is System.DateTime)
								this.CompletedDateTime = (System.DateTime?)value;
							break;
						case "CreatedDateTime":
						
							if (value == null || value is System.DateTime)
								this.CreatedDateTime = (System.DateTime?)value;
							break;
						case "LastUpdateDateTime":
						
							if (value == null || value is System.DateTime)
								this.LastUpdateDateTime = (System.DateTime?)value;
							break;
						case "TemplateSignFileID":
						
							if (value == null || value is System.Int64)
								this.TemplateSignFileID = (System.Int64?)value;
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
		/// Maps to TilakaQuickSignRequest.QuickSignRequestID
		/// </summary>
		virtual public System.Int64? QuickSignRequestID
		{
			get
			{
				return base.GetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.QuickSignRequestID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.QuickSignRequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.RequestID
		/// </summary>
		virtual public System.String RequestID
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.RequestID);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.RequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.CasemixFileID
		/// </summary>
		virtual public System.Int64? CasemixFileID
		{
			get
			{
				return base.GetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.CasemixFileID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.CasemixFileID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.FileName
		/// </summary>
		virtual public System.String FileName
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.FileName);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.FileName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.TemplateNo
		/// </summary>
		virtual public System.String TemplateNo
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.TemplateNo);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.TemplateNo, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.Status
		/// </summary>
		virtual public System.String Status
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.Status);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.Status, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.IsActive
		/// </summary>
		virtual public System.Boolean? IsActive
		{
			get
			{
				return base.GetSystemBoolean(TilakaQuickSignRequestMetadata.ColumnNames.IsActive);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaQuickSignRequestMetadata.ColumnNames.IsActive, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.RequestDateTime
		/// </summary>
		virtual public System.DateTime? RequestDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.RequestDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.RequestDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.CompletedDateTime
		/// </summary>
		virtual public System.DateTime? CompletedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.CompletedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.CompletedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.SignedFilePath
		/// </summary>
		virtual public System.String SignedFilePath
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.SignedFilePath);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.SignedFilePath, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.ErrorMessage
		/// </summary>
		virtual public System.String ErrorMessage
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.ErrorMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.ErrorMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignRequest.TemplateSignFileID
		/// </summary>
		virtual public System.Int64? TemplateSignFileID
		{
			get
			{
				return base.GetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.TemplateSignFileID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaQuickSignRequestMetadata.ColumnNames.TemplateSignFileID, value);
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
			public esStrings(esTilakaQuickSignRequest entity)
			{
				this.entity = entity;
			}
			public System.String QuickSignRequestID
			{
				get
				{
					System.Int64? data = entity.QuickSignRequestID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QuickSignRequestID = null;
					else entity.QuickSignRequestID = Convert.ToInt64(value);
				}
			}
			public System.String RequestID
			{
				get
				{
					System.String data = entity.RequestID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RequestID = null;
					else entity.RequestID = Convert.ToString(value);
				}
			}
			public System.String CasemixFileID
			{
				get
				{
					System.Int64? data = entity.CasemixFileID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CasemixFileID = null;
					else entity.CasemixFileID = Convert.ToInt64(value);
				}
			}
			public System.String FileName
			{
				get
				{
					System.String data = entity.FileName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.FileName = null;
					else entity.FileName = Convert.ToString(value);
				}
			}
			public System.String TemplateNo
			{
				get
				{
					System.String data = entity.TemplateNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TemplateNo = null;
					else entity.TemplateNo = Convert.ToString(value);
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
			public System.String RequestDateTime
			{
				get
				{
					System.DateTime? data = entity.RequestDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RequestDateTime = null;
					else entity.RequestDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String CompletedDateTime
			{
				get
				{
					System.DateTime? data = entity.CompletedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CompletedDateTime = null;
					else entity.CompletedDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String SignedFilePath
			{
				get
				{
					System.String data = entity.SignedFilePath;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SignedFilePath = null;
					else entity.SignedFilePath = Convert.ToString(value);
				}
			}
			public System.String ErrorMessage
			{
				get
				{
					System.String data = entity.ErrorMessage;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ErrorMessage = null;
					else entity.ErrorMessage = Convert.ToString(value);
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
			public System.String TemplateSignFileID
			{
				get
				{
					System.Int64? data = entity.TemplateSignFileID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TemplateSignFileID = null;
					else entity.TemplateSignFileID = Convert.ToInt64(value);
				}
			}
			
			private esTilakaQuickSignRequest entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esTilakaQuickSignRequestQuery query)
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
				throw new Exception("esTilakaQuickSignRequest can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class TilakaQuickSignRequest : esTilakaQuickSignRequest
	{	
	}

	[Serializable]
	abstract public class esTilakaQuickSignRequestQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return TilakaQuickSignRequestMetadata.Meta();
			}
		}	
			
		public esQueryItem QuickSignRequestID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.QuickSignRequestID, esSystemType.Int64);
			}
		} 
		public esQueryItem RequestID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.RequestID, esSystemType.String);
			}
		} 
		public esQueryItem CasemixFileID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.CasemixFileID, esSystemType.Int64);
			}
		} 
		public esQueryItem FileName
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.FileName, esSystemType.String);
			}
		} 
		public esQueryItem TemplateNo
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.TemplateNo, esSystemType.String);
			}
		} 
		public esQueryItem Status
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.Status, esSystemType.String);
			}
		} 
		public esQueryItem IsActive
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.IsActive, esSystemType.Boolean);
			}
		} 
		public esQueryItem RequestDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.RequestDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CompletedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.CompletedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem SignedFilePath
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.SignedFilePath, esSystemType.String);
			}
		} 
		public esQueryItem ErrorMessage
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.ErrorMessage, esSystemType.String);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
		public esQueryItem TemplateSignFileID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignRequestMetadata.ColumnNames.TemplateSignFileID, esSystemType.Int64);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("TilakaQuickSignRequestCollection")]
	public partial class TilakaQuickSignRequestCollection : esTilakaQuickSignRequestCollection, IEnumerable< TilakaQuickSignRequest>
	{
		public TilakaQuickSignRequestCollection()
		{

		}	
		
		public static implicit operator List< TilakaQuickSignRequest>(TilakaQuickSignRequestCollection coll)
		{
			List< TilakaQuickSignRequest> list = new List< TilakaQuickSignRequest>();
			
			foreach (TilakaQuickSignRequest emp in coll)
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
				return  TilakaQuickSignRequestMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaQuickSignRequestQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new TilakaQuickSignRequest(row);
		}

		override protected esEntity CreateEntity()
		{
			return new TilakaQuickSignRequest();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public TilakaQuickSignRequestQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaQuickSignRequestQuery();
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
		public bool Load(TilakaQuickSignRequestQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public TilakaQuickSignRequest AddNew()
		{
			TilakaQuickSignRequest entity = base.AddNewEntity() as TilakaQuickSignRequest;
			
			return entity;		
		}
		public TilakaQuickSignRequest FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as TilakaQuickSignRequest;
		}

		#region IEnumerable< TilakaQuickSignRequest> Members

		IEnumerator< TilakaQuickSignRequest> IEnumerable< TilakaQuickSignRequest>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as TilakaQuickSignRequest;
			}
		}

		#endregion
		
		private TilakaQuickSignRequestQuery query;
	}


	/// <summary>
	/// Encapsulates the 'TilakaQuickSignRequest' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("TilakaQuickSignRequest ({StandardReferenceID})")]
	[Serializable]
	public partial class TilakaQuickSignRequest : esTilakaQuickSignRequest
	{
		public TilakaQuickSignRequest()
		{
		}	
	
		public TilakaQuickSignRequest(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return TilakaQuickSignRequestMetadata.Meta();
			}
		}	
	
		override protected esTilakaQuickSignRequestQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaQuickSignRequestQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public TilakaQuickSignRequestQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaQuickSignRequestQuery();
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
		public bool Load(TilakaQuickSignRequestQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private TilakaQuickSignRequestQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class TilakaQuickSignRequestQuery : esTilakaQuickSignRequestQuery
	{
		public TilakaQuickSignRequestQuery()
		{

		}		
		
		public TilakaQuickSignRequestQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "TilakaQuickSignRequestQuery";
        }
	}

	[Serializable]
	public partial class TilakaQuickSignRequestMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected TilakaQuickSignRequestMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.QuickSignRequestID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.QuickSignRequestID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.RequestID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.RequestID;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.CasemixFileID, 2, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.CasemixFileID;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.FileName, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.FileName;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.TemplateNo, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.TemplateNo;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.Status, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.Status;
			c.CharacterMaxLength = 20;
			c.HasDefault = true;
			c.Default = @"('Requested')";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.IsActive, 6, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.IsActive;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.RequestDateTime, 7, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.RequestDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.CompletedDateTime, 8, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.CompletedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.SignedFilePath, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.SignedFilePath;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.ErrorMessage, 10, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.ErrorMessage;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.CreatedDateTime, 11, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.CreatedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.CreatedByUserID, 12, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateDateTime, 13, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.LastUpdateByUserID, 14, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignRequestMetadata.ColumnNames.TemplateSignFileID, 15, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaQuickSignRequestMetadata.PropertyNames.TemplateSignFileID;
			c.NumericPrecision = 19;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public TilakaQuickSignRequestMetadata Meta()
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
			public const string QuickSignRequestID = "QuickSignRequestID";
			public const string RequestID = "RequestID";
			public const string CasemixFileID = "CasemixFileID";
			public const string FileName = "FileName";
			public const string TemplateNo = "TemplateNo";
			public const string Status = "Status";
			public const string IsActive = "IsActive";
			public const string RequestDateTime = "RequestDateTime";
			public const string CompletedDateTime = "CompletedDateTime";
			public const string SignedFilePath = "SignedFilePath";
			public const string ErrorMessage = "ErrorMessage";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string TemplateSignFileID = "TemplateSignFileID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string QuickSignRequestID = "QuickSignRequestID";
			public const string RequestID = "RequestID";
			public const string CasemixFileID = "CasemixFileID";
			public const string FileName = "FileName";
			public const string TemplateNo = "TemplateNo";
			public const string Status = "Status";
			public const string IsActive = "IsActive";
			public const string RequestDateTime = "RequestDateTime";
			public const string CompletedDateTime = "CompletedDateTime";
			public const string SignedFilePath = "SignedFilePath";
			public const string ErrorMessage = "ErrorMessage";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string TemplateSignFileID = "TemplateSignFileID";
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
			lock (typeof(TilakaQuickSignRequestMetadata))
			{
				if(TilakaQuickSignRequestMetadata.mapDelegates == null)
				{
					TilakaQuickSignRequestMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (TilakaQuickSignRequestMetadata.meta == null)
				{
					TilakaQuickSignRequestMetadata.meta = new TilakaQuickSignRequestMetadata();
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
				
				meta.AddTypeMap("QuickSignRequestID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("RequestID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("CasemixFileID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("FileName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TemplateNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Status", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("RequestDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CompletedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("SignedFilePath", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ErrorMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TemplateSignFileID", new esTypeMap("bigint", "System.Int64"));
		

				meta.Source = "TilakaQuickSignRequest";
				meta.Destination = "TilakaQuickSignRequest";
				meta.spInsert = "proc_TilakaQuickSignRequestInsert";				
				meta.spUpdate = "proc_TilakaQuickSignRequestUpdate";		
				meta.spDelete = "proc_TilakaQuickSignRequestDelete";
				meta.spLoadAll = "proc_TilakaQuickSignRequestLoadAll";
				meta.spLoadByPrimaryKey = "proc_TilakaQuickSignRequestLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private TilakaQuickSignRequestMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		