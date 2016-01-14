/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways
{
    public interface INodesHistoryRepository
    {
        List<SensorData> GetSensorHistory(int nodeId, int sensorId);
        void ClearSensorHistory(int nodeId, int sensorId);
        void ClearAllHistory();

        void ConnectToGateway(Gateway gateway);
        bool IsDbExist();
        void SetWriteInterval(int ms);
    }
}
