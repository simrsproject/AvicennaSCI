/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-27 01:45:59 PM
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
	abstract public class esTilakaCasemixFileCollection : esEntityCollectionWAuditLog
	{
		public esTilakaCasemixFileCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "TilakaCasemixFileCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esTilakaCasemixFileQuery query)
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
			this.InitQuery(query as esTilakaCasemixFileQuery);
		}
		#endregion
			
		virtual public TilakaCasemixFile DetachEntity(TilakaCasemixFile entity)
		{
			return base.DetachEntity(entity) as TilakaCasemixFile;
		}
		
		virtual public TilakaCasemixFile AttachEntity(TilakaCasemixFile entity)
		{
			return base.AttachEntity(entity) as TilakaCasemixFile;
		}
		
		virtual public void Combine(TilakaCasemixFileCollection collection)
		{
			base.Combine(collection);
		}
		
		new public TilakaCasemixFile this[int index]
		{
			get
			{
				return base[index] as TilakaCasemixFile;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(TilakaCasemixFile);
		}
	}

	[Serializable]
	abstract public class esTilakaCasemixFile : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esTilakaCasemixFileQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esTilakaCasemixFile()
		{
		}
	
		public esTilakaCasemixFile(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 casemixFileID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(casemixFileID);
			else
				return LoadByPrimaryKeyStoredProcedure(casemixFileID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 casemixFileID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(casemixFileID);
			else
				return LoadByPrimaryKeyStoredProcedure(casemixFileID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 casemixFileID)
		{
			esTilakaCasemixFileQuery query = this.GetDynamicQuery();
			query.Where(query.CasemixFileID == casemixFileID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 casemixFileID)
		{
			esParameters parms = new esParameters();
			parms.Add("CasemixFileID", casemixFileID);
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
						case "CasemixFileID": this.str.CasemixFileID = (string)value; break;
						case "RegistrationNo": this.str.RegistrationNo = (string)value; break;
						case "ParamedicID": this.str.ParamedicID = (string)value; break;
						case "FileName": this.str.FileName = (string)value; break;
						case "FilePath": this.str.FilePath = (string)value; break;
						case "DocumentType": this.str.DocumentType = (string)value; break;
						case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
						case "IsVoid": this.str.IsVoid = (string)value; break;
						case "LastQuickSignRequestID": this.str.LastQuickSignRequestID = (string)value; break;
						case "LastProcessedDateTime": this.str.LastProcessedDateTime = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
						case "DownloadPath": this.str.DownloadPath = (string)value; break;
						case "IsDeleted": this.str.IsDeleted = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "CasemixFileID":
						
							if (value == null || value is System.Int64)
								this.CasemixFileID = (System.Int64?)value;
							break;
						case "CreatedDateTime":
						
							if (value == null || value is System.DateTime)
								this.CreatedDateTime = (System.DateTime?)value;
							break;
						case "IsVoid":
						
							if (value == null || value is System.Boolean)
								this.IsVoid = (System.Boolean?)value;
							break;
						case "LastQuickSignRequestID":
						
							if (value == null || value is System.Int64)
								this.LastQuickSignRequestID = (System.Int64?)value;
							break;
						case "LastProcessedDateTime":
						
							if (value == null || value is System.DateTime)
								this.LastProcessedDateTime = (System.DateTime?)value;
							break;
						case "LastUpdateDateTime":
						
							if (value == null || value is System.DateTime)
								this.LastUpdateDateTime = (System.DateTime?)value;
							break;
						case "IsDeleted":
						
							if (value == null || value is System.Boolean)
								this.IsDeleted = (System.Boolean?)value;
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
		/// Maps to TilakaCasemixFile.CasemixFileID
		/// </summary>
		virtual public System.Int64? CasemixFileID
		{
			get
			{
				return base.GetSystemInt64(TilakaCasemixFileMetadata.ColumnNames.CasemixFileID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaCasemixFileMetadata.ColumnNames.CasemixFileID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.RegistrationNo
		/// </summary>
		virtual public System.String RegistrationNo
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.RegistrationNo);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.RegistrationNo, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.ParamedicID
		/// </summary>
		virtual public System.String ParamedicID
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.ParamedicID);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.ParamedicID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.FileName
		/// </summary>
		virtual public System.String FileName
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.FileName);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.FileName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.FilePath
		/// </summary>
		virtual public System.String FilePath
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.FilePath);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.FilePath, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.DocumentType
		/// </summary>
		virtual public System.String DocumentType
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.DocumentType);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.DocumentType, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.IsVoid
		/// </summary>
		virtual public System.Boolean? IsVoid
		{
			get
			{
				return base.GetSystemBoolean(TilakaCasemixFileMetadata.ColumnNames.IsVoid);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaCasemixFileMetadata.ColumnNames.IsVoid, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.LastQuickSignRequestID
		/// </summary>
		virtual public System.Int64? LastQuickSignRequestID
		{
			get
			{
				return base.GetSystemInt64(TilakaCasemixFileMetadata.ColumnNames.LastQuickSignRequestID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaCasemixFileMetadata.ColumnNames.LastQuickSignRequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.LastProcessedDateTime
		/// </summary>
		virtual public System.DateTime? LastProcessedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.LastProcessedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.LastProcessedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaCasemixFileMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.DownloadPath
		/// </summary>
		virtual public System.String DownloadPath
		{
			get
			{
				return base.GetSystemString(TilakaCasemixFileMetadata.ColumnNames.DownloadPath);
			}
			
			set
			{
				base.SetSystemString(TilakaCasemixFileMetadata.ColumnNames.DownloadPath, value);
			}
		}
		/// <summary>
		/// Maps to TilakaCasemixFile.IsDeleted
		/// </summary>
		virtual public System.Boolean? IsDeleted
		{
			get
			{
				return base.GetSystemBoolean(TilakaCasemixFileMetadata.ColumnNames.IsDeleted);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaCasemixFileMetadata.ColumnNames.IsDeleted, value);
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
			public esStrings(esTilakaCasemixFile entity)
			{
				this.entity = entity;
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
			public System.String RegistrationNo
			{
				get
				{
					System.String data = entity.RegistrationNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegistrationNo = null;
					else entity.RegistrationNo = Convert.ToString(value);
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
			public System.String DocumentType
			{
				get
				{
					System.String data = entity.DocumentType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DocumentType = null;
					else entity.DocumentType = Convert.ToString(value);
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
			public System.String IsVoid
			{
				get
				{
					System.Boolean? data = entity.IsVoid;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsVoid = null;
					else entity.IsVoid = Convert.ToBoolean(value);
				}
			}
			public System.String LastQuickSignRequestID
			{
				get
				{
					System.Int64? data = entity.LastQuickSignRequestID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.LastQuickSignRequestID = null;
					else entity.LastQuickSignRequestID = Convert.ToInt64(value);
				}
			}
			public System.String LastProcessedDateTime
			{
				get
				{
					System.DateTime? data = entity.LastProcessedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.LastProcessedDateTime = null;
					else entity.LastProcessedDateTime = Convert.ToDateTime(value);
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
			public System.String DownloadPath
			{
				get
				{
					System.String data = entity.DownloadPath;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DownloadPath = null;
					else entity.DownloadPath = Convert.ToString(value);
				}
			}
			public System.String IsDeleted
			{
				get
				{
					System.Boolean? data = entity.IsDeleted;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsDeleted = null;
					else entity.IsDeleted = Convert.ToBoolean(value);
				}
			}
			
			private esTilakaCasemixFile entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esTilakaCasemixFileQuery query)
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
				throw new Exception("esTilakaCasemixFile can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class TilakaCasemixFile : esTilakaCasemixFile
	{	
	}

	[Serializable]
	abstract public class esTilakaCasemixFileQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return TilakaCasemixFileMetadata.Meta();
			}
		}	
			
		public esQueryItem CasemixFileID
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.CasemixFileID, esSystemType.Int64);
			}
		} 
		public esQueryItem RegistrationNo
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.RegistrationNo, esSystemType.String);
			}
		} 
		public esQueryItem ParamedicID
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.ParamedicID, esSystemType.String);
			}
		} 
		public esQueryItem FileName
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.FileName, esSystemType.String);
			}
		} 
		public esQueryItem FilePath
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.FilePath, esSystemType.String);
			}
		} 
		public esQueryItem DocumentType
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.DocumentType, esSystemType.String);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem IsVoid
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.IsVoid, esSystemType.Boolean);
			}
		} 
		public esQueryItem LastQuickSignRequestID
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.LastQuickSignRequestID, esSystemType.Int64);
			}
		} 
		public esQueryItem LastProcessedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.LastProcessedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
		public esQueryItem DownloadPath
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.DownloadPath, esSystemType.String);
			}
		} 
		public esQueryItem IsDeleted
		{
			get
			{
				return new esQueryItem(this, TilakaCasemixFileMetadata.ColumnNames.IsDeleted, esSystemType.Boolean);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("TilakaCasemixFileCollection")]
	public partial class TilakaCasemixFileCollection : esTilakaCasemixFileCollection, IEnumerable< TilakaCasemixFile>
	{
		public TilakaCasemixFileCollection()
		{

		}	
		
		public static implicit operator List< TilakaCasemixFile>(TilakaCasemixFileCollection coll)
		{
			List< TilakaCasemixFile> list = new List< TilakaCasemixFile>();
			
			foreach (TilakaCasemixFile emp in coll)
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
				return  TilakaCasemixFileMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaCasemixFileQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new TilakaCasemixFile(row);
		}

		override protected esEntity CreateEntity()
		{
			return new TilakaCasemixFile();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public TilakaCasemixFileQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaCasemixFileQuery();
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
		public bool Load(TilakaCasemixFileQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public TilakaCasemixFile AddNew()
		{
			TilakaCasemixFile entity = base.AddNewEntity() as TilakaCasemixFile;
			
			return entity;		
		}
		public TilakaCasemixFile FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as TilakaCasemixFile;
		}

		#region IEnumerable< TilakaCasemixFile> Members

		IEnumerator< TilakaCasemixFile> IEnumerable< TilakaCasemixFile>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as TilakaCasemixFile;
			}
		}

		#endregion
		
		private TilakaCasemixFileQuery query;
	}


	/// <summary>
	/// Encapsulates the 'TilakaCasemixFile' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("TilakaCasemixFile ({StandardReferenceID})")]
	[Serializable]
	public partial class TilakaCasemixFile : esTilakaCasemixFile
	{
		public TilakaCasemixFile()
		{
		}	
	
		public TilakaCasemixFile(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return TilakaCasemixFileMetadata.Meta();
			}
		}	
	
		override protected esTilakaCasemixFileQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaCasemixFileQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public TilakaCasemixFileQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaCasemixFileQuery();
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
		public bool Load(TilakaCasemixFileQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private TilakaCasemixFileQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class TilakaCasemixFileQuery : esTilakaCasemixFileQuery
	{
		public TilakaCasemixFileQuery()
		{

		}		
		
		public TilakaCasemixFileQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "TilakaCasemixFileQuery";
        }
	}

	[Serializable]
	public partial class TilakaCasemixFileMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected TilakaCasemixFileMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.CasemixFileID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.CasemixFileID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.RegistrationNo, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.RegistrationNo;
			c.CharacterMaxLength = 40;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.ParamedicID, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.ParamedicID;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.FileName, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.FileName;
			c.CharacterMaxLength = 255;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.FilePath, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.FilePath;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.DocumentType, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.DocumentType;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.CreatedDateTime, 6, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.CreatedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.IsVoid, 7, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.IsVoid;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.LastQuickSignRequestID, 8, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.LastQuickSignRequestID;
			c.NumericPrecision = 19;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.LastProcessedDateTime, 9, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.LastProcessedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.LastUpdateDateTime, 10, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.LastUpdateByUserID, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.DownloadPath, 12, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.DownloadPath;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaCasemixFileMetadata.ColumnNames.IsDeleted, 13, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaCasemixFileMetadata.PropertyNames.IsDeleted;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
		}
		#endregion
	
		static public TilakaCasemixFileMetadata Meta()
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
			public const string CasemixFileID = "CasemixFileID";
			public const string RegistrationNo = "RegistrationNo";
			public const string ParamedicID = "ParamedicID";
			public const string FileName = "FileName";
			public const string FilePath = "FilePath";
			public const string DocumentType = "DocumentType";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string IsVoid = "IsVoid";
			public const string LastQuickSignRequestID = "LastQuickSignRequestID";
			public const string LastProcessedDateTime = "LastProcessedDateTime";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string DownloadPath = "DownloadPath";
			public const string IsDeleted = "IsDeleted";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string CasemixFileID = "CasemixFileID";
			public const string RegistrationNo = "RegistrationNo";
			public const string ParamedicID = "ParamedicID";
			public const string FileName = "FileName";
			public const string FilePath = "FilePath";
			public const string DocumentType = "DocumentType";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string IsVoid = "IsVoid";
			public const string LastQuickSignRequestID = "LastQuickSignRequestID";
			public const string LastProcessedDateTime = "LastProcessedDateTime";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string DownloadPath = "DownloadPath";
			public const string IsDeleted = "IsDeleted";
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
			lock (typeof(TilakaCasemixFileMetadata))
			{
				if(TilakaCasemixFileMetadata.mapDelegates == null)
				{
					TilakaCasemixFileMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (TilakaCasemixFileMetadata.meta == null)
				{
					TilakaCasemixFileMetadata.meta = new TilakaCasemixFileMetadata();
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
				
				meta.AddTypeMap("CasemixFileID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("RegistrationNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ParamedicID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("FileName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("FilePath", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("DocumentType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("IsVoid", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("LastQuickSignRequestID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("LastProcessedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("DownloadPath", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsDeleted", new esTypeMap("bit", "System.Boolean"));
		

				meta.Source = "TilakaCasemixFile";
				meta.Destination = "TilakaCasemixFile";
				meta.spInsert = "proc_TilakaCasemixFileInsert";				
				meta.spUpdate = "proc_TilakaCasemixFileUpdate";		
				meta.spDelete = "proc_TilakaCasemixFileDelete";
				meta.spLoadAll = "proc_TilakaCasemixFileLoadAll";
				meta.spLoadByPrimaryKey = "proc_TilakaCasemixFileLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private TilakaCasemixFileMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		