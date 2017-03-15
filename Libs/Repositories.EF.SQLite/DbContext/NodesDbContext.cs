using Microsoft.EntityFrameworkCore;
using MyNodes.Nodes;

namespace MyNodes.Repositories.EF.SQLite
{
    public class NodesDbContext: DbContext
    {
        public DbSet<SerializedNode> SerializedNodes { get; set; }
        public DbSet<Link> Links { get; set; }

        public NodesDbContext(DbContextOptions<NodesDbContext> options): base(options)
        {
        }
    }
}
