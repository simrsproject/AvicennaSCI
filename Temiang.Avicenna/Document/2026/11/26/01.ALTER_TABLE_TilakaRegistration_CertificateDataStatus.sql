IF COL_LENGTH('dbo.TilakaRegistration', 'CertificateDataStatus') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaRegistration]
    ADD [CertificateDataStatus] VARCHAR(20) NULL;
END
GO
