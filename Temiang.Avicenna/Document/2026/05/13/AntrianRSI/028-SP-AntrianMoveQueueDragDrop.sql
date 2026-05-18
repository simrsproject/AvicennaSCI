CREATE OR ALTER PROCEDURE AntrianMoveQueueDragDrop
(
    @VisitQueueNo        VARCHAR(50), -- item yang dipindah
    @TargetVisitQueueNo  VARCHAR(50), -- anchor (drop target)
    @Position            VARCHAR(10), -- BEFORE / AFTER
    @UserID              VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @QueueDate DATE,
        @QueueKey  VARCHAR(500),

        @CurrSeq   INT,
        @TargetSeq INT,

        @PrevSeq   INT,
        @NextSeq   INT,

        @NewSeq    INT;

    BEGIN TRAN;

    BEGIN TRY

        -- =========================================
        -- AMBIL SOURCE + LOCK
        -- =========================================
        SELECT 
            @CurrSeq   = QueueSequence,
            @QueueDate = QueueDate,
            @QueueKey  = QueueKey
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE 
            VisitQueueNo = @VisitQueueNo
            AND Status = 'WAITING';

        IF @CurrSeq IS NULL
        BEGIN
            THROW 50001,
            'Source tidak ditemukan / bukan WAITING',
            1;
        END

        -- =========================================
        -- AMBIL TARGET
        -- HARUS DALAM QueueKey YANG SAMA
        -- =========================================
        SELECT 
            @TargetSeq = QueueSequence
        FROM VisitQueue
        WHERE
            VisitQueueNo = @TargetVisitQueueNo
            AND QueueDate = @QueueDate
            AND QueueKey = @QueueKey
            AND Status = 'WAITING';

        IF @TargetSeq IS NULL
        BEGIN
            THROW 50002,
            'Target tidak valid (beda QueueKey/tanggal/status)',
            1;
        END

        -- =========================================
        -- VALIDASI POSITION
        -- =========================================
        SET @Position = UPPER(@Position);

        IF @Position NOT IN ('BEFORE', 'AFTER')
        BEGIN
            THROW 50003,
            'Position harus BEFORE atau AFTER',
            1;
        END

        -- =========================================
        -- BEFORE
        -- =========================================
        IF @Position = 'BEFORE'
        BEGIN

            SELECT TOP 1
                @PrevSeq = QueueSequence
            FROM VisitQueue
            WHERE
                QueueDate = @QueueDate
                AND QueueKey = @QueueKey
                AND Status = 'WAITING'
                AND VisitQueueNo <> @VisitQueueNo
                AND QueueSequence < @TargetSeq
            ORDER BY QueueSequence DESC;

            SET @NextSeq = @TargetSeq;

        END

        -- =========================================
        -- AFTER
        -- =========================================
        IF @Position = 'AFTER'
        BEGIN

            SELECT TOP 1
                @NextSeq = QueueSequence
            FROM VisitQueue
            WHERE
                QueueDate = @QueueDate
                AND QueueKey = @QueueKey
                AND Status = 'WAITING'
                AND VisitQueueNo <> @VisitQueueNo
                AND QueueSequence > @TargetSeq
            ORDER BY QueueSequence ASC;

            SET @PrevSeq = @TargetSeq;

        END

        -- =========================================
        -- HITUNG NEW SEQUENCE
        -- =========================================
        IF @PrevSeq IS NULL
        BEGIN
            -- PALING ATAS
            SET @NewSeq = @NextSeq - 10;
        END
        ELSE IF @NextSeq IS NULL
        BEGIN
            -- PALING BAWAH
            SET @NewSeq = @PrevSeq + 10;
        END
        ELSE
        BEGIN
            -- DI TENGAH
            SET @NewSeq = (@PrevSeq + @NextSeq) / 2;
        END

        -- =========================================
        -- GAP HABIS → RESEQUENCE
        -- =========================================
        IF @NewSeq = @PrevSeq
           OR @NewSeq = @NextSeq
        BEGIN

            ;WITH CTE AS
            (
                SELECT
                    VisitQueueNo,
                    ROW_NUMBER() OVER
                    (
                        ORDER BY QueueSequence
                    ) * 10 AS NewSeq
                FROM VisitQueue
                WHERE
                    QueueDate = @QueueDate
                    AND QueueKey = @QueueKey
                    AND Status = 'WAITING'
            )
            UPDATE VQ
            SET QueueSequence = CTE.NewSeq
            FROM VisitQueue VQ
            INNER JOIN CTE
                ON VQ.VisitQueueNo = CTE.VisitQueueNo;

            -- =====================================
            -- RELOAD TARGET SEQ
            -- =====================================
            SELECT
                @TargetSeq = QueueSequence
            FROM VisitQueue
            WHERE VisitQueueNo = @TargetVisitQueueNo;

            IF @Position = 'BEFORE'
                SET @NewSeq = @TargetSeq - 5;
            ELSE
                SET @NewSeq = @TargetSeq + 5;

        END

        -- =========================================
        -- UPDATE SOURCE
        -- =========================================
        UPDATE VisitQueue
        SET
            QueueSequence    = @NewSeq,
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

EXEC AntrianMoveQueueDragDrop
    @VisitQueueNo = 'VQUE-260516-0014',
    @TargetVisitQueueNo = 'VQUE-260516-0016',
    @Position = 'AFTER',
    @UserID = 'admin';