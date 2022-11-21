using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PuppeteerSharp;
using HtmlAgilityPack;
using NewsParsingApp.Data;

namespace NewsParsingApp.Providers
{
    public abstract class NewsProvider
    {
        protected abstract string Url {get;}

        public async Task<List<News>> GetNewsAsync(DateTime sinceDateTime)
        {
            var htmlDoc = await GetHtmlDocumentAsync();
            return ScrapeOut(htmlDoc, sinceDateTime) ?? new List<News>();
        }

        protected async virtual Task<HtmlDocument> GetHtmlDocumentAsync()
        {
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(
                new LaunchOptions { Headless = true });
            await using var page = await browser.NewPageAsync();
            await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>{{"Accept", "*/*"}, {"User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:107.0) Gecko/20100101 Firefox/107.0"}});
            await page.GoToAsync(Url, WaitUntilNavigation.Networkidle0);
            var htmlContent = await page.GetContentAsync();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);
            return htmlDoc;
        }

        protected abstract List<News> ScrapeOut(HtmlDocument htmlDoc, DateTime sinceDateTime);
    }
}