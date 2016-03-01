using Microsoft.Data.Entity;
using MyNodes.Gateways.MySensors;
using Node = MyNodes.Gateways.MySensors.Node;

namespace MyNodes.Repositories.EF.SQLite
{
    public class MySensorsNodesDbContext:DbContext
    {
        public DbSet<Gateways.MySensors.Node> Nodes { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
    }
}
