CREATE OR ALTER PROCEDURE GenerateSRAutoNumberPasienTitipan
(
    @GuarantorID  NVARCHAR(50),
    @ServiceUnitID NVARCHAR(50)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @GuarantorType NVARCHAR(50),
        @ServiceGroup  VARCHAR(50),
        @SRAutoNumber  NVARCHAR(50);

    SELECT
        @GuarantorType = SRGuarantorType
    FROM Guarantor
    WHERE GuarantorID = @GuarantorID;

    IF @GuarantorType IS NULL
    BEGIN
        RAISERROR('Guarantor tidak ditemukan',16,1);
        RETURN;
    END

    SELECT TOP 1
        @ServiceGroup = qs.ServiceGroup
    FROM QueueMappingPivot qmp
    INNER JOIN QueueStage qs
        ON qs.StageID = qmp.StageID
    WHERE qmp.ServiceUnitID = @ServiceUnitID;

    IF @GuarantorType = '00'
    BEGIN
        SET @SRAutoNumber = 'VisitTunaiNo';
    END
    ELSE IF @GuarantorType = '09'
    BEGIN

        IF @ServiceGroup = 'POLI'
            SET @SRAutoNumber = 'VisitBpjsPoliNo';

        ELSE IF @ServiceGroup = 'REHAB'
            SET @SRAutoNumber = 'VisitBpjsIrmNo';

        ELSE IF @ServiceGroup = 'HEMODIALISA'
            SET @SRAutoNumber = 'VisitBpjsHdNo';

        ELSE IF @ServiceGroup IN
        (
            'CT SCAN',
            'ENDOSCOPY',
            'LAB',
            'RADIOLOGI',
            'USG'
        )
            SET @SRAutoNumber = 'VisitBpjsPenunjangNo';

        ELSE
        BEGIN
            RAISERROR(
                'ServiceGroup BPJS belum memiliki konfigurasi Auto Number',
                16,
                1
            );
            RETURN;
        END
    END
    ELSE
    BEGIN
        SET @SRAutoNumber = 'VisitMitraNo';
    END

    SELECT @SRAutoNumber AS SRAutoNumber;
END
GO