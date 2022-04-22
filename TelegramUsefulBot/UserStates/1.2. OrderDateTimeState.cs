using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderDateTimeState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;

            if (string.IsNullOrEmpty(user.CurrentOrder.Address))
                user.CurrentOrder.Address = update.Message.Text;

            string text = "Выберите время:";

            InlineKeyboardMarkup inlineKeyboardMarkup = GetDateTimeKeyboard(GetDateTimes(user.CurrentOrder.ServiceType));

            var message = await botClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: text,
                replyMarkup: inlineKeyboardMarkup);

            user.State.SetState(new OrderConfirmationState(message));

            await Task.CompletedTask;
        }

        private InlineKeyboardMarkup GetDateTimeKeyboard(List<DateTime> dateTimes)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            for (int i = 0; i < 6; i++)
                keyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(dateTimes[i].ToString(), dateTimes[i].ToString()) });

            return new InlineKeyboardMarkup(keyboard); ;
        }

        private List<DateTime> GetDateTimes(ServiceType serviceType)
        {
            List<DateTime> dateTimes = new List<DateTime>();
            var orders = BotDB.GetOrders();

            var now = DateTime.Now;

            for (int day = 0; day < 10; day++)
            {
                for (int hour = 10; hour <= 20; hour++)
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
