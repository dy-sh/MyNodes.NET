using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.SerialController_Console
{
    class MyNetSensorsDbContext : DbContext
    {
        public MyNetSensorsDbContext() : base("name=DbConnection") { }
  
        public DbSet<Message>  Messages { get; set; }
        public DbSet<Node>  Nodes { get; set; }
        public DbSet<Sensor>  Sensors { get; set; }
        public DbSet<SensorData>  SensorDatas{ get; set; }
    }
}
