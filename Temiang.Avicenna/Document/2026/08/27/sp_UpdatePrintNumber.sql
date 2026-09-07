CREATE OR ALTER PROCEDURE dbo.sp_UpdatePrintNumber
    @TransactionType     VARCHAR(50),
    @TransactionNo       VARCHAR(50),
    @LastPrintedByUserID VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    IF @TransactionType = 'PO'
    BEGIN
        UPDATE ItemTransaction
        SET
            PrintNumber = ISNULL(PrintNumber, 0) + 1,
            LastPrintedDateTime = GETDATE(),
            LastPrintedByUserID = @LastPrintedByUserID
        WHERE TransactionNo = @TransactionNo;
    END
END
GO