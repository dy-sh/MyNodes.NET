using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MyNetSensors.Users;

namespace MyNetSensors.Repositories.Dapper
{
    public class UsersRepositoryDapper : IUsersRepository
    {
        private string connectionString;

        public UsersRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
            CreateDb();

        }

        private void CreateDb()
        {
            using (var db = new SqlConnection(connectionString + ";Database= master"))
            {

                try
                {
                    db.Open();
                    db.Execute("CREATE DATABASE [Users]");
                }
                catch
                {
                }
            }

            using (var db = new SqlConnection(connectionString))
            {

                try
                {
                    db.Open();

                    db.Execute(
                        @"CREATE TABLE [dbo].[Users](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [Name] [nvarchar](max) NULL,
	                    [Email] [nvarchar](max) NULL,
	                    [Password] [nvarchar](max) NULL) ON [PRIMARY] ");
                }
                catch (Exception ex)
                {
                }
            }
        }

        public int AddUser(User user)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery = "INSERT INTO [Users] (Name, Email, Password) "
                               +
                               "VALUES(@Name, @Email, @Password); "
                               + "SELECT CAST(SCOPE_IDENTITY() as int)";
                return db.Query<int>(sqlQuery, user).Single();
            }
        }

        public void UpdateUser(User user)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE [Users] SET " +
                    "Name = @Name, " +
                    "Email  = @Email, " +
                    "Password = @Password " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, user);
            }
        }

        public User GetUser(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<User>($"SELECT * FROM [Users] WHERE Id=@Id", id).SingleOrDefault();

            }
        }

        public User GetUser(string name)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<User>($"SELECT * FROM [Users] WHERE Name=@Name", name).SingleOrDefault();

            }
        }

        public List<User> GetAllUsers()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<User>($"SELECT * FROM [Users]").ToList();
            }
        }

        public void RemoveUser(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query($"DELETE FROM [Users] WHERE Id=@Id",id);
            }
        }

        public void RemoveAllUsers()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [Users]");
            }
        }

        public int GetUsersCount()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                return db.Query<int>("SELECT COUNT(*) FROM [Users]").Single();
            }
        }
    }
}
