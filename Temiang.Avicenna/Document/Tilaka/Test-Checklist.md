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

### 0.1 Kunci konfigurasi `Web.config` (appSettings, 16 kunci)

| Kunci | Nilai untuk uji | Catatan |
|---|---|---|
| `TilakaBaseUrl` | `https://sb-api.tilaka.id` | Default sandbox. |
| `TilakaAuthPath` | `/auth` | |
| `TilakaChannelId` | `<channel-id-sandbox>` | Wajib diisi; dipakai juga sebagai `client_id` HMAC callback. |
| `TilakaChannelSecret` | `<channel-secret-sandbox>` | Wajib diisi; **jangan** ditempel ke dokumen/commit. |
| `TilakaChannelType` | `Company` | Harus sesuai tipe channel yang terdaftar di Tilaka. |
| `TilakaTimeoutSeconds` | `60` | |
| `TilakaIsLogEnabled` | `true` | Logging ke `WebServiceAPILog`. |
| `TilakaCallbackToleranceMinutes` | `5` | Toleransi `x-request-timestamp`; jam server harus sinkron (NTP). |
| `TilakaRegistrationExpireDays` | `7` | Nilai > 7 otomatis dibatasi 7. |
| `TilakaConsentTextFile` | `C:\Tilaka\TilakaConsent.txt` | File teks TnC, UTF-8. |
| `TilakaConsentVersion` | `TNT-v.1.0.1` | Harus sama dengan versi TnC yang disepakati; ikut di-hash saat registrasi. |
| `TilakaUploadKTPImage` | `C:\Tilaka\KTP` | Root penyimpanan foto KTP. |
| `TilakaMaxKtpSizeKb` | `1024` | Batas ukuran **setelah kompresi** (1 MB). Kontrol upload sendiri menerima ≤ 2 MB (`MaxFileSize=2097152`). |
| `TilakaWebviewRedirectUrl` | `https://<host>/TilakaWebview/Return` | Halaman pendaratan setelah webview Tilaka (MVC). |
| `TilakaClientCertificatePath` | _(kosong)_ | Isi hanya bila Tilaka mewajibkan mTLS. |
| `TilakaClientCertificatePassword` | _(kosong)_ | |

- [ ] Ke-16 kunci di atas terisi dan `TilakaChannelId`/`TilakaChannelSecret` adalah milik channel sandbox.
- [ ] **Allowlist IP ada di sisi Tilaka**, bukan di aplikasi: daftarkan IP publik server Avicenna ke
      Tilaka agar API mereka menerima request kita. Autentikasi masuk tetap memakai HMAC
      `x-validation-token` + channel secret. Tidak ada key `TilakaCallbackAllowedIps` di aplikasi.
- [ ] `TilakaWebviewRedirectUrl` menunjuk ke host uji (bukan `domain-rs.example`), memakai HTTPS.
- [ ] Minimal satu dokter uji memiliki `Paramedic.IsUsingESignature = 1`; dokter tanpa flag ini
      tidak muncul di daftar registrasi (kecuali sudah punya riwayat registrasi) dan tidak bisa didaftarkan.

### 0.2 Folder server dan ACL

- [ ] Folder `C:\Tilaka\` dibuat.
- [ ] File `C:\Tilaka\TilakaConsent.txt` dibuat (UTF-8, teks TnC lengkap, tidak kosong).
- [ ] Folder `C:\Tilaka\KTP` dibuat (aplikasi membuat subfolder `{ParamedicID}` otomatis).
- [ ] Identitas aplikasi (IIS App Pool, mis. `IIS APPPOOL\<AppPoolName>`) diberi:
  - **Read** pada `C:\Tilaka\TilakaConsent.txt` (disarankan hanya Read; tidak perlu Write).
  - **Modify** pada `C:\Tilaka\KTP` (dan pewarisan ke subfolder).
- [ ] `C:\Tilaka` **tidak** diekspos sebagai static file oleh IIS; foto KTP hanya dilayani lewat
      `~/Module/RADT/Tilaka/TilakaKtpImageHandler.ashx?id=<TilakaRegistrationID>`.
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
- [ ] Redirect webview: `https://<host>/TilakaWebview/Return`

