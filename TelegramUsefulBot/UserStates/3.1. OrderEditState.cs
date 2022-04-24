using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderEditState : State
    {
        private Order order;

        public OrderEditState(Order order) => this.order = order;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery != null)
            {
                string text =
                    $"❗️Ваш заказ:\n\n" +
                    $"<b>{order.ServiceType.Name}</b>\n" +
                    $"по адресу <b>{order.Address}</b>\n\n" +
                    $"<b>{ToLocalDateString(order.StartDateTime)}</b> " +
                    $"с <b><u>{order.StartDateTime.ToShortTimeString()}</u></b> " +
                    $"до <b><u>{order.EndDateTime.ToShortTimeString()}</u></b>\n\n" +
                    $"К Вам приедет <b>{order.Worker.Name}</b>\n" +
                    $"тел. +{order.Worker.PhoneNumber}\n\n";

                ReplyKeyboardMarkup replyKeyboardMarkup = new(
                    new List<List<KeyboardButton>> {
                        new List<KeyboardButton> { new KeyboardButton("Отменить заказ") },
                        new List<KeyboardButton> { new KeyboardButton("Назад") }
                    });

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: text,
                    parseMode: ParseMode.Html,
                    replyMarkup: replyKeyboardMarkup);


            }
            else if (update.Message != null)
            {
                if (update.Message.Text == "Отменить заказ")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: "Заказ успешно отменен",
                        replyMarkup: new ReplyKeyboardRemove());

                    BotDB.DeleteOrder(order);

                    user.State.SetState(new DefaultState());
                }

                if (update.Message.Text == "Назад")
                {
                    await botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        text: "Вы вернулись назад",
                        replyMarkup: new ReplyKeyboardRemove());

                    user.State.SetState(new DefaultState());
                }

                await Task.CompletedTask;
            }
        }
    }
}
