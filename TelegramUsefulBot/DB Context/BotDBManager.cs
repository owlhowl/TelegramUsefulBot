using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramUsefulBot.DB
{
    public static class BotDB
    {
        private static readonly BotDBContext dbContext = BotDBContext.GetDB();
        private static List<User> users = new List<User>(dbContext.Users);
        private static List<Order> orders = new List<Order>(dbContext.Orders);
        private static List<Worker> workers = new List<Worker>(dbContext.Workers);
        private static List<ServiceType> serviceTypes = new List<ServiceType>(dbContext.ServiceTypes);

        public static bool HasUser(long telegramId) => users.Exists(u => u.TelegramId == telegramId);
        public static User GetUser(long telegramId) => users.Find(u => u.TelegramId == telegramId);

        public static User AddUser(long telegramId, string name)
        {
            if (HasUser(telegramId))
                return null;

            var user = new User { TelegramId = telegramId, Name = name };

            dbContext.Add(user);
            dbContext.SaveChanges();

            users = new List<User>(dbContext.Users);

            return user;
        }

        public static List<Order> GetOrders() => orders;
        public static Order GetOrder(long id) => orders.Find(o => o.Id == id);
        public static Order AddOrder(int userId, int serviceTypeId, int workerId, string address, DateTime start, DateTime end)
        {
            var newOrder = new Order { 
                UserId = userId, 
                WorkerId = workerId, 
                ServiceTypeId = serviceTypeId, 
                StartDateTime = start, 
                EndDateTime = end,
                Address = address
            };

            dbContext.Add(newOrder);
            dbContext.SaveChanges();

            orders = new List<Order>(dbContext.Orders);

            return newOrder;
        }

        public static List<ServiceType> GetServiceTypes() => serviceTypes;

        public static List<Worker> GetWorkers() => workers;
    }
}
