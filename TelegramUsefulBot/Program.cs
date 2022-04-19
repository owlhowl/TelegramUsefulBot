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
            var bot = new TelegramBotClient("5378738338:AAETL_jMrdSFWF5LMj34Jh29yz11pnHlAsY");

            var me = bot.GetMeAsync().Result;
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");

            using var cts = new CancellationTokenSource();

            ReceiverOptions receiverOptions = new() { AllowedUpdates = { } };

            bot.StartReceiving(Handlers.HandleUpdateAsync, Handlers.HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine($"Start listening on {me.Username}.");
            Console.ReadKey();

            cts.Cancel();
        }
    }
}
