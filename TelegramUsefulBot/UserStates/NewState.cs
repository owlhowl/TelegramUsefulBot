using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUsefulBot.UserStates
{
    class NewState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery == null)
                return;

            if (update.CallbackQuery.Data == "main_state3")
            {
                await botClient.SendTextMessageAsync(user.Id, "hi");
            }
            else if (update.CallbackQuery.Data == "main_state4")
            {
                await botClient.SendTextMessageAsync(user.Id, "hi");

            }

            await Task.CompletedTask;
        }
    }
}
