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
	abstract public class esTilakaTemplateSignFileCollection : esEntityCollectionWAuditLog
	{
		public esTilakaTemplateSignFileCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "TilakaTemplateSignFileCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esTilakaTemplateSignFileQuery query)
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
			this.InitQuery(query as esTilakaTemplateSignFileQuery);
		}
		#endregion
			
		virtual public TilakaTemplateSignFile DetachEntity(TilakaTemplateSignFile entity)
		{
			return base.DetachEntity(entity) as TilakaTemplateSignFile;
		}
		
		virtual public TilakaTemplateSignFile AttachEntity(TilakaTemplateSignFile entity)
		{
			return base.AttachEntity(entity) as TilakaTemplateSignFile;
		}
		
		virtual public void Combine(TilakaTemplateSignFileCollection collection)
		{
			base.Combine(collection);
		}
		
		new public TilakaTemplateSignFile this[int index]
		{
			get
			{
				return base[index] as TilakaTemplateSignFile;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(TilakaTemplateSignFile);
		}
	}

	[Serializable]
	abstract public class esTilakaTemplateSignFile : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esTilakaTemplateSignFileQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esTilakaTemplateSignFile()
		{
		}
	
		public esTilakaTemplateSignFile(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 templateSignFileID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateSignFileID);
			else
				return LoadByPrimaryKeyStoredProcedure(templateSignFileID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 templateSignFileID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateSignFileID);
			else
				return LoadByPrimaryKeyStoredProcedure(templateSignFileID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 templateSignFileID)
		{
			esTilakaTemplateSignFileQuery query = this.GetDynamicQuery();
			query.Where(query.TemplateSignFileID == templateSignFileID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 templateSignFileID)
		{
			esParameters parms = new esParameters();
			parms.Add("TemplateSignFileID", templateSignFileID);
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
						case "TemplateSignFileID": this.str.TemplateSignFileID = (string)value; break;
						case "ParamedicID": this.str.ParamedicID = (string)value; break;
						case "TemplateNo": this.str.TemplateNo = (string)value; break;
						case "FileName": this.str.FileName = (string)value; break;
						case "FilePath": this.str.FilePath = (string)value; break;
						case "ContentType": this.str.ContentType = (string)value; break;
						case "FileSize": this.str.FileSize = (string)value; break;
						case "IsDefault": this.str.IsDefault = (string)value; break;
						case "IsActive": this.str.IsActive = (string)value; break;
						case "IsRegistered": this.str.IsRegistered = (string)value; break;
						case "RegisteredDateTime": this.str.RegisteredDateTime = (string)value; break;
						case "LastErrorMessage": this.str.LastErrorMessage = (string)value; break;
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
						case "TemplateSignFileID":
						
							if (value == null || value is System.Int64)
								this.TemplateSignFileID = (System.Int64?)value;
							break;
						case "FileSize":
						
							if (value == null || value is System.Int64)
								this.FileSize = (System.Int64?)value;
							break;
						case "IsDefault":
						
							if (value == null || value is System.Boolean)
								this.IsDefault = (System.Boolean?)value;
							break;
						case "IsActive":
						
							if (value == null || value is System.Boolean)
								this.IsActive = (System.Boolean?)value;
							break;
						case "IsRegistered":
						
							if (value == null || value is System.Boolean)
								this.IsRegistered = (System.Boolean?)value;
							break;
						case "RegisteredDateTime":
						
							if (value == null || value is System.DateTime)
								this.RegisteredDateTime = (System.DateTime?)value;
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
		/// Maps to TilakaTemplateSignFile.TemplateSignFileID
		/// </summary>
		virtual public System.Int64? TemplateSignFileID
		{
			get
			{
				return base.GetSystemInt64(TilakaTemplateSignFileMetadata.ColumnNames.TemplateSignFileID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaTemplateSignFileMetadata.ColumnNames.TemplateSignFileID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.ParamedicID
		/// </summary>
		virtual public System.String ParamedicID
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.ParamedicID);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.ParamedicID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.TemplateNo
		/// </summary>
		virtual public System.String TemplateNo
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.TemplateNo);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.TemplateNo, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.FileName
		/// </summary>
		virtual public System.String FileName
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.FileName);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.FileName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.FilePath
		/// </summary>
		virtual public System.String FilePath
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.FilePath);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.FilePath, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.ContentType
		/// </summary>
		virtual public System.String ContentType
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.ContentType);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.ContentType, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.FileSize
		/// </summary>
		virtual public System.Int64? FileSize
		{
			get
			{
				return base.GetSystemInt64(TilakaTemplateSignFileMetadata.ColumnNames.FileSize);
			}
			
			set
			{
				base.SetSystemInt64(TilakaTemplateSignFileMetadata.ColumnNames.FileSize, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.IsDefault
		/// </summary>
		virtual public System.Boolean? IsDefault
		{
			get
			{
				return base.GetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsDefault);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsDefault, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.IsActive
		/// </summary>
		virtual public System.Boolean? IsActive
		{
			get
			{
				return base.GetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsActive);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsActive, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.IsRegistered
		/// </summary>
		virtual public System.Boolean? IsRegistered
		{
			get
			{
				return base.GetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsRegistered);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaTemplateSignFileMetadata.ColumnNames.IsRegistered, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.RegisteredDateTime
		/// </summary>
		virtual public System.DateTime? RegisteredDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.RegisteredDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.RegisteredDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.LastErrorMessage
		/// </summary>
		virtual public System.String LastErrorMessage
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.LastErrorMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.LastErrorMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaTemplateSignFile.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esTilakaTemplateSignFile entity)
			{
				this.entity = entity;
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
			public System.String ParamedicID
			{
				get
				{
					System.String data = entity.ParamedicID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ParamedicID = null;
					else entity.ParamedicID = Convert.ToString(value);
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
			public System.String FilePath
			{
				get
				{
					System.String data = entity.FilePath;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.FilePath = null;
					else entity.FilePath = Convert.ToString(value);
				}
			}
			public System.String ContentType
			{
				get
				{
					System.String data = entity.ContentType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ContentType = null;
					else entity.ContentType = Convert.ToString(value);
				}
			}
			public System.String FileSize
			{
				get
				{
					System.Int64? data = entity.FileSize;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.FileSize = null;
					else entity.FileSize = Convert.ToInt64(value);
				}
			}
			public System.String IsDefault
			{
				get
				{
					System.Boolean? data = entity.IsDefault;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsDefault = null;
					else entity.IsDefault = Convert.ToBoolean(value);
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
			public System.String IsRegistered
			{
				get
				{
					System.Boolean? data = entity.IsRegistered;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsRegistered = null;
					else entity.IsRegistered = Convert.ToBoolean(value);
				}
			}
			public System.String RegisteredDateTime
			{
				get
				{
					System.DateTime? data = entity.RegisteredDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegisteredDateTime = null;
					else entity.RegisteredDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String LastErrorMessage
			{
				get
				{
					System.String data = entity.LastErrorMessage;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.LastErrorMessage = null;
					else entity.LastErrorMessage = Convert.ToString(value);
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
			
			private esTilakaTemplateSignFile entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esTilakaTemplateSignFileQuery query)
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
				throw new Exception("esTilakaTemplateSignFile can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class TilakaTemplateSignFile : esTilakaTemplateSignFile
	{	
	}

	[Serializable]
	abstract public class esTilakaTemplateSignFileQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return TilakaTemplateSignFileMetadata.Meta();
			}
		}	
			
		public esQueryItem TemplateSignFileID
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.TemplateSignFileID, esSystemType.Int64);
			}
		} 
		public esQueryItem ParamedicID
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.ParamedicID, esSystemType.String);
			}
		} 
		public esQueryItem TemplateNo
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.TemplateNo, esSystemType.String);
			}
		} 
		public esQueryItem FileName
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.FileName, esSystemType.String);
			}
		} 
		public esQueryItem FilePath
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.FilePath, esSystemType.String);
			}
		} 
		public esQueryItem ContentType
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.ContentType, esSystemType.String);
			}
		} 
		public esQueryItem FileSize
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.FileSize, esSystemType.Int64);
			}
		} 
		public esQueryItem IsDefault
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.IsDefault, esSystemType.Boolean);
			}
		} 
		public esQueryItem IsActive
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.IsActive, esSystemType.Boolean);
			}
		} 
		public esQueryItem IsRegistered
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.IsRegistered, esSystemType.Boolean);
			}
		} 
		public esQueryItem RegisteredDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.RegisteredDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastErrorMessage
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.LastErrorMessage, esSystemType.String);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("TilakaTemplateSignFileCollection")]
	public partial class TilakaTemplateSignFileCollection : esTilakaTemplateSignFileCollection, IEnumerable< TilakaTemplateSignFile>
	{
		public TilakaTemplateSignFileCollection()
		{

		}	
		
		public static implicit operator List< TilakaTemplateSignFile>(TilakaTemplateSignFileCollection coll)
		{
			List< TilakaTemplateSignFile> list = new List< TilakaTemplateSignFile>();
			
			foreach (TilakaTemplateSignFile emp in coll)
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
				return  TilakaTemplateSignFileMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaTemplateSignFileQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new TilakaTemplateSignFile(row);
		}

		override protected esEntity CreateEntity()
		{
			return new TilakaTemplateSignFile();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public TilakaTemplateSignFileQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaTemplateSignFileQuery();
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
		public bool Load(TilakaTemplateSignFileQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public TilakaTemplateSignFile AddNew()
		{
			TilakaTemplateSignFile entity = base.AddNewEntity() as TilakaTemplateSignFile;
			
			return entity;		
		}
		public TilakaTemplateSignFile FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as TilakaTemplateSignFile;
		}

		#region IEnumerable< TilakaTemplateSignFile> Members

		IEnumerator< TilakaTemplateSignFile> IEnumerable< TilakaTemplateSignFile>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as TilakaTemplateSignFile;
			}
		}

		#endregion
		
		private TilakaTemplateSignFileQuery query;
	}


	/// <summary>
	/// Encapsulates the 'TilakaTemplateSignFile' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("TilakaTemplateSignFile ({StandardReferenceID})")]
	[Serializable]
	public partial class TilakaTemplateSignFile : esTilakaTemplateSignFile
	{
		public TilakaTemplateSignFile()
		{
		}	
	
		public TilakaTemplateSignFile(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return TilakaTemplateSignFileMetadata.Meta();
			}
		}	
	
		override protected esTilakaTemplateSignFileQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaTemplateSignFileQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public TilakaTemplateSignFileQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaTemplateSignFileQuery();
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
		public bool Load(TilakaTemplateSignFileQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private TilakaTemplateSignFileQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class TilakaTemplateSignFileQuery : esTilakaTemplateSignFileQuery
	{
		public TilakaTemplateSignFileQuery()
		{

		}		
		
		public TilakaTemplateSignFileQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "TilakaTemplateSignFileQuery";
        }
	}

	[Serializable]
	public partial class TilakaTemplateSignFileMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected TilakaTemplateSignFileMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.TemplateSignFileID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.TemplateSignFileID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.ParamedicID, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.ParamedicID;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.TemplateNo, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.TemplateNo;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.FileName, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.FileName;
			c.CharacterMaxLength = 255;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.FilePath, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.FilePath;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.ContentType, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.ContentType;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.FileSize, 6, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.FileSize;
			c.NumericPrecision = 19;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.IsDefault, 7, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.IsDefault;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.IsActive, 8, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.IsActive;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.IsRegistered, 9, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.IsRegistered;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.RegisteredDateTime, 10, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.RegisteredDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.LastErrorMessage, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.LastErrorMessage;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.CreatedDateTime, 12, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.CreatedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.CreatedByUserID, 13, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateDateTime, 14, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaTemplateSignFileMetadata.ColumnNames.LastUpdateByUserID, 15, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaTemplateSignFileMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public TilakaTemplateSignFileMetadata Meta()
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
			public const string TemplateSignFileID = "TemplateSignFileID";
			public const string ParamedicID = "ParamedicID";
			public const string TemplateNo = "TemplateNo";
			public const string FileName = "FileName";
			public const string FilePath = "FilePath";
			public const string ContentType = "ContentType";
			public const string FileSize = "FileSize";
			public const string IsDefault = "IsDefault";
			public const string IsActive = "IsActive";
			public const string IsRegistered = "IsRegistered";
			public const string RegisteredDateTime = "RegisteredDateTime";
			public const string LastErrorMessage = "LastErrorMessage";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string TemplateSignFileID = "TemplateSignFileID";
			public const string ParamedicID = "ParamedicID";
			public const string TemplateNo = "TemplateNo";
			public const string FileName = "FileName";
			public const string FilePath = "FilePath";
			public const string ContentType = "ContentType";
			public const string FileSize = "FileSize";
			public const string IsDefault = "IsDefault";
			public const string IsActive = "IsActive";
			public const string IsRegistered = "IsRegistered";
			public const string RegisteredDateTime = "RegisteredDateTime";
			public const string LastErrorMessage = "LastErrorMessage";
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
			lock (typeof(TilakaTemplateSignFileMetadata))
			{
				if(TilakaTemplateSignFileMetadata.mapDelegates == null)
				{
					TilakaTemplateSignFileMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (TilakaTemplateSignFileMetadata.meta == null)
				{
					TilakaTemplateSignFileMetadata.meta = new TilakaTemplateSignFileMetadata();
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
				
				meta.AddTypeMap("TemplateSignFileID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("ParamedicID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TemplateNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("FileName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("FilePath", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ContentType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("FileSize", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("IsDefault", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("IsActive", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("IsRegistered", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("RegisteredDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastErrorMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "TilakaTemplateSignFile";
				meta.Destination = "TilakaTemplateSignFile";
				meta.spInsert = "proc_TilakaTemplateSignFileInsert";				
				meta.spUpdate = "proc_TilakaTemplateSignFileUpdate";		
				meta.spDelete = "proc_TilakaTemplateSignFileDelete";
				meta.spLoadAll = "proc_TilakaTemplateSignFileLoadAll";
				meta.spLoadByPrimaryKey = "proc_TilakaTemplateSignFileLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private TilakaTemplateSignFileMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		