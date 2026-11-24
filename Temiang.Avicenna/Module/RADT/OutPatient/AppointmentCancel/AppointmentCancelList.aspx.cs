
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Net;

using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Temiang.Avicenna.Module.RADT.OutPatient
{
    public partial class AppointmentCancel : BasePage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var appType = Request.QueryString["at"];

            switch (appType)
            {
                case AppConstant.AppoinmentType.OutPatient:
                    ProgramID = "01.06.02";
                    break;

                case AppConstant.AppoinmentType.MedicalCheckUp:
                    ProgramID = "01.06.02";
                    break;
            }

            if (!IsPostBack)
            {
                ComboBox.PopulateWithServiceUnitForTransaction(
                    cboServiceUnitID,
                    BusinessObject.Reference.TransactionCode.Appointment,
                    false
                );
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDate.SelectedDate =
                    (new DateTime()).NowAtSqlServer();
            }
        }

        #region Appointment Data

        private DataTable Appointments
        {
            get
            {
                var query = new AppointmentQuery("a");
                var status = new AppStandardReferenceItemQuery("s");
                var serviceUnit = new ServiceUnitQuery("su");
                var par = new ParamedicQuery("p");
                var visit = new VisitTypeQuery("v");
                var pat = new PatientQuery("pat");
                var sal = new AppStandardReferenceItemQuery("sal");
                var guar = new GuarantorQuery("guar");
                var type = new AppStandardReferenceItemQuery("type");

                query.LeftJoin(status).On(
                    query.SRAppointmentStatus == status.ItemID &&
                    status.StandardReferenceID ==
                    AppEnum.StandardReference.AppointmentStatus.ToString()
                );

                query.InnerJoin(serviceUnit).On(
                    query.ServiceUnitID == serviceUnit.ServiceUnitID
                );

                query.InnerJoin(par).On(
                    query.ParamedicID == par.ParamedicID
                );

                query.LeftJoin(visit).On(
                    query.VisitTypeID == visit.VisitTypeID
                );

                query.LeftJoin(pat).On(
                    query.PatientID == pat.PatientID
                );

                query.LeftJoin(sal).On(
                    sal.StandardReferenceID ==
                    AppEnum.StandardReference.Salutation.ToString() &&
                    sal.ItemID == pat.SRSalutation
                );

                query.LeftJoin(guar).On(
                    query.GuarantorID == guar.GuarantorID
                );

                query.LeftJoin(type).On(
                    query.SRAppoinmentType == type.ItemID &&
                    type.StandardReferenceID ==
                    AppEnum.StandardReference.AppoinmentType.ToString()
                );

                query.Select(
                    query.AppointmentQue,
                    query.AppointmentNo,
                    query.AppointmentDate,
                    query.AppointmentTime,
                    visit.VisitTypeName,
                    query.VisitDuration,
                    query.SRAppointmentStatus,
                    status.ItemName.As("AppointmentStatus"),
                    pat.MedicalNo,
                    query.PatientName,
                    query.Address,
                    "<p.ParamedicName + ' - ' + su.ServiceUnitName as GROUPGRID>",
                    query.PhoneNo,
                    query.MobilePhoneNo,
                    sal.ItemName.As("SalutationName"),
                    guar.GuarantorName,
                    type.ItemName.As("AppointmentType"),
                    query.LastCreateByUserID,
                    query.Notes
                );

                // Hanya appointment dengan status Open
                query.Where(
                    query.SRAppointmentStatus ==
                    AppSession.Parameter.AppointmentStatusOpen
                );

                // Filter tanggal
                if (!txtDate.IsEmpty)
                {
                    query.Where(
                        query.AppointmentDate.Date() ==
                        txtDate.SelectedDate.Value.Date
                    );
                }

                // Filter Medical No
                if (!string.IsNullOrEmpty(txtMedicalNo.Text))
                {
                    string searchMedNo =
                        Helper.EscapeQuery(txtMedicalNo.Text);

                    if (AppSession.Parameter.IsMedicalNoContainStrip)
                    {
                        query.Where(
                            query.Or(
                                pat.MedicalNo == searchMedNo,
                                string.Format(
                                    "< OR REPLACE(pat.MedicalNo, '-', '') LIKE '%{0}%'>",
                                    searchMedNo
                                )
                            )
                        );
                    }
                    else
                    {
                        query.Where(
                            query.Or(
                                pat.MedicalNo == searchMedNo,
                                string.Format(
                                    "< OR pat.MedicalNo LIKE '%{0}%'>",
                                    searchMedNo
                                )
                            )
                        );
                    }
                }

                // Filter Patient Name
                if (!string.IsNullOrEmpty(txtPatientName.Text))
                {
                    string searchPatient =
                        Helper.EscapeQuery(txtPatientName.Text);

                    query.Where(
                        string.Format(
                            "<RTRIM(pat.FirstName+' '+pat.MiddleName)+' '+pat.LastName LIKE '%{0}%'>",
                            searchPatient
                        )
                    );
                }

                // Filter Service Unit
                if (!string.IsNullOrEmpty(cboServiceUnitID.SelectedValue))
                {
                    query.Where(
                        query.ServiceUnitID == cboServiceUnitID.SelectedValue
                    );
                }

                // Filter Physician
                if (!string.IsNullOrEmpty(cboParamedicID.SelectedValue))
                {
                    query.Where(
                        query.ParamedicID == cboParamedicID.SelectedValue
                    );
                }

                query.OrderBy(
                    query.AppointmentDate.Descending,
                    query.AppointmentTime.Ascending
                );

                query.es.Top = AppSession.Parameter.MaxResultRecord;

                return query.LoadDataTable();
            }
        }

        #endregion

        #region Grid

        protected void grdAppointmentList_NeedDataSource(
            object source,
            GridNeedDataSourceEventArgs e)
        {
            var grd = (RadGrid)source;

            if (!IsPostBack && !IsListLoadRecordOnInit)
            {
                grd.DataSource = new string[] { };
                return;
            }

            DataTable dtb = Appointments;

            grd.DataSource = dtb ?? (object)new string[] { };
        }

        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {
            grdAppointmentList.Rebind();
        }

        #endregion

        #region Cancel Appointment WebService

        /// <summary>
        /// Membatalkan appointment melalui WebService V1_0.
        /// Return null jika berhasil, atau pesan error jika gagal.
        /// </summary>
        private string CancelAppointment(string appointmentNo)
        {
            try
            {
                string applicationPath =
                    Request.ApplicationPath.TrimEnd('/');

                string endpoint =
                    Request.Url.GetLeftPart(UriPartial.Authority) +
                    applicationPath +
                    "/WebService/V1_0/AppointmentWS.asmx/AppointmentCancel";

                // Parameter WebService
                string postData = JsonConvert.SerializeObject(new
                {
                    AccessKey = "AvcMblPat",
                    AppointmentNo = appointmentNo
                });

                byte[] data = Encoding.UTF8.GetBytes(postData);

                var request =
                    (HttpWebRequest)WebRequest.Create(endpoint);

                request.Method = "POST";
                request.ContentType = "application/json; charset=utf-8";
                request.Accept = "application/json";
                request.ContentLength = data.Length;
                request.Timeout = 30000;
                request.ReadWriteTimeout = 30000;

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                string responseText;

                using (var response =
                    (HttpWebResponse)request.GetResponse())
                using (var reader =
                    new StreamReader(response.GetResponseStream()))
                {
                    responseText = reader.ReadToEnd();
                }

                // Debug sementara untuk melihat response mentah dari WebService.
                System.Diagnostics.Debug.WriteLine(
                    "APPOINTMENT CANCEL RESPONSE: " + responseText
                );

                // Baca objek JSON pertama saja.
                // WebService ini dapat mengirim konten tambahan setelah JSON pertama
                // karena response ditulis melalui WriteResponseAndLog().
                JObject result = ParseFirstJsonObject(responseText);

                // Antisipasi response ASMX yang dibungkus dalam property d.
                if (result["d"] != null)
                {
                    if (result["d"].Type == JTokenType.String)
                    {
                        result = ParseFirstJsonObject(result["d"].ToString());
                    }
                    else if (result["d"].Type == JTokenType.Object)
                    {
                        result = (JObject)result["d"];
                    }
                }

                string status = Convert.ToString(result["status"]);

                // Validasi hasil WebService
                if (!string.Equals(
                    status,
                    "OK",
                    StringComparison.OrdinalIgnoreCase))
                {
                    string errorMessage =
                        Convert.ToString(result["data"]);

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage =
                            Convert.ToString(result["message"]);
                    }

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage =
                            "Appointment gagal dibatalkan. Response: " +
                            responseText;
                    }

                    return errorMessage;
                }

                // WebService berhasil, lanjutkan integrasi BPJS Antrol
                ProcessBpjsAntrol(appointmentNo);

                return null;
            }
            catch (WebException ex)
            {
                string errorMessage = ex.Message;

                if (ex.Response != null)
                {
                    using (var reader = new StreamReader(
                        ex.Response.GetResponseStream()))
                    {
                        errorMessage = reader.ReadToEnd();
                    }
                }

                return "WebService error: " + errorMessage;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Membaca objek JSON pertama dan mengabaikan konten tambahan
        /// setelah objek JSON tersebut.
        /// </summary>
        private JObject ParseFirstJsonObject(string json)
        {
            using (var stringReader = new StringReader(json))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                jsonReader.SupportMultipleContent = true;

                while (jsonReader.Read())
                {
                    if (jsonReader.TokenType == JsonToken.StartObject)
                    {
                        return JObject.Load(jsonReader);
                    }
                }
            }

            throw new Exception(
                "Response WebService tidak berisi objek JSON yang valid: " + json
            );
        }

        /// <summary>
        /// Integrasi pembatalan appointment dengan BPJS Antrol.
        /// </summary>
        private void ProcessBpjsAntrol(string appointmentNo)
        {
            if (!Helper.IsBpjsAntrolIntegration)
                return;

            try
            {
                var svc = new Common.BPJS.Antrian.Service();

                var response = svc.BatalAntrian(
                    new Common.BPJS.Antrian.Update.BatalAntrian.Request.Root()
                    {
                        Kodebooking = appointmentNo,
                        Keterangan = "tidak hadir/batal"
                    }
                );

                var waktuRequest =
                    new Common.BPJS.Antrian.Update.WaktuAntrian.Request.Root()
                    {
                        Kodebooking = appointmentNo,
                        Taskid = 99,
                        Waktu = Convert.ToInt64(
                            DateTimeOffset.Now.ToUnixTimeMilliseconds()
                        )
                    };

                var responseMetadata =
                    svc.UpdateWaktuAntrian(waktuRequest);

                var log = new WebServiceAPILog();

                log.DateRequest = DateTime.Now;
                log.IPAddress = string.Empty;
                log.UrlAddress = "AppointmentCancel";
                log.Params = JsonConvert.SerializeObject(waktuRequest);
                log.Response = JsonConvert.SerializeObject(response);

                log.Save();
            }
            catch (Exception)
            {
                // Kegagalan BPJS tidak membatalkan perubahan
                // status appointment yang sudah berhasil di WebService.
            }
        }

        #endregion

        #region Cancel Single Appointment

        protected void grdAppointmentList_ItemCommand(
            object source,
            GridCommandEventArgs e)
        {
            if (e.CommandName == "CancelAppointment")
            {
                string appointmentNo =
                    Convert.ToString(e.CommandArgument);

                string error = CancelAppointment(appointmentNo);

                if (!string.IsNullOrEmpty(error))
                {
                    string message =
                        "Gagal membatalkan Appointment " +
                        appointmentNo + ":\n\n" + error;

                    ScriptManager.RegisterStartupScript(
                        this,
                        GetType(),
                        "CancelAppointmentError",
                        "alert(" +
                        JsonConvert.SerializeObject(message) +
                        ");",
                        true
                    );
                }

                grdAppointmentList.Rebind();
            }
        }

        #endregion

        #region Cancel Selected Appointment

        protected void btnCancelSelected_Click(
            object sender,
            EventArgs e)
        {
            var errors = new List<string>();

            int totalSelected = grdAppointmentList.SelectedItems.Count;
            int totalSuccess = 0;

            if (totalSelected == 0)
            {
                ScriptManager.RegisterStartupScript(
                    this,
                    GetType(),
                    "NoAppointmentSelected",
                    "alert('Silakan pilih Appointment yang akan dibatalkan.');",
                    true
                );

                return;
            }

            foreach (GridDataItem item in grdAppointmentList.SelectedItems)
            {
                string appointmentNo =
                    Convert.ToString(
                        item.GetDataKeyValue("AppointmentNo")
                    );

                string error = CancelAppointment(appointmentNo);

                if (string.IsNullOrEmpty(error))
                {
                    totalSuccess++;
                }
                else
                {
                    errors.Add(
                        appointmentNo + " : " + error
                    );
                }
            }

            grdAppointmentList.Rebind();

            string message =
                "Proses pembatalan Appointment selesai.\n\n" +
                "Total dipilih : " + totalSelected + "\n" +
                "Berhasil : " + totalSuccess + "\n" +
                "Gagal : " + errors.Count;

            if (errors.Count > 0)
            {
                message +=
                    "\n\nDaftar Appointment yang gagal:\n\n" +
                    string.Join("\n", errors);
            }

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "CancelSelectedResult",
                "alert(" +
                JsonConvert.SerializeObject(message) +
                ");",
                true
            );
        }

        #endregion

        #region Service Unit & Physician

        protected void cboServiceUnitID_SelectedIndexChanged(
            object o,
            RadComboBoxSelectedIndexChangedEventArgs e)
        {
            cboParamedicID.Items.Clear();
            cboParamedicID.Text = string.Empty;
            cboParamedicID.SelectedValue = string.Empty;
        }

        protected void cboParamedicID_ItemsRequested(
            object o,
            RadComboBoxItemsRequestedEventArgs e)
        {
            string searchText = string.Format("%{0}%", e.Text);

            var medic = new ParamedicQuery("a");
            var unit = new ServiceUnitParamedicQuery("b");

            medic.es.Top = 10;

            medic.InnerJoin(unit).On(
                medic.ParamedicID == unit.ParamedicID &&
                unit.ServiceUnitID == cboServiceUnitID.SelectedValue
            );

            medic.Where(
                medic.ParamedicName.Like(searchText),
                medic.IsAvailable == true,
                medic.IsActive == true
            );

            medic.OrderBy(medic.ParamedicName.Ascending);

            cboParamedicID.DataSource = medic.LoadDataTable();
            cboParamedicID.DataBind();
        }

        protected void cboParamedicID_ItemDataBound(
            object sender,
            RadComboBoxItemEventArgs e)
        {
            e.Item.Text =
                ((DataRowView)e.Item.DataItem)["ParamedicName"].ToString();

            e.Item.Value =
                ((DataRowView)e.Item.DataItem)["ParamedicID"].ToString();
        }

        #endregion
    }
}
