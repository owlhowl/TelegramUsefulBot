using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUsefulBot
{
    public class StateMachine
    {
        private readonly User user;
        State state;

        public StateMachine(User user, State defaultState)
        {
            this.user = user;
            SetState(defaultState);
        }

        public void SetState(State newState) => state = newState;

        internal void UpdateHandler(ITelegramBotClient botClient, Update update)
        {// передаем команду текущему стейту. Если он ее не сможет обработать, то не судьба
            state.UpdateHandler(user, botClient, update);
        }
    }
}
