SET NOCOUNT ON
GO

INSERT INTO [AppStandardReference] (
    [StandardReferenceID],
    [StandardReferenceName],
    [ItemLength],
    [IsUsedBySystem],
    [IsActive],
    [StandardReferenceGroup],
    [Note],
    [LastUpdateDateTime],
    [LastUpdateByUserID],
    [HasCOA],
    [IsNumericValue]
)
VALUES (
    N'SatuSehatClassType',
    N'SatuSehatClassType',
    999,
    1,
    1,
    NULL,
    '',
    GETDATE(),      
    N'sci',
    0,
    NULL
);
INSERT INTO [AppStandardReferenceItem] (
    [StandardReferenceID],
    [ItemID],
    [ItemName],
    [Note],
    [IsUsedBySystem],
    [IsActive],
    [LastUpdateDateTime],
    [LastUpdateByUserID],
    [ReferenceID],
    [coaID],
    [subledgerID],
    [CustomField],
    [LineNumber],
    [NumericValue],
    [CustomField2]
)
VALUES 
(N'SatuSehatClassType', N'1', N'Kelas 1', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'2', N'Kelas 2', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'3', N'Kelas 3', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'eksekutif', N'Kelas Eksekutif', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'reguler', N'Kelas Reguler', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'vip', N'Kelas VIP', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL),
(N'SatuSehatClassType', N'vvip', N'Kelas VVIP', '', 0, 1, GETDATE(), N'sci', NULL, NULL, NULL, NULL, NULL, NULL, NULL);

GO
SET NOCOUNT OFF
GO