Catatan redirect: `TilakaUrls` mengirim `redirect_url` **persis seperti nilai konfigurasi**, tanpa
menambahkan `request-id`. Menurut PRD Tilaka §6.3, Tilaka yang menambahkan parameter `request-id`
(dan `tilaka-name`) ke URL redirect tersebut saat webview selesai. Halaman
`TilakaWebview/Return` mewajibkan `?request-id=`; bila parameter tidak ada, halaman menampilkan
"Parameter request-id tidak ditemukan." — dalam kasus itu gunakan tombol **Cek Status** di halaman
detail (lihat 5.1).

Catatan callback: endpoint callback harus dapat diakses publik via HTTPS, tanpa autentikasi form/cookie,
dan mengembalikan HTTP 200. Callback selalu dibalas `OK` (termasuk saat signature ditolak) agar Tilaka
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
- [ ] Catat bahwa **`/auth` tidak menulis log**: `Service.GetAccessToken` tidak memakai
      `ITilakaLogger`, jadi jangan mencari baris operasi auth di `WebServiceAPILog`. Verifikasi
      logging dilakukan pada langkah berikutnya lewat operasi `GenerateUUID`, `RegisterForKycCheck`,
      atau `CheckCertStatus`.

---

## 2. Verifikasi registrasi

### 2.0 Prasyarat data dokter uji

`TilakaRegistrationGuard.Check` memblokir pendaftaran bila data master tidak lengkap. Siapkan satu
dokter uji (bukan data pasien/dokter nyata) dengan kondisi berikut:

- [ ] `Paramedic.NamaKTP` terisi (nama sesuai KTP; dipakai untuk `GenerateUuid`).
- [ ] `Paramedic.Ssn` berisi NIK **tepat 16 digit angka** dan **tidak dipakai dokter lain**. Cek dengan:

```sql
SELECT ParamedicID, ParamedicName, NamaKTP, Ssn, Email
FROM Paramedic
WHERE Ssn = '<NIK>';
```

  Expected: hanya satu baris, yaitu dokter uji. Lebih dari satu baris akan memicu
  "NIK sudah dipakai dokter lain." (guard membandingkan lewat `IsNikUsedByOther`).
- [ ] `Paramedic.Email` terisi dan valid.
- [ ] Tersedia file foto KTP dokter uji (JPEG/PNG) untuk diunggah.
- [ ] Checkbox TnC tersedia: state dokter `BelumTerdaftar` **dan** `TilakaConsentTextFile` ada/terbaca.

Bila salah satu tidak terpenuhi, tombol **Daftar** menghasilkan pesan guard berikut dan tidak ada
record yang dibuat: "Nama sesuai KTP belum diisi.", "NIK (SSN) harus tepat 16 digit angka.",
"NIK sudah dipakai dokter lain.", "Email dokter belum diisi.", "Foto KTP belum diunggah.", atau
"Persetujuan TnC Tilaka belum dicentang.".

### 2.1 Alur UI

Buka menu **Tilaka E-Signature → Monitoring & Registrasi Sertifikat Dokter**
(`~/Module/RADT/Tilaka/TilakaRegistrationList.aspx`), cari dokter uji dari 2.0, lalu klik **Detail**.

- [ ] Pilih dokter uji yang **belum** punya baris `TilakaRegistration` (`IsLatest = 1`).
- [ ] Unggah foto KTP (JPEG/PNG) pada kontrol upload. Kontrol menerima file ≤ 2 MB
      (`MaxFileSize=2097152`); hasil kompresi harus ≤ `TilakaMaxKtpSizeKb` (1024 KB).
      Sebelum **Daftar**, yang terlihat hanya nama file pada kontrol upload — belum ada pratinjau
      dan belum ada file di `C:\Tilaka\KTP` (file baru ditulis di dalam `RegisterRegistration()`).
- [ ] Centang **"Dokter telah membaca dan menyetujui TnC Tilaka."**.
      Catatan: link **Lihat TnC** hanya tampil bila `TilakaConsentTextFile` berawalan `~/`
      (`ConfigureConsent`). Dengan `C:\Tilaka\TilakaConsent.txt` link tersebut **tersembunyi**;
      keberadaan/isi TnC diverifikasi tidak langsung dari keberhasilan **Daftar** (file dibaca ulang
      saat submit) atau dengan membuka file di server.
