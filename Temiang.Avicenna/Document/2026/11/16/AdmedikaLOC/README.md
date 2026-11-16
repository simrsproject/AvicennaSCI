# Deployment Print LOC AdMedika

1. Jalankan `01.PrintLOC.sql` pada database aplikasi UAT. Pastikan ID report
   `XML.01.2102` belum digunakan oleh report lain sebelum deployment.
2. Salin `BillingCharges/PrintLOC_Admedika.trdx` ke folder `BillingCharges`
   di lokasi report yang dilayani oleh konfigurasi `ReportUrlLocation`.
   Ini lokasi report eksternal, bukan otomatis folder web application.
3. Deploy hasil build web application dan Common bersama-sama.
4. Buka claim yang sudah discharge dan memiliki PrintOutLOC, klik Print LOC
   dari dialog atau daftar. Halaman akan membuka ReportViewer dengan
   parameter ClaimID berisi ClID AdMedika.
5. Verifikasi pasien, kartu, Reference ID, isi PrintOutLOC, logo dan hasil
   cetak untuk respons panjang/multi-baris. Bandingkan Print LOA agar tetap
   menggunakan report XML.01.2101 dan template LOA sebelumnya.

Template LOC diduplikasi dari template LOA yang diberikan. Logo dan layout
dipertahankan; nama report, stored procedure dan field teks diubah menjadi LOC.
Contoh ClaimID pada konfigurasi desain sumber dihapus.
ReportViewer mengganti koneksi data sesuai mekanisme report existing.
Script dan file report disiapkan di repository; belum dijalankan/disalin ke
database atau server report oleh implementasi ini.
