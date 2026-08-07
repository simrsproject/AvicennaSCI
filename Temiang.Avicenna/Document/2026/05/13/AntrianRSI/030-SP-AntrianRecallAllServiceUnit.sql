CREATE OR ALTER PROCEDURE AntrianRecallAllServiceUnit
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50),
	@KamarCode    VARCHAR(50) = NULL
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
		@CategoryID    VARCHAR(50),
        @ParamedicID   VARCHAR(50),
		@QueueLocation VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- 1. AMBIL DATA TARGET + LOCK
        -- =========================================
        SELECT 
            @CurrentStatus = vq.Status,
            @QueueDate     = CAST(vq.QueueDate AS DATE),
            @StageID       = vq.StageID,
			@CurrentStage  = vq.CurrentStage,
            @ServiceUnitID = vq.ServiceUnitID,
            @ParamedicID   = vq.ParamedicID,
			@CategoryID    = vq.CategoryID
        FROM VisitQueue vq WITH (UPDLOCK, HOLDLOCK)
        WHERE 
            vq.VisitQueueNo = @VisitQueueNo;

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
        IF @CurrentStatus <> 'CALLED'
        BEGIN
            THROW 50002, 
            'Hanya antrian CALLED yang bisa di-recall', 
            1;
        END

		-- =========================================
		-- SET QUEUE LOCATION (OPTIONAL)
		-- =========================================
		SET @QueueLocation =
			NULLIF(LTRIM(RTRIM(@KamarCode)), '');

        -- =========================================
        -- 2. UPDATE RECALL
        -- =========================================
        UPDATE VisitQueue
        SET 
            CalledTime      = GETDATE(),
            UpdatedBy       = @UserID,
			QueueLocation    = @QueueLocation,
            LastUpdated     = GETDATE(),
            IsManualOverride = 1,
            RecallCount        = ISNULL(RecallCount, 0) + 1
        WHERE 
            VisitQueueNo = @VisitQueueNo
            AND CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @CurrentStage 
			AND StageID = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID, '') = ISNULL(@ParamedicID, '')
			AND ISNULL(CategoryID, '') = ISNULL(@CategoryID, '');

        -- =========================================
        -- 3. RETURN DATA
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
            UpdatedBy,
            RecallCount
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

EXEC AntrianRecallAllServiceUnit
    @VisitQueueNo = 'VQUE-260707-0190',
    @UserID       = 'Admin',
	@KamarCode        = 'Kamar_3'