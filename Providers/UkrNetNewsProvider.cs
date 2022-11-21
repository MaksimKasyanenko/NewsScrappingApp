using System;
using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using NewsParsingApp.Data;
using NewsParsingApp.Utils;

namespace NewsParsingApp.Providers
{
    internal sealed class UkrNetNewsProvider : NewsProvider
    {
        protected override string Url => "https://www.ukr.net/";

        protected override List<News> ScrapeOut(string htmlContent, DateTime sinceDateTime)
        {
            var res = new List<News>();

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            var sections = htmlDoc.DocumentNode.SelectSingleNode("//article").ChildNodes;
            foreach(var feedSection in sections)
            {
                string sectionName = feedSection.SelectSingleNode("./h2")?.InnerText.Trim() ?? feedSection.SelectSingleNode("./div/a")?.InnerText.Trim();
                var feedItems = feedSection.SelectNodes("./div[@class='feed__item']");
                if(feedItems == null) continue;

                DateTime dateTime = DateTime.Today;
                
                foreach(var feedItem in feedItems)
                {
                    Time time = new Time(feedItem.SelectSingleNode("./time").InnerText);
                    DateTime temp = time.ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day);
                    
                    if(temp <= DateTime.Now)
                    {
                        dateTime = temp;
                    }
                    else
                    {
                        dateTime = temp - new TimeSpan(1, 0, 0, 0);
                    }

                    if(dateTime <= sinceDateTime) break;

                    var a = feedItem.SelectSingleNode(@"./*/a");
                    string title = a.InnerText.Trim();
                    string href = a.GetAttributeValue("href", "").Trim();
                    
                    res.Add(new News{
                        Title = title,
                        Link = href,
                        SectionName = sectionName,
                        PublicationData = dateTime
                    });
                }
            }
            return res;
        }
    }
}