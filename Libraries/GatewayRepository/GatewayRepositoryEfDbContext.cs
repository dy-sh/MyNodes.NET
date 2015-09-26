/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Data.Entity;
using MyNetSensors.Gateway;

namespace MyNetSensors.GatewayRepository
{
    class GatewayRepositoryEfDbContext : DbContext
    {
        public GatewayRepositoryEfDbContext(string connectionString) : base(connectionString) { }
  
        public DbSet<Message>  Messages { get; set; }
        public DbSet<Node>  Nodes { get; set; }
        public DbSet<Sensor>  Sensors { get; set; }
    }
}
