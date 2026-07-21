using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Common;
using Temiang.Avicenna.BusinessObject.Generated;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.WebService
{
    public class ApiResponeForExecutiveMembership
    {
        public static void Success(HttpContext context, object data, string message = "OK")
        {
            var json = JsonConvert.SerializeObject(new
            {
                success = true,
                code = 200,
                errorCode = (string)null,
                message = message,
                data = data
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 200;
            context.Response.Write(json);
            context.Response.Flush();
            context.Response.SuppressContent = true;
            context.ApplicationInstance.CompleteRequest();
        }

        public static void Error(HttpContext context, string message, int code = 500)
        {
            var json = JsonConvert.SerializeObject(new
            {
                success = false,
                code = code,
                errorCode = "ERR",
                message = message,
                data = (object)null
            });

            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            context.Response.Write(json);

            context.ApplicationInstance.CompleteRequest();
        }
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class ExecutiveMembershipRSI : System.Web.Services.WebService
    {

        [WebMethod(EnableSession = true, Description = @"
        Insert Member Executive berdasarkan PatientID atau MedicalNo.

        PARAMETER:
        - PatientID (optional)
        - MedicalNo (optional)

        Pilih salah satu.
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void InsertNewMember()
        {

            if (AppSession.UserLogin == null || string.IsNullOrEmpty(AppSession.UserLogin.UserID))
            {
                AppSession.UserLogin = new UserLogin
                {
                    UserID = "WEBSERVICE",
                    UserName = "Web Service"
                };
            }

            try
            {
                string patientID =
                    (Context.Request["PatientID"] ?? "")
                    .Trim();

                string medicalNo =
                    (Context.Request["MedicalNo"] ?? "")
                    .Trim();

                if (string.IsNullOrWhiteSpace(patientID)
                    && string.IsNullOrWhiteSpace(medicalNo))
                {
                    ApiResponeForExecutiveMembership.Error(
                        HttpContext.Current,
                        "PatientID atau MedicalNo wajib diisi.",
                        400
                    );
                    return;
                }

                var patient = new Patient();

                bool found = false;

                if (!string.IsNullOrWhiteSpace(patientID))
                {
                    found = patient.LoadByPrimaryKey(patientID);
                }
                else
                {
                    found = patient.LoadByMedicalNo(medicalNo);
                }

                if (!found)
                {
                    ApiResponeForExecutiveMembership.Error(
                        HttpContext.Current,
                        "Data pasien tidak ditemukan.",
                        404
                    );
                    return;
                }

                var checkMembership = new Membership();

                checkMembership.Query.Where(
                    checkMembership.Query.PatientID.Equal(patient.PatientID)
                );

                if (checkMembership.Query.Load())
                {
                    ApiResponeForExecutiveMembership.Error(
                        HttpContext.Current,
                        "Pasien sudah terdaftar sebagai Executive Membership.",
                        400
                    );
                    return;
                }

                var autoNumber = Helper.GetNewAutoNumber(
                    (new DateTime()).NowAtSqlServer().Date,
                    AppEnum.AutoNumber.MembershipNo
                );


                var membership = new Membership();

                membership.AddNew();

                membership.MembershipNo = autoNumber.LastCompleteNumber;
                membership.SRMembershipType = "01";
                membership.JoinDate = (new DateTime()).NowAtSqlServer().Date;

                membership.PatientID = patient.PatientID;
                membership.PersonID = patient.PersonID;
                membership.MemberName = patient.PatientName;
                membership.SRSalutation = patient.SRSalutation;
                membership.Sex = patient.Sex;
                membership.CityOfBirth = patient.CityOfBirth;
                membership.DateOfBirth = patient.DateOfBirth;
                membership.Address = patient.Address;
                membership.PhoneNo = patient.PhoneNo;
                membership.MobilePhoneNo = patient.MobilePhoneNo;
                membership.Email = patient.Email;

                membership.IsActive = true;
                membership.CreateDateTime = (new DateTime()).NowAtSqlServer();
                membership.CreateByUserID = "WEBSERVICE";
                membership.LastUpdateDateTime = (new DateTime()).NowAtSqlServer();
                membership.LastUpdateByUserID = "WEBSERVICE";

                membership.MembershipNo = autoNumber.LastCompleteNumber;

                autoNumber.Save();

                membership.Save();

                ApiResponeForExecutiveMembership.Success(
                    HttpContext.Current,
                    new
                    {
                        membership.MembershipNo,
                        membership.PatientID,
                        membership.MemberName,
                        membership.SRSalutation,
                        membership.Sex,
                        membership.CityOfBirth,
                        membership.DateOfBirth,
                        membership.Address,
                        membership.PhoneNo,
                        membership.MobilePhoneNo,
                        membership.Email,
                        membership.IsActive,
                        membership.CreateDateTime,
                        membership.CreateByUserID,
                    },
                    "Member berhasil dibuat."
                );
            }
            catch (Exception ex)
            {
                ApiResponeForExecutiveMembership.Error(
                    HttpContext.Current,
                    ex.Message
                );
            }
        }

        [WebMethod(EnableSession = true, Description = @"
        Get Data Executive Membership.

        PARAMETER:
        - MembershipNo (optional)
        - PatientID (optional)
        - MedicalNo (optional)

        Minimal salah satu harus diisi.
        ")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void GetPatientMembership()
        {
            try
            {
                string membershipNo = (Context.Request["MembershipNo"] ?? "").Trim();
                string patientID = (Context.Request["PatientID"] ?? "").Trim();
                string medicalNo = (Context.Request["MedicalNo"] ?? "").Trim();

                if (string.IsNullOrWhiteSpace(membershipNo) &&
                    string.IsNullOrWhiteSpace(patientID) &&
                    string.IsNullOrWhiteSpace(medicalNo))
                {
                    ApiResponeForExecutiveMembership.Error(
                        HttpContext.Current,
                        "MembershipNo atau PatientID atau MedicalNo wajib diisi.",
                        400
                    );
                    return;
                }

                Membership membership = new Membership();

                bool found = false;

                // Cari berdasarkan MembershipNo
                if (!string.IsNullOrWhiteSpace(membershipNo))
                {
                    found = membership.LoadByPrimaryKey(membershipNo);
                }
                // Cari berdasarkan PatientID
                else if (!string.IsNullOrWhiteSpace(patientID))
                {
                    membership.Query.Where(
                        membership.Query.PatientID.Equal(patientID)
                    );

                    found = membership.Query.Load();
                }
                // Cari berdasarkan MedicalNo
                else
                {
                    Patient patient = new Patient();

                    if (!patient.LoadByMedicalNo(medicalNo))
                    {
                        ApiResponeForExecutiveMembership.Error(
                            HttpContext.Current,
                            "Data pasien tidak ditemukan.",
                            404
                        );
                        return;
                    }

                    membership.Query.Where(
                        membership.Query.PatientID.Equal(patient.PatientID)
                    );

                    found = membership.Query.Load();
                }

                if (!found)
                {
                    ApiResponeForExecutiveMembership.Error(
                        HttpContext.Current,
                        "Data Membership tidak ditemukan.",
                        404
                    );
                    return;
                }

                ApiResponeForExecutiveMembership.Success(
                    HttpContext.Current,
                    new
                    {
                        membership.MembershipNo,
                        membership.PatientID,
                        membership.PersonID,
                        membership.SRMembershipType,
                        membership.JoinDate,
                        membership.MemberName,
                        membership.SRSalutation,
                        membership.Sex,
                        membership.CityOfBirth,
                        membership.DateOfBirth,
                        membership.Address,
                        membership.PhoneNo,
                        membership.MobilePhoneNo,
                        membership.Email,
                        membership.IsActive,
                        membership.CreateDateTime,
                        membership.CreateByUserID,
                        membership.LastUpdateDateTime,
                        membership.LastUpdateByUserID
                    },
                    "Data Membership berhasil diambil."
                );
            }
            catch (Exception ex)
            {
                ApiResponeForExecutiveMembership.Error(
                    HttpContext.Current,
                    ex.Message
                );
            }
        }


    }
}
