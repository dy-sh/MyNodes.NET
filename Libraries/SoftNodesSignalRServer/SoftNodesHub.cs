using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using MyNetSensors.Gateway;


namespace MyNetSensors.SoftNodesSignalRServer
{
    public class SoftNodesHub:Hub
    {
        public void ReceiveMessage(Message message)
        {
            SoftNodesController.softNodesController.OnReceivedMessage(message);
        }
    }
}
