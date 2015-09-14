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
        public bool logGatewayMessages = true;

        public event OnLogMessageEventHandler OnLogMessageEvent;

        private HubConnection hubConnection;
        private IHubProxy hubProxy;
        private Gateway gateway;

        public SignalRController(Gateway gateway)
        {
            this.gateway = gateway;
            gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
            gateway.OnMessageSendEvent += OnMessageSendEvent;
            gateway.messagesLog.OnClearMessages += OnClearMessages;
        }



        public bool IsConnected()
        {
            return hubConnection.State == ConnectionState.Connected;
        }

        private void Log(string message)
        {
            if (OnLogMessageEvent != null)
                OnLogMessageEvent(message);
        }

        public bool Connect(string serverUrl)
        {
            Log(String.Format("Connecting to server {0}... ", serverUrl));

            hubConnection = new HubConnection(serverUrl);
            hubProxy = hubConnection.CreateHubProxy("gatewayHub");

            try
            {
                hubConnection.Start().Wait();
                hubProxy.On("clearLog", ClearLog);
                hubProxy.On("getLog", GetLog);
                hubProxy.On("getNodes", GetNodes);
            }
            catch { }

            bool result = IsConnected();

            if (result)
                Log("OK\n");
            else
                Log("FAILED\n");

            return result;
        }



        private void OnMessageRecievedEvent(Message message)
        {
            if (!IsConnected()) return;

            if (logGatewayMessages)
                Log(String.Format("RX: {0}\n", message.ToString()));

            hubProxy.Invoke("OnMessageRecievedEvent", message);
        }

        private void OnMessageSendEvent(Message message)
        {
            if (!IsConnected()) return;

            if (logGatewayMessages)
                Log(String.Format("TX: {0}\n", message.ToString()));

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

    }
}
