using System;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;

namespace TelegramUsefulBot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new TelegramBotClient("{YOUR TOKEN HERE}");

            var me = bot.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id}.");

            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            bot.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine($"Start listening.");
            Console.ReadKey();

            cts.Cancel();
        }
    }
}
