/*
================================================================================
 DOKUMENTASI SCRIPT MANTIS - LAPORAN CODE BLUE
================================================================================
 Nama Modul      : Laporan Respon Time Code Blue
 Form            : LKCB / Laporan Code Blue
 Report Program  : RPT.01.0300
 Stored Procedure: sp_LaporanResponTimeCodeBlue
 Dibuat Untuk    : Dokumentasi implementasi Mantis laporan Code Blue
 Dibuat Oleh     : sci
 Catatan         :
   1. Script ini disusun dari catatan implementasi.
   2. Jangan langsung execute seluruh file di production tanpa review per section.
   3. Beberapa section berisi INSERT/UPDATE data master dan dapat gagal jika data
      dengan key yang sama sudah ada.
   4. Untuk eksekusi aman, jalankan per section dan validasi hasil SELECT.
================================================================================
*/

SET NOCOUNT ON;
GO

-- DAFTAR SECTION
-- 01. Insert Question LKCB02 - Waktu mulai aktivasi
-- 02. Insert Question LKCB26 - Waktu Tim Code Blue Datang
-- 03. Set posisi LKCB26 di QuestionInGroup RowIndex 3
-- 04. Insert LKCB27, LKCB28, LKCB29 dan set RowIndex 27-29
-- 05. Backup konfigurasi Question LKCB110 sebelum perubahan
-- 06. Update Question LKCB110 - Kolom tindakan/obat Code Blue
-- 07. Update LKCB102 menjadi Perawat Pelaksana 1
-- 08. Tambah Perawat Pelaksana 2 dan 3 di bawah LKCB102
-- 09. Update LKCB103 menjadi lookup Users / CBL
-- 10. Tambah LKCB30 - Nadi di bawah LKCB18
-- 11. Tambah dropdown IRAMAJANTUNG dan update LKCB19
-- 12. Register menu report AppProgram - Respon Time Code Blue
-- 13. Register parameter report DateFromToCtl
-- 14. Register akses report ke AppUserGroupProgram
-- 15. Create procedure sp_LaporanResponTimeCodeBlue
GO

/*
================================================================================
 SECTION 01 - Insert Question LKCB02 - Waktu mulai aktivasi
================================================================================
*/

insert into [dbo].[Question] ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID]) values (0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, NULL, 0, NULL, NULL, NULL, 'sci', '2020-12-23T09:21:12.517Z', NULL, NULL, NULL, '', '', '', NULL, 'LKCB02', 3, 'wkt', 'Waktu mulai aktivasi:', NULL, NULL, NULL, 'TIM', NULL);

GO

/*
================================================================================
 SECTION 02 - Insert Question LKCB26 - Waktu Tim Code Blue Datang
================================================================================
*/

insert into [dbo].[Question] ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID]) values (0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, NULL, 0, NULL, NULL, NULL, 'sci', '2020-12-23T09:21:12.517Z', NULL, NULL, NULL, '', '', '', NULL, 'LKCB26', 3, 'wkt', 'Waktu Tim Code Blue Datang:', NULL, NULL, NULL, 'TIM', NULL);

GO

/*
================================================================================
 SECTION 03 - Set posisi LKCB26 di QuestionInGroup RowIndex 3
================================================================================
*/

BEGIN TRANSACTION;

-- 1. Geser semua RowIndex >= 3 (supaya slot RowIndex 3 kosong)
UPDATE QuestionInGroup
SET RowIndex = RowIndex + 1
WHERE QuestionGroupID = 'LKCB01' AND RowIndex >= 3;

-- 2. Insert LKCB26 ke RowIndex 3
INSERT INTO QuestionInGroup 
    (QuestionGroupID, QuestionID, RowIndex, LastUpdateDateTime, LastUpdateByUserID, PageNo, ParentQuestionID, QuestionLevel)
VALUES 
    ('LKCB01', 'LKCB26', 3, GETDATE(), 'sci', NULL, NULL, 1);

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 04 - Insert LKCB27, LKCB28, LKCB29 dan set RowIndex 27-29
================================================================================
*/

BEGIN TRANSACTION;

