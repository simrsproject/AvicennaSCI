
CREATE PROCEDURE dbo.spxml_AdmedikaPrintLOA
    @ClaimID VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        AdmedikaClaimTransactionID,
        RegistrationNo,
        TransactionNo,
        PatientID,
        GuarantorID,
        CardNo,
        ClID,
        CovID,
        CovCode,
        PlanCode,
        DiagnosisCodeList,
        ClStatus,
        ClDesc,
        ServiceID,
        PrintOut,
        TotAmtIncurred,
        TotAmtApproved,
        TotAmtNotApproved,
        IsClosed,
        CreatedDateTime,
        CreatedByUserID,
        LastUpdateDateTime,
        LastUpdateByUserID,
        PatientName,
        DateOfBirth,
        PrintOutLOA,
        PrintOutLOC,
        PrintOutLOR,
        PolicyNo,
        PayorMemberID,
        PolicyDate
    FROM dbo.AdmedikaClaimTransaction
    WHERE ClID = @ClaimID;
END;
GO
