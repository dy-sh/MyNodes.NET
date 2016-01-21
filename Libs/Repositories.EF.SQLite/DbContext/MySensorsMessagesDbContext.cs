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
using MyNetSensors.Gateways.MySensors.Serial;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class MySensorsMessagesDbContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }

    }
}
