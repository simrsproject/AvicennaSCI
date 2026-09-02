IF COL_LENGTH('dbo.Registration', 'MppNote') IS NULL
BEGIN
    ALTER TABLE dbo.Registration
    ADD MppNote VARCHAR(MAX) NULL CONSTRAINT DF_Registration_MppNote DEFAULT ('');
END
GO
