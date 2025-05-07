using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://0.0.0.0:8080"); // Указываем порт

        var app = builder.Build();
        app.MapGet("/", () => "Bot is running"); // Добавляем обработку HTTP-запросов

        // Запуск Telegram-бота
        string Token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") ?? throw new ArgumentException("Bot token is missing or invalid.");
        var botClient = new TelegramBotClient(Token);
        using CancellationTokenSource cts = new();
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);
        Console.WriteLine("Бот запущен.");

        await app.RunAsync();
        cts.Cancel();
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