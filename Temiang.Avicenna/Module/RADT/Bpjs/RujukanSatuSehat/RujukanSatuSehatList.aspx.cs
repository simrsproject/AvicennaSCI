using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using Telerik.Web.UI;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.Common;

namespace Temiang.Avicenna.Module.RADT.Bpjs
{
    public partial class RujukanSatuSehatList : BasePageList
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            UrlPageSearch = "RujukanSatuSehatSearch.aspx";
            UrlPageDetail = "RujukanSatuSehatDetail.aspx";

            this.WindowSearch.Height = 400;

            ProgramID = AppConstant.Program.BpjsRujukanSatuSehat;
        }

        public override void OnMenuEditClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "edit");
        }

        public override void OnMenuViewClick(GridDataItem[] dataItems)
        {
            RedirectToPageDetail(dataItems[0], "view");
        }

        private void RedirectToPageDetail(GridDataItem dataItem, string mode)
        {
            string nosep = dataItem.GetDataKeyValue(BpjsRujukanSatuSehatMetadata.ColumnNames.NoSep).ToString();
            string NoRujukan = dataItem.GetDataKeyValue(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukan).ToString();
            string noRujukanSatuSehat = dataItem.GetDataKeyValue(BpjsRujukanSatuSehatMetadata.ColumnNames.NoRujukanSatuSehat).ToString();
            Page.Response.Redirect("RujukanSatuSehatDetail.aspx?md=" + mode + "&sep=" + nosep +"&norujukan=" + NoRujukan + "&norujukanss=" + noRujukanSatuSehat, true);
        }

        protected void grdList_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grdList.DataSource = BpjsSeps;
        }

        protected void grdList_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridNestedViewItem nestedItem)
            {
                RadGrid grdKriteria = nestedItem.FindControl("grdKriteria") as RadGrid;

                if (grdKriteria == null) return;

                GridDataItem parentItem = nestedItem.ParentItem as GridDataItem;

                if (parentItem == null) return;

                string noSep = parentItem.GetDataKeyValue("noSep").ToString();
                string noRujukan = parentItem.GetDataKeyValue("NoRujukan").ToString();
                string noRujukanSatuSehat = parentItem.GetDataKeyValue("noRujukanSatuSehat").ToString();

                grdKriteria.DataSource = GetKriteriaRujukan(noSep, noRujukan, noRujukanSatuSehat);
                grdKriteria.DataBind();
            }
        }


        private DataTable BpjsSeps
        {
            get
            {
                object obj = Session[SessionNameForList];
                if (obj != null) return (DataTable)obj;

                BpjsRujukanSatuSehatQuery query;

                if (Session[SessionNameForQuery] != null)
                    query = (BpjsRujukanSatuSehatQuery)Session[SessionNameForQuery];
                else
                {
                    query = new BpjsRujukanSatuSehatQuery("a");

                    var std = new AppStandardReferenceItemQuery("b");
                    var diag = new DiagnoseQuery("c");
                    var reg = new BpjsSEPQuery("e");

                    query.InnerJoin(std)
                        .On(std.StandardReferenceID == AppEnum.StandardReference.BpjsTypeOfService.ToString()
                        && std.ItemID == query.JnsPelayanan);

                    query.InnerJoin(diag)
                        .On(query.DiagRujukan == diag.DiagnoseID);

                    query.InnerJoin(reg)
                        .On(query.NoSep == reg.NoSEP);

                    query.Select(
                        query.NoSep,
                        query.NoRujukan,
                        query.TglRujukan,
                        query.TglRencana,
                        query.NamaPoliRujukan,
                        query.Catatan,
                        query.NoRujukanSatuSehat,
                        query.KodeFaskesSatuSehat,
                        query.IdPasienSatuSehat,
                        query.KdppkSatuSehatTujuanRujukan,
                        query.KdDokterSatuSehat,
                        query.EncounterReference,
                        query.PatientInstruction,
                        query.KeteranganRujukan,
                        query.KriteriaRujukanJson,

                        std.ItemName.As("TypeOfService"),
                        diag.DiagnoseName,
                        reg.NomorKartu,
                        "<e.NamaPasien + ' (' + e.JenisKelamin + ')' AS NamaPasienJK>"
                    );

                    //query.Where(query.TglRujukan == DateTime.Now.Date);
                    query.OrderBy(query.NoRujukan.Descending);
                }

                query.es.Top = AppSession.Parameter.MaxResultRecord;

                DataTable dtb = query.LoadDataTable();

                Session[SessionNameForList] = dtb;

                return dtb;
            }
        }

        private DataTable GetKriteriaRujukan(string noSep, string noRujukan, string noRujukanSatuSehat)
        {
            var query = new BpjsRujukanSatuSehatQuery("a");

            query.Select(query.KriteriaRujukanJson);
            query.Where(
                query.NoSep == noSep &&
                query.NoRujukan == noRujukan &&
                query.NoRujukanSatuSehat == noRujukanSatuSehat
            );

            DataTable dt = query.LoadDataTable();

            DataTable result = new DataTable();
            result.Columns.Add("linkId");
            result.Columns.Add("text");
            result.Columns.Add("answer");

            if (dt.Rows.Count == 0) return result;

            string json = dt.Rows[0]["KriteriaRujukanJson"]?.ToString();

            if (string.IsNullOrWhiteSpace(json)) return result;

            try
            {
                var items = JsonConvert.DeserializeObject<List<KriteriaItem>>(json);

                if (items == null) return result;

                foreach (var item in items)
                {
                    string answerText = "-";

                    var ans = item.answer?.FirstOrDefault();

                    if (ans != null)
                    {
                        if (ans.valueBoolean.HasValue)
                        {
                            // 🔥 biar user friendly
                            answerText = ans.valueBoolean.Value ? "Ya" : "Tidak";
                        }
                        else if (!string.IsNullOrEmpty(ans.valueString))
                        {
                            answerText = ans.valueString;
                        }
                    }

                    result.Rows.Add(
                        item.linkId,
                        item.text,
                        answerText
                    );
                }
            }
            catch (Exception ex)
            {
                // optional debug
                // throw;
            }

            return result;
        }

        public class KriteriaItem
        {
            public string linkId { get; set; }
            public string text { get; set; }
            public List<KriteriaAnswer> answer { get; set; }
        }

        public class KriteriaAnswer
        {
            public bool? valueBoolean { get; set; }
            public string valueString { get; set; }
        }
    }
}