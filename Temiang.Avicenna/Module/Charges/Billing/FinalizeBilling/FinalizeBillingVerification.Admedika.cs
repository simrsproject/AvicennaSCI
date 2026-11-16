using System;
using System.Data;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.Common.Admedika;

namespace Temiang.Avicenna.Module.Charges
{
    public partial class FinalizeBillingVerification
    {
        // Navigation only. Validation, API calls and persistence remain in the discharge dialog.
        protected void AdmedikaToolbar_PreRender(object sender, EventArgs e)
        {
            var button = RadToolBar2.FindItemByValue("admedikaDischarge");
            if (button == null) return;
            button.Visible = false;
            button.Attributes.Remove("claimTransactionID");

            if (string.IsNullOrWhiteSpace(txtRegistrationNo.Text) ||
                !new UserAccess(AppSession.UserLogin.UserID,
                    AppConstant.Program.AdmedikaDischargeOp).IsExist) return;

            var registration = new Registration();
            if (!registration.LoadByPrimaryKey(txtRegistrationNo.Text) ||
                registration.IsVoid == true ||
                registration.SRRegistrationType != AppConstant.RegistrationType.OutPatient) return;

            var query = new AdmedikaClaimTransactionQuery();
            query.Select(query.AdmedikaClaimTransactionID, query.ClStatus, query.IsClosed);
            query.Where(query.RegistrationNo == registration.RegistrationNo,
                query.CovID == AdmedikaConstant.CoverageID.GP);
            query.OrderBy(query.AdmedikaClaimTransactionID.Descending);
            var claims = query.LoadDataTable();
            if (claims.Rows.Count == 0) return;

            DataRow selected = null;
            var openClaims = 0;
            foreach (DataRow row in claims.Rows)
            {
                if (Convert.ToString(row["ClStatus"]) == AdmedikaConstant.ClaimStatusID.EligibilityApproved &&
                    (row["IsClosed"] == DBNull.Value || !Convert.ToBoolean(row["IsClosed"])))
                {
                    selected = row;
                    openClaims++;
                }
            }

            // Multiple open claims require explicit selection from the existing worklist.
            if (openClaims == 0) selected = claims.Rows[0];
            button.Visible = true;
            button.Enabled = openClaims <= 1;
            button.ToolTip = openClaims > 1
                ? "Lebih dari satu claim aktif. Pilih claim melalui Discharge OutPatient."
                : "Buka discharge AdMedika untuk registrasi ini";
            if (selected != null)
                button.Attributes["claimTransactionID"] = Convert.ToString(selected["AdmedikaClaimTransactionID"]);
        }
    }
}
