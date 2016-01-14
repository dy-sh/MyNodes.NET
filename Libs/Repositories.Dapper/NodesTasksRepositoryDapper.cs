/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.NodesTasks;

namespace MyNetSensors.Repositories.Dapper
{
    public class NodesTasksRepositoryDapper : INodesTasksRepository
    {

        private string connectionString;

        public NodesTasksRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public void CreateDb()
        {
            CreateNodesTasksTable();
        }

        private void CreateNodesTasksTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                
                try
                {
                    string req = 
                        @"CREATE TABLE [dbo].[NodesTasks](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
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


        public int AddOrUpdateTask(NodeTask task)
        {
            int id=task.Id;

            NodeTask oldTask = GetTask(task.Id);

            if (oldTask == null)
                id= AddTask(task);
            else
                UpdateTask(task);

            return id;
        }

        public int AddTask(NodeTask task)
        {
            int id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO NodesTasks (enabled,isCompleted,description, nodeId, sensorId, sensorDbId,sensorDescription, executionDate,dataType, executionValue,  isRepeating ,repeatingInterval,repeatingAValue,repeatingBValue,repeatingNeededCount,repeatingDoneCount) "
                             + "VALUES(@enabled,@isCompleted,@description, @nodeId, @sensorId, @sensorDbId,@sensorDescription, @executionDate, @dataType, @executionValue,  @isRepeating, @repeatingInterval, @repeatingAValue, @repeatingBValue, @repeatingNeededCount,@repeatingDoneCount); "
                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                id = db.Query<int>(sqlQuery, task).Single();
            }
            return id;
        }

        public void UpdateTask(NodeTask task)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesTasks SET " +
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
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, task);
            }
        }



        public NodeTask GetTask(int id)
        {
            NodeTask task;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                task = db.Query<NodeTask>("SELECT * FROM NodesTasks WHERE Id=@Id",
                    new { id }).SingleOrDefault();
            }

            return task;
        }

        public List<NodeTask> GetAllTasks()
        {
            List<NodeTask> tasks;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                tasks = db.Query<NodeTask>("SELECT * FROM NodesTasks").ToList();
            }

            return tasks;
        }

        public List<NodeTask> GetTasks(int nodeId, int sensorId)
        {
            List<NodeTask> list;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                list = db.Query<NodeTask>("SELECT * FROM NodesTasks WHERE nodeId=@nodeId AND sensorId=@sensorId",
                    new { nodeId, sensorId }).ToList();
            }

            return list;
        }

        public void RemoveTask(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesTasks WHERE Id=@Id",
                    new { id });
            }
        }

        public void RemoveTasks(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesTasks WHERE nodeId=@nodeId AND sensorId=@sensorId",
                    new { nodeId, sensorId });
            }
        }


        public void RemoveCompletedTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesTasks WHERE isCompleted=1");
            }
        }

        public void RemoveCompletedTasks(int nodeId, int sensorId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM NodesTasks WHERE nodeId=@nodeId AND sensorId=@sensorId AND isCompleted=1",
                    new { nodeId, sensorId });
            }
        }

        public void RemoveAllTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [NodesTasks]");
            }
        }

        public void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesTasks SET " +
                    "isCompleted = @isCompleted, " +
                    "executionDate = @executionDate, " +
                    "executionValue = @executionValue, " +
                    "repeatingDoneCount = @repeatingDoneCount " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, new {
                    id,
                    isCompleted,
                    repeatingDoneCount,
                    executionDate,
                    executionValue
                });
            }
        }

        public void UpdateTask(int id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesTasks SET " +
                    "enabled = @enabled, " +
                    "isCompleted = @isCompleted, " +
                    "executionDate = @executionDate, " +
                    "repeatingDoneCount = @repeatingDoneCount " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, new
                {
                    id,
                    enabled,
                    isCompleted,
                    executionDate,
                    repeatingDoneCount
                });
            }
        }

        public void UpdateTaskEnabled(int id, bool enabled)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE NodesTasks SET " +
                    "enabled = @enabled " +
                    "WHERE Id = @Id";
                db.Execute(sqlQuery, new
                {
                    id,
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
                    "UPDATE NodesTasks SET " +
                    "enabled = 0 ";
                db.Execute(sqlQuery);
            }
        }


     
    }
}
