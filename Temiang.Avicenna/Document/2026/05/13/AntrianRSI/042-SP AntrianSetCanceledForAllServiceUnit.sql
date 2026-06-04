CREATE OR ALTER PROCEDURE AntrianSetCanceledAllServiceUnit
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
        @CategoryID    VARCHAR(50);

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
        IF @CurrentStatus NOT IN ('WAITING','PENDING')
        BEGIN
            THROW 50002,
            'Hanya antrian WAITING atau PENDING yang bisa di-CANCELED',
            1;
        END

        -- =========================================
        -- UPDATE → CANCELED
        -- =========================================
        UPDATE VisitQueue
        SET
            Status           = 'CANCELED',
            QueueSequence    = NULL,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE
            VisitQueueNo = @VisitQueueNo
            AND CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @CurrentStage
            AND StageID = @StageID
            AND ServiceUnitID = @ServiceUnitID
            AND ISNULL(ParamedicID,'') = ISNULL(@ParamedicID,'')
            AND ISNULL(CategoryID,'') = ISNULL(@CategoryID,'');

        -- =========================================
        -- RETURN DATA
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
            QueueSequence,
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

