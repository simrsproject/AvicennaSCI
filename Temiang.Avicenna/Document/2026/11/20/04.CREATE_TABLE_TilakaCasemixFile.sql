
CREATE TABLE [dbo].[TilakaCasemixFile]
(
    [CasemixFileID]           BIGINT IDENTITY(1,1) NOT NULL,
    [RegistrationNo]          VARCHAR(40)  NOT NULL,
    [ParamedicID]             VARCHAR(10)  NOT NULL,
    [FileName]                VARCHAR(255) NULL,
    [FilePath]                VARCHAR(500) NULL,
    [DocumentType]            VARCHAR(50)  NULL,
    [CreatedDateTime]         DATETIME     NULL,
    [IsVoid]                  BIT          NOT NULL
        CONSTRAINT [DF_TilakaCasemixFile_IsVoid] DEFAULT ((0)),
    [LastQuickSignRequestID]  BIGINT       NULL,
    [LastProcessedDateTime]   DATETIME     NULL,
    [LastUpdateDateTime]      DATETIME     NULL,
    [LastUpdateByUserID]      VARCHAR(40)  NULL,

    CONSTRAINT [PK_TilakaCasemixFile]
        PRIMARY KEY CLUSTERED
        (
            [CasemixFileID] ASC
        )
);
GO

-- Foreign Key ke Paramedic
ALTER TABLE [dbo].[TilakaCasemixFile]
ADD CONSTRAINT [FK_TilakaCasemixFile_Paramedic]
    FOREIGN KEY ([ParamedicID])
    REFERENCES [dbo].[Paramedic] ([ParamedicID]);
GO

-- Index RegistrationNo
CREATE NONCLUSTERED INDEX [IX_TilakaCasemixFile_RegistrationNo]
ON [dbo].[TilakaCasemixFile] ([RegistrationNo] ASC);
GO

-- Index ParamedicID
CREATE NONCLUSTERED INDEX [IX_TilakaCasemixFile_Paramedic]
ON [dbo].[TilakaCasemixFile] ([ParamedicID] ASC);
GO

-- Index LastQuickSignRequestID
CREATE NONCLUSTERED INDEX [IX_TilakaCasemixFile_LastRequest]
ON [dbo].[TilakaCasemixFile] ([LastQuickSignRequestID] ASC);
GO
