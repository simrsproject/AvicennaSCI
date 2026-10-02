# CR EMR Correction, MDS, dan Attachment

## Ringkasan keputusan

CR ini tidak seluruhnya bisa selesai hanya dengan modifikasi UI. Ada bagian yang sudah punya fondasi existing, ada yang perlu tambahan struktur data supaya perilakunya akurat dan tidak merusak pemakaian lama.

## Existing yang sudah tersedia

1. Buka akses koreksi oleh Medrek
   - Existing sudah ada flag `Registration.IsOpenEntryMR`.
   - Menu buka/tutupnya ada di Close/Open Registration, dan validasi EMR existing sudah membaca flag ini lewat `IsMedicalRecordOpen` / `MedicalRecordValidate`.
   - Jadi konsep dari jawaban user "akses open dibukain dulu dari medrek" sudah sesuai dengan flow existing.

2. Audit trail
   - Entity yang turun dari `esEntityWAuditLog` sudah bisa menyimpan audit old/new value jika `AuditLogSetting` diaktifkan.
   - `RegistrationInfoMedic` sudah punya setting audit dari dokumen lama.
   - `PatientDocument` juga turun dari audit base, tetapi perlu dipastikan/ditambahkan `AuditLogSetting` agar perubahan attachment tercatat.

3. Attachment
   - List attachment EMR ada di `PatientDocumentHist.aspx`.
   - Form upload/edit ada di `PatientDocumentUpload.aspx.cs`.
   - Sebelumnya mode edit hanya mengubah keterangan; file upload disembunyikan saat edit.

## Implementasi yang sudah dilakukan

1. Tambah tombol Edit pada list Attachment EMR.
   - Tombol hanya muncul untuk user yang berada di group `RM.01` atau `RM.02`.
   - Ini mengikuti jawaban user bahwa Edit Attachment hanya untuk Rekam Medis level Manajer dan Pengatur.

2. Batasi save edit Attachment.
   - Walaupun user membuka URL edit langsung, save edit ditolak jika bukan `RM.01` / `RM.02`.

3. Edit Attachment bisa mengganti nama dokumen, notes, tanggal dokumen, dan isi file.
   - File lama tidak disimpan sebagai versi baru.
   - Sesuai konfirmasi user, file lama ditimpa/diganti dengan file yang benar dan file lama di-take down.
   - Jika file diganti dengan nama file berbeda, file lama dihapus setelah data baru berhasil tersimpan agar tidak menjadi file sampah.
   - Audit perubahan metadata/file name tetap mengandalkan `AuditLogSetting` untuk `PatientDocument`.

4. MDS rawat jalan dicatat relasinya ke SOAP/Assessment sumber.
   - Relasi dicatat ke tabel `MedicalDischargeSummarySource` jika tabel tersebut sudah ada.
   - Jika tabel belum ada, code tidak error dan flow lama tetap berjalan.
   - Saat SOAP/Assessment sumber dibatalkan dari grid EMR, MDS yang terkait ikut diset batal lewat relasi tersebut.

## Catatan database

### Attachment

Tidak perlu tabel khusus untuk menyimpan file lama. User sudah konfirmasi bahwa file lama cukup diganti dengan file yang benar dan file lama di-take down. Yang diperlukan:

1. Aktifkan audit untuk `PatientDocument`.
2. Audit akan menyimpan perubahan field seperti `DocumentName`, `FileAttachName`, `OriFileName`, `DocumentDate`, dan `Notes`.
3. Isi file lama tidak disimpan, hanya jejak nama file lama dan perubahan metadatanya.

### MDS rawat jalan ke SOAP sumber

Existing belum punya relasi yang aman antara MDS rawat jalan dan SOAP/Assessment sumber.

Saat auto-generate MDS rawat jalan, code existing mengisi `RegistrationInfoMedic.ReferenceNo` dengan `RegistrationNo`, bukan dengan `RegistrationInfoMedicID` SOAP sumber. Karena field ini sudah dipakai existing, sebaiknya tidak diubah langsung agar tidak breaking ke laporan/menu lain.

Relasi yang dipakai:

```text
MedicalDischargeSummarySource
- RegistrationNo
- MDSRegistrationInfoMedicID
- SourceRegistrationInfoMedicID
- SourceType
- IsDeleted
- CreatedDateTime
- CreatedByUserID
- LastUpdateDateTime
- LastUpdateByUserID
```

Dengan relasi ini, rule nomor 3 bisa akurat:

1. Kalau SOAP sumber MDS dibatalkan, MDS ikut dibatalkan.
2. Kalau ada beberapa SOAP, hanya MDS yang terkait SOAP tersebut yang ikut batal.
3. Tidak perlu menebak dari `RegistrationNo` saja.

### MDS IGD

Mapping dari user perlu implementasi di flow pembuatan MDS IGD. Bagian ini dipisah sebagai fase berikutnya setelah MDS rawat jalan dan Attachment sudah OK:

1. Jika tidak ada SOAP tambahan, MDS tetap ambil dari Emergency Unit Assessment.
2. Jika ada SOAP tambahan:
   - Keluhan Utama dan Riwayat Penyakit dari Emergency Unit Assessment.
   - Pemeriksaan Fisik, Diagnosa Utama, Diagnosa Sekunder dari SOAP dokter terakhir yang aktif.
   - Jika data poin tersebut kosong di SOAP terakhir, fallback ke Emergency Unit Assessment.
   - Terapi, Anjuran, Tindakan digabung dari Emergency Unit Assessment dan SOAP terakhir, exclude duplicate.
   - QR/sign dokter dari SOAP terakhir yang aktif.
3. Jika SOAP terakhir dibatalkan, MDS IGD mengambil SOAP aktif sebelumnya.

Bagian ini perlu perubahan mapping data di halaman MDS IGD, bukan hanya database.

## Breaking change risk

1. Mengubah arti `RegistrationInfoMedic.ReferenceNo` untuk MDS rawat jalan berisiko breaking, karena sekarang berisi `RegistrationNo`.
2. Menyimpan file attachment lama sebagai versi akan menambah storage dan perlu layar/history baru untuk membukanya.
3. Membuat "semua form EMR bisa dikoreksi" setelah pasien pindah ruangan perlu audit per form, karena setiap form punya validasi dan hak edit masing-masing. Fondasi `IsOpenEntryMR` sudah ada, tetapi tidak semua form pasti memakai pola yang sama.

## Pertanyaan tersisa ke user

1. Untuk semua form EMR, apakah tahap pertama boleh mengikuti form yang sudah memakai validasi Medical Record Open existing dulu, lalu form lain dicek bertahap?
2. Untuk MDS rawat jalan, apakah rumah sakit setuju tambah tabel relasi MDS ke SOAP sumber agar kasus beberapa SOAP bisa ditangani tepat?
