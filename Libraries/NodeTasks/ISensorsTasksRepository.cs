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
        void ConnectToGateway(SerialGateway gateway);

        void AddOrUpdateTask(SensorTask task);
        SensorTask GetTask(int db_Id);
        List<SensorTask> GetTasks(int nodeId, int sensorId);
        void DeleteTask(int db_Id);
        void DeleteTasks(int nodeId,int sensorId);
        void DropAllTasks();
        bool IsDbExist();
        void ExecuteNow(int db_Id);
    }
}
