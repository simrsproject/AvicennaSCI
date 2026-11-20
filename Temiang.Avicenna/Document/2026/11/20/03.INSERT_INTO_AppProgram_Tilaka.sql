-- Parent menu
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
    '01.23', '01', 'Tilaka E-Signature', '01', 1, 1900, '',
    1, 0, 0, 'PRG',
    0, 0, 0, 0,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    NULL, NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    0, 0, 0, NULL, NULL, NULL
);

-- Monitoring & registrasi sertifikat dokter
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
    '01.23.01', '01.23', 'Monitoring & Registrasi Sertifikat Dokter', '01', 2, 1, '',
    0, 1, 0, 'PRG',
    1, 1, 1, 1,
    0, 0, 0, 0,
    0, 0,
    1, 0, 1, 0,
    '~/Module/RADT/Tilaka/TilakaRegistrationList.aspx', NULL, NULL, NULL, NULL, NULL,
    NULL, NULL, NULL, NULL, NULL,
    'LIVE2', NULL, 0, 0, 0, NULL
);
