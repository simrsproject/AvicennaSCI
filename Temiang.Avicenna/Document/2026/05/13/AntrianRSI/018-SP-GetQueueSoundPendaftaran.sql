CREATE OR ALTER PROCEDURE GetQueueSoundPendaftaran
(
    @VisitQueueNo VARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE 
        @VisitNo VARCHAR(50),
        @CounterID VARCHAR(10),
        @Prefix VARCHAR(20),
        @Number VARCHAR(20),
        @Status VARCHAR(50);

    -- =========================
    -- 1. Ambil data
    -- =========================
    SELECT 
        @VisitNo   = VisitNo,
        @CounterID = CAST(CalledByCounterID AS VARCHAR),
        @Status    = Status
    FROM VisitQueue
    WHERE VisitQueueNo = @VisitQueueNo;

    -- 🔥 VALIDASI
    IF @VisitNo IS NULL
    BEGIN
        THROW 50001, 'Data antrian tidak ditemukan', 1;
    END

    IF @Status NOT IN ('CALLED', 'RECALL')
    BEGIN
        THROW 50002, 'Antrian belum dipanggil', 1;
    END

    -- =========================
    -- 2. Pecah prefix & number
    -- =========================
    SELECT 
        @Prefix = LEFT(@VisitNo, CHARINDEX('-', @VisitNo) - 1),
        @Number = RIGHT(@VisitNo, LEN(@VisitNo) - CHARINDEX('-', @VisitNo));

    DECLARE @num INT = CAST(@Number AS INT);

    -- =========================
    -- 3. Temp table suara
    -- =========================
    DECLARE @SoundOrder TABLE (
        Seq INT IDENTITY(1,1),
        SoundCode VARCHAR(50)
    );

    -- =========================
    -- 4. Awalan
    -- =========================
    INSERT INTO @SoundOrder VALUES ('nomor-urut');

    -- 🔥 PREFIX (BA → B + A)
    DECLARE @iPrefix INT = 1;
    WHILE @iPrefix <= LEN(@Prefix)
    BEGIN
        INSERT INTO @SoundOrder
        VALUES (LOWER(SUBSTRING(@Prefix, @iPrefix, 1)));

        SET @iPrefix += 1;
    END

   -- =========================
	-- 🔥 ANGKA HYBRID (FIX)
	-- =========================

	DECLARE @NonZeroPos INT = PATINDEX('%[^0]%', @Number);

	-- kalau semua nol
	IF @NonZeroPos = 0
	BEGIN
		SET @NonZeroPos = LEN(@Number);
	END

	-- =========================
	-- 1. LEADING ZERO → DIGIT
	-- =========================
	DECLARE @iDigit INT = 1;

	WHILE @iDigit < @NonZeroPos
	BEGIN
		INSERT INTO @SoundOrder
		VALUES ('0');

		SET @iDigit += 1;
	END

	-- =========================
	-- 2. SISA ANGKA → NATURAL
	-- =========================
	DECLARE @Remaining VARCHAR(20) = SUBSTRING(@Number, @NonZeroPos, LEN(@Number));

	IF @num < 10
	BEGIN
		INSERT INTO @SoundOrder VALUES (CAST(@num AS VARCHAR));
	END
	ELSE IF @num = 10
	BEGIN
		INSERT INTO @SoundOrder VALUES ('sepuluh');
	END
	ELSE IF @num = 11
	BEGIN
		INSERT INTO @SoundOrder VALUES ('sebelas');
	END
	ELSE IF @num BETWEEN 12 AND 19
	BEGIN
		INSERT INTO @SoundOrder VALUES (CAST(@num - 10 AS VARCHAR));
		INSERT INTO @SoundOrder VALUES ('belas');
	END
	ELSE IF @num BETWEEN 20 AND 99
	BEGIN
		INSERT INTO @SoundOrder VALUES (CAST(@num / 10 AS VARCHAR));
		INSERT INTO @SoundOrder VALUES ('puluh');

		IF @num % 10 <> 0
		BEGIN
			INSERT INTO @SoundOrder VALUES (CAST(@num % 10 AS VARCHAR));
		END
	END

    -- =========================
    -- 🔥 KE KONTER
    -- =========================
    INSERT INTO @SoundOrder VALUES ('konter');

    DECLARE @i INT = 1;
    WHILE @i <= LEN(@CounterID)
    BEGIN
        INSERT INTO @SoundOrder
        VALUES (SUBSTRING(@CounterID, @i, 1));

        SET @i += 1;
    END

   
    -- =========================
	-- 5. Join ke sound table
	-- =========================
	SELECT 
		ROW_NUMBER() OVER (ORDER BY s.Seq) AS Seq,
		s.SoundCode,
		q.FilePath,
		@VisitNo AS VisitNo
	FROM @SoundOrder s
	INNER JOIN QueueingSound q
		ON LOWER(q.Name) = LOWER(s.SoundCode)
	WHERE s.SoundCode <> '0'
	  AND ISNULL(q.FilePath, '') <> ''
	ORDER BY s.Seq;

	END

EXEC GetQueueSoundPendaftaran
@VisitQueueNo = 'VQUE-260515-0003'