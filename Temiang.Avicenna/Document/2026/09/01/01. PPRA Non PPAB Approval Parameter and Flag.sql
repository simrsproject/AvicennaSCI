IF COL_LENGTH('dbo.TransPrescription', 'IsPpraApproved') IS NULL
BEGIN
    ALTER TABLE [dbo].[TransPrescription]
    ADD [IsPpraApproved] BIT NULL
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
        (0, 'imel', GETDATE(), NULL, 'IsNeedPpraApproval', 'Is Need PPRA Approval for Non PPAB Prescription', ' ', 'No')
END
ELSE
BEGIN
    UPDATE [dbo].[AppParameter]
    SET [ParameterName] = 'Is Need PPRA Approval for Non PPAB Prescription',
        [ParameterType] = ' ',
        [ParameterValue] = CASE WHEN ISNULL([ParameterValue], '') = '' THEN 'No' ELSE [ParameterValue] END
    WHERE [ParameterID] = 'IsNeedPpraApproval'
END
