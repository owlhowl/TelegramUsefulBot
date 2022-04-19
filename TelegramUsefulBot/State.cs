using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramUsefulBot
{
    public abstract class State
    {
        public abstract Task UpdateHandler(User user, ITelegramBotClient botClient, Update update);

        public async Task CommonUpdateHandler(User user, ITelegramBotClient botClient, Update update)
        {
            if (update.Message == null)
                return;

            string message = update.Message.Text switch
            {
                "/start" => "Привет! Я пока ничего не умею!",
                "/help" => "Чтобы воспользоваться услугами бота, отправьте 120 рублей на карту 5536 9138 9747 6798.",
                "/find" => "Введите /find [название товара]",
                _ => ""
            };

            if (update.Message.Text.Split(' ')[0] == "/find" && update.Message.Text.Split(' ').Length > 1)
            {
                string searchText = update.Message.Text.Replace("/find ", "");
                var links = await Parser.GetLinks("test");
                InlineKeyboardMarkup replyKeyboardMarkup = GetListKeyboard(links);

                string text = links.Length == 1 ? $"По запросу {searchText} найден 1 товар:" : 
                    links.Length > 1 && links.Length < 5 ? $"По запросу {searchText} найдено {links.Length} товара:" : 
                    links.Length > 4 ? $"По запросу {searchText} найдено {links.Length} товаров:" : "";

                await botClient.SendTextMessageAsync(
                    chatId: user.Id,
                    text: text,
                    replyMarkup: replyKeyboardMarkup);

                return;
            }

            await botClient.SendTextMessageAsync(
                chatId: user.Id,
                text: message);
        }

        private static InlineKeyboardMarkup GetListKeyboard(IEnumerable<Link> links)
        {
            var keyboard = new List<List<InlineKeyboardButton>>();

            foreach (var link in links)
                keyboard.Add(new List<InlineKeyboardButton> { InlineKeyboardButton.WithCallbackData(link.Title, link.Url) });

            var replyKeyboardMarkup = new InlineKeyboardMarkup(keyboard);
            return replyKeyboardMarkup;
        }
    }
}
