using Microsoft.Data.Entity;
using MyNodes.Nodes;

namespace MyNodes.Repositories.EF.SQLite
{
    public class NodesDbContext:DbContext
    {
        public DbSet<SerializedNode> SerializedNodes { get; set; }
        public DbSet<Link> Links { get; set; }
    }
}
