
CREATE TABLE [dbo].[TilakaTemplateSignFile]
(
    [TemplateSignFileID]  BIGINT IDENTITY(1,1) NOT NULL,
    [ParamedicID]         VARCHAR(10)   NOT NULL,
    [TemplateNo]          VARCHAR(50)   NOT NULL,
    [FileName]            VARCHAR(255)  NULL,
    [FilePath]            VARCHAR(500)  NULL,
    [ContentType]         VARCHAR(50)   NULL,
    [FileSize]            BIGINT        NULL,
    [IsDefault]           BIT           NOT NULL
        CONSTRAINT [DF_TilakaTemplateSignFile_IsDefault]
        DEFAULT ((0)),
    [IsActive]            BIT           NOT NULL
        CONSTRAINT [DF_TilakaTemplateSignFile_IsActive]
        DEFAULT ((1)),
    [IsRegistered]        BIT           NOT NULL
        CONSTRAINT [DF_TilakaTemplateSignFile_IsRegistered]
        DEFAULT ((0)),
    [RegisteredDateTime]  DATETIME      NULL,
    [LastErrorMessage]    NVARCHAR(500) NULL,
    [CreatedDateTime]     DATETIME      NULL,
    [CreatedByUserID]     VARCHAR(40)   NULL,
    [LastUpdateDateTime]  DATETIME      NULL,
    [LastUpdateByUserID]  VARCHAR(40)   NULL,

    CONSTRAINT [PK_TilakaTemplateSignFile]
        PRIMARY KEY CLUSTERED
        (
            [TemplateSignFileID] ASC
        )
);
GO

-- Foreign Key ke Paramedic
ALTER TABLE [dbo].[TilakaTemplateSignFile]
ADD CONSTRAINT [FK_TilakaTemplateSignFile_Paramedic]
    FOREIGN KEY ([ParamedicID])
    REFERENCES [dbo].[Paramedic] ([ParamedicID]);
GO

-- Unique Index ParamedicID + TemplateNo
CREATE UNIQUE NONCLUSTERED INDEX [UX_TilakaTemplateSignFile_Paramedic_TemplateNo]
ON [dbo].[TilakaTemplateSignFile]
(
    [ParamedicID] ASC,
    [TemplateNo] ASC
);
GO

-- Satu template default per dokter
CREATE UNIQUE NONCLUSTERED INDEX [UX_TilakaTemplateSignFile_Default]
ON [dbo].[TilakaTemplateSignFile]
(
    [ParamedicID] ASC
)
WHERE [IsDefault] = 1;
GO

-- Index Registered dan Active
CREATE NONCLUSTERED INDEX [IX_TilakaTemplateSignFile_Registered]
ON [dbo].[TilakaTemplateSignFile]
(
    [IsRegistered] ASC,
    [IsActive] ASC
);
GO
