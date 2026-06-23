CREATE OR ALTER PROCEDURE GetVisitQueueReceipt
(
    @VisitQueueNo NVARCHAR(50),
    @UserID       NVARCHAR(50),
	@BarcodeImage NVARCHAR(MAX),
	@IPAdress     NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
		vq.VisitQueueNo,
		vq.VisitNo,
		vq.QueueDate,
		vq.CreatedDate,

		aans.PayerType,
		aans.Channel,

		CASE
			WHEN aans.PayerType = 'BPJS'
				THEN aans.ServiceGroup
			ELSE NULL
		END AS ServiceGroup,

		@IPAdress AS IPAdress,
		@UserID AS UserID,

		vqb.BarcodeImage
	FROM VisitQueue vq
	LEFT JOIN AntrianAutoNumberSemantic aans
		ON aans.SRAutoNumber = vq.SRAutoNumber
	   AND aans.Channel      = vq.QueueLocation

	LEFT JOIN VisitQueueBarcode vqb
		ON vqb.VisitQueueNo = vq.VisitQueueNo

	WHERE vq.VisitQueueNo = @VisitQueueNo;

END
GO

Exec GetVisitQueueReceipt
@VisitQueueNo = 'VQUE-260515-0002',
@IPAdress = '',
@UserID = '240076'