- [ ] Klik **Daftar**.
      Expected: pesan `UUID Tilaka berhasil dibuat. Lanjutkan dengan Kirim Data KYC.`
- [ ] **Setelah Daftar**, pratinjau KTP muncul (`imgKtp` baru terikat saat record tersimpan punya
      `KtpFilePath`) dan file ada di `C:\Tilaka\KTP\<ParamedicID>\<ssn>_yyyyMMddHHmmss.jpg`.
      Bila pratinjau tidak muncul, buka
      `~/Module/RADT/Tilaka/TilakaKtpImageHandler.ashx?id=<TilakaRegistrationID>`.
- [ ] Klik **Kirim Data KYC**.
      Expected: pesan `Data KYC terkirim. Silakan buka webview registrasi.`
- [ ] Klik **Buka Webview**, lalu selesaikan OTP + liveness + pembuatan Tilaka Name di webview Tilaka.
- [ ] Setelah webview selesai, kembali ke Avicenna; gunakan **Cek Status** bila status belum berubah
      (dan bila halaman return melaporkan `request-id` tidak ditemukan — lihat 5.1).

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
- [ ] `LastCheckSource` **NULL** sampai pemeriksaan pertama (Cek Status/callback/rekonsiliasi);
      setelah itu bernilai `manual`, `callback`, atau `reconcile`. `NULL` bukan kegagalan.
- [ ] `Paramedic.NamaKTP` pada dokter uji sesuai nama di KTP (dipakai untuk pencocokan).
- [ ] `TilakaRegistration` lama untuk dokter yang sama sudah `IsLatest = 0` (bila ada pendaftaran ulang).
- [ ] Logging API tercatat (bukan `/auth`) dengan operasi `GenerateUUID`, `RegisterForKycCheck`, dan
      `CheckCertStatus`:

```sql
SELECT TOP (20) DateRequest, UrlAddress, Params, Response
FROM WebServiceAPILog
WHERE IPAddress = 'TILAKA'
ORDER BY DateRequest DESC;
```

---

## 3. Verifikasi callback dengan HMAC lokal

### 3.1 Callback certificate status

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

### 3.2 Callback registrasi

Payload registrasi memakai key `RegisterID` (huruf besar) yang berisi `RegistrationRequestID`.
Jalankan blok berikut **sekaligus** setelah mengganti placeholder:

```powershell
$ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$body = '{"RegisterID":"<request-id>","success":true,"message":"OK","data":{"status":"B","tilaka_name":"<tilaka-name>","manual_registration_status":"V"}}'
$key = [Text.Encoding]::UTF8.GetBytes($env:TILAKA_SECRET)
$msg = [Text.Encoding]::UTF8.GetBytes("$env:TILAKA_ID$ts$body")
$hmac = [System.Security.Cryptography.HMACSHA256]::new($key)
$token = ($hmac.ComputeHash($msg) | ForEach-Object { $_.ToString("x2") }) -join ""
Invoke-WebRequest -Method Post -Uri "https://<host>/TilakaCallback/Registration" -Body $body `
  -ContentType "application/json" -Headers @{ "x-request-timestamp" = $ts; "x-validation-token" = $token }
