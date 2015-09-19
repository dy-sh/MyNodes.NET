/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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
        public MyNetSensorsDbContext(string connectionString) : base(connectionString) { }
  
        public DbSet<Message>  Messages { get; set; }
        public DbSet<Node>  Nodes { get; set; }
        public DbSet<Sensor>  Sensors { get; set; }
    }
}
