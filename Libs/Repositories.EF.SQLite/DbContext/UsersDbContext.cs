using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyNodes.Gateways;
using MyNodes.Nodes;
using MyNodes.Users;

namespace MyNodes.Repositories.EF.SQLite
{
    public class UsersDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options): base(options)
        {
        }
    }
}
