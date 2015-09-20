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
using MyNetSensors.SerialGateway;


namespace MyNetSensors.SerialController_Console
{

    public class SignalRController
    {
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugStateMessage;

        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private Gateway gateway;



        public bool IsConnected()
        {
            return hubConnection.State == ConnectionState.Connected;
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

        public bool Connect(Gateway gateway, string serverUrl)
        {
            DebugState(String.Format("Connecting to server {0}... ", serverUrl));

            hubConnection = new HubConnection(serverUrl);
            hubProxy = hubConnection.CreateHubProxy("gatewayHub");

            try
            {
                hubConnection.Start().Wait();
                hubProxy.On("clearLog", ClearLog);
                hubProxy.On("clearNodes", ClearNodes);
                hubProxy.On("getLog", GetLog);
                hubProxy.On("getNodes", GetNodes);
                hubProxy.On<string>("sendMessage", SendMessage);

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
            }
            catch { }

            bool result = IsConnected();

            if (result)
                DebugState("Connected.");
            else
                DebugState("Can`t connect.");

            return result;
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


        

        private void ClearLog()
        {
            DebugTxRx("Clear log.");

            gateway.messagesLog.ClearLog();
        }

        private void ClearNodes()
        {
            DebugTxRx("Clear nodes.");

            gateway.ClearNodesList();
        }

        private void GetLog()
        {
            DebugTxRx("Get log.");

            List<Message> log = gateway.messagesLog.GetAllMessages();

            hubProxy.Invoke("ReturnLog", log);
        }
        private void GetNodes()
        {
            DebugTxRx("Get nodes.");

            List<Node> nodes = gateway.GetNodes();

            hubProxy.Invoke("ReturnNodes", nodes);
        }

        private void SendMessage(string message)
        {
            DebugTxRx("Send message: "+ message);

            if (!gateway.IsConnected()) return;

            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
            gateway.UpdateSensorFromMessage(mess);
        }
    }
}
