using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Reference;
using Temiang.Avicenna.Common;
using Temiang.Dal.DynamicQuery;
using Temiang.Dal.Interfaces;

namespace Temiang.Avicenna.Module.RADT.Bpjs
{
    public partial class CasemixPhysicianInChargeValidation : BasePage
    {
        private bool _isHideEmptySearchMessage = false;

        protected void Page_Init(object sender, EventArgs e)
        {
            _isHideEmptySearchMessage = false;
            if (Page.IsPostBack)
            {
                if (Request["__EVENTTARGET"].Contains("grd") &&
                    Request["__EVENTARGUMENT"].Contains("rebind"))
                {
                    _isHideEmptySearchMessage = true;
                }
            }
            ProgramID = AppConstant.Program.CasemixDPJPValidation;

            if (!IsPostBack)
            {
                txtOrderDate1.SelectedDate = DateTime.Now.Date;
                txtOrderDate2.SelectedDate = DateTime.Now.Date;
                var coll = new ServiceUnitCollection();
                coll.Query.Where(
                    coll.Query.SRRegistrationType.In(
                            AppConstant.RegistrationType.EmergencyPatient,
                            AppConstant.RegistrationType.InPatient,
                            AppConstant.RegistrationType.OutPatient,
                            AppConstant.RegistrationType.MedicalCheckUp
                        ),
                    coll.Query.IsActive == true
                );
                coll.Query.OrderBy(coll.Query.DepartmentID.Ascending);
                coll.LoadAll();

                cboServiceUnitID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                foreach (ServiceUnit entity in coll)
                {
                    cboServiceUnitID.Items.Add(new RadComboBoxItem(entity.ServiceUnitName, entity.ServiceUnitID));
                }
            }
            ComboBox.PopulateWithGuarantor(cboGuarantorID);
            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (AppSession.Parameter.HealthcareInitialAppsVersion != "RSSA")
                if (!IsPostBack) RestoreValueFromCookie();
        }

