using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Sockets;
using System.Net;
using System.Text;
class Program
{
    static async Task Main()
    {
        var listener = new HttpListener();
        listener.Prefixes.Add("http://0.0.0.0:8080/");
        listener.Start();
        Console.WriteLine("HTTP-сервер запущен на порту 8080");

        _ = Task.Run(() =>
        {
            while (true)
            {
                var context = listener.GetContext();
                var response = context.Response;
                var buffer = Encoding.UTF8.GetBytes("Bot is running");
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.Close();
            }
        });

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

        await Task.Delay(-1);
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