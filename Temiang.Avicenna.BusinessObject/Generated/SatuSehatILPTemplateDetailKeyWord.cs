/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 21/05/2025 16:04:31
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
	abstract public class esSatuSehatILPTemplateDetailKeyWordCollection : esEntityCollectionWAuditLog
	{
		public esSatuSehatILPTemplateDetailKeyWordCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "SatuSehatILPTemplateDetailKeyWordCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateDetailKeyWordQuery query)
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
			this.InitQuery(query as esSatuSehatILPTemplateDetailKeyWordQuery);
		}
		#endregion
			
		virtual public SatuSehatILPTemplateDetailKeyWord DetachEntity(SatuSehatILPTemplateDetailKeyWord entity)
		{
			return base.DetachEntity(entity) as SatuSehatILPTemplateDetailKeyWord;
		}
		
		virtual public SatuSehatILPTemplateDetailKeyWord AttachEntity(SatuSehatILPTemplateDetailKeyWord entity)
		{
			return base.AttachEntity(entity) as SatuSehatILPTemplateDetailKeyWord;
		}
		
		virtual public void Combine(SatuSehatILPTemplateDetailKeyWordCollection collection)
		{
			base.Combine(collection);
		}
		
		new public SatuSehatILPTemplateDetailKeyWord this[int index]
		{
			get
			{
				return base[index] as SatuSehatILPTemplateDetailKeyWord;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(SatuSehatILPTemplateDetailKeyWord);
		}
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplateDetailKeyWord : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esSatuSehatILPTemplateDetailKeyWordQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esSatuSehatILPTemplateDetailKeyWord()
		{
		}
	
		public esSatuSehatILPTemplateDetailKeyWord(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int32 templateID, String testNo, Int32 sequence, String keyWord)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID, testNo, sequence, keyWord);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID, testNo, sequence, keyWord);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int32 templateID, String testNo, Int32 sequence, String keyWord)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(templateID, testNo, sequence, keyWord);
			else
				return LoadByPrimaryKeyStoredProcedure(templateID, testNo, sequence, keyWord);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int32 templateID, String testNo, Int32 sequence, String keyWord)
		{
			esSatuSehatILPTemplateDetailKeyWordQuery query = this.GetDynamicQuery();
			query.Where(query.TemplateID==templateID, query.TestNo==testNo, query.Sequence==sequence, query.KeyWord==keyWord);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int32 templateID, String testNo, Int32 sequence, String keyWord)
		{
			esParameters parms = new esParameters();
			parms.Add("TemplateID",templateID);
			parms.Add("TestNo",testNo);
			parms.Add("Sequence",sequence);
			parms.Add("KeyWord",keyWord);
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
						case "KeyWord": this.str.KeyWord = (string)value; break;
						case "IsQuestionAnswer": this.str.IsQuestionAnswer = (string)value; break;
						case "IsMultipleAnswers": this.str.IsMultipleAnswers = (string)value; break;
						case "SourceType": this.str.SourceType = (string)value; break;
						case "Source": this.str.Source = (string)value; break;
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
						case "IsQuestionAnswer":
						
							if (value == null || value is System.Boolean)
								this.IsQuestionAnswer = (System.Boolean?)value;
							break;
						case "IsMultipleAnswers":
						
							if (value == null || value is System.Boolean)
								this.IsMultipleAnswers = (System.Boolean?)value;
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
		/// Maps to SatuSehatILPTemplateDetailKeyWord.TemplateID
		/// </summary>
		virtual public System.Int32? TemplateID
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TemplateID);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TemplateID, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.TestNo
		/// </summary>
		virtual public System.String TestNo
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TestNo);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TestNo, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.Sequence
		/// </summary>
		virtual public System.Int32? Sequence
		{
			get
			{
				return base.GetSystemInt32(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Sequence);
			}
			
			set
			{
				base.SetSystemInt32(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Sequence, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.KeyWord
		/// </summary>
		virtual public System.String KeyWord
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.KeyWord);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.KeyWord, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.IsQuestionAnswer
		/// </summary>
		virtual public System.Boolean? IsQuestionAnswer
		{
			get
			{
				return base.GetSystemBoolean(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsQuestionAnswer);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsQuestionAnswer, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.SourceType
		/// </summary>
		virtual public System.String SourceType
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.SourceType);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.SourceType, value);
			}
		}
		/// <summary>
		/// Maps to SatuSehatILPTemplateDetailKeyWord.Source
		/// </summary>
		virtual public System.String Source
		{
			get
			{
				return base.GetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Source);
			}
			
			set
			{
				base.SetSystemString(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Source, value);
			}
		}
        /// <summary>
        /// Maps to SatuSehatILPTemplateDetailKeyWord.IsMultipleAnswers
        /// </summary>
        virtual public System.Boolean? IsMultipleAnswers
        {
			get
			{
				return base.GetSystemBoolean(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsMultipleAnswers);
			}
			
			set
			{
				base.SetSystemBoolean(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsMultipleAnswers, value);
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
			public esStrings(esSatuSehatILPTemplateDetailKeyWord entity)
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
			public System.String KeyWord
			{
				get
				{
					System.String data = entity.KeyWord;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KeyWord = null;
					else entity.KeyWord = Convert.ToString(value);
				}
			}
			public System.String IsQuestionAnswer
			{
				get
				{
					System.Boolean? data = entity.IsQuestionAnswer;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsQuestionAnswer = null;
					else entity.IsQuestionAnswer = Convert.ToBoolean(value);
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
			public System.String Source
			{
				get
				{
					System.String data = entity.Source;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.Source = null;
					else entity.Source = Convert.ToString(value);
				}
			}
			public System.String IsMultipleAnswers
            {
				get
				{
					System.Boolean? data = entity.IsMultipleAnswers;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsMultipleAnswers = null;
					else entity.IsMultipleAnswers = Convert.ToBoolean(value);
				}
			}
			private esSatuSehatILPTemplateDetailKeyWord entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esSatuSehatILPTemplateDetailKeyWordQuery query)
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
				throw new Exception("esSatuSehatILPTemplateDetailKeyWord can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class SatuSehatILPTemplateDetailKeyWord : esSatuSehatILPTemplateDetailKeyWord
	{	
	}

	[Serializable]
	abstract public class esSatuSehatILPTemplateDetailKeyWordQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateDetailKeyWordMetadata.Meta();
			}
		}	
			
		public esQueryItem TemplateID
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TemplateID, esSystemType.Int32);
			}
		} 
			
		public esQueryItem TestNo
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TestNo, esSystemType.String);
			}
		} 
			
		public esQueryItem Sequence
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Sequence, esSystemType.Int32);
			}
		} 
			
		public esQueryItem KeyWord
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.KeyWord, esSystemType.String);
			}
		} 
			
		public esQueryItem IsQuestionAnswer
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsQuestionAnswer, esSystemType.Boolean);
			}
		} 

			
		public esQueryItem SourceType
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.SourceType, esSystemType.String);
			}
		} 
			
		public esQueryItem Source
		{
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Source, esSystemType.String);
			}
		} 
		public esQueryItem IsMultipleAnswers
        {
			get
			{
				return new esQueryItem(this, SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsMultipleAnswers, esSystemType.Boolean);
			}
		} 
	
	}

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("SatuSehatILPTemplateDetailKeyWordCollection")]
	public partial class SatuSehatILPTemplateDetailKeyWordCollection : esSatuSehatILPTemplateDetailKeyWordCollection, IEnumerable< SatuSehatILPTemplateDetailKeyWord>
	{
		public SatuSehatILPTemplateDetailKeyWordCollection()
		{

		}	
		
		public static implicit operator List< SatuSehatILPTemplateDetailKeyWord>(SatuSehatILPTemplateDetailKeyWordCollection coll)
		{
			List< SatuSehatILPTemplateDetailKeyWord> list = new List< SatuSehatILPTemplateDetailKeyWord>();
			
			foreach (SatuSehatILPTemplateDetailKeyWord emp in coll)
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
				return  SatuSehatILPTemplateDetailKeyWordMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateDetailKeyWordQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new SatuSehatILPTemplateDetailKeyWord(row);
		}

		override protected esEntity CreateEntity()
		{
			return new SatuSehatILPTemplateDetailKeyWord();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateDetailKeyWordQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateDetailKeyWordQuery();
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
		public bool Load(SatuSehatILPTemplateDetailKeyWordQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public SatuSehatILPTemplateDetailKeyWord AddNew()
		{
			SatuSehatILPTemplateDetailKeyWord entity = base.AddNewEntity() as SatuSehatILPTemplateDetailKeyWord;
			
			return entity;		
		}
		public SatuSehatILPTemplateDetailKeyWord FindByPrimaryKey(Int32 templateID, String testNo, Int32 sequence, String keyWord)
		{
			return base.FindByPrimaryKey(templateID, testNo, sequence, keyWord) as SatuSehatILPTemplateDetailKeyWord;
		}

		#region IEnumerable< SatuSehatILPTemplateDetailKeyWord> Members

		IEnumerator< SatuSehatILPTemplateDetailKeyWord> IEnumerable< SatuSehatILPTemplateDetailKeyWord>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as SatuSehatILPTemplateDetailKeyWord;
			}
		}

		#endregion
		
		private SatuSehatILPTemplateDetailKeyWordQuery query;
	}


	/// <summary>
	/// Encapsulates the 'SatuSehatILPTemplateDetailKeyWord' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("SatuSehatILPTemplateDetailKeyWord ({TemplateID, TestNo, Sequence, KeyWord})")]
	[Serializable]
	public partial class SatuSehatILPTemplateDetailKeyWord : esSatuSehatILPTemplateDetailKeyWord
	{
		public SatuSehatILPTemplateDetailKeyWord()
		{
		}	
	
		public SatuSehatILPTemplateDetailKeyWord(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return SatuSehatILPTemplateDetailKeyWordMetadata.Meta();
			}
		}	
	
		override protected esSatuSehatILPTemplateDetailKeyWordQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new SatuSehatILPTemplateDetailKeyWordQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public SatuSehatILPTemplateDetailKeyWordQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new SatuSehatILPTemplateDetailKeyWordQuery();
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
		public bool Load(SatuSehatILPTemplateDetailKeyWordQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private SatuSehatILPTemplateDetailKeyWordQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class SatuSehatILPTemplateDetailKeyWordQuery : esSatuSehatILPTemplateDetailKeyWordQuery
	{
		public SatuSehatILPTemplateDetailKeyWordQuery()
		{

		}		
		
		public SatuSehatILPTemplateDetailKeyWordQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "SatuSehatILPTemplateDetailKeyWordQuery";
        }
	}

	[Serializable]
	public partial class SatuSehatILPTemplateDetailKeyWordMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected SatuSehatILPTemplateDetailKeyWordMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TemplateID, 0, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.TemplateID;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.TestNo, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.TestNo;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Sequence, 2, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.Sequence;
			c.IsInPrimaryKey = true;
			c.NumericPrecision = 10;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.KeyWord, 3, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.KeyWord;
			c.IsInPrimaryKey = true;
			c.CharacterMaxLength = 50;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsQuestionAnswer, 4, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.IsQuestionAnswer;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.SourceType, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.SourceType;
			c.CharacterMaxLength = 20;
			_columns.Add(c); 
				
			c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.Source, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.Source;
			c.CharacterMaxLength = 4000;
			_columns.Add(c);

            c = new esColumnMetadata(SatuSehatILPTemplateDetailKeyWordMetadata.ColumnNames.IsMultipleAnswers, 7, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = SatuSehatILPTemplateDetailKeyWordMetadata.PropertyNames.IsMultipleAnswers;
            _columns.Add(c);


        }
		#endregion
	
		static public SatuSehatILPTemplateDetailKeyWordMetadata Meta()
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
			public const string KeyWord = "KeyWord";
			public const string IsQuestionAnswer = "IsQuestionAnswer";
			public const string SourceType = "SourceType";
			public const string Source = "Source";
			public const string IsMultipleAnswers = "IsMultipleAnswers";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string TemplateID = "TemplateID";
			public const string TestNo = "TestNo";
			public const string Sequence = "Sequence";
			public const string KeyWord = "KeyWord";
			public const string IsQuestionAnswer = "IsQuestionAnswer";
			public const string SourceType = "SourceType";
			public const string Source = "Source";
			public const string IsMultipleAnswers = "IsMultipleAnswers";
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
			lock (typeof(SatuSehatILPTemplateDetailKeyWordMetadata))
			{
				if(SatuSehatILPTemplateDetailKeyWordMetadata.mapDelegates == null)
				{
					SatuSehatILPTemplateDetailKeyWordMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (SatuSehatILPTemplateDetailKeyWordMetadata.meta == null)
				{
					SatuSehatILPTemplateDetailKeyWordMetadata.meta = new SatuSehatILPTemplateDetailKeyWordMetadata();
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
				meta.AddTypeMap("KeyWord", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsQuestionAnswer", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("SourceType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("Source", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsMultipleAnswers", new esTypeMap("bit", "System.Boolean")); 
		

				meta.Source = "SatuSehatILPTemplateDetailKeyWord";
				meta.Destination = "SatuSehatILPTemplateDetailKeyWord";
				meta.spInsert = "proc_SatuSehatILPTemplateDetailKeyWordInsert";				
				meta.spUpdate = "proc_SatuSehatILPTemplateDetailKeyWordUpdate";		
				meta.spDelete = "proc_SatuSehatILPTemplateDetailKeyWordDelete";
				meta.spLoadAll = "proc_SatuSehatILPTemplateDetailKeyWordLoadAll";
				meta.spLoadByPrimaryKey = "proc_SatuSehatILPTemplateDetailKeyWordLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private SatuSehatILPTemplateDetailKeyWordMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		
