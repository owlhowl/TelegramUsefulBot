using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramUsefulBot.UserStates;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        protected Dictionary<string, Func<User, string>> commands = new Dictionary<string, Func<User, string>>() 
        {
            { 
                "/start", (user) =>
                {
                    user.State.SetState(new DefaultState());
                    return "Привет! Я умею делать заказы на оказание различных услуг!\nЧтобы узнать больше, введите /help";
                } 
            },
            { 
                "/cancel", (user) =>
                {                        
                    if (user.State.GetState() is OrderListState or OrderEditState)
                    {
                        user.State.SetState(new DefaultState());
                        return "Вы вернулись назад";
                    }
                    else
                    {
                        user.State.SetState(new DefaultState());
                        user.CurrentOrder = new Order();
                        return "Оформление заказа отменено";
                    }
                }
            },
            { 
                "/help", (user) =>
                {
                    return "*Список доступных команд бота:*\n" +
                    "/order - начать оформление заказа\n" +
                    "/list - список текущих заказов\n" +
                    "/cancel - отмена текущего действия";
                }
            }
        };

        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);

        protected async Task<bool> CommandHandler(User user, Message prevMessage, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return false;

            foreach (var command in commands)
            {
                if (update.Message.Text == command.Key)
                {
                    await botClient.SendTextMessageAsync(
                        chatId: user.TelegramId,
                        parseMode: ParseMode.Markdown,
                        text: command.Value.Invoke(user));

                    if (prevMessage != null)
                    {
                        await botClient.EditMessageReplyMarkupAsync(
                            chatId: user.TelegramId,
                            messageId: prevMessage.MessageId,
                            replyMarkup: null);

                        await botClient.EditMessageTextAsync(
                            chatId: user.TelegramId,
                            messageId: prevMessage.MessageId,
                            parseMode: ParseMode.Markdown,
                            text: prevMessage.Text + " *Отмена*");
                    }

                    return true;
                }
            }

            return false;
        }

        protected string ToLocalDateString(DateTime dt)
        {
            string day = dt.Day > 10 ? dt.Day.ToString() : "0" + dt.Day;
            string month = dt.Month > 10 ? dt.Month.ToString() : "0" + dt.Month;
            string year = dt.Year.ToString();

            return $"{day}.{month}.{year}";
        } 
    }
}
