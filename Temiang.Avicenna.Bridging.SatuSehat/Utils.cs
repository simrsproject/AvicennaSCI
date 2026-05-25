using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Web;
using Temiang.Avicenna.Bridging.SatuSehat.BusinessObject;
using Temiang.Avicenna.Bridging.SatuSehat.Common;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Common;
using Temiang.Avicenna.BusinessObject.Reference;
using Temiang.Avicenna.Common;
using Fraction = Temiang.Avicenna.BusinessObject.Common.Fraction;
using System.Text.RegularExpressions;

namespace Temiang.Avicenna.Bridging.SatuSehat
{

    public class Utils : Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.BaseUtil
    {
        public Utils()
        {
            // Handle error The request was aborted: Could not create SSL/TLS secure channel (Handono 20260411)
            System.Net.ServicePointManager.SecurityProtocol =
                System.Net.SecurityProtocolType.Tls12 |
                System.Net.SecurityProtocolType.Tls11 |
                System.Net.SecurityProtocolType.Tls;
        }

        #region Consent
        public Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.ConsentResponse.Root GetConsent(string patientID)
        {
            if (string.IsNullOrWhiteSpace(patientID) || string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ClientID))
                return null;

            var accessToken = string.Empty;

            var id = PatientBridgingID(patientID, string.Empty, string.Empty, ref accessToken);
            if (string.IsNullOrWhiteSpace(patientID))
                return null;

            var response = RestClientGet(string.Format("{0}/Consent?patient_id={1}", ConsentUrl, id), ref accessToken);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.ConsentResponse.Root>(response.Content);
            }
            return null;
        }

        public void PostConsent(bool isApprove, string patientID, string userID, string userName)
        {
            if (string.IsNullOrWhiteSpace(patientID) || string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ClientID))
                return;

            var accessToken = string.Empty;

            var id = PatientBridgingID(patientID, string.Empty, string.Empty, ref accessToken);
            if (string.IsNullOrWhiteSpace(patientID))
                return;

            var postData = new
            {
                patient_id = id,
                action = isApprove ? "OPTIN" : "OPTOUT",
                agent = string.Format("{0} [{1}]", userName, userID)
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            var url = string.Format("{0}/Consent", ConsentUrl);
            var response = RestClientExecute(requestBody, url, ref accessToken, Method.Post);
        }

        #endregion Consent

        #region helper
        public static string CleanHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            input = Regex.Replace(input, "<br ?/?>", "\n", RegexOptions.IgnoreCase);

            var noHtml = Regex.Replace(input, "<.*?>", string.Empty);

            noHtml = WebUtility.HtmlDecode(noHtml);

            return noHtml.Trim();
        }
        #endregion

        #region Pelayanan Rawat Jalan
        public string EncounterPost(string registrationNo, ref SatuSehatKunjungan satuSehatLog, ref Registration reg, ref PatientBridging patSs, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs, ref string accessToken, string encounterType, bool isJustForEncounterPost = false)
        {
            var encounterId = string.Empty;
            satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(registrationNo))
            {
                if (satuSehatLog.EncounterID != null)
                {
                    encounterId = satuSehatLog.EncounterID.ToString();
                    if (isJustForEncounterPost)
                        return encounterId;
                }
            }
            else
            {
                satuSehatLog.RegistrationNo = registrationNo;
            }

            reg = new Registration();
            reg.LoadByPrimaryKey(registrationNo);

            if (reg.SRRegistrationType == "IPR") return string.Empty; // Belum untuk rawat inap

            var pat = new Patient();
            if (!pat.LoadByPrimaryKey(reg.PatientID))
            {
                satuSehatLog.ErrorResponse = "Can not find this patient";
                satuSehatLog.Save();
                return string.Empty;
            }


            if (string.IsNullOrWhiteSpace(pat.Ssn) && string.IsNullOrWhiteSpace(pat.MotherSsn))
            {
                satuSehatLog.ErrorResponse = "SSN can not empty, please complete the SSN / Mother SSN on the master patient";
                satuSehatLog.Save();
                return string.Empty;
            }

            patSs = LoadPatientBridging(pat.PatientID);
            if (string.IsNullOrWhiteSpace(patSs.BridgingID))
            {
                // Retrieve SS Patient ID
                var response = new RestResponse();

                if (!string.IsNullOrEmpty(pat.Ssn))
                    response = RestClientGet("Patient?identifier=https://fhir.kemkes.go.id/id", string.Concat("nik|", pat.Ssn), ref accessToken);
                else if (!string.IsNullOrEmpty(pat.MotherSsn))
                    response = RestClientGet("Patient?identifier=https://fhir.kemkes.go.id/id", string.Concat("nik-ibu|", pat.MotherSsn, "&birthdate=", pat.DateOfBirth.Value.ToString("yyyy-MM-dd")), ref accessToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var patientSearchResponse = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.PatientSearch.PatientSearchResponse>(response.Content);
                    if (patientSearchResponse.Total == 1)
                    {
                        // Add PatientBridging
                        if (string.IsNullOrEmpty(patSs.PatientID))
                        {
                            patSs = new PatientBridging();
                        }

                        patSs.PatientID = pat.PatientID;
                        patSs.BridgingID = patientSearchResponse.Entry[0].Resource.Id;
                        //patSs.BridgingName = patientSearchResponse.Entry[0].Resource.Name[0].Text; //Mulai 2023 Okt 12 sudah tidak bisa
                        patSs.BridgingName = pat.PatientName;
                        patSs.SRBridgingType = SatuSehatBridgingType;
                        patSs.IsActive = true;
                        patSs.Save();
                    }
                    else
                    {
                        satuSehatLog.ErrorResponse = string.Format("SSN {0} not found at fhir.kemkes.go.id", pat.Ssn);
                        satuSehatLog.Save();
                        return string.Empty;
                    }
                }
                else
                {
                    satuSehatLog.ErrorResponse = string.Format("Please check SSN. {0}. {1}", response.ErrorMessage, response.Content);
                    satuSehatLog.Save();
                    return string.Empty;
                }
            }


            parMedicSs = LoadPerformerByParamedicID(reg.ParamedicID);
            if (parMedicSs == null)
            {
                var par = new Paramedic();
                par.LoadByPrimaryKey(reg.ParamedicID);
                satuSehatLog.ErrorResponse = string.Format("BridgingID for Physician {0} still empty", par.ParamedicName);
                satuSehatLog.Save();
                return string.Empty;
            }

            locSs = LoadLocation(reg.ServiceUnitID);
            if (locSs == null)
            {
                var su = new ServiceUnit();
                su.LoadByPrimaryKey(reg.ServiceUnitID);
                satuSehatLog.ErrorResponse = string.Format("BridgingID for Service Unit {0} still empty", su.ServiceUnitName);
                satuSehatLog.Save();
                return string.Empty;
            }


            if (string.IsNullOrWhiteSpace(encounterId))
                encounterId = PostEncounter(reg, patSs, parMedicSs, locSs, encounterType, ref accessToken); // Kunjungan

            return encounterId;
        }


        public void PostDataToSatuSehat(string registrationNo, ref string accessToken)
        {
            Registration reg = null;
            PatientBridging patSs = null;
            ParamedicBridging parMedicSs = null;
            SatuSehatKunjungan satuSehatLog = null;
            ServiceUnitBridging locSs = null;
            var epq = new EpisodeDiagnoseQuery("e");
            epq.Select(epq.DiagnoseID, epq.DiagnosisText, epq.SRDiagnoseType, epq.CreateDateTime);
            epq.Where(epq.RegistrationNo == registrationNo, epq.IsVoid == false);
            epq.OrderBy(epq.SequenceNo.Ascending);
            var dtbDiagnoseCheck = epq.LoadDataTable();
            string encounterType = "defaultSs";
            //TB
            var tbIdString = AppParameter.GetParameterValue(AppParameter.ParameterItem.SitbDiagnoseList);
            var tbIdList = tbIdString?.Split(',').ToList() ?? new List<string>();
            //CAD
            string cadId = "I25";
            foreach (DataRow row in dtbDiagnoseCheck.Rows)
            {
                var diagnoseId = row["DiagnoseID"]?.ToString();
                if (!string.IsNullOrEmpty(diagnoseId))
                {
                    //if (tbIdList.Any(tbid => diagnoseId.StartsWith(tbid)))
                    //{
                    //    encounterType = "TB";
                    //    break;
                    //}
                    if (diagnoseId.StartsWith(cadId))
                    {
                        encounterType = "CAD";
                        break;
                    }
                }
            }

            var encounterId = EncounterPost(registrationNo, ref satuSehatLog, ref reg, ref patSs, ref parMedicSs, ref locSs, ref accessToken, encounterType, false);

            if (string.IsNullOrEmpty(encounterId)) return;

            // 02. Pendaftaran Kunjungan Rawat Jalan


            // 03. Anamnesis
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;

            if (pa.Query.Load())
            {
                //03.1. Keluhan Utama
                PostPatientChiefComplaint(reg, pa, patSs, encounterId, ref accessToken);
                // Impresi Klinis
                var mds = new MedicalDischargeSummary();
                mds.Query.Where(mds.Query.RegistrationNo == reg.RegistrationNo);
                mds.Query.es.Top = 1;
                if (mds.Query.Load())
                {
                    PostClinicalImpression(mds, reg, patSs, parMedicSs, pa, encounterId, ref accessToken);
                }
            }

            // 03.2. Riwayat Penyakit
            // TODO: Riwayat Penyakit

            // 03.3. Riwayat Alergi
            // TODO: Riwayat Alergi
            var patal = new PatientAllergy();
            patal.Query.Where(patal.Query.PatientID == reg.PatientID, patal.Query.SRAllergyCategory == "Medication");
            patal.Query.es.Top = 1;
            if (patal.Query.Load())
            {
                PostPatientAllergy(reg, patal, patSs, parMedicSs, encounterId, ref accessToken);
            }

            // 03.4. Riwayat Pengobatan
            // 03.4.1 Obat bukan dari Fasyankes Sendiri
            // Belum ada entriannya

            // 03.4.2 Obat bukan dari Fasyankes Sendiri
            PostMedicationStatement(reg, patSs, parMedicSs, encounterId, ref accessToken);

            // 04. Hasil Pemeriksaan Fisik
            // 04.1. Pemeriksaan Tanda Tanda Vital
            // 04.1.1. Pemeriksaan Tanda Tanda Vital - Hearth Rate
            PostObservation(reg, patSs, parMedicSs, encounterId, accessToken, VitalSign.VitalSignEnum.HeartRate);

            // 04.1.2 Pemeriksaan Tanda Tanda Vital - Pernafasan
            PostObservation(reg, patSs, parMedicSs, encounterId, accessToken, VitalSign.VitalSignEnum.RespiratoryRate);

            // 04.1.3 Pemeriksaan Tanda Tanda Vital - Sistol
            PostObservation(reg, patSs, parMedicSs, encounterId, accessToken, VitalSign.VitalSignEnum.BloodPressureSistolic);

            // 04.1.4 Pemeriksaan Tanda Tanda Vital - Diastol
            PostObservation(reg, patSs, parMedicSs, encounterId, accessToken, VitalSign.VitalSignEnum.BloodPressureDiastolic);

            // 04.1.5 Pemeriksaan Tanda Tanda Vital - Suhu
            PostObservation(reg, patSs, parMedicSs, encounterId, accessToken, VitalSign.VitalSignEnum.Temperature);

            // 04.2. Tingkat Kesadaran AVPU alert, verbal, pain, unresponsive
            //PostConsciousnessLevel(reg, patSs, parMedicSs, encounterId, ref accessToken); 

            // 05. Pemeriksaan Psikologis
            // Belum tahu entriannya

            //06. Rencana Rawat Pasien
            if (pa.AssessmentDateTime != null)
                PostCarePlanRawatPasien(reg, patSs, parMedicSs, pa, encounterId, ref accessToken);

            // 08. Pemeriksaan Penunjang
            // 08.1 Laboratorium
            PostServiceRequest(reg, patSs, parMedicSs, encounterId, ref accessToken);

            if (!AppSession.Parameter.IsUsingHisInterop)
            {
                PostServiceRequestLabOff(reg, patSs, parMedicSs, encounterId, ref accessToken);
            }

            //08.2 Radiologi
            if (Temiang.Avicenna.Common.AppSession.Parameter.IsUsingRisPacsInterop)
            {
                if (Temiang.Avicenna.Common.AppSession.Parameter.HealthcareInitialAppsVersion == "RSUSKY")
                {
                    PostServiceRequestRad(reg, patSs, parMedicSs, encounterId, ref accessToken);

                    var ancill = new TransChargesQuery("ancill");
                    var tci = new TransChargesItemQuery("tci");
                    var i = new ItemQuery("i");
                    var ib = new ItemBridgingQuery("ib");
                    ancill.LeftJoin(tci).On(tci.TransactionNo == ancill.TransactionNo);
                    ancill.LeftJoin(i).On(tci.ItemID == i.ItemID);
                    ancill.LeftJoin(ib).On(tci.ItemID == ib.ItemID);

                    ancill.Select(
                        tci.TransactionNo,
                        tci.SequenceNo
                    );

                    ancill.Where(
                        ancill.RegistrationNo == registrationNo,
                        tci.IsSendToLIS == true,
                        i.IsHasTestResults == true,
                        ib.SRBridgingType == SatuSehatBridgingType
                    );

                    var ancillCollection = new TransChargesCollection();
                    if (ancillCollection.Load(ancill) && ancillCollection.Count > 0)
                    {
                        var trno = ancillCollection[0].TransactionNo;
                        var seqno = ancillCollection[0].GetColumn("SequenceNo").ToString();
                        GetImagingStudy(reg, patSs, parMedicSs, trno, seqno, encounterId, ref accessToken);
                    }
                }
                else if (Temiang.Avicenna.Common.AppSession.Parameter.HealthcareInitialAppsVersion == "RSEE")
                {
                    var diag = dtbDiagnoseCheck.AsEnumerable()
                        .OrderByDescending(d => d.Field<DateTime>("CreateDateTime"))
                        .FirstOrDefault(d => new string[] { AppSession.Parameter.DiagnoseTypeMain, "DiagnoseType-006" }.Contains(d.Field<string>("SRDiagnoseType")));
                    if (diag != null)
                    {
                        var serviceUnitRadiologyID = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyID);
                        var serviceUnitRadiologyIdArray = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyIdArray);

                        // Query charge items PACS based
                        var tci = new TransChargesItemQuery("tci");

                        var tc = new TransChargesQuery("tc");
                        tci.InnerJoin(tc).On(tci.TransactionNo == tc.TransactionNo);

                        var i = new ItemQuery("i");
                        tci.InnerJoin(i).On(tci.ItemID == i.ItemID && i.ItemIDExternal.IsNotNull() && i.ItemIDExternal != string.Empty && i.IsActive == true);

                        var ir = new ItemRadiologyQuery("ir");
                        tci.InnerJoin(ir).On(i.ItemID == ir.ItemID);

                        tci.Where(
                            tc.RegistrationNo == reg.RegistrationNo,
                            tc.IsOrder == true,
                            tc.IsApproved == true,
                            tci.Or(
                                tc.ToServiceUnitID == serviceUnitRadiologyID,
                                tc.ToServiceUnitID.In(serviceUnitRadiologyIdArray)
                            ),
                            tci.IsOrderRealization == true,
                            tci.IsVoid == false,
                            tci.IsSendToLIS == true
                        );
                        tci.Select(
                            tci.TransactionNo,
                            tci.SequenceNo
                        );

                        var dtb = tci.LoadDataTable();

                        //foreach (DataRow row in dtb.Rows)
                        //{
                        //    try
                        //    {
                        //        var root =
                        //            new Temiang.Avicenna.Common.Worklist.EFARINA.Json.SendSatuSehat.Request.Root()
                        //            {
                        //                PatientId = patSs.BridgingID,
                        //                PatientName = patSs.BridgingName,
                        //                EncounterId = encounterId,
                        //                RequesterPracticionerId = parMedicSs.BridgingID,
                        //                RequesterPracticionerName = parMedicSs.BridgingName,
                        //                IcdDiagnoseId = diag["DiagnoseID"].ToString(),
                        //                IcdDiagnoseName = diag["DiagnosisText"].ToString(),
                        //                OrderNo = $"{row["TransactionNo"].ToString()}#{row["SequenceNo"].ToString()}"
                        //            };

                        //        var svc = new Temiang.Avicenna.Common.Worklist.EFARINA.Service();
                        //        var response = svc.SendSatuSehatAll(root);

                        //        var log = new WebServiceAPILog
                        //        {
                        //            DateRequest = DateTime.Now,
                        //            IPAddress = Helper.GetUserHostName(),
                        //            UrlAddress = "SendSatuSehatAll",
                        //            Params = JsonConvert.SerializeObject(root),
                        //            Response = JsonConvert.SerializeObject(response),
                        //            Totalms = 0
                        //        };
                        //        log.Save();
                        //    }
                        //    catch
                        //    {
                        //        continue;
                        //    }
                        //}
                    }
                }
            }


            //08.2 Radiologi ServiceRequest -> Observation -> DiagnosticReport
            //Service Request
            PostServiceRequestRad(reg, patSs, parMedicSs, encounterId, ref accessToken);

            var serviceUnitRadiologyID2 = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyID);
            var serviceUnitRadiologyIdArray2 = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyIdArray);

            var tci2 = new TransChargesItemQuery("tci2");

            var tc2 = new TransChargesQuery("tc2");
            tci2.InnerJoin(tc2).On(tci2.TransactionNo == tc2.TransactionNo);

            var i2 = new ItemQuery("i2");
            tci2.InnerJoin(i2).On(tci2.ItemID == i2.ItemID && i2.IsActive == true);

            var itg2 = new ItemGroupQuery("itg2");
            tci2.InnerJoin(itg2).On(i2.ItemGroupID == itg2.ItemGroupID);

            var ir2 = new ItemRadiologyQuery("ir2");
            tci2.InnerJoin(ir2).On(i2.ItemID == ir2.ItemID);

            var ib2 = new ItemBridgingQuery("ib2");
            tci2.InnerJoin(ib2).On(tci2.ItemID == ib2.ItemID && ib2.SRBridgingType == SatuSehatBridgingType);

            var tr2 = new TestResultQuery("tr2");
            tci2.InnerJoin(tr2).On(
                tci2.TransactionNo == tr2.TransactionNo &&
                tci2.ItemID == tr2.ItemID
            );

            tci2.Where(
                tc2.RegistrationNo == reg.RegistrationNo,
                tc2.IsOrder == true,
                tc2.IsApproved == true,
                tci2.Or(
                    tc2.ToServiceUnitID == serviceUnitRadiologyID2,
                    tc2.ToServiceUnitID.In(serviceUnitRadiologyIdArray2)
                ),
                tci2.IsOrderRealization == true,
                tci2.IsVoid == false
            );

            tci2.Select(
                tci2.TransactionNo,
                tci2.SequenceNo,
                tci2.ItemID,
                tci2.RealizationDateTime,
                i2.ItemName,
                tr2.TestResult,
                tr2.TestResultDateTime,
                tr2.ParamedicID,
                ib2.BridgingID,
                ib2.BridgingName,
                itg2.Initial
            );

            var dtb2 = tci2.LoadDataTable();

            foreach (DataRow row in dtb2.Rows)
            {
                var transactionNo = row["TransactionNo"].ToString();
                var sequenceNo = row["SequenceNo"].ToString();
                var itemId = row["BridgingID"]?.ToString();
                var itemName = row["BridgingName"]?.ToString();
                var rawResult = row["TestResult"]?.ToString();
                var result = CleanHtml(rawResult);
                var resultDate = row["TestResultDateTime"] != DBNull.Value
                    ? Convert.ToDateTime(row["TestResultDateTime"])
                    : DateTime.Now;

                var item = new Item
                {
                    ItemID = itemId,
                    ItemName = itemName
                };

                // ambil ServiceRequestID dari log sebelumnya
                var sr = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
                if (sr == null || sr.ResultID == null) continue;

                var serviceRequestId = sr.ResultID?.ToString();

                // OBSERVATION
                PostObservationRad(
                    reg,
                    patSs,
                    parMedicSs,
                    serviceRequestId,
                    transactionNo,
                    sequenceNo,
                    item,
                    resultDate,
                    result,
                    encounterId,
                    ref accessToken
                );

                // ambil ObservationID
                var obs = LoadSatuSehatResult(encounterId, "Observation", transactionNo, sequenceNo);
                if (obs == null || obs.ResultID == null) continue;

                var observationId = obs.ResultID?.ToString();

                // DIAGNOSTIC REPORT
                PostDiagnosticReportRad(
                    reg,
                    patSs,
                    parMedicSs,
                    transactionNo,
                    sequenceNo,
                    item,
                    resultDate,
                    observationId,
                    serviceRequestId,
                    encounterId,
                    result, // conclusion
                    ref accessToken
                );
            }

            // 09. Tindakan/Prosedur Medis
            PostProcedure(reg, patSs, encounterId, ref accessToken);

            // 10. Diagnosis
            var dtbDiagnosisResult = PostDiagnosis(reg, patSs, encounterId, ref accessToken);
            var isDiagnosisExist = dtbDiagnosisResult.Rows.Count > 0;

            // 11. Diet
            PostCompositionDiet(reg, patSs, parMedicSs, encounterId, ref accessToken);


            // 12. Tatalaksana
            // 12.1. Edukasi
            PostMedicationEducation(reg, patSs, parMedicSs, encounterId, ref accessToken);


            // 12.2.1 & 3 Peresepan Obat & Pengeluaran Obat
            if (isDiagnosisExist)
                PostMedication(reg, patSs, parMedicSs, dtbDiagnosisResult, encounterId, ref accessToken);

            // 12.2.2. Pengkajian Resep
            PostPengkajianResep(reg, patSs, parMedicSs, encounterId, ref accessToken);

            // Kondisi Saat Pulang



            // #6. Radiology

            // Imunisasi
            PostImmunization(reg, patSs, parMedicSs, encounterId, ref accessToken);

            // Update Finish status
            if (isDiagnosisExist)
            {
                string episodeOfCareId = string.Empty;
                if (encounterType != "defaultSs")
                    episodeOfCareId = PostEpisodeOfCare(registrationNo, ref reg, ref patSs, encounterId, encounterType, ref accessToken);
                if (!string.IsNullOrEmpty(episodeOfCareId))
                    EpisodeOfCarePatchData(reg, episodeOfCareId, encounterId, encounterType, ref accessToken);
                PostEncounterFinished(reg, patSs, parMedicSs, locSs, dtbDiagnosisResult, episodeOfCareId, encounterType, ref accessToken); // Kunjungan Finish
                // Close
                Close(reg.RegistrationNo);
            }

        }
        private void Close(string registrationNo)
        {
            var isNoError = true;
            var satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(registrationNo))
            {
                if (string.IsNullOrWhiteSpace(satuSehatLog.ErrorResponse))
                {
                    var ssResults = new SatuSehatResultCollection();
                    ssResults.Query.Where(ssResults.Query.EncounterID == satuSehatLog.EncounterID);
                    ssResults.Query.Select(satuSehatLog.Query.ErrorResponse);
                    ssResults.LoadAll();
                    foreach (var ssResult in ssResults)
                    {
                        if (!string.IsNullOrEmpty(ssResult.ErrorResponse))
                        {
                            isNoError = false;
                            break;
                        }
                    }

                }
            }

            if (isNoError)
            {
                satuSehatLog.IsClosed = true;
                satuSehatLog.Save();
            }
        }

        #region 02. Pendaftaran Kunjungan Rawat Jalan
        #region Encounter / Kunjungan
        private string PostEncounter(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterType, ref string accessToken)
        {
            var encounterId = string.Empty;
            var encounterPostData = EncounterPostData(reg, patSs, parSs, locSs, encounterType);
            var requestBody = JsonConvert.SerializeObject(encounterPostData);

            var satuSehatLog = new SatuSehatKunjungan();
            if (!satuSehatLog.LoadByPrimaryKey(reg.RegistrationNo))
                satuSehatLog = new SatuSehatKunjungan();

            satuSehatLog.KunjunganPostData = requestBody;
            satuSehatLog.RegistrationNo = reg.RegistrationNo;
            satuSehatLog.str.ErrorResponse = string.Empty;

            var response = RestClientPost(requestBody, "Encounter", ref accessToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var encounterResponse = JsonConvert.DeserializeObject<EncounterResponse>(response.Content);
                if (!string.IsNullOrEmpty(encounterResponse.Id))
                {
                    encounterId = encounterResponse.Id;
                    satuSehatLog.EncounterID = new Guid(encounterResponse.Id);
                }
            }
            else
            {
                satuSehatLog.ErrorResponse = string.Format("{0}. {1}", response.ErrorMessage, response.Content);

                // Error Found duplicate resource: Encounter RuleNumber: 20002
                if (satuSehatLog.ErrorResponse.Contains("RuleNumber: 20002"))
                {
                    // Ambil EncounterID
                    //var sampleUrl = "Encounter?subject=P08638876323&identifier=REG/EM/260409-0004";
                    var url = string.Format("{0}/Encounter?subject={1}&identifier={2}", BaseUrl, patSs.BridgingID, reg.RegistrationNo);

                    var getEncounter = RestClientExecute(string.Empty, url, ref accessToken, Method.Get);
                    if (getEncounter.StatusCode == System.Net.HttpStatusCode.Created || getEncounter.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        try
                        {
                            // Parse into JObject
                            var jo = JObject.Parse(getEncounter.Content);

                            // Get first entry resource id
                            string firstEncounterId = (string)jo.SelectToken("entry[0].resource.id");
                            if (!string.IsNullOrEmpty(firstEncounterId))
                            {
                                satuSehatLog.EncounterID = new Guid(firstEncounterId);
                                satuSehatLog.str.ErrorResponse = string.Empty;

                                encounterId = firstEncounterId;
                            }
                        }
                        catch (Exception)
                        {

                            // throw;
                        }

                    }
                }
            }

            satuSehatLog.Save();

            return encounterId;
        }

        private string PostEncounterFinished(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, DataTable dtbDiagnosisResult, string episodeOfCareId, string encounterType, ref string accessToken)
        {
            // Update status Finish
            var satuSehatLog = new SatuSehatKunjungan();
            if (!satuSehatLog.LoadByPrimaryKey(reg.RegistrationNo))
                return string.Empty;

            var encounterId = satuSehatLog.EncounterID.ToString();
            var encounterPostData = EncounterFinishPutData(reg, patSs, parSs, locSs, dtbDiagnosisResult, encounterId, episodeOfCareId, encounterType);
            var requestBody = JsonConvert.SerializeObject(encounterPostData);
            satuSehatLog.KunjunganPostData = requestBody;
            satuSehatLog.str.ErrorResponse = string.Empty;

            var response = RestClientPut(requestBody, string.Format("Encounter/{0}", encounterId), ref accessToken);

            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            { }
            else
            {
                satuSehatLog.ErrorResponse = string.Format("{0}. {1}", response.ErrorMessage, response.Content);
            }

            satuSehatLog.Save();

            return encounterId;
        }
        private EncounterPost EncounterPostData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterType)
        {
            //string patName = patSs.BridgingName;
            //if(encounterType == "TB")
            //    patName = patSs.BridgingName.Substring(0, 2) + new string('*', patSs.BridgingName.Length - 2);
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() { new Coding()
                            {
                                System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                Code = "ATND",
                                Display = "attender"
                            } };
            var types = new List<Code>()
                            {new Code(){ Coding= codings}  };


            var par = new Paramedic();
            par.LoadByPrimaryKey(reg.ParamedicID);
            postData.Participant = new List<Participant>() {
                                    new Participant(){Individual= new Individual(){ Reference= string.Format("Practitioner/{0}",parSs.BridgingID),
                        Display= parSs.BridgingName}, Type = types } };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference= string.Format("Location/{0}",locSs.BridgingID),
                        Display= locSs.BridgingName
                    },
                    Extension = new List<Bridging.SatuSehat.BusinessObject.ExtensionLoc>()
                    {
                        new ExtensionLoc()
                        {
                            Url = "https://fhir.kemkes.go.id/r4/StructureDefinition/ServiceClass",
                            ExtensionItem = new List<ExtensionItem>()
                                            {
                                                new ExtensionItem()
                                                {
                                                    Url= "value",
                                                    ValueCodeableConcept = new Code()
                                                    {
                                                        Coding = new List<Coding>(){ new Coding()
                                                            {
                                                                System = "http://terminology.kemkes.go.id/CodeSystem/locationServiceClass-Outpatient",
                                                                Code = "reguler",
                                                                Display = "Kelas Reguler"
                                                            }
                                                        }
                                                    }

                                                }
                                            }
                            }
                    }
                }
            };


            // StatusHistory
            postData.StatusHistory = new List<StatusHistory>();
            var regTimes = reg.RegistrationTime.Split(':');
            var arrivedTime = reg.RegistrationDate.Value;
            arrivedTime = new DateTime(arrivedTime.Year, arrivedTime.Month, arrivedTime.Day, regTimes[0].ToInt(),
                regTimes[1].ToInt(), 0);

            var startInprogressTime = arrivedTime;
            var finishedTime = arrivedTime;

            //var startInprogress = string.Empty;

            // Jam dipanggil
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Descending);
            if (pa.Query.Load())
            {
                if (arrivedTime > pa.AssessmentDateTime.Value) //Kasus RegistrationTime tidak sesuai dgn jam kedatangan (Contoh dari Appointment)
                    arrivedTime = reg.LastCreateDateTime.Value;

                startInprogressTime = pa.AssessmentDateTime.Value;

                postData.Status = "in-progress"; //Change status
            }
            else
                startInprogressTime = arrivedTime.AddMinutes(5); // tidak diketahui jam dipanggilnya sehingga anggap saja 5 menit

            // selesai ketika diberi resep
            var presc = new TransPrescription();
            presc.Query.Where(presc.Query.RegistrationNo == reg.RegistrationNo, presc.Query.IsApproval == true);
            presc.Query.es.Top = 1;
            presc.Query.OrderBy(presc.Query.PrescriptionDate.Descending);
            if (presc.Query.Load())
            {
                if (startInprogressTime > presc.CreatedDateTime.Value) // Kasus asesmen dientry setelah resep dibuat
                {
                    startInprogressTime = presc.CreatedDateTime.Value.AddMinutes(-1);
                }
                if (encounterType != "TB")
                {
                    postData.StatusHistory.Add(new StatusHistory()
                    {
                        Status = "in-progress",
                        Period = new Period()
                        {
                            Start = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                            End = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    });
                }


                // Status finish dipindah ke akhir karena harus ada diagnosa dulu
                //// finished 
                //postData.StatusHistory.Add(new StatusHistory()
                //{
                //    Status = "finished",
                //    Period = new Period()
                //    {
                //        Start = string.Format("{0}+{1}:00", presc.CreatedDateTime.Value.ToString(_dateFormat), _gmt),
                //        End = string.Format("{0}+{1}:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.ToString(_dateFormat), _gmt)
                //    }
                //});
                //postData.Status = "finished"; //Change status

            }

            // arrived 
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).ToString(DateFormatLong))
                }
            });


            // Period
            postData.Period = new Period() { Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)) }; //"2022-06-14T07:00:00+07:00"

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            // No kunjungan / registrasi internal
            if (encounterType == "TB")
            {
                postData.StatusHistory.Add(new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                });
                //var asrib = new AppStandardReferenceItemBridging();
                //asrib.LoadByPrimaryKey("ReferralGroup", reg.SRReferralGroup, _satuSehatBridgingType);
                postData.Identifier = new List<Identifier>()
                {
                    new Identifier() { System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo },
                    new Identifier() { System = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}",OrganizationID), Value = reg.RegistrationNo }
                };
                var coding = new List<Coding>() {
                    new Coding() {
                        System = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                        Code = "EHA000002",
                        Display = "Datang sendiri"
                    }
                };
                var hospitalization = new Hospitalization()
                {
                    AdmitSource = new AdmitSource { Coding = coding }
                };
                postData.Hospitalization = hospitalization;
                postData.Location[0].Extension[0].ExtensionItem.Add(new ExtensionItem()
                {
                    Url = "upgradeClassIndicator",
                    ValueCodeableConcept = new Code()
                    {
                        Coding = new List<Coding>()
                            {
                                new Coding()
                                {
                                    System = "http://terminology.kemkes.go.id/CodeSystem/locationUpgradeClass",
                                    Code = "kelas-tetap",
                                    Display = "Kelas Tetap Perawatan"
                                }
                            }
                    }
                });
                postData.Location[0].Period = new Bridging.SatuSehat.BusinessObject.Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong))
                };
            }
            else
            {
                postData.Identifier = new List<Identifier>()
                {
                    new Identifier() { System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo }
                };
            }

            return postData;
        }

        private EncounterFinishPut EncounterFinishPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, DataTable dtbDiagnosisResult, string encounterId, string episodeofCareId, string encounterType)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };

            if (dtbDiagnosisResult.Rows.Count == 0)
                return postData;

            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            foreach (DataRow row in dtbDiagnosisResult.Rows)
            {
                var jsonDiag = JsonConvert.DeserializeObject<ConditionResponse>(row["PostData"].ToString());
                var diag = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
                diag.Condition = new Condition() { Display = jsonDiag.Code.Coding[0].Display, Reference = string.Format("Condition/{0}", row["ResultID"].ToString()) };
                diag.Rank = row["IndexNo"].ToInt();
                diag.Use = new Use() { Coding = new List<Coding> { new Coding() { Code = "DD", Display = "Discharge diagnosis", System = "http://terminology.hl7.org/CodeSystem/diagnosis-role" } } };
                diags.Add(diag);
            }
            postData.Diagnosis = diags;

            postData.ID = encounterId;

            if (encounterType == "PNC")
            {
                postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
                {
                    new Bridging.SatuSehat.BusinessObject.Location()
                    {
                        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                        {
                            Reference= string.Format("Location/{0}",locSs.BridgingID),
                            Display= locSs.BridgingName
                        }
                    }
                };
            }
            else if (encounterType == "INC")
            {
                postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
                {
                    new Bridging.SatuSehat.BusinessObject.Location()
                    {
                        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                        {
                            Reference = string.Format("Location/{0}",locSs.BridgingID),
                            Display = locSs.BridgingName
                        },
                        Period = new Period()
                        {
                            Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong))), //belum tau darimana
                            End = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.DischargeDate.Value.AddHours(GmtDif).ToString(DateFormatLong))) //belum tau darimana
                        }
                    }
                };
            }
            else if (encounterType == "TB")
            {
                postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
                {
                    new Bridging.SatuSehat.BusinessObject.Location()
                    {
                        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                        {
                            Reference= string.Format("Location/{0}",locSs.BridgingID),
                            Display= locSs.BridgingName
                        },
                        Extension = new List<Bridging.SatuSehat.BusinessObject.ExtensionLoc>()
                        {
                            new ExtensionLoc()
                            {
                                Url = "https://fhir.kemkes.go.id/r4/StructureDefinition/ServiceClass",
                                ExtensionItem = new List<ExtensionItem>()
                                {
                                                    new ExtensionItem()
                                                    {
                                                        Url= "value",
                                                        ValueCodeableConcept = new Code()
                                                        {
                                                            Coding = new List<Coding>(){ new Coding()
                                                                {
                                                                    System = "http://terminology.kemkes.go.id/CodeSystem/locationServiceClass-Outpatient",
                                                                    Code = "reguler",
                                                                    Display = "Kelas Reguler"
                                                                }
                                                            }
                                                        }

                                                    },
                                                    new ExtensionItem()
                                                    {
                                                        Url= "upgradeClassIndicator",
                                                        ValueCodeableConcept = new Code()
                                                        {
                                                            Coding = new List<Coding>(){ new Coding()
                                                                {
                                                                    System = "http://terminology.kemkes.go.id/CodeSystem/locationUpgradeClass",
                                                                    Code = "kelas-tetap",
                                                                    Display = "Kelas Tetap Perawatan"
                                                                }
                                                            }
                                                        }

                                                    }
                                }
                            }
                        }
                    }
                };
            }
            else
            {
                postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
                {
                    new Bridging.SatuSehat.BusinessObject.Location()
                    {
                        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                        {
                            Reference= string.Format("Location/{0}",locSs.BridgingID),
                            Display= locSs.BridgingName
                        },
                        Extension = new List<Bridging.SatuSehat.BusinessObject.ExtensionLoc>()
                        {
                            new ExtensionLoc()
                            {
                                Url = "https://fhir.kemkes.go.id/r4/StructureDefinition/ServiceClass",
                                ExtensionItem = new List<ExtensionItem>()
                                                {
                                                    new ExtensionItem()
                                                    {
                                                        Url= "value",
                                                        ValueCodeableConcept = new Code()
                                                        {
                                                            Coding = new List<Coding>(){ new Coding()
                                                                {
                                                                    System = "http://terminology.kemkes.go.id/CodeSystem/locationServiceClass-Outpatient",
                                                                    Code = "reguler",
                                                                    Display = "Kelas Reguler"
                                                                }
                                                            }
                                                        }

                                                    }
                                                }
                                }
                        }
                    }
                };
            }


            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() { new Coding()
                            {
                                System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                Code = "ATND",
                                Display = "attender"
                            } };
            var types = new List<Code>()
                            {new Code(){ Coding= codings}  };


            var par = new Paramedic();
            par.LoadByPrimaryKey(reg.ParamedicID);
            postData.Participant = new List<Participant>() {
                                    new Participant(){Individual= new Individual(){ Reference= string.Format("Practitioner/{0}",parSs.BridgingID),
                        Display= parSs.BridgingName}, Type = types } };

            postData.Status = "finished";

            // StatusHistory
            postData.StatusHistory = new List<StatusHistory>();
            var regTimes = reg.RegistrationTime.Split(':');
            var arrivedTime = reg.RegistrationDate.Value;
            arrivedTime = new DateTime(arrivedTime.Year, arrivedTime.Month, arrivedTime.Day, regTimes[0].ToInt(),
                regTimes[1].ToInt(), 0);

            var startInprogressTime = arrivedTime;
            var finishedTime = arrivedTime;

            //var startInprogress = string.Empty;

            // Jam dipanggil
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Descending);
            if (pa.Query.Load())
            {
                if (arrivedTime > pa.AssessmentDateTime.Value) //Kasus RegistrationTime tidak sesuai dgn jam kedatangan (Contoh dari Appointment)
                    arrivedTime = reg.LastCreateDateTime.Value;

                startInprogressTime = pa.AssessmentDateTime.Value;

                postData.Status = "in-progress"; //Change status
            }
            else
                startInprogressTime = arrivedTime.AddMinutes(5); // tidak diketahui jam dipanggilnya sehingga anggap saja 5 menit

            // selesai ketika diberi resep
            var presc = new TransPrescription();
            presc.Query.Where(presc.Query.RegistrationNo == reg.RegistrationNo, presc.Query.IsApproval == true);
            presc.Query.es.Top = 1;
            presc.Query.OrderBy(presc.Query.PrescriptionDate.Descending);
            if (presc.Query.Load())
            {
                if (startInprogressTime > presc.CreatedDateTime.Value) // Kasus asesmen dientry setelah resep dibuat
                {
                    startInprogressTime = presc.CreatedDateTime.Value.AddMinutes(-1);
                }

                postData.StatusHistory.Add(new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                });


                // finished
                postData.StatusHistory.Add(new StatusHistory()
                {
                    Status = "finished",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                });
                postData.Status = "finished"; //Change status

            }

            // arrived
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).ToString(DateFormatLong))
                }
            });

            if (encounterType == "PNC")
            {
                postData.Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                };

                postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
                {
                    Reference = string.Format("EpisodeOfCare/{0}", episodeofCareId)
                };

                var coding = new List<Coding>() {
                    new Coding() {
                        System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                        Code = "other-hcf",
                        Display = "Other healthcare facility"
                    }
                };
                var dischargeDisposition = new DischargeDisposition()
                {
                    Coding = coding,
                    Text = "Anjuran dokter untuk pulang dan kontrol kembali"
                };
                var hospitalization = new Hospitalization()
                {
                    DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
                };
                postData.Hospitalization = hospitalization;
            }
            else if (encounterType == "TB")
            {
                postData.Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong))
                };

                postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
                {
                    Reference = string.Format("EpisodeOfCare/{0}", episodeofCareId)
                };

                var coding = new List<Coding>() {
                    new Coding() {
                        System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                        Code = "home",
                        Display = "Home"
                    }
                };
                var dischargeDisposition = new DischargeDisposition()
                {
                    Coding = coding
                };
                var hospitalization = new Hospitalization()
                {
                    DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
                };
                postData.Hospitalization = hospitalization;
            }
            else if (encounterType == "INC")
            {
                postData.Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong))
                };

                postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
                {
                    Reference = string.Format("EpisodeOfCare/{0}", episodeofCareId)
                };

                var coding = new List<Coding>() {
                    new Coding() {
                        System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                        Code = "home",
                        Display = "Home"
                    }
                };
                var dischargeDisposition = new DischargeDisposition()
                {
                    Coding = coding,
                    Text = "Anjuran dokter untuk pulang dan kontrol kembali"
                };
                var hospitalization = new Hospitalization()
                {
                    DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
                };
                postData.Hospitalization = hospitalization;
            }
            else
            {
                // Period
                postData.Period = new Period() { Start = FormatDateLong(arrivedTime) }; //"2022-06-14T07:00:00+07:00"
            }

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            if (encounterType == "PNC")
            {
                postData.Identifier = new List<Identifier>()
                {
                    new Identifier() {
                        System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = OrganizationID
                    },
                    new Identifier() {
                        System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/puerperium", Value = "KF3"
                    }
                };
            }
            else if (encounterType == "TB")
            {
                postData.Identifier = new List<Identifier>()
                {
                    new Identifier() {
                        System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                        Value = reg.RegistrationNo
                    },
                    new Identifier() {
                        Use = "temp",
                        System = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}",OrganizationID),
                        Value = reg.RegistrationNo
                    }
                };
            }
            else
            {
                // No kunjungan / registrasi internal
                postData.Identifier = new List<Identifier>()
                {
                    new Identifier() { System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo }
                };
            }
            return postData;
        }

        #endregion
        #endregion 02. Pendaftaran Kunjungan Rawat Jalan

        #region 03. Anamnesis
        // 03.1. Keluhan Utama
        private void PostPatientChiefComplaint(Registration reg, PatientAssessment pa, PatientBridging patSs, string encounterId, ref string accessToken)
        {
            if (string.IsNullOrWhiteSpace(pa.SCTChiefComplaint)) return;

            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Condition", "ChiefComplaint", "");
            if (ssResult != null && ssResult.ResultID != null) return;

            var snomedct = new Snomedct();
            if (!snomedct.LoadByPrimaryKey("ChiefComplaint", pa.SCTChiefComplaint)) return;

            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>() { new
                    {
                        system= "http://terminology.hl7.org/CodeSystem/condition-clinical",
                        code= "active",
                        display= "Active"
                    }
                }
                },
                category = new List<object>() { new
                {
                    coding= new List<object>() { new
                            {
                                system= "http://terminology.hl7.org/CodeSystem/condition-category",
                                code= "problem - list - item",
                                display= "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                    {
                    system= "http://snomed.info/sct",
                        code= pa.SCTChiefComplaint,
                                display= snomedct.Display
                            }
                       }
                },
                onsetString = pa.Hpi, // "Ditemukan sejak 1 bulan yang lalu saat musim kemarau",
                recordedDate = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-06-14T08:45:00 + 07:00",
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId),
                    display = string.Format("Kunjungan {0} di hari {1}", patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()])
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "ChiefComplaint",
                    Code = ""
                };
            }
            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Condition", requestBody, ssResult, ref accessToken);
        }

        // 03.2. Riwayat Alergi
        private void PostPatientAllergy(Registration reg, PatientAllergy patal, PatientBridging patSs, ParamedicBridging parSs, string encounterId, ref string accessToken)
        {
            string clinicalStatus = string.IsNullOrEmpty(patal.SRAllergyClinicalStatus?.ToString()) ? "Active" : patal.SRAllergyClinicalStatus.ToString();
            string verificationStatus = string.IsNullOrEmpty(patal.SRAllergyVerificationStatus?.ToString()) ? "Confirmed" : patal.SRAllergyVerificationStatus.ToString();

            if (string.IsNullOrWhiteSpace(patal.Allergen)) return;

            //Check status kirim untuk Alergi nempel ke PatientID
            var ssResultCheck = new SatuSehatResult();
            ssResultCheck.Query.Where(
                ssResultCheck.Query.ResourceType == "AllergyIntolerance",
                ssResultCheck.Query.Category == string.Format("Med-{0}", reg.PatientID),
                ssResultCheck.Query.Code == patal.Allergen
            );

            Guid resultIdGuid;
            if (!string.IsNullOrWhiteSpace(ssResultCheck.Query.ResultID?.ToString()) &&
                Guid.TryParse(ssResultCheck.Query.ResultID.ToString(), out resultIdGuid))
            {
                ssResultCheck.Query.Where(ssResultCheck.Query.ResultID == resultIdGuid);
            }

            ssResultCheck.Query.es.Top = 1;

            if (ssResultCheck.Query.Load()) return;

            var postData = new
            {
                resourceType = "AllergyIntolerance",
                identifier = new List<object>
                {
                new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/allergy/{0}", OrganizationID),
                        use = "official",
                        value = OrganizationID
                    }
                },
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/allergyintolerance-clinical",
                            code = clinicalStatus.ToLower(),
                            display = char.ToUpper(clinicalStatus.ToLower()[0]) + clinicalStatus.ToLower().Substring(1),
                        }
                    }
                },
                verificationStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/allergyintolerance-verification",
                            code = verificationStatus.ToLower(),
                            display = char.ToUpper(verificationStatus.ToLower()[0]) + verificationStatus.ToLower().Substring(1),
                        }
                    }
                },
                category = new List<string> { "medication" },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://sys-ids.kemkes.go.id/kfa",
                            code = patal.Allergen,
                            display = patal.AllergenName
                        }
                    },
                    text = patal.DescAndReaction
                },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId),
                    display = string.Format("Kunjungan {0}", patSs.BridgingName)
                },
                recordedDate = string.Format("{0}+00:00", patal.AllergenDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recorder = new
                {
                    reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                    display = parSs.BridgingName
                }
            };

            var ssResult = new SatuSehatResult
            {
                EncounterID = new Guid(encounterId),
                Category = string.Format("Med-{0}", reg.PatientID),
                Code = patal.Allergen
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("AllergyIntolerance", requestBody, ssResult, ref accessToken);
        }

        #region 03.4. Riwayat Pengobatan 
        // 03.4.1 Obat bukan dari Fasyankes Sendiri
        private void PostMedicationStatement(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterId, ref string accessToken)
        {
            var tpiq = new MedicationReceiveFromPatientQuery("tpi");
            var tpq = new MedicationReceiveQuery("tp");
            tpiq.InnerJoin(tpq).On(tpiq.MedicationReceiveNo == tpq.MedicationReceiveNo);
            tpiq.Where(tpq.RegistrationNo == reg.RegistrationNo);

            tpiq.Select(tpq.MedicationReceiveNo, tpq.ItemID, tpq.ReceiveDateTime);

            var dtbTpi = tpiq.LoadDataTable();

            //Medication Create
            foreach (DataRow row in dtbTpi.Rows)
            {
                var itemID = row["ItemID"].ToString();
                if (string.IsNullOrEmpty(itemID)) continue;

                var ssItem = new ItemBridging();
                ssItem.Query.Where(ssItem.Query.ItemID == itemID, ssItem.Query.SRBridgingType == SatuSehatBridgingType);
                ssItem.Query.es.Top = 1;
                if (!ssItem.Query.Load()) continue;

                var kfaItem = new SatuSehatKfa();
                kfaItem.Query.Where(kfaItem.Query.SsUuid == ssItem.BridgingID);
                kfaItem.Query.es.Top = 1;
                if (!kfaItem.Query.Load()) continue;

                var kfaInfo = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root>(kfaItem.SsResult);

                //Check status kirim
                var ssResult = LoadSatuSehatResult(encounterId, "Medication", "MedicationStatement", row["MedicationReceiveNo"].ToString());
                var medicationResultID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;

                // 1. Medication For MedicationStatement
                if (string.IsNullOrWhiteSpace(medicationResultID))
                {
                    var postData = MedicationForMedicationStatementPostData(reg, row["MedicationReceiveNo"].ToString(), kfaInfo, ssItem, encounterId);
                    if (postData != null)
                    {
                        var requestBody = JsonConvert.SerializeObject(postData);
                        if (ssResult == null)
                        {
                            ssResult = new SatuSehatResult()
                            {
                                EncounterID = new Guid(encounterId),
                                Category = "MedicationStatement",
                                Code = row["MedicationReceiveNo"].ToString()
                            };
                        }
                        var medRespon = RestClientPostAndSaveLog("Medication", requestBody, ssResult, ref accessToken);

                        if (medRespon != null)
                            medicationResultID = medRespon.Id;
                    }
                }

                // 2. MedicationStatement
                if (!string.IsNullOrEmpty(medicationResultID))
                {
                    ssResult = LoadSatuSehatResult(encounterId, "MedicationStatement", "MedicationStatement", row["MedicationReceiveNo"].ToString());

                    if (ssResult == null || ssResult.ResultID == null)
                    {
                        var tpi = new MedicationReceive();
                        tpi.LoadByPrimaryKey(row["MedicationReceiveNo"].ToInt());
                        var postRequestData = MedicationStatementPostData(reg, patSs, parSs, row["MedicationReceiveNo"].ToString(), Convert.ToDateTime(row["ReceiveDateTime"]), ssItem, tpi, medicationResultID, encounterId);
                        if (postRequestData != null)
                        {
                            var requestBody = JsonConvert.SerializeObject(postRequestData);
                            if (ssResult == null)
                            {
                                ssResult = new SatuSehatResult()
                                {
                                    EncounterID = new Guid(encounterId),
                                    Category = "MedicationStatement",
                                    Code = row["MedicationReceiveNo"].ToString()
                                };
                            }
                            var medReqRes = RestClientPostAndSaveLog("MedicationStatement", requestBody, ssResult, ref accessToken);
                        }
                    }
                }
            }
        }

        private object MedicationForMedicationStatementPostData(Registration reg, string medRecNo, Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root kfaInfo, ItemBridging ssItem, string encounterId)
        {
            // Dokumentasi: https://satusehat.kemkes.go.id/platform/docs/id/fhir/resources/medication-statement/

            var postData = new
            {
                resourceType = "Medication",
                meta = new
                {
                    profile = new List<string>() { "https://fhir.kemkes.go.id/r4/StructureDefinition/Medication" }
                },
                identifier = new List<object>() {
                   new {
                       system= string.Format("http://sys-ids.kemkes.go.id/medication/{0}",OrganizationID),
                       use= "official",
                       value= string.Format("{0}",medRecNo)
                   }
                },
                code = new
                {
                    coding = new List<object>() {
                           new
                           {
                               system= "http://sys-ids.kemkes.go.id/kfa",
                               code= ssItem.BridgingID,
                               display= ssItem.BridgingName
                           }
                        }
                },
                status = "active",
                form = new
                {
                    coding = new List<object>() {
               new
               {
                   system= "http://terminology.kemkes.go.id/CodeSystem/medication-form",
                   code= kfaInfo.Data.DosageForm.Code,
                   display= kfaInfo.Data.DosageForm.Name
               }
           }
                },
                extension = new List<object>() {
           new
           {
               url= "https://fhir.kemkes.go.id/r4/StructureDefinition/MedicationType",
               valueCodeableConcept= new {
                   coding= new List<object>() {
                       new
                       {
                           system = "http://terminology.kemkes.go.id/CodeSystem/medication-type",
                           code= "NC",
                           display= "Non - compound"
                       }
           }
               }
           }
       }
            };


            return postData;
        }

        private object MedicationStatementPostData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string medRecNo, DateTime medRecDate, ItemBridging ssItem, MedicationReceive tpi, string medicationReference, string encounterId)
        {
            var cm = new ConsumeMethod();
            cm.LoadByPrimaryKey(tpi.SRConsumeMethod);

            var postData = new
            {
                resourceType = "MedicationStatement",
                status = "completed",
                category =
                    new
                    {
                        coding = new List<object>() {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/medication-statement-category",
                                code = "outpatient",
                                display = "Outpatient"
                            }
                        }
                    }
                ,
                medicationReference = new
                {
                    reference = string.Format("Medication/{0}", medicationReference),
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                dosage = new List<object> {
                        new {
                            text= cm.SRConsumeMethodName,
                            timing= new {
                                repeat= new {
                                    frequency= cm.IterationQty,
                                    period= 1,
                                    periodUnit= "d"
                                }
                            }
                        }
                },
                effectiveDateTime = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                dateAsserted = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                informationSource = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                context = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                }
            };



            return postData;
        }

        #endregion Riwayat Pengobatan - Obat bukan dari Fasyankes Sendiri
        #endregion 03. Anamnesis



        #region 04. Hasil Pemeriksaan Fisik
        #region 04.1. Pemeriksaan Tanda Tanda Vital
        private void PostObservation(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId, string accessToken, VitalSign.VitalSignEnum vitalSignEnum, string encounterType = "defaultSs")
        {
            var vitalSignCode = string.Empty;
            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.BodyWeight:
                    vitalSignCode = "BW";
                    break;
                case VitalSign.VitalSignEnum.BodyHeight:
                    vitalSignCode = "BH";
                    break;
                case VitalSign.VitalSignEnum.BodyMassIndex:
                    vitalSignCode = "BMI";
                    break;
                //case VitalSign.VitalSignEnum.BirthWeightMeasured:
                //    vitalSignCode = "BWM";
                //    break;
                case VitalSign.VitalSignEnum.BloodPressure:
                    break;
                case VitalSign.VitalSignEnum.BloodPressureSistolic:
                    vitalSignCode = "BPS";
                    break;
                case VitalSign.VitalSignEnum.BloodPressureDiastolic:
                    vitalSignCode = "BPD";
                    break;
                case VitalSign.VitalSignEnum.HeartRate:
                    vitalSignCode = "HR";
                    break;
                case VitalSign.VitalSignEnum.RespiratoryRate:
                    vitalSignCode = "Resp";
                    break;
                case VitalSign.VitalSignEnum.Temperature:
                    vitalSignCode = "Temp";
                    break;
                case VitalSign.VitalSignEnum.PainScale:
                    vitalSignCode = "PS";
                    break;
                case VitalSign.VitalSignEnum.SpO2:
                    vitalSignCode = "SPO2";
                    break;
                default:
                    break;
            }

            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Observation", "vital-signs", vitalSignCode);
            if (ssResult != null && ssResult.ResultID != null) return;

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult();
                ssResult.EncounterID = new Guid(encounterId);
                ssResult.ResourceType = "Observation";
                ssResult.Category = "vital-signs";
                ssResult.Code = vitalSignCode;
            }

            string errorMessage = string.Empty;
            var observationPostData = ObservationPostData(reg, patSs, parMedicSs, vitalSignEnum, encounterId, ref errorMessage, encounterType);

            if (!string.IsNullOrEmpty(errorMessage) && errorMessage.Equals("zero_value"))
                return; // skip

            if (observationPostData == null)
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = errorMessage;
                ssResult.Save();
                return;
            }

            var requestBody = JsonConvert.SerializeObject(observationPostData);
            RestClientPostAndSaveLog(observationPostData.ResourceType, requestBody, ssResult, ref accessToken);
        }

        private ObservationPost ObservationPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, VitalSign.VitalSignEnum vitalSignEnum, string encounterId, ref string errorMessage, string encounterType)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, vitalSignEnum, DateTime.Now);
            if (vitalSign.Value == 0 && encounterType == "defaultSs")
            {
                errorMessage = "zero_value";
                return null;
            }
            double BMI = 0;
            if (vitalSign.Value == 0 && vitalSignEnum == VitalSign.VitalSignEnum.BodyMassIndex && encounterType == "TB")
            {
                var vitalSignHeight = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BodyHeight, DateTime.Now);
                var vitalSignWeight = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BodyWeight, DateTime.Now);
                int Height = vitalSignHeight.Value.ToInt();
                int Weight = vitalSignWeight.Value.ToInt();
                double heightInMeters = Height / 100.0;
                BMI = Weight / (heightInMeters * heightInMeters);
                BMI = Math.Round(BMI, 2);
            }

            string vitalSignCode = String.Empty;
            string vitalSignDisplay = String.Empty;
            var valueQuantity = new ValueQuantity();
            var vitalSignDateTime = vitalSign.RecordDateTime;
            List<Interpretation> interpretation = null;


            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.BodyWeight:
                    {
                        vitalSignCode = "29463-7";
                        vitalSignDisplay = "Body weight";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "kg", System = "http://unitsofmeasure.org", Code = "kg" };

                        break;
                    }
                case VitalSign.VitalSignEnum.BodyHeight:
                    {
                        vitalSignCode = "8302-2";
                        vitalSignDisplay = "Body height";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "cm", System = "http://unitsofmeasure.org", Code = "cm" };

                        break;
                    }
                case VitalSign.VitalSignEnum.BodyMassIndex:
                    {
                        vitalSignCode = "39156-5";
                        vitalSignDisplay = "Body mass index (BMI) [Ratio]";
                        double value;
                        if (vitalSign.Value == 0)
                            value = BMI;
                        else
                            value = vitalSign.Value;

                        valueQuantity = new ValueQuantity() { Value = Math.Round(value, 2), Unit = "kg/m2", System = "http://unitsofmeasure.org", Code = "kg/m2" };

                        //interpretation
                        if (value < 17)
                            interpretation = Wasting();
                        else if (value >= 17 && value < 18.5)
                            interpretation = Underweight();
                        else if (value >= 18.5 && value <= 25)
                            interpretation = Underweight();
                        else if (value >= 25.1 && value <= 27)
                            interpretation = Overweight();
                        else if (value >= 27.1)
                            interpretation = Obese();
                        break;
                    }
                case VitalSign.VitalSignEnum.BloodPressure:
                    break;
                case VitalSign.VitalSignEnum.BloodPressureSistolic:
                    {
                        vitalSignCode = "8480-6";
                        vitalSignDisplay = "Systolic blood pressure";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "mm[Hg]", System = "http://unitsofmeasure.org", Code = "mm[Hg]" };

                        if (vitalSign.Value.ToInt() > 199)
                            interpretation = HighObservation();

                        break;
                    }
                case VitalSign.VitalSignEnum.BloodPressureDiastolic:
                    {
                        vitalSignCode = "8462-4";
                        vitalSignDisplay = "Diastolic blood pressure";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "mm[Hg]", System = "http://unitsofmeasure.org", Code = "mm[Hg]" };

                        if (vitalSign.Value.ToInt() > 79)
                        {
                            interpretation = HighObservation();
                        }
                        break;
                    }
                case VitalSign.VitalSignEnum.HeartRate:
                    {
                        vitalSignCode = "8867-4";
                        vitalSignDisplay = "Heart rate";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "beats/minute", System = "http://unitsofmeasure.org", Code = "/min" };
                        break;
                    }
                case VitalSign.VitalSignEnum.RespiratoryRate:
                    {
                        vitalSignCode = "9279-1";
                        vitalSignDisplay = "Respiratory rate";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "breaths/minute", System = "http://unitsofmeasure.org", Code = "/min" };
                        break;
                    }
                case VitalSign.VitalSignEnum.Temperature:
                    {
                        vitalSignCode = "8310-5";
                        vitalSignDisplay = "Body temperature";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToDouble(), Unit = "C", System = "http://unitsofmeasure.org", Code = "Cel" };

                        if (vitalSign.Value.ToDouble() > 37.5)
                            interpretation = HighObservation();
                        else if (vitalSign.Value.ToDouble() > 36.5)
                            interpretation = LowObservation();

                        break;
                    }
                //case VitalSign.VitalSignEnum.BirthWeightMeasured:
                //    {
                //        vitalSignCode = "8339-4";
                //        vitalSignDisplay = "Birth weight Measured";
                //        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToDouble(), Unit = "g", System = "http://unitsofmeasure.org", Code = "g" };

                //        if (vitalSign.Value.ToDouble() >= 4000)
                //            interpretation = HighObservation();
                //        else if (vitalSign.Value.ToDouble() <= 4000 && vitalSign.Value.ToDouble() >= 2500)
                //            interpretation = LowObservation();
                //        else if (vitalSign.Value.ToDouble() <= 2499 && vitalSign.Value.ToDouble() >= 1500)
                //            interpretation = LowObservation();
                //        else if (vitalSign.Value.ToDouble() <= 1499 && vitalSign.Value.ToDouble() >= 1000)
                //            interpretation = LowObservation();
                //        else if (vitalSign.Value.ToDouble() < 1000)
                //            interpretation = LowObservation();

                //        break;
                //    }
                case VitalSign.VitalSignEnum.PainScale:
                    break;
                case VitalSign.VitalSignEnum.SpO2:
                    break;
                default:
                    break;
            }

            var postData = new ObservationPost();
            postData.ResourceType = "Observation";
            postData.Status = "final";
            postData.Category = new List<Category>() { new Category()
            {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code= "vital-signs",
                                Display= "Vital Signs"
                                }
                            }
            }
            };

            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://loinc.org",
                        Code = vitalSignCode,
                        Display = vitalSignDisplay
                    }
             }
            };

            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };


            var performer = LoadPerformerByUserID(vitalSign.ByUserID);
            if (performer == null)
            {
                errorMessage = string.Format("Performer not found, please setting Satusehat bridging ID for User Paramedic [{0}] first", vitalSign.ByUserID);
                return null;
            }

            postData.Performer = new List<RefAndDisplay>(){ new RefAndDisplay()
            {
                Reference = string.Format("Practitioner/{0}", performer.BridgingID),
            }};
            if (encounterType == "TB")
            {
                postData.Encounter = new RefAndDisplay()
                {
                    Reference = String.Format("Encounter/{0}", encounterId)
                };
                postData.Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            }
            else
            {
                postData.Encounter = new RefAndDisplay()
                {
                    Reference = String.Format("Encounter/{0}", encounterId),
                    Display = string.Format("Pemeriksaan Fisik {0} di hari {1}, {2}", patSs.BridgingName, DayNames[vitalSignDateTime.DayOfWeek.ToInt()], vitalSignDateTime.ToString("dd MMM yyyy"))
                };
            }

            // YYYY-MM-DDThh:mm:ss+00:00
            postData.EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            postData.ValueQuantity = valueQuantity;

            if (vitalSignEnum == VitalSign.VitalSignEnum.BloodPressureSistolic || vitalSignEnum == VitalSign.VitalSignEnum.BloodPressureDiastolic)
            {
                postData.BodySite = new Code()
                {
                    Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://snomed.info/sct",
                        Code = "368209003",
                        Display = "Right arm"
                    }
             }
                };


            }


            if (interpretation != null)
                postData.Interpretation = interpretation;

            return postData;

        }

        private List<Interpretation> HighObservation()
        {
            return new List<Interpretation>()
                            {
                                new Interpretation()
                                {
                                     Coding = new List<Coding>() {
                                         new Coding() {System = "http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation",Code= "H",Display= "significantly high"}
                                     },
                                     Text="Di atas nilai referensi"
                                }
                            };
        }
        private List<Interpretation> LowObservation()
        {
            return new List<Interpretation>()
                            {
                                new Interpretation()
                                {
                                     Coding = new List<Coding>() { new Coding() {
                                    System = "http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation",
                          Code= "L",
                          Display= "low"
                                   }
                        },
                                     Text="Di bawah nilai referensi"
                                }
                            };
        }
        private List<Interpretation> Wasting()
        {
            return new List<Interpretation>()
            {
                new Interpretation()
                    {
                       Coding = new List<Coding>()
                          {
                             new Coding() {
                                 System = "http://snomed.info/sct",
                                 Code= "717933005",
                                 Display= "Severe thinness in adulthood"
                             }
                       },
                       Text="Sangat kurus"
                }
            };
        }
        private List<Interpretation> Underweight()
        {
            return new List<Interpretation>()
            {
                new Interpretation()
                    {
                       Coding = new List<Coding>()
                          {
                             new Coding() {
                                 System = "http://snomed.info/sct",
                                 Code= "248342006",
                                 Display= "Underweight"
                             }
                       },
                       Text="kurus"
                }
            };
        }
        private List<Interpretation> NormalWeight()
        {
            return new List<Interpretation>()
            {
                new Interpretation()
                    {
                       Coding = new List<Coding>()
                          {
                             new Coding() {
                                 System = "http://snomed.info/sct",
                                 Code= "43664005",
                                 Display= "Normal weight"
                             }
                       },
                       Text="Normal"
                }
            };
        }
        private List<Interpretation> Overweight()
        {
            return new List<Interpretation>()
            {
                new Interpretation()
                    {
                       Coding = new List<Coding>()
                          {
                             new Coding() {
                                 System = "http://snomed.info/sct",
                                 Code= "238131007",
                                 Display= "Overweight"
                             }
                       },
                       Text="Gemuk (Overweight)"
                }
            };
        }
        private List<Interpretation> Obese()
        {
            return new List<Interpretation>()
            {
                new Interpretation()
                    {
                       Coding = new List<Coding>()
                          {
                             new Coding() {
                                 System = "http://snomed.info/sct",
                                 Code= "414915002",
                                 Display= "Obese"
                             }
                       },
                       Text="Obesitas"
                }
            };
        }
        #endregion

        #region 04.2. Tingkat Kesadaran
        private void PostConsciousnessLevel(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, PatientAssessment pa, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Observation", "vital-signs", "Consciousness");
            if (ssResult != null && ssResult.ResultID != null) return;

            var observationPostData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>() {
        new {
            coding = new List<object>() {
                new {
                    system= "http://terminology.hl7.org/CodeSystem/observation-category",
                    code= "vital-signs",
                    display= "Vital Signs"
                }
            }
        }
    },
                code = new
                {
                    coding = new List<object>() {
            new {
                system= "http://loinc.org",
                code= "67775-7",
                display= "Level of responsiveness"
            }
        }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                //effectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(_gmtDif).ToString(_dateFormat)), //"2023-08-31T01:10:00+00:00",
                //issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(_gmtDif).ToString(_dateFormat)), //"2023-08-31T01:10:00+00:00",
                performer = new List<object>() {
        new
        {
            reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
            display = parMedSs.BridgingName
        }
    },
                valueCodeableConcept = new
                {
                    coding = new List<object>() {

            new {
                system= "http://snomed.info/sct",
                code= "248234008",
                display= "Mentally alert"
            }
}
                }
            };
            if (ssResult == null)
            {
                ssResult = new SatuSehatResult();
                ssResult.EncounterID = new Guid(encounterId);
                ssResult.ResourceType = "Observation";
                ssResult.Category = "vital-signs";
                ssResult.Code = "Consciousness";
            }

            var requestBody = JsonConvert.SerializeObject(observationPostData);
            RestClientPostAndSaveLog("Observation", requestBody, ssResult, ref accessToken);
        }

        #endregion 04.2. Tingkat Kesadaran


        #endregion 04. Hasil Pemeriksaan Fisik

        #region 06. Rencana Rawat Pasien
        #region CarePlan
        private void PostCarePlanRawatPasien(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, PatientAssessment pa, string encounterId, ref string accessToken)
        {
            if (pa.FollowUpPlanType != "IP")
            {
                // Check di reg IP kalau2 dokternya terlewat isi rencana rawat inap
                var regIp = new Registration();
                regIp.Query.Where(regIp.Query.FromRegistrationNo == reg.RegistrationNo, regIp.Query.SRRegistrationType == "IPR");
                regIp.Query.es.Top = 1;
                if (!reg.Query.Load())
                    return;
            }

            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "CarePlan", "Rawat", "736271009");
            if (ssResult != null && ssResult.ResultID != null) return;

            var postData = new
            {
                resourceType = "CarePlan",
                status = "active",
                intent = "plan",
                title = "Rencana Rawat Pasien",
                description = "Rencana Rawat Pasien",
                category = new List<object>()
                { new
                { coding = new List<object>()
                    { new {
                        system = "http://snomed.info/sct",
                        code = "736271009",
                        display = "Outpatient care plan"
                    } }
                }},
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                created = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)), //"2023-08-31T01:20:00+00:00",
                author = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                }

            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "Rawat",
                    Code = "736271009" //Outpatient care plan (http://snomed.info/sct)
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("CarePlan", requestBody, ssResult, ref accessToken);
        }
        #endregion CarePlan
        #endregion 06. Rencana Rawat Pasien

        #region 09. Tindakan/Prosedur Medis
        public void PostProcedure(Registration reg, PatientBridging patSs, string encounterId, ref string accessToken)
        {
            var epProcs = new EpisodeProcedureCollection();
            epProcs.Query.Where(epProcs.Query.RegistrationNo == reg.RegistrationNo, epProcs.Query.IsVoid == false);
            epProcs.LoadAll();

            if (epProcs.Count == 0)
                return;

            foreach (var ep in epProcs)
            {
                //Check status kirim
                var ssResult = LoadSatuSehatResult(encounterId, "Procedure", "ICD9", ep.ProcedureID);
                if (ssResult != null && ssResult.ResultID != null) continue;

                var postData = ProcedurePostData(reg, patSs, ep, encounterId);
                if (postData != null)
                {
                    if (string.IsNullOrWhiteSpace(ep.ProcedureID)) continue;
                    var requestBody = JsonConvert.SerializeObject(postData);

                    if (ssResult == null)
                    {
                        ssResult = new SatuSehatResult()
                        {
                            EncounterID = new Guid(encounterId),
                            Category = "ICD9",
                            Code = ep.ProcedureID
                        };
                    }
                    RestClientPostAndSaveLog(postData.ResourceType, requestBody, ssResult, ref accessToken);
                }
            }
        }

        private ProcedurePost ProcedurePostData(Registration reg, PatientBridging patSs, EpisodeProcedure ep, string encounterId)
        {
            var postData = new ProcedurePost();
            postData.ResourceType = "Procedure";

            postData.Status = "completed";
            postData.Category = new Category()
            {
                Coding = new List<Coding>() { new Coding() {
                                System = "http://snomed.info/sct",
                      Code= "103693007",
                      Display= "Diagnostic procedure"
                               }

                    },
                Text = "Diagnostic procedure"
            };

            postData.Code = new Code
            {
                Coding = new List<Coding>()
                        { new Coding()
                            {
                                System = "http://hl7.org/fhir/sid/icd-9-cm",
                                Code = ep.ProcedureID,
                                Display = ep.ProcedureName
                            }
                        }
            };


            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", encounterId),
                Display = String.Format("Tindakan untuk patient {0} pada hari {1} tanggal {2}", patSs.BridgingName, DayNames[ep.ProcedureDate.Value.DayOfWeek.ToInt()], ep.ProcedureDate.Value.ToString("dd MMM yyyy"))
            };

            // 2023-03-21 00:00:00	09:38 yyyy-MM-ddTHH:mm:ss ->2023-03-21T09:38:00+01:00
            var date = ep.ProcedureDate.Value;
            var times = (ep.ProcedureTime.Contains(":") ? ep.ProcedureTime : "01:01").Split(':');
            var start = new DateTime(date.Year, date.Month, date.Day, times[0].ToInt(), times[1].ToInt(), 0);

            date = ep.ProcedureDate2.Value;
            times = (ep.ProcedureTime.Contains(":") ? ep.ProcedureTime : "01:01").Split(':');
            var end = new DateTime(date.Year, date.Month, date.Day, times[0].ToInt(), times[1].ToInt(), 0);

            postData.PerformedPeriod = new Period()
            {
                Start = string.Format("{0}+00:00", start.AddHours(GmtDif).ToString(DateFormatLong)),  //string.Format("{0}T{1}:00+{2}:00", ep.ProcedureDate.Value.ToString("yyyy-MM-dd"), ep.ProcedureTime, _gmtDif),
                End = string.Format("{0}+00:00", end.AddHours(GmtDif).ToString(DateFormatLong)) //string.Format("{0}T{1}:00+{2}:00", ep.ProcedureDate2.Value.ToString("yyyy-MM-dd"), ep.ProcedureTime2, _gmtDif)
            };

            var pbQr = new ParamedicBridgingQuery("pb");
            pbQr.Where(pbQr.ParamedicID == ep.ParamedicID, pbQr.SRBridgingType == SatuSehatBridgingType);
            pbQr.es.Top = 1;
            var parSsBrid = new ParamedicBridging();
            if (parSsBrid.Load((pbQr)))
            {
                postData.Performer = new List<Performer>() { new Performer() {
                        Actor = new Actor() {
                            Reference = string.Format("Practitioner/{0}",parSsBrid.BridgingID),
                        Display= parSsBrid.BridgingName
                        }
                    }
            };
            }

            //postData.ReasonCode = new List<Code>()
            //    {
            //            new Code() {
            //            Coding = new List<Coding>()
            //                { new Coding(){
            //                System= "http://hl7.org/fhir/sid/icd-10",
            //                Code= "A15.0",
            //                Display= "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
            //                }
            //        }
            //    }
            //};
            //postData.BodySite = new List<BodySite>()
            //    { new BodySite() {
            //            Coding= new List<Coding>()
            //                { new Coding() {
            //                System= "http://snomed.info/sct",
            //                Code= "302551006",
            //                Display= "Entire Thorax"
            //                }
            //        }
            //    }
            //};

            //postData.Note = new List<Note>()
            //    { new Note() {
            //            Text = "Rontgen thorax melihat perluasan infiltrat dan kavitas."
            //    }
            //};

            return postData;
        }

        #endregion 09. Tindakan/Prosedur Medis

        #region 10. Diagnosis
        public DataTable PostDiagnosis(Registration reg, PatientBridging patSs, string encounterId, ref string accessToken)
        {
            var epDiags = new EpisodeDiagnoseCollection();
            epDiags.Query.Where(epDiags.Query.RegistrationNo == reg.RegistrationNo, epDiags.Query.IsVoid == false);
            //epDiags.Query.es.Top = 2;
            epDiags.LoadAll();

            var i = 0;
            foreach (var diag in epDiags)
            {
                if (string.IsNullOrWhiteSpace(diag.DiagnoseID)) continue;

                //Check status kirim
                var ssResult = LoadSatuSehatResult(encounterId, "Condition", "Diagnosis", diag.DiagnoseID);
                if (ssResult != null && ssResult.ResultID != null) continue;

                //Process
                var postData = ConditionPostData(reg, patSs, diag, encounterId);
                if (postData == null)
                {
                    var ssResultFail = new SatuSehatResult()
                    {
                        EncounterID = new Guid(encounterId),
                        Category = "Diagnosis",
                        Code = diag.DiagnoseID,
                        ErrorResponse = string.Format("ICD-10: {0} tidak terdaftar di Satusehat", diag.DiagnoseID),
                        ResourceType = "Condition"
                    };
                    SetResultIndexNo(ssResultFail);
                    ssResultFail.Save();
                    continue;
                }

                var requestBody = JsonConvert.SerializeObject(postData);

                if (ssResult == null)
                {
                    ssResult = new SatuSehatResult()
                    {
                        EncounterID = new Guid(encounterId),
                        Category = "Diagnosis",
                        Code = diag.DiagnoseID
                    };
                }
                RestClientPostAndSaveLog(postData.ResourceType, requestBody, ssResult, ref accessToken);
            }

            // Diagnosis result
            var ssres = new SatuSehatResultQuery("r");
            ssres.Where(ssres.EncounterID == new Guid(encounterId), ssres.ResourceType == "Condition", ssres.Category == "Diagnosis");
            ssres.Select(ssres.IndexNo, ssres.ResultID, ssres.Code, ssres.PostData);
            return ssres.LoadDataTable();
        }

        private ConditionPost ConditionPostData(Registration reg, PatientBridging patSs, EpisodeDiagnose epDiagnose, string encounterId, string EncounterType = "defaultSs")
        {
            var diagID = epDiagnose.DiagnoseID;
            var diagText = string.Empty;

            // Check exist in SatuSehat ICDX
            var diag = new Diagnose();
            if (diag.LoadByPrimaryKey(diagID))
            {
                if (diag.IsSatuSehat == null || false.Equals(diag.IsSatuSehat)) // Tidak terdaftar di satusehat
                {
                    // Naikan level
                    // Sample:
                    // A09.9+ -> A09.9
                    // A18.0    + ->  A18.0

                    var i = 1;
                    while (true)
                    {
                        if (diagID.Length == 0) break;

                        diagID = diagID.Substring(0, diagID.Length - 1); // Naikan level
                        diag.QueryReset();
                        if (diag.LoadByPrimaryKey(diagID) && true.Equals(diag.IsSatuSehat))
                        {
                            diagID = diag.DiagnoseID;
                            diagText = diag.DiagnoseName;
                            break;
                        }

                        i++;
                    }
                }
                else
                    diagText = diag.DiagnoseName;
            }

            if (string.IsNullOrWhiteSpace(diagText)) return null;

            var postData = new ConditionPost();
            postData.ResourceType = "Condition";
            postData.ClinicalStatus = new ClinicalStatus()
            {
                Coding = new List<Coding>() {
                            new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                   Code= "active",
                   Display= "Active"}
                    }
            };


            postData.Category = new List<Category>() { new Category()
             {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/condition-category",
                      Code= "encounter-diagnosis",
                      Display= "Encounter Diagnosis"
                               }
                    }
             }
            };


            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://hl7.org/fhir/sid/icd-10",
                        Code = diagID,
                        Display = diagText
                    }
             }
            };

            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            if (EncounterType == "INC")
            {
                postData.Encounter = new RefAndDisplay()
                {
                    Reference = String.Format("Encounter/{0}", encounterId)
                };
                postData.Note = new List<Note>()
                    { new Note() {
                         Text = epDiagnose.Notes
                    }
                };
            }
            else
            {
                postData.Encounter = new RefAndDisplay()
                {
                    Reference = String.Format("Encounter/{0}", encounterId),
                    Display = string.Format("Kunjungan {0} di hari {1}", patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()])
                };
            }
            postData.OnsetDateTime = string.Format("{0}+00:00", (epDiagnose.CreateDateTime ?? epDiagnose.LastUpdateDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong));
            postData.RecordedDate = postData.OnsetDateTime;

            return postData;
        }

        #endregion 10. Diagnosis

        #region 11. Diet
        private void PostCompositionDiet(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Composition", "Diet", "");
            if (ssResult != null && ssResult.ResultID != null) return;

            var postData = CompositionPostData(reg, patSs, parMedicSs, encounterId);
            if (postData != null)
            {
                var requestBody = JsonConvert.SerializeObject(postData);

                if (ssResult == null)
                {
                    ssResult = new SatuSehatResult()
                    {
                        EncounterID = new Guid(encounterId),
                        Category = "Diet",
                        Code = String.Empty
                    };
                }
                RestClientPostAndSaveLog(postData.ResourceType, requestBody, ssResult, ref accessToken);
            }
        }

        private CompositionPost CompositionPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            // Diet
            var edu = new PatientEducationLine();
            edu.Query.es.Top = 1;
            edu.Query.Where(edu.Query.RegistrationNo == reg.RegistrationNo, edu.Query.SRPatientEducation == "004"); //PatientEducation	004	Diet dan nutrisi
            if (!edu.Query.Load() || string.IsNullOrWhiteSpace(edu.EducationNotes)) return null;

            var postData = new CompositionPost();
            postData.ResourceType = "Composition";
            postData.Status = "final";

            postData.Type = new Bridging.SatuSehat.BusinessObject.Code
            {
                Coding = new List<Coding>()
                            { new Coding() {
                            System = "http://loinc.org",
                        Code= "18842-5",
                        Display= "Discharge summary"
                            }

            }
            };

            postData.Category = new List<Category>() { new Category() { Coding = new List<Coding>()
                    { new Coding(){ System= "http://loinc.org",
                            Code= "LP173421-1",
                            Display= "Report"} } } };


            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };


            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", encounterId),
                Display = String.Format("Kunjungan patient {0} pada hari {1} tanggal {2}", patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()], reg.RegistrationDate.Value.ToString("dd MMM yyyy"))
            };


            //postData.Date = reg.RegistrationDate.Value.ToString("yyyy-MM-dd");

            var eduDate = edu.LastUpdateDateTime != null ? edu.LastUpdateDateTime : reg.RegistrationDate;
            postData.Date = string.Format("{0}+00:00", eduDate.Value.AddHours(GmtDif).ToString(DateFormatLong));


            postData.Author = new List<Author>
                { new Author(){
                        Reference= String.Format("Practitioner/{0}",parMedicSs.BridgingID),
                    Display= parMedicSs.BridgingName
                }};

            postData.Title = "Resume Medis Rawat Jalan";
            postData.Custodian = new Custodian()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            postData.Section = new List<Section>{
                new Section() {
                        Code = new Code() {
                            Coding= new List<Coding>()
                                { new Coding(){
                                System= "http://loinc.org",
                                Code= "42344-2",
                                Display= "Discharge diet (narrative)"
                                }
                            }
                    }, Text = new Text(){ Status= "additional",Div= edu.EducationNotes} }
            };

            return postData;
        }

        #endregion 11. Diet

        #region #5b. Kesadaran, Keluhan Utama, Edukasi, Kondisi Saat Pulang
        //private void PostKondisiPulang( Registration reg, PatientBridging patSs, string encounterId, ref string accessToken)
        //{
        //    var postData = new
        //    {
        //        resourceType = "Condition",
        //        clinicalStatus = new
        //        {
        //            coding = new List<object>() { new
        //            {
        //                system= "http://terminology.hl7.org/CodeSystem/condition-clinical",
        //                code= "active",
        //                display= "Active"
        //            }
        //        }
        //        },
        //        category = new List<object>() { new
        //        {
        //        coding= new List<object>() { new
        //                    {
        //            system= "http://terminology.hl7.org/CodeSystem/condition-category",
        //            code= "problem - list - item",
        //                        display= "Problem List Item"
        //                    }
        //                }
        //            }
        //        },
        //        code = new
        //        {
        //            coding = new List<object>() { new
        //            {
        //            system= "http://snomed.info/sct",
        //                code= "49727002",
        //                        display= "Cough"
        //                    }
        //                }
        //        },
        //        onsetString = "Ditemukan sejak 1 bulan yang lalu saat musim kemarau",
        //        recordedDate = "2022-06-14T08:45:00 + 07:00",
        //        subject = new
        //        {
        //            reference = string.Format("Patient/{0}", patSs.BridgingID),
        //            display = patSs.BridgingName
        //        },
        //        encounter = new
        //        {
        //            reference = string.Format("Encounter/{0}", encounterId),
        //            display = string.Format("Kunjungan {0} di hari {1}", patSs.BridgingName, _dayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()])

        //        }
        //    };

        //    var ssResult = new SatuSehatResult()
        //    {
        //        EncounterID = new Guid(encounterId),
        //        Category = "ChiefComplaint",
        //        Code = ""
        //    };

        //    var requestBody = JsonConvert.SerializeObject(postData);
        //    RestClientPostAndSaveLog( "Condition", requestBody, ssResult, ref accessToken);
        //}


        #endregion #5b. Kesadaran, Keluhan Utama, Edukasi, Kondisi Saat Pulang



        #region 08. Pemeriksaan Penunjang - Laboratorium
        public void PostServiceRequest(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {
            var serviceUnitLaboratoryID = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryID);
            var serviceUnitLaboratoryIdArray = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryIdArray);

            var query = new TransChargesItemQuery("a");
            var tc = new TransChargesQuery("b");
            query.InnerJoin(tc).On(query.TransactionNo == tc.TransactionNo);
            var item = new ItemQuery("i");
            query.InnerJoin(item).On(query.ItemID == item.ItemID);
            query.Where(tc.RegistrationNo == reg.RegistrationNo, tc.IsOrder == true, tc.IsApproved == true,
                         query.Or(
                                    tc.ToServiceUnitID == serviceUnitLaboratoryID,
                                    tc.ToServiceUnitID.In(serviceUnitLaboratoryIdArray)
                                 ),
                            query.IsOrderRealization == true, query.IsVoid == false, item.SRItemType == ItemType.Laboratory
                        );
            query.Select(query.TransactionNo, query.SequenceNo, query.ItemID, item.ItemName, query.Notes.As("ItemNotes"),
                query.RealizationDateTime, query.SpecimenCollectDateTime, query.SpecimenReceiveDateTime, query.SpecimenCollectByUserID, query.SpecimenReceiveByUserID, query.SRCollectMethod,
                tc.Notes.As("HeaderNotes"), tc.ApprovedDateTime);
            var dtb = query.LoadDataTable();
            foreach (DataRow row in dtb.Rows)
            {
                var itemSs = new ItemBridging();
                itemSs.Query.Where(itemSs.Query.ItemID == row["ItemID"].ToString(), itemSs.Query.SRBridgingType == SatuSehatBridgingType);
                itemSs.Query.es.Top = 1;
                if (itemSs.Query.Load() && !string.IsNullOrWhiteSpace(itemSs.BridgingID))
                {
                    var loincItem = new LoincItem();
                    if (loincItem.LoadByPrimaryKey("LAB", itemSs.BridgingID))
                    {
                        var serviceReqResp = PostServiceRequestItem(reg, patSs, parMedSs, row["TransactionNo"].ToString(), row["SequenceNo"].ToString(), row["ItemName"].ToString(), Convert.ToDateTime(row["ApprovedDateTime"]), row["HeaderNotes"].ToString(), row["ItemNotes"].ToString(), loincItem, encounterId, ref accessToken);

                        // Post Specimen
                        if (serviceReqResp != null && !string.IsNullOrEmpty(serviceReqResp.Id) && row["SpecimenCollectDateTime"] != DBNull.Value && row["SpecimenReceiveDateTime"] != DBNull.Value)
                        {
                            PostSpecimen(patSs, row["TransactionNo"].ToString(), row["SequenceNo"].ToString(), row["ItemID"].ToString(), row["SRCollectMethod"].ToString(), Convert.ToDateTime(row["SpecimenCollectDateTime"]), Convert.ToDateTime(row["SpecimenReceiveDateTime"]), serviceReqResp.Id, encounterId, ref accessToken);
                        }
                    }
                }
            }


            // LogToLisInterop
            if (AppParameter.IsYes(AppParameter.ParameterItem.IsUsingHisInterop))
            {
                var lisConnectionName = AppParameter.GetParameterValue(AppParameter.ParameterItem.HisInteropConfigName);
                if (!string.IsNullOrEmpty(lisConnectionName))
                {
                    var transactionNos = dtb.AsEnumerable()
                                        .Select(row => row.Field<string>("TransactionNo"))
                                        .Distinct()
                                        .ToList();

                    foreach (string transactionNo in transactionNos)
                    {
                        // Log ulang
                        LogToLisInterop(reg, transactionNo, lisConnectionName);
                    }
                }
            }

        }
        private BaseResponse PostServiceRequestItem(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string itemName, DateTime approvedDateTime, string headerNotes, string itemNotes, LoincItem loincItem, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>() {
                    new {
                        system= string.Format( "http://sys-ids.kemkes.go.id/servicerequest/{0}",OrganizationID),
                        value= string.Format("{0}-{1}", transactionNo, sequenceNo) //"00001"
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>() {
                    new {
                        coding= new List<object>() {
                            new {
                                system= "http://snomed.info/sct",
                                code= "108252007",
                                display= "Laboratory procedure"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                        system= "http://loinc.org",
                        code= loincItem.Code, // "11477 - 7",
                        display= loincItem.Display // "Microscopic observation[Identifier} in Sputum by Acid fast stain"
                        }
                    },
                    text = itemNotes// "Pemeriksaan Sputum BTA"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId),
                    display = string.Format("Permintaan {0} {1} di hari {2} pukul {3}", itemName, patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()], "09:30 WIB")
                },
                occurrenceDateTime = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), // "2022-06-14T09:30:27+07:00",
                authoredOn = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-06-13T12:30:27+07:00",
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                performer = new List<object>() {
                    new {
                        reference= string.Format("Practitioner/{0}", parMedSs.BridgingID),
                        display= parMedSs.BridgingName
                    }
                },
                reasonCode = new List<object>() {
                    new {
                        text= headerNotes //"Periksa Keseimbangan Elektrolit"
                    }
                }
            };
            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            return RestClientPostAndSaveLog("ServiceRequest", requestBody, ssResult, ref accessToken);
        }

        private void PostSpecimen(PatientBridging patSs, string transactionNo, string sequenceNo, string itemID, string collectMethod, DateTime collectDateTime, DateTime receiveDateTime, string serviceReqID, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Specimen", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo,
                    ResourceType = "Specimen"
                };
            }

            var itemLab = new ItemLaboratory();
            itemLab.LoadByPrimaryKey(itemID);

            var specimenType = new AppStandardReferenceItemBridging();
            if (!specimenType.LoadByPrimaryKey("SpecimenType", itemLab.SRSpecimenType, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging SpecimenType [{0}] not found", itemLab.SRSpecimenType);
                ssResult.Save();
                return;
            }

            var cm = new AppStandardReferenceItemBridging();
            if (!cm.LoadByPrimaryKey("CollectMethod", collectMethod, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging CollectMethod [{0}] not found", collectMethod);
                ssResult.Save();
                return;
            }

            var snomed = new Snomedct();
            snomed.LoadByPrimaryKey("SpecimenType", specimenType.BridgingID);

            var postData = new
            {
                resourceType = "Specimen",
                identifier = new List<object>() { new
                {
                    system =  string.Format("http://sys-ids.kemkes.go.id/specimen/{0}",OrganizationID),
                    value= string.Format("{0}-{1}", transactionNo, sequenceNo),
                    assigner = new {
                        reference =  string.Format("Organization/{0}",OrganizationID)
                    }
                }
            },
                status = "available",
                type = new
                {
                    coding = new List<object>() { new
                    {
                        system =  "http://snomed.info/sct",
                        code = specimenType.BridgingID, // "119297000",
                        display =  snomed.Display // "Blood specimen (specimen)"
                    }
                }
                },
                collection = new
                {
                    method = new
                    {
                        coding = new List<object>() { new
                        {
                            system = "http://snomed.info/sct",
                            code = cm.BridgingID, //"82078001",
                            display = cm.BridgingName //"Collection of blood specimen for laboratory (procedure)"
                        }
                    }
                    },
                    collectedDateTime = string.Format("{0}+00:00", collectDateTime.AddHours(GmtDif).ToString(DateFormatLong)) //"2023 - 08 - 31T15: 15:00 + 00:00"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                request = new List<object>() { new
                    {
                reference = string.Format("ServiceRequest/{0}",serviceReqID)
                    }
                },
                receivedTime = string.Format("{0}+00:00", receiveDateTime.AddHours(GmtDif).ToString(DateFormatLong)) //"2023-08 - 31T15: 25:00 + 00:00"
            };



            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Specimen", requestBody, ssResult, ref accessToken);
        }

        ////Observation
        //private void PostObservationLab(PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string specimenID, string serviceReqID, string loincCode, string loincDisplay, DateTime observationDateTime, string valueCode, string valueDisplay, string encounterId, ref string accessToken)
        //{
        //    //Check status kirim
        //    var ssResult = LoadSatuSehatResult(encounterId, "Observation", transactionNo, sequenceNo);
        //    if (ssResult != null && ssResult.ResultID != null) return;

        //    if (ssResult == null)
        //    {
        //        ssResult = new SatuSehatResult()
        //        {
        //            EncounterID = new Guid(encounterId),
        //            Category = transactionNo,
        //            Code = sequenceNo
        //        };
        //    }

        //    var postData = new
        //    {
        //        resourceType = "Observation",
        //        identifier = new List<object> {
        //        new {
        //            system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", _organizationID),
        //            value = string.Format("{0}-{1}", transactionNo, sequenceNo)
        //        }
        //    },
        //        status = "final",
        //        category = new List<object> {
        //        new {
        //            coding = new List<object> {
        //                new {
        //                    system = "http://terminology.hl7.org/CodeSystem/observation-category",
        //                    code = "laboratory",
        //                    display = "Laboratory"
        //                }
        //            }
        //        }
        //    },
        //        code = new
        //        {
        //            coding = new List<object> {
        //                new {
        //                    system = "http://loinc.org",
        //                    code = loincCode,
        //                    display = loincDisplay
        //                }
        //            }
        //        },
        //        subject = new
        //        {
        //            reference = string.Format("Patient/{0}", patSs.BridgingID)
        //        },
        //        encounter = new
        //        {
        //            reference = string.Format("Encounter/{0}", encounterId)
        //        },
        //        effectiveDateTime = string.Format("{0}+00:00", observationDateTime.AddHours(_gmtDif).ToString(_dateFormat)),
        //        issued = string.Format("{0}+00:00", observationDateTime.AddHours(_gmtDif).ToString(_dateFormat)),
        //        performer = new List<object> {
        //        new {
        //            reference = string.Format("Practitioner/{0}", parMedSs.BridgingID)
        //        },
        //            new {
        //                reference = string.Format("Organization/{0}", _organizationID)
        //            }
        //        },
        //        specimen = new
        //        {
        //            reference = string.Format("Specimen/{0}", specimenID)
        //        },
        //        basedOn = new List<object> {
        //            new {
        //                reference = string.Format("ServiceRequest/{0}", serviceReqID)
        //            }
        //        },
        //        valueCodeableConcept = new
        //        {
        //            coding = new List<object> {
        //                new {
        //                    system = "http://loinc.org",
        //                    code = valueCode,
        //                    display = valueDisplay
        //                }
        //            }
        //        }
        //    };

        //    var requestBody = JsonConvert.SerializeObject(postData);
        //    RestClientPostAndSaveLog("Observation", requestBody, ssResult, ref accessToken);
        //}


        //Diagnostic Report

        #endregion Lab

        #region Pemeriksaan Penunjang Lab Offline

        public void PostServiceRequestLabOff(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {
            var serviceUnitLaboratoryID = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryID);
            var serviceUnitLaboratoryIdArray = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryIdArray);

            // PARENT
            var parentQuery = new TransChargesItemQuery("a");
            var tc = new TransChargesQuery("b");
            var item = new ItemQuery("i");
            var itemLab = new ItemLaboratoryQuery("il");

            parentQuery.InnerJoin(tc).On(parentQuery.TransactionNo == tc.TransactionNo);
            parentQuery.InnerJoin(item).On(parentQuery.ItemID == item.ItemID);
            parentQuery.LeftJoin(itemLab).On(parentQuery.ItemID == itemLab.ItemID);

            parentQuery.Where(
                tc.RegistrationNo == reg.RegistrationNo,
                tc.IsOrder == true,
                tc.IsApproved == true,
                parentQuery.Or(
                    tc.ToServiceUnitID == serviceUnitLaboratoryID,
                    tc.ToServiceUnitID.In(serviceUnitLaboratoryIdArray)
                ),
                parentQuery.IsOrderRealization == true,
                parentQuery.IsVoid == false
            );

            parentQuery.Select(
                parentQuery.TransactionNo,
                parentQuery.SequenceNo.As("ParentSequenceNo"),
                parentQuery.SequenceNo.As("SequenceNo"),
                parentQuery.ItemID,
                item.ItemName,
                parentQuery.Notes.As("ItemNotes"),
                parentQuery.RealizationDateTime,
                parentQuery.SpecimenCollectDateTime,
                parentQuery.SpecimenReceiveDateTime,
                parentQuery.SpecimenCollectByUserID,
                parentQuery.SpecimenReceiveByUserID,
                tc.Notes.As("HeaderNotes"),
                tc.ApprovedDateTime,
                parentQuery.ResultValue,
                itemLab.SRLaboratoryUnit,
                parentQuery.SRCollectMethod,
                parentQuery.LastUpdateDateTime,
                itemLab.SRResultValueType.As("SRResultValueType") //result value type
            );

            var dtParent = parentQuery.LoadDataTable();

            if (!dtParent.Columns.Contains("ParentItemID")) dtParent.Columns.Add("ParentItemID", typeof(string));
            if (!dtParent.Columns.Contains("DetailItemID")) dtParent.Columns.Add("DetailItemID", typeof(string));
            if (!dtParent.Columns.Contains("Level")) dtParent.Columns.Add("Level", typeof(string));

            foreach (DataRow r in dtParent.Rows)
            {
                r["ParentItemID"] = DBNull.Value;
                r["DetailItemID"] = DBNull.Value;
                r["Level"] = "Parent";
            }

            // CHILD
            var childQuery = new TransChargesItemQuery("a");
            var tc2 = new TransChargesQuery("b");
            var itemLabP = new ItemLaboratoryProfileQuery("ilp");
            var x = new TransChargesItemQuery("x"); // detail paket
            var childItem = new ItemQuery("ic");
            var childLab = new ItemLaboratoryQuery("ilc");

            childQuery.InnerJoin(tc2).On(childQuery.TransactionNo == tc2.TransactionNo);
            childQuery.LeftJoin(itemLabP).On(childQuery.ItemID == itemLabP.ParentItemID);
            childQuery.InnerJoin(x).On(x.ItemID == itemLabP.DetailItemID & x.TransactionNo == tc2.TransactionNo);
            childQuery.InnerJoin(childItem).On(x.ItemID == childItem.ItemID);
            childQuery.LeftJoin(childLab).On(x.ItemID == childLab.ItemID);

            childQuery.Where(
                tc2.RegistrationNo == reg.RegistrationNo,
                tc2.IsOrder == true,
                tc2.IsApproved == true,
                childQuery.Or(
                    tc2.ToServiceUnitID == serviceUnitLaboratoryID,
                    tc2.ToServiceUnitID.In(serviceUnitLaboratoryIdArray)
                ),
                childQuery.IsOrderRealization == true,
                childQuery.IsVoid == false
            );

            childQuery.Select(
                childQuery.TransactionNo,
                childQuery.SequenceNo.As("ParentSequenceNo"), // parent sequence
                x.SequenceNo.As("SequenceNo"),               // child sequence (dipakai untuk observation)
                x.ItemID,
                childItem.ItemName,
                childQuery.RealizationDateTime,
                childQuery.SpecimenCollectDateTime,
                childQuery.SpecimenReceiveDateTime,
                childQuery.SpecimenCollectByUserID,
                childQuery.SpecimenReceiveByUserID,
                childQuery.SRCollectMethod,
                tc2.Notes.As("HeaderNotes"),
                tc2.ApprovedDateTime,
                x.ResultValue.As("ResultValue"),             // hasil dari child
                childLab.SRLaboratoryUnit,
                x.LastUpdateDateTime.As("LastUpdateDateTime"),
                childQuery.ItemID.As("ParentItemID"),        // simpan ID parent
                x.ItemID.As("DetailItemID"),
                childLab.SRResultValueType.As("SRResultValueType")
            );

            var dtChild = childQuery.LoadDataTable();

            if (!dtChild.Columns.Contains("ItemNotes")) dtChild.Columns.Add("ItemNotes", typeof(string));
            if (!dtChild.Columns.Contains("Level")) dtChild.Columns.Add("Level", typeof(string));

            foreach (DataRow r in dtChild.Rows)
            {
                r["ItemNotes"] = DBNull.Value;
                r["Level"] = "Child";
            }

            // Merge
            dtParent.Merge(dtChild);

            if (!dtParent.Columns.Contains("StandarValue"))
                dtParent.Columns.Add("StandarValue", typeof(string));

            var patient = new Patient();
            patient.LoadByPrimaryKey(reg.PatientID);
            var ageInDays = (reg.RegistrationDate - patient.DateOfBirth).Value.TotalDays;

            var transactionGroups = dtParent.AsEnumerable().GroupBy(r => r["TransactionNo"].ToString());

            foreach (var trxGroup in transactionGroups)
            {
                var transactionNo = trxGroup.Key;

                var procedureResp = PostProcedureLabOff(reg, patSs, parMedSs, encounterId, ref accessToken);
                if (procedureResp == null || string.IsNullOrEmpty(procedureResp.Id)) continue;

                // Group per service (single = parent row's SequenceNo, paket = ParentSequenceNo)
                var serviceGroups = trxGroup
                    .GroupBy(r =>
                        r["ParentItemID"] == DBNull.Value
                            ? r["SequenceNo"].ToString() // Single item
                            : r["ParentSequenceNo"].ToString() // Paket
                    );

                foreach (var svcGroup in serviceGroups)
                {
                    bool isPackageGroup = svcGroup.Any(r => r["ParentItemID"] != DBNull.Value);

                    // parent dipakai untuk ServiceRequest/specimen
                    var parentRow = svcGroup.FirstOrDefault(r => r["Level"] != DBNull.Value && r["Level"].ToString() == "Parent");
                    var firstRow = parentRow ?? svcGroup.First();

                    var parentItemId = firstRow["ItemID"].ToString();
                    var loincParent = new LoincItem();
                    if (!loincParent.LoadByPrimaryKey("LAB", GetBridgingID(parentItemId))) continue;

                    // POST SERVICE REQUEST
                    var serviceReqResp = PostServiceRequestItemLabOff(
                        reg, patSs, parMedSs,
                        transactionNo,
                        firstRow["ParentSequenceNo"].ToString(),
                        firstRow["ItemName"].ToString(),
                        Convert.ToDateTime(firstRow["ApprovedDateTime"]),
                        firstRow["HeaderNotes"]?.ToString(),
                        firstRow["ItemNotes"]?.ToString(),
                        loincParent,
                        procedureResp.Id,
                        encounterId,
                        ref accessToken
                    );
                    if (serviceReqResp == null || string.IsNullOrEmpty(serviceReqResp.Id)) continue;
                    var serviceRequestId = serviceReqResp.Id;

                    // POST SPECIMEN
                    string specimenId = null;
                    if (firstRow["SpecimenCollectDateTime"] != DBNull.Value && firstRow["SpecimenReceiveDateTime"] != DBNull.Value)
                    {
                        var specimenResp = PostSpecimenLabOff(
                            patSs, parMedSs,
                            transactionNo,
                            firstRow["ParentSequenceNo"].ToString(),
                            firstRow["ItemID"].ToString(),
                            firstRow["SRCollectMethod"]?.ToString(),
                            Convert.ToDateTime(firstRow["SpecimenCollectDateTime"]),
                            Convert.ToDateTime(firstRow["SpecimenReceiveDateTime"]),
                            serviceRequestId,
                            encounterId,
                            ref accessToken
                        );
                        if (specimenResp != null && !string.IsNullOrEmpty(specimenResp.Id))
                            specimenId = specimenResp.Id;
                    }

                    // OBSERVATION
                    var observationIds = new List<string>();
                    foreach (var row in svcGroup)
                    {
                        // Jika ini adalah paket group, skip parent row (kirim child Observations)
                        if (isPackageGroup && row["Level"] != DBNull.Value && row["Level"].ToString() == "Parent")
                            continue;

                        // SequenceNo untuk child = child.SequenceNo, untuk parent single = parent.SequenceNo
                        string seqNo = row["SequenceNo"].ToString();
                        string targetItemId = row["DetailItemID"] == DBNull.Value ? row["ItemID"].ToString() : row["DetailItemID"].ToString();

                        if (row["ResultValue"] != DBNull.Value)
                            row["StandarValue"] = GetNormalRange(targetItemId, ageInDays, patient.Sex);

                        var itemSs = new ItemBridging();
                        itemSs.Query.Where(itemSs.Query.ItemID == targetItemId, itemSs.Query.SRBridgingType == SatuSehatBridgingType);
                        itemSs.Query.es.Top = 1;
                        if (!itemSs.Query.Load() || string.IsNullOrWhiteSpace(itemSs.BridgingID)) continue;

                        var loincItem = new LoincItem();
                        if (!loincItem.LoadByPrimaryKey("LAB", itemSs.BridgingID)) continue;

                        string srType = null;
                        if (row.Table.Columns.Contains("SRResultValueType") && row["SRResultValueType"] != DBNull.Value)
                            srType = row["SRResultValueType"].ToString();

                        string srTypeName = null;
                        if (!string.IsNullOrWhiteSpace(srType))
                        {
                            var apstrfi = new AppStandardReferenceItem();
                            if (apstrfi.LoadByPrimaryKey("LabResultType", srType))
                            {
                                srTypeName = apstrfi.ItemName?.Trim().ToLowerInvariant();
                            }
                        }

                        // Default variables
                        string valueType = null;
                        decimal? resultValue = null;
                        string resultUnit = null;
                        string codeSystem = null, codeValue = null, codeDisplay = null, narrativeValue = null;

                        if (!string.IsNullOrWhiteSpace(srTypeName))
                        {
                            if (srTypeName.Contains("string") || srTypeName.Contains("text") || srTypeName.Contains("narrative"))
                            {
                                valueType = "string";
                                narrativeValue = row["ResultValue"]?.ToString();
                            }
                            else if (srTypeName.Contains("codeable") || srTypeName.Contains("coded"))
                            {
                                valueType = "codeable";
                                codeSystem = row.Table.Columns.Contains("CodeSystem") ? row["CodeSystem"]?.ToString() : null;
                                codeValue = row.Table.Columns.Contains("CodeValue") ? row["CodeValue"]?.ToString() : null;
                                codeDisplay = row.Table.Columns.Contains("CodeDisplay") ? row["CodeDisplay"]?.ToString() : null;
                                if (string.IsNullOrWhiteSpace(codeValue) && row["ResultValue"] != DBNull.Value)
                                    codeValue = row["ResultValue"].ToString();
                            }
                            else
                            {
                                valueType = "quantity";
                                if (row["ResultValue"] != DBNull.Value && decimal.TryParse(row["ResultValue"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var rv))
                                    resultValue = rv;
                                resultUnit = row.Table.Columns.Contains("SRLaboratoryUnit") ? row["SRLaboratoryUnit"]?.ToString() : null;
                            }
                        }
                        else
                        {
                            if (row["ResultValue"] != DBNull.Value && decimal.TryParse(row["ResultValue"].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var rv2))
                            {
                                valueType = "quantity";
                                resultValue = rv2;
                                resultUnit = row.Table.Columns.Contains("SRLaboratoryUnit") ? row["SRLaboratoryUnit"]?.ToString() : null;
                            }
                            else if (row.Table.Columns.Contains("CodeValue") && row["CodeValue"] != DBNull.Value)
                            {
                                valueType = "codeable";
                                codeSystem = row.Table.Columns.Contains("CodeSystem") ? row["CodeSystem"]?.ToString() : null;
                                codeValue = row["CodeValue"]?.ToString();
                                codeDisplay = row.Table.Columns.Contains("CodeDisplay") ? row["CodeDisplay"]?.ToString() : null;
                            }
                            else if (row.Table.Columns.Contains("Narrative") && row["Narrative"] != DBNull.Value)
                            {
                                valueType = "string";
                                narrativeValue = row["Narrative"].ToString();
                            }
                            else if (row["ResultValue"] != DBNull.Value)
                            {
                                valueType = "string";
                                narrativeValue = row["ResultValue"].ToString();
                            }
                            else
                            {
                                continue;
                            }
                        }

                        // parse min/max reference if ada
                        decimal? min = null, max = null;
                        if (row["StandarValue"] != DBNull.Value && row["StandarValue"].ToString().Contains("-"))
                        {
                            var parts = row["StandarValue"].ToString().Split('-');
                            if (parts.Length == 2 &&
                                decimal.TryParse(parts[0].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tMin) &&
                                decimal.TryParse(parts[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out var tMax))
                            {
                                min = tMin; max = tMax;
                            }
                        }

                        if (valueType == "quantity" && string.IsNullOrWhiteSpace(resultUnit) && row.Table.Columns.Contains("SRLaboratoryUnit"))
                            resultUnit = row["SRLaboratoryUnit"]?.ToString() ?? string.Empty;

                        // POST OBSERVATION
                        var obsResp = PostObservationLabOff(
                            patSs,
                            parMedSs,
                            loincItem,
                            Convert.ToDateTime(row["LastUpdateDateTime"]),
                            transactionNo,
                            seqNo,
                            valueType,
                            resultValue,
                            resultUnit,
                            serviceRequestId,
                            specimenId,
                            encounterId,
                            ref accessToken,
                            codeSystem,
                            codeValue,
                            codeDisplay,
                            narrativeValue,
                            min,
                            max
                        );

                        if (obsResp != null && !string.IsNullOrEmpty(obsResp.Id))
                            observationIds.Add(obsResp.Id);
                    }

                    // POST DIAGNOSTIC REPORT
                    if (observationIds.Any())
                    {
                        PostDiagnosticReportLabOff(
                            patSs, parMedSs,
                            loincParent,
                            Convert.ToDateTime(firstRow["LastUpdateDateTime"]),
                            firstRow["HeaderNotes"]?.ToString(),
                            transactionNo,
                            firstRow["ParentSequenceNo"].ToString(),
                            serviceRequestId,
                            specimenId,
                            observationIds,
                            encounterId,
                            ref accessToken
                        );
                    }
                }
            }
        }

        private string GetNormalRange(string itemId, double ageInDays, string sex)
        {
            var stdval = new ItemLaboratoryDetailQuery();
            stdval.Where(stdval.ItemID == itemId);
            stdval.Where(stdval.TotalAgeMin <= ageInDays && stdval.TotalAgeMax >= ageInDays);
            stdval.Where(stdval.Sex == sex);

            var dtbStdVal = stdval.LoadDataTable();
            if (dtbStdVal.Rows.Count == 0)
            {
                stdval = new ItemLaboratoryDetailQuery();
                stdval.Where(stdval.ItemID == itemId);
                stdval.Where(stdval.TotalAgeMin <= ageInDays && stdval.TotalAgeMax >= ageInDays);
                stdval.Where(stdval.Sex.IsNull());
                dtbStdVal = stdval.LoadDataTable();
            }

            if (dtbStdVal.Rows.Count > 0)
            {
                var r = dtbStdVal.Rows[0];
                var tmin = r["NormalValueMin"];
                var tmax = r["NormalValueMax"];
                if (tmin != DBNull.Value && tmax != DBNull.Value)
                    return $"{tmin} - {tmax}";
                else if (tmin != DBNull.Value)
                    return tmin.ToString();
            }
            return null;
        }

        private string GetBridgingID(string itemId)
        {
            var itemSs = new ItemBridging();
            itemSs.Query.Where(itemSs.Query.ItemID == itemId, itemSs.Query.SRBridgingType == SatuSehatBridgingType);
            itemSs.Query.es.Top = 1;
            return itemSs.Query.Load() ? itemSs.BridgingID : null;
        }

        private BaseResponse PostProcedureLabOff(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterId, ref string accessToken)
        {
            // Check if already sent
            var ssResult = LoadSatuSehatResult(encounterId, "Procedure", "Diagnostic procedure", "Fasting");
            if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            DateTime regDateTime;
            if (TimeSpan.TryParse(reg.RegistrationTime, out TimeSpan regTime))
            {
                regDateTime = reg.RegistrationDate.Value.Date.Add(regTime);
            }
            else
            {
                regDateTime = reg.RegistrationDate.Value;
            }

            var startTime = regDateTime.AddHours(GmtDif).ToString(DateFormatLong) + "+00:00";
            var endTime = regDateTime.AddHours(GmtDif).AddMinutes(30).ToString(DateFormatLong) + "+00:00";


            var postData = new
            {
                resourceType = "Procedure",
                status = "not-done",
                category = new
                {
                    coding = new[]
                    {
                    new {
                        system = "http://terminology.kemkes.go.id",
                        code = "TK000028",
                        display = "Diagnostic procedure"
                    }
                },
                    text = "Prosedur diagnostik"
                },
                code = new
                {
                    coding = new[]
                    {
                        new {
                            system = "http://snomed.info/sct",
                            code = "792805006",
                            display = "Fasting",
                        }
                    }
                },
                subject = new
                {
                    reference = $"Patient/{patSs.BridgingID}",
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = $"Encounter/{encounterId}"
                },
                performedPeriod = new
                {
                    start = startTime,
                    end = endTime
                },
                performer = new[]
                {
                    new {
                        actor = new {
                            reference = $"Practitioner/{parSs.BridgingID}",
                            display = parSs.BridgingName
                        }
                    }
                },
                note = new[]
                {
                    new {
                        text = "tidak puasa sebelum pemeriksaan"
                    }
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "Diagnostic procedure",
                    Code = "Fasting"
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            return RestClientPostAndSaveLog("Procedure", requestBody, ssResult, ref accessToken);
        }

        private BaseResponse PostServiceRequestItemLabOff(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string itemName, DateTime approvedDateTime, string headerNotes, string itemNotes, LoincItem loincItem, string procedureId, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>() {
                    new {
                        system= string.Format( "http://sys-ids.kemkes.go.id/servicerequest/{0}",OrganizationID),
                        value= string.Format("{0}-{1}", transactionNo, sequenceNo) //"00001"
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>() {
                    new {
                        coding= new List<object>() {
                            new {
                                system= "http://snomed.info/sct",
                                code= "108252007",
                                display= "Laboratory procedure"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>()
                    {
                        new
                        {
                            system= "http://loinc.org",
                            code= loincItem.Code, // "11477 - 7",
                            display= loincItem.Display // "Microscopic observation[Identifier} in Sputum by Acid fast stain"
                        }
                    },
                    text = itemNotes// "Pemeriksaan Sputum BTA"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId),
                    display = string.Format("Permintaan {0} {1} di hari {2} pukul {3}", itemName, patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()], "09:30 WIB")
                },
                occurrenceDateTime = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), // "2022-06-14T09:30:27+07:00",
                authoredOn = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-06-13T12:30:27+07:00",
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                performer = new List<object>() {
                    new {
                        reference= string.Format("Practitioner/{0}", parMedSs.BridgingID),
                        display= parMedSs.BridgingName
                    }
                },
                reasonCode = new List<object>() {
                    new {
                        text = headerNotes //"Periksa Keseimbangan Elektrolit"
                    }
                },
                supportingInfo = new List<object>() {
                    new {
                        reference = $"Procedure/{procedureId}"
                    }
                }
            };
            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            return RestClientPostAndSaveLog("ServiceRequest", requestBody, ssResult, ref accessToken);
        }

        private BaseResponse PostSpecimenLabOff(PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string itemID, string collectMethod, DateTime collectDateTime, DateTime receiveDateTime, string serviceReqID, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Specimen", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo,
                    ResourceType = "Specimen"
                };
            }

            var itemLab = new ItemLaboratory();
            itemLab.LoadByPrimaryKey(itemID);

            var specimenType = new AppStandardReferenceItemBridging();
            if (!specimenType.LoadByPrimaryKey("SpecimenType", itemLab.SRSpecimenType, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging SpecimenType [{0}] not found", itemLab.SRSpecimenType);
                ssResult.Save();
                return new BaseResponse();
            }

            var cm = new AppStandardReferenceItemBridging();
            if (!cm.LoadByPrimaryKey("CollectMethod", collectMethod, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging CollectMethod [{0}] not found", collectMethod);
                ssResult.Save();
                return new BaseResponse();
            }

            var snomed = new Snomedct();
            snomed.LoadByPrimaryKey("SpecimenType", specimenType.BridgingID);

            var postData = new
            {
                resourceType = "Specimen",
                identifier = new List<object>()
                {
                    new
                    {
                        system =  string.Format("http://sys-ids.kemkes.go.id/specimen/{0}",OrganizationID),
                        value= string.Format("{0}-{1}", transactionNo, sequenceNo),
                        assigner = new {
                            reference =  string.Format("Organization/{0}",OrganizationID)
                        }
                    }
                },
                status = "available",
                type = new
                {
                    coding = new List<object>()
                    {
                        new
                        {
                            system =  "http://snomed.info/sct",
                            code = specimenType.BridgingID, // "119297000",
                            display =  snomed.Display // "Blood specimen (specimen)"
                        }
                    }
                },
                collection = new
                {
                    collector = new
                    {
                        reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                        display = parMedSs.BridgingName
                    },
                    method = new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = cm.BridgingID, //"82078001",
                                display = cm.BridgingName //"Collection of blood specimen for laboratory (procedure)"
                            }
                        }
                    },
                    collectedDateTime = string.Format("{0}+00:00", collectDateTime.AddHours(GmtDif).ToString(DateFormatSort)) //"2023 - 08 - 31T15: 15:00 + 00:00"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                request = new List<object>()
                {
                    new
                    {
                        reference = string.Format("ServiceRequest/{0}",serviceReqID)
                    }
                },
                receivedTime = string.Format("{0}+00:00", receiveDateTime.AddHours(GmtDif).ToString(DateFormatSort)) //"2023-08 - 31T15: 25:00 + 00:00"
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            return RestClientPostAndSaveLog("Specimen", requestBody, ssResult, ref accessToken);
        }

        private BaseResponse PostObservationLabOff(PatientBridging patSs, ParamedicBridging parMedSs, LoincItem loincItem, DateTime lastUpdateDateTime, string transactionNo, string sequenceNo, string valueType, // "quantity" | "codeable" | "string"
            decimal? resultValue, string resultUnit, string serviceReqID, string specimenID, string encounterId, ref string accessToken, string codeSystem = null, string codeValue = null, string codeDisplay = null, string narrativeValue = null, decimal? min = null, decimal? max = null, string referenceRangeText = null)
        {
            var ssResult = LoadSatuSehatResult(encounterId, "Observation", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null)
                return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo,
                    ResourceType = "Observation"
                };
            }

            string interpretationCode = "N";
            string interpretationDisplay = "Normal";

            if (valueType == "quantity" && min.HasValue && resultValue.HasValue)
            {
                if (resultValue < min)
                {
                    interpretationCode = "L";
                    interpretationDisplay = "Low";
                }
                else if (resultValue > max)
                {
                    interpretationCode = "H";
                    interpretationDisplay = "High";
                }
            }

            var postData = new Dictionary<string, object>
            {
                ["resourceType"] = "Observation",
                ["identifier"] = new List<object>
                {
                    new {
                        system = $"http://sys-ids.kemkes.go.id/observation/{OrganizationID}",
                        value = $"{transactionNo}-{sequenceNo}"
                    }
                },
                ["status"] = "final",
                ["category"] = new List<object>
                {
                    new {
                        coding = new List<object>
                        {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "laboratory",
                                display = "Laboratory"
                            }
                        }
                    }
                },
                ["code"] = new
                {
                    coding = new List<object>
                    {
                        new {
                            system = "http://loinc.org",
                            code = loincItem.Code,
                            display = loincItem.Display
                        }
                    }
                },
                ["subject"] = new { reference = $"Patient/{patSs.BridgingID}" },
                ["encounter"] = new { reference = $"Encounter/{encounterId}" },
                ["effectiveDateTime"] = $"{lastUpdateDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                ["issued"] = $"{lastUpdateDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                ["performer"] = new List<object>
                {
                    new { reference = $"Practitioner/{parMedSs.BridgingID}" },
                    new { reference = $"Organization/{OrganizationID}" }
                },
                ["specimen"] = new { reference = $"Specimen/{specimenID}" },
                ["basedOn"] = new List<object> { new { reference = $"ServiceRequest/{serviceReqID}" } }
            };

            // ✅ Value handler
            if (valueType == "quantity")
            {
                postData["valueQuantity"] = new
                {
                    value = resultValue,
                    unit = resultUnit,
                    system = "http://unitsofmeasure.org",
                    code = resultUnit
                };

                postData["interpretation"] = new List<object>
                {
                    new {
                        coding = new List<object>
                        {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-ObservationInterpretation",
                                code = interpretationCode,
                                display = interpretationDisplay
                            }
                        }
                    }
                };

                postData["referenceRange"] = new List<object>
                {
                    new {
                        low = (min.HasValue ? new {
                            value = min,
                            unit = resultUnit,
                            system = "http://unitsofmeasure.org",
                            code = resultUnit
                        } : null),
                        high = (max.HasValue ? new {
                            value = max,
                            unit = resultUnit,
                            system = "http://unitsofmeasure.org",
                            code = resultUnit
                        } : null)
                    }
                };
            }
            else if (valueType == "codeable")
            {
                postData["valueCodeableConcept"] = new
                {
                    coding = new List<object>
                    {
                        new {
                            system = codeSystem,
                            code = codeValue,
                            display = codeDisplay
                        }
                    }
                };

                if (!string.IsNullOrEmpty(referenceRangeText))
                {
                    postData["referenceRange"] = new List<object>
                    {
                        new { text = referenceRangeText }
                    };
                }
            }
            else if (valueType == "string")
            {
                postData["valueString"] = narrativeValue;
            }

            var requestBody = JsonConvert.SerializeObject(postData, Formatting.None,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return RestClientPostAndSaveLog("Observation", requestBody, ssResult, ref accessToken);
        }

        private void PostDiagnosticReportLabOff(
            PatientBridging patSs,
            ParamedicBridging parMedSs,
            LoincItem loincItem,
            DateTime lastUpdateDateTime,
            string notes,
            string transactionNo,
            string sequenceNo,
            string serviceReqID,
            string specimenID,
            List<string> observationIds,
            string encounterId,
            ref string accessToken)
        {
            // Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "DiagnosticReport", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo,
                    ResourceType = "DiagnosticReport"
                };
            }

            var resultList = observationIds
                .Select((obsId, index) => new
                {
                    id = (index + 1).ToString(),
                    reference = $"Observation/{obsId}"
                })
                .ToList();

            var postData = new
            {
                resourceType = "DiagnosticReport",
                identifier = new List<object>
                {
                    new {
                        system = $"http://sys-ids.kemkes.go.id/diagnostic/{OrganizationID}/lab",
                        use = "official",
                        value = $"{transactionNo}-{sequenceNo}"
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new {
                        coding = new List<object>
                        {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v2-0074",
                                code = "CH",
                                display = "Chemistry"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new {
                            system = "http://loinc.org",
                            code = loincItem.Code,
                            display = loincItem.Display
                        }
                    }
                },
                subject = new { reference = $"Patient/{patSs.BridgingID}" },
                encounter = new { reference = $"Encounter/{encounterId}" },
                effectiveDateTime = $"{lastUpdateDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                issued = $"{lastUpdateDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                performer = new List<object>
                {
                    new { reference = $"Practitioner/{parMedSs.BridgingID}" },
                    new { reference = $"Organization/{OrganizationID}" }
                },
                result = resultList,        // <-- array Observation terpisah
                specimen = new List<object>
                {
                    new { reference = $"Specimen/{specimenID}" }
                },
                basedOn = new List<object>
                {
                    new { reference = $"ServiceRequest/{serviceReqID}" }
                },
                conclusion = notes
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("DiagnosticReport", requestBody, ssResult, ref accessToken);
        }
        #endregion

        #region 08.A Pemeriksaan Penunjang - Radiologi
        public void PostServiceRequestRad(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {
            var serviceUnitRadiologyID = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyID);
            var serviceUnitRadiologyIdArray = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitRadiologyIdArray);

            //// Load latest diagnosis
            //var epsdiag = new EpisodeDiagnose();
            //epsdiag.Query.es.Top = 1;
            //epsdiag.Query.Where(
            //    epsdiag.Query.RegistrationNo == reg.RegistrationNo,
            //    epsdiag.Query.SRDiagnoseType.In("DiagnoseType-001", "DiagnoseType-006"),
            //    epsdiag.Query.IsVoid == false
            //);
            //epsdiag.Query.OrderBy(epsdiag.Query.CreateDateTime.Descending);
            //var isEpsDiag = epsdiag.Query.Load();

            //// Validate the diagnosis
            //var epDiagnose = isEpsDiag ? new Diagnose
            //{
            //    DiagnoseID = epsdiag.DiagnoseID,
            //    DiagnoseName = epsdiag.DiagnosisText
            //} : null;

            //if (epDiagnose == null)
            //    return; // Skip if no valid diagnosis

            // Query charge items
            var query = new TransChargesItemQuery("a");
            var tc = new TransChargesQuery("b");
            query.LeftJoin(tc).On(query.TransactionNo == tc.TransactionNo);
            var item = new ItemQuery("i");
            query.LeftJoin(item).On(query.ItemID == item.ItemID);
            query.Where(
                tc.RegistrationNo == reg.RegistrationNo,
                tc.IsOrder == true,
                tc.IsApproved == true,
                query.Or(
                    tc.ToServiceUnitID == serviceUnitRadiologyID,
                    tc.ToServiceUnitID.In(serviceUnitRadiologyIdArray)
                ),
                query.IsOrderRealization == true,
                query.IsVoid == false,
                item.SRItemType == ItemType.Radiology
            );
            query.Select(
                query.TransactionNo,
                query.SequenceNo,
                query.ItemID,
                item.ItemName,
                item.ItemGroupID,
                query.Notes.As("ItemNotes"),
                tc.Notes.As("HeaderNotes"),
                tc.ApprovedDateTime,
                query.ResultValue
            );

            var dtb = query.LoadDataTable();
            foreach (DataRow row in dtb.Rows)
            {
                var itemSs = new ItemBridging();
                itemSs.Query.Where(
                    itemSs.Query.ItemID == row["ItemID"].ToString(),
                    itemSs.Query.SRBridgingType == SatuSehatBridgingType
                );
                itemSs.Query.es.Top = 1;

                if (itemSs.Query.Load() && !string.IsNullOrWhiteSpace(itemSs.BridgingID))
                {
                    var loincItem = new LoincItem();
                    if (loincItem.LoadByPrimaryKey("RAD", itemSs.BridgingID))
                    {
                        var itemData = new Item
                        {
                            ItemID = row["ItemID"].ToString(),
                            ItemName = row["ItemName"].ToString(),
                            ItemGroupID = row["ItemGroupID"].ToString()
                        };

                        var approvedDate = row["ApprovedDateTime"] != DBNull.Value
                            ? Convert.ToDateTime(row["ApprovedDateTime"])
                            : DateTime.Now;

                        PostServiceRequestItemRad(
                            reg,
                            patSs,
                            parMedSs,
                            row["TransactionNo"].ToString(),
                            row["SequenceNo"].ToString(),
                            row["ResultValue"].ToString(),
                            itemData,
                            approvedDate,
                            row["HeaderNotes"]?.ToString(),
                            row["ItemNotes"]?.ToString(),
                            loincItem,
                            //epDiagnose,
                            encounterId,
                            ref accessToken
                        );
                    }
                }
            }
        }

        private void PostServiceRequestItemRad(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string resultValue, Item itemName, DateTime approvedDateTime, string headerNotes, string itemNotes, LoincItem loincItem, /*Diagnose epDiagnose,*/ string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            var itg = new ItemGroup();
            itg.LoadByPrimaryKey(itemName.ItemGroupID);

            string accessionValue;

            if (Temiang.Avicenna.Common.AppSession.Parameter.HealthcareInitialAppsVersion == "RSIMT")
            {
                accessionValue =
                    $"{new string(transactionNo.Where(char.IsDigit).ToArray())}" +
                    $"{sequenceNo.Substring(sequenceNo.Length - 1)}";
            }
            else if (Temiang.Avicenna.Common.AppSession.Parameter.RisPacsInteropVendor == "ELVA")
            {
                accessionValue =
                    $"{transactionNo}" +
                    $"{sequenceNo.Substring(sequenceNo.Length - 2)}";
            }
            else if (Temiang.Avicenna.Common.AppSession.Parameter.RisPacsInteropVendor == "INTIWID")
            {
                var orderno = string.Empty;

                foreach (var c in transactionNo.ToCharArray())
                {
                    if (!int.TryParse(c.ToString(), out int number)) continue;
                    if (number == 0) continue;
                    orderno += number.ToString();
                }

                accessionValue = orderno;
            }
            else
            {
                accessionValue = $"{transactionNo}-{sequenceNo}";
            }

            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>() {
                    new {
                        system= string.Format( "http://sys-ids.kemkes.go.id/servicerequest/{0}",OrganizationID),
                        value = $"{transactionNo}-{sequenceNo}"//accession number
                },
                new {
                    use = "usual",
                    type = new {
                        coding = new List<object>() {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v2-0203",
                                code = "ACSN"
                            }
                        }
                    },
                        system = $"http://sys-ids.kemkes.go.id/acsn/{OrganizationID}",
                        value = accessionValue //ACCNO
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>() {
                    new {
                        coding= new List<object>() {
                            new {
                                system= "http://snomed.info/sct",
                                code= "363679005",
                                display= "Imaging"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                        system= "http://loinc.org",
                        code= loincItem.Code,
                        display= loincItem.Display
                        }
                    },
                    text = loincItem.Display
                },
                orderDetail = new List<object>() {
                    new {
                        coding = new List<object>() {
                            new {
                                system = "http://dicom.nema.org/resources/ontology/DCM",
                                code = itg.Initial.Substring(itg.Initial.Length - 2)
                            }
                        },
                        text = $"Modality Code: {itg.Initial.Substring(itg.Initial.Length - 2)}"
                    },
                        new {
                            coding = new List<object>() {
                                new {
                                    system = "http://sys-ids.kemkes.go.id/ae-title",
                                    display = itg.Initial.Substring(itg.Initial.Length - 2)
                                }
                            }
                        }
                    },
                subject = new
                {
                    reference = $"Patient/{patSs.BridgingID}"
                },
                encounter = new
                {
                    reference = $"Encounter/{encounterId}",
                    display = $"Permintaan {loincItem.Display} {patSs.BridgingName} di hari {DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()]} pukul {approvedDateTime.AddHours(GmtDif):HH:mm} WIB"
                },
                occurrenceDateTime = $"{approvedDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                requester = new
                {
                    reference = $"Practitioner/{parMedSs.BridgingID}",
                    display = parMedSs.BridgingName
                },
                performer = new List<object>() {
                        new {
                            reference = $"Practitioner/{parMedSs.BridgingID}",
                            display = parMedSs.BridgingName
                        }
                    }
                //reasonCode = new List<object>() {
                //    new {
                //        coding = new List<object>() {
                //            new {
                //                system = "http://hl7.org/fhir/sid/icd-10",
                //                code = epDiagnose.DiagnoseID,
                //                display = epDiagnose.DiagnoseName
                //            }
                //        }
                //    }
                //}
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("ServiceRequest", requestBody, ssResult, ref accessToken);
        }

        private void GetImagingStudy(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string trno, string seqno, string encounterId, ref string accessToken)
        {
            var ssResult = LoadSatuSehatResult(encounterId, "ImagingStudy", trno, seqno);
            if (ssResult != null && ssResult.ResultID != null) return;

            var util = new Bridging.SatuSehat.Utils();
            var token = accessToken;

            string orgId = OrganizationID;
            string acsn = $"{trno.Replace("-", "")}{seqno.Substring(seqno.Length - 2)}"; //accession number
            string identifierQuery = $"identifier=http://sys-ids.kemkes.go.id/acsn/{orgId}|{acsn}";

            var response = util.RestClientGet($"ImagingStudy?{identifierQuery}", string.Empty, ref token);

            accessToken = token;

            if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var imagingStudyResponse = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.ImagingStudyResponse.ImagingStudyResponse>(response.Content);

                if (imagingStudyResponse.Total > 0 && imagingStudyResponse.Entry != null && imagingStudyResponse.Entry.Count > 0)
                {
                    var firstResource = imagingStudyResponse.Entry[0].Resource;

                    if (ssResult == null)
                    {
                        ssResult = new SatuSehatResult()
                        {
                            EncounterID = new Guid(encounterId),
                            Category = trno,
                            Code = seqno
                        };
                    }

                    ssResult.ResultID = Guid.Parse(firstResource.Id);

                    var requestBody = JsonConvert.SerializeObject(imagingStudyResponse);
                    RestClientPostAndSaveLog("ImagingStudy", requestBody, ssResult, ref accessToken);

                    //PostObservationRad(reg, patSs, parMedSs, trno, seqno, firstResource.Id ,encounterId, ref accessToken);
                }
                else
                {
                    Console.WriteLine("No ImagingStudy found for the given identifier.");
                }
            }
            else
            {
                Console.WriteLine($"Failed to retrieve ImagingStudy. StatusCode: {response.StatusCode}");
            }
        }

        private void PostObservationRad(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string serviceRequestId, string transactionNo, string sequenceNo, Item itemName, DateTime approvedDateTime, string observationValueString, string encounterId, ref string accessToken)
        {
            var ssResult = LoadSatuSehatResult(encounterId, "Observation", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            var postData = new
            {
                resourceType = "Observation",
                identifier = new List<object>()
                {
                    new
                    {
                        system = $"http://sys-ids.kemkes.go.id/observation/{OrganizationID}",
                        value = $"{transactionNo}-{sequenceNo}"
                    }
                },
                status = "final",
                category = new List<object>()
                {
                    new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "imaging",
                                display = "Imaging"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>()
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = itemName.ItemID,
                            display = itemName.ItemName
                        }
                    }
                },
                subject = new
                {
                    reference = $"Patient/{patSs.BridgingID}",
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = $"Encounter/{encounterId}"
                },
                effectiveDateTime = $"{approvedDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                issued = $"{approvedDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                performer = new List<object>()
                {
                    new
                    {
                        reference = $"Practitioner/{parMedSs.BridgingID}",
                        display = parMedSs.BridgingName
                    }
                },
                valueString = observationValueString,
                basedOn = new List<object>()
                {
                    new
                    {
                        reference = $"ServiceRequest/{serviceRequestId}"
                    }
                }
                //derivedFrom = new List<object>()
                //{
                //    new
                //    {
                //        reference = $"ImagingStudy/{imagingStudyId}"
                //    }
                //}
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Observation", requestBody, ssResult, ref accessToken);
        }

        private void PostDiagnosticReportRad(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, Item itemName, DateTime approvedDateTime, string observationId, string serviceRequestId, string encounterId, string conclusion, ref string accessToken)
        {
            var ssResult = LoadSatuSehatResult(encounterId, "DiagnosticReport", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            var postData = new
            {
                resourceType = "DiagnosticReport",
                identifier = new List<object>()
                {
                    new
                    {
                        system = $"http://sys-ids.kemkes.go.id/diagnostic/{OrganizationID}/rad",
                        use = "official",
                        value = $"{transactionNo}-{sequenceNo}"
                    }
                },
                status = "final",
                category = new List<object>()
                {
                    new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/v2-0074",
                                code = "RAD",
                                display = "Radiology"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>()
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = itemName.ItemID,
                            display = itemName.ItemName
                        }
                    }
                },
                subject = new
                {
                    reference = $"Patient/{patSs.BridgingID}"
                },
                encounter = new
                {
                    reference = $"Encounter/{encounterId}"
                },
                effectiveDateTime = $"{approvedDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                issued = $"{approvedDateTime.AddHours(GmtDif):yyyy-MM-ddTHH:mm:ss}+00:00",
                performer = new List<object>()
                {
                    new
                    {
                        reference = $"Practitioner/{parMedSs.BridgingID}",
                        display = parMedSs.BridgingName
                    },
                    new
                    {
                        reference = $"Organization/{OrganizationID}"
                    }
                },
                //imagingStudy = new List<object>()
                //{
                //    new
                //    {
                //        reference = $"ImagingStudy/{imagingStudyId}"
                //    }
                //},
                result = new List<object>()
                {
                    new
                    {
                        reference = $"Observation/{observationId}"
                    }
                },
                basedOn = new List<object>()
                {
                    new
                    {
                        reference = $"ServiceRequest/{serviceRequestId}"
                    }
                },
                conclusion = conclusion
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("DiagnosticReport", requestBody, ssResult, ref accessToken);
        }

        #endregion Rad

        #region Episode of Care
        private string PostEpisodeOfCare(string registrationNo, ref Registration reg, ref PatientBridging patSs, string encounterId, string encounterType, ref string accessToken)
        {
            reg = new Registration();
            reg.LoadByPrimaryKey(registrationNo);
            var episodeOfCareId = string.Empty;
            var result = EpisodeOfCarePostData(reg, patSs, encounterId, ref accessToken, encounterType);
            episodeOfCareId = result as string ?? string.Empty;

            return episodeOfCareId;
        }

        private object EpisodeOfCarePostData(Registration reg, PatientBridging patSs, string encounterId, ref string accessToken, string encounterType)
        {
            List<object> type = null;
            switch (encounterType)
            {
                case "CAD":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = encounterType,
                                    display = "Coronary Arterial Disease"
                                }
                            }
                        }
                    };
                    break;

                case "ANC":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = encounterType,
                                    display = "Antenatal Care"
                                }
                            }
                        }
                    };
                    break;

                case "PNC":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = encounterType,
                                    display = "Postnatal Care"
                                }
                            }
                        }
                    };
                    break;

                case "Neonate":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = encounterType,
                                    display = "Neonate"
                                }
                            }
                        }
                    };
                    break;

                case "TB":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = "TB-SO",
                                    display = "Tuberkulosis Sensitif Obat"
                                }
                            }
                        }
                    };
                    break;

                case "TB-RO":
                    type = new List<object>
                    {
                        new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                    code = encounterType,
                                    display = "Tuberkulosis Resisten Obat"
                                }
                            }
                        }
                    };
                    break;

                default:
                    throw new ArgumentException($"Encounter type '{encounterType}' is not recognized.");
            }


            var ssResultCheck = new SatuSehatResult();
            ssResultCheck.Query.Where(
                ssResultCheck.Query.ResourceType == "EpisodeOfCare",
                ssResultCheck.Query.Category == "EpisodeOfCare",
                ssResultCheck.Query.Code == encounterType
            );

            Guid resultIdGuid;
            if (!string.IsNullOrWhiteSpace(ssResultCheck.Query.ResultID?.ToString()) &&
                Guid.TryParse(ssResultCheck.Query.ResultID.ToString(), out resultIdGuid))
            {
                ssResultCheck.Query.Where(ssResultCheck.Query.ResultID == resultIdGuid);
            }

            ssResultCheck.Query.es.Top = 1;
            if (ssResultCheck.Query.Load())
            {
                if (ssResultCheck.ResultID.HasValue && ssResultCheck.EncounterID == new Guid(encounterId))
                    return ssResultCheck.ResultID.Value.ToString();
            }

            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "waitlist",
                statusHistory = new List<object> {
                    new {
                        status = "waitlist",
                        period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),

                        }
                    }
                },
                type,
                //type = new List<object> {
                //    new {
                //        coding = new List<object> {
                //            new {
                //                system = "http://terminology.hl7.org/CodeSystem/episodeofcare-type",
                //                code = "hacc", //penyesuaian asrib
                //                display = "Home and Community Care"//penyesuaian asrib
                //            }
                //        }
                //    }
                //},
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                }
            };

            var ssResult = new SatuSehatResult
            {
                EncounterID = new Guid(encounterId),
                Category = "EpisodeOfCare",
                Code = encounterType
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            var response = RestClientPostAndSaveLog("EpisodeOfCare", requestBody, ssResult, ref accessToken);
            var episodeOfCareId = response.Id;
            if (response == null)
                return null;
            return episodeOfCareId;
        }

        private void EpisodeOfCarePatchData(Registration reg, string episodeOfCareId, string encounterId, string encounterType, ref string accessToken)
        {
            var ssResultCheck = new SatuSehatResult();
            ssResultCheck.Query.Where(
                ssResultCheck.Query.ResourceType == "EpisodeOfCare/Patch",
                ssResultCheck.Query.Category == "EpisodeOfCare",
                ssResultCheck.Query.Code == encounterType
            );

            Guid resultIdGuid;
            if (!string.IsNullOrWhiteSpace(ssResultCheck.Query.ResultID?.ToString()) &&
                Guid.TryParse(ssResultCheck.Query.ResultID.ToString(), out resultIdGuid))
            {
                ssResultCheck.Query.Where(ssResultCheck.Query.ResultID == resultIdGuid);
            }

            ssResultCheck.Query.es.Top = 1;
            if (ssResultCheck.Query.Load())
            {
                if (ssResultCheck.ResultID.HasValue && ssResultCheck.EncounterID == new Guid(encounterId))
                    return;
            }
            var regTimes = reg.RegistrationTime.Split(':');
            var arrivedTime = reg.RegistrationDate.Value;
            arrivedTime = new DateTime(arrivedTime.Year, arrivedTime.Month, arrivedTime.Day, regTimes[0].ToInt(),
                regTimes[1].ToInt(), 0);

            var startInprogressTime = arrivedTime;
            var finishedTime = arrivedTime;

            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Descending);
            if (pa.Query.Load())
            {
                if (arrivedTime > pa.AssessmentDateTime.Value) //Kasus RegistrationTime tidak sesuai dgn jam kedatangan (Contoh dari Appointment)
                    arrivedTime = reg.LastCreateDateTime.Value;

                startInprogressTime = pa.AssessmentDateTime.Value;

            }
            else
                startInprogressTime = arrivedTime.AddMinutes(5); // tidak diketahui jam dipanggilnya sehingga anggap saja 5 menit

            // selesai ketika diberi resep
            var presc = new TransPrescription();
            presc.Query.Where(presc.Query.RegistrationNo == reg.RegistrationNo, presc.Query.IsApproval == true);
            presc.Query.es.Top = 1;
            presc.Query.OrderBy(presc.Query.PrescriptionDate.Descending);
            if (presc.Query.Load())
            {
                if (startInprogressTime > presc.CreatedDateTime.Value) // Kasus asesmen dientry setelah resep dibuat
                {
                    startInprogressTime = presc.CreatedDateTime.Value.AddMinutes(-1);
                }
            }
            var patchData = new List<object>
            {
                new
                {
                    op = "replace",
                    path = "/status",
                    value = "finished"
                },
                new
                {
                    op = "add",
                    path = "/period/end",
                    value = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong))
                },
                new
                {
                    op = "replace",
                    path = "/statusHistory/0",
                    value = new
                    {
                        status = "active",
                        period = new
                        {
                            start = string.Format("{0}+00:00", startInprogressTime.AddMinutes(-20).AddHours(GmtDif).ToString(DateFormatLong)),
                            end = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddMinutes(-10).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                new
                {
                    op = "add",
                    path = "/statusHistory/1",
                    value = new
                    {
                        status = "finished",
                        period = new
                        {
                            start = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                            end = string.Format("{0}+00:00", (presc.DeliverDateTime ?? presc.ApprovalDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                }
            };

            var requestBody = JsonConvert.SerializeObject(patchData);

            var ssResult = new SatuSehatResult
            {
                EncounterID = new Guid(encounterId),
                Category = "EpisodeOfCare",
                ResultID = new Guid(episodeOfCareId)
            };

            RestClientPatchAndSaveLog(requestBody, "EpisodeOfCare", episodeOfCareId, ssResult, ref accessToken);
        }
        #endregion
        //ClinicalImpression
        private void PostClinicalImpression(MedicalDischargeSummary mds, Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, PatientAssessment pa, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "ClinicalImpression", "PROGNOSIS", "HOD");
            if (ssResult != null && ssResult.ResultID != null) return;

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var codeValue =
                mds.SRDischargeCondition == "E01" || mds.SRDischargeCondition == "I01" || mds.SRDischargeCondition == "O01" ? "170968001" :
                mds.SRDischargeCondition == "E02" || mds.SRDischargeCondition == "I02" || mds.SRDischargeCondition == "O02" ? "65872000" :
                mds.SRDischargeCondition == "E03" || mds.SRDischargeCondition == "I03" || mds.SRDischargeCondition == "O03" ? "67334001" :
                "170968001";

            var displayValue =
                mds.SRDischargeCondition == "E01" || mds.SRDischargeCondition == "I01" || mds.SRDischargeCondition == "O01" ? "Prognosis good" :
                mds.SRDischargeCondition == "E02" || mds.SRDischargeCondition == "I02" || mds.SRDischargeCondition == "O02" ? "Fair prognosis" :
                mds.SRDischargeCondition == "E03" || mds.SRDischargeCondition == "I03" || mds.SRDischargeCondition == "O03" ? "Guarded prognosis" :
                "Prognosis good";

            var postData = new
            {
                resourceType = "ClinicalImpression",
                status = "completed",
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "312850006",
                            display = "History of disorder"
                        }
                    }
                },
                subject = new
                {
                    reference = $"Patient/{patSs.BridgingID}",
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = $"Encounter/{encounterId}",
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                },
                effectiveDateTime = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                date = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                assessor = new
                {
                    reference = $"Practitioner/{parMedSs.BridgingID}"
                },
                summary = $"{pa.Hpi}", //Pasien datang dengan keluhan utama
                prognosisCodeableConcept = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = codeValue,
                                display = displayValue
                            }
                        }
                    }
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "PROGNOSIS",
                    Code = "HOD"
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("ClinicalImpression", requestBody, ssResult, ref accessToken);
        }

        #region 12. Tatalaksana
        private void PostMedicationEducation(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Procedure", "Education", "RSP");
            if (ssResult != null && ssResult.ResultID != null) return;

            var edu = new PatientEducation();
            edu.Query.Where(edu.Query.RegistrationNo == reg.RegistrationNo, edu.Query.EducationType == "RSP");
            edu.Query.es.Top = 1;
            if (!edu.Query.Load()) return;


            var pract = LoadPerformer(edu.EducationByUserID, parMedSs.ParamedicID);

            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new
                {
                    coding = new List<object>() { new
            {
                system = "http://snomed.info/sct",
                code = "409073007",
                display = "Education"
            }
        }
                },
                code = new
                {
                    coding = new List<object>() { new
            {
                system = "http://snomed.info/sct",
                code = "61310001",
                display = "Nutrition education"
            }
        }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", edu.EducationDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)), //"2023 - 08 - 31T03: 30:00 + 00:00",
                    end = string.Format("{0}+00:00", edu.EducationDateTime.Value.AddMinutes(edu.Duration ?? 5).AddHours(GmtDif).ToString(DateFormatLong)) //"2023 - 08 - 31T03: 40:00 + 00:00"
                },
                performer = new List<object>() { new
                {
                    actor = new {
                                reference = string.Format( "Practitioner/{0}",pract.BridgingID),
                                display = pract.BridgingName
                            }
                        }
                    }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "Education",
                    Code = "RSP"
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Procedure", requestBody, ssResult, ref accessToken);
        }


        #region Obat - Medication Request
        private void PostMedication(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, DataTable dtbDiagnosisResult, string encounterId, ref string accessToken)
        {
            var tpiq = new TransPrescriptionItemQuery("tpi");
            var tpq = new TransPrescriptionQuery("tp");
            tpiq.InnerJoin(tpq).On(tpiq.PrescriptionNo == tpq.PrescriptionNo);
            tpiq.Where(tpq.RegistrationNo == reg.RegistrationNo, tpq.IsApproval == true, tpq.IsVoid == false, tpiq.IsVoid == false);

            tpiq.Select(tpiq.ItemID, tpiq.ItemInterventionID, tpiq.ParentNo, tpiq.SequenceNo, tpiq.IsCompound, tpq.PrescriptionNo,
                tpq.PrescriptionDate, tpq.InProgressDateTime, tpq.DeliverDateTime, tpiq.SequenceNo, tpq.ServiceUnitID,
                tpq.DeliverByUserID, tpq.InProgressByUserID);

            var dtbTpi = tpiq.LoadDataTable();

            //Medication Create
            foreach (DataRow row in dtbTpi.Rows)
            {
                var itemID = row["ItemInterventionID"] != DBNull.Value && !string.IsNullOrEmpty(row["ItemInterventionID"].ToString()) ? row["ItemInterventionID"].ToString() : row["ItemID"].ToString();

                if (false.Equals(row["IsCompound"]))
                {
                    var ssItem = new ItemBridging();
                    ssItem.Query.Where(ssItem.Query.ItemID == itemID, ssItem.Query.SRBridgingType == SatuSehatBridgingType);
                    ssItem.Query.es.Top = 1;
                    if (!ssItem.Query.Load()) continue;

                    var kfaItem = new SatuSehatKfa();
                    kfaItem.Query.Where(kfaItem.Query.SsUuid == ssItem.BridgingID);
                    kfaItem.Query.es.Top = 1;
                    if (!kfaItem.Query.Load()) continue;

                    var kfaInfo = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root>(kfaItem.SsResult);

                    //ZatActive
                    var ingredientZas = new List<object>();
                    foreach (var za in kfaInfo.Data.ActiveIngredients)
                    {
                        // ex. kekuatan_zat_aktif	: 5 mg/1 g
                        var zaInfos = new string[2];
                        var numerators = new string[2];
                        var denominators = new string[2];
                        if (za.KekuatanZatAktif.Contains("/"))
                        {
                            zaInfos = za.KekuatanZatAktif.Split('/');
                            numerators = zaInfos[0].Split(' ');
                            denominators = zaInfos[1].Split(' '); // satuan g tidak dikenal

                        }
                        else
                        {
                            // ex. kekuatan_zat_aktif	:	100 mg
                            numerators = za.KekuatanZatAktif.Split(' ');
                            denominators[0] = "1";
                            denominators[1] = "TAB";
                        }

                        string denominatorUnit = denominators[1];
                        string denominatorSystem;

                        // list of UCUM units
                        var ucumUnits = new[] { "mg", "g", "mcg", "mL", "L", "IU" };

                        if (ucumUnits.Contains(denominatorUnit))
                        {
                            denominatorSystem = "http://unitsofmeasure.org";
                        }
                        else
                        {
                            denominatorSystem = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm";
                        }

                        var ingredientZa =
                                new
                                {
                                    itemCodeableConcept = new
                                    {
                                        coding = new List<object>() {
                                           new
                                           {
                                               system= "http://sys-ids.kemkes.go.id/kfa",
                                               code= za.KfaCode,
                                               display= za.ZatAktif
                                           }
                                        }
                                    },
                                    isActive = za.Active,
                                    strength = new
                                    {
                                        numerator = new
                                        {
                                            value = numerators[0].ToInt(),
                                            system = "http://unitsofmeasure.org",
                                            code = numerators[1]
                                        },
                                        denominator = new
                                        {
                                            value = denominators[0].ToInt(),
                                            //system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                                            system = denominatorSystem,
                                            code = denominators[1]
                                        }
                                    }
                                };
                        ingredientZas.Add(ingredientZa);
                    }

                    // 1. Medication for Request
                    var ssResult = LoadSatuSehatResult(encounterId, "Medication", string.Format("REQ-{0}", row["PrescriptionNo"]), row["SequenceNo"].ToString());
                    var medicationForRequestID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;
                    if (string.IsNullOrWhiteSpace(medicationForRequestID))
                    {
                        var postData = MedicationForRequestNonCompoundPostData(reg, row["PrescriptionNo"].ToString(), row["SequenceNo"].ToString(), kfaInfo, ssItem, ingredientZas, encounterId);
                        if (postData != null)
                        {
                            var requestBody = JsonConvert.SerializeObject(postData);
                            if (ssResult == null)
                            {
                                ssResult = new SatuSehatResult()
                                {
                                    EncounterID = new Guid(encounterId),
                                    Category = string.Format("REQ-{0}", row["PrescriptionNo"]),
                                    Code = row["SequenceNo"].ToString()
                                };
                            }
                            var medRespon = RestClientPostAndSaveLog("Medication", requestBody, ssResult, ref accessToken);

                            if (medRespon != null && !string.IsNullOrEmpty(medRespon.Id))
                                medicationForRequestID = medRespon.Id;
                        }
                    }

                    //2. Medication Request
                    var tpi = new TransPrescriptionItem();
                    tpi.LoadByPrimaryKey(row["PrescriptionNo"].ToString(), row["SequenceNo"].ToString());

                    ssResult = LoadSatuSehatResult(encounterId, "MedicationRequest", string.Format("REQ-{0}", row["PrescriptionNo"]), row["SequenceNo"].ToString());
                    var medicationRequestID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(medicationForRequestID) && string.IsNullOrWhiteSpace(medicationRequestID))
                    {

                        var postRequestData = MedicationRequestNonCompoundPostData(reg, patSs, parMedSs, row["PrescriptionNo"].ToString(), Convert.ToDateTime(row["PrescriptionDate"]), ssItem, tpi, medicationForRequestID, dtbDiagnosisResult, encounterId);
                        if (postRequestData != null)
                        {
                            var requestBody = JsonConvert.SerializeObject(postRequestData);
                            if (ssResult == null)
                            {
                                ssResult = new SatuSehatResult()
                                {
                                    EncounterID = new Guid(encounterId),
                                    Category = string.Format("REQ-{0}", row["PrescriptionNo"]),
                                    Code = row["SequenceNo"].ToString()
                                };
                            }
                            var medReqRes = RestClientPostAndSaveLog("MedicationRequest", requestBody, ssResult, ref accessToken);
                            if (medReqRes != null && !string.IsNullOrEmpty(medReqRes.Id))
                                medicationRequestID = medReqRes.Id;
                        }
                    }

                    // 3. Medication for Dispense
                    ssResult = LoadSatuSehatResult(encounterId, "Medication", string.Format("DISP-{0}", row["PrescriptionNo"]), row["SequenceNo"].ToString());
                    var medicationForDispenseID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(medicationForRequestID) && !string.IsNullOrWhiteSpace(medicationRequestID) && string.IsNullOrWhiteSpace(medicationForDispenseID))
                    {

                        var postRequestData = MedicationForDispenseNonCompoundPostData(reg, row["PrescriptionNo"].ToString(), row["SequenceNo"].ToString(), kfaInfo, ssItem, ingredientZas, encounterId);
                        if (postRequestData != null)
                        {
                            var requestBody = JsonConvert.SerializeObject(postRequestData);
                            if (ssResult == null)
                            {
                                ssResult = new SatuSehatResult()
                                {
                                    EncounterID = new Guid(encounterId),
                                    Category = string.Format("DISP-{0}", row["PrescriptionNo"]),
                                    Code = row["SequenceNo"].ToString()
                                };
                            }

                            var medForDispRes = RestClientPostAndSaveLog("Medication", requestBody, ssResult, ref accessToken);
                            if (medForDispRes != null && !string.IsNullOrEmpty(medForDispRes.Id))
                                medicationForDispenseID = medForDispRes.Id;
                        }

                    }

                    //4. Medication Dispense
                    ssResult = LoadSatuSehatResult(encounterId, "MedicationDispense", string.Format("DISP-{0}", row["PrescriptionNo"]), row["SequenceNo"].ToString());
                    var medicationDispenseID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;
                    if (!string.IsNullOrEmpty(medicationForRequestID) && !string.IsNullOrWhiteSpace(medicationRequestID) && !string.IsNullOrWhiteSpace(medicationForDispenseID) && string.IsNullOrWhiteSpace(medicationDispenseID))
                    {
                        if (row["InProgressDateTime"] != DBNull.Value && row["DeliverDateTime"] != DBNull.Value)
                        {
                            var postDispenseData = MedicationDispenseNonCompoundPostData(reg, patSs, parMedSs, row["PrescriptionNo"].ToString(),
                                row["ServiceUnitID"].ToString(), Convert.ToDateTime(row["PrescriptionDate"]),
                                Convert.ToDateTime(row["InProgressDateTime"]),
                                Convert.ToDateTime(row["DeliverDateTime"]), row["DeliverByUserID"].ToString(),
                                tpi, medicationForDispenseID, medicationRequestID, ssItem, dtbDiagnosisResult, encounterId);
                            if (postDispenseData != null)
                            {
                                var requestBody = JsonConvert.SerializeObject(postDispenseData);
                                if (ssResult == null)
                                {
                                    ssResult = new SatuSehatResult()
                                    {
                                        EncounterID = new Guid(encounterId),
                                        Category = string.Format("DISP-{0}", row["PrescriptionNo"]),
                                        Code = row["SequenceNo"].ToString()
                                    };
                                }
                                var medDispRes = RestClientPostAndSaveLog("MedicationDispense", requestBody, ssResult, ref accessToken);
                            }
                        }
                        else
                        {
                            if (ssResult == null)
                            {
                                ssResult = new SatuSehatResult()
                                {
                                    EncounterID = new Guid(encounterId),
                                    Category = string.Format("DISP-{0}", row["PrescriptionNo"]),
                                    Code = row["SequenceNo"].ToString(),
                                    ResourceType = "MedicationDispense"
                                };
                                SetResultIndexNo(ssResult);
                            }
                            ssResult.ErrorResponse = "Deliver status still empty";
                            ssResult.Save();
                        }
                    }

                }
            }

        }

        private object MedicationForRequestNonCompoundPostData(Registration reg, string prescNo, string seqNo, Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root kfaInfo, ItemBridging ssItem, List<object> ingredientZas, string encounterId)
        {
            // Dokumentasi: https://satusehat.kemkes.go.id/platform/docs/id/fhir/resources/medication

            var postData = new
            {
                resourceType = "Medication",
                meta = new
                {
                    profile = new List<string>() { "https://fhir.kemkes.go.id/r4/StructureDefinition/Medication" }
                },
                identifier = new List<object>() {
                   new {
                       system= string.Format("http://sys-ids.kemkes.go.id/medication/{0}",OrganizationID),
                       use= "official",
                       value= string.Format("{0}-{1}",prescNo, seqNo)
                   }
                },
                code = new
                {
                    coding = new List<object>() {
                           new
                           {
                               system= "http://sys-ids.kemkes.go.id/kfa",
                               code= ssItem.BridgingID,
                               display= ssItem.BridgingName
                           }
                        }
                },
                status = "active",
                manufacturer = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                form = new
                {
                    coding = new List<object>() {
               new
               {
                   system= "http://terminology.kemkes.go.id/CodeSystem/medication-form",
                   code= kfaInfo.Data.DosageForm.Code,
                   display= kfaInfo.Data.DosageForm.Name
               }
           }
                },
                ingredient = ingredientZas,
                extension = new List<object>() {
           new
           {
               url= "https://fhir.kemkes.go.id/r4/StructureDefinition/MedicationType",
               valueCodeableConcept= new {
                   coding= new List<object>() {
                       new
                       {
                           system = "http://terminology.kemkes.go.id/CodeSystem/medication-type",
                           code= "NC",
                           display= "Non - compound"
                       }
           }
               }
           }
       }
            };


            return postData;
        }

        private object MedicationRequestNonCompoundPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string prescNo, DateTime prescDate, ItemBridging ssItem, TransPrescriptionItem tpi, string medicationReference, DataTable dtbDiagnosisResult, string encounterId)
        {
            // reasonCodes
            //var ssres = new SatuSehatResultQuery("r");
            //ssres.Where(ssres.EncounterID == new Guid(encounterId), ssres.ResourceType == "Condition", ssres.Category == "Diagnosis");
            //ssres.Select(ssres.IndexNo, ssres.ResultID, ssres.Code, ssres.PostData);
            //var dtbDiag = ssres.LoadDataTable();

            var reasonCodes = new List<object>();
            foreach (DataRow row in dtbDiagnosisResult.Rows)
            {
                var jsonDiag = JsonConvert.DeserializeObject<ConditionResponse>(row["PostData"].ToString());
                var diag = new
                {
                    coding = new List<object>() {
                                new {
                                system= "http://hl7.org/fhir/sid/icd-10",
                                code= jsonDiag.Code.Coding[0].Code,
                                display= jsonDiag.Code.Coding[0].Display
                                }
                            }
                };

                reasonCodes.Add(diag);
            }
            // timing
            // TODO: Berapa hari konsumsi obat
            var cm = new ConsumeMethod();
            cm.LoadByPrimaryKey(tpi.SRConsumeMethod);

            var postData = new
            {
                resourceType = "MedicationRequest",
                identifier = new List<object>() {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/prescription/{0}", OrganizationID),
                        use = "official",
                        value = prescNo
                    },
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/prescription-item/{0}", OrganizationID),
                        use = "official",
                        value = string.Format("{0}-{1}", prescNo, tpi.SequenceNo)//"123456788-1"
                    }
                },
                status = "completed",
                intent = "order",
                category = new List<object>() {
                    new {
                        coding = new List<object>() {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/medicationrequest-category",
                                code = "outpatient",
                                display = "Outpatient"
                            }
                        }
                    }
                },
                priority = "routine",
                medicationReference = new
                {
                    reference = string.Format("Medication/{0}", medicationReference),
                    display = ssItem.BridgingName
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                authoredOn = string.Format("{0}+00:00", prescDate.AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                reasonCode = reasonCodes,
                courseOfTherapyType = new
                {
                    coding = new List<object>() {
                        new {
                            system = "http://terminology.hl7.org/CodeSystem/medicationrequest-course-of-therapy",
                            code = "continuous",
                            display = "Continuing long term therapy"
                        }
                    }
                },
                dosageInstruction = new List<object>() {
                    new {
                        sequence = 1,
                        text = cm.SRConsumeMethodName, // tpi.DosageQty, // "4 tablet per hari",
                        additionalInstruction = new List<object>() {
                            new {
                                text = tpi.Notes //"Diminum setiap hari"
                            }
                        },
                        patientInstruction = tpi.Notes, // "4 tablet perhari, diminum setiap hari tanpa jeda sampai prose pengobatan berakhir",
                        timing = new
                        {
                            repeat = new
                            {
                                frequency = cm.IterationQty,
                                period = 1,
                                periodUnit = "d"
                            }
                        },
                        route = new {
                            coding = new List<object> {
                                new {
                                    system = "http://www.whocc.no/atc",
                                    code = "O",
                                    display = "Oral"
                                }
                            }
                        },
                        doseAndRate = new List<object> {
                            new {
                                type = new {
                                    coding = new List<object> {
                                        new {
                                            system = "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                                            code = "ordered",
                                            display = "Ordered"
                                        }
                                    }
                                },
                                //doseQuantity = new {
                                //    value = Convert.ToDecimal(new Fraction(tpi.DosageQty)) , // 4,
                                //    unit = tpi.SRDosageUnit, //"TAB",
                                //    system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                                //    code = AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", tpi.SRDosageUnit,SatuSehatBridgingType)
                                //}
                                doseQuantity = new {
                                    value = Convert.ToDecimal(new Fraction(tpi.ConsumeQty)) , // 4,
                                    unit = tpi.SRConsumeUnit, //"TAB",
                                    system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                                    code = AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", tpi.SRConsumeUnit,SatuSehatBridgingType) // ConsumeUnit pakai stdref DosageUnit
                                }
                            }
                        }
                    }
                },
                dispenseRequest = new
                {
                    dispenseInterval = new
                    {
                        value = 1,
                        unit = "days",
                        system = "http://unitsofmeasure.org",
                        code = "d"
                    },
                    validityPeriod = new
                    {
                        start = string.Format("{0}+00:00", prescDate.AddHours(GmtDif).ToString(DateFormatLong)),
                        end = string.Format("{0}+00:00", prescDate.AddDays(30).AddHours(GmtDif).ToString(DateFormatLong)),
                    },
                    numberOfRepeatsAllowed = 0,
                    quantity = new
                    {
                        value = tpi.TakenQty, //120,
                        unit = tpi.SRItemUnit, // "TAB",
                        system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                        code = AppStandardReferenceItemBridging.GetBridgingID("ItemUnit", tpi.SRItemUnit, SatuSehatBridgingType)
                    },
                    expectedSupplyDuration = new
                    {
                        value = 30,
                        unit = "days",
                        system = "http://unitsofmeasure.org",
                        code = "d"
                    },
                    performer = new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID)
                    }
                }
            };



            return postData;
        }

        #endregion

        #region Medication Dispense
        private object MedicationForDispenseNonCompoundPostData(Registration reg, string prescNo, string seqNo, Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root kfaInfo, ItemBridging ssItem, List<object> ingredientZas, string encounterId)
        {
            // Dokumentasi: https://satusehat.kemkes.go.id/platform/docs/id/fhir/resources/medication
            // LotNumber / Batch Number
            var im = new ItemMovement();
            im.Query.Where(im.Query.TransactionNo == prescNo, im.Query.SequenceNo == seqNo, im.Query.TransactionCode == "091");
            im.Query.es.Top = 1;
            if (!im.Query.Load()) return null;

            var postData = new
            {
                resourceType = "Medication",
                meta = new
                {
                    profile = new List<string>() { "https://fhir.kemkes.go.id/r4/StructureDefinition/Medication" }
                },
                identifier = new List<object>() {
                   new {
                       system= string.Format("http://sys-ids.kemkes.go.id/medication/{0}",OrganizationID),
                       use= "official",
                       value= string.Format("{0}-{1}",prescNo, seqNo)
                   }
                },
                code = new
                {
                    coding = new List<object>() {
                           new
                           {
                               system= "http://sys-ids.kemkes.go.id/kfa",
                               code= ssItem.BridgingID,
                               display= ssItem.BridgingName
                           }
                        }
                },
                status = "active",
                manufacturer = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                form = new
                {
                    coding = new List<object>() {
                       new
                       {
                           system= "http://terminology.kemkes.go.id/CodeSystem/medication-form",
                           code= kfaInfo.Data.DosageForm.Code,
                           display= kfaInfo.Data.DosageForm.Name
                       }
                    }
                },
                ingredient = ingredientZas,
                batch = new
                {
                    lotNumber = im.BatchNumber ?? "-", //"1625042A",
                    expirationDate = (im.ExpiredDate == null ? DateTime.Today.AddDays(60) : im.ExpiredDate.Value).ToString("yyyy-MM-dd"), //"2025-07-28"
                },
                extension = new List<object>() {
                   new
                   {
                       url= "https://fhir.kemkes.go.id/r4/StructureDefinition/MedicationType",
                       valueCodeableConcept= new {
                           coding= new List<object>() {
                               new
                               {
                                   system = "http://terminology.kemkes.go.id/CodeSystem/medication-type",
                                   code= "NC",
                                   display= "Non - compound"
                               }
                   }
                       }
                   }
               }
            };


            return postData;
        }

        private object MedicationDispenseNonCompoundPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string prescriptionNo, string serviceUnitID, DateTime prescriptionDate,
            DateTime inProgressDateTime, DateTime deliverDateTime, string deliverByUserID, TransPrescriptionItem tpi, string medicationForDispenseID, string medicationRequestID, ItemBridging ssItem, DataTable dtbDiagnosisResult, string encounterId)
        {
            //// MedicationRequest EncounterID
            //var medReq = new SatuSehatResult();
            //medReq.Query.Where(medReq.Query.ResourceType == "MedicationRequest", medReq.Query.EncounterID == encounterId, medReq.Query.Category == prescNo, medReq.Query.Code == tpi.SequenceNo);
            //medReq.Query.es.Top = 1;
            //if (!medReq.Query.Load()) return null;

            //var ssItem = new ItemBridging();
            //ssItem.Query.Where(ssItem.Query.ItemID == itemID, ssItem.Query.SRBridgingType == _satuSehatBridgingType);
            //ssItem.Query.es.Top = 1;
            //if (!ssItem.Query.Load()) return null;

            var ssSu = new ServiceUnitBridging();
            ssSu.Query.Where(ssSu.Query.SRBridgingType == SatuSehatBridgingType, ssSu.Query.ServiceUnitID == serviceUnitID);
            if (!ssSu.Query.Load()) return null;

            // reasonCodes
            //var ssres = new SatuSehatResultQuery("r");
            //ssres.Where(ssres.EncounterID == new Guid(encounterId), ssres.ResourceType == "Condition", ssres.Category == "Diagnosis");
            //ssres.Select(ssres.IndexNo, ssres.ResultID, ssres.Code, ssres.PostData);
            //var dtbDiag = ssres.LoadDataTable();

            var reasonCodes = new List<object>();
            foreach (DataRow row in dtbDiagnosisResult.Rows)
            {
                var jsonDiag = JsonConvert.DeserializeObject<ConditionResponse>(row["PostData"].ToString());
                var diag = new
                {
                    coding = new List<object>() {
                                new {
                                system= "http://hl7.org/fhir/sid/icd-10",
                                code= jsonDiag.Code.Coding[0].Code,
                                display= jsonDiag.Code.Coding[0].Display
                                }
                            }
                };

                reasonCodes.Add(diag);
            }
            // timing
            // TODO: Berapa hari konsumsi obat

            var deliverBy = LoadPerformer(deliverByUserID, parMedSs.ParamedicID);

            var cm = new ConsumeMethod();
            cm.LoadByPrimaryKey(tpi.SRConsumeMethod);

            var postData = new
            {
                resourceType = "MedicationDispense",
                identifier = new List<object>() {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/prescription/{0}", OrganizationID),
                        use = "official",
                        value = prescriptionNo
                    },
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/prescription-item/{0}", OrganizationID),
                        use = "official",
                        value = string.Format("{0}-{1}", prescriptionNo, tpi.SequenceNo)//"123456788-1"
                    }
                },
                status = "completed",
                category = new
                {
                    coding = new List<object>() {
                       new
                       {
                           system = "http://terminology.hl7.org/fhir/CodeSystem/medicationdispense-category",
                           code= "outpatient",
                           display= "Outpatient"
                       }
                   }
                },
                medicationReference = new
                {
                    reference = string.Format("Medication/{0}", medicationForDispenseID),
                    display = ssItem.BridgingName //"Obat Anti Tuberculosis / Rifampicin 150 mg / Isoniazid 75 mg / Pyrazinamide 400 mg / Ethambutol 275 mg Kaplet Salut Selaput(KIMIA FARMA)"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                context = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                performer = new List<object>() {
                   new
                   {
                       actor= new {
                           reference = string.Format( "Practitioner/{0}",deliverBy.BridgingID),
                           display= deliverBy.BridgingName
                        }
                   }
                },
                location = new
                {
                    reference = string.Format("Location/{0}", ssSu.BridgingID),
                    display = ssSu.BridgingName
                },
                authorizingPrescription = new List<object>() {
                   new
                   {
                       reference = string.Format( "MedicationRequest/{0}", medicationRequestID)
                   }
                },
                quantity = new
                {
                    system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                    code = AppStandardReferenceItemBridging.GetBridgingID("ItemUnit", tpi.SRItemUnit, SatuSehatBridgingType),
                    value = tpi.TakenQty
                },

                daysSupply = new
                {
                    value = 30,
                    unit = "Day",
                    system = "http://unitsofmeasure.org",
                    code = "d"
                },
                whenPrepared = string.Format("{0}+00:00", inProgressDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-01-15T10:20:00Z",
                whenHandedOver = string.Format("{0}+00:00", deliverDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-01-15T16:20:00Z",
                dosageInstruction = new List<object>() {
                   new
                   {
                       sequence= 1,
                       text= cm.SRConsumeMethodName, //"Diminum 4 tablet sekali dalam sehari",
                       timing= new {
                           repeat= new {
                               frequency= cm.IterationQty,
                               period= 1,
                               periodUnit= "d"
                   }
                },
                doseAndRate= new List<object>() {
               new
               {
                   type= new {
                       coding= new List<object>() {
                           new
                           {
                               system = "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                               code= "ordered",
                               display= "Ordered"
                           }
               }
                   },
                   //doseQuantity= new {
                   //    value = Convert.ToDecimal(new Fraction(tpi.DosageQty)), // 4,
                   //    unit= tpi.SRDosageUnit, //"TAB",
                   //    system= "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                   //    code= AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", tpi.SRDosageUnit,SatuSehatBridgingType)

                    doseQuantity= new {
                    value = Convert.ToDecimal(new Fraction(tpi.ConsumeQty)), // 4,
                    unit= tpi.SRDosageUnit, //"TAB",
                    system= "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                    code= AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", tpi.SRConsumeUnit,SatuSehatBridgingType) // ConsumeUnit pakai stdref DosageUnit
                   }
               }
           }
       }
   }
            };

            return postData;
        }

        #endregion  Medication Dispense

        #region Obat - Pengkajian Resep
        private object AnswerPengkajian(string itemID, string bridgingName, bool isYes)
        {


            if (bridgingName.ToLower().Contains("sesuai"))
                // https://satusehat.kemkes.go.id/platform/docs/id/interoperability/rme-rawat-jalan/
                return
            new List<object>() {
                        new {
                            valueCoding = new
                            {
                                system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                code = isYes? "OV000052":"OV000053",
                                display = isYes? "Sesuai":"Tidak Sesuai"
                            }
                        }
            };

            return new List<object>() { new { valueBoolean = isYes } };
        }
        private void PostPengkajianResep(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {

            // Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "QuestionnaireResponse", "QuestionnaireResponse", "");
            if (ssResult != null && ssResult.ResultID != null) return;

            var reviewedDateTime = DateTime.Now;
            var reviewedByUserID = string.Empty;
            DataTable dtbPrescRevResult = null;


            // Check PrescriptionReview
            var healthcareInitialAppsVersion = AppParameter.GetParameterValue(AppParameter.ParameterItem.HealthcareInitialAppsVersion);
            if (healthcareInitialAppsVersion == "YBRSGKP") // Sementara pakai ini krn belum ada flag status module Farmasi Klinis diberikan atau tidak (Handono)
            {
                var presc = new TransPrescriptionQuery("p");

                var prescRev = new PrescriptionReviewQuery("pr"); // Dientry dari Menu Prescription Review
                presc.InnerJoin(prescRev).On(presc.PrescriptionNo == prescRev.PrescriptionNo);

                var brg = new AppStandardReferenceItemBridgingQuery("brg");
                presc.InnerJoin(brg).On(brg.SRBridgingType == SatuSehatBridgingType && brg.ItemID == prescRev.SRPrescReview);

                presc.Select(prescRev.PrescriptionNo, brg.ItemID, prescRev.IsRight.As("IsYes"), presc.ReviewedDateTime, presc.ReviewedByUserID);
                presc.Where(presc.RegistrationNo == reg.RegistrationNo);
                presc.OrderBy(prescRev.PrescriptionNo.Ascending, brg.BridgingID.Ascending);
                dtbPrescRevResult = presc.LoadDataTable();
                if (dtbPrescRevResult.Rows.Count == 0) return;

                foreach (DataRow row in dtbPrescRevResult.Rows)
                {
                    reviewedDateTime = Convert.ToDateTime(row["ReviewedDateTime"]);
                    reviewedByUserID = row["ReviewedByUserID"].ToString();
                    break;
                }
            }
            else
            {
                var presc = new TransPrescriptionQuery("p");

                var prescRev = new TransPrescriptionReviewQuery("pr"); // Dientry dari tombol review pada Prescription Handling
                presc.InnerJoin(prescRev).On(presc.PrescriptionNo == prescRev.PrescriptionNo);

                var brg = new AppStandardReferenceItemBridgingQuery("brg");
                presc.InnerJoin(brg).On(brg.SRBridgingType == SatuSehatBridgingType && brg.ItemID == prescRev.SRPrescriptionReview);


                presc.Select(prescRev.PrescriptionNo, brg.ItemID, prescRev.IsPrescriptionReview.As("IsYes"), prescRev.PrescriptionReviewDateTime.As("ReviewedDateTime"), prescRev.PrescriptionReviewByUserID.As("ReviewedByUserID"));
                presc.Where(presc.RegistrationNo == reg.RegistrationNo);
                presc.OrderBy(prescRev.PrescriptionNo.Ascending, brg.BridgingID.Ascending);
                dtbPrescRevResult = presc.LoadDataTable();
                if (dtbPrescRevResult.Rows.Count == 0) return;

                foreach (DataRow row in dtbPrescRevResult.Rows)
                {
                    reviewedDateTime = Convert.ToDateTime(row["ReviewedDateTime"]);
                    reviewedByUserID = row["ReviewedByUserID"].ToString();
                    break;
                }
            }

            var stdib = new AppStandardReferenceItemBridgingQuery("stdib");

            if (healthcareInitialAppsVersion == "YBRSGKP") // Sementara pakai ini krn belum ada flag status module Farmasi Klinis diberikan atau tidak (Handono)
                stdib.Where(stdib.StandardReferenceID == "PrescReview");
            else
                stdib.Where(stdib.StandardReferenceID == "PrescriptionReview");

            stdib.OrderBy(stdib.BridgingID.Ascending);
            var dtbRev = stdib.LoadDataTable();
            var listRev1 = new List<object>();
            var listRev2 = new List<object>();
            var listRev3 = new List<object>();

            foreach (DataRow row in dtbRev.Rows)
            {
                var itemID = row["ItemID"];
                var isYes = false;
                var isReviewed = false;
                foreach (DataRow rowResult in dtbPrescRevResult.Rows)
                {
                    if (itemID.Equals(rowResult["ItemID"]))
                    {
                        if (rowResult["IsYes"] != DBNull.Value)
                        {
                            isYes = Convert.ToBoolean(rowResult["IsYes"]);
                            isReviewed = true;
                        }
                        break;
                    }
                }

                if (!isReviewed) continue;

                var bid = row["BridgingID"].ToString();
                if (bid.Contains("1."))
                    listRev1.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerPengkajian(row["ItemID"].ToString(), row["BridgingName"].ToString(), isYes)
                        }
                    );
                else if (bid.Contains("2."))
                    listRev2.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerPengkajian(row["ItemID"].ToString(), row["BridgingName"].ToString(), isYes)
                        }
                    );
                else if (bid.Contains("3."))
                    listRev3.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerPengkajian(row["ItemID"].ToString(), row["BridgingName"].ToString(), isYes)
                        }
                    );
            }

            var author = LoadPerformer(reviewedByUserID, parMedSs.ParamedicID);

            var postData = new
            {
                resourceType = "QuestionnaireResponse",
                questionnaire = "https://fhir.kemkes.go.id/Questionnaire/Q0007",
                status = "completed",
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                authored = string.Format("{0}+00:00", reviewedDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                author = new
                {
                    reference = string.Format("Practitioner/{0}", author.BridgingID),
                    display = author.BridgingName
                },
                source = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },

                item = new List<object>() {
                    new {
                        linkId = "1",
                        text= "Persyaratan Administrasi",
                        item = listRev1
                    },
                    new {
                        linkId = "2",
                        text= "Persyaratan Farmasetik",
                        item = listRev2
                    },
                    new {
                        linkId = "3",
                        text= "Persyaratan Klinis",
                        item = listRev3
                    }
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "QuestionnaireResponse",
                    Code = ""
                };
            }
            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("QuestionnaireResponse", requestBody, ssResult, ref accessToken);
        }
        #endregion Obat - Pengkajian Resep
        #endregion 12. Tatalaksana

        #region 13. Prognosis
        #endregion 13. Prognosis

        #region Immunization
        private void PostImmunization(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId, ref string accessToken)
        {
            // Doc: https://satusehat.kemkes.go.id/platform/docs/id/terminology/lampiran-terminologi/imunisasi-new/

            // Check PatientImmunization
            var pimColl = new PatientImmunizationCollection();
            pimColl.Query.Where(pimColl.Query.RegistrationNo == reg.RegistrationNo);
            pimColl.LoadAll();
            if (pimColl.Count == 0) return;

            foreach (var pim in pimColl)
            {
                var ssResult = new SatuSehatResult();
                ssResult.Query.Where(ssResult.Query.EncounterID == new Guid(encounterId), ssResult.Query.ResourceType == "Immunization", ssResult.Query.Category == pim.ReferenceNo, ssResult.Query.Code == pim.VaccineID);

                if (!ssResult.Query.Load())
                    ssResult = new SatuSehatResult();
                else if (ssResult.ResultID != null)
                    continue; //skip

                var postData = ImmunizationPostData(ref ssResult, reg, patSs, parMedicSs, encounterId, pim);
                if (postData != null)
                {
                    var requestBody = JsonConvert.SerializeObject(postData);
                    RestClientPostAndSaveLog(postData.ResourceType, requestBody, ssResult, ref accessToken);
                }
                else
                {
                    // Save error log
                    if (!string.IsNullOrEmpty(ssResult.ErrorResponse))
                        ssResult.Save();
                }
            }
        }
        private ImmunizationPost.Root ImmunizationPostData(ref SatuSehatResult ssResult, Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, PatientImmunization pim)
        {
            var postData = new ImmunizationPost.Root();
            postData.ResourceType = "Immunization";
            postData.Status = "completed";

            // SatuSehatResult
            if (ssResult.EncounterID == null)
            {
                ssResult.EncounterID = new Guid(encounterId);
                ssResult.Category = pim.ReferenceNo;
                ssResult.Code = pim.VaccineID;
                ssResult.ResourceType = postData.ResourceType;
                SetResultIndexNo(ssResult);
            }

            #region VaccineCode
            // Vaccine Code terdiri dari Vaccine Drug + Detil Imunisasi (Cvx Group) + Cvx name
            var codings = new List<ImmunizationPost.Coding>();

            var cvxGroupColl = new ItemImmunizationCollection();
            cvxGroupColl.Query.Where(cvxGroupColl.Query.ItemID == pim.VaccineID);
            cvxGroupColl.LoadAll();
            if (cvxGroupColl.Count == 0)
            {
                ssResult.ErrorResponse = string.Format("Item Immunization for Vaccine {0} still empty", pim.VaccineID);
                return null;
            }

            var ssItem = LoadItem(pim.VaccineID);
            if (ssItem == null)
            {
                ssResult.ErrorResponse = string.Format("Item Bridging for Vaccine {0} still empty", pim.VaccineID);
                return null;
            }

            // Vaccin Drug
            codings.Add(new ImmunizationPost.Coding()
            {
                System = "http://sys-ids.kemkes.go.id/kfa",
                Code = ssItem.BridgingID,
                Display = ssItem.BridgingName

            });

            // Cvx Group
            foreach (var cvxGroup in cvxGroupColl)
            {
                var ssImm = new ImmunizationBridging();
                ssImm.Query.Where(ssImm.Query.ImmunizationID == cvxGroup.ImmunizationID, ssImm.Query.SRBridgingType == SatuSehatBridgingType);
                ssImm.Query.es.Top = 1;
                if (!ssImm.Query.Load())
                {
                    ssResult.ErrorResponse = string.Format("Immunization Bridging for {0} still empty", cvxGroup.ImmunizationID);
                    return null;
                }

                codings.Add(new ImmunizationPost.Coding()
                {
                    System = "http://sys-ids.kemkes.go.id/kfa",
                    Code = ssImm.BridgingID,
                    Display = ssImm.BridgingName
                });
            }

            // Cvx Name
            var ipm = new ItemProductMedic();
            ipm.LoadByPrimaryKey(pim.VaccineID);

            var cvx = AppStandardReferenceItemBridging.Load(AppStandardReferenceItemBridging.SatusehatRef.CvxName, ipm.SRCvxName, SatuSehatBridgingType);
            if (cvx == null)
            {
                ssResult.ErrorResponse = string.Format("Cvx Name Bridging for {0} still empty (Std Ref)", ipm.SRCvxName);
                return null;
            }
            codings.Add(new ImmunizationPost.Coding()
            {
                System = "http://hl7.org/fhir/sid/cvx",
                Code = cvx.BridgingID,
                Display = cvx.BridgingName
            });


            postData.VaccineCode = new ImmunizationPost.VaccineCode()
            {
                Coding = codings
            };
            #endregion VaccineCode

            postData.Patient = new ImmunizationPost.Patient()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            postData.Encounter = new ImmunizationPost.Encounter()
            {
                Reference = string.Format("Encounter/{0}", encounterId)
            };

            postData.OccurrenceDateTime = FormatDateLong(pim.ImmunizationDate.Value); // string.Format("{0}+00:00", pim.ImmunizationDate.Value.AddHours(GmtDif).ToString(DateFormat));
            postData.Recorded = FormatDateLong(pim.LastUpdateDateTime.Value);
            postData.PrimarySource = true;

            var tc = new TransCharges();
            tc.LoadByPrimaryKey(pim.ReferenceNo);

            var ssLoc = LoadLocation(pim.ServiceUnitID);
            if (ssLoc == null)
            {
                ssResult.ErrorResponse = string.Format("Service Unit Bridging for {0} still empty", pim.ServiceUnitID);
                return null;
            }

            postData.Location = new ImmunizationPost.Location()
            {
                Reference = string.Format("Location/{0}", ssLoc.BridgingID),
                Display = ssLoc.BridgingName
            };

            postData.LotNumber = pim.BatchNumber;
            postData.ExpirationDate = FormatDateSort(pim.ImmunizationDate.Value.AddDays(60));

            // Route
            var route = AppStandardReferenceItemBridging.Load(AppStandardReferenceItemBridging.SatusehatRef.Route, ipm.SRRoute, SatuSehatBridgingType);
            if (route == null)
            {
                ssResult.ErrorResponse = string.Format("Route Bridging for {0} still empty (Std Ref)", ipm.SRRoute);
                return null;
            }

            postData.Route = new ImmunizationPost.Route()
            {
                Coding = new List<ImmunizationPost.Coding>()
                {
                    new ImmunizationPost.Coding()
                    {
                        System= "http://www.whocc.no/atc",
                        Code= route.BridgingID,
                        Display= route.BridgingName
                    }
                }
            };

            // Dosage
            var dsg = AppStandardReferenceItemBridging.Load(AppStandardReferenceItemBridging.SatusehatRef.DosageUnit, pim.SRDosageUnit, SatuSehatBridgingType);
            if (dsg == null)
            {
                ssResult.ErrorResponse = string.Format("Dosage Unit Bridging for {0} still empty (Std Ref)", pim.SRDosageUnit);
                return null;
            }
            postData.DoseQuantity = new ImmunizationPost.DoseQuantity()
            {
                Value = pim.QtyDosage.ToInt(),
                Unit = dsg.BridgingID,
                System = "http://unitsofmeasure.org",
                Code = dsg.BridgingID

            };

            // Imunisasi dilakukan oleh Nakes
            var ssPerf = LoadPerformerByParamedicID(pim.ParamedicID);
            if (ssPerf == null)
            {
                ssResult.ErrorResponse = string.Format("Paramedic Bridging for {0} still empty", pim.ParamedicID);
                return null;
            }
            postData.Performer = new List<ImmunizationPost.Performer>()
            {
               new ImmunizationPost.Performer(){
                   Function = new ImmunizationPost.Function(){
                       Coding = new List<ImmunizationPost.Coding>()
                       {
                           new ImmunizationPost.Coding() {
                               System = "http://terminology.hl7.org/CodeSystem/v2-0443",
                               Code = "AP",
                               Display = "Administering Provider"
                           }
                       }
                   },
                   Actor = new ImmunizationPost.Actor() { Reference = string.Format("Practitioner/{0}", ssPerf.BridgingID) }
               }
            };

            var reas = AppStandardReferenceItemBridging.Load(AppStandardReferenceItemBridging.SatusehatRef.ImmReason, pim.SRImmReason, SatuSehatBridgingType);
            if (reas == null)
            {
                ssResult.ErrorResponse = string.Format("Immunization Reason Bridging for {0} still empty", pim.SRImmReason);
                return null;
            }

            var tim = AppStandardReferenceItemBridging.Load(AppStandardReferenceItemBridging.SatusehatRef.ImmTiming, pim.SRImmTiming, SatuSehatBridgingType);
            if (tim == null)
            {
                ssResult.ErrorResponse = string.Format("Immunization Routine Timing Bridging for {0} still empty", pim.SRImmTiming);
                return null;
            }

            postData.ReasonCode = new List<ImmunizationPost.ReasonCode>()
            {
                new ImmunizationPost.ReasonCode(){
                    Coding = new List<ImmunizationPost.Coding>() {
                        new ImmunizationPost.Coding() {
                            System = "http://terminology.kemkes.go.id/CodeSystem/immunization-reason",
                            Code= reas.BridgingID,
                            Display= reas.BridgingName
                        },
                        new ImmunizationPost.Coding() {
                            System = "http://terminology.kemkes.go.id/CodeSystem/immunization-routine-timing",
                            Code= tim.BridgingID,
                            Display= tim.BridgingName
                        }
                    }
                }
            };

            postData.ProtocolApplied = new List<ImmunizationPost.ProtocolApplied>()
            {
                new ImmunizationPost.ProtocolApplied()
                {
                    DoseNumberPositiveInt = 1
                }
            };

            return postData;
        }
        #endregion Immunization

        public string PatientBridgingID(string patientID, string ssn, string patientName, ref string accessToken)
        {
            var patSs = new PatientBridging();
            if (patSs.LoadByPrimaryKey(patientID, SatuSehatBridgingType) && !string.IsNullOrWhiteSpace(patSs.BridgingID))
                return patSs.BridgingID;

            if (string.IsNullOrWhiteSpace(patSs.BridgingID))
            {
                if (!string.IsNullOrWhiteSpace(ssn))
                {
                    var pat = new Patient();
                    if (!pat.LoadByPrimaryKey(patientID))
                        return null;

                    ssn = pat.Ssn;
                    patientName = pat.PatientName;
                }

                // Retrieve SS Patient ID
                var response = RestClientGet("Patient?identifier=https://fhir.kemkes.go.id/id", string.Concat("nik|", ssn), ref accessToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var patientSearchResponse = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.PatientSearch.PatientSearchResponse>(response.Content);
                    if (patientSearchResponse.Total == 1)
                    {
                        // Add PatientBridging
                        if (string.IsNullOrEmpty(patSs.PatientID))
                        {
                            patSs = new PatientBridging();
                        }

                        patSs.PatientID = patientID;
                        patSs.BridgingID = patientSearchResponse.Entry[0].Resource.Id;
                        patSs.BridgingName = patientName;
                        patSs.SRBridgingType = SatuSehatBridgingType;
                        patSs.IsActive = true;
                        patSs.Save();

                        return patSs.BridgingID;
                    }
                    else
                    {
                        //satuSehatLog.ErrorResponse = string.Format("SSN {0} not found at fhir.kemkes.go.id", pat.Ssn);
                        //satuSehatLog.Save();
                        //return;
                    }
                }
                //else
                //{
                //    satuSehatLog.ErrorResponse = response.Content;
                //    satuSehatLog.Save();
                //    return;
                //}
            }
            return string.Empty;
        }

        #endregion Pelayanan Rawat Jalan

        #region Mapping ID
        public RestResponse PostServiceUnit(string serviceUnitID)
        {
            if (string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ClientID))
                return null;

            var serviceUnit = new ServiceUnit();
            if (!serviceUnit.LoadByPrimaryKey(serviceUnitID))
                return null;


            // Check Mapping SatuSehat hanay boleh 1 untuk 1 ServiceUnit
            var sub = new ServiceUnitBridging();
            var qr = new ServiceUnitBridgingQuery("q");
            qr.Where(qr.ServiceUnitID == serviceUnit.ServiceUnitID, qr.SRBridgingType == Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID));
            qr.es.Top = 1;
            if (sub.Load(qr))
                return null;

            var accessToken = string.Empty;

            var hc = new Healthcare();
            hc.LoadByPrimaryKey(AppParameter.GetParameterValue(AppParameter.ParameterItem.HealthcareID));

            var telecom = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom>()
            {
                new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "phone",
                    Value = hc.PhoneNo,
                    Use = "work",
                },
                 new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "fax",
                    Value = hc.FaxNo,
                    Use = "work",
                }
            };

            var postData = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Resource()
            {
                ResourceType = "Location",
                Identifier = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Identifier>()
                {
                    new Bridging.SatuSehat.BusinessObject.Master.Location.Identifier()
                    {
                        //System = String.Concat("http://sys-ids.kemkes.go.id/location/",serviceUnit.DepartmentID),
                        System = String.Concat("http://sys-ids.kemkes.go.id/location/",OrganizationID),
                        Value = serviceUnit.ServiceUnitID
                    }
                },
                Status = "active",
                Name = serviceUnit.ServiceUnitName,
                Description = serviceUnit.ServiceUnitName,
                Mode = "instance",
                Telecom = telecom,
                Address = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Address()
                {
                    Use = "work",
                    Line = new List<string>()
                    {
                        hc.AddressLine1,
                        hc.AddressLine2
                    },
                    City = hc.City,
                    PostalCode = hc.ZipCode,
                    Country = "ID",
                    ExtensionInfo = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.ExtensionInfo>()
                    {
                        new Bridging.SatuSehat.BusinessObject.Master.Location.ExtensionInfo()
                        {
                            Url="https://fhir.kemkes.go.id/r4/StructureDefinition/administrativeCode",
                            Extension = new List<Bridging.SatuSehat.BusinessObject.Master.Location.Extension>
                            {
                                new Bridging.SatuSehat.BusinessObject.Master.Location.Extension(){
                                    Url="province",
                                    ValueCode = String.IsNullOrWhiteSpace(hc.ProvincesCode) ? "31":hc.ProvincesCode
                                    },
                            }
                        }
                    },
                },
                PhysicalType = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType()
                {
                    Coding = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding>()
                    {
                        new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding()
                        {
                            System = "http://terminology.hl7.org/CodeSystem/location-physical-type",
                            Code = "ro",
                            Display = "Room",
                        }
                    },
                },
                Position = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Position()
                {
                    Longitude = 1,
                    Latitude = 1,
                    Altitude = 1,
                },
                ManagingOrganization = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.ManagingOrganization()
                {
                    Reference = String.Concat("Organization/", OrganizationID),
                },
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            var response = RestClientPost(requestBody, "Location", ref accessToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resp = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PostPutResponse>(response.Content);
                sub = new ServiceUnitBridging();
                sub.ServiceUnitID = serviceUnit.ServiceUnitID;
                sub.SRBridgingType = Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID);
                sub.BridgingID = resp.Id;
                sub.BridgingName = resp.Name;
                sub.IsActive = true;
                sub.Save();
            }
            return response;
        }

        public RestResponse PostRoom(string serviceUnitID, string RoomID)
        {
            if (string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ClientID))
                return null;

            var serviceUnit = new ServiceUnit();
            if (!serviceUnit.LoadByPrimaryKey(serviceUnitID))
                return null;

            var serviceRoom = new ServiceRoom();
            if (!serviceRoom.LoadByPrimaryKey(RoomID))
                return null;


            // Check Mapping SatuSehat hanya boleh 1 untuk 1 ServiceUnit
            var sub = new ServiceUnitBridging();
            var qr = new ServiceUnitBridgingQuery("q");
            qr.Where(qr.ServiceUnitID == serviceUnit.ServiceUnitID, qr.SRBridgingType == Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID));
            qr.es.Top = 1;
            sub.Load(qr);

            var srb = new ServiceRoomBridging();
            var qsr = new ServiceRoomBridgingQuery("q");
            qsr.Where(qsr.RoomID == serviceRoom.RoomID, qsr.SRBridgingType == Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID));
            qsr.es.Top = 1;
            if (srb.Load(qsr))
                return null;

            var accessToken = string.Empty;

            var hc = new Healthcare();
            hc.LoadByPrimaryKey(AppParameter.GetParameterValue(AppParameter.ParameterItem.HealthcareID));

            var telecom = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom>()
            {
                new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "phone",
                    Value = hc.PhoneNo,
                    Use = "work",
                },
                 new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "fax",
                    Value = hc.FaxNo,
                    Use = "work",
                }
            };

            var postData = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Resource()
            {
                ResourceType = "Location",
                Identifier = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Identifier>()
                {
                    new Bridging.SatuSehat.BusinessObject.Master.Location.Identifier()
                    {
                        System = String.Concat("http://sys-ids.kemkes.go.id/location/",OrganizationID),
                        Value = serviceRoom.RoomID
                    }
                },
                Status = "active",
                Name = serviceRoom.RoomName,
                Description = string.Format("{0},{1}", serviceRoom.RoomName, serviceUnit.ServiceUnitName),
                Mode = "instance",
                Telecom = telecom,
                Type = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType>()
                {
                    new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType()
                    {
                        Coding = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding>()
                        {
                            new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding()
                            {
                                System = "http://terminology.kemkes.go.id/CodeSystem/location-type",
                                Code = "RT0016",
                                Display = "Ruang Rawat Inap"
                            }
                        }
                    }
                },
                PhysicalType = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType()
                {
                    Coding = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding>()
                    {
                        new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding()
                        {
                            System = "http://terminology.hl7.org/CodeSystem/location-physical-type",
                            Code = "ro",
                            Display = "Room",
                        }
                    },
                },
                Position = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Position()
                {
                    Longitude = 1,
                    Latitude = 1,
                    Altitude = 1,
                },
                ManagingOrganization = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.ManagingOrganization()
                {
                    Reference = String.Concat("Organization/", OrganizationID),
                },
                partOf = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.partOf()
                {
                    Reference = String.Concat("Location/", sub.BridgingID),
                    Display = sub.BridgingName
                }
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            var response = RestClientPost(requestBody, "Location", ref accessToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resp = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PostPutResponse>(response.Content);
                srb = new ServiceRoomBridging();
                srb.RoomID = serviceRoom.RoomID;
                srb.SRBridgingType = Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID);
                srb.BridgingID = resp.Id;
                srb.BridgingName = resp.Name;
                srb.IsActive = true;
                srb.Save();
            }
            return response;
        }

        public void PostBed(string serviceUnitID, string RoomID)
        {
            if (string.IsNullOrWhiteSpace(OrganizationID) || string.IsNullOrWhiteSpace(ClientID))
                return;

            var serviceUnit = new ServiceUnit();
            if (!serviceUnit.LoadByPrimaryKey(serviceUnitID))
                return;

            var serviceRoom = new ServiceRoom();
            if (!serviceRoom.LoadByPrimaryKey(RoomID))
                return;

            var bed = new BedCollection();
            bed.Query.Where(bed.Query.RoomID == RoomID, bed.Query.IsActive == true);
            bed.LoadAll();

            foreach (var bd in bed)
            {
                if (string.IsNullOrWhiteSpace(bd.BedID) || !string.IsNullOrWhiteSpace(bd.SatuSehatBridgingID)) continue;
                //Process
                PostBedJson(serviceUnit, RoomID, bd.BedID);
            }
        }
        public RestResponse PostBedJson(ServiceUnit su, string RoomID, string BedID)
        {

            var srb = new ServiceRoomBridging();
            var qsr = new ServiceRoomBridgingQuery("q");
            qsr.Where(qsr.RoomID == RoomID, qsr.SRBridgingType == Temiang.Avicenna.BusinessObject.AppParameter.GetParameterValue(Temiang.Avicenna.BusinessObject.AppParameter.ParameterItem.SatuSehatBridgingTypeID));
            qsr.es.Top = 1;
            if (!srb.Load(qsr))
                return null;

            var bb = new Bed();
            var bbq = new BedQuery("q");
            bbq.Where(bbq.RoomID == RoomID, bbq.BedID == BedID);
            bbq.es.Top = 1;
            if (!bb.Load(bbq))
                return null;

            var accessToken = string.Empty;

            var hc = new Healthcare();
            hc.LoadByPrimaryKey(AppParameter.GetParameterValue(AppParameter.ParameterItem.HealthcareID));

            var telecom = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom>()
            {
                new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "phone",
                    Value = hc.PhoneNo,
                    Use = "work",
                },
                 new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Telecom()
                {
                    System = "fax",
                    Value = hc.FaxNo,
                    Use = "work",
                }
            };

            var postData = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Resource()
            {
                ResourceType = "Location",
                Identifier = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Identifier>()
                {
                    new Bridging.SatuSehat.BusinessObject.Master.Location.Identifier()
                    {
                        System = String.Concat("http://sys-ids.kemkes.go.id/location/",OrganizationID),
                        Value = BedID
                    }
                },
                Status = "active",
                Name = string.Format("Bed {0},{1},{02}", BedID, srb.BridgingName, su.ServiceUnitName),
                Description = string.Format("Bed {0},{1},{02}", BedID, srb.BridgingName, su.ServiceUnitName),
                Mode = "instance",
                Telecom = telecom,
                Type = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType>()
                {
                    new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType()
                    {
                        Coding = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding>()
                        {
                            new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding()
                            {
                                System = "http://terminology.kemkes.go.id/CodeSystem/location-type",
                                Code = "RT0004",
                                Display = "Tempat Tidur"
                            }
                        }
                    }
                },
                PhysicalType = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PhysicalType()
                {
                    Coding = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding>()
                    {
                        new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Coding()
                        {
                            System = "http://terminology.hl7.org/CodeSystem/location-physical-type",
                            Code = "bd",
                            Display = "Bed",
                        }
                    },
                },
                Position = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.Position()
                {
                    Longitude = 1,
                    Latitude = 1,
                    Altitude = 1,
                },
                ManagingOrganization = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.ManagingOrganization()
                {
                    Reference = String.Concat("Organization/", OrganizationID),
                },
                partOf = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.partOf()
                {
                    Reference = String.Concat("Location/", srb.BridgingID),
                    Display = srb.BridgingName
                }
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            var response = RestClientPost(requestBody, "Location", ref accessToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var resp = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Master.Location.PostPutResponse>(response.Content);
                //bb = new Bed();
                bb.SatuSehatBridgingID = resp.Id;
                bb.SatuSehatBridgingName = resp.Name;
                bb.Save();
            }
            return response;
        }

        #endregion

        #region ILP SATUSEHAT

        #region ANC
        // 2.1	EpisodeOfCare - Saat Registrasi ANC Pertama Kali
        private object EpisodeOfCareANCPostData(Registration reg, PatientBridging patSs)
        {
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                statusHistory = new List<object> {
                    new {
                        status = "active",
                        period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                type = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                code = "CAD",
                                display = "Antenatal Care"
                            }
                        }
                    }
                },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            };

            return postData;
        }

        // 2.4	Encounter - Kunjungan Baru
        private EncounterPost CreateNewEncounterANCPostData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterANCId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/puerperium", Value = "KF3"
                }
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterANCId)
            };
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.StatusHistory.Add(new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        // 2.5	Encounter - Masuk Ruang
        private EncounterPost EncounterVisitANCPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterANCId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.ID = encounterANCId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/puerperium", Value = "KF3"
                }
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterANCId)
            };
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        // 3.1	Observation - HPHT
        private object ObservationPostDataFDOLM(PatientBridging patSs, ParamedicBridging parMedicSs, string encounterANCId)
        {
            var lastmens = new PatientField();
            lastmens.LoadByPrimaryKey(patSs.PatientID, 1);

            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                {
                    new
                    {
                        system = "http://loinc.org",
                        code = "8665-2",
                        display = "Last menstrual period start date"
                    },
                        new
                        {
                            system = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            code = "ANC.B6.DE14",
                            display = "Last menstrual period (LMP) date"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterANCId)
                },
                effectiveDateTime = string.Format("{0}+00:00", lastmens.DataDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", lastmens.DataDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                valueDateTime = string.Format("{0}+00:00", lastmens.ValueInDatetime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            return postData;
        }

        // 3.2	Observation - BB Sebelum Hamil
        private object ObservationPostDataBBBP(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BodyWeight, DateTime.Now);
            var vitalSignDateTime = vitalSign.RecordDateTime;

            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);

            var pat2 = new Patient();
            pat2.LoadByMedicalNo(reg.RegistrationNo);

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "56077-1",
                            Display = "Body weight --pre current pregnancy"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B8.DE2",
                            Display = "Pre-gestational weight"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    Value = vitalSign.Value,
                    Unit = "kg",
                    System = "http://unitsofmeasure.org",
                    Code = "kg"
                }
            };

            return postData;
        }

        // 3.3	Observation - Tinggi Badan
        private object ObservationPostDataTB(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BodyHeight, DateTime.Now);
            var vitalSignDateTime = vitalSign.RecordDateTime;

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "8302-2",
                            Display = "Body height"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B8.DE1",
                            Display = "Height"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    Value = vitalSign.Value,
                    Unit = "cm",
                    System = "http://unitsofmeasure.org",
                    Code = "cm"
                }
            };

            return postData;
        }

        // 3.4	Immunization - Riwayat Imunisasi TT0

        // 3.5	Immunization - Riwayat Imunisasi TT1 - TT5

        // 3.6	Immunization - Pemberian Imunisasi TT1 - TT5

        // 4.1	Observation - Berat Badan
        private object ObservationPostDataBB(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BodyWeight, DateTime.Now);
            var vitalSignDateTime = vitalSign.RecordDateTime;

            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);

            var pat2 = new Patient();
            pat2.LoadByMedicalNo(reg.RegistrationNo);

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "29463-7",
                            Display = "Body weight"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B8.DE3",
                            Display = "Current weight"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    Value = vitalSign.Value,
                    Unit = "kg",
                    System = "http://unitsofmeasure.org",
                    Code = "kg"
                }
            };

            return postData;
        }

        // 4.2	Observation - Lingkar Lengan Atas (LILA)

        // 4.3	Observation - Tinggi Fundus

        // 4.4	Observation - Tekanan Darah Sistolik
        private object ObservationPostDataSistolik(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BloodPressureSistolic, DateTime.Now);
            var vitalSignDateTime = vitalSign.RecordDateTime;

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "8480-6",
                            Display = "Systolic blood pressure"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B8.DE17",
                            Display = "Systolic blood pressure"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    Value = vitalSign.Value,
                    Unit = "mm[Hg]",
                    System = "http://unitsofmeasure.org",
                    Code = "mm[Hg]"
                }
            };

            return postData;
        }

        // 4.5	Observation - Tekanan Darah Diastolik
        private object ObservationPostDataDiastolik(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, VitalSign.VitalSignEnum.BloodPressureDiastolic, DateTime.Now);
            var vitalSignDateTime = vitalSign.RecordDateTime;

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "8462-4",
                            Display = "Diastolic blood pressure"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B8.DE19",
                            Display = "Diastolic blood pressure"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    Value = vitalSign.Value,
                    Unit = "mm[Hg]",
                    System = "http://unitsofmeasure.org",
                    Code = "mm[Hg]"
                }
            };

            return postData;
        }

        // 4.6	Observation - Golongan Darah
        private object ObservationPostGoldar(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("BloodType", pat.SRBloodType);

            var codeValue = asri.ItemName == "A" ? "LA19710-5" : asri.ItemName == "B" ? "LA19709-7" : asri.ItemName == "O" ? "LA19708-9" : asri.ItemName == "AB" ? "LA28449-9" : "";
            var displayValue = asri.ItemName == "A" ? "Group A" : asri.ItemName == "B" ? "Group B" : asri.ItemName == "O" ? "Group O" : asri.ItemName == "AB" ? "AB" : "";
            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "laboratory",
                                Display = "Laboratory"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "883-9",
                            Display = "ABO group [Type] in Blood"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B9.DE24",
                            Display = "Blood type"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                Issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    System = "http://loinc.org",
                    Code = codeValue,
                    Display = displayValue
                }
            };

            return postData;
        }

        // 4.7	Observation - Rhesus
        private object ObservationPostDataRhesus(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterANCId)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var codeValue = pat.BloodRhesus == "-" ? "LA6577-6" : pat.BloodRhesus == "+" ? "LA6576-8" : "LA4489-6";
            var displayValue = pat.BloodRhesus == "-" ? "Negative" : pat.BloodRhesus == "+" ? "Positive" : "Unknown";

            var postData = new
            {
                ResourceType = "Observation",
                Status = "final",
                Category = new List<Category>
                {
                    new Category
                    {
                        Coding = new List<Coding>
                        {
                            new Coding
                            {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code = "vital-signs",
                                Display = "Vital Signs"
                            }
                        }
                    }
                },
                Code = new Code
                {
                    Coding = new List<Coding>
                    {
                        new Coding
                        {
                            System = "http://loinc.org",
                            Code = "10331-7",
                            Display = "Rh [Type] in Blood"
                        },
                        new Coding
                        {
                            System = "http://fhir.org/guides/who/anc-cds/CodeSystem/anc-custom-codes",
                            Code = "ANC.B9.DE29",
                            Display = "Rh factor"
                        }
                    }
                },
                Subject = new RefAndDisplay
                {
                    Reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                Encounter = new RefAndDisplay
                {
                    Reference = string.Format("Encounter/{0}", encounterANCId)
                },
                EffectiveDateTime = FormatDateLong(reg.RegistrationDate.Value), // string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormat)),
                Issued = FormatDateLong(reg.RegistrationDate.Value), //string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormat)),
                Performer = new List<RefAndDisplay>
                {
                    new RefAndDisplay
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                ValueQuantity = new ValueQuantity
                {
                    System = "http://loinc.org",
                    Code = codeValue,
                    Display = displayValue
                }
            };

            return postData;
        }

        // 4.8	Observation - Denyut Jantung Janin

        // 4.9	Observation - Presentasi Janin

        // 5.1	Procedure - Create Konseling/Edukasi
        private void ProcedurePostDataEducation(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Procedure", "Education", "Nutrition");
            if (ssResult != null && ssResult.ResultID != null) return;

            var edu = new PatientEducationLine();
            edu.Query.es.Top = 1;
            edu.Query.Where(edu.Query.RegistrationNo == reg.RegistrationNo, edu.Query.SRPatientEducation == "004"); //PatientEducation	004	Diet dan nutrisi
            if (!edu.Query.Load() || string.IsNullOrWhiteSpace(edu.EducationNotes)) return;

            var pract = LoadPerformer(edu.LastUpdateByUserID, parMedSs.ParamedicID);

            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new
                {
                    coding = new List<object>() { new
                    {
                        system = "http://snomed.info/sct",
                        code = "409073007",
                        display = "Education"
                    }
                }
                },
                code = new
                {
                    coding = new List<object>() { new
                    {
                        system = "http://snomed.info/sct",
                        code = "61310001",
                        display = "Nutrition education"
                    }
                }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", edu.LastUpdateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)), //"2023 - 08 - 31T03: 30:00 + 00:00",
                    end = string.Format("{0}+00:00", edu.LastUpdateDateTime.Value.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong)) //"2023 - 08 - 31T03: 40:00 + 00:00"
                },
                performer = new List<object>() { new
                    {
                    actor = new {
                                reference = string.Format( "Practitioner/{0}",pract.BridgingID),
                                display = pract.BridgingName
                            }
                        }
                    }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "Education",
                    Code = "Nutrition"
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Procedure", requestBody, ssResult, ref accessToken);
        }

        // 6.1	ServiceRequest - Create

        // 6.2	Specimen - Create - Darah

        // 6.3	Specimen - Create - Urin

        // 6.4	Observation - Pemeriksaan Hemoglobin

        // 6.5	Observation - Skrining PPIA HIV

        // 6.6	Observation - Skrining PPIA Sifilis (RPR)

        // 6.7	Observation - Skrining PPIA Sifilis (VDRL)

        // 6.8	Observation - Skrining PPIA Hepatitis B	Observation - Skrining PPIA Hepatitis B

        // 6.9	Observation - Pemeriksaan Gula Darah Sewaktu

        // 6.10	Observation - Pemeriksaan Protein Urine

        // 6.11	DiagnosticReport - Pemeriksaan Hemoglobin

        // 6.12	DiagnosticReport - Skrining PPIA HIV

        // 6.13	DiagnosticReport - Skrining PPIA Sifilis(RPR)

        // 6.14	DiagnosticReport - Skrining PPIA Sifilis(VDRL)

        // 6.15	DiagnosticReport - Skrining PPIA Hepatitis B

        // 6.17	DiagnosticReport - Pemeriksaan Gula Darah Sewaktu

        // 6.18	DiagnosticReport - Pemeriksaan Protein Urin

        // 6.19	ServiceRequest - Create - For SATUSEHAT

        // 6.20	ServiceRequest - Create - For MWL di dalam DICOM Router

        // 6.21	ImagingStudy - Get ID based on Accession Number

        // 6.22	Observation - GS

        // 6.23	Observation - CRL

        // 6.24	Observation - DJJ

        // 6.25	DiagnosticReport - Create

        // 7.1	Condition - Primary Tuberculosis
        private object ConditionANCPostData(PatientBridging patSs, DateTime createDateTime, string encounterANCId)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "A15.0",
                                display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterANCId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10
            };

            return postData;
        }

        // 8.1	Procedure - Tindakan/Prosedur Medis pada USG kehamilan

        // 8.2	Procedure - Tindakan/Prosedur Medis pada Nebulisasi

        // 9.1	MedicationRequest - Create

        // 9.1	MedicationDispense - Create

        // 10.1	ServiceRequest - Kontrol kembali
        private object ServiceRequestKontrolKembali(PatientBridging patSs, ParamedicBridging parSs, Registration reg, string encounterANCId)
        {
            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var reff = new ReferExternal();
            reff.LoadByPrimaryKey(reg.RegistrationNo);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("ReferReason", reff.SRReferReason);

            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("RefferalType", reff.SRReferType, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    },
                    text = asri.ItemName
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterANCId),
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                },
                occurrenceDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                    Display = parSs.BridgingName
                },
                performer = new List<object>() { new
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "A15.0",
                                display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                            }
                        },
                        text = asri.ItemName
                    }
                },
                patientInstruction = reff.OtherInformation
            };

            return postData;
        }

        // 10.2	ServiceRequest - Rujukan ke Rawat Inap di Faskes Lain dengan Ambulans
        private object ServiceRequestRujukanRawatInap(PatientBridging patSs, ParamedicBridging parSs, Registration reg, string encounterANCId)
        {
            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var reff = new ReferExternal();
            reff.LoadByPrimaryKey(reg.RegistrationNo);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("ReferReason", reff.SRReferReason);

            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("RefferalType", reff.SRReferType, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    },
                    text = asri.ItemName
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterANCId),
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                },
                occurrenceDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                    Display = parSs.BridgingName
                },
                performer = new List<object>() { new
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "A15.0",
                                display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                            }
                        }
                    }
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "HOSP",
                                display = "Hospital"
                            },
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "AMB",
                                display = "Ambulance"
                            }
                        }
                    }
                },
                patientInstruction = reff.OtherInformation
            };

            return postData;
        }

        // 11.1	Condition - Stabil
        private object ConditionANCPostData(PatientBridging patSs, Registration reg, string encounterANCId)
        {
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("DischargeCondition", reg.SRDischargeCondition, SatuSehatBridgingType);

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>() {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>() { new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterANCId),
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                }
            };

            return postData;
        }

        // 12.1	Encounter - Update (Pulang dan Kontrol Kembali)
        private EncounterFinishPut DischargeMethodPNCPutDataKontrol(PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, Registration reg, string encounterANCId, string MainDiagnoseANCId)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = encounterANCId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = OrganizationID
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/ANC", Value = "K3"
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterANCId)
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.Diagnosis = new List<Diagnosis>();
            postData.Diagnosis.Insert(0, new Diagnosis()
            {
                Condition = new Condition()
                {
                    Reference = string.Format("Condition/{0}", MainDiagnoseANCId),
                    Display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                },
                Use = new Use()
                {
                    Coding = new List<Coding>
                    {
                        new Coding()
                        {
                            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                            Code = "DD",
                            Display = "Discharge diagnosis"
                        }
                    }
                },
                Rank = 1
            });
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            //hospitalization kontrol
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        // 12.2	Encounter - Update(Rujukan)
        private EncounterFinishPut DischargeMethodPNCPutDataRujukan(PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, Registration reg, string encounterANCId, string MainDiagnoseANCId)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = encounterANCId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = OrganizationID
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/ANC", Value = "K3"
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterANCId)
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.Diagnosis = new List<Diagnosis>();
            postData.Diagnosis.Insert(0, new Diagnosis()
            {
                Condition = new Condition()
                {
                    Reference = string.Format("Condition/{0}", MainDiagnoseANCId),
                    Display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                },
                Use = new Use()
                {
                    Coding = new List<Coding>
                    {
                        new Coding()
                        {
                            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                            Code = "DD",
                            Display = "Discharge diagnosis"
                        }
                    }
                },
                Rank = 1
            });
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // belum di set
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // belum di set
                }
            });
            //hospitalization rujukan
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        #endregion

        #region INC
        //2.4 kunjungan ibu
        private object EncounterPostDataINC(Registration reg, PatientBridging patSs, string episodeOfCareANCId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs, string serviceReqID)
        {
            reg.IsParturition = true;
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.EpisodeOfCare = new ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareANCId)
            };
            postData.Status = "in-progress";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "IMP",
                Display = "inpatient encounter"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() { new Coding()
                            {
                                System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                Code = "ATND",
                                Display = "attender"
                            } };
            var types = new List<Code>()
                            {new Code(){ Coding= codings}  };


            var par = new Paramedic();
            par.LoadByPrimaryKey(reg.ParamedicID);
            postData.Participant = new List<Participant>() {
                                    new Participant(){Individual= new Individual(){ Reference= string.Format("Practitioner/{0}",parMedicSs.BridgingID),
                        Display= parMedicSs.BridgingName}, Type = types } };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference= string.Format("Location/{0}",locSs.BridgingID),
                        Display= locSs.BridgingName
                    }
                }
            };

            // StatusHistory
            postData.StatusHistory = new List<StatusHistory>();
            var regTimes = reg.RegistrationTime.Split(':');
            var arrivedTime = reg.RegistrationDate.Value;
            arrivedTime = new DateTime(arrivedTime.Year, arrivedTime.Month, arrivedTime.Day, regTimes[0].ToInt(),
                regTimes[1].ToInt(), 0);

            var startInprogressTime = arrivedTime;
            var finishedTime = arrivedTime;


            // Jam dipanggil
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Descending);
            if (pa.Query.Load())
            {
                if (arrivedTime > pa.AssessmentDateTime.Value)
                    arrivedTime = reg.LastCreateDateTime.Value;

                startInprogressTime = pa.AssessmentDateTime.Value;
            }
            else
                startInprogressTime = arrivedTime.AddMinutes(5);

            // selesai ketika diberi resep
            var presc = new TransPrescription();
            presc.Query.Where(presc.Query.RegistrationNo == reg.RegistrationNo, presc.Query.IsApproval == true);
            presc.Query.es.Top = 1;
            presc.Query.OrderBy(presc.Query.PrescriptionDate.Descending);
            if (presc.Query.Load())
            {
                if (startInprogressTime > presc.CreatedDateTime.Value)
                {
                    startInprogressTime = presc.CreatedDateTime.Value.AddMinutes(-1);
                }

                postData.StatusHistory.Add(new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", presc.CreatedDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                });

            }

            // arrived
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", startInprogressTime.AddHours(GmtDif).ToString(DateFormatLong))
                }
            });


            // Period
            postData.Period = new Period() { Start = string.Format("{0}+00:00", arrivedTime.AddHours(GmtDif).ToString(DateFormatLong)) }; //"2022-06-14T07:00:00+07:00"

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            postData.BasedOn = new ServiceProvider()
            {
                Reference = String.Format("ServiceRequest/{0}", serviceReqID)
            };

            return postData;
        }

        //patch location

        //endpatch
        //3.1 tanggal jam persalinan
        private void PostObservationINC(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId, string accessToken, string type)
        {
            var code = string.Empty;

            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Observation", "survey", code);
            if (ssResult != null && ssResult.ResultID != null) return;

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult();
                ssResult.EncounterID = new Guid(encounterId);
                ssResult.ResourceType = "Observation";
                ssResult.Category = "survey";
                ssResult.Code = code;
            }

            string errorMessage = string.Empty;
            var observationPostData = ObservationPostDataINC(reg, patSs, ref parMedicSs, encounterId, ref errorMessage, type);

            if (!string.IsNullOrEmpty(errorMessage) || observationPostData == null)
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = errorMessage;
                ssResult.Save();
                return;
            }

            var requestBody = JsonConvert.SerializeObject(observationPostData);
            RestClientPostAndSaveLog("Observation", requestBody, ssResult, ref accessToken);
        }
        private object ObservationPostDataINC(Registration reg, PatientBridging patSs, ref ParamedicBridging parMedicSs, string encounterINCId, ref string errorMessage, string type)
        {
            var mother = new Patient();
            mother.LoadByPrimaryKey(patSs.PatientID);
            var child = new Patient();
            child.Query.Where(child.Query.MotherMedicalNo == mother.MedicalNo);
            child.Query.es.Top = 1;
            child.Query.Load();
            List<object> coding = null;
            object value = null;
            if (type == "PostDate")
            {
                if (!child.DateOfBirth.HasValue)
                {
                    errorMessage = string.Format("Date of birth is empty. Please fill in the date of birth before proceeding.");
                    return null;
                }
                coding = new List<object> {
                    new {
                        system = "http://loinc.org",
                        code = "93857-1",
                        display = "Date and time of obstetric delivery"
                    }
                };
                value = new
                {
                    valueDateTime = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).ToString(DateFormatLong))
                };
            }
            else if (type == "PostDeliveryMethod")
            {
                var pbr = new PatientBirthRecord();
                pbr.LoadByPrimaryKey(patSs.PatientID);
                var asrib = new AppStandardReferenceItemBridging();
                asrib.LoadByPrimaryKey("BirthMethod", pbr.SRBirthMethod, SatuSehatBridgingType);
                if (!string.IsNullOrEmpty(pbr.SRBirthMethod))
                {
                    errorMessage = string.Format("Birth method is empty. Please fill in the date of birth before proceeding.");
                    return null;
                }
                coding = new List<object> {
                    new {
                        system = "http://loinc.org",
                        code = "57071-3",
                        display = "Obstetric delivery method"
                    }
                };
                value = new
                {
                    valueCodeableConcept = new
                    {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = asrib.BridgingID,
                                display = asrib.BridgingName
                            }
                        }
                    }
                };
            }
            else if (type == "PostDeliveryLocation")
            {
                //var pbr = new PatientBirthRecord();
                //pbr.LoadByPrimaryKey(patSs.PatientID);
                //var asrib = new AppStandardReferenceItemBridging();
                //asrib.LoadByPrimaryKey("BirthMethod", pbr.SRBirthMethod, _satuSehatBridgingType);
                //if (!string.IsNullOrEmpty(pbr.SRBirthMethod))
                //{
                //    errorMessage = string.Format("Birth method is empty. Please fill in the date of birth before proceeding.");
                //    return null;
                //}
                coding = new List<object> {
                    new {
                        system = "http://loinc.org",
                        code = "72150-6",
                        display = "Delivery location"
                    }
                };
                value = new
                {
                    valueCodeableConcept = new
                    {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.kemkes.go.id/CodeSystem/organization-type",
                                code = "104",
                                display = "Rumah Sakit"
                            }
                        }
                    }
                };
            }
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterINCId)
                },
                effectiveDateTime = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object> {
                    new {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                value
            };
            return postData;
        }

        //4.3 Episode of Care Nifas ( dipindah ke method PostEpisodeOfCare)
        //private object EpisodeofCarePostDataPNC(Registration reg, PatientBridging patSs)
        //{
        //    var postData = new
        //    {
        //        resourceType = "EpisodeOfCare",
        //        identifier = new List<object> {
        //            new {
        //                system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", _organizationID),
        //                value = _organizationID
        //            }
        //        },
        //        status = "active",
        //        statusHistory = new List<object> {
        //            new {
        //                status = "active",
        //                period = new {
        //                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),

        //                }
        //            }
        //        },
        //        type = new List<object> {
        //            new {
        //                coding = new List<object> {
        //                    new {
        //                        system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
        //                        code = "PNC",
        //                        display = "Postnatal Care"
        //                    }
        //                }
        //            }
        //        },
        //        patient = new
        //        {
        //            reference = string.Format("Patient/{0}", patSs.BridgingID),
        //            display = patSs.BridgingName
        //        },
        //        managingOrganization = new
        //        {
        //            reference = string.Format("Organization/{0}", _organizationID)
        //        },
        //        period = new
        //        {
        //            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
        //        }
        //    };
        //    return postData;
        //}

        // 4.4cara persalinan (tanggalnya perbaiki) // pindah ke ObservationPostDataINC
        //private object ObservationMethodPostDataINC(Registration reg, PatientBridging patSs, string encounterINCId, ref ParamedicBridging parMedicSs)
        //{
        //    //var postData = new
        //    //{
        //    //    resourceType = "Observation",
        //    //    status = "final",
        //    //    category = new List<object> {
        //    //        new {
        //    //            coding = new List<object> {
        //    //                new {
        //    //                    system = "http://terminology.hl7.org/CodeSystem/observation-category",
        //    //                    code = "survey",
        //    //                    display = "Survey"
        //    //                }
        //    //            }
        //    //        }
        //    //    },
        //    //    code = new
        //    //    {
        //    //        coding = new List<object> {
        //    //            new {
        //    //                system = "http://loinc.org",
        //    //                code = "57071-3",
        //    //                display = "Obstetric delivery method"
        //    //            }
        //    //        }
        //    //    },
        //    //    subject = new
        //    //    {
        //    //        reference = string.Format("Patient/{0}", patSs.BridgingID),
        //    //        display = patSs.BridgingName
        //    //    },
        //    //    encounter = new
        //    //    {
        //    //        reference = string.Format("Encounter/{0}", encounterINCId)
        //    //    },
        //    //    effectiveDateTime = string.Format("{0}+00:00", DateTime.Parse("2015-10-02T03:04:00").ToString(_dateFormat)),
        //    //    issued = string.Format("{0}+00:00", DateTime.Parse("2015-10-02T03:04:00").ToString(_dateFormat)),
        //    //    performer = new List<object> {
        //    //        new {
        //    //            reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
        //    //        }
        //    //    },
        //    //    valueCodeableConcept = new
        //    //    {
        //    //        coding = new List<object> {
        //    //            new {
        //    //                system = "http://snomed.info/sct",
        //    //                code = "48782003",
        //    //                display = "Delivery normal"
        //    //            }
        //    //        }
        //    //    }
        //    //};
        //    return postData;
        //}

        //4.5 encounter pnc (tanggalnya perbaiki)
        private object EnconterPutDataPNC(Registration reg, PatientBridging patSs, string encounterINCId, string episodeCareANCId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs, DateTime encounterDate)
        {
            var putData = new EncounterPost();
            putData.ResourceType = "Encounter";
            putData.ID = encounterINCId;
            putData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeCareANCId)
            };
            putData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            putData.Status = "in-progress";
            putData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "IMP",
                Display = "inpatient encounter"
            };
            putData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>()
            {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };

            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };

            putData.Participant = new List<Participant>()
            {
                new Participant()
                {
                    Type = types,
                    Individual = new Individual()
                    {
                        Reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                        Display = parMedicSs.BridgingName
                    }
                }
            };

            putData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong))
            };

            putData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}", locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", encounterDate.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };

            putData.StatusHistory = new List<StatusHistory>()
            {
                new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", encounterDate.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };

            putData.ServiceProvider = new ServiceProvider()
            {
                Reference = string.Format("Organization/{0}", OrganizationID)
            };

            return putData;
        }

        //4.6 create patient newborn
        private object PatientNewBornPostDataINC(Patient mother, Patient childData)
        {

            var postData = new
            {
                resourceType = "Patient",
                meta = new
                {
                    profile = new List<string> {
                        "https://fhir.kemkes.go.id/r4/StructureDefinition/Patient"
                    }
                },
                identifier = new List<object>
                {
                    new
                    {
                        use = "official",
                        system = "https://fhir.kemkes.go.id/id/nik-ibu",
                        value = mother.Ssn
                    }
                },
                active = true,
                name = new List<object>
                {
                    new
                    {
                        use = "official",
                        text = string.Format("{0} {1} {2}", childData.FirstName, childData.MiddleName, childData.LastName).Trim()
                    }
                },
                telecom = new List<object>
                {
                    new
                    {
                        system = "phone",
                        value = mother.MobilePhoneNo,
                        use = "mobile"
                    },
                    new
                    {
                        system = "phone",
                        value = mother.PhoneNo,
                        use = "home"
                    },
                    new
                    {
                        system = "email",
                        value = mother.Email,
                        use = "home"
                    }
                },
                gender = childData.Sex,
                birthDate = childData.DateOfBirth,
                deceasedBoolean = false,
                address = new List<object>
                {
                    new
                    {
                        use = "home",
                        line = new List<string>
                        {
                            mother.StreetName
                        },
                        city = mother.City,
                        postalCode = mother.ZipCode,
                        country = mother.County,
                        extension = new List<object>
                        {
                            new
                            {
                                url = "https://fhir.kemkes.go.id/r4/StructureDefinition/administrativeCode",
                                extension = new List<object>
                                {
                                    new { url = "province", valueCode = "31" },
                                    new { url = "city", valueCode = "3171" },
                                    new { url = "district", valueCode = "317106" },
                                    new { url = "village", valueCode = "3171061001" },
                                    new { url = "rt", valueCode = "2" },
                                    new { url = "rw", valueCode = "2" }
                                }
                            }
                        }
                    }
                },
                maritalStatus = new
                {
                    coding = new List<object>
                {
                    new
                    {
                        system = "http://terminology.hl7.org/CodeSystem/v3-MaritalStatus",
                        code = "U",
                        display = "Unmarried"
                    }
                },
                    text = "Unmarried"
                },
                multipleBirthInteger = 0,
                contact = new List<object>
                {
                    new
                    {
                        relationship = new List<object>
                        {
                            new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/v2-0131",
                                        code = "C"
                                    }
                                }
                            }
                        },
                        name = new
                        {
                            use = "official",
                            text = string.Format("{0} {1} {2}", mother.FirstName, mother.MiddleName, mother.LastName).Trim()
                        },
                        telecom = new List<object>
                        {
                            new
                            {
                                system = "phone",
                                value = "0690383372",
                                use = "mobile"
                            }
                        }
                    }
                },
                communication = new List<object>
                {
                    new
                    {
                        language = new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "urn:ietf:bcp:47",
                                    code = "id-ID",
                                    display = "Indonesian"
                                }
                            },
                            text = "Indonesian"
                        },
                        preferred = true
                    }
                },
                extension = new List<object>
                {
                    new
                    {
                        url = "https://fhir.kemkes.go.id/r4/StructureDefinition/birthPlace",
                        valueAddress = new
                        {
                            city = childData.CityOfBirth,
                            country = "ID"
                        }
                    },
                    new
                    {
                        url = "https://fhir.kemkes.go.id/r4/StructureDefinition/citizenshipStatus",
                        valueCode = "I"
                    }
                }
            };
            return postData;
        }
        //4.7 eoc neonatus //pindah ke EpisodeOfCarePostData
        //private object EpisodeofCarePostDataNeonatus(Registration reg, PatientBridging patSs)
        //{
        //    var pat = new Patient();
        //    pat.LoadByPrimaryKey(patSs.PatientID);
        //    var postData = new
        //    {
        //        resourceType = "EpisodeOfCare",
        //        identifier = new List<object>
        //        {
        //            new
        //            {
        //                system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", _organizationID),
        //                value = _organizationID
        //            }
        //        },
        //        status = "active",
        //        statusHistory = new List<object>
        //        {
        //            new
        //            {
        //                status = "active",
        //                period = new
        //                {
        //                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
        //                }
        //            }
        //        },
        //        type = new List<object>
        //        {
        //            new
        //            {
        //                coding = new List<object>
        //                {
        //                    new
        //                    {
        //                        system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
        //                        code = "Neonate",
        //                        display = "Neonate"
        //                    }
        //                }
        //            }
        //        },
        //        patient = new
        //        {
        //            reference = string.Format("Patient/{0}", patSs.BridgingID),
        //            display = patSs.BridgingName
        //        },
        //        managingOrganization = new
        //        {
        //            reference = string.Format("Organization/{0}", _organizationID)
        //        },
        //        period = new
        //        {
        //            start = string.Format("{0}+00:00", pat.DateOfBirth.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
        //        }

        //    };
        //    return postData;
        //}

        //4.8 kunjungan bayi
        private object EncounterPostBirthData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, ServiceUnitBridging locSs, string episodeOfCareNeoId)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.EpisodeOfCare = new ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
            };
            postData.Status = "in-progress";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "IMP",
                Display = "inpatient encounter"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() { new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                        Code = "ATND",
                        Display = "attender"
                    } };
            var types = new List<Code>()
                    {new Code(){ Coding= codings}  };
            postData.Participant = new List<Participant>()
            {
                                    new Participant()
                                    {
                                        Individual= new Individual()
                                        {
                                            Reference= string.Format("Practitioner/{0}",parMedicSs.BridgingID),
                                            Display= parMedicSs.BridgingName
                                        },
                                        Type = types
                                    }
            };
            postData.Period = new Period() { Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) };
            //location
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)))
                    }
                }
            };

            postData.StatusHistory.Add(new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)))
                }
            });

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        //4,9 Observation Lokasi
        private object ObservationPostDataLocBirth(PatientBridging patSs, ParamedicBridging parMedicSs, string encounterAnakINCId, ref ServiceUnitBridging locSs)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);

            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = "72150-6",
                            display = "Delivery location"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", pat.PatientID),
                    display = string.Format("{0} {1} {2}", pat.FirstName, pat.MiddleName, pat.LastName).Trim()
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId)
                },
                effectiveDateTime = string.Format("{0}+00:00", pat.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", pat.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                valueCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.kemkes.go.id/CodeSystem/organization-type",
                            code = locSs.BridgingID,
                            display = locSs.BridgingName
                        }
                    }
                },

            };
            return postData;
        }

        //4.10 berat badan bayi
        private object BirthWeightPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterAnakINCId, ref ServiceUnitBridging locSs)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var pbr = new PatientBirthRecord();
            pbr.LoadByPrimaryKey(patSs.PatientID);
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "vital-signs",
                                display = "Vital Signs"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code =  "8339-4",
                            display = "Birth weight Measured"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                valueQuantity = new
                {
                    value = pbr.Weight,
                    unit = "g",
                    system = "http://unitsofmeasure.org",
                    code = "g"
                },
                interpretation = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "276613009",
                                display = "High birth weight"
                            }
                        },
                        text = "BBLB (Bayi Berat Lahir Besar)"
                    }
                }

            };
            return postData;
        }
        //4.11 (pastikan lagi waktunya)
        private object EncounterEnteringRoomPutData(Registration reg, PatientBridging patSs, string encounterAnakINCId, string episodeOfCareNeoId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs)
        {
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            pa.Query.Load();
            var putData = new
            {
                resourceType = "Encounter",
                id = encounterAnakINCId,
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "in-progress",
                _class = new
                {
                    system = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                    code = "IMP",
                    display = "inpatient encounter"
                },
                episodeOfCare = new List<object>
                {
                    new
                    {
                        reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                participant = new List<object>
                {
                    new
                    {
                        type = new List<object>
                        {
                            new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                        code = "ATND",
                                        display = "attender"
                                    }
                                }
                            }
                        },
                        individual = new
                        {
                            reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                            display = parMedicSs.BridgingName
                        }
                    }
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                location = new List<object>
                {
                    new
                    {
                        location = new
                        {
                            reference = string.Format("Location/{0}", locSs.BridgingID),
                            display = locSs.BridgingName
                        },
                        period = new
                        {
                            start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                            end = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddMinutes(15).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    },
                    new
                    {
                        location = new
                        {
                            reference = string.Format("Location/{0}", locSs.BridgingID),
                            display = locSs.BridgingName
                        },
                        period = new
                        {
                            start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddMinutes(15).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                statusHistory = new List<object>
                {
                    new
                    {
                        status = "in-progress",
                        period = new
                        {
                            start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddMinutes(15).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                serviceProvider = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                }

            };

            return putData;
        }

        //05.Diagnosis
        //5.1 Moderate Pre-Eclampsia
        private object MomDiagnosisPostDataINC(PatientBridging patSs, string encounterINCId, EpisodeDiagnose ed)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var pbr = new PatientBirthRecord();
            pbr.LoadByPrimaryKey(patSs.PatientID);
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = ed.DiagnoseID,
                            display = ed.DiagnoseName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterINCId)
                },
                onsetDateTime = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recordedDate = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                note = new List<object>
                {
                    new
                    {
                        text = ed.Notes
                    }
                }

            };
            return postData;
        }

        //5.4 Primary Mild
        private object ChildDiagnosisPostDataINC(PatientBridging patSs, string encounterAnakINCId, EpisodeDiagnose ed)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var asri = new AppStandardReferenceItem();
            asri.Query.Where(asri.Query.StandardReferenceID == "Salutation" && asri.Query.ItemID == pat.SRSalutation);
            asri.Query.es.Top = 1;
            asri.Query.Load();
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = ed.DiagnoseID,
                            display = ed.DiagnoseName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId)
                },
                onsetDateTime = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recordedDate = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                note = new List<object>
                {
                    new
                    {
                        text = string.Format("Bayi {0} {1} mengalami {2}",asri.ItemName , patSs.BridgingName, ed.DiagnoseName)
                    }
                }
            };
            return postData;
        }

        //6.1 Procedure Delivery (PERLU PENYESUAIAN DELIVERY BAYI)
        private object ProcedureMDSDeliveryPostData(PatientBridging patSs, ref ParamedicBridging parMedicSs, MedicalDischargeSummaryDiagnose mdsd, MedicalDischargeSummaryProcedure mdsp, string encounterINCId)
        {
            //var mdsdColl = new MedicalDischargeSummaryDiagnoseCollection();
            //mdsdColl.Query.Where(mdsdColl.Query.RegistrationNo == reg.RegistrationNo, mdsdColl.Query.IsVoid == false);
            //mdsdColl.LoadAll();
            //var mdspColl = new MedicalDischargeSummaryProcedureCollection();
            //mdspColl.Query.Where(mdspColl.Query.RegistrationNo == reg.RegistrationNo, mdspColl.Query.IsVoid == false);
            //mdspColl.LoadAll();
            //var postDataProcedure = ProcedureMDSDeliveryPostData(patSs, parMedicSs, mdsd, mdsp, encounterINCId); foreach pemanggilan tiap Diagnose/Procedure (Danang)
            var setRecordDate = mdsd.CreatedDateTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            var setRecordEndDate = mdsd.CreatedDateTime.Value.AddMinutes(20).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(setRecordDate);
            var formattedDeliveryDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var pbr = new PatientBirthRecord();
            pbr.LoadByPrimaryKey(patSs.PatientID);
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("BirthMethod", pbr.SRBirthMethod, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "277132007",
                            display = "Therapeutic procedure"
                        }
                    },
                    text = "Therapeutic procedure"
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-9-cm",
                            code = mdsp.ProcedureID,
                            display = mdsp.ProcedureName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterINCId),
                    display = string.Format("Tindakan Persalinan {0} pada tanggal {1}", patSs.BridgingName, formattedDeliveryDate)
                },
                performedPeriod = new
                {
                    start = setRecordDate,
                    end = setRecordEndDate
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                            display = parMedicSs.BridgingName
                        }
                    }
                },
                reasonCode = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = mdsd.DiagnoseID,
                                display = mdsd.DiagnoseName
                            }
                        }
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = string.Format("Persalinan {0}",asrib.BridgingName)
                    }
                }
            };
            return postData;
        }

        //6.2 Procedure anak
        private object ProcedureMDSNonMechanicalPostData(PatientBridging patSs, ref ParamedicBridging parMedicSs, MedicalDischargeSummaryDiagnose mdsd, string encounterAnakINCId)
        {
            var setRecordDate = string.Format("{0}+00:00", mdsd.CreatedDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong));
            var setRecordEndDate = string.Format("{0}+00:00", mdsd.CreatedDateTime.Value.AddMinutes(20).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong));
            DateTime parsedDate = DateTime.Parse(setRecordDate);
            var formattedDeliveryDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "373110003",
                            display = "Emergency procedure"
                        }
                    },
                    text = "Emergency procedure"
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-9-cm",
                            code = mdsd.DiagnoseID,
                            display = mdsd.DiagnoseName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId),
                    display = string.Format("Tindakan Resusitasi {0} pada tanggal {1}", patSs.BridgingName, formattedDeliveryDate)
                },
                performedPeriod = new
                {
                    start = setRecordDate,
                    end = setRecordEndDate
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                            display = parMedicSs.BridgingName
                        }
                    }
                },
                reasonCode = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = mdsd.DiagnoseID,
                                display = mdsd.DiagnoseName
                            }
                        }
                    }
                },
                bodySite = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "123851003",
                                display = "Mouth region structure"
                            },
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "45206002",
                                display = "Nasal structure"
                            }
                        }
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = "Pemberian resusitasi neonatus melalui mulut dan hidung."
                    }
                }
            };
            return postData;
        }

        //8.1 Refer ibu
        private object ServiceRequestMomPostData(Registration reg, PatientBridging patSs, string encounterINCId, ParamedicConsultRefer pcr, EpisodeDiagnose ed)
        {
            //var pcr = new ParamedicConsultRefer();
            //pcr.Query.Where(pcr.Query.RegistrationNo == reg.RegistrationNo);
            //pcr.Query.Load();
            //var postDataServiceRequestMom = ServiceRequestMotherPostData(patSs, encounterINCId, pcr);
            var fromPar = new ParamedicBridging();
            fromPar.Query.Where(fromPar.Query.ParamedicID == pcr.ParamedicID);
            fromPar.Query.Load();

            var toPar = new ParamedicBridging();
            toPar.Query.Where(toPar.Query.ParamedicID == pcr.ToParamedicID);
            toPar.Query.Load();

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = reg.RegistrationNo
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "185389009",
                            display = "Follow-up visit"
                        }
                    },
                    text = "Pemeriksaan lanjutan pasca melahirkan"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterINCId),
                    display = string.Format("Kunjungan Melahirkan {0}, {1}", patSs.BridgingName, formatVisitDate)
                },
                occurrenceDateTime = string.Format("{0}+00:00", pcr.ConsultDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", fromPar.BridgingID),
                    display = fromPar.BridgingName
                },
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", toPar.BridgingID),
                        display = toPar.BridgingName
                    }
                },
                reasonCode = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = ed.DiagnoseID,
                                display = ed.DiagnoseName
                            }
                        },
                        text = string.Format("Pemeriksaan lanjutan {0} pasca melahirkan", ed.DiagnoseName),
                    }
                },
                patientInstruction = string.Format("Pemeriksaan lanjutan {0} pasca melahirkan", ed.DiagnoseName),
            };
            return postData;
        }


        //8.2 Refer Anak
        private object ServiceRequestChildPostData(Registration reg, PatientBridging patSs, string encounterAnakINCId, ParamedicConsultRefer pcr, MedicalDischargeSummaryDiagnose mdsd)
        {
            //var pcr = new ParamedicConsultRefer();
            //pcr.Query.Where(pcr.Query.RegistrationNo == reg.RegistrationNo);
            //pcr.Query.Load();
            //var mdsdColl = new MedicalDischargeSummaryDiagnoseCollection();
            //mdsdColl.Query.Where(mdsdColl.Query.RegistrationNo == reg.RegistrationNo, mdsdColl.Query.IsVoid == false);
            //mdsdColl.LoadAll();
            //ServiceRequestChildPostData(patSs, encounterINCId, pcr, mdsd);
            var fromPar = new ParamedicBridging();
            fromPar.Query.Where(fromPar.Query.ParamedicID == pcr.ParamedicID);
            fromPar.Query.Load();

            var toPar = new ParamedicBridging();
            toPar.Query.Where(toPar.Query.ParamedicID == pcr.ToParamedicID);
            toPar.Query.Load();

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },

                status = "active",
                intent = "original-order",
                priority = "routine",

                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },

                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "185389009",
                            display = "Follow-up visit"
                        }
                    },
                    text = "Pemeriksaan lanjutan pasca lahir"
                },

                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },

                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId),
                    display = string.Format("Kunjungan {0} {1}", patSs.BridgingName, formatVisitDate)
                },

                occurrenceDateTime = string.Format("{0}+00:00", pcr.ConsultDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),

                requester = new
                {
                    reference = string.Format("Practitioner/{0}", fromPar.BridgingID),
                    display = fromPar.BridgingName
                },

                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", toPar.BridgingID),
                        display = toPar.BridgingName
                    }
                },

                reasonCode = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = mdsd.DiagnoseID,
                                display = mdsd.DiagnoseName
                            }
                        },
                        text = string.Format("Pemeriksaan lanjutan {0} pasca lahir", mdsd.DiagnoseName)
                    }
                },

                patientInstruction = string.Format("Pemeriksaan lanjutan {0} pasca lahir", mdsd.DiagnoseName)
            };
            return postData;
        }

        //9.1 Kondisi Ibu
        private object ConditionMomPostData(Registration reg, PatientBridging patSs, MedicalDischargeSummary mds, string encounterINCId)
        {
            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("DischargeCondition", reg.SRDischargeCondition, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterINCId),
                    display = string.Format("Kunjungan {0} pada {1}", patSs.BridgingName, formatVisitDate)
                }
            };
            return postData;
        }

        //9.2 Kondisi Bayi
        private object ConditionChildPostData(Registration reg, PatientBridging patSs, string encounterAnakINCId)
        {
            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("DischargeCondition", reg.SRDischargeCondition, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterAnakINCId),
                    display = string.Format("Kunjungan {0} pada {1}", patSs.BridgingName, formatVisitDate)
                }
            };
            return postData;
        }
        //10.Encounter update INC Ibu
        private EncounterFinishPut EncounterUpdateINCMomPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterINCId, string episodeOfCareANCId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            //postData.ResourceType = "Encounter";
            //postData.ID = encounterINCId;
            //var mds = new MedicalDischargeSummary();
            //mds.LoadByPrimaryKey(reg.RegistrationNo);
            //var pa = new PatientAssessment();
            //pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            //pa.Query.es.Top = 1;
            //pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            //pa.Query.Load();
            //postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            //{
            //    Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareANCId)
            //};
            //postData.Identifier = new List<Identifier>()
            //{
            //    new Identifier() {
            //        System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",_organizationID),
            //        Value = _organizationID
            //    }
            //};
            //postData.Status = "finished";
            //postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            //{
            //    System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
            //    Code = "IMP",
            //    Display = "inpatient encounter"
            //};
            //postData.Subject = new RefAndDisplay()
            //{
            //    Reference = string.Format("Patient/{0}", patSs.BridgingID),
            //    Display = patSs.BridgingName
            //};
            //var codings = new List<Coding>() {
            //    new Coding()
            //    {
            //        System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
            //        Code = "ATND",
            //        Display = "attender"
            //    }
            //};
            //var types = new List<Code>()
            //{
            //    new Code() { Coding= codings }
            //};
            //postData.Participant = new List<Participant>() {
            //    new Participant() {
            //        Type = types,
            //        Individual= new Individual() {
            //            Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
            //            Display = parSs.BridgingName
            //        }
            //    }
            //};
            //postData.Period = new Period()
            //{
            //    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).ToString(_dateFormat)),
            //    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).ToString(_dateFormat))
            //};

            //postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            //{
            //    new Bridging.SatuSehat.BusinessObject.Location()
            //    {
            //        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
            //        {
            //            Reference = string.Format("Location/{0}",locSs.BridgingID),
            //            Display = locSs.BridgingName
            //        },
            //        Period = new Period()
            //        {
            //            Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).ToString(_dateFormat))), //belum tau darimana
            //            End = string.Format("{0}+00:00", string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).ToString(_dateFormat))) //belum tau darimana
            //        }
            //    }
            //};
            //var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            //var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag1.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", primaryDiagnose),
            //    Display = "Moderate Pre-Eclampsia"
            //};
            //diag1.Rank = 1;
            //diag1.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag1);

            //// Diagnosis 2
            //var diag2 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag2.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", secondaryDiagnose),
            //    Display = "Assisted single delivery, unspecified"
            //};
            //diag2.Rank = 2;
            //diag2.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag2);

            //// Diagnosis 3
            //var diag3 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag3.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", tertiaryDiagnose),
            //    Display = "Single Live Birth"
            //};
            //diag3.Rank = 3;
            //diag3.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag3);
            //postData.Diagnosis = diags;
            //postData.StatusHistory = new List<StatusHistory>();
            //postData.StatusHistory.Insert(0, new StatusHistory()
            //{
            //    Status = "in-progress",
            //    Period = new Period()
            //    {
            //        Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
            //        End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
            //    }
            //});
            //postData.StatusHistory.Insert(1, new StatusHistory()
            //{
            //    Status = "finished",
            //    Period = new Period()
            //    {
            //        Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
            //        End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
            //    }
            //});
            //var coding = new List<Coding>() {
            //    new Coding() {
            //        System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
            //        Code = "home",
            //        Display = "Home"
            //    }
            //};
            //var dischargeDisposition = new DischargeDisposition()
            //{
            //    Coding = coding,
            //    Text = "Anjuran dokter untuk pulang dan kontrol kembali"
            //};
            //var hospitalization = new Hospitalization()
            //{
            //    DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            //};
            //postData.Hospitalization = hospitalization;
            //postData.ServiceProvider = new ServiceProvider()
            //{
            //    Reference = String.Format("Organization/{0}", _organizationID)
            //};
            return postData;
        }

        //10.2 Encounter update bayi
        private EncounterFinishPut EncounterUpdateINCChildPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterAnakINCId, string episodeOfCareNeoId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            //postData.ResourceType = "Encounter";
            //postData.ID = encounterAnakINCId;
            //var mds = new MedicalDischargeSummary();
            //mds.LoadByPrimaryKey(reg.RegistrationNo);
            //var pa = new PatientAssessment();
            //pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            //pa.Query.es.Top = 1;
            //pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            //pa.Query.Load();
            //postData.Identifier = new List<Identifier>()
            //{
            //    new Identifier() {
            //        System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",_organizationID),
            //        Value = _organizationID
            //    }
            //};
            //postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            //{
            //    Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
            //};
            //postData.Status = "finished";
            //postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            //{
            //    System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
            //    Code = "IMP",
            //    Display = "inpatient encounter"
            //};
            //postData.Subject = new RefAndDisplay()
            //{
            //    Reference = string.Format("Patient/{0}", patSs.BridgingID),
            //    Display = patSs.BridgingName
            //};
            //var codings = new List<Coding>() {
            //    new Coding()
            //    {
            //        System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
            //        Code = "ATND",
            //        Display = "attender"
            //    }
            //};
            //var types = new List<Code>()
            //{
            //    new Code() { Coding= codings }
            //};
            //postData.Participant = new List<Participant>() {
            //    new Participant() {
            //        Type = types,
            //        Individual= new Individual() {
            //            Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
            //            Display = parSs.BridgingName
            //        }
            //    }
            //};
            //postData.Period = new Period()
            //{
            //    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
            //    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
            //};

            //postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            //{
            //    new Bridging.SatuSehat.BusinessObject.Location()
            //    {
            //        LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
            //        {
            //            Reference = string.Format("Location/{0}",locSs.BridgingID),
            //            Display = locSs.BridgingName
            //        },
            //        Period = new Period()
            //        {
            //            Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))),
            //            End = string.Format("{0}+00:00", string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)))
            //        }
            //    }
            //};
            //var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            //var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag1.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", primaryDiagnose),
            //    Display = "Mild and moderate birth asphyxia"
            //};
            //diag1.Rank = 1;
            //diag1.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag1);

            //// Diagnosis 2
            //var diag2 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag2.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", secondaryDiagnose),
            //    Display = "Exceptionally large baby"
            //};
            //diag2.Rank = 2;
            //diag2.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag2);

            //// Diagnosis 3
            //var diag3 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            //diag3.Condition = new Condition()
            //{
            //    Reference = string.Format("Condition/{0}", tertiaryDiagnose),
            //    Display = "Singleton, born in hospital"
            //};
            //diag3.Rank = 3;
            //diag3.Use = new Use()
            //{
            //    Coding = new List<Coding>
            //    {
            //        new Coding()
            //        {
            //            System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //            Code = "DD",
            //            Display = "Discharge diagnosis"
            //        }
            //    }
            //};
            //diags.Add(diag3);
            //postData.Diagnosis = diags;
            //postData.StatusHistory = new List<StatusHistory>();
            //postData.StatusHistory.Insert(0, new StatusHistory()
            //{
            //    Status = "in-progress",
            //    Period = new Period()
            //    {
            //        Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
            //        End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
            //    }
            //});
            //postData.StatusHistory.Insert(1, new StatusHistory()
            //{
            //    Status = "finished",
            //    Period = new Period()
            //    {
            //        Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat)),
            //        End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(_gmtDif).AddHours(_gmtDif).ToString(_dateFormat))
            //    }
            //});
            //var coding = new List<Coding>() {
            //    new Coding() {
            //        System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
            //        Code = "home",
            //        Display = "Home"
            //    }
            //};
            //var dischargeDisposition = new DischargeDisposition()
            //{
            //    Coding = coding,
            //    Text = "Anjuran dokter untuk pulang dan kontrol kembali"
            //};
            //var hospitalization = new Hospitalization()
            //{
            //    DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            //};
            //postData.Hospitalization = hospitalization;
            //postData.ServiceProvider = new ServiceProvider()
            //{
            //    Reference = String.Format("Organization/{0}", _organizationID)
            //};
            return postData;
        }
        #endregion

        #region PNC
        // 2.1 Kunjungan Saat Pertama Kali PNC
        private object EpisodeOfCarePNCPostData(Registration reg, PatientBridging patSs)
        {
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                statusHistory = new List<object> {
                    new {
                        status = "active",
                        period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                type = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                code = "PNC",
                                display = "Postnatal Care"
                            }
                        }
                    }
                },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            };

            return postData;
        }

        // 2.3 Pembuatan Kunjungan Baru
        private EncounterPost CreateNewEncounterPNCPostData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterPNCId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/puerperium", Value = "KF3"
                }
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterPNCId)
            };
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.StatusHistory.Add(new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        // 2.4 Masuk ke Ruangan Pemeriksaan
        private EncounterPost EncounterVisitPNCPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterPNCId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.ID = encounterPNCId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID), Value = reg.RegistrationNo
                },
                new Identifier() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare/puerperium", Value = "KF3"
                }
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", encounterPNCId)
            };
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };

            var startDtmProgress = reg.RegistrationDate.Value;
            var patAssess = FirstPatientAssessment(reg.RegistrationNo);
            if (patAssess != null)
                startDtmProgress = (DateTime)patAssess.AssessmentDateTime;

            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", startDtmProgress.AddMinutes(6).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        // 3.1 Observation - Tanggal dan Jam Persalinan
        private object ObservationLabourPNCPostData(PatientBridging patSs, ParamedicBridging parSs, Registration reg, string encounterPNCId)
        {
            var mother = new Patient();
            mother.LoadByPrimaryKey(patSs.PatientID);
            var child = new PatientBirthRecord();
            child.Query.Where(child.Query.MotherMedicalNo == mother.MedicalNo);
            child.Query.es.Top = 1;
            child.Query.Load();
            var childData = new Patient();
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://loinc.org",
                            code = "93857-1",
                            display = "Date and time of obstetric delivery"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object> {
                    new {
                        Practitioner = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                valueDateTime = string.Format("{0}+00:00", childData.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            return postData;
        }

        // 4.1 Condition - Tuberculosis complicating pregnancy, childbirth and the puerperium
        private object MainDiagnosePNCPostData(PatientBridging patSs, string encounterPNCId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "O98.0",
                            display = "Tuberculosis complicating pregnancy, childbirth and the puerperium"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10
            };

            return postData;
        }

        // 4.2 Condition - Secondary Malnutrisi Ringan
        private object SecondaryDiagnosePNCPostData(PatientBridging patSs, string encounterPNCId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "E44.1",
                            display = "Mild protein-calorie malnutrition"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10
            };

            return postData;
        }

        // 5.1 Procedure - Routine Gynocological Exam
        private object ProcedurePNCPostData(PatientBridging patSs, ParamedicBridging parSs, string encounterPNCId)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object>() { new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "103693007",
                                display = "Diagnostic procedure"
                            }
                        },
                        text = "Diagnostic procedure"
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                            system = "http://hl7.org/fhir/sid/icd-9-cm",
                            code = "72.31",
                            display = "Routine gynecological examination"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", DateTime.Now.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                            display = parSs.BridgingName
                        }
                    }
                }
            };

            return postData;
        }

        // 7.1 Service - Rujukan
        private object ReferPNCPostData(PatientBridging patSs, ParamedicBridging parSs, Registration reg, string encounterPNCId)
        {
            var reff = new ReferExternal();
            reff.LoadByPrimaryKey(reg.RegistrationNo);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("ReferReason", reff.SRReferReason);

            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("RefferalType", reff.SRReferType, SatuSehatBridgingType);

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    },
                    text = asri.ItemName
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId),
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                },
                occurrenceDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                    Display = parSs.BridgingName
                },
                performer = new List<object>() { new
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "O98.0",
                                display = "Tuberculosis complicating pregnancy, childbirth and the puerperium"
                            }
                        },
                        text = asri.ItemName
                    }
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "HOSP",
                                display = "Hospital"
                            }
                        }
                    }
                },
                patientInstruction = reff.OtherInformation
            };

            return postData;
        }

        // 8.1 Condition - Stabil
        private object ConditionPNCPostData(PatientBridging patSs, Registration reg, string encounterPNCId)
        {
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("DischargeCondition", reg.SRDischargeCondition, SatuSehatBridgingType);

            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>() {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>() { new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPNCId),
                    display = $"Kunjungan {patSs.BridgingName} Pada {formatVisitDate}"
                }
            };

            return postData;
        }

        // 9.2 Encounter - Pulang dan Kontrol Kembali
        private EncounterFinishPut DischargeMethodPNCPutData(PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, Registration reg, string encounterPNCId, string MainDiagnosePNCId, string SecondaryDiagnosePNCId)
        {
            var encounterPostData = EncounterFinishPutData(reg, patSs, parSs, locSs, new DataTable(), encounterPNCId, "", "PNC");
            return encounterPostData;

            //var postData = new EncounterFinishPut();
            //postData.Diagnosis = new List<Diagnosis>();
            //postData.Diagnosis.Insert(0, new Diagnosis()
            //{
            //    Condition = new Condition()
            //    {
            //        Reference = string.Format("Condition/{0}", MainDiagnosePNCId),
            //        Display = "Tuberculosis complicating pregnancy, childbirth and the puerperium"
            //    },
            //    Use = new Use() 
            //    { 
            //        Coding = new List<Coding> 
            //        { 
            //            new Coding() 
            //            { 
            //                System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //                Code = "DD", 
            //                Display = "Discharge diagnosis"
            //            } 
            //        } 
            //    },
            //    Rank = 1
            //});
            //postData.Diagnosis.Insert(1, new Diagnosis()
            //{
            //    Condition = new Condition()
            //    {
            //        Reference = string.Format("Condition/{0}", SecondaryDiagnosePNCId),
            //        Display = "Mild protein-calorie malnutrition"
            //    },
            //    Use = new Use()
            //    {
            //        Coding = new List<Coding>
            //        {
            //            new Coding()
            //            {
            //                System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
            //                Code = "DD",
            //                Display = "Discharge diagnosis"
            //            }
            //        }
            //    },
            //    Rank = 2
            //});

            //return postData;
        }
        #endregion

        #region NEONATUS
        //Kunjungan pertama Neonatus
        private object EpisodeOfCarePostDataNeo(Registration reg, PatientBridging patSs)
        {
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> { new {
                    system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                    value = OrganizationID}},
                status = "active",
                statusHistory = new List<object> { new {
                    status = "active",
                    period = new {
                        start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        } }
                    },
                type = new List<object> { new {
                    coding = new List<object> { new {
                        system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                        code = "Neonate",
                        display = "Neonate"
                            } }
                    } },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new { reference = string.Format("Organization/{0}", OrganizationID) },
                period = new { start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) }

            };
            return postData;
        }

        //Pembuatan Kunjungan Baru
        private EncounterPost EncounterNewPostDataNeo(Registration reg, PatientBridging patSs, string episodeOfCareNeoId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            { new Identifier() {
                System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                Value = reg.RegistrationNo}};
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>()
            {
                new Coding()
                {
                    System= "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code= "ATND",
                    Display= "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding = codings}
            };
            postData.Participant = new List<Participant>()
            {
                new Participant()
                {
                    Type = types,
                    Individual = new Individual()
                    {
                        Reference = string.Format("Practitioner/{0}",parMedicSs.BridgingID),
                        Display = parMedicSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.StatusHistory.Add(new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = string.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        //Masuk ke Ruang Pmeriksaan
        private EncounterPost EncounterPutDataNeo(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string episodeOfCareNeoId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareNeoId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "in-progress";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))

                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        // Observation Jam Lahir Bayi/Balita
        private object ObservationBirthTimePostDataNeo(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareNeoId)
        {
            var mother = new Patient();
            mother.LoadByPrimaryKey(patSs.PatientID);
            var child = new Patient();
            child.Query.Where(child.Query.MotherMedicalNo == mother.MedicalNo);
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                code = "57715-5",
                                display = "Birth time",
                                system = "http://loinc.org"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId)
                },
                effectiveDateTime = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object> {
                    new {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                    }
                },
                valueTime = string.Format("{0}+00:00", child.DateOfBirth.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            return postData;
        }

        // Skrining PPIA : SHK
        private object ProcedureSHKPostDataNeo(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareNeoId)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "103693007",
                                display = "Diagnostic procedure"
                            }
                        },
                        text = "Diagnostic procedure"
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "400984005",
                                display = "Congenital hypothyroidism screening test"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", DateTime.Now.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                            display = parSs.BridgingName
                        }
                    }
                }
            };
            return postData;
        }

        // Observation Asi Eksklusif
        private object ObservationASIPostDataNeo(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareNeoId)
        {
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "1145307003",
                                display = "Exclusively breastfed"
                            }
                        }
                    }
                },
                performer = new
                {
                    reference = string.Format("Practitioner/{0}", parSs.BridgingID)
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId),
                    display = string.Format("Pemeriksaan {0} di tanggal {1}", patSs.BridgingName, reg.RegistrationDate)
                },
                effectiveDateTime = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                valueBoolean = true
            };
            return postData;
        }

        // Condition - Primary Asfiksia Sedang
        private object PrimaryAsfiksiaPostDataNeo(PatientBridging patSs, string episodeOfCareNeoId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "P21.1",
                                display = "Mild and moderate birth asphyxia"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                note = new List<object>
                {
                    new
                    {
                        text = "Pasien mengalami Asfiksia Sedang"
                    }
                }
            };
            return postData;
        }

        // Condition - Secondary Respiratory Failure
        private object SecondaryRespiratoryPostDataNeo(PatientBridging patSs, string episodeOfCareNeoId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "P28.5",
                                display = "Respiratory  failure of newborn"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10                
            };
            return postData;
        }

        // Procedure - resuscitation neonatus
        private object ProcedureResuscitationPostDataNeo(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareNeoId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "373110003",
                                display = "Emergency procedure"
                            }
                        },
                        text = "Emergency procedure"
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-9-cm",
                                code = "93.93",
                                display = "Nonmechanical methods of resuscitation"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId),
                    display = string.Format("Tindakan Resusitasi {0} pada tanggal {1}", patSs.BridgingName, createDateTime)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", DateTime.Now.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                            display = parSs.BridgingName
                        }
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "P21.1",
                                display = "Mild and moderate birth asphyxia"
                            }
                        }
                    }
                },
                bodySite = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "123851003",
                                display = "Mouth region structure"
                            },
                            new {
                                system = "http://snomed.info/sct",
                                code = "45206002",
                                display = "Nasal structure"
                            }
                        }
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = "Pemberian resusitasi neonatus melalui mulut dan hidung."
                    }
                }
            };
            return postData;
        }

        // ServiceRequest - Rujukan Keluar Faskes
        private object ServiceRequestPostDataNeo(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareNeoId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object> { new {
                    system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                    value = OrganizationID}},
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "737492002",
                                display = "Outpatient care management"
                            }
                        },
                        text = "Pemeriksaan lanjutan asfiksia"
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId),
                    display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, createDateTime)
                },
                occurrenceDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                requester = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "P21.1",
                                display = "Mild and moderate birth asphyxia"
                            }
                        },
                        text = "Pemeriksaan lanjutan asfiksia"
                    }
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "AMB",
                                display = "Ambulance"
                            }
                        },
                        text = "Pemeriksaan lanjutan asfiksia"
                    }
                },
                patientInstruction = "Rujukan ke RSUP Fatmawati.Dalam keadaan darurat dapat menghubungi hotline Fasyankes di nomor 14045"
            };
            return postData;
        }

        // Condition - Stabil
        private object ConditionStabilPostDataNeo(Registration reg, PatientBridging patSs, string episodeOfCareNeoId)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "359746009",
                                display = "Patient's condition stable"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareNeoId),
                    display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, reg.RegistrationDate)
                }
            };
            return postData;
        }

        // Cara keluar - Encounter - Update (Pulang dan Kontrol Kembali)
        private EncounterFinishPut EncounterUpdateDischargeNeo(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterAnakINCId, string episodeOfCareNeoId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareNeoId;
            var mds = new MedicalDischargeSummary();
            mds.LoadByPrimaryKey(reg.RegistrationNo);
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            pa.Query.Load();
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = OrganizationID
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.EpisodeOfCare = new Bridging.SatuSehat.BusinessObject.ServiceProvider()
            {
                Reference = string.Format("EpisodeOfCare/{0}", episodeOfCareNeoId)
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag1.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", primaryDiagnose),
                Display = "Mild and moderate birth asphyxia"
            };
            diag1.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag1.Rank = 1;
            diags.Add(diag1);
            // Diagnosis 2
            var diag2 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag2.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", secondaryDiagnose),
                Display = "Respiratory  failure of newborn"
            };
            diag2.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag2.Rank = 2;
            diags.Add(diag2);
            postData.Diagnosis = diags;
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                    Code = "home",
                    Display = "Home"
                }
            };
            var dischargeDisposition = new DischargeDisposition()
            {
                Coding = coding,
                Text = "Anjuran dokter untuk pulang dan kontrol kembali 3 hari setelah Kelahiran"
            };
            var hospitalization = new Hospitalization()
            {
                DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            };
            postData.Hospitalization = hospitalization;
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        #endregion

        #region TUMBUH KEMBANG
        // Pembuatan Kunjungan Baru
        private object EncounterPostDataTK(Registration reg, PatientBridging patSs, string episodeOfCareTKId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs, string serviceReqID)
        {
            reg.IsParturition = true;
            var postData = new
            {
                resourceType = "Encounter",
                identifier = new List<object> {
                new {
                    system = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                    value = OrganizationID
                    }
                },
                status = "arrived",
                _class = new
                {
                    system = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                    code = "AMB",
                    display = "ambulatory"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                participant = new List<object> {
                new {
                    type = new List<object> {
                        new {
                            coding = new List<object> {
                                new {
                                    system = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                                    code = "ATND",
                                    display = "attender"
                                }
                            }
                        }
                    },
                    individual = new {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                        display = parMedicSs.BridgingName
                        }
                    }
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                location = new List<object> {
                new {
                    location = new {
                            Reference= string.Format("Location/{0}",locSs.BridgingID),
                            Display= locSs.BridgingName
                        },
                    period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                statusHistory = new List<object> {
                new {
                    status = "arrived",
                    period = new {
                        start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                        }
                    }
                },
                serviceProvider = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                }
            };
            return postData;
        }

        //Masuk ke Ruang Pemeriksaan
        private EncounterPost EncounterPutDataTK(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string episodeOfCareTKId)
        {
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareTKId;

            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "in-progress";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };

            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.ConfirmedAttendanceDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))

                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        //03. Antropometri - Observation - Berat Badan
        private ObservationPost ObservationWeightTKPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, VitalSign.VitalSignEnum vitalSignEnum, string episodeOfCareTKId, ref string errorMessage)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, vitalSignEnum, DateTime.Now);
            if (vitalSign.Value == 0)
            {
                errorMessage = "zero_value";
                return null;
            }

            string vitalSignCode = String.Empty;
            string vitalSignDisplay = String.Empty;
            var valueQuantity = new ValueQuantity();
            var vitalSignDateTime = vitalSign.RecordDateTime;
            List<Interpretation> interpretation = null;


            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.BodyWeight:
                    {
                        vitalSignCode = "29463-7";
                        vitalSignDisplay = "Body weight";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "kg", System = "http://unitsofmeasure.org", Code = "kg" };

                        break;
                    }
                default:
                    break;
            }

            var postData = new ObservationPost();
            postData.ResourceType = "Observation";
            postData.Identifier = new List<Identifier>()
            { new Identifier() {
                System = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                Value = OrganizationID}};
            postData.Status = "final";
            postData.Category = new List<Category>() { new Category()
            {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code= "vital-signs",
                                Display= "Vital Signs"
                            }
                            }
            }};
            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://loinc.org",
                        Code = vitalSignCode,
                        Display = vitalSignDisplay
                    }
             }
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", episodeOfCareTKId),
                Display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, vitalSignDateTime.ToString("dd MMM yyyy"))
            };
            // YYYY-MM-DDThh:mm:ss+00:00
            postData.EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            postData.Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            var performer = LoadPerformerByUserID(vitalSign.ByUserID);
            if (performer == null)
            {
                errorMessage = string.Format("Performer not found, please setting Satusehat bridging ID for User Paramedic [{0}] first", vitalSign.ByUserID);
                return null;
            }

            postData.Performer = new List<RefAndDisplay>(){ new RefAndDisplay()
            {
                Reference = string.Format("Practitioner/{0}", performer.BridgingID),
                Display = performer.BridgingName
            }};
            postData.ValueQuantity = valueQuantity;

            return postData;

        }
        //03. Antropometri - Observation - Tinggi Badan Telentang/Panjang Badan
        private ObservationPost ObservationHeightLyingTKPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, VitalSign.VitalSignEnum vitalSignEnum, string episodeOfCareTKId, ref string errorMessage)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, vitalSignEnum, DateTime.Now);
            if (vitalSign.Value == 0)
            {
                errorMessage = "zero_value";
                return null;
            }

            string vitalSignCode = String.Empty;
            string vitalSignDisplay = String.Empty;
            var valueQuantity = new ValueQuantity();
            var vitalSignDateTime = vitalSign.RecordDateTime;
            List<Interpretation> interpretation = null;


            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.BodyHeight:
                    {
                        vitalSignCode = "8306-3";
                        vitalSignDisplay = "Body height --lying";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "cm", System = "http://unitsofmeasure.org", Code = "cm" };

                        break;
                    }
                default:
                    break;
            }

            var postData = new ObservationPost();
            postData.ResourceType = "Observation";
            postData.Identifier = new List<Identifier>()
            { new Identifier() {
                System = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                Value = OrganizationID}};
            postData.Status = "final";
            postData.Category = new List<Category>() { new Category()
            {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code= "vital-signs",
                                Display= "Vital Signs"
                            }
                            }
            }};
            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://loinc.org",
                        Code = vitalSignCode,
                        Display = vitalSignDisplay
                    }
             }
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", episodeOfCareTKId),
                Display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, vitalSignDateTime.ToString("dd MMM yyyy"))
            };
            // YYYY-MM-DDThh:mm:ss+00:00
            postData.EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            postData.Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            var performer = LoadPerformerByUserID(vitalSign.ByUserID);
            if (performer == null)
            {
                errorMessage = string.Format("Performer not found, please setting Satusehat bridging ID for User Paramedic [{0}] first", vitalSign.ByUserID);
                return null;
            }

            postData.Performer = new List<RefAndDisplay>(){ new RefAndDisplay()
            {
                Reference = string.Format("Practitioner/{0}", performer.BridgingID),
                Display = performer.BridgingName
            }};
            postData.ValueQuantity = valueQuantity;

            return postData;

        }
        //03. Antropometri - Observation - Tinggi Badan Berdiri
        private ObservationPost ObservationHeightTKPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, VitalSign.VitalSignEnum vitalSignEnum, string episodeOfCareTKId, ref string errorMessage)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, vitalSignEnum, DateTime.Now);
            if (vitalSign.Value == 0)
            {
                errorMessage = "zero_value";
                return null;
            }

            string vitalSignCode = String.Empty;
            string vitalSignDisplay = String.Empty;
            var valueQuantity = new ValueQuantity();
            var vitalSignDateTime = vitalSign.RecordDateTime;
            List<Interpretation> interpretation = null;


            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.BodyHeight:
                    {
                        vitalSignCode = "8308-9";
                        vitalSignDisplay = "Body height --standing";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "cm", System = "http://unitsofmeasure.org", Code = "cm" };

                        break;
                    }
                default:
                    break;
            }

            var postData = new ObservationPost();
            postData.ResourceType = "Observation";
            postData.Identifier = new List<Identifier>()
            { new Identifier() {
                System = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                Value = OrganizationID}};
            postData.Status = "final";
            postData.Category = new List<Category>() { new Category()
            {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code= "vital-signs",
                                Display= "Vital Signs"
                            }
                            }
            }};
            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://loinc.org",
                        Code = vitalSignCode,
                        Display = vitalSignDisplay
                    }
             }
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", episodeOfCareTKId),
                Display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, vitalSignDateTime.ToString("dd MMM yyyy"))
            };
            // YYYY-MM-DDThh:mm:ss+00:00
            postData.EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            postData.Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            var performer = LoadPerformerByUserID(vitalSign.ByUserID);
            if (performer == null)
            {
                errorMessage = string.Format("Performer not found, please setting Satusehat bridging ID for User Paramedic [{0}] first", vitalSign.ByUserID);
                return null;
            }

            postData.Performer = new List<RefAndDisplay>(){ new RefAndDisplay()
            {
                Reference = string.Format("Practitioner/{0}", performer.BridgingID),
                Display = performer.BridgingName
            }};
            postData.ValueQuantity = valueQuantity;

            return postData;

        }
        //03. Antropometri - Observation - Lingkar Kepala (LK
        private ObservationPost ObservationHeadCircumferenceTKPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, VitalSign.VitalSignEnum vitalSignEnum, string episodeOfCareTKId, ref string errorMessage)
        {
            var vitalSign = VitalSign.LastVitalSignItem(reg.RegistrationNo, reg.FromRegistrationNo, vitalSignEnum, DateTime.Now);
            if (vitalSign.Value == 0)
            {
                errorMessage = "zero_value";
                return null;
            }

            string vitalSignCode = String.Empty;
            string vitalSignDisplay = String.Empty;
            var valueQuantity = new ValueQuantity();
            var vitalSignDateTime = vitalSign.RecordDateTime;
            List<Interpretation> interpretation = null;


            switch (vitalSignEnum)
            {
                case VitalSign.VitalSignEnum.HeadCircumference:
                    {
                        vitalSignCode = "9843-4";
                        vitalSignDisplay = "Head Occipital-frontal circumference";
                        valueQuantity = new ValueQuantity() { Value = vitalSign.Value.ToInt(), Unit = "cm", System = "http://unitsofmeasure.org", Code = "cm" };

                        break;
                    }
                default:
                    break;
            }

            var postData = new ObservationPost();
            postData.ResourceType = "Observation";
            postData.Identifier = new List<Identifier>()
            { new Identifier() {
                System = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                Value = OrganizationID}};
            postData.Status = "final";
            postData.Category = new List<Category>() { new Category()
            {
                            Coding = new List<Coding>() { new Coding() {
                                System = "http://terminology.hl7.org/CodeSystem/observation-category",
                                Code= "vital-signs",
                                Display= "Vital Signs"
                            }
                            }
            }};
            postData.Code = new Code()
            {
                Coding = new List<Coding>(){ new Coding()
                    {
                        System = "http://loinc.org",
                        Code = vitalSignCode,
                        Display = vitalSignDisplay
                    }
             }
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            postData.Encounter = new RefAndDisplay()
            {
                Reference = String.Format("Encounter/{0}", episodeOfCareTKId),
                Display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, vitalSignDateTime.ToString("dd MMM yyyy"))
            };
            // YYYY-MM-DDThh:mm:ss+00:00
            postData.EffectiveDateTime = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            postData.Issued = string.Format("{0}+00:00", vitalSignDateTime.AddHours(GmtDif).ToString(DateFormatLong));
            var performer = LoadPerformerByUserID(vitalSign.ByUserID);
            if (performer == null)
            {
                errorMessage = string.Format("Performer not found, please setting Satusehat bridging ID for User Paramedic [{0}] first", vitalSign.ByUserID);
                return null;
            }

            postData.Performer = new List<RefAndDisplay>(){ new RefAndDisplay()
            {
                Reference = string.Format("Practitioner/{0}", performer.BridgingID),
                Display = performer.BridgingName
            }};
            postData.ValueQuantity = valueQuantity;

            return postData;

        }
        //04. QuestionnaireResponse - Stimulasi, Deteksi, dan Intervensi Dini Tumbuh Kembang (SDIDTK)


        //05. Condition - Gangguan Tumbuh Kembang
        private object MainDiagnoseTKPostData(PatientBridging patSs, string episodeOfCareTKId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "R62.9",
                            display = "Lack of expected normal physiologic development unspecified"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10
            };

            return postData;
        }

        //05.Condition - Tuberkulosis
        private object SecondaryDiagnoseTKPostData(PatientBridging patSs, string episodeOfCareTKId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "A15.7",
                            display = "Primary respiratory tuberculosis, confirmed bacteriologically and histologically"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)) // tarik dari record date pengisian icd 10
            };

            return postData;
        }
        //06.Procedure - Terapetik - Nebulisasi
        private object ProcedureTKTeraPostData(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareTKId)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object>() { new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "277132007",
                                display = "Therapeutic procedure"
                            }
                        },
                        text = "Therapeutic procedure"
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                            system = "http://hl7.org/fhir/sid/icd-9-cm",
                            code = "93.74",
                            display = "Speech defect training"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId),
                    display = string.Format("Tindakan terapi wicara {0} pada tanggal {1}}", patSs.BridgingName, DateTime.Now)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", DateTime.Now.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                            display = parSs.BridgingName
                        }
                    }
                },
                ReasonCode = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "R62.9",
                            display = "Lack of expected normal physiologic development"
                        }
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = "Terapi wicara untuk masalah tumbuh kembang anak"
                    }
                }
            };

            return postData;
        }
        //06.Procedure - Counselling
        private object ProcedureTKCounsellingPostData(PatientBridging patSs, ParamedicBridging parSs, string episodeOfCareTKId)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object>() { new
                    {
                        coding = new List<object>()
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "409063005",
                                display = "Counselling"
                            }
                        },
                        text = "Counselling"
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                            system = "http://hl7.org/fhir/sid/icd-9-cm",
                            code = "94.4",
                            display = "Other psychotherapy and counselling"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId),
                    display = string.Format("Tindakan terapi wicara {0} pada tanggal {1}}", patSs.BridgingName, DateTime.Now)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", DateTime.Now.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", DateTime.Now.AddMinutes(5).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                },
                performer = new List<object>
                {
                    new
                    {
                        actor = new
                        {
                            reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                            display = parSs.BridgingName
                        }
                    }
                },
                ReasonCode = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = "A15.0",
                            display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                        }
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = "Konseling keresahan pasien karena diagnosis TB"
                    }
                }
            };

            return postData;
        }

        // 08.Service Request - Rujukan/kontrol
        private object ServiceRequestPostDataTK(PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string episodeOfCareTKId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object> { new {
                    system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                    value = OrganizationID}},
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "185389009",
                                display = "Follow-up visit"
                            }
                        },
                        text = "Kontrol rutin regimen TB bulan ke-2"
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId),
                    display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, createDateTime)
                },
                occurrenceDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                authoredOn = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                requester = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "A15.0",
                                display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture"
                            }
                        },
                        text = "Kontrol rutin bulanan"
                    }
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "OF",
                                display = "Outpatient Facility"
                            }
                        }
                    }
                },
                locationReference = new List<object> {
                    new {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                    }
                },
                patientInstruction = "Kontrol setelah 1 bulan minum obat anti tuberkulosis. Dalam keadaan darurat dapat menghubungi hotlineFasyankesdi nomor 14045"
            };
            return postData;
        }

        //09. Condition - Stabil
        private object ConditionStabilPostDataTK(Registration reg, PatientBridging patSs, string episodeOfCareTKId)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "359746009",
                                display = "Patient's condition stable"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareTKId),
                    display = string.Format("Kunjungan {0} pada tanggal {1}", patSs.BridgingName, reg.RegistrationDate)
                }
            };
            return postData;
        }

        //10. Encounter - Update (Pulang dan Kontrol Kembali)
        private EncounterFinishPut EncounterUpdateDischargeTK(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterAnakINCId, string episodeOfCareTKId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareTKId;
            var mds = new MedicalDischargeSummary();
            mds.LoadByPrimaryKey(reg.RegistrationNo);
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            pa.Query.Load();
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag1.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", primaryDiagnose),
                Display = "Lack of expected normal physiologic development unspecified"
            };
            diag1.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag1.Rank = 1;
            diags.Add(diag1);
            // Diagnosis 2
            var diag2 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag2.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", secondaryDiagnose),
                Display = "Primary respiratory tuberculosis, confirmed bacteriologically and histologically"
            };
            diag2.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag2.Rank = 2;
            diags.Add(diag2);
            postData.Diagnosis = diags;
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                    Code = "home",
                    Display = "Home"
                }
            };
            var dischargeDisposition = new DischargeDisposition()
            {
                Coding = coding,
                Text = "Anjuran dokter untuk pulang dan kontrol kembali 1 bulan setelah minum obat"
            };
            var hospitalization = new Hospitalization()
            {
                DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            };
            postData.Hospitalization = hospitalization;
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        //10. Encounter - Update (Rujukan)
        private EncounterFinishPut EncounterUpdateReferralTK(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterAnakINCId, string episodeOfCareTKId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareTKId;
            var mds = new MedicalDischargeSummary();
            mds.LoadByPrimaryKey(reg.RegistrationNo);
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            pa.Query.Load();
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag1.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", primaryDiagnose),
                Display = "Lack of expected normal physiologic development unspecified"
            };
            diag1.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag1.Rank = 1;
            diags.Add(diag1);
            // Diagnosis 2
            var diag2 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag2.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", secondaryDiagnose),
                Display = "Primary respiratory tuberculosis, confirmed bacteriologically and histologically"
            };
            diag2.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag2.Rank = 2;
            diags.Add(diag2);
            postData.Diagnosis = diags;
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                    Code = "oth",
                    Display = "other-hcf"
                }
            };
            var dischargeDisposition = new DischargeDisposition()
            {
                Coding = coding,
                Text = "Rujukan ke RSUP Fatmawati dengan nomor rujukan 17896543"
            };
            var hospitalization = new Hospitalization()
            {
                DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            };
            postData.Hospitalization = hospitalization;
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        #endregion

        #region IMUNISASI
        // 02. Encounter - Create
        // 03. Immunization -  Riwayat Imunisasi TT0
        // 03. Immunization -  Riwayat Imunisasi TT1 - TT5
        // 03. Immunization - Imunisasi Dilakukan oleh Nakes
        // 03. Procedure - Vaksinasi Pertusis
        // 04. Condition - Create
        private object ConditionCreatePostDataIMN(PatientBridging patSs, string episodeOfCareIMNId, DateTime createDateTime)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "Z27.8",
                                display = "Need for immunization against other combinations of infectious diseases"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", episodeOfCareIMNId)
                },
                onsetDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10
                recordedDate = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)), // tarik dari record date pengisian icd 10                
            };
            return postData;
        }

        // 05. Encounter - Update
        private EncounterFinishPut EncounterUpdateDischargeIMN(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterAnakINCId, string episodeOfCareIMNId, string primaryDiagnose, string secondaryDiagnose, string tertiaryDiagnose)
        {
            var postData = new EncounterFinishPut();
            postData.ResourceType = "Encounter";
            postData.ID = episodeOfCareIMNId;
            var mds = new MedicalDischargeSummary();
            mds.LoadByPrimaryKey(reg.RegistrationNo);
            var pa = new PatientAssessment();
            pa.Query.Where(pa.Query.RegistrationNo == reg.RegistrationNo);
            pa.Query.es.Top = 1;
            pa.Query.OrderBy(pa.Query.AssessmentDateTime.Ascending);
            pa.Query.Load();
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = OrganizationID
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
            };
            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    }
                }
            };
            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag1.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", primaryDiagnose),
                Display = "Need for immunization against other combinations of infectious diseases"
            };
            diag1.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diag1.Rank = 1;
            diags.Add(diag1);
            postData.Diagnosis = diags;
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddMinutes(-1).AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        #endregion

        #region PKPR
        private object EncounterPostDataPKPR(Registration reg, PatientBridging patSs, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs, string serviceReqID)
        {
            DateTime date = reg.RegistrationDate ?? DateTime.MinValue;
            string registrationTime = reg.RegistrationTime;
            TimeSpan time = TimeSpan.Parse(registrationTime);
            DateTime registrationDate = date.Date.Add(time);
            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "in-progress";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "Ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>()
            {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };

            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };

            postData.Participant = new List<Participant>()
            {
                new Participant()
                {
                    Type = types,
                    Individual = new Individual()
                    {
                        Reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                        Display = parMedicSs.BridgingName
                    }
                }
            };

            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}", locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };

            postData.StatusHistory = new List<StatusHistory>()
            {
                new StatusHistory()
                {
                    Status = "arrived",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                },
                new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = string.Format("Organization/{0}", OrganizationID)
            };

            return postData;
        }

        private object hemoglobinExaminationPostDataPKPR(PatientBridging patSs, string encounterPKPRId, ref ParamedicBridging parMedicSs, DateTime procedureDate)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "171201007",
                                display = "Anemia screening"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                            code = "PC000015",
                            display = "Point of Care Testing HB Meter"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPKPRId)
                },
                performedPeriod = new
                {
                    start = string.Format("{0}+00:00", procedureDate.AddHours(GmtDif).ToString(DateFormatLong)),
                    end = string.Format("{0}+00:00", procedureDate.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong)),
                },
                performer = new List<object>() { new
                {
                    actor = new {
                                reference = string.Format( "Practitioner/{0}",parMedicSs.BridgingID),
                                display = parMedicSs.BridgingName
                            }
                        }
                    }
            };
            return postData;
        }

        private object MedicationStatementPostDataPKPR(Registration reg, PatientBridging patSs, string encounterPKPRId, MedicationReceive mr, ItemBridging ssItem, TransPrescriptionItem tpi, DateTime medRecDate)
        {
            var cm = new ConsumeMethod();
            cm.LoadByPrimaryKey(mr.SRConsumeMethod);
            var postData = new
            {
                resourceType = "MedicationStatement",
                identifier = new List<object>
                {
                    new
                    {
                        System = string.Format("http://sys-ids.kemkes.go.id/medicationstatement/{0}", OrganizationID),
                        use = "official",
                        value = reg.RegistrationNo
                    }
                },
                status = "completed",
                category = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/medication-statement-category",
                            code = "community",
                            display = "Community"
                        }
                    }
                },
                medicationCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://sys-ids.kemkes.go.id/kfa",
                            code = ssItem.BridgingID,
                            display = ssItem.BridgingName
                        }
                    },
                    text = ssItem.BridgingName
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                context = new
                {
                    reference = string.Format("Encounter/{0}", encounterPKPRId)
                },
                effectiveDateTime = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                dateAsserted = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                informationSource = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                dosage = new List<object>
                {
                    new
                    {
                        timing = new
                        {
                            repeat = new
                            {
                                frequency = cm.IterationQty,
                                period = 1,
                                periodUnit = "d",
                                duration = 30,
                                durationUnit = "d"
                            }
                        },
                        route = new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://www.whocc.no/atc",
                                    code = "O",
                                    display = "Oral"
                                }
                            }
                        },
                        doseAndRate = new List<object>
                        {
                            new
                            {
                                type = new
                                {
                                    coding = new List<object>
                                    {
                                        new
                                        {
                                            system = "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                                            code = "ordered",
                                            display = "Ordered"
                                        }
                                    }
                                },
                                doseQuantity = new {
                                    value = Convert.ToDecimal(new Fraction(tpi.DosageQty)) , // 4,
                                    unit = tpi.SRDosageUnit, //"TAB",
                                    system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                                    code = AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", tpi.SRDosageUnit,SatuSehatBridgingType)
                                }
                            }
                        }
                    },
                    new
                    {
                        timing = new
                        {
                            repeat = new
                            {
                                duration = 30,
                                durationUnit = "d"
                            }
                        },
                        route = new
                        {
                            coding = new List<object>
                            {
                                new
                                {
                                    system = "http://www.whocc.no/atc",
                                    code = "O",
                                    display = "Oral"
                                }
                            }
                        },
                        doseAndRate = new List<object> {
                            new {
                                type = new {
                                    coding = new List<object> {
                                        new {
                                            system = "http://terminology.hl7.org/CodeSystem/dose-rate-type",
                                            code = "MSD000001",
                                            display = "Consumed"
                                        }
                                    }
                                },
                                doseQuantity = new {
                                    value = mr.ReceiveQty,
                                    unit = mr.SRConsumeUnit,
                                    system = "http://terminology.hl7.org/CodeSystem/v3-orderableDrugForm",
                                    code = AppStandardReferenceItemBridging.GetBridgingID("DosageUnit", mr.SRConsumeUnit,SatuSehatBridgingType)
                                }
                            }
                        }
                    }
                }
            };
            return postData;
        }

        private object DiagnosisPostDataPKPR(Registration reg, PatientBridging patSs, string encounterPKPRId, EpisodeDiagnose ed, ref ParamedicBridging parMedicSs)
        {
            var postData = new
            {
                resourceType = "Condition",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/condition/{0}", OrganizationID),
                        value = reg.RegistrationNo
                    }
                },
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = ed.DiagnoseID,
                            display = ed.DiagnoseName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPKPRId)
                },
                recordedDate = string.Format("{0}+00:00", (ed.CreateDateTime ?? ed.LastUpdateDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recorder = new
                {
                    reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                }
            };
            return postData;
        }

        private object PostServiceRequestPKPR(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, EpisodeDiagnose ed, ParamedicConsultRefer pcr, string encounterPKPRId)
        {
            var setSkriningDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(setSkriningDate);
            var formattedSkriningDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>() {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = reg.RegistrationNo
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>() {
                    new {
                        coding = new List<object>() {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                    {
                        system = "http://snomed.info/sct",
                        code = "737492002",
                        display = "Outpatient care management"
                    }
                    },
                    text = pcr.Notes
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterPKPRId),
                    display = string.Format("Skrining PKPR {0} di hari {1}, {2}", patSs.BridgingName, DayNames[reg.RegistrationDate.Value.DayOfWeek.ToInt()], formattedSkriningDate)
                },
                occurrenceDateTime = string.Format("{0}+00:00", pcr.ConsultDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                performer = new List<object>() {
                    new {
                        reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                        display = parMedSs.BridgingName
                    }
                },
                reasonCode = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = ed.DiagnoseID,
                                display = ed.DiagnoseName
                            }
                        }
                    }
                },
                patientInstruction = pcr.Notes,
            };

            return postData;
        }

        private EncounterFinishPut EncounterUpdatePKPRPutData(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, string encounterPKPRId, string episodeOfCareANCId)
        {
            var postData = new EncounterFinishPut();

            var mds = new MedicalDischargeSummary();
            var mdsq = new MedicalDischargeSummaryQuery("a");
            mdsq.Where(mdsq.RegistrationNo == reg.RegistrationNo);
            mdsq.Select(mdsq.DischargeDate, mdsq.DischargeTime);
            mds.Load(mdsq);
            var mdsd = new MedicalDischargeSummaryDiagnose();
            var mdsdq = new MedicalDischargeSummaryDiagnoseQuery("b");
            mdsdq.Where(mdsdq.RegistrationNo == reg.RegistrationNo);
            mdsdq.Select(mdsdq.DiagnoseID, mdsdq.DiagnosisText);
            mdsdq.es.Top = 1;
            mdsd.Load(mdsdq);

            DateTime date = mds.DischargeDate ?? DateTime.MinValue;
            string dischargeTime = mds.DischargeTime;
            TimeSpan time = TimeSpan.Parse(dischargeTime);
            DateTime dischargeDate = date.Date.Add(time);

            postData.ResourceType = "Encounter";
            postData.ID = encounterPKPRId;
            postData.Identifier = new List<Identifier>()
            {
                new Identifier() {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}",OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "finished";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "Ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };
            var codings = new List<Coding>() {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };
            var types = new List<Code>()
            {
                new Code() { Coding= codings }
            };
            postData.Participant = new List<Participant>() {
                new Participant() {
                    Type = types,
                    Individual= new Individual() {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                }
            };
            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                End = string.Format("{0}+00:00", mds.DischargeDate.Value.AddHours(GmtDif).ToString(DateFormatLong))
            };

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}",locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))), //belum tau darimana
                        End = string.Format("{0}+00:00", string.Format("{0}+00:00", dischargeDate.AddHours(GmtDif).ToString(DateFormatLong))) //belum tau darimana
                    }
                }
            };
            var diags = new List<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis>();
            var diag1 = new Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Diagnosis();
            diag1.Condition = new Condition()
            {
                Reference = string.Format("Condition/{0}", mdsd.DiagnoseID),
                Display = mdsd.DiagnosisText
            };
            diag1.Rank = 1;
            diag1.Use = new Use()
            {
                Coding = new List<Coding>
                {
                    new Coding()
                    {
                        System = "http://terminology.hl7.org/CodeSystem/diagnosis-role",
                        Code = "DD",
                        Display = "Discharge diagnosis"
                    }
                }
            };
            diags.Add(diag1);
            postData.Diagnosis = diags;
            postData.StatusHistory = new List<StatusHistory>();
            postData.StatusHistory.Insert(0, new StatusHistory()
            {
                Status = "arrived",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(1, new StatusHistory()
            {
                Status = "in-progress",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", dischargeDate.AddMinutes(-1).AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            postData.StatusHistory.Insert(2, new StatusHistory()
            {
                Status = "finished",
                Period = new Period()
                {
                    Start = string.Format("{0}+00:00", dischargeDate.AddHours(GmtDif).ToString(DateFormatLong)),
                    End = string.Format("{0}+00:00", dischargeDate.AddHours(GmtDif).ToString(DateFormatLong))
                }
            });
            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.hl7.org/CodeSystem/discharge-disposition",
                    Code = "home",
                    Display = "Home"
                }
            };
            var dischargeDisposition = new DischargeDisposition()
            {
                Coding = coding,
                Text = "Anjuran dokter untuk pulang dan kontrol kembali"
            };
            var hospitalization = new Hospitalization()
            {
                DischargeDisposition = new List<DischargeDisposition> { dischargeDisposition }
            };
            postData.Hospitalization = hospitalization;
            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = String.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        #endregion

        #region AKIAKB
        #endregion

        #region TB

        //2.1 encounter pembuatan kunjungan baru
        private EncounterPost EncounterPostDataTB(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, ServiceUnitBridging locSs)
        {
            //panggil diagnose dengan AppSession.Parameter.SitbDiagnoseList;
            DateTime date = reg.RegistrationDate ?? DateTime.MinValue;
            string registrationTime = reg.RegistrationTime;
            TimeSpan time = TimeSpan.Parse(registrationTime);
            DateTime registrationDate = date.Date.Add(time);
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("ReferralGroup", reg.SRReferralGroup, SatuSehatBridgingType);

            var postData = new EncounterPost();
            postData.ResourceType = "Encounter";
            postData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                },
                new Identifier()
                {
                    Use = "temp",
                    System = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            postData.Status = "arrived";
            postData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "Ambulatory"
            };
            postData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>()
            {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };

            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };

            postData.Participant = new List<Participant>()
            {
                new Participant()
                {
                    Type = types,
                    Individual = new Individual()
                    {
                        Reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                        Display = parMedicSs.BridgingName
                    }
                }
            };

            postData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
            };

            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                    Code = asrib.BridgingID,
                    Display = asrib.BridgingName
                }
            };
            var hospitalization = new Hospitalization()
            {
                AdmitSource = new AdmitSource { Coding = coding }
            };
            postData.Hospitalization = hospitalization;

            postData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Display = locSs.BridgingName,
                        Reference = string.Format("Location/{0}", locSs.BridgingID)
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
                    },
                    Extension = new List<Bridging.SatuSehat.BusinessObject.ExtensionLoc>()
                    {
                        new ExtensionLoc()
                        {
                            Url = "https://fhir.kemkes.go.id/r4/StructureDefinition/ServiceClass",
                            ExtensionItem = new List<ExtensionItem>()
                                            {
                                                new ExtensionItem()
                                                {
                                                    Url= "value",
                                                    ValueCodeableConcept = new Code()
                                                    {
                                                        Coding = new List<Coding>(){ new Coding()
                                                            {
                                                                System = "http://terminology.kemkes.go.id/CodeSystem/locationServiceClass-Outpatient",
                                                                Code = "reguler",
                                                                Display = "Kelas Reguler"
                                                            }
                                                        }
                                                    }
                                                },
                                                new ExtensionItem()
                                                {
                                                    Url= "upgradeClassIndicator",
                                                    ValueCodeableConcept = new Code()
                                                    {
                                                        Coding = new List<Coding>(){ new Coding()
                                                            {
                                                                System = "http://terminology.kemkes.go.id/CodeSystem/locationUpgradeClass",
                                                                Code = "kelas-tetap",
                                                                Display = "Kelas Tetap Perawatan"
                                                            }
                                                        }
                                                    }

                                                }
                            }
                        }
                    }
                }
            };

            postData.StatusHistory = new List<StatusHistory>()
            {
                new StatusHistory()
                {
                    Status = "arrived",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong)),
                    }
                }
            };

            postData.ServiceProvider = new ServiceProvider()
            {
                Reference = string.Format("Organization/{0}", OrganizationID)
            };
            return postData;
        }

        //2.1 encounter put masuk ke ruangan
        private object EnconterPutDataTB(Registration reg, PatientBridging patSs, string encounterTBId, string episodeCareANCId, ref ParamedicBridging parMedicSs, ref ServiceUnitBridging locSs)
        {
            DateTime date = reg.RegistrationDate ?? DateTime.MinValue;
            string registrationTime = reg.RegistrationTime;
            TimeSpan time = TimeSpan.Parse(registrationTime);
            DateTime registrationDate = date.Date.Add(time);
            var startDtmProgress = registrationDate;
            var patAssess = FirstPatientAssessment(reg.RegistrationNo);
            if (patAssess != null)
                startDtmProgress = (DateTime)patAssess.AssessmentDateTime;
            var putData = new EncounterPost();
            putData.ResourceType = "Encounter";
            putData.ID = encounterTBId;
            putData.Identifier = new List<Identifier>()
            {
                new Identifier()
                {
                    System = string.Format("http://sys-ids.kemkes.go.id/encounter/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                },
                new Identifier()
                {
                    Use = "temp",
                    System = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}", OrganizationID),
                    Value = reg.RegistrationNo
                }
            };
            putData.Status = "in-progress";
            putData.Class = new Bridging.SatuSehat.BusinessObject.Class()
            {
                System = "http://terminology.hl7.org/CodeSystem/v3-ActCode",
                Code = "AMB",
                Display = "ambulatory"
            };
            putData.Subject = new RefAndDisplay()
            {
                Reference = string.Format("Patient/{0}", patSs.BridgingID),
                Display = patSs.BridgingName
            };

            var codings = new List<Coding>()
            {
                new Coding()
                {
                    System = "http://terminology.hl7.org/CodeSystem/v3-ParticipationType",
                    Code = "ATND",
                    Display = "attender"
                }
            };

            var types = new List<Code>()
            {
                new Code() { Coding = codings }
            };

            putData.Participant = new List<Participant>()
            {
                new Participant()
                {
                    Type = types,
                    Individual = new Individual()
                    {
                        Reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID),
                        Display = parMedicSs.BridgingName
                    }
                }
            };

            putData.Period = new Period()
            {
                Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
            };

            var coding = new List<Coding>() {
                new Coding() {
                    System = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                    Code = "EHA000002",
                    Display = "Datang sendiri"
                }
            };
            var hospitalization = new Hospitalization()
            {
                AdmitSource = new AdmitSource { Coding = coding }
            };
            putData.Hospitalization = hospitalization;

            putData.Location = new List<Bridging.SatuSehat.BusinessObject.Location>()
            {
                new Bridging.SatuSehat.BusinessObject.Location()
                {
                    LocationItem = new Bridging.SatuSehat.BusinessObject.RefDisplay()
                    {
                        Reference = string.Format("Location/{0}", locSs.BridgingID),
                        Display = locSs.BridgingName
                    },
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong))
                    },
                    Extension = new List<Bridging.SatuSehat.BusinessObject.ExtensionLoc>()
                    {
                        new ExtensionLoc()
                        {
                            Url = "https://fhir.kemkes.go.id/r4/StructureDefinition/ServiceClass",
                            ExtensionItem = new List<ExtensionItem>()
                            {
                                                new ExtensionItem()
                                                {
                                                    Url= "value",
                                                    ValueCodeableConcept = new Code()
                                                    {
                                                        Coding = new List<Coding>(){ new Coding()
                                                            {
                                                                System = "http://terminology.kemkes.go.id/CodeSystem/locationServiceClass-Outpatient",
                                                                Code = "reguler",
                                                                Display = "Kelas Reguler"
                                                            }
                                                        }
                                                    }

                                                },
                                                new ExtensionItem()
                                                {
                                                    Url= "upgradeClassIndicator",
                                                    ValueCodeableConcept = new Code()
                                                    {
                                                        Coding = new List<Coding>(){ new Coding()
                                                            {
                                                                System = "http://terminology.kemkes.go.id/CodeSystem/locationUpgradeClass",
                                                                Code = "kelas-tetap",
                                                                Display = "Kelas Tetap Perawatan"
                                                            }
                                                        }
                                                    }

                                                }
                            }
                        }
                    }
                }
            };

            putData.StatusHistory = new List<StatusHistory>()
            {
                new StatusHistory()
                {
                    Status = "arrived",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", registrationDate.AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", registrationDate.AddMinutes(5).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                },
                new StatusHistory()
                {
                    Status = "in-progress",
                    Period = new Period()
                    {
                        Start = string.Format("{0}+00:00", startDtmProgress.AddMinutes(6).AddHours(GmtDif).ToString(DateFormatLong)),
                        End = string.Format("{0}+00:00", startDtmProgress.AddMinutes(12).AddHours(GmtDif).ToString(DateFormatLong))
                    }
                }
            };

            putData.ServiceProvider = new ServiceProvider()
            {
                Reference = string.Format("Organization/{0}", OrganizationID)
            };

            return putData;
        }

        //3.1 Anamnesis keluhan utama
        private void PostPatientTBChiefComplaint(Registration reg, PatientAssessment pa, PatientBridging patSs, string encounterTBId, ref string accessToken)
        {
            if (string.IsNullOrWhiteSpace(pa.SCTChiefComplaint)) return;

            var ssResult = LoadSatuSehatResult(encounterTBId, "Condition", "ChiefComplaint", "");
            if (ssResult != null && ssResult.ResultID != null) return;

            var snomedct = new Snomedct();
            if (!snomedct.LoadByPrimaryKey("ChiefComplaint", pa.SCTChiefComplaint)) return;
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = pa.SCTChiefComplaint,
                            display= snomedct.Display
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                onsetDateTime = string.Format("{0}+00:00", (pa.AssessmentDateTime ?? pa.LastUpdateDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recordedDate = string.Format("{0}+00:00", (pa.AssessmentDateTime ?? pa.LastUpdateDateTime).Value.AddHours(GmtDif).ToString(DateFormatLong)),
                note = new
                {
                    text = pa.Hpi
                }
            };
            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterTBId),
                    Category = "ChiefComplaint",
                    Code = ""
                };
            }
            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Condition", requestBody, ssResult, ref accessToken);
        }

        //3.2 observation Riwayat penyakit
        private object ObservationPostDataTB(Registration reg, PatientBridging patSs, string encounterTBId, ref ParamedicBridging parMedicSs)
        {
            var pmh = new PastMedicalHistory();
            PastMedicalHistoryQuery pmhq = new PastMedicalHistoryQuery("a");
            pmhq.Select(pmhq.SRMedicalDisease);
            pmhq.Where(pmhq.PatientID == patSs.PatientID);
            pmh.Load(pmhq);
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("PastMedHist", pmh.SRMedicalDisease, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                    new {
                        system = "http://snomed.info/sct",
                        code = asrib.BridgingID, //Mapping Asri PastMedHist
                        display = asrib.BridgingName
                    }
                }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                effectiveDateTime = string.Format("{0}+00:00", pmh.EffectiveDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", pmh.EffectiveDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object> {
                    new {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                component = new List<object>
                {
                     new
                        {
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://snomed.info/sct",
                                        code = "715047008",
                                        display = "Does obtain medication"
                                    }
                                }
                            },
                            valueBoolean = true
                        },
                     new
                        {
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://snomed.info/sct",
                                        code = "31509003",
                                        display = "Controlled"
                                    }
                                }
                            },
                            valueBoolean = true
                        }
                },
            };
            return postData;
        }

        //Riwayat Pengobatan
        private void PostTBMedicationStatement(Registration reg, PatientBridging patSs, ParamedicBridging parSs, string encounterId, ref string accessToken)
        {
            var tpiq = new MedicationReceiveFromPatientQuery("tpi");
            var tpq = new MedicationReceiveQuery("tp");
            tpiq.InnerJoin(tpq).On(tpiq.MedicationReceiveNo == tpq.MedicationReceiveNo);
            tpiq.Where(tpq.RegistrationNo == reg.RegistrationNo);

            tpiq.Select(tpq.MedicationReceiveNo, tpq.ItemID, tpq.ReceiveDateTime);

            var dtbTpi = tpiq.LoadDataTable();

            //Medication Create
            foreach (DataRow row in dtbTpi.Rows)
            {
                var itemID = row["ItemID"].ToString();
                if (string.IsNullOrEmpty(itemID)) continue;

                var ssItem = new ItemBridging();
                ssItem.Query.Where(ssItem.Query.ItemID == itemID, ssItem.Query.SRBridgingType == SatuSehatBridgingType);
                ssItem.Query.es.Top = 1;
                if (!ssItem.Query.Load()) continue;

                var kfaItem = new SatuSehatKfa();
                kfaItem.Query.Where(kfaItem.Query.SsUuid == ssItem.BridgingID);
                kfaItem.Query.es.Top = 1;
                if (!kfaItem.Query.Load()) continue;

                var kfaInfo = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa.Root>(kfaItem.SsResult);

                //Check status kirim
                var ssResult = LoadSatuSehatResult(encounterId, "Medication", "MedicationStatement", row["MedicationReceiveNo"].ToString());
                var medicationResultID = ssResult != null ? ssResult.ResultID.ToString() : string.Empty;

                ssResult = LoadSatuSehatResult(encounterId, "MedicationStatement", "MedicationStatement", row["MedicationReceiveNo"].ToString());

                if (ssResult == null || ssResult.ResultID == null)
                {
                    var tpi = new MedicationReceive();
                    tpi.LoadByPrimaryKey(row["MedicationReceiveNo"].ToInt());
                    var postRequestData = MedicationStatementTBPostData(reg, patSs, Convert.ToDateTime(row["ReceiveDateTime"]), ssItem, tpi, encounterId);
                    if (postRequestData != null)
                    {
                        var requestBody = JsonConvert.SerializeObject(postRequestData);
                        if (ssResult == null)
                        {
                            ssResult = new SatuSehatResult()
                            {
                                EncounterID = new Guid(encounterId),
                                Category = "MedicationStatement",
                                Code = row["MedicationReceiveNo"].ToString()
                            };
                        }
                        var medReqRes = RestClientPostAndSaveLog("MedicationStatement", requestBody, ssResult, ref accessToken);
                    }
                }
            }
        }

        private object MedicationStatementTBPostData(Registration reg, PatientBridging patSs, DateTime medRecDate, ItemBridging ssItem, MedicationReceive tpi, string encounterId)
        {
            var cm = new ConsumeMethod();
            cm.LoadByPrimaryKey(tpi.SRConsumeMethod);

            var postData = new
            {
                resourceType = "MedicationStatement",
                status = "active",
                category =
                    new
                    {
                        coding = new List<object>() {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/medication-statement-category",
                                code = "community",
                                display = "community"
                            }
                        }
                    },
                medicationCodeableConcept = new
                {
                    coding = new List<object>() {
                       new
                       {
                           system = "http://sys-ids.kemkes.go.id/kfa",
                           code= ssItem.BridgingID,
                           display= ssItem.BridgingName
                       }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                dosage = new List<object> {
                        new {
                            text= cm.SRConsumeMethodName,
                            timing= new {
                                repeat= new {
                                    frequency= cm.IterationQty,
                                    period= 1,
                                    periodUnit= "d"
                                }
                            }
                        }
                },
                effectiveDateTime = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                dateAsserted = string.Format("{0}+00:00", medRecDate.AddHours(GmtDif).ToString(DateFormatLong)),
                informationSource = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                context = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                }
            };

            return postData;
        }
        //5.2 Qusetionaire TB 
        private object AnswerTB(string itemID, string bridgingName)
        {
            new List<object>() {
                        new {
                            valueCoding = new
                            {
                                system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term"
                                //code = isYes? "OV000052":"OV000053",
                                //display = isYes? "Sesuai":"Tidak Sesuai"
                            }
                        }
            };

            return new List<object>();
        }
        private void PostQuestionaireTB(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string encounterTBId, ref string accessToken)
        {

            // Check status kirim
            var ssResult = LoadSatuSehatResult(encounterTBId, "QuestionnaireResponse", "QuestionnaireResponse", "");
            if (ssResult != null && ssResult.ResultID != null) return;

            var createDateTime = DateTime.Now;
            var createByUserID = string.Empty;
            DataTable dtbDiagnoseResult = null;

            var diag = new DiagnoseQuery("d");
            var ed = new EpisodeDiagnoseQuery("ed");
            ed.InnerJoin(diag).On(ed.DiagnoseID == diag.DiagnoseID);
            ed.Where(ed.RegistrationNo == reg.RegistrationNo, ed.IsVoid == false, ed.DiagnosisText.Like(string.Format("%tuberculosis%")));
            var brg = new AppStandardReferenceItemBridgingQuery("brg");
            ed.InnerJoin(brg).On(brg.SRBridgingType == SatuSehatBridgingType && brg.ItemID == ed.DiagnoseID);

            ed.Select(ed.DiagnoseID, diag.DiagnoseName, brg.BridgingID, brg.BridgingName, ed.CreateDateTime);
            ed.OrderBy(ed.SequenceNo.Ascending, brg.BridgingID.Ascending);
            dtbDiagnoseResult = ed.LoadDataTable();
            if (dtbDiagnoseResult.Rows.Count == 0) return;

            foreach (DataRow row in dtbDiagnoseResult.Rows)
            {
                createDateTime = Convert.ToDateTime(row["CreateDateTime"]);
                break;
            }
            var stdib = new AppStandardReferenceItemBridgingQuery("stdib");
            //stdib.Where(stdib.StandardReferenceID == "PrescReview");
            stdib.OrderBy(stdib.BridgingID.Ascending);
            var dtbRev = stdib.LoadDataTable();
            var listRev1 = new List<object>();
            var listRev2 = new List<object>();
            var listRev3 = new List<object>();

            foreach (DataRow row in dtbRev.Rows)
            {
                var itemID = row["ItemID"];
                var isYes = false;
                foreach (DataRow rowResult in dtbDiagnoseResult.Rows)
                {
                    if (itemID.Equals(rowResult["ItemID"]))
                    {
                        isYes = true;
                        break;
                    }
                }
                if (!isYes) continue;
                var bid = row["BridgingID"].ToString();
                if (bid.Contains("1."))
                    listRev1.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerTB(row["ItemID"].ToString(), row["BridgingName"].ToString())
                        }
                    );
                else if (bid.Contains("2."))
                    listRev2.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerTB(row["ItemID"].ToString(), row["BridgingName"].ToString())
                        }
                    );
                else if (bid.Contains("3."))
                    listRev3.Add(
                        new
                        {
                            linkId = bid,
                            text = row["BridgingName"].ToString(),
                            answer = AnswerTB(row["ItemID"].ToString(), row["BridgingName"].ToString())
                        }
                    );
            }

            var postData = new
            {
                resourceType = "QuestionnaireResponse",
                questionnaire = "https://fhir.kemkes.go.id/Questionnaire/Q0001",
                status = "completed",
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                authored = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                author = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                source = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },

                item = new List<object>() {
                    new {
                        linkId = "1",
                        text= "Klasifikasi Kasus Terduga TB",
                        item = listRev1
                    }
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterTBId),
                    Category = "QuestionnaireResponse",
                    Code = ""
                };
            }
            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("QuestionnaireResponse", requestBody, ssResult, ref accessToken);
        }

        //6.1 Status HIV
        private object ObservationHIVPostDataTB(Registration reg, PatientBridging patSs, string encounterTBId, ref ParamedicBridging parMedicSs)
        {
            DataTable dtbDiagnoseResult = null;
            bool isYes = false;
            var ed = new EpisodeDiagnoseQuery("ed");
            ed.Where(ed.RegistrationNo == reg.RegistrationNo, ed.IsVoid == false, ed.DiagnosisText.Like(string.Format("%hiv%")));
            ed.es.Top = 1;
            ed.Select(ed.DiagnoseID, ed.DiagnosisText, ed.CreateDateTime);
            ed.OrderBy(ed.SequenceNo.Ascending);
            dtbDiagnoseResult = ed.LoadDataTable();
            if (dtbDiagnoseResult.Rows.Count > 0)
                isYes = true;
            var ep = new EpisodeDiagnose();
            ep.Load(ed);
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://loinc.org",
                            code = "55277-8",
                            display = "HIV status"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.PatientID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                effectiveDateTime = string.Format("{0}+00:00", ep.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object> {
                    new {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new {
                        reference = string.Format("Organization/{0}", OrganizationID)
                    }
                },
                valueCodeableConcept = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = isYes? "10828004":"260385009",
                            display = isYes? "Positive":"Negative"
                        }
                    }
                }
            };
            return postData;
        }

        //.6.2 status diabetes
        private object ObservationDMPostDataTB(Registration reg, PatientBridging patSs, string encounterTBId, ref ParamedicBridging parMedicSs)
        {
            DataTable dtbDiagnoseResult = null;
            bool isYes = false;
            var ed = new EpisodeDiagnoseQuery("ed");
            ed.Where(ed.RegistrationNo == reg.RegistrationNo, ed.IsVoid == false, ed.DiagnosisText.Like(string.Format("%diabetes%")));
            ed.es.Top = 1;
            ed.Select(ed.DiagnoseID, ed.DiagnosisText, ed.CreateDateTime);
            ed.OrderBy(ed.SequenceNo.Ascending);
            dtbDiagnoseResult = ed.LoadDataTable();
            if (dtbDiagnoseResult.Rows.Count > 0)
                isYes = true;
            var ep = new EpisodeDiagnose();
            ep.Load(ed);
            var pmh = new PastMedicalHistory();
            PastMedicalHistoryQuery pmhq = new PastMedicalHistoryQuery("a");
            pmhq.Select(pmhq.SRMedicalDisease);
            pmhq.Where(pmhq.PatientID == patSs.PatientID);
            pmh.Load(pmhq);
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://loinc.org",
                            code = "33248-6",
                            display = "Diabetes status"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.PatientID)
                },
                performer = new List<object> {
                    new {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new {
                        reference = string.Format("Organization/{0}", OrganizationID)
                    }
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                effectiveDateTime = string.Format("{0}+00:00", pmh.EffectiveDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                valueCodeableConcept = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = isYes? "10828004":"260385009",
                            display = isYes? "Positive":"Negative"
                        }
                    }
                }
            };
            return postData;
        }

        //7.1 EOC TB SO
        private object EpisodeofCarePostDataTBSO(Registration reg, PatientBridging patSs)
        {
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "waitlist",
                statusHistory = new List<object> {
                    new {
                        status = "waitlist",
                        period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),

                        }
                    }
                },
                type = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/episodeofcare-type",
                                code = "hacc",
                                display = "Home and Community Care"
                            }
                        }
                    }
                },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                }
            };
            return postData;
        }

        //7.1 EOC TB RO
        private object EpisodeofCarePostDataTBRO(Registration reg, PatientBridging patSs)
        {
            var postData = new
            {
                resourceType = "EpisodeOfCare",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/episode-of-care/{0}", OrganizationID),
                        value = reg.RegistrationNo
                    }
                },
                status = "waitlist",
                statusHistory = new List<object> {
                    new {
                        status = "waitlist",
                        period = new {
                            start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),

                        }
                    }
                },
                type = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.kemkes.go.id/CodeSystem/episodeofcare-type",
                                code = "TB-RO",
                                display = "Tuberkulosis Resisten Obat"
                            }
                        }
                    }
                },
                patient = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                managingOrganization = new
                {
                    reference = string.Format("Organization/{0}", OrganizationID)
                },
                period = new
                {
                    start = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).AddHours(GmtDif).ToString(DateFormatLong)),
                }
            };
            return postData;
        }

        //9 Diagnosa
        private object DiagnosePostDataTB(Registration reg, PatientBridging patSs, EpisodeDiagnose ed, string encounterTBId)
        {
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                            code = "active",
                            display = "Active"
                        }
                    }
                },
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "encounter-diagnosis",
                                display = "Encounter Diagnosis"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://hl7.org/fhir/sid/icd-10",
                            code = ed.DiagnoseID,
                            display = ed.DiagnoseName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                onsetDateTime = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                recordedDate = string.Format("{0}+00:00", ed.CreateDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
            };
            return postData;
        }

        //10 Rencana tindak lanjut
        private object ServiceRequestPostDataTB(Registration reg, PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, EpisodeDiagnose ed, string encounterTBId, DateTime createDateTime)
        {
            var reff = new ReferExternal();
            reff.LoadByPrimaryKey(reg.RegistrationNo);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("ReferReason", reff.SRReferReason);

            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("RefferalType", reff.SRReferType, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = reg.RegistrationNo
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = asrib.BridgingID,
                                display = asrib.BridgingName
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "185389009",
                                display = "Follow-up visit"
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                occurrenceDateTime = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                authoredOn = string.Format("{0}+00:00", createDateTime.AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        display = parSs.BridgingName
                    }
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "OF",
                                display = "Outpatient Facility"
                            }
                        }
                    }
                },
                locationReference = new List<object>
                {
                    new
                    {
                        display = locSs.BridgingName,
                        reference = string.Format("Location/{0}", locSs.BridgingID)
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = ed.DiagnoseID,
                                display = ed.DiagnoseName
                            }
                        },
                        text = asri.ItemName
                    }
                },
                patientInstruction = reff.OtherInformation
            };
            return postData;
        }

        //11 kondisi meninggalkan RS
        private object ConditionPostDataTB(Registration reg, PatientBridging patSs, MedicalDischargeSummary mds, string encounterTBId)
        {
            DateTime date = mds.DischargeDate ?? DateTime.MinValue;
            string dischargeTime = mds.DischargeTime;
            TimeSpan time = TimeSpan.Parse(dischargeTime);
            DateTime dischargeDate = date.Date.Add(time);
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("DischargeCondition", reg.SRDischargeCondition, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Condition",
                clinicalStatus = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-clinical",
                                code = "active",
                                display = "Active"
                            }
                        }
                    }
                },
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/condition-category",
                                code = "problem-list-item",
                                display = "Problem List Item"
                            }
                        }
                    }
                },
                code = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = asrib.BridgingID,
                                display = asrib.BridgingName
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterTBId)
                },
                recordedDate = string.Format("{0}+00:00", dischargeDate.AddHours(GmtDif).ToString(DateFormatLong)),
            };
            return postData;
        }

        //12.cara keliuar RS
        //private EncounterFinishPut DischargeMethodTBPutData(PatientBridging patSs, ParamedicBridging parSs, ServiceUnitBridging locSs, Registration reg, string encounterTBId, string MainDiagnosePNCId, string SecondaryDiagnosePNCId)
        //{
        //    var mdsdq = new MedicalDischargeSummaryDiagnoseQuery("b");
        //    mdsdq.Where(mdsdq.RegistrationNo == reg.RegistrationNo);
        //    mdsdq.Select(mdsdq.DiagnoseID, mdsdq.DiagnosisText);
        //    var dtbDiagnosisResult = mdsdq.LoadDataTable();
        //    var encounterPostData = EncounterFinishPutData(reg, patSs, parSs, locSs, dtbDiagnosisResult, encounterTBId, "TB");
        //    return encounterPostData;

        //}

        private object JobObservationPostData(PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            var pat = new Patient();
            pat.LoadByPrimaryKey(patSs.PatientID);
            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("Occupation", pat.SROccupation, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "Observation",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = "394704008",
                            display = "Occupation history"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", pat.CreatedDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", pat.CreatedDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID)
                    }
                },
                valueCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                }
            };

            return postData;
        }

        private object PregnancyStatusObservationPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            bool isPregnant = reg.IsPregnant ?? false;
            var postData = new
            {
                resourceType = "Observation",
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = "82810-3",
                            display = "Pregnancy status"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID)
                    }
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                valueCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = isPregnant? "LA15173-0":"LA26683-5",
                            display = isPregnant? "Pregnant":"Not Pregnant",
                        }
                    }
                }
            };

            return postData;
        }
        private object PediatricTBObservationPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)// dijalankan jika pasien anak(0-14th)
        {
            //var pat = new Patient();
            //pat.LoadByPrimaryKey(patSs.PatientID);
            //int age = 0;
            //if (pat.DateOfBirth != null)
            //{
            //    DateTime dateOfBirth = pat.DateOfBirth.Value; 
            //    age = DateTime.Now.Year - dateOfBirth.Year;

            //    if (DateTime.Now.Date < dateOfBirth.AddYears(age))
            //    {
            //        age--;
            //    }
            //    if(age < 14)
            //    {
            //        PediatricTBObservationPostData(reg, patSs, parMedicSs, encounterId);
            //    }
            //}
            var postData = new
            {
                resourceType = "Observation",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "survey",
                                display = "Survey"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                            code = "OC000191",
                            display = "Jumlah Skoring TBC Anak"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    Display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID),// diisi divisi lab 
                        display = OrganizationID  //diisi divisi lab
                    }
                },
                valueInteger = 8, // Diisi dengan jumlah skoring TBC anak. Data ini hanya perlu dikirimkan untuk kasus TB anak
                derivedFrom = new
                {
                    reference = string.Format("Observation/{0}", OrganizationID /* Diisi dengan keterangan hasil pemeriksaan uji tuberkulin */),
                    Display = "Hasil uji tuberkulin"
                }
            };

            return postData;
        }
        private object SensitivityTBObservationPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            var postData = new
            {
                resourceType = "Observation",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "laboratory",
                                display = "Laboratory"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = "OC000192",
                            display = "Klasifikasi Berdasarkan Uji Kepekaan"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)), //jam asesmen
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID), //divisi lab
                        display = OrganizationID  //diisi divisi lab
                    }
                },
                derivedFrom = new
                {
                    reference = string.Format("Observation/{0}", OrganizationID), //belum tahu ambil dari mana
                },
                valueCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "423092005",
                            display = "Multidrug resistant tuberculosis (disorder)"
                        }
                    }
                }
            };
            return postData;
        }
        private object DiagnosisTypeTBObservationPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            var postData = new
            {
                resourceType = "Observation",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/observation-category",
                                code = "exam",
                                display = "Exam"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "106229004",
                            display = "Qualifier for type of diagnosis"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)), //jam asesmen
                performer = new List<object>
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format("Organization/{0}", OrganizationID) //divisi lab
                    }
                },
                derivedFrom = new
                {
                    reference = string.Format("Observation/{0}", OrganizationID), //belum tahu ambil dari mana
                },
                valueCodeableConcept = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.kemkes.go.id/CodeSystem/tb-case-definition",
                            code = "tb-bac", //tipe diagnosis tb-bac/tb-clin
                            display = "Terkonfirmasi bakteriologis"
                        }
                    }
                }
            };
            return postData;
        }
        private object EducationDateProcedurePostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            var postData = new
            {
                resourceType = "Procedure",
                status = "completed",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://snomed.info/sct",
                                code = "420227002",
                                display = "Recommendation to (procedure)"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.kemkes.go.id",
                            code = "TK000016",
                            display = "Edukasi Tes HIV"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                performedDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)), //tanggal nya
                performer = new List<object>()
                {
                    new
                    {
                        actor = new
                        {
                                reference = string.Format( "Practitioner/{0}",parMedicSs.BridgingID),
                                display = parMedicSs.BridgingName
                        }
                    }
                }
            };
            return postData;
        }
        //careplan TB
        private void PostCarePlanTB(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, PatientAssessment pa, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "CarePlan", "TB", "736271009");
            if (ssResult != null && ssResult.ResultID != null) return;

            var postData = new
            {
                resourceType = "CarePlan",
                status = "active",
                intent = "plan",
                title = "Rencana Pengobatan Pasien TB",
                description = "Mencakup data paduan OAT, Bentuk OAT, dan Paduan Pengobatan pasien TB",
                category = new List<object>()
                { new
                { coding = new List<object>()
                    { new {
                        system = "http://terminology.kemkes.go.id/CodeSystem/careplan-category",
                        code = "TB-SO", //kategori TB
                        display = "Tuberkulosis Sensitif Obat"
                    } }
                }},
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                created = string.Format("{0}+00:00", pa.AssessmentDateTime.Value.AddHours(GmtDif).ToString(DateFormatLong)), //"2023-08-31T01:20:00+00:00",
                author = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID)
                },
                activity = new List<object>
                {
                    new
                    {
                        detail = new
                        {
                            status = "not-started",
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id",
                                        code = "TK000022",
                                        display = "Akan diobati/dirujuk"
                                    }
                                }
                            },
                            performer = new List<object>
                            {
                                new
                                {
                                    reference = string.Format("Organization/{0}", OrganizationID)
                                }
                            }
                        }
                    },
                    new
                    {
                        detail = new
                        {
                            status = "not-started",
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000006",
                                        display = "Paduan OAT"
                                    }
                                }
                            },
                            productCodeableConcept = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000007",
                                        display = "OAT Kategori 1" //panduan obat cbo
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        detail = new
                        {
                            status = "not-started",
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000004",
                                        display = "Bentuk OAT"
                                    }
                                }
                            },
                            productCodeableConcept = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000091",
                                        display = "KDT" //bentuk OAT cbo
                                    }
                                }
                            }
                        }
                    },
                    new
                    {
                        detail = new
                        {
                            status = "not-started",
                            code = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000005",
                                        display = "Paduan Pengobatan"
                                    }
                                }
                            },
                            productCodeableConcept = new
                            {
                                coding = new List<object>
                                {
                                    new
                                    {
                                        system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                        code = "CP000015",
                                        display = "2(HRZE)/4(HR)3" //panduan pengobatan cbo
                                    }
                                }
                            }
                        }
                    }
                }
            };

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = "Rawat",
                    Code = "736271009" //Outpatient care plan (http://snomed.info/sct)
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("CarePlan", requestBody, ssResult, ref accessToken);
        }
        //service request kontrol kembali   
        private object ServiceRequestKontrolKembaliV2(PatientBridging patSs, ParamedicBridging parSs, Registration reg, ServiceUnitBridging sub, string encounterId)
        {
            var visitDate = reg.RegistrationDate.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK");
            DateTime parsedDate = DateTime.Parse(visitDate);
            var formatVisitDate = parsedDate.ToString("d MMMM yyyy", new System.Globalization.CultureInfo("id-ID"));

            var reff = new ReferExternal();
            reff.LoadByPrimaryKey(reg.RegistrationNo);

            var asri = new AppStandardReferenceItem();
            asri.LoadByPrimaryKey("ReferReason", reff.SRReferReason);

            var asrib = new AppStandardReferenceItemBridging();
            asrib.LoadByPrimaryKey("RefferalType", reff.SRReferType, SatuSehatBridgingType);
            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object> {
                    new {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = OrganizationID
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://snomed.info/sct",
                                code = "3457005",
                                display = "Patient referral"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object> {
                        new {
                            system = "http://snomed.info/sct",
                            code = asrib.BridgingID,
                            display = asrib.BridgingName
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                occurrenceDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                authoredOn = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)),
                requester = new
                {
                    Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                    Display = parSs.BridgingName
                },
                performer = new List<object>() { new
                    {
                        Reference = string.Format("Practitioner/{0}", parSs.BridgingID),
                        Display = parSs.BridgingName
                    }
                },
                locationReference = new
                {
                    Reference = string.Format("Practitioner/{0}", sub.BridgingID),
                    Display = sub.BridgingName
                },
                locationCode = new List<object> {
                    new {
                        coding = new List<object> {
                            new {
                                system = "http://terminology.hl7.org/CodeSystem/v3-RoleCode",
                                code = "OF",
                                display = "Outpatient Facility"
                            }
                        }
                    }
                },
                reasonCode = new List<object> {
                    new {
                        coding = new List<object>{
                            new {
                                system = "http://hl7.org/fhir/sid/icd-10",
                                code = "A15.0",
                                display = "Tuberculosis of lung, confirmed by sputum microscopy with or without culture" //diagnosis
                            }
                        },
                        text = asri.ItemName
                    }
                },
                patientInstruction = reff.OtherInformation
            };

            return postData;
        }
        private BaseResponse PostServiceRequestExamination(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string transactionNo, string sequenceNo, string itemName, DateTime approvedDateTime, string headerNotes, string itemNotes, LoincItem loincItem, string encounterId, string encounterType, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

            var postData = new
            {
                resourceType = "ServiceRequest",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/servicerequest/{0}", OrganizationID),
                        value = string.Format("{0}-{1}", transactionNo, sequenceNo) //"00001"
                    },
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}", OrganizationID),
                        value = string.Format("{0}-{1}", transactionNo, sequenceNo)
                    }
                },
                status = "active",
                intent = "original-order",
                priority = "routine",
                category = new List<object>() {
                    new {
                        coding= new List<object>() {
                            new {
                                system= "http://snomed.info/sct",
                                code= "108252007",
                                display= "Laboratory procedure"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>() { new
                        {
                        system= "http://terminology.kemkes.go.id/CodeSystem/examination",
                        code= loincItem.Code,     //ada di https://satusehat.kemkes.go.id/platform/docs/id/terminology/lampiran-terminologi/tuberkulosis/
                        display = loincItem.Display // "Microscopic observation[Identifier} in Sputum by Acid fast stain"
                        }
                    },
                    text = itemNotes// "Pemeriksaan Sputum BTA"
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID)
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                occurrenceDateTime = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), // "2022-06-14T09:30:27+07:00",
                authoredOn = string.Format("{0}+00:00", approvedDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2022-06-13T12:30:27+07:00",
                requester = new
                {
                    reference = string.Format("Practitioner/{0}", parMedSs.BridgingID),
                    display = parMedSs.BridgingName
                },
                performer = new List<object>() {
                    new {
                        reference= string.Format("Practitioner/{0}", parMedSs.BridgingID),
                        display= parMedSs.BridgingName
                    }
                },
                reasonCode = new List<object>()
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                                code = "SRR000008", //mungkin perlu mapping lagi
                                display = "Follow Up" //mungkin perlu mapping lagi
                            }
                        },
                        text = "3"
                    }
                },
                supportingInfo = new List<object>
                {
                    new
                    {
                        reference = string.Format("QuestionnaireResponse/{0}", "sa"), //diisi klasifikasi TB-RO/SO
                    }
                },
                note = new List<object>
                {
                    new
                    {
                        text = "Pasien pernah mengalami tuberkulosis paru tahun 2014. Pasien didiagnosis sebagai tuberkulosis paru kambuh."
                    }
                }
            };
            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo
                };
            }

            var requestBody = JsonConvert.SerializeObject(postData);
            return RestClientPostAndSaveLog("ServiceRequest", requestBody, ssResult, ref accessToken);
        }
        private void PostSpecimenTB(PatientBridging patSs, string transactionNo, string sequenceNo, string itemID, string collectMethod, DateTime collectDateTime, DateTime receiveDateTime, string serviceReqID, string encounterId, ref string accessToken)
        {
            //Check status kirim
            var ssResult = LoadSatuSehatResult(encounterId, "Specimen", transactionNo, sequenceNo);
            if (ssResult != null && ssResult.ResultID != null) return;

            if (ssResult == null)
            {
                ssResult = new SatuSehatResult()
                {
                    EncounterID = new Guid(encounterId),
                    Category = transactionNo,
                    Code = sequenceNo,
                    ResourceType = "Specimen"
                };
            }

            var itemLab = new ItemLaboratory();
            itemLab.LoadByPrimaryKey(itemID);

            var specimenType = new AppStandardReferenceItemBridging();
            if (!specimenType.LoadByPrimaryKey("SpecimenType", itemLab.SRSpecimenType, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging SpecimenType [{0}] not found", itemLab.SRSpecimenType);
                ssResult.Save();
                return;
            }

            var cm = new AppStandardReferenceItemBridging();
            if (!cm.LoadByPrimaryKey("CollectMethod", collectMethod, SatuSehatBridgingType))
            {
                SetResultIndexNo(ssResult);
                ssResult.ErrorResponse = string.Format("Bridging CollectMethod [{0}] not found", collectMethod);
                ssResult.Save();
                return;
            }

            var snomed = new Snomedct();
            snomed.LoadByPrimaryKey("SpecimenType", specimenType.BridgingID);

            var postData = new
            {
                resourceType = "Specimen",
                identifier = new List<object>()
                { new
                    {
                        system =  string.Format("http://sys-ids.kemkes.go.id/specimen/{0}",OrganizationID),
                        value= string.Format("{0}-{1}", transactionNo, sequenceNo),
                        assigner = new
                        {
                            reference =  string.Format("Organization/{0}",OrganizationID)
                        }
                    }
                },
                status = "available",
                type = new
                {
                    coding = new List<object>()
                    {
                        new
                        {
                            system =  "http://terminology.kemkes.go.id/CodeSystem/clinical-term",
                            code = specimenType.BridgingID, // "ST000001", untuk Jenis Contoh Uji https://satusehat.kemkes.go.id/platform/docs/id/terminology/lampiran-terminologi/tuberkulosis/
                            display =  snomed.Display // "Sputum Pagi"
                        }
                    }
                },
                collection = new
                {
                    collectedDateTime = string.Format("{0}+00:00", collectDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2023 - 08 - 31T15: 15:00 + 00:00"
                    extension = new List<object>()
                    {
                        new
                        {
                            url = "https://fhir.kemkes.go.id/r4/StructureDefinition/CollectorOrganization",
                            valueReference = new
                            {
                                reference =  string.Format("Organization/{0}",OrganizationID)
                            }
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                request = new List<object>()
                {
                    new
                    {
                        reference = string.Format("ServiceRequest/{0}",serviceReqID)
                    }
                },
                receivedTime = string.Format("{0}+00:00", receiveDateTime.AddHours(GmtDif).ToString(DateFormatLong)), //"2023-08 - 31T15: 25:00 + 00:00"
                extension = new List<object>()
                {
                    new
                    {
                        url = "https://fhir.kemkes.go.id/r4/StructureDefinition/TransportedTime",
                        valueDateTime = string.Format("{0}+00:00", collectDateTime.AddHours(GmtDif).ToString(DateFormatLong))
                    }
                },
                condition = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://snomed.info/sct",
                            code = "710070002", //konfirmasi penerimaan ada di https://satusehat.kemkes.go.id/platform/docs/id/terminology/lampiran-terminologi/tuberkulosis/
                            display = "Specimen unsatisfactory for evaluation due to broken container"
                        }

                    },
                    text = "Kontainer spesimen terbentur dalam perjalanan dari faskes ke lab"
                }
            };

            var requestBody = JsonConvert.SerializeObject(postData);
            RestClientPostAndSaveLog("Specimen", requestBody, ssResult, ref accessToken);
        }
        //private BaseResponse ObservationTestResultPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedSs, string itemName, DateTime approvedDateTime, string serviceReqID, string itemNotes, LoincItem loincItem, string encounterId, string encounterType, string specimenId, ref string accessToken)
        //{
        //    //Check status kirim
        //    //var ssResult = LoadSatuSehatResult(encounterId, "ServiceRequest", transactionNo, sequenceNo);
        //    //if (ssResult != null && ssResult.ResultID != null) return new BaseResponse() { Id = ssResult.ResultID.ToString() };

        //    var postData = new
        //    {
        //        resourceType = "Observation",
        //        identifier = new List<object>
        //        {
        //            new
        //            {
        //                system = string.Format("http://sys-ids.kemkes.go.id/observation/{0}", _organizationID),
        //                value = _organizationID
        //            },
        //            new
        //            {
        //                system = string.Format("http://sys-ids.kemkes.go.id/sitb/{0}", _organizationID),
        //                value = _organizationID
        //            }
        //        },
        //        status = "final",
        //        category = new List<object>() {
        //            new {
        //                coding= new List<object>() {
        //                    new {
        //                        system= "http://terminology.hl7.org/CodeSystem/observation-category",
        //                        code= "laboratory",
        //                        display= "Laboratory"
        //                    }
        //                }
        //            }
        //        },
        //        code = new
        //        {
        //            coding = new List<object>() { new
        //                {
        //                system= "http://terminology.kemkes.go.id/CodeSystem/examination",
        //                code= "X099379",     //ada di https://satusehat.kemkes.go.id/platform/docs/id/terminology/lampiran-terminologi/tuberkulosis/
        //                display = "Pemeriksaaan TCM XDR" // "Microscopic observation[Identifier} in Sputum by Acid fast stain"
        //                }
        //            }
        //        },
        //        subject = new
        //        {
        //            reference = string.Format("Patient/{0}", patSs.BridgingID),
        //            display = patSs.BridgingName
        //        },
        //        encounter = new
        //        {
        //            reference = string.Format("Encounter/{0}", encounterId)
        //        },
        //        effectiveDateTime = string.Format("{0}+00:00", approvedDateTime.AddHours(_gmtDif).ToString(_dateFormat)), // "2022-06-14T09:30:27+07:00",
        //        issued = string.Format("{0}+00:00", approvedDateTime.AddHours(_gmtDif).ToString(_dateFormat)), //"2022-06-13T12:30:27+07:00",
        //        performer = new List<object>() {
        //            new {
        //                reference= string.Format("Practitioner/{0}", parMedSs.BridgingID)
        //            },
        //            new {
        //                reference= string.Format("Organization/{0}", _organizationID/*divisi_lab*/),
        //                display= "Divisi Laboratorium Puskesmas SATUSEHAT"
        //            }
        //        },
        //        specimen = new
        //        {
        //            reference = string.Format("Specimen/{0}", specimenId)
        //        },
        //        basedOn = new List<object>
        //        {
        //            new
        //            {
        //                reference = string.Format("ServiceRequest/{0}", serviceReqID)
        //            }
        //        },
        //        valueCodeableConcept = new
        //        {
        //            coding = new List<object>() {
        //               new
        //               {
        //                   system = "http://terminology.kemkes.go.id/CodeSystem/examination",
        //                   code= "X099781",
        //                   display= "Hasil Valid"
        //               }
        //            }
        //        },
        //        component = new List<object>
        //        {
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://loinc.org",
        //                                code = "88874-3",
        //                                display = "Mycobacterium tuberculosis complex DNA [Presence] in Isolate or Specimen by Molecular genetics method"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://terminology.kemkes.go.id/CodeSystem/examination",
        //                                code = "X099391",
        //                                display = "H Dosis Rendah (Low)"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://loinc.org",
        //                                code = "89488-1",
        //                                display = "Isoniazid [Susceptibility] by Genotype method"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://terminology.kemkes.go.id/CodeSystem/examination",
        //                                code = "X099392",
        //                                display = "FQ Dosis Rendah (Low)"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://loinc.org",
        //                                code = "89487-3",
        //                                display = "Fluoroquinolone [Susceptibility] by Genotype method"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://loinc.org",
        //                                code = "89484-0",
        //                                display = "Amikacin [Susceptibility] by Genotype method"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //                {
        //                    code = new
        //                    {
        //                        coding = new List<object>
        //                        {
        //                            new
        //                            {
        //                                system = "http://loinc.org",
        //                                code = "89482-4",
        //                                display = "Kanamycin [Susceptibility] by Genotype method"
        //                            }
        //                        }
        //                    },
        //                    valueCodeableConcept = new
        //                    {
        //                        coding = new List<object>() {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code= "LA11882-0",
        //                               display= "Detected"
        //                           }
        //                        }
        //                    }
        //                },
        //             new
        //               {
        //                   code = new
        //                   {
        //                       coding = new List<object>
        //                       {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code = "89483-2",
        //                               display = "Capreomycin [Susceptibility] by Genotype method"
        //                           }
        //                       }
        //                   },
        //                   valueCodeableConcept = new
        //                   {
        //                       coding = new List<object>() {
        //                          new
        //                          {
        //                              system = "http://loinc.org",
        //                              code= "LA11882-0",
        //                              display= "Detected"
        //                          }
        //                       }
        //                   }
        //               },
        //             new
        //               {
        //                   code = new
        //                   {
        //                       coding = new List<object>
        //                       {
        //                           new
        //                           {
        //                               system = "http://loinc.org",
        //                               code = "96110-2",
        //                               display = "Ethionamide [Susceptibility] by Genotype method"
        //                           }
        //                       }
        //                   },
        //                   valueCodeableConcept = new
        //                   {
        //                       coding = new List<object>() {
        //                          new
        //                          {
        //                              system = "http://loinc.org",
        //                              code= "LA11882-0",
        //                              display= "Detected"
        //                          }
        //                       }
        //                   }
        //               }

        //        },
        //        note = new List<object>() {
        //            new
        //            {
        //                text = "Hasil menunjukkan positif TB"
        //            }
        //        }
        //        //if (ssResult == null)
        //        //{
        //        //    ssResult = new SatuSehatResult()
        //        //    {
        //        //        EncounterID = new Guid(encounterId),
        //        //        Category = transactionNo,
        //        //        Code = sequenceNo
        //        //    };
        //        //}
        //    };
        //    var requestBody = JsonConvert.SerializeObject(postData);
        //    return RestClientPostAndSaveLog("ServiceRequest", requestBody, ssResult, ref accessToken);
        //}
        private object DiagnosticReportTBPostData(Registration reg, PatientBridging patSs, ParamedicBridging parMedicSs, string encounterId)
        {
            var postData = new
            {
                resourceType = "DiagnosticReport",
                identifier = new List<object>
                {
                    new
                    {
                        system = string.Format("http://sys-ids.kemkes.go.id/diagnostic/{0}/lab", OrganizationID),
                        use = "official",
                        value = OrganizationID
                    }
                },
                status = "final",
                category = new List<object>
                {
                    new
                    {
                        coding = new List<object>
                        {
                            new
                            {
                                system = "http://terminology.hl7.org/CodeSystem/v2-0074",
                                code = "LAB",
                                display = "Laboratory"
                            }
                        }
                    }
                },
                code = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://terminology.kemkes.go.id/CodeSystem/examination",
                            code = "X099379",
                            display = "Pemeriksaaan TCM XDR"
                        }
                    }
                },
                subject = new
                {
                    reference = string.Format("Patient/{0}", patSs.BridgingID),
                    display = patSs.BridgingName
                },
                encounter = new
                {
                    reference = string.Format("Encounter/{0}", encounterId)
                },
                effectiveDateTime = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)), //tanggal nya
                issued = string.Format("{0}+00:00", reg.RegistrationDate.Value.AddHours(GmtDif).ToString(DateFormatLong)), //tanggal nya
                performer = new List<object>()
                {
                    new
                    {
                        reference = string.Format( "Practitioner/{0}",parMedicSs.BridgingID)
                    },
                    new
                    {
                        reference = string.Format( "Organization/{0}",parMedicSs.BridgingID), //ini diisi {{Divisi_Lab}}
                        display = "Divisi Laboratorium Puskesmas SATUSEHAT"
                    }
                },
                resultsInterpreter = new List<object>()
                {
                    new
                    {
                        reference = string.Format("Practitioner/{0}", parMedicSs.BridgingID)
                    }
                },
                result = new List<object>()
                {
                    new
                    {
                        reference = string.Format("Observation/{0}", parMedicSs.BridgingID), //{{Observation_TCMXDR003}}
                        display = "MTB detected"
                    }
                },
                specimen = new List<object>()
                {
                    new
                    {
                        reference = string.Format("Specimen/{0}", parMedicSs.BridgingID) //{{Specimen_TCMXDR003_Pagi}}
                    }
                },
                basedOn = new List<object>()
                {
                    new
                    {
                        reference = string.Format("ServiceRequest/{0}", parMedicSs.BridgingID) //{{ServiceRequest_TCMXDR003}}
                    }
                },
                conclusionCode = new
                {
                    coding = new List<object>
                    {
                        new
                        {
                            system = "http://loinc.org",
                            code = "LA11882-0",
                            display = "Detected"
                        }
                    }
                },
                conclusion = "MTB detected, H Dosis Rendah (Low) Susceptible, H Susceptible, FQ Dosis Rendah (Low) Susceptible, FQ Susceptible, Amk Susceptible, Km Susceptible, Cm Susceptible, Eto Susceptible"
            };
            return postData;
        }

        #endregion

        #region PTM(TIDAK DIBUAT)
        #endregion
        #endregion

        #region Satu Sehat Generic
        #region Utils
        public string SSDateYMD(DateTime dtm)
        {
            return string.Format("{0}+00:00", dtm.AddHours(GmtDif).ToString(DateFormatLong));
        }
        public string SSDateIdDDMMMMYYYY(DateTime dtm)
        {
            // Gunakan CultureInfo Indonesia
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("id-ID");
            // Format tanggal
            return dtm.ToString("dd MMMM yyyy", culture);
        }
        public string SSDateIdDDDDMMMMYYYY(DateTime dtm)
        {
            // Format tanggal
            return String.Format("{0}, {1}", SSDateIdDDDD(dtm), SSDateIdDDMMMMYYYY(dtm));
        }
        public string SSDateIdDDDD(DateTime dtm)
        {
            // Format tanggal
            return String.Format("{0}", DayNames[dtm.DayOfWeek.ToInt()]);
        }
        public string SSDateYYYYMMDD(DateTime dtm)
        {
            return dtm.ToString("yyyy-MM-dd");
        }
        #endregion
        #region Patient Bridging
        public string PatientBridging(string patientID, ref string accessToken, string fieldToReturn, bool isDev)
        {
            if (isDev)
            {
                switch (fieldToReturn)
                {
                    case "BridgingID": return "Dummy Patient ID";
                    case "BridgingName": return "Dummy Patient Name";
                    default: return "Dummy Patient unknown field";
                }
            }
            return PatientBridging(patientID, ref accessToken, fieldToReturn);
        }
        public string PatientBridging(string patientID, ref string accessToken, string fieldToReturn)
        {
            //object oVal = null;
            var patSs = new PatientBridging();
            if (!patSs.LoadByPrimaryKey(patientID, SatuSehatBridgingType) || string.IsNullOrWhiteSpace(patSs.BridgingID))
            {
                var pat = new Patient();
                if (pat.LoadByPrimaryKey(patientID))
                {
                    if (string.IsNullOrWhiteSpace(pat.Ssn))
                        throw new Exception(string.Format("SSN {0} not found for {1}", pat.Ssn, pat.PatientName));

                    var response = RestClientGet("Patient?identifier=https://fhir.kemkes.go.id/id", string.Concat("nik|", pat.Ssn), ref accessToken);
                    if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var patientSearchResponse = JsonConvert.DeserializeObject<Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.PatientSearch.PatientSearchResponse>(response.Content);
                        if (patientSearchResponse.Total == 1)
                        {
                            // Add PatientBridging
                            if (string.IsNullOrEmpty(patSs.PatientID))
                            {
                                patSs = new PatientBridging();
                            }

                            patSs.PatientID = patientID;
                            patSs.BridgingID = patientSearchResponse.Entry[0].Resource.Id;
                            patSs.BridgingName = pat.PatientName;
                            patSs.SRBridgingType = SatuSehatBridgingType;
                            patSs.IsActive = true;
                            patSs.Save();

                            //oVal = FieldValue(patSs, fieldToReturn);
                            //if (oVal != null) return oVal.ToString();
                            return FieldValue<string>(patSs, fieldToReturn);
                        }
                        else
                        {
                            throw new Exception(string.Format("SSN {0} not found at fhir.kemkes.go.id", pat.Ssn));
                            //satuSehatLog.ErrorResponse = string.Format("SSN {0} not found at fhir.kemkes.go.id", pat.Ssn);
                            //satuSehatLog.Save();
                            //return;
                        }
                    }
                }
                else
                {
                    throw new Exception(string.Format("Patient {0} not found.", patientID));
                }
            }

            //oVal = FieldValue(patSs, fieldToReturn);
            //if (oVal != null) return oVal.ToString();
            return FieldValue<string>(patSs, fieldToReturn);

            return string.Empty;
        }

        public string ParamedicBridging(string paramedicID, string fieldToReturn, bool isDev)
        {
            if (isDev)
            {
                switch (fieldToReturn)
                {
                    case "BridgingID": return "Dummy Paramedic ID";
                    case "BridgingName": return "Dummy Paramedic Name";
                    default: return "Dummy Paramedic unknown field";
                }
            }
            return ParamedicBridging(paramedicID, fieldToReturn);
        }
        public string ParamedicBridging(string paramedicID, string fieldToReturn)
        {
            var parMedSs = new ParamedicBridging();
            parMedSs.Query.Where(parMedSs.Query.ParamedicID == paramedicID, parMedSs.Query.SRBridgingType == SatuSehatBridgingType);
            parMedSs.Query.es.Top = 1;
            if (parMedSs.Query.Load())
            {
                return FieldValue<string>(parMedSs, fieldToReturn);
            }
            return string.Empty;
        }

        private T FieldValue<T>(object dataObject, string fieldToReturn)
        {
            if (string.IsNullOrWhiteSpace(fieldToReturn) || dataObject == null)
                return default;

            var propInfo = dataObject.GetType().GetProperty(fieldToReturn);
            if (propInfo != null)
            {
                var value = propInfo.GetValue(dataObject);
                if (value is T typedValue)
                    return typedValue;
            }

            return default;
        }

        //private object FieldValue(PatientBridging patSs, string fieldToReturn) {
        //    if (string.IsNullOrWhiteSpace(fieldToReturn)) fieldToReturn = "BridgingID";
        //    PropertyInfo propInfo = typeof(PatientBridging).GetProperty(fieldToReturn);

        //    if (propInfo != null)
        //    {
        //        object value = propInfo.GetValue(patSs);
        //        return value;
        //    }
        //    return null;
        //}
        #endregion
        #region Encounter
        public bool SendToSatuSehat(SatuSehatILPPreparation ssp, SatuSehatILPTemplateDetail sstd, ref string accessToken)
        {
            //bool testingDummy = true;
            try
            {
                Method method = (Method)Enum.Parse(typeof(Method), sstd.PostMethod, true);
                // tgl, sementara buat testing
                ssp.SentDateTime = DateTime.Now.NowAtSqlServer();

                //throw new Exception("test error aja");

                var response = RestClientExecute(ssp.PostData, sstd.PostUrl.Replace("{{base_url}}", BaseUrl), ref accessToken, method);

                if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ssp.IsSent = true;
                    ssp.IsError = false;
                    //ssp.SentDateTime = DateTime.Now;
                    ssp.RespondData = response.Content;
                    SyncKeSSLama(ssp, sstd, "");
                    return true;
                }
                else
                {
                    ssp.IsSent = true;
                    ssp.IsError = true;
                    ssp.RespondData = response.Content;
                    SyncKeSSLama(ssp, sstd, response.Content);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ssp.IsSent = false;
                ssp.IsError = true;
                ssp.RespondData = ex.Message;
                SyncKeSSLama(ssp, sstd, ex.Message);
                return false;
            }
        }

        private void SyncKeSSLama(SatuSehatILPPreparation ssp, SatuSehatILPTemplateDetail sstd, string errMsg)
        {
            if (sstd.PostUrl.EndsWith("Encounter", true, null))
            {
                // simpan encounter
                var satuSehatLog = new SatuSehatKunjungan();
                if (!satuSehatLog.LoadByPrimaryKey(ssp.RegistrationNo))
                    satuSehatLog = new SatuSehatKunjungan();

                satuSehatLog.KunjunganPostData = ssp.PostData;
                satuSehatLog.RegistrationNo = ssp.RegistrationNo;
                satuSehatLog.str.ErrorResponse = string.Empty;

                if (string.IsNullOrWhiteSpace(errMsg))
                {
                    var obj = JObject.Parse(ssp.RespondData);
                    string id = (string)obj["id"];

                    if (!string.IsNullOrEmpty(id))
                    {
                        ssp.AnswerText = id;
                        satuSehatLog.EncounterID = new Guid(id);
                    }
                }
                else
                {
                    satuSehatLog.ErrorResponse = errMsg;
                }

                satuSehatLog.Save();
            }
        }
        #endregion
        public string PatientPhysicalExam(string registrationNo, string ExamKeyowrd)
        {
            var valToReturn = string.Empty;
            var paq = new PatientAssessmentQuery("paq");
            paq.Where(paq.RegistrationNo == registrationNo);
            paq.Select(paq.PhysicalExam);
            paq.es.Top = 1;
            paq.OrderBy(paq.CreatedDateTime.Descending);
            var dtb = paq.LoadDataTable();
            if (dtb.Rows.Count > 0)
            {
                var json = dtb.Rows[0]["PhysicalExam"]?.ToString();
                if (!string.IsNullOrWhiteSpace(json))
                {
                    using (var doc = JsonDocument.Parse(json))
                    {
                        var root = doc.RootElement;

                        string rightSummary = ExtractSide(root, "Right", ExamKeyowrd);
                        string leftSummary = ExtractSide(root, "Left", ExamKeyowrd);

                        valToReturn = string.Join(" ; ", new[] { rightSummary, leftSummary }
                            .Where(x => !string.IsNullOrWhiteSpace(x)));
                        if (string.IsNullOrWhiteSpace(valToReturn))
                        {
                            valToReturn = ExtractTopLevel(root, ExamKeyowrd);
                        }
                    }

                }
            }
            if (string.IsNullOrWhiteSpace(valToReturn) && (ExamKeyowrd == "Telinga" || ExamKeyowrd == "Hidung" || ExamKeyowrd == "Tenggorok"))
            {
                string query = string.Format(
                    "SELECT CASE WHEN JSON_VALUE(PhysicalExam, '$.Tht.IsAbNormal') = 'true' " +
                    "THEN 'Abnormal : ' + JSON_VALUE(PhysicalExam, '$.Tht.Notes') " +
                    "ELSE 'Normal : ' + JSON_VALUE(PhysicalExam, '$.Tht.Notes') END AS THT_Status " +
                    "FROM PatientAssessment WHERE RegistrationNo = '{0}'",
                    registrationNo
                );
                var ret = (new QualityIndicatorSurveyCollection()).ExecuteQuery(query);
                if (ret.Rows.Count > 0 && ret.Rows[0][0] != DBNull.Value)
                    valToReturn = ret.Rows[0][0].ToString();
            }
            return valToReturn;
        }
        private string ExtractSide(JsonElement root, string side, string ExamKeyowrd)
        {
            if (!root.TryGetProperty(ExamKeyowrd, out JsonElement part) ||
                !part.TryGetProperty(side, out JsonElement sideElement))
                return "";

            var items = new List<string>();

            foreach (var prop in sideElement.EnumerateObject())
            {
                string value = prop.Value.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    items.Add($"{prop.Name} = {value}");
            }

            if (items.Count == 0)
                return "";

            string label = $"{ExamKeyowrd} {(side == "Right" ? "Kanan" : "Kiri")}";
            return $"{label}: {string.Join(" ; ", items)}";
        }
        private string ExtractTopLevel(JsonElement root, string ExamKeyowrd)
        {
            if (!root.TryGetProperty(ExamKeyowrd, out JsonElement part))
                return "";

            var items = new List<string>();
            foreach (var prop in part.EnumerateObject())
            {
                string value = prop.Value.GetString();
                if (!string.IsNullOrWhiteSpace(value))
                    items.Add($"{prop.Name} = {value}");
            }

            return items.Count > 0 ? $"{ExamKeyowrd}: {string.Join(" ; ", items)}" : null;
        }

        public string PatientRoomBed(string registrationNo, string columnToReturn)
        {
            var bq = new BedQuery("bq");
            bq.Where(bq.RegistrationNo == registrationNo);
            var dtb = bq.LoadDataTable();

            if (dtb.Rows.Count == 0)
                return null;

            var row = dtb.Rows[0];

            if (!dtb.Columns.Contains(columnToReturn))
                throw new ArgumentException($"Kolom '{columnToReturn}' tidak ditemukan dalam hasil query.");

            return row[columnToReturn]?.ToString();
        }
        public static DataTable LabObservationIDPackageItem(string registrationNo)
        {
            var serviceUnitLaboratoryID = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryID);
            var serviceUnitLaboratoryIdArray = AppParameter.GetParameterValue(AppParameter.ParameterItem.ServiceUnitLaboratoryIdArray);

            var query = new TransChargesItemQuery("tciq");
            var tcq = new TransChargesQuery("tcq");
            query.InnerJoin(tcq).On(query.TransactionNo == tcq.TransactionNo);
            var parent = new TransChargesItemQuery("tcipq");
            query.InnerJoin(parent).On(
                (query.TransactionNo == parent.TransactionNo) &
                (parent.SequenceNo == query.ParentNo)
            );
            query.Where(
                 tcq.RegistrationNo == registrationNo,
                 tcq.IsOrder == true,
                 parent.IsApprove == true,
                 parent.IsVoid == false,
                 parent.IsOrderRealization == true,
                 query.ResultValue.IsNotNull(),
                 query.ResultValue != "",
                 parent.SRCollectMethod.IsNotNull(),
                 parent.SRCollectMethod != "",
                 query.Or(
                      tcq.ToServiceUnitID == serviceUnitLaboratoryID,
                      tcq.ToServiceUnitID.In(serviceUnitLaboratoryIdArray)
                 )
            );
            query.Select(parent.TransactionNo, parent.SequenceNo);
            var dtb = query.LoadDataTable();

            var ssPrep = new SatuSehatILPPreparationQuery("ssP");
            ssPrep.Select(ssPrep.RespondData);
            ssPrep.Where(ssPrep.RegistrationNo == registrationNo, ssPrep.TestNo == "10.1.4.02");
            ssPrep.es.Top = 1;
            var dtb2 = ssPrep.LoadDataTable();
            if (dtb.Rows.Count < 1 || dtb2.Rows.Count < 1)
                return null;
            var jsonChek = dtb2.Rows[0][0]?.ToString()?.Trim();
            if (string.IsNullOrEmpty(jsonChek) || !IsValidJson(jsonChek))
                return null;
            bool validStructure = false;
            try
            {
                using (var doc = JsonDocument.Parse(jsonChek))
                {
                    if (doc.RootElement.TryGetProperty("entry", out var entries) && entries.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var e in entries.EnumerateArray())
                        {
                            if (e.TryGetProperty("response", out var response)
                                && response.TryGetProperty("resourceID", out var _))
                            {
                                validStructure = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }

            if (!validStructure)
                return null;
            var resourceIds = new List<string>();
            if (dtb2.Rows.Count > 0 && dtb2.Rows[0][0] != DBNull.Value)
            {
                var json = dtb2.Rows[0][0]?.ToString();
                if (IsValidJson(json))
                {
                    try
                    {
                        using (var doc = JsonDocument.Parse(json))
                        {
                            if (doc.RootElement.TryGetProperty("entry", out var entries) && entries.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var e in entries.EnumerateArray())
                                {
                                    if (e.TryGetProperty("response", out var response)
                                        && response.TryGetProperty("resourceID", out var ridProp))
                                    {
                                        var rid = ridProp.GetString();
                                        if (!string.IsNullOrWhiteSpace(rid))
                                            resourceIds.Add(rid);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ArgumentException(ex.Message);
                    }
                }
            }
            var merged = new DataTable();
            merged.Columns.Add("TransactionNo", typeof(string));
            merged.Columns.Add("ResourceID", typeof(string));

            var rowCount = Math.Min(dtb.Rows.Count, resourceIds.Count);
            for (int i = 0; i < rowCount; i++)
            {
                var col1 = dtb.Rows[i][0]?.ToString()?.Trim() ?? string.Empty;
                var col2 = dtb.Rows[i][1]?.ToString()?.Trim() ?? string.Empty;

                var trx = $"{col1}-{col2}";

                //var trx = dtb.Rows[i][0]?.ToString()?.Trim() ?? string.Empty;
                var rid = resourceIds[i]?.Trim() ?? string.Empty;
                merged.Rows.Add(trx, rid);
            }

            return merged;
        }
        private static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return false;

            try
            {
                using (JsonDocument.Parse(json)) { }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Realisasi Order
        public void SatusehatServiceRequestPostAndLogToLis(Registration reg, string transactionNo, string lisConnectionName)
        {
            var tc = new TransCharges();
            if (!tc.LoadByPrimaryKey(transactionNo)) return;

            // Stusehat post encounter & ServiceRequest
            OrderLabRealization(transactionNo);

            LogToLisInterop(reg, transactionNo, lisConnectionName);
        }

        public void LogToLisInterop(Registration reg, string transactionNo, string lisConnectionName)
        {
            // Update data sharing dengan LIS
            var satuSehatLog = new SatuSehatKunjungan();
            if (!satuSehatLog.LoadByPrimaryKey(reg.RegistrationNo)) return;

            if (!satuSehatLog.EncounterID.HasValue) return;

            // Load history Service Request
            var ssServiceReqs = new SatuSehatResultCollection();
            ssServiceReqs.Query.Where(ssServiceReqs.Query.EncounterID == satuSehatLog.EncounterID, ssServiceReqs.Query.ResourceType == "ServiceRequest", ssServiceReqs.Query.Category == transactionNo);
            ssServiceReqs.Query.Load();

            var satuSehatBridgingType = AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID);
            var patSs = new PatientBridging();
            patSs.LoadByPrimaryKey(reg.PatientID, satuSehatBridgingType);

            var parSs = new ParamedicBridging();
            parSs.Query.Where(parSs.Query.ParamedicID == reg.ParamedicID, parSs.Query.SRBridgingType == satuSehatBridgingType);
            parSs.Query.es.Top = 1;
            parSs.Query.Load();

            var seqNos = string.Empty;
            var itemIds = string.Empty;
            var itemNames = string.Empty;
            var loincIds = string.Empty;
            var loincNames = string.Empty;
            var servReqs = string.Empty;
            var specimens = string.Empty;
            var itemCount = 0;
            var isSpecimenExist = false;

            foreach (var ssResult in ssServiceReqs)
            {
                var tci = new TransChargesItem();
                if (!tci.LoadByPrimaryKey(transactionNo, ssResult.Code)) continue;

                if (tci.IsOrderRealization == null || tci.IsOrderRealization == false) continue;

                itemCount++;
                var item = new Item();
                item.LoadByPrimaryKey(tci.ItemID);

                // Hanya item Lab
                if (item.SRItemType != ItemType.Laboratory) continue;

                seqNos = string.Concat(seqNos, tci.SequenceNo, "~");

                switch (lisConnectionName)
                {
                    #region SYSMEX_LIS_INTEROP_CONNECTION_NAME
                    case AppConstant.HIS_INTEROP.MEDICLAB_LIS_INTEROP_CONNECTION_NAME:
                    case AppConstant.HIS_INTEROP.VANSLAB_LIS_INTEROP_CONNECTION_NAME:
                    case AppConstant.HIS_INTEROP.SYSMEX_LIS_INTEROP_CONNECTION_NAME:
                        itemIds = string.Concat(itemIds, item.ItemIDExternal, "~");
                        break;
                    #endregion 
                    case AppConstant.HIS_INTEROP.WYNAKOM_LIS_INTEROP_CONNECTION_NAME:
                        itemIds = string.Concat(itemIds, item.ItemID, "~");
                        break;
                    default:
                        itemIds = string.Concat(itemIds, item.ItemID, "~"); // Pakai versi Wynakom dulu
                        break;
                }

                itemNames = string.Concat(itemNames, item.ItemName, "~");

                var itemBg = new ItemBridging();
                itemBg.Query.Where(itemBg.Query.ItemID == tci.ItemID, itemBg.Query.SRBridgingType == satuSehatBridgingType);
                itemBg.Query.es.Top = 1;
                itemBg.Query.Load();

                loincIds = string.Concat(loincIds, itemBg.BridgingID, "~");
                loincNames = string.Concat(loincNames, itemBg.BridgingName, "~");
                servReqs = string.Concat(servReqs, ssResult.ResultID.ToString(), "~");

                // Specimen
                var ssSpecimen = new SatuSehatResult();
                ssSpecimen.Query.Where(ssSpecimen.Query.EncounterID == satuSehatLog.EncounterID, ssSpecimen.Query.ResourceType == "Specimen", ssSpecimen.Query.Category == transactionNo, ssSpecimen.Query.Code == tci.SequenceNo);
                ssSpecimen.Query.es.Top = 1;
                if (ssSpecimen.Query.Load())
                {
                    isSpecimenExist = true;
                    specimens = string.Concat(specimens, ssSpecimen.ResultID.ToString(), "~");
                }
                else
                    specimens = string.Concat(specimens, "~");
            }

            if (!string.IsNullOrWhiteSpace(itemIds))
            {
                var orderNumber = string.Empty;
                switch (lisConnectionName)
                {
                    #region SYSMEX_LIS_INTEROP_CONNECTION_NAME
                    case AppConstant.HIS_INTEROP.MEDICLAB_LIS_INTEROP_CONNECTION_NAME:
                    case AppConstant.HIS_INTEROP.VANSLAB_LIS_INTEROP_CONNECTION_NAME:
                    case AppConstant.HIS_INTEROP.SYSMEX_LIS_INTEROP_CONNECTION_NAME:
                        orderNumber = transactionNo;
                        break;
                    #endregion 
                    case AppConstant.HIS_INTEROP.WYNAKOM_LIS_INTEROP_CONNECTION_NAME:
                        orderNumber = string.Format("{0}^{1:000}", transactionNo, itemCount);
                        break;

                    default:
                        orderNumber = string.Format("{0}^{1:000}", transactionNo, itemCount); // Pakai versi Wynakom dulu
                        break;
                }

                if (AppParameter.IsYes(AppParameter.ParameterItem.IsSatusehatLisDataSharePerItemLab))
                {
                    var itemIdArr = itemIds.Split('~');
                    var itemNameArr = itemNames.Split('~');
                    var loincIdArr = loincIds.Split('~');
                    var loincNameArr = loincNames.Split('~');
                    var servReqArr = servReqs.Split('~');
                    var specimenArr = specimens.Split('~');
                    var seqNoArr = seqNos.Split('~');

                    var count = itemIdArr.Length;

                    for (int i = 0; i < count; i++)
                    {
                        AddToSatusehatOrderedItems(lisConnectionName, satuSehatLog, patSs, parSs, itemIdArr[i], itemNameArr[i], loincIdArr[i], loincNameArr[i], servReqArr[i], specimenArr[i], orderNumber, seqNoArr[i]);
                    }
                }
                else
                {
                    itemIds = itemIds.Remove(itemIds.Length - 1);
                    seqNos = string.Empty; // Untuk order yg digabung jadi 1 record, info seqno nya diisi kosong
                    itemNames = itemNames.Remove(itemNames.Length - 1);

                    if (!string.IsNullOrEmpty(loincIds))
                        loincIds = loincIds.Remove(loincIds.Length - 1);

                    if (!string.IsNullOrEmpty(loincNames))
                        loincNames = loincNames.Remove(loincNames.Length - 1);

                    if (!string.IsNullOrEmpty(servReqs))
                        servReqs = servReqs.Remove(servReqs.Length - 1);

                    if (isSpecimenExist && !string.IsNullOrEmpty(specimens))
                        specimens = specimens.Remove(specimens.Length - 1);
                    else
                        specimens = string.Empty;

                    AddToSatusehatOrderedItems(lisConnectionName, satuSehatLog, patSs, parSs, itemIds, itemNames, loincIds, loincNames, servReqs, specimens, orderNumber, seqNos);
                }
            }
        }

        private static void AddToSatusehatOrderedItems(string lisConnectionName, SatuSehatKunjungan satuSehatLog, PatientBridging patSs, ParamedicBridging parSs, string itemId, string itemName, string loinscId, string loinscName, string servReq, string specimen, string orderNumber, string seqNos)
        {
            var ssItem = new Temiang.Avicenna.BusinessObject.Interop.SatusehatOrderedItems(); // Saat ini menggunakan table yang sama untuk beberapa macam LIS
            ssItem.es.Connection.Name = lisConnectionName;

            if (!ssItem.LoadByPrimaryKey(orderNumber, seqNos))
            {
                ssItem = new Temiang.Avicenna.BusinessObject.Interop.SatusehatOrderedItems();
                ssItem.es.Connection.Name = lisConnectionName;
                ssItem.OrderSequenceNo = seqNos;
            }

            ssItem.OrderNumber = orderNumber;
            ssItem.SSEncounterID = satuSehatLog.EncounterID.ToString();

            ssItem.SSPatientID = patSs.BridgingID;
            ssItem.SSPatientName = patSs.BridgingName;

            ssItem.SSRequesterPractionerID = parSs.BridgingID;
            ssItem.SSRequesterPractionerName = parSs.BridgingName;

            ssItem.OrderItemID = itemId;
            ssItem.OrderItemName = itemName;

            if (!string.IsNullOrEmpty(loinscId))
                ssItem.SSLoincID = loinscId;

            if (!string.IsNullOrEmpty(loinscName))
                ssItem.SSLoincName = loinscName;

            if (string.IsNullOrEmpty(servReq))
                ssItem.str.SSServiceRequestID = string.Empty;
            else
                ssItem.SSServiceRequestID = servReq;

            if (string.IsNullOrEmpty(specimen))
                ssItem.str.SSSpecimenID = string.Empty;
            else
                ssItem.SSSpecimenID = specimen;

            ssItem.Save();
        }

        public string OrderLabRealization(string labOrderNo)
        {
            var org = new AppParameter();
            if (!org.LoadByPrimaryKey("SatuSehatOrganizationID")) return OrderLabInfVal("error", "10", "SatuSehatOrganizationID empty");

            var tc = new TransCharges();
            if (!tc.LoadByPrimaryKey(labOrderNo)) return OrderLabInfVal("error", "10", "Lab Order not found");

            var encounterId = string.Empty;
            var satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(tc.RegistrationNo))
            {
                if (satuSehatLog.EncounterID != null)
                {
                    encounterId = satuSehatLog.EncounterID.ToString();
                }
                else
                    encounterId = EncounterAndLabOrderPost(tc.RegistrationNo);
            }
            else
            {
                encounterId = EncounterAndLabOrderPost(tc.RegistrationNo);
            }

            if (!string.IsNullOrEmpty(encounterId))
            {
                var reg = new Registration();
                reg.LoadByPrimaryKey(tc.RegistrationNo);

                var satuSehatBridgingType = AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID);

                var patSs = new PatientBridging();
                if (!patSs.LoadByPrimaryKey(reg.PatientID, satuSehatBridgingType)) return OrderLabInfVal("error", "10", "Patient bridging not found");

                var parMedicSs = new ParamedicBridging();
                var pbQr = new ParamedicBridgingQuery("pb");
                pbQr.Where(pbQr.ParamedicID == reg.ParamedicID, pbQr.SRBridgingType == satuSehatBridgingType);
                pbQr.es.Top = 1;
                parMedicSs = new ParamedicBridging();
                if (!parMedicSs.Load((pbQr))) return OrderLabInfVal("error", "10", "Requester bridging not found");

                return OrderLabInfVal("success", "", "", encounterId, patSs.BridgingID, patSs.BridgingName, parMedicSs.BridgingID, parMedicSs.BridgingName, org.ParameterValue);
            }

            satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(tc.RegistrationNo))
            {
                if (!string.IsNullOrEmpty(satuSehatLog.ErrorResponse))
                    return OrderLabInfVal("error", "10", satuSehatLog.ErrorResponse);
            }
            return OrderLabInfVal("error", "10", "Please contact IT Support");
        }

        private string OrderLabInfVal(string issueSeverity, string issueCode, string issueErrorMessage, string encounterId = "", string patientId = "", string patientName = "", string requesterId = "", string requesterName = "", string organizationId = "", string serviceRequestId = "", string specimenId = "")
        {
            var val = new
            {
                issue = new { severity = issueSeverity, code = issueCode, text = issueErrorMessage },
                satuSehat = new
                {
                    encounterId = encounterId,
                    patientId = patientId,
                    patientName = patientName,
                    requesterId = requesterId,
                    requesterName = requesterName,
                    organizationId = organizationId,
                    serviceRequestId = serviceRequestId,
                    specimenId = specimenId
                }
            };
            return JsonConvert.SerializeObject(val);
        }

        private string EncounterAndLabOrderPost(string registrationNo)
        {
            string accessToken = string.Empty;
            Registration reg = null;
            PatientBridging patSs = null;
            ParamedicBridging parMedicSs = null;
            SatuSehatKunjungan satuSehatLog = null;
            ServiceUnitBridging locSs = null;

            // Step 1 Post Encounter
            var encounterId = EncounterPost(registrationNo, ref satuSehatLog, ref reg, ref patSs, ref parMedicSs, ref locSs, ref accessToken, "", false);

            if (!string.IsNullOrEmpty(encounterId))
            {
                var dtbDiagnosisResult = PostDiagnosis(reg, patSs, encounterId, ref accessToken);

                // Step 2 Post Service Request and Specimen
                PostServiceRequest(reg, patSs, parMedicSs, encounterId, ref accessToken);
            }
            return encounterId;
        }

        #region RAD
        private string EncounterAndRadOrderPost(string registrationNo)
        {
            string accessToken = string.Empty;
            Registration reg = null;
            PatientBridging patSs = null;
            ParamedicBridging parMedicSs = null;
            SatuSehatKunjungan satuSehatLog = null;
            ServiceUnitBridging locSs = null;

            // Step 1 Post Encounter
            var encounterId = EncounterPost(registrationNo, ref satuSehatLog, ref reg, ref patSs, ref parMedicSs, ref locSs, ref accessToken, "", false);

            if (!string.IsNullOrEmpty(encounterId))
            {
                var dtbDiagnosisResult = PostDiagnosis(reg, patSs, encounterId, ref accessToken);

                PostServiceRequestRad(reg, patSs, parMedicSs, encounterId, ref accessToken);
            }
            return encounterId;
        }

        public string OrderRadRealization(string radOrderNo)
        {
            var org = new AppParameter();
            if (!org.LoadByPrimaryKey("SatuSehatOrganizationID")) return OrderRadInfVal("error", "10", "SatuSehatOrganizationID empty");

            var tc = new TransCharges();
            if (!tc.LoadByPrimaryKey(radOrderNo)) return OrderRadInfVal("error", "10", "Rad Order not found");

            var encounterId = string.Empty;
            var satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(tc.RegistrationNo))
            {
                if (satuSehatLog.EncounterID != null)
                {
                    encounterId = satuSehatLog.EncounterID.ToString();
                }
                else
                    encounterId = EncounterAndRadOrderPost(tc.RegistrationNo);
            }
            else
            {
                encounterId = EncounterAndRadOrderPost(tc.RegistrationNo);
            }

            if (!string.IsNullOrEmpty(encounterId))
            {
                var reg = new Registration();
                reg.LoadByPrimaryKey(tc.RegistrationNo);

                var satuSehatBridgingType = AppParameter.GetParameterValue(AppParameter.ParameterItem.SatuSehatBridgingTypeID);

                var patSs = new PatientBridging();
                if (!patSs.LoadByPrimaryKey(reg.PatientID, satuSehatBridgingType)) return OrderRadInfVal("error", "10", "Patient bridging not found");

                var parMedicSs = new ParamedicBridging();
                var pbQr = new ParamedicBridgingQuery("pb");
                pbQr.Where(pbQr.ParamedicID == reg.ParamedicID, pbQr.SRBridgingType == satuSehatBridgingType);
                pbQr.es.Top = 1;
                parMedicSs = new ParamedicBridging();
                if (!parMedicSs.Load((pbQr))) return OrderRadInfVal("error", "10", "Requester bridging not found");

                return OrderRadInfVal("success", "", "", encounterId, patSs.BridgingID, patSs.BridgingName, parMedicSs.BridgingID, parMedicSs.BridgingName, org.ParameterValue);
            }

            satuSehatLog = new SatuSehatKunjungan();
            if (satuSehatLog.LoadByPrimaryKey(tc.RegistrationNo))
            {
                if (!string.IsNullOrEmpty(satuSehatLog.ErrorResponse))
                    return OrderRadInfVal("error", "10", satuSehatLog.ErrorResponse);
            }
            return OrderRadInfVal("error", "10", "Please contact IT Support");
        }

        private string OrderRadInfVal(string issueSeverity, string issueCode, string issueErrorMessage, string encounterId = "", string patientId = "", string patientName = "", string requesterId = "", string requesterName = "", string organizationId = "", string serviceRequestId = "")
        {
            var val = new
            {
                issue = new { severity = issueSeverity, code = issueCode, text = issueErrorMessage },
                satuSehat = new
                {
                    encounterId = encounterId,
                    patientId = patientId,
                    patientName = patientName,
                    requesterId = requesterId,
                    requesterName = requesterName,
                    organizationId = organizationId,
                    serviceRequestId = serviceRequestId
                }
            };
            return JsonConvert.SerializeObject(val);
        }
        #endregion

        #endregion
    }
}
