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
        void ConnectToGateway(SerialGateway gateway);


        void AddMessage(Message message);
        List<Message> GetMessages();
        void DropMessages();


        void AddOrUpdateNode(Node node);
        void AddOrUpdateSensor(Sensor sensor);
        List<Node> GetNodes();
        Node GetNodeByDbId(int db_Id);
        Node GetNodeByNodeId(int nodeId);
        Sensor GetSensor(int db_Id);
        Sensor GetSensor(int nodeId, int sensorId);
        void DropNodes();
        

        bool IsDbExist();
        void SetWriteInterval(int ms);
        void SetStoreTxRxMessages(bool enable);

        void ShowDebugInConsole(bool enable);


        void UpdateNodeSettings(Node node);
        void UpdateSensorSettings(Sensor sensor);
        void DeleteNodeByDbId(int db_Id);
        void DeleteNodeByNodeId(int nodeId);
    }
}
