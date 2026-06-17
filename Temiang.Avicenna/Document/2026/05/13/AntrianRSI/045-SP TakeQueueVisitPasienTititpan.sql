CREATE OR ALTER PROCEDURE TakeQueueVisitPasienTitipan
(
    @GuarantorID NVARCHAR(50),
	@ServiceUnitID NVARCHAR(50),
    @UserID      NVARCHAR(50),
    @TransDate   DATE,
    @VisitNo     NVARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @GuarantorType NVARCHAR(50),
        @SRAutoNumber  NVARCHAR(50),
		@ServiceGroup VARCHAR(50);

    SELECT
        @GuarantorType = SRGuarantorType
    FROM Guarantor
    WHERE GuarantorID = @GuarantorID;

    IF @GuarantorType IS NULL
    BEGIN
        RAISERROR('Guarantor tidak ditemukan',16,1);
        RETURN;
    END

	SELECT TOP 1
    @ServiceGroup = qs.ServiceGroup
	FROM QueueMappingPivot qmp
	INNER JOIN QueueStage qs
		ON qs.StageID = qmp.StageID
	WHERE qmp.ServiceUnitID = @ServiceUnitID;

    IF @GuarantorType = '00'
	BEGIN
		SET @SRAutoNumber = 'VisitTunaiNo';
	END
	ELSE IF @GuarantorType = '09'
	BEGIN

		IF @ServiceGroup = 'POLI'
			SET @SRAutoNumber = 'VisitBpjsPoliNo';

		ELSE IF @ServiceGroup = 'REHAB'
			SET @SRAutoNumber = 'VisitBpjsIrmNo';

		ELSE IF @ServiceGroup = 'HEMODIALISA'
			SET @SRAutoNumber = 'VisitBpjsHdNo';

		ELSE IF @ServiceGroup IN
		(
			'CT SCAN',
			'ENDOSCOPY',
			'LAB',
			'RADIOLOGI',
			'USG'
		)
			SET @SRAutoNumber = 'VisitBpjsPenunjangNo';

		ELSE
		BEGIN
			RAISERROR(
				'ServiceGroup BPJS belum memiliki konfigurasi Auto Number',
				16,
				1
			);
			RETURN;
		END
	END
	ELSE
	BEGIN
		SET @SRAutoNumber = 'VisitMitraNo';
	END

    EXEC GenerateVisitAutoNumber
        @SRAutoNumber = @SRAutoNumber,
        @TransDate    = @TransDate,
        @ResultNumber = @VisitNo OUTPUT;

    SELECT
        @VisitNo        AS VisitNo,
        @GuarantorID    AS GuarantorID,
        @GuarantorType  AS GuarantorType,
        @SRAutoNumber   AS SRAutoNumber,
        @UserID         AS GeneratedBy;
END
GO

DECLARE
    @VisitNo VARCHAR(50),
    @TransDate DATE;

SET @TransDate = GETDATE();

EXEC TakeQueueVisitPasienTitipan
    @GuarantorID = 'B2260',
	@ServiceUnitID = 'D2.2.02',
    @UserID       = 'APMPD01',
    @TransDate    = @TransDate,
    @VisitNo      = @VisitNo OUTPUT;