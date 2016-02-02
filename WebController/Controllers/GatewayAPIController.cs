/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using MyNetSensors.Gateways;
using MyNetSensors.Gateways.MySensors.Serial;
using MyNetSensors.WebController.Code;

namespace MyNetSensors.WebController.Controllers
{
    [ResponseCache(Duration = 0)]

    public class GatewayAPIController : Controller
    {
        private Gateway gateway = SystemController.gateway;


        public List<Node> GetNodes()
        {
            return gateway.GetNodes();
        }

        public bool IsConnected()
        {
            if (gateway == null)
                return false;

            return gateway.IsConnected();
        }

        //public string GetMessages()
        //{
        //    List<Message> messages = gateway.messagesLog.GetAllMessages();
        //    string text = null;
        //    foreach (var message in messages)
        //    {
        //        text += message.ToString();
        //        text += " <br>\n";
        //    }
        //    return text;
        //}

        //public bool ClearMessages()
        //{
        //    gateway.messagesLog.ClearLog();
        //    return true;
        //}

        public bool SendMessage(int nodeId, int sensorId, string state)
        {
            if (!gateway.IsConnected())
                return false;

            gateway.SendSensorState(nodeId, sensorId, state);
            return true;
        }


        public GatewayInfo GetGatewayInfo()
        {
            GatewayInfo info = new GatewayInfo
            {
                isGatewayConnected = IsConnected(),
                gatewayNodesRegistered = 0,
                gatewaySensorsRegistered = 0,
                state = GatewayState.Disconnected
            };

            if (gateway != null)
            {
                info.state = gateway.GetGatewayState();
                info.gatewayNodesRegistered = gateway.GetNodes().Count;
                info.gatewaySensorsRegistered = gateway.GetNodes().Sum(node => node.sensors.Count);

                if (gateway.connectionPort is EthernetConnectionPort)
                    info.type = GatewayType.Ethernet;
                else if (gateway.connectionPort is SerialConnectionPort)
                    info.type = GatewayType.Serial;
            }

            return info;
        }


        public bool UpdateNodeSettings(Node node)
        {
            //todo
            // gateway.UpdateNode(node);
            return true;
        }


        public bool RemoveNode(int nodeId)
        {
            if (gateway.GetNode(nodeId) == null)
                return false;
            gateway.RemoveNode(nodeId);
            return true;
        }


        public async Task<bool> RemoveAllNodes()
        {
            gateway.RemoveAllNodes();
            return true;
        }

    }
}
