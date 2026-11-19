CREATE TABLE [dbo].[AdmedikaDocument]
(
    [AdmedikaDocumentID] BIGINT IDENTITY(1,1) NOT NULL,
    [AdmedikaClaimTransactionID] BIGINT NOT NULL,
    [DocType] VARCHAR(20) NULL,
    [FileName] VARCHAR(500) NULL,
    [CreatedDateTime] DATETIME NULL,
    [CreatedByUserID] VARCHAR(40) NULL,
    [LastUpdateDateTime] DATETIME NULL,
    [LastUpdateByUserID] VARCHAR(40) NULL,

    CONSTRAINT [PK_AdmedikaDocument]
        PRIMARY KEY CLUSTERED
        (
            [AdmedikaDocumentID] ASC
        ),

    CONSTRAINT [FK_AdmedikaDocument_AdmedikaClaimTransaction]
        FOREIGN KEY ([AdmedikaClaimTransactionID])
        REFERENCES [dbo].[AdmedikaClaimTransaction]
        (
            [AdmedikaClaimTransactionID]
        )
);
GO

ALTER TABLE [dbo].[AdmedikaDocument]
ADD
    [IsSentSuccess] BIT NULL,
    [SentDateTime] DATETIME NULL,
    [ResponseMessage] VARCHAR(500) NULL;
GO

ALTER TABLE [dbo].[AdmedikaDocument]
ADD
    [SentByUserID] VARCHAR(40) NULL;
GO