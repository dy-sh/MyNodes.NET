/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNetSensors.Gateway
{
    public delegate void OnReceivedMessageHandler(Message message);
    public interface IGatewayServer
    {
        void StartServer(SerialGateway gateway, string serverUrl = "http://localhost:13121/", string connectionPassword = "");
        void StopServer();

        event Action OnConnected;
        event Action OnDisconnected;
        event DebugMessageEventHandler OnDebugTxRxMessage;
        event DebugMessageEventHandler OnDebugStateMessage;

        void DebugTxRx(string message);
        void DebugState(string message);

        void UpdateNodeSettings(Node node);
        void OnGatewayConnectedEvent();
        void OnGatewayDisconnectedEvent();
        void OnMessageRecievedEvent(Message message);
        void OnMessageSendEvent(Message message);
        void OnNodeUpdatedEvent(Node node);
        void OnNodeLastSeenUpdatedEvent(Node node);
        void OnNewNodeEvent(Node node);
        void OnNodeBatteryUpdatedEvent(Node node);
        void OnSensorUpdatedEvent(Sensor sensor);
        void OnNewSensorEvent(Sensor sensor);
        void OnClearNodesListEvent();
        void OnClearMessagesEvent();


        void ClearLog(string userId);
        void ClearNodes(string userId);
        List<Message> GetLog(string userId);
        List<Node> GetNodes(string userId);
        bool GetGatewayHardwareConnected(string userId);
        GatewayInfo GetGatewayInfo(string userId);
        void SendMessage(string userId, string message);
    }

}
