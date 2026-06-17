CREATE OR ALTER PROCEDURE AntrianCallNextQueue
(
    @QueueLocation       VARCHAR(50),
    @UserID             VARCHAR(50),
    @CounterID          VARCHAR(50),
    @QueueDate          DATE,

    @VisitQueueNo       VARCHAR(50) OUTPUT,
    @VisitNo            VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRAN;

    BEGIN TRY

        -- ==========================================
        -- VALIDASI PASIEN YANG SEDANG DIPANGGIL
        -- ==========================================
        IF EXISTS
        (
            SELECT 1
            FROM VisitQueue VQ
            WHERE
                CAST(VQ.QueueDate AS DATE) = @QueueDate
                AND VQ.CurrentStage = @QueueLocation
                AND VQ.CalledByCounterID = @CounterID
                AND VQ.Status = 'CALLED'
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM VisitQueue VQ2
                    WHERE
                        VQ2.VisitNo = VQ.VisitNo
                        AND CAST(VQ2.QueueDate AS DATE) = CAST(VQ.QueueDate AS DATE)
                        AND ISNULL(LTRIM(RTRIM(VQ2.RegistrationNo)), '') <> ''
                )
        )
        BEGIN
            RAISERROR
            (
                'Registrasikan Pasien yang di Panggil',
                16,
                1
            );

            ROLLBACK;
            RETURN;
        END

        -- ==========================================
        -- AUTO FINISH ANTRIAN SEBELUMNYA
        -- ==========================================
        UPDATE VisitQueue
        SET
            Status      = 'FINISHED',
            UpdatedBy   = @UserID,
            LastUpdated = GETDATE()
        WHERE
            CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @QueueLocation
            AND CalledByCounterID = @CounterID
            AND Status = 'CALLED';

        -- ==========================================
        -- RESET OUTPUT
        -- ==========================================
        SET @VisitQueueNo = NULL;
        SET @VisitNo = NULL;

        -- ==========================================
        -- AMBIL ANTRIAN BERIKUTNYA
        -- ==========================================
        SELECT TOP 1
            @VisitQueueNo = VisitQueueNo,
            @VisitNo      = VisitNo
        FROM VisitQueue WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE
            CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = @QueueLocation
            AND Status = 'WAITING'
        ORDER BY
            Priority ASC,
            QueueSequence ASC,
            CreatedDate ASC;

        -- ==========================================
        -- TIDAK ADA ANTRIAN
        -- ==========================================
        IF @VisitQueueNo IS NULL
        BEGIN
            COMMIT;
            RETURN;
        END

        -- ==========================================
        -- UPDATE MENJADI CALLED
        -- ==========================================
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

        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;

    END CATCH
END

DECLARE 
    @VisitQueueNo VARCHAR(50),
    @VisitNo VARCHAR(50);

EXEC AntrianCallNextQueue
    @CurrentStage = 'LOKET',
    @UserID = '240070',
    @CounterID = '2',
    @VisitQueueNo = @VisitQueueNo OUTPUT,
    @VisitNo = @VisitNo OUTPUT;

SELECT @VisitQueueNo AS VisitQueueNo, @VisitNo AS VisitNo;