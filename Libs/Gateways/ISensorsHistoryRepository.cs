/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways
{
    public interface ISensorsHistoryRepository
    {
        List<SensorData> GetSensorHistory(int db_Id);
        void DropSensorHistory(int db_Id);
        void DropHistory();

        void ConnectToGateway(Gateway gateway);
        bool IsDbExist();
        void SetWriteInterval(int ms);
    }
}
