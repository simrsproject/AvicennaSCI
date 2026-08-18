
INSERT INTO AppParameter
(
    ParameterID,
    ParameterName,
    ParameterValue,
    ParameterType,
    LastUpdateDateTime,
    LastUpdateByUserID,
    IsUsedBySystem,
    Message
)
VALUES
(
    'IsAllowZeroNewPriceForTariff',
    'Is Allow Zero New Price For Tariff? (Yes/No)',
    'No',
    '',
    GETDATE(),
    'sci',
    1,
    NULL
);