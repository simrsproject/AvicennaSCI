using System;
using System.Data;
using System.Text;
using System.Web;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Reference;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Module.RADT.Emr;

namespace Temiang.Avicenna.Module.RADT.Ppra
{
    public partial class PpraReview : BasePageDialog
    {
        #region QueryString Properties

        private string PrescriptionNo
        {
            get { return Request.QueryString["prescno"] ?? string.Empty; }
        }

        #endregion

        #region Page Events

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.Ppra;

            if (!IsPostBack)
                PopulateAll();
        }

        protected void Page_Load(object sender, EventArgs e) { }

        #endregion

        #region Populate

        private void PopulateAll()
        {
            var presc = new TransPrescription();
            if (!presc.LoadByPrimaryKey(PrescriptionNo))
                return;

            PopulatePatientInfo(presc.RegistrationNo);
            PopulatePendingPrescriptions(presc.RegistrationNo);
            PopulateRasproForm(presc.RegistrationNo, presc.RasproSeqNo ?? 0);
            soapInfoCtl.PopulateSoap(presc.RegistrationNo);
            grdLab.Rebind();
            grdRad.Rebind();
        }

        private void PopulatePatientInfo(string registrationNo)
        {
            var reg = new Registration();
            if (!reg.LoadByPrimaryKey(registrationNo))
                return;

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);

            var room = new ServiceRoom();
            room.LoadByPrimaryKey(reg.RoomID);

            var unit = new ServiceUnit();
            unit.LoadByPrimaryKey(reg.ServiceUnitID);

            var doctor = new Paramedic();
            doctor.LoadByPrimaryKey(reg.ParamedicID);

            var guarantor = new Guarantor();
            guarantor.LoadByPrimaryKey(reg.GuarantorID);

            litPatientHeader.Text = string.Format("{0} [{1}]",
                patient.PatientName, patient.MedicalNo);

            litMedicalNo.Text        = patient.MedicalNo;
            litRegistrationNo.Text   = registrationNo;
            litRegistrationDate.Text = reg.RegistrationDate.HasValue
                ? reg.RegistrationDate.Value.ToString(AppConstant.DisplayFormat.Date)
                : string.Empty;
            litParamedicName.Text    = doctor.ParamedicName;
            litGuarantorName.Text    = guarantor.GuarantorName;
            litServiceUnit.Text      = unit.ServiceUnitName;
            litRoomBed.Text          = string.Format("R: {0} / B: {1}", room.RoomName, reg.BedID);
            litGender.Text           = patient.Sex;
            litDobAge.Text           = patient.DateOfBirth.HasValue
                ? string.Format("{0} ({1}Y {2}M {3}D)",
                    patient.DateOfBirth.Value.ToString("dd-MMM-yyyy"),
                    Helper.GetAgeInYear(patient.DateOfBirth.Value),
                    Helper.GetAgeInMonth(patient.DateOfBirth.Value),
                    Helper.GetAgeInDay(patient.DateOfBirth.Value))
                : string.Empty;

