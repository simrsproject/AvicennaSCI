using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Common.BPJS.Apotek.PelayananObat
{
    public class HapusPelayananObat
    {
        public class Request
        {
            public class Root
            {
                [JsonProperty("nosepapotek")]
                public string Nosepapotek;

                [JsonProperty("noresep")]
                public string Noresep;

                [JsonProperty("kodeobat")]
                public string Kodeobat;

                [JsonProperty("tipeobat")]
                public string Tipeobat;
            }
        }

        public class Response : Metadata
        {

        }
    }

    public class DaftarPelayananObat : Metadata
    {
        public class DetailSep
        {
            [JsonProperty("noSepApotek")]
            public string NoSepApotek { get; set; }

            [JsonProperty("noSepAsal")]
            public string NoSepAsal { get; set; }

            [JsonProperty("noresep")]
            public string Noresep { get; set; }

            [JsonProperty("nokartu")]
            public string Nokartu { get; set; }

            [JsonProperty("nmpst")]
            public string Nmpst { get; set; }

            [JsonProperty("kdjnsobat")]
            public string Kdjnsobat { get; set; }

            [JsonProperty("nmjnsobat")]
            public string Nmjnsobat { get; set; }

            [JsonProperty("tglpelayanan")]
            public string Tglpelayanan { get; set; }

            [JsonProperty("listobat")]
            public List<ListObat> Listobat { get; set; }
        }

        public class ListObat
        {
            [JsonProperty("kodeobat")]
            public string Kodeobat { get; set; }

            [JsonProperty("namaobat")]
            public string Namaobat { get; set; }

            [JsonProperty("tipeobat")]
            public string Tipeobat { get; set; }

            [JsonProperty("signa1")]
            public string Signa1 { get; set; }

            [JsonProperty("signa2")]
            public string Signa2 { get; set; }

            [JsonProperty("hari")]
            public string Hari { get; set; }

            [JsonProperty("permintaan")]
            public string Permintaan { get; set; }

            [JsonProperty("jumlah")]
            public string Jumlah { get; set; }

            [JsonProperty("harga")]
            public string Harga { get; set; }
        }

        public class Root
        {
            [JsonProperty("response")]
            public DetailSep Response { get; set; }

            [JsonProperty("metaData")]
            public Metadata MetaData { get; set; }
        }
    }

    public class RiwayatPelayananObat
    {
        public class History
        {
            [JsonProperty("nosjp")]
            public string Nosjp { get; set; }

            [JsonProperty("tglpelayanan")]
            public string Tglpelayanan { get; set; }

            [JsonProperty("noresep")]
            public string Noresep { get; set; }

            [JsonProperty("kodeobat")]
            public string Kodeobat { get; set; }

            [JsonProperty("namaobat")]
            public string Namaobat { get; set; }

            [JsonProperty("jmlobat")]
            public string Jmlobat { get; set; }
        }

        public class ListItem
        {
            [JsonProperty("nokartu")]
            public string Nokartu { get; set; }

            [JsonProperty("namapeserta")]
            public string Namapeserta { get; set; }

            [JsonProperty("tgllhr")]
            public string Tgllhr { get; set; }

            [JsonProperty("history")]
            public List<History> Histories { get; set; }
        }

        public class Response
        {
            [JsonProperty("list")]
            public ListItem List { get; set; }
        }

        public class Root
        {
            [JsonProperty("response")]
            public Response Response { get; set; }

            [JsonProperty("metaData")]
            public Metadata MetaData { get; set; }
        }
    }

}
