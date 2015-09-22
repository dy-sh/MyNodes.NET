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
        List<SensorData> GetSensorDataLog(int sensorDbId);
        Node GetNodeByNodeId(int nodeId);
        Node GetNodeByDbId(int db_Id);
        Sensor GetSensorByDbId(int db_Id);
        Sensor GetSensorBySensorId(int ownerNodeId, int sensorId);
    }
}
