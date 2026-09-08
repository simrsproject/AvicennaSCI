IF COL_LENGTH('dbo.TransPrescription', 'IsPpraApproved') IS NULL
BEGIN
    ALTER TABLE [dbo].[TransPrescription]
    ADD [IsPpraApproved] BIT NULL
END

IF COL_LENGTH('dbo.TransPrescription', 'IsPpraRejected') IS NULL
BEGIN
    ALTER TABLE [dbo].[TransPrescription]
    ADD [IsPpraRejected] BIT NULL
END

IF COL_LENGTH('dbo.TransPrescription', 'PpraRejectionReason') IS NULL
BEGIN
    ALTER TABLE [dbo].[TransPrescription]
    ADD [PpraRejectionReason] NVARCHAR(500) NULL
END

IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[AppParameter]
    WHERE [ParameterID] = 'IsNeedPpraApproval'
)
BEGIN
    INSERT INTO [dbo].[AppParameter]
        ([IsUsedBySystem], [LastUpdateByUserID], [LastUpdateDateTime], [Message], [ParameterID], [ParameterName], [ParameterType], [ParameterValue])
    VALUES
        (0, '240092', '2026-09-01T13:50:49.360Z', NULL, 'IsNeedPpraApproval', 'Is Need PPRA Approval for Non PPAB Prescription (Yes/No)', ' ', 'No')
END
ELSE
BEGIN
    UPDATE [dbo].[AppParameter]
    SET [ParameterName] = 'Is Need PPRA Approval for Non PPAB Prescription (Yes/No)',
        [ParameterType] = ' ',
        [ParameterValue] = CASE WHEN ISNULL([ParameterValue], '') = '' THEN 'No' ELSE [ParameterValue] END
    WHERE [ParameterID] = 'IsNeedPpraApproval'
END
