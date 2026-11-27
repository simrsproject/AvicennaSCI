/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-27 05:16:18 PM
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
	abstract public class esTilakaRegistrationCollection : esEntityCollectionWAuditLog
	{
		public esTilakaRegistrationCollection()
		{

		}
		
				
		protected override string GetCollectionName()
		{
			return "TilakaRegistrationCollection";
		}		
		
		#region Query Logic
		protected void InitQuery(esTilakaRegistrationQuery query)
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
			this.InitQuery(query as esTilakaRegistrationQuery);
		}
		#endregion
			
		virtual public TilakaRegistration DetachEntity(TilakaRegistration entity)
		{
			return base.DetachEntity(entity) as TilakaRegistration;
		}
		
		virtual public TilakaRegistration AttachEntity(TilakaRegistration entity)
		{
			return base.AttachEntity(entity) as TilakaRegistration;
		}
		
		virtual public void Combine(TilakaRegistrationCollection collection)
		{
			base.Combine(collection);
		}
		
		new public TilakaRegistration this[int index]
		{
			get
			{
				return base[index] as TilakaRegistration;
			}
		}

		public override Type GetEntityType()
		{
			return typeof(TilakaRegistration);
		}
	}

	[Serializable]
	abstract public class esTilakaRegistration : esEntityWAuditLog
	{
		/// <summary>
		/// Used internally by the entity's DynamicQuery mechanism.
		/// </summary>
		virtual protected esTilakaRegistrationQuery GetDynamicQuery()
		{
			return null;
		}
		
		public esTilakaRegistration()
		{
		}
	
		public esTilakaRegistration(DataRow row)
			: base(row)
		{
		}
		
				
		#region LoadByPrimaryKey
		public virtual bool LoadByPrimaryKey(Int64 tilakaRegistrationID)
		{
			if(this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(tilakaRegistrationID);
			else
				return LoadByPrimaryKeyStoredProcedure(tilakaRegistrationID);
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
		public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 tilakaRegistrationID)
		{
			if (sqlAccessType == esSqlAccessType.DynamicSQL)
				return LoadByPrimaryKeyDynamic(tilakaRegistrationID);
			else
				return LoadByPrimaryKeyStoredProcedure(tilakaRegistrationID);
		}
	
		private bool LoadByPrimaryKeyDynamic(Int64 tilakaRegistrationID)
		{
			esTilakaRegistrationQuery query = this.GetDynamicQuery();
			query.Where(query.TilakaRegistrationID == tilakaRegistrationID);
			return query.Load();
		}
	
		private bool LoadByPrimaryKeyStoredProcedure(Int64 tilakaRegistrationID)
		{
			esParameters parms = new esParameters();
			parms.Add("TilakaRegistrationID", tilakaRegistrationID);
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
						case "TilakaRegistrationID": this.str.TilakaRegistrationID = (string)value; break;
						case "RelatedType": this.str.RelatedType = (string)value; break;
						case "RelatedID": this.str.RelatedID = (string)value; break;
						case "IsLatest": this.str.IsLatest = (string)value; break;
						case "RegistrationState": this.str.RegistrationState = (string)value; break;
						case "RegistrationRequestID": this.str.RegistrationRequestID = (string)value; break;
						case "RequestType": this.str.RequestType = (string)value; break;
						case "RegistrationDateTime": this.str.RegistrationDateTime = (string)value; break;
						case "DateExpire": this.str.DateExpire = (string)value; break;
						case "RegisteredName": this.str.RegisteredName = (string)value; break;
						case "RegisteredNIK": this.str.RegisteredNIK = (string)value; break;
						case "RegisteredEmail": this.str.RegisteredEmail = (string)value; break;
						case "KycStatus": this.str.KycStatus = (string)value; break;
						case "KycReasonCode": this.str.KycReasonCode = (string)value; break;
						case "ManualRegistrationStatus": this.str.ManualRegistrationStatus = (string)value; break;
						case "TilakaName": this.str.TilakaName = (string)value; break;
						case "CertificateStatus": this.str.CertificateStatus = (string)value; break;
						case "CertificateStatusInfo": this.str.CertificateStatusInfo = (string)value; break;
						case "CertificateSerialNumber": this.str.CertificateSerialNumber = (string)value; break;
						case "CertificateStartDate": this.str.CertificateStartDate = (string)value; break;
						case "CertificateExpiryDate": this.str.CertificateExpiryDate = (string)value; break;
						case "ConsentVersion": this.str.ConsentVersion = (string)value; break;
						case "ConsentTimestamp": this.str.ConsentTimestamp = (string)value; break;
						case "ConsentApprovedByUserID": this.str.ConsentApprovedByUserID = (string)value; break;
						case "KtpFilePath": this.str.KtpFilePath = (string)value; break;
						case "KtpFileName": this.str.KtpFileName = (string)value; break;
						case "KtpContentType": this.str.KtpContentType = (string)value; break;
						case "KtpUploadedDateTime": this.str.KtpUploadedDateTime = (string)value; break;
						case "LastCheckedDateTime": this.str.LastCheckedDateTime = (string)value; break;
						case "LastCheckSource": this.str.LastCheckSource = (string)value; break;
						case "LastErrorMessage": this.str.LastErrorMessage = (string)value; break;
						case "CreatedDateTime": this.str.CreatedDateTime = (string)value; break;
						case "CreatedByUserID": this.str.CreatedByUserID = (string)value; break;
						case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
						case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
						case "IsQuickSignUser": this.str.IsQuickSignUser = (string)value; break;
						case "QuickSignRegisteredDateTime": this.str.QuickSignRegisteredDateTime = (string)value; break;
						case "QuickSignLastErrorMessage": this.str.QuickSignLastErrorMessage = (string)value; break;
						case "CertificateDataStatus": this.str.CertificateDataStatus = (string)value; break;
						case "RevokeRequestID": this.str.RevokeRequestID = (string)value; break;
						case "RevokeReason": this.str.RevokeReason = (string)value; break;
						case "RevokeNote": this.str.RevokeNote = (string)value; break;
						case "RevokeStatus": this.str.RevokeStatus = (string)value; break;
						case "RevokeMessage": this.str.RevokeMessage = (string)value; break;
						case "RevokeRequestedDateTime": this.str.RevokeRequestedDateTime = (string)value; break;
						case "RevokeRequestedByUserID": this.str.RevokeRequestedByUserID = (string)value; break;
						case "RevokeCompletedDateTime": this.str.RevokeCompletedDateTime = (string)value; break;
					}
				}
				else
				{
					switch (name)
					{	
						case "TilakaRegistrationID":
						
							if (value == null || value is System.Int64)
								this.TilakaRegistrationID = (System.Int64?)value;
							break;
						case "IsLatest":
						
							if (value == null || value is System.Boolean)
								this.IsLatest = (System.Boolean?)value;
							break;
						case "RegistrationDateTime":
						
							if (value == null || value is System.DateTime)
								this.RegistrationDateTime = (System.DateTime?)value;
							break;
						case "DateExpire":
						
							if (value == null || value is System.DateTime)
								this.DateExpire = (System.DateTime?)value;
							break;
						case "KycReasonCode":
						
							if (value == null || value is System.Int32)
								this.KycReasonCode = (System.Int32?)value;
							break;
						case "CertificateStatus":
						
							if (value == null || value is System.Int32)
								this.CertificateStatus = (System.Int32?)value;
							break;
						case "CertificateStartDate":
						
							if (value == null || value is System.DateTime)
								this.CertificateStartDate = (System.DateTime?)value;
							break;
						case "CertificateExpiryDate":
						
							if (value == null || value is System.DateTime)
								this.CertificateExpiryDate = (System.DateTime?)value;
							break;
						case "ConsentTimestamp":
						
							if (value == null || value is System.DateTime)
								this.ConsentTimestamp = (System.DateTime?)value;
							break;
						case "KtpUploadedDateTime":
						
							if (value == null || value is System.DateTime)
								this.KtpUploadedDateTime = (System.DateTime?)value;
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
						case "IsQuickSignUser":
						
							if (value == null || value is System.Boolean)
								this.IsQuickSignUser = (System.Boolean?)value;
							break;
						case "QuickSignRegisteredDateTime":
						
							if (value == null || value is System.DateTime)
								this.QuickSignRegisteredDateTime = (System.DateTime?)value;
							break;
						case "RevokeRequestedDateTime":
						
							if (value == null || value is System.DateTime)
								this.RevokeRequestedDateTime = (System.DateTime?)value;
							break;
						case "RevokeCompletedDateTime":
						
							if (value == null || value is System.DateTime)
								this.RevokeCompletedDateTime = (System.DateTime?)value;
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
		/// Maps to TilakaRegistration.TilakaRegistrationID
		/// </summary>
		virtual public System.Int64? TilakaRegistrationID
		{
			get
			{
				return base.GetSystemInt64(TilakaRegistrationMetadata.ColumnNames.TilakaRegistrationID);
			}
			
			set
			{
				base.SetSystemInt64(TilakaRegistrationMetadata.ColumnNames.TilakaRegistrationID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RelatedType
		/// </summary>
		virtual public System.String RelatedType
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RelatedType);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RelatedType, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RelatedID
		/// </summary>
		virtual public System.String RelatedID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RelatedID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RelatedID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.IsLatest
		/// </summary>
		virtual public System.Boolean? IsLatest
		{
			get
			{
				return base.GetSystemBoolean(TilakaRegistrationMetadata.ColumnNames.IsLatest);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaRegistrationMetadata.ColumnNames.IsLatest, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegistrationState
		/// </summary>
		virtual public System.String RegistrationState
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RegistrationState);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RegistrationState, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegistrationRequestID
		/// </summary>
		virtual public System.String RegistrationRequestID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RegistrationRequestID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RegistrationRequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RequestType
		/// </summary>
		virtual public System.String RequestType
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RequestType);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RequestType, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegistrationDateTime
		/// </summary>
		virtual public System.DateTime? RegistrationDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RegistrationDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RegistrationDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.DateExpire
		/// </summary>
		virtual public System.DateTime? DateExpire
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.DateExpire);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.DateExpire, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegisteredName
		/// </summary>
		virtual public System.String RegisteredName
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredName);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegisteredNIK
		/// </summary>
		virtual public System.String RegisteredNIK
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredNIK);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredNIK, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RegisteredEmail
		/// </summary>
		virtual public System.String RegisteredEmail
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredEmail);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RegisteredEmail, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KycStatus
		/// </summary>
		virtual public System.String KycStatus
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.KycStatus);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.KycStatus, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KycReasonCode
		/// </summary>
		virtual public System.Int32? KycReasonCode
		{
			get
			{
				return base.GetSystemInt32(TilakaRegistrationMetadata.ColumnNames.KycReasonCode);
			}
			
			set
			{
				base.SetSystemInt32(TilakaRegistrationMetadata.ColumnNames.KycReasonCode, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.ManualRegistrationStatus
		/// </summary>
		virtual public System.String ManualRegistrationStatus
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.ManualRegistrationStatus);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.ManualRegistrationStatus, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.TilakaName
		/// </summary>
		virtual public System.String TilakaName
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.TilakaName);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.TilakaName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateStatus
		/// </summary>
		virtual public System.Int32? CertificateStatus
		{
			get
			{
				return base.GetSystemInt32(TilakaRegistrationMetadata.ColumnNames.CertificateStatus);
			}
			
			set
			{
				base.SetSystemInt32(TilakaRegistrationMetadata.ColumnNames.CertificateStatus, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateStatusInfo
		/// </summary>
		virtual public System.String CertificateStatusInfo
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateStatusInfo);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateStatusInfo, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateSerialNumber
		/// </summary>
		virtual public System.String CertificateSerialNumber
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateSerialNumber);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateSerialNumber, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateStartDate
		/// </summary>
		virtual public System.DateTime? CertificateStartDate
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CertificateStartDate);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CertificateStartDate, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateExpiryDate
		/// </summary>
		virtual public System.DateTime? CertificateExpiryDate
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CertificateExpiryDate);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CertificateExpiryDate, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.ConsentVersion
		/// </summary>
		virtual public System.String ConsentVersion
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.ConsentVersion);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.ConsentVersion, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.ConsentTimestamp
		/// </summary>
		virtual public System.DateTime? ConsentTimestamp
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.ConsentTimestamp);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.ConsentTimestamp, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.ConsentApprovedByUserID
		/// </summary>
		virtual public System.String ConsentApprovedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.ConsentApprovedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.ConsentApprovedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KtpFilePath
		/// </summary>
		virtual public System.String KtpFilePath
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpFilePath);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpFilePath, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KtpFileName
		/// </summary>
		virtual public System.String KtpFileName
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpFileName);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpFileName, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KtpContentType
		/// </summary>
		virtual public System.String KtpContentType
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpContentType);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.KtpContentType, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.KtpUploadedDateTime
		/// </summary>
		virtual public System.DateTime? KtpUploadedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.KtpUploadedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.KtpUploadedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.LastCheckedDateTime
		/// </summary>
		virtual public System.DateTime? LastCheckedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.LastCheckedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.LastCheckedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.LastCheckSource
		/// </summary>
		virtual public System.String LastCheckSource
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.LastCheckSource);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.LastCheckSource, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.LastErrorMessage
		/// </summary>
		virtual public System.String LastErrorMessage
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.LastErrorMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.LastErrorMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CreatedDateTime
		/// </summary>
		virtual public System.DateTime? CreatedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CreatedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.CreatedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CreatedByUserID
		/// </summary>
		virtual public System.String CreatedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.CreatedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.CreatedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.LastUpdateDateTime
		/// </summary>
		virtual public System.DateTime? LastUpdateDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.LastUpdateDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.LastUpdateDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.LastUpdateByUserID
		/// </summary>
		virtual public System.String LastUpdateByUserID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.LastUpdateByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.LastUpdateByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.IsQuickSignUser
		/// </summary>
		virtual public System.Boolean? IsQuickSignUser
		{
			get
			{
				return base.GetSystemBoolean(TilakaRegistrationMetadata.ColumnNames.IsQuickSignUser);
			}
			
			set
			{
				base.SetSystemBoolean(TilakaRegistrationMetadata.ColumnNames.IsQuickSignUser, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.QuickSignRegisteredDateTime
		/// </summary>
		virtual public System.DateTime? QuickSignRegisteredDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.QuickSignRegisteredDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.QuickSignRegisteredDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.QuickSignLastErrorMessage
		/// </summary>
		virtual public System.String QuickSignLastErrorMessage
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.QuickSignLastErrorMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.QuickSignLastErrorMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.CertificateDataStatus
		/// </summary>
		virtual public System.String CertificateDataStatus
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateDataStatus);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.CertificateDataStatus, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeRequestID
		/// </summary>
		virtual public System.String RevokeRequestID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeRequestID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeRequestID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeReason
		/// </summary>
		virtual public System.String RevokeReason
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeReason);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeReason, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeNote
		/// </summary>
		virtual public System.String RevokeNote
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeNote);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeNote, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeStatus
		/// </summary>
		virtual public System.String RevokeStatus
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeStatus);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeStatus, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeMessage
		/// </summary>
		virtual public System.String RevokeMessage
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeMessage);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeMessage, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeRequestedDateTime
		/// </summary>
		virtual public System.DateTime? RevokeRequestedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedDateTime, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeRequestedByUserID
		/// </summary>
		virtual public System.String RevokeRequestedByUserID
		{
			get
			{
				return base.GetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedByUserID);
			}
			
			set
			{
				base.SetSystemString(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedByUserID, value);
			}
		}
		/// <summary>
		/// Maps to TilakaRegistration.RevokeCompletedDateTime
		/// </summary>
		virtual public System.DateTime? RevokeCompletedDateTime
		{
			get
			{
				return base.GetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RevokeCompletedDateTime);
			}
			
			set
			{
				base.SetSystemDateTime(TilakaRegistrationMetadata.ColumnNames.RevokeCompletedDateTime, value);
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
			public esStrings(esTilakaRegistration entity)
			{
				this.entity = entity;
			}
			public System.String TilakaRegistrationID
			{
				get
				{
					System.Int64? data = entity.TilakaRegistrationID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TilakaRegistrationID = null;
					else entity.TilakaRegistrationID = Convert.ToInt64(value);
				}
			}
			public System.String RelatedType
			{
				get
				{
					System.String data = entity.RelatedType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RelatedType = null;
					else entity.RelatedType = Convert.ToString(value);
				}
			}
			public System.String RelatedID
			{
				get
				{
					System.String data = entity.RelatedID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RelatedID = null;
					else entity.RelatedID = Convert.ToString(value);
				}
			}
			public System.String IsLatest
			{
				get
				{
					System.Boolean? data = entity.IsLatest;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsLatest = null;
					else entity.IsLatest = Convert.ToBoolean(value);
				}
			}
			public System.String RegistrationState
			{
				get
				{
					System.String data = entity.RegistrationState;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegistrationState = null;
					else entity.RegistrationState = Convert.ToString(value);
				}
			}
			public System.String RegistrationRequestID
			{
				get
				{
					System.String data = entity.RegistrationRequestID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegistrationRequestID = null;
					else entity.RegistrationRequestID = Convert.ToString(value);
				}
			}
			public System.String RequestType
			{
				get
				{
					System.String data = entity.RequestType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RequestType = null;
					else entity.RequestType = Convert.ToString(value);
				}
			}
			public System.String RegistrationDateTime
			{
				get
				{
					System.DateTime? data = entity.RegistrationDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegistrationDateTime = null;
					else entity.RegistrationDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String DateExpire
			{
				get
				{
					System.DateTime? data = entity.DateExpire;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.DateExpire = null;
					else entity.DateExpire = Convert.ToDateTime(value);
				}
			}
			public System.String RegisteredName
			{
				get
				{
					System.String data = entity.RegisteredName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegisteredName = null;
					else entity.RegisteredName = Convert.ToString(value);
				}
			}
			public System.String RegisteredNIK
			{
				get
				{
					System.String data = entity.RegisteredNIK;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegisteredNIK = null;
					else entity.RegisteredNIK = Convert.ToString(value);
				}
			}
			public System.String RegisteredEmail
			{
				get
				{
					System.String data = entity.RegisteredEmail;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RegisteredEmail = null;
					else entity.RegisteredEmail = Convert.ToString(value);
				}
			}
			public System.String KycStatus
			{
				get
				{
					System.String data = entity.KycStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KycStatus = null;
					else entity.KycStatus = Convert.ToString(value);
				}
			}
			public System.String KycReasonCode
			{
				get
				{
					System.Int32? data = entity.KycReasonCode;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KycReasonCode = null;
					else entity.KycReasonCode = Convert.ToInt32(value);
				}
			}
			public System.String ManualRegistrationStatus
			{
				get
				{
					System.String data = entity.ManualRegistrationStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ManualRegistrationStatus = null;
					else entity.ManualRegistrationStatus = Convert.ToString(value);
				}
			}
			public System.String TilakaName
			{
				get
				{
					System.String data = entity.TilakaName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.TilakaName = null;
					else entity.TilakaName = Convert.ToString(value);
				}
			}
			public System.String CertificateStatus
			{
				get
				{
					System.Int32? data = entity.CertificateStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateStatus = null;
					else entity.CertificateStatus = Convert.ToInt32(value);
				}
			}
			public System.String CertificateStatusInfo
			{
				get
				{
					System.String data = entity.CertificateStatusInfo;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateStatusInfo = null;
					else entity.CertificateStatusInfo = Convert.ToString(value);
				}
			}
			public System.String CertificateSerialNumber
			{
				get
				{
					System.String data = entity.CertificateSerialNumber;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateSerialNumber = null;
					else entity.CertificateSerialNumber = Convert.ToString(value);
				}
			}
			public System.String CertificateStartDate
			{
				get
				{
					System.DateTime? data = entity.CertificateStartDate;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateStartDate = null;
					else entity.CertificateStartDate = Convert.ToDateTime(value);
				}
			}
			public System.String CertificateExpiryDate
			{
				get
				{
					System.DateTime? data = entity.CertificateExpiryDate;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateExpiryDate = null;
					else entity.CertificateExpiryDate = Convert.ToDateTime(value);
				}
			}
			public System.String ConsentVersion
			{
				get
				{
					System.String data = entity.ConsentVersion;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConsentVersion = null;
					else entity.ConsentVersion = Convert.ToString(value);
				}
			}
			public System.String ConsentTimestamp
			{
				get
				{
					System.DateTime? data = entity.ConsentTimestamp;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConsentTimestamp = null;
					else entity.ConsentTimestamp = Convert.ToDateTime(value);
				}
			}
			public System.String ConsentApprovedByUserID
			{
				get
				{
					System.String data = entity.ConsentApprovedByUserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.ConsentApprovedByUserID = null;
					else entity.ConsentApprovedByUserID = Convert.ToString(value);
				}
			}
			public System.String KtpFilePath
			{
				get
				{
					System.String data = entity.KtpFilePath;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KtpFilePath = null;
					else entity.KtpFilePath = Convert.ToString(value);
				}
			}
			public System.String KtpFileName
			{
				get
				{
					System.String data = entity.KtpFileName;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KtpFileName = null;
					else entity.KtpFileName = Convert.ToString(value);
				}
			}
			public System.String KtpContentType
			{
				get
				{
					System.String data = entity.KtpContentType;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KtpContentType = null;
					else entity.KtpContentType = Convert.ToString(value);
				}
			}
			public System.String KtpUploadedDateTime
			{
				get
				{
					System.DateTime? data = entity.KtpUploadedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.KtpUploadedDateTime = null;
					else entity.KtpUploadedDateTime = Convert.ToDateTime(value);
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
			public System.String LastCheckSource
			{
				get
				{
					System.String data = entity.LastCheckSource;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.LastCheckSource = null;
					else entity.LastCheckSource = Convert.ToString(value);
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
			public System.String IsQuickSignUser
			{
				get
				{
					System.Boolean? data = entity.IsQuickSignUser;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.IsQuickSignUser = null;
					else entity.IsQuickSignUser = Convert.ToBoolean(value);
				}
			}
			public System.String QuickSignRegisteredDateTime
			{
				get
				{
					System.DateTime? data = entity.QuickSignRegisteredDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QuickSignRegisteredDateTime = null;
					else entity.QuickSignRegisteredDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String QuickSignLastErrorMessage
			{
				get
				{
					System.String data = entity.QuickSignLastErrorMessage;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.QuickSignLastErrorMessage = null;
					else entity.QuickSignLastErrorMessage = Convert.ToString(value);
				}
			}
			public System.String CertificateDataStatus
			{
				get
				{
					System.String data = entity.CertificateDataStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.CertificateDataStatus = null;
					else entity.CertificateDataStatus = Convert.ToString(value);
				}
			}
			public System.String RevokeRequestID
			{
				get
				{
					System.String data = entity.RevokeRequestID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeRequestID = null;
					else entity.RevokeRequestID = Convert.ToString(value);
				}
			}
			public System.String RevokeReason
			{
				get
				{
					System.String data = entity.RevokeReason;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeReason = null;
					else entity.RevokeReason = Convert.ToString(value);
				}
			}
			public System.String RevokeNote
			{
				get
				{
					System.String data = entity.RevokeNote;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeNote = null;
					else entity.RevokeNote = Convert.ToString(value);
				}
			}
			public System.String RevokeStatus
			{
				get
				{
					System.String data = entity.RevokeStatus;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeStatus = null;
					else entity.RevokeStatus = Convert.ToString(value);
				}
			}
			public System.String RevokeMessage
			{
				get
				{
					System.String data = entity.RevokeMessage;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeMessage = null;
					else entity.RevokeMessage = Convert.ToString(value);
				}
			}
			public System.String RevokeRequestedDateTime
			{
				get
				{
					System.DateTime? data = entity.RevokeRequestedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeRequestedDateTime = null;
					else entity.RevokeRequestedDateTime = Convert.ToDateTime(value);
				}
			}
			public System.String RevokeRequestedByUserID
			{
				get
				{
					System.String data = entity.RevokeRequestedByUserID;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeRequestedByUserID = null;
					else entity.RevokeRequestedByUserID = Convert.ToString(value);
				}
			}
			public System.String RevokeCompletedDateTime
			{
				get
				{
					System.DateTime? data = entity.RevokeCompletedDateTime;
					return (data == null) ? String.Empty : Convert.ToString(data);
				}

				set
				{
					if (value == null || value.Length == 0) entity.RevokeCompletedDateTime = null;
					else entity.RevokeCompletedDateTime = Convert.ToDateTime(value);
				}
			}
			
			private esTilakaRegistration entity;
		}
		#endregion

		#region Query Logic
		protected void InitQuery(esTilakaRegistrationQuery query)
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
				throw new Exception("esTilakaRegistration can only hold one record of data");
			}

			return dataFound;
		}
		#endregion
		
		[NonSerialized]
		private esStrings esstrings;
	}


	public partial class TilakaRegistration : esTilakaRegistration
	{	
	}

	[Serializable]
	abstract public class esTilakaRegistrationQuery : esDynamicQuery
	{
				
		override protected IMetadata Meta
		{
			get
			{
				return TilakaRegistrationMetadata.Meta();
			}
		}	
			
		public esQueryItem TilakaRegistrationID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.TilakaRegistrationID, esSystemType.Int64);
			}
		} 
		public esQueryItem RelatedType
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RelatedType, esSystemType.String);
			}
		} 
		public esQueryItem RelatedID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RelatedID, esSystemType.String);
			}
		} 
		public esQueryItem IsLatest
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.IsLatest, esSystemType.Boolean);
			}
		} 
		public esQueryItem RegistrationState
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegistrationState, esSystemType.String);
			}
		} 
		public esQueryItem RegistrationRequestID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegistrationRequestID, esSystemType.String);
			}
		} 
		public esQueryItem RequestType
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RequestType, esSystemType.String);
			}
		} 
		public esQueryItem RegistrationDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegistrationDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem DateExpire
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.DateExpire, esSystemType.DateTime);
			}
		} 
		public esQueryItem RegisteredName
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegisteredName, esSystemType.String);
			}
		} 
		public esQueryItem RegisteredNIK
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegisteredNIK, esSystemType.String);
			}
		} 
		public esQueryItem RegisteredEmail
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RegisteredEmail, esSystemType.String);
			}
		} 
		public esQueryItem KycStatus
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KycStatus, esSystemType.String);
			}
		} 
		public esQueryItem KycReasonCode
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KycReasonCode, esSystemType.Int32);
			}
		} 
		public esQueryItem ManualRegistrationStatus
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.ManualRegistrationStatus, esSystemType.String);
			}
		} 
		public esQueryItem TilakaName
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.TilakaName, esSystemType.String);
			}
		} 
		public esQueryItem CertificateStatus
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateStatus, esSystemType.Int32);
			}
		} 
		public esQueryItem CertificateStatusInfo
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateStatusInfo, esSystemType.String);
			}
		} 
		public esQueryItem CertificateSerialNumber
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateSerialNumber, esSystemType.String);
			}
		} 
		public esQueryItem CertificateStartDate
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateStartDate, esSystemType.DateTime);
			}
		} 
		public esQueryItem CertificateExpiryDate
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateExpiryDate, esSystemType.DateTime);
			}
		} 
		public esQueryItem ConsentVersion
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.ConsentVersion, esSystemType.String);
			}
		} 
		public esQueryItem ConsentTimestamp
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.ConsentTimestamp, esSystemType.DateTime);
			}
		} 
		public esQueryItem ConsentApprovedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.ConsentApprovedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem KtpFilePath
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KtpFilePath, esSystemType.String);
			}
		} 
		public esQueryItem KtpFileName
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KtpFileName, esSystemType.String);
			}
		} 
		public esQueryItem KtpContentType
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KtpContentType, esSystemType.String);
			}
		} 
		public esQueryItem KtpUploadedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.KtpUploadedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastCheckedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.LastCheckedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastCheckSource
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.LastCheckSource, esSystemType.String);
			}
		} 
		public esQueryItem LastErrorMessage
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.LastErrorMessage, esSystemType.String);
			}
		} 
		public esQueryItem CreatedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem CreatedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem LastUpdateDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem LastUpdateByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
			}
		} 
		public esQueryItem IsQuickSignUser
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.IsQuickSignUser, esSystemType.Boolean);
			}
		} 
		public esQueryItem QuickSignRegisteredDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.QuickSignRegisteredDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem QuickSignLastErrorMessage
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.QuickSignLastErrorMessage, esSystemType.String);
			}
		} 
		public esQueryItem CertificateDataStatus
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.CertificateDataStatus, esSystemType.String);
			}
		} 
		public esQueryItem RevokeRequestID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeRequestID, esSystemType.String);
			}
		} 
		public esQueryItem RevokeReason
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeReason, esSystemType.String);
			}
		} 
		public esQueryItem RevokeNote
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeNote, esSystemType.String);
			}
		} 
		public esQueryItem RevokeStatus
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeStatus, esSystemType.String);
			}
		} 
		public esQueryItem RevokeMessage
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeMessage, esSystemType.String);
			}
		} 
		public esQueryItem RevokeRequestedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeRequestedDateTime, esSystemType.DateTime);
			}
		} 
		public esQueryItem RevokeRequestedByUserID
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeRequestedByUserID, esSystemType.String);
			}
		} 
		public esQueryItem RevokeCompletedDateTime
		{
			get
			{
				return new esQueryItem(this, TilakaRegistrationMetadata.ColumnNames.RevokeCompletedDateTime, esSystemType.DateTime);
			}
		} 
	} 
	
    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
	[Serializable]
	[XmlType("TilakaRegistrationCollection")]
	public partial class TilakaRegistrationCollection : esTilakaRegistrationCollection, IEnumerable< TilakaRegistration>
	{
		public TilakaRegistrationCollection()
		{

		}	
		
		public static implicit operator List< TilakaRegistration>(TilakaRegistrationCollection coll)
		{
			List< TilakaRegistration> list = new List< TilakaRegistration>();
			
			foreach (TilakaRegistration emp in coll)
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
				return  TilakaRegistrationMetadata.Meta();
			}
		}
		
		override protected esDynamicQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaRegistrationQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		
		override protected esEntity CreateEntityForCollection(DataRow row)
		{
			return new TilakaRegistration(row);
		}

		override protected esEntity CreateEntity()
		{
			return new TilakaRegistration();
		}
		
		#endregion

		[BrowsableAttribute( false )]
		public TilakaRegistrationQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaRegistrationQuery();
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
		public bool Load(TilakaRegistrationQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}		
		
		/// <summary>
		/// Adds a new entity to the collection.
		/// Always calls AddNew() on the entity, in case it is overridden.
		/// </summary>
		public TilakaRegistration AddNew()
		{
			TilakaRegistration entity = base.AddNewEntity() as TilakaRegistration;
			
			return entity;		
		}
		public TilakaRegistration FindByPrimaryKey(String standardReferenceID)
		{
			return base.FindByPrimaryKey(standardReferenceID) as TilakaRegistration;
		}

		#region IEnumerable< TilakaRegistration> Members

		IEnumerator< TilakaRegistration> IEnumerable< TilakaRegistration>.GetEnumerator()
		{
			System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
			System.Collections.IEnumerator iterator = enumer.GetEnumerator();

			while(iterator.MoveNext())
			{
				yield return iterator.Current as TilakaRegistration;
			}
		}

		#endregion
		
		private TilakaRegistrationQuery query;
	}


	/// <summary>
	/// Encapsulates the 'TilakaRegistration' table
	/// </summary>
	[System.Diagnostics.DebuggerDisplay("TilakaRegistration ({StandardReferenceID})")]
	[Serializable]
	public partial class TilakaRegistration : esTilakaRegistration
	{
		public TilakaRegistration()
		{
		}	
	
		public TilakaRegistration(DataRow row)
			: base(row)
		{
		}
		
		#region Housekeeping methods
		override protected IMetadata Meta
		{
			get
			{
				return TilakaRegistrationMetadata.Meta();
			}
		}	
	
		override protected esTilakaRegistrationQuery GetDynamicQuery()
		{
			if (this.query == null)
			{
				this.query = new TilakaRegistrationQuery();
				this.InitQuery(query);
			}
			return this.query;
		}
		#endregion
		
		[BrowsableAttribute( false )]
		public TilakaRegistrationQuery Query
		{
			get
			{
				if (this.query == null)
				{
					this.query = new TilakaRegistrationQuery();
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
		public bool Load(TilakaRegistrationQuery query)
		{
			this.query = query;
			base.InitQuery(this.query);
			return this.Query.Load();
		}			
		
		private TilakaRegistrationQuery query;
	}

	[System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
	[Serializable]
	public partial class TilakaRegistrationQuery : esTilakaRegistrationQuery
	{
		public TilakaRegistrationQuery()
		{

		}		
		
		public TilakaRegistrationQuery(string joinAlias)
		{
			this.es.JoinAlias = joinAlias;
		}	
		
		override protected string GetQueryName()
        {
            return "TilakaRegistrationQuery";
        }
	}

	[Serializable]
	public partial class TilakaRegistrationMetadata : esMetadata, IMetadata
	{
		#region Protected Constructor
		protected TilakaRegistrationMetadata()
		{
			_columns = new esColumnMetadataCollection();
			esColumnMetadata c;
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.TilakaRegistrationID, 0, typeof(System.Int64), esSystemType.Int64);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.TilakaRegistrationID;
			c.IsInPrimaryKey = true;
			c.IsAutoIncrement = true;
			c.NumericPrecision = 19;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RelatedType, 1, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RelatedType;
			c.CharacterMaxLength = 30;
			c.HasDefault = true;
			c.Default = @"('Paramedic')";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RelatedID, 2, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RelatedID;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.IsLatest, 3, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.IsLatest;
			c.HasDefault = true;
			c.Default = @"((1))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegistrationState, 4, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegistrationState;
			c.CharacterMaxLength = 30;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegistrationRequestID, 5, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegistrationRequestID;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RequestType, 6, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RequestType;
			c.CharacterMaxLength = 20;
			c.HasDefault = true;
			c.Default = @"('register')";
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegistrationDateTime, 7, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegistrationDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.DateExpire, 8, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.DateExpire;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegisteredName, 9, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegisteredName;
			c.CharacterMaxLength = 200;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegisteredNIK, 10, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegisteredNIK;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RegisteredEmail, 11, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RegisteredEmail;
			c.CharacterMaxLength = 200;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KycStatus, 12, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KycStatus;
			c.CharacterMaxLength = 1;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KycReasonCode, 13, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KycReasonCode;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.ManualRegistrationStatus, 14, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.ManualRegistrationStatus;
			c.CharacterMaxLength = 1;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.TilakaName, 15, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.TilakaName;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateStatus, 16, typeof(System.Int32), esSystemType.Int32);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateStatus;
			c.NumericPrecision = 10;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateStatusInfo, 17, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateStatusInfo;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateSerialNumber, 18, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateSerialNumber;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateStartDate, 19, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateStartDate;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateExpiryDate, 20, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateExpiryDate;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.ConsentVersion, 21, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.ConsentVersion;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.ConsentTimestamp, 22, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.ConsentTimestamp;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.ConsentApprovedByUserID, 23, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.ConsentApprovedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KtpFilePath, 24, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KtpFilePath;
			c.CharacterMaxLength = 300;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KtpFileName, 25, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KtpFileName;
			c.CharacterMaxLength = 255;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KtpContentType, 26, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KtpContentType;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.KtpUploadedDateTime, 27, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.KtpUploadedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.LastCheckedDateTime, 28, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.LastCheckedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.LastCheckSource, 29, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.LastCheckSource;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.LastErrorMessage, 30, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.LastErrorMessage;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CreatedDateTime, 31, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CreatedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CreatedByUserID, 32, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CreatedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.LastUpdateDateTime, 33, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.LastUpdateDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.LastUpdateByUserID, 34, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.LastUpdateByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.IsQuickSignUser, 35, typeof(System.Boolean), esSystemType.Boolean);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.IsQuickSignUser;
			c.HasDefault = true;
			c.Default = @"((0))";
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.QuickSignRegisteredDateTime, 36, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.QuickSignRegisteredDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.QuickSignLastErrorMessage, 37, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.QuickSignLastErrorMessage;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.CertificateDataStatus, 38, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.CertificateDataStatus;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeRequestID, 39, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeRequestID;
			c.CharacterMaxLength = 50;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeReason, 40, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeReason;
			c.CharacterMaxLength = 100;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeNote, 41, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeNote;
			c.CharacterMaxLength = 500;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeStatus, 42, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeStatus;
			c.CharacterMaxLength = 20;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeMessage, 43, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeMessage;
			c.CharacterMaxLength = 1000;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedDateTime, 44, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeRequestedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeRequestedByUserID, 45, typeof(System.String), esSystemType.String);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeRequestedByUserID;
			c.CharacterMaxLength = 40;
			c.IsNullable = true;
			_columns.Add(c); 
				
			c = new esColumnMetadata(TilakaRegistrationMetadata.ColumnNames.RevokeCompletedDateTime, 46, typeof(System.DateTime), esSystemType.DateTime);
			c.PropertyName = TilakaRegistrationMetadata.PropertyNames.RevokeCompletedDateTime;
			c.IsNullable = true;
			_columns.Add(c); 
		}
		#endregion
	
		static public TilakaRegistrationMetadata Meta()
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
			public const string TilakaRegistrationID = "TilakaRegistrationID";
			public const string RelatedType = "RelatedType";
			public const string RelatedID = "RelatedID";
			public const string IsLatest = "IsLatest";
			public const string RegistrationState = "RegistrationState";
			public const string RegistrationRequestID = "RegistrationRequestID";
			public const string RequestType = "RequestType";
			public const string RegistrationDateTime = "RegistrationDateTime";
			public const string DateExpire = "DateExpire";
			public const string RegisteredName = "RegisteredName";
			public const string RegisteredNIK = "RegisteredNIK";
			public const string RegisteredEmail = "RegisteredEmail";
			public const string KycStatus = "KycStatus";
			public const string KycReasonCode = "KycReasonCode";
			public const string ManualRegistrationStatus = "ManualRegistrationStatus";
			public const string TilakaName = "TilakaName";
			public const string CertificateStatus = "CertificateStatus";
			public const string CertificateStatusInfo = "CertificateStatusInfo";
			public const string CertificateSerialNumber = "CertificateSerialNumber";
			public const string CertificateStartDate = "CertificateStartDate";
			public const string CertificateExpiryDate = "CertificateExpiryDate";
			public const string ConsentVersion = "ConsentVersion";
			public const string ConsentTimestamp = "ConsentTimestamp";
			public const string ConsentApprovedByUserID = "ConsentApprovedByUserID";
			public const string KtpFilePath = "KtpFilePath";
			public const string KtpFileName = "KtpFileName";
			public const string KtpContentType = "KtpContentType";
			public const string KtpUploadedDateTime = "KtpUploadedDateTime";
			public const string LastCheckedDateTime = "LastCheckedDateTime";
			public const string LastCheckSource = "LastCheckSource";
			public const string LastErrorMessage = "LastErrorMessage";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string IsQuickSignUser = "IsQuickSignUser";
			public const string QuickSignRegisteredDateTime = "QuickSignRegisteredDateTime";
			public const string QuickSignLastErrorMessage = "QuickSignLastErrorMessage";
			public const string CertificateDataStatus = "CertificateDataStatus";
			public const string RevokeRequestID = "RevokeRequestID";
			public const string RevokeReason = "RevokeReason";
			public const string RevokeNote = "RevokeNote";
			public const string RevokeStatus = "RevokeStatus";
			public const string RevokeMessage = "RevokeMessage";
			public const string RevokeRequestedDateTime = "RevokeRequestedDateTime";
			public const string RevokeRequestedByUserID = "RevokeRequestedByUserID";
			public const string RevokeCompletedDateTime = "RevokeCompletedDateTime";
		}
		#endregion	
		
		#region PropertyNames
		public class PropertyNames
		{ 
			public const string TilakaRegistrationID = "TilakaRegistrationID";
			public const string RelatedType = "RelatedType";
			public const string RelatedID = "RelatedID";
			public const string IsLatest = "IsLatest";
			public const string RegistrationState = "RegistrationState";
			public const string RegistrationRequestID = "RegistrationRequestID";
			public const string RequestType = "RequestType";
			public const string RegistrationDateTime = "RegistrationDateTime";
			public const string DateExpire = "DateExpire";
			public const string RegisteredName = "RegisteredName";
			public const string RegisteredNIK = "RegisteredNIK";
			public const string RegisteredEmail = "RegisteredEmail";
			public const string KycStatus = "KycStatus";
			public const string KycReasonCode = "KycReasonCode";
			public const string ManualRegistrationStatus = "ManualRegistrationStatus";
			public const string TilakaName = "TilakaName";
			public const string CertificateStatus = "CertificateStatus";
			public const string CertificateStatusInfo = "CertificateStatusInfo";
			public const string CertificateSerialNumber = "CertificateSerialNumber";
			public const string CertificateStartDate = "CertificateStartDate";
			public const string CertificateExpiryDate = "CertificateExpiryDate";
			public const string ConsentVersion = "ConsentVersion";
			public const string ConsentTimestamp = "ConsentTimestamp";
			public const string ConsentApprovedByUserID = "ConsentApprovedByUserID";
			public const string KtpFilePath = "KtpFilePath";
			public const string KtpFileName = "KtpFileName";
			public const string KtpContentType = "KtpContentType";
			public const string KtpUploadedDateTime = "KtpUploadedDateTime";
			public const string LastCheckedDateTime = "LastCheckedDateTime";
			public const string LastCheckSource = "LastCheckSource";
			public const string LastErrorMessage = "LastErrorMessage";
			public const string CreatedDateTime = "CreatedDateTime";
			public const string CreatedByUserID = "CreatedByUserID";
			public const string LastUpdateDateTime = "LastUpdateDateTime";
			public const string LastUpdateByUserID = "LastUpdateByUserID";
			public const string IsQuickSignUser = "IsQuickSignUser";
			public const string QuickSignRegisteredDateTime = "QuickSignRegisteredDateTime";
			public const string QuickSignLastErrorMessage = "QuickSignLastErrorMessage";
			public const string CertificateDataStatus = "CertificateDataStatus";
			public const string RevokeRequestID = "RevokeRequestID";
			public const string RevokeReason = "RevokeReason";
			public const string RevokeNote = "RevokeNote";
			public const string RevokeStatus = "RevokeStatus";
			public const string RevokeMessage = "RevokeMessage";
			public const string RevokeRequestedDateTime = "RevokeRequestedDateTime";
			public const string RevokeRequestedByUserID = "RevokeRequestedByUserID";
			public const string RevokeCompletedDateTime = "RevokeCompletedDateTime";
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
			lock (typeof(TilakaRegistrationMetadata))
			{
				if(TilakaRegistrationMetadata.mapDelegates == null)
				{
					TilakaRegistrationMetadata.mapDelegates = new Dictionary<string,MapToMeta>();
				}
				
				if (TilakaRegistrationMetadata.meta == null)
				{
					TilakaRegistrationMetadata.meta = new TilakaRegistrationMetadata();
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
				
				meta.AddTypeMap("TilakaRegistrationID", new esTypeMap("bigint", "System.Int64"));
				meta.AddTypeMap("RelatedType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RelatedID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsLatest", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("RegistrationState", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RegistrationRequestID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RequestType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RegistrationDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("DateExpire", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("RegisteredName", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RegisteredNIK", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RegisteredEmail", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KycStatus", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KycReasonCode", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("ManualRegistrationStatus", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("TilakaName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("CertificateStatus", new esTypeMap("int", "System.Int32"));
				meta.AddTypeMap("CertificateStatusInfo", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CertificateSerialNumber", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("CertificateStartDate", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CertificateExpiryDate", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("ConsentVersion", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("ConsentTimestamp", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("ConsentApprovedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KtpFilePath", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KtpFileName", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KtpContentType", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("KtpUploadedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastCheckedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastCheckSource", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastErrorMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("IsQuickSignUser", new esTypeMap("bit", "System.Boolean"));
				meta.AddTypeMap("QuickSignRegisteredDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("QuickSignLastErrorMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("CertificateDataStatus", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RevokeRequestID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RevokeReason", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RevokeNote", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RevokeStatus", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RevokeMessage", new esTypeMap("nvarchar", "System.String"));
				meta.AddTypeMap("RevokeRequestedDateTime", new esTypeMap("datetime", "System.DateTime"));
				meta.AddTypeMap("RevokeRequestedByUserID", new esTypeMap("varchar", "System.String"));
				meta.AddTypeMap("RevokeCompletedDateTime", new esTypeMap("datetime", "System.DateTime"));
		

				meta.Source = "TilakaRegistration";
				meta.Destination = "TilakaRegistration";
				meta.spInsert = "proc_TilakaRegistrationInsert";				
				meta.spUpdate = "proc_TilakaRegistrationUpdate";		
				meta.spDelete = "proc_TilakaRegistrationDelete";
				meta.spLoadAll = "proc_TilakaRegistrationLoadAll";
				meta.spLoadByPrimaryKey = "proc_TilakaRegistrationLoadByPrimaryKey";
				
				this._providerMetadataMaps["esDefault"] = meta;
			}
			
			return this._providerMetadataMaps["esDefault"];
		}

		#endregion

		static private TilakaRegistrationMetadata meta;
		static protected Dictionary<string, MapToMeta> mapDelegates;
		static private int _esDefault = RegisterDelegateesDefault();
	}

}		