/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 03/06/2025 16:17:26
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
	abstract public class esSatuSehatILPTemplateDetailCollection : esEntityCollectionWAuditLog
	{
		public esSatuSehatILPTemplateDetailCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "SatuSehatILPTemplateDetailCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateDetailQuery query)
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
			this.InitQuery(query as esSatuSehatILPTemplateDetailQuery);
		}
		#endregion
			
		virtual public SatuSehatILPTemplateDetail DetachEntity(SatuSehatILPTemplateDetail entity)
		{
			return base.DetachEntity(entity) as SatuSehatILPTemplateDetail;
		}
		
		virtual public SatuSehatILPTemplateDetail AttachEntity(SatuSehatILPTemplateDetail entity)
		{
			return base.AttachEntity(entity) as SatuSehatILPTemplateDetail;
		}
		
		virtual public void Combine(SatuSehatILPTemplateDetailCollection collection)
		{
			base.Combine(collection);
		}
		
		new public SatuSehatILPTemplateDetail this[int index]
		{
			get
			{
				return base[index] as SatuSehatILPTemplateDetail;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(SatuSehatILPTemplateDetail);
		}
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplateDetail : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esSatuSehatILPTemplateDetailQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esSatuSehatILPTemplateDetail()
		{
		}
	
		public esSatuSehatILPTemplateDetail(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int32 templateID, String testNo, Int32 sequence)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID, testNo, sequence);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID, testNo, sequence);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 templateID, String testNo, Int32 sequence)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID, testNo, sequence);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID, testNo, sequence);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int32 templateID, String testNo, Int32 sequence)
		{
			esSatuSehatILPTemplateDetailQuery query = this.GetDynamicQuery();
			query.Where(query.TemplateID==templateID, query.TestNo==testNo, query.Sequence==sequence);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int32 templateID, String testNo, Int32 sequence)
		{
			esParameters parms = new esParameters();
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
						case "TemplateID": this.str.TemplateID = (string)value; break;
						case "TestNo": this.str.TestNo = (string)value; break;
						case "Sequence": this.str.Sequence = (string)value; break;
						case "IsOptional": this.str.IsOptional = (string)value; break;
						case "TaskDesc": this.str.TaskDesc = (string)value; break;
						case "SRAnswerType": this.str.SRAnswerType = (string)value; break;
						case "AnswerWidth": this.str.AnswerWidth = (string)value; break;
						case "AnswerDefault": this.str.AnswerDefault = (string)value; break;
						case "AnswerPrefix": this.str.AnswerPrefix = (string)value; break;
						case "AnswerSuffix": this.str.AnswerSuffix = (string)value; break;
						case "AnswerSelection": this.str.AnswerSelection = (string)value; break;
						case "AnswerSource": this.str.AnswerSource = (string)value; break;
						case "IsEditable": this.str.IsEditable = (string)value; break;
						case "PostUrl": this.str.PostUrl = (string)value; break;
						case "PostMethod": this.str.PostMethod = (string)value; break;
						case "PostJsonTemplate": this.str.PostJsonTemplate = (string)value; break;
						case "MultipleElements": this.str.MultipleElements = (string)value; break;
						case "JsonPathKeyword": this.str.JsonPathKeyword = (string)value; break;
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
						case "IsOptional":
						
							if (value == null || value is System.Boolean)
								this.IsOptional = (System.Boolean?)value;
							break;
						case "AnswerWidth":
						
							if (value == null || value is System.Int32)
								this.AnswerWidth = (System.Int32?)value;
							break;
						case "IsEditable":
						
							if (value == null || value is System.Boolean)
								this.IsEditable = (System.Boolean?)value;
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
		/// Maps to SatuSehatILPTemplateDetail.TemplateID
		/// </summary>
		virtual public System.Int32? TemplateID
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.TemplateID);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.TemplateID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.TestNo
		/// </summary>
		virtual public System.String TestNo
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.TestNo);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.TestNo, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.Sequence
		/// </summary>
		virtual public System.Int32? Sequence
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.Sequence);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.Sequence, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.IsOptional
		/// </summary>
		virtual public System.Boolean? IsOptional
		{
			get
			{
				return base.GetSystemBoolean(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsOptional);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsOptional, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.TaskDesc
		/// </summary>
		virtual public System.String TaskDesc
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.TaskDesc);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.TaskDesc, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.SRAnswerType
		/// </summary>
		virtual public System.String SRAnswerType
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.SRAnswerType);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.SRAnswerType, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerWidth
		/// </summary>
		virtual public System.Int32? AnswerWidth
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerWidth);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerWidth, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerDefault
		/// </summary>
		virtual public System.String AnswerDefault
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerDefault);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerDefault, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerPrefix
		/// </summary>
		virtual public System.String AnswerPrefix
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerPrefix);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerPrefix, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerSuffix
		/// </summary>
		virtual public System.String AnswerSuffix
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSuffix);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSuffix, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerSelection
		/// </summary>
		virtual public System.String AnswerSelection
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSelection);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSelection, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.AnswerSource
		/// </summary>
		virtual public System.String AnswerSource
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSource);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSource, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.IsEditable
		/// </summary>
		virtual public System.Boolean? IsEditable
		{
			get
			{
				return base.GetSystemBoolean(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsEditable);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsEditable, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.PostUrl
		/// </summary>
		virtual public System.String PostUrl
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostUrl);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostUrl, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.PostMethod
		/// </summary>
		virtual public System.String PostMethod
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostMethod);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostMethod, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetail.PostJsonTemplate
		/// </summary>
		virtual public System.String PostJsonTemplate
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostJsonTemplate);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostJsonTemplate, value);
			}
		}
        /// <summary>
        /// Maps to SatuSehatILPTemplateDetail.MultipleElements
        /// </summary>
        virtual public System.String MultipleElements
        {
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.MultipleElements);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.MultipleElements, value);
			}
		}
        /// <summary>
        /// Maps to SatuSehatILPTemplateDetail.JsonPathKeyword
        /// </summary>
        virtual public System.String JsonPathKeyword
        {
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.JsonPathKeyword);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailMetadata.ColumnNames.JsonPathKeyword, value);
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
			public esStrings(esSatuSehatILPTemplateDetail entity)
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
			public System.String IsOptional
			{
				get
				{
					System.Boolean? data = entity.IsOptional;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsOptional = null;
					else entity.IsOptional = Convert.ToBoolean(value);
				}
			}
			public System.String TaskDesc
			{
				get
				{
					System.String data = entity.TaskDesc;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TaskDesc = null;
					else entity.TaskDesc = Convert.ToString(value);
				}
			}
			public System.String SRAnswerType
			{
				get
				{
					System.String data = entity.SRAnswerType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.SRAnswerType = null;
					else entity.SRAnswerType = Convert.ToString(value);
				}
			}
			public System.String AnswerWidth
			{
				get
				{
					System.Int32? data = entity.AnswerWidth;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerWidth = null;
					else entity.AnswerWidth = Convert.ToInt32(value);
				}
			}
			public System.String AnswerDefault
			{
				get
				{
					System.String data = entity.AnswerDefault;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerDefault = null;
					else entity.AnswerDefault = Convert.ToString(value);
				}
			}
			public System.String AnswerPrefix
			{
				get
				{
					System.String data = entity.AnswerPrefix;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerPrefix = null;
					else entity.AnswerPrefix = Convert.ToString(value);
				}
			}
			public System.String AnswerSuffix
			{
				get
				{
					System.String data = entity.AnswerSuffix;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerSuffix = null;
					else entity.AnswerSuffix = Convert.ToString(value);
				}
			}
			public System.String AnswerSelection
			{
				get
				{
					System.String data = entity.AnswerSelection;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerSelection = null;
					else entity.AnswerSelection = Convert.ToString(value);
				}
			}
			public System.String AnswerSource
			{
				get
				{
					System.String data = entity.AnswerSource;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.AnswerSource = null;
					else entity.AnswerSource = Convert.ToString(value);
				}
			}
			public System.String IsEditable
			{
				get
				{
					System.Boolean? data = entity.IsEditable;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsEditable = null;
					else entity.IsEditable = Convert.ToBoolean(value);
				}
			}
			public System.String PostUrl
			{
				get
				{
					System.String data = entity.PostUrl;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.PostUrl = null;
					else entity.PostUrl = Convert.ToString(value);
				}
			}
			public System.String PostMethod
			{
				get
				{
					System.String data = entity.PostMethod;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.PostMethod = null;
					else entity.PostMethod = Convert.ToString(value);
				}
			}
			public System.String PostJsonTemplate
			{
				get
				{
					System.String data = entity.PostJsonTemplate;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.PostJsonTemplate = null;
					else entity.PostJsonTemplate = Convert.ToString(value);
				}
			}

			public System.String MultipleElements
            {
				get
				{
					System.String data = entity.MultipleElements;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.MultipleElements = null;
					else entity.MultipleElements = Convert.ToString(value);
				}
			}

			public System.String JsonPathKeyword
            {
				get
				{
					System.String data = entity.JsonPathKeyword;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.JsonPathKeyword = null;
					else entity.JsonPathKeyword = Convert.ToString(value);
				}
			}
			private esSatuSehatILPTemplateDetail entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateDetailQuery query)
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
				throw new Exception("esSatuSehatILPTemplateDetail can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class SatuSehatILPTemplateDetail : esSatuSehatILPTemplateDetail
	{	
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplateDetailQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateDetailMetadata.Meta();
			}
		}	
			
		public esQueryItem TemplateID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.TemplateID, esSystemType.Int32);
			}
		} 
			
		public esQueryItem TestNo
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.TestNo, esSystemType.String);
			}
		} 
			
		public esQueryItem Sequence
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.Sequence, esSystemType.Int32);
			}
		} 
			
		public esQueryItem IsOptional
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.IsOptional, esSystemType.Boolean);
			}
		} 
			
		public esQueryItem TaskDesc
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.TaskDesc, esSystemType.String);
			}
		} 
			
		public esQueryItem SRAnswerType
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.SRAnswerType, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerWidth
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerWidth, esSystemType.Int32);
			}
		} 
			
		public esQueryItem AnswerDefault
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerDefault, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerPrefix
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerPrefix, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerSuffix
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSuffix, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerSelection
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSelection, esSystemType.String);
			}
		} 
			
		public esQueryItem AnswerSource
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSource, esSystemType.String);
			}
		} 
			
		public esQueryItem IsEditable
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.IsEditable, esSystemType.Boolean);
			}
		} 
			
		public esQueryItem PostUrl
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.PostUrl, esSystemType.String);
			}
		} 
			
		public esQueryItem PostMethod
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.PostMethod, esSystemType.String);
			}
		} 
			
		public esQueryItem PostJsonTemplate
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.PostJsonTemplate, esSystemType.String);
			}
		} 

		public esQueryItem MultipleElements
        {
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.MultipleElements, esSystemType.String);
			}
		}

        public esQueryItem JsonPathKeyword
        {
            get
            {
                return new esQueryItem(this, SatuSehatILPTemplateDetailMetadata.ColumnNames.JsonPathKeyword, esSystemType.String);
            }
        }

    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("SatuSehatILPTemplateDetailCollection")]
	public partial class SatuSehatILPTemplateDetailCollection : esSatuSehatILPTemplateDetailCollection, IEnumerable< SatuSehatILPTemplateDetail>
	{
		public SatuSehatILPTemplateDetailCollection()
		{

		}	
		
		public static implicit operator List< SatuSehatILPTemplateDetail>(SatuSehatILPTemplateDetailCollection coll)
		{
			List< SatuSehatILPTemplateDetail> list = new List< SatuSehatILPTemplateDetail>();
			
			foreach (SatuSehatILPTemplateDetail emp in coll)
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
				return  SatuSehatILPTemplateDetailMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateDetailQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new SatuSehatILPTemplateDetail(row);
		}

		override protected esEntity CreateEntity()
		{
			return new SatuSehatILPTemplateDetail();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateDetailQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateDetailQuery();
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
		public bool Load(SatuSehatILPTemplateDetailQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public SatuSehatILPTemplateDetail AddNew()
		{
			SatuSehatILPTemplateDetail entity = base.AddNewEntity() as SatuSehatILPTemplateDetail;
			
			return entity;		
		}
		public SatuSehatILPTemplateDetail FindByPrimaryKey(Int32 templateID, String testNo, Int32 sequence)
		{
			return base.FindByPrimaryKey(templateID, testNo, sequence) as SatuSehatILPTemplateDetail;
		}

		#region IEnumerable< SatuSehatILPTemplateDetail> Members

		IEnumerator< SatuSehatILPTemplateDetail> IEnumerable< SatuSehatILPTemplateDetail>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as SatuSehatILPTemplateDetail;
			}
		}

		#endregion
		
		private SatuSehatILPTemplateDetailQuery query;
	}


	/// <summary>
	/// Encapsulates the 'SatuSehatILPTemplateDetail' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("SatuSehatILPTemplateDetail ({TemplateID, TestNo, Sequence})")]
	[Serializable]
	public partial class SatuSehatILPTemplateDetail : esSatuSehatILPTemplateDetail
	{
		public SatuSehatILPTemplateDetail()
		{
		}	
	
		public SatuSehatILPTemplateDetail(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateDetailMetadata.Meta();
			}
		}	
	
		override protected esSatuSehatILPTemplateDetailQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateDetailQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateDetailQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateDetailQuery();
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
		public bool Load(SatuSehatILPTemplateDetailQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private SatuSehatILPTemplateDetailQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class SatuSehatILPTemplateDetailQuery : esSatuSehatILPTemplateDetailQuery
	{
		public SatuSehatILPTemplateDetailQuery()
		{

		}		
		
		public SatuSehatILPTemplateDetailQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "SatuSehatILPTemplateDetailQuery";
        }
	}

	[Serializable]
	public partial class SatuSehatILPTemplateDetailMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected SatuSehatILPTemplateDetailMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.TemplateID, 0, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.TemplateID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.TestNo, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.TestNo;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.Sequence, 2, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.Sequence;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsOptional, 3, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.IsOptional;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.TaskDesc, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.TaskDesc;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.SRAnswerType, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.SRAnswerType;
			c.CharacterMaxLength = 3;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerWidth, 6, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerWidth;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerDefault, 7, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerDefault;
			c.CharacterMaxLength = 100;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerPrefix, 8, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerPrefix;
			c.CharacterMaxLength = 15;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSuffix, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerSuffix;
			c.CharacterMaxLength = 60;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSelection, 10, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerSelection;
			c.CharacterMaxLength = 8000;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.AnswerSource, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.AnswerSource;
			c.CharacterMaxLength = 8000;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.IsEditable, 12, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.IsEditable;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostUrl, 13, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.PostUrl;
			c.CharacterMaxLength = 500;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostMethod, 14, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.PostMethod;
			c.CharacterMaxLength = 8;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.PostJsonTemplate, 15, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.PostJsonTemplate;
			c.CharacterMaxLength = 2147483647;
			_columns.Add(c); 

			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.MultipleElements, 16, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.MultipleElements;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 

			c = new esColumnMetadata(SatuSehatILPTemplateDetailMetadata.ColumnNames.JsonPathKeyword, 17, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailMetadata.PropertyNames.JsonPathKeyword;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				

		}
		#endregion
	
		static public SatuSehatILPTemplateDetailMetadata Meta()
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
			public const string TestNo = "TestNo";
			public const string Sequence = "Sequence";
			public const string IsOptional = "IsOptional";
			public const string TaskDesc = "TaskDesc";
			public const string SRAnswerType = "SRAnswerType";
			public const string AnswerWidth = "AnswerWidth";
			public const string AnswerDefault = "AnswerDefault";
			public const string AnswerPrefix = "AnswerPrefix";
			public const string AnswerSuffix = "AnswerSuffix";
			public const string AnswerSelection = "AnswerSelection";
			public const string AnswerSource = "AnswerSource";
			public const string IsEditable = "IsEditable";
			public const string PostUrl = "PostUrl";
			public const string PostMethod = "PostMethod";
			public const string PostJsonTemplate = "PostJsonTemplate";
			public const string MultipleElements = "MultipleElements";
			public const string JsonPathKeyword = "JsonPathKeyword";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string TemplateID = "TemplateID";
			public const string TestNo = "TestNo";
			public const string Sequence = "Sequence";
			public const string IsOptional = "IsOptional";
			public const string TaskDesc = "TaskDesc";
			public const string SRAnswerType = "SRAnswerType";
			public const string AnswerWidth = "AnswerWidth";
			public const string AnswerDefault = "AnswerDefault";
			public const string AnswerPrefix = "AnswerPrefix";
			public const string AnswerSuffix = "AnswerSuffix";
			public const string AnswerSelection = "AnswerSelection";
			public const string AnswerSource = "AnswerSource";
			public const string IsEditable = "IsEditable";
			public const string PostUrl = "PostUrl";
			public const string PostMethod = "PostMethod";
			public const string PostJsonTemplate = "PostJsonTemplate";
			public const string MultipleElements = "MultipleElements";
			public const string JsonPathKeyword = "JsonPathKeyword";
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
			lock (typeof(SatuSehatILPTemplateDetailMetadata))
			{
				if(SatuSehatILPTemplateDetailMetadata.mapDelegates == null)
				{
					SatuSehatILPTemplateDetailMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (SatuSehatILPTemplateDetailMetadata.meta == null)
				{
					SatuSehatILPTemplateDetailMetadata.meta = new SatuSehatILPTemplateDetailMetadata();
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
				meta.AddTypeMap("TestNo", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Sequence", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("IsOptional", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("TaskDesc", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("SRAnswerType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerWidth", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("AnswerDefault", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerPrefix", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerSuffix", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerSelection", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("AnswerSource", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsEditable", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("PostUrl", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("PostMethod", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("PostJsonTemplate", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("MultipleElements", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("JsonPathKeyword", new esTypeMap("varchar", "System.String"));
		

				meta.Source = "SatuSehatILPTemplateDetail";
				meta.Destination = "SatuSehatILPTemplateDetail";
				meta.spInsert = "proc_SatuSehatILPTemplateDetailInsert";				
				meta.spUpdate = "proc_SatuSehatILPTemplateDetailUpdate";		
				meta.spDelete = "proc_SatuSehatILPTemplateDetailDelete";
				meta.spLoadAll = "proc_SatuSehatILPTemplateDetailLoadAll";
				meta.spLoadByPrimaryKey = "proc_SatuSehatILPTemplateDetailLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private SatuSehatILPTemplateDetailMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
