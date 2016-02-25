using Microsoft.Data.Entity;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesDbContext:DbContext
    {
        public DbSet<SerializedNode> SerializedNodes { get; set; }
        public DbSet<Link> Links { get; set; }
    }
}
