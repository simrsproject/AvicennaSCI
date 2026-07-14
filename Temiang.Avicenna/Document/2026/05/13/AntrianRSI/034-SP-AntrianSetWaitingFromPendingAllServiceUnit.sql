CREATE OR ALTER PROCEDURE AntrianSetWaitingFromPendingAllServiceUnit
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
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
        @ParamedicID   VARCHAR(50),
        @MaxSeq        INT,
        @NewSeq        INT,
		@CategoryID    VARCHAR(50);

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
        IF @CurrentStatus <> 'PENDING'
        BEGIN
            THROW 50002,
            'Hanya antrian PENDING yang bisa dikembalikan ke WAITING',
            1;
        END

        -- =========================================
        -- 2. AMBIL MAX SEQUENCE PER GROUP
        -- =========================================
        SELECT
            @MaxSeq = MAX(QueueSequence)
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE
            CAST(QueueDate AS DATE) = @QueueDate
            AND StageID = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID, '') = ISNULL(@ParamedicID, '')
			AND ISNULL(CategoryID, '') = ISNULL(@CategoryID, '')
            AND Status = 'WAITING';

        SET @NewSeq = ISNULL(@MaxSeq, 0) + 10;

        -- =========================================
        -- 3. UPDATE → WAITING
        -- =========================================
        UPDATE VisitQueue
        SET
            Status           = 'WAITING',
            QueueSequence    = @NewSeq,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE
            VisitQueueNo = @VisitQueueNo
            AND CAST(QueueDate AS DATE) = @QueueDate
            AND StageID = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID, '') = ISNULL(@ParamedicID, '')
			AND ISNULL(CategoryID, '') = ISNULL(@CategoryID, '');

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
            QueueSequence,
            ServiceUnitID,
            ParamedicID,
			CalledTime,
            IsManualOverride,
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