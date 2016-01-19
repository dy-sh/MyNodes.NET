/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.Dapper
{
    public class UITimerNodesRepositoryDapper : IUITimerNodesRepository
    {

        private string connectionString;

        public UITimerNodesRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public void CreateDb()
        {
            CreateUITimerNodesTable();
        }

        private void CreateUITimerNodesTable()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                
                try
                {
                    string req =
                        @"CREATE TABLE [dbo].[UITimerNodes](
	                    [Id] [int] IDENTITY(1,1) NOT NULL,
	                    [enabled] [bit] NULL,       
	                    [isCompleted] [bit] NULL,       
	                    [description] [nvarchar](max) NULL,	        
	                    [nodeId] [int] NULL,
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


        public int AddOrUpdateTask(UITimerTask task)
        {
            int id=task.Id;

            UITimerTask oldTask = GetTask(task.Id);

            if (oldTask == null)
                id= AddTask(task);
            else
                UpdateTask(task);

            return id;
        }

        public int AddTask(UITimerTask task)
        {
            int id;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();

                var sqlQuery = "INSERT INTO UITimerNodes (enabled,isCompleted,description, nodeId,  executionDate,dataType, executionValue,  isRepeating ,repeatingInterval,repeatingAValue,repeatingBValue,repeatingNeededCount,repeatingDoneCount) "
                             + "VALUES(@enabled,@isCompleted,@description, @nodeId, @executionDate, @dataType, @executionValue,  @isRepeating, @repeatingInterval, @repeatingAValue, @repeatingBValue, @repeatingNeededCount,@repeatingDoneCount); "
                            + "SELECT CAST(SCOPE_IDENTITY() as int)";

                id = db.Query<int>(sqlQuery, task).Single();
            }
            return id;
        }

        public void UpdateTask(UITimerTask task)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE UITimerNodes SET " +
                    "enabled = @enabled, " +
                    "isCompleted = @isCompleted, " +
                    "description = @description, " +
                    //"nodeId = @nodeId, " +
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

        public void UpdateTasks(List<UITimerTask> tasks)
        {
            foreach (var task in tasks)
            {
                UpdateTask(task);
            }
        }


        public UITimerTask GetTask(int id)
        {
            UITimerTask task;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                task = db.Query<UITimerTask>("SELECT * FROM UITimerNodes WHERE Id=@Id",
                    new { id }).SingleOrDefault();
            }

            return task;
        }

        public List<UITimerTask> GetTasksForNode(string nodeId)
        {
            List<UITimerTask> list;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                list = db.Query<UITimerTask>("SELECT * FROM UITimerNodes WHERE nodeId=@nodeId",
                    new { nodeId}).ToList();
            }

            return list;
        }

        public List<UITimerTask> GetAllTasks()
        {
            List<UITimerTask> tasks;
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                tasks = db.Query<UITimerTask>("SELECT * FROM UITimerNodes").ToList();
            }

            return tasks;
        }


        public void RemoveTask(int id)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM UITimerNodes WHERE Id=@Id",
                    new { id });
            }
        }

        public void RemoveTasks(List<UITimerTask> tasks)
        {
            foreach (var task in tasks)
            {
                RemoveTask(task.Id);
            }
        }

        public void RemoveTasksForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM UITimerNodes WHERE nodeId=@nodeId",
                    new { nodeId });
            }
        }




        public void RemoveCompletedTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM UITimerNodes WHERE isCompleted=1");
            }
        }

        public void RemoveCompletedTasksForNode(string nodeId)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("DELETE FROM UITimerNodes WHERE nodeId=@nodeId AND isCompleted=1",
                    new { nodeId });
            }
        }


        public void RemoveAllTasks()
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                db.Query("TRUNCATE TABLE [UITimerNodes]");
            }
        }

        public void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            using (var db = new SqlConnection(connectionString))
            {
                db.Open();
                var sqlQuery =
                    "UPDATE UITimerNodes SET " +
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
                    "UPDATE UITimerNodes SET " +
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
                    "UPDATE UITimerNodes SET " +
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
                    "UPDATE UITimerNodes SET " +
                    "enabled = 0 ";
                db.Execute(sqlQuery);
            }
        }


     
    }
}
