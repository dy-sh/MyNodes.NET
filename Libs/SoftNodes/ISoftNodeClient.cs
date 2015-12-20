/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public delegate void DebugMessageEventHandler(string message);

    public interface ISoftNodeClient
    {
        void ConnectToServer(string url = "http://localhost:13122/");
        void Disconnect();
        bool IsConnected();
        void SendMessage(Message message);
        event OnReceivedMessageHandler OnReceivedMessage;
        event Action OnConnected;
        event Action OnDisconnected;
        event DebugMessageEventHandler OnConnectionFailed;
        event DebugMessageEventHandler OnSendingMessageFailed;
    }
}
