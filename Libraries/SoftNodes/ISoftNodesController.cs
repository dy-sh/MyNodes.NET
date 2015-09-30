using MyNetSensors.Gateway;

namespace MyNetSensors.SoftNodes
{
    public interface ISoftNodesController
    {
        void SendMessage(Message message);
        void OnReceivedMessage(Message message);
    }
}
