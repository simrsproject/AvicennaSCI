CREATE OR ALTER PROCEDURE AntrianCallNow
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50),
    @CounterID    VARCHAR(50),

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
        -- 1. Ambil data target + LOCK
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
        -- 2. Validasi hanya WAITING
        -- =========================
        IF @CurrentStatus <> 'WAITING'
        BEGIN
            THROW 50002, 'Hanya antrian WAITING yang bisa di-Call Now', 1;
        END

		-- =========================
		-- 3. AUTO TURUNKAN CALLED → PENDING
		-- =========================
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
			AND CurrentStage = @Stage
			AND CalledByCounterID = @CounterID
			AND UpdatedBy = @UserID;

		-- =========================
		-- 4. Update target menjadi CALLED
		-- =========================
		UPDATE VisitQueue
		SET 
			Status            = 'CALLED',
			CalledByCounterID = @CounterID,
			CalledTime        = GETDATE(),
			IsManualOverride  = 1,
			UpdatedBy         = @UserID,
			LastUpdated       = GETDATE()
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

DECLARE @VisitNo VARCHAR(50);

EXEC AntrianCallNow
    @VisitQueueNo = 'VQUE-260421-0020',
    @UserID = 'admin',
    @CounterID = 'COUNTER_01',
    @VisitNo = @VisitNo OUTPUT;

SELECT @VisitNo;