ALTER PROCEDURE [dbo].[sp_GetLatestIPRRegistration]
    @MedicalNo VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
		a.RegistrationNo, 
		a.SRRegistrationType, 
		b.MedicalNo, 
		a.PatientID, 
		a.RegistrationDate, 
		a.RegistrationTime, 
		a.DischargeDate, 
		a.DischargeTime
	FROM Registration a
	LEFT JOIN Patient b ON a.PatientID = b.PatientID
	WHERE b.MedicalNo LIKE @MedicalNo + '%'
	  AND a.SRRegistrationType = 'IPR'
	  AND a.IsVoid = 0
	ORDER BY a.RegistrationDate DESC, a.RegistrationTime DESC
	OFFSET 1 ROW FETCH NEXT 1 ROW ONLY;
END