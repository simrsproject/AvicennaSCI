INSERT INTO AppAutoNumber
(
    SRAutoNumber,
    EffectiveDate,
    Prefik,
    SeparatorAfterPrefik,
    IsUsedDepartment,
    SeparatorAfterDept,

    IsUsedYear,
    YearDigit,
    SeparatorAfterYear,

    IsUsedMonth,
    IsMonthInRomawi,
    SeparatorAfterMonth,

    IsUsedDay,
    SeparatorAfterDay,

    NumberLength,
    NumberGroupLength,
    NumberGroupSeparator,
    NumberFormat,
    SeparatorAfterNumber,

    IsUsedYearToDateOrder,

    LastUpdateDateTime,
    LastUpdateByUserID
)
VALUES
(
    'VisitQueueNo',
    GETDATE(),

    'VQUE',      -- Prefix
    '-',         -- setelah prefix

    0,
    '',

    1,           -- pakai tahun
    2,           -- 2 digit (26)
    '',

    1,           -- pakai bulan
    0,
    '',

    1,           -- pakai hari
    '-',         -- separator setelah tanggal

    4,           -- 0001
    0,
    '',
    '',
    '',          -- tidak perlu separator tambahan

    1,           -- mengikuti pola Appointment (penting)

    GETDATE(),
    'wildan_rsi'
);
