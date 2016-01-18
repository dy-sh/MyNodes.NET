/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using MyNetSensors.LogicalNodes;


namespace MyNetSensors.NodesTasks
{
    public class NodesTasksEngine
    {
        //If you have tons of tasks, and system perfomance decreased, increase this value,
        //and you will get less tasks updating frequency 
        private int updateTasksInterval = 10;


        private Timer updateTasksTimer = new Timer();

        private LogicalNodesEngine engine;
        private INodesTasksRepository db;

        private List<NodeTask> tasks = new List<NodeTask>();

        public NodesTasksEngine(LogicalNodesEngine engine, INodesTasksRepository db=null)
        {
            this.db = db;
            this.engine = engine;

            engine.OnRemoveNodeEvent += OnRemoveNodeEvent;


            updateTasksTimer.Elapsed += UpdateTasks;
            updateTasksTimer.Interval = updateTasksInterval;

            GetTasksFromRepository();

            Start();
        }

        private void OnRemoveNodeEvent(LogicalNode node)
        {

            var list = tasks.Where(x => x.NodeId == node.Id).ToList();
            foreach (var nodeTask in list)
            {
                tasks.Remove(nodeTask);
            }

            db?.RemoveTasksForNode(node.Id);
        }

        public void Start()
        {
            updateTasksTimer.Start();
        }
        public void Stop()
        {
            updateTasksTimer.Stop();
        }

        public void GetTasksFromRepository()
        {
            tasks = db?.GetAllTasks();
        }

        private void UpdateTasks(object sender, ElapsedEventArgs e)
        {
            updateTasksTimer.Stop();

            try
            {
                //to prevent changing of collection while writing to db is not yet finished
                NodeTask[] tasksTemp = new NodeTask[tasks.Count];
                tasks.CopyTo(tasksTemp);

                foreach (var task in tasksTemp)
                {
                    if (!task.IsCompleted && task.Enabled && task.ExecutionDate <= DateTime.Now)
                        Execute(task);
                }
            }
            catch{}

            updateTasksTimer.Start();
        }



        private void Execute(NodeTask task)
        {
            task.RepeatingDoneCount++;

            if (!task.IsRepeating)
                task.IsCompleted = true;
            else
            {
                if (task.RepeatingNeededCount!=0 
                    && task.RepeatingDoneCount >= task.RepeatingNeededCount)
                    task.IsCompleted = true;

                if (task.ExecutionValue == task.RepeatingAValue)
                    task.ExecutionValue = task.RepeatingBValue;
                else
                    task.ExecutionValue = task.RepeatingAValue;

                if (!task.IsCompleted)
                    task.ExecutionDate = DateTime.Now.AddMilliseconds(task.RepeatingInterval);
            }

            //we should not update the whole record, because other parts of the record can be updated from outside
            db?.UpdateTask(
                task.Id,
                task.IsCompleted,
                task.ExecutionDate,
                task.ExecutionValue,
                task.RepeatingDoneCount);

            LogicalNodeTask node = engine.GetNode(task.NodeId) as LogicalNodeTask;
            if (node == null)
            {
                engine.LogEngineError($"Can`t execute task for Node [{task.NodeId}]. Not found.");
            }
            node.SetState(task.ExecutionValue);
        }

        public void SetUpdateInterval(int ms)
        {
            updateTasksInterval = ms;
            updateTasksTimer.Stop();
            updateTasksTimer.Interval = updateTasksInterval;
            updateTasksTimer.Start();
        }
    }
}
