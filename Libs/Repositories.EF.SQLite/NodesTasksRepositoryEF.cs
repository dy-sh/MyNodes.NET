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

        public NodesTasksRepositoryEF(NodesTasksDbContext nodesDbContext)
        {
            this.db = nodesDbContext;
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

        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
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
            oldTask.dataType = task.dataType;
            oldTask.description = task.description;
            oldTask.enabled = task.enabled;
            oldTask.executionDate = task.executionDate;
            oldTask.executionValue = task.executionValue;
            oldTask.isCompleted = task.isCompleted;
            oldTask.isRepeating = task.isRepeating;
            oldTask.nodeId = task.nodeId;
            oldTask.repeatingAValue = task.repeatingAValue;
            oldTask.repeatingBValue = task.repeatingBValue;
            oldTask.repeatingDoneCount = task.repeatingDoneCount;
            oldTask.repeatingInterval = task.repeatingInterval;
            oldTask.repeatingNeededCount = task.repeatingNeededCount;
            oldTask.sensorDbId = task.sensorDbId;
            oldTask.sensorId = task.sensorId;
            oldTask.repeatingInterval = task.repeatingInterval;

            db.SaveChanges();
        }



        public NodeTask GetTask(int id)
        {
            return db.NodesTasks.FirstOrDefault(x => x.Id == id);

        }

        public List<NodeTask> GetAllTasks()
        {
            return db.NodesTasks.ToList();
        }

        public List<NodeTask> GetTasks(int nodeId, int sensorId)
        {
            return db.NodesTasks.Where(x => x.nodeId == nodeId && x.sensorId == sensorId).ToList();

        }

        public void DeleteTask(int id)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;
            db.NodesTasks.Remove(task);
            db.SaveChanges();
        }

        public void DeleteTasks(int nodeId, int sensorId)
        {
            List<NodeTask> tasks = GetTasks(nodeId, sensorId);
            //if (!tasks.Any())
            //    return;
            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }



        public void DeleteCompleted()
        {
            List<NodeTask> tasks = db.NodesTasks.Where(x => x.isCompleted == true).ToList();

            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }

        public void DeleteCompleted(int nodeId, int sensorId)
        {
            List<NodeTask> tasks = db.NodesTasks.Where(
                x => x.nodeId == nodeId
                && x.sensorId == sensorId
                && x.isCompleted == true).ToList();

            db.NodesTasks.RemoveRange(tasks);
            db.SaveChanges();
        }

        public void DropTasks()
        {
            db.NodesTasks.RemoveRange(db.NodesTasks);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.isCompleted = isCompleted;
            task.executionDate = executionDate;
            task.executionValue = executionValue;
            task.repeatingDoneCount = repeatingDoneCount;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.isCompleted = isCompleted;
            task.executionDate = executionDate;
            task.repeatingDoneCount = repeatingDoneCount;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void UpdateTaskEnabled(int id, bool enabled)
        {
            NodeTask task = db.NodesTasks.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.enabled = enabled;

            db.NodesTasks.Update(task);
            db.SaveChanges();
        }

        public void DisableTasks()
        {
            List<NodeTask> tasks = db.NodesTasks.Where(x => x.enabled == true).ToList();
            if (tasks==null || !tasks.Any())
                return;

            foreach (var task in tasks)
            {
                task.enabled = false;
            }

            db.NodesTasks.UpdateRange(tasks);
            db.SaveChanges();
        }



    }
}
