using DevExpress.Text.Interop;
using DevExpress.XtraPrinting;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.Pdf;
using Telerik.Web.UI;
using Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.Kfa;
using Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.MedicationRequestResponse;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Util
{
    public class KeyWords
    {
        public int TemplateID { get; set; }
        public string TestNo { get; set; }
        public int Sequence { get; set; }
        public string KeyWord { get; set; }
        public int Count { get; set; }
    }
    public static class SatuSehatHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationNo"></param>
        /// <param name="TemplateID"></param>
        /// <param name="replaceFlag">0:Skip replace variabel, 1:Replace hanya variabel yang masih kosong, 2:Replace ulang semua variabel</param>
        /// <returns></returns>
        public static SatuSehatILPPreparationCollection SatuSehatPreparation(string RegistrationNo, int TemplateID, int replaceFlag)
        {
            var ilpTdColl = new SatuSehatILPTemplateDetailCollection();
            ilpTdColl.Query.Where(ilpTdColl.Query.TemplateID == TemplateID)
                .OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
            ilpTdColl.LoadAll();

            var ilpPrepColl = new SatuSehatILPPreparationCollection();
            var ilpPrepQ = ilpPrepColl.Query;
            ilpPrepQ.Where(ilpPrepQ.RegistrationNo == RegistrationNo, ilpPrepQ.TemplateID == TemplateID);
            ilpPrepColl.LoadAll();

            SatuSehatPreparation(RegistrationNo, TemplateID, ilpTdColl, ilpPrepColl, replaceFlag);

            return ilpPrepColl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationNo"></param>
        /// <param name="TemplateID"></param>
        /// <param name="TestNo"></param>
        /// <param name="SequenceNo"></param>
        /// <param name="replaceFlag">0:Skip replace variabel, 1:Replace hanya variabel yang masih kosong, 2:Replace ulang semua variabel</param>
        /// <returns></returns>
        public static SatuSehatILPPreparationCollection SatuSehatPreparation(string RegistrationNo, int TemplateID, string TestNo, int SequenceNo, int replaceFlag)
        {
            var ilpTdColl = new SatuSehatILPTemplateDetailCollection();
            ilpTdColl.Query.Where(
                ilpTdColl.Query.TemplateID == TemplateID,
                ilpTdColl.Query.TestNo == TestNo,
                ilpTdColl.Query.Sequence == SequenceNo
            ).OrderBy(ilpTdColl.Query.TestNo.Ascending, ilpTdColl.Query.Sequence.Ascending);
            ilpTdColl.LoadAll();

            var ilpPrepColl = new SatuSehatILPPreparationCollection();
            var ilpPrepQ = ilpPrepColl.Query;
            ilpPrepQ.Where(
                ilpPrepQ.RegistrationNo == RegistrationNo,
                ilpPrepQ.TemplateID == TemplateID,
                ilpPrepQ.TestNo == TestNo,
                ilpPrepQ.Sequence == SequenceNo);
            ilpPrepColl.LoadAll();

            SatuSehatPreparation(RegistrationNo, TemplateID, ilpTdColl, ilpPrepColl, replaceFlag);

            return ilpPrepColl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RegistrationNo"></param>
        /// <param name="TemplateID"></param>
        /// <param name="ilpTdColl"></param>
        /// <param name="ilpPrepColl"></param>
        /// <param name="replaceFlag">0:Skip replace variabel, 1:Replace hanya variabel yang masih kosong, 2:Replace ulang semua variabel</param>
        private static void SatuSehatPreparation(string RegistrationNo, int TemplateID, SatuSehatILPTemplateDetailCollection ilpTdColl,
            SatuSehatILPPreparationCollection ilpPrepColl, int replaceFlag)
        {
            foreach (var ilpTd in ilpTdColl)
            {

                SatuSehatILPPreparation ilpPrep = null;
                ilpPrep = ilpPrepColl.Where(i => i.TestNo == ilpTd.TestNo && i.Sequence == ilpTd.Sequence).FirstOrDefault();
                if (ilpPrep == null)
                {
                    ilpPrep = ilpPrepColl.AddNew();
                    ilpPrep.RegistrationNo = RegistrationNo;
                    ilpPrep.TemplateID = TemplateID;
                    ilpPrep.TestNo = ilpTd.TestNo;
                    ilpPrep.Sequence = ilpTd.Sequence;
                    ilpPrep.AnswerValue = "";
                    ilpPrep.AnswerText = "";
                    ilpPrep.PostData = ilpTd.PostJsonTemplate;
                    ilpPrep.IsSent = false;
                    ilpPrep.IsError = false;
                    ilpPrep.RespondData = "";

                    ilpPrep.CreateByUserID = AppSession.UserLogin.UserID;
                    ilpPrep.CreateDateTime = DateTime.Now;
                    ilpPrep.LastUpdateByUserID = AppSession.UserLogin.UserID;
                    ilpPrep.LastUpdateDateTime = DateTime.Now;
                }
                if (replaceFlag == 2)
                {
                    if ((ilpPrep.IsSent ?? false) && !(ilpPrep.IsError ?? false))
                        continue; // sudah terkirim dan tidak error, tidak boleh diubah lagi
                    // tarik ulang template
                    ilpPrep.PostData = ilpTd.PostJsonTemplate;
                }
            }

            if (replaceFlag > 0) ReplaceVariables(RegistrationNo, ilpPrepColl, true, TemplateID, ilpTdColl);

            ilpPrepColl.Save();
        }

        public static void ReplaceVariables(string RegistrationNo, SatuSehatILPPreparationCollection ilpPrepColl, bool isInit,
            int TemplateID, SatuSehatILPTemplateDetailCollection ilpTdColl)
        {
            var templateKeywords = new SatuSehatILPTemplateDetailKeyWordCollection();
            templateKeywords.Query.Where(
                templateKeywords.Query.Or(
                    templateKeywords.Query.TemplateID == TemplateID,
                    templateKeywords.Query.TemplateID == 0
                )
            );
            templateKeywords.LoadAll();

            foreach (var ilpPrep in ilpPrepColl.Where(ilp => !(ilp.IsSent ?? false) || (ilp.IsError ?? false)))
            {
                var ilpTd = ilpTdColl.Where(i => i.TemplateID == ilpPrep.TemplateID && i.TestNo == ilpPrep.TestNo &&
                    i.Sequence == ilpPrep.Sequence).FirstOrDefault();

                ilpPrep.PostData = ReplaceVariables(RegistrationNo, ilpPrep, ilpPrep.PostData, ilpTd, isInit, templateKeywords, 1);
            }
        }

        private static string ReplaceVariables(string RegistrationNo, SatuSehatILPPreparation ilpPrep, string StringSource,
            SatuSehatILPTemplateDetail ilpTd, bool isInit, SatuSehatILPTemplateDetailKeyWordCollection templateKeywords, int Depth)
        {
            if (Depth >= 10) return StringSource;// failsafe infinite loop

            List<KeyWords> kwColl = new List<KeyWords>();
            if (!string.IsNullOrWhiteSpace(StringSource))
            {
                MatchCollection matches = Regex.Matches(StringSource, @"\{\{(.*?)\}\}");
                foreach (Match match in matches)
                {
                    var str = match.Groups[1].Value;

                    KeyWords kw = kwColl.Where(k =>
                        k.TemplateID == ilpPrep.TemplateID &&
                        k.TestNo == ilpPrep.TestNo &&
                        k.Sequence == ilpPrep.Sequence &&
                        k.KeyWord == str
                    ).FirstOrDefault();

                    if (kw == null)
                    {
                        kw = new KeyWords();
                        kwColl.Add(kw);

                        kw.TemplateID = ilpPrep.TemplateID.Value;
                        kw.TestNo = ilpPrep.TestNo;
                        kw.Sequence = ilpPrep.Sequence.Value;
                        kw.KeyWord = str;
                        kw.Count = 0;
                    }
                    kw.Count++;
                }
            }
            _placeholderCleaned = false;
            foreach (var kw in kwColl)
            {
                // cari full sesuai template
                var tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower() && t.TemplateID == ilpPrep.TemplateID).FirstOrDefault();
                if (tk == null)
                {
                    // cari tanpa tipe data penyerta
                    tk = templateKeywords.Where(t => t.KeyWord.ToLower().Split(':')[0] == kw.KeyWord.ToLower() && t.TemplateID == ilpPrep.TemplateID).FirstOrDefault();
                    if (tk == null)
                    {
                        tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split(':')[0] && t.TemplateID == ilpPrep.TemplateID).FirstOrDefault();
                        if (tk == null)
                        {
                            // cari di template umum
                            tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower() && t.TemplateID == 0).FirstOrDefault();
                            if (tk == null)
                            {
                                // cari tanpa tipe data penyerta
                                tk = templateKeywords.Where(t => t.KeyWord.ToLower().Split(':')[0] == kw.KeyWord.ToLower() && t.TemplateID == 0).FirstOrDefault();
                                if (tk == null)
                                {
                                    tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split(':')[0] && t.TemplateID == 0).FirstOrDefault();
                                    if (tk == null)
                                    {
                                        // kalau masih null brarti blm ada setting variabelnya
                                    }
                                }
                            }
                        }
                    }
                }
                // cari yang pakai titik (untuk case combobox)
                if (tk == null)
                {
                    tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split('.')[0] && t.TemplateID == ilpPrep.TemplateID).FirstOrDefault();
                    if (tk == null)
                    {
                        tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split('.')[0] && t.TemplateID == 0).FirstOrDefault();
                        //if (tk == null)
                        //{
                        //    // kalau masih null brarti blm ada setting variabelnya
                        //}
                    }
                }
                // cari yang pakai - (untuk case data lebih dari 1)
                if (tk == null)
                {
                    tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split('-')[0] && t.TemplateID == ilpPrep.TemplateID).FirstOrDefault();
                    if (tk == null)
                    {
                        tk = templateKeywords.Where(t => t.KeyWord.ToLower() == kw.KeyWord.ToLower().Split('-')[0] && t.TemplateID == 0).FirstOrDefault();
                        if (tk == null)
                        {
                            // kalau masih null brarti blm ada setting variabelnya
                        }
                    }
                }
                if (tk != null)
                {
                    bool isTextOnly = !((tk.IsQuestionAnswer ?? false) && (ilpTd.SRAnswerType.ToLower() == "cbo"));
                    if (!string.IsNullOrWhiteSpace(tk.Source))
                    {
                        // if has another variable
                        tk.Source = ReplaceVariables(RegistrationNo, ilpPrep, tk.Source, ilpTd, isInit, templateKeywords, Depth + 1);

                        tk.Source = tk.Source.ReplaceWholeWord("@RegistrationNo", RegistrationNo);
                        tk.Source = tk.Source.ReplaceWholeWord("@TemplateID", ilpTd.TemplateID.ToString());
                        tk.Source = tk.Source.ReplaceWholeWord("@TestNo", ilpTd.TestNo);
                        tk.Source = tk.Source.ReplaceWholeWord("@Sequence", ilpTd.Sequence.ToString());

                        //object val_ = DBNull.Value;

                        if (!tk.IsExecuted_)
                        {
                            //jika sudah pernah execute jangan execute lagi supaya tidak lemot
                            //var jsoncontoh = "{\r\n    \"entry\": [\r\n        {\r\n            \"response\": {\r\n                \"etag\": \"W/\\\"MTc1MzkzNTY4NTM1MzIzOTAwMA\\\"\",\r\n                \"lastModified\": \"2025-07-31T04:21:25.353239+00:00\",\r\n                \"location\": \"https://api-satusehat-stg.dto.kemkes.go.id/fhir-r4/v1/AllergyIntolerance/d59a46cc-be28-49df-98fd-3973e87d68c0/_history/MTc1MzkzNTY4NTM1MzIzOTAwMA\",\r\n                \"status\": \"201 Created\",\r\n                \"resourceType\": \"AllergyIntolerance\",\r\n                \"resourceID\": \"d59a46cc-be28-49df-98fd-3973e87d68c0\"\r\n            }\r\n        },\r\n        {\r\n            \"response\": {\r\n                \"etag\": \"W/\\\"MTc1MzkzNTY4NTM1MzIzOTAwMA\\\"\",\r\n                \"lastModified\": \"2025-07-31T04:21:25.353239+00:00\",\r\n                \"location\": \"https://api-satusehat-stg.dto.kemkes.go.id/fhir-r4/v1/AllergyIntolerance/cc20cee0-c8aa-4603-8729-30d2c15fb56c/_history/MTc1MzkzNTY4NTM1MzIzOTAwMA\",\r\n                \"status\": \"201 Created\",\r\n                \"resourceType\": \"AllergyIntolerance\",\r\n                \"resourceID\": \"cc20cee0-c8aa-4603-8729-30d2c15fb56c\"\r\n            }\r\n        }\r\n    ],\r\n    \"resourceType\": \"Bundle\",\r\n    \"type\": \"transaction-response\",\r\n    \"total\": 2\r\n}";
                            //var results = Temiang.Avicenna.Bridging.SatuSehat.Utils.LabObservationIDPackageItem(RegistrationNo);
                            switch (tk.SourceType.ToLower())
                            {
                                case "sql":
                                    {
                                        // cek dulu kalau masih ada container di parameter brarti belum bisa exe function karena parameter blm ada valid value
                                        if (tk.Source.Contains("{{") && tk.Source.Contains("}}"))
                                        {
                                            // belum boleh invoke function karena masih ada placeholder yang belum keisi
                                        }
                                        else
                                        {
                                            if (!IsSqlSafe(tk.Source) || !IsSelectOnly(tk.Source))
                                                throw new Exception("Query tidak aman dan diblokir.");

                                            var ret = (new QualityIndicatorSurveyCollection()).ExecuteQuery(tk.Source);
                                            if (ret.Columns.Count > 2)
                                            {
                                                tk.IsExecuted_ = true;
                                                tk.Value_ = string.Join(";", ret.Rows
                                                       .Cast<DataRow>()
                                                       .Select(r => r[0]?.ToString()?.Trim() ?? "")
                                                    );
                                                tk.Text_ = string.Join(";", ret.Rows
                                                   .Cast<DataRow>()
                                                   .Select(r => r[1]?.ToString()?.Trim() ?? "")
                                                );
                                                tk.Helper_ = string.Join(";", ret.Rows
                                                   .Cast<DataRow>()
                                                   .Select(r => r[2]?.ToString()?.Trim() ?? "")
                                                );
                                            }
                                            else if (ret.Rows.Count > 0)
                                            {
                                                //val_ = ret.Rows[0][0];
                                                //text_ = val_;
                                                tk.IsExecuted_ = true;
                                                if (ret.Columns.Count > 1)
                                                {
                                                    tk.Value_ = string.Join(";", ret.Rows
                                                       .Cast<DataRow>()
                                                       .Select(r => r[0]?.ToString()?.Trim() ?? "")
                                                    );
                                                    tk.Text_ = string.Join(";", ret.Rows
                                                       .Cast<DataRow>()
                                                       .Select(r => r[1]?.ToString()?.Trim() ?? "")
                                                    );
                                                }
                                                else if (tk.IsMultipleAnswers ?? false)
                                                {
                                                    tk.Value_ = string.Join(";", ret.Rows
                                                       .Cast<DataRow>()
                                                       .Select(r => r[0]?.ToString()?.Trim() ?? "")
                                                    );
                                                    if (ret.Columns.Count > 1)
                                                    {
                                                        tk.Text_ = string.Join(";", ret.Rows
                                                           .Cast<DataRow>()
                                                           .Select(r => r[1]?.ToString()?.Trim() ?? "")
                                                        );
                                                    }
                                                    else
                                                        tk.Text_ = tk.Value_;
                                                }
                                                //else if (ret.Columns.Count > 1 && ret.Rows.Count == 1 && (tk.IsMultipleAnswers ?? false))
                                                //{
                                                //    tk.Text_ = ret.Rows[0][1]?.ToString()?.Trim();
                                                //}
                                                else
                                                {
                                                    tk.Value_ = ret.Rows[0][0];
                                                    tk.Text_ = tk.Value_;
                                                }

                                                if (tk.Text_ == null)
                                                {
                                                    //text_ = ret.Rows[0][1];
                                                    tk.Text_ = tk.Value_;
                                                }
                                                else
                                                {
                                                    if (ilpTd.SRAnswerType.ToLower() == "cbo")
                                                    {
                                                        Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(ilpTd.AnswerSelection);
                                                        var teks = dict.Where(d => d.Key.ToLower() == tk.Value_.ToString().ToLower()/*val_.ToString().ToLower()*/).Select(d => d.Value).FirstOrDefault();
                                                        if (!string.IsNullOrWhiteSpace(teks))
                                                        {
                                                            //text_ = teks;
                                                            tk.Text_ = teks;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case "code":
                                    {
                                        // cek dulu kalau masih ada container di parameter brarti belum bisa exe function karena parameter blm ada valid value
                                        if (tk.Source.Contains("{{") && tk.Source.Contains("}}"))
                                        {
                                            // belum boleh invoke function karena masih ada placeholder yang belum keisi
                                        }
                                        else
                                        {
                                            tk.IsExecuted_ = true;
                                            var result = Util.DynamicInvoker.InvokeFromString(tk.Source);
                                            //tk.Text_ = Util.DynamicInvoker.InvokeFromString(tk.Source);
                                            if (result is System.Data.DataTable dt && tk.IsMultipleAnswers == true)
                                            {
                                                var values = dt.AsEnumerable()
                                                               .Select(r => (r[0]?.ToString() ?? "").Trim())
                                                               .Where(s => !string.IsNullOrWhiteSpace(s));
                                                var texts = dt.AsEnumerable()
                                                               .Select(r => (r[1]?.ToString() ?? "").Trim())
                                                               .Where(s => !string.IsNullOrWhiteSpace(s));

                                                tk.Value_ = string.Join(";", values);
                                                tk.Text_ = string.Join(";", texts);
                                            }
                                            else if (result is IEnumerable<string> listOfStrings)
                                            {
                                                var val = string.Join(";", listOfStrings.Where(s => !string.IsNullOrWhiteSpace(s)));
                                                tk.Value_ = val;
                                                tk.Text_ = val;
                                            }
                                            else
                                            {
                                                tk.Text_ = result?.ToString();
                                                tk.Value_ = tk.Text_;
                                            }
                                        }
                                        break;
                                    }
                                default:
                                    {
                                        //text_ = tk.Source;
                                        tk.IsExecuted_ = true;
                                        tk.Text_ = tk.Source;
                                        break;
                                    }
                            }
                        }


                        if (tk.Text_/*text_*/ != null && tk.Text_/*text_*/ != DBNull.Value)
                        {
                            if (tk.IsQuestionAnswer ?? false)
                            {
                                if (isInit && ilpPrep.PostData.Contains("{{" + kw.KeyWord + "}}"))
                                {
                                    ilpPrep.AnswerText = tk.Text_.ToString();// text_.ToString();
                                    ilpPrep.AnswerValue = (!isTextOnly || (tk.IsMultipleAnswers ?? false)) ? tk.Value_.ToString() : "";// val_.ToString();
                                }
                                else if (tk.Text_ == null)
                                {
                                    /*text_*/
                                    tk.Text_ = ilpPrep.AnswerText;
                                    /*val_*/
                                    tk.Value_ = ilpPrep.AnswerValue;
                                }

                            }
                            if (new[] { "15.1.1.01", "15.1.3.01" }.Contains(ilpPrep.TestNo) && tk.Helper_ != null && tk.Helper_ != DBNull.Value && !string.IsNullOrEmpty(tk.Helper_.ToString()))
                                StringSource = CustomReplaceForIngredient(StringSource, kw.KeyWord, tk.Value_, tk.Text_, tk.Helper_);
                            else
                                StringSource = ReplaceToJsonString(StringSource, kw.KeyWord, tk.Value_, tk.Text_/*val_, text_*/, isTextOnly, tk.IsMultipleAnswers ?? false, ilpTd.MultipleElements);
                        }
                    }
                    else if (tk.IsQuestionAnswer ?? false)
                    {
                        StringSource = ReplaceToJsonString(StringSource, kw.KeyWord, ilpPrep.AnswerValue, ilpPrep.AnswerText, isTextOnly, tk.IsMultipleAnswers ?? false, ilpTd.MultipleElements);
                    }
                }
            }
            //StringSource = Regex.Replace(StringSource, @"\{\{uuid\}\}", match => Guid.NewGuid().ToString(), RegexOptions.IgnoreCase);

            if (IsValidJson(StringSource))
            {
                var uuidMatches = Regex.Matches(StringSource, @"\{\{uuid\}\}", RegexOptions.IgnoreCase);
                if (uuidMatches.Count > 0)
                {
                    var uuidList = Enumerable.Range(0, uuidMatches.Count)
                                             .Select(_ => Guid.NewGuid().ToString())
                                             .ToList();

                    int uuidIndex = 0;
                    StringSource = Regex.Replace(StringSource, @"\{\{uuid\}\}", m => uuidList[uuidIndex++]);
                }
                if (!string.IsNullOrWhiteSpace(ilpTd.MultipleElements) && ilpPrep.TestNo.StartsWith("10.1.4"))
                {
                    var jToken = JToken.Parse(StringSource);
                    var bundle = (JObject)jToken;
                    CleanObservationEntries(bundle);
                    StringSource = bundle.ToString(Formatting.Indented);
                }
                StringSource = Regex.Replace(StringSource, "\"([^\"]*)\"",
                    j =>
                    {
                        string content = j.Groups[1].Value;

                        content = content.Replace("\r", "")
                                         .Replace("\n", "")
                                         .Replace("\t", "");
                        content = Regex.Replace(content, @"\s{2,}", " ");
                        content = content.Trim();

                        return $"\"{content}\"";
                    },
                    RegexOptions.Compiled
                );
            }

            return StringSource;
        }

        private static string ReplaceToJsonString(string JsonStringSource, string KeyWord, object val_, object text_, bool isTextOnly, bool IsMultipleAnswers, string multipleElements)
        {
            //var ReplaceWithText = ReplaceToJsonString(JsonStringSource, KeyWord, text_);
            ////JsonStringSource = JsonStringSource.Replace("{{" + KeyWord + ".text}}", ReplaceWithText);
            //JsonStringSource = Regex.Replace(JsonStringSource, "{{" + KeyWord + ".text}}", ReplaceWithText, RegexOptions.IgnoreCase);

            //if (!isTextOnly){
            //    var ReplaceWithValue = ReplaceToJsonString(JsonStringSource, KeyWord, val_);
            //    JsonStringSource = Regex.Replace(JsonStringSource, "{{" + KeyWord + ".value}}", ReplaceWithValue, RegexOptions.IgnoreCase);
            //}
            if (val_ != null && val_.ToString().Split(';').Any(v => !string.IsNullOrWhiteSpace(v)) && !string.IsNullOrWhiteSpace(multipleElements) && IsMultipleAnswers && IsValidJson(JsonStringSource))
            {
                return JsonStringSource = ReplaceJsonBlockByKeyword(
                    JsonStringSource,
                    KeyWord,
                    multipleElements,
                    val_,
                    text_
                );
            }
            var ReplaceWithText = ReplaceToJsonString(JsonStringSource, KeyWord, text_);
            if (KeyWord.Contains(".text"))
            {
                JsonStringSource = Regex.Replace(JsonStringSource, "{{" + KeyWord + "}}", ReplaceWithText, RegexOptions.IgnoreCase);
            }
            else if (KeyWord.Contains(".value"))
            {
                var ReplaceWithValue = ReplaceToJsonString(JsonStringSource, KeyWord, val_);
                JsonStringSource = Regex.Replace(JsonStringSource, "{{" + KeyWord + "}}", ReplaceWithValue, RegexOptions.IgnoreCase);
            }
            else if (KeyWord.Contains("-code") || KeyWord.Contains("-name"))
            {
                // replace kalau ada -code -name
                var JsonTokenSource = JToken.Parse(JsonStringSource);
                ReplaceAllPlaceholders2(JsonTokenSource, KeyWord, val_.ToString(), text_.ToString());
                JsonStringSource = JsonTokenSource.ToString(Formatting.Indented);
            } else if (KeyWord.Contains(".int"))
            {
                var ReplaceWithValue = ReplaceToJsonString(JsonStringSource, KeyWord, text_);
                JsonStringSource = Regex.Replace(
                    JsonStringSource,
                    "\"{{" + Regex.Escape(KeyWord) + "}}\"",
                    ReplaceWithValue,
                    RegexOptions.IgnoreCase
                );
            }
            else
            {
                if (!IsValidJson(JsonStringSource) && val_ != null && val_.ToString().Contains(";"))
                {
                    var sqlIn = string.Join(",",
                            val_.ToString()
                                .Split(';')
                                .Select(v => $"'{v.Trim().Replace("'", "''")}'")
                        );
                    
                    JsonStringSource = Regex.Replace(
                        JsonStringSource,
                        "'{{" + Regex.Escape(KeyWord) + "}}'",
                        sqlIn,
                        RegexOptions.IgnoreCase
                    );
                }
                else
                {
                    JsonStringSource = Regex.Replace(
                        JsonStringSource,
                        "{{" + KeyWord + "}}",
                        ReplaceWithText,
                        RegexOptions.IgnoreCase
                    );
                }
            }

            return JsonStringSource;
        }

        private static string ReplaceJsonBlockByKeyword(string json, string keyword, string multipleElements, object valueCsv, object textCsv)
        {
            if (keyword.StartsWith("Detail-", StringComparison.OrdinalIgnoreCase))
                return AttachToDetail(json, keyword, valueCsv, textCsv);
            if (keyword.StartsWith("QuestionPrescription", StringComparison.OrdinalIgnoreCase))
                return UpdateLinkIds(json, keyword, valueCsv, textCsv);

            string[] values;
            string[] texts;
            if (valueCsv.ToString().Contains(";"))
            {
                values = valueCsv.ToString().Split(';').Select(v => v.Trim()).ToArray();
                texts = textCsv.ToString().Split(';').Select(t => t.Trim()).ToArray();
            }
            else if (valueCsv is System.Collections.IEnumerable enumerable && !(valueCsv is string))
            {
                values = enumerable.Cast<object>()
                       .Select(v => v?.ToString()?.Trim() ?? "")
                       .ToArray();
                texts = enumerable.Cast<object>()
                          .Select(t => t?.ToString()?.Trim() ?? "")
                          .ToArray();
            }
            else
            {
                values = valueCsv.ToString().Split(';').Select(v => v.Trim()).ToArray();
                texts = textCsv.ToString().Split(';').Select(t => t.Trim()).ToArray();
            }


            if (values.Length != texts.Length)
                throw new ArgumentException("Jumlah value dan text tidak sama.");

            for (int i = 0; i < texts.Length; i++)
            {
                if (!double.TryParse(texts[i], out _) && DateTime.TryParse(texts[i], out DateTime dt))
                {
                    var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                    texts[i] = ssu.SSDateYMD(Convert.ToDateTime(dt));
                }
            }
            //if (!IsValidJson(json))
            //    return json;
            var jToken = JToken.Parse(json);
            string[] elementNames;
            if (keyword.Equals("MedicationRequestReview_id", StringComparison.OrdinalIgnoreCase))
                elementNames = new[] { "answer" };
            else
                elementNames = multipleElements.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //var elementNames = multipleElements.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            bool foundMatch = false;
            foreach (var elementName in elementNames)
            {
                var elementTokens = jToken.SelectTokens($"$..{elementName}")
                              .OfType<JArray>()
                              .ToList();

                foreach (var element in elementTokens)
                {
                    var matchingElements = element
                        .Children<JObject>()
                        .Where(e => e.ToString().Contains($"{{{{{keyword}}}}}"))
                        .ToList();

                    if (matchingElements.Count > 0)
                    {
                        foundMatch = true;
                        // Jika jumlah matching element kurang dari jumlah values → lakukan duplikasi
                        if (matchingElements.Count == 1 && values.Length > 1)
                        {
                            var original = matchingElements.First();
                            element.Remove(original);

                            for (int i = 0; i < values.Length; i++)
                            {
                                var clone = JObject.Parse(original.ToString());
                                element.Add(clone);
                            }

                            matchingElements = element
                                .Children<JObject>()
                                .Where(e => e.ToString().Contains($"{{{{{keyword}}}}}"))
                                .ToList();
                        }

                        int index = 0;
                        foreach (var el in element.Children<JObject>())
                        {
                            if (index >= values.Length) break;

                            if (el.ToString().Contains($"{{{{{keyword}}}}}"))
                            {
                                ReplaceAllPlaceholders2(el, keyword, values[index], texts[index]);
                                index++;
                            }
                        }
                    }
                }

                if (foundMatch)
                    break; // keluar dari foreach elementNames
            }
            json = jToken.ToString(Formatting.Indented);
            //var uuidMatches = Regex.Matches(json, @"\{\{uuid\}\}", RegexOptions.IgnoreCase);
            //var uuidList = Enumerable.Range(0, uuidMatches.Count)
            //                         .Select(_ => Guid.NewGuid().ToString())
            //                         .ToList();

            //int uuidIndex = 0;
            //json = Regex.Replace(json, @"\{\{uuid\}\}", m => uuidList[uuidIndex++]);

            return json;
        }

        public static void ReplaceAllPlaceholders2(JToken token, string keyword, string value, string text)
        {
            string pattern = @"{{\s*" + Regex.Escape(keyword) + @"\s*}}";
            string replacement = keyword.Contains("-code") ? value : text;
            bool isIntReplacement = keyword.Contains(".int");

            if (token.Type == JTokenType.String && token is JValue jVal)
            {
                string str = jVal.ToString();

                if (isIntReplacement && Regex.IsMatch(str.Trim(), $"^\\s*{pattern}\\s*$", RegexOptions.IgnoreCase))
                {
                    if (int.TryParse(replacement, out int intVal))
                        jVal.Value = intVal;
                    else if (double.TryParse(replacement, out double dblVal))
                        jVal.Value = dblVal;
                    else
                        jVal.Value = replacement;
                }
                else
                {
                    string updated = Regex.Replace(str, pattern, replacement, RegexOptions.IgnoreCase);
                    jVal.Value = updated;
                }
            }
            else if (token is JContainer container)
            {
                foreach (var child in container.DescendantsAndSelf().OfType<JValue>().Where(v => v.Type == JTokenType.String))
                {
                    string str = child.ToString();

                    if (isIntReplacement && Regex.IsMatch(str.Trim(), $"^\\s*{pattern}\\s*$", RegexOptions.IgnoreCase))
                    {
                        if (int.TryParse(replacement, out int intVal))
                            child.Value = intVal;
                        else if (double.TryParse(replacement, out double dblVal))
                            child.Value = dblVal;
                        else
                            child.Value = replacement;
                    }
                    else
                    {
                        string updated = Regex.Replace(str, pattern, replacement, RegexOptions.IgnoreCase);
                        child.Value = updated;
                    }
                }
            }
        }
        #region CUSTOM TEMPLATE
        private static string UpdateLinkIds(string json, string keyword, object valueCsv, object textCsv)
        {
            var values = valueCsv.ToString().Split(';').Select(v => v.Trim()).ToArray();
            var texts = textCsv.ToString().Split(';').Select(t => t.Trim()).ToArray();

            if (values.Length != texts.Length)
                throw new ArgumentException("Jumlah value dan text tidak sama.");

            JToken root = JToken.Parse(json);

            StripPlaceholder(root, "{{MedicationRequestReview_id}}");

            var grouped = values
                .Select((v, i) => new { Value = v, Text = texts[i] })
                .GroupBy(x => x.Value.Split('.')[0])
                .ToDictionary(g => g.Key, g => g.ToList());

            foreach (var parent in root.SelectTokens("$.item[*]").OfType<JObject>())
            {
                var parentLinkId = parent["linkId"] == null ? null : parent["linkId"].ToString();
                if (string.IsNullOrEmpty(parentLinkId)) continue;
                if (!grouped.ContainsKey(parentLinkId)) continue;

                var childArray = parent["item"] as JArray;
                if (childArray == null) continue;

                var templateChild = childArray
                    .OfType<JObject>()
                    .FirstOrDefault(c => c.ToString().Contains("{{" + keyword + "}}"));

                if (templateChild == null) continue;

                childArray.Clear();

                foreach (var g in grouped[parentLinkId])
                {
                    var clone = JObject.Parse(templateChild.ToString());
                    clone["linkId"] = g.Value;

                    ReplaceAllPlaceholders2(clone, keyword, g.Value, g.Text);

                    childArray.Add(clone);
                }
            }

            foreach (var parent in root.SelectTokens("$.item[*]").OfType<JObject>())
            {
                var linkId = parent["linkId"] == null ? null : parent["linkId"].ToString();
                if (linkId == "4")
                {
                    parent.Remove("item");
                    parent["answer"] = new JArray
                    {
                        new JObject
                        {
                            ["valueReference"] = new JObject
                            {
                                // Tanpa {{MedicationRequestReview_id}}
                                // (kalau kamu butuh slash, ganti "MedicationRequest" -> "MedicationRequest/")
                                ["reference"] = "MedicationRequest/{{MedicationRequestReview_id}}"
                            }
                        }
                    };
                }
            }

            return root.ToString(Formatting.Indented);
        }
        private static bool _placeholderCleaned = false;
        private static void StripPlaceholder(JToken node, string placeholder)
        {
            if (_placeholderCleaned) return; // skip kalau sudah pernah
            _placeholderCleaned = true;
            var allValues = node.SelectTokens("$..*")
                .OfType<JValue>()
                .Where(v => v.Type == JTokenType.String)
                .ToList();

            foreach (var v in allValues)
            {
                var s = v.Value == null ? null : v.Value.ToString();
                if (string.IsNullOrEmpty(s)) continue;

                if (s.Contains(placeholder))
                {
                    s = s.Replace(placeholder, "");
                    if (s.EndsWith("/"))
                    {
                        s = s.TrimEnd('/');
                    }
                    v.Value = s;
                }
            }
        }

        private static string AttachToDetail(string json, string keyword, object valueCsv, object textCsv)
        {
            string[] values = valueCsv.ToString().Split(';').Select(v => v.Trim()).ToArray();
            string[] texts = textCsv.ToString().Split(';').Select(t => t.Trim()).ToArray();

            if (values.Length != texts.Length)
                throw new ArgumentException("Panjang valueCsv dan textCsv harus sama.");

            var jToken = JToken.Parse(json);
            var entries = jToken.SelectTokens("$.entry[*]").OfType<JObject>().ToList();

            foreach (var entry in entries)
            {
                var trxNo = entry.SelectToken("resource.identifier[0].value")?.ToString();
                if (string.IsNullOrEmpty(trxNo)) continue;

                // ambil detail sesuai header ini
                var details = values
                    .Select((v, i) => new { Value = v, Text = texts[i] })
                    .Where(x => x.Value == trxNo)
                    .ToList();

                if (!details.Any()) continue;

                // cari semua node yang punya {{keyword}}
                var tokensWithKeyword = entry
                    .SelectTokens($"$..*")
                    .Where(t => t.Type == JTokenType.String && t.ToString().Contains($"{{{{{keyword}}}}}"))
                    .ToList();

                foreach (var token in tokensWithKeyword)
                {
                    var parent = token.Parent;
                    if (parent is JProperty jp)
                    {
                        var originalObj = jp.Parent; // contoh { "reference": "Observation/{{Observation_id}}" }
                        if (originalObj == null) continue;

                        var arrayParent = originalObj.Parent as JArray;
                        if (arrayParent == null) continue; // pastikan dalam array (seperti "result": [ ... ])

                        // hapus aslinya
                        arrayParent.Remove(originalObj);

                        // tambahkan clone sesuai jumlah detail
                        foreach (var d in details)
                        {
                            var clone = JObject.Parse(originalObj.ToString());
                            // replace {{keyword}} dengan id observation (d.Text)
                            var refProp = clone.Properties()
                                                .FirstOrDefault(p => p.Value.ToString().Contains($"{{{{{keyword}}}}}"));
                            if (refProp != null)
                                refProp.Value = refProp.Value.ToString().Replace($"{{{{{keyword}}}}}", d.Text);

                            arrayParent.Add(clone);
                        }
                    }
                }
            }

            return jToken.ToString(Formatting.Indented);
        }
        private static string CustomReplaceForIngredient(string json, string keyword, object valueCsv, object textCsv, object helperCsv)
        {
            string[] values = valueCsv.ToString().Split(';').Select(v => v.Trim()).ToArray();
            string[] texts = textCsv.ToString().Split(';').Select(t => t.Trim()).ToArray();
            string[] helpers = helperCsv.ToString().Split(';').Select(h => h.Trim()).ToArray();

            if (values.Length != texts.Length || values.Length != helpers.Length)
                throw new ArgumentException("Panjang valueCsv, textCsv, dan helperCsv harus sama.");

            // satukan jadi triplet
            var triplets = values
                .Select((v, i) => new { Value = v, Text = texts[i], Helper = helpers[i] })
                .ToList();

            var jToken = JToken.Parse(json);
            var entries = jToken.SelectTokens("$.entry[*]").OfType<JObject>().ToList();

            foreach (var entry in entries)
            {
                // ambil identifier.value dari entry
                var identifier = entry.SelectToken("resource.identifier[0].value")?.ToString();
                if (string.IsNullOrEmpty(identifier)) continue;

                // ambil semua detail untuk helper yang cocok
                var details = triplets.Where(x => x.Helper == identifier).ToList();
                if (!details.Any()) continue;

                var ingredientArray = entry.SelectToken("resource.ingredient") as JArray;
                if (ingredientArray == null || ingredientArray.Count == 0) continue;

                var originalIngredient = (JObject)ingredientArray.First();

                // kalau jumlah ingredient sudah sesuai jumlah details → langsung replace saja
                if (ingredientArray.Count == details.Count)
                {
                    for (int i = 0; i < details.Count; i++)
                    {
                        ReplaceAllPlaceholders2(ingredientArray[i] as JObject, keyword, details[i].Value, details[i].Text);
                    }
                }
                else
                {
                    ingredientArray.RemoveAll();

                    foreach (var d in details)
                    {
                        var clone = JObject.Parse(originalIngredient.ToString());
                        ReplaceAllPlaceholders2(clone, keyword, d.Value, d.Text);
                        ingredientArray.Add(clone);
                    }
                }
            }

            return jToken.ToString(Formatting.Indented);
        }

        #endregion

        public static string ReplaceToJsonString(string JsonStringSource, string KeyWord, object val)
        {
            var ReplaceWith = "";
            // deteksi tipe data, kalau tanggal maka ubah ke format std satu sehat
            if (val != DBNull.Value && val != null)
            {
                switch (val.GetType().Name)
                {
                    case "DateTime":
                        {
                            var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                            ReplaceWith = ssu.SSDateYMD(Convert.ToDateTime(val));
                            break;
                        }
                    default:
                        {
                            if (val is System.Collections.IEnumerable enumerable && !(val is string))
                            {
                                var firstItem = enumerable.Cast<object>().FirstOrDefault();
                                ReplaceWith = firstItem?.ToString() ?? null;
                            }
                            else
                            {
                                ReplaceWith = val.ToString();
                            }
                            break;
                        }
                }
            }

            var ansRole = KeyWord.Split(':');
            if (ansRole.Length > 1)
            {
                var typePart = ansRole[1].ToLower().Split('.')[0];
                switch (typePart)
                {
                    case "bool":
                        {
                            ReplaceWith = ToBoolean(ReplaceWith).ToString().ToLower();
                            break;
                        }
                    case "ssdateid":
                    case "ssdateddmmmmyyyy":
                        {
                            var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                            ReplaceWith = ssu.SSDateIdDDMMMMYYYY(Convert.ToDateTime(val));
                            break;
                        }
                    case "ssdateddddmmmmyyyy":
                        {
                            var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                            ReplaceWith = ssu.SSDateIdDDDDMMMMYYYY(Convert.ToDateTime(val));
                            break;
                        }
                    case "ssdatedddd":
                        {
                            var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                            ReplaceWith = ssu.SSDateIdDDDD(Convert.ToDateTime(val));
                            break;
                        }
                    case "ssdateyyyymmdd":
                        {
                            var ssu = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                            ReplaceWith = ssu.SSDateYYYYMMDD(Convert.ToDateTime(val));
                            break;
                        }
                    case "unix":
                        {
                            ReplaceWith = ((DateTimeOffset)Convert.ToDateTime(val)).ToUnixTimeSeconds().ToString();
                            break;
                        }
                }
            }
            return ReplaceWith;
        }

        private static void CleanObservationEntries(JObject bundle)
        {
            var entries = bundle["entry"] as JArray;
            if (entries == null) return;

            foreach (var entry in entries.OfType<JObject>())
            {
                var resource = entry["resource"] as JObject;
                if (resource == null) continue;

                var valueQuantity = resource["valueQuantity"] as JObject;
                if (valueQuantity != null)
                {
                    var valueStr = valueQuantity["value"]?.ToString();

                    // Kalau masih ada placeholder {{}}, hapus block
                    if (string.IsNullOrWhiteSpace(valueStr) || valueStr.Contains("{{"))
                    {
                        resource.Remove("valueQuantity");
                        continue;
                    }

                    // Kalau bukan angka, ubah ke valueString
                    if (!double.TryParse(valueStr, out _))
                    {
                        resource["valueString"] = valueStr;
                        resource.Remove("valueQuantity");
                        resource.Remove("referenceRange");
                        resource.Remove("interpretation");
                    }
                }

                var interpretation = resource["interpretation"] as JArray;
                if (interpretation != null && interpretation.ToString().Contains("{{"))
                {
                    resource.Remove("interpretation");
                }

                var refRange = resource["referenceRange"] as JArray;
                if (refRange != null && refRange.ToString().Contains("{{"))
                {
                    resource.Remove("referenceRange");
                }
            }
        }
        public static bool IsValidJson(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            input = input.Trim();

            if (!(input.StartsWith("{") && input.EndsWith("}")) &&
                !(input.StartsWith("[") && input.EndsWith("]")))
                return false;

            try
            {
                JToken.Parse(input);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }


        public static bool ToBoolean(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string nilai = input.Trim().ToLower();

            string[] nilaiTrue = { "yes", "ya", "ada", "true", "1", "ok" };
            string[] nilaiFalse = { "no", "bukan", "tidak", "false", "0", "kosong" };

            if (nilaiTrue.Contains(nilai))
                return true;
            else if (nilaiFalse.Contains(nilai))
                return false;
            else
                throw new ArgumentException($"Nilai tidak dikenali untuk boolean: '{input}'");
        }

        public static bool IsSqlSafe(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql)) return false;

            string lowerSql = sql.ToLowerInvariant();
            string[] blacklist = new[] {
                "drop ", "delete ", "truncate ", "alter ", "--", ";--", ";", "exec ", "execute ", "insert ", "update ", "xp_"
            };

            return !blacklist.Any(bad => lowerSql.Contains(bad));
        }
        public static bool IsSelectOnly(string sql)
        {
            string cleaned = sql.Trim().ToLowerInvariant();
            return cleaned.StartsWith("select") && !cleaned.Contains(";"); // cegah multiple statement
        }

        /// <summary>
        /// kirim sekaligus berdasarkan nomor registrasi
        /// </summary>
        /// <param name="RegistrationNo"></param>
        /// <param name="accessToken"></param>
        public static void SendToSatuSehat(string RegistrationNo, ref string accessToken)
        {
            var sspColl = new SatuSehatILPPreparationCollection();
            var sspQ = sspColl.Query;
            sspQ.Where(sspQ.RegistrationNo == RegistrationNo);

            if (sspColl.LoadAll())
            {
                var sstColl = new SatuSehatILPTemplateDetailCollection();
                var sstQ = sstColl.Query;
                var ssp1 = sspColl.First();
                sstQ.Where(sstQ.TemplateID == ssp1.TemplateID);
                sstColl.LoadAll();

                var templateKeywords = new SatuSehatILPTemplateDetailKeyWordCollection();
                templateKeywords.Query.Where(
                    templateKeywords.Query.Or(
                        templateKeywords.Query.TemplateID == ssp1.TemplateID,
                        templateKeywords.Query.TemplateID == 0
                    )
                );
                templateKeywords.LoadAll();

                //string accessToken = "";
                var utils = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                foreach (var ssp in sspColl)
                {
                    if (string.IsNullOrWhiteSpace(ssp.PostData)) continue;

                    // kyknya perlu replace lagi variabel yang belum ada isinya, untuk case nilainya baru ada setelah proses kirim satu sehat
                    // untuk task lain saat looping blok ini
                    var sst = sstColl.Where(ss => ss.TemplateID == ssp.TemplateID && ss.TestNo == ssp.TestNo && ss.Sequence == ssp.Sequence).First();
                    ssp.PostData = ReplaceVariables(RegistrationNo, ssp, ssp.PostData, sst, true, templateKeywords, 1);
                    // kirim
                    SendToSatuSehat(RegistrationNo, ssp, utils, ref accessToken);

                    // detach supaya bisa simpan satu2,
                    // soalnya ada kemungkinan hasil simpanannya dipakai lagi oleh variabel yang lain
                    // kalau gak belum disimpan jadi gak dapat querynya
                    sspColl.DetachEntity(ssp);
                    ssp.Save();
                }
            }

            //sspColl.Save();
        }

        /// <summary>
        /// kirim satu saja berdasarkan nomor registrasi dan SatuSehatILPPreparation
        /// </summary>
        /// <param name="RegistrationNo"></param>
        /// <param name="ssp"></param>
        /// <param name="accessToken"></param>
        public static void SendToSatuSehat(string RegistrationNo, SatuSehatILPPreparation ssp, ref string accessToken)
        {
            //var sspColl = new SatuSehatILPPreparationCollection();
            //sspColl.AttachEntity(ssp);
            //if (sspColl.LoadAll())
            {
                //string accessToken = "";
                var utils = new Temiang.Avicenna.Bridging.SatuSehat.Utils();
                if (string.IsNullOrWhiteSpace(ssp.PostData))
                {
                    // skip
                }
                else
                {
                    var sstColl = new SatuSehatILPTemplateDetailCollection();
                    var sstQ = sstColl.Query;
                    sstQ.Where(sstQ.TemplateID == ssp.TemplateID);
                    sstColl.LoadAll();

                    var templateKeywords = new SatuSehatILPTemplateDetailKeyWordCollection();
                    templateKeywords.Query.Where(
                        templateKeywords.Query.Or(
                            templateKeywords.Query.And(
                                templateKeywords.Query.TemplateID == ssp.TemplateID,
                                templateKeywords.Query.TestNo == ssp.TestNo),
                            templateKeywords.Query.TemplateID == 0
                        )
                    );
                    templateKeywords.LoadAll();

                    // kyknya perlu replace lagi variabel yang belum ada isinya, untuk case nilainya baru ada setelah proses kirim satu sehat
                    // untuk task lain saat looping blok ini
                    var sst = sstColl.Where(ss => ss.TemplateID == ssp.TemplateID && ss.TestNo == ssp.TestNo && ss.Sequence == ssp.Sequence).First();
                    ssp.PostData = ReplaceVariables(RegistrationNo, ssp, ssp.PostData, sst, true, templateKeywords, 1);
                    // kirim
                    SendToSatuSehat(RegistrationNo, ssp, utils, ref accessToken);
                    ssp.Save();
                }
            }

            //sspColl.Save();
        }


        private static void SendToSatuSehat(string RegistrationNo, SatuSehatILPPreparation ssp, Temiang.Avicenna.Bridging.SatuSehat.Utils utils, ref string accessToken)
        {
            if (string.IsNullOrWhiteSpace(ssp.PostData)) return;
            string resourceType = string.Empty;
            JToken token = JToken.Parse(ssp.PostData);
            if (token is JObject obj)
            {
                // kalau JSON object
                resourceType = (string)obj["resourceType"];
                //Console.WriteLine($"ResourceType: {resourceType}");
            }
            //else if (token is JArray arr)
            //{
            //    // kalau JSON array
            //    foreach (var item in arr)
            //    {
            //        Console.WriteLine($"{item["path"]} = {item["value"]}");
            //    }
            //}
            // harusnya gak dipakai lagi
            //ReplaceEncounterID(RegistrationNo, ssp, resourceType);

            if (string.IsNullOrWhiteSpace(ssp.AnswerText))
            {
                // kalau data tidak ada brarti task ini tidak perlu dikirim

                // untuk encounter tidak perlu ada answertext
                if (resourceType.ToLower() == "encounter" || token is JArray arr)
                {
                    // boleh dikirim khusus encounter
                }
                else
                {
                    ssp.RespondData = "No data available";
                    return;
                }
            }
            if (ssp.AnswerText.ToString() == "0" && resourceType.ToLower() == "Observation")
            {
                ssp.RespondData = "Data not filled in";
                return;
            }

            var sstd = new SatuSehatILPTemplateDetail();
            if (sstd.LoadByPrimaryKey(ssp.TemplateID ?? 0, ssp.TestNo, ssp.Sequence ?? 0))
            {
                if (string.IsNullOrWhiteSpace(sstd.PostUrl)) return;
                if (!(ssp.IsSent ?? false) || (ssp.IsError ?? false)) // kirim jika belum pernah dikirim atau sudah pernah dikirim tapi error
                {
                    var errMsg = ValidateSatuSehatILPPreparation(ssp);
                    if (!string.IsNullOrWhiteSpace(errMsg))
                    {
                        ssp.IsError = true;
                        ssp.IsSent = false;
                        ssp.RespondData = errMsg;
                    }
                    else
                    {
                        if (resourceType.ToLower() == "encounter" && sstd.PostMethod == "PUT")
                        {
                            var cekKirim = new SatuSehatILPPreparationQuery("ck");
                            if(ssp.TemplateID == 100)
                                cekKirim.Where(cekKirim.TestNo.In("04.4.01", "04.4.02", "12.1", "15.1.3.02", "19.1"), cekKirim.RegistrationNo == RegistrationNo);

                            cekKirim.Select(cekKirim.IsSent, cekKirim.IsError);
                            var dtbcheck = cekKirim.LoadDataTable();

                            bool allSent = dtbcheck.AsEnumerable().All(r => r.Field<bool>("IsSent"));
                            bool allNoError = dtbcheck.AsEnumerable().All(r => !r.Field<bool>("IsError"));

                            var encounterId = GetEncounterOrBedId(RegistrationNo, "encounter");
                            if (!string.IsNullOrWhiteSpace(encounterId))
                            {
                                // ganti urlnya
                                sstd.PostUrl = sstd.PostUrl.Replace("{{Encounter_id}}", encounterId);
                            }
                            if (allSent && allNoError)
                                utils.SendToSatuSehat(ssp, sstd, ref accessToken); // tutup encounter ketika sudah kirim vsign,diagnosa,obat dan mds
                            else
                                return; // skip dulu, tunggu task lain selesai dikirim
                        } else if (sstd.PostMethod == "PATCH")
                        {
                            var bedId = GetEncounterOrBedId(RegistrationNo, "bed");
                            sstd.PostUrl = sstd.PostUrl.Replace("{{BedId}}", bedId);
                            utils.SendToSatuSehat(ssp, sstd, ref accessToken);
                        }
                        utils.SendToSatuSehat(ssp, sstd, ref accessToken);
                    }
                }
            }
        }
        public static string GetEncounterOrBedId(string registrationNo, string idToReturn)
        {
            var Id = string.Empty;
            if(idToReturn.ToLower() == "bed")
            {
                var sb = new BedQuery("sb");
                var bs = new BedStatusHistoryQuery("bs");
                sb.InnerJoin(bs).On(sb.BedID == bs.BedID);
                sb.Where(bs.RegistrationNo == registrationNo);
                sb.OrderBy(bs.LastUpdateDateTime.Descending);
                sb.es.Top = 1;
                var dtb = sb.LoadDataTable();
                if (dtb.Rows.Count > 0)
                {
                    var rowBd = dtb.Rows[0];
                    if (!string.IsNullOrWhiteSpace(rowBd["SatuSehatBridgingID"].ToString()))
                    {
                        Id = rowBd["SatuSehatBridgingID"].ToString();
                    }
                }
            }
            else if(idToReturn.ToLower() == "encounter")
            { 
                var ssk = new SatuSehatKunjungan();
                if (ssk.LoadByPrimaryKey(registrationNo))
                {
                    if (!string.IsNullOrWhiteSpace(ssk.EncounterID.ToString()))
                    {
                        Id = ssk.EncounterID.ToString();
                    }
                }
            }
            return Id;

        }

        private static string ValidateSatuSehatILPPreparation(SatuSehatILPPreparation ssp)
        {
            // Validasi kalau ada
            if (!string.IsNullOrWhiteSpace(ssp.PostData))
            {
                List<string> list = new List<string>();
                MatchCollection matches = Regex.Matches(ssp.PostData, @"\{\{(.*?)\}\}");
                foreach (Match match in matches)
                {
                    var str = match.Groups[1].Value;

                    if (!list.Contains(str))
                    {
                        list.Add(str);
                    }
                }
                if (list.Count > 0)
                {
                    return "The data is invalid due to an uninitialized or empty variable (" + string.Join(", ", list) + ")";
                }
            }
            else
            {
                return "Data is empty";
            }

            return "";
        }

        //private static bool ReplaceEncounterID(string RegistrationNo, SatuSehatILPPreparation ssp, string resourceType) {
        //    // harusnya gak dipakai lagi
        //    var encounterIdKeyWord = "Encounter_id";
        //    if (ssp.PostData.Contains("{{" + encounterIdKeyWord + "}}") && resourceType.ToLower() != "encounter") {
        //        var kw = new SatuSehatILPTemplateDetailKeyWord();
        //        if (kw.LoadByPrimaryKey(0, "-", 1, encounterIdKeyWord)) {
        //            var kwColl = new SatuSehatILPTemplateDetailKeyWordCollection();
        //            kwColl.AttachEntity(kw);
        //            var ilpTd = new SatuSehatILPTemplateDetail();
        //            if (ilpTd.LoadByPrimaryKey(100, "02.1", 1))
        //            {
        //                ssp.PostData = ReplaceVariables(RegistrationNo, ssp, ssp.PostData, ilpTd, true, kwColl, 1);
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}