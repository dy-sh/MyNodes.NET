using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using MyNetSensors.Gateway;

namespace MyNetSensors.SerialController_Console
{
    public class GatewayServer : IGatewayServer
    {
        private IHubContext gatewayHub = GlobalHost.ConnectionManager.GetHubContext<GatewayHub>();
        private SerialGateway gateway;
        public static GatewayServer gatewayServer;
        
        public event Action OnConnected;
        public event Action OnDisconnected;
        public event DebugMessageEventHandler OnDebugTxRxMessage;
        public event DebugMessageEventHandler OnDebugStateMessage;

        public GatewayServer()
        {
            gatewayServer = this;
        }

        public void StartServer(SerialGateway gateway, string serverUrl, string connectionPassword )
        {
            try
            {
                WebApp.Start<Startup>(serverUrl);


                this.gateway = gateway;
                gateway.OnMessageRecievedEvent += OnMessageRecievedEvent;
                gateway.OnMessageSendEvent += OnMessageSendEvent;
                gateway.messagesLog.OnClearMessages += OnClearMessagesEvent;
                gateway.OnNewNodeEvent += OnNewNodeEvent;
                gateway.OnNodeLastSeenUpdatedEvent += OnNodeLastSeenUpdatedEvent;
                gateway.OnNodeUpdatedEvent += OnNodeUpdatedEvent;
                gateway.OnNodeBatteryUpdatedEvent += OnNodeBatteryUpdatedEvent;
                gateway.OnNewSensorEvent += OnNewSensorEvent;
                gateway.OnSensorUpdatedEvent += OnSensorUpdatedEvent;
                gateway.OnClearNodesListEvent += OnClearNodesListEvent;
                gateway.OnDisconnectedEvent += OnGatewayDisconnectedEvent;
                gateway.OnConnectedEvent += OnGatewayConnectedEvent;

                DebugState(string.Format("Server started at {0}", serverUrl));

                if (OnConnected != null)
                    OnConnected();

            }
            catch (Exception e)
            {
                DebugState(string.Format("Failed to start server: {0}", e.Message));
            }

        }



        public void DebugTxRx(string message)
        {
            if (OnDebugTxRxMessage != null)
                OnDebugTxRxMessage(message);
        }

        public void DebugState(string message)
        {
            if (OnDebugStateMessage != null)
                OnDebugStateMessage(message);
        }





        public void StopServer()
        {
            DebugState("Server closed");

            if (gateway != null)
            {
                gateway.OnMessageRecievedEvent -= OnMessageRecievedEvent;
                gateway.OnMessageSendEvent -= OnMessageSendEvent;
                gateway.messagesLog.OnClearMessages -= OnClearMessagesEvent;
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

            if (OnDisconnected != null)
                OnDisconnected();
        }


        #region messages from gateway to clients


        public void OnGatewayConnectedEvent()
        {
            gatewayHub.Clients.All.OnGatewayConnectedEvent();
        }

        public void OnGatewayDisconnectedEvent()
        {
            gatewayHub.Clients.All.OnGatewayDisconnectedEvent();
        }

        public void OnMessageRecievedEvent(Message message)
        {
            gatewayHub.Clients.All.OnMessageRecievedEvent(message);
        }

        public void OnMessageSendEvent(Message message)
        {
            gatewayHub.Clients.All.OnMessageSendEvent(message);
        }


        public void OnNodeUpdatedEvent(Node node)
        {
            gatewayHub.Clients.All.OnNodeUpdatedEvent(node);
        }

        public void OnNodeLastSeenUpdatedEvent(Node node)
        {
            gatewayHub.Clients.All.OnNodeLastSeenUpdatedEvent(node);
        }

        public void OnNewNodeEvent(Node node)
        {
            gatewayHub.Clients.All.OnNewNodeEvent(node);
        }

        public void OnNodeBatteryUpdatedEvent(Node node)
        {
            gatewayHub.Clients.All.OnNodeBatteryUpdatedEvent(node);
        }

        public void OnSensorUpdatedEvent(Sensor sensor)
        {
            gatewayHub.Clients.All.OnSensorUpdatedEvent(sensor);
        }

        public void OnNewSensorEvent(Sensor sensor)
        {
            gatewayHub.Clients.All.OnNewSensorEvent(sensor);
        }

        public void OnClearNodesListEvent()
        {
            gatewayHub.Clients.All.OnClearNodesListEvent();
        }

        public void OnClearMessagesEvent()
        {
            gatewayHub.Clients.All.OnClearMessagesEvent();
        }


        #endregion




        #region messages from clients to gateway

        public void UpdateNodeSettings(Node node)
        {
            DebugTxRx(String.Format("Update node settings"));

            gateway.UpdateNodeSettings(node);
        }



        public void ClearLog(string userId)
        {
            DebugTxRx(String.Format("Clear log ({0})", userId));

            gateway.messagesLog.ClearLog();
        }

        public void ClearNodes(string userId)
        {
            DebugTxRx(String.Format("Clear nodes ({0})", userId));

            gateway.ClearNodesList();
        }

        public List<Message> GetLog(string userId)
        {
            DebugTxRx(String.Format("Get log ({0})", userId));

            return gateway.messagesLog.GetAllMessages();
        }

        public List<Node> GetNodes(string userId)
        {
            DebugTxRx(String.Format("Get nodes ({0})", userId));

            return gateway.GetNodes();
        }
        
        public bool GetGatewayHardwareConnected(string userId)
        {
            DebugTxRx(String.Format("Get gateway hardware status ({0})", userId));

            if (gateway == null)
                return false;
            else return gateway.IsConnected();
        }

        public GatewayInfo GetGatewayInfo(string userId)
        {
            DebugTxRx(String.Format("Get gateway info ({0})", userId));

            return gateway.GetGatewayInfo();
        }

        public void SendMessage(string userId, string message)
        {
            DebugTxRx(String.Format("Send message: {0} ({1})", message, userId));

            if (!gateway.IsConnected()) return;

            Message mess = gateway.ParseMessageFromString(message);
            gateway.SendMessage(mess);
            gateway.UpdateSensorFromMessage(mess);
        }

        #endregion

        public void UpdateSensorsLinks(string userId)
        {
            Program.sensorsLinksEngine.GetLinksFromRepository();
        }

        public void UpdateSensorsTasks(string userId)
        {
            Program.sensorsTasksEngine.GetTasksFromRepository();
        }

        public void UpdateNodeSettings(string userId, Node node)
        {
            gateway.UpdateNodeSettings(node);
        }

        public void DeleteNode(string userId, int nodeId)
        {
            DebugTxRx(String.Format("Clear log ({0})", userId));

            gateway.DeleteNode(nodeId);
        }
    }
}
