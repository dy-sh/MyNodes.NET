using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Timers;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public class SensorsTasksRepositoryDapper: ISensorsTasksRepository
    {
        private int updateTasksInterval = 10;


        private Timer updateTasksTimer = new Timer();

        private SerialGateway gateway;

        private string connectionString;

        private List<SensorTask> tasks=new List<SensorTask>();

        public SensorsTasksRepositoryDapper(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public bool IsDbExist()
        {
            //todo check if db exist
            return true;
        }

        public void ConnectToGateway(SerialGateway gateway)
        {
            this.gateway = gateway;

            gateway.OnClearNodesListEvent += OnClearNodesListEvent;

            updateTasksTimer.Elapsed += UpdateTasks;

            updateTasksTimer.Interval = updateTasksInterval;
            updateTasksTimer.Start();

        }

        private void UpdateTasks(object sender, ElapsedEventArgs e)
        {
            updateTasksTimer.Stop();
            foreach (var task in tasks)
            {
                if (!task.isCompleted && task.executionDate<=DateTime.Now)
                    Execute(task);
            }
            updateTasksTimer.Start();

        }



        private void OnClearNodesListEvent(object sender, EventArgs e)
        {
            DropAllTasks();
        }

        public void AddOrUpdateTask(SensorTask task)
        {
            tasks.Add(task);
        }

        public SensorTask GetTask(int db_Id)
        {
            SensorTask task = tasks.FirstOrDefault(x => x.db_Id == db_Id);
            return task;
        }

        public List<SensorTask> GetTasks(int nodeId, int sensorId)
        {
            return tasks;
        }

        public void DeleteTask(int db_Id)
        {
            tasks.RemoveAll(x => x.db_Id == db_Id);
        }

        public void DeleteTasks(int nodeId, int sensorId)
        {
            tasks.RemoveAll(x => x.nodeId == nodeId && x.sensorId==sensorId);
        }

        public void DropAllTasks()
        {
            tasks.Clear();
        }

        public void ExecuteNow(int db_Id)
        {
            SensorTask task = GetTask(db_Id);
            Execute(task);
        }

        private void Execute(SensorTask task)
        {
            task.executionsDoneCount++;

            if (!task.isRepeating)
                task.isCompleted = true;
            else
            {
                if (task.repeatingCount > 0)
                    task.repeatingCount--;

                if (task.repeatingCount == 0)
                    task.isCompleted = true;

                if (task.executionValue.state == task.repeatingAValue.state)
                    task.executionValue = task.repeatingBValue;
                else 
                    task.executionValue = task.repeatingAValue;

                if (!task.isCompleted)
                    task.executionDate = task.executionDate.AddMilliseconds(task.repeatingInterval);
            }

            task.executionValue.dateTime = DateTime.Now;

            gateway.SendSensorState(task.nodeId,task.sensorId, task.executionValue);
        }
    }
}
