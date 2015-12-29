/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Dapper;
using MyNetSensors.Gateways;

namespace MyNetSensors.GatewayRepository
{




    public class GatewayRepositoryDapper : IGatewayRepository
    {
        public void ConnectToGateway(Gateway gateway)
        {
            throw new NotImplementedException();
        }

        public void AddMessage(Message message)
        {
            throw new NotImplementedException();
        }

        public List<Message> GetMessages()
        {
            throw new NotImplementedException();
        }

        public void DropMessages()
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdateNode(Node node)
        {
            throw new NotImplementedException();
        }

        public void AddOrUpdateSensor(Sensor sensor)
        {
            throw new NotImplementedException();
        }

        public List<Node> GetNodes()
        {
            throw new NotImplementedException();
        }

        public Node GetNodeByDbId(int db_Id)
        {
            throw new NotImplementedException();
        }

        public Node GetNodeByNodeId(int nodeId)
        {
            throw new NotImplementedException();
        }

        public Sensor GetSensor(int db_Id)
        {
            throw new NotImplementedException();
        }

        public Sensor GetSensor(int nodeId, int sensorId)
        {
            throw new NotImplementedException();
        }

        public void DropNodes()
        {
            throw new NotImplementedException();
        }

        public bool IsDbExist()
        {
            throw new NotImplementedException();
        }

        public void SetWriteInterval(int ms)
        {
            throw new NotImplementedException();
        }

        public void SetStoreTxRxMessages(bool enable)
        {
            throw new NotImplementedException();
        }

        public void ShowDebugInConsole(bool enable)
        {
            throw new NotImplementedException();
        }

        public void UpdateNodeSettings(Node node)
        {
            throw new NotImplementedException();
        }

        public void UpdateSensorSettings(Sensor sensor)
        {
            throw new NotImplementedException();
        }

        public void DeleteNodeByDbId(int db_Id)
        {
            throw new NotImplementedException();
        }

        public void DeleteNodeByNodeId(int nodeId)
        {
            throw new NotImplementedException();
        }
    }

}
