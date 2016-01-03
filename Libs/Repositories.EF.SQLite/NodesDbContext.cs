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
    public class NodesDbContext:DbContext
    {
        public DbSet<LogicalNodeSerialized> LogicalNodesSerialized { get; set; }
        public DbSet<LogicalLink> LogicalLinks { get; set; }
        public DbSet<SensorData> SensorsData { get; set; }
        public DbSet<NodeTask> NodesTasks { get; set; }
        public DbSet<Node> Nodes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Sensor> Sensors { get; set; }

        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    //  builder.Entity<LogicalNodeSerialized>().HasKey(m => m.Id);
        //    // base.OnModelCreating(builder);
        //}
    }
}
