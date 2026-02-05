using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Telerik.Web.UI;
using Temiang.Avicenna.Bridging.SatuSehat.BusinessObject;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.BusinessObject.Reference;
using Temiang.Avicenna.Bridging.SatuSehat;
using Temiang.Avicenna.Bridging.SatuSehat.Common;
using RestSharp;
using System.Text.RegularExpressions;
using System.Configuration;
using MedicationPost = Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.MedicationPost;
using Temiang.Avicenna.Bridging.PCare.Common;


namespace Temiang.Avicenna.Module.RADT
{
    public partial class SatuSehatRawatInap : BasePage
    {
        private string _encounterID;
        private const string _dateFormat = "yyyy-MM-ddTHH:mm:ss";
        private string _satuSehatBridgingType = AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID); //"BridgingType-008";
        private string _organizationID = ConfigurationManager.AppSettings["SatuSehatOrganizationID"];// "100026631"; //Dev -> "10000208"; //RS Umum Daerah Tamansari
        private string[] _dayNames = { "Minggu", "Senin", "Selasa", "Rabu", "Kamis", "Jumat", "Sabtu" };
        //private string _gmt = string.Format("{0:00}", AppParameter.GetParameterValue(AppParameter.ParameterItem.GMT).ToInt());
        private int _gmt = 0 - AppParameter.GetParameterValue(AppParameter.ParameterItem.GMT).ToInt();

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ProgramID = AppConstant.Program.ProcessDataToSatuSehatIPR;