            // Drug allergies
            var allergies = string.Empty;
            var pacoll = new PatientAllergyCollection();
            pacoll.Query.Where(
                pacoll.Query.PatientID == reg.PatientID,
                pacoll.Query.AllergyGroup == AppSession.Parameter.DrugAllergenGroupID
            );
            pacoll.Query.OrderBy(pacoll.Query.AllergenName.Ascending);
            pacoll.LoadAll();
            foreach (var pa in pacoll)
            {
                var item = pa.DescAndReaction.Replace("{", "(").Replace("}", ")");
                allergies = string.IsNullOrEmpty(allergies) ? item : allergies + ", " + item;
            }
            litAllergies.Text = string.IsNullOrEmpty(allergies)
                ? "<span style='color:#888;'>None recorded</span>"
                : "<span style='color:#d9534f;font-weight:bold;'>" + HttpUtility.HtmlEncode(allergies) + "</span>";
        }

        private void PopulatePendingPrescriptions(string registrationNo)
        {
            var tpQuery = new TransPrescriptionQuery("tp");
            var rrQuery = new RegistrationRasproQuery("rr");
            var itemQuery = new TransPrescriptionItemQuery("tpi");
            var medicQuery = new ItemQuery("itm");

            tpQuery.InnerJoin(rrQuery).On(
                rrQuery.RegistrationNo == tpQuery.RegistrationNo
                & rrQuery.SeqNo == tpQuery.RasproSeqNo);
            tpQuery.LeftJoin(itemQuery).On(itemQuery.PrescriptionNo == tpQuery.PrescriptionNo);
            tpQuery.LeftJoin(medicQuery).On(medicQuery.ItemID == itemQuery.ItemID);

            tpQuery.Select(
                tpQuery.PrescriptionNo,
                tpQuery.PrescriptionDate,
                tpQuery.Note,
                tpQuery.IsPpraRejected,
                tpQuery.PpraRejectionReason,
                "<ISNULL(STRING_AGG(itm.ItemName, ', '), '-') AS AntibioticList>"
            );

            tpQuery.Where(
                tpQuery.RegistrationNo == registrationNo,
                tpQuery.Or(tpQuery.IsPpraApproved.IsNull(), tpQuery.IsPpraApproved == false),
                tpQuery.Or(tpQuery.IsApproval.IsNull(), tpQuery.IsApproval == false),
                tpQuery.Or(tpQuery.IsVoid.IsNull(), tpQuery.IsVoid == false),
                tpQuery.Or(tpQuery.IsPpraRejected.IsNull(), tpQuery.IsPpraRejected == false),
                rrQuery.AbRestrictionID == AbRestriction.NonPpabID
            );
            tpQuery.GroupBy(
                tpQuery.PrescriptionNo,
                tpQuery.PrescriptionDate,
                tpQuery.Note,
                tpQuery.IsPpraRejected,
                tpQuery.PpraRejectionReason
            );
            tpQuery.OrderBy(tpQuery.PrescriptionDate.Descending);

            var dtb = tpQuery.LoadDataTable();

            if (dtb.Rows.Count == 0)
            {
                litPendingPrescriptions.Text = "<div style='padding:6px;color:#888;'>No pending Non PPAB prescriptions.</div>";
                return;
            }

            var urlRoot = Helper.UrlRoot();
            var sb = new StringBuilder();
            foreach (DataRow row in dtb.Rows)
            {
                var prescNo       = row["PrescriptionNo"].ToString();
                var prescDate     = row["PrescriptionDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["PrescriptionDate"]).ToString(AppConstant.DisplayFormat.DateShortMonthHourMinute)
                    : string.Empty;
                var note          = row["Note"] != DBNull.Value ? row["Note"].ToString() : string.Empty;
                var abList        = row["AntibioticList"].ToString();

                sb.Append("<table style='width:100%;background:#f9f9f9;border:1px solid #ddd;margin-bottom:6px;'>");
                sb.AppendFormat("<tr style='background:#e8f0fe;'><td colspan='2' style='padding:5px 8px;font-weight:bold;'>{0} &nbsp; <span style='color:#555;font-weight:normal;'>{1}</span></td></tr>",
                    HttpUtility.HtmlEncode(prescNo), prescDate);
                sb.AppendFormat("<tr><td class='label' style='width:140px;padding:4px 8px;'>Antibiotic(s)</td><td style='padding:4px 8px;color:#d9534f;font-weight:bold;'>{0}</td></tr>",
                    HttpUtility.HtmlEncode(abList));
                if (!string.IsNullOrWhiteSpace(note))
                    sb.AppendFormat("<tr><td class='label' style='padding:4px 8px;'>Note</td><td style='padding:4px 8px;'>{0}</td></tr>",
                        HttpUtility.HtmlEncode(note));
                sb.AppendFormat(
                    "<tr><td colspan='2' style='padding:6px 8px;'>" +
                    "<a href='#' onclick=\"approveNonPpabPrescription('{0}'); return false;\" style='margin-right:10px;'>" +
                    "<img src='{1}/Images/Toolbar/post16.png' border='0' /> Approve</a>" +
                    "<a href='#' onclick=\"rejectNonPpabPrescription('{0}'); return false;\" style='color:#d9534f;'>" +
                    "<img src='{1}/Images/Toolbar/delete16.png' border='0' /> Reject</a>" +
                    "</td></tr>",
                    prescNo, urlRoot);
                sb.Append("</table>");
            }
            litPendingPrescriptions.Text = sb.ToString();
        }

        private void PopulateRasproForm(string registrationNo, int rasproSeqNo)
        {
            if (rasproSeqNo <= 0)
            {
                litRasproForm.Text = "<div style='padding:6px;color:#888;'>No PPRA form found for this registration.</div>";
                return;
            }

            var rr = new RegistrationRaspro();
            if (!rr.LoadByPrimaryKey(registrationNo, rasproSeqNo))
            {
                litRasproForm.Text = "<div style='padding:6px;color:#888;'>PPRA form not found.</div>";
                return;
            }

            var sb = new StringBuilder();

            // Form type & restriction info
            var stdi = StandardReference.LoadStandardReferenceItem(AppEnum.StandardReference.RASPRO, rr.SRRaspro);
            var abr = new AbRestriction();
            abr.LoadByPrimaryKey(rr.AbRestrictionID);

            sb.Append("<table width='100%' style='margin-bottom:8px;'>");
            sb.AppendFormat("<tr><td class='label' width='180px'>Form Type</td><td><strong>{0}</strong></td></tr>",
                HttpUtility.HtmlEncode(stdi != null ? stdi.ItemName : rr.SRRaspro));
            sb.AppendFormat("<tr><td class='label'>Infection Focus</td><td>{0}</td></tr>",
                HttpUtility.HtmlEncode(abr != null ? abr.AbRestrictionName : rr.AbRestrictionID));
            sb.AppendFormat("<tr><td class='label'>Diagnose</td><td>{0}</td></tr>",
                HttpUtility.HtmlEncode(rr.Diagnose ?? string.Empty));
            if (rr.RasproDateTime.HasValue)
                sb.AppendFormat("<tr><td class='label'>Date</td><td>{0}</td></tr>",
                    rr.RasproDateTime.Value.ToString(AppConstant.DisplayFormat.DateShortMonthHourMinute));
            sb.Append("</table>");

            // Antibiotic suggestion
            var usedSeqNo = 0;
            var suggestion = AbRestriction.AntibioticSuggestion(rr, ref usedSeqNo);
            if (!string.IsNullOrWhiteSpace(suggestion))
            {
                sb.Append("<fieldset style='margin-bottom:6px;'><legend>Antibiotic Suggestion</legend>");
                sb.Append(suggestion);
                sb.Append("</fieldset>");
            }

            // Form lines (answered observations)
            var rrlColl = new RegistrationRasproLineCollection();
            rrlColl.Query.Where(
                rrlColl.Query.RegistrationNo == registrationNo,
                rrlColl.Query.SeqNo == rasproSeqNo
            );
            rrlColl.LoadAll();

            if (rrlColl.Count > 0)
                sb.Append(RasproForm.PreviouseSpecificationHtml(rr, rrlColl, rrlColl.Count));

            litRasproForm.Text = sb.ToString();
        }

        #endregion

        #region Grid Data Sources

        protected void grdLab_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdLab.DataSource = LoadExamOrderTable("LAB");
        }

        protected void grdRad_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdRad.DataSource = LoadExamOrderTable("RAD");
        }

        private DataTable LoadExamOrderTable(string transType)
        {
            var presc = new TransPrescription();
            if (!presc.LoadByPrimaryKey(PrescriptionNo))
                return new DataTable();

            var reg = new Registration();
            if (!reg.LoadByPrimaryKey(presc.RegistrationNo))
                return new DataTable();

            var tciQuery = new TransChargesItemQuery("a");
            var tcQuery  = new TransChargesQuery("b");
            var itemQuery = new ItemQuery("c");
            var toUnit   = new ServiceUnitQuery("tu");
            var regQuery = new RegistrationQuery("f");

            tciQuery.InnerJoin(tcQuery).On(tciQuery.TransactionNo == tcQuery.TransactionNo);
            tciQuery.InnerJoin(itemQuery).On(tciQuery.ItemID == itemQuery.ItemID);
            tciQuery.InnerJoin(toUnit).On(tcQuery.ToServiceUnitID == toUnit.ServiceUnitID);
            tciQuery.InnerJoin(regQuery).On(tcQuery.RegistrationNo == regQuery.RegistrationNo);

            tciQuery.Select(
                tcQuery.TransactionNo,
                tcQuery.ExecutionDate.As("TransactionDate"),
                itemQuery.ItemName,
                tciQuery.ResultValue.Coalesce("''")
            );

            tciQuery.Where(
                regQuery.RegistrationNo == presc.RegistrationNo,
                tcQuery.IsVoid == false,
                tcQuery.IsCorrection == false,
                tciQuery.ParentNo == ""
            );

            switch (transType)
            {
                case "LAB":
                    tciQuery.Where(
                        tcQuery.IsOrder == true,
                        tciQuery.Or(
                            tcQuery.ToServiceUnitID == AppSession.Parameter.ServiceUnitLaboratoryID,
                            tcQuery.ToServiceUnitID.In(AppSession.Parameter.ServiceUnitLaboratoryIdArray)
                        )
                    );
                    break;
                case "RAD":
                    tciQuery.Where(
                        tcQuery.IsOrder == true,
                        tciQuery.Or(
                            tcQuery.ToServiceUnitID == AppSession.Parameter.ServiceUnitRadiologyID,
                            tcQuery.ToServiceUnitID == AppSession.Parameter.ServiceUnitRadiologyID2,
                            tcQuery.ToServiceUnitID.In(AppSession.Parameter.ServiceUnitRadiologyIdArray)
                        )
                    );
                    break;
            }

            tciQuery.OrderBy(tcQuery.ExecutionDate.Descending);
            return tciQuery.LoadDataTable();
        }

        #endregion

        #region Approve / Reject

        protected void btnAction_Click(object sender, EventArgs e)
        {
            var prescNo = hdnPrescNo.Value;
            var actionRaw = hdnAction.Value;

            if (string.IsNullOrWhiteSpace(prescNo) || string.IsNullOrWhiteSpace(actionRaw))
                return;

            if (actionRaw == "approve")
            {
                var presc = new TransPrescription();
                if (!presc.LoadByPrimaryKey(prescNo) || !IsPendingNonPpabPrescription(presc))
                {
                    Helper.ShowMessageAfterPostback(this, "Resep tidak ditemukan atau tidak dapat diapprove.");
                    return;
                }

                presc.IsPpraApproved      = true;
                presc.IsPpraRejected      = false;
                presc.PpraRejectionReason = string.Empty;
                presc.IsApproval          = true;
                presc.ApprovalDateTime    = (new DateTime()).NowAtSqlServer();
                presc.ApprovedByUserID    = AppSession.UserLogin.UserID;
                presc.Save();

                TransPrescription.SoapeUpdatePrescriptionHist(
                    presc.ParamedicID, presc.RegistrationNo, presc.PrescriptionDate ?? DateTime.Now);

                Helper.ShowMessageAfterPostback(this, "Resep Non PPAB sudah disetujui dan diteruskan ke Farmasi.");
                CloseAndRefreshParent();
            }
            else if (actionRaw.StartsWith("reject|"))
            {
                var reason = HttpUtility.UrlDecode(actionRaw.Substring("reject|".Length));
                if (string.IsNullOrWhiteSpace(reason)) return;

                var presc = new TransPrescription();
                if (!presc.LoadByPrimaryKey(prescNo) || !IsPendingNonPpabPrescription(presc))
                {
                    Helper.ShowMessageAfterPostback(this, "Resep tidak ditemukan atau tidak dapat ditolak.");
                    return;
                }

                presc.IsPpraRejected      = true;
                presc.IsPpraApproved      = false;
                presc.PpraRejectionReason = reason;
                presc.Save();

                Helper.ShowMessageAfterPostback(this, "Resep Non PPAB ditolak PPRA. Alasan penolakan akan muncul pada resep pasien.");
                CloseAndRefreshParent();
            }

            // Reset hidden fields
            hdnPrescNo.Value = string.Empty;
            hdnAction.Value  = string.Empty;
        }

        private void CloseAndRefreshParent()
        {
            // Setelah alert di-dismiss, tutup popup dan rebind grid di parent (PpraDesktop)
            const string script = "function f(){" +
                "var oWnd = GetRadWindow();" +
                "if(oWnd){" +
                "  var opener = oWnd.BrowserWindow;" +
                "  if(opener && opener.__doPostBack){" +
                "    oWnd.close();" +
                "    opener.__doPostBack('grdList','rebind');" +
                "  } else {" +
                "    oWnd.close();" +
                "  }" +
                "}" +
                "Sys.Application.remove_load(f);}" +
                "Sys.Application.add_load(f);";
            ScriptManager.RegisterStartupScript(this, GetType(), "closeRefresh", script, true);
        }

        private static bool IsPendingNonPpabPrescription(TransPrescription presc)
        {
            if (!AppSession.Parameter.IsNeedPpraApproval)
                return false;

            return AbRestriction.IsPendingNonPpab(presc);
        }

        #endregion
    }
}
