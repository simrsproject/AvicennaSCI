using System;
using System.Data;
using Temiang.Avicenna.BusinessObject;

namespace Temiang.Avicenna.Module.RADT.Admedika
{
    internal static class AdmedikaDischargeBillingProvider
    {
        public static DataTable Load(string registrationNo)
        {
            var result = CreateResultTable();

            if (string.IsNullOrWhiteSpace(registrationNo))
                return result;

            AppendCharges(result, registrationNo);
            AppendPrescriptions(result, registrationNo);

            var view = result.DefaultView;
            view.Sort = "IssueDate ASC, TransactionNo ASC, SequenceNo ASC";
            return view.ToTable();
        }

        private static void AppendCharges(DataTable target, string registrationNo)
        {
            var detail = new TransChargesItemQuery("a");
            var header = new TransChargesQuery("c");
            var item = new ItemQuery("b");

            detail.Select(
                detail.TransactionNo,
                detail.SequenceNo,
                detail.ItemID,
                item.ItemName,
                detail.ChargeQuantity.As("Quantity"),
                header.TransactionDate.As("IssueDate"),
                @"<CASE WHEN c.IsCorrection = 1
                    THEN 0 - (((ABS(a.ChargeQuantity) * a.Price) - a.DiscountAmount) + a.CitoAmount)
                    ELSE ((a.ChargeQuantity * a.Price) - a.DiscountAmount) + a.CitoAmount
                  END AS TotalPrice>"
            );

            detail.InnerJoin(header).On(detail.TransactionNo == header.TransactionNo);
            detail.InnerJoin(item).On(detail.ItemID == item.ItemID);
            detail.Where(
                header.RegistrationNo == registrationNo,
                header.IsVoid == false,
                detail.IsVoid == false,
                header.IsApproved == true,
                detail.IsApprove == true,
                detail.Or(detail.IsBillProceed == true, header.IsBillProceed == true),
                detail.Or(detail.ParentNo == string.Empty, detail.ParentNo.IsNull()),
                detail.Or(header.PackageReferenceNo == string.Empty, header.PackageReferenceNo.IsNull())
            );

            AppendRows(target, detail.LoadDataTable(), "CHARGE");
        }

        private static void AppendPrescriptions(DataTable target, string registrationNo)
        {
            var detail = new TransPrescriptionItemQuery("a");
            var header = new TransPrescriptionQuery("c");
            var item = new ItemQuery("b");

            detail.Select(
                detail.PrescriptionNo.As("TransactionNo"),
                detail.SequenceNo,
                @"<CASE WHEN ISNULL(a.ItemInterventionID, '') = ''
                    THEN a.ItemID ELSE a.ItemInterventionID END AS ItemID>",
                @"<CASE WHEN ISNULL(a.ItemInterventionID, '') = ''
                    THEN b.ItemName
                    ELSE ISNULL((SELECT TOP 1 ItemName FROM Item WHERE ItemID = a.ItemInterventionID), b.ItemName)
                  END AS ItemName>",
                detail.ResultQty.As("Quantity"),
                header.PrescriptionDate.As("IssueDate"),
                detail.LineAmount.As("TotalPrice")
            );

            detail.InnerJoin(header).On(detail.PrescriptionNo == header.PrescriptionNo);
            detail.InnerJoin(item).On(detail.ItemID == item.ItemID);
            detail.Where(
                header.RegistrationNo == registrationNo,
                header.IsVoid == false,
                detail.IsVoid == false,
                header.IsApproval == true,
                detail.IsApprove == true,
                detail.Or(detail.IsBillProceed == true, header.IsBillProceed == true)
            );

            AppendRows(target, detail.LoadDataTable(), "PRESCRIPTION");
        }

        private static void AppendRows(DataTable target, DataTable source, string sourceType)
        {
            if (source == null) return;

            foreach (DataRow sourceRow in source.Rows)
            {
                var row = target.NewRow();
                row["SourceType"] = sourceType;
                row["TransactionNo"] = Convert.ToString(sourceRow["TransactionNo"]);
                row["SequenceNo"] = Convert.ToString(sourceRow["SequenceNo"]);
                row["ItemID"] = Convert.ToString(sourceRow["ItemID"]);
                row["ItemName"] = Convert.ToString(sourceRow["ItemName"]);
                row["Quantity"] = ToDecimal(sourceRow["Quantity"]);
                row["TotalPrice"] = ToDecimal(sourceRow["TotalPrice"]);
                row["IssueDate"] = sourceRow["IssueDate"] == DBNull.Value
                    ? DBNull.Value
                    : sourceRow["IssueDate"];
                row["BenID"] = string.Empty;
                target.Rows.Add(row);
            }
        }

        private static DataTable CreateResultTable()
        {
            var table = new DataTable();
            table.Columns.Add("SourceType", typeof(string));
            table.Columns.Add("TransactionNo", typeof(string));
            table.Columns.Add("SequenceNo", typeof(string));
            table.Columns.Add("ItemID", typeof(string));
            table.Columns.Add("ItemName", typeof(string));
            table.Columns.Add("Quantity", typeof(decimal));
            table.Columns.Add("TotalPrice", typeof(decimal));
            table.Columns.Add("IssueDate", typeof(DateTime));
            table.Columns.Add("BenID", typeof(string));
            return table;
        }

        private static decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0;

            decimal result;
            return decimal.TryParse(Convert.ToString(value), out result) ? result : 0;
        }
    }
}