-- =========================================
-- 1. INSERT ke table Question (3 field baru)
-- =========================================

-- LKCB27 - Pernapasan
INSERT INTO [dbo].[Question] 
    ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], 
     [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], 
     [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], 
     [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], 
     [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], 
     [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], 
     [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID])
VALUES 
    (0, NULL, 'x/mnt', NULL, NULL, NULL, 
     '', NULL, NULL, 1, 1, NULL, 
     0, NULL, NULL, NULL, 
     'sci', GETDATE(), NULL, 'Pernapasan :', NULL, 
     '', '', NULL, 
     NULL, 'LKCB27', 3, 'per', 'Pernapasan :', 
     NULL, NULL, NULL, 'NUM', NULL);

-- LKCB28 - Tekanan Darah Sistole
INSERT INTO [dbo].[Question] 
    ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], 
     [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], 
     [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], 
     [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], 
     [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], 
     [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], 
     [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID])
VALUES 
    (0, NULL, ' mmHg', NULL, NULL, NULL, 
     '', NULL, NULL, 1, 1, NULL, 
     0, NULL, NULL, NULL, 
     'sci', GETDATE(), NULL, 'Tekanan Darah - Sistole', NULL, 
     '', '', NULL, 
     NULL, 'LKCB28', 2, NULL, 'Tekanan Darah - Sistole', 
     NULL, NULL, NULL, 'NUM', 'BP1');

-- LKCB29 - Tekanan Darah Diastole
INSERT INTO [dbo].[Question] 
    ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], 
     [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], 
     [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], 
     [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], 
     [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], 
     [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], 
     [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID])
VALUES 
    (0, NULL, ' mmHg', NULL, NULL, NULL, 
     '', NULL, NULL, 1, 1, NULL, 
     0, NULL, NULL, NULL, 
     'sci', GETDATE(), NULL, 'Tekanan Darah - Diastole', NULL, 
     '', '', NULL, 
     NULL, 'LKCB29', 2, NULL, 'Tekanan Darah - Diastole', 
     NULL, NULL, NULL, 'NUM', 'BP2');

-- =========================================
-- 2. Geser RowIndex >= 27 sebanyak +3
-- =========================================
UPDATE QuestionInGroup
SET RowIndex = RowIndex + 3
WHERE QuestionGroupID = 'LKCB01' AND RowIndex >= 27;

-- =========================================
-- 3. Insert ke QuestionInGroup di RowIndex 27, 28, 29
-- =========================================
INSERT INTO QuestionInGroup 
    (QuestionGroupID, QuestionID, RowIndex, LastUpdateDateTime, LastUpdateByUserID, PageNo, ParentQuestionID, QuestionLevel)
VALUES 
    ('LKCB01', 'LKCB27', 27, GETDATE(), 'sci', NULL, NULL, 1),
    ('LKCB01', 'LKCB28', 28, GETDATE(), 'sci', NULL, NULL, 1),
    ('LKCB01', 'LKCB29', 29, GETDATE(), 'sci', NULL, NULL, 1);

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 05 - Backup konfigurasi Question LKCB110 sebelum perubahan
================================================================================
*/

-- Backup original data/config LKCB110

insert into [dbo].[Question] ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID]) values (0, '', '', 17, NULL, '', '', '', NULL, 1, 1, NULL, 0, NULL, NULL, NULL, 'sci', '2021-01-18T13:26:03.047Z', '', '', '', '1|||||||||||||||2|||||||||||||||3|||||||||||||||4|||||||||||||||5|||||||||||||||6|||||||||||||||7|||||||||||||||8|||||||||||||||9|||||||||||||||10|||||||||||||||11|||||||||||||||12|||||||||||||||13|||||||||||||||14|||||||||||||||15|||||||||||||||16|||||||||||||||17||||||||||||||', '', 'Waktu|Pernafasan:75|Tekanan Darah:100|Nadi Irama Jantung:125|(Joules) Defibrilator:125|Pacemaker:75|Epinephrine:75|Atropine:75|Amiodarone:75|Lidocaine:75|Vasopressine:75|Sulfate Magnesium:125|Adenosin:75|Dopamine:75|Catatan Perawat:100', '', 'LKCB110', 1, '.', '.', '', '', '', 'TBL', '');

