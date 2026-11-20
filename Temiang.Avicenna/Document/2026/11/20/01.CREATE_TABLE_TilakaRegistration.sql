
CREATE TABLE [dbo].[TilakaRegistration]
(
    [TilakaRegistrationID] BIGINT IDENTITY(1,1) NOT NULL,

    [RelatedType] VARCHAR(30) NOT NULL
        CONSTRAINT [DF_TilakaRegistration_RelatedType] DEFAULT ('Paramedic'),

    [RelatedID] VARCHAR(20) NULL,

    [IsLatest] BIT NOT NULL
        CONSTRAINT [DF_TilakaRegistration_IsLatest] DEFAULT (1),

    [RegistrationState] VARCHAR(30) NULL,

    [RegistrationRequestID] VARCHAR(50) NULL,
    [RequestType] VARCHAR(20) NULL
        CONSTRAINT [DF_TilakaRegistration_RequestType] DEFAULT ('register'),

    [RegistrationDateTime] DATETIME NULL,
    [DateExpire] DATETIME NULL,

    [RegisteredName] NVARCHAR(200) NULL,
    [RegisteredNIK] VARCHAR(50) NULL,
    [RegisteredEmail] VARCHAR(200) NULL,

    [KycStatus] VARCHAR(1) NULL,
    [KycReasonCode] INT NULL,
    [ManualRegistrationStatus] VARCHAR(1) NULL,
    [TilakaName] VARCHAR(100) NULL,

    [CertificateStatus] INT NULL,
    [CertificateStatusInfo] NVARCHAR(500) NULL,
    [CertificateSerialNumber] VARCHAR(100) NULL,
    [CertificateStartDate] DATETIME NULL,
    [CertificateExpiryDate] DATETIME NULL,

    [ConsentVersion] VARCHAR(20) NULL,
    [ConsentTimestamp] DATETIME NULL,
    [ConsentApprovedByUserID] VARCHAR(40) NULL,

    [KtpFilePath] VARCHAR(300) NULL,
    [KtpFileName] VARCHAR(255) NULL,
    [KtpContentType] VARCHAR(50) NULL,
    [KtpUploadedDateTime] DATETIME NULL,

    [LastCheckedDateTime] DATETIME NULL,
    [LastCheckSource] VARCHAR(20) NULL,
    [LastErrorMessage] NVARCHAR(1000) NULL,

    [CreatedDateTime] DATETIME NULL,
    [CreatedByUserID] VARCHAR(40) NULL,
    [LastUpdateDateTime] DATETIME NULL,
    [LastUpdateByUserID] VARCHAR(40) NULL,

    CONSTRAINT [PK_TilakaRegistration]
        PRIMARY KEY CLUSTERED
        (
            [TilakaRegistrationID] ASC
        )
);
GO


CREATE UNIQUE INDEX [UX_TilakaRegistration_RequestID]
    ON [dbo].[TilakaRegistration] ([RegistrationRequestID])
    WHERE [RegistrationRequestID] IS NOT NULL;
GO

CREATE INDEX [IX_TilakaRegistration_Related_IsLatest]
    ON [dbo].[TilakaRegistration] ([RelatedType] ASC, [RelatedID] ASC, [IsLatest] ASC);
GO

CREATE INDEX [IX_TilakaRegistration_State_Expire]
    ON [dbo].[TilakaRegistration] ([RegistrationState] ASC, [DateExpire] ASC);
GO


ALTER TABLE [dbo].[TilakaRegistration]
    ADD [IsQuickSignUser]             BIT           NOT NULL CONSTRAINT [DF_TilakaRegistration_IsQuickSignUser] DEFAULT ((0)),
        [QuickSignRegisteredDateTime] DATETIME      NULL,
        [QuickSignLastErrorMessage]   NVARCHAR(500) NULL;
GO
