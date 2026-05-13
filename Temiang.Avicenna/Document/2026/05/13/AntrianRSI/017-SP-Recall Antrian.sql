CREATE OR ALTER PROCEDURE AntrianRecall
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50),

    @VisitNo      VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @CurrentStatus VARCHAR(50),
        @QueueDate     DATE,
        @Stage         VARCHAR(50);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================
        -- 1. Ambil data + LOCK
        -- =========================
        SELECT 
            @CurrentStatus = Status,
            @QueueDate     = QueueDate,
            @Stage         = CurrentStage,
            @VisitNo       = VisitNo
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE VisitQueueNo = @VisitQueueNo;

        IF @CurrentStatus IS NULL
        BEGIN
            THROW 50001, 'Data antrian tidak ditemukan', 1;
        END

        -- =========================
        -- 2. Validasi harus CALLED
        -- =========================
        IF @CurrentStatus <> 'CALLED'
        BEGIN
            THROW 50002, 'Hanya antrian CALLED yang bisa di-Recall', 1;
        END

        -- =========================
        -- 3. Update Recall
        -- =========================
        UPDATE VisitQueue
        SET 
            CalledTime   = GETDATE(),
            UpdatedBy    = @UserID,
            LastUpdated  = GETDATE(),
            IsManualOverride = 1,
            IsRecall = ISNULL(IsRecall, 0) + 1
        WHERE 
            VisitQueueNo = @VisitQueueNo
            AND QueueDate = @QueueDate
            AND CurrentStage = @Stage;

        COMMIT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END