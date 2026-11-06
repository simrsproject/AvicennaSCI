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
    '01.21', '01', 'AdMedika Insurance', '01', 1, 1898, '',
    1, 0, 0, 'PRG',
    0, 0, 0, 0,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    NULL, NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    0, 0, 0, NULL, NULL, NULL
);


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
    '01.21.01', '01.21', 'Kelayakan Kepesertaan (Eligibility)', '01', 2, 1, '',
    0, 1, 0, 'PRG',
    1, 1, 1, 1,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    '~/Module/RADT/Admedika/Eligibility/AdmedikaEligibilityList.aspx', NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    'LIVE2', NULL, 0, 0, 0, NULL
);



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
    '01.21.02', '01.21', 'Discharge OutPatient', '01', 2, 2, '',
    0, 1, 0, 'PRG',
    1, 1, 1, 1,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    '~/Module/RADT/Admedika/Discharge/AdmedikaDischargeList.aspx?rt=OPR', NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    'LIVE2', NULL, 0, 0, 0, NULL
);



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
    '01.21.03', '01.21', 'Discharge InPatient', '01', 2, 3, '',
    0, 1, 0, 'PRG',
    1, 1, 1, 1,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    '~/Module/RADT/Admedika/Discharge/AdmedikaDischargeList.aspx?rt=IPR', NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    'LIVE2', NULL, 0, 0, 0, NULL
);




