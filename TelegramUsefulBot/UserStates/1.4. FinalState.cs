using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class FinalState : State
    {
        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;

            if (await CommandHandler(user, null, botClient, update))
                return;

            if (update.Message.Text == "Подтвердить")
            {
                BotDB.AddOrder(
                    user.Id, 
                    user.CurrentOrder.ServiceType.Id, 
                    user.CurrentOrder.Worker.Id,
                    user.CurrentOrder.Address, 
                    user.CurrentOrder.StartDateTime, 
                    user.CurrentOrder.EndDateTime);

                string message = "Заказ успешно оформлен!";
                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: message,
                    replyMarkup: new ReplyKeyboardRemove());

                user.CurrentOrder = new Order();
                user.State.SetState(new OrderMakeState());
            }

            else if (update.Message.Text == "Назад")
            {
                user.State.SetState(new OrderDateTimeState());
                user.State.UpdateHandler(botClient, update);

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: "Вы вернулись назад",
                    replyMarkup: new ReplyKeyboardRemove());
            }

            await Task.CompletedTask;
        }
    }
}
