# Bed Monitoring MJKN Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Membangun halaman display Kiosk baru `BedMonitoringDisplayMjkn.aspx` untuk menampilkan ketersediaan tempat tidur rawat inap berdasarkan pengelompokan kelas BPJS / Mobile JKN (6 panel) dengan auto-pagination dan real-time background sync.

**Architecture:** Menambahkan WebMethod `BedMonitoringMjknGetList` di ASMX Web Service yang mengeksekusi query kustom `InpatientBedAvailabilityCustom()`. Di sisi frontend, halaman ASP.NET Web Forms menggunakan master page `MasterBootstrap.Master` dengan grid Bootstrap 4 (2 baris x 3 kolom), auto-pagination client-side 6 baris per halaman berotasi tiap 8–10 detik, dan Ajax background polling tiap 30 detik.

**Tech Stack:** ASP.NET Web Forms (.NET Framework 4.6.1), C#, ASMX Web Services, JavaScript, jQuery, Bootstrap 4.

## Global Constraints
- Target Framework: .NET Framework 4.6.1.
- Format file project MSBuild klasik (`.csproj`). File baru wajib didaftarkan di `Temiang.Avicenna.csproj`.
- Jangan memodifikasi file pada folder `Temiang.Avicenna.BusinessObject/Generated/`.
- Jangan menuliskan data sensitif (kredensial, IP publik, token, atau data medis pasien).

---

### Task 1: Tambahkan WebMethod BedMonitoringMjknGetList di KioskQueue.asmx.cs

**Files:**
- Modify: `Temiang.Avicenna/WebService/KioskQueue.asmx.cs:650-658`

**Interfaces:**
- Consumes: `ServiceUnitCollection.InpatientBedAvailabilityCustom()` dari `Temiang.Avicenna.BusinessObject.Custom`
- Produces: WebMethod endpoint `/WebService/KioskQueue.asmx/BedMonitoringMjknGetList` yang mengembalikan JSON data ketersediaan kamar.

- [ ] **Step 1: Buka dan periksa lokasi penambahan WebMethod di `Temiang.Avicenna/WebService/KioskQueue.asmx.cs`**
Periksa region `Bed Monitoring` pada baris 641–658 di mana method `BedMonitoringGetList` dan `BedMonitoringGetListV2` berada.

- [ ] **Step 2: Tambahkan method `BedMonitoringMjknGetList`**
Tambahkan kode berikut tepat di bawah `BedMonitoringGetListV2`:
```csharp
        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public void BedMonitoringMjknGetList()
        {
            var dtb = (new ServiceUnitCollection()).InpatientBedAvailabilityCustom();
            Context.Response.Write(JSonRetFormatted(ConvertDataTabletoObject(dtb)));
        }
```

- [ ] **Step 3: Verifikasi sintaks dan integritas file**
Pastikan kurung kurawal pembuka/penutup pada class `KioskQueue` tetap valid dan tidak ada sintaks yang rusak.

- [ ] **Step 4: Commit perubahan Task 1**
```bash
git add Temiang.Avicenna/WebService/KioskQueue.asmx.cs
git commit -m "feat(kiosk): add BedMonitoringMjknGetList web method"
```

---

### Task 2: Buat Code-Behind dan Designer untuk BedMonitoringDisplayMjkn

**Files:**
- Create: `Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx.cs`
- Create: `Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx.designer.cs`

**Interfaces:**
- Consumes: `Temiang.Avicenna.BusinessObject.Common.Brand`
- Produces: Class `Temiang.Avicenna.Module.Kiosk.BedMonitoringDisplayMjkn : System.Web.UI.Page` dengan helper `GetBrandName()`

- [ ] **Step 1: Buat file code-behind `BedMonitoringDisplayMjkn.aspx.cs`**
Tuliskan kode berikut:
```csharp
using System;
using System.Web.UI;
using Temiang.Avicenna.BusinessObject.Common;

namespace Temiang.Avicenna.Module.Kiosk
{
    public partial class BedMonitoringDisplayMjkn : Page
    {
        protected void Page_PreInit(object sender, EventArgs e)
        {
            Page.Theme = "";
        }

        protected void Page_Init(object sender, EventArgs e)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string GetBrandName()
        {
            var b = new Brand();
            return b.Name;
        }
    }
}
```

- [ ] **Step 2: Buat file designer `BedMonitoringDisplayMjkn.aspx.designer.cs`**
Tuliskan kode berikut:
```csharp
namespace Temiang.Avicenna.Module.Kiosk
{
    public partial class BedMonitoringDisplayMjkn
    {
        /// <summary>
        /// Form1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// </remarks>
        protected global::System.Web.UI.HtmlControls.HtmlForm Form1;
    }
}
```

