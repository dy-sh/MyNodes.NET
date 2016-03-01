/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNodes.Users;

namespace MyNodes.Repositories.EF.SQLite
{
    public class UsersRepositoryEf : IUsersRepository
    {
        private UsersDbContext db;

        public UsersRepositoryEf(UsersDbContext db)
        {
            this.db = db;
            CreateDb();

        }

        public void CreateDb()
        {
            try
            {
                db.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw ex;
            }
        }

        public int AddUser(User user)
        {
            db.Users.Add(user);
            db.SaveChanges();

            return user.Id;
        }

        public void UpdateUser(User user)
        {
            db.Users.Update(user);
            db.SaveChanges();
        }

        public User GetUser(int id)
        {
            return db.Users.FirstOrDefault(x => x.Id == id);
        }

        public User GetUser(string name)
        {
            return db.Users.FirstOrDefault(x => x.Name == name);
        }

        public List<User> GetAllUsers()
        {
            return db.Users.ToList();
        }

        public void RemoveUser(int id)
        {
            User user = GetUser(id);
            db.Users.Remove(user);
            db.SaveChanges();
        }

        public void RemoveAllUsers()
        {
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();
        }

        public void RemoveUsers(List<User> users)
        {
            db.Users.RemoveRange(users);
            db.SaveChanges();
        }

        public int GetUsersCount()
        {
            return db.Users.Count();
        }
    }
}
