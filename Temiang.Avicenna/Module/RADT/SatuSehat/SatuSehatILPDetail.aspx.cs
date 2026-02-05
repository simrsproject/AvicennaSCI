using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using DateTime = System.DateTime;

namespace Temiang.Avicenna.Module.RADT.Emr
{
    public partial class SatuSehatILPDetail : BasePageDialogEntry
    {
        private int TemplateID {
            get {
                var _tempID = Request.QueryString["templateid"];
                if(Helper.IsNumeric(_tempID)) return Convert.ToInt32(_tempID);
                return 1;    
            }
        }
        protected void Page_Init(object sender, EventArgs e)
        {
            ProgramID = AppConstant.Program.ElectronicMedicalRecord;
            ProgramReferenceID = "NURES";

            // Program Fiture
            IsSingleRecordMode = false; //Save then close
            ToolBar.NavigationVisible = false;
            ToolBar.ApprovalUnApprovalVisible = false;
            ToolBar.VoidUnVoidVisible = false;
            ToolBar.PrintVisible = false;
            ToolBar.DeleteVisible = false;

            ToolBar.EditVisible = true;
            ToolBar.AddVisible = false;
            // -------------------

            if (!IsPostBack)
            {
                var pat = new Patient();
                if (pat.LoadByPrimaryKey(PatientID))
                {
                    this.Title = "ILP Satu Sehat of : " + pat.PatientName + " (MRN: " + pat.MedicalNo + ", Reg No: " + RegistrationNo + ")";
                }
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            var ssIlpprep = new SatuSehatILPPreparation();
            ssIlpprep.LoadByPrimaryKey(RegistrationNo, TemplateID, "02.1", 1);
            if (!IsPostBack)
            {

            }
            CreateInput(true);
        }

        private void RegistrationInfo() {
            var reg = new Registration();
            reg.LoadByPrimaryKey(RegistrationNo);
            lblRegNo.InnerText = reg.RegistrationNo;
            lblRegDate.InnerText = reg.RegistrationDate.Value.ToString(AppConstant.DisplayFormat.DateCultureInfo.DateTimeFormat.ShortDatePattern);

            var su = new ServiceUnit();
            su.LoadByPrimaryKey(reg.ServiceUnitID);
            lblServiceUnit.InnerText = su.ServiceUnitName;

            var par = new Paramedic();
            par.LoadByPrimaryKey(reg.ParamedicID);
            lblPhysician.InnerText = par.ParamedicName;

            var pat = new Patient();
            pat.LoadByPrimaryKey(reg.PatientID);
            lblMRN.InnerText = pat.MedicalNo;
            lblPatientName.InnerText = pat.PatientName;
            lblBirthDate.InnerText = pat.DateOfBirth.Value.ToString(AppConstant.DisplayFormat.DateCultureInfo.DateTimeFormat.ShortDatePattern);
            lblSsn.InnerText = pat.Ssn;

            var patSs = new PatientBridging();
            patSs.LoadByPrimaryKey(pat.PatientID, AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID).ToLower());
            lblSSPatientID.InnerText = patSs.BridgingID;

            var encounterId = new SatuSehatKunjungan();
            encounterId.LoadByPrimaryKey(RegistrationNo);
            lblSSEncID.InnerText = encounterId.EncounterID.ToString();
        }

