using Newtonsoft.Json;
using System.Collections.Generic;

namespace Temiang.Avicenna.Common.BPJS.VClaim.v11.RujukanSatuSehat
{

    #region COMMON

    public class MetaData
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class Encounter
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }
    }

    public class CodeJejaringWilayah
    {
        [JsonProperty("kodePropinsi")]
        public string KodePropinsi { get; set; }

        [JsonProperty("namaPropinsi")]
        public string NamaPropinsi { get; set; }

        [JsonProperty("kodeKabupaten")]
        public string KodeKabupaten { get; set; }

        [JsonProperty("namaKabupaten")]
        public string NamaKabupaten { get; set; }
    }

    #endregion

    #region KRITERIA RUJUKAN

    public class KriteriaRujukanRequest
    {
        [JsonProperty("kodeFaskesSatuSehat")]
        public string KodeFaskesSatuSehat { get; set; }

        [JsonProperty("kodeDiagnosa")]
        public string KodeDiagnosa { get; set; }

        [JsonProperty("encounter")]
        public Encounter Encounter { get; set; }
    }

    public class KriteriaRujukanResponse
    {
        [JsonProperty("response")]
        public KriteriaResponse Response { get; set; }

        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }
    }

    public class KriteriaResponse
    {
        [JsonProperty("kriteriaRujukan")]
        public List<KriteriaItem> KriteriaRujukan { get; set; }

        [JsonProperty("JejaringWilayah")]
        public List<JejaringWilayah> JejaringWilayahRujukan { get; set; }
    }

    public class KriteriaItem
    {
        [JsonProperty("linkId")]
        public string LinkId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("item")]
        public object Item { get; set; }
    }

    public class JejaringWilayah
    {
        [JsonProperty("linkId")]
        public string LinkId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("item")]
        public List<JejaringItem> Item { get; set; }
    }

    public class JejaringItem
    {
        [JsonProperty("linkId")]
        public string LinkId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("answerOption")]
        public List<AnswerOption> AnswerOption { get; set; }
    }

    public class AnswerOption
    {
        [JsonProperty("valueCoding")]
        public ValueCoding ValueCoding { get; set; }
    }

    public class ValueCoding
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("display")]
        public string Display { get; set; }
    }


    #endregion

    #region FASKES RUJUKAN

    public class FaskesRujukanRequest
    {
        [JsonProperty("kodeFaskesSatuSehat")]
        public string KodeFaskesSatuSehat { get; set; }

        [JsonProperty("kodeDiagnosa")]
        public string KodeDiagnosa { get; set; }

        [JsonProperty("kodeSpesialis")]
        public string KodeSpesialis { get; set; }

        [JsonProperty("tglRencanaKunjungan")]
        public string TglRencanaKunjungan { get; set; }

        [JsonProperty("kriteriaRujukan")]
        public KriteriaRujukanWrapper KriteriaRujukan { get; set; }

        [JsonProperty("codeJejaringWilayah")]
        public CodeJejaringWilayah CodeJejaringWilayah { get; set; }

        [JsonProperty("encounter")]
        public Encounter Encounter { get; set; }
    }

    public class KriteriaRujukanWrapper
    {
        [JsonProperty("item")]
        public List<KriteriaAnswerItem> Item { get; set; }
    }

    public class KriteriaAnswerItem
    {
        [JsonProperty("linkId")]
        public string LinkId { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("answer")]
        public List<Answer> Answer { get; set; }
    }

    public class Answer
    {
        [JsonProperty("valueBoolean", NullValueHandling = NullValueHandling.Ignore)]
        public bool? ValueBoolean { get; set; }

        [JsonProperty("valueString", NullValueHandling = NullValueHandling.Ignore)]
        public string ValueString { get; set; }
    }

    public class FaskesRujukanResponse
    {
        [JsonProperty("response")]
        public FaskesResponse Response { get; set; }

        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }
    }

    public class FaskesResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("list")]
        public List<FaskesItem> List { get; set; }
    }

    public class FaskesItem
    {
        [JsonProperty("kodeFaskesSatuSehat")]
        public string KodeFaskesSatuSehat { get; set; }

        [JsonProperty("kdppk")]
        public string Kdppk { get; set; }

        [JsonProperty("nmppk")]
        public string Nmppk { get; set; }

        [JsonProperty("strataSatuSehat")]
        public string StrataSatuSehat { get; set; }

        [JsonProperty("alamatPpk")]
        public string AlamatPpk { get; set; }

        [JsonProperty("telpPpk")]
        public string TelpPpk { get; set; }

        [JsonProperty("kelas")]
        public string Kelas { get; set; }

        [JsonProperty("nmkc")]
        public string Nmkc { get; set; }

        [JsonProperty("kapasitas")]
        public int? Kapasitas { get; set; }

        [JsonProperty("jmlRujuk")]
        public int? JmlRujuk { get; set; }

        [JsonProperty("persentase")]
        public decimal? Persentase { get; set; }

        [JsonProperty("distance")]
        public decimal? Distance { get; set; }
    }

    #endregion

    #region CREATE RUJUKAN RS

    public class CreateRujukanRequest
    {
        [JsonProperty("request")]
        public RequestWrapper Request { get; set; }
    }

    public class RequestWrapper
    {
        [JsonProperty("t_rujukan")]
        public TRujukan TRujukan { get; set; }
    }

    public class TRujukan
    {
        [JsonProperty("noSep")]
        public string NoSep { get; set; }

        [JsonProperty("tglRujukan")]
        public string TglRujukan { get; set; }

        [JsonProperty("tglRencanaKunjungan")]
        public string TglRencanaKunjungan { get; set; }

        [JsonProperty("ppkDirujuk")]
        public string PpkDirujuk { get; set; }

        [JsonProperty("jnsPelayanan")]
        public string JnsPelayanan { get; set; }

        [JsonProperty("catatan")]
        public string Catatan { get; set; }

        [JsonProperty("diagRujukan")]
        public string DiagRujukan { get; set; }

        [JsonProperty("tipeRujukan")]
        public string TipeRujukan { get; set; }

        [JsonProperty("poliRujukan")]
        public string PoliRujukan { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }

        [JsonProperty("satuSehatRujukan")]
        public SatuSehatRujukan SatuSehatRujukan { get; set; }
    }

    public class SatuSehatRujukan
    {
        [JsonProperty("kodeFaskesSatuSehat")]
        public string KodeFaskesSatuSehat { get; set; }

        [JsonProperty("idPasienSatuSehat")]
        public string IdPasienSatuSehat { get; set; }

        [JsonProperty("kdppkSatuSehatTujuanRujukan")]
        public string KdppkSatuSehatTujuanRujukan { get; set; }

        [JsonProperty("kdDokterSatuSehat")]
        public string KdDokterSatuSehat { get; set; }

        [JsonProperty("encounter")]
        public Encounter Encounter { get; set; }

        [JsonProperty("patientInstruction")]
        public string PatientInstruction { get; set; }

        [JsonProperty("kriteriaRujukan")]
        public KriteriaRujukanWrapper KriteriaRujukan { get; set; }

        [JsonProperty("keteranganRujukan")]
        public string KeteranganRujukan { get; set; }

        [JsonProperty("codeJejaringWilayah")]
        public CodeJejaringWilayah CodeJejaringWilayah { get; set; }
    }

    public class CreateRujukanResponse
    {
        [JsonProperty("response")]
        public RujukanResponse Response { get; set; }

        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }
    }

    public class RujukanResponse
    {
        [JsonProperty("rujukan")]
        public RujukanData Rujukan { get; set; }
    }

    public class RujukanData
    {
        [JsonProperty("noRujukan")]
        public string NoRujukan { get; set; }

        [JsonProperty("noRujukanSatuSehat")]
        public string NoRujukanSatuSehat { get; set; }

        [JsonProperty("serviceRequestId")]
        public string ServiceRequestId { get; set; }

        [JsonProperty("tglRujukan")]
        public string TglRujukan { get; set; }

        [JsonProperty("AsalRujukan")]
        public RujukanProvider AsalRujukan { get; set; }

        [JsonProperty("tujuanRujukan")]
        public RujukanProvider TujuanRujukan { get; set; }

        [JsonProperty("diagnosa")]
        public Diagnosa Diagnosa { get; set; }

        [JsonProperty("peserta")]
        public Peserta Peserta { get; set; }

        [JsonProperty("poliTujuan")]
        public PoliTujuan PoliTujuan { get; set; }
    }

    public class RujukanProvider
    {
        [JsonProperty("kode")]
        public string Kode { get; set; }

        [JsonProperty("nama")]
        public string Nama { get; set; }
    }

    public class Diagnosa
    {
        [JsonProperty("kode")]
        public string Kode { get; set; }

        [JsonProperty("nama")]
        public string Nama { get; set; }
    }

    public class Peserta
    {
        [JsonProperty("asuransi")]
        public string Asuransi { get; set; }

        [JsonProperty("hakKelas")]
        public string HakKelas { get; set; }

        [JsonProperty("jnsPeserta")]
        public string JnsPeserta { get; set; }

        [JsonProperty("kelamin")]
        public string Kelamin { get; set; }

        [JsonProperty("nama")]
        public string Nama { get; set; }

        [JsonProperty("noKartu")]
        public string NoKartu { get; set; }

        [JsonProperty("noMr")]
        public string NoMr { get; set; }

        [JsonProperty("tglLahir")]
        public string TglLahir { get; set; }
    }

    public class PoliTujuan
    {
        [JsonProperty("kode")]
        public string Kode { get; set; }

        [JsonProperty("nama")]
        public string Nama { get; set; }
    }

    #endregion

    #region spesialis
    public class GetSpesialisResponse
    {
        [JsonProperty("response")]
        public List<SpesialisItem> Response { get; set; }

        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }
    }

    public class SpesialisItem
    {
        [JsonProperty("kodeSpesialis")]
        public string KodeSpesialis { get; set; }

        [JsonProperty("namaSpesialis")]
        public string NamaSpesialis { get; set; }
    }
    #endregion

    #region DELETE RUJUKAN

    public class DeleteRujukanRequest
    {
        [JsonProperty("request")]
        public DeleteRequestWrapper Request { get; set; }
    }

    public class DeleteRequestWrapper
    {
        [JsonProperty("t_rujukan")]
        public DeleteTRujukan TRujukan { get; set; }
    }

    public class DeleteTRujukan
    {
        [JsonProperty("noRujukan")]
        public string NoRujukan { get; set; }

        [JsonProperty("user")]
        public string User { get; set; }
    }

    public class DeleteRujukanResponse
    {
        [JsonProperty("metaData")]
        public MetaData MetaData { get; set; }

        [JsonProperty("response")]
        public DeleteResponse Response { get; set; }
    }

    public class DeleteResponse
    {
        [JsonProperty("noRujukan")]
        public string NoRujukan { get; set; }
    }

    #endregion

}