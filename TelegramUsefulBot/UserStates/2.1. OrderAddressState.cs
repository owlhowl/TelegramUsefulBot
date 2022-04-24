using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderAddressState : State
    {
        private Message prevMessage;

        public OrderAddressState(Message message) => prevMessage = message;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (await CommandHandler(user, prevMessage, botClient, update))
                return;

            if (update.CallbackQuery != null)
            {
                user.CurrentOrder.ServiceType = BotDB.GetServiceTypes().Find(t => t.Id == int.Parse(update.CallbackQuery.Data));

                await botClient.EditMessageReplyMarkupAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    replyMarkup: null);

                await botClient.EditMessageTextAsync(
                    chatId: user.TelegramId,
                    messageId: prevMessage.MessageId,
                    parseMode: ParseMode.Markdown,
                    text: $"{prevMessage.Text} *{user.CurrentOrder.ServiceType.Name}*");

                await botClient.SendTextMessageAsync(
                    chatId: user.TelegramId,
                    text: "Введите адрес, по которому необходимо оказать услуги");
            }

            else if (update.Message != null)
            {
                user.CurrentOrder.Address = update.Message.Text;
                user.State.SetState(new OrderDateTimeState());
                user.State.UpdateHandler(botClient, update);
            }

            await Task.CompletedTask;
        }
    }
}
