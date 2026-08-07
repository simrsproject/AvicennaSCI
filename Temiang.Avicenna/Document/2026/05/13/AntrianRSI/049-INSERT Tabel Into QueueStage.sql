CREATE TABLE QueueStage
(
    StageID       VARCHAR(50)  NOT NULL,
    StageName     VARCHAR(200) NOT NULL,
    ServiceGroup  VARCHAR(100) NOT NULL,
    StepOrder     INT          NOT NULL,
    IsQueue       BIT          NOT NULL,
    IsActive      BIT          NOT NULL,

    CONSTRAINT PK_QueueStage PRIMARY KEY (StageID)
);
GO

INSERT INTO QueueStage
(
    StageID,
    StageName,
    ServiceGroup,
    StepOrder,
    IsQueue,
    IsActive
)
VALUES
('CTSCAN_AMBIL',         'Pengambilan Hasil CT Scan',       'CT SCAN',     2, 1, 1),
('CTSCAN_VERIF',         'Verifikasi CT Scan',              'CT SCAN',     1, 1, 1),

('ENDOSCOPY_AMBIL',      'Pengambilan Hasil Endoscopy',     'ENDOSCOPY',   2, 1, 1),
('ENDOSCOPY_VERIF',      'Verifikasi Endoscopy',            'ENDOSCOPY',   1, 1, 1),

('FARMASI_AMBIL',        'Pengambilan Obat',                'FARMASI',     2, 1, 1),
('FARMASI_VERIF',        'Verifikasi Farmasi',              'FARMASI',     1, 1, 1),

('HEMODIALISA_TINDAKAN', 'Tindakan Hemodialisa',            'HEMODIALISA', 2, 1, 1),
('HEMODIALISA_VERIF',    'Verifikasi Hemodialisa',          'HEMODIALISA', 1, 1, 1),

('LAB_SAMPLE',           'Pengambilan Sample Lab',          'LAB',         2, 1, 1),
('LAB_VERIF',            'Verifikasi Laboratorium',         'LAB',         1, 1, 1),

('LOKET',                'Loket Pendaftaran',              'REG',         1, 1, 1),

('POLI',                 'Pelayanan Poliklinik',           'POLI',        1, 1, 1),

('RADILOGI_AMBIL_FOTO',  'Pengambilan Foto Rongten',       'RADIOLOGI',   2, 1, 1),
('RADIOLOGI_VERIF',      'Verifikasi Radiologi',           'RADIOLOGI',   1, 1, 1),

('REHAB_TINDAKAN',       'Tindakan Rehabilitasi Medis',    'REHAB',       1, 1, 1),

('USG_TINDAKAN',         'Tindakan USG',                   'USG',         2, 1, 1),
('USG_VERIF',            'Verifikasi USG',                 'USG',         1, 1, 1);
GO