using DevExpress.DataProcessing.InMemoryDataProcessor;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.BPJS.VClaim.Klaim;
using Temiang.Avicenna.Common.BPJS.VClaim.v11.RujukanSatuSehat;
using Temiang.Dal.Core;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.RADT.Bpjs
{
    public partial class RujukanSatuSehatDetail : BasePageDetail
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            UrlPageSearch = "RujukanSatuSehatSearch.aspx";
            UrlPageList = "RujukanSatuSehatList.aspx";

            this.WindowSearch.Height = 400;

            ProgramID = AppConstant.Program.BpjsRujukanSatuSehat;

            cboFaskesRujukan.ItemDataBound += cboFaskesRujukan_ItemDataBound;

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSpesialistik();
            }
        }

        protected void cboPoliDirujuk_ItemDataBound(object sender, Telerik.Web.UI.RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((Common.BPJS.VClaim.v11.Poli.Poli2)e.Item.DataItem).Nama;
            e.Item.Value = ((Common.BPJS.VClaim.v11.Poli.Poli2)e.Item.DataItem).Kode;
        }

        protected void cboDiagnosaSep_ItemDataBound(object sender, Telerik.Web.UI.RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((Common.BPJS.VClaim.v11.Diagnosa.Diagnosa2)e.Item.DataItem).Nama;
            e.Item.Value = ((Common.BPJS.VClaim.v11.Diagnosa.Diagnosa2)e.Item.DataItem).Kode;
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);

            if (sourceControl is RadTextBox)
            {
                if (((RadTextBox)sourceControl).ID == txtNoSep.ID)
                {
                    var sep = new BpjsSEP();
                    sep.Query.Where(sep.Query.NoSEP == txtNoSep.Text);
                    if (sep.Query.Load())
                    {
                        txtTglSep.SelectedDate = sep.TanggalSEP;
                        txtNoPeserta.Text = sep.NomorKartu;
                        txtNamaPeserta.Text = sep.NamaPasien;
                        txtTglRujukan.SelectedDate = sep.TanggalSEP;
                        cboPelayanan.SelectedValue = sep.JenisPelayanan;

                    }
                }
            }
        }

        protected override void OnMenuNewClick()
        {
            txtNoRujukan.Text = string.Empty;
            txtTglRujukan.SelectedDate = DateTime.Now.Date;
            txtNoSep.Text = string.Empty;
            txtTglSep.Clear();
            txtNoPeserta.Text = string.Empty;
            txtNamaPeserta.Text = string.Empty;
            txtTglRujukan.Clear();
            txtTglRencanaKunjungan.Clear();
            cboPelayanan.SelectedValue = string.Empty;
            cboTipeRujukan.SelectedValue = string.Empty;

            cboPoliDirujuk.DataSource = null;
            cboPoliDirujuk.DataBind();
            cboPoliDirujuk.Items.Clear();
            cboPoliDirujuk.SelectedValue = string.Empty;
            cboPoliDirujuk.Text = string.Empty;

            cboDiagnosa.DataSource = null;
            cboDiagnosa.DataBind();
            cboDiagnosa.Items.Clear();
            cboDiagnosa.SelectedValue = string.Empty;
            cboDiagnosa.Text = string.Empty;

            txtCatatan.Text = string.Empty;
        }

        protected override void OnMenuEditClick()
        {
            txtNoSep.ReadOnly = true;
            btnCariPasien.Enabled = false;
            txtTglRujukan.DateInput.ReadOnly = true;
            txtTglRujukan.DatePopupButton.Enabled = false;
        }

        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            var ruj = new BpjsRujukanSatuSehat();
            ruj.Query.Where(ruj.Query.NoSep == txtNoSep.Text);
            if (ruj.Query.Load())
            {
                args.MessageText = "Rujukan atas SEP sudah dibuat";
                args.IsCancel = true;
                return;
            }

            var entity = new BpjsRujukanSatuSehat();
            entity.AddNew();
            SetEntityValue(entity);
            SaveEntity(entity, args);
        }

        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            var entity = new BpjsRujukanSatuSehat();
            if (entity.LoadByPrimaryKey(txtNoSep.Text, txtNoRujukan.Text))
            {
                SetEntityValue(entity);
                SaveEntity(entity, args);
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
                args.IsCancel = true;
                return;
            }
        }

        protected override void OnMenuDeleteClick(ValidateArgs args)
        {
            var entity = new BpjsRujukanSatuSehat();
            if (entity.LoadByPrimaryKey(txtNoSep.Text, txtNoRujukan.Text))
            {
                entity.MarkAsDeleted();
                SaveEntity(entity, args);
            }
            else
            {
                args.MessageText = AppConstant.Message.RecordNotExist;
                args.IsCancel = true;
                return;
            }
        }

        private void SetEntityValue(BpjsRujukanSatuSehat br)
        {
            br.NoSep = txtNoSep.Text;
            br.NoRujukan = txtNoRujukan.Text;
            br.TglRujukan = txtTglRujukan.SelectedDate;
            br.TglRencana = txtTglRencanaKunjungan.SelectedDate;
            br.JnsPelayanan = cboPelayanan.SelectedValue;
            br.Catatan = txtCatatan.Text;
            br.DiagRujukan = cboDiagnosa.SelectedValue;
            br.TipeRujukan = cboTipeRujukan.SelectedValue;
            br.PoliRujukan = cboPoliDirujuk.SelectedValue;
            br.NamaPoliRujukan = cboPoliDirujuk.Text;
            br.User = AppSession.UserLogin.UserID;

            // REGISTRASI
            var reg = new Registration();
            reg.Query.Where(reg.Query.BpjsSepNo == br.NoSep);
            reg.Query.Load();

            var pat = new Patient();
            pat.Query.Where(pat.Query.PatientID == reg.PatientID);
            pat.Query.Load();

            // SEP
            var sep = new BpjsSEP();
            sep.Query.Where(sep.Query.NoSEP == br.NoSep);
            sep.Query.Load();

            // ENCOUNTER
            var ssk = new SatuSehatKunjungan();
            ssk.Query.Where(ssk.Query.RegistrationNo == reg.RegistrationNo);
            ssk.Query.Load();

            // PATIENT
            var pb = new PatientBridging();
            pb.Query.Where(pb.Query.PatientID == reg.PatientID);
            pb.Query.Load();

            // DOKTER
            var db = new ParamedicBridging();
            db.Query.Where(
                db.Query.ParamedicID == reg.ParamedicID,
                db.Query.SRBridgingType == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID)
            );
            db.Query.Load();

            // FASKES
            var list = Session["faskesList"] as List<FaskesItem>;
            var selectedFaskes = list?.FirstOrDefault(x => x.Kdppk == cboFaskesRujukan.SelectedValue);

            string kodeFaskes = AppParameter.GetParameterValue(
                AppParameter.ParameterItem.SatuSehatOrganizationID);

            // Kriteria JSON
            var kriteria = GetKriteriaAnswer();
            br.KriteriaRujukanJson = kriteria != null
                ? JsonConvert.SerializeObject(kriteria)
                : null;

            // SET VALUE SATUSEHAT
            br.PpkDirujuk = cboFaskesRujukan.SelectedValue;
            br.NamaPpkDirujuk = cboFaskesRujukan.Text;

            br.KodeFaskesSatuSehat = kodeFaskes;
            br.IdPasienSatuSehat = pb?.BridgingID;

            br.KdppkSatuSehatTujuanRujukan = selectedFaskes != null
                ? selectedFaskes.KodeFaskesSatuSehat?.Replace("Organization/", "")
                : null;

            br.KdDokterSatuSehat = db?.BridgingID;

            br.EncounterReference = ssk?.EncounterID?.ToString();

            br.PatientInstruction = txtInstruksiPasien.Text;
            br.KeteranganRujukan = txtKeteranganRujukan.Text;

            // WILAYAH
            br.KodePropinsi = cboProvinsi.SelectedValue;
            br.NamaPropinsi = cboProvinsi.Text;
            br.KodeKabupaten = cboKabupaten.SelectedValue;
            br.NamaKabupaten = cboKabupaten.Text;

            // PESERTA
            br.PesertaNama = txtNamaPeserta.Text;
            br.PesertaNoKartu = sep?.NomorKartu;
            br.PesertaNoMR = pat.MedicalNo;

            if (sep != null)
            {
                br.PesertaKelamin = sep.JenisKelamin;
                br.PesertaTglLahir = sep.TanggalLahir;
            }

            // DIAGNOSA
            br.DiagnosaKode = cboDiagnosa.SelectedValue;
            br.DiagnosaNama = cboDiagnosa.Text;

            // POLI TUJUAN
            br.PoliTujuanKode = cboPoliDirujuk.SelectedValue;
            br.PoliTujuanNama = cboPoliDirujuk.Text;

            // TUJUAN RUJUKAN
            br.TujuanRujukanKode = cboFaskesRujukan.SelectedValue;
            br.TujuanRujukanNama = cboFaskesRujukan.Text;

            br.NoRujukanSatuSehat = txtNoRujukanSS.Text;

            // RESPONSE (default kosong dulu)
            br.BpjsResponseCode = null;
            br.BpjsResponseMessage = null;

            // RAW JSON (optional nanti diisi di SaveEntity)
            br.RequestJson = null;
            br.ResponseJson = null;

            // AUDIT
            br.LastUpdateDateTime = DateTime.Now;
            br.LastUpdateByUserID = AppSession.UserLogin.UserID;
        }

        private void SaveEntity(BpjsRujukanSatuSehat entity, ValidateArgs args)
        {
            var svc = new Common.BPJS.VClaim.v11.Service();

            using (var trans = new esTransactionScope())
            {
                if (entity.es.IsAdded)
                {
                    // VALIDASI
                    if (string.IsNullOrEmpty(cboFaskesRujukan.SelectedValue))
                    {
                        args.MessageText = "Faskes rujukan belum dipilih";
                        args.IsCancel = true;
                        return;
                    }

                    var kriteria = GetKriteriaAnswer();
                    if (kriteria == null || kriteria.Count == 0)
                    {
                        args.MessageText = "Kriteria rujukan belum diisi";
                        args.IsCancel = true;
                        return;
                    }

                    // REGISTRASI
                    var reg = new Registration();
                    reg.Query.Where(reg.Query.BpjsSepNo == entity.NoSep);
                    if (!reg.Query.Load())
                    {
                        args.MessageText = "Registrasi tidak ditemukan";
                        args.IsCancel = true;
                        return;
                    }

                    // ENCOUNTER
                    var ssk = new SatuSehatKunjungan();
                    ssk.Query.Where(ssk.Query.RegistrationNo == reg.RegistrationNo);
                    if (!ssk.Query.Load())
                    {
                        args.MessageText = "Encounter belum terbentuk";
                        args.IsCancel = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(cboSpesialistik.SelectedValue))
                    {
                        args.MessageText = "Spesialistik belum dipilih";
                        args.IsCancel = true;
                        return;
                    }

                    if (txtTglRencanaKunjungan.SelectedDate == null)
                    {
                        args.MessageText = "Tanggal rencana belum diisi";
                        args.IsCancel = true;
                        return;
                    }

                    // PATIENT BRIDGING
                    var pb = new PatientBridging();
                    pb.Query.Where(pb.Query.PatientID == reg.PatientID);
                    if (!pb.Query.Load())
                    {
                        args.MessageText = "Pasien belum bridging SatuSehat";
                        args.IsCancel = true;
                        return;
                    }

                    // DOKTER BRIDGING
                    var db = new ParamedicBridging();
                    db.Query.Where(
                        db.Query.ParamedicID == reg.ParamedicID,
                        db.Query.SRBridgingType == AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID)
                    );

                    if (!db.Query.Load())
                    {
                        args.MessageText = "Dokter belum bridging SatuSehat";
                        args.IsCancel = true;
                        return;
                    }

                    var list = Session["faskesList"] as List<FaskesItem>;

                    var selectedFaskes = list?.FirstOrDefault(x => x.Kdppk == cboFaskesRujukan.SelectedValue);

                    if (selectedFaskes == null)
                    {
                        args.MessageText = "Faskes tidak ditemukan";
                        args.IsCancel = true;
                        return;
                    }

                    // VALIDASI TAMBAHAN
                    if (string.IsNullOrEmpty(cboDiagnosa.SelectedValue))
                    {
                        args.MessageText = "Diagnosa belum dipilih";
                        args.IsCancel = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(cboPoliDirujuk.SelectedValue))
                    {
                        args.MessageText = "Poli dirujuk belum dipilih";
                        args.IsCancel = true;
                        return;
                    }

                    if (string.IsNullOrEmpty(cboProvinsi.SelectedValue))
                    {
                        args.MessageText = "Provinsi belum dipilih";
                        args.IsCancel = true;
                        return;
                    }

                    string kodeFaskes = AppParameter.GetParameterValue(
                        AppParameter.ParameterItem.SatuSehatOrganizationID);

                    // BUILD REQUEST
                    var request = new CreateRujukanRequest
                    {
                        Request = new RequestWrapper
                        {
                            TRujukan = new TRujukan
                            {
                                NoSep = entity.NoSep,
                                TglRujukan = entity.TglRujukan.Value.ToString("yyyy-MM-dd"),
                                TglRencanaKunjungan = entity.TglRencana.Value.ToString("yyyy-MM-dd"),
                                PpkDirujuk = cboFaskesRujukan.SelectedValue,
                                JnsPelayanan = entity.JnsPelayanan,
                                Catatan = entity.Catatan,
                                DiagRujukan = entity.DiagRujukan,
                                TipeRujukan = entity.TipeRujukan,
                                PoliRujukan = entity.PoliRujukan,
                                User = entity.User,

                                SatuSehatRujukan = new SatuSehatRujukan
                                {
                                    KodeFaskesSatuSehat = kodeFaskes,

                                    IdPasienSatuSehat = pb.BridgingID,

                                    KdppkSatuSehatTujuanRujukan = selectedFaskes.KodeFaskesSatuSehat.Replace("Organization/", ""),

                                    KdDokterSatuSehat = db.BridgingID,

                                    Encounter = new Encounter
                                    {
                                        Reference = $"{ssk.EncounterID}"
                                    },

                                    PatientInstruction = txtInstruksiPasien.Text,

                                    KriteriaRujukan = new KriteriaRujukanWrapper
                                    {
                                        Item = kriteria
                                    },

                                    KeteranganRujukan = txtKeteranganRujukan.Text,

                                    CodeJejaringWilayah = new CodeJejaringWilayah
                                    {
                                        KodePropinsi = cboProvinsi.SelectedValue,
                                        NamaPropinsi = cboProvinsi.Text,
                                        KodeKabupaten = cboKabupaten.SelectedValue ?? "",
                                        NamaKabupaten = cboKabupaten.Text ?? ""
                                    }
                                }
                            }
                        }
                    };

                    var json = JsonConvert.SerializeObject(request,
                        new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore
                        });

                    var response = svc.PostKunjungan(request);

                    entity.RequestJson = json;
                    entity.ResponseJson = JsonConvert.SerializeObject(response);
                    entity.BpjsResponseCode = response?.MetaData?.Code;
                    entity.BpjsResponseMessage = response?.MetaData?.Message;

                    if (response.MetaData.Code == "200" && response.Response != null)
                    {
                        txtNoRujukan.Text = response.Response.Rujukan.NoRujukan;
                        txtNoRujukanSS.Text = response.Response.Rujukan.NoRujukanSatuSehat;

                        entity.NoRujukan = txtNoRujukan.Text;
                        entity.NoRujukanSatuSehat = txtNoRujukanSS.Text;
                        entity.ServiceRequestId = response.Response.Rujukan.ServiceRequestId;
                    }
                    else
                    {
                        args.MessageText = $"{response.MetaData.Code} - {response.MetaData.Message}";
                        args.IsCancel = true;
                        return;
                    }
                }

                entity.Save();
                trans.Complete();
            }
        }

        protected override void OnMenuMovePrevClick(ValidateArgs args)
        {
            MoveRecord(false);
        }

        protected override void OnMenuMoveNextClick(ValidateArgs args)
        {
            MoveRecord(true);
        }

        protected override void OnMenuAuditLogClick(AuditLogFilter auditLogFilter)
        {
            //TODO: Betulkan PrimaryKeyData nya
            auditLogFilter.PrimaryKeyData = string.Format("NoSep='{0}'", txtNoSep.Text);
            auditLogFilter.TableName = "BpjsRujukanSatuSehat";
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            btnCariPasien.Enabled = (newVal == AppEnum.DataMode.New);
        }

        private void MoveRecord(bool isNextRecord)
        {
            var que = new BpjsRujukanSatuSehatQuery();
            que.es.Top = 1; // SELECT TOP 1 ..
            if (isNextRecord)
            {
                que.Where(que.NoSep > txtNoSep.Text);
                que.OrderBy(que.NoSep.Ascending);
            }
            else
            {
                que.Where(que.NoSep < txtNoSep.Text);
                que.OrderBy(que.NoSep.Descending);
            }
            var entity = new BpjsRujukanSatuSehat();
            if (entity.Load(que)) OnPopulateEntryControl(entity);
            else OnMenuNewClick();
        }

        protected override void OnPopulateEntryControl(params string[] parameters)
        {
            var entity = new BpjsRujukanSatuSehat();
            if (parameters.Length > 0)
            {
                string sepId = parameters[0];
                if (!parameters[0].Equals(string.Empty))
                {
                    entity.Query.Where(entity.Query.NoSep == sepId);
                    entity.Query.Load();
                }
            }
            else
            {
                entity.Query.Where(entity.Query.NoSep == txtNoSep.Text);
                entity.Query.Load();
            }

            OnPopulateEntryControl(entity);
        }

        protected override void OnPopulateEntryControl(esEntity entity)
        {
            var br = (BpjsRujukanSatuSehat)entity;

            if (br == null) return;

            txtNoSep.Text = br.NoSep;

            var sep = new BpjsSEP();
            sep.Query.es.Top = 1;
            sep.Query.Where(sep.Query.NoSEP == txtNoSep.Text);
            if (sep.Query.Load())
            {
                txtTglSep.SelectedDate = sep.TanggalSEP;
                txtNoPeserta.Text = sep.NomorKartu;
                txtNamaPeserta.Text = sep.NamaPasien;
            }

            txtNoRujukan.Text = br.NoRujukan;
            txtNoRujukanSS.Text = br.NoRujukanSatuSehat;

            txtTglRujukan.SelectedDate = br.TglRujukan;
            txtTglRencanaKunjungan.SelectedDate = br.TglRencana;

            cboPelayanan.SelectedValue = br.JnsPelayanan;
            txtCatatan.Text = br.Catatan;

            cboTipeRujukan.SelectedValue = br.TipeRujukan;

            if (!string.IsNullOrEmpty(br.DiagRujukan))
            {
                cboDiagnosa.Text = br.DiagnosaNama;
                cboDiagnosa.SelectedValue = br.DiagRujukan;
            }

            if (!string.IsNullOrEmpty(br.PoliRujukan))
            {
                cboPoliDirujuk.Text = br.NamaPoliRujukan;
                cboPoliDirujuk.SelectedValue = br.PoliRujukan;
            }

            if (!string.IsNullOrEmpty(br.PpkDirujuk))
            {
                cboFaskesRujukan.Text = br.NamaPpkDirujuk;
                cboFaskesRujukan.SelectedValue = br.PpkDirujuk;
            }

            txtInstruksiPasien.Text = br.PatientInstruction;
            txtKeteranganRujukan.Text = br.KeteranganRujukan;

            if (!string.IsNullOrEmpty(br.KodePropinsi))
            {
                cboProvinsi.SelectedValue = br.KodePropinsi;
                cboProvinsi.Text = br.NamaPropinsi;
            }

            if (!string.IsNullOrEmpty(br.KodeKabupaten))
            {
                cboKabupaten.SelectedValue = br.KodeKabupaten;
                cboKabupaten.Text = br.NamaKabupaten;
            }

            if (!string.IsNullOrEmpty(br.KriteriaRujukanJson))
            {
                try
                {
                    var list = JsonConvert.DeserializeObject<List<KriteriaAnswerItem>>(br.KriteriaRujukanJson);

                    ApplyKriteriaToUI(list);
                }
                catch { }
            }
        }

        private void ApplyKriteriaToUI(List<KriteriaAnswerItem> list)
        {
            if (list == null || list.Count == 0) return;

            foreach (RepeaterItem item in rptKriteria.Items)
            {
                var hf = (HiddenField)item.FindControl("hfLinkId");
                var cbo = (RadComboBox)item.FindControl("cboKriteria");

                if (hf == null || cbo == null) continue;

                var data = list.FirstOrDefault(x => x.LinkId == hf.Value);
                if (data == null) continue;

                var val = data.Answer?.FirstOrDefault()?.ValueString;

                if (!string.IsNullOrEmpty(val))
                {
                    cbo.SelectedValue = val;
                    cbo.Text = val;
                }
            }

            foreach (RepeaterItem item in rptBoolean.Items)
            {
                var hf = (HiddenField)item.FindControl("hfLinkId");
                var chk = (CheckBox)item.FindControl("chkBoolean");

                if (hf == null || chk == null) continue;

                var data = list.FirstOrDefault(x => x.LinkId == hf.Value);
                if (data == null) continue;

                var val = data.Answer?.FirstOrDefault()?.ValueBoolean;

                if (val.HasValue)
                {
                    chk.Checked = val.Value;
                }
            }
        }


        protected override void OnMenuPrintClick(ValidateArgs args, ref string programID, PrintJobParameterCollection printJobParameters)
        {
            printJobParameters.AddNew("p_NoRujukan", txtNoRujukan.Text);
        }

        protected void cboDiagnosa_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            cboFaskesRujukan.Items.Clear();
            LoadKriteriaRujukan();
        }

        private void LoadSpesialistik()
        {
            var svc = new Common.BPJS.VClaim.v11.Service();
            var res = svc.GetSpesialis();

            if (res.MetaData.Code == "200" && res.Response != null)
            {
                cboSpesialistik.ItemDataBound += (s, e) =>
                {
                    var data = (SpesialisItem)e.Item.DataItem;
                    if (data == null) return;

                    e.Item.Text = $"{data.NamaSpesialis} ({data.KodeSpesialis})";
                };

                cboSpesialistik.DataSource = res.Response;
                cboSpesialistik.DataTextField = "NamaSpesialis";
                cboSpesialistik.DataValueField = "KodeSpesialis";
                cboSpesialistik.DataBind();

                cboSpesialistik.Items.Insert(0,
                    new RadComboBoxItem("-- pilih spesialistik --", ""));
            }
        }

        protected void cboFaskes_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            var data = (FaskesItem)e.Item.DataItem;

            e.Item.Text = $"{data.Nmppk} ({data.Kelas}) - {data.Distance:0} m";
            e.Item.Value = data.Kdppk;
        }

        protected void cboSpesialistik_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            LoadFaskesRujukan();
        }

        private void LoadFaskesRujukan()
        {
            if (string.IsNullOrEmpty(cboDiagnosa.SelectedValue))
                return;

            if (txtTglRencanaKunjungan.SelectedDate == null)
                return;

            if (string.IsNullOrEmpty(cboProvinsi.SelectedValue))
                return;

            if (string.IsNullOrEmpty(cboSpesialistik.SelectedValue))
                return;

            var reg = new Registration();
            reg.Query.Where(reg.Query.BpjsSepNo == txtNoSep.Text);

            if (!reg.Query.Load())
                return;

            var ssk = new SatuSehatKunjungan();
            ssk.Query.Where(ssk.Query.RegistrationNo == reg.RegistrationNo);

            if (!ssk.Query.Load())
                return;

            string kodeFaskes = AppParameter.GetParameterValue(
                AppParameter.ParameterItem.SatuSehatOrganizationID);

            var svc = new Common.BPJS.VClaim.v11.Service();

            var kriteria = GetKriteriaAnswer();

            if (kriteria == null || kriteria.Count == 0)
                return;

            var request = new FaskesRujukanRequest
            {
                KodeFaskesSatuSehat = kodeFaskes,
                KodeDiagnosa = cboDiagnosa.SelectedValue,
                KodeSpesialis = cboSpesialistik.SelectedValue,
                TglRencanaKunjungan = txtTglRencanaKunjungan.SelectedDate.Value.ToString("yyyy-MM-dd"),

                KriteriaRujukan = new KriteriaRujukanWrapper
                {
                    Item = kriteria
                },

                CodeJejaringWilayah = new CodeJejaringWilayah
                {
                    KodePropinsi = cboProvinsi.SelectedValue,
                    NamaPropinsi = cboProvinsi.Text,
                    KodeKabupaten = cboKabupaten.SelectedValue ?? "",
                    NamaKabupaten = cboKabupaten.Text ?? ""
                },

                Encounter = new Encounter
                {
                    Reference = $"Encounter/{ssk.EncounterID}"
                }
            };

            var response = svc.GetFaskesRujukan(request);

            if (response.MetaData.Code == "200" && response.Response != null)
            {
                BindFaskes(response.Response.List);
            }
        }

        private void BindFaskes(List<FaskesItem> list)
        {
            if (list == null || list.Count == 0)
            {
                Alert("Faskes tidak ditemukan");
                return;
            }

            list = list.OrderBy(x => x.Distance).ToList();

            cboFaskesRujukan.DataSource = list;
            cboFaskesRujukan.DataTextField = "Nmppk";
            cboFaskesRujukan.DataValueField = "Kdppk";
            cboFaskesRujukan.DataBind();

            cboFaskesRujukan.Items.Insert(0,
                new RadComboBoxItem("-- pilih faskes --", ""));

            Session["faskesList"] = list;
        }

        protected void cboFaskesRujukan_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            var data = (FaskesItem)e.Item.DataItem;
            if (data == null) return;

            e.Item.Text = $"{data.Nmppk} - {data.Nmkc} - Jumlah Rujuk: {data.JmlRujuk:0} - Kapasitas: {data.Kapasitas:0}";
        }

        private void Alert(string msg)
        {
            ScriptManager.RegisterStartupScript(this, GetType(),
                "alert", $"alert('{msg}');", true);
        }

        private void LoadKriteriaRujukan()
        {
            if (rptKriteria.Items.Count > 0 && ViewState[VS_LAST_DIAG]?.ToString() == cboDiagnosa.SelectedValue)
                return;

            ViewState[VS_LAST_DIAG] = cboDiagnosa.SelectedValue;

            if (string.IsNullOrEmpty(cboDiagnosa.SelectedValue))
                return;

            var svc = new Common.BPJS.VClaim.v11.Service();

            string kodeFaskes = AppParameter.GetParameterValue(
                AppParameter.ParameterItem.SatuSehatOrganizationID);

            var reg = new Registration();
            reg.Query.Where(reg.Query.BpjsSepNo == txtNoSep.Text);

            if (!reg.Query.Load())
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "err", "alert('Registrasi tidak ditemukan');", true);
                return;
            }

            var ssk = new SatuSehatKunjungan();
            ssk.Query.Where(ssk.Query.RegistrationNo == reg.RegistrationNo);

            if (!ssk.Query.Load())
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "err", "alert('Encounter SatuSehat belum terbentuk');", true);
                return;
            }

            var request = new KriteriaRujukanRequest
            {
                KodeFaskesSatuSehat = kodeFaskes,
                KodeDiagnosa = cboDiagnosa.SelectedValue,
                Encounter = new Encounter
                {
                    Reference = $"Encounter/{ssk.EncounterID}"
                }
            };

            var response = svc.GetKriteriaRujukan(request);

            if (response.MetaData.Code == "200" && response.Response != null)
            {
                var all = response.Response.KriteriaRujukan;

                var textItems = all.Where(x => x.Type == "text").ToList();
                var booleanItems = all.Where(x => x.Type == "boolean").ToList();

                rptKriteria.DataSource = textItems;
                rptKriteria.DataBind();

                rptBoolean.DataSource = booleanItems;
                rptBoolean.DataBind();

                Wilayah = response.Response.JejaringWilayahRujukan;
                BindProvinsi(Wilayah);

                cboKabupaten.Items.Clear();
                cboKabupaten.Items.Insert(0,
                    new RadComboBoxItem("-- pilih kabupaten --", ""));
            }

            cboProvinsi.ClearSelection();
            cboKabupaten.ClearSelection();
        }

        private List<KriteriaAnswerItem> GetKriteriaAnswer()
        {
            var list = new List<KriteriaAnswerItem>();

            foreach (RepeaterItem item in rptKriteria.Items)
            {
                var linkId = ((HiddenField)item.FindControl("hfLinkId")).Value;
                var cbo = (RadComboBox)item.FindControl("cboKriteria");

                if (cbo != null && cbo.Visible)
                {
                    var lbl = (Label)item.FindControl("lblText");

                    list.Add(new KriteriaAnswerItem
                    {
                        LinkId = linkId,
                        Text = lbl != null ? lbl.Text : "",
                        Answer = new List<Answer>
                        {
                            new Answer
                            {
                                ValueString = cbo.SelectedValue
                            }
                        }
                    });
                }
            }

            foreach (RepeaterItem item in rptBoolean.Items)
            {
                var chk = (CheckBox)item.FindControl("chkBoolean");

                var linkId = ((HiddenField)item.FindControl("hfLinkId"))?.Value;

                list.Add(new KriteriaAnswerItem
                {
                    LinkId = linkId,
                    Text = chk.Text,
                    Answer = new List<Answer>
                    {
                        new Answer
                        {
                            ValueBoolean = chk.Checked
                        }
                    }
                });
            }

            return list;
        }

        protected void cboProvinsi_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var wilayah = Wilayah;
            if (wilayah == null) return;

            var prov = cboProvinsi.SelectedValue;
            if (string.IsNullOrEmpty(prov)) return;

            cboKabupaten.ClearSelection();

            var root = wilayah.FirstOrDefault();
            if (root == null) return;

            var kab = root
                .Item
                .FirstOrDefault(x => x.Text == "Kabupaten/Kota")
                ?.AnswerOption
                ?.Where(x => x.ValueCoding != null && x.ValueCoding.Code.StartsWith(prov))
                .ToList();

            if (kab == null) return;

            var data = kab.Select(x => new
            {
                Code = x.ValueCoding.Code,
                Display = x.ValueCoding.Display
            }).ToList();

            cboKabupaten.DataSource = data;
            cboKabupaten.DataTextField = "Display";
            cboKabupaten.DataValueField = "Code";
            cboKabupaten.DataBind();

            cboKabupaten.Items.Insert(0,
                new RadComboBoxItem("-- pilih kabupaten --", ""));

            cboFaskesRujukan.Items.Clear();
        }

        private void BindProvinsi(List<JejaringWilayah> wilayah)
        {
            if (wilayah == null || wilayah.Count == 0)
                return;

            var provinsi = wilayah
                .First()
                .Item
                .FirstOrDefault(x => x.Text == "Provinsi")
                ?.AnswerOption;

            if (provinsi == null) return;

            var data = provinsi.Select(x => new
            {
                Code = x.ValueCoding.Code,
                Display = x.ValueCoding.Display
            }).ToList();

            cboProvinsi.DataSource = data;
            cboProvinsi.DataTextField = "Display";
            cboProvinsi.DataValueField = "Code";
            cboProvinsi.DataBind();

            cboProvinsi.Items.Insert(0,
                new RadComboBoxItem("-- pilih provinsi --", ""));
        }

        private void BindKabupaten(List<JejaringWilayah> wilayah)
        {
            var kab = wilayah
                .First()
                .Item
                .First(x => x.Text == "Kabupaten/Kota")
                .AnswerOption;

            var data = kab.Select(x => new
            {
                Code = x.ValueCoding.Code,
                Display = x.ValueCoding.Display
            }).ToList();

            cboKabupaten.DataSource = data;
            cboKabupaten.DataTextField = "Display";
            cboKabupaten.DataValueField = "Code";
            cboKabupaten.DataBind();

            cboKabupaten.Items.Insert(0,
                new RadComboBoxItem("-- pilih kabupaten --", ""));
        }

        private List<JejaringWilayah> Wilayah
        {
            get
            {
                return Session["wilayah"] as List<JejaringWilayah>;
            }
            set
            {
                Session["wilayah"] = value;
            }
        }

        private const string VS_LAST_DIAG = "LAST_DIAG";

    }
}