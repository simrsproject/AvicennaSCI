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
    'DashboardClinicConfigNo',   -- SRAutoNumber
    '2000-01-01',                -- EffectiveDate
    'CFG',                       -- Prefik
    '-',                         -- SeparatorAfterPrefik
    0,                           -- IsUsedDepartment
    '',                          -- SeparatorAfterDept
    1,                           -- IsUsedYear
    2,                           -- YearDigit
    '',                          -- SeparatorAfterYear
    1,                           -- IsUsedMonth
    0,                           -- IsMonthInRomawi
    '',                          -- SeparatorAfterMonth
    1,                           -- IsUsedDay
    '-',                         -- SeparatorAfterDay
    4,                           -- NumberLength
    0,                           -- NumberGroupLength
    '',                          -- NumberGroupSeparator
    '',                          -- NumberFormat
    '',                          -- SeparatorAfterNumber
    1,                           -- IsUsedYearToDateOrder
    GETDATE(),                   -- LastUpdateDateTime
    'WEBSERVICE'                 -- LastUpdateByUserID
);