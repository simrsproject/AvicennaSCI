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
    'VisitMitraNo',
    GETDATE(),
    'C',        -- Prefix Tunai
    '',         
    0,
    '',
    0,          -- tidak pakai tahun
    0,
    '',
    0,          -- tidak pakai bulan
    0,
    '',
    0,          -- tidak pakai hari
    '',
    3,          -- format 0001
    0,
    '',
    '',
    '-',        -- hasil: A-0001
    0,
    GETDATE(),
    'wildan_rsi'
);
