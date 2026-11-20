/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-20 07:53:57 PM
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
	abstract public class esTilakaQuickSignSignerCollection : esEntityCollectionWAuditLog
	{
		public esTilakaQuickSignSignerCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "TilakaQuickSignSignerCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esTilakaQuickSignSignerQuery query)
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
			this.InitQuery(query as esTilakaQuickSignSignerQuery);
		}
		#endregion
			
		virtual public TilakaQuickSignSigner DetachEntity(TilakaQuickSignSigner entity)
		{
			return base.DetachEntity(entity) as TilakaQuickSignSigner;
		}
		
		virtual public TilakaQuickSignSigner AttachEntity(TilakaQuickSignSigner entity)
		{
			return base.AttachEntity(entity) as TilakaQuickSignSigner;
		}
		
		virtual public void Combine(TilakaQuickSignSignerCollection collection)
		{
			base.Combine(collection);
		}
		
		new public TilakaQuickSignSigner this[int index]
		{
			get
			{
				return base[index] as TilakaQuickSignSigner;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(TilakaQuickSignSigner);
		}
	}

	[Serializable]
	abstract public class esTilakaQuickSignSigner : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esTilakaQuickSignSignerQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esTilakaQuickSignSigner()
		{
		}
	
		public esTilakaQuickSignSigner(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 quickSignSignerID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(quickSignSignerID);
			else
				return LoadByPrimaryKeyStoredProcedure(quickSignSignerID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 quickSignSignerID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(quickSignSignerID);
			else
				return LoadByPrimaryKeyStoredProcedure(quickSignSignerID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 quickSignSignerID)
		{
			esTilakaQuickSignSignerQuery query = this.GetDynamicQuery();
			query.Where(query.QuickSignSignerID == quickSignSignerID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 quickSignSignerID)
		{
			esParameters parms = new esParameters();
			parms.Add("QuickSignSignerID", quickSignSignerID);
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
						case "QuickSignSignerID": this.str.QuickSignSignerID = (string)value; break;
						case "QuickSignRequestID": this.str.QuickSignRequestID = (string)value; break;
						case "Sequence": this.str.Sequence = (string)value; break;
						case "UserIdentifier": this.str.UserIdentifier = (string)value; break;
						case "Email": this.str.Email = (string)value; break;
						case "Status": this.str.Status = (string)value; break;
						case "NumSignatures": this.str.NumSignatures = (string)value; break;
						case "NumSignaturesDone": this.str.NumSignaturesDone = (string)value; break;
						case "AuthUrl": this.str.AuthUrl = (string)value; break;
						case "TokenKey": this.str.TokenKey = (string)value; break;
						case "SignedUrl": this.str.SignedUrl = (string)value; break;
						case "ErrorMessage": this.str.ErrorMessage = (string)value; break;
						case "LastCheckedDateTime": this.str.LastCheckedDateTime = (string)value; break;
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
						case "QuickSignSignerID":
						
							if (value == null || value is System.Int64)
								this.QuickSignSignerID = (System.Int64?)value;
							break;
						case "QuickSignRequestID":
						
							if (value == null || value is System.Int64)
								this.QuickSignRequestID = (System.Int64?)value;
							break;
						case "Sequence":
						
							if (value == null || value is System.Int32)
								this.Sequence = (System.Int32?)value;
							break;
						case "NumSignatures":
						
							if (value == null || value is System.Int32)
								this.NumSignatures = (System.Int32?)value;
							break;
						case "NumSignaturesDone":
						
							if (value == null || value is System.Int32)
								this.NumSignaturesDone = (System.Int32?)value;
							break;
						case "LastCheckedDateTime":
						
							if (value == null || value is System.DateTime)
								this.LastCheckedDateTime = (System.DateTime?)value;
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
		/// Maps to TilakaQuickSignSigner.QuickSignSignerID
		/// </summary>
		virtual public System.Int64? QuickSignSignerID
		{
			get
			{
				return base.GetSystemInt64(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignSignerID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignSignerID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.QuickSignRequestID
		/// </summary>
		virtual public System.Int64? QuickSignRequestID
		{
			get
			{
				return base.GetSystemInt64(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignRequestID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignRequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.Sequence
		/// </summary>
		virtual public System.Int32? Sequence
		{
			get
			{
				return base.GetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.Sequence);
			}
			
			set
			{
				base.SetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.Sequence, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.UserIdentifier
		/// </summary>
		virtual public System.String UserIdentifier
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.UserIdentifier);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.UserIdentifier, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.Email
		/// </summary>
		virtual public System.String Email
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.Email);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.Email, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.Status
		/// </summary>
		virtual public System.String Status
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.Status);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.Status, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.NumSignatures
		/// </summary>
		virtual public System.Int32? NumSignatures
		{
			get
			{
				return base.GetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.NumSignatures);
			}
			
			set
			{
				base.SetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.NumSignatures, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.NumSignaturesDone
		/// </summary>
		virtual public System.Int32? NumSignaturesDone
		{
			get
			{
				return base.GetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.NumSignaturesDone);
			}
			
			set
			{
				base.SetSystemInt32(TilakaQuickSignSignerMetadata.ColumnNames.NumSignaturesDone, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.AuthUrl
		/// </summary>
		virtual public System.String AuthUrl
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.AuthUrl);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.AuthUrl, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.TokenKey
		/// </summary>
		virtual public System.String TokenKey
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.TokenKey);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.TokenKey, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.SignedUrl
		/// </summary>
		virtual public System.String SignedUrl
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.SignedUrl);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.SignedUrl, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.ErrorMessage
		/// </summary>
		virtual public System.String ErrorMessage
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.ErrorMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.ErrorMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.LastCheckedDateTime
		/// </summary>
		virtual public System.DateTime? LastCheckedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.LastCheckedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.LastCheckedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaQuickSignSigner.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateByUserID, value);
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
			public esStrings(esTilakaQuickSignSigner entity)
			{
				this.entity = entity;
			}
			public System.String QuickSignSignerID
			{
				get
				{
					System.Int64? data = entity.QuickSignSignerID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QuickSignSignerID = null;
					else entity.QuickSignSignerID = Convert.ToInt64(value);
				}
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
			public System.String Sequence
			{
				get
				{
					System.Int32? data = entity.Sequence;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Sequence = null;
					else entity.Sequence = Convert.ToInt32(value);
				}
			}
			public System.String UserIdentifier
			{
				get
				{
					System.String data = entity.UserIdentifier;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.UserIdentifier = null;
					else entity.UserIdentifier = Convert.ToString(value);
				}
			}
			public System.String Email
			{
				get
				{
					System.String data = entity.Email;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Email = null;
					else entity.Email = Convert.ToString(value);
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
			public System.String NumSignatures
			{
				get
				{
					System.Int32? data = entity.NumSignatures;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.NumSignatures = null;
					else entity.NumSignatures = Convert.ToInt32(value);
				}
			}
			public System.String NumSignaturesDone
			{
				get
				{
					System.Int32? data = entity.NumSignaturesDone;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.NumSignaturesDone = null;
					else entity.NumSignaturesDone = Convert.ToInt32(value);
				}
			}
			public System.String AuthUrl
			{
				get
				{
					System.String data = entity.AuthUrl;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AuthUrl = null;
					else entity.AuthUrl = Convert.ToString(value);
				}
			}
			public System.String TokenKey
			{
				get
				{
					System.String data = entity.TokenKey;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TokenKey = null;
					else entity.TokenKey = Convert.ToString(value);
				}
			}
			public System.String SignedUrl
			{
				get
				{
					System.String data = entity.SignedUrl;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SignedUrl = null;
					else entity.SignedUrl = Convert.ToString(value);
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
			public System.String LastCheckedDateTime
			{
				get
				{
					System.DateTime? data = entity.LastCheckedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.LastCheckedDateTime = null;
					else entity.LastCheckedDateTime = Convert.ToDateTime(value);
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
			
			private esTilakaQuickSignSigner entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esTilakaQuickSignSignerQuery query)
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
				throw new Exception("esTilakaQuickSignSigner can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class TilakaQuickSignSigner : esTilakaQuickSignSigner
	{	
	}

	[Serializable]
	abstract public class esTilakaQuickSignSignerQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return TilakaQuickSignSignerMetadata.Meta();
			}
		}	
			
		public esQueryItem QuickSignSignerID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.QuickSignSignerID, esSystemType.Int64);
			}
		} 
		public esQueryItem QuickSignRequestID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.QuickSignRequestID, esSystemType.Int64);
			}
		} 
		public esQueryItem Sequence
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.Sequence, esSystemType.Int32);
			}
		} 
		public esQueryItem UserIdentifier
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.UserIdentifier, esSystemType.String);
			}
		} 
		public esQueryItem Email
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.Email, esSystemType.String);
			}
		} 
		public esQueryItem Status
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.Status, esSystemType.String);
			}
		} 
		public esQueryItem NumSignatures
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.NumSignatures, esSystemType.Int32);
			}
		} 
		public esQueryItem NumSignaturesDone
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.NumSignaturesDone, esSystemType.Int32);
			}
		} 
		public esQueryItem AuthUrl
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.AuthUrl, esSystemType.String);
			}
		} 
		public esQueryItem TokenKey
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.TokenKey, esSystemType.String);
			}
		} 
		public esQueryItem SignedUrl
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.SignedUrl, esSystemType.String);
			}
		} 
		public esQueryItem ErrorMessage
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.ErrorMessage, esSystemType.String);
			}
		} 
		public esQueryItem LastCheckedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.LastCheckedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("TilakaQuickSignSignerCollection")]
	public partial class TilakaQuickSignSignerCollection : esTilakaQuickSignSignerCollection, IEnumerable< TilakaQuickSignSigner>
	{
		public TilakaQuickSignSignerCollection()
		{

		}	
		
		public static implicit operator List< TilakaQuickSignSigner>(TilakaQuickSignSignerCollection coll)
		{
			List< TilakaQuickSignSigner> list = new List< TilakaQuickSignSigner>();
			
			foreach (TilakaQuickSignSigner emp in coll)
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
				return  TilakaQuickSignSignerMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaQuickSignSignerQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new TilakaQuickSignSigner(row);
		}

		override protected esEntity CreateEntity()
		{
			return new TilakaQuickSignSigner();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public TilakaQuickSignSignerQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaQuickSignSignerQuery();
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
		public bool Load(TilakaQuickSignSignerQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public TilakaQuickSignSigner AddNew()
		{
			TilakaQuickSignSigner entity = base.AddNewEntity() as TilakaQuickSignSigner;
			
			return entity;		
		}
		public TilakaQuickSignSigner FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as TilakaQuickSignSigner;
		}

		#region IEnumerable< TilakaQuickSignSigner> Members

		IEnumerator< TilakaQuickSignSigner> IEnumerable< TilakaQuickSignSigner>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as TilakaQuickSignSigner;
			}
		}

		#endregion
		
		private TilakaQuickSignSignerQuery query;
	}


	/// <summary>
	/// Encapsulates the 'TilakaQuickSignSigner' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("TilakaQuickSignSigner ({StandardReferenceID})")]
	[Serializable]
	public partial class TilakaQuickSignSigner : esTilakaQuickSignSigner
	{
		public TilakaQuickSignSigner()
		{
		}	
	
		public TilakaQuickSignSigner(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return TilakaQuickSignSignerMetadata.Meta();
			}
		}	
	
		override protected esTilakaQuickSignSignerQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaQuickSignSignerQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public TilakaQuickSignSignerQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaQuickSignSignerQuery();
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
		public bool Load(TilakaQuickSignSignerQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private TilakaQuickSignSignerQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class TilakaQuickSignSignerQuery : esTilakaQuickSignSignerQuery
	{
		public TilakaQuickSignSignerQuery()
		{

		}		
		
		public TilakaQuickSignSignerQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "TilakaQuickSignSignerQuery";
        }
	}

	[Serializable]
	public partial class TilakaQuickSignSignerMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected TilakaQuickSignSignerMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignSignerID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.QuickSignSignerID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.QuickSignRequestID, 1, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.QuickSignRequestID;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.Sequence, 2, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.Sequence;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.UserIdentifier, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.UserIdentifier;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.Email, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.Email;
			c.CharacterMaxLength = 200;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.Status, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.Status;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.NumSignatures, 6, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.NumSignatures;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.NumSignaturesDone, 7, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.NumSignaturesDone;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.AuthUrl, 8, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.AuthUrl;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.TokenKey, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.TokenKey;
			c.CharacterMaxLength = 200;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.SignedUrl, 10, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.SignedUrl;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.ErrorMessage, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.ErrorMessage;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.LastCheckedDateTime, 12, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.LastCheckedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.CreatedDateTime, 13, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.CreatedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.CreatedByUserID, 14, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateDateTime, 15, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaQuickSignSignerMetadata.ColumnNames.LastUpdateByUserID, 16, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaQuickSignSignerMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public TilakaQuickSignSignerMetadata Meta()
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
			public const string QuickSignSignerID = "QuickSignSignerID";
			public const string QuickSignRequestID = "QuickSignRequestID";
			public const string Sequence = "Sequence";
			public const string UserIdentifier = "UserIdentifier";
			public const string Email = "Email";
			public const string Status = "Status";
			public const string NumSignatures = "NumSignatures";
			public const string NumSignaturesDone = "NumSignaturesDone";
			public const string AuthUrl = "AuthUrl";
			public const string TokenKey = "TokenKey";
			public const string SignedUrl = "SignedUrl";
			public const string ErrorMessage = "ErrorMessage";
			public const string LastCheckedDateTime = "LastCheckedDateTime";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string QuickSignSignerID = "QuickSignSignerID";
			public const string QuickSignRequestID = "QuickSignRequestID";
			public const string Sequence = "Sequence";
			public const string UserIdentifier = "UserIdentifier";
			public const string Email = "Email";
			public const string Status = "Status";
			public const string NumSignatures = "NumSignatures";
			public const string NumSignaturesDone = "NumSignaturesDone";
			public const string AuthUrl = "AuthUrl";
			public const string TokenKey = "TokenKey";
			public const string SignedUrl = "SignedUrl";
			public const string ErrorMessage = "ErrorMessage";
			public const string LastCheckedDateTime = "LastCheckedDateTime";
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
			lock (typeof(TilakaQuickSignSignerMetadata))
			{
				if(TilakaQuickSignSignerMetadata.mapDelegates == null)
				{
					TilakaQuickSignSignerMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (TilakaQuickSignSignerMetadata.meta == null)
				{
					TilakaQuickSignSignerMetadata.meta = new TilakaQuickSignSignerMetadata();
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
				
				meta.AddTypeMap("QuickSignSignerID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("QuickSignRequestID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("Sequence", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("UserIdentifier", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Email", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Status", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("NumSignatures", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("NumSignaturesDone", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("AuthUrl", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TokenKey", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("SignedUrl", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ErrorMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("LastCheckedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "TilakaQuickSignSigner";
				meta.Destination = "TilakaQuickSignSigner";
				meta.spInsert = "proc_TilakaQuickSignSignerInsert";				
				meta.spUpdate = "proc_TilakaQuickSignSignerUpdate";		
				meta.spDelete = "proc_TilakaQuickSignSignerDelete";
				meta.spLoadAll = "proc_TilakaQuickSignSignerLoadAll";
				meta.spLoadByPrimaryKey = "proc_TilakaQuickSignSignerLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private TilakaQuickSignSignerMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		