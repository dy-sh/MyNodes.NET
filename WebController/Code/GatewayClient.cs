/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;

namespace MyNetSensors.WebController.Code
{
    public class GatewayClient : IGatewayClient
    {
        IHubContext clientsHub = GlobalHost.ConnectionManager.GetHubContext<ClientsHub>();

        private IHubProxy hubProxy;
        private HubConnection hubConnection;
        private string url;

        public event Action OnConnected;
        public event Action OnDisconnected;
        public event DebugMessageEventHandler OnConnectionFailed;
        public event DebugMessageEventHandler OnSendingMessageFailed;

        public event StringHandler OnMessageRecieved;
        public event StringHandler OnMessageSend;
        public event Action OnClearMessages;
        public event NodeHandler OnNodeUpdated;
        public event NodeHandler OnNodeLastSeenUpdated;
        public event NodeHandler OnNewNode;
        public event NodeHandler OnNodeBatteryUpdated;
        public event SensorHandler OnSensorUpdated;
        public event SensorHandler OnNewSensor;
        public event Action OnClearNodesList;
        public event Action OnGatewayConnected;
        public event Action OnGatewayDisconnected;
        public event StringBool ReturnGatewayHardwareConnected;
        public event StringString ReturnLog;
        public event StringListNode ReturnNodes;
        public event StringGatewayInfo ReturnGatewayInfo;
   

        public void ConnectToServer(string url)
        {
            this.url = url;

            hubConnection = new HubConnection(url);
            hubProxy = hubConnection.CreateHubProxy("GatewayHub");

            try
            {
                hubConnection.Start().Wait();
                hubConnection.Closed += OnHubConnectionClosed;
                hubProxy.On<Message>("OnMessageRecievedEvent", OnMessageRecievedEvent);
                hubProxy.On<Message>("OnMessageSendEvent", OnMessageSendEvent);
                hubProxy.On("OnClearMessagesEvent", OnClearMessagesEvent);
                hubProxy.On<Node>("OnNodeUpdatedEvent", OnNodeUpdatedEvent);
                hubProxy.On<Node>("OnNodeLastSeenUpdatedEvent", OnNodeLastSeenUpdatedEvent);
                hubProxy.On<Node>("OnNewNodeEvent", OnNewNodeEvent);
                hubProxy.On<Node>("OnNodeBatteryUpdatedEvent", OnNodeBatteryUpdatedEvent);
                hubProxy.On<Sensor>("OnSensorUpdatedEvent", OnSensorUpdatedEvent);
                hubProxy.On<Sensor>("OnNewSensorEvent", OnNewSensorEvent);
                hubProxy.On("OnClearNodesListEvent", OnClearNodesListEvent);
                hubProxy.On("OnGatewayConnectedEvent", OnGatewayConnectedEvent);
                hubProxy.On("OnGatewayDisconnectedEvent", OnGatewayDisconnectedEvent);
                hubProxy.On<string, bool>("ReturnGatewayHardwareConnectedEvent", ReturnGatewayHardwareConnectedEvent);
                hubProxy.On<string, List<Message>>("ReturnLogEvent", ReturnLogEvent);
                hubProxy.On<string, List<Node>>("ReturnNodesEvent", ReturnNodesEvent);
                hubProxy.On<string, GatewayInfo>("ReturnGatewayInfoEvent", ReturnGatewayInfoEvent);


            }
            catch (Exception e)
            {
                if (OnConnectionFailed != null)
                    OnConnectionFailed(e.Message);
            }

        }

        public void OnHubConnectionClosed()
        {
            if (OnDisconnected != null)
                OnDisconnected();
        }


        public bool IsConnected()
        {
            return hubConnection != null && hubConnection.State == ConnectionState.Connected;
        }

        public void Disconnect()
        {
            hubConnection.Stop();
            if (OnDisconnected != null)
                OnDisconnected();
        }

        public bool IsGatewayServiceConnected()
        {
            return GatewayClientStatic.gatewayClient.IsConnected();
        }





        #region Messages from gateway

        public void OnMessageRecievedEvent(Message message)
        {
            clientsHub.Clients.All.OnMessageRecieved(message);
            OnMessageRecieved?.Invoke(message.ToString());
            Debug.WriteLine("OnMessageRecieved");
        }

