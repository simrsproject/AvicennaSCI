-- Jalankan pada database UAT. Report LOA tidak diubah.
IF OBJECT_ID('dbo.spxml_AdmedikaPrintLOC', 'P') IS NULL
    EXEC('CREATE PROCEDURE dbo.spxml_AdmedikaPrintLOC AS SET NOCOUNT ON;');
GO
ALTER PROCEDURE dbo.spxml_AdmedikaPrintLOC @ClaimID VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.AdmedikaClaimTransactionID, a.RegistrationNo, a.TransactionNo,
           a.PatientID, r.GuarantorID, a.CardNo, a.ClID, a.CovID, a.CovCode,
           a.PlanCode, a.DiagnosisCodeList, a.ClStatus, a.ClDesc, a.ServiceID,
           a.PrintOut, a.TotAmtIncurred, a.TotAmtApproved, a.TotAmtNotApproved,
           a.IsClosed, a.CreatedDateTime, a.CreatedByUserID,
           a.LastUpdateDateTime, a.LastUpdateByUserID, a.PatientName,
           a.DateOfBirth, a.PrintOutLOA, a.PrintOutLOC, a.PrintOutLOR,
           a.PolicyNo, a.PayorMemberID, a.PolicyDate
    FROM dbo.AdmedikaClaimTransaction a
    LEFT JOIN dbo.Registration r ON a.RegistrationNo = r.RegistrationNo
    WHERE a.ClID = @ClaimID;
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.AppProgram WHERE ProgramID = 'XML.01.2102')
BEGIN
    INSERT dbo.AppProgram
        (ProgramID, ParentProgramID, ProgramName, TopLevelProgramID, RootLevel,
         RowIndex, IsParentProgram, IsProgram, IsBeginGroup, ProgramType,
         IsVisible, IsDiscontinue, NavigateUrl, StoreProcedureName, AccessKey,
         IsUsingReportHeader, IsDirectPrintEnable, IsListLoadRecordOnInit,
         IsListLoadRecordIfFiltered, ApplicationID)
    VALUES
        ('XML.01.2102', '', 'Print LOC (Admedika)', '01', 0, 0, 0, 0, 0, 'XML',
         1, 0, 'BillingCharges/PrintLOC_Admedika.trdx', 'spxml_AdmedikaPrintLOC', '2',
         0, 1, 1, 0, '');
END;
GO
