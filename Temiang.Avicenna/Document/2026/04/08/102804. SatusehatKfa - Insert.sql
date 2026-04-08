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
CREATE TABLE dbo.Tmp_SatuSehatKfa
	(
	id bigint NOT NULL,
	ss_uuid varchar(20) NOT NULL,
	ss_type varchar(10) NOT NULL,
	ss_nama varchar(500) NOT NULL,
	ss_result varchar(MAX) NULL,
	ss_kfa_total_data int NULL,
	created_at varchar(50) NULL,
	updated_at varchar(50) NULL,
	deleted_at varchar(50) NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.Tmp_SatuSehatKfa SET (LOCK_ESCALATION = TABLE)
GO
IF EXISTS(SELECT * FROM dbo.SatuSehatKfa)
	 EXEC('INSERT INTO dbo.Tmp_SatuSehatKfa (id, ss_uuid, ss_type, ss_nama, ss_result, ss_kfa_total_data, created_at, updated_at, deleted_at)
		SELECT id, ss_uuid, CONVERT(varchar(10), ss_type), CONVERT(varchar(500), ss_nama), ss_result, ss_kfa_total_data, created_at, updated_at, deleted_at FROM dbo.SatuSehatKfa WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.SatuSehatKfa
GO
EXECUTE sp_rename N'dbo.Tmp_SatuSehatKfa', N'SatuSehatKfa', 'OBJECT' 
GO
ALTER TABLE dbo.SatuSehatKfa ADD CONSTRAINT
	PK_SatuSehatKfa PRIMARY KEY CLUSTERED 
	(
	id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_SatuSehatKfa_UuId ON dbo.SatuSehatKfa
	(
	ss_uuid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_SatuSehatKfa_sstype ON dbo.SatuSehatKfa
	(
	ss_type
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
COMMIT


INSERT INTO SatuSehatKfa (
  id, [ss_uuid], [ss_type], [ss_nama]
) 
select 
  (
    ROW_NUMBER() OVER (
      ORDER BY 
        BridgingID
    )
  )+ 100000 AS sequential_id, 
  BridgingID, 
  'cvxgroup', 
  BridgingName 
FROM 
  (
    SELECT 
      DISTINCT BridgingID, 
      BridgingName 
    from 
      ImmunizationBridging
  ) a