        private bool ValidateSearch(bool isEmptyFilter, string searchingLabel)
        {
            if (!IsListLoadRecordIfFiltered) return true;
            if (!IsPostBack) return false;
            if (!isEmptyFilter) return true;
            if (!_isHideEmptySearchMessage)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "invalid",
                    string.Format("alert('Please entry {0} searching criteria');", searchingLabel), true);
            }
            return false;
        }

        protected void cboServiceUnitID_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            ApplyServiceUnitID(e.Value);
        }

        private void ApplyServiceUnitID(string serviceUnitID)
        {
            cboParamedicID.Items.Clear();
            cboParamedicID.Text = string.Empty;
            if (!string.IsNullOrEmpty(serviceUnitID))
            {
                var unit = new ServiceUnit();
                if (unit.LoadByPrimaryKey(serviceUnitID))
                {
                    ComboBox.PopulateWithParamedic(cboParamedicID, serviceUnitID);
                }
            }
        }

        protected void grdList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var grd = (RadGrid)source;

            //if (!IsPostBack && !IsListLoadRecordOnInit)
            //{
            //    grd.DataSource = new String[] { };
            //    return;
            //}

            var dataSource = Registrations;
            if (dataSource == null)
                grd.DataSource = new String[] { }; // Clear rows
            else
            {
                if (!e.IsFromDetailTable)
                    grd.DataSource = dataSource;
            }
        }

        protected void btnFilter_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (AppSession.Parameter.HealthcareInitialAppsVersion != "RSSA")
                SaveValueToCookie();

            grdList.Rebind();
        }

        private DataTable Registrations
        {
            get
            {
                var isEmptyFilter = txtOrderDate1.IsEmpty && txtOrderDate2.IsEmpty && string.IsNullOrEmpty(cboServiceUnitID.SelectedValue) && string.IsNullOrEmpty(cboParamedicID.SelectedValue) &&
                    string.IsNullOrEmpty(txtRegistrationNo.Text) && string.IsNullOrEmpty(txtPatientName.Text) && string.IsNullOrEmpty(cboGuarantorID.SelectedValue);
                if (!ValidateSearch(isEmptyFilter, "Registration")) return null;

                var qr = new RegistrationQuery("r");
                var qp = new PatientQuery("p");
                var qm = new ParamedicQuery("m");
                var unit = new ServiceUnitQuery("s");
                var room = new ServiceRoomQuery("d");
                //var mrg = new MergeBillingQuery("b");
                var grr = new GuarantorQuery("c");
                var sal = new AppStandardReferenceItemQuery("sal");
                var mdsCmx = new MedicalDischargeSummaryCmxQuery("mdsCmx");
                //var sumInfo = new RegistrationInfoSumaryQuery("h");
                //var gdc = new GuarantorDocumentChecklistQuery("gdc");
                //var dc = new AppStandardReferenceItemQuery("dc");

                qr.es.Top = AppSession.Parameter.MaxResultRecord;
                qr.es.Distinct = true;

                qr.Select
                    (
                        qr.PatientID,
                        qr.RegistrationNo,
                        qr.RegistrationDate,
                        qr.RegistrationTime,
                        qp.MedicalNo,
                        //qp.PatientName,
                        "<(LTRIM(RTRIM(LTRIM(p.FirstName + ' ' + p.MiddleName)) + ' ' + p.LastName) + case when (r.IsNonPatient = 1 and ISNULL(r.DischargeNotes,'') <> '') then ' (['+r.DischargeMedicalNotes+'] ' + r.DischargeNotes + ')' else '' end) as PatientName>",
                        qp.Sex,
                        qm.ParamedicName,
                        unit.ServiceUnitName,
                        room.RoomName,
                        qr.BedID,
                        //qr.IsTransferedToInpatient,
                        qr.SRRegistrationType,
                        grr.GuarantorName,
                        //qr.IsConsul,
                        qr.ServiceUnitID,
                        qr.FromRegistrationNo,
                        sal.ItemName.As("SalutationName"),
                        qr.DischargeNotes, qr.DischargeMedicalNotes,
                        //qr.IsNonPatient,
                        @"<CAST(1 AS BIT) AS IsParamedicTeam>"
                        //@"<'' AS CashManagementNo>",
                        //@"<CASE WHEN h.NoteCount <= 0 THEN NULL ELSE h.NoteCount END AS NoteCount>",
                        //@"<CASE WHEN dc.LineNumber IS NULL OR (dc.LineNumber - h.DocumentCheckListCount) <= 0 THEN NULL ELSE (dc.LineNumber - h.DocumentCheckListCount) END AS DocumentCheckListCountRemains>"
                        //,
                        //mrg.FromRegistrationNo
                    );

                //if (AppSession.Parameter.IsShowArReceiptInVerificationAndPaymentList)
                //    qr.Select(@"<CASE WHEN (SELECT TOP 1 tp.PaymentNo FROM TransPayment tp 
                //                        INNER JOIN TransPaymentItem tpi ON tpi.PaymentNo = tp.PaymentNo
                //                    WHERE tp.RegistrationNo = r.RegistrationNo AND tp.TransactionCode = '016' AND tp.IsVoid = 0 AND tp.IsApproved = 1
                //                        AND tpi.SRPaymentType IN ('PaymentType-002', 'PaymentType-003', 'PaymentType-004')
                //            ) IS NULL THEN CAST(0 AS BIT) ELSE CAST(1 AS BIT) END AS 'IsArReceipt'>");
                //else
                //    qr.Select(@"<CAST(0 AS BIT) AS 'IsArReceipt'>");

                qr.InnerJoin(qp).On(qr.PatientID == qp.PatientID);
                qr.LeftJoin(qm).On(qr.ParamedicID == qm.ParamedicID);
                qr.LeftJoin(unit).On(qr.ServiceUnitID == unit.ServiceUnitID);
                qr.LeftJoin(room).On(qr.RoomID == room.RoomID);
                //qr.InnerJoin(mrg).On(qr.RegistrationNo == mrg.RegistrationNo);
                qr.InnerJoin(grr).On(qr.GuarantorID == grr.GuarantorID);
                qr.InnerJoin(mdsCmx).On(qr.RegistrationNo == mdsCmx.RegistrationNo);
                qr.LeftJoin(sal).On(sal.StandardReferenceID == "Salutation" & qp.SRSalutation == sal.ItemID);
                //qr.LeftJoin(sumInfo).On(qr.RegistrationNo == sumInfo.RegistrationNo);
                //qr.LeftJoin(gdc).On(qr.GuarantorID == gdc.GuarantorID & qr.SRRegistrationType == gdc.SRRegistrationType);
                //qr.LeftJoin(dc).On(dc.StandardReferenceID == "DocumentChecklist" & gdc.SRDocumentChecklist == dc.ItemID);

                if (!txtOrderDate1.IsEmpty && !txtOrderDate2.IsEmpty)
                    qr.Where(qr.RegistrationDate >= txtOrderDate1.SelectedDate, qr.RegistrationDate < txtOrderDate2.SelectedDate.Value.AddDays(1));
                if (cboServiceUnitID.SelectedValue != string.Empty)
                    qr.Where(qr.ServiceUnitID == cboServiceUnitID.SelectedValue);
                if (cboParamedicID.SelectedValue != string.Empty)
                    qr.Where(qr.ParamedicID == cboParamedicID.SelectedValue);
                if (txtRegistrationNo.Text != string.Empty)
                {
                    string searchReg = Helper.EscapeQuery(txtRegistrationNo.Text);
                    //qr.Where(
                    //    qr.Or(
                    //        qp.MedicalNo == searchReg,
                    //        qp.OldMedicalNo == searchReg,
                    //        qr.RegistrationNo == searchReg,
                    //        string.Format("< OR REPLACE(p.MedicalNo, '-', '') LIKE '%{0}%'>", searchReg),
                    //        string.Format("< OR REPLACE(p.OldMedicalNo, '-', '') LIKE '%{0}%'>", searchReg)
                    //        )
                    //    );
                    Helper.AddFilterMedNoOrRegNoOrPatName(qr, qp, searchReg, "registration");
                }
                if (txtPatientName.Text != string.Empty)
                {
                    string searchPatient = "%" + Helper.EscapeQuery(txtPatientName.Text) + "%";
                    qr.Where(qp.FullName.Like(searchPatient));
                    //qr.Where
                    //    (
                    //      string.Format("<(LTRIM(RTRIM(LTRIM(p.FirstName + ' ' + p.MiddleName)) + ' ' + p.LastName) + case when (r.IsNonPatient = 1 and ISNULL(r.DischargeNotes,'') <> '') then ' (['+r.DischargeMedicalNotes+'] ' + r.DischargeNotes + ')' else '' end) LIKE '{0}'>", searchPatient)
                    //    );
                }

                var group = new esQueryItem(qr, "Group", esSystemType.String);
                group = unit.ServiceUnitName;
                qr.Select(group.As("Group"));

                if (cboGuarantorID.SelectedValue != string.Empty)
                    qr.Where(qr.GuarantorID == cboGuarantorID.SelectedValue);

                //qr.Where(qr.IsClosed == false, qr.IsVoid == false, 
                //    qr.ServiceUnitID != AppSession.Parameter.ServiceUnitIDForCafe);

                qr.Where(qr.IsClosed == false, qr.IsVoid == false, mdsCmx.IsNeedDPJPValidation == true);

                //if (!AppSession.Parameter.IsSeparatePaymentForOpConsul)
                //    qr.Where(qr.Or(qr.IsConsul == false, mrg.FromRegistrationNo == string.Empty));

                qr.OrderBy(qr.RegistrationDate.Descending, qr.RegistrationNo.Ascending);

                DataTable tbl = qr.LoadDataTable();

                //var usr = new AppUser();
                //usr.LoadByPrimaryKey(AppSession.UserLogin.UserID);

                foreach (DataRow row in tbl.Rows)
                {
                    row["IsParamedicTeam"] = ParamedicTeam.IsParamedicTeamStatusDpjp(row["RegistrationNo"].ToString(), AppSession.UserLogin.ParamedicID);
                }

                tbl.AcceptChanges();

                return tbl;
            }
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
        {
            base.RaisePostBackEvent(sourceControl, eventArgument);

            if (string.IsNullOrEmpty(eventArgument))
                return;

            if (!(sourceControl is RadGrid))
                return;
            base.RaisePostBackEvent(sourceControl, eventArgument);
            var args = eventArgument.Split('|').Count() > 0 ? eventArgument.Split('|')[0] : eventArgument;
            switch (args)
            {
                case "rebind":
                    grdList.Rebind();
                    break;
            }
        }

    }
}
