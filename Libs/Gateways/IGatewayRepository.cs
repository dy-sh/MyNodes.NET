/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways
{
    public interface IGatewayRepository
    {
        void ConnectToGateway(Gateway gateway);


        void AddMessage(Message message);
        List<Message> GetMessages();
        void DropMessages();


        int AddOrUpdateNode(Node node);
        int AddNode(Node node);
        void UpdateNode(Node node);
        int AddOrUpdateSensor(Sensor sensor);
        int AddSensor(Sensor sensor);
        void UpdateSensor(Sensor sensor);
        List<Node> GetNodes();
        Node GetNode(int id);
        Sensor GetSensor(int id);
        Sensor GetSensor(int nodeId, int sensorId);
        void DropNodes();
        

        bool IsDbExist();
        void SetWriteInterval(int ms);
        void SetStoreTxRxMessages(bool enable);

        void ShowDebugInConsole(bool enable);


        void UpdateNodeSettings(Node node);
        void UpdateSensorSettings(Sensor sensor);
        void DeleteNode(int id);
    }
}
