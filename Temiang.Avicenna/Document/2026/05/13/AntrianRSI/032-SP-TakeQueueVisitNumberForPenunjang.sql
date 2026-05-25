CREATE OR ALTER PROCEDURE TakeQueueVisitNumberForPenunjang
(
    @RegistrationNo         NVARCHAR(50),
    @ServiceUnitID			NVARCHAR(50),
    @UserID                 NVARCHAR(50) = 'KIOSK',
    @TransDate              DATE = NULL,

    @VisitQueueNo           NVARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @VisitNo               NVARCHAR(50),
        @PatientID             NVARCHAR(50),
        @ToServiceUnitID       NVARCHAR(50),
        @ParamedicID           NVARCHAR(50),
        @SRAutoNumber          NVARCHAR(50),

        @CurrentStage          NVARCHAR(50),
        @StageID               NVARCHAR(50),
        @QueueKey              NVARCHAR(200),
        @QueueSequence         INT,
        @LastStatus            NVARCHAR(50),

        @TransactionNo         NVARCHAR(50),
        @TransactionDate       DATETIME,

		@CurrentStepOrder      INT,
		@NextStepOrder         INT;

    BEGIN TRY

        BEGIN TRAN;

        /* =========================================
           1. VALIDASI INPUT
        ========================================= */

        IF ISNULL(@RegistrationNo, '') = ''
        BEGIN
            THROW 50001, 'RegistrationNo wajib diisi', 1;
        END

        IF ISNULL(@ServiceUnitID, '') = ''
        BEGIN
            THROW 50002, 'ServiceUnitID wajib diisi', 1;
        END

        IF @TransDate IS NULL
        BEGIN
            SET @TransDate = CAST(GETDATE() AS DATE);
        END

        /* =========================================
           2. AMBIL VISIT SEBELUMNYA
        ========================================= */

        SELECT TOP 1
            @VisitNo = vq.VisitNo,
            @PatientID = vq.PatientID,
            @SRAutoNumber = vq.SRAutoNumber
        FROM VisitQueue vq WITH (NOLOCK)
        WHERE
            vq.RegistrationNo = @RegistrationNo
        ORDER BY vq.CreatedDate DESC;

        IF @VisitNo IS NULL
        BEGIN
            THROW 50003, 'Visit sebelumnya tidak ditemukan', 1;
        END

        /* =========================================
           3. CEK JOB ORDER PENUNJANG
        ========================================= */

        SELECT TOP 1
            @TransactionNo = tc.TransactionNo,
            @ToServiceUnitID = tc.ToServiceUnitID,
            @TransactionDate = tc.TransactionDate
        FROM TransCharges tc WITH (NOLOCK)
			   WHERE
			tc.RegistrationNo = @RegistrationNo
			AND tc.TransactionNo LIKE 'JO%'
			AND tc.ToServiceUnitID = @ServiceUnitID
			AND ISNULL(tc.IsVoid, 0) = 0
		ORDER BY tc.TransactionDate DESC;

        IF @TransactionNo IS NULL
        BEGIN
            THROW 50004, 'Job Order penunjang tidak ditemukan', 1;
        END

        /* =========================================
           4. VALIDASI LOKASI SCANNER
        ========================================= */

        IF @ServiceUnitID <> @ToServiceUnitID
        BEGIN
            THROW 50005, 'Job Order tidak sesuai unit penunjang medis', 1;
        END

        /* =========================================
		   5. AMBIL CURRENT STEP ORDER
		========================================= */

		SELECT TOP 1
			@CurrentStageID = vq.StageID,
			@CurrentStepOrder = qs.StepOrder
		FROM VisitQueue vq
		INNER JOIN QueueStage qs
			ON qs.StageID = vq.StageID
		WHERE
			vq.RegistrationNo = @RegistrationNo
			AND vq.ServiceUnitID = @ServiceUnitID
			AND vq.Status = 'FINISHED'
		ORDER BY vq.LastUpdated DESC;

		/* =========================================
		   NEXT STEP
		========================================= */

		IF @CurrentStepOrder IS NULL
		BEGIN
			SET @NextStepOrder = 1;
		END
		ELSE
		BEGIN
			SET @NextStepOrder = @CurrentStepOrder + 1;
		END

		/* =========================================
		   6. AMBIL NEXT STAGE
		========================================= */

		SELECT TOP 1
			@StageID = qs.StageID,
			@CurrentStage = qs.ServiceGroup
		FROM QueueMappingPivot qmp
		INNER JOIN QueueStage qs
			ON qs.StageID = qmp.StageID
		WHERE
			qmp.ServiceUnitID = @ToServiceUnitID
			AND qs.IsActive = 1
			AND qs.StepOrder = @NextStepOrder
		ORDER BY qs.StepOrder ASC;

		-- =========================================
		-- JIKA SUDAH STEP TERAKHIR
		-- =========================================
		IF @StageID IS NULL
		BEGIN
			COMMIT;
			RETURN;
		END

        /* =========================================
           6. VALIDASI DUPLIKAT QUEUE
        ========================================= */

        IF EXISTS
        (
            SELECT 1
            FROM VisitQueue vq
            WHERE
                vq.RegistrationNo = @RegistrationNo
                AND vq.StageID = @StageID
                AND vq.QueueDate = @TransDate
                AND vq.Status NOT IN ('FINISHED', 'CANCEL')
        )
        BEGIN
            THROW 50007, 'Queue aktif sudah ada', 1;
        END

        /* =========================================
           7. VALIDASI STAGE SEBELUMNYA
        ========================================= */

        SELECT TOP 1
            @LastStatus = vq.Status
        FROM VisitQueue vq WITH (UPDLOCK, HOLDLOCK)
        WHERE
            vq.VisitNo = @VisitNo
            AND vq.QueueDate = @TransDate
        ORDER BY vq.CreatedDate DESC;

        IF @LastStatus IS NOT NULL
        BEGIN
            IF @LastStatus NOT IN ('CALLED', 'FINISHED')
            BEGIN
                THROW 50008, 'Stage sebelumnya belum selesai', 1;
            END
        END

        /* =========================================
           8. GENERATE VISIT QUEUE NO
        ========================================= */

        EXEC GenerateVisitAutoNumber
			@SRAutoNumber = 'VisitQueueNo',
			@TransDate = @TransDate,
			@ResultNumber = @VisitQueueNo OUTPUT;

        /* =========================================
           9. GENERATE QUEUE KEY
        ========================================= */

        SET @QueueKey =
            ISNULL(@ToServiceUnitID, '')
            + '|'
            + ISNULL(@StageID, '')
            + '|'
            + ISNULL(@TransactionNo, '');

        /* =========================================
           10. GENERATE QUEUE SEQUENCE
        ========================================= */

        SELECT
            @QueueSequence =
                ISNULL(MAX(vq.QueueSequence), 0) + 10
        FROM VisitQueue vq WITH (UPDLOCK, HOLDLOCK)
        WHERE
            vq.QueueDate = @TransDate
            AND vq.CurrentStage = @CurrentStage;

        /* =========================================
           11. INSERT VISIT QUEUE
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
            @ToServiceUnitID,
            NULL,
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

DECLARE @VisitQueueNo NVARCHAR(50);

EXEC TakeQueueVisitNumberForPenunjang
    @RegistrationNo = 'REG/OP/260516-0008',
	@ServiceUnitID = 'D3.0.07',
    @VisitQueueNo = @VisitQueueNo OUTPUT;

SELECT @VisitQueueNo;