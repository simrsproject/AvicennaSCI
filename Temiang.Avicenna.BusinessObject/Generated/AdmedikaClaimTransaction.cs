/*
===============================================================================
                       Persistence Layer and Business Objects
===============================================================================
                    Date Generated       : 2026-11-03 12:08:17 PM
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
    abstract public class esAdmedikaClaimTransactionCollection : esEntityCollectionWAuditLog
    {
        public esAdmedikaClaimTransactionCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "AdmedikaClaimTransactionCollection";
        }

        #region Query Logic
        protected void InitQuery(esAdmedikaClaimTransactionQuery query)
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
            this.InitQuery(query as esAdmedikaClaimTransactionQuery);
        }
        #endregion

        virtual public AdmedikaClaimTransaction DetachEntity(AdmedikaClaimTransaction entity)
        {
            return base.DetachEntity(entity) as AdmedikaClaimTransaction;
        }

        virtual public AdmedikaClaimTransaction AttachEntity(AdmedikaClaimTransaction entity)
        {
            return base.AttachEntity(entity) as AdmedikaClaimTransaction;
        }

        virtual public void Combine(AdmedikaClaimTransactionCollection collection)
        {
            base.Combine(collection);
        }

        new public AdmedikaClaimTransaction this[int index]
        {
            get
            {
                return base[index] as AdmedikaClaimTransaction;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(AdmedikaClaimTransaction);
        }
    }

    [Serializable]
    abstract public class esAdmedikaClaimTransaction : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esAdmedikaClaimTransactionQuery GetDynamicQuery()
        {
            return null;
        }

        public esAdmedikaClaimTransaction()
        {
        }

        public esAdmedikaClaimTransaction(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(Int64 admedikaClaimTransactionID)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(admedikaClaimTransactionID);
            else
                return LoadByPrimaryKeyStoredProcedure(admedikaClaimTransactionID);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, Int64 admedikaClaimTransactionID)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(admedikaClaimTransactionID);
            else
                return LoadByPrimaryKeyStoredProcedure(admedikaClaimTransactionID);
        }

        private bool LoadByPrimaryKeyDynamic(Int64 admedikaClaimTransactionID)
        {
            esAdmedikaClaimTransactionQuery query = this.GetDynamicQuery();
            query.Where(query.AdmedikaClaimTransactionID == admedikaClaimTransactionID);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(Int64 admedikaClaimTransactionID)
        {
            esParameters parms = new esParameters();
            parms.Add("AdmedikaClaimTransactionID", admedikaClaimTransactionID);
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
            if (this.Row == null) this.AddNew();

            esColumnMetadata col = this.Meta.Columns.FindByPropertyName(name);
            if (col != null)
            {
                if (value == null || value is System.String)
                {
                    // Use the strongly typed property
                    switch (name)
                    {
                        case "AdmedikaClaimTransactionID": this.str.AdmedikaClaimTransactionID = (string)value; break;
                        case "RegistrationNo": this.str.RegistrationNo = (string)value; break;
                        case "TransactionNo": this.str.TransactionNo = (string)value; break;
                        case "PatientID": this.str.PatientID = (string)value; break;
                        case "GuarantorID": this.str.GuarantorID = (string)value; break;
                        case "CardNo": this.str.CardNo = (string)value; break;
                        case "ClID": this.str.ClID = (string)value; break;
                        case "CovID": this.str.CovID = (string)value; break;
                        case "CovCode": this.str.CovCode = (string)value; break;
                        case "PlanCode": this.str.PlanCode = (string)value; break;
                        case "DiagnosisCodeList": this.str.DiagnosisCodeList = (string)value; break;
                        case "ClStatus": this.str.ClStatus = (string)value; break;
                        case "ClDesc": this.str.ClDesc = (string)value; break;
                        case "ServiceID": this.str.ServiceID = (string)value; break;
                        case "PrintOut": this.str.PrintOut = (string)value; break;
                        case "TotAmtIncurred": this.str.TotAmtIncurred = (string)value; break;
                        case "TotAmtApproved": this.str.TotAmtApproved = (string)value; break;
                        case "TotAmtNotApproved": this.str.TotAmtNotApproved = (string)value; break;
                        case "IsClosed": this.str.IsClosed = (string)value; break;
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
                        case "AdmedikaClaimTransactionID":

                            if (value == null || value is System.Int64)
                                this.AdmedikaClaimTransactionID = (System.Int64?)value;
                            break;
                        case "TotAmtIncurred":

                            if (value == null || value is System.Decimal)
                                this.TotAmtIncurred = (System.Decimal?)value;
                            break;
                        case "TotAmtApproved":

                            if (value == null || value is System.Decimal)
                                this.TotAmtApproved = (System.Decimal?)value;
                            break;
                        case "TotAmtNotApproved":

                            if (value == null || value is System.Decimal)
                                this.TotAmtNotApproved = (System.Decimal?)value;
                            break;
                        case "IsClosed":

                            if (value == null || value is System.Boolean)
                                this.IsClosed = (System.Boolean?)value;
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
            else if (this.Row.Table.Columns.Contains(name))
            {
                this.Row[name] = value;
            }
            else
            {
                throw new Exception("SetProperty Error: '" + name + "' not found");
            }
        }

        /// <summary>
        /// Maps to AdmedikaClaimTransaction.AdmedikaClaimTransactionID
        /// </summary>
        virtual public System.Int64? AdmedikaClaimTransactionID
        {
            get
            {
                return base.GetSystemInt64(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID);
            }

            set
            {
                base.SetSystemInt64(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.RegistrationNo
        /// </summary>
        virtual public System.String RegistrationNo
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.RegistrationNo);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.RegistrationNo, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.TransactionNo
        /// </summary>
        virtual public System.String TransactionNo
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.TransactionNo);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.TransactionNo, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.PatientID
        /// </summary>
        virtual public System.String PatientID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PatientID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PatientID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.GuarantorID
        /// </summary>
        virtual public System.String GuarantorID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.GuarantorID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.GuarantorID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.CardNo
        /// </summary>
        virtual public System.String CardNo
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CardNo);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CardNo, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.ClID
        /// </summary>
        virtual public System.String ClID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.CovID
        /// </summary>
        virtual public System.String CovID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CovID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CovID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.CovCode
        /// </summary>
        virtual public System.String CovCode
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CovCode);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CovCode, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.PlanCode
        /// </summary>
        virtual public System.String PlanCode
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PlanCode);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PlanCode, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.DiagnosisCodeList
        /// </summary>
        virtual public System.String DiagnosisCodeList
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.DiagnosisCodeList);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.DiagnosisCodeList, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.ClStatus
        /// </summary>
        virtual public System.String ClStatus
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClStatus);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClStatus, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.ClDesc
        /// </summary>
        virtual public System.String ClDesc
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClDesc);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ClDesc, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.ServiceID
        /// </summary>
        virtual public System.String ServiceID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ServiceID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.ServiceID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.PrintOut
        /// </summary>
        virtual public System.String PrintOut
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PrintOut);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.PrintOut, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.TotAmtIncurred
        /// </summary>
        virtual public System.Decimal? TotAmtIncurred
        {
            get
            {
                return base.GetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtIncurred);
            }

            set
            {
                base.SetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtIncurred, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.TotAmtApproved
        /// </summary>
        virtual public System.Decimal? TotAmtApproved
        {
            get
            {
                return base.GetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtApproved);
            }

            set
            {
                base.SetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtApproved, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.TotAmtNotApproved
        /// </summary>
        virtual public System.Decimal? TotAmtNotApproved
        {
            get
            {
                return base.GetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtNotApproved);
            }

            set
            {
                base.SetSystemDecimal(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtNotApproved, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.IsClosed
        /// </summary>
        virtual public System.Boolean? IsClosed
        {
            get
            {
                return base.GetSystemBoolean(AdmedikaClaimTransactionMetadata.ColumnNames.IsClosed);
            }

            set
            {
                base.SetSystemBoolean(AdmedikaClaimTransactionMetadata.ColumnNames.IsClosed, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.CreatedDateTime
        /// </summary>
        virtual public System.DateTime? CreatedDateTime
        {
            get
            {
                return base.GetSystemDateTime(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedDateTime);
            }

            set
            {
                base.SetSystemDateTime(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedDateTime, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.CreatedByUserID
        /// </summary>
        virtual public System.String CreatedByUserID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedByUserID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedByUserID, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.LastUpdateDateTime
        /// </summary>
        virtual public System.DateTime? LastUpdateDateTime
        {
            get
            {
                return base.GetSystemDateTime(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateDateTime);
            }

            set
            {
                base.SetSystemDateTime(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateDateTime, value);
            }
        }
        /// <summary>
        /// Maps to AdmedikaClaimTransaction.LastUpdateByUserID
        /// </summary>
        virtual public System.String LastUpdateByUserID
        {
            get
            {
                return base.GetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateByUserID);
            }

            set
            {
                base.SetSystemString(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateByUserID, value);
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
        [BrowsableAttribute(false)]
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
            public esStrings(esAdmedikaClaimTransaction entity)
            {
                this.entity = entity;
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
            public System.String TransactionNo
            {
                get
                {
                    System.String data = entity.TransactionNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TransactionNo = null;
                    else entity.TransactionNo = Convert.ToString(value);
                }
            }
            public System.String PatientID
            {
                get
                {
                    System.String data = entity.PatientID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PatientID = null;
                    else entity.PatientID = Convert.ToString(value);
                }
            }
            public System.String GuarantorID
            {
                get
                {
                    System.String data = entity.GuarantorID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.GuarantorID = null;
                    else entity.GuarantorID = Convert.ToString(value);
                }
            }
            public System.String CardNo
            {
                get
                {
                    System.String data = entity.CardNo;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CardNo = null;
                    else entity.CardNo = Convert.ToString(value);
                }
            }
            public System.String ClID
            {
                get
                {
                    System.String data = entity.ClID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ClID = null;
                    else entity.ClID = Convert.ToString(value);
                }
            }
            public System.String CovID
            {
                get
                {
                    System.String data = entity.CovID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CovID = null;
                    else entity.CovID = Convert.ToString(value);
                }
            }
            public System.String CovCode
            {
                get
                {
                    System.String data = entity.CovCode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.CovCode = null;
                    else entity.CovCode = Convert.ToString(value);
                }
            }
            public System.String PlanCode
            {
                get
                {
                    System.String data = entity.PlanCode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PlanCode = null;
                    else entity.PlanCode = Convert.ToString(value);
                }
            }
            public System.String DiagnosisCodeList
            {
                get
                {
                    System.String data = entity.DiagnosisCodeList;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DiagnosisCodeList = null;
                    else entity.DiagnosisCodeList = Convert.ToString(value);
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
            public System.String ServiceID
            {
                get
                {
                    System.String data = entity.ServiceID;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ServiceID = null;
                    else entity.ServiceID = Convert.ToString(value);
                }
            }
            public System.String PrintOut
            {
                get
                {
                    System.String data = entity.PrintOut;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PrintOut = null;
                    else entity.PrintOut = Convert.ToString(value);
                }
            }
            public System.String TotAmtIncurred
            {
                get
                {
                    System.Decimal? data = entity.TotAmtIncurred;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TotAmtIncurred = null;
                    else entity.TotAmtIncurred = Convert.ToDecimal(value);
                }
            }
            public System.String TotAmtApproved
            {
                get
                {
                    System.Decimal? data = entity.TotAmtApproved;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TotAmtApproved = null;
                    else entity.TotAmtApproved = Convert.ToDecimal(value);
                }
            }
            public System.String TotAmtNotApproved
            {
                get
                {
                    System.Decimal? data = entity.TotAmtNotApproved;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TotAmtNotApproved = null;
                    else entity.TotAmtNotApproved = Convert.ToDecimal(value);
                }
            }
            public System.String IsClosed
            {
                get
                {
                    System.Boolean? data = entity.IsClosed;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IsClosed = null;
                    else entity.IsClosed = Convert.ToBoolean(value);
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

            private esAdmedikaClaimTransaction entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esAdmedikaClaimTransactionQuery query)
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
                throw new Exception("esAdmedikaClaimTransaction can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class AdmedikaClaimTransaction : esAdmedikaClaimTransaction
    {
    }

    [Serializable]
    abstract public class esAdmedikaClaimTransactionQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return AdmedikaClaimTransactionMetadata.Meta();
            }
        }

        public esQueryItem AdmedikaClaimTransactionID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID, esSystemType.Int64);
            }
        }
        public esQueryItem RegistrationNo
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.RegistrationNo, esSystemType.String);
            }
        }
        public esQueryItem TransactionNo
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.TransactionNo, esSystemType.String);
            }
        }
        public esQueryItem PatientID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.PatientID, esSystemType.String);
            }
        }
        public esQueryItem GuarantorID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.GuarantorID, esSystemType.String);
            }
        }
        public esQueryItem CardNo
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.CardNo, esSystemType.String);
            }
        }
        public esQueryItem ClID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.ClID, esSystemType.String);
            }
        }
        public esQueryItem CovID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.CovID, esSystemType.String);
            }
        }
        public esQueryItem CovCode
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.CovCode, esSystemType.String);
            }
        }
        public esQueryItem PlanCode
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.PlanCode, esSystemType.String);
            }
        }
        public esQueryItem DiagnosisCodeList
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.DiagnosisCodeList, esSystemType.String);
            }
        }
        public esQueryItem ClStatus
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.ClStatus, esSystemType.String);
            }
        }
        public esQueryItem ClDesc
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.ClDesc, esSystemType.String);
            }
        }
        public esQueryItem ServiceID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.ServiceID, esSystemType.String);
            }
        }
        public esQueryItem PrintOut
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.PrintOut, esSystemType.String);
            }
        }
        public esQueryItem TotAmtIncurred
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtIncurred, esSystemType.Decimal);
            }
        }
        public esQueryItem TotAmtApproved
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtApproved, esSystemType.Decimal);
            }
        }
        public esQueryItem TotAmtNotApproved
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtNotApproved, esSystemType.Decimal);
            }
        }
        public esQueryItem IsClosed
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.IsClosed, esSystemType.Boolean);
            }
        }
        public esQueryItem CreatedDateTime
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.CreatedDateTime, esSystemType.DateTime);
            }
        }
        public esQueryItem CreatedByUserID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.CreatedByUserID, esSystemType.String);
            }
        }
        public esQueryItem LastUpdateDateTime
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
            }
        }
        public esQueryItem LastUpdateByUserID
        {
            get
            {
                return new esQueryItem(this, AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
            }
        }
    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("AdmedikaClaimTransactionCollection")]
    public partial class AdmedikaClaimTransactionCollection : esAdmedikaClaimTransactionCollection, IEnumerable<AdmedikaClaimTransaction>
    {
        public AdmedikaClaimTransactionCollection()
        {

        }

        public static implicit operator List<AdmedikaClaimTransaction>(AdmedikaClaimTransactionCollection coll)
        {
            List<AdmedikaClaimTransaction> list = new List<AdmedikaClaimTransaction>();

            foreach (AdmedikaClaimTransaction emp in coll)
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
                return AdmedikaClaimTransactionMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AdmedikaClaimTransactionQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new AdmedikaClaimTransaction(row);
        }

        override protected esEntity CreateEntity()
        {
            return new AdmedikaClaimTransaction();
        }

        #endregion

        [BrowsableAttribute(false)]
        public AdmedikaClaimTransactionQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AdmedikaClaimTransactionQuery();
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
        public bool Load(AdmedikaClaimTransactionQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public AdmedikaClaimTransaction AddNew()
        {
            AdmedikaClaimTransaction entity = base.AddNewEntity() as AdmedikaClaimTransaction;

            return entity;
        }
        public AdmedikaClaimTransaction FindByPrimaryKey(String standardReferenceID)
        {
            return base.FindByPrimaryKey(standardReferenceID) as AdmedikaClaimTransaction;
        }

        #region IEnumerable< AdmedikaClaimTransaction> Members

        IEnumerator<AdmedikaClaimTransaction> IEnumerable<AdmedikaClaimTransaction>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as AdmedikaClaimTransaction;
            }
        }

        #endregion

        private AdmedikaClaimTransactionQuery query;
    }


    /// <summary>
    /// Encapsulates the 'AdmedikaClaimTransaction' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("AdmedikaClaimTransaction ({StandardReferenceID})")]
    [Serializable]
    public partial class AdmedikaClaimTransaction : esAdmedikaClaimTransaction
    {
        public AdmedikaClaimTransaction()
        {
        }

        public AdmedikaClaimTransaction(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return AdmedikaClaimTransactionMetadata.Meta();
            }
        }

        override protected esAdmedikaClaimTransactionQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new AdmedikaClaimTransactionQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public AdmedikaClaimTransactionQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new AdmedikaClaimTransactionQuery();
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
        public bool Load(AdmedikaClaimTransactionQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private AdmedikaClaimTransactionQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class AdmedikaClaimTransactionQuery : esAdmedikaClaimTransactionQuery
    {
        public AdmedikaClaimTransactionQuery()
        {

        }

        public AdmedikaClaimTransactionQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "AdmedikaClaimTransactionQuery";
        }
    }

    [Serializable]
    public partial class AdmedikaClaimTransactionMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected AdmedikaClaimTransactionMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.AdmedikaClaimTransactionID, 0, typeof(System.Int64), esSystemType.Int64);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.AdmedikaClaimTransactionID;
            c.IsInPrimaryKey = true;
            c.NumericPrecision = 19;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.RegistrationNo, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.RegistrationNo;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.TransactionNo, 2, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.TransactionNo;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.PatientID, 3, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.PatientID;
            c.CharacterMaxLength = 15;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.GuarantorID, 4, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.GuarantorID;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.CardNo, 5, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.CardNo;
            c.CharacterMaxLength = 32;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.ClID, 6, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.ClID;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.CovID, 7, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.CovID;
            c.CharacterMaxLength = 5;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.CovCode, 8, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.CovCode;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.PlanCode, 9, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.PlanCode;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.DiagnosisCodeList, 10, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.DiagnosisCodeList;
            c.CharacterMaxLength = 500;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.ClStatus, 11, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.ClStatus;
            c.CharacterMaxLength = 5;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.ClDesc, 12, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.ClDesc;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.ServiceID, 13, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.ServiceID;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.PrintOut, 14, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.PrintOut;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtIncurred, 15, typeof(System.Decimal), esSystemType.Decimal);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.TotAmtIncurred;
            c.NumericPrecision = 18;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtApproved, 16, typeof(System.Decimal), esSystemType.Decimal);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.TotAmtApproved;
            c.NumericPrecision = 18;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.TotAmtNotApproved, 17, typeof(System.Decimal), esSystemType.Decimal);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.TotAmtNotApproved;
            c.NumericPrecision = 18;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.IsClosed, 18, typeof(System.Boolean), esSystemType.Boolean);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.IsClosed;
            c.HasDefault = true;
            c.Default = @"((0))";
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedDateTime, 19, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.CreatedDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.CreatedByUserID, 20, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.CreatedByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateDateTime, 21, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.LastUpdateDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(AdmedikaClaimTransactionMetadata.ColumnNames.LastUpdateByUserID, 22, typeof(System.String), esSystemType.String);
            c.PropertyName = AdmedikaClaimTransactionMetadata.PropertyNames.LastUpdateByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);
        }
        #endregion

        static public AdmedikaClaimTransactionMetadata Meta()
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
            get { return base._columns; }
        }

        #region ColumnNames
        public class ColumnNames
        {
            public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
            public const string RegistrationNo = "RegistrationNo";
            public const string TransactionNo = "TransactionNo";
            public const string PatientID = "PatientID";
            public const string GuarantorID = "GuarantorID";
            public const string CardNo = "CardNo";
            public const string ClID = "ClID";
            public const string CovID = "CovID";
            public const string CovCode = "CovCode";
            public const string PlanCode = "PlanCode";
            public const string DiagnosisCodeList = "DiagnosisCodeList";
            public const string ClStatus = "ClStatus";
            public const string ClDesc = "ClDesc";
            public const string ServiceID = "ServiceID";
            public const string PrintOut = "PrintOut";
            public const string TotAmtIncurred = "TotAmtIncurred";
            public const string TotAmtApproved = "TotAmtApproved";
            public const string TotAmtNotApproved = "TotAmtNotApproved";
            public const string IsClosed = "IsClosed";
            public const string CreatedDateTime = "CreatedDateTime";
            public const string CreatedByUserID = "CreatedByUserID";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string AdmedikaClaimTransactionID = "AdmedikaClaimTransactionID";
            public const string RegistrationNo = "RegistrationNo";
            public const string TransactionNo = "TransactionNo";
            public const string PatientID = "PatientID";
            public const string GuarantorID = "GuarantorID";
            public const string CardNo = "CardNo";
            public const string ClID = "ClID";
            public const string CovID = "CovID";
            public const string CovCode = "CovCode";
            public const string PlanCode = "PlanCode";
            public const string DiagnosisCodeList = "DiagnosisCodeList";
            public const string ClStatus = "ClStatus";
            public const string ClDesc = "ClDesc";
            public const string ServiceID = "ServiceID";
            public const string PrintOut = "PrintOut";
            public const string TotAmtIncurred = "TotAmtIncurred";
            public const string TotAmtApproved = "TotAmtApproved";
            public const string TotAmtNotApproved = "TotAmtNotApproved";
            public const string IsClosed = "IsClosed";
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
            lock (typeof(AdmedikaClaimTransactionMetadata))
            {
                if (AdmedikaClaimTransactionMetadata.mapDelegates == null)
                {
                    AdmedikaClaimTransactionMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (AdmedikaClaimTransactionMetadata.meta == null)
                {
                    AdmedikaClaimTransactionMetadata.meta = new AdmedikaClaimTransactionMetadata();
                }

                MapToMeta mapMethod = new MapToMeta(meta.esDefault);
                mapDelegates.Add("esDefault", mapMethod);
                mapMethod("esDefault");
            }
            return 0;
        }

        private esProviderSpecificMetadata esDefault(string mapName)
        {
            if (!_providerMetadataMaps.ContainsKey(mapName))
            {
                esProviderSpecificMetadata meta = new esProviderSpecificMetadata();

                meta.AddTypeMap("AdmedikaClaimTransactionID", new esTypeMap("bigint", "System.Int64"));
                meta.AddTypeMap("RegistrationNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("TransactionNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PatientID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("GuarantorID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CardNo", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ClID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CovID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("CovCode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PlanCode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("DiagnosisCodeList", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ClStatus", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ClDesc", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PrintOut", new esTypeMap("nvarchar", "System.String"));
                meta.AddTypeMap("TotAmtIncurred", new esTypeMap("decimal", "System.Decimal"));
                meta.AddTypeMap("TotAmtApproved", new esTypeMap("decimal", "System.Decimal"));
                meta.AddTypeMap("TotAmtNotApproved", new esTypeMap("decimal", "System.Decimal"));
                meta.AddTypeMap("IsClosed", new esTypeMap("bit", "System.Boolean"));
                meta.AddTypeMap("CreatedDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("CreatedByUserID", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));


                meta.Source = "AdmedikaClaimTransaction";
                meta.Destination = "AdmedikaClaimTransaction";
                meta.spInsert = "proc_AdmedikaClaimTransactionInsert";
                meta.spUpdate = "proc_AdmedikaClaimTransactionUpdate";
                meta.spDelete = "proc_AdmedikaClaimTransactionDelete";
                meta.spLoadAll = "proc_AdmedikaClaimTransactionLoadAll";
                meta.spLoadByPrimaryKey = "proc_AdmedikaClaimTransactionLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private AdmedikaClaimTransactionMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}