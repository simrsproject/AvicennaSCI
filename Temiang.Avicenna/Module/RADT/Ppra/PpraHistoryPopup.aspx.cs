using System;
using System.Data;
using System.Text;
using System.Web;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Ppra
{
    public partial class PpraHistoryPopup : BasePageDialog
    {
        private string RegistrationNo
        {
            get { return Request.QueryString["regno"] ?? string.Empty; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.Ppra;

            if (!IsPostBack)
                PopulateHistory();
        }

        protected void Page_Load(object sender, EventArgs e) { }

        private void PopulateHistory()
        {
            if (string.IsNullOrWhiteSpace(RegistrationNo))
                return;

            // Header pasien
            var reg = new Registration();
            if (reg.LoadByPrimaryKey(RegistrationNo))
            {
                var patient = new Patient();
                patient.LoadByPrimaryKey(reg.PatientID);
                litHeader.Text = string.Format("{0} [{1}] &mdash; {2}",
                    HttpUtility.HtmlEncode(patient.PatientName),
                    HttpUtility.HtmlEncode(patient.MedicalNo),
                    HttpUtility.HtmlEncode(RegistrationNo));
            }

            // Load semua resep PPRA (pending, approved, rejected)
            var tpQuery = new TransPrescriptionQuery("tp");
            var rrQuery = new RegistrationRasproQuery("rr");

            tpQuery.InnerJoin(rrQuery).On(
                rrQuery.RegistrationNo == tpQuery.RegistrationNo
                & rrQuery.SeqNo == tpQuery.RasproSeqNo);

            tpQuery.Select(
                tpQuery.PrescriptionNo,
                tpQuery.PrescriptionDate,
                tpQuery.IsApproval,
                tpQuery.IsPpraApproved,
                tpQuery.IsPpraRejected,
                tpQuery.PpraRejectionReason,
                tpQuery.IsVoid
            );

            tpQuery.Where(
                tpQuery.RegistrationNo == RegistrationNo,
                tpQuery.Or(tpQuery.IsVoid.IsNull(), tpQuery.IsVoid == false),
                rrQuery.AbRestrictionID.Like(AbRestriction.NonPpabID + "%")
            );
            tpQuery.OrderBy(tpQuery.PrescriptionDate.Descending);

            var dtb = tpQuery.LoadDataTable();

            if (dtb.Rows.Count == 0)
            {
                litHistory.Text = "<div style='color:#888;padding:8px;'>Tidak ada riwayat resep Non PPAB.</div>";
                return;
            }

            var sb = new StringBuilder();
            foreach (DataRow row in dtb.Rows)
            {
                var prescNo   = row["PrescriptionNo"].ToString();
                var prescDate = row["PrescriptionDate"] != DBNull.Value
                    ? Convert.ToDateTime(row["PrescriptionDate"]).ToString(AppConstant.DisplayFormat.DateShortMonthHourMinute)
                    : string.Empty;
                var isApproved    = row["IsPpraApproved"] != DBNull.Value && Convert.ToBoolean(row["IsPpraApproved"]);
                var isRejected    = row["IsPpraRejected"] != DBNull.Value && Convert.ToBoolean(row["IsPpraRejected"]);
                var isPending     = !isApproved && !isRejected;
                var reason        = row["PpraRejectionReason"] != DBNull.Value ? row["PpraRejectionReason"].ToString() : string.Empty;

                // Status badge
                string statusHtml;
                if (isRejected)
                    statusHtml = "<span style='background:#d9534f;color:#fff;padding:2px 6px;border-radius:3px;font-size:11px;font-weight:bold;'>&#9888; Ditolak</span>";
                else if (isApproved)
                    statusHtml = "<span style='background:#5cb85c;color:#fff;padding:2px 6px;border-radius:3px;font-size:11px;font-weight:bold;'>&#10003; Disetujui</span>";
                else
                    statusHtml = "<span style='background:#f0ad4e;color:#fff;padding:2px 6px;border-radius:3px;font-size:11px;font-weight:bold;'>&#9203; Pending</span>";

                sb.Append("<div style='border:1px solid #ddd;border-radius:4px;margin-bottom:6px;overflow:hidden;'>");

                // Row header
                sb.AppendFormat(
                    "<div style='background:#f5f5f5;padding:5px 8px;display:flex;justify-content:space-between;align-items:center;'>" +
                    "<span style='font-weight:bold;font-size:12px;'>{0}</span>" +
                    "<span style='color:#777;font-size:11px;'>{1}</span>" +
                    "{2}" +
                    "</div>",
                    HttpUtility.HtmlEncode(prescNo), prescDate, statusHtml);

                // Alasan penolakan
                if (isRejected && !string.IsNullOrWhiteSpace(reason))
                {
                    sb.AppendFormat(
                        "<div style='background:#fff0f0;padding:5px 8px;color:#d9534f;font-size:11px;'>" +
                        "<b>Alasan:</b> {0}</div>",
                        HttpUtility.HtmlEncode(reason));
                }

                sb.Append("</div>");
            }

            litHistory.Text = sb.ToString();
        }
    }
}
