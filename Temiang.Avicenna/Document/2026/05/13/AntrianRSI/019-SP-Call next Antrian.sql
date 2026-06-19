CREATE OR ALTER PROCEDURE AntrianCallNextQueue
(
    @QueueLocation   VARCHAR(50),
    @UserID          VARCHAR(50),
    @CounterID       VARCHAR(50),
    @QueueDate       DATE,

    @VisitQueueNo    VARCHAR(50) OUTPUT,
    @VisitNo         VARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRAN;

    BEGIN TRY

        ---------------------------------------------------
        -- 1. AMBIL PASIEN YANG SEDANG CALLED
        ---------------------------------------------------
        DECLARE @CurrentVisitNo VARCHAR(50);

        SELECT TOP 1
            @VisitQueueNo = VisitQueueNo,
            @CurrentVisitNo = VisitNo
        FROM VisitQueue WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE
            CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = 'LOKET'
            AND QueueLocation = @QueueLocation
            AND CalledByCounterID = @CounterID
            AND Status = 'CALLED'
        ORDER BY CalledTime DESC;


        ---------------------------------------------------
        -- 2. VALIDASI: HARUS SUDAH ADA ROW REGISTRASI
        ---------------------------------------------------
        IF @CurrentVisitNo IS NOT NULL
        AND NOT EXISTS
        (
            SELECT 1
            FROM VisitQueue REG
            WHERE
                REG.VisitNo = @CurrentVisitNo
                AND REG.QueueDate = @QueueDate
                AND ISNULL(REG.RegistrationNo, '') <> ''
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


        ---------------------------------------------------
        -- 3. FINISH CURRENT CALLED
        ---------------------------------------------------
        UPDATE VisitQueue
        SET
            Status = 'FINISHED',
            FinishedTime = GETDATE(),
            UpdatedBy = @UserID,
            LastUpdated = GETDATE()
        WHERE
            VisitQueueNo = @VisitQueueNo
            AND Status = 'CALLED';


        ---------------------------------------------------
        -- 4. RESET OUTPUT
        ---------------------------------------------------
        SET @VisitQueueNo = NULL;
        SET @VisitNo = NULL;


        ---------------------------------------------------
        -- 5. AMBIL ANTRIAN BERIKUTNYA
        ---------------------------------------------------
        SELECT TOP 1
            @VisitQueueNo = VisitQueueNo,
            @VisitNo      = VisitNo
        FROM VisitQueue WITH (ROWLOCK, READPAST, UPDLOCK)
        WHERE
            CAST(QueueDate AS DATE) = @QueueDate
            AND CurrentStage = 'LOKET'
            AND QueueLocation = @QueueLocation
            AND Status = 'WAITING'
        ORDER BY
            Priority ASC,
            QueueSequence ASC,
            CreatedDate ASC;


        ---------------------------------------------------
        -- 6. JIKA TIDAK ADA ANTRIAN
        ---------------------------------------------------
        IF @VisitQueueNo IS NULL
        BEGIN
            COMMIT;
            RETURN;
        END


        ---------------------------------------------------
        -- 7. SET MENJADI CALLED
        ---------------------------------------------------
        UPDATE VisitQueue
        SET
            Status = 'CALLED',
            CalledByCounterID = @CounterID,
            CalledTime = GETDATE(),
            UpdatedBy = @UserID,
            LastUpdated = GETDATE()
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

DECLARE 
    @VisitQueueNo VARCHAR(50),
    @VisitNo VARCHAR(50);

EXEC AntrianCallNextQueue
    @QueueLocation = 'LOKET_PD',
    @UserID = '240070',
    @CounterID = '1',
	@QueueDate = '2026-06-19',
    @VisitQueueNo = @VisitQueueNo OUTPUT,
    @VisitNo = @VisitNo OUTPUT;

SELECT @VisitQueueNo AS VisitQueueNo, @VisitNo AS VisitNo;