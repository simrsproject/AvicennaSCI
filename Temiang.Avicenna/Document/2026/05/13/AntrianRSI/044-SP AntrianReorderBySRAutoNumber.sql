CREATE OR ALTER PROCEDURE AntrianReorderBySRAutoNumber
(
    @SRAutoNumber  VARCHAR(50),
    @QueueDate     DATE = NULL,
    @UserID        VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    IF @QueueDate IS NULL
        SET @QueueDate = CAST(GETDATE() AS DATE);

    DECLARE @BaseServiceGroup VARCHAR(50);

    BEGIN TRY
        BEGIN TRAN;

        /* =========================================
           1. AMBIL SERVICE GROUP DARI SRAutoNumber
        ========================================= */
        SELECT TOP 1
            @BaseServiceGroup = ServiceGroup
        FROM AntrianAutoNumberSemantic
        WHERE SRAutoNumber = @SRAutoNumber
          AND IsActive = 1;

        IF @BaseServiceGroup IS NULL
        BEGIN
            THROW 50001, 'SRAutoNumber tidak valid', 1;
        END

        /* =========================================
           2. AMBIL DATA QUEUE (LOKET ONLY)
        ========================================= */
        ;WITH Q AS
        (
            SELECT
                VQ.VisitQueueNo,
                VQ.QueueSequence,
                VQ.SRAutoNumber,
                VQ.QueueDate,
                VQ.QueueKey,
                VQ.Status,
                ISNULL(SAS.DisplayOrder, 999) AS DisplayOrder,
                CASE 
                    WHEN VQ.SRAutoNumber = @SRAutoNumber THEN 0
                    ELSE 1
                END AS IsPriority
            FROM VisitQueue VQ WITH (UPDLOCK, HOLDLOCK)
            LEFT JOIN AntrianAutoNumberSemantic SAS
                ON SAS.SRAutoNumber = VQ.SRAutoNumber
            WHERE
                VQ.QueueDate = @QueueDate
                AND VQ.CurrentStage = 'LOKET'
                AND VQ.Status = 'WAITING'
        ),
        R AS
        (
            SELECT *,
                   ROW_NUMBER() OVER
                   (
                       ORDER BY
                           IsPriority ASC,       -- dipilih user dulu
                           DisplayOrder ASC,     -- BPJS/IRM/TUNAI/MITRA order
                           QueueSequence ASC     -- stabilisasi
                   ) AS RN
            FROM Q
        )
        UPDATE VQ
        SET QueueSequence = R.RN * 10,
            IsManualOverride = 1,
            UpdatedBy = @UserID,
            LastUpdated = GETDATE()
        FROM VisitQueue VQ
        INNER JOIN R
            ON VQ.VisitQueueNo = R.VisitQueueNo;

        COMMIT;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK;

        THROW;
    END CATCH
END
GO

DECLARE @QueueDate DATE;

SET @QueueDate = GETDATE();

EXEC AntrianReorderBySRAutoNumber
    @SRAutoNumber = 'VisitTunaiNo',
    @QueueDate    = @QueueDate,
    @UserID       = 'APMPD01';