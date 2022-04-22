using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUsefulBot.DB;

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

            bot.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions, cts.Token);

            Console.WriteLine($"Start listening on {me.Username}.");
            Console.ReadKey();

            cts.Cancel();
        }

        static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            long userId = -1;

            if (update.Type == UpdateType.Message)
                userId = update.Message.From.Id;

            else if (update.Type == UpdateType.CallbackQuery)
                userId = update.CallbackQuery.From.Id;

            else if (update.Type == UpdateType.MyChatMember)
                userId = update.MyChatMember.From.Id;

            else
                return Task.CompletedTask;

            if (BotDB.HasUser(userId))
                BotDB.GetUser(userId)
                    .State
                    .UpdateHandler(botClient, update);
            else
                BotDB.AddUser(userId, update.Message?.From.Username)
                    .State
                    .UpdateHandler(botClient, update);

            return Task.CompletedTask;
        }

        static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
