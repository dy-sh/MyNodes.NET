using Microsoft.Data.Entity;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesDataDbContext : DbContext
    {
        public DbSet<NodeData> NodesData { get; set; }
    }
}
