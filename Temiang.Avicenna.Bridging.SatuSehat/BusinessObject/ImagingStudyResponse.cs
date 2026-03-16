using System;
using System.Collections.Generic;

namespace Temiang.Avicenna.Bridging.SatuSehat.BusinessObject.ImagingStudyResponse
{
    public class ImagingStudyResponse
    {
        public string ResourceType { get; set; }
        public string Type { get; set; }
        public int Total { get; set; }
        public List<Link> Link { get; set; }
        public List<Entry> Entry { get; set; }
    }

    public class Link
    {
        public string Relation { get; set; }
        public string Url { get; set; }
    }

    public class Entry
    {
        public string FullUrl { get; set; }
        public ImagingStudy Resource { get; set; }
    }

    public class ImagingStudy
    {
        public string ResourceType { get; set; }
        public string Id { get; set; }
        public Meta Meta { get; set; }
        public List<Identifier> Identifier { get; set; }
        public List<References> BasedOn { get; set; }
        public string Status { get; set; }
        public Subject Subject { get; set; }
        public List<References> Interpreter { get; set; }
        public List<Modality> Modality { get; set; }
        public int NumberOfSeries { get; set; }
        public int NumberOfInstances { get; set; }
        public DateTime Started { get; set; }
        public List<Series> Series { get; set; }
    }

    public class Meta
    {
        public string VersionId { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class Identifier
    {
        public string Use { get; set; }
        public string System { get; set; }
        public string Value { get; set; }
        public IdentifierType Type { get; set; }
    }

    public class IdentifierType
    {
        public List<Coding> Coding { get; set; }
    }

    public class Coding
    {
        public string System { get; set; }
        public string Code { get; set; }
    }

    public class References
    {
        public string Reference { get; set; }
        public string Display { get; set; }
    }

    public class Subject
    {
        public string Reference { get; set; }
    }

    public class Modality
    {
        public string System { get; set; }
        public string Code { get; set; }
    }

    public class Series
    {
        public string Uid { get; set; }
        public Modality Modality { get; set; }
        public int Number { get; set; }
        public int NumberOfInstances { get; set; }
        public DateTime Started { get; set; }
        public List<Instance> Instance { get; set; }
    }

    public class Instance
    {
        public string Uid { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public SopClass SopClass { get; set; }
    }

    public class SopClass
    {
        public string System { get; set; }
        public string Code { get; set; }
    }
}
