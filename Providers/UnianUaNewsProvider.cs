using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using NewsParsingApp.Data;
using NewsParsingApp.Utils;

namespace NewsParsingApp.Providers
{
    internal sealed class UnianUaNewsProvider : NewsProvider
    {
        protected override string Url => "https://www.unian.ua/";

        protected override List<News> ScrapeOut(HtmlDocument htmlDoc, DateTime sinceDateTime)
        {
            var res = new List<News>();
            var feedItems = htmlDoc.DocumentNode.SelectNodes("//ul[@class='newsfeed__list ']/li[@class='newsfeed__item ']");

            DateTime dateTime = DateTime.Today;

            foreach (var feedItem in feedItems)
            {
                DateTime temp = new Time(
                        feedItem.SelectSingleNode("./span[@class='newsfeed__time']").InnerText
                    )
                    .ToDateTime(dateTime.Year, dateTime.Month, dateTime.Day);

                if (temp <= DateTime.Now)
                {
                    dateTime = temp;
                }
                else
                {
                    dateTime = temp - new TimeSpan(1, 0, 0, 0);
                }

                var linkElement = feedItem.SelectSingleNode("./h3[@class='newsfeed__link']/a");

                string title = linkElement.InnerText.Trim();
                string link = linkElement.GetAttributeValue("href", "").Trim();

                if (dateTime <= sinceDateTime) break;

                res.Add(new News
                {
                    Title = title,
                    Link = link,
                    SectionName = "",
                    PublicationData = dateTime
                });
            }

            return res;
        }
    }
}