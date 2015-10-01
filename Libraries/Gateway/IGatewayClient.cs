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
    public delegate void StringHandler(string s);
    public delegate void NodeHandler(Node n);
    public delegate void SensorHandler(Sensor s);
    public delegate void StringBool(string s, bool b);
    public delegate void StringString(string s1, string s2);
    public delegate void StringListMessage(string s, List<Message> l);
    public delegate void StringListNode(string s, List<Node> l);
    public delegate void StringGatewayInfo(string s, GatewayInfo i);

    public interface IGatewayClient
    {
        void ConnectToServer(string url = "http://localhost:13121/");
        void Disconnect();
        bool IsConnected();
        bool IsGatewayServiceConnected();
        event Action OnConnected;
        event Action OnDisconnected;
        event DebugMessageEventHandler OnConnectionFailed;
        event DebugMessageEventHandler OnSendingMessageFailed;

        #region Messages from gateway

        event StringHandler OnMessageRecieved;
        event StringHandler OnMessageSend;
        event Action OnClearMessages;
        event NodeHandler OnNodeUpdated;
        event NodeHandler OnNodeLastSeenUpdated;
        event NodeHandler OnNewNode;
        event NodeHandler OnNodeBatteryUpdated;
        event SensorHandler OnSensorUpdated;
        event SensorHandler OnNewSensor;
        event Action OnClearNodesList;
        event Action OnGatewayConnected;
        event Action OnGatewayDisconnected;
        event StringBool ReturnGatewayHardwareConnected;
        event StringString ReturnLog;
        event StringListNode ReturnNodes;
        event StringGatewayInfo ReturnGatewayInfo;

        #endregion
        

        #region Messages from clients to gateway

        void SendMessage(string clientId, string message);
        void GetGatewayHardwareConnected(string clientId);
        void GetNodes(string clientId);
        void GetLog(string clientId);
        void GetGatewayInfo(string clientId);
        void ClearLog(string clientId);

        #endregion
    }
}
