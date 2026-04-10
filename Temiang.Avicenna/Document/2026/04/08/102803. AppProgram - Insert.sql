INSERT INTO [dbo].[AppProgram](
  [ProgramID], [ParentProgramID], [ProgramName], 
  [TopLevelProgramID], [RootLevel], 
  [RowIndex], [Note], [IsParentProgram], 
  [IsProgram], [IsBeginGroup], [ProgramType], 
  [IsProgramAddAble], [IsProgramEditAble], 
  [IsProgramDeleteAble], [IsProgramViewAble], 
  [IsProgramApprovalAble], [IsProgramUnApprovalAble], 
  [IsProgramVoidAble], [IsProgramUnVoidAble], 
  [IsProgramDirectVoid], [IsProgramPrintAble], 
  [IsMenuAddVisible], [IsMenuHomeVisible], 
  [IsVisible], [IsDiscontinue], [NavigateUrl], 
  [HelpLinkID], [AssemblyName], [AssemblyClassName], 
  [StoreProcedureName], [AccessKey], 
  [IsUsingReportHeader], [IsDirectPrintEnable], 
  [IsListLoadRecordOnInit], [IsListLoadRecordIfFiltered], 
  [IsProgramRedirected], [ApplicationID], 
  [ZplCommandTemplate], [IsProgramExportAble], 
  [IsProgramCrossUnitAble], [IsProgramPowerUserAble], 
  [SRProgramCategory]
) 
VALUES 
  (
    '01.03.70', '01.03', 'Immunization', 
    '01', '3', '1915', '', 0, 1, 1, 'PRG', 
    1, 1, 1, 1, 0, 0, NULL, NULL, NULL, 0, NULL, 
    NULL, 1, 0, '~/Module/RADT/Master/Immunization/ImmunizationList.aspx', 
    '', '', '', NULL, NULL, NULL, 1, 1, 0, 
    NULL, 'HIS2015', NULL, 0, 0, 0, NULL
  )
