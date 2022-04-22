using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TelegramUsefulBot.UserStates;

#nullable disable

namespace TelegramUsefulBot
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            State = new StateMachine(this, new OrderMakeState());
            CurrentOrder = new Order();
        }

        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string Name { get; set; }
        
        [NotMapped]
        public StateMachine State { get; set; }
        [NotMapped]
        public Order CurrentOrder { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
