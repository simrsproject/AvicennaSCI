ALTER TABLE ServiceUnitParamedic
ADD IsDisplayActive BIT NOT NULL
    CONSTRAINT DF_ServiceUnitParamedic_IsDisplayActive
    DEFAULT (0);