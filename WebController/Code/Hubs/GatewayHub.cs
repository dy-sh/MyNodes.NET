/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.AspNet.SignalR;
using MyNetSensors.SerialGateway;
using System.Threading.Tasks;

namespace MyNetSensors.WebController.Code.Hubs
{
    public class GatewayHub : Hub
    {


        public override Task OnConnected()
        {
            string clientId = Context.ConnectionId;

            bool isGateway = Context.QueryString["IsGateway"]=="true";

            if (isGateway)
            {
                string connectionPassword = ConfigurationManager.AppSettings["GateToWebConnectionPassword"];
                string sendedPassword = Context.QueryString["ConnectionPassword"];
                if (connectionPassword == sendedPassword)
                {
                    GatewayHubStaticData.gatewayId = clientId;
                    Clients.Caller.authorizationCompleted();
                    Clients.Others.onGatewayServiceConnected();
                }
                else
                {
                    Clients.Caller.authorizationFailed();
                }
            }
            else if (!GatewayHubStaticData.connectedUsersId.Contains(clientId))
                GatewayHubStaticData.connectedUsersId.Add(clientId);

            return base.OnConnected();
        }


        public override Task OnReconnected()
        {
            string clientId = Context.ConnectionId;
            if (!GatewayHubStaticData.connectedUsersId.Contains(clientId))
                GatewayHubStaticData.connectedUsersId.Add(clientId);

            return base.OnReconnected();
        }


        public override Task OnDisconnected(bool stopCalled)
        {
            string clientId = Context.ConnectionId;

            if (clientId == GatewayHubStaticData.gatewayId)
            {
                GatewayHubStaticData.gatewayId = null;
                Clients.Others.onGatewayServiceDisconnected();
            }
            else if (GatewayHubStaticData.connectedUsersId.Contains(clientId))
                GatewayHubStaticData.connectedUsersId.Remove(clientId);

            return base.OnDisconnected(stopCalled);
        }



        public string GetConnectedUsersCount()
        {
            Clients.Caller.returnConnectedUsersCount(GatewayHubStaticData.connectedUsersId.Count);
            return GatewayHubStaticData.connectedUsersId.Count.ToString();
        }


        public void OnMessageRecievedEvent(Message message)
        {
            Clients.Others.onMessageRecievedEvent(message.ToString());
        }

        public void OnMessageSendEvent(Message message)
        {
            Clients.Others.onMessageSendEvent(message.ToString());
        }

        public void OnClearMessages()
        {
            Clients.Others.onClearMessages();
        }

        public void ReturnLog(List<Message> log)
        {
            string sLog="";
            foreach (var message in log)
            {
                sLog += message.ToString()+"<br/>";
            }
            Clients.Others.returnLog(sLog);
        }

        public void ReturnNodes(List<Node> nodes)
        {
            Clients.Others.returnNodes(nodes);
        }

        public void OnNodeUpdatedEvent(Node node)
        {
            Clients.Others.onNodeUpdatedEvent(node);
        }

        public void OnNodeLastSeenUpdatedEvent(Node node)
        {
            Clients.Others.onNodeLastSeenUpdatedEvent(node);
        }

        public void OnNewNodeEvent(Node node)
        {
            Clients.Others.onNewNodeEvent(node);
        }

        public void OnNodeBatteryUpdatedEvent(Node node)
        {
            Clients.Others.onNodeBatteryUpdatedEvent(node);
        }

        public void OnSensorUpdatedEvent(Sensor sensor)
        {
            Clients.Others.onSensorUpdatedEvent(sensor);
        }

        public void OnNewSensorEvent(Sensor sensor)
        {
            Clients.Others.onNewSensorEvent(sensor);
        }

        public void OnClearNodesListEvent()
        {
            Clients.Others.onClearNodesListEvent();
        }

        public void SendMessage(string message)
        {
            Clients.Others.sendMessage(message);
        }

        public void OnGatewayDisconnectedEvent()
        {
            Clients.Others.onGatewayHardwareDisconnected();
        }

        public void GetGatewayServiceConnected()
        {
            Clients.Caller.returnGatewayServiceConnected(IsGatewayServiceConnected());
        }

        public void OnGatewayConnectedEvent()
        {
            Clients.Others.onGatewayHardwareConnected();
        }

        private bool IsGatewayServiceConnected()
        {
            return !String.IsNullOrEmpty(GatewayHubStaticData.gatewayId);
        }

  
    }

}