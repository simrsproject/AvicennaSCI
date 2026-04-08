ALTER TABLE dbo.ItemLaboratory ADD
	SRResultValueType VARCHAR(30) NULL

ALTER TABLE dbo.ItemLaboratory ADD
	IsFasting BIT NULL


INSERT INTO AppstandardReference(StandardReferenceID,StandardReferenceName,ItemLength,IsUsedBySystem,IsActive,StandardReferenceGroup,Note,LastUpdateDateTime,LastUpdateByUserID,HasCOA,IsNumericValue) VALUES(N'LabResultType',N'Laboratory Result Type',20,1,1,NULL,NULL,NULL,N'sci',0,NULL);

INSERT INTO AppstandardReferenceItem(StandardReferenceID,ItemID,ItemName,Note,IsUsedBySystem,IsActive,LastUpdateDateTime,LastUpdateByUserID,ReferenceID,coaID,subledgerID,CustomField,LineNumber,NumericValue,CustomField2) VALUES(N'LabResultType',N'LabResultType-001',N'Quantity','',1,1,NULL,N'sci',N'nrs,lab',NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO AppstandardReferenceItem(StandardReferenceID,ItemID,ItemName,Note,IsUsedBySystem,IsActive,LastUpdateDateTime,LastUpdateByUserID,ReferenceID,coaID,subledgerID,CustomField,LineNumber,NumericValue,CustomField2) VALUES(N'LabResultType',N'LabResultType-002',N'Codeable','',1,1,NULL,N'sci',N'nrs',NULL,NULL,NULL,NULL,NULL,NULL);
INSERT INTO AppstandardReferenceItem(StandardReferenceID,ItemID,ItemName,Note,IsUsedBySystem,IsActive,LastUpdateDateTime,LastUpdateByUserID,ReferenceID,coaID,subledgerID,CustomField,LineNumber,NumericValue,CustomField2) VALUES(N'LabResultType',N'LabResultType-003',N'String','',1,1,NULL,N'sci',N'nrs',NULL,NULL,NULL,NULL,NULL,NULL);