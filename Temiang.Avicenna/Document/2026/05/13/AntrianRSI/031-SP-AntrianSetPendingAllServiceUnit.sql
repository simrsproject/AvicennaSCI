CREATE OR ALTER PROCEDURE AntrianSetPendingAllServiceUnit
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
        @ServiceUnitID VARCHAR(50),
        @ParamedicID   VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- 1. AMBIL DATA TARGET + LOCK
        -- =========================================
        SELECT 
            @CurrentStatus = vq.Status,
            @QueueDate     = CAST(vq.QueueDate AS DATE),
            @StageID       = vq.CurrentStage,
            @ServiceUnitID = vq.ServiceUnitID,
            @ParamedicID   = vq.ParamedicID
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
        IF @CurrentStatus <> 'WAITING'
        BEGIN
            THROW 50002,
            'Hanya antrian WAITING yang bisa di-PENDING',
            1;
        END

        -- =========================================
        -- 2. UPDATE → PENDING
        -- =========================================
        UPDATE VisitQueue
        SET 
            Status           = 'PENDING',
            QueueSequence    = NULL,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE 
            VisitQueueNo = @VisitQueueNo
            AND CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID, '') = ISNULL(@ParamedicID, '');

        -- =========================================
        -- 3. RETURN DATA
        -- =========================================
        SELECT
            VisitQueueNo,
            VisitNo,
            Status,
            CurrentStage AS StageID,
            ServiceUnitID,
            ParamedicID,
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

EXEC AntrianSetPendingAllServiceUnit
    @VisitQueueNo = 'VQUE-260516-0013',
    @UserID       = 'admin';