using System.Collections.Generic;
using MyNetSensors.Gateway;

namespace MyNetSensors.SensorsHistoryRepository
{
    public interface ISensorsHistoryRepository
    {

        List<SensorData> GetSensorHistory(int db_Id);
        void DropSensorHistory(int db_Id);

        void ConnectToGateway(SerialGateway gateway);
        bool IsDbExist();
    }
}
