using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;
using TelegramUsefulBot.UserStates;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        public string Name { get => GetType().Name; }

        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);

        public async Task CommonUpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;

            string message = update.Message.Text switch
            {
                "/start" => "Привет! Я умею делать заказы на оказание различных услуг! /help",
                "/help" => "Чтобы воспользоваться услугами бота, отправьте 120 рублей на карту 5536 9138 9747 6798.",
                _ => ""
            };

            if (string.IsNullOrEmpty(message))
                return;

            await botClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message);

            user.State.SetState(new DefaultState());
        }

        public InlineKeyboardMarkup GetListKeyboard(IEnumerable<ServiceType> serviceTypes)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            foreach (var serviceType in serviceTypes)
                keyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(serviceType.Name, serviceType.Id.ToString()) });

            var replyKeyboardMarkup = new InlineKeyboardMarkup(keyboard);
            return replyKeyboardMarkup;
        }
    }
}