GO

/*
================================================================================
 SECTION 06 - Update Question LKCB110 - Kolom tindakan/obat Code Blue
================================================================================
*/

BEGIN TRANSACTION;

UPDATE [dbo].[Question]
SET 
    [QuestionAnswerSelectionID] = 'Waktu|(Joules) Defibrilator:125|Pacemaker:75|Epinephrine:75|Atropine:75|Amiodarone:75|Lidocaine:75|Vasopressine:75|Sulfate Magnesium:125|Adenosin:75|Dopamine:75|Catatan Perawat:100',
    [QuestionAnswerDefaultSelectionID] = '1|||||||||||2|||||||||||3|||||||||||4|||||||||||5|||||||||||6|||||||||||7|||||||||||8|||||||||||9|||||||||||10|||||||||||11|||||||||||12|||||||||||13|||||||||||14|||||||||||15|||||||||||16|||||||||||17||||||||||',
    [LastUpdateByUserID] = 'sci',
    [LastUpdateDateTime] = GETDATE()
WHERE [QuestionID] = 'LKCB110';

SELECT 
    QuestionID,
    QuestionAnswerSelectionID,
    QuestionAnswerDefaultSelectionID
FROM [dbo].[Question]
WHERE [QuestionID] = 'LKCB110';

-- kalau sudah sesuai
COMMIT TRANSACTION;

-- kalau salah:
-- ROLLBACK TRANSACTION;

GO

/*
================================================================================
 SECTION 07 - Update LKCB102 menjadi Perawat Pelaksana 1
================================================================================
*/

BEGIN TRANSACTION;

UPDATE [dbo].[Question]
SET
    [QuestionAnswerSelectionID] = 'Users',
    [SRAnswerType] = 'CBL',
    [QuestionText] = 'Perawat Pelaksana 1 :',
    [LastUpdateByUserID] = 'sci',
    [LastUpdateDateTime] = GETDATE()
WHERE [QuestionID] = 'LKCB102';

SELECT 
    QuestionID,
    QuestionText,
    QuestionShortText,
    SRAnswerType,
    QuestionAnswerSelectionID,
    LastUpdateByUserID,
    LastUpdateDateTime
FROM [dbo].[Question]
WHERE [QuestionID] = 'LKCB102';

-- kalau hasil sudah benar
COMMIT TRANSACTION;

-- kalau salah, jalankan sebelum commit:
-- ROLLBACK TRANSACTION;

GO

/*
================================================================================
 SECTION 08 - Tambah Perawat Pelaksana 2 dan 3 di bawah LKCB102
================================================================================
*/

BEGIN TRANSACTION;

DECLARE @QuestionID1 VARCHAR(50) = 'LKCB102';
DECLARE @QuestionID2 VARCHAR(50) = 'LKCB1022';
DECLARE @QuestionID3 VARCHAR(50) = 'LKCB1023';

DECLARE @QuestionGroupID VARCHAR(50);
DECLARE @RowIndex INT;
DECLARE @PageNo INT;
DECLARE @ParentQuestionID VARCHAR(50);
DECLARE @QuestionLevel INT;

-- Ambil posisi LKCB102 di QuestionInGroup
SELECT 
    @QuestionGroupID = QuestionGroupID,
    @RowIndex = RowIndex,
    @PageNo = PageNo,
    @ParentQuestionID = ParentQuestionID,
    @QuestionLevel = QuestionLevel
FROM [dbo].[QuestionInGroup]
WHERE QuestionID = @QuestionID1;


-- 1. Update Perawat Pelaksana lama jadi Perawat Pelaksana 1
UPDATE [dbo].[Question]
SET
    QuestionText = 'Perawat Pelaksana 1 :',
    QuestionAnswerSelectionID = 'Users',
    SRAnswerType = 'CBL',
    LastUpdateByUserID = 'sci',
    LastUpdateDateTime = GETDATE()
WHERE QuestionID = @QuestionID1;


-- 2. Insert Perawat Pelaksana 2 dan 3
INSERT INTO [dbo].[Question] 
    ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], 
     [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], 
     [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], 
     [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], 
     [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], 
     [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], 
     [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID])
