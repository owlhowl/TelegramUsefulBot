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
        private int dayOffset;
        private Message prevMessage;

        public OrderDateTimeState()
        {
            if (!GetDateTimes().Exists(d => d.Day == DateTime.Now.Day))
                dayOffset = 1;
        }

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, prevMessage, botClient, update))
                return;

            if (update.Message != null)
            {
                InlineKeyboardMarkup inlineKeyboardMarkup = GetDateTimeKeyboard(GetDateTimes());

                prevMessage = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    parseMode: ParseMode.Markdown,
                    text: $"Выберите время на *{ToLocalDateString(DateTime.Now.AddDays(dayOffset))}*:",
                    replyMarkup: inlineKeyboardMarkup);
            }

            else if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data == "nextDay")
                    dayOffset++;

                else if (update.CallbackQuery.Data == "prevDay")
                    dayOffset--;

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
                    text: $"Выберите время на *{ToLocalDateString(DateTime.Now.AddDays(dayOffset))}*:");

                InlineKeyboardMarkup replyKeyboardMarkup = GetDateTimeKeyboard(GetDateTimes());

                await botClient.EditMessageReplyMarkupAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    replyMarkup: replyKeyboardMarkup);
            }

            await Task.CompletedTask;
        }

        private InlineKeyboardMarkup GetDateTimeKeyboard(List<DateTime> dateTimes)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            var pageDateTimes = dateTimes.FindAll(d => d.Day == DateTime.Now.AddDays(dayOffset).Day);

            if (pageDateTimes.Count % 2 == 0)
            {
                foreach (var dt in pageDateTimes)
                {
                    int index = pageDateTimes.IndexOf(dt);

                    if (index % 2 == 0)
                    {
                        keyboard.Add(new List<InlineKeyboardButton> {
                            InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString()),
                            InlineKeyboardButton.WithCallbackData($"{pageDateTimes[index+1].ToShortTimeString()}", pageDateTimes[index+1].ToString())
                        });
                    }
                }
            }
            else
            {
                foreach (var dt in pageDateTimes)
                {
                    int index = pageDateTimes.IndexOf(dt);

                    if (index % 2 == 0)
                    {
                        if (dt == pageDateTimes.Last())
                            keyboard.Add(new List<InlineKeyboardButton> {
                                InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString())
                            });
                        else
                            keyboard.Add(new List<InlineKeyboardButton> {
                                InlineKeyboardButton.WithCallbackData($"{dt.ToShortTimeString()}", dt.ToString()),
                                InlineKeyboardButton.WithCallbackData($"{pageDateTimes[index+1].ToShortTimeString()}", pageDateTimes[index+1].ToString())
                            });
                    }
                }
            }

            if (dateTimes.Exists(d => d.Day == DateTime.Now.AddDays(dayOffset + 1).Day) &&
                dateTimes.Exists(d => d.Day == DateTime.Now.AddDays(dayOffset - 1).Day))
            {
                keyboard.Add(new List<InlineKeyboardButton> {
                    InlineKeyboardButton.WithCallbackData("⬅️", "prevDay"),
                    InlineKeyboardButton.WithCallbackData("➡️", "nextDay")
                });
            }

            else if (dateTimes.Exists(d => d.Day == DateTime.Now.AddDays(dayOffset + 1).Day))
            {
                keyboard.Add(new List<InlineKeyboardButton> {
                    InlineKeyboardButton.WithCallbackData("➡️", "nextDay")
                });
            }

            else if (dateTimes.Exists(d => d.Day == DateTime.Now.AddDays(dayOffset - 1).Day))
            {
                keyboard.Add(new List<InlineKeyboardButton> {
                    InlineKeyboardButton.WithCallbackData("⬅️", "prevDay")
                });
            }

            return new InlineKeyboardMarkup(keyboard);
        }

        private List<DateTime> GetDateTimes()
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
