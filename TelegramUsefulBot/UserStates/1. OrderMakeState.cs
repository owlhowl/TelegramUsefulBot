using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    class OrderMakeState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, null, botClient, update))
                return;

            if (update.Message == null)
                return;

            if (update.Message.Text == "/order")
            {
                string text = "Выберите услугу для заказа:";

                InlineKeyboardMarkup inlineKeyboardMarkup = GetServiceTypesKeyboard(BotDB.GetServiceTypes());

                var message = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: text,
                    replyMarkup: inlineKeyboardMarkup);

                user.State.SetState(new OrderAddressState(message));
            }

            if (update.Message.Text == "/list")
            { 
                user.State.SetState(new OrderListState());
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