VALUES
    (0, NULL, NULL, NULL, NULL, NULL,
     NULL, NULL, NULL, 1, 1, NULL,
     0, NULL, NULL, NULL,
     'sci', GETDATE(), NULL, NULL, NULL,
     '', '', 'Users',
     NULL, @QuestionID2, 1, 'pp2', 'Perawat Pelaksana 2 :',
     NULL, NULL, NULL, 'CBL', NULL),

    (0, NULL, NULL, NULL, NULL, NULL,
     NULL, NULL, NULL, 1, 1, NULL,
     0, NULL, NULL, NULL,
     'sci', GETDATE(), NULL, NULL, NULL,
     '', '', 'Users',
     NULL, @QuestionID3, 1, 'pp3', 'Perawat Pelaksana 3 :',
     NULL, NULL, NULL, 'CBL', NULL);


-- 3. Geser row di bawah LKCB102 sebanyak +2
UPDATE [dbo].[QuestionInGroup]
SET 
    RowIndex = RowIndex + 2,
    LastUpdateByUserID = 'sci',
    LastUpdateDateTime = GETDATE()
WHERE QuestionGroupID = @QuestionGroupID
  AND RowIndex > @RowIndex;


-- 4. Insert Perawat Pelaksana 2 dan 3 tepat di bawah LKCB102
INSERT INTO [dbo].[QuestionInGroup]
    (QuestionGroupID, QuestionID, RowIndex, LastUpdateDateTime, LastUpdateByUserID, PageNo, ParentQuestionID, QuestionLevel)
VALUES
    (@QuestionGroupID, @QuestionID2, @RowIndex + 1, GETDATE(), 'sci', @PageNo, @ParentQuestionID, @QuestionLevel),
    (@QuestionGroupID, @QuestionID3, @RowIndex + 2, GETDATE(), 'sci', @PageNo, @ParentQuestionID, @QuestionLevel);


-- 5. Cek hasil
SELECT 
    qig.QuestionGroupID,
    qig.RowIndex,
    q.QuestionID,
    q.QuestionText,
    q.SRAnswerType,
    q.QuestionAnswerSelectionID
FROM [dbo].[QuestionInGroup] qig
INNER JOIN [dbo].[Question] q 
    ON q.QuestionID = qig.QuestionID
WHERE qig.QuestionGroupID = @QuestionGroupID
ORDER BY qig.RowIndex;

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 09 - Update LKCB103 menjadi lookup Users / CBL
================================================================================
*/

BEGIN TRANSACTION;

UPDATE [dbo].[Question]
SET
    [QuestionAnswerSelectionID] = 'Users',
    [SRAnswerType] = 'CBL',
    [LastUpdateByUserID] = 'sci',
    [LastUpdateDateTime] = GETDATE()
WHERE [QuestionID] = 'LKCB103';

SELECT 
    QuestionID,
    QuestionText,
    QuestionShortText,
    SRAnswerType,
    QuestionAnswerSelectionID,
    QuestionAnswerDefaultSelectionID,
    QuestionAnswerDefaultSelectionID2,
    LastUpdateByUserID,
    LastUpdateDateTime
FROM [dbo].[Question]
WHERE [QuestionID] = 'LKCB103';

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 10 - Tambah LKCB30 - Nadi di bawah LKCB18
================================================================================
*/

BEGIN TRANSACTION;

DECLARE @NewQuestionID VARCHAR(50) = 'LKCB30';
DECLARE @AfterQuestionID VARCHAR(50) = 'LKCB18';

DECLARE @QuestionGroupID VARCHAR(50);
DECLARE @RowIndex INT;
DECLARE @PageNo INT;
DECLARE @ParentQuestionID VARCHAR(50);
DECLARE @QuestionLevel INT;

-- Ambil posisi "Nadi teraba :"
SELECT 
    @QuestionGroupID = QuestionGroupID,
    @RowIndex = RowIndex,
    @PageNo = PageNo,
    @ParentQuestionID = ParentQuestionID,
    @QuestionLevel = QuestionLevel
