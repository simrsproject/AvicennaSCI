DECLARE @SatuSehatBridgingTypeID AS VARCHAR(20)
SELECT @SatuSehatBridgingTypeID = ParameterValue FROM AppParameter AS ap WHERE ap.ParameterID = 'SatuSehatBridgingTypeID'

INSERT INTO [AppStandardReferenceItemBridging]
    ([StandardReferenceID],[ItemID],[SRBridgingType],
     [BridgingID],[BridgingName],[LastUpdateDateTime],[LastUpdateByUserID])
VALUES
    (N'DischargeCondition', N'I01', @SatuSehatBridgingTypeID, N'359746009', N'Patient’s condition stable', NULL, NULL),
    (N'DischargeCondition', N'I02', @SatuSehatBridgingTypeID, N'268910001', N'Patient’s condition improved', NULL, NULL),
    (N'DischargeCondition', N'I03', @SatuSehatBridgingTypeID, N'162668006', N'Patient’s condition unstable', NULL, NULL),
    (N'DosageUnit',         N'KPL', @SatuSehatBridgingTypeID, N'CAPLET',     N'Caplet', NULL, NULL),
    (N'FamilyPastMedHist',  N'001', @SatuSehatBridgingTypeID, N'160357008', N'Family history of hypertension', NULL, NULL),
    (N'FamilyPastMedHist',  N'002', @SatuSehatBridgingTypeID, N'429959009', N'Family history of heart failure', NULL, NULL),
    (N'FamilyPastMedHist',  N'003', @SatuSehatBridgingTypeID, N'266883004', N'Family history of neoplasm', NULL, NULL),
    (N'FamilyPastMedHist',  N'007', @SatuSehatBridgingTypeID, N'160303001', N'Family history of diabetes mellitus', NULL, NULL),
    (N'FamilyPastMedHist',  N'008', @SatuSehatBridgingTypeID, N'160377001', N'Family history of asthma', NULL, NULL),
    (N'FamilyPastMedHist',  N'010', @SatuSehatBridgingTypeID, N'289916006', N'Family history of kidney disease', NULL, NULL),
    (N'ItemUnit',           N'KPL', @SatuSehatBridgingTypeID, N'CAPLET',     N'Caplet', NULL, NULL),
    (N'PastMedHist',        N'001', @SatuSehatBridgingTypeID, N'161501007', N'History of hypertension', NULL, NULL),
    (N'PastMedHist',        N'002', @SatuSehatBridgingTypeID, N'275544003', N'History of heart disorder', NULL, NULL),
    (N'PastMedHist',        N'007', @SatuSehatBridgingTypeID, N'161445009', N'History of diabetes mellitus', NULL, NULL),
    (N'PastMedHist',        N'008', @SatuSehatBridgingTypeID, N'161527007', N'H/O: asthma', NULL, NULL),
    (N'PastMedHist',        N'010', @SatuSehatBridgingTypeID, N'275552000', N'History of kidney disease', NULL, NULL),
    (N'PatientRiskColor',   N'01',  @SatuSehatBridgingTypeID, N'186193001', N'Tuberculosis of lung, confirmed by sputum microscopy with or without culture', NULL, NULL),
    (N'PatientRiskColor',   N'02',  @SatuSehatBridgingTypeID, N'154283005', N'Pulmonary tuberculosis', NULL, NULL),
    (N'PatientRiskColor',   N'03',  @SatuSehatBridgingTypeID, N'735521001', N'WHO 2007 HIV infection clinical stage 1 and tuberculosis', NULL, NULL),
    (N'PatientRiskColor',   N'06',  @SatuSehatBridgingTypeID, N'86406008',  N'Human immunodeficiency virus infection', NULL, NULL);
