CREATE PROCEDURE [dbo].[sp_UpdateHistStockPerStockGroup]
	@p_StockGroup VARCHAR(20),
	@p_UserID VARCHAR(20)
AS
SET NOCOUNT ON  

--DECLARE @p_StockGroup VARCHAR(20) = 'FI'
--DECLARE @p_UserID VARCHAR(20) = 'sci'

DECLARE @maxSalesPeriod INT = CAST(ISNULL((SELECT ap.ParameterValue FROM AppParameter AS ap WHERE ap.ParameterID = 'PorBaseSalesDayMax'), '90') AS INT)
DECLARE @startDate DATETIME = CAST(DATEADD(DAY,(0-@maxSalesPeriod), GETDATE()) AS DATE)

DELETE FROM ItemSalesPerDate WHERE MovementDate >= @startDate AND SRStockGroup = @p_StockGroup
INSERT INTO ItemSalesPerDate
(
	MovementDate,
	SRStockGroup,
	ItemID,
	ServiceUnitID,
	LocationID,
	QuantityOut,
	LastUpdateDateTime,
	LastUpdateByUserID
)
SELECT CAST(im.MovementDate AS DATE),
    l.SRStockGroup,
    im.ItemID,
    im.ServiceUnitID,
    im.LocationID,
    SUM(CASE WHEN im.TransactionCode IN ('091', '094', '003') THEN im.QuantityOut - im.QuantityIN ELSE im.QuantityOut END),
    GETDATE(),
    @p_UserID
FROM ItemMovement AS im WITH (NOLOCK)
INNER JOIN Location l WITH (NOLOCK) ON l.LocationID = im.LocationID 
WHERE im.MovementDate >= @startDate 
	AND im.TransactionCode IN ('091', '094', '003', '082','074','075')
	AND l.SRStockGroup = @p_StockGroup
GROUP BY
	CAST(im.MovementDate AS DATE),
    l.SRStockGroup,
    im.ItemID,
    im.ServiceUnitID,
    im.LocationID

DELETE FROM ItemBalanceByStockGroup WHERE SRStockGroup = @p_StockGroup
INSERT INTO ItemBalanceByStockGroup
(
	SRStockGroup,
	ItemID,
	Minimum,
	Maximum,
	Balance,
	LastUpdateDateTime,
	LastUpdateByUserID
)
SELECT q.SRStockGroup,
	q.ItemID,
	0,
	0,
	SUM(q.Balance),
	GETDATE(),
	@p_UserID
FROM (
	SELECT 
		l.SRStockGroup, 
		ib.ItemID,
        Balance = (SELECT TOP 1 COALESCE(im.InitialStock, 0) + COALESCE(im.QuantityIn, 0) - COALESCE(im.QuantityOut, 0)
					FROM ItemMovement AS im WITH (NOLOCK)
					WHERE im.ItemID = ib.ItemID 
						AND im.LocationID = l.LocationID
                   ORDER BY im.MovementDate DESC)
           FROM Location l
           INNER JOIN ItemBalance AS ib WITH (NOLOCK) ON ib.LocationID = l.LocationID
           WHERE l.SRStockGroup = @p_StockGroup
       ) q
WHERE  q.Balance IS NOT NULL
GROUP BY
       q.SRStockGroup,
       q.ItemID



