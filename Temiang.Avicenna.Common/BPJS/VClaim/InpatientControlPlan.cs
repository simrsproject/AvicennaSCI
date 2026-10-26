using System;
using System.Linq;
using Newtonsoft.Json;
using Temiang.Avicenna.BusinessObject;
using Temiang.Avicenna.BusinessObject.JsonField;
using ControlLetter = Temiang.Avicenna.Common.BPJS.VClaim.v11.RencanaKontrol;

namespace Temiang.Avicenna.Common.BPJS.VClaim
{
    public static class InpatientControlPlan
    {
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
