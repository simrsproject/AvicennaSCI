/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 13/06/2025 10:03:35
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
	abstract public class esSatuSehatILPPreparationCollection : esEntityCollectionWAuditLog
	{
		public esSatuSehatILPPreparationCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "SatuSehatILPPreparationCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esSatuSehatILPPreparationQuery query)
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
			this.InitQuery(query as esSatuSehatILPPreparationQuery);
		}
		#endregion
			
		virtual public SatuSehatILPPreparation DetachEntity(SatuSehatILPPreparation entity)
		{
			return base.DetachEntity(entity) as SatuSehatILPPreparation;
		}
		
		virtual public SatuSehatILPPreparation AttachEntity(SatuSehatILPPreparation entity)
		{
			return base.AttachEntity(entity) as SatuSehatILPPreparation;
		}
		
		virtual public void Combine(SatuSehatILPPreparationCollection collection)
		{
			base.Combine(collection);
		}
		
		new public SatuSehatILPPreparation this[int index]
		{
			get
			{
				return base[index] as SatuSehatILPPreparation;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(SatuSehatILPPreparation);
		}
	}

	[Serializable]
	abstract public class esSatuSehatILPPreparation : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esSatuSehatILPPreparationQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esSatuSehatILPPreparation()
		{
		}
	
		public esSatuSehatILPPreparation(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(String registrationNo, Int32 templateID, String testNo, Int32 sequence)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(registrationNo, templateID, testNo, sequence);
			else
				return LoadByPrimaryKeyStoredProcedure(registrationNo, templateID, testNo, sequence);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String registrationNo, Int32 templateID, String testNo, Int32 sequence)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(registrationNo, templateID, testNo, sequence);
			else
				return LoadByPrimaryKeyStoredProcedure(registrationNo, templateID, testNo, sequence);
		}
	
		private bool LoadByPrimaryKeyDynamic(String registrationNo, Int32 templateID, String testNo, Int32 sequence)
		{
			esSatuSehatILPPreparationQuery query = this.GetDynamicQuery();
			query.Where(query.RegistrationNo==registrationNo, query.TemplateID==templateID, query.TestNo==testNo, query.Sequence==sequence);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(String registrationNo, Int32 templateID, String testNo, Int32 sequence)
		{
			esParameters parms = new esParameters();
			parms.Add("RegistrationNo",registrationNo);
			parms.Add("TemplateID",templateID);
			parms.Add("TestNo",testNo);
			parms.Add("Sequence",sequence);
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
						case "RegistrationNo": this.str.RegistrationNo = (string)value; break;
						case "TemplateID": this.str.TemplateID = (string)value; break;
						case "TestNo": this.str.TestNo = (string)value; break;
						case "Sequence": this.str.Sequence = (string)value; break;
						case "AnswerValue": this.str.AnswerValue = (string)value; break;
						case "AnswerText": this.str.AnswerText = (string)value; break;
						case "PostData": this.str.PostData = (string)value; break;
						case "IsSent": this.str.IsSent = (string)value; break;
						case "IsError": this.str.IsError = (string)value; break;
						case "RespondData": this.str.RespondData = (string)value; break;
						case "SentDateTime": this.str.SentDateTime = (string)value; break;
						case "CreateByUserID": this.str.CreateByUserID = (string)value; break;
						case "CreateDateTime": this.str.CreateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
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
						case "Sequence":
						
							if (value == null || value is System.Int32)
								this.Sequence = (System.Int32?)value;
							break;
						case "IsSent":
						
							if (value == null || value is System.Boolean)
								this.IsSent = (System.Boolean?)value;
							break;
						case "IsError":
						
							if (value == null || value is System.Boolean)
								this.IsError = (System.Boolean?)value;
							break;
						case "SentDateTime":
						
							if (value == null || value is System.DateTime)
								this.SentDateTime = (System.DateTime?)value;
							break;
						case "CreateDateTime":
						
							if (value == null || value is System.DateTime)
								this.CreateDateTime = (System.DateTime?)value;
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
		/// Maps to SatuSehatILPPreparation.RegistrationNo
		/// </summary>
		virtual public System.String RegistrationNo
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.RegistrationNo);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.RegistrationNo, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.TemplateID
		/// </summary>
		virtual public System.Int32? TemplateID
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPPreparationMetadata.ColumnNames.TemplateID);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPPreparationMetadata.ColumnNames.TemplateID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.TestNo
		/// </summary>
		virtual public System.String TestNo
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.TestNo);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.TestNo, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.Sequence
		/// </summary>
		virtual public System.Int32? Sequence
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPPreparationMetadata.ColumnNames.Sequence);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPPreparationMetadata.ColumnNames.Sequence, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.AnswerValue
		/// </summary>
		virtual public System.String AnswerValue
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.AnswerValue);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.AnswerValue, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.AnswerText
		/// </summary>
		virtual public System.String AnswerText
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.AnswerText);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.AnswerText, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.PostData
		/// </summary>
		virtual public System.String PostData
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.PostData);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.PostData, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.IsSent
		/// </summary>
		virtual public System.Boolean? IsSent
		{
			get
			{
				return base.GetSystemBoolean(SatuSehatILPPreparationMetadata.ColumnNames.IsSent);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPPreparationMetadata.ColumnNames.IsSent, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.IsError
		/// </summary>
		virtual public System.Boolean? IsError
		{
			get
			{
				return base.GetSystemBoolean(SatuSehatILPPreparationMetadata.ColumnNames.IsError);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPPreparationMetadata.ColumnNames.IsError, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.RespondData
		/// </summary>
		virtual public System.String RespondData
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.RespondData);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.RespondData, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.SentDateTime
		/// </summary>
		virtual public System.DateTime? SentDateTime
		{
			get
			{
				return base.GetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.SentDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.SentDateTime, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.CreateByUserID
		/// </summary>
		virtual public System.String CreateByUserID
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.CreateByUserID);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.CreateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.CreateDateTime
		/// </summary>
		virtual public System.DateTime? CreateDateTime
		{
			get
			{
				return base.GetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.CreateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.CreateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPPreparation.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateDateTime, value);
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
			public esStrings(esSatuSehatILPPreparation entity)
			{
				this.entity = entity;
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
			public System.String TestNo
			{
				get
				{
					System.String data = entity.TestNo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TestNo = null;
					else entity.TestNo = Convert.ToString(value);
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
			public System.String AnswerValue
			{
				get
				{
					System.String data = entity.AnswerValue;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerValue = null;
					else entity.AnswerValue = Convert.ToString(value);
				}
			}
			public System.String AnswerText
			{
				get
				{
					System.String data = entity.AnswerText;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerText = null;
					else entity.AnswerText = Convert.ToString(value);
				}
			}
			public System.String PostData
			{
				get
				{
					System.String data = entity.PostData;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.PostData = null;
					else entity.PostData = Convert.ToString(value);
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
			public System.String IsError
			{
				get
				{
					System.Boolean? data = entity.IsError;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsError = null;
					else entity.IsError = Convert.ToBoolean(value);
				}
			}
			public System.String RespondData
			{
				get
				{
					System.String data = entity.RespondData;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RespondData = null;
					else entity.RespondData = Convert.ToString(value);
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
			public System.String CreateByUserID
			{
				get
				{
					System.String data = entity.CreateByUserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CreateByUserID = null;
					else entity.CreateByUserID = Convert.ToString(value);
				}
			}
			public System.String CreateDateTime
			{
				get
				{
					System.DateTime? data = entity.CreateDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CreateDateTime = null;
					else entity.CreateDateTime = Convert.ToDateTime(value);
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
			private esSatuSehatILPPreparation entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esSatuSehatILPPreparationQuery query)
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
				throw new Exception("esSatuSehatILPPreparation can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class SatuSehatILPPreparation : esSatuSehatILPPreparation
	{	
	}

	[Serializable]
	abstract public class esSatuSehatILPPreparationQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPPreparationMetadata.Meta();
			}
		}	
			
		public esQueryItem RegistrationNo
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.RegistrationNo, esSystemType.String);
			}
		} 
			
		public esQueryItem TemplateID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.TemplateID, esSystemType.Int32);
			}
		} 
			
		public esQueryItem TestNo
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.TestNo, esSystemType.String);
			}
		} 
			
		public esQueryItem Sequence
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.Sequence, esSystemType.Int32);
			}
		} 
			
		public esQueryItem AnswerValue
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.AnswerValue, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerText
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.AnswerText, esSystemType.String);
			}
		} 
			
		public esQueryItem PostData
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.PostData, esSystemType.String);
			}
		} 
			
		public esQueryItem IsSent
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.IsSent, esSystemType.Boolean);
			}
		} 
			
		public esQueryItem IsError
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.IsError, esSystemType.Boolean);
			}
		} 
			
		public esQueryItem RespondData
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.RespondData, esSystemType.String);
			}
		} 
			
		public esQueryItem SentDateTime
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.SentDateTime, esSystemType.DateTime);
			}
		} 
			
		public esQueryItem CreateByUserID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.CreateByUserID, esSystemType.String);
			}
		} 
			
		public esQueryItem CreateDateTime
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.CreateDateTime, esSystemType.DateTime);
			}
		} 
			
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
			
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
	
	}

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("SatuSehatILPPreparationCollection")]
	public partial class SatuSehatILPPreparationCollection : esSatuSehatILPPreparationCollection, IEnumerable< SatuSehatILPPreparation>
	{
		public SatuSehatILPPreparationCollection()
		{

		}	
		
		public static implicit operator List< SatuSehatILPPreparation>(SatuSehatILPPreparationCollection coll)
		{
			List< SatuSehatILPPreparation> list = new List< SatuSehatILPPreparation>();
			
			foreach (SatuSehatILPPreparation emp in coll)
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
				return  SatuSehatILPPreparationMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPPreparationQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new SatuSehatILPPreparation(row);
		}

		override protected esEntity CreateEntity()
		{
			return new SatuSehatILPPreparation();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public SatuSehatILPPreparationQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPPreparationQuery();
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
		public bool Load(SatuSehatILPPreparationQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public SatuSehatILPPreparation AddNew()
		{
			SatuSehatILPPreparation entity = base.AddNewEntity() as SatuSehatILPPreparation;
			
			return entity;		
		}
		public SatuSehatILPPreparation FindByPrimaryKey(String registrationNo, Int32 templateID, String testNo, Int32 sequence)
		{
			return base.FindByPrimaryKey(registrationNo, templateID, testNo, sequence) as SatuSehatILPPreparation;
		}

		#region IEnumerable< SatuSehatILPPreparation> Members

		IEnumerator< SatuSehatILPPreparation> IEnumerable< SatuSehatILPPreparation>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as SatuSehatILPPreparation;
			}
		}

		#endregion
		
		private SatuSehatILPPreparationQuery query;
	}


	/// <summary>
	/// Encapsulates the 'SatuSehatILPPreparation' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("SatuSehatILPPreparation ({RegistrationNo, TemplateID, TestNo, Sequence})")]
	[Serializable]
	public partial class SatuSehatILPPreparation : esSatuSehatILPPreparation
	{
		public SatuSehatILPPreparation()
		{
		}	
	
		public SatuSehatILPPreparation(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPPreparationMetadata.Meta();
			}
		}	
	
		override protected esSatuSehatILPPreparationQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPPreparationQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public SatuSehatILPPreparationQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPPreparationQuery();
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
		public bool Load(SatuSehatILPPreparationQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private SatuSehatILPPreparationQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class SatuSehatILPPreparationQuery : esSatuSehatILPPreparationQuery
	{
		public SatuSehatILPPreparationQuery()
		{

		}		
		
		public SatuSehatILPPreparationQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "SatuSehatILPPreparationQuery";
        }
	}

	[Serializable]
	public partial class SatuSehatILPPreparationMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected SatuSehatILPPreparationMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.RegistrationNo, 0, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.RegistrationNo;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.TemplateID, 1, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.TemplateID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.TestNo, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.TestNo;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.Sequence, 3, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.Sequence;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.AnswerValue, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.AnswerValue;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.AnswerText, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.AnswerText;
			c.CharacterMaxLength = 4000;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.PostData, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.PostData;
			c.CharacterMaxLength = 2147483647;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.IsSent, 7, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.IsSent;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.IsError, 8, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.IsError;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.RespondData, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.RespondData;
			c.CharacterMaxLength = 2147483647;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.SentDateTime, 10, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.SentDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.CreateByUserID, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.CreateByUserID;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.CreateDateTime, 12, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.CreateDateTime;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateByUserID, 13, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPPreparationMetadata.ColumnNames.LastUpdateDateTime, 14, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = SatuSehatILPPreparationMetadata.PropertyNames.LastUpdateDateTime;
			_columns.Add(c); 
				

		}
		#endregion
	
		static public SatuSehatILPPreparationMetadata Meta()
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
			public const string RegistrationNo = "RegistrationNo";
			public const string TemplateID = "TemplateID";
			public const string TestNo = "TestNo";
			public const string Sequence = "Sequence";
			public const string AnswerValue = "AnswerValue";
			public const string AnswerText = "AnswerText";
			public const string PostData = "PostData";
			public const string IsSent = "IsSent";
			public const string IsError = "IsError";
			public const string RespondData = "RespondData";
			public const string SentDateTime = "SentDateTime";
			public const string CreateByUserID = "CreateByUserID";
			public const string CreateDateTime = "CreateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string RegistrationNo = "RegistrationNo";
			public const string TemplateID = "TemplateID";
			public const string TestNo = "TestNo";
			public const string Sequence = "Sequence";
			public const string AnswerValue = "AnswerValue";
			public const string AnswerText = "AnswerText";
			public const string PostData = "PostData";
			public const string IsSent = "IsSent";
			public const string IsError = "IsError";
			public const string RespondData = "RespondData";
			public const string SentDateTime = "SentDateTime";
			public const string CreateByUserID = "CreateByUserID";
			public const string CreateDateTime = "CreateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
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
			lock (typeof(SatuSehatILPPreparationMetadata))
			{
				if(SatuSehatILPPreparationMetadata.mapDelegates == null)
				{
					SatuSehatILPPreparationMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (SatuSehatILPPreparationMetadata.meta == null)
				{
					SatuSehatILPPreparationMetadata.meta = new SatuSehatILPPreparationMetadata();
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
				
				meta.AddTypeMap("RegistrationNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TemplateID", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("TestNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Sequence", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("AnswerValue", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerText", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("PostData", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsSent", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("IsError", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("RespondData", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("SentDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("CreateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
		

				meta.Source = "SatuSehatILPPreparation";
				meta.Destination = "SatuSehatILPPreparation";
				meta.spInsert = "proc_SatuSehatILPPreparationInsert";				
				meta.spUpdate = "proc_SatuSehatILPPreparationUpdate";		
				meta.spDelete = "proc_SatuSehatILPPreparationDelete";
				meta.spLoadAll = "proc_SatuSehatILPPreparationLoadAll";
				meta.spLoadByPrimaryKey = "proc_SatuSehatILPPreparationLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private SatuSehatILPPreparationMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
