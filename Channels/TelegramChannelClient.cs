using System;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace NewsParsingApp.Channels
{
    public class TelegramChannelClient
    {
        public TelegramChannelClient(string botToken, string channelId, HttpClient httpClient)
        {
            this._botToken = !string.IsNullOrWhiteSpace(botToken) ? botToken : throw new ArgumentException("Bot token must be specified");
            this._channelId = !string.IsNullOrWhiteSpace(channelId) ? channelId : throw new ArgumentException("Channel id must be specified");
            this._httpClient = httpClient ?? throw new ArgumentNullException($"{nameof(httpClient)} hasn't been passed");
        }

        private readonly string _botToken;
        private readonly string _channelId;
        private readonly string _apiLinkTemplate = @"https://api.telegram.org/bot{0}/sendMessage?chat_id={1}&text={2}&parse_mode=HTML";
        private readonly HttpClient _httpClient;

        public async Task SendMessageAsync(string title, string body, string detailsLink, DateTime publishingTime)
        {
            string htmlString = FormatMessage(title, body, detailsLink, publishingTime);
            string httpLink = string.Format(_apiLinkTemplate, _botToken, _channelId, htmlString);
            var response = await _httpClient.GetAsync(httpLink);
            response.EnsureSuccessStatusCode();
        }

        private string FormatMessage(string title, string body, string detailsLink, DateTime publishingTime)
        {
            var sb = new StringBuilder();
            sb.Append($"<b>{title}</b>\n");
            sb.Append($"<i>{publishingTime}</i>\n");
            sb.Append($"{body}\n");
            sb.Append(detailsLink);
            return sb.ToString();
        }
    }
}