FROM [dbo].[QuestionInGroup]
WHERE QuestionID = @AfterQuestionID;


-- 1. Insert Question baru: Nadi
insert into [dbo].[Question] ([AnswerDecimalDigit], [AnswerPrefix], [AnswerSuffix], [AnswerWidth], [AnswerWidth2], [BodyID], [EquivalentQuestionID], [Formula], [IndexNo], [IsActive], [IsAlwaysPrint], [IsEmptyDefault], [IsMandatory], [IsNotOverWriteRelatedEntity], [IsReadOnly], [IsUpdateRelatedEntity], [LastUpdateByUserID], [LastUpdateDateTime], [LookUpID], [NursingDisplayAs], [ParentQuestionID], [QuestionAnswerDefaultSelectionID], [QuestionAnswerDefaultSelectionID2], [QuestionAnswerSelectionID], [QuestionAnswerSelectionID2], [QuestionID], [QuestionLevel], [QuestionShortText], [QuestionText], [ReferenceQuestionID], [RelatedColumnName], [RelatedEntityName], [SRAnswerType], [VitalSignID]) values (0, NULL, 'x/mnt', NULL, NULL, NULL, '', NULL, NULL, 1, 1, NULL, 0, NULL, NULL, NULL, 'sci', '2026-07-02T10:44:12.853Z', NULL, 'Nadi :', NULL, '', '', NULL, NULL, 'LKCB30', 3, 'nadi', 'Nadi :', NULL, NULL, NULL, 'NUM', 'HR');

-- 2. Geser row setelah "Nadi teraba :" sebanyak +1
UPDATE [dbo].[QuestionInGroup]
SET 
    RowIndex = RowIndex + 1,
    LastUpdateByUserID = 'sci',
    LastUpdateDateTime = GETDATE()
WHERE QuestionGroupID = @QuestionGroupID
  AND RowIndex > @RowIndex;


-- 3. Insert Nadi tepat di bawah "Nadi teraba :"
INSERT INTO [dbo].[QuestionInGroup]
    (QuestionGroupID, QuestionID, RowIndex, LastUpdateDateTime, LastUpdateByUserID, PageNo, ParentQuestionID, QuestionLevel)
VALUES
    (@QuestionGroupID, @NewQuestionID, @RowIndex + 1, GETDATE(), 'sci', @PageNo, @ParentQuestionID, 3);


-- 4. Cek hasil sekitar Nadi teraba
SELECT 
    qig.QuestionGroupID,
    qig.RowIndex,
    q.QuestionID,
    q.QuestionText,
    q.QuestionShortText,
    q.SRAnswerType,
    q.AnswerSuffix,
    q.NursingDisplayAs,
    qig.QuestionLevel
FROM [dbo].[QuestionInGroup] qig
INNER JOIN [dbo].[Question] q 
    ON q.QuestionID = qig.QuestionID
WHERE qig.QuestionGroupID = @QuestionGroupID
  AND qig.RowIndex BETWEEN @RowIndex - 2 AND @RowIndex + 5
ORDER BY qig.RowIndex;

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 11 - Tambah dropdown IRAMAJANTUNG dan update LKCB19
================================================================================
*/

BEGIN TRANSACTION;

-- =========================================
-- 1. Insert master dropdown
-- =========================================
INSERT INTO [dbo].[QuestionAnswerSelection]
    ([LastUpdateByUserID], [LastUpdateDateTime], [QuestionAnswerSelectionID], [QuestionAnswerSelectionText])
VALUES
    ('sci', GETDATE(), 'IRAMAJANTUNG', 'Irama Jantung');


-- =========================================
-- 2. Insert pilihan dropdown
-- =========================================
INSERT INTO [dbo].[QuestionAnswerSelectionLine]
    ([LastUpdateByUserID], [LastUpdateDateTime], [QuestionAnswerSelectionID], [QuestionAnswerSelectionLineID], [QuestionAnswerSelectionLineText], [Score])
