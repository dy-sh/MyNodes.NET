/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways
{
    public interface ISensorsHistoryRepository
    {
        List<SensorData> GetSensorHistory(int nodeId, int sensorId);
        void DropSensorHistory(int nodeId, int sensorId);
        void DropHistory();

        void ConnectToGateway(Gateway gateway);
        bool IsDbExist();
        void SetWriteInterval(int ms);
    }
}