```

- [ ] Respons HTTP `200` dan body `OK`.
- [ ] `KycStatus` menjadi `B` (atau nilai `data.status` yang dikirim).
- [ ] `ManualRegistrationStatus` menjadi `V`, `TilakaName` terisi dari `data.tilaka_name`.
- [ ] `LastCheckSource` menjadi `callback`; `RegistrationState` menjadi `MenungguVerifikasi`.
- [ ] `WebServiceAPILog` berisi `Callback-Registration` dengan respons `applied`.
- [ ] Bila `RegisterID` tidak dikenal, tidak ada perubahan data dan log berisi `Callback-Registration`
      dengan respons `register-id-unknown` (tetap HTTP 200).

Catatan: callback certificate status (3.1) dan registrasi (3.2) boleh diuji dalam urutan mana pun;
processor menangani callback yang datang lebih dulu tanpa menghapus field KYC yang sudah ada.

---

## 4. Verifikasi negatif (signature tidak valid)

- [ ] Ulangi langkah 3.1 dengan `x-validation-token` acak, misalnya:

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
- [ ] Kembali ke halaman return. `TilakaUrls` mengirim `redirect_url` tanpa `request-id`; menurut
      PRD Tilaka §6.3 Tilaka menambahkan `request-id`/`tilaka-name` ke URL tersebut. Bila halaman
      menampilkan `Parameter request-id tidak ditemukan.` atau
      `Nama Tilaka belum tersedia. Silakan gunakan tombol Cek Status di Avicenna.`, gunakan tombol
      **Cek Status** pada halaman detail sebagai fallback.
- [ ] Bila redirect membawa `request-id`, halaman `TilakaWebview/Return?request-id=<request-id>`
      menampilkan `Aktivasi selesai. Status: Aktif`.
- [ ] `CertificateStatus` = `3`, `RegistrationState` = `Aktif`, `LastCheckSource` = `callback`
      (atau `manual` bila aktivasi diverifikasi lewat **Cek Status**).

### 5.2 Rekonsiliasi massal (ASMX)

Gunakan `accessKey` valid dari tabel `WebServiceAccessKey` (tanggal mulai/berakhir mencakup hari ini).
`max` dibatasi 50; nilai ≤ 0 atau > 50 menjadi 10.

- [ ] Jalankan:

```powershell
$body = @{ accessKey = "<key>"; max = 5 } | ConvertTo-Json
Invoke-RestMethod -Method Post -Uri "https://<host>/WebService/TilakaWS.asmx/ReconcilePending" `
  -ContentType "application/json" -Body $body
```

- [ ] Respons sukses dan memuat field `checked`, `changed`, `failed`, `messages`.
- [ ] `checked` ≤ `max`.
- [ ] **Jangan** mengandalkan `checked = changed + failed`: pada `TilakaWS.asmx.cs`, kegagalan
      `checkcertstatus` menambah `failed` lalu tetap mengeksekusi `changed++` untuk record yang sama,
      sehingga bisa muncul `checked = 1, changed = 1, failed = 1`. Gunakan `messages` sebagai sinyal
      kegagalan, bukan selisih counter.
- [ ] Bila `messages` berisi kegagalan `checkcertstatus`, tidak ada kolom `LastErrorMessage` yang
      menjamin isi pesan tersebut (jalur ini hanya menambah `messages`); periksa `messages` dan
      `CertificateStatus` record terkait. `LastErrorMessage` hanya diisi oleh kegagalan
      `userregstatus` dan exception.
- [ ] Setelah rekonsiliasi, `LastCheckSource` = `reconcile` dan
      `LastCheckedDateTime`/`RegistrationState` diperbarui untuk record yang diperiksa (termasuk
      record yang gagal `checkcertstatus`, yang tetap disimpan tanpa `LastErrorMessage`).

Catatan: respons ASMX dapat dibungkus `{"d":"{...}"}` (string JSON). Bila demikian, parse `d`
sekali lagi dengan `ConvertFrom-Json` sebelum membaca field.

### 5.3 Alternatif dari UI

- [ ] Tombol **Cek Status Massal** di halaman list memproses maksimal 5 record per klik dan
      menulis `LastCheckSource = 'manual'` (bukan `reconcile`).
      Expected: pesan `<n> diperiksa (maksimal 5 per klik), <m> gagal. Grid sudah diperbarui.`

---

## 6. Verifikasi tambahan yang disarankan

- [ ] Login sebagai user tanpa hak ProgramID `01.23.01` → menu tidak muncul / akses ditolak.
- [ ] **(Best-effort)** Unggah file bukan gambar (mis. PDF) → ditolak dengan pesan "Foto KTP harus
      berupa gambar JPEG atau PNG." Kontrol `RadAsyncUpload` sudah memfilter ekstensi di sisi klien
      (`.jpg,.jpeg,.png`), jadi penolakan server sulit dipicu lewat UI normal; uji hanya bila bisa
      mengirim file dengan `Content-Type`/ekstensi menyimpang.
- [ ] **(Best-effort)** Unggah gambar yang setelah kompresi masih > 1024 KB → ditolak dengan pesan
      ukuran melebihi batas. Kompresi (resize maks 1600 px + JPEG) membuat ini sulit dipicu; gunakan
      gambar sangat detail mendekati 2 MB.
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
- Rekonsiliasi massal via `TilakaWS.asmx` belum punya penjadwalan otomatis; harus dipanggil manual
  (tombol UI atau endpoint ASMX).
