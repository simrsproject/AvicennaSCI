CREATE OR ALTER PROCEDURE AntrianMoveNextStage
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @CurrentStageID      VARCHAR(50),
        @CurrentStage        VARCHAR(50),
        @ServiceUnitID       VARCHAR(50),
        @CurrentServiceGroup VARCHAR(50),
        @CurrentStepOrder    INT,
        @NextStageID         VARCHAR(50),
        @NextStepOrder       INT,
		@CategoryID		     VARCHAR(50),
		@ParamedicID		 VARCHAR(50),
		@QueueDate			 DATE,
		@MaxQueueSeq		 INT,
		@NewQueueSeq		 INT,
		@NewVisitQueueNo	 VARCHAR(50),
		@VisitNo			 VARCHAR(50),
		@RegistrationNo		 VARCHAR(50),
		@PatientID			 VARCHAR(50),
		@Priority			 INT,
		@IsManualOverride	 BIT,
		@QueueLocation		 VARCHAR(50),
		@CreatedBy			 VARCHAR(50),
		@CreatedDate		 DATETIME,
		@LastQueueNumber	 INT,
		@SRAutoNumber VARCHAR(100),
        @Status              VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- 1. AMBIL DATA ANTRIAN
        -- =========================================
        SELECT
			@CurrentStageID = StageID,
			@CurrentStage = CurrentStage,
			@ServiceUnitID = ServiceUnitID,
			@Status = Status,
			@CategoryID = CategoryID,
			@ParamedicID = ParamedicID,
			@QueueDate = CAST(QueueDate AS DATE),

			@VisitNo = VisitNo,
			@RegistrationNo = RegistrationNo,
			@SRAutoNumber = SRAutoNumber,
			@PatientID = PatientID,
			@Priority = Priority,
			@IsManualOverride = IsManualOverride,
			@QueueLocation = QueueLocation,
			@CreatedBy = CreatedBy,
			@CreatedDate = CreatedDate
		FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
		WHERE VisitQueueNo = @VisitQueueNo;

        IF @CurrentStageID IS NULL
        BEGIN
            THROW 50001, 'Data antrian tidak ditemukan.', 1;
        END

        -- =========================================
        -- OPTIONAL VALIDASI STATUS
        -- =========================================
        IF @Status NOT IN ('WAITING')
        BEGIN
            THROW 50002,
            'Status antrian tidak dapat dipindahkan ke tahap berikutnya.',
            1;
        END

        -- =========================================
        -- 2. AMBIL INFORMASI STAGE SEKARANG
        -- =========================================
        SELECT
            @CurrentServiceGroup = ServiceGroup,
            @CurrentStepOrder = StepOrder
        FROM QueueStage
        WHERE StageID = @CurrentStageID;

        IF @CurrentServiceGroup IS NULL
        BEGIN
            THROW 50003,
            'StageID tidak ditemukan pada QueueStage.',
            1;
        END

        -- =========================================
        -- 3. CARI STAGE BERIKUTNYA
        -- =========================================
        SELECT TOP (1)
            @NextStageID = StageID,
            @NextStepOrder = StepOrder
        FROM QueueStage
        WHERE
            ServiceGroup = @CurrentServiceGroup
            AND StepOrder > @CurrentStepOrder
            AND IsActive = 1
        ORDER BY StepOrder;

        IF @NextStageID IS NULL
        BEGIN
            THROW 50004,
            'Pasien sudah berada pada tahap terakhir.',
            1;
        END

		-- =========================================
		-- 4. AMBIL QUEUE SEQUENCE TERAKHIR
		-- =========================================
		SELECT
			@MaxQueueSeq = MAX(QueueSequence)
		FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
		WHERE
			CAST(QueueDate AS DATE) = @QueueDate
			AND StageID = @NextStageID
			AND ServiceUnitID = @ServiceUnitID
			AND ISNULL(ParamedicID,'') = ISNULL(@ParamedicID,'')
			AND ISNULL(CategoryID,'') = ISNULL(@CategoryID,'');

		SET @NewQueueSeq = ISNULL(@MaxQueueSeq,0) + 10;

				-- =========================================
		-- GENERATE VISIT QUEUE NO BARU
		-- FORMAT : VQUE-YYMMDD-0001
		-- =========================================

		SELECT
			@LastQueueNumber =
				MAX(
					TRY_CAST(RIGHT(VisitQueueNo, 4) AS INT)
				)
		FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
		WHERE
			QueueDate = @QueueDate
			AND VisitQueueNo LIKE
				'VQUE-'
				+ RIGHT(CAST(YEAR(@QueueDate) AS VARCHAR(4)),2)
				+ RIGHT('0' + CAST(MONTH(@QueueDate) AS VARCHAR(2)),2)
				+ RIGHT('0' + CAST(DAY(@QueueDate) AS VARCHAR(2)),2)
				+ '-%';

		SET @LastQueueNumber = ISNULL(@LastQueueNumber,0) + 1;

		SET @NewVisitQueueNo =
			'VQUE-'
			+ RIGHT(CAST(YEAR(@QueueDate) AS VARCHAR(4)),2)
			+ RIGHT('0' + CAST(MONTH(@QueueDate) AS VARCHAR(2)),2)
			+ RIGHT('0' + CAST(DAY(@QueueDate) AS VARCHAR(2)),2)
			+ '-'
			+ RIGHT('0000' + CAST(@LastQueueNumber AS VARCHAR(4)),4);

        -- =========================================
        -- 4. UPDATE VISITQUEUE
        -- =========================================
        UPDATE VisitQueue
		SET
			Status = 'FINISHED',
			FinishedTime = GETDATE(),
			UpdatedBy = @UserID,
			LastUpdated = GETDATE()
		WHERE VisitQueueNo = @VisitQueueNo;

		INSERT INTO VisitQueue
		(
			VisitQueueNo,
			VisitNo,
			SRAutoNumber,
			RegistrationNo,
			QueueDate,
			Status,
			CurrentStage,
			CalledByCounterID,
			CalledTime,
			ServedTime,
			FinishedTime,
			PatientID,
			CreatedDate,
			CreatedBy,
			QueueSequence,
			Priority,
			IsManualOverride,
			LastUpdated,
			UpdatedBy,
			ServiceUnitID,
			ParamedicID,
			StageID,
			CategoryID,
			QueueKey,
			QueueLocation,
			RecallCount
		)
		VALUES
		(
			@NewVisitQueueNo,
			@VisitNo,
			@SRAutoNumber,
			@RegistrationNo,
			@QueueDate,
			'WAITING',
			@CurrentStage,
			NULL,
			NULL,
			NULL,
			NULL,
			@PatientID,
			GETDATE(),
			@UserID,
			@NewQueueSeq,
			@Priority,
			@IsManualOverride,
			NULL,
			NULL,
			@ServiceUnitID,
			@ParamedicID,
			@NextStageID,
			@CategoryID,
			CONCAT(@ServiceUnitID,'|',@NextStageID),
			@QueueLocation,
			0
		);

        -- =========================================
        -- 5. RETURN DATA
        -- =========================================
        SELECT
            VisitQueueNo,
            VisitNo,
            RegistrationNo,
            Status,
            CurrentStage,
            StageID,
            QueueKey,
            QueueSequence,
            ServiceUnitID,
            ParamedicID,
            CategoryID,
            UpdatedBy,
            LastUpdated
        FROM VisitQueue
        WHERE VisitQueueNo = @NewVisitQueueNo;

        COMMIT;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH
END
GO

EXEC AntrianMoveNextStage
    @VisitQueueNo='VQUE-260713-0010',
    @UserID='240092'