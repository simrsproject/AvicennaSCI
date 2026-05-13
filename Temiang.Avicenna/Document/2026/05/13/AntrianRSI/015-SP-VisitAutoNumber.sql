CREATE OR ALTER PROCEDURE [dbo].[GenerateVisitAutoNumber]
(
    @SRAutoNumber NVARCHAR(50),
    @TransDate    DATE,
    @ResultNumber NVARCHAR(100) OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @Prefix NVARCHAR(50),
        @SepAfterPrefix NVARCHAR(5),
        @SepAfterMonth NVARCHAR(5),
        @SepAfterYear NVARCHAR(5),
        @IsUsedYear BIT,
        @YearDigit INT,
        @IsUsedMonth BIT,
        @IsMonthRoman BIT,
        @NumberLength INT,
        @LastNumber INT,
        @NextNumber INT,
        @YearNo INT,
        @MonthNo INT,
        @DayNo INT;

    BEGIN TRAN;

    /* 1️⃣ Ambil konfigurasi */
    SELECT TOP 1
        @Prefix         = Prefik,
        @SepAfterPrefix = SeparatorAfterPrefik,
        @IsUsedYear     = IsUsedYear,
        @YearDigit      = YearDigit,
        @SepAfterYear   = SeparatorAfterYear,
        @IsUsedMonth    = IsUsedMonth,
        @IsMonthRoman   = IsMonthInRomawi,
        @SepAfterMonth  = SeparatorAfterMonth,
        @NumberLength   = NumberLength
    FROM AppAutoNumber WITH (NOLOCK)
    WHERE SRAutoNumber = @SRAutoNumber
      AND @TransDate >= EffectiveDate
    ORDER BY EffectiveDate DESC;

    IF @NumberLength IS NULL
    BEGIN
        ROLLBACK;
        RAISERROR('Auto number configuration not found.', 16, 1);
        RETURN;
    END

    SET @YearNo  = YEAR(@TransDate);
    SET @MonthNo = MONTH(@TransDate);
    SET @DayNo   = DAY(@TransDate);

    /* 2️⃣ Ambil & lock last number */
    SELECT @LastNumber = LastNumber
    FROM AppAutoNumberLast WITH (UPDLOCK, HOLDLOCK)
    WHERE SRAutoNumber = @SRAutoNumber
      AND YearNo = @YearNo
      AND MonthNo = @MonthNo
      AND DayNo = @DayNo;

    IF @LastNumber IS NULL
    BEGIN
        SET @NextNumber = 1;

        INSERT INTO AppAutoNumberLast
        (
            SRAutoNumber,
            EffectiveDate,
            YearNo,
            MonthNo,
            DayNo,
            LastNumber,
            LastCompleteNumber,
            LastUpdateDateTime,
            LastUpdateByUserID
        )
        VALUES
        (
            @SRAutoNumber,
            GETDATE(),
            @YearNo,
            @MonthNo,
            @DayNo,
            @NextNumber,
            '',
            GETDATE(),
            'sci'
        );
    END
    ELSE
    BEGIN
        SET @NextNumber = @LastNumber + 1;
    END

    /* 3️⃣ Susun nomor */

    IF @SRAutoNumber = 'VisitQueueNo'
    BEGIN
        -- 🔥 FORMAT KHUSUS QUEUE
        -- VQUE-260420-0001
        SET @ResultNumber =
            ISNULL(@Prefix, '')
            + ISNULL(NULLIF(@SepAfterPrefix, ''), '-')
            + RIGHT(CAST(@YearNo AS NVARCHAR), 2)
            + RIGHT('0' + CAST(@MonthNo AS NVARCHAR), 2)
            + RIGHT('0' + CAST(@DayNo AS NVARCHAR), 2)
            + '-'
            + RIGHT(REPLICATE('0', @NumberLength) + CAST(@NextNumber AS NVARCHAR), @NumberLength);
    END
    ELSE
    BEGIN
        -- 🔹 FORMAT LAMA (TIDAK DIUBAH)
        SET @ResultNumber =
            ISNULL(@Prefix, '')
            + ISNULL(NULLIF(@SepAfterPrefix, ''), '-')
            + RIGHT(REPLICATE('0', @NumberLength) + CAST(@NextNumber AS NVARCHAR), @NumberLength);

        /* Tambahan Month */
        IF @IsUsedMonth = 1
        BEGIN
            SET @ResultNumber = @ResultNumber 
                + ISNULL(NULLIF(@SepAfterMonth, ''), '-')
                + CASE 
                    WHEN @IsMonthRoman = 1 THEN
                        CASE @MonthNo
                            WHEN 1 THEN 'I' WHEN 2 THEN 'II' WHEN 3 THEN 'III'
                            WHEN 4 THEN 'IV' WHEN 5 THEN 'V' WHEN 6 THEN 'VI'
                            WHEN 7 THEN 'VII' WHEN 8 THEN 'VIII' WHEN 9 THEN 'IX'
                            WHEN 10 THEN 'X' WHEN 11 THEN 'XI' WHEN 12 THEN 'XII'
                        END
                    ELSE
                        RIGHT('0' + CAST(@MonthNo AS NVARCHAR), 2)
                END;
        END

        /* Tambahan Year */
        IF @IsUsedYear = 1
        BEGIN
            SET @ResultNumber = @ResultNumber
                + ISNULL(NULLIF(@SepAfterYear, ''), '-')
                + RIGHT(CAST(@YearNo AS NVARCHAR), @YearDigit);
        END
    END

    /* 4️⃣ Update last number */
    UPDATE AppAutoNumberLast
    SET LastNumber = @NextNumber,
        LastCompleteNumber = @ResultNumber,
        LastUpdateDateTime = GETDATE(),
        LastUpdateByUserID = 'sci'
    WHERE SRAutoNumber = @SRAutoNumber
      AND YearNo = @YearNo
      AND MonthNo = @MonthNo
      AND DayNo = @DayNo;

    COMMIT;
END