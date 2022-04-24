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
            if (await CommandHandler(user, prevMessage, botClient, update))
                return;

            if (update.CallbackQuery != null)
            {
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

                string text = $"Ваш заказ:\n\n" +
                    $"*{newOrder.ServiceType.Name}*\n" +
                    $"по адресу *{newOrder.Address}*\n" +
                    $"*{ToLocalDateString(newOrder.StartDateTime)}* " +
                    $"с *{newOrder.StartDateTime.ToShortTimeString()}* " +
                    $"до *{newOrder.EndDateTime.ToShortTimeString()}*\n\n" +
                    $"К Вам приедет {newOrder.Worker.Name}\n" +
                    $"тел. +{newOrder.Worker.PhoneNumber}\n\n" +
                    $"Все верно?";

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: text,
                    parseMode: ParseMode.Markdown,
                    replyMarkup: replyKeyboardMarkup);
            }

            else if (update.Message != null)
            {
                if (update.Message.Text == "Подтвердить")
                {
                    BotDB.AddOrder(
                        user.Id,
                        user.CurrentOrder.ServiceType.Id,
                        user.CurrentOrder.Worker.Id,
                        user.CurrentOrder.Address,
                        user.CurrentOrder.StartDateTime,
                        user.CurrentOrder.EndDateTime);

                    await botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: "Заказ успешно оформлен!",
                        replyMarkup: new ReplyKeyboardRemove());

                    user.CurrentOrder = new Order();

                    user.State.SetState(new DefaultState());
                }

                else if (update.Message.Text == "Назад")
                {
                    var backMessage = await botClient.SendTextMessageAsync(
                       chatId: user.TelegramId,
                       text: "Возвращаем назад...",
                       replyMarkup: new ReplyKeyboardRemove());

                    await Task.Delay(500);

                    await botClient.DeleteMessageAsync(
                        chatId: user.TelegramId,
                        messageId: backMessage.MessageId);

                    user.State.SetState(new OrderDateTimeState());
                    user.State.UpdateHandler(botClient, update);
                }
            }

            await Task.CompletedTask;
        }
    }
}
