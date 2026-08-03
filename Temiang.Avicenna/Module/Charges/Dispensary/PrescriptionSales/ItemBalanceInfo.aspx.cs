using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Temiang.Avicenna.Common;
using Temiang.Avicenna.BusinessObject;
using DevExpress.Web;

namespace Temiang.Avicenna.Module.Charges.Dispensary.PrescriptionSales
{
    public partial class ItemBalanceInfo : BasePageDialog
    {
        public string ItemID {
            get {
                return Request.QueryString["itemid"];
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ButtonOk.Visible = false;
            ButtonCancel.Text = "Close";
        }

        protected void grdBalance_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var ib = new ItemBalanceQuery("ib");
            var i = new ItemQuery("i");
            var loc = new LocationQuery("loc");
            var im = new ItemProductMedicQuery("im");

            ib.InnerJoin(i).On(ib.ItemID == i.ItemID)
                .InnerJoin(loc).On(ib.LocationID == loc.LocationID)
                .LeftJoin(im).On(i.ItemID == im.ItemID)
                .Where(ib.ItemID == ItemID, loc.IsActive == true)
                .Select(ib, i.ItemName, im.SRItemUnit, loc.LocationName);
            var dtb = ib.LoadDataTable();

            grdBalance.DataSource = dtb;
        }


        protected void grdBalance_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName != "UpdateStok") return;

            var gridItem = e.Item as Telerik.Web.UI.GridDataItem;
            if (gridItem == null) return;

            var itemId = Convert.ToString(gridItem["ItemID"].Text)?.Trim();
            var balanceText = Convert.ToString(gridItem["Balance"].Text)?.Trim();

            if (string.IsNullOrWhiteSpace(itemId))
            {
                ShowAlert("ItemID tidak ditemukan.");
                return;
            }

            if (!decimal.TryParse(balanceText, out var balanceDecimal))
            {
                ShowAlert("Balance tidak valid.");
                return;
            }

            var balance = Convert.ToInt32(balanceDecimal);

            var map = new ItemBridging();
            map.Query.Where(
                map.Query.ItemID == itemId,
                map.Query.SRBridgingType == "BridgingType-018"
            );

            if (!map.Query.Load())
            {
                ShowAlert("Item belum dimapping ke BPJS (APOTEKONLINE).");
                return;
            }

            var kdObatBpjs = map.BridgingID;

            if (string.IsNullOrWhiteSpace(kdObatBpjs))
            {
                ShowAlert("BridgingID kosong.");
                return;
            }

            try
            {
                var svc = new Common.BPJS.Apotek.Service();

                var response = svc.UpdateStokObat(
                    new Common.BPJS.Apotek.Obat.UpdateStok.Request.Root
                    {
                        Kdobat = kdObatBpjs,
                        Stok = balance
                    });

                if (response?.MetaData?.Code == "200")
                {
                    ShowAlert($"Stok berhasil diupdate ke BPJS. KDOBAT: {kdObatBpjs}");
                }
                else
                {
                    ShowAlert(response?.MetaData?.Message ?? "Gagal update stok.");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error: " + ex.Message);
            }
        }

        private void ShowAlert(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(),
                Guid.NewGuid().ToString(),
                $"alert('{message.Replace("'", "")}');",
                true);
        }
    }
}
