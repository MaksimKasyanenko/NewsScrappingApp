using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp;
using HtmlAgilityPack;
using NewsParsingApp.Data;

namespace NewsParsingApp.Providers
{
    internal class UkrNetNewsProvider
    {
        const string _ukrNetUrl = "https://www.ukr.net/";

        public async Task<List<News>> GetNewsAsync(DateTime sinceDateTime)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(await GetPageContentAsync());

            var res = new List<News>();

            var sections = htmlDoc.DocumentNode.SelectSingleNode("//article").ChildNodes;
            foreach(var feedSection in sections)
            {
                string sectionName = feedSection.SelectSingleNode("./h2")?.InnerText.Trim() ?? feedSection.SelectSingleNode("./div/a")?.InnerText.Trim();
                var feedItems = feedSection.SelectNodes("./div[@class='feed__item']");
                if(feedItems == null) continue;

                DateTime dateTime = DateTime.Today;
                
                foreach(var feedItem in feedItems)
                {
                    int[] time = ParseTime(feedItem.SelectSingleNode("./time").InnerText);

                    DateTime temp = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, time[0], time[1], 0);
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

        private async Task<string> GetPageContentAsync()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>{{"Accept", "*/*"}, {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0"}});
            await page.GoToAsync(_ukrNetUrl, WaitUntilNavigation.Networkidle0);
            return await page.GetContentAsync();
        }

        private int[] ParseTime(string text)
        {
            return text.Split(':').Select(n => Int32.Parse(n)).ToArray();
        }
    }
}