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
    public delegate void OnLogMessageEventHandler(string message);

    public class SignalRController
    {
        public event OnLogMessageEventHandler OnLogMessageEvent;

        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private Gateway gateway;



        public bool IsConnected()
        {
            return hubConnection.State == ConnectionState.Connected;
        }

        private void Log(string message)
        {
            if (OnLogMessageEvent != null)
                OnLogMessageEvent(message);
        }

        public bool Connect(Gateway gateway, string serverUrl)
        {
            Log(String.Format("Connecting to server {0}... ", serverUrl));

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
                gateway.OnClearNodesList += OnClearNodesList;
            }
            catch { }

            bool result = IsConnected();

            if (result)
                Log("OK\n");
            else
                Log("FAILED\n");

            return result;
        }



        private void SendMessage(string message)
        {
            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
            gateway.UpdateSensorFromMessage(mess);
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

        private void ClearLog()
        {
            Log("Clear log... ");
            try
            {
                gateway.messagesLog.ClearLog();
                Log("OK\n");
            }
            catch
            {
                Log("FAILED\n");
            }
        }

        private void ClearNodes()
        {
            Log("Clear nodes... ");
            try
            {
                gateway.ClearNodesList();
                Log("OK\n");
            }
            catch
            {
                Log("FAILED\n");
            }
        }

        private void GetLog()
        {
            List<Message> log = null;

            Log("Get log... ");
            try
            {
                log = gateway.messagesLog.GetAllMessages();
                Log("OK\n");
            }
            catch
            {
                Log("FAILED\n");
            }

            hubProxy.Invoke("ReturnLog", log);
        }


        private void OnClearMessages(object sender, EventArgs e)
        {
            hubProxy.Invoke("OnClearMessages");
        }

        private void GetNodes()
        {
            List<Node> nodes = null;

            Log("Get nodes... ");
            try
            {
                nodes = gateway.GetNodes();
                Log("OK\n");
            }
            catch
            {
                Log("FAILED\n");
            }

            hubProxy.Invoke("ReturnNodes", nodes);
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

        private void OnClearNodesList(object sender, EventArgs e)
        {
            if (!IsConnected()) return;

            hubProxy.Invoke("OnClearNodesList");
        }
    }
}
