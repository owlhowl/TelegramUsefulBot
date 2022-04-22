using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramUsefulBot.UserStates;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        public string Name { get => GetType().Name; }

        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);

        public async Task CommonUpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;

            string message = "";

            Action action = update.Message.Text switch
            {
                "/start" =>
                    () =>
                    {
                        message = "Привет! Я умею делать заказы на оказание различных услуг! /help";
                        user.State.SetState(new OrderMakeState());
                    }
                ,
                "/cancel" =>
                    () =>
                    {
                        message = "Оформление заказа отменено";
                        user.CurrentOrder = new Order();
                        user.State.SetState(new OrderMakeState());
                    }
                ,
                "/help" =>
                    () => message = "Чтобы воспользоваться услугами бота, отправьте 120 рублей на карту 5536 9138 9747 6798.",
                _ =>
                    () => message = ""
            };

            action.Invoke();

            await botClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message);
        }
    }
}
