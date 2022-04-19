using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramUsefulBot.UserStates
{
    public class MainMenuState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            await CommonUpdateHandler(user, botClient, update);

            if (update.CallbackQuery == null)
                return;

            if (update.CallbackQuery.Data == "main_state1")
            {
                InlineKeyboardMarkup replyKeyboardMarkup = new(
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "3", callbackData: "main_state3"),
                        InlineKeyboardButton.WithCallbackData(text: "4", callbackData: "main_state4"),
                    });

                await botClient.SendTextMessageAsync(user.Id,
                    "Снова привет",
                    replyMarkup: replyKeyboardMarkup);
            }

            else if (update.CallbackQuery.Data == "main_state2")
            {
                await botClient.SendTextMessageAsync(user.Id, "2");
                user.State.SetState(new DefaultState());
            }

            await Task.CompletedTask; // заглушка
        }
    }
}
