/*  MyNodes.NET 
    Copyright (C) 2016 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System.Collections.Generic;
using System.Linq;
using MyNodes.Gateways.MySensors;
using MyNodes.Users;
using MyNodes.WebController.Code;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyNodes.WebController.Controllers
{

    public class GatewayAPIController : Controller
    {
        private Gateway gateway = SystemController.gateway;


        [Authorize(UserClaims.HardwareObserver)]

        //public List<Node> GetAllNodes()
        //{
        //    return gateway?.GetNodes();
        //}

        public bool IsConnected()
        {
            if (gateway == null)
                return false;

            return gateway.IsConnected;
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


        [Authorize(UserClaims.DashboardEditor)]

        public bool SendMessage(int nodeId, int sensorId, string state)
        {
            if (gateway==null || !gateway.IsConnected)
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
                info.state = gateway.GatewayState;
                info.gatewayNodesRegistered = gateway.NodeCount;
                info.gatewaySensorsRegistered = gateway.SensorCount;

                if (gateway.connectionPort is EthernetConnectionPort)
                    info.type = GatewayType.Ethernet;
                else if (gateway.connectionPort is SerialConnectionPort)
                    info.type = GatewayType.Serial;
            }

            return info;
        }


        [Authorize(UserClaims.EditorEditor)]

        public bool UpdateNodeSettings(Node node)
        {
            //todo
            // gateway.UpdateNode(node);
            return true;
        }


        [Authorize(UserClaims.EditorEditor)]

        public bool RemoveNode(int nodeId)
        {
            if (gateway?.GetNode(nodeId) == null)
                return false;
            gateway.RemoveNode(nodeId);
            return true;
        }


        [Authorize(UserClaims.EditorEditor)]

        public bool RemoveAllNodes()
        {
            gateway.RemoveAllNodes();
            return true;
        }

    }
}
