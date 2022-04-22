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

                var message = await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: "Назад",
                    replyMarkup: new ReplyKeyboardRemove());

                await botClient.DeleteMessageAsync(
                    chatId: user.TelegramId,
                    messageId: message.MessageId);
            }

            await Task.CompletedTask;
        }
    }
}
