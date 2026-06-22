CREATE OR ALTER PROCEDURE GetQueueSoundForAllServiceUnit
(
    @VisitQueueNo VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @VisitNo VARCHAR(50),
        @Prefix VARCHAR(20),
        @Number VARCHAR(20),
        @Status VARCHAR(50),
        @CurrentStage VARCHAR(50),
        @DestinationSound VARCHAR(50),
        @ServiceUnitID VARCHAR(50),
		@ServiceUnitName VARCHAR(200),
        @ServiceUnitFilePath VARCHAR(200);

    -- ==================================
    -- Ambil Data Queue
    -- ==================================
    SELECT
        @VisitNo = VisitNo,
        @Status = Status,
        @CurrentStage = CurrentStage,
        @ServiceUnitID = ServiceUnitID
    FROM VisitQueue
    WHERE VisitQueueNo = @VisitQueueNo;

    IF @VisitNo IS NULL
    BEGIN
        THROW 50001, 'Data antrian tidak ditemukan', 1;
    END

    IF @Status NOT IN ('CALLED', 'RECALL')
    BEGIN
        THROW 50002, 'Antrian belum dipanggil', 1;
    END

    -- ==================================
    -- Validasi Current Stage
    -- ==================================
    IF ISNULL(@CurrentStage,'') = ''
    BEGIN
        THROW 50003,
              'CurrentStage tidak ditemukan',
              1;
    END

    IF UPPER(@CurrentStage) = 'LOKET'
    BEGIN
        THROW 50004,
              'LOKET tidak menggunakan GetQueueSoundForAllServiceUnit',
              1;
    END

    SET @DestinationSound =
        CASE
            WHEN UPPER(@CurrentStage) = 'POLI'
                THEN 'ke poliklinik'

            WHEN UPPER(@CurrentStage) = 'FARMASI'
                THEN 'ke loket'

            WHEN UPPER(@CurrentStage) IN
            (
                'CT SCAN',
                'ENDOSCOPY',
                'LAB',
                'RADIOLOGI',
                'REHAB',
                'USG'
            )
                THEN 'ke'

            ELSE 'ke'
        END;

    -- ==================================
    -- Ambil File Audio Service Unit
    -- ==================================
    SELECT
		@ServiceUnitName = ServiceUnitName,
        @ServiceUnitFilePath = SoundFilePath
    FROM ServiceUnit
    WHERE ServiceUnitID = @ServiceUnitID;

    IF ISNULL(@ServiceUnitFilePath,'') = ''
    BEGIN
        THROW 50005,
              'SoundFilePath belum disetting pada Service Unit',
              1;
    END

    -- ==================================
    -- Pecah Prefix & Number
    -- ==================================
    SELECT
        @Prefix = LEFT(@VisitNo, CHARINDEX('-', @VisitNo) - 1),
        @Number = RIGHT(@VisitNo, LEN(@VisitNo) - CHARINDEX('-', @VisitNo));

    DECLARE @num INT = CAST(@Number AS INT);

    -- ==================================
    -- Temp Sound Order
    -- ==================================
    DECLARE @SoundOrder TABLE
    (
        Seq INT IDENTITY(1,1),
        SoundCode VARCHAR(100),
        IsDirectFile BIT DEFAULT(0)
    );

    -- ==================================
    -- Nomor Urut
    -- ==================================
    INSERT INTO @SoundOrder (SoundCode)
    VALUES ('nomor-urut');

    DECLARE @iPrefix INT = 1;

    WHILE @iPrefix <= LEN(@Prefix)
    BEGIN
        INSERT INTO @SoundOrder (SoundCode)
        VALUES (LOWER(SUBSTRING(@Prefix,@iPrefix,1)));

        SET @iPrefix += 1;
    END

    -- ==================================
    -- Leading Zero
    -- ==================================
    DECLARE @NonZeroPos INT =
        PATINDEX('%[^0]%', @Number);

    IF @NonZeroPos = 0
        SET @NonZeroPos = LEN(@Number);

    DECLARE @iDigit INT = 1;

    WHILE @iDigit < @NonZeroPos
    BEGIN
        INSERT INTO @SoundOrder (SoundCode)
        VALUES ('0');

        SET @iDigit += 1;
    END

    -- ==================================
    -- Angka Natural
    -- ==================================
    IF @num < 10
    BEGIN
        INSERT INTO @SoundOrder
        VALUES (CAST(@num AS VARCHAR),0);
    END
    ELSE IF @num = 10
    BEGIN
        INSERT INTO @SoundOrder VALUES ('sepuluh',0);
    END
    ELSE IF @num = 11
    BEGIN
        INSERT INTO @SoundOrder VALUES ('sebelas',0);
    END
    ELSE IF @num BETWEEN 12 AND 19
    BEGIN
        INSERT INTO @SoundOrder VALUES (CAST(@num - 10 AS VARCHAR),0);
        INSERT INTO @SoundOrder VALUES ('belas',0);
    END
    ELSE IF @num BETWEEN 20 AND 99
    BEGIN
        INSERT INTO @SoundOrder VALUES (CAST(@num / 10 AS VARCHAR),0);
        INSERT INTO @SoundOrder VALUES ('puluh',0);

        IF @num % 10 <> 0
        BEGIN
            INSERT INTO @SoundOrder VALUES (CAST(@num % 10 AS VARCHAR),0);
        END
    END

    -- ==================================
    -- Ke / Ke Loket / Ke Poliklinik
    -- ==================================
    INSERT INTO @SoundOrder (SoundCode)
    VALUES (@DestinationSound);

    -- ==================================
    -- Nama Poli / Unit
    -- ==================================
    INSERT INTO @SoundOrder
    (
        SoundCode,
        IsDirectFile
    )
    VALUES
    (
        @ServiceUnitFilePath,
        1
    );

   
    -- ==================================
	-- Result
	-- ==================================
	SELECT
		ROW_NUMBER() OVER (ORDER BY s.Seq) AS Seq,
		s.SoundCode,
		CASE
			WHEN s.IsDirectFile = 1
				THEN s.SoundCode
			ELSE q.FilePath
		END AS FilePath,
		@VisitNo AS VisitNo,
		@CurrentStage AS CurrentStage,
		@ServiceUnitID AS ServiceUnitID,
		@ServiceUnitName AS ServiceUnitName
	FROM @SoundOrder s
	LEFT JOIN QueueingSound q
		ON LOWER(q.Name) = LOWER(s.SoundCode)
	WHERE
	(
		s.IsDirectFile = 1
		OR ISNULL(q.FilePath,'') <> ''
	)
	AND s.SoundCode <> '0'
	ORDER BY s.Seq;

	END
GO

Exec GetQueueSoundForAllServiceUnit
@VisitQueueNo = 'VQUE-260525-0002'