using System;

#nullable disable

namespace TelegramUsefulBot
{
    public partial class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int WorkerId { get; set; }
        public int ServiceTypeId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string Address { get; set; }

        public virtual ServiceType ServiceType { get; set; }
        public virtual User User { get; set; }
        public virtual Worker Worker { get; set; }
    }
}
