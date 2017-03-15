using Microsoft.EntityFrameworkCore;
using MyNodes.Nodes;

namespace MyNodes.Repositories.EF.SQLite
{
    public class NodesDataDbContext : DbContext
    {
        public DbSet<NodeData> NodesData { get; set; }

        public NodesDataDbContext(DbContextOptions<NodesDataDbContext> options): base(options)
        {
        }
    }
}
