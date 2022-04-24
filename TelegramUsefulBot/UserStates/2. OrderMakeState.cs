using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderMakeState : State
    {
        private Message prevMessage;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, prevMessage, botClient, update))
                return;

            if (update.Message != null)
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = GetServiceTypesKeyboard(BotDB.GetServiceTypes());

                prevMessage = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: "Выберите услугу для заказа:",
                    replyMarkup: inlineKeyboardMarkup);
            }

            else if (update.CallbackQuery != null)
            {
                user.State.SetState(new OrderAddressState(prevMessage));
                user.State.UpdateHandler(botClient, update);
            }

            await Task.CompletedTask;
        }

        private InlineKeyboardMarkup GetServiceTypesKeyboard(IEnumerable<ServiceType> serviceTypes)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            foreach (var serviceType in serviceTypes)
                keyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(serviceType.Name, serviceType.Id.ToString()) });

            return new InlineKeyboardMarkup(keyboard);
        }
    }
}
