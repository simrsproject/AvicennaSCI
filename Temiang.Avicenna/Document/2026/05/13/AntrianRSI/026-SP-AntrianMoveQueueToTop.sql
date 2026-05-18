CREATE OR ALTER PROCEDURE AntrianMoveQueueToTop
(
    @VisitQueueNo VARCHAR(50),
    @UserID       VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @CurrSeq    INT,
        @MinSeq     INT,

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
        -- AMBIL SEQUENCE TERKECIL
        -- DALAM QUEUEKEY YANG SAMA
        -- =========================================
        SELECT 
            @MinSeq = MIN(QueueSequence)
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE
            QueueDate = @QueueDate
            AND QueueKey = @QueueKey
            AND Status = 'WAITING';

        -- =========================================
        -- JIKA SUDAH PALING ATAS
        -- =========================================
        IF @MinSeq IS NOT NULL
           AND @CurrSeq <= @MinSeq
        BEGIN
            COMMIT;
            RETURN;
        END

        -- =========================================
        -- MOVE TO TOP
        -- =========================================
        SET @NewSeq = ISNULL(@MinSeq, 10) - 10;

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

EXEC AntrianMoveQueueToTop
    @VisitQueueNo = 'VQUE-260516-0011',
    @UserID = 'admin';