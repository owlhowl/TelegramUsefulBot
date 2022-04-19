using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramUsefulBot
{
    public static class Parser
    {
        public static async Task<Link[]> GetLinks(string searchText)
        {
            return Seed();
        }

        private static Link[] Seed()
        {
            return new Link[]
            {
                new Link() { Title = "Первый товар", Url = "vk.com" },
                new Link() { Title = "Второй товар", Url = "telegram.org" },
                new Link() { Title = "Третий товар", Url = "youtube.com" }
            };
        }
    }
}
