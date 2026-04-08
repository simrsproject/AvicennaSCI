/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.PatientImmunization ADD
	BatchNumber varchar(50) NULL,
	ExpirationDate datetime NULL,
	SRImmTiming varchar(20) NULL
GO
ALTER TABLE dbo.PatientImmunization
	DROP COLUMN SatusehatResultID
GO
ALTER TABLE dbo.PatientImmunization SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
