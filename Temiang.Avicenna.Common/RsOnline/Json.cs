using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Common.RsOnline
{
    public class Json
    {
        public class Response
        {
            public class ReferensiTempatTidur
            {
                public class Root
                {
                    [JsonProperty("tempat_tidur")]
                    public List<TempatTidur> TempatTidur;
                }

                public class TempatTidur
                {
                    [JsonProperty("kode_tt")]
                    public string KodeTt;

                    [JsonProperty("nama_tt")]
                    public string NamaTt;
                }
            }

            public class DataTempatTidur
            {
                public class Fasyanke
                {
                    [JsonProperty("id_tt")]
                    public string IdTt;

                    [JsonProperty("tt")]
                    public string Tt;

                    [JsonProperty("ruang")]
                    public string Ruang;

                    [JsonProperty("kode_siranap")]
                    public string KodeSiranap;

                    [JsonProperty("jumlah_ruang")]
                    public string JumlahRuang;

                    [JsonProperty("jumlah")]
                    public string Jumlah;

                    [JsonProperty("terpakai_suspek")]
                    public string TerpakaiSuspek;

                    [JsonProperty("terpakai_konfirmasi")]
                    public string TerpakaiKonfirmasi;

                    [JsonProperty("antrian")]
                    public string Antrian;

                    [JsonProperty("prepare")]
                    public string Prepare;

                    [JsonProperty("prepare_plan")]
                    public object PreparePlan;

                    [JsonProperty("kosong")]
                    public string Kosong;

                    [JsonProperty("terpakai_dbd")]
                    public string TerpakaiDbd;

                    [JsonProperty("terpakai_dbd_anak")]
                    public string TerpakaiDbdAnak;

                    [JsonProperty("terpakai")]
                    public string Terpakai;

                    [JsonProperty("covid")]
                    public string Covid;

                    [JsonProperty("id_t_tt")]
                    public string IdTTt;

                    [JsonProperty("tglupdate")]
                    public string Tglupdate;
                }

                public class Root
                {
                    [JsonProperty("fasyankes")]
                    public List<Fasyanke> Fasyankes;
                }
            }
        }

        public class Request
        {
            public class Insert
            {
                [JsonProperty("id_tt")]
                public string IdTt { get; set; }

                [JsonProperty("ruang")]
                public string Ruang { get; set; }

                [JsonProperty("jumlah_ruang")]
                public string JumlahRuang { get; set; }

                [JsonProperty("jumlah")]
                public string Jumlah { get; set; }

                [JsonProperty("terpakai")]
                public string Terpakai { get; set; }

                [JsonProperty("terpakai_suspek")]
                public string TerpakaiSuspek { get; set; }

                [JsonProperty("terpakai_konfirmasi")]
                public string TerpakaiKonfirmasi { get; set; }

                [JsonProperty("antrian")]
                public string Antrian { get; set; }

                [JsonProperty("prepare")]
                public string Prepare { get; set; }

                [JsonProperty("prepare_plan")]
                public string PreparePlan { get; set; }

                [JsonProperty("covid")]
                public string Covid { get; set; }

                [JsonIgnore]
                public string ServiceUnitID { get; set; }

                [JsonIgnore]
                public string RoomID { get; set; }

                [JsonIgnore]
                public string BedID { get; set; }
            }

            public class Update : Root
            {
                [JsonProperty("id_t_tt")]
                public string IdTtt { get; set; }
            }

            public class Delete
            {
                [JsonProperty("id_t_tt")]
                public string IdTtt { get; set; }
            }

            public class Root
            {
                [JsonProperty("ruang")]
                public string Ruang { get; set; }

                [JsonProperty("jumlah_ruang")]
                public string JumlahRuang { get; set; }

                [JsonProperty("jumlah")]
                public string Jumlah { get; set; }

                [JsonProperty("terpakai")]
                public string Terpakai { get; set; }

                [JsonProperty("terpakai_suspek")]
                public string TerpakaiSuspek { get; set; }

                [JsonProperty("terpakai_konfirmasi")]
                public string TerpakaiKonfirmasi { get; set; }

                [JsonProperty("antrian")]
                public string Antrian { get; set; }

                [JsonProperty("prepare")]
                public string Prepare { get; set; }

                [JsonProperty("prepare_plan")]
                public string PreparePlan { get; set; }

                [JsonProperty("covid")]
                public string Covid { get; set; }

                [JsonIgnore]
                public string ServiceUnitID { get; set; }
            }
        }
    }
}
