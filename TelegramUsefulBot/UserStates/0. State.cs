using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUsefulBot.UserStates;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        protected Dictionary<string, Func<User, string>> commands = new Dictionary<string, Func<User, string>>() 
        {
            { 
                "/start", (user) =>
                {
                    user.State.SetState(new OrderMakeState());
                    return "Привет! Я умею делать заказы на оказание различных услуг! /help";
                } 
            },
            { 
                "/cancel", (user) =>
                {
                    user.CurrentOrder = new Order();
                    user.State.SetState(new OrderMakeState());
                    return "Оформление заказа отменено";
                }
            },
            { 
                "/help", (user) =>
                {
                    return "Чтобы воспользоваться услугами бота, отправьте 120 рублей на карту 5536 9138 9747 6798.";
                }
            },
        };

        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);

        protected async Task<bool> CommandHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return false;

            foreach (var command in commands)
            {
                if (update.Message.Text == command.Key)
                {
                    await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: command.Value.Invoke(user));
                    return true;
                }
            }

            return false;
        }
    }
}
