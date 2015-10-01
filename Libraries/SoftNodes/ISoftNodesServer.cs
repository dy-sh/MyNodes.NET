/*  MyNetSensors 
    Copyright (C) 2015 Derwish <derwish.pro@gmail.com>
    License: http://www.gnu.org/licenses/gpl-3.0.txt  
*/

using System;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public delegate void OnReceivedMessageHandler(Message message);

    public interface ISoftNodesServer
    {
        void StartServer(string url = "http://localhost:13122/");
        void SendMessage(Message message);
        event OnReceivedMessageHandler OnReceivedMessage;
        event Action OnConnected;
        event Action OnDisconnected;

        event DebugMessageEventHandler OnDebugTxRxMessage;
        event DebugMessageEventHandler OnDebugStateMessage;
    }
}
