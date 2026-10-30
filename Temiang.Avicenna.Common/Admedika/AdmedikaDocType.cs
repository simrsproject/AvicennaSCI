using System;
using System.Collections.Generic;
using System.Linq;

namespace Temiang.Avicenna.Common.Admedika
{
    public sealed class AdmedikaDocType
    {
        public const string GroupEForms = "eFORMS";
        public const string GroupMandatoryEdocs = "MandatoryEdocs";

        public string Code { get; private set; }
        public string Description { get; private set; }
        public string Group { get; private set; }

        private AdmedikaDocType(string code, string description, string group)
        {
            Code = code;
            Description = description;
            Group = group;
        }

        private static readonly AdmedikaDocType[] Items =
        {
            new AdmedikaDocType("LMA", "Laporan Medis Awal", GroupEForms),
            new AdmedikaDocType("FTDF", "Fast Track Discharge Form", GroupEForms),
            new AdmedikaDocType("AL", "Surat Pernyataan (Authorization Letter)", GroupEForms),
            new AdmedikaDocType("CL", "Surat Konfirmasi Medis (Perkembangan, Lanjutan & Konsul)", GroupEForms),
            new AdmedikaDocType("AP", "Formulir Penjaminan Biaya (Approved Procedure Form)", GroupEForms),
            new AdmedikaDocType("PREADMISSION", "Surat Pre-admission (Pre-Authorization Form)", GroupEForms),
            new AdmedikaDocType("GL", "Surat Jaminan Biaya (Guarantee Letter)", GroupEForms),
            new AdmedikaDocType("DL", "Surat Penolakan Jaminan Biaya (Decline Letter)", GroupEForms),

            new AdmedikaDocType("ADCARD", "Kartu Asuransi (AdMedika Card)", GroupMandatoryEdocs),
            new AdmedikaDocType("IDCARD", "Kartu Identitas (Identification Card)", GroupMandatoryEdocs),
            new AdmedikaDocType("KRONO", "Surat Kronologis Kecelakaan", GroupMandatoryEdocs),
            new AdmedikaDocType("SUKETKMR", "Surat Keterangan Kelas Kamar", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTBEDAH", "Surat Persetujuan Tindakan", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTOBAT", "Surat Persetujuan Therapy", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTLAB", "Surat Persetujuan Tindakan (Laboratorium)", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTRAD", "Surat Persetujuan Tindakan (Radiology)", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTKONSUL", "Konsultasi Dokter (Surat Persetujuan Tindakan)", GroupMandatoryEdocs),
            new AdmedikaDocType("SPTFISIO", "Surat Persetujuan Tindakan (Fisiotherapy)", GroupMandatoryEdocs),
            new AdmedikaDocType("LAB", "Hasil Pemeriksaan Laboratorium", GroupMandatoryEdocs),
            new AdmedikaDocType("RADIOLOGY", "Hasil Pemeriksaan Radiology", GroupMandatoryEdocs),
            new AdmedikaDocType("RESUMEMEDIS", "Resume Medis", GroupMandatoryEdocs),
            new AdmedikaDocType("BILLSEM", "Billing Sementara - Perkiraan Biaya RI Sementara (Interim Bill)", GroupMandatoryEdocs),
            new AdmedikaDocType("BILLAKHIR", "Billing Akhir - Jaminan Akhir Rawat Inap (Final Bill)", GroupMandatoryEdocs),
            new AdmedikaDocType("LOA_LOC", "LOA / LOC (Printout Admedika)", GroupMandatoryEdocs),
            new AdmedikaDocType("FORM_CLAIM", "Formulir klaim - diisi lengkap oleh dokter", GroupMandatoryEdocs),
            new AdmedikaDocType("BIAYA_PENGOBATAN", "Perincian biaya - pengobatan", GroupMandatoryEdocs),
            new AdmedikaDocType("OBAT_RACIKAN", "Perincian obat obatan - termasuk racikan", GroupMandatoryEdocs),
            new AdmedikaDocType("OTHERS", "Other related documents", GroupMandatoryEdocs)
        };

        public static IReadOnlyList<AdmedikaDocType> All
        {
            get { return Items; }
        }

        public static bool IsValid(string code)
        {
            return Get(code) != null;
        }

        public static AdmedikaDocType Get(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            var normalized = code.Trim();
            return Items.FirstOrDefault(item =>
                string.Equals(item.Code, normalized, StringComparison.OrdinalIgnoreCase));
        }

        public static string AllowedText()
        {
            return string.Join(", ", Items.Select(item => item.Code));
        }
    }
}
