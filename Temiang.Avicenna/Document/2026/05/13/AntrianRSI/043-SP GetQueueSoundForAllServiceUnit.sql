CREATE OR ALTER PROCEDURE GetQueueSoundForAllServiceUnit
(
    @VisitQueueNo VARCHAR(50),
	@Kamar VARCHAR(20) = NULL
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
	-- Angka Natural (1 - 5000)
	-- ==================================

	DECLARE @Ribu INT, @Ratus INT, @Sisa INT;

	IF @num < 10
	BEGIN
		INSERT INTO @SoundOrder VALUES (CAST(@num AS VARCHAR),0);
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
		INSERT INTO @SoundOrder VALUES (CAST(@num-10 AS VARCHAR),0);
		INSERT INTO @SoundOrder VALUES ('belas',0);
	END
	ELSE IF @num BETWEEN 20 AND 99
	BEGIN
		INSERT INTO @SoundOrder VALUES (CAST(@num/10 AS VARCHAR),0);
		INSERT INTO @SoundOrder VALUES ('puluh',0);

		IF @num%10<>0
			INSERT INTO @SoundOrder VALUES (CAST(@num%10 AS VARCHAR),0);
	END
	ELSE IF @num BETWEEN 100 AND 999
	BEGIN
		SET @Ratus=@num/100;
		SET @Sisa=@num%100;

		IF @Ratus=1
			INSERT INTO @SoundOrder VALUES('seratus',0);
		ELSE
		BEGIN
			INSERT INTO @SoundOrder VALUES(CAST(@Ratus AS VARCHAR),0);
			INSERT INTO @SoundOrder VALUES('ratus',0);
		END

		IF @Sisa>0
		BEGIN
			IF @Sisa<10
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa AS VARCHAR),0);

			ELSE IF @Sisa=10
				INSERT INTO @SoundOrder VALUES('sepuluh',0);

			ELSE IF @Sisa=11
				INSERT INTO @SoundOrder VALUES('sebelas',0);

			ELSE IF @Sisa BETWEEN 12 AND 19
			BEGIN
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa-10 AS VARCHAR),0);
				INSERT INTO @SoundOrder VALUES('belas',0);
			END
			ELSE
			BEGIN
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa/10 AS VARCHAR),0);
				INSERT INTO @SoundOrder VALUES('puluh',0);

				IF @Sisa%10<>0
					INSERT INTO @SoundOrder VALUES(CAST(@Sisa%10 AS VARCHAR),0);
			END
		END
	END
	ELSE IF @num BETWEEN 1000 AND 5000
	BEGIN
		SET @Ribu=@num/1000;
		SET @Sisa=@num%1000;

		-- =========================
		-- Ribuan
		-- =========================
		IF @Ribu=1
			INSERT INTO @SoundOrder VALUES('seribu',0);
		ELSE
		BEGIN
			INSERT INTO @SoundOrder VALUES(CAST(@Ribu AS VARCHAR),0);
			INSERT INTO @SoundOrder VALUES('ribu',0);
		END

		-- =========================
		-- Sisa 1-999
		-- =========================
		IF @Sisa>0
		BEGIN
			IF @Sisa<10
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa AS VARCHAR),0);

			ELSE IF @Sisa=10
				INSERT INTO @SoundOrder VALUES('sepuluh',0);

			ELSE IF @Sisa=11
				INSERT INTO @SoundOrder VALUES('sebelas',0);

			ELSE IF @Sisa BETWEEN 12 AND 19
			BEGIN
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa-10 AS VARCHAR),0);
				INSERT INTO @SoundOrder VALUES('belas',0);
			END
			ELSE IF @Sisa BETWEEN 20 AND 99
			BEGIN
				INSERT INTO @SoundOrder VALUES(CAST(@Sisa/10 AS VARCHAR),0);
				INSERT INTO @SoundOrder VALUES('puluh',0);

				IF @Sisa%10<>0
					INSERT INTO @SoundOrder VALUES(CAST(@Sisa%10 AS VARCHAR),0);
			END
			ELSE
			BEGIN
				SET @Ratus=@Sisa/100;
				DECLARE @Sisa2 INT=@Sisa%100;

				IF @Ratus=1
					INSERT INTO @SoundOrder VALUES('seratus',0);
				ELSE
				BEGIN
					INSERT INTO @SoundOrder VALUES(CAST(@Ratus AS VARCHAR),0);
					INSERT INTO @SoundOrder VALUES('ratus',0);
				END

				IF @Sisa2>0
				BEGIN
					IF @Sisa2<10
						INSERT INTO @SoundOrder VALUES(CAST(@Sisa2 AS VARCHAR),0);

					ELSE IF @Sisa2=10
						INSERT INTO @SoundOrder VALUES('sepuluh',0);

					ELSE IF @Sisa2=11
						INSERT INTO @SoundOrder VALUES('sebelas',0);

					ELSE IF @Sisa2 BETWEEN 12 AND 19
					BEGIN
						INSERT INTO @SoundOrder VALUES(CAST(@Sisa2-10 AS VARCHAR),0);
						INSERT INTO @SoundOrder VALUES('belas',0);
					END
					ELSE
					BEGIN
						INSERT INTO @SoundOrder VALUES(CAST(@Sisa2/10 AS VARCHAR),0);
						INSERT INTO @SoundOrder VALUES('puluh',0);

						IF @Sisa2%10<>0
							INSERT INTO @SoundOrder VALUES(CAST(@Sisa2%10 AS VARCHAR),0);
					END
				END
			END
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
	-- Tambahan Suara Kamar (Optional)
	-- ==================================
	IF TRY_CAST(@Kamar AS INT) IS NOT NULL
	BEGIN
		DECLARE @NoKamar INT = CAST(@Kamar AS INT);

		-- "di kamar"
		INSERT INTO @SoundOrder (SoundCode)
		VALUES ('kamar');

		-- nomor kamar
		IF @NoKamar < 10
		BEGIN
			INSERT INTO @SoundOrder (SoundCode)
			VALUES (CAST(@NoKamar AS VARCHAR));
		END
		ELSE IF @NoKamar = 10
		BEGIN
			INSERT INTO @SoundOrder (SoundCode)
			VALUES ('sepuluh');
		END
	END

   
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
@VisitQueueNo = 'VQUE-260707-0184'