CREATE OR ALTER PROCEDURE AntrianMoveQueueUp
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @CurrSeq    INT,
        @PrevSeq    INT,
        @PrevID     VARCHAR(50),

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
        -- CARI PREVIOUS QUEUE
        -- =========================================
        SELECT TOP 1
            @PrevSeq = QueueSequence,
            @PrevID  = VisitQueueNo
        FROM VisitQueue
        WHERE
            QueueDate = @QueueDate
            AND QueueKey = @QueueKey
            AND Status = 'WAITING'
            AND QueueSequence < @CurrSeq
        ORDER BY QueueSequence DESC;

        -- =========================================
        -- SUDAH PALING ATAS
        -- =========================================
        IF @PrevID IS NULL
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
        WHERE VisitQueueNo = @PrevID;

        UPDATE VisitQueue
        SET
            QueueSequence    = @PrevSeq,
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
GO

EXEC AntrianMoveQueueUp
    @VisitQueueNo = 'VQUE-260516-0017',
    @UserID = 'admin';