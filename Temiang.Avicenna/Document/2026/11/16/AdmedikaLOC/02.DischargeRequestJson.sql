-- Deploy before the web application. No generated BusinessObject edits required.
IF COL_LENGTH('dbo.AdmedikaClaimTransaction', 'DischargeRequestJson') IS NULL
    ALTER TABLE dbo.AdmedikaClaimTransaction ADD DischargeRequestJson nvarchar(max) NULL;
GO
