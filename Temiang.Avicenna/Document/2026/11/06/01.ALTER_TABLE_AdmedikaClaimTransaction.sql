ALTER TABLE AdmedikaClaimTransaction
ADD
    PatientName NVARCHAR(100) NULL,
    DateOfBirth DATETIME NULL,
    PrintOutLOA NVARCHAR(MAX) NULL,
    PrintOutLOC NVARCHAR(MAX) NULL,
    PrintOutLOR NVARCHAR(MAX) NULL,
    PolicyNo NVARCHAR(32) NULL,
    PayorMemberID NVARCHAR(32) NULL,
    PolicyDate NVARCHAR(32) NULL;