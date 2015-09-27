using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.NodeTasks
{
    public interface ISensorsTasksRepository
    {
        void CreateDb();
        bool IsDbExist();
        int AddOrUpdateTask(SensorTask task);
        int AddTask(SensorTask task);
        void UpdateTask(SensorTask task);
        SensorTask GetTask(int db_Id);
        List<SensorTask> GetTasks(int nodeId, int sensorId);
        List<SensorTask> GetAllTasks();
        void DeleteTask(int db_Id);
        void DeleteTasks(int nodeId,int sensorId);
        void DeleteCompleted(int nodeId, int sensorId);
        void DropAllTasks();
    }
}
