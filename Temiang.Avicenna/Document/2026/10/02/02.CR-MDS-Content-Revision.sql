-- Records actual MDS content revisions. Initial inserts and metadata-only saves are excluded.
-- Run on the application/report database before deploying the report Web Service.
IF OBJECT_ID('dbo.MedicalDischargeSummaryRevision', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.MedicalDischargeSummaryRevision
    (
        RegistrationNo varchar(20) NOT NULL,
        IsForCasemix bit NOT NULL,
        LastContentChangeDateTime datetime NOT NULL,
        CONSTRAINT PK_MedicalDischargeSummaryRevision PRIMARY KEY (RegistrationNo, IsForCasemix)
    );
END;

-- Each trigger is compiled in its own SQL batch; works with Run All without GO.
EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.tr_MedicalDischargeSummary_ContentRevision
ON dbo.MedicalDischargeSummary
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @changed TABLE (RegistrationNo varchar(20) PRIMARY KEY);
    INSERT INTO @changed (RegistrationNo)
    SELECT i.RegistrationNo
    FROM inserted i
    INNER JOIN deleted d ON d.RegistrationNo = i.RegistrationNo
    WHERE EXISTS
    (
        SELECT CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ChiefComplaint])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[HistOfPresentIllness])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Komorbiditas])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PhysicalExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AncillaryExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[MedicalProcedures])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ProcedureID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Medications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PresentStatus])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SuggestionFollowUp])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[TreatmentIndications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PastMedicalHistory])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ProcedureName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ParamedicID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ParamedicName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRUnitIntended])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRDischargeMethod])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRDischargeCondition])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Prognosis])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsRichTextMode])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AncillaryExamOther])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Diet])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[HomeCare])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[EducationAtHome])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Consul])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[MedicalSupport])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[InLocation])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[CollectionDateTime])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[InitialDiagnose])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRTypeOfService])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRCauseOfDisease])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRCauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[CauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRNatureOfSurgery])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction6])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction6]))
        EXCEPT
        SELECT CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ChiefComplaint])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[HistOfPresentIllness])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Komorbiditas])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PhysicalExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AncillaryExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[MedicalProcedures])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ProcedureID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Medications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PresentStatus])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SuggestionFollowUp])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[TreatmentIndications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PastMedicalHistory])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ProcedureName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ParamedicID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ParamedicName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRUnitIntended])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRDischargeMethod])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRDischargeCondition])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Prognosis])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsRichTextMode])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AncillaryExamOther])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Diet])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[HomeCare])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[EducationAtHome])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Consul])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[MedicalSupport])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[InLocation])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[CollectionDateTime])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[InitialDiagnose])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRTypeOfService])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRCauseOfDisease])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRCauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[CauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRNatureOfSurgery])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction6])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction6]))
    );

    UPDATE r SET LastContentChangeDateTime = GETDATE()
    FROM dbo.MedicalDischargeSummaryRevision r
    INNER JOIN @changed c ON c.RegistrationNo = r.RegistrationNo
    WHERE r.IsForCasemix = 0;

    INSERT INTO dbo.MedicalDischargeSummaryRevision
        (RegistrationNo, IsForCasemix, LastContentChangeDateTime)
    SELECT c.RegistrationNo, 0, GETDATE()
    FROM @changed c
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.MedicalDischargeSummaryRevision r WITH (UPDLOCK, HOLDLOCK)
        WHERE r.RegistrationNo = c.RegistrationNo AND r.IsForCasemix = 0
    );
END;';

EXEC sys.sp_executesql N'CREATE OR ALTER TRIGGER dbo.tr_MedicalDischargeSummaryCmx_ContentRevision
ON dbo.MedicalDischargeSummaryCmx
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @changed TABLE (RegistrationNo varchar(20) PRIMARY KEY);
    INSERT INTO @changed (RegistrationNo)
    SELECT i.RegistrationNo
    FROM inserted i
    INNER JOIN deleted d ON d.RegistrationNo = i.RegistrationNo
    WHERE EXISTS
    (
        SELECT CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ChiefComplaint])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[HistOfPresentIllness])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Komorbiditas])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PhysicalExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AncillaryExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[MedicalProcedures])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ProcedureID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Medications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AdmittingDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseID3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[FinalDiagnoseName3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PresentStatus])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SuggestionFollowUp])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[TreatmentIndications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[PastMedicalHistory])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ProcedureName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ParamedicID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[ParamedicName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRUnitIntended])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRDischargeMethod])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRDischargeCondition])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Prognosis])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsRichTextMode])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[AncillaryExamOther])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Diet])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[HomeCare])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[EducationAtHome])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Consul])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[MedicalSupport])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[InLocation])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[CollectionDateTime])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[InitialDiagnose])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRTypeOfService])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRCauseOfDisease])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRCauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[CauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[SRNatureOfSurgery])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[IsInstruction6])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), i.[Instruction6]))
        EXCEPT
        SELECT CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ChiefComplaint])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[HistOfPresentIllness])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Komorbiditas])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PhysicalExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AncillaryExam])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[MedicalProcedures])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ProcedureID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Medications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AdmittingDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseID3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[FinalDiagnoseName3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PresentStatus])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SuggestionFollowUp])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[TreatmentIndications])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[PastMedicalHistory])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ProcedureName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ParamedicID])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[ParamedicName])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRUnitIntended])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRDischargeMethod])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRDischargeCondition])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Prognosis])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsRichTextMode])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[AncillaryExamOther])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Diet])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[HomeCare])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[EducationAtHome])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Consul])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[MedicalSupport])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[InLocation])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[CollectionDateTime])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[InitialDiagnose])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRTypeOfService])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRCauseOfDisease])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRCauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[CauseOfDevelopDisorder])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[SRNatureOfSurgery])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction1])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction2])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction3])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction4])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction5])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[IsInstruction6])),
                CONVERT(varbinary(max), CONVERT(nvarchar(max), d.[Instruction6]))
    );

    UPDATE r SET LastContentChangeDateTime = GETDATE()
    FROM dbo.MedicalDischargeSummaryRevision r
    INNER JOIN @changed c ON c.RegistrationNo = r.RegistrationNo
    WHERE r.IsForCasemix = 1;

    INSERT INTO dbo.MedicalDischargeSummaryRevision
        (RegistrationNo, IsForCasemix, LastContentChangeDateTime)
    SELECT c.RegistrationNo, 1, GETDATE()
    FROM @changed c
    WHERE NOT EXISTS
    (
        SELECT 1 FROM dbo.MedicalDischargeSummaryRevision r WITH (UPDLOCK, HOLDLOCK)
        WHERE r.RegistrationNo = c.RegistrationNo AND r.IsForCasemix = 1
    );
END;';

