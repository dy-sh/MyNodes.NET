/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Timers;
using MyNetSensors.Gateways;

namespace MyNetSensors.NodesTasks
{
    public class NodesTasksEngine
    {
        //If you have tons of tasks, and system perfomance decreased, increase this value,
        //and you will get less tasks updating frequency 
        private int updateTasksInterval = 10;


        private Timer updateTasksTimer = new Timer();

        private Gateway gateway;
        private INodesTasksRepository db;

        private List<NodeTask> tasks = new List<NodeTask>();

        public NodesTasksEngine(Gateway gateway, INodesTasksRepository db)
        {
            this.db = db;
            this.gateway = gateway;

            gateway.OnClearNodesListEvent += OnClearNodesListEvent;

            updateTasksTimer.Elapsed += UpdateTasks;
            updateTasksTimer.Interval = updateTasksInterval;

            db.CreateDb();
            GetTasksFromRepository();

            Start();
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
            tasks = db.GetAllTasks();
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
                    if (!task.isCompleted && task.enabled && task.executionDate <= DateTime.Now)
                        Execute(task);
                }
            }
            catch{}

            updateTasksTimer.Start();
        }

        private void OnClearNodesListEvent()
        {
            tasks.Clear();
            db.DropTasks();
        }

        private void Execute(NodeTask task)
        {
            task.repeatingDoneCount++;

            if (!task.isRepeating)
                task.isCompleted = true;
            else
            {
                if (task.repeatingNeededCount!=0 
                    && task.repeatingDoneCount >= task.repeatingNeededCount)
                    task.isCompleted = true;

                if (task.executionValue == task.repeatingAValue)
                    task.executionValue = task.repeatingBValue;
                else
                    task.executionValue = task.repeatingAValue;

                if (!task.isCompleted)
                    task.executionDate = DateTime.Now.AddMilliseconds(task.repeatingInterval);
            }

            //we should not update the whole record, because other parts of the record can be updated from outside
            db.UpdateTask(
                task.Id,
                task.isCompleted,
                task.executionDate,
                task.executionValue,
                task.repeatingDoneCount);

            gateway.SendSensorState(task.nodeId, task.sensorId, task.executionValue);
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
