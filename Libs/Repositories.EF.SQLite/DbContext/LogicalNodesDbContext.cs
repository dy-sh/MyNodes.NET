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
using MyNetSensors.Gateways;
using MyNetSensors.NodesTasks;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class LogicalNodesDbContext:DbContext
    {
        public DbSet<LogicalNodeSerialized> LogicalNodesSerialized { get; set; }
        public DbSet<LogicalLink> LogicalLinks { get; set; }
    }
}
