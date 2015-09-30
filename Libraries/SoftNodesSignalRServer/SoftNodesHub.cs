using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace MyNetSensors.SoftNodesSignalRServer
{
    public class SoftNodesHub:Hub
    {
        public void DetermineLength(string message)
        {
            Console.WriteLine(message);

            string newMessage = string.Format(@"{0} has a length of: {1}", message, message.Length);
            Clients.All.ReceiveLength(newMessage);
        }
    }
}
