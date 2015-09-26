/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using MyNetSensors.Gateway;

namespace MyNetSensors.GatewayRepository
{
    public interface IGatewayRepository
    {
        void Connect(SerialGateway gateway, string connectionString);
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

    }
}
