using System;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public delegate void OnReceivedMessageHandler(Message message);

    public interface ISoftNodesServer
    {
        void StartServer(string url = "http://localhost:13122/");
        void SendMessage(Message message);
        event OnReceivedMessageHandler OnReceivedMessageEvent;
        event Action OnConnected;
        event Action OnDisconnected;
    }
}