        private SatuSehatILPTemplateDetailCollection CreateInput(bool readOnly) {
            var ilpT = new BridgingTemplate();
            var ilpTdColl = new SatuSehatILPTemplateDetailCollection();

            var sspColl = new SatuSehatILPPreparationCollection();
            sspColl.Query.Where(sspColl.Query.RegistrationNo == RegistrationNo);
            sspColl.LoadAll();

            if (ilpT.LoadByPrimaryKey(TemplateID)) {
                lblFormName.InnerText = ilpT.TemplateName;
    
                ilpTdColl.Query.Where(ilpTdColl.Query.TemplateID == ilpT.TemplateID)
                    .OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
                ilpTdColl.LoadAll();
                foreach (var ilpTd in ilpTdColl)
                {
                    var tRow = new HtmlTableRow();

                    var tCell0 = new HtmlTableCell();
                    tCell0.Attributes.Add("class", "label");
                    tCell0.Attributes.Add("style", "width: 20px");

                    var lblNo = new Label(); lblNo.Text = ilpTd.TestNo;
                    tCell0.Controls.Add(lblNo);

                    tRow.Cells.Add(tCell0);

                    var tCell1 = new HtmlTableCell();
                    tCell1.Attributes.Add("class", "label");

                    var lbl = new Label(); lbl.Text = ilpTd.TaskDesc;
                    tCell1.Controls.Add(lbl);

                    tRow.Cells.Add(tCell1);

                    switch (ilpTd.SRAnswerType.ToLower()) {
                        case "lbl": {
                                var tCell2 = new HtmlTableCell();
                                tCell2.Attributes.Add("class", "label");
                                var lbl2 = new Label();
                                tCell2.Controls.Add(lbl2);
                                tRow.Cells.Add(tCell2);
                                break;
                            }
                        case "txt": {
                                InitializedRowText(ilpTd, tRow, readOnly || (!(ilpTd.IsEditable ?? false)));
                                break;
                            }
                        case "cbo": {
                                InitializedRowCbo(ilpTd, tRow, readOnly || (!(ilpTd.IsEditable ?? false)));
                                break;
                            }
                    }
                    
                    tblInput.Rows.Add(tRow);

                    var tCellSt = new HtmlTableCell();
                    tCellSt.Attributes.Add("class", "label");
                    tCellSt.Attributes.Add("style", "width: 20px");

                    var lblSt = new Label(); 
                    lblSt.Text = "";                    
                    tCellSt.Controls.Add(lblSt);
                    tRow.Cells.Add(tCellSt);

                    var tCellMsg = new HtmlTableCell();
                    tCellMsg.Attributes.Add("class", "label");
                    //tCellMsg.Attributes.Add("style", "width: 20px");

                    var lblMsg = new Label(); lblMsg.Text = "";
                    tCellMsg.Controls.Add(lblMsg);

                    tRow.Cells.Add(tCellMsg);

                    var ssp = sspColl.Where(x => x.TemplateID == ilpTd.TemplateID && x.TestNo == ilpTd.TestNo && x.Sequence == ilpTd.Sequence).FirstOrDefault();
                    if (ssp != null)
                    {
                        if (ssp.SentDateTime.HasValue)
                            lblSt.Text = ssp.SentDateTime.Value.ToString();
                        if (ssp.IsSent == true && ssp.IsError == false)
                        {
                            // bikin link + image
                            var lnk = new HyperLink();
                            lnk.NavigateUrl = "javascript:openSatuSehatRespondData('"
                                + ssp.RegistrationNo + "','"
                                + TemplateID + "','"
                                + ssp.TestNo + "');"; lnk.ToolTip = "Result View";
                            var img = new Image();
                            img.ImageUrl = "../../../Images/Toolbar/views16.png";
                            img.AlternateText = "Respond View";
                            img.BorderWidth = 0;

                            lnk.Controls.Add(img);
                            var lblSuccess = new Label();
                            lblSuccess.Text = "&nbsp;Data sent successfully, See details";

                            lnk.Controls.Add(img);
                            lnk.Controls.Add(lblSuccess);

                            lblMsg.Controls.Clear();
                            tCellMsg.Controls.Clear();
                            tCellMsg.Controls.Add(lnk);
                        }
                        else
                        {
                            // fallback ke text message
                            lblMsg.Text = ssp.RespondData;
                        }
                        //lblMsg.Text = ssp.RespondData;
                    }
                }
            }

            return ilpTdColl;
        }
        private void SetInputEnabled()
        {
            var ilpTdColl = new SatuSehatILPTemplateDetailCollection();
            ilpTdColl.Query.Where(ilpTdColl.Query.TemplateID == TemplateID)
                .OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
            ilpTdColl.LoadAll();

            foreach (var ilpTd in ilpTdColl)
            {
                switch (ilpTd.SRAnswerType.ToLower())
                {
                    case "txt":
                        var txt = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as TextBox;
                        if (txt != null)
                            txt.Enabled = ilpTd.IsEditable ?? false;
                        break;

                    case "cbo":
                        var cbo = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as RadComboBox;
                        if (cbo != null)
                            cbo.Enabled = ilpTd.IsEditable ?? false;
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isReadOnly"></param>
        /// <param name="replaceFlag">0:Skip replace variabel, 1:Replace hanya variabel yang masih kosong, 2:Replace ulang semua variabel</param>
        private void LoadData(bool isReadOnly, int replaceFlag)
        {
            //var ilpTdColl = CreateInput(isReadOnly);
            var ilpPrepColl = Temiang.Avicenna.Util.SatuSehatHelper.SatuSehatPreparation(RegistrationNo, TemplateID, replaceFlag);

            var ilpTdColl = new SatuSehatILPTemplateDetailCollection();
            ilpTdColl.Query.Where(ilpTdColl.Query.TemplateID == TemplateID)
                .OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
            ilpTdColl.LoadAll();

            //var ilpPrepColl = new SatuSehatILPPreparationCollection();
            //var ilpPrepQ = ilpPrepColl.Query;
            //ilpPrepQ.Where(ilpPrepQ.RegistrationNo == RegistrationNo, ilpPrepQ.TemplateID == TemplateID);
            //ilpPrepColl.LoadAll();

            foreach (var ilpTd in ilpTdColl)
            {
                switch (ilpTd.SRAnswerType.ToLower())
                {
                    case "lbl":
                        {
                            break;
                        }
                    case "txt":
                        {
                            var txt = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as TextBox;
                            if (txt != null)
                            {
                                var ilpPrep = ilpPrepColl.Where(i => i.TestNo == ilpTd.TestNo && i.Sequence == ilpTd.Sequence).FirstOrDefault();
                                if (ilpPrep != null)
                                {
                                    txt.Text = ilpPrep.AnswerText;
                                }
                            }
                            break;
                        }
                    case "cbo":
                        {
                            var cbo = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as RadComboBox;
                            if (cbo != null)
                            {
                                var ilpPrep = ilpPrepColl.Where(i => i.TestNo == ilpTd.TestNo && i.Sequence == ilpTd.Sequence).FirstOrDefault();
                                if (ilpPrep != null)
                                {
                                    var selectedItem = cbo.Items.FindItemByValue(ilpPrep.AnswerValue);
                                    if (selectedItem != null && !string.IsNullOrEmpty(ilpPrep.AnswerValue))
                                    {
                                        //selectedItem.Selected = true;
                                        cbo.SelectedIndex = selectedItem.Index;
                                    }
                                    else {
                                        selectedItem = cbo.Items.FindItemByText(ilpPrep.AnswerText);
                                        if (selectedItem != null)
                                        {
                                            //selectedItem.Selected = true;
                                            cbo.SelectedIndex = selectedItem.Index;
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }
            }
        }

        private string GetControlID(SatuSehatILPTemplateDetail ilpTd) {
            return string.Format("q_{0}_{1}_{2}", ilpTd.TemplateID.Value.ToString(), ilpTd.TestNo, ilpTd.Sequence.Value.ToString());
        }

        private void InitializedRowText(SatuSehatILPTemplateDetail ilpTd, HtmlTableRow tRow, bool readOnly)
        {
            var txt = TextBoxControl(GetControlID(ilpTd), ilpTd.AnswerWidth.Value, ilpTd.AnswerDefault, readOnly);

            var cell1 = new HtmlTableCell();
            //cell1.Attributes.Add("class", "entry");
            tRow.Cells.Add(cell1);

            if (!string.IsNullOrEmpty(ilpTd.AnswerSuffix))
            {
                //var litSep = new Literal();
                //litSep.Text = "&nbsp;&nbsp;" + ilpTd.AnswerSuffix;
                //tblRow.Cells[1].Controls.Add(litSep);

                //var cell2 = new HtmlTableCell();
                //cell2.Attributes.Add("class", "entry");

                cell1.Controls.Add(txt);
                //cell2.Controls.Add(litSep);

                //tRow.Cells.Add(cell2);

                //tblRow.Cells[1].Controls.Add(tab);
            }
            else
                cell1.Controls.Add(txt);

            //InitializedValidationCtl(txt.ID, "", rowQuestion, tblRow.Cells[1]);

            //InitializedLookUpLink(rowQuestion, tblRow.Cells[0], txt.ClientID);
        }
        private void InitializedRowCbo(SatuSehatILPTemplateDetail ilpTd, HtmlTableRow tRow, bool readOnly)
        {
            var cbo = ComboBoxControl(ilpTd, tRow, readOnly);

            var cell1 = new HtmlTableCell();
            cell1.Attributes.Add("class", "entry");
            tRow.Cells.Add(cell1);

            cell1.Controls.Add(cbo);
        }
        private RadComboBox ComboBoxControl(SatuSehatILPTemplateDetail ilpTd, HtmlTableRow tRow, bool readOnly)
        {
            var cbo = new RadComboBox();
            cbo.ID = GetControlID(ilpTd);
            int w = ilpTd.AnswerWidth ?? 0;
            cbo.Width = Unit.Percentage(95); //Unit.Pixel(w == 0 ? 304 : w);
            cbo.Enabled = !readOnly;
            // Populate Items
            if (ilpTd.AnswerSelection.Contains("[RANGE_"))
            {
                // Add range selection ex. RANGE_1_TO_10, RANGE_0_TO_100_STEP_10 (Handono 230308)
                var ranges = ilpTd.AnswerSelection.Substring(0, ilpTd.AnswerSelection.Length - 1).Split('_');
                var from = ranges[1].ToInt();
                var to = ranges[3].ToInt();

                var step = 1;
                if (ilpTd.AnswerSelection.Contains("_STEP_"))
                {
                    step = ranges[5].ToInt();
                }
                to = to + step;
                cbo.Items.Clear();
                cbo.Items.Add(new RadComboBoxItem(string.Empty, string.Empty));
                for (int i = from; i < to; i = i + step)
                {
                    cbo.Items.Add(new RadComboBoxItem(i.ToString(), i.ToString()));
                }
            }
            else
            {
                try
                {
                    //Dictionary<string, string> dic = new Dictionary<string, string>();
                    //dic.Add("0", "No");
                    //dic.Add("1", "Yes");
                    //var sss = JsonConvert.SerializeObject(dic);

                    Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ilpTd.AnswerSelection);
                    foreach (var p in dict) {
                        cbo.Items.Add(new RadComboBoxItem(p.Value, p.Key));
                    }
                }
                catch { 
                
                }
            }
            // Set Default value
            if (!string.IsNullOrEmpty(ilpTd.AnswerDefault))
                ComboBox.SelectedValue(cbo, ilpTd.AnswerDefault);

            return cbo;
        }
        private Control TextBoxControl(string id, int width, string defaultValue, bool readOnly)
        {
            // Supaya support autosize
            var textBox = new TextBox();
            textBox.ID = id;
            textBox.Width = Unit.Percentage(95); //Unit.Pixel(width == 0 ? 300 : width);
            textBox.TextMode = TextBoxMode.MultiLine;
            textBox.Rows = 1;
            textBox.CssClass = "riTextBox";
            if (readOnly)
                textBox.Enabled = false;
            else
                textBox.Enabled = true;
            // default value
            switch (defaultValue)
            {
                case "[USERNAME]":
                    textBox.Text = AppSession.UserLogin.UserName;
                    break;
                default:
                    textBox.Text = defaultValue;
                    break;
            }

            return textBox;


        }

        #region override method
        protected override void OnPopulateEntryControl(ValidateArgs args)
        {
            RegistrationInfo();
        }

        protected override void OnDataModeChanged(AppEnum.DataMode oldVal, AppEnum.DataMode newVal)
        {
            var isReadOnly = newVal == AppEnum.DataMode.Read;
            btnLoad.Enabled = !isReadOnly;
            btnSend.Enabled = !btnLoad.Enabled;
            btnClear.Enabled = !isReadOnly;

            tblInput.Rows.Clear();
            CreateInput(isReadOnly);
            LoadData(isReadOnly, 0);
        }
        protected override void OnMenuNewClick()
        {
        }
        protected override void OnMenuSaveNewClick(ValidateArgs args)
        {
            SaveILP(args);
        }


        protected override void OnMenuSaveEditClick(ValidateArgs args)
        {
            SaveILP(args);
        }

        protected override void OnMenuPrintClick(ValidateArgs args, string programID, PrintJobParameterCollection printJobParameters)
        {


        }

        protected override void OnBeforeMenuEditClick(ValidateArgs args)
        {
        }

        protected override void OnMenuEditClick()
        {
        }

        protected override void OnMenuAuditLogClick(AuditLogFilter auditLogFilter)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnMenuMovePrevClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnMenuMoveNextClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnMenuDeleteClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnBeforeMenuNewClick(ValidateArgs args)
        {

        }
        protected override void OnMenuApprovalClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnMenuUnApprovalClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void OnMenuVoidClick(ValidateArgs args)
        {
        }

        protected override void OnMenuUnVoidClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }
        protected override void OnMenuRejournalClick(ValidateArgs args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override string OnGetScriptToolBarNewClicking()
        {
            return string.Empty;
        }
        public override string OnGetScriptToolBarSaveClicking()
        {
            return string.Empty;
        }
        public override bool OnGetStatusMenuEdit()
        {
            return true;
        }

        public override bool OnGetStatusMenuDelete()
        {
            return true;
        }

        public override bool? OnGetStatusMenuApproval()
        {
            return true;
        }

        public override bool OnGetStatusMenuVoid()
        {
            return true;
        }

        protected override void OnInitializeAjaxManager(RadAjaxManager ajaxManager)
        {
        }

        protected override void OnInitializeAjaxManagerSettingsCollection(AjaxSettingsCollection ajax)
        {
        }
        #endregion

        private void SaveILP(ValidateArgs args)
        {
            SetValue(args);
            //if (args.IsCancel == false)
                //ent.Save();
        }

        private void SetValue(ValidateArgs args)
        {
            try
            {
                var ilpTdColl = new SatuSehatILPTemplateDetailCollection();
                ilpTdColl.Query.Where(ilpTdColl.Query.TemplateID == TemplateID)
                    .OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
                ilpTdColl.LoadAll();

                var ilpPrepColl = new SatuSehatILPPreparationCollection();
                var ilpPrepQ = ilpPrepColl.Query;
                ilpPrepQ.Where(ilpPrepQ.RegistrationNo == RegistrationNo, ilpPrepQ.TemplateID == TemplateID);
                ilpPrepColl.LoadAll();

                foreach (var ilpTd in ilpTdColl)
                {
                    SatuSehatILPPreparation ilpPrep = null;
                    switch (ilpTd.SRAnswerType.ToLower())
                    {
                        case "lbl":
                            {
                                break;
                            }
                        case "txt":
                            {
                                var txt = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as TextBox;
                                if (txt != null)
                                {
                                    ilpPrep = ilpPrepColl.Where(i => i.TestNo == ilpTd.TestNo && i.Sequence == ilpTd.Sequence).FirstOrDefault();
                                    if (ilpPrep == null)
                                    {
                                        ilpPrep = ilpPrepColl.AddNew();
                                        ilpPrep.RegistrationNo = RegistrationNo;
                                        ilpPrep.TemplateID = TemplateID;
                                        ilpPrep.TestNo = ilpTd.TestNo;
                                        ilpPrep.Sequence = ilpTd.Sequence;
                                        ilpPrep.PostData = "";
                                        ilpPrep.CreateByUserID = AppSession.UserLogin.UserID;
                                        ilpPrep.CreateDateTime = DateTime.Now;
                                        ilpPrep.AnswerValue = "";
                                    }
                                    ilpPrep.AnswerText = txt.Text;
                                    if (ilpTd.IsEditable == true && (!(ilpPrep.IsSent ?? false) || (ilpPrep.IsError ?? false)) && !string.IsNullOrWhiteSpace(ilpTd.JsonPathKeyword))
                                    {
                                        var jObj = JObject.Parse(ilpPrep.PostData);
                                        var token = jObj.SelectToken(ilpTd.JsonPathKeyword);

                                        if (token != null)
                                        {
                                            if (token.Type == JTokenType.Integer || token.Type == JTokenType.Float)
                                            {
                                                if (decimal.TryParse(txt.Text, out var numValue))
                                                    token.Replace(new JValue(numValue));
                                            }
                                            else
                                                token.Replace(new JValue(txt.Text));
                                        }

                                        ilpPrep.PostData = jObj.ToString();
                                    }
                                    //if(Regex.IsMatch(ilpPrep.PostData, @"\{\{.+?\}\}") || ilpPrep.PostData == string.Empty)
                                    //    ilpPrep.PostData = ilpTd.PostJsonTemplate;

                                    ilpPrep.LastUpdateByUserID = AppSession.UserLogin.UserID;
                                    ilpPrep.LastUpdateDateTime = DateTime.Now;
                                }
                                break;
                            }
                        case "cbo":
                            {
                                var cbo = Helper.FindControlRecursive(Page, GetControlID(ilpTd)) as RadComboBox;
                                if (cbo != null)
                                {
                                    ilpPrep = ilpPrepColl.Where(i => i.TestNo == ilpTd.TestNo && i.Sequence == ilpTd.Sequence).FirstOrDefault();
                                    if (ilpPrep == null)
                                    {
                                        ilpPrep = ilpPrepColl.AddNew();
                                        ilpPrep.RegistrationNo = RegistrationNo;
                                        ilpPrep.TemplateID = TemplateID;
                                        ilpPrep.TestNo = ilpTd.TestNo;
                                        ilpPrep.Sequence = ilpTd.Sequence;
                                        ilpPrep.PostData = "";
                                        ilpPrep.CreateByUserID = AppSession.UserLogin.UserID;
                                        ilpPrep.CreateDateTime = DateTime.Now;
                                    }
                                    ilpPrep.AnswerValue = cbo.SelectedItem.Value;
                                    ilpPrep.AnswerText = cbo.SelectedItem.Text;
                                    if (ilpTd.IsEditable == true && (!(ilpPrep.IsSent ?? false) || (ilpPrep.IsError ?? false)) && !string.IsNullOrWhiteSpace(ilpTd.JsonPathKeyword))
                                    {
                                        var jObj = JObject.Parse(ilpPrep.PostData);
                                        var paths = ilpTd.JsonPathKeyword.Split(';');
                                        if (paths.Length > 0)
                                        {
                                            var token1 = jObj.SelectToken(paths[0]);
                                            if (token1 != null) token1.Replace(cbo.SelectedItem.Value);
                                        }
                                        if (paths.Length > 1)
                                        {
                                            var token2 = jObj.SelectToken(paths[1]);
                                            if (token2 != null) token2.Replace(cbo.SelectedItem.Text);
                                        }

                                        ilpPrep.PostData = jObj.ToString();
                                    }
                                    //if (Regex.IsMatch(ilpPrep.PostData, @"\{\{.+?\}\}") || ilpPrep.PostData == string.Empty)
                                    //    ilpPrep.PostData = ilpTd.PostJsonTemplate;

                                    //ilpPrep.PostData = ilpTd.PostJsonTemplate;
                                    ilpPrep.LastUpdateByUserID = AppSession.UserLogin.UserID;
                                    ilpPrep.LastUpdateDateTime = DateTime.Now;
                                }
                                break;
                            }
                    }
                }

                //Temiang.Avicenna.Util.SatuSehatHelper.ReplaceVariables(RegistrationNo, ilpPrepColl, false, TemplateID, ilpTdColl);

                ilpPrepColl.Save();
            }
            catch (Exception ex)
            {
                args.MessageText = Helper.GetFullExceptionMessage(ex);
                args.IsCancel = true;
            }
        }

        protected void btnLoad_Click(object sender, EventArgs e)
        {
            HideInformationHeader();
            LoadData(false, 1);
            SetInputEnabled();
            try
            {
                //LoadData(false, true);
            }
            catch (Exception ex)
            {
                ShowInformationHeader(Helper.GetFullExceptionMessage(ex));
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            HideInformationHeader();
            LoadData(false, 2);
            SetInputEnabled();
            try
            {
                //LoadData(false, true);
            }
            catch (Exception ex)
            {
                ShowInformationHeader(Helper.GetFullExceptionMessage(ex));
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string accessToken = "";
            var ssk = new SatuSehatKunjungan();
            ssk.LoadByPrimaryKey(RegistrationNo);
            if(ssk.IsClosed == true)
            {
                string script = "alert('SatuSehat data for this patient is closed.');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ErrorAlert", script, true);
            }
            else
                Util.SatuSehatHelper.SendToSatuSehat(RegistrationNo, ref accessToken);

            // load ulang, tapi blm langsung refresh ke browser, masih bermasalah
            //LoadData(IsReadOnly, 0);
            Response.Redirect(Request.RawUrl, false);
        }
    }
}
