/*
===============================================================================
                       Persistence Layer and Business Objects  
===============================================================================
                       Date Generated       : 3/6/2026 10:02:01 AM
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
    abstract public class esBpjsRujukanSatuSehatCollection : esEntityCollectionWAuditLog
    {
        public esBpjsRujukanSatuSehatCollection()
        {

        }


        protected override string GetCollectionName()
        {
            return "BpjsRujukanSatuSehatCollection";
        }

        #region Query Logic
        protected void InitQuery(esBpjsRujukanSatuSehatQuery query)
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
            this.InitQuery(query as esBpjsRujukanSatuSehatQuery);
        }
        #endregion

        virtual public BpjsRujukanSatuSehat DetachEntity(BpjsRujukanSatuSehat entity)
        {
            return base.DetachEntity(entity) as BpjsRujukanSatuSehat;
        }

        virtual public BpjsRujukanSatuSehat AttachEntity(BpjsRujukanSatuSehat entity)
        {
            return base.AttachEntity(entity) as BpjsRujukanSatuSehat;
        }

        virtual public void Combine(BpjsRujukanSatuSehatCollection collection)
        {
            base.Combine(collection);
        }

        new public BpjsRujukanSatuSehat this[int index]
        {
            get
            {
                return base[index] as BpjsRujukanSatuSehat;
            }
        }

        public override Type GetEntityType()
        {
            return typeof(BpjsRujukanSatuSehat);
        }
    }

    [Serializable]
    abstract public class esBpjsRujukanSatuSehat : esEntityWAuditLog
    {
        /// <summary>
        /// Used internally by the entity's DynamicQuery mechanism.
        /// </summary>
        virtual protected esBpjsRujukanSatuSehatQuery GetDynamicQuery()
        {
            return null;
        }

        public esBpjsRujukanSatuSehat()
        {
        }

        public esBpjsRujukanSatuSehat(DataRow row)
            : base(row)
        {
        }


        #region LoadByPrimaryKey
        public virtual bool LoadByPrimaryKey(String noSep, String noRujukan)
        {
            if (this.es.Connection.SqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(noSep, noRujukan);
            else
                return LoadByPrimaryKeyStoredProcedure(noSep, noRujukan);
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
        public virtual bool LoadByPrimaryKey(esSqlAccessType sqlAccessType, String noSep, String noRujukan)
        {
            if (sqlAccessType == esSqlAccessType.DynamicSQL)
                return LoadByPrimaryKeyDynamic(noSep, noRujukan);
            else
                return LoadByPrimaryKeyStoredProcedure(noSep, noRujukan);
        }

        private bool LoadByPrimaryKeyDynamic(String noSep, String noRujukan)
        {
            esBpjsRujukanSatuSehatQuery query = this.GetDynamicQuery();
            query.Where(query.NoSep == noSep, query.NoRujukan == noRujukan);
            return query.Load();
        }

        private bool LoadByPrimaryKeyStoredProcedure(String noSep, String noRujukan)
        {
            esParameters parms = new esParameters();
            parms.Add("noSep", noSep);
            parms.Add("NoRujukan", noRujukan);
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
                        case "NoSep": this.str.NoSep = (string)value; break;
                        case "NoRujukan": this.str.NoRujukan = (string)value; break;
                        case "TglRujukan": this.str.TglRujukan = (string)value; break;
                        case "TglRencana": this.str.TglRencana = (string)value; break;
                        case "PpkDirujuk": this.str.PpkDirujuk = (string)value; break;
                        case "NamaPpkDirujuk": this.str.NamaPpkDirujuk = (string)value; break;
                        case "JnsPelayanan": this.str.JnsPelayanan = (string)value; break;
                        case "Catatan": this.str.Catatan = (string)value; break;
                        case "DiagRujukan": this.str.DiagRujukan = (string)value; break;
                        case "TipeRujukan": this.str.TipeRujukan = (string)value; break;
                        case "PoliRujukan": this.str.PoliRujukan = (string)value; break;
                        case "NamaPoliRujukan": this.str.NamaPoliRujukan = (string)value; break;
                        case "User": this.str.User = (string)value; break;
                        case "KodeFaskesSatuSehat": this.str.KodeFaskesSatuSehat = (string)value; break;
                        case "IdPasienSatuSehat": this.str.IdPasienSatuSehat = (string)value; break;
                        case "KdppkSatuSehatTujuanRujukan": this.str.KdppkSatuSehatTujuanRujukan = (string)value; break;
                        case "KdDokterSatuSehat": this.str.KdDokterSatuSehat = (string)value; break;
                        case "EncounterReference": this.str.EncounterReference = (string)value; break;
                        case "PatientInstruction": this.str.PatientInstruction = (string)value; break;
                        case "KeteranganRujukan": this.str.KeteranganRujukan = (string)value; break;
                        case "KodePropinsi": this.str.KodePropinsi = (string)value; break;
                        case "NamaPropinsi": this.str.NamaPropinsi = (string)value; break;
                        case "KodeKabupaten": this.str.KodeKabupaten = (string)value; break;
                        case "NamaKabupaten": this.str.NamaKabupaten = (string)value; break;
                        case "KriteriaRujukanJson": this.str.KriteriaRujukanJson = (string)value; break;
                        case "NoRujukanSatuSehat": this.str.NoRujukanSatuSehat = (string)value; break;
                        case "ServiceRequestId": this.str.ServiceRequestId = (string)value; break;
                        case "AsalRujukanKode": this.str.AsalRujukanKode = (string)value; break;
                        case "AsalRujukanNama": this.str.AsalRujukanNama = (string)value; break;
                        case "DiagnosaKode": this.str.DiagnosaKode = (string)value; break;
                        case "DiagnosaNama": this.str.DiagnosaNama = (string)value; break;
                        case "PesertaAsuransi": this.str.PesertaAsuransi = (string)value; break;
                        case "PesertaHakKelas": this.str.PesertaHakKelas = (string)value; break;
                        case "PesertaJenis": this.str.PesertaJenis = (string)value; break;
                        case "PesertaKelamin": this.str.PesertaKelamin = (string)value; break;
                        case "PesertaNama": this.str.PesertaNama = (string)value; break;
                        case "PesertaNoKartu": this.str.PesertaNoKartu = (string)value; break;
                        case "PesertaNoMR": this.str.PesertaNoMR = (string)value; break;
                        case "PesertaTglLahir": this.str.PesertaTglLahir = (string)value; break;
                        case "PoliTujuanKode": this.str.PoliTujuanKode = (string)value; break;
                        case "PoliTujuanNama": this.str.PoliTujuanNama = (string)value; break;
                        case "TujuanRujukanKode": this.str.TujuanRujukanKode = (string)value; break;
                        case "TujuanRujukanNama": this.str.TujuanRujukanNama = (string)value; break;
                        case "BpjsResponseCode": this.str.BpjsResponseCode = (string)value; break;
                        case "BpjsResponseMessage": this.str.BpjsResponseMessage = (string)value; break;
                        case "RequestJson": this.str.RequestJson = (string)value; break;
                        case "ResponseJson": this.str.ResponseJson = (string)value; break;
                        case "LastUpdateDateTime": this.str.LastUpdateDateTime = (string)value; break;
                        case "LastUpdateByUserID": this.str.LastUpdateByUserID = (string)value; break;
                    }
                }
                else
                {
                    switch (name)
                    {
                        case "tglRujukan":

                            if (value == null || value is System.DateTime)
                                this.TglRujukan = (System.DateTime?)value;
                            break;
                        case "tglRencana":

                            if (value == null || value is System.DateTime)
                                this.TglRencana = (System.DateTime?)value;
                            break;
                        case "pesertaTglLahir":

                            if (value == null || value is System.DateTime)
                                this.PesertaTglLahir = (System.DateTime?)value;
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
        /// Maps to BpjsRujukanSatuSehat.noSep
        /// </summary>
        virtual public System.String NoSep
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoSep);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoSep, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.NoRujukan
        /// </summary>
        virtual public System.String NoRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.tglRujukan
        /// </summary>
        virtual public System.DateTime? TglRujukan
        {
            get
            {
                return base.GetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRujukan);
            }

            set
            {
                base.SetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.tglRencana
        /// </summary>
        virtual public System.DateTime? TglRencana
        {
            get
            {
                return base.GetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRencana);
            }

            set
            {
                base.SetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRencana, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.ppkDirujuk
        /// </summary>
        virtual public System.String PpkDirujuk
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PpkDirujuk);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PpkDirujuk, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.namaPpkDirujuk
        /// </summary>
        virtual public System.String NamaPpkDirujuk
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPpkDirujuk);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPpkDirujuk, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.jnsPelayanan
        /// </summary>
        virtual public System.String JnsPelayanan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.JnsPelayanan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.JnsPelayanan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.catatan
        /// </summary>
        virtual public System.String Catatan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.Catatan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.Catatan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.diagRujukan
        /// </summary>
        virtual public System.String DiagRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.tipeRujukan
        /// </summary>
        virtual public System.String TipeRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TipeRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TipeRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.poliRujukan
        /// </summary>
        virtual public System.String PoliRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.namaPoliRujukan
        /// </summary>
        virtual public System.String NamaPoliRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPoliRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPoliRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.user
        /// </summary>
        virtual public System.String User
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.User);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.User, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.kodeFaskesSatuSehat
        /// </summary>
        virtual public System.String KodeFaskesSatuSehat
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeFaskesSatuSehat);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeFaskesSatuSehat, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.idPasienSatuSehat
        /// </summary>
        virtual public System.String IdPasienSatuSehat
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.IdPasienSatuSehat);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.IdPasienSatuSehat, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.kdppkSatuSehatTujuanRujukan
        /// </summary>
        virtual public System.String KdppkSatuSehatTujuanRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KdppkSatuSehatTujuanRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KdppkSatuSehatTujuanRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.kdDokterSatuSehat
        /// </summary>
        virtual public System.String KdDokterSatuSehat
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KdDokterSatuSehat);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KdDokterSatuSehat, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.EncounterReference
        /// </summary>
        virtual public System.String EncounterReference
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.EncounterReference);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.EncounterReference, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.patientInstruction
        /// </summary>
        virtual public System.String PatientInstruction
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PatientInstruction);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PatientInstruction, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.keteranganRujukan
        /// </summary>
        virtual public System.String KeteranganRujukan
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KeteranganRujukan);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KeteranganRujukan, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.kodePropinsi
        /// </summary>
        virtual public System.String KodePropinsi
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodePropinsi);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodePropinsi, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.namaPropinsi
        /// </summary>
        virtual public System.String NamaPropinsi
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPropinsi);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPropinsi, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.kodeKabupaten
        /// </summary>
        virtual public System.String KodeKabupaten
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeKabupaten);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeKabupaten, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.namaKabupaten
        /// </summary>
        virtual public System.String NamaKabupaten
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaKabupaten);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaKabupaten, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.KriteriaRujukanJson
        /// </summary>
        virtual public System.String KriteriaRujukanJson
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KriteriaRujukanJson);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.KriteriaRujukanJson, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.noRujukanSatuSehat
        /// </summary>
        virtual public System.String NoRujukanSatuSehat
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukanSatuSehat);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukanSatuSehat, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.serviceRequestId
        /// </summary>
        virtual public System.String ServiceRequestId
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.ServiceRequestId);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.ServiceRequestId, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.asalRujukanKode
        /// </summary>
        virtual public System.String AsalRujukanKode
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanKode);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanKode, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.asalRujukanNama
        /// </summary>
        virtual public System.String AsalRujukanNama
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanNama);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanNama, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.diagnosaKode
        /// </summary>
        virtual public System.String DiagnosaKode
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaKode);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaKode, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.diagnosaNama
        /// </summary>
        virtual public System.String DiagnosaNama
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaNama);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaNama, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaAsuransi
        /// </summary>
        virtual public System.String PesertaAsuransi
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaAsuransi);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaAsuransi, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaHakKelas
        /// </summary>
        virtual public System.String PesertaHakKelas
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaHakKelas);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaHakKelas, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaJenis
        /// </summary>
        virtual public System.String PesertaJenis
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaJenis);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaJenis, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaKelamin
        /// </summary>
        virtual public System.String PesertaKelamin
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaKelamin);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaKelamin, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaNama
        /// </summary>
        virtual public System.String PesertaNama
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNama);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNama, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaNoKartu
        /// </summary>
        virtual public System.String PesertaNoKartu
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoKartu);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoKartu, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaNoMR
        /// </summary>
        virtual public System.String PesertaNoMR
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoMR);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoMR, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.pesertaTglLahir
        /// </summary>
        virtual public System.DateTime? PesertaTglLahir
        {
            get
            {
                return base.GetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaTglLahir);
            }

            set
            {
                base.SetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaTglLahir, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.poliTujuanKode
        /// </summary>
        virtual public System.String PoliTujuanKode
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanKode);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanKode, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.poliTujuanNama
        /// </summary>
        virtual public System.String PoliTujuanNama
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanNama);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanNama, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.tujuanRujukanKode
        /// </summary>
        virtual public System.String TujuanRujukanKode
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanKode);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanKode, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.tujuanRujukanNama
        /// </summary>
        virtual public System.String TujuanRujukanNama
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanNama);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanNama, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.bpjsResponseCode
        /// </summary>
        virtual public System.String BpjsResponseCode
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseCode);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseCode, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.bpjsResponseMessage
        /// </summary>
        virtual public System.String BpjsResponseMessage
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseMessage);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseMessage, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.RequestJson
        /// </summary>
        virtual public System.String RequestJson
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.RequestJson);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.RequestJson, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.ResponseJson
        /// </summary>
        virtual public System.String ResponseJson
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.ResponseJson);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.ResponseJson, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.LastUpdateDateTime
        /// </summary>
        virtual public System.DateTime? LastUpdateDateTime
        {
            get
            {
                return base.GetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateDateTime);
            }

            set
            {
                base.SetSystemDateTime(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateDateTime, value);
            }
        }
        /// <summary>
        /// Maps to BpjsRujukanSatuSehat.LastUpdateByUserID
        /// </summary>
        virtual public System.String LastUpdateByUserID
        {
            get
            {
                return base.GetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateByUserID);
            }

            set
            {
                base.SetSystemString(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateByUserID, value);
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
            public esStrings(esBpjsRujukanSatuSehat entity)
            {
                this.entity = entity;
            }
            public System.String NoSep
            {
                get
                {
                    System.String data = entity.NoSep;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NoSep = null;
                    else entity.NoSep = Convert.ToString(value);
                }
            }
            public System.String NoRujukan
            {
                get
                {
                    System.String data = entity.NoRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NoRujukan = null;
                    else entity.NoRujukan = Convert.ToString(value);
                }
            }
            public System.String TglRujukan
            {
                get
                {
                    System.DateTime? data = entity.TglRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TglRujukan = null;
                    else entity.TglRujukan = Convert.ToDateTime(value);
                }
            }
            public System.String TglRencana
            {
                get
                {
                    System.DateTime? data = entity.TglRencana;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TglRencana = null;
                    else entity.TglRencana = Convert.ToDateTime(value);
                }
            }
            public System.String PpkDirujuk
            {
                get
                {
                    System.String data = entity.PpkDirujuk;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PpkDirujuk = null;
                    else entity.PpkDirujuk = Convert.ToString(value);
                }
            }
            public System.String NamaPpkDirujuk
            {
                get
                {
                    System.String data = entity.NamaPpkDirujuk;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NamaPpkDirujuk = null;
                    else entity.NamaPpkDirujuk = Convert.ToString(value);
                }
            }
            public System.String JnsPelayanan
            {
                get
                {
                    System.String data = entity.JnsPelayanan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.JnsPelayanan = null;
                    else entity.JnsPelayanan = Convert.ToString(value);
                }
            }
            public System.String Catatan
            {
                get
                {
                    System.String data = entity.Catatan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.Catatan = null;
                    else entity.Catatan = Convert.ToString(value);
                }
            }
            public System.String DiagRujukan
            {
                get
                {
                    System.String data = entity.DiagRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DiagRujukan = null;
                    else entity.DiagRujukan = Convert.ToString(value);
                }
            }
            public System.String TipeRujukan
            {
                get
                {
                    System.String data = entity.TipeRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TipeRujukan = null;
                    else entity.TipeRujukan = Convert.ToString(value);
                }
            }
            public System.String PoliRujukan
            {
                get
                {
                    System.String data = entity.PoliRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PoliRujukan = null;
                    else entity.PoliRujukan = Convert.ToString(value);
                }
            }
            public System.String NamaPoliRujukan
            {
                get
                {
                    System.String data = entity.NamaPoliRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NamaPoliRujukan = null;
                    else entity.NamaPoliRujukan = Convert.ToString(value);
                }
            }
            public System.String User
            {
                get
                {
                    System.String data = entity.User;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.User = null;
                    else entity.User = Convert.ToString(value);
                }
            }
            public System.String KodeFaskesSatuSehat
            {
                get
                {
                    System.String data = entity.KodeFaskesSatuSehat;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KodeFaskesSatuSehat = null;
                    else entity.KodeFaskesSatuSehat = Convert.ToString(value);
                }
            }
            public System.String IdPasienSatuSehat
            {
                get
                {
                    System.String data = entity.IdPasienSatuSehat;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.IdPasienSatuSehat = null;
                    else entity.IdPasienSatuSehat = Convert.ToString(value);
                }
            }
            public System.String KdppkSatuSehatTujuanRujukan
            {
                get
                {
                    System.String data = entity.KdppkSatuSehatTujuanRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KdppkSatuSehatTujuanRujukan = null;
                    else entity.KdppkSatuSehatTujuanRujukan = Convert.ToString(value);
                }
            }
            public System.String KdDokterSatuSehat
            {
                get
                {
                    System.String data = entity.KdDokterSatuSehat;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KdDokterSatuSehat = null;
                    else entity.KdDokterSatuSehat = Convert.ToString(value);
                }
            }
            public System.String EncounterReference
            {
                get
                {
                    System.String data = entity.EncounterReference;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.EncounterReference = null;
                    else entity.EncounterReference = Convert.ToString(value);
                }
            }
            public System.String PatientInstruction
            {
                get
                {
                    System.String data = entity.PatientInstruction;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PatientInstruction = null;
                    else entity.PatientInstruction = Convert.ToString(value);
                }
            }
            public System.String KeteranganRujukan
            {
                get
                {
                    System.String data = entity.KeteranganRujukan;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KeteranganRujukan = null;
                    else entity.KeteranganRujukan = Convert.ToString(value);
                }
            }
            public System.String KodePropinsi
            {
                get
                {
                    System.String data = entity.KodePropinsi;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KodePropinsi = null;
                    else entity.KodePropinsi = Convert.ToString(value);
                }
            }
            public System.String NamaPropinsi
            {
                get
                {
                    System.String data = entity.NamaPropinsi;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NamaPropinsi = null;
                    else entity.NamaPropinsi = Convert.ToString(value);
                }
            }
            public System.String KodeKabupaten
            {
                get
                {
                    System.String data = entity.KodeKabupaten;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KodeKabupaten = null;
                    else entity.KodeKabupaten = Convert.ToString(value);
                }
            }
            public System.String NamaKabupaten
            {
                get
                {
                    System.String data = entity.NamaKabupaten;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NamaKabupaten = null;
                    else entity.NamaKabupaten = Convert.ToString(value);
                }
            }
            public System.String KriteriaRujukanJson
            {
                get
                {
                    System.String data = entity.KriteriaRujukanJson;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.KriteriaRujukanJson = null;
                    else entity.KriteriaRujukanJson = Convert.ToString(value);
                }
            }
            public System.String NoRujukanSatuSehat
            {
                get
                {
                    System.String data = entity.NoRujukanSatuSehat;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.NoRujukanSatuSehat = null;
                    else entity.NoRujukanSatuSehat = Convert.ToString(value);
                }
            }
            public System.String ServiceRequestId
            {
                get
                {
                    System.String data = entity.ServiceRequestId;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ServiceRequestId = null;
                    else entity.ServiceRequestId = Convert.ToString(value);
                }
            }
            public System.String AsalRujukanKode
            {
                get
                {
                    System.String data = entity.AsalRujukanKode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.AsalRujukanKode = null;
                    else entity.AsalRujukanKode = Convert.ToString(value);
                }
            }
            public System.String AsalRujukanNama
            {
                get
                {
                    System.String data = entity.AsalRujukanNama;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.AsalRujukanNama = null;
                    else entity.AsalRujukanNama = Convert.ToString(value);
                }
            }
            public System.String DiagnosaKode
            {
                get
                {
                    System.String data = entity.DiagnosaKode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DiagnosaKode = null;
                    else entity.DiagnosaKode = Convert.ToString(value);
                }
            }
            public System.String DiagnosaNama
            {
                get
                {
                    System.String data = entity.DiagnosaNama;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.DiagnosaNama = null;
                    else entity.DiagnosaNama = Convert.ToString(value);
                }
            }
            public System.String PesertaAsuransi
            {
                get
                {
                    System.String data = entity.PesertaAsuransi;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaAsuransi = null;
                    else entity.PesertaAsuransi = Convert.ToString(value);
                }
            }
            public System.String PesertaHakKelas
            {
                get
                {
                    System.String data = entity.PesertaHakKelas;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaHakKelas = null;
                    else entity.PesertaHakKelas = Convert.ToString(value);
                }
            }
            public System.String PesertaJenis
            {
                get
                {
                    System.String data = entity.PesertaJenis;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaJenis = null;
                    else entity.PesertaJenis = Convert.ToString(value);
                }
            }
            public System.String PesertaKelamin
            {
                get
                {
                    System.String data = entity.PesertaKelamin;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaKelamin = null;
                    else entity.PesertaKelamin = Convert.ToString(value);
                }
            }
            public System.String PesertaNama
            {
                get
                {
                    System.String data = entity.PesertaNama;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaNama = null;
                    else entity.PesertaNama = Convert.ToString(value);
                }
            }
            public System.String PesertaNoKartu
            {
                get
                {
                    System.String data = entity.PesertaNoKartu;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaNoKartu = null;
                    else entity.PesertaNoKartu = Convert.ToString(value);
                }
            }
            public System.String PesertaNoMR
            {
                get
                {
                    System.String data = entity.PesertaNoMR;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaNoMR = null;
                    else entity.PesertaNoMR = Convert.ToString(value);
                }
            }
            public System.String PesertaTglLahir
            {
                get
                {
                    System.DateTime? data = entity.PesertaTglLahir;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PesertaTglLahir = null;
                    else entity.PesertaTglLahir = Convert.ToDateTime(value);
                }
            }
            public System.String PoliTujuanKode
            {
                get
                {
                    System.String data = entity.PoliTujuanKode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PoliTujuanKode = null;
                    else entity.PoliTujuanKode = Convert.ToString(value);
                }
            }
            public System.String PoliTujuanNama
            {
                get
                {
                    System.String data = entity.PoliTujuanNama;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.PoliTujuanNama = null;
                    else entity.PoliTujuanNama = Convert.ToString(value);
                }
            }
            public System.String TujuanRujukanKode
            {
                get
                {
                    System.String data = entity.TujuanRujukanKode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TujuanRujukanKode = null;
                    else entity.TujuanRujukanKode = Convert.ToString(value);
                }
            }
            public System.String TujuanRujukanNama
            {
                get
                {
                    System.String data = entity.TujuanRujukanNama;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.TujuanRujukanNama = null;
                    else entity.TujuanRujukanNama = Convert.ToString(value);
                }
            }
            public System.String BpjsResponseCode
            {
                get
                {
                    System.String data = entity.BpjsResponseCode;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.BpjsResponseCode = null;
                    else entity.BpjsResponseCode = Convert.ToString(value);
                }
            }
            public System.String BpjsResponseMessage
            {
                get
                {
                    System.String data = entity.BpjsResponseMessage;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.BpjsResponseMessage = null;
                    else entity.BpjsResponseMessage = Convert.ToString(value);
                }
            }
            public System.String RequestJson
            {
                get
                {
                    System.String data = entity.RequestJson;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.RequestJson = null;
                    else entity.RequestJson = Convert.ToString(value);
                }
            }
            public System.String ResponseJson
            {
                get
                {
                    System.String data = entity.ResponseJson;
                    return (data == null) ? String.Empty : Convert.ToString(data);
                }

                set
                {
                    if (value == null || value.Length == 0) entity.ResponseJson = null;
                    else entity.ResponseJson = Convert.ToString(value);
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
            private esBpjsRujukanSatuSehat entity;
        }
        #endregion

        #region Query Logic
        protected void InitQuery(esBpjsRujukanSatuSehatQuery query)
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
                throw new Exception("esBpjsRujukanSatuSehat can only hold one record of data");
            }

            return dataFound;
        }
        #endregion

        [NonSerialized]
        private esStrings esstrings;
    }


    public partial class BpjsRujukanSatuSehat : esBpjsRujukanSatuSehat
    {
    }

    [Serializable]
    abstract public class esBpjsRujukanSatuSehatQuery : esDynamicQuery
    {

        override protected IMetadata Meta
        {
            get
            {
                return BpjsRujukanSatuSehatMetadata.Meta();
            }
        }

        public esQueryItem NoSep
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NoSep, esSystemType.String);
            }
        }

        public esQueryItem NoRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukan, esSystemType.String);
            }
        }

        public esQueryItem TglRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.TglRujukan, esSystemType.DateTime);
            }
        }

        public esQueryItem TglRencana
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.TglRencana, esSystemType.DateTime);
            }
        }

        public esQueryItem PpkDirujuk
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PpkDirujuk, esSystemType.String);
            }
        }

        public esQueryItem NamaPpkDirujuk
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPpkDirujuk, esSystemType.String);
            }
        }

        public esQueryItem JnsPelayanan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.JnsPelayanan, esSystemType.String);
            }
        }

        public esQueryItem Catatan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.Catatan, esSystemType.String);
            }
        }

        public esQueryItem DiagRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.DiagRujukan, esSystemType.String);
            }
        }

        public esQueryItem TipeRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.TipeRujukan, esSystemType.String);
            }
        }

        public esQueryItem PoliRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PoliRujukan, esSystemType.String);
            }
        }

        public esQueryItem NamaPoliRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPoliRujukan, esSystemType.String);
            }
        }

        public esQueryItem User
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.User, esSystemType.String);
            }
        }

        public esQueryItem KodeFaskesSatuSehat
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KodeFaskesSatuSehat, esSystemType.String);
            }
        }

        public esQueryItem IdPasienSatuSehat
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.IdPasienSatuSehat, esSystemType.String);
            }
        }

        public esQueryItem KdppkSatuSehatTujuanRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KdppkSatuSehatTujuanRujukan, esSystemType.String);
            }
        }

        public esQueryItem KdDokterSatuSehat
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KdDokterSatuSehat, esSystemType.String);
            }
        }

        public esQueryItem EncounterReference
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.EncounterReference, esSystemType.String);
            }
        }

        public esQueryItem PatientInstruction
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PatientInstruction, esSystemType.String);
            }
        }

        public esQueryItem KeteranganRujukan
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KeteranganRujukan, esSystemType.String);
            }
        }

        public esQueryItem KodePropinsi
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KodePropinsi, esSystemType.String);
            }
        }

        public esQueryItem NamaPropinsi
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPropinsi, esSystemType.String);
            }
        }

        public esQueryItem KodeKabupaten
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KodeKabupaten, esSystemType.String);
            }
        }

        public esQueryItem NamaKabupaten
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NamaKabupaten, esSystemType.String);
            }
        }

        public esQueryItem KriteriaRujukanJson
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.KriteriaRujukanJson, esSystemType.String);
            }
        }

        public esQueryItem NoRujukanSatuSehat
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukanSatuSehat, esSystemType.String);
            }
        }

        public esQueryItem ServiceRequestId
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.ServiceRequestId, esSystemType.String);
            }
        }

        public esQueryItem AsalRujukanKode
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanKode, esSystemType.String);
            }
        }

        public esQueryItem AsalRujukanNama
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanNama, esSystemType.String);
            }
        }

        public esQueryItem DiagnosaKode
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaKode, esSystemType.String);
            }
        }

        public esQueryItem DiagnosaNama
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaNama, esSystemType.String);
            }
        }

        public esQueryItem PesertaAsuransi
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaAsuransi, esSystemType.String);
            }
        }

        public esQueryItem PesertaHakKelas
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaHakKelas, esSystemType.String);
            }
        }

        public esQueryItem PesertaJenis
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaJenis, esSystemType.String);
            }
        }

        public esQueryItem PesertaKelamin
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaKelamin, esSystemType.String);
            }
        }

        public esQueryItem PesertaNama
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNama, esSystemType.String);
            }
        }

        public esQueryItem PesertaNoKartu
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoKartu, esSystemType.String);
            }
        }

        public esQueryItem PesertaNoMR
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoMR, esSystemType.String);
            }
        }

        public esQueryItem PesertaTglLahir
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaTglLahir, esSystemType.DateTime);
            }
        }

        public esQueryItem PoliTujuanKode
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanKode, esSystemType.String);
            }
        }

        public esQueryItem PoliTujuanNama
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanNama, esSystemType.String);
            }
        }

        public esQueryItem TujuanRujukanKode
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanKode, esSystemType.String);
            }
        }

        public esQueryItem TujuanRujukanNama
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanNama, esSystemType.String);
            }
        }

        public esQueryItem BpjsResponseCode
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseCode, esSystemType.String);
            }
        }

        public esQueryItem BpjsResponseMessage
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseMessage, esSystemType.String);
            }
        }

        public esQueryItem RequestJson
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.RequestJson, esSystemType.String);
            }
        }

        public esQueryItem ResponseJson
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.ResponseJson, esSystemType.String);
            }
        }

        public esQueryItem LastUpdateDateTime
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateDateTime, esSystemType.DateTime);
            }
        }

        public esQueryItem LastUpdateByUserID
        {
            get
            {
                return new esQueryItem(this, BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateByUserID, esSystemType.String);
            }
        }

    }

    [System.Diagnostics.DebuggerDisplay("Count = {Count}")]
    [Serializable]
    [XmlType("BpjsRujukanSatuSehatCollection")]
    public partial class BpjsRujukanSatuSehatCollection : esBpjsRujukanSatuSehatCollection, IEnumerable<BpjsRujukanSatuSehat>
    {
        public BpjsRujukanSatuSehatCollection()
        {

        }

        public static implicit operator List<BpjsRujukanSatuSehat>(BpjsRujukanSatuSehatCollection coll)
        {
            List<BpjsRujukanSatuSehat> list = new List<BpjsRujukanSatuSehat>();

            foreach (BpjsRujukanSatuSehat emp in coll)
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
                return BpjsRujukanSatuSehatMetadata.Meta();
            }
        }

        override protected esDynamicQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new BpjsRujukanSatuSehatQuery();
                this.InitQuery(query);
            }
            return this.query;
        }

        override protected esEntity CreateEntityForCollection(DataRow row)
        {
            return new BpjsRujukanSatuSehat(row);
        }

        override protected esEntity CreateEntity()
        {
            return new BpjsRujukanSatuSehat();
        }

        #endregion

        [BrowsableAttribute(false)]
        public BpjsRujukanSatuSehatQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new BpjsRujukanSatuSehatQuery();
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
        public bool Load(BpjsRujukanSatuSehatQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        /// <summary>
        /// Adds a new entity to the collection.
        /// Always calls AddNew() on the entity, in case it is overridden.
        /// </summary>
        public BpjsRujukanSatuSehat AddNew()
        {
            BpjsRujukanSatuSehat entity = base.AddNewEntity() as BpjsRujukanSatuSehat;

            return entity;
        }
        public BpjsRujukanSatuSehat FindByPrimaryKey(String noSep, String noRujukan)
        {
            return base.FindByPrimaryKey(noSep, noRujukan) as BpjsRujukanSatuSehat;
        }

        #region IEnumerable< BpjsRujukanSatuSehat> Members

        IEnumerator<BpjsRujukanSatuSehat> IEnumerable<BpjsRujukanSatuSehat>.GetEnumerator()
        {
            System.Collections.IEnumerable enumer = this as System.Collections.IEnumerable;
            System.Collections.IEnumerator iterator = enumer.GetEnumerator();

            while (iterator.MoveNext())
            {
                yield return iterator.Current as BpjsRujukanSatuSehat;
            }
        }

        #endregion

        private BpjsRujukanSatuSehatQuery query;
    }


    /// <summary>
    /// Encapsulates the 'BpjsRujukanSatuSehat' table
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("BpjsRujukanSatuSehat ({noSep, NoRujukan})")]
    [Serializable]
    public partial class BpjsRujukanSatuSehat : esBpjsRujukanSatuSehat
    {
        public BpjsRujukanSatuSehat()
        {
        }

        public BpjsRujukanSatuSehat(DataRow row)
            : base(row)
        {
        }

        #region Housekeeping methods
        override protected IMetadata Meta
        {
            get
            {
                return BpjsRujukanSatuSehatMetadata.Meta();
            }
        }

        override protected esBpjsRujukanSatuSehatQuery GetDynamicQuery()
        {
            if (this.query == null)
            {
                this.query = new BpjsRujukanSatuSehatQuery();
                this.InitQuery(query);
            }
            return this.query;
        }
        #endregion

        [BrowsableAttribute(false)]
        public BpjsRujukanSatuSehatQuery Query
        {
            get
            {
                if (this.query == null)
                {
                    this.query = new BpjsRujukanSatuSehatQuery();
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
        public bool Load(BpjsRujukanSatuSehatQuery query)
        {
            this.query = query;
            base.InitQuery(this.query);
            return this.Query.Load();
        }

        private BpjsRujukanSatuSehatQuery query;
    }

    [System.Diagnostics.DebuggerDisplay("LastQuery = {es.LastQuery}")]
    [Serializable]
    public partial class BpjsRujukanSatuSehatQuery : esBpjsRujukanSatuSehatQuery
    {
        public BpjsRujukanSatuSehatQuery()
        {

        }

        public BpjsRujukanSatuSehatQuery(string joinAlias)
        {
            this.es.JoinAlias = joinAlias;
        }

        override protected string GetQueryName()
        {
            return "BpjsRujukanSatuSehatQuery";
        }
    }

    [Serializable]
    public partial class BpjsRujukanSatuSehatMetadata : esMetadata, IMetadata
    {
        #region Protected Constructor
        protected BpjsRujukanSatuSehatMetadata()
        {
            _columns = new esColumnMetadataCollection();
            esColumnMetadata c;

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NoSep, 0, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NoSep;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukan, 1, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NoRujukan;
            c.IsInPrimaryKey = true;
            c.CharacterMaxLength = 50;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRujukan, 2, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.TglRujukan;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.TglRencana, 3, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.TglRencana;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PpkDirujuk, 4, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PpkDirujuk;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPpkDirujuk, 5, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NamaPpkDirujuk;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.JnsPelayanan, 6, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.JnsPelayanan;
            c.CharacterMaxLength = 1;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.Catatan, 7, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.Catatan;
            c.CharacterMaxLength = 2147483647;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagRujukan, 8, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.DiagRujukan;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.TipeRujukan, 9, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.TipeRujukan;
            c.CharacterMaxLength = 1;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliRujukan, 10, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PoliRujukan;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPoliRujukan, 11, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NamaPoliRujukan;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.User, 12, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.User;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeFaskesSatuSehat, 13, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KodeFaskesSatuSehat;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.IdPasienSatuSehat, 14, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.IdPasienSatuSehat;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KdppkSatuSehatTujuanRujukan, 15, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KdppkSatuSehatTujuanRujukan;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KdDokterSatuSehat, 16, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KdDokterSatuSehat;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.EncounterReference, 17, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.EncounterReference;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PatientInstruction, 18, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PatientInstruction;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KeteranganRujukan, 19, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KeteranganRujukan;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KodePropinsi, 20, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KodePropinsi;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaPropinsi, 21, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NamaPropinsi;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KodeKabupaten, 22, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KodeKabupaten;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NamaKabupaten, 23, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NamaKabupaten;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.KriteriaRujukanJson, 24, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.KriteriaRujukanJson;
            c.CharacterMaxLength = 2147483647;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukanSatuSehat, 25, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.NoRujukanSatuSehat;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.ServiceRequestId, 26, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.ServiceRequestId;
            c.CharacterMaxLength = 100;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanKode, 27, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.AsalRujukanKode;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.AsalRujukanNama, 28, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.AsalRujukanNama;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaKode, 29, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.DiagnosaKode;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.DiagnosaNama, 30, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.DiagnosaNama;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaAsuransi, 31, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaAsuransi;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaHakKelas, 32, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaHakKelas;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaJenis, 33, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaJenis;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaKelamin, 34, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaKelamin;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNama, 35, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaNama;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoKartu, 36, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaNoKartu;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaNoMR, 37, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaNoMR;
            c.CharacterMaxLength = 50;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PesertaTglLahir, 38, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PesertaTglLahir;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanKode, 39, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PoliTujuanKode;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.PoliTujuanNama, 40, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.PoliTujuanNama;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanKode, 41, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.TujuanRujukanKode;
            c.CharacterMaxLength = 20;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.TujuanRujukanNama, 42, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.TujuanRujukanNama;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseCode, 43, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.BpjsResponseCode;
            c.CharacterMaxLength = 10;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.BpjsResponseMessage, 44, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.BpjsResponseMessage;
            c.CharacterMaxLength = 255;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.RequestJson, 45, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.RequestJson;
            c.CharacterMaxLength = 2147483647;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.ResponseJson, 46, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.ResponseJson;
            c.CharacterMaxLength = 2147483647;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateDateTime, 47, typeof(System.DateTime), esSystemType.DateTime);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.LastUpdateDateTime;
            c.IsNullable = true;
            _columns.Add(c);

            c = new esColumnMetadata(BpjsRujukanSatuSehatMetadata.ColumnNames.LastUpdateByUserID, 48, typeof(System.String), esSystemType.String);
            c.PropertyName = BpjsRujukanSatuSehatMetadata.PropertyNames.LastUpdateByUserID;
            c.CharacterMaxLength = 40;
            c.IsNullable = true;
            _columns.Add(c);


        }
        #endregion

        static public BpjsRujukanSatuSehatMetadata Meta()
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
            public const string NoSep = "noSep";
            public const string NoRujukan = "NoRujukan";
            public const string TglRujukan = "tglRujukan";
            public const string TglRencana = "tglRencana";
            public const string PpkDirujuk = "ppkDirujuk";
            public const string NamaPpkDirujuk = "namaPpkDirujuk";
            public const string JnsPelayanan = "jnsPelayanan";
            public const string Catatan = "catatan";
            public const string DiagRujukan = "diagRujukan";
            public const string TipeRujukan = "tipeRujukan";
            public const string PoliRujukan = "poliRujukan";
            public const string NamaPoliRujukan = "namaPoliRujukan";
            public const string User = "user";
            public const string KodeFaskesSatuSehat = "kodeFaskesSatuSehat";
            public const string IdPasienSatuSehat = "idPasienSatuSehat";
            public const string KdppkSatuSehatTujuanRujukan = "kdppkSatuSehatTujuanRujukan";
            public const string KdDokterSatuSehat = "kdDokterSatuSehat";
            public const string EncounterReference = "EncounterReference";
            public const string PatientInstruction = "patientInstruction";
            public const string KeteranganRujukan = "keteranganRujukan";
            public const string KodePropinsi = "kodePropinsi";
            public const string NamaPropinsi = "namaPropinsi";
            public const string KodeKabupaten = "kodeKabupaten";
            public const string NamaKabupaten = "namaKabupaten";
            public const string KriteriaRujukanJson = "KriteriaRujukanJson";
            public const string NoRujukanSatuSehat = "noRujukanSatuSehat";
            public const string ServiceRequestId = "serviceRequestId";
            public const string AsalRujukanKode = "asalRujukanKode";
            public const string AsalRujukanNama = "asalRujukanNama";
            public const string DiagnosaKode = "diagnosaKode";
            public const string DiagnosaNama = "diagnosaNama";
            public const string PesertaAsuransi = "pesertaAsuransi";
            public const string PesertaHakKelas = "pesertaHakKelas";
            public const string PesertaJenis = "pesertaJenis";
            public const string PesertaKelamin = "pesertaKelamin";
            public const string PesertaNama = "pesertaNama";
            public const string PesertaNoKartu = "pesertaNoKartu";
            public const string PesertaNoMR = "pesertaNoMR";
            public const string PesertaTglLahir = "pesertaTglLahir";
            public const string PoliTujuanKode = "poliTujuanKode";
            public const string PoliTujuanNama = "poliTujuanNama";
            public const string TujuanRujukanKode = "tujuanRujukanKode";
            public const string TujuanRujukanNama = "tujuanRujukanNama";
            public const string BpjsResponseCode = "bpjsResponseCode";
            public const string BpjsResponseMessage = "bpjsResponseMessage";
            public const string RequestJson = "RequestJson";
            public const string ResponseJson = "ResponseJson";
            public const string LastUpdateDateTime = "LastUpdateDateTime";
            public const string LastUpdateByUserID = "LastUpdateByUserID";
        }
        #endregion

        #region PropertyNames
        public class PropertyNames
        {
            public const string NoSep = "NoSep";
            public const string NoRujukan = "NoRujukan";
            public const string TglRujukan = "TglRujukan";
            public const string TglRencana = "TglRencana";
            public const string PpkDirujuk = "PpkDirujuk";
            public const string NamaPpkDirujuk = "NamaPpkDirujuk";
            public const string JnsPelayanan = "JnsPelayanan";
            public const string Catatan = "Catatan";
            public const string DiagRujukan = "DiagRujukan";
            public const string TipeRujukan = "TipeRujukan";
            public const string PoliRujukan = "PoliRujukan";
            public const string NamaPoliRujukan = "NamaPoliRujukan";
            public const string User = "User";
            public const string KodeFaskesSatuSehat = "KodeFaskesSatuSehat";
            public const string IdPasienSatuSehat = "IdPasienSatuSehat";
            public const string KdppkSatuSehatTujuanRujukan = "KdppkSatuSehatTujuanRujukan";
            public const string KdDokterSatuSehat = "KdDokterSatuSehat";
            public const string EncounterReference = "EncounterReference";
            public const string PatientInstruction = "PatientInstruction";
            public const string KeteranganRujukan = "KeteranganRujukan";
            public const string KodePropinsi = "KodePropinsi";
            public const string NamaPropinsi = "NamaPropinsi";
            public const string KodeKabupaten = "KodeKabupaten";
            public const string NamaKabupaten = "NamaKabupaten";
            public const string KriteriaRujukanJson = "KriteriaRujukanJson";
            public const string NoRujukanSatuSehat = "NoRujukanSatuSehat";
            public const string ServiceRequestId = "ServiceRequestId";
            public const string AsalRujukanKode = "AsalRujukanKode";
            public const string AsalRujukanNama = "AsalRujukanNama";
            public const string DiagnosaKode = "DiagnosaKode";
            public const string DiagnosaNama = "DiagnosaNama";
            public const string PesertaAsuransi = "PesertaAsuransi";
            public const string PesertaHakKelas = "PesertaHakKelas";
            public const string PesertaJenis = "PesertaJenis";
            public const string PesertaKelamin = "PesertaKelamin";
            public const string PesertaNama = "PesertaNama";
            public const string PesertaNoKartu = "PesertaNoKartu";
            public const string PesertaNoMR = "PesertaNoMR";
            public const string PesertaTglLahir = "PesertaTglLahir";
            public const string PoliTujuanKode = "PoliTujuanKode";
            public const string PoliTujuanNama = "PoliTujuanNama";
            public const string TujuanRujukanKode = "TujuanRujukanKode";
            public const string TujuanRujukanNama = "TujuanRujukanNama";
            public const string BpjsResponseCode = "BpjsResponseCode";
            public const string BpjsResponseMessage = "BpjsResponseMessage";
            public const string RequestJson = "RequestJson";
            public const string ResponseJson = "ResponseJson";
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
            lock (typeof(BpjsRujukanSatuSehatMetadata))
            {
                if (BpjsRujukanSatuSehatMetadata.mapDelegates == null)
                {
                    BpjsRujukanSatuSehatMetadata.mapDelegates = new Dictionary<string, MapToMeta>();
                }

                if (BpjsRujukanSatuSehatMetadata.meta == null)
                {
                    BpjsRujukanSatuSehatMetadata.meta = new BpjsRujukanSatuSehatMetadata();
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

                meta.AddTypeMap("NoSep", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NoRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("TglRujukan", new esTypeMap("smalldatetime", "System.DateTime"));
                meta.AddTypeMap("TglRencana", new esTypeMap("smalldatetime", "System.DateTime"));
                meta.AddTypeMap("PpkDirujuk", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NamaPpkDirujuk", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("JnsPelayanan", new esTypeMap("char", "System.String"));
                meta.AddTypeMap("Catatan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("DiagRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("TipeRujukan", new esTypeMap("char", "System.String"));
                meta.AddTypeMap("PoliRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NamaPoliRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("User", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KodeFaskesSatuSehat", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("IdPasienSatuSehat", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KdppkSatuSehatTujuanRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KdDokterSatuSehat", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("EncounterReference", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PatientInstruction", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KeteranganRujukan", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KodePropinsi", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NamaPropinsi", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KodeKabupaten", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NamaKabupaten", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("KriteriaRujukanJson", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("NoRujukanSatuSehat", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ServiceRequestId", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("AsalRujukanKode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("AsalRujukanNama", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("DiagnosaKode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("DiagnosaNama", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaAsuransi", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaHakKelas", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaJenis", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaKelamin", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaNama", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaNoKartu", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaNoMR", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PesertaTglLahir", new esTypeMap("date", "System.DateTime"));
                meta.AddTypeMap("PoliTujuanKode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("PoliTujuanNama", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("TujuanRujukanKode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("TujuanRujukanNama", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("BpjsResponseCode", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("BpjsResponseMessage", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("RequestJson", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("ResponseJson", new esTypeMap("varchar", "System.String"));
                meta.AddTypeMap("LastUpdateDateTime", new esTypeMap("datetime", "System.DateTime"));
                meta.AddTypeMap("LastUpdateByUserID", new esTypeMap("varchar", "System.String"));


                meta.Source = "BpjsRujukanSatuSehat";
                meta.Destination = "BpjsRujukanSatuSehat";
                meta.spInsert = "proc_BpjsRujukanSatuSehatInsert";
                meta.spUpdate = "proc_BpjsRujukanSatuSehatUpdate";
                meta.spDelete = "proc_BpjsRujukanSatuSehatDelete";
                meta.spLoadAll = "proc_BpjsRujukanSatuSehatLoadAll";
                meta.spLoadByPrimaryKey = "proc_BpjsRujukanSatuSehatLoadByPrimaryKey";

                this._providerMetadataMaps["esDefault"] = meta;
            }

            return this._providerMetadataMaps["esDefault"];
        }

        #endregion

        static private BpjsRujukanSatuSehatMetadata meta;
        static protected Dictionary<string, MapToMeta> mapDelegates;
        static private int _esDefault = RegisterDelegateesDefault();
    }

}
