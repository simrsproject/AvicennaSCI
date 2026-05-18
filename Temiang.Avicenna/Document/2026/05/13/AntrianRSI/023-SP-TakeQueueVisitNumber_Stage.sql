CREATE OR ALTER PROCEDURE TakeQueueVisitNumber_Stage
(
    @VisitNo           NVARCHAR(50),
    @SRAutoNumber      NVARCHAR(50),
    @UserID            NVARCHAR(50),
    @TransDate         DATE,

    @ServiceUnitID     NVARCHAR(50) = NULL,
    @ParamedicID       NVARCHAR(50) = NULL,
    @RegistrationNo    NVARCHAR(50) = NULL,
    @PatientID         NVARCHAR(50) = NULL,

    @VisitQueueNo      NVARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @CurrentStage      NVARCHAR(50),
        @StageID           NVARCHAR(50),
        @QueueKey          NVARCHAR(200),
        @QueueSequence     INT,
        @LastStatus        NVARCHAR(50);

    BEGIN TRY

        BEGIN TRAN;

        /* =========================================
           1. VALIDASI INPUT
        ========================================= */

        IF @VisitNo IS NULL
        BEGIN
            THROW 50001, 'VisitNo wajib diisi', 1;
        END

        IF @ServiceUnitID IS NULL
        BEGIN
            THROW 50002, 'ServiceUnitID wajib diisi', 1;
        END

        /* =========================================
           2. AMBIL STAGE PERTAMA
        ========================================= */

        SELECT TOP 1
            @StageID = qs.StageID,
            @CurrentStage = qs.ServiceGroup
        FROM QueueMappingPivot qmp
        INNER JOIN QueueStage qs
            ON qs.StageID = qmp.StageID
        WHERE
            qmp.ServiceUnitID = @ServiceUnitID
            AND qs.IsActive = 1
            AND qs.StepOrder = 1
        ORDER BY qs.StepOrder ASC;

        IF @StageID IS NULL
        BEGIN
            THROW 50003, 'Stage awal tidak ditemukan', 1;
        END

        /* =========================================
           3. VALIDASI STAGE SEBELUMNYA
        ========================================= */

        SELECT TOP 1
            @LastStatus = Status
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE
            VisitNo = @VisitNo
            AND QueueDate = @TransDate
        ORDER BY CreatedDate DESC;

        IF @LastStatus IS NOT NULL
        BEGIN
            IF @LastStatus NOT IN ('CALLED', 'FINISHED')
            BEGIN
                THROW 50004, 'Stage sebelumnya belum selesai', 1;
            END
        END

        /* =========================================
           4. CEK DUPLIKAT ACTIVE QUEUE
        ========================================= */

        IF EXISTS
        (
            SELECT 1
            FROM VisitQueue
            WHERE
                VisitNo = @VisitNo
                AND CurrentStage = @CurrentStage
                AND QueueDate = @TransDate
                AND Status NOT IN ('FINISHED', 'CANCEL')
        )
        BEGIN
            THROW 50005, 'Queue aktif sudah ada pada stage ini', 1;
        END

        /* =========================================
           5. GENERATE VISIT QUEUE NO
        ========================================= */

        EXEC GenerateVisitAutoNumber
            @SRAutoNumber = 'VisitQueueNo',
            @TransDate = @TransDate,
            @ResultNumber = @VisitQueueNo OUTPUT;

        /* =========================================
           6. GENERATE QUEUE KEY
        ========================================= */

        SET @QueueKey =
            ISNULL(@ServiceUnitID, '')
            + '|'
            + ISNULL(@StageID, '')
			+ '|'
			+ ISNULL(@ParamedicID,'');
        /* =========================================
           7. GENERATE QUEUE SEQUENCE
        ========================================= */

        SELECT
            @QueueSequence =
                ISNULL(MAX(QueueSequence), 0) + 10
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE
            QueueDate = @TransDate
            AND CurrentStage = @CurrentStage;

        /* =========================================
           8. INSERT VISIT QUEUE
        ========================================= */

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
            ParamedicID,
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
            @ParamedicID,
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

        /* =========================================
           9. RESULT
        ========================================= */

        SELECT
            @VisitQueueNo AS VisitQueueNo,
            @VisitNo AS VisitNo,
            @CurrentStage AS CurrentStage,
            @StageID AS StageID,
            @QueueSequence AS QueueSequence;

        COMMIT;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        DECLARE @ErrMsg NVARCHAR(MAX);

        SET @ErrMsg = ERROR_MESSAGE();

        RAISERROR(@ErrMsg, 16, 1);

    END CATCH
END
GO