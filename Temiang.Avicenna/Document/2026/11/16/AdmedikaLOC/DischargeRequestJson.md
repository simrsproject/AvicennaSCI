# Snapshot request discharge

Jalankan `02.DischargeRequestJson.sql` sebelum deploy aplikasi. Kolom baru
`AdmedikaClaimTransaction.DischargeRequestJson` bertipe `nvarchar(max) NULL`.
BusinessObject sudah memiliki property DischargeRequestJson beserta metadata,
query item dan string accessor. Penyimpanan menggunakan entity.Save().
Saat generator tim dijalankan ulang, pastikan kolom baru tersedia pada sumber
schema generator agar property dan metadata ini tetap dihasilkan.

Format versi 1 menyimpan `schemaVersion`, `capturedAt`, `clID`, `request`,
`billingItems` dan `benefits`. Request berisi data bisnis discharge; billingItems
menyimpan identitas sumber/nomor transaksi/sequence dan benID agar item dengan
kode sama pada transaksi berbeda tidak tertukar. Tidak ada token atau security
word. Data ini mengandung informasi pasien dan mengikuti akses database claim.

Snapshot disimpan setelah validasi ICD berhasil dan sebelum DISCHARGE_OP.
Kegagalan penyimpanan menghentikan pengiriman. Snapshot merupakan percobaan
terakhir yang disiapkan untuk dikirim, bukan bukti bahwa AdMedika menerimanya;
status dan hasil transaksi tetap menjadi acuan keberhasilan.
Retry dapat mengganti snapshot sebelum claim selesai. Riwayat seluruh attempt
memerlukan tabel terpisah jika kelak dibutuhkan.

Claim selesai menampilkan medical/billing/mapping dari snapshot dalam keadaan
read-only. Claim lama tanpa JSON tidak direkonstruksi dari billing terkini/log;
detail medical dan billing disembunyikan dengan keterangan belum tersedia.
Tidak ada backfill otomatis dan tidak perlu mengirim ulang claim sukses.

Uji pada claim UAT baru: proses discharge, tutup dialog, buka View; diagnosis,
flags, remarks, jumlah, nominal dan pilihan benefit harus tetap sesuai request.
Perubahan billing sumber sesudahnya tidak boleh mengubah tampilan snapshot.
Uji claim lama tanpa JSON: ringkasan hasil dan Print LOC tetap tersedia tanpa
menampilkan nilai default sebagai riwayat yang seolah-olah tersimpan.
