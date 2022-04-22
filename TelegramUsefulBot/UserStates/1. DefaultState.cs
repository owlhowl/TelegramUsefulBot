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
            await CommonUpdateHandler(user, botClient, update);
        }
    }
}
