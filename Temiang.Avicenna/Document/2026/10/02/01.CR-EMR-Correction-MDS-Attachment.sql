/*
    CR EMR Correction, MDS, dan Attachment
    Date: 2026-10-02

    Script ini draft database untuk bagian yang tidak aman jika hanya memakai field existing.
    Review dulu sebelum apply ke production.
*/

/* 1. Aktifkan audit trail untuk Attachment EMR */
IF NOT EXISTS (
    SELECT 1
    FROM dbo.AuditLogSetting
    WHERE TableName = 'PatientDocument'
)
BEGIN
    INSERT INTO dbo.AuditLogSetting
    (
        TableName,
        TableDescription,
        IsAuditLog,
        LastUpdateDateTime,
        LastUpdateByUserID,
        IsConsolidationBranchToHO,
        IsConsolidationHOToBranch,
        ExcludeAuditColumn
    )
    VALUES
    (
        'PatientDocument',
        'EMR Attachment',
        1,
        GETDATE(),
        'system',
        0,
        0,
        'SmallImage'
    );
END
GO

/* 2. Relasi MDS rawat jalan ke SOAP/Assessment sumber */
IF OBJECT_ID('dbo.MedicalDischargeSummarySource', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MedicalDischargeSummarySource
    (
        MedicalDischargeSummarySourceID bigint IDENTITY(1,1) NOT NULL,
        RegistrationNo varchar(50) NOT NULL,
        MDSRegistrationInfoMedicID varchar(50) NOT NULL,
        SourceRegistrationInfoMedicID varchar(50) NOT NULL,
        SourceType varchar(30) NOT NULL,
        IsDeleted bit NOT NULL CONSTRAINT DF_MedicalDischargeSummarySource_IsDeleted DEFAULT (0),
        CreatedDateTime datetime NULL,
        CreatedByUserID varchar(50) NULL,
        LastUpdateDateTime datetime NULL,
        LastUpdateByUserID varchar(50) NULL,
        CONSTRAINT PK_MedicalDischargeSummarySource PRIMARY KEY CLUSTERED (MedicalDischargeSummarySourceID)
    );

    CREATE INDEX IX_MedicalDischargeSummarySource_RegistrationNo
        ON dbo.MedicalDischargeSummarySource (RegistrationNo);

    CREATE INDEX IX_MedicalDischargeSummarySource_SourceRegistrationInfoMedicID
        ON dbo.MedicalDischargeSummarySource (SourceRegistrationInfoMedicID);

    CREATE UNIQUE INDEX UX_MedicalDischargeSummarySource_MDS_Source
        ON dbo.MedicalDischargeSummarySource (MDSRegistrationInfoMedicID, SourceRegistrationInfoMedicID)
        WHERE IsDeleted = 0;
END
GO

/* Pastikan hanya SOAP terakhir yang menjadi sumber aktif untuk setiap MDS */
;WITH ActiveSource AS
(
    SELECT
        MedicalDischargeSummarySourceID,
        ROW_NUMBER() OVER
        (
            PARTITION BY MDSRegistrationInfoMedicID
            ORDER BY MedicalDischargeSummarySourceID DESC
        ) AS RowNo
    FROM dbo.MedicalDischargeSummarySource
    WHERE IsDeleted = 0
)
UPDATE src
SET
    src.IsDeleted = 1,
    src.LastUpdateDateTime = GETDATE(),
    src.LastUpdateByUserID = 'system'
FROM dbo.MedicalDischargeSummarySource src
INNER JOIN ActiveSource activeSource
    ON activeSource.MedicalDischargeSummarySourceID = src.MedicalDischargeSummarySourceID
WHERE activeSource.RowNo > 1;
GO

IF NOT EXISTS
(
    SELECT 1
    FROM sys.indexes
    WHERE object_id = OBJECT_ID('dbo.MedicalDischargeSummarySource')
        AND name = 'UX_MedicalDischargeSummarySource_ActiveMDS'
)
BEGIN
    CREATE UNIQUE INDEX UX_MedicalDischargeSummarySource_ActiveMDS
        ON dbo.MedicalDischargeSummarySource (MDSRegistrationInfoMedicID)
        WHERE IsDeleted = 0;
END
GO
