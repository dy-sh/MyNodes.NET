/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Data.Entity;
using MyNetSensors.Nodes;

namespace MyNetSensors.Repositories.EF.SQLite
{
    public class UITimerNodesRepositoryEf : IUITimerNodesRepository
    {

        private UITimerNodesDbContext db;

        public UITimerNodesRepositoryEf(UITimerNodesDbContext dbContext)
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




        public int AddOrUpdateTask(UITimerTask task)
        {
            int id = task.Id;

            UITimerTask oldTask = GetTask(task.Id);

            if (oldTask == null)
                id = AddTask(task);
            else
                UpdateTask(task);

            return id;
        }

        public int AddTask(UITimerTask task)
        {
            db.UITimerNodes.Add(task);
            db.SaveChanges();

            return task.Id;
        }

        public void UpdateTask(UITimerTask task)
        {
            UITimerTask oldTask = GetTask(task.Id);
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

        public void UpdateTasks(List<UITimerTask> tasks)
        {
            foreach (var task in tasks)
            {
                UITimerTask oldTask = GetTask(task.Id);
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
            }
            db.SaveChanges();
        }


        public UITimerTask GetTask(int id)
        {
            return db.UITimerNodes.FirstOrDefault(x => x.Id == id);

        }

        public List<UITimerTask> GetTasksForNode(string nodeId)
        {
            return db.UITimerNodes.Where(x => x.NodeId == nodeId).ToList();
        }

        public List<UITimerTask> GetAllTasks()
        {
            return db.UITimerNodes.ToList();
        }


        public void RemoveTask(int id)
        {
            UITimerTask task = db.UITimerNodes.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;
            db.UITimerNodes.Remove(task);
            db.SaveChanges();
        }

        public void RemoveTasks(List<UITimerTask> tasks)
        {
            db.UITimerNodes.RemoveRange(tasks);
            db.SaveChanges();
        }

        public void RemoveTasksForNode(string nodeId)
        {
            List<UITimerTask> tasks = GetTasksForNode(nodeId);

            if (!tasks.Any())
                return;

            db.UITimerNodes.RemoveRange(tasks);
            db.SaveChanges();
        }





        public void RemoveCompletedTasks()
        {
            List<UITimerTask> tasks = db.UITimerNodes.Where(x => x.IsCompleted == true).ToList();

            db.UITimerNodes.RemoveRange(tasks);
            db.SaveChanges();
        }

        public void RemoveCompletedTasksForNode(string nodeId)
        {
            List<UITimerTask> tasks = db.UITimerNodes.Where(
                x => x.NodeId == nodeId
                && x.IsCompleted == true).ToList();

            db.UITimerNodes.RemoveRange(tasks);
            db.SaveChanges();
        }



        public void RemoveAllTasks()
        {
            db.UITimerNodes.RemoveRange(db.UITimerNodes);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool isCompleted, DateTime executionDate, string executionValue, int repeatingDoneCount)
        {
            UITimerTask task = db.UITimerNodes.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.IsCompleted = isCompleted;
            task.ExecutionDate = executionDate;
            task.ExecutionValue = executionValue;
            task.RepeatingDoneCount = repeatingDoneCount;

            db.UITimerNodes.Update(task);
            db.SaveChanges();
        }

        public void UpdateTask(int id, bool enabled, bool isCompleted, DateTime executionDate, int repeatingDoneCount)
        {
            UITimerTask task = db.UITimerNodes.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.IsCompleted = isCompleted;
            task.ExecutionDate = executionDate;
            task.RepeatingDoneCount = repeatingDoneCount;

            db.UITimerNodes.Update(task);
            db.SaveChanges();
        }

        public void UpdateTaskEnabled(int id, bool enabled)
        {
            UITimerTask task = db.UITimerNodes.FirstOrDefault(x => x.Id == id);
            if (task == null)
                return;

            task.Enabled = enabled;

            db.UITimerNodes.Update(task);
            db.SaveChanges();
        }

        public void DisableTasks()
        {
            List<UITimerTask> tasks = db.UITimerNodes.Where(x => x.Enabled == true).ToList();
            if (tasks==null || !tasks.Any())
                return;

            foreach (var task in tasks)
            {
                task.Enabled = false;
            }

            db.UITimerNodes.UpdateRange(tasks);
            db.SaveChanges();
        }



    }
}
