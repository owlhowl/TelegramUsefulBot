﻿using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramUsefulBot
{
    public class StateMachine
    {
        private readonly User user;
        private State state;

        public StateMachine(User user, State state)
        {
            this.user = user;
            SetState(state);
        }

        public void SetState(State newState) => state = newState;

        public void UpdateHandler(ITelegramBotClient botClient, Update update)
        {
            state.UpdateHandler(user, botClient, update);
        }
    }
}
