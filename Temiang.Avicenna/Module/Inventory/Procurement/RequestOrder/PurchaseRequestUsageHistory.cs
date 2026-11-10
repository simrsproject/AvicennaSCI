using System;
using System.Data;
using System.Linq;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.Reference;

namespace Temiang.Avicenna.Module.Inventory.Procurement
{
    public static class PurchaseRequestUsageHistory
    {
        internal static bool ShouldUseItemUnits(string itemType)
        {
            if (itemType == ItemType.Medical)
            {
                var setting = Convert.ToString(AppParameter.GetParameterNullableValue(
                    AppParameter.ParameterItem.IsPurchaseRequestUsageHistoryMedicalUsingItemUnit)).Trim();
                if (string.Equals(setting, "Yes", StringComparison.OrdinalIgnoreCase))
                    return true;
                if (string.Equals(setting, "No", StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            var existingSetting = Convert.ToString(AppParameter.GetParameterNullableValue(
                AppParameter.ParameterItem.IsPurchaseRequestsUsingItemUnit)).Trim();
            return AppParameter.IsYes(existingSetting);
        }

        public static void UseItemUnits(DataTable items)
        {
            foreach (DataRow row in items.Rows)
            {
                row["ConversionFactor"] = 1;
                row["SRPurchaseUnit"] = row["SRItemUnit"];
                row["Unit"] = row["SRItemUnit"];
                row["Price"] = row["PriceInBaseUnit"];
                row["PurcUnitBalance"] = row["QtyOrder"];
            }
        }

        public static void AddLatestOutstandingPurchaseOrders(DataTable items, string purchasingUnitId, string itemType)
        {
            items.Columns.Add("LastPODate", typeof(DateTime));
            items.Columns.Add("LastPONo", typeof(string));
            items.Columns.Add("LastPOQuantity", typeof(decimal));

            // Never show orders from another purchasing unit when the PR unit is absent.
            if (items.Rows.Count == 0 || string.IsNullOrEmpty(purchasingUnitId)) return;

            var itemIds = items.AsEnumerable().Select(row => row.Field<string>("ItemID")).Distinct().ToArray();
            DataTable orders = null;
            // Keep IN parameters comfortably below SQL Server's parameter limit.
            for (var offset = 0; offset < itemIds.Length; offset += 500)
            {
                var detail = new ItemTransactionItemQuery("pod");
                var header = new ItemTransactionQuery("poh");
                var receipt = new ItemTransactionItemQuery("rd");
                var receiptHeader = new ItemTransactionQuery("rh");

                receipt.InnerJoin(receiptHeader).On(receipt.TransactionNo == receiptHeader.TransactionNo);
                receipt.Select((receipt.Quantity * receipt.ConversionFactor).Sum().As("ReceivedQuantity"));
                receipt.Where(receipt.ReferenceNo == detail.TransactionNo,
                    receipt.ReferenceSequenceNo == detail.SequenceNo,
                    receiptHeader.TransactionCode == TransactionCode.PurchaseOrderReceive,
                    receiptHeader.IsApproved == true, receiptHeader.IsVoid == false);

                detail.InnerJoin(header).On(detail.TransactionNo == header.TransactionNo);
                detail.Select(detail.ItemID, header.TransactionNo, header.TransactionDate,
                    (detail.Quantity * detail.ConversionFactor).As("OrderedInBaseUnit"),
                    receipt.Select().As("ReceivedInBaseUnit"));
                detail.Where(header.TransactionCode == TransactionCode.PurchaseOrder,
                    header.IsApproved == true, header.IsVoid == false, header.IsClosed == false,
                    header.FromServiceUnitID == purchasingUnitId, header.SRItemType == itemType,
                    detail.IsClosed == false, detail.ItemID.In(itemIds.Skip(offset).Take(500).ToArray()));

                var batch = detail.LoadDataTable();
                if (orders == null) orders = batch;
                else orders.Merge(batch);
            }

            var latest = SelectLatestOutstandingPurchaseOrders(orders);
            foreach (DataRow row in items.Rows)
            {
                var po = latest.Rows.Find(row["ItemID"]);
                if (po == null) continue;
                row["LastPODate"] = po["LastPODate"];
                row["LastPONo"] = po["LastPONo"];
                row["LastPOQuantity"] = po["LastPOQuantity"];
            }
        }

        public static DataTable SelectLatestOutstandingPurchaseOrders(DataTable orders)
        {
            var result = new DataTable();
            result.Columns.Add("ItemID", typeof(string));
            result.Columns.Add("LastPODate", typeof(DateTime));
            result.Columns.Add("LastPONo", typeof(string));
            result.Columns.Add("LastPOQuantity", typeof(decimal));
            result.PrimaryKey = new[] { result.Columns["ItemID"] };
            if (orders == null) return result;

            // Aggregate repeated item lines within the same PO, in the smallest unit.
            var outstanding = orders.AsEnumerable()
                .Select(row => new
                {
                    ItemId = row.Field<string>("ItemID"),
                    Number = row.Field<string>("TransactionNo"),
                    Date = row.Field<DateTime>("TransactionDate"),
                    Quantity = row.Field<decimal>("OrderedInBaseUnit") -
                        (row.Field<decimal?>("ReceivedInBaseUnit") ?? 0)
                })
                .Where(row => row.Quantity > 0)
                .GroupBy(row => new { row.ItemId, row.Number, row.Date })
                .Select(group => new
                {
                    group.Key.ItemId, group.Key.Number, group.Key.Date,
                    Quantity = group.Sum(row => row.Quantity)
                });

            foreach (var item in outstanding.GroupBy(row => row.ItemId))
            {
                var latest = item.OrderByDescending(row => row.Date)
                    .ThenByDescending(row => row.Number, StringComparer.Ordinal).First();
                result.Rows.Add(latest.ItemId, latest.Date, latest.Number, latest.Quantity);
            }
            return result;
        }
    }
}
