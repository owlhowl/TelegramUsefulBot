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
            State = new StateMachine(this, new DefaultState());
        }

        public int Id { get; set; }
        public long TelegramId { get; set; }
        public string Name { get; set; }
        public long PhoneNumber { get; set; }
        
        [NotMapped]
        public StateMachine State { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
