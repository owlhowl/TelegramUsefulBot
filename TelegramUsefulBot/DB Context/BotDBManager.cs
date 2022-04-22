using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramUsefulBot.DB
{
    public class BotDB
    {
        private readonly BotDBContext dbContext;
        private List<User> users;
        private List<Order> orders;
        private List<Worker> workers;
        private List<ServiceType> serviceTypes;

        public BotDB()
        {
            dbContext = BotDBContext.GetDB();
            users = new List<User>(dbContext.Users);
            orders = new List<Order>(dbContext.Orders);
        }

        public bool HasUser(long id) => users.Exists(u => u.TelegramId == id);

        public User GetUser(long id) => users.Find(u => u.TelegramId == id);

        public User AddUser(long telegramId, string name, long phoneNumber = -1)
        {
            if (HasUser(telegramId))
                return null;

            var user = new User { TelegramId = telegramId, Name = name, PhoneNumber = phoneNumber };

            dbContext.Add(user);
            dbContext.SaveChanges();

            users = new List<User>(dbContext.Users);

            return user;
        }

        public Order GetOrder(long id) => orders.Find(o => o.Id == id);

        public Order AddOrder(int userId, int serviceTypeId, DateTime start, DateTime end)
        {
            int workerId = 0;

            foreach (var worker in workers)
            {
                foreach (var order in orders.Where(o => o.WorkerId == worker.Id))
                {
                    if (start >= order.StartDateTime && start <= order.EndDateTime)
                        continue;

                    if (start < order.StartDateTime && end < order.StartDateTime || start > order.EndDateTime && end > order.EndDateTime)
                        workerId = worker.Id;


                }
            }

            var newOrder = new Order { UserId = userId, WorkerId = workerId, ServiceTypeId = serviceTypeId, StartDateTime = start, EndDateTime = end };

            dbContext.Add(newOrder);
            dbContext.SaveChanges();

            orders = new List<Order>(dbContext.Orders);

            return newOrder;
        }
    }
}
