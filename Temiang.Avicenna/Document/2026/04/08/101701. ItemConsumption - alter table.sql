ALTER TABLE dbo.ItemConsumption ADD
	QtyDosage NUMERIC(10,2) NULL,
	SRDosageUnit VARCHAR(20) NULL
GO
	
ALTER TABLE dbo.TransChargesItemConsumption ADD
	QtyDosage NUMERIC(10,2) NULL,
	SRDosageUnit VARCHAR(20) NULL
GO