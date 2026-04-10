using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Bridging.SatuSehat.BusinessObject
{
    public class ImmunizationPost
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Actor
        {
            [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
            public string Reference;
        }

        public class Coding
        {
            [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
            public string System;

            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
            public string Code;

            [JsonProperty("display", NullValueHandling = NullValueHandling.Ignore)]
            public string Display;
        }

        public class DoseQuantity
        {
            [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
            public int? Value;

            [JsonProperty("unit", NullValueHandling = NullValueHandling.Ignore)]
            public string Unit;

            [JsonProperty("system", NullValueHandling = NullValueHandling.Ignore)]
            public string System;

            [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
            public string Code;
        }

        public class Encounter
        {
            [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
            public string Reference;
        }

        public class Function
        {
            [JsonProperty("coding", NullValueHandling = NullValueHandling.Ignore)]
            public List<Coding> Coding;
        }

        public class Location
        {
            [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
            public string Reference;

            [JsonProperty("display", NullValueHandling = NullValueHandling.Ignore)]
            public string Display;
        }

        public class Patient
        {
            [JsonProperty("reference", NullValueHandling = NullValueHandling.Ignore)]
            public string Reference;

            [JsonProperty("display", NullValueHandling = NullValueHandling.Ignore)]
            public string Display;
        }

        public class Performer
        {
            [JsonProperty("function", NullValueHandling = NullValueHandling.Ignore)]
            public Function Function;

            [JsonProperty("actor", NullValueHandling = NullValueHandling.Ignore)]
            public Actor Actor;
        }

        public class ProtocolApplied
        {
            [JsonProperty("doseNumberPositiveInt", NullValueHandling = NullValueHandling.Ignore)]
            public int? DoseNumberPositiveInt;
        }

        public class ReasonCode
        {
            [JsonProperty("coding", NullValueHandling = NullValueHandling.Ignore)]
            public List<Coding> Coding;
        }

        public class Root
        {
            [JsonProperty("resourceType", NullValueHandling = NullValueHandling.Ignore)]
            public string ResourceType;

            [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
            public string Status;

            [JsonProperty("vaccineCode", NullValueHandling = NullValueHandling.Ignore)]
            public VaccineCode VaccineCode;

            [JsonProperty("patient", NullValueHandling = NullValueHandling.Ignore)]
            public Patient Patient;

            [JsonProperty("encounter", NullValueHandling = NullValueHandling.Ignore)]
            public Encounter Encounter;

            [JsonProperty("occurrenceDateTime", NullValueHandling = NullValueHandling.Ignore)]
            public string OccurrenceDateTime;

            [JsonProperty("recorded", NullValueHandling = NullValueHandling.Ignore)]
            public string Recorded;

            [JsonProperty("primarySource", NullValueHandling = NullValueHandling.Ignore)]
            public bool? PrimarySource;

            [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
            public Location Location;

            [JsonProperty("lotNumber", NullValueHandling = NullValueHandling.Ignore)]
            public string LotNumber;

            [JsonProperty("expirationDate", NullValueHandling = NullValueHandling.Ignore)]
            public string ExpirationDate;

            [JsonProperty("route", NullValueHandling = NullValueHandling.Ignore)]
            public Route Route;

            [JsonProperty("doseQuantity", NullValueHandling = NullValueHandling.Ignore)]
            public DoseQuantity DoseQuantity;

            [JsonProperty("performer", NullValueHandling = NullValueHandling.Ignore)]
            public List<Performer> Performer;

            [JsonProperty("reasonCode", NullValueHandling = NullValueHandling.Ignore)]
            public List<ReasonCode> ReasonCode;

            [JsonProperty("protocolApplied", NullValueHandling = NullValueHandling.Ignore)]
            public List<ProtocolApplied> ProtocolApplied;
        }

        public class Route
        {
            [JsonProperty("coding", NullValueHandling = NullValueHandling.Ignore)]
            public List<Coding> Coding;
        }

        public class VaccineCode
        {
            [JsonProperty("coding", NullValueHandling = NullValueHandling.Ignore)]
            public List<Coding> Coding;
        }


    }
}
