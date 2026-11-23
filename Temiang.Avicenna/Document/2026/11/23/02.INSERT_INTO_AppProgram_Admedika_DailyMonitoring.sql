INSERT INTO AppProgram (
    ProgramID, ParentProgramID, ProgramName, TopLevelProgramID, RootLevel, RowIndex, Note,
    IsParentProgram, IsProgram, IsBeginGroup, ProgramType,
    IsProgramAddAble, IsProgramEditAble, IsProgramDeleteAble, IsProgramViewAble,
    IsProgramApprovalAble, IsProgramUnApprovalAble, IsProgramVoidAble, IsProgramUnVoidAble,
    IsProgramDirectVoid, IsProgramPrintAble,
    IsMenuAddVisible, IsMenuHomeVisible, IsVisible, IsDiscontinue,
    NavigateUrl, HelpLinkID, AssemblyName, AssemblyClassName, StoreProcedureName, AccessKey,
    IsUsingReportHeader, IsDirectPrintEnable, IsListLoadRecordOnInit, IsListLoadRecordIfFiltered, IsProgramRedirected,
    ApplicationID, ZplCommandTemplate, IsProgramExportAble, IsProgramCrossUnitAble, IsProgramPowerUserAble, SRProgramCategory
) VALUES (
    '01.21.05', '01.21', 'Daily Monitoring', '01', 2, 5, '',
    0, 1, 0, 'PRG',
    0, 1, 0, 1,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    '~/Module/RADT/Admedika/DailyMonitoring/AdmedikaDailyMonitoringList.aspx', NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    'LIVE2', NULL, 0, 0, 0, NULL
);
