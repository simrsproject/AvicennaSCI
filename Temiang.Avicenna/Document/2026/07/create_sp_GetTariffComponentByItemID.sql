CREATE OR ALTER PROCEDURE sp_GetTariffComponentByItemID
(
    @ItemID VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT tc.TariffComponentID
    FROM
    (
        SELECT DISTINCT TariffComponentID
        FROM TransChargesItemComp WITH (NOLOCK)
    ) AS tc
    WHERE EXISTS
    (
        SELECT 1
        FROM TransChargesItem tci WITH (NOLOCK)
        INNER JOIN TransChargesItemComp tcic WITH (NOLOCK)
            ON tcic.TransactionNo = tci.TransactionNo
           AND tcic.TariffComponentID = tc.TariffComponentID
        WHERE tci.ItemID = @ItemID
          --AND tci.CreatedDateTime >= DATEADD(DAY, -700, GETDATE())
    )
    ORDER BY tc.TariffComponentID;
END;
GO