/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using MyNetSensors.Gateway;

namespace MyNetSensors.SensorsHistoryRepository
{
    public interface ISensorsHistoryRepository
    {
        List<SensorData> GetSensorHistory(int db_Id);
        void DropSensorHistory(int db_Id);
        void DropAllSensorsHistory();

        void ConnectToGateway(SerialGateway gateway);
        bool IsDbExist();
        void SetWriteInterval(int ms);
    }
}
