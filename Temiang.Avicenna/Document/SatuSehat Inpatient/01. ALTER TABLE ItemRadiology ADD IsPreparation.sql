ALTER TABLE ItemRadiology ADD IsPreparation BIT NULL
GO
UPDATE ir SET IsPreparation = 0 FROM ItemRadiology AS ir WHERE ir.IsPreparation IS NULL
GO