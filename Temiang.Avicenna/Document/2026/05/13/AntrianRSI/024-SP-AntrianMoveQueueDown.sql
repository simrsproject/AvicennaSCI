CREATE OR ALTER PROCEDURE AntrianMoveQueueDown
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @CurrSeq    INT,
        @NextSeq    INT,
        @NextID     VARCHAR(50),

        @QueueDate  DATE,
        @QueueKey   VARCHAR(500);

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- AMBIL TARGET + LOCK
        -- =========================================
        SELECT 
            @CurrSeq   = QueueSequence,
            @QueueDate = QueueDate,
            @QueueKey  = QueueKey
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE 
            VisitQueueNo = @VisitQueueNo
            AND Status = 'WAITING';

        -- =========================================
        -- VALIDASI
        -- =========================================
        IF @CurrSeq IS NULL
        BEGIN
            THROW 50001,
            'Data antrian tidak ditemukan / bukan WAITING',
            1;
        END

        -- =========================================
        -- CARI NEXT QUEUE
        -- =========================================
        SELECT TOP 1
            @NextSeq = QueueSequence,
            @NextID  = VisitQueueNo
        FROM VisitQueue
        WHERE
            QueueDate = @QueueDate
            AND QueueKey = @QueueKey
            AND Status = 'WAITING'
            AND QueueSequence > @CurrSeq
        ORDER BY QueueSequence ASC;

        -- =========================================
        -- SUDAH PALING BAWAH
        -- =========================================
        IF @NextID IS NULL
        BEGIN
            COMMIT;
            RETURN;
        END

        -- =========================================
        -- SWAP
        -- =========================================
        DECLARE @TmpSeq INT = @CurrSeq;

        UPDATE VisitQueue
        SET QueueSequence = -999999
        WHERE VisitQueueNo = @VisitQueueNo;

        UPDATE VisitQueue
        SET
            QueueSequence    = @TmpSeq,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE VisitQueueNo = @NextID;

        UPDATE VisitQueue
        SET
            QueueSequence    = @NextSeq,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE VisitQueueNo = @VisitQueueNo;

        COMMIT;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH
END

EXEC AntrianMoveQueueDown
    @VisitQueueNo = 'VQUE-260516-0001',
    @UserID = 'admin';