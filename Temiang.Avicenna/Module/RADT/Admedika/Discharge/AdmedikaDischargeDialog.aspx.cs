using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;
using AdmedikaService = Temiang.Avicenna.Common.Admedika.Service;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    public partial class AdmedikaDischargeDialog : BasePageDialog
    {
        private Dictionary<string, string> _benefitSelections;
        private DataTable _storedBilling;

        private void SaveRequestSnapshot(AdmedikaClaimTransaction claim, DischargeOpPayload payload)
        {
            var billing = AdmedikaDischargeBillingProvider.Load(claim.RegistrationNo);
            foreach (DataRow row in billing.Rows)
            {
                string benefit;
                if (_benefitSelections.TryGetValue(ItemKey(row["SourceType"], row["TransactionNo"], row["SequenceNo"]), out benefit))
                    row["BenID"] = benefit;
            }
            var snapshot = JsonConvert.SerializeObject(new
            {
                schemaVersion = 1, capturedAt = DateTime.Now, clID = claim.ClID,
                request = payload, billingItems = billing, benefits = BenefitTable
            });
            if (claim.IsClosed == true)
                throw new InvalidOperationException("Claim sudah ditutup. Request belum dikirim.");
            claim.DischargeRequestJson = snapshot;
            claim.Save();
        }

        private bool LoadRequestSnapshot()
        {
            var claim = LoadClaim();
            var json = claim == null ? null : claim.DischargeRequestJson;
            if (string.IsNullOrWhiteSpace(json)) return false;
            var snapshot = JObject.Parse(json);
            var request = snapshot["request"].ToObject<DischargeOpPayload>();
            txtDiagnosisCodeList.Text = request.DiagnosisCodeList;
            double days;
            txtMcDays.Value = double.TryParse(request.McDays, out days) ? days : (double?)null;
            rblAccidentFlag.ClearSelection();
            rblSurgicalFlag.ClearSelection();
            if (rblAccidentFlag.Items.FindByValue(request.AccidentFlag ?? "") != null)
                rblAccidentFlag.SelectedValue = request.AccidentFlag;
            if (rblSurgicalFlag.Items.FindByValue(request.SurgicalFlag ?? "") != null)
                rblSurgicalFlag.SelectedValue = request.SurgicalFlag;
            txtRemarks.Text = request.Remarks;
            BenefitTable = snapshot["benefits"].ToObject<DataTable>();
            _storedBilling = snapshot["billingItems"].ToObject<DataTable>();
            _benefitSelections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in _storedBilling.Rows)
                _benefitSelections[ItemKey(row["SourceType"], row["TransactionNo"], row["SequenceNo"])] = Convert.ToString(row["BenID"]);
            return true;
        }

        private new void ShowInformationHeader(string message)
        {
            var claim = LoadClaim();
            var success = claim != null && claim.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims;
            lblProcessMessage.Text = Encode(message);
            lblProcessMessage.Visible = true;
            lblProcessMessage.Style["display"] = "block";
            lblProcessMessage.Style["padding"] = "10px";
            lblProcessMessage.Style["margin-bottom"] = "8px";
            lblProcessMessage.Style["border"] = success ? "1px solid #b7d7bd" : "1px solid #ebccd1";
            lblProcessMessage.Style["background-color"] = success ? "#edf7ef" : "#fdf0ef";
            lblProcessMessage.Style["color"] = success ? "#286238" : "#a94442";
            if (IsPostBack)
                AjaxManager.ResponseScripts.Add("document.getElementById('" +
                    pnlResponse.ClientID + "').scrollIntoView({block:'center'});");
        }

        private void CaptureBenefitSelections()
        {
            _benefitSelections = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (GridDataItem item in grdBillingItems.Items)
            {
                var combo = item.FindControl("cboBenefit") as RadComboBox;
                _benefitSelections[ItemKey(item.GetDataKeyValue("SourceType"),
                    item.GetDataKeyValue("TransactionNo"), item.GetDataKeyValue("SequenceNo"))] =
                    combo == null ? string.Empty : combo.SelectedValue;
            }
        }

        private bool BillingHasChanged(string registrationNo)
        {
            var current = AdmedikaDischargeBillingProvider.Load(registrationNo);
            if (current.Rows.Count != grdBillingItems.Items.Count) return true;

            var rows = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in current.Rows)
                rows[ItemKey(row["SourceType"], row["TransactionNo"], row["SequenceNo"])] = row;

            foreach (GridDataItem item in grdBillingItems.Items)
            {
                DataRow row;
                if (!rows.TryGetValue(ItemKey(item.GetDataKeyValue("SourceType"),
                    item.GetDataKeyValue("TransactionNo"), item.GetDataKeyValue("SequenceNo")), out row))
                    return true;

                foreach (var field in new[] { "ItemID", "ItemName", "Quantity", "TotalPrice", "IssueDate" })
                {
                    if (!object.Equals(row[field], item.GetDataKeyValue(field))) return true;
                }
            }
            return false;
        }

        public long ClaimTransactionID
        {
            get
            {
                long id;
                return long.TryParse(Request.QueryString["id"], out id) ? id : 0;
            }
        }

        private DataTable BenefitTable
        {
            get
            {
                var value = ViewState["AdmedikaDischarge:Benefits"] as DataTable;
                return value ?? CreateBenefitTable();
            }
            set { ViewState["AdmedikaDischarge:Benefits"] = value; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.AdmedikaDischargeOp;

            ButtonOk.Visible = false;
            ButtonCancel.Text = "Close";
            ButtonCancel.OnClientClick = "closeDischargeDialog(); return false;";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            if (!LoadClaimHeader()) return;

            LoadDefaultDiagnosis();
            LoadEntitlement();
            grdBillingItems.Rebind();
        }

        private bool LoadClaimHeader()
        {
            var claim = new AdmedikaClaimTransaction();
            if (ClaimTransactionID == 0 || !claim.LoadByPrimaryKey(ClaimTransactionID))
            {
                ShowInformationHeader("Data claim AdMedika tidak ditemukan.");
                DisableProcess();
                return false;
            }

            if (!string.Equals(claim.CovID, AdmedikaConstant.CoverageID.GP,
                StringComparison.Ordinal))
            {
                ShowInformationHeader("Claim bukan coverage rawat jalan (GP/02).");
                DisableProcess();
                return false;
            }

            var registration = new Registration();
            if (string.IsNullOrWhiteSpace(claim.RegistrationNo) ||
                !registration.LoadByPrimaryKey(claim.RegistrationNo))
            {
                ShowInformationHeader("Claim belum terhubung dengan registrasi Avicenna.");
                DisableProcess();
                return false;
            }

            if (!string.Equals(registration.SRRegistrationType,
                AppConstant.RegistrationType.OutPatient, StringComparison.Ordinal))
            {
                ShowInformationHeader("Registrasi bukan rawat jalan.");
                DisableProcess();
                return false;
            }

            var patientName = claim.PatientName;
            var patient = new Patient();
            if (!string.IsNullOrWhiteSpace(registration.PatientID) &&
                patient.LoadByPrimaryKey(registration.PatientID))
                patientName = patient.FullName;

            var physicianName = string.Empty;
            var physician = new Paramedic();
            if (!string.IsNullOrWhiteSpace(registration.ParamedicID) &&
                physician.LoadByPrimaryKey(registration.ParamedicID))
                physicianName = physician.ParamedicName;

            lblRegistrationNo.Text = Value(registration.RegistrationNo);
            lblPatientName.Text = Value(patientName);
            lblCardNo.Text = Encode(Value(claim.CardNo));
            lblClaimID.Text = Value(claim.ClID);
            lblClaimStatus.Text = Value(claim.ClStatus) + " - " + Value(claim.ClDesc);
            lblPhysician.Text = Value(physicianName);

            if (registration.IsVoid == true)
            {
                ShowInformationHeader("Registrasi sudah void. Discharge tidak dapat diproses.");
                DisableProcess();
                return false;
            }
            else if (registration.IsClosed == true && claim.ClStatus != AdmedikaConstant.ClaimStatusID.ValidClaims)
            {
                ShowInformationHeader("Registrasi Avicenna sudah closed sebelum discharge AdMedika selesai.");
                DisableProcess();
                return false;
            }
            else if (claim.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims || claim.IsClosed == true)
            {
                txtDiagnosisCodeList.Text = claim.DiagnosisCodeList ?? string.Empty;
                var hasSnapshot = LoadRequestSnapshot();
                if (!hasSnapshot)
                {
                    txtMcDays.Value = null;
                    rblAccidentFlag.ClearSelection();
                    rblSurgicalFlag.ClearSelection();
                    txtRemarks.Text = string.Empty;
                    lblStoredMedicalNote.Visible = true;
                    grdBillingItems.MasterTableView.Columns[6].Visible = false;
                }
                pnlMappingHint.Visible = false;
                lblStoredBillingNote.Visible = true;
                lblStoredBillingNote.Text = hasSnapshot
                    ? "Data billing dan mapping sesuai snapshot request discharge."
                    : "Billing saat ini, bukan snapshot saat discharge. Mapping benefit lama belum tersedia.";
                pnlMedical.Enabled = false;
                pnlEntitlement.Visible = false;
                grdBillingItems.Enabled = false;
                btnProcessDischarge.Enabled = false;
                btnRefreshEntitlement.Enabled = false;
                btnPrintLOC.Enabled = !string.IsNullOrWhiteSpace(claim.PrintOutLOC);
                litResponse.Text = BuildStoredResponseHtml(claim);
                ShowInformationHeader(hasSnapshot ? "Claim sudah berhasil discharge." :
                    "Claim sudah berhasil discharge. Detail isian discharge lama belum tersedia.");
                return false;
            }

            return true;
        }

        private void LoadDefaultDiagnosis()
        {
            var claim = LoadClaim();
            if (claim == null) return;

            if (!string.IsNullOrWhiteSpace(claim.DiagnosisCodeList))
            {
                txtDiagnosisCodeList.Text = claim.DiagnosisCodeList;
                return;
            }

            var query = new EpisodeDiagnoseQuery("a");
            query.Select(query.DiagnoseID, query.SequenceNo);
            query.Where(
                query.RegistrationNo == claim.RegistrationNo,
                query.IsVoid == false,
                query.DiagnoseID.IsNotNull(),
                query.DiagnoseID != string.Empty);
            query.OrderBy(query.SequenceNo.Ascending);

            var table = query.LoadDataTable();
            var diagnoses = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                var code = Convert.ToString(row["DiagnoseID"]).Trim();
                if (code.Length > 0 && !diagnoses.Contains(code))
                    diagnoses.Add(code);
            }

            txtDiagnosisCodeList.Text = string.Join(", ", diagnoses.ToArray());
        }

        protected void btnRefreshEntitlement_Click(object sender, EventArgs e)
        {
            CaptureBenefitSelections();
            LoadEntitlement();
            grdBillingItems.Rebind();
        }

        private bool LoadEntitlement()
        {
            lblEntitlementMessage.Text = string.Empty;
            litEntitlement.Text = string.Empty;
            BenefitTable = CreateBenefitTable();

            var claim = LoadClaim();
            if (claim == null || string.IsNullOrWhiteSpace(claim.CardNo))
            {
                SetEntitlementMessage("Card No tidak tersedia.", true);
                btnProcessDischarge.Enabled = false;
                return false;
            }

            var service = new AdmedikaService();
            var result = service.GetEntitlement(claim.CardNo);
            string message;
            if (!service.IsOk(result, out message))
            {
                SetEntitlementMessage(message, true);
                btnProcessDischarge.Enabled = false;
                return false;
            }

            if (!string.IsNullOrWhiteSpace(result.ClID) &&
                !string.Equals(result.ClID.Trim(), claim.ClID, StringComparison.Ordinal))
            {
                SetEntitlementMessage("ClID dari GET_ENTITLEMENT berbeda dengan claim terpilih.", true);
                btnProcessDischarge.Enabled = false;
                return false;
            }

            var table = CreateBenefitTable();
            var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var benefit in result.Entitlement)
            {
                var benID = (benefit.BenID ?? string.Empty).Trim();
                if (benID.Length == 0 || !used.Add(benID)) continue;

                var row = table.NewRow();
                row["BenID"] = benID;
                row["ShortName"] = benefit.ShortName ?? string.Empty;
                row["LongName"] = benefit.LongName ?? string.Empty;
                row["DisplayName"] = string.Format("[{0}] {1} - {2}", benID,
                    benefit.ShortName, benefit.LongName).TrimEnd(' ', '-');
                table.Rows.Add(row);
            }

            BenefitTable = table;
            litEntitlement.Text = BuildEntitlementHtml(table);
            SetEntitlementMessage(table.Rows.Count + " benefit tersedia.", false);

            if (table.Rows.Count == 0)
                btnProcessDischarge.Enabled = false;
            else
                btnProcessDischarge.Enabled = true;

            return table.Rows.Count > 0;
        }

        protected void grdBillingItems_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (_storedBilling != null)
            {
                grdBillingItems.DataSource = _storedBilling;
                return;
            }
            var claim = LoadClaim();
            grdBillingItems.DataSource = claim == null
                ? new DataTable()
                : AdmedikaDischargeBillingProvider.Load(claim.RegistrationNo);
        }

        protected void grdBillingItems_ItemDataBound(object sender, GridItemEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item == null) return;

            var list = item.FindControl("cboBenefit") as RadComboBox;
            if (list == null) return;

            list.Items.Clear();
            list.ClearSelection();
            list.Text = string.Empty;
            list.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
            foreach (DataRow row in BenefitTable.Rows)
                list.Items.Add(new RadComboBoxItem(Convert.ToString(row["DisplayName"]),
                    Convert.ToString(row["BenID"])));

            var sourceType = Convert.ToString(item.GetDataKeyValue("SourceType"));
            string previous;
            if (_benefitSelections != null)
            {
                if (_benefitSelections.TryGetValue(ItemKey(sourceType,
                    item.GetDataKeyValue("TransactionNo"), item.GetDataKeyValue("SequenceNo")), out previous) &&
                    list.FindItemByValue(previous) != null)
                {
                    list.SelectedValue = previous;
                    return;
                }
            }
            if (sourceType == "PRESCRIPTION" && list.FindItemByValue("41") != null)
                list.SelectedValue = "41";
            else if (BenefitTable.Rows.Count == 1)
                list.SelectedValue = Convert.ToString(BenefitTable.Rows[0]["BenID"]);
        }

        protected void btnProcessDischarge_Click(object sender, EventArgs e)
        {
            Page.Validate("discharge");
            if (!Page.IsValid)
            {
                ShowInformationHeader("Diagnosis ICD-10 wajib diisi.");
                return;
            }

            HideInformationHeader();
            lblProcessMessage.Text = string.Empty;
            litResponse.Text = string.Empty;

            var claim = LoadClaim();
            if (claim == null)
            {
                ShowInformationHeader("Data claim AdMedika tidak ditemukan.");
                return;
            }

            if (claim.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims || claim.IsClosed == true)
            {
                ShowInformationHeader("Claim sudah discharge. Request tidak dikirim ulang.");
                btnProcessDischarge.Enabled = false;
                return;
            }

            CaptureBenefitSelections();
            if (!LoadEntitlement())
            {
                grdBillingItems.Rebind();
                ShowInformationHeader("Benefit terbaru belum dapat diverifikasi. Discharge belum dikirim.");
                return;
            }

            var reviewRequired = BillingHasChanged(claim.RegistrationNo);
            foreach (var selection in _benefitSelections.Values)
            {
                if (!string.IsNullOrEmpty(selection) && !BenefitExists(selection))
                    reviewRequired = true;
            }
            if (reviewRequired)
            {
                grdBillingItems.Rebind();
                ShowInformationHeader("Billing atau benefit berubah. Data sudah diperbarui; periksa item, total dan mapping benefit, lalu klik Process Discharge kembali. Discharge belum dikirim.");
                return;
            }

            string validationMessage;
            DischargeEntitlementInput[] entitlements;
            if (!BuildDischargeEntitlements(claim.RegistrationNo, out entitlements,
                out validationMessage))
            {
                ShowInformationHeader(validationMessage);
                return;
            }

            var diagnoses = NormalizeDiagnosisList(txtDiagnosisCodeList.Text);
            if (string.IsNullOrWhiteSpace(diagnoses))
            {
                ShowInformationHeader("ICD-10 required.");
                return;
            }

            var registration = new Registration();
            if (!registration.LoadByPrimaryKey(claim.RegistrationNo) || registration.IsVoid == true)
            {
                ShowInformationHeader("Registrasi tidak ditemukan atau sudah void.");
                return;
            }

            if (registration.IsClosed == true)
            {
                ShowInformationHeader("Registrasi Avicenna sudah closed. Request discharge tidak dikirim.");
                return;
            }

            var physicianName = string.Empty;
            var physician = new Paramedic();
            if (!string.IsNullOrWhiteSpace(registration.ParamedicID) &&
                physician.LoadByPrimaryKey(registration.ParamedicID))
                physicianName = physician.ParamedicName;

            var service = new AdmedikaService();

            var icdResult = service.CheckIcdExclusion(claim.CardNo,
                AdmedikaConstant.CoverageID.GP, diagnoses);
            string message;
            if (!service.IsOk(icdResult, out message))
            {
                litResponse.Text = BuildErrorHtml(icdResult, "Validasi ICD gagal: " + message);
                ShowInformationHeader("Validasi ICD gagal. Discharge belum dikirim.");
                return;
            }

            var mcDays = txtMcDays.Value.HasValue
                ? Convert.ToInt32(txtMcDays.Value.Value).ToString(CultureInfo.InvariantCulture)
                : null;

            try
            {
                SaveRequestSnapshot(claim, new DischargeOpPayload
                {
                    CardNo = claim.CardNo, DiagnosisCodeList = diagnoses, McDays = mcDays,
                    ProviderTransID = registration.RegistrationNo, PhysicianName = physicianName,
                    AccidentFlag = rblAccidentFlag.SelectedValue,
                    SurgicalFlag = rblSurgicalFlag.SelectedValue, Remarks = txtRemarks.Text.Trim(),
                    Entitlement = entitlements
                });
            }
            catch (Exception)
            {
                ShowInformationHeader("Snapshot discharge gagal disimpan. Pastikan migrasi DischargeRequestJson sudah dijalankan. Discharge belum dikirim.");
                return;
            }

            var result = service.DischargeOp(
                claim.CardNo,
                diagnoses,
                mcDays,
                registration.RegistrationNo,
                null,
                null,
                physicianName,
                rblAccidentFlag.SelectedValue,
                rblSurgicalFlag.SelectedValue,
                null,
                null,
                txtRemarks.Text.Trim(),
                entitlements);

            if (!service.IsOk(result, out message))
            {
                if (!string.IsNullOrWhiteSpace(result.TechnicalError) || result.ErrorCode == "1990")
                {
                    ReconcileUnknownResult(claim, service, message);
                    return;
                }

                litResponse.Text = BuildErrorHtml(result, message);
                ShowInformationHeader(message);
                return;
            }

            SaveDischargeResult(claim, result, diagnoses);
            litResponse.Text = BuildResponseHtml(result);
            MarkParentForRefresh();

            if (result.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims)
            {
                btnProcessDischarge.Enabled = false;
                btnRefreshEntitlement.Enabled = false;
                btnPrintLOC.Enabled = !string.IsNullOrWhiteSpace(result.PrintOut);
                ShowInformationHeader("Discharge berhasil diproses. Silakan periksa nilai penjaminan dan ekses pasien, kemudian cetak LOC melalui tombol Print LOC.");
            }
            else
            {
                btnPrintLOC.Enabled = !string.IsNullOrWhiteSpace(result.PrintOut);
                ShowInformationHeader(string.Format("Discharge diterima dengan status {0} - {1}.",
                    result.ClStatus, result.ClDesc));
            }
        }

        private bool BuildDischargeEntitlements(string registrationNo,
            out DischargeEntitlementInput[] entitlements, out string message)
        {
            entitlements = null;
            message = null;

            var currentItems = AdmedikaDischargeBillingProvider.Load(registrationNo);
            var currentByKey = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in currentItems.Rows)
                currentByKey[ItemKey(row["SourceType"], row["TransactionNo"], row["SequenceNo"])] = row;

            if (currentItems.Rows.Count != grdBillingItems.Items.Count)
            {
                message = "Billing berubah sejak halaman dibuka. Refresh halaman sebelum submit.";
                return false;
            }

            var grouped = new Dictionary<string, List<DischargeBenItemInput>>(
                StringComparer.OrdinalIgnoreCase);
            var totals = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (GridDataItem gridItem in grdBillingItems.Items)
            {
                var sourceType = Convert.ToString(gridItem.GetDataKeyValue("SourceType"));
                var transactionNo = Convert.ToString(gridItem.GetDataKeyValue("TransactionNo"));
                var sequenceNo = Convert.ToString(gridItem.GetDataKeyValue("SequenceNo"));
                var key = ItemKey(sourceType, transactionNo, sequenceNo);

                DataRow row;
                if (!currentByKey.TryGetValue(key, out row))
                {
                    message = "Billing item " + transactionNo + "/" + sequenceNo +
                              " sudah berubah. Refresh halaman sebelum submit.";
                    return false;
                }

                var originalTotal = Convert.ToDecimal(gridItem.GetDataKeyValue("TotalPrice"));
                var currentTotal = Convert.ToDecimal(row["TotalPrice"]);
                if (originalTotal != currentTotal)
                {
                    message = "Nilai billing berubah. Refresh halaman sebelum submit.";
                    return false;
                }

                if (currentTotal <= 0)
                {
                    message = "Billing berisi nilai nol/negatif pada item " +
                              Convert.ToString(row["ItemID"]) + ". Koreksi billing sebelum discharge.";
                    return false;
                }

                var benefitList = gridItem.FindControl("cboBenefit") as RadComboBox;
                var benID = benefitList == null ? string.Empty : benefitList.SelectedValue.Trim();
                if (benID.Length == 0)
                {
                    message = "Semua billing item wajib dipetakan ke benefit AdMedika.";
                    return false;
                }

                if (!BenefitExists(benID))
                {
                    message = "Benefit " + benID + " tidak terdapat pada entitlement terbaru.";
                    return false;
                }

                List<DischargeBenItemInput> items;
                if (!grouped.TryGetValue(benID, out items))
                {
                    items = new List<DischargeBenItemInput>();
                    grouped.Add(benID, items);
                    totals.Add(benID, 0);
                }

                var itemID = Limit(Convert.ToString(row["ItemID"]), 50);
                if (string.IsNullOrWhiteSpace(itemID))
                    itemID = Limit(transactionNo + "-" + sequenceNo, 50);

                var issueDate = row["IssueDate"] == DBNull.Value
                    ? null
                    : Convert.ToDateTime(row["IssueDate"]).ToString("ddMMyyyy",
                        CultureInfo.InvariantCulture);

                items.Add(new DischargeBenItemInput
                {
                    Code = itemID,
                    Name = Limit(Convert.ToString(row["ItemName"]), 50),
                    DateIssue = issueDate,
                    Qty = Convert.ToDecimal(row["Quantity"]).ToString("0.##",
                        CultureInfo.InvariantCulture),
                    TotPrice = currentTotal.ToString("0.##", CultureInfo.InvariantCulture)
                });
                totals[benID] += currentTotal;
            }

            if (grouped.Count == 0)
            {
                message = "Tidak ada billing item yang dapat dikirim.";
                return false;
            }

            var result = new List<DischargeEntitlementInput>();
            foreach (var group in grouped)
            {
                result.Add(new DischargeEntitlementInput
                {
                    BenID = group.Key,
                    BenAmount = totals[group.Key].ToString("0.##", CultureInfo.InvariantCulture),
                    BenItemList = group.Value.ToArray()
                });
            }

            entitlements = result.ToArray();
            return true;
        }

        private void ReconcileUnknownResult(AdmedikaClaimTransaction claim,
            AdmedikaService service, string originalMessage)
        {
            var status = service.CheckMemberClaimTxnStatus(claim.CardNo, claim.ClID);
            string statusMessage;
            if (!service.IsOk(status, out statusMessage))
            {
                litResponse.Text = BuildErrorHtml(status,
                    "Hasil discharge belum dapat dipastikan. Error awal: " + originalMessage +
                    ". Check status: " + statusMessage);
                ShowInformationHeader("Status discharge belum diketahui. Jangan submit ulang sebelum Check Claim Status berhasil.");
                return;
            }

            claim.ClStatus = status.ClStatus;
            claim.ClDesc = status.ClDesc;

            if (status.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims)
            {
                var print = service.RePrintOut(claim.ClID, AdmedikaConstant.PrintTypeID.LOC);
                string printMessage;
                if (service.IsOk(print, out printMessage) && !string.IsNullOrWhiteSpace(print.PrintOut))
                {
                    claim.PrintOutLOC = print.PrintOut;
                    claim.PrintOut = print.PrintOut;
                }

                claim.IsClosed = true;
                claim.DischargeDateTime = DateTime.Now;
                claim.ServiceID = AdmedikaConstant.ServiceID.DischargeOp;
                btnProcessDischarge.Enabled = false;
                btnRefreshEntitlement.Enabled = false;
                btnPrintLOC.Enabled = !string.IsNullOrWhiteSpace(claim.PrintOutLOC);
            }

            claim.Save();
            litResponse.Text = BuildStoredResponseHtml(claim);
            MarkParentForRefresh();
            ShowInformationHeader("Request awal tidak pasti; status claim berhasil direkonsiliasi menjadi " +
                                  status.ClStatus + " - " + status.ClDesc + ".");
        }

        private static void SaveDischargeResult(AdmedikaClaimTransaction claim,
            DischargeOpResponse result, string diagnoses)
        {
            claim.TransactionNo = string.IsNullOrWhiteSpace(result.ProviderTransID)
                ? claim.RegistrationNo
                : result.ProviderTransID;
            claim.DiagnosisCodeList = diagnoses;
            claim.ClStatus = result.ClStatus;
            claim.ClDesc = result.ClDesc;
            claim.ServiceID = AdmedikaConstant.ServiceID.DischargeOp;
            claim.PrintOut = result.PrintOut;

            if (result.ClStatus == AdmedikaConstant.ClaimStatusID.ClaimsRejected ||
                result.ClStatus == AdmedikaConstant.ClaimStatusID.ReimbursementDenied)
                claim.PrintOutLOR = result.PrintOut;
            else
                claim.PrintOutLOC = result.PrintOut;

            decimal amount;
            claim.TotAmtIncurred = TryParseAmount(result.TotAmtIncurred, out amount)
                ? amount
                : (decimal?)null;
            claim.TotAmtApproved = TryParseAmount(result.TotAmtApproved, out amount)
                ? amount
                : (decimal?)null;
            claim.TotAmtNotApproved = TryParseAmount(result.TotAmtNotApproved, out amount)
                ? amount
                : (decimal?)null;

            if (!string.IsNullOrWhiteSpace(result.PatientName))
                claim.PatientName = result.PatientName;

            claim.IsClosed = result.ClStatus == AdmedikaConstant.ClaimStatusID.ValidClaims;
            claim.DischargeDateTime = claim.IsClosed == true ? DateTime.Now : (DateTime?)null;
            claim.Save();
        }

        private void MarkParentForRefresh()
        {
            AjaxManager.ResponseScripts.Add(
                "var oWnd = GetRadWindow();" +
                "if (oWnd) { oWnd.argument = oWnd.argument || {}; oWnd.argument.mode = 'rebind'; }");
        }

        private bool BenefitExists(string benID)
        {
            foreach (DataRow row in BenefitTable.Rows)
            {
                if (string.Equals(Convert.ToString(row["BenID"]), benID,
                    StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private AdmedikaClaimTransaction LoadClaim()
        {
            var claim = new AdmedikaClaimTransaction();
            return ClaimTransactionID > 0 && claim.LoadByPrimaryKey(ClaimTransactionID)
                ? claim
                : null;
        }

        private void DisableProcess()
        {
            btnProcessDischarge.Enabled = false;
            btnRefreshEntitlement.Enabled = false;
            btnPrintLOC.Enabled = false;
        }

        private void SetEntitlementMessage(string message, bool isError)
        {
            lblEntitlementMessage.Text = message;
            lblEntitlementMessage.ForeColor = isError
                ? System.Drawing.Color.Red
                : System.Drawing.Color.Green;
        }

        private static DataTable CreateBenefitTable()
        {
            var table = new DataTable();
            table.Columns.Add("BenID", typeof(string));
            table.Columns.Add("ShortName", typeof(string));
            table.Columns.Add("LongName", typeof(string));
            table.Columns.Add("DisplayName", typeof(string));
            return table;
        }

        private static string BuildEntitlementHtml(DataTable table)
        {
            if (table == null || table.Rows.Count == 0)
                return "<div style=\"padding:6px;\">Tidak ada entitlement.</div>";

            var html = new StringBuilder();
            html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\" border=\"1\" cellpadding=\"4\" cellspacing=\"0\">");
            html.Append("<thead><tr style=\"background:#f0f0f0;\"><th>BenID</th><th>Short Name</th><th>Long Name</th></tr></thead><tbody>");
            foreach (DataRow row in table.Rows)
            {
                html.Append("<tr><td>").Append(Encode(Convert.ToString(row["BenID"]))).Append("</td><td>")
                    .Append(Encode(Convert.ToString(row["ShortName"]))).Append("</td><td>")
                    .Append(Encode(Convert.ToString(row["LongName"]))).Append("</td></tr>");
            }
            html.Append("</tbody></table>");
            return html.ToString();
        }

        private static string BuildResponseHtml(DischargeOpResponse response)
        {
            var html = new StringBuilder();
            html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\">");
            AppendSummaryRow(html, "Claim ID", response.ClID);
            AppendSummaryRow(html, "Status", Join(response.ClStatus, response.ClDesc));
            AppendSummaryRow(html, "Total Incurred", response.TotAmtIncurred);
            AppendSummaryRow(html, "Approved", response.TotAmtApproved);
            AppendSummaryRow(html, "Patient Excess", response.TotAmtNotApproved);
            html.Append("</table>");

            if (response.Entitlement != null && response.Entitlement.Count > 0)
            {
                html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;margin-top:8px;\" border=\"1\" cellpadding=\"4\" cellspacing=\"0\">");
                html.Append("<thead><tr style=\"background:#f0f0f0;\"><th>Benefit</th><th>Incurred</th><th>Approved</th><th>Not Approved</th></tr></thead><tbody>");
                foreach (var benefit in response.Entitlement)
                {
                    html.Append("<tr><td>").Append(Encode((benefit.BenID ?? string.Empty).Trim()))
                        .Append(" - ").Append(Encode(benefit.ShortName)).Append("</td><td align=\"right\">")
                        .Append(Encode(benefit.AmtIncurred)).Append("</td><td align=\"right\">")
                        .Append(Encode(benefit.AmtApproved)).Append("</td><td align=\"right\">")
                        .Append(Encode(benefit.AmtNotApproved)).Append("</td></tr>");
                }
                html.Append("</tbody></table>");
            }

            return html.ToString();
        }

        private static string BuildStoredResponseHtml(AdmedikaClaimTransaction claim)
        {
            var html = new StringBuilder();
            html.Append("<table style=\"width:100%;border-collapse:collapse;font-size:12px;\">");
            AppendSummaryRow(html, "Claim ID", claim.ClID);
            AppendSummaryRow(html, "Status", Join(claim.ClStatus, claim.ClDesc));
            AppendSummaryRow(html, "Total Incurred", FormatAmount(claim.TotAmtIncurred));
            AppendSummaryRow(html, "Approved", FormatAmount(claim.TotAmtApproved));
            AppendSummaryRow(html, "Patient Excess", FormatAmount(claim.TotAmtNotApproved));
            html.Append("</table>");
            return html.ToString();
        }

        private static string BuildErrorHtml(AdmedikaResponse response, string message)
        {
            var html = new StringBuilder();
            html.Append("<div style=\"border:1px solid #d9534f;background:#f2dede;color:#a94442;padding:8px;\">")
                .Append(Encode(message)).Append("</div>");
            if (response != null)
            {
                html.Append("<table style=\"width:100%;font-size:12px;margin-top:6px;\">");
                AppendSummaryRow(html, "Reference ID", response.ReferenceID);
                AppendSummaryRow(html, "Status", Join(response.StatusCode, response.StatusMsg));
                AppendSummaryRow(html, "Error", Join(response.ErrorCode, response.ErrorDesc));
                AppendSummaryRow(html, "Technical Error", response.TechnicalError);
                html.Append("</table>");
            }
            return html.ToString();
        }

        private static string BuildPrintOutHtml(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return "<div style=\"margin-top:8px;font-weight:bold;\">Print Out LOC</div>" +
                   "<pre style=\"white-space:pre-wrap;border:1px solid #ccc;padding:8px;max-height:320px;overflow:auto;\">" +
                   Encode(value) + "</pre>";
        }

        private static void AppendSummaryRow(StringBuilder html, string label, string value)
        {
            html.Append("<tr><td style=\"width:28%;padding:4px;border-bottom:1px solid #eee;font-weight:bold;\">")
                .Append(Encode(label)).Append("</td><td style=\"padding:4px;border-bottom:1px solid #eee;\">")
                .Append(Encode(Value(value))).Append("</td></tr>");
        }

        private static string NormalizeDiagnosisList(string value)
        {
            var result = new List<string>();
            foreach (var part in (value ?? string.Empty).Split(','))
            {
                var code = part.Trim().ToUpperInvariant();
                if (code.Length > 0 && !result.Contains(code)) result.Add(code);
            }
            return string.Join(", ", result.ToArray());
        }

        private static string ItemKey(object sourceType, object transactionNo, object sequenceNo)
        {
            return Convert.ToString(sourceType) + "|" + Convert.ToString(transactionNo) + "|" +
                   Convert.ToString(sequenceNo);
        }

        private static string Limit(string value, int length)
        {
            value = (value ?? string.Empty).Trim();
            return value.Length <= length ? value : value.Substring(0, length);
        }

        private static bool TryParseAmount(string value, out decimal amount)
        {
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture,
                out amount);
        }

        private static string FormatAmount(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("n2") : null;
        }

        private static string Join(string left, string right)
        {
            if (string.IsNullOrWhiteSpace(left)) return right;
            if (string.IsNullOrWhiteSpace(right)) return left;
            return left + " - " + right;
        }

        private static string Value(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value;
        }

        private static string Encode(string value)
        {
            return HttpUtility.HtmlEncode(value ?? string.Empty);
        }
    }
}
