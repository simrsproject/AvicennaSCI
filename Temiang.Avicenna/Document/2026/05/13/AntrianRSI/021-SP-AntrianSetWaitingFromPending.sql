CREATE OR ALTER PROCEDURE AntrianSetWaitingFromPending
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
        @Stage         VARCHAR(50),
        @MaxSeq        INT,
        @NewSeq        INT;

    BEGIN TRAN;

    BEGIN TRY
        -- =========================
        -- Ambil data + LOCK
        -- =========================
        SELECT 
            @CurrentStatus = Status,
            @QueueDate     = QueueDate,
            @Stage         = CurrentStage
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE VisitQueueNo = @VisitQueueNo;

        IF @CurrentStatus IS NULL
        BEGIN
            THROW 50001, 'Data antrian tidak ditemukan', 1;
        END

        -- =========================
        -- Validasi hanya PENDING
        -- =========================
        IF @CurrentStatus <> 'PENDING'
        BEGIN
            THROW 50002, 'Hanya antrian PENDING yang bisa dikembalikan ke WAITING', 1;
        END

        -- =========================
        -- Ambil urutan terakhir
        -- =========================
        SELECT 
            @MaxSeq = MAX(QueueSequence)
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE 
            QueueDate = @QueueDate
            AND CurrentStage = @Stage
            AND Status = 'WAITING';

        SET @NewSeq = ISNULL(@MaxSeq, 0) + 10;

        -- =========================
        -- Update kembali ke WAITING
        -- =========================
        UPDATE VisitQueue
        SET 
            Status           = 'WAITING',
            QueueSequence    = @NewSeq,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
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

EXEC AntrianSetWaitingFromPending
    @VisitQueueNo = 'VQUE-260421-0010',
    @UserID = 'admin';