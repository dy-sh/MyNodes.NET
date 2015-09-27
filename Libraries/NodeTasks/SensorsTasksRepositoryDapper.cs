using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;
using Dapper;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public class SensorsTasksRepositoryDapper : ISensorsTasksRepository
    {

        private string connectionString;

        public SensorsTasksRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public void CreateDb()
        {
            CreateSensorsTasksTable();
        }

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }


        public int AddOrUpdateTask(SensorTask task)
        {
            int db_Id=task.db_Id;

            SensorTask oldTask = GetTask(task.db_Id);

            if (oldTask == null)
                db_Id= AddTask(task);
            else
                UpdateTask(task);

            return db_Id;
        }

        public int AddTask(SensorTask task)
        {
            int db_Id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO SensorsTasks (description, nodeId, sensorId, sensorDbId, executionDate,dataType, executionValue, isCompleted, isRepeating ,repeatingInterval,repeatingAValue,repeatingBValue,repeatingCount,executionsDoneCount) "
                             + "VALUES(@description, @nodeId, @sensorId, @sensorDbId, @executionDate, @dataType, @executionValue, @isCompleted, @isRepeating, @repeatingInterval, @repeatingAValue, @repeatingBValue, @repeatingCount,@executionsDoneCount); "
                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                db_Id = db.Query<int>(sqlQuery, task).Single();
            }
            return db_Id;
        }

        public void UpdateTask(SensorTask task)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE SensorsTasks SET " +
                    "description = @description, " +
                    "executionDate = @executionDate, " +
                    "dataType = @dataType, " +
                    "executionValue = @executionValue, " +
                    "isCompleted = @isCompleted, " +
                    "isRepeating = @isRepeating, " +
                    "repeatingInterval = @repeatingInterval, " +
                    "repeatingAValue = @repeatingAValue, " +
                    "repeatingBValue = @repeatingBValue, " +
                    "repeatingCount = @repeatingCount, " +
                    "executionsDoneCount = @executionsDoneCount " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, task);
            }
        }

        public SensorTask GetTask(int db_Id)
        {
            SensorTask task;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                task = db.Query<SensorTask>("SELECT * FROM SensorsTasks WHERE db_Id=@db_Id",
                    new { db_Id }).SingleOrDefault();
            }

            return task;
        }

        public List<SensorTask> GetAllTasks()
        {
            List<SensorTask> tasks;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                tasks = db.Query<SensorTask>("SELECT * FROM SensorsTasks").ToList();
            }

            return tasks;
        }

        public List<SensorTask> GetTasks(int nodeId, int sensorId)
        {
            List<SensorTask> list;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                list = db.Query<SensorTask>("SELECT * FROM SensorsTasks WHERE nodeId=@nodeId AND sensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return list;
        }

        public void DeleteTask(int db_Id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM SensorsTasks WHERE db_Id=@db_Id",
                    new { db_Id });
            }
        }

        public void DeleteTasks(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM SensorsTasks WHERE nodeId=@nodeId AND sensorId=@sensorId",
                    new { nodeId, sensorId });
            }
        }

        public void DeleteCompleted(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM SensorsTasks WHERE nodeId=@nodeId AND sensorId=@sensorId AND isCompleted=1",
                    new { nodeId, sensorId });
            }
        }

        public void DropAllTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [SensorsTasks]");
            }
        }

        private void CreateSensorsTasksTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                try
                {
                    string req = String.Format(
                        @"CREATE TABLE [dbo].[SensorsTasks](
	                    [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                    [description] [nvarchar](max) NULL,	        
	                    [nodeId] [int] NULL,
	                    [sensorId] [int] NULL,
	                    [sensorDbId] [int] NULL,
	                    [executionDate] [datetime] NULL,
	                    [dataType] [int] NULL,
	                    [executionValue] [nvarchar](max) NULL,
	                    [isCompleted] [bit] NULL,       
	                    [isRepeating] [bit] NULL,       
	                    [repeatingInterval] [int] NULL,       
	                    [repeatingAValue] [nvarchar](max) NULL,       
	                    [repeatingBValue] [nvarchar](max) NULL,       
	                    [repeatingCount] [int] NULL,       
	                    [executionsDoneCount] [int] NULL
                        ) ON [PRIMARY] ");

                    db.Query(req);
                }
                catch
                {
                }
            }
        }
    }
}
