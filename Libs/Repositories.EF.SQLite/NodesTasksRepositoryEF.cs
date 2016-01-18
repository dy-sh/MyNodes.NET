/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity;
using MyNetSensors.NodesTasks;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class NodesTasksRepositoryEF : INodesTasksRepository
    {

        private NodesTasksDbContext db;

        public NodesTasksRepositoryEF(NodesTasksDbContext dbContext)
        {
            this.db = dbContext;
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




        public int AddOrUpdateTask(NodeTask task)
        {
            int id = task.Id;

            NodeTask oldTask = GetTask(task.Id);

            if (oldTask == null)
                id = AddTask(task);
            else
                UpdateTask(task);

            return id;
        }

        public int AddTask(NodeTask task)
        {
            db.NodesTasks.Add(task);
            db.SaveChanges();

            return task.Id;
        }

        public void UpdateTask(NodeTask task)
        {
            NodeTask oldTask = GetTask(task.Id);
            oldTask.Description = task.Description;
            oldTask.Enabled = task.Enabled;
            oldTask.ExecutionDate = task.ExecutionDate;
            oldTask.ExecutionValue = task.ExecutionValue;
            oldTask.IsCompleted = task.IsCompleted;
            oldTask.IsRepeating = task.IsRepeating;
            oldTask.NodeId = task.NodeId;
            oldTask.RepeatingAValue = task.RepeatingAValue;
            oldTask.RepeatingBValue = task.RepeatingBValue;
            oldTask.RepeatingDoneCount = task.RepeatingDoneCount;
            oldTask.RepeatingInterval = task.RepeatingInterval;
            oldTask.RepeatingNeededCount = task.RepeatingNeededCount;
            oldTask.RepeatingInterval = task.RepeatingInterval;

            db.SaveChanges();
        }



        public NodeTask GetTask(int id)
        {
            return db.NodesTasks.FirstOrDefault(x => x.Id == id);

        }

        public List<NodeTask> GetTasksForNode(string nodeId)
        {
            return db.NodesTasks.Where(x => x.NodeId == nodeId).ToList();
        }

        public List<NodeTask> GetAllTasks()
        {
            return db.NodesTasks.ToList();
        }


        public void RemoveTask(int id)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;
            db.NodesTasks.Remove(task);
            db.SaveChanges();
        }

        public void RemoveTasksForNode(string nodeId)
        {
            List<NodeTask> tasks = GetTasksForNode(nodeId);
            //if (!tasks.Any())
            //    return;
            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }





        public void RemoveCompletedTasks()
        {
            List<NodeTask> tasks = db.NodesTasks.Where(x => x.IsCompleted == true).ToList();

            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }

        public void RemoveCompletedTasksForNode(string nodeId)
        {
            List<NodeTask> tasks = db.NodesTasks.Where(
                x => x.NodeId == nodeId
                && x.IsCompleted == true).ToList();

            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }



        public void RemoveAllTasks()
        {
            db.NodesTasks.RemoveRange(db.NodesTasks);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.IsCompleted = isCompleted;
            task.ExecutionDate = executionDate;
            task.ExecutionValue = executionValue;
            task.RepeatingDoneCount = repeatingDoneCount;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.IsCompleted = isCompleted;
            task.ExecutionDate = executionDate;
            task.RepeatingDoneCount = repeatingDoneCount;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void UpdateTaskEnabled(int id, bool enabled)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.Enabled = enabled;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void DisableTasks()
        {
            List<NodeTask> tasks = db.NodesTasks.Where(x => x.Enabled == true).ToList();
            if (tasks==null || !tasks.Any())
                return;

            foreach (var task in tasks)
            {
                task.Enabled = false;
            }

            db.NodesTasks.UpdateRange(tasks);
            db.SaveChanges();
        }



    }
}
