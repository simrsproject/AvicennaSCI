CREATE OR ALTER PROCEDURE TakeQueueVisitNumberForFarmasi
(
    @RegistrationNo NVARCHAR(50),
    @ServiceUnitID  NVARCHAR(50),
    @UserID         NVARCHAR(50) = 'KIOSK_FARMASI',
    @TransDate      DATE = NULL,

    @VisitQueueNo   NVARCHAR(50) OUTPUT
)
AS
BEGIN

    SET NOCOUNT ON;

    DECLARE
        @VisitNo          NVARCHAR(50),
        @PatientID        NVARCHAR(50),
        @SRAutoNumber     NVARCHAR(50),

        @CurrentStage     NVARCHAR(50),
        @StageID          NVARCHAR(50),

        @QueueKey         NVARCHAR(200),
        @QueueSequence    INT,
        @LastStatus       NVARCHAR(50);

    BEGIN TRY

        BEGIN TRAN;

        -- =========================================
        -- DEFAULT DATE
        -- =========================================
        IF @TransDate IS NULL
        BEGIN
            SET @TransDate = CAST(GETDATE() AS DATE);
        END

        -- =========================================
        -- AMBIL VISIT TERAKHIR
        -- =========================================
        SELECT TOP 1
            @VisitNo      = vq.VisitNo,
            @PatientID    = vq.PatientID,
            @SRAutoNumber = vq.SRAutoNumber
        FROM VisitQueue vq
        WHERE vq.RegistrationNo = @RegistrationNo
        ORDER BY vq.CreatedDate DESC;

        IF @VisitNo IS NULL
        BEGIN
            THROW 50001, 'Visit tidak ditemukan', 1;
        END

        -- =========================================
        -- AMBIL STAGE FARMASI
        -- =========================================
        SELECT TOP 1
            @StageID = qs.StageID,
            @CurrentStage = qs.ServiceGroup
        FROM QueueMappingPivot qmp
        INNER JOIN QueueStage qs
            ON qs.StageID = qmp.StageID
        WHERE
            qmp.ServiceUnitID = @ServiceUnitID
            AND qs.IsActive = 1
        ORDER BY qs.StepOrder ASC;

        IF @StageID IS NULL
        BEGIN
            THROW 50002, 'Stage farmasi tidak ditemukan', 1;
        END

        -- =========================================
        -- VALIDASI DUPLIKAT
        -- =========================================
        IF EXISTS
        (
            SELECT 1
            FROM VisitQueue
            WHERE
                RegistrationNo = @RegistrationNo
                AND StageID = @StageID
                AND Status NOT IN ('FINISHED', 'CANCEL')
        )
        BEGIN
            THROW 50003, 'Queue farmasi masih aktif', 1;
        END

        -- =========================================
        -- VALIDASI STAGE SEBELUMNYA
        -- =========================================
        SELECT TOP 1
            @LastStatus = Status
        FROM VisitQueue
        WHERE
            VisitNo = @VisitNo
        ORDER BY CreatedDate DESC;

        IF @LastStatus NOT IN ('CALLED', 'FINISHED')
        BEGIN
            THROW 50004, 'Stage sebelumnya belum selesai', 1;
        END

        -- =========================================
        -- GENERATE NUMBER
        -- =========================================
        EXEC GenerateVisitAutoNumber
            @SRAutoNumber = 'VisitQueueNo',
            @TransDate = @TransDate,
            @ResultNumber = @VisitQueueNo OUTPUT;

        -- =========================================
        -- GENERATE SEQUENCE
        -- =========================================
        SELECT
            @QueueSequence =
                ISNULL(MAX(QueueSequence), 0) + 10
        FROM VisitQueue
        WHERE
            QueueDate = @TransDate
            AND CurrentStage = @CurrentStage
            AND StageID = @StageID;

        -- =========================================
        -- GENERATE KEY
        -- =========================================
        SET @QueueKey =
            ISNULL(@ServiceUnitID, '')
            + '|'
            + ISNULL(@StageID, '');

        -- =========================================
        -- INSERT
        -- =========================================
        INSERT INTO VisitQueue
        (
            VisitQueueNo,
            VisitNo,
            SRAutoNumber,
            RegistrationNo,
            QueueDate,
            Status,
            CurrentStage,
            ServiceUnitID,
            PatientID,
            StageID,
            QueueSequence,
            Priority,
            QueueKey,
            IsManualOverride,
            IsRecall,
            CreatedBy,
            CreatedDate,
            LastUpdated,
            UpdatedBy
        )
        VALUES
        (
            @VisitQueueNo,
            @VisitNo,
            @SRAutoNumber,
            @RegistrationNo,
            @TransDate,
            'WAITING',
            @CurrentStage,
            @ServiceUnitID,
            @PatientID,
            @StageID,
            @QueueSequence,
            100,
            @QueueKey,
            0,
            0,
            @UserID,
            GETDATE(),
            GETDATE(),
            @UserID
        );

		-- =========================================
        -- RESULT
        -- =========================================
        SELECT
            VisitQueueNo,
            VisitNo,
            RegistrationNo,
            ServiceUnitID,
            CurrentStage,
            StageID,
            QueueSequence,
            Status
        FROM VisitQueue
        WHERE VisitQueueNo = @VisitQueueNo;

        COMMIT;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH

END
GO

DECLARE @VisitQueueNo NVARCHAR(50);

EXEC TakeQueueVisitNumberForFarmasi
    @RegistrationNo = 'REG/OP/260516-0008',
	@ServiceUnitID = 'D3.0.01.2',
    @VisitQueueNo = @VisitQueueNo OUTPUT;

SELECT @VisitQueueNo;