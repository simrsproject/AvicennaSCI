ALTER TABLE ItemLaboratory
ADD IsFasting BIT NULL
GO

ALTER TABLE PatientAssessment
ADD SCTHpi VARCHAR(20) NULL
GO

ALTER TABLE ItemRadiology
ADD DicomCode VARCHAR(20) NULL
GO

INSERT INTO AppParameter([ParameterID],[ParameterName],[ParameterValue],[ParameterType],[LastUpdateDateTime],[LastUpdateByUserID],[IsUsedBySystem],[Message])
VALUES(N'AdlScoreQuestionFormIdAndQuestionId',N'QuestionFormId and QuestionId for ADL Score Format QuestionFormId|QuestionId. divided by |',N'',N' ',GETDATE(),N'naufal',0,NULL);
GO

ALTER TABLE ItemLaboratory
ADD IsFasting BIT NULL
GO