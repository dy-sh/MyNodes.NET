using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Dapper;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public class SensorsTasksEngine
    {
        private int updateTasksInterval = 10;


        private Timer updateTasksTimer = new Timer();

        private SerialGateway gateway;
        private ISensorsTasksRepository db;

        private List<SensorTask> tasks = new List<SensorTask>();

        public SensorsTasksEngine(SerialGateway gateway, ISensorsTasksRepository db)
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

            //to prevent changing of collection while writing to db is not yet finished
            SensorTask[] tasksTemp = new SensorTask[tasks.Count];
            tasks.CopyTo(tasksTemp);
            tasks.Clear();

            foreach (var task in tasksTemp)
            {
                if (!task.isCompleted && task.executionDate <= DateTime.Now)
                    Execute(task);
            }
            updateTasksTimer.Start();
        }

        private void OnClearNodesListEvent(object sender, EventArgs e)
        {
            tasks.Clear();
            db.DropAllTasks();
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

                if (task.executionValue == task.repeatingAValue)
                    task.executionValue = task.repeatingBValue;
                else
                    task.executionValue = task.repeatingAValue;

                if (!task.isCompleted)
                    task.executionDate = DateTime.Now.AddMilliseconds(task.repeatingInterval);
            }

            db.UpdateTask(task);

            gateway.SendSensorState(task.nodeId, task.sensorId, task.GetExecutionSensorData());
        }
    }
}
