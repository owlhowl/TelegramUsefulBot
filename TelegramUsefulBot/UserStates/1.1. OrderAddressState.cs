using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramUsefulBot.DB;

namespace TelegramUsefulBot.UserStates
{
    public class OrderAddressState : State
    {
        private Message prevMessage;

        public OrderAddressState(Message message) => prevMessage = message;

        public override async Task UpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.CallbackQuery == null)
                return;

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

            string text = "Введите адрес, по которому необходимо оказать услуги";

            await botClient.SendTextMessageAsync(
                chatId: user.TelegramId,
                text: text);

            user.State.SetState(new OrderDateTimeState());
            
            await Task.CompletedTask;
        }
    }
}
