IF COL_LENGTH('dbo.TilakaCasemixFile', 'IsDeleted') IS NULL
BEGIN
    ALTER TABLE [dbo].[TilakaCasemixFile]
    ADD [IsDeleted] BIT NOT NULL
        CONSTRAINT [DF_TilakaCasemixFile_IsDeleted] DEFAULT ((0));
END
GO
