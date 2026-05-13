CREATE UNIQUE INDEX UX_AntrianAutoNumberSemantic_Composite
ON dbo.AntrianAutoNumberSemantic
(
    SRAutoNumber,
    Channel,
    PayerType,
    ServiceGroup
);