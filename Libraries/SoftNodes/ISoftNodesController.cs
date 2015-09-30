using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public interface ISoftNodesController
    {
        void StartServer(string url = "http://localhost:13122/");
        void SendMessage(Message message);
        void OnReceivedMessage(Message message);
    }
}
