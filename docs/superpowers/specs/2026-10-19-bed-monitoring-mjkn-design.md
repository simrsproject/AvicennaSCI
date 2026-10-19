# Spesifikasi Desain: BedMonitoringDisplayMjkn.aspx

## 1. Ringkasan
Dokumen ini mendefinisikan desain implementasi halaman kiosk baru `BedMonitoringDisplayMjkn.aspx` untuk menampilkan ketersediaan tempat tidur rawat inap (Inpatient Bed Availability) berbasis pengelompokan kelas BPJS / Mobile JKN.

## 2. Arsitektur & Sumber Data

### 2.1 Backend / Web Service
- **File**: `Temiang.Avicenna/WebService/KioskQueue.asmx.cs`
- **WebMethod Baru**: `BedMonitoringMjknGetList()`
  - Memanggil `(new ServiceUnitCollection()).InpatientBedAvailabilityCustom()` dari `Temiang.Avicenna.BusinessObject/Custom/ServiceUnit.cs`.
  - Mengembalikan JSON hasil query dalam format standar respons KioskQueue.

### 2.2 Struktur Data Sumber
Kolom yang dihasilkan oleh query `InpatientBedAvailabilityCustom()`:
- `BpjsClassID` (string): Kode kelas BPJS atau penanda ruangan (`KL1`, `KL2`, `KL3`, `VIP`, `VVP`, `ICU`, `HCU`, `NIC`, `PIC`, `ISO`).
- `RoomID` (string): ID ruangan/kamar.
- `ServiceUnitName` (string): Nama ruangan.
- `Capacity` (int): Total kapasitas tempat tidur.
- `Available` (int): Jumlah tempat tidur siap/tersedia.

### 2.3 Pemetaan Kelas (6 Panel Display)
Tampilan dibagi menjadi 6 kelompok panel:
1. **KELAS I**: Ruangan dengan `BpjsClassID == 'KL1'`
2. **KELAS II**: Ruangan dengan `BpjsClassID == 'KL2'`
3. **KELAS III**: Ruangan dengan `BpjsClassID == 'KL3'`
4. **KELAS VIP**: Ruangan dengan `BpjsClassID == 'VIP'`
5. **KELAS VVIP**: Ruangan dengan `BpjsClassID == 'VVP'`
6. **INTENSIVE CARE**: Ruangan dengan `BpjsClassID` dalam `['ICU', 'HCU', 'NIC', 'PIC']` (termasuk unit intensif anak/bayi dan perantara)

Setiap baris data kamar menghitung:
- `Occupied = Math.max(0, Capacity - Available)`
- `Ready = Available`

## 3. Desain Antarmuka Pengguna (UI)

### 3.1 File Baru
1. `Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx`
2. `Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx.cs`
3. Registrasi file di file project `Temiang.Avicenna/Temiang.Avicenna.csproj`.

### 3.2 Layout Layar
- Menggunakan master page `~/MasterPage/MasterBootstrap.Master`.
- Mematikan theming ASP.NET standar (`EnableTheming="False"`).
- Collapse sidebar saat inisialisasi (`$('body').addClass('sidebar-collapse')`).
- Grid Bootstrap 4: 2 baris x 3 kolom:
  - **Baris 1**: KELAS I, KELAS II, KELAS III
  - **Baris 2**: KELAS VIP, KELAS VVIP, INTENSIVE CARE
- Tiap panel berisi Card dengan Header (nama kelas & indikator halaman `Hal X/Y`) dan Tabel (kolom: NAMA RUANGAN, OCCUPIED, READY).

### 3.3 Gaya Visual (Styling Kiosk)
- Font berukuran besar dan jelas untuk keterbacaan jarak jauh di layar monitor/TV Kiosk.
- Baris dengan `Ready == 0` (kamar penuh) diberi sorotan khusus (warna merah/danger subtle).
- Header panel memiliki warna tegas (primary/info/dark) untuk membedakan kategori kelas dengan jelas.

## 4. Mekanisme Paginasi & Sinkronisasi Data

### 4.1 Auto-Pagination (Rotasi Halaman)
- **Baris per Halaman**: Maksimal 6 baris per panel.
- **Interval Rotasi**: Setiap 8–10 detik.
- **Logika Rotasi**:
  - Panel dengan jumlah baris > 6 berpindah ke halaman berikutnya secara siklis: `currentPage = (currentPage + 1) % totalPages`.
  - Panel dengan jumlah baris $\le$ 6 tetap berada di halaman 1 (`Hal 1/1`).
  - Transisi pergantian halaman menggunakan animasi *fade-in* halus untuk menghindari kelelahan mata.

### 4.2 Background Polling
- Client-side memanggil `BedMonitoringMjknGetList` melalui Ajax setiap 30 detik.
- Hasil data memperbarui cache state di browser tanpa me-reload halaman atau mengganggu siklus animasi yang sedang berjalan.
- Jika terjadi error pada jaringan/server, data yang sudah tampil tetap dipertahankan dan background retry dilakukan di interval berikutnya.

## 5. Rencana Pengujian
Pengujian manual akan dilakukan oleh pengguna dengan mem-build dan mendeploy aplikasi langsung ke environment pengujian. Verifikasi kode mencakup:
- Sintaks markup ASP.NET Web Forms dan referensi code-behind.
- WebMethod di `KioskQueue.asmx.cs`.
- Pendaftaran file `.aspx` dan `.aspx.cs` di `Temiang.Avicenna.csproj`.
