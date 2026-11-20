
CREATE TABLE [dbo].[TilakaQuickSignSigner]
(
    [QuickSignSignerID]    BIGINT IDENTITY(1,1) NOT NULL,
    [QuickSignRequestID]   BIGINT         NOT NULL,
    [Sequence]             INT            NOT NULL,
    [UserIdentifier]       VARCHAR(100)   NOT NULL,
    [Email]                VARCHAR(200)   NULL,
    [Status]               VARCHAR(20)    NULL,
    [NumSignatures]        INT            NULL,
    [NumSignaturesDone]    INT            NULL,
    [AuthUrl]              VARCHAR(1000)  NULL,
    [TokenKey]             VARCHAR(200)   NULL,
    [SignedUrl]            VARCHAR(1000)  NULL,
    [ErrorMessage]         NVARCHAR(1000) NULL,
    [LastCheckedDateTime]  DATETIME       NULL,
    [CreatedDateTime]      DATETIME       NULL,
    [CreatedByUserID]      VARCHAR(40)    NULL,
    [LastUpdateDateTime]   DATETIME       NULL,
    [LastUpdateByUserID]   VARCHAR(40)    NULL,

    CONSTRAINT [PK_TilakaQuickSignSigner]
        PRIMARY KEY CLUSTERED
        (
            [QuickSignSignerID] ASC
        )
);
GO

-- Foreign Key ke TilakaQuickSignRequest
ALTER TABLE [dbo].[TilakaQuickSignSigner]
ADD CONSTRAINT [FK_TilakaQuickSignSigner_Request]
    FOREIGN KEY ([QuickSignRequestID])
    REFERENCES [dbo].[TilakaQuickSignRequest] ([QuickSignRequestID]);
GO

-- Unique Index untuk Sequence per Request
CREATE UNIQUE NONCLUSTERED INDEX [UX_TilakaQuickSignSigner_Sequence]
ON [dbo].[TilakaQuickSignSigner]
(
    [QuickSignRequestID] ASC,
    [Sequence] ASC
);
GO

-- Index UserIdentifier
CREATE NONCLUSTERED INDEX [IX_TilakaQuickSignSigner_UserIdentifier]
ON [dbo].[TilakaQuickSignSigner]
(
    [UserIdentifier] ASC
);
GO
