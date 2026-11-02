namespace Temiang.Avicenna.Common.Admedika
{
    public static class AdmedikaConstant
    {
        public static class ServiceID
        {
            public const string HelloWorld = "HELLO_WORLD";
            public const string CheckIcdExclusion = "CHECK_ICD_EXCLUSION";
            public const string Eligibility = "ELIGIBILITY";
            public const string GetEntitlement = "GET_ENTITLEMENT";
            public const string DischargeOp = "DISCHARGE_OP";
            public const string CheckMemberClaimTxnStatus = "CHECK_MEMBER_CLAIM_TXN_STATUS";
            public const string GetMemberEnrolledPlanTc = "GET_MEMBER_ENROLLED_PLAN_TC";
            public const string CancelOpenClaimsTxn = "CANCEL_OPEN_CLAIMS_TXN";
            public const string DailyMonitoring = "DAILY_MONITORING";
            public const string EdocsUpload = "EDOCS_UPLOAD";
            public const string InvoiceTracking = "INVOICE_TRACKING";
            public const string RePrintOut = "RE_PRINT_OUT";
        }

        public static class CoverageID
        {
            public const string HNS = "01"; // Rawat Inap
            public const string GP = "02"; // Rawat Jalan
            public const string SP = "03"; // Pre & Post
            public const string OP = "04"; // Kacamata
            public const string DENTAL = "05"; // Rawat Gigi
            public const string MAT = "06"; // Persalinan
            public const string LAB = "07"; // Laboratory
            public const string PHAR = "08"; // Pharmacy/Apotek
        }

        public static class ClaimStatusID
        {
            public const string EligibilityApproved = "08"; // Eligibility Approved
            public const string ClaimsRejected = "22"; // Claims rejected
            public const string ClaimsOnHold = "39"; // Claims on hold
            public const string ValidClaims = "40"; // Valid claims
            public const string IncompleteDocuments = "41"; // Incomplete documents
            public const string ReimbursementInitialStatus = "42"; // Reimbursement/Claims transaction initial status
            public const string ReimbursementPending = "43"; // Reimbursement/Claims transaction pending
            public const string ReimbursementDenied = "44"; // Reimbursement/Claims transaction denied
        }

        public static class DailyMonitoringFormID
        {
            public const string DCM001A = "001-A"; // Usage : DCM - Instruksi
            public const string DCM002A = "002-A"; // Usage : DCM - Notes / Keterangan
            public const string DCM003A = "003-A"; // Usage : DCM - Nama Ruang Perawatan
            public const string DCM003B = "003-B"; // Usage : DCM - Kelas Perawatan
            public const string DCM003C = "003-C"; // Usage : DCM - Informasi Kamar Perawatan (Naik APS/penuh/toleransi SK)
            public const string DCM003D = "003-D"; // Usage : DCM - Harga Kamar Perawatan
            public const string DCM003E = "003-E"; // Usage : DCM - Harga Kamar Perawatan Terdekat dari Hak Kamar Ditempati
            public const string DCM004A = "004-A"; // Usage : DCM - AL, SPR, GL, CL, Persetujuan tindakan, DL
            public const string DCM005A = "005-A"; // Usage : DCM - Indemnity Form
            public const string DCM006A = "006-A"; // Usage : DCM - Perkiraan Biaya
            public const string DCM007A = "007-A"; // Usage : DCM - Dr. yang merawat / dokter DPJP
            public const string DCM007B = "007-B"; // Usage : DCM - Dokter rawat bersama
            public const string DCM007C = "007-C"; // Usage : DCM - Dokter Konsulen
            public const string DCM008A = "008-A"; // Usage : DCM - Petugas RS (R. Perawatan), No Telp / HP Kontak
            public const string DCM008B = "008-B"; // Usage : DCM - Petugas RS (Admin) No Telp / HP Kontak
            public const string DCM008C = "008-C"; // Usage : DCM - Nama Penanggung Jawab Pasien
            public const string DCM008D = "008-D"; // Usage : DCM - Alamat Penanggung Jawab Pasien
            public const string DCM008E = "008-E"; // Usage : DCM - Nomor Telphone Penanggung Jawab Pasien
            public const string DCM008F = "008-F"; // Usage : DCM - Hubungan Penanggung Jawab Pasien dengan Pasien
            public const string DCM009A = "009-A"; // Usage : DCM - DIAGNOSA RIWAYAT PENYAKIT / RAWAT INAP TERDAHULU
            public const string DCM010A = "010-A"; // Usage : DCM - Anamnesa Medis
            public const string DCM011A = "011-A"; // Usage : DCM - Tanda Vital K/U
            public const string DCM011B = "011-B"; // Usage : DCM - Tanda Vital Nadi
            public const string DCM011C = "011-C"; // Usage : DCM - Tanda Vital Pernapasan
            public const string DCM011D = "011-D"; // Usage : DCM - Tanda Vital Suhu Tubuh
            public const string DCM011E = "011-E"; // Usage : DCM - Tanda Vital Cor / Pumo
            public const string DCM011F = "011-F"; // Usage : DCM - Pemeriksaan Penunjang RADIOLOGI
            public const string DCM011G = "011-G"; // Usage : DCM - Pemeriksaan Fisik (Abdomen, Tumor/kelainan kulit: regio, ukuran ?)
            public const string DCM011H = "011-H"; // Usage : DCM - Pemeriksaan Penunjang Laboratorium & PATOLOGI ANATOMI
            public const string DCM012A = "012-A"; // Usage : DCM - Diagnosa Primer
            public const string DCM012B = "012-B"; // Usage : DCM - Diagnosa Sekunder
            public const string DCM012C = "012-C"; // Usage : DCM - Diagnosa Ketiga
            public const string DCM013A = "013-A"; // Usage : DCM - Theraphy / pengobatan
            public const string DCM013B = "013-B"; // Usage : DCM - Rencana Treatment / Tindakan
            public const string DCM013C = "013-C"; // Usage : DCM - Jenis / Nama Operasi
            public const string DCM013D = "013-D"; // Usage : DCM - Laporan Operasi
            public const string DCM013E = "013-E"; // Usage : DCM - Kode ICD9-CM Rencana Treatment / Tindakan
            public const string DCM013F = "013-F"; // Usage : DCM - Kode ICD9-CM Jenis / Nama Operasi
            public const string DCM014A = "014-A"; // Usage : DCM - Pemeriksaan Penunjang Medis
            public const string DCM014B = "014-B"; // Usage : DCM - Indikasi Rawat Inap (Bila tidak kuat investigasi ke dr.)
            public const string DCM015A = "015-A"; // Usage : DCM - Nomor Kartu BPJS
            public const string DCM015B = "015-B"; // Usage : DCM - Nomor SEP Rawat Inap
            public const string DCM015C = "015-C"; // Usage : DCM - Asal Rujukan Rawat Inap
            public const string DCM015D = "015-D"; // Usage : DCM - Dokter yang merujuk
            public const string DCM015E = "015-E"; // Usage : DCM - Alasan dirujuk
            public const string DCM016A = "016-A"; // Usage : DCM - Jaminan Asuransi lain
            public const string DCM017A = "017-A"; // Usage : DCM - Nama & jumlah Alat Bantu (Kruk, Tongkat, Korset, dll)
        }
    }
}
