using System;
using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public interface ISoftNodeClient
    {
        void ConnectToServer(string url = "http://localhost:13122/");
        void Disconnect();
        bool IsConnected();
        void SendMessage(Message message);
        event OnReceivedMessageHandler OnReceivedMessageEvent;
        event Action OnConnected;
        event Action OnDisconnected;
    }
}
