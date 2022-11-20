using System;

namespace NewsParsingApp.Data
{
    public class News
    {
        public int Id {get;set;}

        public string Title {get;set;}

        public string Link {get;set;}

        public string SectionName {get;set;}

        public DateTime PublicationData {get;set;}

        public bool PublishedOnChannel {get;set;}
    }
}