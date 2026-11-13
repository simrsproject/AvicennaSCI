using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.JsonField;
using Telerik.Web.UI;
namespace Temiang.Avicenna.Module.RADT.EmrIp.EmrIpCommon.ResumeMedis
{
    public partial class ControlPlanCtl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public bool ValidateForDischarge(DateTime dischargeDate, out string message)
        {
            for (var i = 1; i <= 5; i++)
            {
                var suffix = i.ToString("00");
                var date = (RadDateTimePicker)FindControl("txtControlPlanDateTime" + suffix);
                var unit = (RadComboBox)FindControl("cboServiceUnitID" + suffix);
                var physician = (RadComboBox)FindControl("cboParamedicName" + suffix);
                if (date.IsEmpty && string.IsNullOrWhiteSpace(unit.SelectedValue)
                    && string.IsNullOrWhiteSpace(physician.SelectedValue)) continue;

                if (date.IsEmpty || string.IsNullOrWhiteSpace(unit.SelectedValue)
                    || string.IsNullOrWhiteSpace(physician.SelectedValue))
                {
                    message = string.Format("Rencana kontrol baris {0}: tanggal, poli, dan dokter harus diisi.", i);
                    return false;
                }
                if (date.SelectedDate.Value.Date <= dischargeDate.Date || date.SelectedDate.Value.Date <= DateTime.Today)
                {
                    message = string.Format("Rencana kontrol baris {0}: tanggal harus setelah tanggal pulang dan hari ini.", i);
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        private void AddControlPlanItem(List<ControlPlanItem> itemPlans, RadDateTimePicker txtControlPlanDateTime, RadComboBox cboServiceUnitID
            , RadComboBox cboParamedicName, RadTextBox txtSpecialtyName, RadTextBox txtNotes, RadTextBox txtAppointmentNo, RadTextBox txtAppointmentQue, RadTextBox txtAppointmentTime, bool includeUndatedPlans = false)
        {

            if ((!txtControlPlanDateTime.IsEmpty || includeUndatedPlans) && !string.IsNullOrWhiteSpace(cboServiceUnitID.SelectedValue) && !string.IsNullOrWhiteSpace(cboParamedicName.SelectedValue))
            {
                // Check double 
                var isExist = false;
                foreach (var item in itemPlans)
                {
                    if (item.ControlPlanDateTime.Equals(txtControlPlanDateTime.SelectedDate.GetValueOrDefault())
                        && item.ServiceUnitID.Equals(cboServiceUnitID.SelectedValue)
                        && item.ParamedicID.Equals(cboParamedicName.SelectedValue))
                    {
                        isExist = true;
                        break;
                    }
                }

                if (!isExist)
                {
                    itemPlans.Add(new ControlPlanItem
                    {
                        ControlPlanDateTime = txtControlPlanDateTime.SelectedDate.GetValueOrDefault(),
                        ServiceUnitID = cboServiceUnitID.SelectedValue,
                        ParamedicID = cboParamedicName.SelectedValue,
                        ParamedicName = cboParamedicName.Text,
                        SpecialtyName = txtSpecialtyName.Text,
                        Notes = txtNotes.Text,
                        AppointmentNo = txtAppointmentNo.Text,
                        AppointmentQue = txtAppointmentQue.Text.ToInt(),
                        AppointmentTime = txtAppointmentTime.Text
                    });
                }
            }
        }
        public ControlPlan GetControlPlan()
        {
            return GetControlPlan(false);
        }

        public ControlPlan GetControlPlan(bool includeUndatedPlans)
        {
            var itemPlans = new List<ControlPlanItem>();

            //if (!txtControlPlanDateTime01.IsEmpty && !string.IsNullOrWhiteSpace(cboServiceUnitID01.SelectedValue) && )
            //    itemPlans.Add(new ControlPlanItem
            //    {
            //        ControlPlanDateTime = txtControlPlanDateTime01.SelectedDate.Value,
            //        ServiceUnitID = cboServiceUnitID01.SelectedValue,
            //        ParamedicID = cboParamedicName01.SelectedValue,
            //        ParamedicName = cboParamedicName01.Text,
            //        SpecialtyName = txtSpecialtyName01.Text,
            //        AppointmentNo = txtAppointmentNo01.Text,
            //        AppointmentQue = txtAppointmentQue01.Text.ToInt(),
            //        AppointmentTime = txtAppointmentTime01.Text
            //    });

            //if (!txtControlPlanDateTime02.IsEmpty)
            //    itemPlans.Add(new ControlPlanItem
            //    {
            //        ControlPlanDateTime = txtControlPlanDateTime02.SelectedDate.Value,
            //        ServiceUnitID = cboServiceUnitID02.SelectedValue,
            //        ParamedicID = cboParamedicName02.SelectedValue,
            //        ParamedicName = cboParamedicName02.Text,
            //        SpecialtyName = txtSpecialtyName02.Text,
            //        AppointmentNo = txtAppointmentNo02.Text,
            //        AppointmentQue = txtAppointmentQue02.Text.ToInt(),
            //        AppointmentTime = txtAppointmentTime02.Text
            //    });

            //if (!txtControlPlanDateTime03.IsEmpty)
            //    itemPlans.Add(new ControlPlanItem
            //    {
            //        ControlPlanDateTime = txtControlPlanDateTime03.SelectedDate.Value,
            //        ServiceUnitID = cboServiceUnitID03.SelectedValue,
            //        ParamedicID = cboParamedicName03.SelectedValue,
            //        ParamedicName = cboParamedicName03.Text,
            //        SpecialtyName = txtSpecialtyName03.Text,
            //        AppointmentNo = txtAppointmentNo03.Text,
            //        AppointmentQue = txtAppointmentQue03.Text.ToInt(),
            //        AppointmentTime = txtAppointmentTime03.Text
            //    });

            //if (!txtControlPlanDateTime04.IsEmpty)
            //    itemPlans.Add(new ControlPlanItem
            //    {
            //        ControlPlanDateTime = txtControlPlanDateTime04.SelectedDate.Value,
            //        ServiceUnitID = cboServiceUnitID04.SelectedValue,
            //        ParamedicID = cboParamedicName04.SelectedValue,
            //        ParamedicName = cboParamedicName04.Text,
            //        SpecialtyName = txtSpecialtyName04.Text,
            //        AppointmentNo = txtAppointmentNo04.Text,
            //        AppointmentQue = txtAppointmentQue04.Text.ToInt(),
            //        AppointmentTime = txtAppointmentTime04.Text
            //    });

            //if (!txtControlPlanDateTime05.IsEmpty)
            //    itemPlans.Add(new ControlPlanItem
            //    {
            //        ControlPlanDateTime = txtControlPlanDateTime05.SelectedDate.Value,
            //        ServiceUnitID = cboServiceUnitID05.SelectedValue,
            //        ParamedicID = cboParamedicName05.SelectedValue,
            //        ParamedicName = cboParamedicName05.Text,
            //        SpecialtyName = txtSpecialtyName05.Text,
            //        AppointmentNo = txtAppointmentNo05.Text,
            //        AppointmentQue = txtAppointmentQue05.Text.ToInt(),
            //        AppointmentTime = txtAppointmentTime05.Text
            //    });

            AddControlPlanItem(itemPlans, txtControlPlanDateTime01, cboServiceUnitID01, cboParamedicName01, txtSpecialtyName01, txtNotes01, txtAppointmentNo01, txtAppointmentQue01, txtAppointmentTime01, includeUndatedPlans);
            AddControlPlanItem(itemPlans, txtControlPlanDateTime02, cboServiceUnitID02, cboParamedicName02, txtSpecialtyName02, txtNotes02, txtAppointmentNo02, txtAppointmentQue02, txtAppointmentTime02, includeUndatedPlans);
            AddControlPlanItem(itemPlans, txtControlPlanDateTime03, cboServiceUnitID03, cboParamedicName03, txtSpecialtyName03, txtNotes03, txtAppointmentNo03, txtAppointmentQue03, txtAppointmentTime03, includeUndatedPlans);
            AddControlPlanItem(itemPlans, txtControlPlanDateTime04, cboServiceUnitID04, cboParamedicName04, txtSpecialtyName04, txtNotes04, txtAppointmentNo04, txtAppointmentQue04, txtAppointmentTime04, includeUndatedPlans);
            AddControlPlanItem(itemPlans, txtControlPlanDateTime05, cboServiceUnitID05, cboParamedicName05, txtSpecialtyName05, txtNotes05, txtAppointmentNo05, txtAppointmentQue05, txtAppointmentTime05, includeUndatedPlans);


            var controlPlan = new ControlPlan();
            controlPlan.Items = itemPlans.Where(item => item.ControlPlanDateTime != DateTime.MinValue).ToList();
            var undated = itemPlans.Where(item => item.ControlPlanDateTime == DateTime.MinValue).ToList();
            if (undated.Count > 0) controlPlan.UndatedItems = undated;
            return controlPlan;
        }

        public void Populate(string controlPlan)
        {
            // Reset
            PopulatePlanItem(new ControlPlanItem(), txtControlPlanDateTime01, cboServiceUnitID01, cboParamedicName01, txtSpecialtyName01, txtNotes01, txtAppointmentNo01, txtAppointmentQue01, txtAppointmentTime01);
            PopulatePlanItem(new ControlPlanItem(), txtControlPlanDateTime02, cboServiceUnitID02, cboParamedicName02, txtSpecialtyName02, txtNotes02, txtAppointmentNo02, txtAppointmentQue02, txtAppointmentTime02);
            PopulatePlanItem(new ControlPlanItem(), txtControlPlanDateTime03, cboServiceUnitID03, cboParamedicName03, txtSpecialtyName03, txtNotes03, txtAppointmentNo03, txtAppointmentQue03, txtAppointmentTime03);
            PopulatePlanItem(new ControlPlanItem(), txtControlPlanDateTime04, cboServiceUnitID04, cboParamedicName04, txtSpecialtyName04, txtNotes04, txtAppointmentNo04, txtAppointmentQue04, txtAppointmentTime04);
            PopulatePlanItem(new ControlPlanItem(), txtControlPlanDateTime05, cboServiceUnitID05, cboParamedicName05, txtSpecialtyName05, txtNotes05, txtAppointmentNo05, txtAppointmentQue05, txtAppointmentTime05);

            if (!string.IsNullOrEmpty(controlPlan))
            {
                var planCount = 0;
                var oplan = JsonConvert.DeserializeObject<ControlPlan>(controlPlan);
                if (oplan == null) return;
                oplan.Items = (oplan.Items ?? new List<ControlPlanItem>())
                    .Concat(oplan.UndatedItems ?? new List<ControlPlanItem>()).ToList();
                if (oplan != null && oplan.Items != null)
                    planCount = oplan.Items.Count;

                //ControlPlanItem cpi;
                if (planCount > 0)
                {
                    PopulatePlanItem(oplan.Items[0], txtControlPlanDateTime01, cboServiceUnitID01, cboParamedicName01, txtSpecialtyName01, txtNotes01, txtAppointmentNo01, txtAppointmentQue01, txtAppointmentTime01);
                }

                if (planCount > 1)
                {
                    //cpi = oplan.Items[1];
                    //txtControlPlanDateTime02.SelectedDate = oplan.Items[1].ControlPlanDateTime;
                    //Common.ComboBox.PopulateWithOneServiceUnit(cboServiceUnitID02, oplan.Items[1].ServiceUnitID);
                    //cboParamedicName02.Text = oplan.Items[1].ParamedicName;
                    //txtSpecialtyName02.Text = oplan.Items[1].SpecialtyName;
                    //if (!string.IsNullOrWhiteSpace(oplan.Items[1].ParamedicID))
                    //    Common.ComboBox.PopulateWithOneParamedic(cboParamedicName02, oplan.Items[1].ParamedicID);

                    //txtAppointmentNo02.Text = cpi.AppointmentNo;
                    //txtAppointmentQue02.Text = cpi.AppointmentQue.ToString();
                    //txtAppointmentTime02.Text = cpi.AppointmentTime;
                    PopulatePlanItem(oplan.Items[1], txtControlPlanDateTime02, cboServiceUnitID02, cboParamedicName02, txtSpecialtyName02, txtNotes02, txtAppointmentNo02, txtAppointmentQue02, txtAppointmentTime02);

                }

                if (planCount > 2)
                {
                    //cpi = oplan.Items[2];
                    //txtControlPlanDateTime03.SelectedDate = cpi.ControlPlanDateTime;
                    //Common.ComboBox.PopulateWithOneServiceUnit(cboServiceUnitID03, cpi.ServiceUnitID);
                    //cboParamedicName03.Text = cpi.ParamedicName;
                    //txtSpecialtyName03.Text = cpi.SpecialtyName;
                    //if (!string.IsNullOrWhiteSpace(cpi.ParamedicID))
                    //    Common.ComboBox.PopulateWithOneParamedic(cboParamedicName03, cpi.ParamedicID);

                    //txtAppointmentNo03.Text = cpi.AppointmentNo;
                    //txtAppointmentQue03.Text = cpi.AppointmentQue.ToString();
                    //txtAppointmentTime03.Text = cpi.AppointmentTime;

                    PopulatePlanItem(oplan.Items[2], txtControlPlanDateTime03, cboServiceUnitID03, cboParamedicName03, txtSpecialtyName03, txtNotes03, txtAppointmentNo03, txtAppointmentQue03, txtAppointmentTime03);
                }

                if (planCount > 3)
                {
                    //cpi = oplan.Items[3];
                    //txtControlPlanDateTime04.SelectedDate = cpi.ControlPlanDateTime;
                    //Common.ComboBox.PopulateWithOneServiceUnit(cboServiceUnitID04, cpi.ServiceUnitID);
                    //cboParamedicName04.Text = cpi.ParamedicName;
                    //txtSpecialtyName04.Text = cpi.SpecialtyName;
                    //if (!string.IsNullOrWhiteSpace(cpi.ParamedicID))
                    //    Common.ComboBox.PopulateWithOneParamedic(cboParamedicName04, cpi.ParamedicID);

                    //txtAppointmentNo04.Text = cpi.AppointmentNo;
                    //txtAppointmentQue04.Text = cpi.AppointmentQue.ToString();
                    //txtAppointmentTime04.Text = cpi.AppointmentTime;

                    PopulatePlanItem(oplan.Items[3], txtControlPlanDateTime04, cboServiceUnitID04, cboParamedicName04, txtSpecialtyName04, txtNotes04, txtAppointmentNo04, txtAppointmentQue04, txtAppointmentTime04);

                }

                if (planCount > 4)
                {
                    //cpi = oplan.Items[4];
                    //txtControlPlanDateTime05.SelectedDate = cpi.ControlPlanDateTime;
                    //Common.ComboBox.PopulateWithOneServiceUnit(cboServiceUnitID05, cpi.ServiceUnitID);
                    //cboParamedicName05.Text = cpi.ParamedicName;
                    //txtSpecialtyName05.Text = cpi.SpecialtyName;
                    //if (!string.IsNullOrWhiteSpace(cpi.ParamedicID))
                    //    Common.ComboBox.PopulateWithOneParamedic(cboParamedicName05, cpi.ParamedicID);

                    //txtAppointmentNo05.Text = cpi.AppointmentNo;
                    //txtAppointmentQue05.Text = cpi.AppointmentQue.ToString();
                    //txtAppointmentTime05.Text = cpi.AppointmentTime;
                    PopulatePlanItem(oplan.Items[4], txtControlPlanDateTime05, cboServiceUnitID05, cboParamedicName05, txtSpecialtyName05, txtNotes05, txtAppointmentNo05, txtAppointmentQue05, txtAppointmentTime05);
                }
            }
        }

        public void PopulatePlanItem(ControlPlan cp)
        {
            PopulatePlanItem(cp.Items[0], txtControlPlanDateTime01, cboServiceUnitID01, cboParamedicName01, txtSpecialtyName01, txtNotes01, txtAppointmentNo01, txtAppointmentQue01, txtAppointmentTime01);
        }

        private void PopulatePlanItem(ControlPlanItem cpi, RadDateTimePicker txtControlPlanDateTime, RadComboBox cboServiceUnitID
            , RadComboBox cboParamedicName, RadTextBox txtSpecialtyName, RadTextBox txtNotes, RadTextBox txtAppointmentNo, RadTextBox txtAppointmentQue, RadTextBox txtAppointmentTime)
        {

            if (cpi.ControlPlanDateTime.Equals(Convert.ToDateTime("1/1/0001 12:00:00 AM")))
                txtControlPlanDateTime.Clear();
            else
                txtControlPlanDateTime.SelectedDate = cpi.ControlPlanDateTime;

            if (!string.IsNullOrWhiteSpace(cpi.ServiceUnitID))
            {
                Common.ComboBox.PopulateWithOneServiceUnit(cboServiceUnitID, cpi.ServiceUnitID);
            }
            else
            {
                cboServiceUnitID.Items.Clear();
                cboServiceUnitID.SelectedIndex = -1;
                cboServiceUnitID.Text = string.Empty;
            }

            if (!string.IsNullOrWhiteSpace(cpi.ParamedicID))
            {
                Common.ComboBox.PopulateWithOneParamedic(cboParamedicName, cpi.ParamedicID);
            }
            else
            {
                cboParamedicName.Items.Clear();
                cboParamedicName.SelectedIndex = -1;
                cboParamedicName.Text = string.Empty;
            }

            //cboParamedicName.Text = cpi.ParamedicName;
            txtSpecialtyName.Text = cpi.SpecialtyName;
            txtNotes.Text = cpi.Notes;
            txtAppointmentNo.Text = cpi.AppointmentNo;
            if (cpi.AppointmentQue > 0)
                txtAppointmentQue.Text = cpi.AppointmentQue.ToString();
            else
                txtAppointmentQue.Text = string.Empty;
            txtAppointmentTime.Text = cpi.AppointmentTime;
        }
    }
}
