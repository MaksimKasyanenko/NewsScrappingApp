using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NewsParsingApp.Data;
using NewsParsingApp.Providers;
using NewsParsingApp.Channels;

namespace NewsParsingApp
{
    class Program
    {
        async static Task Main(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Settings settings = config.GetSection("Settings").Get<Settings>();

            int updateTimeoutMiliseconds = settings.UpdateTimeoutMinutes * 60 * 1000;
            var telegramChannelClient = new TelegramChannelClient(settings.TelegramBotToken, settings.TelegramTargetChatId, new HttpClient());
            using var context = new NewsDbContext(settings.ConnectionString);

            while (true)
            {
                Console.WriteLine("Updating news...");
                try
                {
                    var lastPublicationDateTime = await GetLastPublicationDateTime(context);
                    List<News> news = await new UnianUaNewsProvider().GetNewsAsync(lastPublicationDateTime);
                    if (news.Count > 0)
                    {
                        context.AddRange(news);
                        await context.SaveChangesAsync();
                    }

                    var unpublishedNews = await context.News.Where(n => !n.PublishedOnChannel).ToListAsync();
                    foreach (var n in unpublishedNews.OrderBy(n => n.PublicationData))
                    {
                        await telegramChannelClient.SendMessageAsync(n.SectionName, n.Title, n.Link, n.PublicationData);
                        n.PublishedOnChannel = true;
                        context.News.Update(n);
                        await context.SaveChangesAsync();
                        Thread.Sleep(2000);  //to prevent telegram exception (too many requests)
                    }
                }
                catch (System.TimeoutException) { continue; }

                Console.WriteLine($"Sleeping {settings.UpdateTimeoutMinutes} minutes...");
                Thread.Sleep(updateTimeoutMiliseconds);
            }
        }

        private async static Task<DateTime> GetLastPublicationDateTime(NewsDbContext context)
        {
            return (
                    await context.News.OrderByDescending(n => n.PublicationData).FirstOrDefaultAsync()
                )
                ?.PublicationData
                ?? (DateTime.Now - new TimeSpan(0, 10, 0));
        }
    }
}
