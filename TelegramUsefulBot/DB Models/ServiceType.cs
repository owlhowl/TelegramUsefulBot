using System;
using System.Collections.Generic;

#nullable disable

namespace TelegramUsefulBot
{
    public partial class ServiceType
    {
        public ServiceType()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
