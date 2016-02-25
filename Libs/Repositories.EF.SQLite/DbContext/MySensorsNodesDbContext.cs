using Microsoft.Data.Entity;
using MyNetSensors.Gateways.MySensors;
using Node = MyNetSensors.Gateways.MySensors.Node;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class MySensorsNodesDbContext:DbContext
    {
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
    }
}