            if (!IsPostBack)
            {
                txtDate.SelectedDate = DateTime.Now;
                PopulateServiceUnit();
            }
        }

        private void PopulateServiceUnit()
        {
            // Hanya untuk non rajal
            var coll = new ServiceUnitCollection();
            var query = new ServiceUnitQuery("a");
            query.Where(query.SRRegistrationType == AppConstant.RegistrationType.InPatient);

            query.OrderBy(query.ServiceUnitName.Ascending);

            if (AppSession.UserLogin.SRUserType == "NRS")
            {
                var qusr = new AppUserServiceUnitQuery("u");
                query.InnerJoin(qusr).On(query.ServiceUnitID == qusr.ServiceUnitID);
                query.Where(qusr.UserID == AppSession.UserLogin.UserID);
            }
            coll.Load(query);

            cboServiceUnitID.Items.Clear();
            cboServiceUnitID.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
            foreach (ServiceUnit item in coll)
            {
                cboServiceUnitID.Items.Add(new RadComboBoxItem(item.ServiceUnitName, item.ServiceUnitID));
            }
        }
        private DataTable Registrations
        {
            get
            {
                var qr = new RegistrationQuery("r");
                var qp = new PatientQuery("p");
                qr.InnerJoin(qp).On(qr.PatientID == qp.PatientID);

                var qm = new ParamedicQuery("m");
                qr.LeftJoin(qm).On(qr.ParamedicID == qm.ParamedicID);

                var unit = new ServiceUnitQuery("s");
                qr.LeftJoin(unit).On(qr.ServiceUnitID == unit.ServiceUnitID);

                var guar = new GuarantorQuery("g");
                qr.InnerJoin(guar).On(qr.GuarantorID == guar.GuarantorID);

                var satuSehat = new SatuSehatKunjunganQuery("pc");
                qr.LeftJoin(satuSehat).On(qr.RegistrationNo == satuSehat.RegistrationNo);

                var pb = new PatientBridgingQuery("pb");
                qr.LeftJoin(pb).On(qr.PatientID == pb.PatientID);


                qr.es.Top = AppSession.Parameter.MaxResultRecord;

                // Sub Query Check status Prescription 
                var transPresc = new TransPrescriptionQuery("tp");
                transPresc.Select(transPresc.IsApproval);
                transPresc.Where(transPresc.RegistrationNo == qr.RegistrationNo, "<tp.IsApproval = 1>");
                transPresc.es.Top = 1;

                var GenCon = new PatientHealthRecordQuery("ph");
                GenCon.Select("<Cast('1' as BIT) as IsGenCons>");
                GenCon.Where(GenCon.RegistrationNo == qr.RegistrationNo, "<ph.QuestionFormID = (select a.ParameterValue from AppParameter as a where a.ParameterID ='QuestionFormIDGeneralConsent')>"); //&& GenCon.QuestionFormID == _GenCon);
                GenCon.es.Top = 1;

                // Sub Query Check status ICD-10
                var icd = new EpisodeDiagnoseQuery("icd");
                icd.Select("<CAST('1' as BIT) as IsIcd10>");
                icd.Where(icd.RegistrationNo == qr.RegistrationNo, "<icd.DiagnoseID > ''>");
                icd.es.Top = 1;

                // Sub Query Check status VitalSign
                var vs = new PatientHealthRecordLineQuery("vs");
                var qs = new QuestionQuery("q");
                vs.InnerJoin(qs).On(vs.QuestionID == qs.QuestionID);
                vs.Select("<CAST('1' as BIT) as IsVitalSign>");
                vs.Where(vs.RegistrationNo == qr.RegistrationNo, "<q.VitalSignID > ''>");
                vs.es.Top = 1;

                // Sub Query Check status SOAP
                var soap = new RegistrationInfoMedicQuery("soap");
                soap.Select("<CAST('1' as BIT) as IsSoap>");
                soap.Where(soap.RegistrationNo == qr.RegistrationNo);
                soap.es.Top = 1;

                // Sub Query Check status ICD-9
                var icd9 = new EpisodeProcedureQuery("icd9");
                icd9.Select("<CAST('1' as BIT) as IsIcd9>");
                icd9.Where(icd9.RegistrationNo == qr.RegistrationNo);
                icd9.es.Top = 1;

                // Sub Query Check status Education
                var edu = new PatientEducationLineQuery("edu");
                edu.Select("<CAST('1' as BIT) as IsEduDiet>");
                edu.Where(edu.RegistrationNo == qr.RegistrationNo, "<edu.SRPatientEducation = '004'>");//PatientEducation	004	Diet dan nutrisi
                edu.es.Top = 1;


                //var paramedicBridging = new ParamedicBridgingQuery("pcmd");
                //qr.LeftJoin(paramedicBridging).On(qr.ParamedicID == paramedicBridging.BridgingID & paramedicBridging.SRBridgingType == SatuSehatBridgingType);

                qr.Select
                    (
                        qp.PatientID,
                        qr.RegistrationNo,
                        qr.RegistrationDate,
                        qr.RegistrationTime,
                        qp.MedicalNo,
                        qp.PatientName,
                        qp.Sex,
                        qp.Ssn,
                        qm.ParamedicName,
                        unit.ServiceUnitName,
                        "<CONVERT(BIT, CASE WHEN pc.IsClosed = 1 THEN 0 ELSE 1 END) AS IsAllowProcess>",
                        guar.GuarantorName,
                        qr.GuarantorCardNo,
                        satuSehat.ErrorResponse,
                        satuSehat.EncounterID,
                        satuSehat.LastUpdateDateTime,
                        satuSehat.IsClosed,
                        string.Format("<IsVitalSign=COALESCE( ({0}),CAST('0' as BIT))>", vs.Parse()),
                        string.Format("<IsPrescription=COALESCE( ({0}),CAST('0' as BIT))>", transPresc.Parse()),
                        string.Format("<IsGenCons=COALESCE( ({0}),CAST('0' as BIT))>", GenCon.Parse()),
                        string.Format("<IsIcd10=COALESCE( ({0}),CAST('0' as BIT))>", icd.Parse()),
                        string.Format("<IsIcd9=COALESCE( ({0}),CAST('0' as BIT))>", icd9.Parse()),
                        string.Format("<IsSoap=COALESCE( ({0}),CAST('0' as BIT))>", soap.Parse()),
                        string.Format("<IsEduDiet=COALESCE( ({0}),CAST('0' as BIT))>", edu.Parse())
                        , qr.ParamedicID
                        , pb.BridgingID.As("PatientBridgingID")
                    );


                qr.Where(qr.SRRegistrationType == "IPR");

                if (!chkIncludeClosed.Checked)
                    qr.Where(qr.Or(satuSehat.IsClosed.IsNull(), satuSehat.IsClosed == false));

                if (!chkIncludeFailed.Checked)
                    qr.Where(qr.Or(satuSehat.ErrorResponse.IsNull(), satuSehat.ErrorResponse == ""));

                if (chkHideEmptyIcd10.Checked)
                {
                    var icdExist = new EpisodeDiagnoseQuery("icd");
                    icdExist.Select(icdExist.RegistrationNo);
                    icdExist.Where(icd.RegistrationNo == qr.RegistrationNo, "<icd.DiagnoseID > ''>");

                    qr.Where(qr.RegistrationNo.In(icdExist));
                }

                if (chkHideEmptySSN.Checked)
                {
                    qr.Where(qp.Ssn != "");
                }
                // remark dulu supaya data muncul semua
                if (!txtDate.IsEmpty)
                    qr.Where(qr.RegistrationDate == txtDate.SelectedDate);

                if (!string.IsNullOrWhiteSpace(cboServiceUnitID.SelectedValue))
                    qr.Where(qr.ServiceUnitID == cboServiceUnitID.SelectedValue);

                if (!string.IsNullOrWhiteSpace(cboParamedicID.SelectedValue))
                    qr.Where(qr.ParamedicID == cboParamedicID.SelectedValue);

                if (txtMedicalNo.Text != string.Empty)
                {
                    var searchMedNo = Helper.EscapeQuery(txtMedicalNo.Text);
                    var patIdSearchByMedNos = Helper.PatientIds(searchMedNo, false);
                    if (patIdSearchByMedNos != null && patIdSearchByMedNos.Length > 0)
                        qr.Where(qr.PatientID.In(patIdSearchByMedNos), qp.PatientID.In(patIdSearchByMedNos));
                    else
                        qr.Where(qr.PatientID == "0", qp.PatientID == "0");
                }


                if (txtRegistrationNo.Text != string.Empty)
                    qr.Where(qr.RegistrationNo == txtRegistrationNo.Text);

                if (txtPatientName.Text != string.Empty)
                {
                    var searchMedNo = Helper.EscapeQuery(txtPatientName.Text);
                    var patIdSearchByMedNos = Helper.PatientIds(searchMedNo, true);
                    if (patIdSearchByMedNos != null && patIdSearchByMedNos.Length > 0)
                        qr.Where(qr.PatientID.In(patIdSearchByMedNos), qp.PatientID.In(patIdSearchByMedNos));
                    else
                        qr.Where(qr.PatientID == "0", qp.PatientID == "0");
                }

                qr.Where
                    (
                        qr.IsVoid == false
                    );

                qr.OrderBy(qr.RegistrationNo.Ascending);

                var tbl = qr.LoadDataTable();

                return tbl;
            }
        }

        protected void grdRegisteredList_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdRegisteredList.DataSource = Registrations;
        }

        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {
            grdRegisteredList.CurrentPageIndex = 0; //Reset page
            grdRegisteredList.Rebind();
        }

        protected void ToggleSelectedState(object sender, EventArgs e)
        {
            SelectedState(((CheckBox)sender).Checked);
        }

        private void SelectedState(bool selected)
        {
            foreach (CheckBox chkBox in grdRegisteredList.MasterTableView.Items.Cast<GridDataItem>().Select(dataItem => (CheckBox)dataItem.FindControl("detailChkbox")).Where(chkBox => chkBox.Visible))
            {
                chkBox.Checked = selected;
            }
        }

        protected override void RaisePostBackEvent(IPostBackEventHandler source, string eventArgument)
        {
            base.RaisePostBackEvent(source, eventArgument);
            if ((source is RadGrid))
            {
                if (eventArgument == "process")
                {
                    //// Check apakah ada registrasi ke dokter yg tidak terdaftar di PCare 
                    //// Jika ada munculkan popup penggantinya
                    //var emptyParamedicBridgingID = string.Empty;
                    //foreach (
                    //    GridDataItem dataItem in
                    //    grdRegisteredList.MasterTableView.Items.Cast<GridDataItem>()
                    //        .Where(dataItem => ((CheckBox)dataItem.FindControl("detailChkbox")).Checked))
                    //{
                    //    if (dataItem["ParamedicBridgingID"].Text == "&nbsp" || string.IsNullOrEmpty(dataItem["ParamedicBridgingID"].Text))
                    //    {
                    //        emptyParamedicBridgingID = string.Concat(emptyParamedicBridgingID, "_", dataItem["ParamedicID"].Text);
                    //    }
                    //}


                    //disable dulu untuk development
                    var accessToken = string.Empty;
                    foreach (
                        GridDataItem dataItem in
                            grdRegisteredList.MasterTableView.Items.Cast<GridDataItem>()
                                .Where(dataItem => ((CheckBox)dataItem.FindControl("detailChkbox")).Checked))
                    {
                        //util.PostDataToSatuSehat(dataItem["RegistrationNo"].Text, ref accessToken);
                        var regNo = dataItem["RegistrationNo"].Text;
                        var ilpPrepColl = Temiang.Avicenna.Util.SatuSehatHelper.SatuSehatPreparation(regNo, 100, 2);
                        Util.SatuSehatHelper.SendToSatuSehat(regNo, ref accessToken);
                    }

                    grdRegisteredList.CurrentPageIndex = 0; //Reset page
                    grdRegisteredList.Rebind();
                }
                else if (eventArgument.StartsWith("closestatus"))
                {
                    var parts = eventArgument.Split('|');
                    var regno = parts[1];
                    var isClosedValue = parts.Length > 2 ? parts[2] : "1";

                    var kunjunganLog = new SatuSehatKunjungan();
                    if (!kunjunganLog.LoadByPrimaryKey(regno))
                    {
                        kunjunganLog = new SatuSehatKunjungan();
                        kunjunganLog.RegistrationNo = regno;
                    }

                    kunjunganLog.IsClosed = (isClosedValue == "1");
                    kunjunganLog.Save();

                    grdRegisteredList.Rebind();
                }
            }
        }
        protected void grdRegisteredList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                _encounterID = Convert.ToString(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["EncounterID"]);
            }

            //if (e.Item is GridNestedViewItem)
            //{
            //    if (string.IsNullOrEmpty(_encounterID)) return;

            //    //
            //    var result = new SatuSehatResultQuery("r");
            //    var guid = new Guid(_encounterID);
            //    result.Where(result.EncounterID == guid);
            //    result.OrderBy(result.IndexNo.Ascending);
            //    result.Select(result, string.Format("<'{0}' as HeaderEncounterID>", _encounterID));
            //    var dtb = result.LoadDataTable();

            //    // Populate 
            //    var grdResult = (RadGrid)e.Item.FindControl("grdResult");

            //    InitializeCultureGrid(grdResult); // Set date  format

            //    grdResult.DataSource = dtb;
            //    grdResult.Rebind();
            //}
        }

        protected void grdRegisteredList_ItemCommand(object sender, GridCommandEventArgs e)
        {
            return; // Masih terkendala dgn method yg memproses beberapa record
            if (e.CommandName == "Resend")
            {
                var accessToken = string.Empty;
                var util = new Bridging.SatuSehat.Utils();


                var args = e.CommandArgument.ToString().Split('|');
                var regNo = args[0];
                var encounterID = args[1];

                var reg = new Registration();
                reg.LoadByPrimaryKey(regNo);

                var pat = new Patient();
                pat.LoadByPrimaryKey(reg.PatientID);

                var patSs = new PatientBridging();
                patSs.LoadByPrimaryKey(reg.PatientID, _satuSehatBridgingType);

                switch (args[2].ToLower())
                {
                    case "Procedure":
                        {
                            util.PostProcedure(reg, patSs, encounterID, ref accessToken);
                            break;
                        }
                    default:
                        break;
                }
            }

        }
    }
}