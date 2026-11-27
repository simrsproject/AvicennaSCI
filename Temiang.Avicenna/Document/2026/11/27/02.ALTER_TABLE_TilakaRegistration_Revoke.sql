IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeRequestID') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeRequestID] VARCHAR(50) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeReason') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeReason] NVARCHAR(100) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeNote') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeNote] NVARCHAR(500) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeStatus] VARCHAR(20) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeMessage') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeMessage] NVARCHAR(1000) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeRequestedDateTime') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeRequestedDateTime] DATETIME NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeRequestedByUserID') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeRequestedByUserID] VARCHAR(40) NULL;
END
GO

IF COL_LENGTH('dbo.TilakaRegistration', 'RevokeCompletedDateTime') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [RevokeCompletedDateTime] DATETIME NULL;
END
GO
