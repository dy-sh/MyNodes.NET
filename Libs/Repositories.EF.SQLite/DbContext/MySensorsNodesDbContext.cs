using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using MyNetSensors.Gateways;
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
