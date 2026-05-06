using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Common.BPJS.Antrian
{
    public class QrCode
    {
        public class Param
        {
            public class Root
            {
                [JsonProperty("nomorKartu")]
                public string NomorKartu;

                [JsonProperty("nama")]
                public string Nama;

                [JsonProperty("alamat")]
                public string Alamat;

                [JsonProperty("tglLhr")]
                public string TglLhr;

                [JsonProperty("nik")]
                public string Nik;

                [JsonProperty("fktp")]
                public string Fktp;

                [JsonProperty("fktpGigi")]
                public string FktpGigi;

                [JsonProperty("statusPeserta")]
                public string StatusPeserta;

                [JsonProperty("nokapst")]
                public string Nokapst;

                [JsonProperty("kodeBooking")]
                public string KodeBooking;

                [JsonProperty("noRujukan")]
                public string NoRujukan;

                [JsonProperty("norm")]
                public string Norm;

                [JsonProperty("ketKunjungan")]
                public string KetKunjungan;

                [JsonProperty("namaFaskesAsalRujuk")]
                public object NamaFaskesAsalRujuk;

                [JsonProperty("namaPoli")]
                public string NamaPoli;

                [JsonProperty("namaDokter")]
                public string NamaDokter;

                [JsonProperty("nomorAntrean")]
                public string NomorAntrean;
            }
        }
    }
}
