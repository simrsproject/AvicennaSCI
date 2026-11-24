# Tilaka Quick Sign — Verification Checklist (Phase 2)

Checklist ini dipakai **setelah** `tilaka-lite` dan kredensial Quick Sign tersedia.
Tanpa `tilaka-lite`, halaman dan service dapat diuji dengan build + unit test, tetapi
langkah di bawah belum bisa dijalankan.

## 1. Prasyarat

- [ ] `TilakaLiteBaseUrl` terisi dan dapat diakses dari server Avicenna.
- [ ] `TilakaLiteAuthPath` sesuai (default `/auth`).
- [ ] Channel id/secret Quick Sign valid.
- [ ] Folder `C:\Tilaka\TemplateSign` dan `C:\Tilaka\Signed` dibuat; app pool punya Modify.
- [ ] SQL `07.CREATE_TABLE_TilakaTemplateSignFile.sql` dan `08.INSERT_INTO_AppProgram_TilakaQuickSign.sql` sudah dijalankan.
- [ ] Kolom `TilakaQuickSignRequest.TemplateSignFileID` ada di database.
- [ ] Doctor uji: `Paramedic.IsUsingESignature = 1`, sertifikat `Aktif`, `TilakaName` terisi, `AppUser.SignatureImage` tidak kosong.
- [ ] Data `TilakaCasemixFile` uji sudah tersedia (diisi di luar modul).
- [ ] Ada satu dokter uji dengan `IsUsingESignature = 0` untuk memverifikasi tombol Process Signature disabled dan penolakan backend.

## 2. Onboarding Quick Sign

- [ ] Buka **Tilaka E-Signature → Quick Sign User** (`01.23.04`).
- [ ] Hanya dokter dengan `IsUsingESignature = 1` yang tampil di daftar.
- [ ] Filter “Belum terdaftar”; tombol **Daftarkan Quick Sign** muncul untuk dokter bersertifikat aktif.
- [ ] Klik tombol → `addquicksignusers` dipanggil; status berubah menjadi “Terdaftar” dan tanggal terisi.
- [ ] Bila email sudah terdaftar, aplikasi tetap menandai terdaftar (idempoten).

## 3. Template

- [ ] Buka **Template Tanda Tangan** (`01.23.05`).
- [ ] Isi ID dokter, unggah PDF, klik **Upload**.
- [ ] `TemplateNo` terbentuk `TTD-{ParamedicID}-01`; file ada di `C:\Tilaka\TemplateSign\{ParamedicID}\`.
- [ ] Klik **Daftarkan** → `quicksign-addtemplates`; `IsRegistered = 1`, `RegisteredDateTime` terisi.
- [ ] Template pertama otomatis `IsDefault = 1`; template berikutnya tidak menimpa default.
- [ ] Peringatan “auth session batal” tampil sebelum pendaftaran.

## 4. Process Signature

- [ ] Buka **File Casemix** (`01.23.02`); kolom RegistrationNo, ParamedicID, FileName, FilePath tampil.
- [ ] Tombol **Process Signature** aktif hanya untuk file tanpa request atau request terakhir `Failed`, dan hanya bila dokter `IsUsingESignature = 1`.
- [ ] Untuk dokter dengan `IsUsingESignature = 0`, file tetap tampil tetapi tombol disabled dan request langsung ditolak bila dipaksa.
- [ ] Klik tombol untuk file uji.
- [ ] Request dibuat dengan `request_id`, satu signer (DPJP), posisi 1/1/1/1/1.
- [ ] Halaman status menampilkan status per signer dan tautan **Buka Auth** bila `UNAUTHORIZED`.
- [ ] `TilakaCasemixFile.LastQuickSignRequestID` terisi.
- [ ] Dua klik bersamaan tidak menghasilkan dua request aktif (filtered unique index).

## 5. Auth dan signing

- [ ] Buka auth URL; login Tilaka + OTP/FR.
- [ ] Kembali ke halaman status; klik **Cek Status**.
- [ ] Status signer berubah `PROCESS` → `DONE`; request menjadi `Completed`.
- [ ] Callback `POST /TilakaCallback/Signing` diterima dan idempoten (kirim dua kali tidak mengubah hasil).
- [ ] PDF hasil tersimpan di `C:\Tilaka\Signed\{RequestID}\` dan dapat diunduh lewat handler.
- [ ] Bila presigned URL kedaluwarsa, `GetPresignedUrl` dipakai dan unduhan tetap berhasil.

## 6. Uji negatif

- [ ] File Casemix non-PDF ditolak sebelum upload.
- [ ] Dokter tanpa sertifikat aktif / bukan Quick Sign user / tanpa `SignatureImage` ditolak.
- [ ] Template belum terdaftar → Process Signature ditolak.
- [ ] Callback unsigned ditolak bila `TilakaQuickSignAllowUnsignedCallback=false`.
- [ ] `TilakaLiteBaseUrl` kosong → halaman menampilkan pesan konfigurasi, tidak error.
- [ ] Status `DONE` tidak bisa turun menjadi `PROCESS`/`FAILED`.
- [ ] Unduh gagal → request tetap `InProgress`, bukan `Completed`.

## 7. Rekonsiliasi

- [ ] Panggil `TilakaWS.asmx/ReconcileSigning` dengan access key valid.
- [ ] Ringkasan `{ checked, changed, failed, messages }` masuk akal.
- [ ] Request yang sudah final tidak diproses lagi (`IsActive = 0`).
