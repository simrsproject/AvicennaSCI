# Tilaka Sign Plus — Checklist Verifikasi Sandbox

Dokumen ini adalah checklist manual untuk memverifikasi integrasi Tilaka (registrasi & aktivasi
sertifikat dokter) di lingkungan sandbox/staging. Tidak ada kode baru pada tahap ini; seluruh
langkah dijalankan oleh operator di server uji.

- Lingkungan: sandbox Tilaka (`https://sb-api.tilaka.id`) — **jangan** memakai channel produksi.
- Host uji: `https://<host>` (ganti dengan domain server uji yang dapat diakses internet).
- Database uji: jalankan skrip `Temiang.Avicenna/Document/2026/11/20/01.CREATE_TABLE_TilakaRegistration.sql`,
  `02.ALTER_TABLE_Paramedic.sql`, dan `03.INSERT_INTO_AppProgram_Tilaka.sql` secara berurutan.
- Entity `Temiang.Avicenna.BusinessObject/Generated/TilakaRegistration.cs` dan kolom
  `Paramedic.NamaKTP` disediakan oleh pemilik repo (tidak dibuat ulang di sini).

---

## 0. Prasyarat

### 0.1 Kunci konfigurasi `Web.config` (appSettings)

| Kunci | Nilai untuk uji | Catatan |
|---|---|---|
| `TilakaBaseUrl` | `https://sb-api.tilaka.id` | Default sandbox. |
| `TilakaAuthPath` | `/auth` | |
| `TilakaChannelId` | `<channel-id-sandbox>` | Wajib diisi; dipakai juga sebagai `client_id` HMAC callback. |
| `TilakaChannelSecret` | `<channel-secret-sandbox>` | Wajib diisi; **jangan** ditempel ke dokumen/commit. |
| `TilakaChannelType` | `Company` | Harus sesuai tipe channel yang terdaftar di Tilaka. |
| `TilakaTimeoutSeconds` | `60` | |
| `TilakaIsLogEnabled` | `true` | Logging ke `WebServiceAPILog`. |
| `TilakaCallbackBaseUrl` | `https://<host>` | Base URL callback (tanpa slash di akhir). |
| `TilakaCallbackToleranceMinutes` | `5` | Toleransi `x-request-timestamp`; jam server harus sinkron (NTP). |
| `TilakaCallbackAllowedIps` | `<ip-tilaka>` | Dicatat; belum ditegakkan oleh controller (lihat Known limitations). |
| `TilakaRegistrationExpireDays` | `7` | Nilai > 7 otomatis dibatasi 7. |
| `TilakaConsentTextFile` | `C:\Tilaka\TilakaConsent.txt` | File teks TnC, UTF-8. |
| `TilakaConsentVersion` | `TNT-v.1.0.1` | Harus sama dengan versi TnC yang disepakati; ikut di-hash saat registrasi. |
| `TilakaUploadKTPImage` | `C:\Tilaka\KTP` | Root penyimpanan foto KTP. |
| `TilakaMaxKtpSizeKb` | `1024` | Batas ukuran setelah kompresi. |
| `TilakaWebviewRedirectUrl` | `https://<host>/Module/RADT/Tilaka/TilakaWebviewReturn.aspx` | Harus halaman return yang ada. |
| `TilakaClientCertificatePath` | _(kosong)_ | Isi hanya bila Tilaka mewajibkan mTLS. |
| `TilakaClientCertificatePassword` | _(kosong)_ | |

- [ ] Semua kunci di atas terisi dan `TilakaChannelId`/`TilakaChannelSecret` adalah milik channel sandbox.
- [ ] `TilakaWebviewRedirectUrl` menunjuk ke host uji (bukan `domain-rs.example`), memakai HTTPS.

### 0.2 Folder server dan ACL

