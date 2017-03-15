using Microsoft.EntityFrameworkCore;
using MyNodes.Gateways.MySensors;

namespace MyNodes.Repositories.EF.SQLite
{
    public class MySensorsMessagesDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

        public MySensorsMessagesDbContext(DbContextOptions<MySensorsMessagesDbContext> options): base(options)
        {
        }
    }
}
