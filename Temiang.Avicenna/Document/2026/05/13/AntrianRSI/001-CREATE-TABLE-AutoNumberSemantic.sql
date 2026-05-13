CREATE TABLE dbo.AntrianAutoNumberSemantic
(
    AntrianAutoNumberSemanticNo INT IDENTITY(1,1) PRIMARY KEY,

    SRAutoNumber VARCHAR(100) NOT NULL,
    PayerType VARCHAR(50) NOT NULL,
    ServiceGroup VARCHAR(50) NOT NULL,

    Channel VARCHAR(50) NOT NULL, -- LOKET_PD / LOKET_PM / FARMASI

    DisplayOrder INT NULL,
    DisplayName VARCHAR(100) NULL,

    IsActive BIT NOT NULL DEFAULT(1)
);

