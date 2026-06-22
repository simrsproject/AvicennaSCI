
CREATE VIEW [dbo].[Vw_POPreComputeQtyFinished]
AS
SELECT 
    iti.ReferenceNo,
    iti.ReferenceSequenceNo,
    SUM(iti.Quantity * iti.ConversionFactor) AS QtyFinished
FROM ItemTransactionItem AS iti WITH (NOLOCK)
INNER JOIN ItemTransaction AS it WITH (NOLOCK) 
    ON it.TransactionNo = iti.TransactionNo
WHERE it.IsVoid = 0
  AND iti.ReferenceNo IS NOT NULL 
  AND iti.ReferenceNo <> ''
GROUP BY 
    iti.ReferenceNo, 
    iti.ReferenceSequenceNo;
GO