VALUES
    ('sci', GETDATE(), 'IRAMAJANTUNG', '1',  '[1] Synus Rytm', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '2',  '[2] Aritmia', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '3',  '[3] ST Elevasi', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '4',  '[4] ST Depresi', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '5',  '[5] Takikardi', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '6',  '[6] Patologi', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '7',  '[7] Inverted', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '8',  '[8] Bradikardi', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '9',  '[9] PEA', NULL),
    ('sci', GETDATE(), 'IRAMAJANTUNG', '10', '[10] Asistole', NULL);


-- =========================================
-- 3. Update question Irama jantung jadi dropdown
-- =========================================
UPDATE [dbo].[Question]
SET
    [QuestionText] = 'Irama jantung :',
    [QuestionShortText] = 'ij',
    [SRAnswerType] = 'CBO',
    [QuestionAnswerSelectionID] = 'IRAMAJANTUNG',
    [QuestionAnswerDefaultSelectionID] = '',
    [LastUpdateByUserID] = 'sci',
    [LastUpdateDateTime] = GETDATE()
WHERE [QuestionID] = 'LKCB19';


-- =========================================
-- 4. Cek hasil
-- =========================================
SELECT 
    QuestionID,
    QuestionText,
    QuestionShortText,
    SRAnswerType,
    QuestionAnswerSelectionID,
    QuestionAnswerDefaultSelectionID,
    LastUpdateByUserID,
    LastUpdateDateTime
FROM [dbo].[Question]
WHERE QuestionID = 'LKCB19';

SELECT 
    QuestionAnswerSelectionID,
    QuestionAnswerSelectionLineID,
    QuestionAnswerSelectionLineText,
    Score
FROM [dbo].[QuestionAnswerSelectionLine]
WHERE QuestionAnswerSelectionID = 'IRAMAJANTUNG'
ORDER BY CAST(QuestionAnswerSelectionLineID AS INT);

COMMIT TRANSACTION;

GO

/*
================================================================================
 SECTION 12 - Register menu report AppProgram - Respon Time Code Blue
================================================================================
*/

insert into [dbo].[AppProgram] ([AccessKey], [ApplicationID], [AssemblyClassName], [AssemblyName], [HelpLinkID], [IsBeginGroup], [IsDirectPrintEnable], [IsDiscontinue], [IsListLoadRecordIfFiltered], [IsListLoadRecordOnInit], [IsMenuAddVisible], [IsMenuHomeVisible], [IsParentProgram], [IsProgram], [IsProgramAddAble], [IsProgramApprovalAble], [IsProgramCrossUnitAble], [IsProgramDeleteAble], [IsProgramDirectVoid], [IsProgramEditAble], [IsProgramExportAble], [IsProgramPowerUserAble], [IsProgramPrintAble], [IsProgramRedirected], [IsProgramUnApprovalAble], [IsProgramUnVoidAble], [IsProgramViewAble], [IsProgramVoidAble], [IsUsingReportHeader], [IsVisible], [NavigateUrl], [Note], [ParentProgramID], [ProgramID], [ProgramName], [ProgramType], [RootLevel], [RowIndex], [SRProgramCategory], [StoreProcedureName], [TopLevelProgramID], [ZplCommandTemplate]) values ('1', 'HISRPT', NULL, NULL, NULL, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, NULL, 0, 0, 0, NULL, NULL, 0, 0, 0, 0, 0, 0, NULL, 1, 'LaporanResponTimeCodeBlue.trdx', '', '01', 'RPT.01.0300', 'Respon Time Code Blue', 'XML', 0, 0, NULL, 'sp_LaporanResponTimeCodeBlue', '01', NULL);

GO

/*
================================================================================
 SECTION 13 - Register parameter report DateFromToCtl
================================================================================
*/

insert into [dbo].[AppReportParameter] ([IndexNo], [ParameterCaption], [ParameterName], [ProgramID], [ReferenceID], [ReportControlName]) values (1, 'Period', 'p_FromDate;p_ToDate', 'RPT.01.0300', NULL, 'DateFromToCtl');

GO

/*
================================================================================
 SECTION 14 - Register akses report ke AppUserGroupProgram
================================================================================
*/

