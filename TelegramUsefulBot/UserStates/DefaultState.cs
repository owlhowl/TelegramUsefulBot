using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramUsefulBot.UserStates
{
    class DefaultState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;
            if (update.Message.Text == "/start")
            {
                InlineKeyboardMarkup replyKeyboardMarkup = new(
                    new[]{
                        InlineKeyboardButton.WithCallbackData(text: "1", callbackData: "main_state1"),
                        InlineKeyboardButton.WithCallbackData(text: "2", callbackData: "main_state2"),
                    });

                // меняем интерфейс
                await botClient.SendTextMessageAsync(update.Message.Chat.Id,
                    "Привет",
                    ParseMode.Markdown,
                    replyMarkup: replyKeyboardMarkup);

                user.State.SetState(new MainMenuState()); // тут указываем класс-обработчик новых команд, таких классов может быть дофига
            }
        }
    }
}
