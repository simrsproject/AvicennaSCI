
CREATE TABLE [dbo].[TilakaQuickSignRequest]
(
    [QuickSignRequestID]  BIGINT IDENTITY(1,1) NOT NULL,
    [RequestID]           VARCHAR(50)    NOT NULL,
    [CasemixFileID]       BIGINT         NOT NULL,
    [FileName]            VARCHAR(500)   NULL,
    [TemplateNo]          VARCHAR(50)    NULL,
    [Status]              VARCHAR(20)    NOT NULL
        CONSTRAINT [DF_TilakaQuickSignRequest_Status]
        DEFAULT ('Requested'),
    [IsActive]            BIT            NOT NULL
        CONSTRAINT [DF_TilakaQuickSignRequest_IsActive]
        DEFAULT ((1)),
    [RequestDateTime]     DATETIME       NULL,
    [CompletedDateTime]   DATETIME       NULL,
    [SignedFilePath]      VARCHAR(500)   NULL,
    [ErrorMessage]        NVARCHAR(1000) NULL,
    [CreatedDateTime]     DATETIME       NULL,
    [CreatedByUserID]     VARCHAR(40)    NULL,
    [LastUpdateDateTime]  DATETIME       NULL,
    [LastUpdateByUserID]  VARCHAR(40)    NULL,

    CONSTRAINT [PK_TilakaQuickSignRequest]
        PRIMARY KEY CLUSTERED
        (
            [QuickSignRequestID] ASC
        )
);
GO

-- Foreign Key ke TilakaCasemixFile
ALTER TABLE [dbo].[TilakaQuickSignRequest]
ADD CONSTRAINT [FK_TilakaQuickSignRequest_CasemixFile]
    FOREIGN KEY ([CasemixFileID])
    REFERENCES [dbo].[TilakaCasemixFile] ([CasemixFileID]);
GO

-- Unique Index RequestID
CREATE UNIQUE NONCLUSTERED INDEX [UX_TilakaQuickSignRequest_RequestID]
ON [dbo].[TilakaQuickSignRequest]
(
    [RequestID] ASC
);
GO

-- Satu CasemixFile hanya boleh memiliki satu request aktif
CREATE UNIQUE NONCLUSTERED INDEX [UX_TilakaQuickSignRequest_ActiveCasemix]
ON [dbo].[TilakaQuickSignRequest]
(
    [CasemixFileID] ASC
)
WHERE [IsActive] = 1;
GO

-- Index Status dan RequestDateTime
CREATE NONCLUSTERED INDEX [IX_TilakaQuickSignRequest_Status]
ON [dbo].[TilakaQuickSignRequest]
(
    [Status] ASC,
    [RequestDateTime] ASC
);
GO

-- Index CasemixFileID
CREATE NONCLUSTERED INDEX [IX_TilakaQuickSignRequest_CasemixFile]
ON [dbo].[TilakaQuickSignRequest]
(
    [CasemixFileID] ASC
);
GO
