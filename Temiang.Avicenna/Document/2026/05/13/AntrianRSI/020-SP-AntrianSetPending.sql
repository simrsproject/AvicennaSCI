CREATE OR ALTER PROCEDURE AntrianSetPending
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
        @Stage         VARCHAR(50);

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
        -- Validasi hanya WAITING
        -- =========================
        IF @CurrentStatus <> 'WAITING'
        BEGIN
            THROW 50002, 'Hanya antrian WAITING yang bisa di-PENDING', 1;
        END

        -- =========================
        -- Update ke PENDING
        -- =========================
        UPDATE VisitQueue
        SET 
            Status           = 'PENDING',
            QueueSequence    = NULL, -- keluar dari antrian aktif
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

EXEC AntrianSetPending
    @VisitQueueNo = 'VQUE-260421-0010',
    @UserID = 'admin';