- [ ] **Step 3: Commit perubahan Task 2**
```bash
git add Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx.cs Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx.designer.cs
git commit -m "feat(kiosk): add code-behind and designer for BedMonitoringDisplayMjkn"
```

---

### Task 3: Buat Markup dan Logika Client-Side BedMonitoringDisplayMjkn.aspx

**Files:**
- Create: `Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx`

**Interfaces:**
- Consumes: Endpoint `/WebService/KioskQueue.asmx/BedMonitoringMjknGetList`
- Produces: Antarmuka display Kiosk lengkap (2 baris x 3 kolom) dengan auto-pagination per panel dan background polling.

- [ ] **Step 1: Buat file `BedMonitoringDisplayMjkn.aspx`**
Isi file mencakup:
1. Directive ASP.NET dengan `MasterPageFile="~/MasterPage/MasterBootstrap.Master"`.
2. Styling CSS untuk layout 6 panel responsif, font ukuran besar ramah layar TV/Kiosk, dan indikator warna merah (`rowRed`) untuk kamar penuh (`Ready == 0`).
3. Markup 6 panel:
   - Panel 1: `KELAS I` (`KL1`)
   - Panel 2: `KELAS II` (`KL2`)
   - Panel 3: `KELAS III` (`KL3`)
   - Panel 4: `KELAS VIP` (`VIP`)
   - Panel 5: `KELAS VVIP` (`VVP`)
   - Panel 6: `INTENSIVE CARE` (`ICU`, `HCU`, `NIC`, `PIC`)
4. JavaScript client-side:
   - Pengelompokan data per kategori dari respon `BedMonitoringMjknGetList`.
   - Perhitungan `Occupied = Math.max(0, Capacity - Available)` dan `Ready = Available`.
   - Auto-pagination 6 baris per halaman dengan interval rotasi 8–10 detik.
   - Paginasi mandiri per panel (panel dengan $\le 6$ baris tetap di halaman 1, panel $> 6$ baris berputar mulus).
   - Polling latar belakang tiap 30 detik untuk memperbarui data tanpa kedipan layar.

- [ ] **Step 2: Verifikasi struktur HTML, CSS, dan JavaScript**
Pastikan tidak ada tag unclosed, selector ID dan class konsisten, dan data mapping sesuai spek.

- [ ] **Step 3: Commit perubahan Task 3**
```bash
git add Temiang.Avicenna/Module/Kiosk/BedMonitoringDisplayMjkn.aspx
git commit -m "feat(kiosk): add BedMonitoringDisplayMjkn.aspx markup and client-side logic"
```

---

### Task 4: Daftarkan File Baru di Temiang.Avicenna.csproj dan Verifikasi Akhir

**Files:**
- Modify: `Temiang.Avicenna/Temiang.Avicenna.csproj`

**Interfaces:**
- Consumes: File-file baru dari Task 2 dan Task 3
- Produces: Project file terkonfigurasi untuk mengikutsertakan `BedMonitoringDisplayMjkn.aspx`, `.cs`, dan `.designer.cs` dalam build MSBuild.

- [ ] **Step 1: Buka `Temiang.Avicenna/Temiang.Avicenna.csproj` dan temukan referensi sibling `BedMonitoringDisplayNonPandemi.aspx`**
Lokasi tag `<Content>` ada di sekitar baris 2831, dan tag `<Compile>` ada di sekitar baris 13586.

- [ ] **Step 2: Tambahkan pendaftaran `<Content>` dan `<Compile>` untuk `BedMonitoringDisplayMjkn`**
1. Tambahkan di blok `<Content>`:
```xml
    <Content Include="Module\Kiosk\BedMonitoringDisplayMjkn.aspx" />
```
2. Tambahkan di blok `<Compile>`:
```xml
    <Compile Include="Module\Kiosk\BedMonitoringDisplayMjkn.aspx.cs">
      <DependentUpon>BedMonitoringDisplayMjkn.aspx</DependentUpon>
      <SubType>ASPXCodeBehind</SubType>
    </Compile>
    <Compile Include="Module\Kiosk\BedMonitoringDisplayMjkn.aspx.designer.cs">
      <DependentUpon>BedMonitoringDisplayMjkn.aspx</DependentUpon>
    </Compile>
```

- [ ] **Step 3: Verifikasi XML project file**
Pastikan format XML valid dan tidak merusak node XML sekitarnya.

- [ ] **Step 4: Commit perubahan Task 4**
```bash
git add Temiang.Avicenna/Temiang.Avicenna.csproj
git commit -m "build: register BedMonitoringDisplayMjkn in Temiang.Avicenna.csproj"
```
