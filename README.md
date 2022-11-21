# NewsScrappingApp

Required:

- .NET 5
- Command-line interface (CLI) tools for Entity Framework Core
- A telegram bot and a channel set up
    1. To create telegram bot
    2. To create telegram *public* channel
    3. To add the bot to the channel as admin

Get installed:

1. To run command [dotnet restore] in CLI
2. To rename the 'appsettings.example.json' file to 'appsettings.json'
3. To specify telegram bot token and channel name in the 'appsettings.json' file
4. To run [dotnet ef database update] in CLI
5. To run [dotnet run] command