- [ ] Folder `C:\Tilaka\` dibuat.
- [ ] File `C:\Tilaka\TilakaConsent.txt` dibuat (UTF-8, teks TnC lengkap, tidak kosong).
- [ ] Folder `C:\Tilaka\KTP` dibuat (rekursif: aplikasi membuat subfolder `yyyy\MM\dd`).
- [ ] Identitas aplikasi (IIS App Pool, mis. `IIS APPPOOL\<AppPoolName>`) diberi:
  - **Read** pada `C:\Tilaka\TilakaConsent.txt` (disarankan hanya Read; tidak perlu Write).
  - **Modify** pada `C:\Tilaka\KTP` (dan pewarisan ke subfolder).
- [ ] `C:\Tilaka` **tidak** diekspos sebagai static file oleh IIS; foto KTP hanya dilayani lewat
      `TilakaKtpImageHandler.ashx?id=<TilakaRegistrationID>`.
- [ ] Backup/antivirus tidak mengunci folder KTP saat unggah (uji tulis file kecil).

Perkiraan ACL PowerShell (jalankan sebagai Administrator, sesuaikan nama App Pool):

```powershell
$acl = Get-Acl "C:\Tilaka\KTP"
$rule = New-Object System.Security.AccessControl.FileSystemAccessRule(
    "IIS APPPOOL\<AppPoolName>", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$acl.SetAccessRule($rule)
Set-Acl "C:\Tilaka\KTP" $acl

icacls "C:\Tilaka\TilakaConsent.txt" /grant "IIS APPPOOL\<AppPoolName>:(R)"
```

### 0.3 Callback dan redirect yang didaftarkan di portal Tilaka

- [ ] Callback registrasi: `https://<host>/TilakaCallback/Registration`
- [ ] Callback certificate status: `https://<host>/TilakaCallback/CertificateStatus`
- [ ] Redirect webview: `https://<host>/Module/RADT/Tilaka/TilakaWebviewReturn.aspx`

Catatan: endpoint callback harus dapat diakses publik via HTTPS, tanpa autentikasi form/cookie, dan
mengembalikan HTTP 200. Callback selalu dibalas `OK` (termasuk saat signature ditolak) agar Tilaka
tidak melakukan retry tanpa henti.

### 0.4 Variabel sesi PowerShell

Jalankan di PowerShell pada mesin yang dapat menjangkau host uji. Nilai tidak disimpan di file.

```powershell
$env:TILAKA_ID = "<channel-id-sandbox>"
$env:TILAKA_SECRET = "<channel-secret-sandbox>"
```

---

## 1. Verifikasi token (`/auth`)

- [ ] Jalankan:

```powershell
$body = @{ client_id = $env:TILAKA_ID; grant_type = "client_credentials"; client_secret = $env:TILAKA_SECRET }
Invoke-RestMethod -Method Post -Uri "https://sb-api.tilaka.id/auth" -Body $body
```

- [ ] `access_token` terisi (bukan null/kosong).
- [ ] `expires_in` bernilai `300`.
- [ ] Tidak ada error HTTP 401/403 (bila ada, periksa `TilakaChannelId`/`TilakaChannelSecret`).
- [ ] Catat waktu respons untuk perbandingan dengan log di `WebServiceAPILog` (operasi auth).

---

## 2. Verifikasi registrasi

### 2.1 Alur UI

Buka menu **Tilaka E-Signature → Monitoring & Registrasi Sertifikat Dokter**
(`~/Module/RADT/Tilaka/TilakaRegistrationList.aspx`), cari satu dokter uji, lalu klik **Detail**.

- [ ] Pilih dokter uji yang **belum** punya baris `TilakaRegistration` (`IsLatest = 1`).
- [ ] Unggah foto KTP (JPEG/PNG, ≤ 1024 KB) pada kontrol upload.
- [ ] Pratinjau KTP tampil dan file tersimpan di bawah `C:\Tilaka\KTP\yyyy\MM\dd\`.
- [ ] Centang **"Dokter telah membaca dan menyetujui TnC Tilaka."** (link **Lihat TnC** membuka file TnC).
- [ ] Klik **Daftar**.
      Expected: pesan `UUID Tilaka berhasil dibuat. Lanjutkan dengan Kirim Data KYC.`
- [ ] Klik **Kirim Data KYC**.
      Expected: pesan `Data KYC terkirim. Silakan buka webview registrasi.`
- [ ] Klik **Buka Webview**, lalu selesaikan OTP + liveness + pembuatan Tilaka Name di webview Tilaka.
- [ ] Setelah webview selesai, kembali ke Avicenna; gunakan **Cek Status** bila status belum berubah.

### 2.2 Verifikasi data

```sql
SELECT TOP (1) TilakaRegistrationID, RelatedID, RegistrationRequestID, KycStatus, KycReasonCode,
       TilakaName, CertificateStatus, RegistrationState, LastCheckSource, LastCheckedDateTime
FROM TilakaRegistration
WHERE RelatedType = 'Paramedic' AND RelatedID = '<ParamedicID>' AND IsLatest = 1
ORDER BY TilakaRegistrationID DESC;
```

- [ ] `RegistrationRequestID` terisi.
- [ ] `KycStatus` terisi (mis. `S`/`B` sesuai respons Tilaka).
- [ ] `TilakaName` terisi setelah webview selesai.
- [ ] `RegistrationState` menjadi `MenungguVerifikasi` atau `MenungguKyc`.
- [ ] `LastCheckSource` = `manual` (dari tombol **Cek Status**) atau `callback`.
- [ ] `Paramedic.NamaKTP` pada dokter uji sesuai nama di KTP (dipakai untuk pencocokan).
- [ ] `TilakaRegistration` lama untuk dokter yang sama sudah `IsLatest = 0` (bila ada pendaftaran ulang).

---

## 3. Verifikasi callback dengan HMAC lokal

Ganti `<tilaka-name>` dengan `TilakaName` dan `<request-id>` dengan `RegistrationRequestID` dari
langkah 2. Jalankan blok berikut **sekaligus** (timestamp dihitung saat itu juga; toleransi 5 menit).
`$body` harus dikirim persis seperti yang di-hash — jangan menambah spasi/newline.

- [ ] Jalankan:

```powershell
$ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$body = '{"user_identifier":"<tilaka-name>","success":true,"request_id":"<request-id>","status":2}'
$key = [Text.Encoding]::UTF8.GetBytes($env:TILAKA_SECRET)
$msg = [Text.Encoding]::UTF8.GetBytes("$env:TILAKA_ID$ts$body")
$hmac = [System.Security.Cryptography.HMACSHA256]::new($key)
$token = ($hmac.ComputeHash($msg) | ForEach-Object { $_.ToString("x2") }) -join ""
Invoke-WebRequest -Method Post -Uri "https://<host>/TilakaCallback/CertificateStatus" -Body $body `
  -ContentType "application/json" -Headers @{ "x-request-timestamp" = $ts; "x-validation-token" = $token }
```

- [ ] Respons HTTP `200` dan body `OK`.
- [ ] `CertificateStatus` pada baris `request_id` tersebut menjadi `2`.
- [ ] `LastCheckSource` menjadi `callback`.
- [ ] `RegistrationState` menjadi `SertifikatTerbit` (status sertifikat 2 = Registered).
- [ ] `LastCheckedDateTime` terisi.
- [ ] `WebServiceAPILog` bertambah dengan operasi `Callback-CertificateStatus` dan respons `applied`:

```sql
SELECT TOP (20) DateRequest, IPAddress, UrlAddress, Params, Response, Totalms
FROM WebServiceAPILog
WHERE IPAddress = 'TILAKA'
ORDER BY DateRequest DESC;
```

Catatan: timestamp memakai format `yyyy-MM-dd HH:mm:ss` waktu lokal server; selisih jam server uji
dan mesin operator > 5 menit akan menghasilkan `timestamp-out-of-range`.

---

## 4. Verifikasi negatif (signature tidak valid)

- [ ] Ulangi langkah 3 dengan `x-validation-token` acak, misalnya:

```powershell
$ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$body = '{"user_identifier":"<tilaka-name>","success":true,"request_id":"<request-id>","status":2}'
Invoke-WebRequest -Method Post -Uri "https://<host>/TilakaCallback/CertificateStatus" -Body $body `
  -ContentType "application/json" -Headers @{ "x-request-timestamp" = $ts; "x-validation-token" = "deadbeefdeadbeef" }
```

- [ ] Respons HTTP `200` dan body `OK` (tidak ada error 500/401).
- [ ] **Tidak ada** perubahan data: `CertificateStatus`, `LastCheckSource`, `LastCheckedDateTime`,
      dan `RegistrationState` tetap seperti sebelum request.
- [ ] `WebServiceAPILog` berisi baris baru dengan `UrlAddress = 'Callback-CertificateStatus-Rejected'`
      dan `Response = 'invalid-signature'`.

---

## 5. Verifikasi aktivasi dan rekonsiliasi

### 5.1 Aktivasi via webview

- [ ] Pada detail dokter, pastikan state `SertifikatTerbit` sehingga tombol **Buka Webview** membuka
      webview **aktivasi** (`ActivationWebview`, memakai `RegistrationRequestID`).
- [ ] Setujui sertifikat di webview Tilaka.
- [ ] Kembali ke halaman return `TilakaWebviewReturn.aspx?request-id=<request-id>`.
      Expected: pesan `Aktivasi selesai. Status: Aktif`.
- [ ] `CertificateStatus` = `3`, `RegistrationState` = `Aktif`, `LastCheckSource` = `callback`.

### 5.2 Rekonsiliasi massal (ASMX)

Gunakan `accessKey` valid dari tabel `WebServiceAccessKey` (tanggal mulai/berakhir mencakup hari ini).
`max` dibatasi 50; nilai ≤ 0 atau > 50 menjadi 10.

- [ ] Jalankan:

```powershell
Invoke-RestMethod -Method Post -Uri "https://<host>/WebService/TilakaWS.asmx/ReconcilePending" `
  -Body @{ accessKey = "<key>"; max = 5 }
```

- [ ] Respons sukses dan memuat field `checked`, `changed`, `failed`, `messages`.
- [ ] `checked` ≤ `max`; setiap record yang diperiksa menambah `changed` atau `failed`, sehingga
      `checked = changed + failed`.
- [ ] Bila `messages` berisi kegagalan, periksa `LastErrorMessage` pada baris terkait.
- [ ] Setelah rekonsiliasi, `LastCheckSource` = `reconcile` dan
      `LastCheckedDateTime`/`RegistrationState` diperbarui untuk record yang diperiksa.

Catatan: respons ASMX dapat dibungkus `{"d":"{...}"}` (string JSON). Bila demikian, parse `d`
sekali lagi dengan `ConvertFrom-Json` sebelum membaca field.

### 5.3 Alternatif dari UI

- [ ] Tombol **Cek Status Massal** di halaman list memproses maksimal 5 record per klik dan
      menulis `LastCheckSource = 'manual'` (bukan `reconcile`).
      Expected: pesan `<n> diperiksa (maksimal 5 per klik), <m> gagal. Grid sudah diperbarui.`

---

## 6. Verifikasi tambahan yang disarankan

- [ ] Login sebagai user tanpa hak ProgramID `01.23.01` → menu tidak muncul / akses ditolak.
- [ ] Unggah file bukan gambar (mis. PDF) → ditolak dengan pesan "Foto KTP harus berupa gambar JPEG atau PNG."
- [ ] Unggah gambar > 1024 KB setelah kompresi → ditolak dengan pesan ukuran melebihi batas.
- [ ] Hapus sementara `C:\Tilaka\TilakaConsent.txt` → tombol **Daftar** nonaktif dan pesan file TnC
      tidak ditemukan muncul (kembalikan file setelah uji).
- [ ] Callback dengan `status` dikirim sebagai string (`"2"`) tetap diterima (parser fleksibel).
- [ ] Callback `Registration` dengan signature valid mengisi field KYC dan `LastCheckSource = callback`.
- [ ] Record `MenungguKyc` yang `DateExpire` sudah lewat tampil `Kedaluwarsa` (uji dengan data uji).

---

## Known limitations / follow-ups

- **Belum ada UI revoke/re-enroll** pada fase ini. Pencabutan sertifikat dan pendaftaran ulang harus
  dilakukan manual di portal Tilaka; aplikasi hanya menampilkan state `Dicabut` bila datanya ada.
- **Quick Sign tidak termasuk** dalam fase ini (hanya registrasi & aktivasi regular).
- **WNA belum ada di UI**: form hanya memakai data KTP WNI (`NamaKTP`, NIK, foto KTP). Alur paspor/WNA
  belum disediakan.
- `Temiang.Avicenna.BusinessObject/Generated/TilakaRegistration.cs` dan kolom `Paramedic.NamaKTP`
  **disediakan oleh pemilik repo**; jangan mengedit `Generated/` secara manual.
- `TilakaCallbackAllowedIps` baru dibaca ke config dan **belum ditegakkan** di `TilakaCallbackController`;
  pembatasan IP saat ini mengandalkan firewall/reverse proxy.
- Rekonsiliasi massal via `TilakaWS.asmx` belum punya penjadwalan otomatis; harus dipanggil manual
  (tombol UI atau endpoint ASMX).
