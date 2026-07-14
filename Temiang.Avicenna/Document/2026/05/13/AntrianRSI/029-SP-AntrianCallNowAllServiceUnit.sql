CREATE OR ALTER PROCEDURE AntrianCallNowAllServiceUnit
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50),
	@Kamar        VARCHAR(10) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @CurrentStatus VARCHAR(50),
        @QueueDate     DATE,
        @StageID       VARCHAR(50),
		@CurrentStage  VARCHAR(50),
        @ServiceUnitID VARCHAR(50),
		@CategoryID	   VARCHAR(50),
		@QueueLocation VARCHAR(50),
        @ParamedicID   VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- 1. AMBIL DATA TARGET + LOCK
        -- =========================================
        SELECT
            @CurrentStatus = vq.Status,
            @QueueDate     = CAST(vq.QueueDate AS DATE),
            @CurrentStage  = vq.CurrentStage,
			@StageID       = vq.StageID,
            @ServiceUnitID = vq.ServiceUnitID,
            @ParamedicID   = vq.ParamedicID,
			@CategoryID    = vq.CategoryID
        FROM VisitQueue vq WITH (UPDLOCK, HOLDLOCK)
        WHERE vq.VisitQueueNo = @VisitQueueNo;

        -- =========================================
        -- VALIDASI DATA
        -- =========================================
        IF @CurrentStatus IS NULL
        BEGIN
            THROW 50001,
            'Data antrian tidak ditemukan',
            1;
        END

        -- =========================================
        -- VALIDASI STATUS
        -- =========================================
        IF @CurrentStatus <> 'WAITING'
        BEGIN
            THROW 50002,
            'Hanya antrian WAITING yang bisa dipanggil',
            1;
        END

		-- =========================================
		-- MAPPING KAMAR (OPTIONAL)
		-- =========================================
		IF ISNULL(@Kamar,'') <> ''
		BEGIN
			SELECT
				@QueueLocation = KamarCode
			FROM ListKamarForAntrian
			WHERE
				KamarID = TRY_CAST(@Kamar AS INT)
				AND IsActive = 1;
		END
		ELSE
		BEGIN
			SET @QueueLocation = NULL;
		END

        -- =========================================
        -- 2. TURUNKAN CALLED → PENDING
        -- GROUP YANG SAMA
        -- =========================================
        UPDATE VisitQueue
        SET
            Status           = 'PENDING',
            QueueSequence    = NULL,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE
            Status = 'CALLED'
            AND CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @CurrentStage
            AND StageID = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID, '') = ISNULL(@ParamedicID, '')
			AND ISNULL(CategoryID, '') = ISNULL(@CategoryID, '');

        -- =========================================
        -- 3. UPDATE TARGET → CALLED
        -- =========================================
        UPDATE VisitQueue
        SET
            Status           = 'CALLED',
            CalledTime       = GETDATE(),
			QueueLocation    = @QueueLocation,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE
            VisitQueueNo = @VisitQueueNo;

        -- =========================================
        -- 4. RETURN RESULT
        -- =========================================
        SELECT
            VisitQueueNo,
            VisitNo,
            Status,
            StageID,
			CurrentStage,
			CategoryID,
            ServiceUnitID,
            ParamedicID,
			QueueLocation,
            CalledTime,
            LastUpdated,
            UpdatedBy
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

DECLARE @VisitNo VARCHAR(50);

EXEC AntrianCallNowAllServiceUnit
    @VisitQueueNo = 'VQUE-260713-0114',
    @UserID       = '240076',
	@Kamar        = 2

SELECT @VisitNo;