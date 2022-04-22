using Microsoft.EntityFrameworkCore;

namespace TelegramUsefulBot.DB
{
    public partial class BotDBContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Worker>().HasData(
                new Worker { Id = 1, Name = "Алексей", PhoneNumber = 79992223344 },
                new Worker { Id = 2, Name = "Сергей", PhoneNumber = 79886662211 },
                new Worker { Id = 3, Name = "Николай", PhoneNumber = 79771112525 }
                );

            modelBuilder.Entity<ServiceType>().HasData(
                new ServiceType { Id = 1, Name = "Сантехнический ремонт"},
                new ServiceType { Id = 2, Name = "Ремонт автомобиля"},
                new ServiceType { Id = 3, Name = "Сварочные работы"}
                );
        }
    }
}
