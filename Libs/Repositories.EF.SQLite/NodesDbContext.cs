using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using MyNetSensors.LogicalNodes;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesDbContext:DbContext
    {
        public DbSet<LogicalNodeSerialized> LogicalNodesSerialized { get; set; }
        public DbSet<LogicalLink> LogicalLinks { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<LogicalNodeSerialized>().HasKey(m => m.Id);
        //    base.OnModelCreating(builder);
        //}
    }
}
