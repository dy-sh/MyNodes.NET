using Microsoft.Data.Entity;
using MyNodes.Gateways.MySensors;

namespace MyNodes.Repositories.EF.SQLite
{
    public class MySensorsMessagesDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

    }
}
