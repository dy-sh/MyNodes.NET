/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;

namespace MyNetSensors.Gateways.MySensors.Serial
{
    public interface IMySensorsRepository
    {
        event LogEventHandler OnLogInfo;
        event LogEventHandler OnLogError;

        void ConnectToGateway(Gateway gateway);





        int AddOrUpdateNode(Node node);
        int AddNode(Node node);
        void UpdateNode(Node node);
        int AddOrUpdateSensor(Sensor sensor);
        int AddSensor(Sensor sensor);
        void UpdateSensor(Sensor sensor);
        List<Node> GetNodes();
        Node GetNode(int id);
        Sensor GetSensor(int nodeId, int sensorId);
        void RemoveAllNodes();
        

        bool IsDbExist();
        void SetWriteInterval(int ms);


        void UpdateNodeSettings(Node node);
        void UpdateSensorSettings(Sensor sensor);
        void RemoveNode(int id);

    }
}
