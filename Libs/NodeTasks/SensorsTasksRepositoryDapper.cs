/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

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

        private void CreateSensorsTasksTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                
                try
                {
                    string req = 
                        @"CREATE TABLE [dbo].[SensorsTasks](
	                    [db_Id] [int] IDENTITY(1,1) NOT NULL,
	                    [enabled] [bit] NULL,       
	                    [isCompleted] [bit] NULL,       
	                    [description] [nvarchar](max) NULL,	        
	                    [nodeId] [int] NULL,
	                    [sensorId] [int] NULL,
	                    [sensorDbId] [int] NULL,
	                    [sensorDescription] [nvarchar](max) NULL,
	                    [executionDate] [datetime] NULL,
	                    [dataType] [int] NULL,
	                    [executionValue] [nvarchar](max) NULL,
	                    [isRepeating] [bit] NULL,       
	                    [repeatingInterval] [int] NULL,       
	                    [repeatingAValue] [nvarchar](max) NULL,       
	                    [repeatingBValue] [nvarchar](max) NULL,       
	                    [repeatingNeededCount] [int] NULL,
	                    [repeatingDoneCount] [int] NULL
                        ) ON [PRIMARY] ";

                    db.Query(req);
                }
                catch
                {
                }
            }
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

                var sqlQuery = "INSERT INTO SensorsTasks (enabled,isCompleted,description, nodeId, sensorId, sensorDbId,sensorDescription, executionDate,dataType, executionValue,  isRepeating ,repeatingInterval,repeatingAValue,repeatingBValue,repeatingNeededCount,repeatingDoneCount) "
                             + "VALUES(@enabled,@isCompleted,@description, @nodeId, @sensorId, @sensorDbId,@sensorDescription, @executionDate, @dataType, @executionValue,  @isRepeating, @repeatingInterval, @repeatingAValue, @repeatingBValue, @repeatingNeededCount,@repeatingDoneCount); "
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
                    "enabled = @enabled, " +
                    "isCompleted = @isCompleted, " +
                    "description = @description, " +
                    //"nodeId = @nodeId, " +
                    //"sensorId = @sensorId, " +
                    //"sensorDbId = @sensorDbId, " +
                    //"sensorDescription = @sensorDescription, " +
                    "executionDate = @executionDate, " +
                    "dataType = @dataType, " +
                    "executionValue = @executionValue, " +
                    "isRepeating = @isRepeating, " +
                    "repeatingInterval = @repeatingInterval, " +
                    "repeatingAValue = @repeatingAValue, " +
                    "repeatingBValue = @repeatingBValue, " +
                    "repeatingNeededCount = @repeatingNeededCount, " +
                    "repeatingDoneCount = @repeatingDoneCount " +
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

        public void DeleteTasks(int sensorDbId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM SensorsTasks WHERE sensorDbId=@sensorDbId",
                    new { sensorDbId });
            }
        }

        public void DeleteCompleted()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM SensorsTasks WHERE isCompleted=1");
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

        public void DropTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [SensorsTasks]");
            }
        }

        public void UpdateTask(int db_Id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE SensorsTasks SET " +
                    "isCompleted = @isCompleted, " +
                    "executionDate = @executionDate, " +
                    "executionValue = @executionValue, " +
                    "repeatingDoneCount = @repeatingDoneCount " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, new {
                    db_Id,
                    isCompleted,
                    repeatingDoneCount,
                    executionDate,
                    executionValue
                });
            }
        }

        public void UpdateTask(int db_Id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE SensorsTasks SET " +
                    "enabled = @enabled, " +
                    "isCompleted = @isCompleted, " +
                    "executionDate = @executionDate, " +
                    "repeatingDoneCount = @repeatingDoneCount " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, new
                {
                    db_Id,
                    enabled,
                    isCompleted,
                    executionDate,
                    repeatingDoneCount
                });
            }
        }

        public void UpdateTaskEnabled(int db_Id, bool enabled)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE SensorsTasks SET " +
                    "enabled = @enabled " +
                    "WHERE db_Id = @db_Id";
                db.Execute(sqlQuery, new
                {
                    db_Id,
                    enabled
                });
            }
        }

        public void DisableTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE SensorsTasks SET " +
                    "enabled = 0 ";
                db.Execute(sqlQuery);
            }
        }


     
    }
}
