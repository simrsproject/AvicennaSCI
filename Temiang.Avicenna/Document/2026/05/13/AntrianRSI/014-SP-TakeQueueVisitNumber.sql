CREATE OR ALTER PROCEDURE TakeQueueVisitNumber
(
    @SRAutoNumber NVARCHAR(50),
    @UserID NVARCHAR(50),
    @TransDate DATE,

    @ServiceUnitID VARCHAR(50) = NULL,
    @QueueLocation VARCHAR(50) = NULL,
    @CategoryID VARCHAR(50) = NULL,

    @VisitNo NVARCHAR(50) OUTPUT,
    @VisitQueueNo NVARCHAR(50) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @QueueServiceGroup VARCHAR(50),
        @StageID VARCHAR(50),
        @QueueKey VARCHAR(200),
        @NextSequence INT;

    BEGIN TRAN;

    BEGIN TRY

        /* =========================================
           1. Generate VisitNo
        ========================================= */
        EXEC GenerateVisitAutoNumber
            @SRAutoNumber = @SRAutoNumber,
            @TransDate = @TransDate,
            @ResultNumber = @VisitNo OUTPUT;

        /* =========================================
           2. Generate VisitQueueNo
        ========================================= */
        EXEC GenerateVisitAutoNumber
            @SRAutoNumber = 'VisitQueueNo',
            @TransDate = @TransDate,
            @ResultNumber = @VisitQueueNo OUTPUT;

        /* =========================================
           3. Tentukan Stage & ServiceGroup
        ========================================= */
        IF @ServiceUnitID IS NULL
        BEGIN
            IF @QueueLocation IS NULL
            BEGIN
                THROW 50001, 'QueueLocation wajib untuk registrasi', 1;
            END

            SET @QueueServiceGroup = 'REG';
            SET @StageID = 'LOKET';
        END
        ELSE
        BEGIN
            SELECT @QueueServiceGroup = QueueServiceGroup
            FROM ServiceUnit
            WHERE ServiceUnitID = @ServiceUnitID;

            IF @QueueServiceGroup IS NULL
            BEGIN
                THROW 50002, 'ServiceUnit tidak memiliki QueueServiceGroup', 1;
            END

            SELECT TOP 1 
                @StageID = StageID
            FROM QueueStage
            WHERE ServiceGroup = @QueueServiceGroup
              AND StepOrder = 1
              AND IsActive = 1;

            IF @StageID IS NULL
            BEGIN
                THROW 50003, 'Stage tidak ditemukan', 1;
            END
        END

        /* =========================================
           4. Validasi Category
        ========================================= */
        IF @CategoryID IS NOT NULL
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1
                FROM QueueCategory
                WHERE CategoryID = @CategoryID
                  AND StageID = @StageID
                  AND IsActive = 1
            )
            BEGIN
                THROW 50004, 'Category tidak valid', 1;
            END
        END

        /* =========================================
           5. Generate QueueKey
        ========================================= */
        SET @QueueKey =
            ISNULL(@QueueLocation, @ServiceUnitID)
            + '|'
            + @StageID
            + '|'
            + ISNULL(@CategoryID, '');

        /* =========================================
           6. Hitung QueueSequence
        ========================================= */
        SELECT
            @NextSequence =
                ISNULL(MAX(QueueSequence), 0) + 10
        FROM VisitQueue WITH (UPDLOCK, HOLDLOCK)
        WHERE QueueDate = @TransDate
          AND QueueKey = @QueueKey;

        /* =========================================
           7. Insert
        ========================================= */
        INSERT INTO VisitQueue
        (
            VisitQueueNo,
            VisitNo,
            SRAutoNumber,
            QueueDate,
            Status,
            CurrentStage,
            QueueSequence,
            Priority,
            CreatedBy,
            CreatedDate,
            ServiceUnitID,
            StageID,
            CategoryID,
            QueueLocation,
            QueueKey
        )
        VALUES
        (
            @VisitQueueNo,
            @VisitNo,
            @SRAutoNumber,
            @TransDate,
            'WAITING',
            @StageID,
            @NextSequence,
            100,
            @UserID,
            GETDATE(),
            @ServiceUnitID,
            @StageID,
            @CategoryID,
            @QueueLocation,
            @QueueKey
        );

        /* =========================================
           8. RETURN RESULTSET
        ========================================= */
        SELECT
            @VisitNo AS VisitNo,
            @VisitQueueNo AS VisitQueueNo,
            @StageID AS StageID,
            @QueueKey AS QueueKey,
            @NextSequence AS QueueSequence;

        COMMIT;

    END TRY
    BEGIN CATCH

        ROLLBACK;

        DECLARE @ErrMsg NVARCHAR(MAX);

        SET @ErrMsg = ERROR_MESSAGE();

        RAISERROR(@ErrMsg, 16, 1);

    END CATCH
END