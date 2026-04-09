using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Bridging.SatuSehat.BusinessObject
{
    internal class ConditionGet
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Category
        {
            [JsonProperty("coding")]
            public List<Coding> Coding;
        }

        public class ClinicalStatus
        {
            [JsonProperty("coding")]
            public List<Coding> Coding;
        }

        public class Code
        {
            [JsonProperty("coding")]
            public List<Coding> Coding;
        }

        public class Coding
        {
            [JsonProperty("code")]
            public string Code;

            [JsonProperty("display")]
            public string Display;

            [JsonProperty("system")]
            public string System;
        }

        public class Encounter
        {
            [JsonProperty("display")]
            public string Display;

            [JsonProperty("reference")]
            public string Reference;
        }

        public class Entry
        {
            [JsonProperty("fullUrl")]
            public string FullUrl;

            [JsonProperty("resource")]
            public Resource Resource;

            [JsonProperty("search")]
            public Search Search;
        }

        public class Meta
        {
            [JsonProperty("lastUpdated")]
            public DateTime? LastUpdated;

            [JsonProperty("versionId")]
            public string VersionId;
        }

        public class Resource
        {
            [JsonProperty("category")]
            public List<Category> Category;

            [JsonProperty("clinicalStatus")]
            public ClinicalStatus ClinicalStatus;

            [JsonProperty("code")]
            public Code Code;

            [JsonProperty("encounter")]
            public Encounter Encounter;

            [JsonProperty("id")]
            public string Id;

            [JsonProperty("meta")]
            public Meta Meta;

            [JsonProperty("resourceType")]
            public string ResourceType;

            [JsonProperty("subject")]
            public Subject Subject;
        }

        public class Root
        {
            [JsonProperty("entry")]
            public List<Entry> Entry;

            [JsonProperty("resourceType")]
            public string ResourceType;

            [JsonProperty("total")]
            public int? Total;

            [JsonProperty("type")]
            public string Type;
        }

        public class Search
        {
            [JsonProperty("mode")]
            public string Mode;
        }

        public class Subject
        {
            [JsonProperty("display")]
            public string Display;

            [JsonProperty("reference")]
            public string Reference;
        }




    }
}
