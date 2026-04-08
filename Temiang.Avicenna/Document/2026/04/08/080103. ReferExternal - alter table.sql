ALTER TABLE ReferExternal
ADD SRReferType VARCHAR(15)
GO

ALTER TABLE ReferExternalBak
ADD SRReferType VARCHAR(15)
GO

INSERT INTO AppStandardReferenceItem
(StandardReferenceID, ItemID, ItemName, IsUsedBySystem, IsActive, LastUpdateDateTime, LastUpdateByUserID, LineNumber)
VALUES
('RefferalType', 'REFTYPE01', 'Rawat Inap (Internal)', 1, 1, GETDATE(), 'fajri', 1),
('RefferalType', 'REFTYPE02', 'Kontrol Ulang', 1, 1, GETDATE(), 'fajri', 2),
('RefferalType', 'REFTYPE03', 'Konsultasi', 1, 1, GETDATE(), 'fajri', 3),
('RefferalType', 'REFTYPE04', 'Rawat Inap (External)', 1, 1, GETDATE(), 'fajri', 4),
('RefferalType', 'REFTYPE05', 'Rawat Jalan (External)', 1, 1, GETDATE(), 'fajri', 5)
GO

INSERT INTO AppStandardReferenceItemBridging
(StandardReferenceID, ItemID, SRBridgingType, BridgingID, BridgingName, LastUpdateDateTime, LastUpdateByUserID)
VALUES
('RefferalType', 'REFTYPE01', 'BridgingType-014', '737481003', 'Inpatient care management' ,GETDATE(), 'fajri'),
('RefferalType', 'REFTYPE02', 'BridgingType-014', '185389009', 'Follow-up visit' ,GETDATE(), 'fajri'),
('RefferalType', 'REFTYPE03', 'BridgingType-014', '11429006', 'Consultation' ,GETDATE(), 'fajri'),
('RefferalType', 'REFTYPE04', 'BridgingType-014', '737481003', 'Inpatient care management' ,GETDATE(), 'fajri'),
('RefferalType', 'REFTYPE05', 'BridgingType-014', '737492002', 'Outpatient care management' ,GETDATE(), 'fajri')
GO