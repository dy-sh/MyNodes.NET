/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using MyNetSensors.Gateway;
using MyNetSensors.NodeTasks;


namespace MyNetSensors.SerialController_Console
{

    public class GatewaySignalRController
    {
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugStateMessage;
        public event EventHandler OnConnected;
        public event EventHandler OnDisconnected;
        public event EventHandler OnConnectionFailed;
        public event EventHandler OnAuthorizationCompleted;
        public event EventHandler OnAuthorizationFailed;
        private bool isAuthorized;

        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private SerialGateway gateway;
        private SensorsTasksEngine tasksEngine;


        public bool IsConnected()
        {
            return hubConnection != null && hubConnection.State == ConnectionState.Connected;
        }

        public bool IsAuthorized()
        {
            return isAuthorized;
        }

        private void DebugTxRx(string message)
        {
            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message);
        }

        private void DebugState(string message)
        {
            if (OnDebugStateMessage != null)
                OnDebugStateMessage(message);
        }

        public bool Connect(SerialGateway gateway, SensorsTasksEngine tasksEngine, string serverUrl, string connectionPassword)
        {
            DebugState(String.Format("Connecting to server {0}... ", serverUrl));

            this.tasksEngine = tasksEngine;
            isAuthorized = false;

            var querystringData = new Dictionary<string, string>();
            querystringData.Add("IsGateway", "true");
            querystringData.Add("ConnectionPassword", connectionPassword);

            hubConnection = new HubConnection(serverUrl, querystringData);
            hubProxy = hubConnection.CreateHubProxy("gatewayHub");

            try
            {
                hubProxy.On<string>("clearLog", ClearLog);
                hubProxy.On("clearNodes", ClearNodes);
                hubProxy.On<string>("getLog", GetLog);
                hubProxy.On<string>("getNodes", GetNodes);
                hubProxy.On<string>("getGatewayInfo", GetGatewayInfo);
                hubProxy.On<string>("getGatewayHardwareConnected", GetGatewayHardwareConnected);
                hubProxy.On<Node>("updateNodeSettings", UpdateNodeSettings);
                hubProxy.On("authorizationFailed", AuthorizationFailed);
                hubProxy.On("authorizationCompleted", AuthorizationCompleted);
                hubProxy.On("updateSensorsTasks", UpdateSensorsTasks);
                hubProxy.On<string, string>("sendMessage", SendMessage);

                hubConnection.Start().Wait();
                hubConnection.Closed += Disconnect;

                this.gateway = gateway;
                gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
                gateway.OnMessageSendEvent += OnMessageSendEvent;
                gateway.messagesLog.OnClearMessages += OnClearMessages;
                gateway.OnNewNodeEvent += OnNewNodeEvent;
                gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
                gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
                gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
                gateway.OnNewSensorEvent += OnNewSensorEvent;
                gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
                gateway.OnClearNodesListEvent += OnClearNodesListEvent;
                gateway.OnDisconnectedEvent += OnGatewayDisconnectedEvent;
                gateway.OnConnectedEvent += OnGatewayConnectedEvent;


                if (OnConnected != null && IsConnected())
                    OnConnected(this, null);

                // DebugState("Connected.");

                return true;
            }
            catch
            {
                DebugState("Can`t connect.");
                if (OnConnectionFailed != null)
                    OnConnectionFailed(this, null);
                return false;
            }

        }

        private void UpdateSensorsTasks()
        {
            tasksEngine.GetTasksFromRepository();
        }

        private void UpdateNodeSettings(Node node)
        {
            gateway.UpdateNodeSettings(node);
        }


