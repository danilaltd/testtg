using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main()
    {
        // Инициализация Telegram-бота
        string Token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? throw new ArgumentException("Bot token is missing or invalid.");
        var botClient = new TelegramBotClient(Token);
        using CancellationTokenSource cts = new();
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
        Console.WriteLine("Бот запущен.");

        // Запуск HTTP-сервера
        var host = new WebHostBuilder()
            .UseKestrel()
            .Configure(app =>
            {
                app.Run(async context =>
                {
                    await context.Response.WriteAsync("Bot is running");
                });
            })
            .UseUrls($"http://*:{Environment.GetEnvironmentVariable("PORT") ?? "8080"}")
            .Build();

        await host.RunAsync();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { Text: { } messageText }) return;
        Console.WriteLine($"Вы сказали: {messageText}");

        await botClient.SendMessage(update.Message.Chat.Id, $"Вы сказали: {messageText}", cancellationToken: cancellationToken);
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Ошибка: {exception.Message}");
        return Task.CompletedTask;
    }
}