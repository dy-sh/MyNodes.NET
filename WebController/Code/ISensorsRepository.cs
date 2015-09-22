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
        Node GetNode(int nodeDbId);
        Sensor GetSensor(int sensorDbId);
    }
}
