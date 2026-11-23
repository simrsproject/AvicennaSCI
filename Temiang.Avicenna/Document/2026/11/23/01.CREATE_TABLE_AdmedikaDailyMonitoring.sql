/* ============================================================
   TABLE: AdmedikaDailyMonitoring
   SQL Server 2019
   ============================================================ */

CREATE TABLE [dbo].[AdmedikaDailyMonitoring]
(
    [AdmedikaDailyMonitoringID] BIGINT IDENTITY(1,1) NOT NULL,
    [AdmedikaClaimTransactionID] BIGINT NOT NULL,
    [MonitoringDate] DATE NOT NULL,

    [IsSent] BIT NOT NULL
        CONSTRAINT [DF_AdmedikaDailyMonitoring_IsSent] DEFAULT ((0)),
    [SentDateTime] DATETIME NULL,
    [SentByUserID] VARCHAR(40) NULL,

    [IsSuccess] BIT NULL,
    [OkCount] INT NULL,
    [ErCount] INT NULL,
    [ClStatus] VARCHAR(10) NULL,
    [ClDesc] VARCHAR(100) NULL,
    [Message] NVARCHAR(500) NULL,
    [ErrorCode] VARCHAR(10) NULL,
    [ErrorDesc] NVARCHAR(500) NULL,

    [RequestJson] NVARCHAR(MAX) NULL,
    [RawResponse] NVARCHAR(MAX) NULL,

    [CreatedDateTime] DATETIME NOT NULL,
    [CreatedByUserID] VARCHAR(40) NOT NULL,
    [LastUpdateDateTime] DATETIME NULL,
    [LastUpdateByUserID] VARCHAR(40) NULL,

    CONSTRAINT [PK_AdmedikaDailyMonitoring]
        PRIMARY KEY CLUSTERED
        (
            [AdmedikaDailyMonitoringID] ASC
        ),

    CONSTRAINT [UQ_AdmedikaDailyMonitoring_Claim_Date]
        UNIQUE NONCLUSTERED
        (
            [AdmedikaClaimTransactionID] ASC,
            [MonitoringDate] ASC
        ),

    CONSTRAINT [FK_AdmedikaDailyMonitoring_AdmedikaClaimTransaction]
        FOREIGN KEY
        (
            [AdmedikaClaimTransactionID]
        )
        REFERENCES [dbo].[AdmedikaClaimTransaction]
        (
            [AdmedikaClaimTransactionID]
        )
);
GO


/* ============================================================
   TABLE: AdmedikaDailyMonitoringItem
   SQL Server 2019
   ============================================================ */

CREATE TABLE [dbo].[AdmedikaDailyMonitoringItem]
(
    [AdmedikaDailyMonitoringItemID] BIGINT IDENTITY(1,1) NOT NULL,
    [AdmedikaDailyMonitoringID] BIGINT NOT NULL,
    [SeqNo] INT NULL,

    [CategoryCode] INT NOT NULL,
    [FormCode] VARCHAR(5) NULL,
    [Remarks] NVARCHAR(MAX) NULL,
    [EntryDateTime] DATETIME NOT NULL,

    [SourceType] VARCHAR(10) NULL,
    [Status] VARCHAR(100) NULL,

    [IsSent] BIT NOT NULL
        CONSTRAINT [DF_AdmedikaDailyMonitoringItem_IsSent] DEFAULT ((0)),

    [CreatedDateTime] DATETIME NOT NULL,
    [CreatedByUserID] VARCHAR(40) NOT NULL,
    [LastUpdateDateTime] DATETIME NULL,
    [LastUpdateByUserID] VARCHAR(40) NULL,

    CONSTRAINT [PK_AdmedikaDailyMonitoringItem]
        PRIMARY KEY CLUSTERED
        (
            [AdmedikaDailyMonitoringItemID] ASC
        ),

    CONSTRAINT [FK_AdmedikaDailyMonitoringItem_AdmedikaDailyMonitoring]
        FOREIGN KEY
        (
            [AdmedikaDailyMonitoringID]
        )
        REFERENCES [dbo].[AdmedikaDailyMonitoring]
        (
            [AdmedikaDailyMonitoringID]
        )
        ON DELETE CASCADE
);
GO


/* ============================================================
   INDEX: AdmedikaDailyMonitoringItem
   ============================================================ */

CREATE NONCLUSTERED INDEX [IX_AdmedikaDailyMonitoringItem_Header_Seq]
ON [dbo].[AdmedikaDailyMonitoringItem]
(
    [AdmedikaDailyMonitoringID] ASC,
    [SeqNo] ASC
);
GO