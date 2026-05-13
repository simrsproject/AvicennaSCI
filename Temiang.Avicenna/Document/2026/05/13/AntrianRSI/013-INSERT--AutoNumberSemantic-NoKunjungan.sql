INSERT INTO dbo.AntrianAutoNumberSemantic
(SRAutoNumber, PayerType, ServiceGroup, Channel, DisplayOrder, DisplayName)
VALUES

-- =========================
-- SYSTEM (tidak tampil di UI)
-- =========================
('ClosingVisitDpNo',        'UMUM',  'CLOSING',     'SYSTEM',     NULL, NULL),

-- =========================
-- LOKET PD (Pendaftaran)
-- =========================

-- BPJS
('VisitBpjsPoliNo',         'BPJS',  'POLI',        'LOKET_PD',   1, 'BPJS - POLI'),
('VisitBpjsHdNo',           'BPJS',  'HD',          'LOKET_PD',   2, 'BPJS - HD'),
('VisitBpjsIrmNo',          'BPJS',  'IRM',         'LOKET_PD',   3, 'BPJS - REHABILITASI MEDIS'),
('VisitBpjsPenunjangNo',    'BPJS',  'PENUNJANG',   'LOKET_PD',   4, 'BPJS - PENUNJANG'),

-- TUNAI & MITRA
('VisitTunaiNo',            'TUNAI', 'ALL',         'LOKET_PD',   5, 'TUNAI'),
('VisitMitraNo',            'MITRA', 'ALL',         'LOKET_PD',   6, 'MITRA'),

-- =========================
-- LOKET PM (Langsung pilih)
-- =========================

-- UMUM
('VisitCsNo',               'UMUM',  'CS',          'LOKET_PM',   1, 'CUSTOMER SERVICE'),

-- BPJS (SEMUA MUNCUL DI PM)
('VisitBpjsPoliNo',         'BPJS',  'POLI',        'LOKET_PM',   2, 'POLI BPJS'),
('VisitBpjsHdNo',           'BPJS',  'HD',          'LOKET_PM',   3, 'HD BPJS'),
('VisitBpjsIrmNo',          'BPJS',  'IRM',         'LOKET_PM',   4, 'REHAB MEDIS BPJS'),
('VisitBpjsPenunjangNo',    'BPJS',  'PENUNJANG',   'LOKET_PM',   5, 'PENUNJANG BPJS'),

-- TUNAI & MITRA (JUGA MUNCUL DI PM)
('VisitTunaiNo',            'TUNAI', 'ALL',         'LOKET_PM',   6, 'TUNAI'),
('VisitMitraNo',            'MITRA', 'ALL',         'LOKET_PM',   7, 'MITRA'),

-- =========================
-- FARMASI (HANYA DI SINI)
-- =========================
('VisitFarmasiApsNo',       'UMUM',  'FARMASI',     'FARMASI',    1, 'FARMASI');
