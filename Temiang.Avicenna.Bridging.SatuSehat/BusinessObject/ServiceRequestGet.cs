using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Temiang.Avicenna.Bridging.SatuSehat.BusinessObject
{
    internal class ServiceRequestGet
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
        public class Category
        {
            [JsonProperty("coding")]
            public List<Coding> Coding;
        }

        public class Code
        {
            [JsonProperty("coding")]
            public List<Coding> Coding;

            [JsonProperty("text")]
            public string Text;
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
        }

        public class Identifier
        {
            [JsonProperty("system")]
            public string System;

            [JsonProperty("value")]
            public string Value;
        }

        public class Link
        {
            [JsonProperty("relation")]
            public string Relation;

            [JsonProperty("url")]
            public string Url;
        }

        public class Meta
        {
            [JsonProperty("lastUpdated")]
            public DateTime? LastUpdated;

            [JsonProperty("versionId")]
            public string VersionId;
        }

        public class Performer
        {
            [JsonProperty("display")]
            public string Display;

            [JsonProperty("reference")]
            public string Reference;
        }

        public class ReasonCode
        {
            [JsonProperty("text")]
            public string Text;
        }

        public class Requester
        {
            [JsonProperty("display")]
            public string Display;

            [JsonProperty("reference")]
            public string Reference;
        }

        public class Resource
        {
            [JsonProperty("authoredOn")]
            public DateTime? AuthoredOn;

            [JsonProperty("category")]
            public List<Category> Category;

            [JsonProperty("code")]
            public Code Code;

            [JsonProperty("encounter")]
            public Encounter Encounter;

            [JsonProperty("id")]
            public string Id;

            [JsonProperty("identifier")]
            public List<Identifier> Identifier;

            [JsonProperty("intent")]
            public string Intent;

            [JsonProperty("meta")]
            public Meta Meta;

            [JsonProperty("occurrenceDateTime")]
            public DateTime? OccurrenceDateTime;

            [JsonProperty("performer")]
            public List<Performer> Performer;

            [JsonProperty("priority")]
            public string Priority;

            [JsonProperty("reasonCode")]
            public List<ReasonCode> ReasonCode;

            [JsonProperty("requester")]
            public Requester Requester;

            [JsonProperty("resourceType")]
            public string ResourceType;

            [JsonProperty("status")]
            public string Status;

            [JsonProperty("subject")]
            public Subject Subject;
        }

        public class Root
        {
            [JsonProperty("entry")]
            public List<Entry> Entry;

            [JsonProperty("link")]
            public List<Link> Link;

            [JsonProperty("resourceType")]
            public string ResourceType;

            [JsonProperty("total")]
            public int? Total;

            [JsonProperty("type")]
            public string Type;
        }

        public class Subject
        {
            [JsonProperty("reference")]
            public string Reference;
        }


    }
}
