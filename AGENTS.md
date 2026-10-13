# Avicenna repository notes

Catatan kerja lokal untuk membantu menemukan bagian kode yang tepat saat mengerjakan task berikutnya. Informasi struktur diverifikasi dari solution dan project yang ada; cek kembali file terkait karena repo legacy ini terus berubah.

## Gambaran umum

- Avicenna adalah SIMRS (sistem informasi manajemen rumah sakit).
- Solution utama: `Temiang.Avicenna.sln`.
- Mayoritas project menggunakan C# dan project file MSBuild klasik/non-SDK (`.csproj`). Target framework yang tercantum pada project-project solution saat catatan ini dibuat adalah **.NET Framework 4.6.1**. `README.md` masih menyebut 4.5; gunakan `.csproj` sebagai acuan teknis dan perbarui dokumentasi hanya bila memang diminta.
- Aplikasi utama adalah ASP.NET berbasis Web Forms, dengan bagian MVC 5, Web API, dan layanan ASMX.
- README menyebut Microsoft SQL Server. Aplikasi juga memakai Telerik UI/Reporting dan library internal/third-party yang direferensikan dari `Reference/`, `lib/`, `packages/`, atau folder project.

## Project dalam solution

| Project | Peran utama yang terlihat dari nama dan struktur |
| --- | --- |
| `Temiang.Avicenna` | Web application utama: halaman/modul SIMRS, UI, controller, API, serta integrasi yang digunakan aplikasi. |
| `Temiang.Avicenna.BusinessObject` | Business object dan komponen data/domain bersama. |
| `Temiang.Avicenna.Common` | Utilitas dan komponen bersama; mencakup helper serta beberapa area integrasi. |
| `Temiang.Avicenna.Bridging.PCare` | Library bridging BPJS PCare. |
| `Temiang.Avicenna.Bridging.SatuSehat` | Library bridging SatuSehat. |
| `Temiang.Avicenna.ReportDataSource` | Komponen sumber data report. |
| `Temiang.Avicenna.ReportLibrary` | Report standar, termasuk `RLib_Rpt` dan `RLib_Slip`. |
| `Temiang.Avicenna.ReportLibrary.Custom` | Report dan resource custom, termasuk struktur per fasilitas/tenant. |
| `Temiang.Avicenna.Tests` | Test MSTest; saat ini mencakup area bridging, query, print, dan SatuSehat. |

Project utama memiliki `ProjectReference` ke BusinessObject, Common, bridging PCare/SatuSehat, dan ReportLibrary. Periksa `.csproj` jika perlu memahami dependency lebih lanjut.

## Peta folder web application

Di dalam `Temiang.Avicenna/`:

- `Module/` — halaman Web Forms yang dikelompokkan menurut area bisnis. Area utama: `Ambulance`, `AssetManagement`, `BloodBank`, `Charges`, `ControlPanel`, `CRM`, `Cssd`, `Finance`, `HR`, `Inventory`, `Kiosk`, `Laundry`, `Nutrient`, `Payroll`, `RADT`, dan `Reports`.
- `Controllers/` dan `Views/` — bagian MVC; view dikelompokkan menurut controller/fitur.
- `Bridging/Controllers/` — controller Web API untuk endpoint bridging.
- `WebService/` — layanan ASMX untuk fungsi seperti registrasi, mobile, antrean, dan integrasi eksternal.
- `MasterPage/` — master page dan layout Web Forms.
- `CustomControl/`, `SearchPopUp/`, `PCareCommon/` — kontrol dan halaman pendukung yang digunakan lintas modul.
- `Scripts/`, `JavaScript/`, `Bootstrap/`, `content/`, `App_Themes/`, `Images/`, `Resources/` — aset UI dan resource.
- `App_Data/`, `App_Document/`, `Document/` — data/dokumen aplikasi dan file pendukung; periksa isi serta konfigurasi pemakaian sebelum mengubahnya.
- `Web.config`, `Global.asax`, `RouteConfig.cs` — konfigurasi aplikasi, startup, dan routing.

### Peta area `Module`

Subfolder di bawah area menunjukkan pengelompokan fitur; beberapa area memakai kategori umum `Master`/`Transaction`, sementara yang lain memakai nama proses bisnis yang lebih spesifik. Contoh orientasi:

- `Inventory/` — `Information`, `Master`, `Procurement`, `Stock`, `Warehouse`.
- `Finance/` — antara lain `Master`, `Tariff`, `Receivable`, `Payable`, `CashManagement`, `BKU`, dan `Voucher`.
- `HR/` — antara lain `EmployeeHR`, `Credential`, `Leave`, `TrainingHR`, `Appraisal`, dan `Permission`.
- `RADT/` — area klinis dan rekam medis yang luas; antara lain `Registration`, `OutPatient`, `InPatient`, `Emergency`, `Cpoe`, `MedicalRecord`, `Phr`, `PharmaceuticalCare`, `TestResult`, `Bpjs`, dan `SatuSehat`. Sebagian area memiliki subfolder lanjutan.
- `Charges/` — `Billing`, `Cashier`, `Dispensary`, `ServiceUnit`.
- `ControlPanel/` — `Admin`, `AuditLog`, `Integration`, `PrinterManagement`, `Process`, `Setting`.
- `AssetManagement/` — `Management`, `Master`, `Process`; `Ambulance/`, `Cssd/`, dan `Laundry/` memiliki pengelompokan `Master` dan/atau `Transaction`.
- `BloodBank/` mengelompokkan fungsi seperti `BloodRequest`, `BloodReceived`, `Stock`, dan `TransfusionMonitoring`; `Reports/` berisi viewer/opsi report.

