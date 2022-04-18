using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);
    }
}
