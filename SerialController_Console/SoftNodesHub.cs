using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;

namespace MyNetSensors.SerialController_Console
{
    public class SoftNodesHub:Hub
    {
        public void ReceiveMessage(Message message)
        {
            SoftNodesServer.softNodesServer.ReceiveMessage(message);
        }
    }
}
