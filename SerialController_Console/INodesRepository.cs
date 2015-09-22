/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyNetSensors.SerialGateway;

namespace MyNetSensors.SerialController_Console
{
    interface INodesRepository
    {
        void Connect(Gateway gateway, string connectionString);
        void DropMessages();

        void DropNodes();

        List<Message> GetMessages();

        void AddMessage(Message message);

        List<Node> GetNodes();
        void AddOrUpdateNode(Node node);

        void AddOrUpdateSensor(Sensor sensor);

        bool IsConnected();

        void ShowDebugInConsole(bool enable);
        void SetStoreInterval(int ms);
        void StoreTxRxMessages(bool enable);

        List<SensorData> GetSensorDataLog(Sensor sensor);
    }
}
