using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUsefulBot.UserStates
{
    class DefaultState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, null, botClient, update))
                return;

            if (update.Message == null)
                return;

            if (update.Message.Text == "/order")
            {
                user.State.SetState(new OrderMakeState());
                user.State.UpdateHandler(botClient, update);
            }

            if (update.Message.Text == "/list")
            { 
                user.State.SetState(new OrderListState());
                user.State.UpdateHandler(botClient, update);
            }

            await Task.CompletedTask;
        }
    }
}