Daftar ini hanya peta awal untuk pencarian. Untuk fitur yang sedang dikerjakan, telusuri struktur aktual di bawah area tersebut dan gunakan sibling yang paling mirip sebagai contoh.

Di project lain, struktur penting yang mudah ditelusuri:

- `Temiang.Avicenna.BusinessObject/Base`, `Common`, `Custom`, `Mpi`, `Util` — kelompok komponen BusinessObject.
- `Temiang.Avicenna.BusinessObject/Generated/` — seluruh file di sini dihasilkan oleh Business Object Generator yang dijalankan di luar aplikasi/repo ini. Perlakukan sebagai output generator: jangan mengeditnya langsung; cari dan ubah sumber atau konfigurasi generator di luar repo, lalu generate ulang sesuai prosedur tim.
- `Temiang.Avicenna.Common/` memiliki folder area/helper integrasi, antara lain `BPJS`, `Disdukcapil`, `Inacbg`, `LinkLis`, dan `Sisrute`.
- `Temiang.Avicenna.ReportLibrary/RLib_Rpt/` dan `RLib_Slip/` — report dan slip standar.
- `Temiang.Avicenna.ReportLibrary.Custom/ReportXML/` serta folder custom per tenant (contoh `RSMP/`) — definisi report/resource custom. Ikuti struktur report sejenis yang sudah ada.
- `Reference/`, `lib/`, dan `packages/` — assembly/referensi dependency; project lama dapat bergantung pada versi atau path lokal tertentu.

## Cara menelusuri dan mengubah kode

1. Identifikasi fitur melalui folder `Module/<Area>/...`, lalu ikuti pasangan file halaman dan code-behind yang ada. Contoh pola list/detail: `Module/ControlPanel/Setting/TransactionCode/TransactionCodeList.aspx(.cs)` dan `TransactionCodeDetail.aspx(.cs)`.
2. Halaman Web Forms sering mewarisi base class seperti `BasePage`, `BasePageList`, `BasePageDetail`, atau `BasePageDialog`. Periksa base class dan halaman sejenis sebelum mengubah lifecycle, binding, validasi, atau penyimpanan.
3. Untuk endpoint, bedakan MVC di `Controllers/`, Web API di `Bridging/Controllers/`, dan ASMX di `WebService/`. Gunakan pola di endpoint sejenis dan cek routing/konfigurasi terkait.
4. Perubahan domain/data dapat melibatkan BusinessObject, Common, DAL, web application, dan report sekaligus. Telusuri pemanggil serta project reference sebelum menentukan cakupan perubahan.
5. Project `.csproj` yang lama umumnya mencantumkan file secara eksplisit. Saat menambah file, periksa apakah file perlu didaftarkan di `.csproj` agar ikut build.
6. File `.Designer.cs` dan file hasil generate/resource sebaiknya diubah lewat pola/tool yang sesuai; hindari edit manual yang dapat tertimpa generator.
7. Jangan menyalin credential, connection string, token, atau data pasien ke catatan, log, test, maupun commit. `Web.config` dan file konfigurasi lokal dapat memuat nilai sensitif—jangan menuliskannya ke output.

Untuk menelusuri alur halaman, gunakan `Module/ControlPanel/Setting/TransactionCode/` sebagai salah satu contoh yang sudah ada: file `.aspx` memasangkan markup dengan code-behind, halaman list mengarahkan ke search/detail dan mengisi grid, sedangkan detail menangani operasi entity. Pada contoh ini list/detail memakai `BasePageList`/`BasePageDetail`, mengatur `ProgramID`, dan berinteraksi dengan BusinessObject/query. Pola ini membantu orientasi, tetapi tidak semua halaman mengikuti triplet list/search/detail atau implementasi yang sama; periksa master page, base class, routing, dan halaman sejenis pada fitur target.

Khusus `Temiang.Avicenna.BusinessObject/Generated/`, semua file adalah hasil generator Business Object eksternal. Jangan memperlakukan file tersebut sebagai source of truth atau mengeditnya langsung; perubahan harus dilakukan pada sumber generator di luar aplikasi ini dan diikuti proses generate ulang.

## Build dan test

- Buka/build solution `Temiang.Avicenna.sln` dengan Visual Studio atau MSBuild yang mendukung project .NET Framework klasik.
- Target framework yang tercantum adalah .NET Framework 4.6.1; hasil build juga bergantung pada dependency yang direferensikan dari `Reference/`, `lib/`, `packages/`, serta instalasi tooling tertentu (termasuk Telerik Reporting).
- Test berada di `Temiang.Avicenna.Tests` dan menggunakan MSTest. Jalankan test yang relevan setelah perubahan; untuk perubahan lintas project, build project/solution yang terdampak jika environment memiliki dependency yang diperlukan.
- Tidak terlihat script build/test tingkat root. Periksa konfigurasi Visual Studio dan dependency lokal bila build tidak tersedia di environment.

## Batasan catatan ini

`AGENTS.md` sengaja dikecualikan dari Git sesuai permintaan. File ini hanya tersedia di working copy ini dan tidak akan ikut ke clone/checkout lain. Perbarui catatan jika struktur project berubah secara signifikan.
