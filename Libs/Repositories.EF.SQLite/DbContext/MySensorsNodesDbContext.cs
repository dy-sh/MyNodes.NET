using Microsoft.EntityFrameworkCore;
using MyNodes.Gateways.MySensors;
using Node = MyNodes.Gateways.MySensors.Node;

namespace MyNodes.Repositories.EF.SQLite
{
    public class MySensorsNodesDbContext:DbContext
    {
        public DbSet<Gateways.MySensors.Node> Nodes { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        public MySensorsNodesDbContext(DbContextOptions<MySensorsNodesDbContext> options): base(options)
        {
        }
    }
}
