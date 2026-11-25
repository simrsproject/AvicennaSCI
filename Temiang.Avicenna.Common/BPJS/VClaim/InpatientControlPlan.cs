using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using Newtonsoft.Json;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.JsonField;
using ControlLetter = Temiang.Avicenna.Common.BPJS.VClaim.v11.RencanaKontrol;

namespace Temiang.Avicenna.Common.BPJS.VClaim
{
    public static class InpatientControlPlan
    {
        public static string ResolveControlSep(Registration registration, string cardNo, string poliCode,
            DateTime controlDate)
        {
            if (!IsSentOnDischarge(registration)) return registration.BpjsSepNo;

            var referrals = new List<v11.Rujukan.Select.Rujukan2>();
            foreach (var source in new[] { Enum.JenisFaskes.Faskes_1, Enum.JenisFaskes.RS })
            {
                var result = new v11.Service().GetRujukan(cardNo, source);
                if (result == null || result.MetaData == null)
                    throw new InvalidOperationException("Respons pemeriksaan rujukan BPJS tidak tersedia.");
                if (!result.MetaData.IsValid)
                {
                    if (result.MetaData.Code == "201" && !string.IsNullOrWhiteSpace(result.MetaData.Message)
                        && (result.MetaData.Message.IndexOf("tidak ditemukan", StringComparison.OrdinalIgnoreCase) >= 0
                            || string.Equals(result.MetaData.Message.Trim(), "Rujukan Tidak Ada", StringComparison.OrdinalIgnoreCase)))
                        continue;
                    throw new InvalidOperationException("Pemeriksaan rujukan BPJS gagal: " + result.MetaData.Message);
                }
                if (result.Response == null || result.Response.Rujukan == null)
                    throw new InvalidOperationException("Daftar rujukan BPJS tidak lengkap.");
                referrals.AddRange(result.Response.Rujukan);
            }

            var active = new List<v11.Rujukan.Select.Rujukan2>();
            foreach (var referral in referrals)
            {
                if (referral == null || referral.PoliRujukan == null || referral.Peserta == null
                    || referral.Peserta.NoKartu != cardNo)
                    throw new InvalidOperationException("Detail poli atau peserta rujukan BPJS tidak sesuai.");
                if (!string.Equals(referral.PoliRujukan.Kode, poliCode, StringComparison.OrdinalIgnoreCase)) continue;
                DateTime referralDate;
                if (!DateTime.TryParseExact(referral.TglKunjungan, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out referralDate) || string.IsNullOrWhiteSpace(referral.NoKunjungan))
                    throw new InvalidOperationException("Tanggal atau nomor rujukan BPJS tidak valid.");
                // Referral validity is evaluated on the planned control date, per control-plan item.
                if (referralDate.Date <= controlDate.Date && (controlDate.Date - referralDate.Date).TotalDays <= 90)
                    active.Add(referral);
            }
            if (!active.Any()) return registration.BpjsSepNo;

            // Search only services already issued, independently of the future control date.
            var historyEndDate = DateTime.Today;
            var history = new v11.Service().GetDataHistoriPelayananPeserta(cardNo, historyEndDate.AddDays(-90), historyEndDate);
            if (history == null || history.MetaData == null || !history.MetaData.IsValid
                || history.Response == null || history.Response.Histori == null)
                throw new InvalidOperationException("Riwayat SEP BPJS belum berhasil diperiksa. SRK belum dikirim.");

            var hospital = ConfigurationManager.AppSettings["BPJSHospitalID"];
            if (string.IsNullOrWhiteSpace(hospital))
                throw new InvalidOperationException("Kode PPK pelayanan BPJS belum dikonfigurasi.");
            var candidates = new List<Tuple<string, DateTime>>();
            foreach (var sep in history.Response.Histori)
            {
                if (sep == null || sep.JnsPelayanan != "2" || sep.NoKartu != cardNo
                    || string.IsNullOrWhiteSpace(sep.NoSep) || !sep.NoSep.StartsWith(hospital, StringComparison.Ordinal)
                    || !active.Any(r => r.NoKunjungan == sep.NoRujukan)) continue;
                if (!string.Equals(sep.Poli, poliCode, StringComparison.OrdinalIgnoreCase)
                    && !active.Any(r => r.NoKunjungan == sep.NoRujukan
                        && string.Equals(r.PoliRujukan.Nama, sep.Poli, StringComparison.OrdinalIgnoreCase)))
                    continue;
                DateTime sepDate;
                if (!DateTime.TryParseExact(sep.TglSep, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out sepDate))
                    throw new InvalidOperationException("Tanggal riwayat SEP BPJS tidak valid.");
                if (sepDate.Date <= historyEndDate) candidates.Add(Tuple.Create(sep.NoSep, sepDate));
            }
            if (!candidates.Any())
                throw new InvalidOperationException("Rujukan aktif ke poli " + poliCode +
                    " ditemukan, tetapi SEP rawat jalan terkait belum ditemukan. Periksa melalui menu BPJS sebelum menerbitkan SRK.");
            var latestDate = candidates.Max(c => c.Item2);
            var latest = candidates.Where(c => c.Item2 == latestDate).Select(c => c.Item1).Distinct().ToList();
            if (latest.Count != 1)
                throw new InvalidOperationException("Ada beberapa SEP rawat jalan terakhir pada tanggal yang sama. Periksa SEP asal SRK melalui menu BPJS.");
            var detail = new v11.Service().GetSep(latest[0]);
            if (detail == null || detail.MetaData == null || !detail.MetaData.IsValid
                || detail.Response == null || detail.Response.NoSep != latest[0]
                || !active.Any(r => r.NoKunjungan == detail.Response.NoRujukan))
                throw new InvalidOperationException("SEP rawat jalan terpilih belum berhasil diverifikasi ke BPJS. SRK belum dikirim.");
            return latest[0];
        }

