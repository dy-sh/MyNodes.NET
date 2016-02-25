using Microsoft.Data.Entity;
using MyNetSensors.Gateways.MySensors;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class MySensorsMessagesDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

    }
}