        public void Disconnect()
        {
            DebugState("Disconnected.");

            if (gateway != null)
            {
                gateway.OnMessageRecievedEvent -= OnMessageRecievedEvent;
                gateway.OnMessageSendEvent -= OnMessageSendEvent;
                gateway.messagesLog.OnClearMessages -= OnClearMessages;
                gateway.OnNewNodeEvent -= OnNewNodeEvent;
                gateway.OnNodeLastSeenUpdatedEvent -= OnNodeLastSeenUpdatedEvent;
                gateway.OnNodeUpdatedEvent -= OnNodeUpdatedEvent;
                gateway.OnNodeBatteryUpdatedEvent -= OnNodeBatteryUpdatedEvent;
                gateway.OnNewSensorEvent -= OnNewSensorEvent;
                gateway.OnSensorUpdatedEvent -= OnSensorUpdatedEvent;
                gateway.OnClearNodesListEvent -= OnClearNodesListEvent;
                gateway.OnDisconnectedEvent -= OnGatewayDisconnectedEvent;
                gateway.OnConnectedEvent -= OnGatewayConnectedEvent;
                gateway = null;
            }

            isAuthorized = false;

            if (hubConnection != null && hubConnection.State == ConnectionState.Connected)
                hubConnection.Stop();

            if (OnDisconnected != null)
                OnDisconnected(this, null);
        }


        private void AuthorizationFailed()
        {
            DebugState("Authorization failed.");
            isAuthorized = false;
            if (OnAuthorizationFailed != null)
                OnAuthorizationFailed(this, null);
        }

        private void AuthorizationCompleted()
        {
            DebugState("Connected. Authorization completed.");
            isAuthorized = true;
            if (OnAuthorizationCompleted != null)
                OnAuthorizationCompleted(this, null);
        }


        private void OnGatewayConnectedEvent(object sender, EventArgs e)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnGatewayConnectedEvent");
        }

        private void OnGatewayDisconnectedEvent(object sender, EventArgs e)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnGatewayDisconnectedEvent");
        }




        private void OnMessageRecievedEvent(Message message)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnMessageRecievedEvent", message);
        }

        private void OnMessageSendEvent(Message message)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnMessageSendEvent", message);
        }


        private void OnNodeUpdatedEvent(Node node)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnNodeUpdatedEvent", node);
        }

        private void OnNodeLastSeenUpdatedEvent(Node node)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnNodeLastSeenUpdatedEvent", node);
        }

        private void OnNewNodeEvent(Node node)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnNewNodeEvent", node);
        }

        private void OnNodeBatteryUpdatedEvent(Node node)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnNodeBatteryUpdatedEvent", node);
        }

        private void OnSensorUpdatedEvent(Sensor sensor)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnSensorUpdatedEvent", sensor);
        }

        private void OnNewSensorEvent(Sensor sensor)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnNewSensorEvent", sensor);
        }

        private void OnClearNodesListEvent(object sender, EventArgs e)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnClearNodesListEvent");
        }

        private void OnClearMessages(object sender, EventArgs e)
        {
            hubProxy.Invoke("OnClearMessages");
        }




        private void ClearLog(string userId)
        {
            DebugTxRx(String.Format("Clear log ({0})", userId));

            gateway.messagesLog.ClearLog();
        }

        private void ClearNodes()
        {
            DebugTxRx("Clear nodes.");

            gateway.ClearNodesList();
        }

        private void GetLog(string userId)
        {
            DebugTxRx(String.Format("Get log ({0})", userId));

            List<Message> log = gateway.messagesLog.GetAllMessages();

            hubProxy.Invoke("ReturnLog", log, userId);
        }
        private void GetNodes(string userId)
        {
            DebugTxRx(String.Format("Get nodes ({0})", userId));

            List<Node> nodes = gateway.GetNodes();

            hubProxy.Invoke("ReturnNodes", nodes, userId);
        }

        private void SendMessage(string message, string userId)
        {
            DebugTxRx(String.Format("Send message: {0} ({1})", message, userId));

            if (!gateway.IsConnected()) return;

            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
            gateway.UpdateSensorFromMessage(mess);
        }

        private void GetGatewayHardwareConnected(string userId)
        {
            DebugTxRx(String.Format("Get gateway status ({0})", userId));
            if (gateway == null)
            {
                hubProxy.Invoke("returnGatewayHardwareConnected", userId, false);
                return;
            }
            hubProxy.Invoke("returnGatewayHardwareConnected", userId, gateway.IsConnected());
        }

        private void GetGatewayInfo(string userId)
        {
            DebugTxRx(String.Format("Get gateway info ({0})", userId));

            GatewayInfo info = gateway.GetGatewayInfo();

            hubProxy.Invoke("returnGatewayInfo",  info, userId);
        }
    }
}
