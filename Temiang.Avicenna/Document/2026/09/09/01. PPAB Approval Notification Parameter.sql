IF NOT EXISTS
(
    SELECT 1
    FROM [dbo].[AppParameter]
    WHERE [ParameterID] = 'IsShowPpabApprovalNotification'
)
BEGIN
    INSERT INTO [dbo].[AppParameter]
        ([IsUsedBySystem], [LastUpdateByUserID], [LastUpdateDateTime], [Message], [ParameterID], [ParameterName], [ParameterType], [ParameterValue])
    VALUES
        (0, '240092', '2026-09-09T09:00:00.000Z', NULL, 'IsShowPpabApprovalNotification', 'Is Show PPAB Approval Notification (Yes/No)', ' ', 'No')
END
ELSE
BEGIN
    UPDATE [dbo].[AppParameter]
    SET [ParameterName] = 'Is Show PPAB Approval Notification (Yes/No)',
        [ParameterType] = ' ',
        [ParameterValue] = CASE WHEN ISNULL([ParameterValue], '') = '' THEN 'No' ELSE [ParameterValue] END
    WHERE [ParameterID] = 'IsShowPpabApprovalNotification'
END