        public static bool IsSentOnDischarge(Registration registration)
        {
            return registration != null && IsSentOnDischarge(AppSession.Parameter.HealthcareInitial,
                registration.SRRegistrationType,
                AppSession.Parameter.GuarantorAskesID.Contains(registration.GuarantorID),
                registration.BpjsSepNo);
        }

        public static bool IsSentOnDischarge(string healthcareInitial, string registrationType,
            bool isBpjsGuarantor, string sepNo)
        {
            // Keep the existing RSI exception (SRK only, appointments are booked through MJKN).
            return healthcareInitial == "RSI" && registrationType == AppConstant.RegistrationType.InPatient
                && isBpjsGuarantor && !string.IsNullOrWhiteSpace(sepNo);
        }

        public static void SaveDraft(string registrationNo, ControlPlan plan)
        {
            var summary = new MedicalDischargeSummaryByNurse();
            if (!summary.LoadByPrimaryKey(registrationNo))
                summary.RegistrationNo = registrationNo;
            summary.ControlPlan = JsonConvert.SerializeObject(plan);
            summary.Save();
        }

        public static string EnsureControlLetter(ControlLetter.Insert.Request.Root request,
            Func<ControlLetter.Select.ResponseSuratKontrolList.Root> find,
            Func<ControlLetter.Insert.Request.Root, ControlLetter.Insert.Response.Root> insert)
        {
            var existing = find();
            if (existing == null || existing.MetaData == null)
                throw new InvalidOperationException("BPJS tidak mengembalikan hasil pemeriksaan SRK.");

            if (existing.MetaData.IsValid)
            {
                if (existing.Response == null || existing.Response.List == null)
                    throw new InvalidOperationException("Daftar SRK dari BPJS tidak lengkap. Silakan coba kembali.");

                var match = existing.Response.List.FirstOrDefault(item => item.JnsKontrol == "2"
                    && item.NoSepAsalKontrol == request.Request.NoSEP
                    && item.PoliTujuan == request.Request.PoliKontrol
                    && item.KodeDokter == request.Request.KodeDokter
                    && item.TglRencanaKontrol == request.Request.TglRencanaKontrol);
                if (match != null && !string.IsNullOrWhiteSpace(match.NoSuratKontrol))
                    return match.NoSuratKontrol;

                // An existing letter with different details must be corrected explicitly in the BPJS menu.
                if (existing.Response.List.Any(item => item.JnsKontrol == "2"
                    && item.NoSepAsalKontrol == request.Request.NoSEP
                    && item.PoliTujuan == request.Request.PoliKontrol))
                    throw new InvalidOperationException("SRK untuk SEP dan poli ini sudah ada dengan data berbeda. Periksa menu Rencana Kontrol BPJS.");
            }
            else if (existing.MetaData.Code != "201" || string.IsNullOrWhiteSpace(existing.MetaData.Message)
                || existing.MetaData.Message.IndexOf("tidak ditemukan", StringComparison.OrdinalIgnoreCase) < 0)
            {
                // Do not create a letter when the duplicate check failed (e.g. authentication or timeout).
                throw new InvalidOperationException("Pemeriksaan SRK BPJS gagal: " + existing.MetaData.Message);
            }

            var result = insert(request);
            if (result == null || result.MetaData == null || !result.MetaData.IsValid
                || result.Response == null || string.IsNullOrWhiteSpace(result.Response.NoSuratKontrol))
                throw new InvalidOperationException("Pengiriman SRK BPJS gagal: " +
                    (result != null && result.MetaData != null ? result.MetaData.Message : "Respons tidak tersedia."));

            return result.Response.NoSuratKontrol;
        }
    }
}
