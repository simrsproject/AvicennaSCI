SET NOCOUNT ON
GO
ALTER TABLE MedicalDischargeSummaryCmx
ADD IsNeedDPJPValidation BIT NULL;

INSERT INTO [AppProgram]([ProgramID],[ParentProgramID],[ProgramName],[TopLevelProgramID],[RootLevel],[RowIndex],[Note],[IsParentProgram],[IsProgram],[IsBeginGroup],[ProgramType],[IsProgramAddAble],[IsProgramEditAble],[IsProgramDeleteAble],[IsProgramViewAble],[IsProgramApprovalAble],[IsProgramUnApprovalAble],[IsProgramVoidAble],[IsProgramUnVoidAble],[IsProgramDirectVoid],[IsProgramPrintAble],[IsMenuAddVisible],[IsMenuHomeVisible],[IsVisible],[IsDiscontinue],[NavigateUrl],[HelpLinkID],[AssemblyName],[AssemblyClassName],[StoreProcedureName],[AccessKey],[IsUsingReportHeader],[IsDirectPrintEnable],[IsListLoadRecordOnInit],[IsListLoadRecordIfFiltered],[IsProgramRedirected],[ApplicationID],[ZplCommandTemplate],[IsProgramExportAble],[IsProgramCrossUnitAble],[IsProgramPowerUserAble],[SRProgramCategory])
VALUES(N'01.20.12B',N'01.20',N'Casemix DPJP Validation',N'01',2,1899,'',0,1,0,N'PRG',1,1,0,1,0,0,0,0,0,0,1,0,1,0,N'~/Module/RADT/Bpjs/Casemix/CasemixPhysicianInChargeValidation.aspx',NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL,N'HIS2015',NULL,0,0,0,NULL);

GO
SET NOCOUNT OFF
GO