        public void OnMessageSendEvent(Message message)
        {
            clientsHub.Clients.All.OnMessageSend(message);
            OnMessageSend?.Invoke(message.ToString());
            Debug.WriteLine("OnMessageSend");
        }

        public void OnClearMessagesEvent()
        {
            clientsHub.Clients.All.OnClearMessages();
            OnClearMessages?.Invoke();
        }

        public void OnNodeUpdatedEvent(Node node)
        {
            clientsHub.Clients.All.OnNodeUpdated(node);
            OnNodeUpdated?.Invoke(node);
        }

        public void OnNodeLastSeenUpdatedEvent(Node node)
        {
            clientsHub.Clients.All.OnNodeLastSeenUpdated(node);
            OnNodeLastSeenUpdated?.Invoke(node);
        }

        public void OnNewNodeEvent(Node node)
        {
            clientsHub.Clients.All.OnNewNode(node);
            OnNewNode?.Invoke(node);
        }

        public void OnNodeBatteryUpdatedEvent(Node node)
        {
            clientsHub.Clients.All.OnNodeBatteryUpdated(node);
            OnNodeBatteryUpdated?.Invoke(node);
        }

        public void OnSensorUpdatedEvent(Sensor sensor)
        {
            clientsHub.Clients.All.OnSensorUpdated(sensor);
            OnSensorUpdated?.Invoke(sensor);
            Debug.WriteLine("OnSensorUpdated");
        }

        public void OnNewSensorEvent(Sensor sensor)
        {
            clientsHub.Clients.All.OnNewSensor(sensor);
            OnNewSensor?.Invoke(sensor);
        }

        public void OnClearNodesListEvent()
        {
            clientsHub.Clients.All.OnClearNodesList();
            OnClearNodesList?.Invoke();
        }

        public void OnGatewayConnectedEvent()
        {
            clientsHub.Clients.All.OnGatewayConnected();
            OnGatewayConnected?.Invoke();
        }

        public void OnGatewayDisconnectedEvent()
        {
            clientsHub.Clients.All.OnGatewayDisconnected();
            OnGatewayDisconnected?.Invoke();
        }

        public void ReturnGatewayHardwareConnectedEvent(string userId, bool isConnected)
        {
            clientsHub.Clients.Client(userId).ReturnGatewayHardwareConnected(isConnected);
            ReturnGatewayHardwareConnected?.Invoke(userId, isConnected);
        }

        public void ReturnLogEvent(string userId, List<Message> log)
        {
            string sLog = "";
            foreach (var message in log)
            {
                sLog += message.ToString() + "<br/>";
            }
            clientsHub.Clients.Client(userId).ReturnLog(sLog);
            ReturnLog?.Invoke(userId, sLog);
        }

        public void ReturnNodesEvent(string userId, List<Node> nodes)
        {
            clientsHub.Clients.Client(userId).ReturnNodes(nodes);
            ReturnNodes?.Invoke(userId, nodes);
        }

        public void ReturnGatewayInfoEvent(string userId, GatewayInfo info)
        {
            clientsHub.Clients.Client(userId).ReturnGatewayInfo(info);
            ReturnGatewayInfo?.Invoke(userId, info);
        }



        #endregion




        #region Messages from clients to gateway

        public void SendMessage(string clientId, string message)
        {
            if (!IsGatewayServiceConnected())
                return;

            hubProxy.Invoke("sendMessage", clientId, message);
        }


        public void GetGatewayHardwareConnected(string clientId)
        {
            if (!IsGatewayServiceConnected())
            {
                //todo Clients.Caller.returnGatewayHardwareConnected(false);
                return;
            }

            hubProxy.Invoke("getGatewayHardwareConnected", clientId);
        }

        public void GetNodes(string clientId)
        {
            if (!IsGatewayServiceConnected())
                return;

            hubProxy.Invoke("getNodes", clientId);
        }

        public void GetLog(string clientId)
        {
            if (!IsGatewayServiceConnected())
                return;

            hubProxy.Invoke("getLog", clientId);
        }

        public void ClearLog(string clientId)
        {
            if (!IsGatewayServiceConnected())
                return;

            hubProxy.Invoke("clearLog", clientId);
        }

        public void GetGatewayInfo(string clientId)
        {
            if (!IsGatewayServiceConnected())
                return;

            hubProxy.Invoke("GetGatewayInfo", clientId);
        }

        #endregion
    }
}
