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
using System.Threading.Tasks;
using MyNetSensors.Gateway;

namespace MyNetSensors.WebController.Code.Hubs
{
    public class GatewayHub : Hub
    {



        private bool IsGatewayServiceConnected()
        {
            return !String.IsNullOrEmpty(GatewayHubStaticData.gatewayId);
        }



        public override Task OnConnected()
        {
            string clientId = Context.ConnectionId;

            bool isGateway = Context.QueryString["IsGateway"] == "true";

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




        #region Messages from gateway

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
        
        public void OnGatewayConnectedEvent()
        {
            Clients.Others.onGatewayHardwareConnected();
        }
        
        public void OnGatewayDisconnectedEvent()
        {
            Clients.Others.onGatewayHardwareDisconnected();
        }

        public void ReturnGatewayHardwareConnected(string userId, bool isConnected)
        {
            Clients.Client(userId).returnGatewayHardwareConnected(isConnected);
        }

        public void ReturnLog(List<Message> log, string userId)
        {
            string sLog = "";
            foreach (var message in log)
            {
                sLog += message.ToString() + "<br/>";
            }
            Clients.Client(userId).returnLog(sLog);
        }

        public void ReturnNodes(List<Node> nodes, string userId)
        {
            Clients.Client(userId).returnNodes(nodes);
        }

        public void ReturnGatewayInfo(GatewayInfo info, string userId)
        {
            Clients.Client(userId).returnGatewayInfo(info);
        }



        #endregion

        #region Messages from clients to hub

        public void GetConnectedUsersCount()
        {
            Clients.Caller.returnConnectedUsersCount(GatewayHubStaticData.connectedUsersId.Count);
        }

        public void GetGatewayServiceConnected()
        {
            Clients.Caller.returnGatewayServiceConnected(IsGatewayServiceConnected());
        }

        #endregion



        #region Messages from clients to gateway

        public void SendMessage(string message)
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).sendMessage(message, clientId);
        }
        

        public void GetGatewayHardwareConnected()
        {
            if (!IsGatewayServiceConnected())
            {
                Clients.Caller.returnGatewayHardwareConnected(false);
                return;
            }

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).getGatewayHardwareConnected(clientId);
        }

        public void GetNodes()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).getNodes(clientId);
        }

        public void GetLog()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).getLog(clientId);
        }

        public void ClearLog()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).clearLog(clientId);
        }

        public void GetGatewayInfo()
        {
            if (!IsGatewayServiceConnected())
                return;

            string clientId = Context.ConnectionId;
            Clients.Client(GatewayHubStaticData.gatewayId).getGatewayInfo(clientId);
        }

        #endregion

    }

}