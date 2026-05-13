CREATE OR ALTER PROCEDURE AntrianCallNextQueue
(
    @CurrentStage       VARCHAR(50),
    @UserID             VARCHAR(50),
    @CounterID          VARCHAR(50),

    @VisitQueueNo       VARCHAR(50) OUTPUT,
    @VisitNo            VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @QueueDate DATE = '2026-04-20';

    BEGIN TRAN;

    BEGIN TRY

        -- =========================
        -- 1. AUTO FINISH antrian sebelumnya di counter ini
        -- =========================
        UPDATE VisitQueue
        SET 
            Status      = 'FINISHED',
            UpdatedBy   = @UserID,
            LastUpdated = GETDATE()
        WHERE 
			CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @CurrentStage
            AND CalledByCounterID = @CounterID
            AND Status = 'CALLED';

        -- =========================
        -- Reset output
        -- =========================
        SET @VisitQueueNo = NULL;
        SET @VisitNo = NULL;

        -- =========================
        -- 2. Ambil 1 antrian berikutnya + LOCK
        -- =========================
        SELECT TOP 1
            @VisitQueueNo = VisitQueueNo,
            @VisitNo      = VisitNo
        FROM VisitQueue WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE 
            QueueDate = @QueueDate
            AND CurrentStage = @CurrentStage
            AND Status = 'WAITING'
        ORDER BY 
            Priority ASC,
            QueueSequence ASC,
            CreatedDate ASC;

        -- =========================
        -- 3. Jika tidak ada antrian
        -- =========================
        IF @VisitQueueNo IS NULL
        BEGIN
            COMMIT;
            RETURN;
        END

        -- =========================
        -- 4. Update menjadi CALLED
        -- =========================
        UPDATE VisitQueue
        SET 
            Status            = 'CALLED',
            CalledByCounterID = @CounterID,
            CalledTime        = GETDATE(),
            UpdatedBy         = @UserID,
            LastUpdated       = GETDATE()
        WHERE VisitQueueNo = @VisitQueueNo;

        COMMIT;

    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH
END

DECLARE 
    @VisitQueueNo VARCHAR(50),
    @VisitNo VARCHAR(50);

EXEC AntrianCallNextQueue
    @CurrentStage = 'LOKET_PD',
    @UserID = 'admin',
    @CounterID = 'COUNTER_01',
    @VisitQueueNo = @VisitQueueNo OUTPUT,
    @VisitNo = @VisitNo OUTPUT;

SELECT @VisitQueueNo AS VisitQueueNo, @VisitNo AS VisitNo;