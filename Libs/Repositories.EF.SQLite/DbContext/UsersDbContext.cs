using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using MyNetSensors.Gateways;
using MyNetSensors.Nodes;
using MyNetSensors.Users;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
    }
}
