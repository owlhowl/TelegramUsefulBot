using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderConfirmationState : State
    {
        private Message prevMessage;

        public OrderConfirmationState(Message message) => prevMessage = message;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery == null)
                return;

            user.CurrentOrder.StartDateTime = DateTime.Parse(update.CallbackQuery.Data);
            user.CurrentOrder.EndDateTime = user.CurrentOrder.StartDateTime.AddHours(1);

            await botClient.EditMessageReplyMarkupAsync(
                chatId: user.TelegramId,
                messageId: prevMessage.MessageId,
                replyMarkup: null);

            await botClient.EditMessageTextAsync(
                chatId: user.TelegramId,
                messageId: prevMessage.MessageId,
                parseMode: ParseMode.Markdown,
                text: $"{prevMessage.Text} *{user.CurrentOrder.StartDateTime.ToShortDateString()} {user.CurrentOrder.StartDateTime.ToShortTimeString()}*");

            foreach (var worker in BotDB.GetWorkers())
                if (!BotDB.GetOrders().Exists(o => o.StartDateTime == user.CurrentOrder.StartDateTime && o.WorkerId == worker.Id))
                {
                    user.CurrentOrder.Worker = worker;
                    break;
                }

            Order newOrder = user.CurrentOrder;

            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new List<List<KeyboardButton>> {
                    new List<KeyboardButton> { new KeyboardButton("Подтвердить") },
                    new List<KeyboardButton> { new KeyboardButton("Назад") }
                });

            string message = $"*Ваш заказ:*\n\n" +
                $"*{newOrder.ServiceType.Name}*\n" +
                $"по адресу *{newOrder.Address}*\n" +
                $"*{newOrder.StartDateTime.ToShortDateString()}* " +
                $"с *{newOrder.StartDateTime.ToShortTimeString()}* " +
                $"до *{newOrder.EndDateTime.ToShortTimeString()}*\n\n" +
                $"К Вам приедет {newOrder.Worker.Name}\n" +
                $"тел. +{newOrder.Worker.PhoneNumber}\n\n" +
                $"Все верно?";

            await botClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: message,
                parseMode: ParseMode.Markdown,
                replyMarkup: replyKeyboardMarkup);

            user.State.SetState(new FinalState());

            await Task.CompletedTask;
        }
    }
}
