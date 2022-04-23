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
    public class OrderListState : State
    {
        private int ordersPerPage = 6;
        private int page = 1;
        private Message prevMessage;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message != null)
            {
                if (await CommandHandler(user, null, botClient, update))
                    return;

                var userOrders = new List<Order>(BotDB.GetOrders().FindAll(o => o.UserId == user.Id && o.EndDateTime > DateTime.Now));

                var keyboardMarkup = GetOrderListKeyboard(userOrders);

                prevMessage = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: "Список ваших текущих заказов: \n(назад: /cancel)",
                    replyMarkup: keyboardMarkup);
            }
            else if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data == "next")
                    page++;

                else if (update.CallbackQuery.Data == "prev")
                    page--;

                else
                {
                    Order order = BotDB.GetOrder(int.Parse(update.CallbackQuery.Data));

                    await botClient.EditMessageReplyMarkupAsync(
                        chatId: user.TelegramId,
                        messageId: prevMessage.MessageId,
                        replyMarkup: null);

                    await botClient.DeleteMessageAsync(
                        chatId: user.TelegramId,
                        messageId: prevMessage.MessageId);

                    user.State.SetState(new OrderEditState(order));
                    user.State.UpdateHandler(botClient, update);

                    await Task.CompletedTask;
                    return;
                }

                var userOrders = new List<Order>(BotDB.GetOrders()).FindAll(o => o.UserId == user.Id && o.EndDateTime > DateTime.Now);

                var keyboardMarkup = GetOrderListKeyboard(userOrders);

                await botClient.EditMessageReplyMarkupAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    replyMarkup: keyboardMarkup);
            }

            await Task.CompletedTask;
        }

        private InlineKeyboardMarkup GetOrderListKeyboard(List<Order> orders)
        {
            orders.Sort((o1, o2) => o1.StartDateTime.CompareTo(o2.StartDateTime));
            var pageOrders = orders.FindAll(o => orders.IndexOf(o) < page * ordersPerPage && orders.IndexOf(o) >= (page-1) * ordersPerPage);

            var keyboard = new List<List<InlineKeyboardButton>>();

            foreach (var order in pageOrders)
            {
                string address = order.Address.Length > 28 ? order.Address.Substring(0, 20).TrimEnd(): order.Address;
                string serviceType = order.ServiceType.Name.Length > 10 ? order.ServiceType.Name.Substring(0, 10).TrimEnd() + "..." : order.ServiceType.Name;
                string buttonText = $"{order.StartDateTime.ToShortTimeString()} {ToLocalDateString(order.StartDateTime)} | {serviceType} | {address}";

                keyboard.Add(new List<InlineKeyboardButton> { 
                    InlineKeyboardButton.WithCallbackData(buttonText, order.Id.ToString()) 
                });
            }

            if (orders.Count > ordersPerPage)
            {
                if (page == 1)
                    keyboard.Add(new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData("➡️", "next")
                    });

                if (page > 1 && page < orders.Count / ordersPerPage + 1)
                    keyboard.Add(new List<InlineKeyboardButton> {
                            InlineKeyboardButton.WithCallbackData("⬅️", "prev"),
                            InlineKeyboardButton.WithCallbackData("➡️", "next")
                        });

                if (page == orders.Count / ordersPerPage + 1)
                    keyboard.Add(new List<InlineKeyboardButton> {
                        InlineKeyboardButton.WithCallbackData("⬅️", "prev")
                    });
            }

            return new InlineKeyboardMarkup(keyboard);
        }
    }
}