insert into [dbo].[AppUserGroupProgram] ([IsUserGroupAddAble], [IsUserGroupApprovalAble], [IsUserGroupCrossUnitAble], [IsUserGroupDeleteAble], [IsUserGroupEditAble], [IsUserGroupExportAble], [IsUserGroupPowerUserAble], [IsUserGroupUnApprovalAble], [IsUserGroupUnVoidAble], [IsUserGroupVoidAble], [LastUpdateByUserID], [LastUpdateDateTime], [ProgramID], [UserGroupID]) values (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'sci', '2020-07-28T12:04:51.570Z', 'RPT.01.0300', 'ADMIN'), (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'sci', '2020-07-28T12:04:51.570Z', 'RPT.01.0300', 'BILL.01'), (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, '931628', '2023-03-17T16:47:41.617Z', 'RPT.01.0300', 'BILL.02'), (0, 0, 1, 0, 0, 0, 0, 0, 0, 0, '921490', '2023-10-05T07:55:54.983Z', 'RPT.01.0300', 'KaUnitSIM'), (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'sci', '2020-08-01T16:24:45.160Z', 'RPT.01.0300', 'PENGAWAS'), (0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 'sci', '2020-08-01T16:24:45.160Z', 'RPT.01.0300', 'RM.02'), (0, 0, 1, 0, 1, 0, 0, 0, 0, 0, 'sci', '2022-12-12T21:10:15.030Z', 'RPT.01.0300', 'RM.03');

GO

/*
================================================================================
 SECTION 15 - Create procedure sp_LaporanResponTimeCodeBlue
================================================================================
*/

CREATE PROCEDURE [dbo].[sp_LaporanResponTimeCodeBlue] @p_FromDate DATETIME,
@p_ToDate DATETIME AS BEGIN
SET
NOCOUNT ON;

