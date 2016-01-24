/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNetSensors.Nodes
{
    public class UITimerNodesEngine
    {
        private NodesEngine engine;
        private IUITimerNodesRepository db;

        private List<UITimerTask> tasks;

        private bool abortExecuting = false;

        public UITimerNodesEngine(NodesEngine engine, IUITimerNodesRepository db = null)
        {
            this.db = db;
            this.engine = engine;

            tasks = db?.GetAllTasks();
            if (tasks == null)
                tasks = new List<UITimerTask>();

            engine.OnRemoveNode += OnRemoveNode;
            engine.OnUpdateLoop += UpdateTasks;
            engine.OnRemoveAllNodesAndLinks += OnRemoveAllNodesAndLinks;
        }

        private void OnRemoveAllNodesAndLinks()
        {
            tasks = new List<UITimerTask>();
            db?.RemoveAllTasks();
        }

        private void OnRemoveNode(Node node)
        {
            RemoveTasksForNode(node.Id);
        }


        //used to abort UpdateTasks() method
        public void AbortExecuting()
        {
            abortExecuting = true;
        }

        private void UpdateTasks()
        {
            try
            {
                foreach (var task in tasks)
                {
                    if (!task.IsCompleted && task.Enabled && task.ExecutionDate <= DateTime.Now)
                        Execute(task);

                    if (abortExecuting)
                    {
                        abortExecuting = false;
                        return;
                    }
                }
            }
            catch { }
        }



        private void Execute(UITimerTask task)
        {
            task.RepeatingDoneCount++;

            if (!task.IsRepeating)
                task.IsCompleted = true;
            else
            {
                if (task.RepeatingNeededCount != 0
                    && task.RepeatingDoneCount >= task.RepeatingNeededCount)
                    task.IsCompleted = true;

                if (task.ExecutionValue == task.RepeatingAValue)
                    task.ExecutionValue = task.RepeatingBValue;
                else
                    task.ExecutionValue = task.RepeatingAValue;

                if (!task.IsCompleted)
                    task.ExecutionDate = DateTime.Now.AddMilliseconds(task.RepeatingInterval);
            }

            db?.UpdateTask(task);

            UiTimerNode node = engine.GetNode(task.NodeId) as UiTimerNode;
            if (node == null)
            {
                engine.LogEngineError($"Can`t execute task for Node [{task.NodeId}]. Not found.");
                return;
            }
            node.SetState(task.ExecutionValue);
        }


        public void DisableAllTasks()
        {
            List<UITimerTask> tasksForUpdate = tasks.Where(x => x.Enabled).ToList();

            foreach (var task in tasksForUpdate)
            {
                task.Enabled = false;
            }

            AbortExecuting();

            if (tasksForUpdate.Any())
                db?.UpdateTasks(tasksForUpdate);
        }

        public void RemoveAllTasks()
        {
            tasks.Clear();
            AbortExecuting();

            db?.RemoveAllTasks();
        }

        public List<UITimerTask> GetTasksForNode(string id)
        {
            return tasks.Where(x => x.NodeId == id).ToList();
        }

        public bool AddTask(UITimerTask task)
        {
            UiTimerNode node = engine.GetNode(task.NodeId) as UiTimerNode;

            if (node == null)
                return false;

            if (task.IsRepeating)
                task.ExecutionValue = task.RepeatingBValue;

            if (db != null)
            {
                task.Id = db.AddTask(task);
            }

            tasks.Add(task);
            AbortExecuting();

            return true;
        }

        public UITimerTask GetTask(int id)
        {
            return tasks.FirstOrDefault(x => x.Id == id);
        }

        public bool UpdateTask(UITimerTask task)
        {
            UITimerTask oldTask = GetTask(task.Id);
            if (oldTask == null)
                return false;

            //oldTask.NodeId = task.NodeId;
            oldTask.Description = task.Description;
            oldTask.Enabled = task.Enabled;
            oldTask.ExecutionDate = task.ExecutionDate;
            oldTask.ExecutionValue = task.ExecutionValue;
            oldTask.IsCompleted = task.IsCompleted;
            oldTask.IsRepeating = task.IsRepeating;
            oldTask.RepeatingAValue = task.RepeatingAValue;
            oldTask.RepeatingBValue = task.RepeatingBValue;
            oldTask.RepeatingDoneCount = task.RepeatingDoneCount;
            oldTask.RepeatingInterval = task.RepeatingInterval;
            oldTask.RepeatingNeededCount = task.RepeatingNeededCount;
            oldTask.RepeatingInterval = task.RepeatingInterval;

            if (oldTask.IsRepeating
                && oldTask.ExecutionValue != oldTask.RepeatingBValue
                && oldTask.ExecutionValue != oldTask.RepeatingAValue)
                oldTask.ExecutionValue = oldTask.RepeatingBValue;


            db?.UpdateTask(oldTask);

            return true;
        }

        public bool RemoveTask(int id)
        {
            UITimerTask oldTask = GetTask(id);
            if (oldTask == null)
                return false;

            tasks.Remove(oldTask);
            AbortExecuting();

            db?.RemoveTask(oldTask.Id);

            return true;
        }

        public bool ExecuteNowTask(int id)
        {
            UITimerTask oldTask = GetTask(id);
            if (oldTask == null)
                return false;

            Execute(oldTask);
            return true;
        }

        public bool RemoveTasksForNode(string id)
        {
            var list = tasks.Where(x => x.NodeId == id).ToList();
            foreach (var task in list)
            {
                tasks.Remove(task);
            }
            AbortExecuting();

            db?.RemoveTasks(list);

            return true;
        }

        public bool RemoveCompletedTasksForNode(string id)
        {
            var list = tasks.Where(x => x.NodeId == id && x.IsCompleted).ToList();
            foreach (var task in list)
            {
                tasks.Remove(task);
            }
            AbortExecuting();

            db?.RemoveTasks(list);

            return true;
        }
    }
}
