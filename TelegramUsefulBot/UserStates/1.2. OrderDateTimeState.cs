using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderDateTimeState : State
    {
        private DateTime selectedDay = DateTime.Now;
        private Message prevMessage;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, botClient, update))
                return;

            if (update.Message != null && string.IsNullOrEmpty(user.CurrentOrder.Address))
                user.CurrentOrder.Address = update.Message.Text;
            
            if(update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data == "nextDay")
                    selectedDay = selectedDay.AddDays(1);

                else if (update.CallbackQuery.Data == "prevDay")
                    selectedDay = selectedDay.AddDays(-1);

                else
                {
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
                        text: $"{prevMessage.Text} *{user.CurrentOrder.StartDateTime.ToShortTimeString()}*");

                    user.State.SetState(new OrderConfirmationState(prevMessage));
                    user.State.UpdateHandler(botClient, update);

                    await Task.CompletedTask;
                    return;
                }

                await botClient.EditMessageTextAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    parseMode: ParseMode.Markdown,
                    text: $"Выберите время на *{selectedDay.ToShortDateString()}*:");

                InlineKeyboardMarkup replyKeyboardMarkup = GetDateTimeKeyboard(GetDateTimes(user.CurrentOrder.ServiceType), selectedDay);

                await botClient.EditMessageReplyMarkupAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    replyMarkup: replyKeyboardMarkup);

                user.State.SetState(this);
            }
            else
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = GetDateTimeKeyboard(GetDateTimes(user.CurrentOrder.ServiceType), selectedDay);

                prevMessage = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    parseMode: ParseMode.Markdown,
                    text: $"Выберите время на *{selectedDay.ToShortDateString()}*:",
                    replyMarkup: inlineKeyboardMarkup);
            }

            await Task.CompletedTask;
        }

        protected InlineKeyboardMarkup GetDateTimeKeyboard(List<DateTime> dateTimes, DateTime selectedDay)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            dateTimes = dateTimes.FindAll(d => d.Day == selectedDay.Day);

            if (dateTimes.Count % 2 == 0)
            {
                foreach (var dt in dateTimes)
                {
                    int index = dateTimes.IndexOf(dt);

                    if (index % 2 == 0)
                    {
                        keyboard.Add(new List<InlineKeyboardButton> {
                            InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString()),
                            InlineKeyboardButton.WithCallbackData($"{dateTimes[index+1].ToShortTimeString()}", dateTimes[index+1].ToString())
                        });
                    }
                }
            }

            else
            {
                foreach (var dt in dateTimes)
                {
                    int index = dateTimes.IndexOf(dt);

                    if (index % 2 == 0)
                    {
                        if (dt == dateTimes.Last())
                            keyboard.Add(new List<InlineKeyboardButton> {
                                InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString())
                            });
                        else
                            keyboard.Add(new List<InlineKeyboardButton> {
                                InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString()),
                                InlineKeyboardButton.WithCallbackData($"{dateTimes[index+1].ToShortTimeString()}", dateTimes[index+1].ToString())
                            });
                    }
                }
            }

            keyboard.Add(new List<InlineKeyboardButton> {
                InlineKeyboardButton.WithCallbackData("⬅️", "prevDay"),
                InlineKeyboardButton.WithCallbackData("➡️", "nextDay")
            });

            return new InlineKeyboardMarkup(keyboard);
        }

        protected List<DateTime> GetDateTimes(ServiceType serviceType)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            var orders = BotDB.GetOrders();

            var now = DateTime.Now;

            for (int day = 0; day < 10; day++)
            {
                for (int hour = 9; hour <= 20; hour++)
                {
                    var newTime = new DateTime(now.Year, now.Month, now.Day, hour, 0, 0).AddDays(day);

                    dateTimes.Add(newTime);
                }
            }

            var workers = BotDB.GetWorkers();
            var formattedDateTimes = new List<DateTime>(dateTimes);
            
            foreach (var time in dateTimes)
            {
                int countBusy = 0;

                foreach (var worker in workers)
                    if (orders.Exists(o => o.StartDateTime == time && o.WorkerId == worker.Id))
                        countBusy++;

                if (DateTime.Now > time || (countBusy == workers.Count && orders.Exists(o => o.StartDateTime == time)))
                    formattedDateTimes.Remove(time);
            }

            return formattedDateTimes;
        }
    }
}
