# SRK rawat inap dikirim saat discharge

## Cakupan

- Khusus `HealthcareInitial = RSI`, registrasi rawat inap, penjamin dalam `GuarantorAskesID`, dan nomor SEP terisi. Ini mempertahankan batas cabang khusus SRK RSI yang sebelumnya ada di MDS.
- Simpan MDS DPJP maupun resume keperawatan menyimpan rencana kontrol, tanpa mengirim SRK atau membuat appointment untuk kelompok tersebut.
- Konfirmasi discharge mengambil rencana kontrol MDS; perawat dapat melengkapi tanggal, poli, dan dokter bila belum lengkap.
- Pasien tetap mengambil appointment melalui MJKN. Jalur SRK ini tidak otomatis membuat appointment/Antrol.
- Pasien meninggal mengikuti pengecualian kontrol yang sudah ada. Rencana kontrol tidak diwajibkan untuk seluruh pasien: form kosong tidak mengirim SRK.

## Penyimpanan dan kompatibilitas

- Tidak ada penambahan tabel, kolom, parameter, atau perubahan file hasil generator.
- Rencana kontrol tetap disimpan pada `MedicalDischargeSummaryByNurse.ControlPlan`.
- JSON `Items` tetap berisi rencana bertanggal. Rencana dokter yang poli/dokternya lengkap tetapi tanggalnya kosong disimpan pada properti JSON opsional `UndatedItems`; properti ini dihilangkan bila tidak ada isinya. Konsumen lama yang membaca `Items` tidak menerima tanggal minimum tahun 0001 sebagai jadwal kontrol.
- Form discharge menggunakan kembali kontrol Follow Up Plan (maksimal lima baris), tanpa mengubah primary key tabel appointment lama.
- Setelah discharge berhasil, rencana kontrol lengkap yang diperbarui perawat disimpan kembali pada `MedicalDischargeSummaryByNurse.ControlPlan`. Saat MDS dibuka kembali, dokter dapat melihat tanggal kontrol yang diisi perawat.
- Perubahan tanggal/poli/dokter oleh perawat baru tersimpan bersama penyelesaian discharge. Jika pengiriman gagal, isian tetap ada di form untuk dicoba kembali.
- Jalur rawat jalan, non-BPJS, fasilitas lain, serta cabang Casemix tetap menggunakan proses sebelumnya. Kasus tanpa SEP belum masuk jalur baru ini.
- Deploy BusinessObject, Common, dan web application bersama agar model JSON dan pemanggilan helper/overload tersedia pada versi yang sama.
- Pemulihan versi lama: rencana bertanggal tetap kompatibel, tetapi versi lama tidak menampilkan `UndatedItems`. Lengkapi rencana yang masih belum bertanggal sebelum rollback atau lakukan rekonsiliasi manual.

## Validasi dan kegagalan

- Baris yang mulai diisi harus lengkap tanggal, poli, dan dokter. Tanggal harus setelah hari ini dan tanggal pulang.
- Mapping poli/dokter BPJS diperiksa sebelum proses discharge dijalankan.
- Nomor kartu BPJS harus tersedia agar pemeriksaan SRK dapat dilakukan.
- Sebelum insert, cek SRK berdasarkan nomor peserta dan bulan/tahun rencana kontrol. Gunakan kembali surat yang cocok SEP, poli, dokter, dan tanggalnya.
- Jika ditemukan surat untuk SEP/poli yang sama dalam bulan tersebut dengan rincian berbeda, proses dihentikan; koreksi melalui menu Rencana Kontrol BPJS.
- Kegagalan pemeriksaan, respons tidak lengkap, atau kegagalan insert tidak dianggap sebagai keberhasilan. Respons 201 hanya diperlakukan sebagai daftar kosong jika pesannya menyatakan data tidak ditemukan.
- Perubahan rencana di MDS saat form discharge terbuka mengharuskan form dibuka ulang. Pasien yang sudah dipulangkan tidak diproses ulang melalui jalur baru.
- API BPJS berada di luar transaksi database lokal. Tanggal pulang/SRK yang sudah berhasil di BPJS tidak ikut rollback bila tahap berikutnya gagal. Percobaan ulang memeriksa kembali SRK yang sudah terbit. Pemeriksaan bulanan ini bukan jaminan deduplikasi lintas bulan atau transaksi serentak pada beberapa server; perlu rekonsiliasi melalui menu BPJS bila ada konflik.

## UAT

1. Simpan MDS dengan jadwal lengkap: data tersimpan, belum ada SRK baru dari aksi simpan MDS.
2. Simpan MDS dengan poli/dokter tanpa tanggal: isian tetap muncul pada MDS/discharge; perawat melengkapi tanggal lalu discharge mengirim SRK. Buka kembali MDS dan pastikan tanggal yang diisi perawat tampil sebagai informasi untuk dokter.
3. MDS belum memiliki rencana kontrol: perawat dapat mengisi rencana lengkap di discharge. Form kosong tidak mewajibkan SRK.
4. Uji beberapa baris untuk poli berbeda; periksa semua surat yang dihasilkan dan perilaku saat salah satu gagal.
5. Uji tanggal tidak valid, mapping kosong, kartu kosong, integrasi tidak aktif, respons gagal, dan timeout BPJS. Jangan menganggap pasien telah selesai discharge saat proses mengembalikan kegagalan.
6. Ulangi setelah SRK sudah berhasil terbit: surat yang identik digunakan kembali; rincian yang konflik meminta koreksi.
7. Uji perubahan MDS saat form discharge masih terbuka serta pengulangan konfirmasi setelah pasien sudah pulang.
8. Uji pembatalan discharge lalu discharge kembali, termasuk SRK yang sudah terbit sebelum pembatalan.
9. Regresi rawat jalan, non-BPJS, fasilitas lain, tanpa SEP, Casemix, pasien meninggal, serta appointment/Antrol existing.
10. Periksa tampilan kontrol, dropdown dokter/poli, nilai setelah postback, dan laporan/SatuSehat yang membaca `Items`. Uji lima baris agar baris kelima tidak menampilkan ulang baris ketiga.

## Verifikasi otomatis

- Test kebijakan cakupan, pemeriksaan/pengiriman ulang SRK, penanganan kegagalan BPJS, serta input dan kompatibilitas rencana tanpa tanggal berada di `Temiang.Avicenna.Tests/Bridging/`.
- Pengujian menggunakan data sintetis dan fungsi pengganti respons BPJS; tidak mengirim ke BPJS atau mengakses database pasien.
- UAT pada aplikasi berjalan dan BPJS testing tetap diperlukan untuk memverifikasi UI dan perilaku layanan sebenarnya.
