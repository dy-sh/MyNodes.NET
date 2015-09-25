using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.WebController.Code
{
    interface ISensorsRepository
    {
        Node GetNodeByDbId(int db_Id);
        Node GetNodeByNodeId(int nodeId);
        Sensor GetSensor(int db_Id);
        Sensor GetSensor(int ownerNodeId, int sensorId);
        List<SensorData> GetSensorLog(int db_Id);
        List<SensorData> GetSensorLog(int ownerNodeId, int sensorId);
        void DropSensorLog(int db_Id);
        void DropSensorLog(int ownerNodeId, int sensorId);
        void UpdateNodeSettings(Node node);
        void UpdateSensorSettings(Sensor sensor);
    }
}
