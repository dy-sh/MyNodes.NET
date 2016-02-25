/*  MyNetSensors 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MyNetSensors.Nodes
{
    public class UiTimerNode : UiNode
    {

        private List<UITimerTask> tasks = new List<UITimerTask>();
        private bool abortExecuting;

        public UiTimerNode() : base("UI", "Timer")
        {
            AddOutput();
            GetTasksFromDB();
        }

        public void SetState(string state)
        {
            Outputs[0].Value = state;
        }


        #region ---------------- Tasks Repository -------------------------
        private void GetTasksFromDB()
        {
            tasks = new List<UITimerTask>();
            List<NodeData> data = GetAllNodeData();
            foreach (var nodeData in data)
            {
                UITimerTask task = JsonConvert.DeserializeObject<UITimerTask>(nodeData.Value);
                tasks.Add(task);
            }
        }

        private void UpdateTaskInDb(UITimerTask task)
        {
            NodeData data = new NodeData
            {
                DateTime = DateTime.Now,
                Id = task.Id,
                NodeId = Id,
                Value = JsonConvert.SerializeObject(task)
            };

            UpdateNodeData(data);
        }

        private void AddTaskToDb(UITimerTask task)
        {
            int id = AddNodeDataImmediately(JsonConvert.SerializeObject(task));
            task.Id = id;
        }

        private void RemoveTaskFromDb(int id)
        {
            RemoveNodeData(id);
        }

        #endregion

        public override void OnRemove()
        {
            RemoveAllNodeData();
            base.OnRemove();
        }



        //used to abort UpdateTasks() method
        public void AbortExecuting()
        {
            abortExecuting = true;
        }


        public override void Loop()
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

                task.ExecutionValue = task.ExecutionValue == task.RepeatingAValue
                    ? task.RepeatingBValue : task.RepeatingAValue;

                if (!task.IsCompleted)
                    task.ExecutionDate = DateTime.Now.AddMilliseconds(task.RepeatingInterval);
            }

            UpdateTaskInDb(task);

            Outputs[0].Value = task.ExecutionValue;
        }


        public void DisableAllTasks()
        {
            List<UITimerTask> tasksForUpdate = tasks.Where(x => x.Enabled).ToList();

            foreach (var task in tasksForUpdate.Where(task => task.Enabled))
            {
                task.Enabled = false;
                UpdateTaskInDb(task);
            }

            AbortExecuting();
        }

        public void RemoveAllTasks()
        {
            tasks.Clear();
            AbortExecuting();

            RemoveAllNodeData();
        }



        public bool AddTask(UITimerTask task)
        {
            if (task.IsRepeating)
                task.ExecutionValue = task.RepeatingBValue;

            AddTaskToDb(task);

            tasks.Add(task);
            AbortExecuting();

            return true;
        }

        public UITimerTask GetTask(int id)
        {
            return tasks.FirstOrDefault(x => x.Id == id);
        }

        public List<UITimerTask> GetAllTasks()
        {
            return tasks;
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


            UpdateTaskInDb(oldTask);

            return true;
        }

        public bool RemoveTask(int id)
        {
            UITimerTask oldTask = GetTask(id);
            if (oldTask == null)
                return false;

            tasks.Remove(oldTask);
            AbortExecuting();

            RemoveTaskFromDb(oldTask.Id);

            return true;
        }



        public bool RemoveCompletedTasks()
        {
            var list = tasks.Where(x => x.IsCompleted).ToList();
            foreach (var task in list)
            {
                tasks.Remove(task);
                RemoveTaskFromDb(task.Id);
            }
            AbortExecuting();

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

 





        public override string GetJsListGenerationScript()
        {
            return @"

            //UiTimerNode
            function UiTimerNode() {
                this.properties = {
                    'ObjectType': 'MyNetSensors.Nodes.UiTimerNode',
                    'Assembly': 'Nodes.UITimer'
                };
            }
            UiTimerNode.prototype.getExtraMenuOptions = function(graphcanvas)
            {
                var that = this;
                return [
                { content: 'Open interface', callback: function() { var win = window.open('/UITimer/Tasks/' + that.id, '_blank'); win.focus(); } }
                    , null
                ];
            }
            UiTimerNode.title = 'Timer';
            LiteGraph.registerNodeType('UI/Timer', UiTimerNode);

            ";
        }

        public override string GetNodeDescription()
        {
            return "This is a UI node. It performs a timer function. <br/>" +
                   "This node can be configured on the dashboard. <br/>" +
                   "You can create multiple timer events in the interface of this node. <br/>" +
                   "The node conveniently use to schedule any action, " +
                   "without going into the editor.";
        }
    }
}
