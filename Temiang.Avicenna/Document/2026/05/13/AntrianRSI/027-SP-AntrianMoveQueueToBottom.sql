CREATE OR ALTER PROCEDURE AntrianMoveQueueToBottom
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @CurrSeq    INT,
        @MaxSeq     INT,

        @QueueDate  DATE,
        @QueueKey   VARCHAR(500),

        @NewSeq     INT;

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
        -- AMBIL SEQUENCE TERBESAR
        -- DALAM QUEUEKEY YANG SAMA
        -- =========================================
        SELECT 
            @MaxSeq = MAX(QueueSequence)
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE
            QueueDate = @QueueDate
            AND QueueKey = @QueueKey
            AND Status = 'WAITING';

        -- =========================================
        -- JIKA SUDAH PALING BAWAH
        -- =========================================
        IF @MaxSeq IS NOT NULL
           AND @CurrSeq >= @MaxSeq
        BEGIN
            COMMIT;
            RETURN;
        END

        -- =========================================
        -- MOVE TO BOTTOM
        -- =========================================
        SET @NewSeq = ISNULL(@MaxSeq, 0) + 10;

        UPDATE VisitQueue
        SET
            QueueSequence    = @NewSeq,
            IsManualOverride = 1,
            UpdatedBy        = @UserID,
            LastUpdated      = GETDATE()
        WHERE
            VisitQueueNo = @VisitQueueNo;

        COMMIT;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH
END
GO

EXEC AntrianMoveQueueToBottom
    @VisitQueueNo = 'VQUE-260516-0011',
    @UserID = 'admin';