SELECT
  -- Title & SubTitle
  'Laporan Respon Time Tim Code Blue' AS ReportName,
  'Periode : ' + CONVERT(VARCHAR(11), @p_FromDate, 113) + ' s/d ' + CONVERT(VARCHAR(11), @p_ToDate, 113) AS SubReportName,
  ROW_NUMBER() OVER (
    ORDER BY
      phr.RecordDate,
      phr.RecordTime
  ) AS No,
  phr.RecordDate AS Tanggal,
  su.ServiceUnitName AS Ruangan,
  phr.RegistrationNo AS NoReg,
  pat.MedicalNo AS NoRM,
  RTRIM(
    LTRIM(
      RTRIM(
        LTRIM(
          ISNULL(pat.FirstName, '') + ' ' + ISNULL(pat.MiddleName, '')
        )
      ) + ' ' + ISNULL(pat.LastName, '')
    )
  ) AS NamaPasien,
  lkcb02.QuestionAnswerText AS WaktuMulaiAktivasi,
  lkcb26.QuestionAnswerText AS WaktuCodeBlueDatang,
  lkcb20.QuestionAnswerText AS WaktuMulaiRJP,
  lkcb08.QuestionAnswerText AS WaktuSelesai,
  lkcb103.QuestionAnswerText AS DokterCodeBlue,
  -- Respon Time formatted
  CASE
    WHEN lkcb02.QuestionAnswerText IS NOT NULL
    AND lkcb02.QuestionAnswerText <> ''
    AND lkcb20.QuestionAnswerText IS NOT NULL
    AND lkcb20.QuestionAnswerText <> ''
    AND TRY_CONVERT(
      DATETIME,
      CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
    ) IS NOT NULL
    AND TRY_CONVERT(
      DATETIME,
      CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
    ) IS NOT NULL THEN CAST(
      ABS(
        DATEDIFF(
          SECOND,
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
          ),
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
          )
        )
      ) / 60 AS VARCHAR(10)
    ) + ' mnt ' + CAST(
      ABS(
        DATEDIFF(
          SECOND,
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
          ),
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
          )
        )
      ) % 60 AS VARCHAR(10)
    ) + ' dtk'
    ELSE NULL
  END AS ResponTime,
  -- Flag <= 5 menit per baris
  CASE
    WHEN lkcb02.QuestionAnswerText IS NOT NULL
    AND lkcb02.QuestionAnswerText <> ''
    AND lkcb20.QuestionAnswerText IS NOT NULL
    AND lkcb20.QuestionAnswerText <> ''
    AND TRY_CONVERT(
      DATETIME,
      CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
    ) IS NOT NULL
    AND TRY_CONVERT(
      DATETIME,
      CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
    ) IS NOT NULL
    AND ABS(
      DATEDIFF(
        SECOND,
        TRY_CONVERT(
          DATETIME,
          CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
        ),
        TRY_CONVERT(
          DATETIME,
          CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
        )
      )
    ) <= 300 THEN 1
    ELSE 0
  END AS IsResponTimeLessEqual5Minute,
  -- Summary untuk footer
  COUNT(*) OVER () AS JumlahKasus,
  SUM(
    CASE
      WHEN lkcb02.QuestionAnswerText IS NOT NULL
      AND lkcb02.QuestionAnswerText <> ''
      AND lkcb20.QuestionAnswerText IS NOT NULL
      AND lkcb20.QuestionAnswerText <> ''
      AND TRY_CONVERT(
        DATETIME,
        CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
      ) IS NOT NULL
      AND TRY_CONVERT(
        DATETIME,
        CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
      ) IS NOT NULL
      AND ABS(
        DATEDIFF(
          SECOND,
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
          ),
          TRY_CONVERT(
            DATETIME,
            CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
          )
        )
      ) <= 300 THEN 1
      ELSE 0
    END
  ) OVER () AS JumlahKasusLe5Menit,
  -- Persentase
  CAST(
    CAST(
      ROUND(
        100.0 * SUM(
          CASE
            WHEN lkcb02.QuestionAnswerText IS NOT NULL
            AND lkcb02.QuestionAnswerText <> ''
            AND lkcb20.QuestionAnswerText IS NOT NULL
            AND lkcb20.QuestionAnswerText <> ''
            AND TRY_CONVERT(
              DATETIME,
              CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
            ) IS NOT NULL
            AND TRY_CONVERT(
              DATETIME,
              CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
            ) IS NOT NULL
            AND ABS(
              DATEDIFF(
                SECOND,
                TRY_CONVERT(
                  DATETIME,
                  CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb02.QuestionAnswerText
                ),
                TRY_CONVERT(
                  DATETIME,
                  CONVERT(VARCHAR(10), phr.RecordDate, 120) + ' ' + lkcb20.QuestionAnswerText
                )
              )
            ) <= 300 THEN 1
            ELSE 0
          END
        ) OVER () / NULLIF(COUNT(*) OVER (), 0),
        2
      ) AS DECIMAL(10, 2)
    ) AS VARCHAR(20)
  ) + '%' AS Persentase
FROM
  PatientHealthRecord phr
  JOIN Registration reg ON reg.RegistrationNo = phr.RegistrationNo
  JOIN Patient pat ON pat.PatientID = reg.PatientID
  LEFT JOIN ServiceUnit su ON su.ServiceUnitID = phr.ServiceUnitID
  LEFT JOIN PatientHealthRecordLine lkcb02 ON lkcb02.TransactionNo = phr.TransactionNo
  AND lkcb02.QuestionID = 'LKCB02'
  LEFT JOIN PatientHealthRecordLine lkcb26 ON lkcb26.TransactionNo = phr.TransactionNo
  AND lkcb26.QuestionID = 'LKCB26'
  LEFT JOIN PatientHealthRecordLine lkcb20 ON lkcb20.TransactionNo = phr.TransactionNo
  AND lkcb20.QuestionID = 'LKCB20'
  LEFT JOIN PatientHealthRecordLine lkcb08 ON lkcb08.TransactionNo = phr.TransactionNo
  AND lkcb08.QuestionID = 'LKCB08'
  LEFT JOIN PatientHealthRecordLine lkcb103 ON lkcb103.TransactionNo = phr.TransactionNo
  AND lkcb103.QuestionID = 'LKCB103'
WHERE
  phr.QuestionFormID = 'LKCB'
  AND phr.RecordDate >= @p_FromDate
  AND phr.RecordDate < DATEADD(DAY, 1, CONVERT(DATE, @p_ToDate))
ORDER BY
  phr.RecordDate,
  phr.RecordTime;

END

GO
